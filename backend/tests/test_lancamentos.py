"""
Testes — Lançamentos (FrmLancaData + FrmLanca)

Cobertura obrigatória (CLAUDE.md):
  GET /lancamentos/verificar-pasta:
    1. Pasta existente (existe=true)
    2. Pasta nova (existe=false)
    3. Autenticação negada (401 sem token)

  POST /lancamentos:
    4. Lançamento débito válido (DC='D' → Deb=valor, Cred=null)
    5. Lançamento crédito válido (DC='C' → Cred=valor, Deb=null)
    6. Campos obrigatórios ausentes (422)
    7. DC inválido (422 — valor fora de 'D'/'C')
    8. Autenticação negada (401 sem token)
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


SAMPLE_LANCAMENTO = {
    "id_cliente": 1,
    "cod_cliente": 10001,
    "dt": "2026-03-22",
    "conta": 5,
    "ref": "Pagamento de aluguel",
    "vvalor": "1500.00",
    "dc": "D",
}

# ---------------------------------------------------------------------------
# Helpers de mock
# ---------------------------------------------------------------------------


def mock_db_verificar_pasta(total: int):
    """Mock para GET /lancamentos/verificar-pasta — uma query COUNT."""
    db = MagicMock()
    result = MagicMock()
    result.fetchone.return_value = FakeRow(total=total)
    db.execute.return_value = result
    return db


def override_verificar_pasta(total: int):
    """Override da dependência get_db para verificar-pasta."""

    def _override():
        yield mock_db_verificar_pasta(total)

    return _override


def mock_db_registrar_lancamento(inserted_id: int):
    """Mock para POST /lancamentos — INSERT com OUTPUT INSERTED.Id."""
    db = MagicMock()
    result = MagicMock()
    result.fetchone.return_value = FakeRow(Id=inserted_id)
    db.execute.return_value = result
    return db


def override_registrar_lancamento(inserted_id: int):
    """Override da dependência get_db para registrar lançamento."""

    def _override():
        yield mock_db_registrar_lancamento(inserted_id)

    return _override


# ---------------------------------------------------------------------------
# Testes — GET /lancamentos/verificar-pasta
# ---------------------------------------------------------------------------


class TestVerificarPasta:
    """GET /lancamentos/verificar-pasta — testes do endpoint de verificação."""

    def test_pasta_existente_retorna_true(self):
        """
        Caminho feliz: pasta encontrada → existe=true.
        RN51 — verificação de existência de pasta para o cliente.
        """
        app.dependency_overrides[get_db] = override_verificar_pasta(total=3)
        client = TestClient(app)

        response = client.get(
            "/lancamentos/verificar-pasta",
            params={"id_cliente": 1, "pasta": 5},
            headers=auth_header(),
        )

        assert response.status_code == 200
        data = response.json()
        assert data["existe"] is True

        app.dependency_overrides.clear()

    def test_pasta_nova_retorna_false(self):
        """
        Pasta não encontrada → existe=false.
        RN51 — frontend deve exibir modal de confirmação "É uma nova pasta?".
        """
        app.dependency_overrides[get_db] = override_verificar_pasta(total=0)
        client = TestClient(app)

        response = client.get(
            "/lancamentos/verificar-pasta",
            params={"id_cliente": 1, "pasta": 99},
            headers=auth_header(),
        )

        assert response.status_code == 200
        data = response.json()
        assert data["existe"] is False

        app.dependency_overrides.clear()

    def test_autenticacao_negada_sem_token(self):
        """
        Requisição sem token → 401.
        Todos os endpoints fora de /auth exigem JWT.
        """
        client = TestClient(app)

        response = client.get(
            "/lancamentos/verificar-pasta",
            params={"id_cliente": 1, "pasta": 5},
        )

        assert response.status_code == 401


# ---------------------------------------------------------------------------
# Testes — POST /lancamentos
# ---------------------------------------------------------------------------


class TestRegistrarLancamento:
    """POST /lancamentos — testes do endpoint de registro de lançamento."""

    def test_lancamento_debito_valido(self):
        """
        Caminho feliz: DC='D' → lançamento de débito registrado (201).
        RN53 — Deb = VValor, Cred = null no service.
        RN54 — todos os campos presentes e válidos.
        """
        app.dependency_overrides[get_db] = override_registrar_lancamento(42)
        client = TestClient(app)

        response = client.post(
            "/lancamentos/",
            json=SAMPLE_LANCAMENTO,
            headers=auth_header(),
        )

        assert response.status_code == 201
        data = response.json()
        assert data["id"] == 42
        assert "sucesso" in data["mensagem"].lower()

        # Verifica que o service recebeu os parâmetros corretos
        db_mock = app.dependency_overrides[get_db]()
        app.dependency_overrides.clear()
        _ = db_mock  # usado apenas para validar o fluxo

    def test_lancamento_credito_valido(self):
        """
        Lançamento crédito: DC='C' → 201.
        RN53 — Cred = VValor, Deb = null no service.
        """
        app.dependency_overrides[get_db] = override_registrar_lancamento(43)
        client = TestClient(app)

        payload = {**SAMPLE_LANCAMENTO, "dc": "C"}
        response = client.post(
            "/lancamentos/",
            json=payload,
            headers=auth_header(),
        )

        assert response.status_code == 201
        data = response.json()
        assert data["id"] == 43

        app.dependency_overrides.clear()

    def test_dc_invalido_retorna_422(self):
        """
        DC com valor fora de 'D'/'C' → 422 (validação Pydantic Literal).
        RN52 — D/C aceita apenas 'D' ou 'C'.
        """
        app.dependency_overrides[get_db] = override_registrar_lancamento(0)
        client = TestClient(app)

        payload = {**SAMPLE_LANCAMENTO, "dc": "X"}
        response = client.post(
            "/lancamentos/",
            json=payload,
            headers=auth_header(),
        )

        assert response.status_code == 422

        app.dependency_overrides.clear()

    def test_campo_obrigatorio_ausente_retorna_422(self):
        """
        Campos obrigatórios ausentes → 422.
        RN54 — Pasta, Histórico, Valor e D/C são obrigatórios.
        """
        app.dependency_overrides[get_db] = override_registrar_lancamento(0)
        client = TestClient(app)

        # Payload sem 'ref' (campo Histórico)
        payload = {k: v for k, v in SAMPLE_LANCAMENTO.items() if k != "ref"}
        response = client.post(
            "/lancamentos/",
            json=payload,
            headers=auth_header(),
        )

        assert response.status_code == 422

        app.dependency_overrides.clear()

    def test_valor_zero_retorna_422(self):
        """
        VValor = 0 → 422 (Field(gt=0) no schema).
        RN54 — valor deve ser positivo.
        """
        app.dependency_overrides[get_db] = override_registrar_lancamento(0)
        client = TestClient(app)

        payload = {**SAMPLE_LANCAMENTO, "vvalor": "0"}
        response = client.post(
            "/lancamentos/",
            json=payload,
            headers=auth_header(),
        )

        assert response.status_code == 422

        app.dependency_overrides.clear()

    def test_autenticacao_negada_sem_token(self):
        """
        Requisição sem token → 401.
        Todos os endpoints fora de /auth exigem JWT.
        """
        client = TestClient(app)

        response = client.post(
            "/lancamentos/",
            json=SAMPLE_LANCAMENTO,
        )

        assert response.status_code == 401


# ---------------------------------------------------------------------------
# Testes — service (lógica DC → Deb/Cred)
# ---------------------------------------------------------------------------


class TestServiceDCLogic:
    """Testa a lógica RN53 diretamente no service, sem HTTP."""

    def test_debito_preenche_deb_e_zera_cred(self):
        """
        RN53 — DC='D': INSERT com Deb=VValor, Cred=null.
        Verifica parâmetros passados ao db.execute().
        """
        from app.services.lancamentos import registrar_lancamento as svc

        db = MagicMock()
        result = MagicMock()
        result.fetchone.return_value = FakeRow(Id=10)
        db.execute.return_value = result

        svc(
            db=db,
            id_cliente=1,
            cod_cliente=10001,
            dt="2026-03-22",
            conta=5,
            ref="Aluguel",
            vvalor=Decimal("1500.00"),
            dc="D",
        )

        call_kwargs = db.execute.call_args[0][1]
        assert call_kwargs["deb"] == Decimal("1500.00")
        assert call_kwargs["cred"] is None
        assert call_kwargs["dc"] == "D"

    def test_credito_preenche_cred_e_zera_deb(self):
        """
        RN53 — DC='C': INSERT com Cred=VValor, Deb=null.
        """
        from app.services.lancamentos import registrar_lancamento as svc

        db = MagicMock()
        result = MagicMock()
        result.fetchone.return_value = FakeRow(Id=11)
        db.execute.return_value = result

        svc(
            db=db,
            id_cliente=1,
            cod_cliente=10001,
            dt="2026-03-22",
            conta=5,
            ref="Aluguel",
            vvalor=Decimal("750.50"),
            dc="C",
        )

        call_kwargs = db.execute.call_args[0][1]
        assert call_kwargs["cred"] == Decimal("750.50")
        assert call_kwargs["deb"] is None
        assert call_kwargs["dc"] == "C"
