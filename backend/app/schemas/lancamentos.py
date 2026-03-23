"""
Schemas Pydantic — Lançamentos
Alinhados aos endpoints de lançamentos (POST /lancamentos, GET /lancamentos/verificar-pasta).
"""

from datetime import date
from decimal import Decimal
from typing import Literal

from pydantic import BaseModel, Field


class LancamentoCreate(BaseModel):
    """
    Request body para POST /lancamentos.
    RN53 — DC determina qual campo (Deb ou Cred) recebe VValor.
    RN54 — Todos os campos são obrigatórios.
    """

    id_cliente: int = Field(ge=1)
    cod_cliente: int = Field(ge=1)
    dt: date  # Data do lançamento
    conta: int = Field(ge=1)  # "Pasta" na UI — coluna Conta no banco
    ref: str = Field(min_length=1)  # "Histórico" — coluna Ref
    vvalor: Decimal = Field(gt=0)  # Valor do lançamento
    # RN52 — aceita apenas "D" (Débito) ou "C" (Crédito)
    dc: Literal["D", "C"]


class LancamentoCreateResponse(BaseModel):
    """Response do endpoint POST /lancamentos."""

    id: int
    mensagem: str


class VerificarPastaResponse(BaseModel):
    """
    Response do endpoint GET /lancamentos/verificar-pasta.
    RN51 — indica se a pasta já existe para o cliente.
    """

    existe: bool
