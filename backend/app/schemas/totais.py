"""
Schemas Pydantic — Totais
Equivalente a: frmTotais2.vb (Solo Consultoria de Imóveis)
"""

from decimal import Decimal

from pydantic import BaseModel


class TotaisResponse(BaseModel):
    """
    Resposta do endpoint GET /totais.

    Contém os 4 valores calculados pela query CTE em services/totais.py:
      - RN63 — qtde_credores: quantidade de clientes com TC > TD
      - RN64 — valor_credores: somatório (TC − TD) para clientes credores
      - RN65 — qtde_devedores: quantidade de clientes com TD > TC
      - RN66 — valor_devedores: somatório (TD − TC) para clientes devedores
    """

    qtde_credores: int
    valor_credores: Decimal
    qtde_devedores: int
    valor_devedores: Decimal
