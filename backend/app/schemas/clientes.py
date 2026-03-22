"""
Schemas Pydantic — Clientes
Alinhados aos endpoints de clientes (GET, POST).
"""

from pydantic import BaseModel, Field


class ClienteListItem(BaseModel):
    """
    Item da lista de clientes exibida no grid da tela principal.
    Equivalente às colunas do RadGridView1 no FrmPrincipal.
    """

    id: int  # Coluna oculta no grid — identificador interno
    codigo: int  # Coluna "Código" — exibida no grid e no header dinâmico
    cliente: str  # Coluna "Cliente" — nome do cliente, ordenação padrão


class ClientesResponse(BaseModel):
    """Response do endpoint GET /clientes."""

    clientes: list[ClienteListItem]


# ---------------------------------------------------------------------------
# frmClienteNovo — schemas para criação de cliente
# ---------------------------------------------------------------------------


class ProximoCodigoResponse(BaseModel):
    """Response do endpoint GET /clientes/proximo-codigo. RN23."""

    proximo_codigo: int  # Primeiro código disponível >= 10000
    proximo_id: int  # Próximo Id (maior existente + 1)


class ClienteCreate(BaseModel):
    """Request body para POST /clientes. RN21, RN22, RN24, RN26."""

    # RN22 — código entre 10000 e 20000
    codigo: int = Field(ge=10000, le=20000)
    cliente: str = Field(min_length=1)  # RN24 — nome obrigatório


class ClienteCreateResponse(BaseModel):
    """Response do endpoint POST /clientes."""

    id: int
    codigo: int
    cliente: str


# ---------------------------------------------------------------------------
# frmAlterar — schemas para busca, atualização e exclusão de cliente
# ---------------------------------------------------------------------------


class ClienteDetail(BaseModel):
    """Response do endpoint GET /clientes/{id}. RN33, RN42."""

    id: int
    codigo: int
    cliente: str
    # RN42 — indica se há lançamentos em Contas para o cliente
    tem_lancamentos: bool


class ClienteUpdate(BaseModel):
    """Request body para PUT /clientes/{id}. RN36, RN37, RN47, RN48."""

    codigo: int = Field(ge=1)  # RN48 — unicidade validada no service
    # RN47 — convertido para maiúsculas no service
    cliente: str = Field(min_length=1)


class ClienteUpdateResponse(BaseModel):
    """Response do endpoint PUT /clientes/{id}."""

    id: int
    codigo: int
    cliente: str


class VerificarSenhaRequest(BaseModel):
    """Request body para POST /clientes/{id}/verificar-senha. RN41."""

    senha: str = Field(min_length=1)


class VerificarSenhaResponse(BaseModel):
    """Response do endpoint POST /clientes/{id}/verificar-senha. RN41, RN42."""

    # RN41 — True se senha confere com Chaves WHERE Ref='Exclusão de cliente'
    valido: bool
    # RN42 — True se cliente tem registros em Contas
    tem_lancamentos: bool
