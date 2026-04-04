"""
Testes — Relatórios (frmReports / frmRptDevedores1 / frmCredores1)

Cobertura:
  GET /relatorios/devedores:
    1. Sem filtros (happy path) — RN108
    2. Faixa de código acima (>=10000) — RN109
    3. Faixa de código abaixo (<10000) — RN109
    4. Filtro por saldo mínimo — RN110
    5. Ordenação por nome — RN111
    6. Resultado vazio (data no passado)
  GET /relatorios/credores:
    7. Sem filtros (happy path) — RN114
    8. Filtro por saldo mínimo — RN116
  Autenticação negada:
    9. Sem token → 401 em /devedores
   10. Sem token → 401 em /credores
"""

from datetime import date
from unittest.mock import MagicMock, patch

from fastapi.testclient import TestClient
from jose import jwt

from app.dependencies import get_db
from app.main import app

# ---------------------------------------------------------------------------
# Helpers
# ---------------------------------------------------------------------------

SECRET_KEY = "troque-esta-chave-em-producao"
ALGORITHM = "HS256"


def make_token(usuario: str = "Admin", perfil: str = "Administrador") -> str:
    return jwt.encode({"sub": usuario, "perfil": perfil}, SECRET_KEY, algorithm=ALGORITHM)


def auth_header() -> dict:
    return {"Authorization": f"Bearer {make_token()}"}


# ---------------------------------------------------------------------------
# Fake data
# ---------------------------------------------------------------------------


def _fake_devedor(cod: int = 10001, nome: str = "Cliente Devedor", saldo: float = 500.0):
    row = MagicMock()
    row.id = cod
    row.CodCliente = cod
    row.Cliente = nome
    row.saldo = saldo
    return row


def _fake_credor(cod: int = 10002, nome: str = "Cliente Credor", saldo: float = 300.0):
    row = MagicMock()
    row.id = cod
    row.CodCliente = cod
    row.Cliente = nome
    row.saldo = saldo
    return row


# ---------------------------------------------------------------------------
# Fixture de banco mockado
# ---------------------------------------------------------------------------


def _override_db_with(rows):
    mock_db = MagicMock()
    mock_db.execute.return_value.fetchall.return_value = rows

    def override():
        yield mock_db

    return override


# ---------------------------------------------------------------------------
# Testes — Devedores
# ---------------------------------------------------------------------------


def test_devedores_sem_filtros():
    """RN108 — retorna lista de devedores sem filtros adicionais."""
    rows = [_fake_devedor()]
    app.dependency_overrides[get_db] = _override_db_with(rows)
    client = TestClient(app)

    resp = client.get(
        "/relatorios/devedores",
        params={"data_corte": "2026-04-04"},
        headers=auth_header(),
    )

    assert resp.status_code == 200
    data = resp.json()
    assert len(data["itens"]) == 1
    assert data["itens"][0]["cod_cliente"] == 10001
    assert "devedores" in data["titulo"].lower()
    app.dependency_overrides.clear()


def test_devedores_faixa_acima():
    """RN109 — faixa_codigo=acima (código >= 10000)."""
    rows = [_fake_devedor(cod=10001)]
    app.dependency_overrides[get_db] = _override_db_with(rows)
    client = TestClient(app)

    resp = client.get(
        "/relatorios/devedores",
        params={"data_corte": "2026-04-04", "faixa_codigo": "acima"},
        headers=auth_header(),
    )

    assert resp.status_code == 200
    assert resp.json()["faixa_codigo"] == "acima"
    app.dependency_overrides.clear()


def test_devedores_faixa_abaixo():
    """RN109 — faixa_codigo=abaixo (código < 10000)."""
    rows = [_fake_devedor(cod=500)]
    app.dependency_overrides[get_db] = _override_db_with(rows)
    client = TestClient(app)

    resp = client.get(
        "/relatorios/devedores",
        params={"data_corte": "2026-04-04", "faixa_codigo": "abaixo"},
        headers=auth_header(),
    )

    assert resp.status_code == 200
    assert resp.json()["faixa_codigo"] == "abaixo"
    app.dependency_overrides.clear()


def test_devedores_saldo_minimo():
    """RN110 — filtra por saldo mínimo."""
    rows = [_fake_devedor(saldo=1000.0)]
    app.dependency_overrides[get_db] = _override_db_with(rows)
    client = TestClient(app)

    resp = client.get(
        "/relatorios/devedores",
        params={"data_corte": "2026-04-04", "saldo_minimo": "500"},
        headers=auth_header(),
    )

    assert resp.status_code == 200
    assert resp.json()["saldo_minimo"] == 500.0
    app.dependency_overrides.clear()


def test_devedores_ordenacao_nome():
    """RN111 — ordenação por nome."""
    rows = [_fake_devedor()]
    app.dependency_overrides[get_db] = _override_db_with(rows)
    client = TestClient(app)

    resp = client.get(
        "/relatorios/devedores",
        params={"data_corte": "2026-04-04", "ordenacao": "nome"},
        headers=auth_header(),
    )

    assert resp.status_code == 200
    assert resp.json()["ordenacao"] == "nome"
    app.dependency_overrides.clear()


def test_devedores_resultado_vazio():
    """Retorna lista vazia quando não há devedores no período."""
    app.dependency_overrides[get_db] = _override_db_with([])
    client = TestClient(app)

    resp = client.get(
        "/relatorios/devedores",
        params={"data_corte": "2000-01-01"},
        headers=auth_header(),
    )

    assert resp.status_code == 200
    assert resp.json()["itens"] == []
    app.dependency_overrides.clear()


# ---------------------------------------------------------------------------
# Testes — Credores
# ---------------------------------------------------------------------------


def test_credores_sem_filtros():
    """RN114 — retorna lista de credores sem filtros adicionais."""
    rows = [_fake_credor()]
    app.dependency_overrides[get_db] = _override_db_with(rows)
    client = TestClient(app)

    resp = client.get(
        "/relatorios/credores",
        params={"data_corte": "2026-04-04"},
        headers=auth_header(),
    )

    assert resp.status_code == 200
    data = resp.json()
    assert len(data["itens"]) == 1
    assert "credores" in data["titulo"].lower()
    app.dependency_overrides.clear()


def test_credores_saldo_minimo():
    """RN116 — filtra credores por saldo mínimo."""
    rows = [_fake_credor(saldo=800.0)]
    app.dependency_overrides[get_db] = _override_db_with(rows)
    client = TestClient(app)

    resp = client.get(
        "/relatorios/credores",
        params={"data_corte": "2026-04-04", "saldo_minimo": "200"},
        headers=auth_header(),
    )

    assert resp.status_code == 200
    assert resp.json()["saldo_minimo"] == 200.0
    app.dependency_overrides.clear()


# ---------------------------------------------------------------------------
# Testes — Autenticação negada
# ---------------------------------------------------------------------------


def test_devedores_sem_autenticacao():
    """Endpoint /relatorios/devedores exige autenticação → 401."""
    client = TestClient(app)
    resp = client.get("/relatorios/devedores", params={"data_corte": "2026-04-04"})
    assert resp.status_code == 401


def test_credores_sem_autenticacao():
    """Endpoint /relatorios/credores exige autenticação → 401."""
    client = TestClient(app)
    resp = client.get("/relatorios/credores", params={"data_corte": "2026-04-04"})
    assert resp.status_code == 401
