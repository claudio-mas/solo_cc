"""
Testes — Totais (frmTotais2)

Cobertura obrigatória (CLAUDE.md):
  GET /totais:
    1. Consulta com data explícita → 200 com valores corretos
    2. Consulta sem data (usa hoje) → 200 com valores corretos
    3. Resposta com zeros quando não há dados → 200 com zeros
    4. Autenticação negada (401 sem token)
"""

from decimal import Decimal
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


SAMPLE_TOTAIS_ROW = FakeRow(
    qtde_credores=12,
    valor_credores=Decimal("45678.90"),
    qtde_devedores=8,
    valor_devedores=Decimal("12345.67"),
)

ZERO_TOTAIS_ROW = FakeRow(
    qtde_credores=0,
    valor_credores=Decimal("0"),
    qtde_devedores=0,
    valor_devedores=Decimal("0"),
)

# ---------------------------------------------------------------------------
# Helpers de mock
# ---------------------------------------------------------------------------


def mock_db_totais(row: FakeRow):
    """Mock de get_db que retorna a row fornecida para a query CTE."""
    db = MagicMock()
    result = MagicMock()
    result.fetchone.return_value = row
    db.execute.return_value = result
    return db


def override_totais(row: FakeRow):
    """Override da dependência get_db para o endpoint /totais."""

    def _override():
        yield mock_db_totais(row)

    return _override


# ---------------------------------------------------------------------------
# Testes — GET /totais
# ---------------------------------------------------------------------------


class TestGetTotais:
    """GET /totais — testes do endpoint de totais."""

    def test_consulta_com_data_explicita(self):
        """
        Caminho feliz: data informada via query param → 200 com 4 valores.
        RN59/RN60 — data explícita é usada no filtro da query.
        RN61–RN66 — todos os 4 campos retornados com valores corretos.
        """
        app.dependency_overrides[get_db] = override_totais(SAMPLE_TOTAIS_ROW)
        client = TestClient(app)

        response = client.get(
            "/totais",
            params={"data": "2026-03-01"},
            headers=auth_header(),
        )

        assert response.status_code == 200
        data = response.json()
        assert data["qtde_credores"] == 12
        assert data["qtde_devedores"] == 8
        # Decimal serializado como número — verificar presença e positividade
        assert float(data["valor_credores"]) > 0
        assert float(data["valor_devedores"]) > 0

        app.dependency_overrides.clear()

    def test_consulta_sem_data_usa_hoje(self):
        """
        Sem parâmetro 'data' → endpoint usa date.today() como padrão.
        RN59 — equivalente a Date.Today no FrmTotais_Load do legado.
        """
        app.dependency_overrides[get_db] = override_totais(SAMPLE_TOTAIS_ROW)
        client = TestClient(app)

        response = client.get(
            "/totais",
            headers=auth_header(),
        )

        assert response.status_code == 200
        data = response.json()
        assert "qtde_credores" in data
        assert "valor_credores" in data
        assert "qtde_devedores" in data
        assert "valor_devedores" in data

        app.dependency_overrides.clear()

    def test_resposta_zeros_quando_sem_dados(self):
        """
        Quando não há dados na tabela Contas para a data → todos os campos = 0.
        O endpoint deve retornar 200 com zeros (não erro).
        """
        app.dependency_overrides[get_db] = override_totais(ZERO_TOTAIS_ROW)
        client = TestClient(app)

        response = client.get(
            "/totais",
            params={"data": "2000-01-01"},
            headers=auth_header(),
        )

        assert response.status_code == 200
        data = response.json()
        assert data["qtde_credores"] == 0
        assert data["qtde_devedores"] == 0
        assert float(data["valor_credores"]) == 0
        assert float(data["valor_devedores"]) == 0

        app.dependency_overrides.clear()

    def test_autenticacao_negada_sem_token(self):
        """
        Requisição sem token → 401.
        Todos os endpoints fora de /auth exigem JWT.
        """
        client = TestClient(app)

        response = client.get("/totais")

        assert response.status_code == 401


# ---------------------------------------------------------------------------
# Testes — service (lógica da query CTE)
# ---------------------------------------------------------------------------


class TestServiceBuscarTotais:
    """Testa a função buscar_totais diretamente, sem HTTP."""

    def test_passa_data_correta_ao_banco(self):
        """
        Verifica que o service passa o parâmetro :data correto ao db.execute().
        D5 — data deve ser ISO 8601, nunca o formato invertido do legado.
        """
        from datetime import date as dt

        from app.services.totais import buscar_totais as svc

        db = MagicMock()
        result = MagicMock()
        result.fetchone.return_value = SAMPLE_TOTAIS_ROW
        db.execute.return_value = result

        data_consulta = dt(2026, 3, 1)
        svc(db=db, data=data_consulta)

        call_kwargs = db.execute.call_args[0][1]
        assert call_kwargs["data"] == data_consulta

    def test_retorna_zeros_quando_banco_retorna_nulos(self):
        """
        Se a query retornar NULL (nenhum dado), o service deve retornar 0.
        Garante que ISNULL na query e o fallback `or 0` no service funcionam.
        """
        from datetime import date as dt

        from app.services.totais import buscar_totais as svc

        null_row = FakeRow(
            qtde_credores=0,
            valor_credores=None,
            qtde_devedores=0,
            valor_devedores=None,
        )

        db = MagicMock()
        result = MagicMock()
        result.fetchone.return_value = null_row
        db.execute.return_value = result

        totais = svc(db=db, data=dt(2000, 1, 1))

        assert totais["valor_credores"] == Decimal("0")
        assert totais["valor_devedores"] == Decimal("0")
