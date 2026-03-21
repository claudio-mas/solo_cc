"""
Testes — GET /clientes (FrmPrincipal)

Cobertura obrigatória (CLAUDE.md):
  1. Caminho feliz (200 com lista ordenada)
  2. Autenticação negada (401 sem token)
  3. Lista vazia (200 com lista vazia, não 404)
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


# ---------------------------------------------------------------------------
# Dados de teste
# ---------------------------------------------------------------------------


class FakeRow:
    """Simula uma row SQLAlchemy com atributos nomeados."""

    def __init__(self, Id, Código, Cliente):
        self.Id = Id
        self.Código = Código
        self.Cliente = Cliente


SAMPLE_CLIENTES = [
    FakeRow(Id=1, Código=101, Cliente="Ana Silva"),
    FakeRow(Id=2, Código=202, Cliente="Bruno Costa"),
    FakeRow(Id=3, Código=303, Cliente="Carlos Lima"),
]


# ---------------------------------------------------------------------------
# Testes
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
