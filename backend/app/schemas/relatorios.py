"""
Schemas Pydantic — Relatórios (frmReports / frmRptDevedores1 / frmCredores1)
"""

from datetime import date
from typing import Literal

from pydantic import BaseModel


class ClienteRelatorioItem(BaseModel):
    id: int
    cod_cliente: int
    cliente: str
    saldo: float


class RelatorioResponse(BaseModel):
    titulo: str
    data_corte: date
    saldo_minimo: float
    faixa_codigo: Literal["todos", "acima", "abaixo"]
    ordenacao: Literal["codigo", "nome"]
    itens: list[ClienteRelatorioItem]
