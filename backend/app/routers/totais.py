"""
Endpoints FastAPI — Totais
Equivalente a: frmTotais2.vb
(Solo Consultoria de Imóveis)
"""

from datetime import date

from fastapi import APIRouter, Depends

from app.dependencies import get_current_user, get_db
from app.schemas.totais import TotaisResponse
from app.services.totais import buscar_totais

router = APIRouter(prefix="/totais", tags=["Totais"])


@router.get(
    "",
    response_model=TotaisResponse,
    summary="Retorna totais de clientes credores e devedores",
    description=(
        "Calcula os 4 totais de clientes até a data informada (inclusive): "
        "quantidade e valor total de credores (TC > TD) e devedores (TD > TC). "
        "RN59 — se 'data' não for informada, usa a data de hoje. "
        "RN60 — cada chamada retorna os totais para a data recebida. "
        "RN61 — credores: clientes onde TC > TD acumulado até a data. "
        "RN62 — devedores: clientes onde TD > TC acumulado até a data. "
        "RN63 — qtde_credores: COUNT(CodCliente) WHERE TC > TD. "
        "RN64 — valor_credores: SUM(TC − TD) WHERE TC > TD. "
        "RN65 — qtde_devedores: COUNT(CodCliente) WHERE TD > TC. "
        "RN66 — valor_devedores: SUM(TD − TC) WHERE TD > TC. "
        "RN67 — filtro: WHERE Dt <= data (acumulado desde o início até a data). "
        "Equivalente a FrmTotais_Load + Dt_ValueChanged no frmTotais2.vb."
    ),
)
def get_totais(
    data: date | None = None,
    db=Depends(get_db),
    _current_user=Depends(get_current_user),
):
    # RN59 — data padrão é hoje (equivalente a Date.Today no legado)
    data_consulta = data or date.today()

    # RN61–RN67 — lógica de credores/devedores no service
    totais = buscar_totais(db, data_consulta)

    return TotaisResponse(**totais)
