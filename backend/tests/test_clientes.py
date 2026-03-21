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

from unittest.mock import MagicMock, call

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
        db = mock_db_proximo_codigo(
            max_id=50, codigos=[10000, 10001, 10002]
        )

        def _override():
            yield db

        app.dependency_overrides[get_db] = _override
        client = TestClient(app)

        response = client.get(
            "/clientes/proximo-codigo", headers=auth_header()
        )

        assert response.status_code == 200
        data = response.json()
        assert data["proximo_codigo"] == 10003
        assert data["proximo_id"] == 51

        app.dependency_overrides.clear()

    def test_gap_no_meio_da_sequencia(self):
        """RN23 — Encontra gap no meio: 10000, 10002 → retorna 10001."""
        db = mock_db_proximo_codigo(
            max_id=10, codigos=[10000, 10002, 10003]
        )

        def _override():
            yield db

        app.dependency_overrides[get_db] = _override
        client = TestClient(app)

        response = client.get(
            "/clientes/proximo-codigo", headers=auth_header()
        )

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

        response = client.get(
            "/clientes/proximo-codigo", headers=auth_header()
        )

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
