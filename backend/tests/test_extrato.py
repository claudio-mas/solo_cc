"""
Testes — Extrato (FrmExtrato)

Cobertura obrigatória (CLAUDE.md):
  GET /extrato/{id_cliente}:
    1. Listagem sem filtros
    2. Filtro por pasta (RN79)
    3. Filtro por ND (RN80)
    4. Filtro sem_nd (RN81)
    5. Filtro por histórico (RN82)
    6. Combinação de filtros (RN83)
    7. Saldo acumulado correto (RN84)
  PATCH /lancamentos/{id}:
    8. Edição de cada campo editável (RN88/RN90)
    9. Lançamento não encontrado → 404
  POST /extrato/desbloquear:
   10. Senha correta → {ok: true} (RN87)
   11. Senha incorreta → {ok: false}
  POST /extrato/transferir:
   12. Transferência com senha correta (RN93–RN98)
   13. Senha incorreta → 403 (RN94)
   14. Destino = origem → 400 (RN96)
  POST /extrato/{id}/sincronizar-vvalor:
   15. Sincronização bem-sucedida (RN99)
  Autenticação negada:
   16. Sem token → 401 para cada endpoint
"""

from datetime import datetime
from decimal import Decimal
from unittest.mock import MagicMock

from fastapi.testclient import TestClient
from jose import jwt

from app.dependencies import get_db
from app.main import app

# ---------------------------------------------------------------------------
# Configuração
# ---------------------------------------------------------------------------

SECRET_KEY = "troque-esta-chave-em-producao"
ALGORITHM = "HS256"


def make_token(usuario: str = "Admin", perfil: str = "Administrador") -> str:
    return jwt.encode({"sub": usuario, "perfil": perfil}, SECRET_KEY, algorithm=ALGORITHM)


def auth_header(token: str | None = None) -> dict:
    t = token or make_token()
    return {"Authorization": f"Bearer {t}"}


# ---------------------------------------------------------------------------
# Fake rows
# ---------------------------------------------------------------------------


class FakeRow:
    def __init__(self, **kwargs):
        for k, v in kwargs.items():
            setattr(self, k, v)


LANCAMENTO_1 = FakeRow(
    Id=1, Dt=datetime(2026, 1, 15), Conta=1, ND="123", Ref="Aluguel",
    VValor=Decimal("100.00"), DC="D", Deb=Decimal("100.00"), Cred=None,
    Saldo=Decimal("-100.00"),
)

LANCAMENTO_2 = FakeRow(
    Id=2, Dt=datetime(2026, 1, 20), Conta=1, ND=None, Ref="Pagamento",
    VValor=Decimal("50.00"), DC="C", Deb=None, Cred=Decimal("50.00"),
    Saldo=Decimal("-50.00"),
)

LANCAMENTO_3 = FakeRow(
    Id=3, Dt=datetime(2026, 2, 1), Conta=2, ND="456", Ref="AGENCIA taxa",
    VValor=Decimal("30.00"), DC="D", Deb=Decimal("30.00"), Cred=None,
    Saldo=Decimal("-80.00"),
)

CLIENTE_DESTINO = FakeRow(Id=99, Código=10001, Cliente="Cliente Destino")
CHAVE_DESBLOQUEIO = FakeRow(Chave="desbloqueio123")
CHAVE_TRANSFERENCIA = FakeRow(Chave="transferencia456")


# ---------------------------------------------------------------------------
# Helpers de mock
# ---------------------------------------------------------------------------


def make_db(fetchall=None, fetchone=None):
    db = MagicMock()
    result = MagicMock()
    result.fetchall.return_value = fetchall or []
    result.fetchone.return_value = fetchone
    result.rowcount = 2
    db.execute.return_value = result
    return db


def override_db(db):
    def _override():
        yield db

    return _override


# ---------------------------------------------------------------------------
# Testes — GET /extrato/{id_cliente}
# ---------------------------------------------------------------------------


class TestListarExtrato:
    def test_listagem_sem_filtros(self):
        """RN78 — extrato do cliente sem filtros, retorna todos os lançamentos."""
        db = make_db(fetchall=[LANCAMENTO_1, LANCAMENTO_2, LANCAMENTO_3])
        app.dependency_overrides[get_db] = override_db(db)
        client = TestClient(app)

        resp = client.get("/extrato/10", headers=auth_header())

        assert resp.status_code == 200
        data = resp.json()
        assert len(data["lancamentos"]) == 3
        assert "saldo_total" in data

        # Verifica que a query contém WHERE IdCliente
        sql = str(db.execute.call_args[0][0])
        assert "IdCliente" in sql

        app.dependency_overrides.clear()

    def test_filtro_por_pasta(self):
        """RN79 — filtro por Pasta (Conta)."""
        db = make_db(fetchall=[LANCAMENTO_1])
        app.dependency_overrides[get_db] = override_db(db)
        client = TestClient(app)

        resp = client.get("/extrato/10?pasta=1", headers=auth_header())

        assert resp.status_code == 200
        sql = str(db.execute.call_args[0][0])
        assert "Conta" in sql

        app.dependency_overrides.clear()

    def test_filtro_por_nd(self):
        """RN80 — filtro por ND."""
        db = make_db(fetchall=[LANCAMENTO_1])
        app.dependency_overrides[get_db] = override_db(db)
        client = TestClient(app)

        resp = client.get("/extrato/10?nd=123", headers=auth_header())

        assert resp.status_code == 200
        sql = str(db.execute.call_args[0][0])
        assert "ND" in sql

        app.dependency_overrides.clear()

    def test_filtro_sem_nd(self):
        """RN81 — checkbox Sem ND (ND IS NULL)."""
        db = make_db(fetchall=[LANCAMENTO_2])
        app.dependency_overrides[get_db] = override_db(db)
        client = TestClient(app)

        resp = client.get("/extrato/10?sem_nd=true", headers=auth_header())

        assert resp.status_code == 200
        sql = str(db.execute.call_args[0][0])
        assert "ND IS NULL" in sql

        app.dependency_overrides.clear()

    def test_filtro_por_historico(self):
        """RN82 — filtro por Histórico (LIKE)."""
        db = make_db(fetchall=[LANCAMENTO_1])
        app.dependency_overrides[get_db] = override_db(db)
        client = TestClient(app)

        resp = client.get("/extrato/10?hist=Aluguel", headers=auth_header())

        assert resp.status_code == 200
        sql = str(db.execute.call_args[0][0])
        assert "LIKE" in sql

        app.dependency_overrides.clear()

    def test_combinacao_de_filtros(self):
        """RN83 — combinação de filtros simultâneos (AND)."""
        db = make_db(fetchall=[LANCAMENTO_1])
        app.dependency_overrides[get_db] = override_db(db)
        client = TestClient(app)

        resp = client.get("/extrato/10?pasta=1&nd=123&hist=Aluguel", headers=auth_header())

        assert resp.status_code == 200
        sql = str(db.execute.call_args[0][0])
        assert "Conta" in sql
        assert "ND" in sql
        assert "LIKE" in sql

        app.dependency_overrides.clear()

    def test_saldo_acumulado_correto(self):
        """RN84 — saldo acumulado via window function. O saldo_total é o último saldo."""
        db = make_db(fetchall=[LANCAMENTO_1, LANCAMENTO_2, LANCAMENTO_3])
        app.dependency_overrides[get_db] = override_db(db)
        client = TestClient(app)

        resp = client.get("/extrato/10", headers=auth_header())

        assert resp.status_code == 200
        data = resp.json()
        # Último saldo é -80.00 (do LANCAMENTO_3)
        assert Decimal(data["saldo_total"]) == Decimal("-80.00")
        # A query deve usar window function
        sql = str(db.execute.call_args[0][0])
        assert "OVER" in sql
        assert "ORDER BY" in sql

        app.dependency_overrides.clear()

    def test_autenticacao_negada_sem_token(self):
        client = TestClient(app)
        resp = client.get("/extrato/10")
        assert resp.status_code == 401


# ---------------------------------------------------------------------------
# Testes — PATCH /lancamentos/{id}
# ---------------------------------------------------------------------------


class TestAtualizarLancamento:
    def test_edicao_campo_nd(self):
        """RN88/RN90 — edição do campo ND salva imediatamente."""
        db = MagicMock()
        db.execute.return_value.fetchone.return_value = FakeRow(Id=1)
        app.dependency_overrides[get_db] = override_db(db)
        client = TestClient(app)

        resp = client.patch(
            "/lancamentos/1",
            json={"nd": "789"},
            headers=auth_header(),
        )

        assert resp.status_code == 200
        assert resp.json()["ok"] is True

        # Verifica que houve UPDATE com ND
        update_found = False
        for c in db.execute.call_args_list:
            sql = str(c[0][0])
            if "UPDATE" in sql.upper() and "ND" in sql:
                update_found = True
                break
        assert update_found, "UPDATE com ND não encontrado"

        app.dependency_overrides.clear()

    def test_edicao_campo_ref(self):
        """RN88/RN90 — edição do campo Histórico (Ref)."""
        db = MagicMock()
        db.execute.return_value.fetchone.return_value = FakeRow(Id=1)
        app.dependency_overrides[get_db] = override_db(db)
        client = TestClient(app)

        resp = client.patch(
            "/lancamentos/1",
            json={"ref": "Novo histórico"},
            headers=auth_header(),
        )

        assert resp.status_code == 200
        assert resp.json()["ok"] is True

        app.dependency_overrides.clear()

    def test_edicao_campo_dt(self):
        """RN88/RN90 — edição do campo Data (Dt)."""
        db = MagicMock()
        db.execute.return_value.fetchone.return_value = FakeRow(Id=1)
        app.dependency_overrides[get_db] = override_db(db)
        client = TestClient(app)

        resp = client.patch(
            "/lancamentos/1",
            json={"dt": "2026-03-15"},
            headers=auth_header(),
        )

        assert resp.status_code == 200
        assert resp.json()["ok"] is True

        app.dependency_overrides.clear()

    def test_edicao_campo_conta(self):
        """RN88/RN90 — edição do campo Pasta (Conta)."""
        db = MagicMock()
        db.execute.return_value.fetchone.return_value = FakeRow(Id=1)
        app.dependency_overrides[get_db] = override_db(db)
        client = TestClient(app)

        resp = client.patch(
            "/lancamentos/1",
            json={"conta": 5},
            headers=auth_header(),
        )

        assert resp.status_code == 200
        assert resp.json()["ok"] is True

        app.dependency_overrides.clear()

    def test_edicao_campo_deb(self):
        """RN88/RN90 — edição do campo Débito (Deb)."""
        db = MagicMock()
        db.execute.return_value.fetchone.return_value = FakeRow(Id=1)
        app.dependency_overrides[get_db] = override_db(db)
        client = TestClient(app)

        resp = client.patch(
            "/lancamentos/1",
            json={"deb": "150.50"},
            headers=auth_header(),
        )

        assert resp.status_code == 200
        assert resp.json()["ok"] is True

        app.dependency_overrides.clear()

    def test_edicao_campo_cred(self):
        """RN88/RN90 — edição do campo Crédito (Cred)."""
        db = MagicMock()
        db.execute.return_value.fetchone.return_value = FakeRow(Id=1)
        app.dependency_overrides[get_db] = override_db(db)
        client = TestClient(app)

        resp = client.patch(
            "/lancamentos/1",
            json={"cred": "200.00"},
            headers=auth_header(),
        )

        assert resp.status_code == 200
        assert resp.json()["ok"] is True

        app.dependency_overrides.clear()

    def test_lancamento_nao_encontrado(self):
        """Lançamento inexistente → 404."""
        db = MagicMock()
        db.execute.return_value.fetchone.return_value = None
        app.dependency_overrides[get_db] = override_db(db)
        client = TestClient(app)

        resp = client.patch(
            "/lancamentos/9999",
            json={"nd": "xxx"},
            headers=auth_header(),
        )

        assert resp.status_code == 404

        app.dependency_overrides.clear()

    def test_autenticacao_negada_sem_token(self):
        client = TestClient(app)
        resp = client.patch("/lancamentos/1", json={"nd": "x"})
        assert resp.status_code == 401


# ---------------------------------------------------------------------------
# Testes — POST /extrato/desbloquear
# ---------------------------------------------------------------------------


class TestDesbloquear:
    def test_senha_correta_retorna_ok_true(self):
        """RN87 — senha correta para desbloqueio → {ok: true}."""
        db = make_db(fetchone=FakeRow(Chave="desbloqueio123"))
        app.dependency_overrides[get_db] = override_db(db)
        client = TestClient(app)

        resp = client.post(
            "/extrato/desbloquear",
            json={"chave": "desbloqueio123"},
            headers=auth_header(),
        )

        assert resp.status_code == 200
        assert resp.json()["ok"] is True

        app.dependency_overrides.clear()

    def test_senha_incorreta_retorna_ok_false(self):
        """RN87 — senha incorreta → {ok: false}."""
        db = make_db(fetchone=FakeRow(Chave="desbloqueio123"))
        app.dependency_overrides[get_db] = override_db(db)
        client = TestClient(app)

        resp = client.post(
            "/extrato/desbloquear",
            json={"chave": "errada"},
            headers=auth_header(),
        )

        assert resp.status_code == 200
        assert resp.json()["ok"] is False

        app.dependency_overrides.clear()

    def test_autenticacao_negada_sem_token(self):
        client = TestClient(app)
        resp = client.post("/extrato/desbloquear", json={"chave": "x"})
        assert resp.status_code == 401


# ---------------------------------------------------------------------------
# Testes — POST /extrato/transferir
# ---------------------------------------------------------------------------


class TestTransferir:
    def test_transferencia_com_senha_correta(self):
        """RN93–RN98 — transferência bem-sucedida."""
        db = MagicMock()
        result_mock = MagicMock()
        # Call 1: verificar_senha_operacao → SELECT Chave FROM Chaves
        # Call 2: SELECT Código FROM Clientes (destino)
        # Call 3: UPDATE Contas
        chave_row = FakeRow(Chave="transferencia456")
        destino_row = FakeRow(Id=99, Código=10001)
        result_mock.fetchone.side_effect = [chave_row, destino_row]
        result_mock.rowcount = 2
        db.execute.return_value = result_mock

        app.dependency_overrides[get_db] = override_db(db)
        client = TestClient(app)

        resp = client.post(
            "/extrato/transferir?id_cliente_origem=10",
            json={"ids": [1, 2], "id_destino": 99, "chave": "transferencia456"},
            headers=auth_header(),
        )

        assert resp.status_code == 200
        data = resp.json()
        assert data["ok"] is True
        assert data["transferidos"] == 2

        app.dependency_overrides.clear()

    def test_transferencia_senha_incorreta(self):
        """RN94 — senha incorreta → 403."""
        db = make_db(fetchone=FakeRow(Chave="transferencia456"))
        app.dependency_overrides[get_db] = override_db(db)
        client = TestClient(app)

        resp = client.post(
            "/extrato/transferir?id_cliente_origem=10",
            json={"ids": [1], "id_destino": 99, "chave": "errada"},
            headers=auth_header(),
        )

        assert resp.status_code == 403

        app.dependency_overrides.clear()

    def test_transferencia_destino_igual_origem(self):
        """RN96 — destino = origem → 400."""
        db = make_db()
        app.dependency_overrides[get_db] = override_db(db)
        client = TestClient(app)

        resp = client.post(
            "/extrato/transferir?id_cliente_origem=10",
            json={"ids": [1], "id_destino": 10, "chave": "qualquer"},
            headers=auth_header(),
        )

        assert resp.status_code == 400

        app.dependency_overrides.clear()

    def test_autenticacao_negada_sem_token(self):
        client = TestClient(app)
        resp = client.post(
            "/extrato/transferir?id_cliente_origem=10",
            json={"ids": [1], "id_destino": 99, "chave": "x"},
        )
        assert resp.status_code == 401


# ---------------------------------------------------------------------------
# Testes — POST /extrato/{id}/sincronizar-vvalor
# ---------------------------------------------------------------------------


class TestSincronizarVValor:
    def test_sincronizacao_bem_sucedida(self):
        """RN99 — sincroniza VValor com Deb/Cred."""
        db = make_db()
        app.dependency_overrides[get_db] = override_db(db)
        client = TestClient(app)

        resp = client.post("/extrato/10/sincronizar-vvalor", headers=auth_header())

        assert resp.status_code == 200
        data = resp.json()
        assert data["ok"] is True

        # Verifica que dois UPDATEs foram executados (Deb > 0 e Cred > 0)
        update_calls = [
            c for c in db.execute.call_args_list
            if "UPDATE" in str(c[0][0]).upper()
        ]
        assert len(update_calls) == 2

        app.dependency_overrides.clear()

    def test_autenticacao_negada_sem_token(self):
        client = TestClient(app)
        resp = client.post("/extrato/10/sincronizar-vvalor")
        assert resp.status_code == 401


# ---------------------------------------------------------------------------
# Testes — GET /extrato/{id}/clientes-destino
# ---------------------------------------------------------------------------


class TestClientesDestino:
    def test_lista_clientes_excluindo_atual(self):
        """RN96 — lista clientes excluindo o atual."""
        db = make_db(fetchall=[CLIENTE_DESTINO])
        app.dependency_overrides[get_db] = override_db(db)
        client = TestClient(app)

        resp = client.get("/extrato/10/clientes-destino", headers=auth_header())

        assert resp.status_code == 200
        data = resp.json()
        assert len(data) == 1
        assert data[0]["cliente"] == "Cliente Destino"

        app.dependency_overrides.clear()

    def test_autenticacao_negada_sem_token(self):
        client = TestClient(app)
        resp = client.get("/extrato/10/clientes-destino")
        assert resp.status_code == 401
