"""
Schemas Pydantic — Extrato (FrmExtrato)
Equivalente a: frmExtrato.vb (Solo Consultoria de Imóveis)

Regras referenciadas:
  RN78–RN101 (ver docs/FrmExtrato_documentacao.md)
"""

from datetime import date, datetime
from decimal import Decimal
from typing import Optional

from pydantic import BaseModel, Field

# ---------------------------------------------------------------------------
# Response — Lançamento com saldo acumulado
# ---------------------------------------------------------------------------


class LancamentoExtratoItem(BaseModel):
    """
    Representação de um lançamento no extrato, incluindo saldo acumulado.
    RN84 — saldo calculado via window function no banco.
    RN89 — campos Id, IdCliente, CodCliente, DC são retornados mas não editáveis.
    """

    id: int
    dt: datetime
    conta: int  # "Pasta" na UI
    nd: Optional[str] = None
    ref: Optional[str] = None  # "Histórico" na UI
    vvalor: Optional[Decimal] = None
    dc: Optional[str] = None
    deb: Optional[Decimal] = None
    cred: Optional[Decimal] = None
    saldo: Decimal  # RN84 — calculado pelo banco


class ExtratoResponse(BaseModel):
    """
    Response do endpoint GET /extrato/{id_cliente}.
    Inclui lista de lançamentos e saldo total.
    """

    lancamentos: list[LancamentoExtratoItem]
    saldo_total: Decimal  # RN85 — último saldo acumulado


# ---------------------------------------------------------------------------
# Request — Edição de lançamento (PATCH)
# ---------------------------------------------------------------------------


class LancamentoPatch(BaseModel):
    """
    Payload para PATCH /lancamentos/{id}.
    RN88 — apenas campos editáveis: Data, Pasta, ND, Histórico, Débito, Crédito.
    RN90 — cada edição salva imediatamente.
    Todos os campos são opcionais; apenas o enviado será atualizado.
    """

    dt: Optional[date] = Field(None, description="Data do lançamento")
    conta: Optional[int] = Field(None, ge=1, description="Pasta (coluna Conta)")
    nd: Optional[str] = Field(None, description="Número do documento")
    ref: Optional[str] = Field(None, description="Histórico (coluna Ref)")
    deb: Optional[Decimal] = Field(None, ge=0, description="Valor de débito")
    cred: Optional[Decimal] = Field(None, ge=0, description="Valor de crédito")


class LancamentoPatchResponse(BaseModel):
    """Response do PATCH /lancamentos/{id}."""

    ok: bool
    mensagem: str


# ---------------------------------------------------------------------------
# Request — Transferência de lançamentos
# ---------------------------------------------------------------------------


class TransferenciaRequest(BaseModel):
    """
    Payload para POST /extrato/transferir.
    RN93–RN98 — transfere lançamentos selecionados para outro cliente.
    """

    ids: list[int] = Field(min_length=1, description="Ids dos lançamentos a transferir")
    id_destino: int = Field(ge=1, description="Id do cliente destino")
    chave: str = Field(min_length=1, description="Senha de autorização")


class TransferenciaResponse(BaseModel):
    """Response do POST /extrato/transferir."""

    ok: bool
    mensagem: str
    transferidos: int


# ---------------------------------------------------------------------------
# Request — Desbloqueio de edição
# ---------------------------------------------------------------------------


class DesbloquearRequest(BaseModel):
    """
    Payload para POST /extrato/desbloquear.
    RN87 — valida senha contra Chaves (Ref = 'Desbloquear lançamentos').
    """

    chave: str = Field(min_length=1, description="Senha de autorização")


class DesbloquearResponse(BaseModel):
    """Response do POST /extrato/desbloquear."""

    ok: bool


# ---------------------------------------------------------------------------
# Response — Sincronização VValor
# ---------------------------------------------------------------------------


class SincronizarVValorResponse(BaseModel):
    """Response do POST /extrato/{id_cliente}/sincronizar-vvalor."""

    ok: bool
    mensagem: str


# ---------------------------------------------------------------------------
# Response — Clientes para combo de transferência
# ---------------------------------------------------------------------------


class ClienteDestinoItem(BaseModel):
    """Item da lista de clientes para o combo de transferência (RN96)."""

    id: int
    codigo: int
    cliente: str
