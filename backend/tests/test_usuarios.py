"""
Testes — Usuários e Chaves (FrmUsuarios)

Cobertura obrigatória (CLAUDE.md):
  GET /usuarios:
    1. Listagem como Administrador → todos os usuários
    2. Listagem como perfil comum → apenas o próprio registro
  POST /usuarios:
    3. Criação de usuário por Administrador → 201
    4. Criação por perfil comum → 403
  PUT /usuarios/{id}:
    5. Atualização de senha → hash bcrypt verificado
    6. Tentativa de alterar Perfil por perfil comum → 403
  GET /chaves:
    7. Listagem por Administrador → 200
    8. Listagem por perfil comum → 403
  PUT /chaves/{id}:
    9. Atualização de chave por Administrador → 200
   10. Atualização por perfil comum → 403
  POST /auth/verificar-senha:
   11. Senha correta → {ok: true}
   12. Senha incorreta → {ok: false}
  Autenticação negada:
   13. Sem token → 401
"""

from unittest.mock import MagicMock

from fastapi.testclient import TestClient
from jose import jwt
from passlib.context import CryptContext

from app.dependencies import get_db
from app.main import app

# ---------------------------------------------------------------------------
# Configuração
# ---------------------------------------------------------------------------

SECRET_KEY = "troque-esta-chave-em-producao"
ALGORITHM = "HS256"
pwd_context = CryptContext(schemes=["bcrypt"], deprecated="auto")


def make_token(usuario: str = "Admin", perfil: str = "Administrador") -> str:
    """Gera JWT válido para testes."""
    return jwt.encode({"sub": usuario, "perfil": perfil}, SECRET_KEY, algorithm=ALGORITHM)


def auth_header(token: str | None = None) -> dict:
    t = token or make_token()
    return {"Authorization": f"Bearer {t}"}


def admin_header() -> dict:
    return auth_header(make_token("Admin", "Administrador"))


def comum_header(usuario: str = "Joao") -> dict:
    return auth_header(make_token(usuario, "Comum"))


# ---------------------------------------------------------------------------
# Fake rows
# ---------------------------------------------------------------------------


class FakeRow:
    def __init__(self, **kwargs):
        for k, v in kwargs.items():
            setattr(self, k, v)


ADMIN_ROW = FakeRow(Id=1, Usuário="Admin", Psw="hash_ignorado", Perfil="Administrador")
JOAO_ROW = FakeRow(Id=2, Usuário="Joao", Psw="hash_ignorado", Perfil="Comum")
CHAVE_ROW = FakeRow(Id=1, Ref="Alteração de senhas", Chave="segredo")

USUARIOS_ROWS = [ADMIN_ROW, JOAO_ROW]


# ---------------------------------------------------------------------------
# Helpers de mock
# ---------------------------------------------------------------------------


def make_db(fetchall=None, fetchone=None):
    """Mock de Session SQLAlchemy."""
    db = MagicMock()
    result = MagicMock()
    result.fetchall.return_value = fetchall or []
    result.fetchone.return_value = fetchone
    db.execute.return_value = result
    return db


def override_db(db):
    def _override():
        yield db

    return _override


# ---------------------------------------------------------------------------
# Testes — GET /usuarios
# ---------------------------------------------------------------------------


class TestListarUsuarios:
    def test_admin_ve_todos_os_usuarios(self):
        """
        RN69 — Administrador deve receber todos os registros da tabela.
        D12 — Psw não deve aparecer no response.
        """
        db = make_db(fetchall=USUARIOS_ROWS)
        app.dependency_overrides[get_db] = override_db(db)
        client = TestClient(app)

        resp = client.get("/usuarios", headers=admin_header())

        assert resp.status_code == 200
        data = resp.json()
        assert len(data) == 2
        # D12 — Psw nunca retornada
        for u in data:
            assert "Psw" not in u
            assert "psw" not in u
            assert "senha" not in u

        app.dependency_overrides.clear()

    def test_perfil_comum_ve_apenas_proprio_registro(self):
        """
        RN69 — perfil comum deve ver apenas o próprio usuário.
        O service filtra WHERE Usuário = :usuario_logado.
        """
        db = make_db(fetchall=[JOAO_ROW])
        app.dependency_overrides[get_db] = override_db(db)
        client = TestClient(app)

        resp = client.get("/usuarios", headers=comum_header("Joao"))

        assert resp.status_code == 200
        data = resp.json()
        assert len(data) == 1
        assert data[0]["usuario"] == "Joao"

        app.dependency_overrides.clear()

    def test_autenticacao_negada_sem_token(self):
        """Sem token → 401."""
        client = TestClient(app)
        resp = client.get("/usuarios")
        assert resp.status_code == 401


# ---------------------------------------------------------------------------
# Testes — POST /usuarios
# ---------------------------------------------------------------------------


class TestCriarUsuario:
    def test_admin_cria_usuario(self):
        """
        RN70 — Administrador pode criar usuários.
        RN73 — senha deve ser armazenada como bcrypt (verificado via INSERT mock).
        D12 — Psw não aparece no response.
        """
        db = MagicMock()
        # fetchone para verificar unicidade → None (não existe)
        # fetchone para buscar recém-criado → novo row
        novo_row = FakeRow(Id=3, Usuário="NovoUser", Psw="bcrypt_hash", Perfil="Comum")
        db.execute.return_value.fetchone.side_effect = [None, novo_row]

        app.dependency_overrides[get_db] = override_db(db)
        client = TestClient(app)

        resp = client.post(
            "/usuarios",
            json={"usuario": "NovoUser", "senha": "minhasenha", "perfil": "Comum"},
            headers=admin_header(),
        )

        assert resp.status_code == 201
        data = resp.json()
        assert data["usuario"] == "NovoUser"
        assert "psw" not in data
        assert "senha" not in data

        # RN73 — verifica que o INSERT recebeu um hash bcrypt (não texto plano)
        insert_call = None
        for call in db.execute.call_args_list:
            sql = str(call[0][0])
            if "INSERT" in sql.upper():
                insert_call = call
                break
        assert insert_call is not None
        params = insert_call[0][1]
        assert params["psw"].startswith("$2b$"), "Senha deve ser bcrypt"

        app.dependency_overrides.clear()

    def test_perfil_comum_nao_pode_criar_usuario(self):
        """RN70 — perfil comum deve receber 403."""
        db = make_db()
        app.dependency_overrides[get_db] = override_db(db)
        client = TestClient(app)

        resp = client.post(
            "/usuarios",
            json={"usuario": "Outro", "senha": "senha", "perfil": "Comum"},
            headers=comum_header(),
        )

        assert resp.status_code == 403

        app.dependency_overrides.clear()

    def test_autenticacao_negada_sem_token(self):
        client = TestClient(app)
        resp = client.post("/usuarios", json={"usuario": "x", "senha": "y", "perfil": "z"})
        assert resp.status_code == 401


# ---------------------------------------------------------------------------
# Testes — PUT /usuarios/{id}
# ---------------------------------------------------------------------------


class TestAtualizarUsuario:
    def test_atualizacao_de_senha_armazena_bcrypt(self):
        """
        RN73 — ao atualizar senha, deve ser armazenada como bcrypt.
        O UPDATE deve receber hash, não texto plano.
        """
        db = MagicMock()
        row_atual = FakeRow(Id=2, Usuário="Joao", Psw="senha_plana", Perfil="Comum")
        row_atualizado = FakeRow(Id=2, Usuário="Joao", Psw="$2b$...", Perfil="Comum")
        db.execute.return_value.fetchone.side_effect = [row_atual, row_atualizado]

        app.dependency_overrides[get_db] = override_db(db)
        client = TestClient(app)

        resp = client.put(
            "/usuarios/2",
            json={"senha": "nova_senha"},
            headers=comum_header("Joao"),
        )

        assert resp.status_code == 200
        data = resp.json()
        assert "psw" not in data
        assert "senha" not in data

        # Verifica bcrypt no UPDATE
        update_call = None
        for call in db.execute.call_args_list:
            sql = str(call[0][0])
            if "UPDATE" in sql.upper():
                update_call = call
                break
        assert update_call is not None
        params = update_call[0][1]
        assert params.get("Psw", "").startswith("$2b$"), "Senha deve ser bcrypt no UPDATE"

        app.dependency_overrides.clear()

    def test_perfil_comum_nao_pode_alterar_perfil(self):
        """RN71 — perfil comum tentando alterar campo Perfil → 403."""
        db = MagicMock()
        row_atual = FakeRow(Id=2, Usuário="Joao", Psw="hash", Perfil="Comum")
        db.execute.return_value.fetchone.return_value = row_atual

        app.dependency_overrides[get_db] = override_db(db)
        client = TestClient(app)

        resp = client.put(
            "/usuarios/2",
            json={"perfil": "Administrador"},
            headers=comum_header("Joao"),
        )

        assert resp.status_code == 403

        app.dependency_overrides.clear()

    def test_perfil_comum_nao_pode_alterar_outro_usuario(self):
        """RN69 — perfil comum não pode alterar registro de outro usuário → 403."""
        db = MagicMock()
        row_outro = FakeRow(Id=1, Usuário="Admin", Psw="hash", Perfil="Administrador")
        db.execute.return_value.fetchone.return_value = row_outro

        app.dependency_overrides[get_db] = override_db(db)
        client = TestClient(app)

        resp = client.put(
            "/usuarios/1",
            json={"usuario": "HackTentativa"},
            headers=comum_header("Joao"),
        )

        assert resp.status_code == 403

        app.dependency_overrides.clear()

    def test_autenticacao_negada_sem_token(self):
        client = TestClient(app)
        resp = client.put("/usuarios/1", json={"usuario": "x"})
        assert resp.status_code == 401


# ---------------------------------------------------------------------------
# Testes — GET /chaves
# ---------------------------------------------------------------------------


class TestListarChaves:
    def test_admin_lista_chaves(self):
        """RN72 — Administrador pode listar as chaves."""
        db = make_db(fetchall=[CHAVE_ROW])
        app.dependency_overrides[get_db] = override_db(db)
        client = TestClient(app)

        resp = client.get("/chaves", headers=admin_header())

        assert resp.status_code == 200
        data = resp.json()
        assert len(data) == 1
        assert data[0]["ref"] == "Alteração de senhas"

        app.dependency_overrides.clear()

    def test_perfil_comum_nao_pode_listar_chaves(self):
        """RN72 — perfil comum recebe 403."""
        db = make_db(fetchall=[CHAVE_ROW])
        app.dependency_overrides[get_db] = override_db(db)
        client = TestClient(app)

        resp = client.get("/chaves", headers=comum_header())

        assert resp.status_code == 403

        app.dependency_overrides.clear()

    def test_autenticacao_negada_sem_token(self):
        client = TestClient(app)
        resp = client.get("/chaves")
        assert resp.status_code == 401


# ---------------------------------------------------------------------------
# Testes — PUT /chaves/{id}
# ---------------------------------------------------------------------------


class TestAtualizarChave:
    def test_admin_atualiza_chave(self):
        """RN72 + RN75 — Administrador pode alterar Chave; Ref permanece intacta."""
        db = MagicMock()
        row_atual = FakeRow(Id=1, Ref="Alteração de senhas", Chave="velha")
        row_atualizado = FakeRow(Id=1, Ref="Alteração de senhas", Chave="nova")
        db.execute.return_value.fetchone.side_effect = [row_atual, row_atualizado]

        app.dependency_overrides[get_db] = override_db(db)
        client = TestClient(app)

        resp = client.put("/chaves/1", json={"chave": "nova"}, headers=admin_header())

        assert resp.status_code == 200
        data = resp.json()
        assert data["chave"] == "nova"
        assert data["ref"] == "Alteração de senhas"

        # RN75 — UPDATE só deve alterar Chave, não Ref
        update_call = None
        for call in db.execute.call_args_list:
            sql = str(call[0][0])
            if "UPDATE" in sql.upper():
                update_call = call
                break
        assert update_call is not None
        sql_text = str(update_call[0][0])
        assert "Ref" not in sql_text, "UPDATE não deve incluir Ref"

        app.dependency_overrides.clear()

    def test_perfil_comum_nao_pode_alterar_chave(self):
        """RN72 — perfil comum → 403."""
        db = make_db(fetchone=CHAVE_ROW)
        app.dependency_overrides[get_db] = override_db(db)
        client = TestClient(app)

        resp = client.put("/chaves/1", json={"chave": "nova"}, headers=comum_header())

        assert resp.status_code == 403

        app.dependency_overrides.clear()

    def test_autenticacao_negada_sem_token(self):
        client = TestClient(app)
        resp = client.put("/chaves/1", json={"chave": "x"})
        assert resp.status_code == 401


# ---------------------------------------------------------------------------
# Testes — POST /auth/verificar-senha
# ---------------------------------------------------------------------------


class TestVerificarSenha:
    def test_senha_correta_retorna_ok_true(self):
        """RN68 — senha correta deve retornar {ok: true}."""
        db = make_db(fetchone=FakeRow(Chave="segredo"))
        app.dependency_overrides[get_db] = override_db(db)
        client = TestClient(app)

        resp = client.post(
            "/auth/verificar-senha",
            json={"ref": "Alteração de senhas", "chave": "segredo"},
            headers=admin_header(),
        )

        assert resp.status_code == 200
        assert resp.json()["ok"] is True

        app.dependency_overrides.clear()

    def test_senha_incorreta_retorna_ok_false(self):
        """RN68 — senha incorreta deve retornar {ok: false}; nunca 401/403."""
        db = make_db(fetchone=FakeRow(Chave="segredo"))
        app.dependency_overrides[get_db] = override_db(db)
        client = TestClient(app)

        resp = client.post(
            "/auth/verificar-senha",
            json={"ref": "Alteração de senhas", "chave": "errada"},
            headers=admin_header(),
        )

        assert resp.status_code == 200
        assert resp.json()["ok"] is False

        app.dependency_overrides.clear()

    def test_ref_inexistente_retorna_ok_false(self):
        """Ref não encontrada na tabela → {ok: false}."""
        db = make_db(fetchone=None)
        app.dependency_overrides[get_db] = override_db(db)
        client = TestClient(app)

        resp = client.post(
            "/auth/verificar-senha",
            json={"ref": "Operação Inexistente", "chave": "qualquer"},
            headers=admin_header(),
        )

        assert resp.status_code == 200
        assert resp.json()["ok"] is False

        app.dependency_overrides.clear()

    def test_autenticacao_negada_sem_token(self):
        """POST /auth/verificar-senha exige JWT."""
        client = TestClient(app)
        resp = client.post(
            "/auth/verificar-senha",
            json={"ref": "Alteração de senhas", "chave": "x"},
        )
        assert resp.status_code == 401


# ---------------------------------------------------------------------------
# Testes — service direto
# ---------------------------------------------------------------------------


class TestServiceUsuarios:
    def test_criar_usuario_hash_bcrypt(self):
        """RN73 — service deve armazenar hash bcrypt, nunca texto plano."""
        from app.services.usuarios import criar_usuario

        db = MagicMock()
        novo = FakeRow(Id=3, Usuário="Novo", Psw="$2b$hash", Perfil="Comum")
        db.execute.return_value.fetchone.side_effect = [None, novo]

        criar_usuario(db, "Novo", "minhasenha", "Comum", "Administrador")

        insert_call = None
        for call in db.execute.call_args_list:
            sql = str(call[0][0])
            if "INSERT" in sql.upper():
                insert_call = call
                break
        assert insert_call is not None
        psw_salvo = insert_call[0][1]["psw"]
        assert psw_salvo.startswith("$2b$"), f"Esperado bcrypt, recebido: {psw_salvo}"
        assert pwd_context.verify("minhasenha", psw_salvo)

    def test_verificar_senha_texto_plano_legado(self):
        """
        verificar_senha_operacao deve suportar senhas em texto plano (legado).
        Coexistência com bcrypt — não quebra o sistema durante a migração.
        """
        from app.services.usuarios import verificar_senha_operacao

        db = make_db(fetchone=FakeRow(Chave="legado_plano"))
        assert verificar_senha_operacao(db, "qualquer_ref", "legado_plano") is True

    def test_verificar_senha_bcrypt(self):
        """verificar_senha_operacao deve suportar senhas já migradas para bcrypt."""
        from app.services.usuarios import verificar_senha_operacao

        hash_bcrypt = pwd_context.hash("seguro")
        db = make_db(fetchone=FakeRow(Chave=hash_bcrypt))
        assert verificar_senha_operacao(db, "qualquer_ref", "seguro") is True
        assert verificar_senha_operacao(db, "qualquer_ref", "errada") is False
