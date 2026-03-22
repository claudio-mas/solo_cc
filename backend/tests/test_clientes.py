"""
Testes — Clientes (FrmPrincipal + frmClienteNovo)

Cobertura obrigatória (CLAUDE.md):
  GET /clientes:
    1. Caminho feliz (200 com lista ordenada)
    2. Autenticação negada (401 sem token)
    3. Lista vazia (200 com lista vazia, não 404)

  GET /clientes/proximo-codigo:
    4. Caminho feliz (200 com próximo código e Id)
    5. Autenticação negada (401 sem token)

  POST /clientes:
    6. Caminho feliz (201 com cliente criado)
    7. Código duplicado (409 "Código já existente")
    8. Código > 20.000 (422)
    9. Autenticação negada (401 sem token)
    10. Campos obrigatórios (422 — código e cliente)
"""

from unittest.mock import MagicMock

from fastapi.testclient import TestClient
from jose import jwt

from app.dependencies import get_db
from app.main import app

# ---------------------------------------------------------------------------
# Fixtures
# ---------------------------------------------------------------------------

SECRET_KEY = "troque-esta-chave-em-producao"
ALGORITHM = "HS256"


def make_token(
    usuario: str = "Teste",
    perfil: str = "Admin",
) -> str:
    """Gera um JWT válido para testes."""
    return jwt.encode(
        {"sub": usuario, "perfil": perfil},
        SECRET_KEY,
        algorithm=ALGORITHM,
    )


def auth_header(token: str | None = None) -> dict:
    """Retorna header Authorization com Bearer token."""
    t = token or make_token()
    return {"Authorization": f"Bearer {t}"}


# ---------------------------------------------------------------------------
# Dados de teste
# ---------------------------------------------------------------------------


class FakeRow:
    """Simula uma row SQLAlchemy com atributos nomeados."""

    def __init__(self, **kwargs):
        for k, v in kwargs.items():
            setattr(self, k, v)


SAMPLE_CLIENTES = [
    FakeRow(Id=1, Código=101, Cliente="Ana Silva"),
    FakeRow(Id=2, Código=202, Cliente="Bruno Costa"),
    FakeRow(Id=3, Código=303, Cliente="Carlos Lima"),
]


# ---------------------------------------------------------------------------
# Helpers de mock
# ---------------------------------------------------------------------------


def mock_db_with_clientes(rows: list):
    """Cria um mock de sessão DB que retorna as rows fornecidas."""
    db = MagicMock()
    result = MagicMock()
    result.fetchall.return_value = rows
    db.execute.return_value = result
    return db


def override_get_db(rows: list):
    """Override da dependência get_db com dados mockados."""

    def _override():
        yield mock_db_with_clientes(rows)

    return _override


def mock_db_proximo_codigo(max_id: int, codigos: list[int]):
    """Mock para GET /clientes/proximo-codigo — duas queries sequenciais."""
    db = MagicMock()

    # Primeira chamada: MAX(Id)
    result_max = MagicMock()
    result_max.fetchone.return_value = FakeRow(max_id=max_id)

    # Segunda chamada: SELECT Código >= 10000
    result_codigos = MagicMock()
    result_codigos.fetchall.return_value = [FakeRow(Código=c) for c in codigos]

    db.execute.side_effect = [result_max, result_codigos]
    return db


def mock_db_criar_cliente(existing: bool = False, inserted_row=None):
    """Mock para POST /clientes — query unicidade + INSERT."""
    db = MagicMock()

    # Primeira chamada: SELECT unicidade
    result_check = MagicMock()
    if existing:
        result_check.fetchone.return_value = FakeRow(Id=99)
    else:
        result_check.fetchone.return_value = None

    # Segunda chamada: INSERT
    result_insert = MagicMock()
    if inserted_row:
        result_insert.fetchone.return_value = inserted_row

    db.execute.side_effect = [result_check, result_insert]
    return db


# ---------------------------------------------------------------------------
# Testes — GET /clientes
# ---------------------------------------------------------------------------


class TestListarClientes:
    """GET /clientes — testes do endpoint de listagem."""

    def test_caminho_feliz_retorna_lista_ordenada(self):
        """
        Caminho feliz: JWT válido → 200 com lista de clientes.
        RN09 — lista retornada ordenada por nome (garantido pela query).
        """
        app.dependency_overrides[get_db] = override_get_db(SAMPLE_CLIENTES)
        client = TestClient(app)

        response = client.get("/clientes/", headers=auth_header())

        assert response.status_code == 200
        data = response.json()
        assert "clientes" in data
        assert len(data["clientes"]) == 3
        assert data["clientes"][0]["codigo"] == 101
        assert data["clientes"][0]["cliente"] == "Ana Silva"
        assert data["clientes"][1]["codigo"] == 202
        assert data["clientes"][2]["cliente"] == "Carlos Lima"

        app.dependency_overrides.clear()

    def test_autenticacao_negada_sem_token(self):
        """
        Autenticação negada: requisição sem token → 401.
        Todos os endpoints fora de /auth exigem JWT.
        """
        app.dependency_overrides[get_db] = override_get_db([])
        client = TestClient(app)

        response = client.get("/clientes/")

        assert response.status_code == 401

        app.dependency_overrides.clear()

    def test_lista_vazia_retorna_200(self):
        """
        Lista vazia: tabela Clientes sem registros → 200 com lista vazia.
        Não deve retornar 404.
        """
        app.dependency_overrides[get_db] = override_get_db([])
        client = TestClient(app)

        response = client.get("/clientes/", headers=auth_header())

        assert response.status_code == 200
        data = response.json()
        assert data["clientes"] == []

        app.dependency_overrides.clear()


# ---------------------------------------------------------------------------
# Testes — GET /clientes/proximo-codigo
# ---------------------------------------------------------------------------


class TestProximoCodigo:
    """GET /clientes/proximo-codigo — RN23, RN27."""

    def test_caminho_feliz_retorna_proximo_codigo(self):
        """
        RN23 — Retorna primeiro gap >= 10000.
        RN27 — Retorna próximo Id (MAX(Id)+1).
        """
        db = mock_db_proximo_codigo(max_id=50, codigos=[10000, 10001, 10002])

        def _override():
            yield db

        app.dependency_overrides[get_db] = _override
        client = TestClient(app)

        response = client.get("/clientes/proximo-codigo", headers=auth_header())

        assert response.status_code == 200
        data = response.json()
        assert data["proximo_codigo"] == 10003
        assert data["proximo_id"] == 51

        app.dependency_overrides.clear()

    def test_gap_no_meio_da_sequencia(self):
        """RN23 — Encontra gap no meio: 10000, 10002 → retorna 10001."""
        db = mock_db_proximo_codigo(max_id=10, codigos=[10000, 10002, 10003])

        def _override():
            yield db

        app.dependency_overrides[get_db] = _override
        client = TestClient(app)

        response = client.get("/clientes/proximo-codigo", headers=auth_header())

        assert response.status_code == 200
        assert response.json()["proximo_codigo"] == 10001

        app.dependency_overrides.clear()

    def test_nenhum_codigo_existente_retorna_10000(self):
        """RN23 — Sem códigos >= 10000 → retorna 10000."""
        db = mock_db_proximo_codigo(max_id=5, codigos=[])

        def _override():
            yield db

        app.dependency_overrides[get_db] = _override
        client = TestClient(app)

        response = client.get("/clientes/proximo-codigo", headers=auth_header())

        assert response.status_code == 200
        assert response.json()["proximo_codigo"] == 10000
        assert response.json()["proximo_id"] == 6

        app.dependency_overrides.clear()

    def test_autenticacao_negada_sem_token(self):
        """Autenticação negada: requisição sem token → 401."""
        client = TestClient(app)

        response = client.get("/clientes/proximo-codigo")

        assert response.status_code == 401

        app.dependency_overrides.clear()


# ---------------------------------------------------------------------------
# Testes — POST /clientes
# ---------------------------------------------------------------------------


class TestCriarCliente:
    """POST /clientes — RN21, RN22, RN24, RN25, RN26."""

    def test_caminho_feliz_cria_cliente(self):
        """
        Caminho feliz: código válido e único + nome preenchido → 201.
        RN25 — Nome convertido para maiúsculas.
        """
        inserted = FakeRow(Id=51, Código=10005, Cliente="JOÃO SILVA")
        db = mock_db_criar_cliente(existing=False, inserted_row=inserted)

        def _override():
            yield db

        app.dependency_overrides[get_db] = _override
        client = TestClient(app)

        response = client.post(
            "/clientes/",
            json={"codigo": 10005, "cliente": "João Silva"},
            headers=auth_header(),
        )

        assert response.status_code == 201
        data = response.json()
        assert data["id"] == 51
        assert data["codigo"] == 10005
        assert data["cliente"] == "JOÃO SILVA"

        app.dependency_overrides.clear()

    def test_codigo_duplicado_retorna_409(self):
        """
        RN21/RN29 — Código já existente → 409 com mensagem.
        """
        db = mock_db_criar_cliente(existing=True)

        def _override():
            yield db

        app.dependency_overrides[get_db] = _override
        client = TestClient(app)

        response = client.post(
            "/clientes/",
            json={"codigo": 10001, "cliente": "Teste"},
            headers=auth_header(),
        )

        assert response.status_code == 409
        assert "Código já existente" in response.json()["detail"]

        app.dependency_overrides.clear()

    def test_codigo_acima_do_limite_retorna_422(self):
        """
        RN22/RN30 — Código > 20.000 → 422 (validação Pydantic).
        """
        client = TestClient(app)

        response = client.post(
            "/clientes/",
            json={"codigo": 20001, "cliente": "Teste"},
            headers=auth_header(),
        )

        assert response.status_code == 422

        app.dependency_overrides.clear()

    def test_codigo_abaixo_do_minimo_retorna_422(self):
        """
        Código < 10.000 → 422 (validação Pydantic: ge=10000).
        """
        client = TestClient(app)

        response = client.post(
            "/clientes/",
            json={"codigo": 9999, "cliente": "Teste"},
            headers=auth_header(),
        )

        assert response.status_code == 422

        app.dependency_overrides.clear()

    def test_autenticacao_negada_sem_token(self):
        """Autenticação negada: requisição sem token → 401."""
        client = TestClient(app)

        response = client.post(
            "/clientes/",
            json={"codigo": 10001, "cliente": "Teste"},
        )

        assert response.status_code == 401

        app.dependency_overrides.clear()

    def test_cliente_vazio_retorna_422(self):
        """RN24 — Campo Cliente obrigatório (min_length=1)."""
        client = TestClient(app)

        response = client.post(
            "/clientes/",
            json={"codigo": 10001, "cliente": ""},
            headers=auth_header(),
        )

        assert response.status_code == 422

        app.dependency_overrides.clear()

    def test_codigo_ausente_retorna_422(self):
        """RN26 — Campo Código obrigatório."""
        client = TestClient(app)

        response = client.post(
            "/clientes/",
            json={"cliente": "Teste"},
            headers=auth_header(),
        )

        assert response.status_code == 422

        app.dependency_overrides.clear()


# ---------------------------------------------------------------------------
# Testes — GET /clientes/{id}
# ---------------------------------------------------------------------------


class TestBuscarCliente:
    """GET /clientes/{id} — RN33, RN42."""

    def test_caminho_feliz_sem_lancamentos(self):
        """
        RN33 — Retorna dados do cliente pelo Id.
        RN42 — tem_lancamentos=False quando Contas não tem registros.
        """
        db = MagicMock()

        result_cliente = MagicMock()
        result_cliente.fetchone.return_value = FakeRow(Id=1, Código=10001, Cliente="ANA SILVA")

        result_total = MagicMock()
        result_total.fetchone.return_value = FakeRow(total=0)

        db.execute.side_effect = [result_cliente, result_total]

        def _override():
            yield db

        app.dependency_overrides[get_db] = _override
        client = TestClient(app)

        response = client.get("/clientes/1", headers=auth_header())

        assert response.status_code == 200
        data = response.json()
        assert data["id"] == 1
        assert data["codigo"] == 10001
        assert data["cliente"] == "ANA SILVA"
        assert data["tem_lancamentos"] is False

        app.dependency_overrides.clear()

    def test_caminho_feliz_com_lancamentos(self):
        """RN42 — tem_lancamentos=True quando Contas tem registros."""
        db = MagicMock()

        result_cliente = MagicMock()
        result_cliente.fetchone.return_value = FakeRow(Id=2, Código=10002, Cliente="BRUNO COSTA")

        result_total = MagicMock()
        result_total.fetchone.return_value = FakeRow(total=5)

        db.execute.side_effect = [result_cliente, result_total]

        def _override():
            yield db

        app.dependency_overrides[get_db] = _override
        client = TestClient(app)

        response = client.get("/clientes/2", headers=auth_header())

        assert response.status_code == 200
        assert response.json()["tem_lancamentos"] is True

        app.dependency_overrides.clear()

    def test_nao_encontrado_retorna_404(self):
        """RN33 — Cliente inexistente → 404."""
        db = MagicMock()

        result_cliente = MagicMock()
        result_cliente.fetchone.return_value = None

        db.execute.return_value = result_cliente

        def _override():
            yield db

        app.dependency_overrides[get_db] = _override
        client = TestClient(app)

        response = client.get("/clientes/999", headers=auth_header())

        assert response.status_code == 404

        app.dependency_overrides.clear()

    def test_autenticacao_negada_sem_token(self):
        """Autenticação negada: requisição sem token → 401."""
        client = TestClient(app)

        response = client.get("/clientes/1")

        assert response.status_code == 401


# ---------------------------------------------------------------------------
# Testes — PUT /clientes/{id}
# ---------------------------------------------------------------------------


class TestAtualizarCliente:
    """PUT /clientes/{id} — RN36, RN37, RN47, RN48."""

    def _mock_db_update(
        self,
        codigo_duplicado: bool = False,
        cliente_existe: bool = True,
        updated_row=None,
    ):
        """Mock para PUT /clientes/{id} — unicidade + exists + UPDATE."""
        db = MagicMock()

        # 1ª chamada: SELECT unicidade (RN48)
        result_uniq = MagicMock()
        result_uniq.fetchone.return_value = FakeRow(Id=99) if codigo_duplicado else None

        # 2ª chamada: SELECT exists
        result_exists = MagicMock()
        result_exists.fetchone.return_value = FakeRow(Id=1) if cliente_existe else None

        # 3ª chamada: UPDATE
        result_update = MagicMock()
        result_update.fetchone.return_value = updated_row

        db.execute.side_effect = [
            result_uniq,
            result_exists,
            result_update,
        ]
        return db

    def test_alterar_codigo_retorna_200(self):
        """
        RN36/RN37 — Código ou nome alterado: backend salva sem lógica de
        confirmação (confirmação é responsabilidade do frontend).
        RN47 — Nome retornado em maiúsculas.
        """
        updated = FakeRow(Id=1, Código=10099, Cliente="ANA SILVA")
        db = self._mock_db_update(updated_row=updated)

        def _override():
            yield db

        app.dependency_overrides[get_db] = _override
        client = TestClient(app)

        response = client.put(
            "/clientes/1",
            json={"codigo": 10099, "cliente": "Ana Silva"},
            headers=auth_header(),
        )

        assert response.status_code == 200
        data = response.json()
        assert data["codigo"] == 10099
        assert data["cliente"] == "ANA SILVA"

        app.dependency_overrides.clear()

    def test_alterar_nome_retorna_200(self):
        """RN37 — Alterar só nome (mesmo código): backend salva normalmente."""
        updated = FakeRow(Id=1, Código=10001, Cliente="ANA COSTA")
        db = self._mock_db_update(updated_row=updated)

        def _override():
            yield db

        app.dependency_overrides[get_db] = _override
        client = TestClient(app)

        response = client.put(
            "/clientes/1",
            json={"codigo": 10001, "cliente": "Ana Costa"},
            headers=auth_header(),
        )

        assert response.status_code == 200
        assert response.json()["cliente"] == "ANA COSTA"

        app.dependency_overrides.clear()

    def test_codigo_duplicado_retorna_409(self):
        """RN48 — Código já usado por outro cliente → 409."""
        db = self._mock_db_update(codigo_duplicado=True)

        def _override():
            yield db

        app.dependency_overrides[get_db] = _override
        client = TestClient(app)

        response = client.put(
            "/clientes/1",
            json={"codigo": 10002, "cliente": "Teste"},
            headers=auth_header(),
        )

        assert response.status_code == 409
        assert "Código já existente" in response.json()["detail"]

        app.dependency_overrides.clear()

    def test_cliente_nao_encontrado_retorna_404(self):
        """RN33 — Cliente inexistente → 404."""
        db = self._mock_db_update(codigo_duplicado=False, cliente_existe=False)

        def _override():
            yield db

        app.dependency_overrides[get_db] = _override
        client = TestClient(app)

        response = client.put(
            "/clientes/999",
            json={"codigo": 10001, "cliente": "Teste"},
            headers=auth_header(),
        )

        assert response.status_code == 404

        app.dependency_overrides.clear()

    def test_autenticacao_negada_sem_token(self):
        """Autenticação negada: requisição sem token → 401."""
        client = TestClient(app)

        response = client.put(
            "/clientes/1",
            json={"codigo": 10001, "cliente": "Teste"},
        )

        assert response.status_code == 401


# ---------------------------------------------------------------------------
# Testes — POST /clientes/{id}/verificar-senha
# ---------------------------------------------------------------------------


class TestVerificarSenha:
    """POST /clientes/{id}/verificar-senha — RN41, RN42."""

    def _mock_db_senha(
        self,
        chave: str = "4321",
        total_lancamentos: int = 0,
    ):
        """Mock para /verificar-senha — SELECT Chaves + COUNT Contas."""
        db = MagicMock()

        result_chave = MagicMock()
        result_chave.fetchone.return_value = FakeRow(Chave=chave)

        result_total = MagicMock()
        result_total.fetchone.return_value = FakeRow(total=total_lancamentos)

        db.execute.side_effect = [result_chave, result_total]
        return db

    def test_senha_correta_sem_lancamentos(self):
        """
        RN41 — Senha correta → valido=True.
        RN42 — Sem lançamentos → tem_lancamentos=False.
        """
        db = self._mock_db_senha(chave="4321", total_lancamentos=0)

        def _override():
            yield db

        app.dependency_overrides[get_db] = _override
        client = TestClient(app)

        response = client.post(
            "/clientes/1/verificar-senha",
            json={"senha": "4321"},
            headers=auth_header(),
        )

        assert response.status_code == 200
        data = response.json()
        assert data["valido"] is True
        assert data["tem_lancamentos"] is False

        app.dependency_overrides.clear()

    def test_senha_correta_com_lancamentos(self):
        """RN42 — Com lançamentos → tem_lancamentos=True."""
        db = self._mock_db_senha(chave="4321", total_lancamentos=3)

        def _override():
            yield db

        app.dependency_overrides[get_db] = _override
        client = TestClient(app)

        response = client.post(
            "/clientes/1/verificar-senha",
            json={"senha": "4321"},
            headers=auth_header(),
        )

        assert response.status_code == 200
        data = response.json()
        assert data["valido"] is True
        assert data["tem_lancamentos"] is True

        app.dependency_overrides.clear()

    def test_senha_incorreta_retorna_valido_false(self):
        """RN41 — Senha errada → valido=False (não lança exceção)."""
        db = self._mock_db_senha(chave="4321", total_lancamentos=0)

        def _override():
            yield db

        app.dependency_overrides[get_db] = _override
        client = TestClient(app)

        response = client.post(
            "/clientes/1/verificar-senha",
            json={"senha": "ERRADA"},
            headers=auth_header(),
        )

        assert response.status_code == 200
        assert response.json()["valido"] is False

        app.dependency_overrides.clear()

    def test_autenticacao_negada_sem_token(self):
        """Autenticação negada: requisição sem token → 401."""
        client = TestClient(app)

        response = client.post(
            "/clientes/1/verificar-senha",
            json={"senha": "4321"},
        )

        assert response.status_code == 401


# ---------------------------------------------------------------------------
# Testes — DELETE /clientes/{id}
# ---------------------------------------------------------------------------


class TestExcluirCliente:
    """DELETE /clientes/{id} — RN43, RN44, RN45."""

    def _mock_db_delete(self, cliente_existe: bool = True):
        """Mock para DELETE — SELECT exists + DELETE Contas + DELETE Clientes."""
        db = MagicMock()

        result_exists = MagicMock()
        result_exists.fetchone.return_value = FakeRow(Id=1) if cliente_existe else None

        db.execute.side_effect = [
            result_exists,
            MagicMock(),  # DELETE Contas
            MagicMock(),  # DELETE Clientes
        ]
        return db

    def test_excluir_sem_lancamentos_retorna_204(self):
        """
        RN45 — Exclusão sem lançamentos: DELETE Clientes apenas.
        Backend sempre executa DELETE Contas + Clientes em sequência.
        """
        db = self._mock_db_delete(cliente_existe=True)

        def _override():
            yield db

        app.dependency_overrides[get_db] = _override
        client = TestClient(app)

        response = client.delete("/clientes/1", headers=auth_header())

        assert response.status_code == 204
        # Verifica que DELETE foi chamado (Contas + Clientes)
        assert db.execute.call_count == 3
        assert db.commit.called

        app.dependency_overrides.clear()

    def test_excluir_com_lancamentos_retorna_204(self):
        """
        RN44 — Exclusão com lançamentos: DELETE Contas + DELETE Clientes.
        O frontend já confirmou a exclusão dos lançamentos antes de chamar.
        """
        db = self._mock_db_delete(cliente_existe=True)

        def _override():
            yield db

        app.dependency_overrides[get_db] = _override
        client = TestClient(app)

        response = client.delete("/clientes/1", headers=auth_header())

        assert response.status_code == 204

        app.dependency_overrides.clear()

    def test_cliente_nao_encontrado_retorna_404(self):
        """RN33 — Cliente inexistente → 404."""
        db = self._mock_db_delete(cliente_existe=False)

        def _override():
            yield db

        app.dependency_overrides[get_db] = _override
        client = TestClient(app)

        response = client.delete("/clientes/999", headers=auth_header())

        assert response.status_code == 404

        app.dependency_overrides.clear()

    def test_autenticacao_negada_sem_token(self):
        """Autenticação negada: requisição sem token → 401."""
        client = TestClient(app)

        response = client.delete("/clientes/1")

        assert response.status_code == 401
