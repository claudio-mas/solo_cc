"""
Endpoints FastAPI — Relatórios (frmReports / frmRptDevedores1 / frmCredores1)
Equivalente a: frmReports.vb + frmRptDevedores1.vb + frmCredores1.vb

Regras implementadas:
  RN102 — Tipo de relatório (devedores/credores) selecionado no endpoint chamado
  RN103 — Filtro por faixa de código (todos / acima >= 10000 / abaixo < 10000)
  RN104 — Data de corte: lançamentos até Dt <= :data_corte
  RN105 — Saldo mínimo: retorna apenas clientes com saldo > saldo_minimo
  RN106 — Ordenação por código ou por nome do cliente
  RN108–RN113 — Devedores: query, filtros, título e coloração
  RN114–RN119 — Credores: query, filtros, título e coloração

Nota: FrmExtratoRpt (RN120–RN124) usa o endpoint existente GET /extrato/{id_cliente}.
Não há endpoint duplicado aqui.
"""

from datetime import date
from typing import Literal

from fastapi import APIRouter, Depends, Query
from sqlalchemy.orm import Session

from app.dependencies import get_current_user, get_db
from app.schemas.relatorios import RelatorioResponse
from app.services import relatorios as svc

router = APIRouter(prefix="/relatorios", tags=["Relatórios"])


@router.get(
    "/devedores",
    response_model=RelatorioResponse,
    summary="Lista clientes devedores com saldo e filtros",
    description=(
        "RN108 — retorna clientes com SUM(Deb) > SUM(Cred) até a data de corte. "
        "RN109 — filtro por faixa de código (todos/acima/abaixo de 10000). "
        "RN110 — filtro por saldo mínimo: saldo devedor > saldo_minimo. "
        "RN111 — ordenação por código ou nome. "
        "RN112 — título dinâmico retornado com data e saldo mínimo. "
        "Equivalente à query de frmRptDevedores1_Load no legado."
    ),
)
def listar_devedores(
    data_corte: date = Query(default_factory=date.today, description="Data de corte — RN104"),
    saldo_minimo: float = Query(default=0, ge=0, description="Saldo mínimo devedor — RN105"),
    faixa_codigo: Literal["todos", "acima", "abaixo"] = Query(
        default="todos",
        description="Faixa de código (todos / acima=>=10000 / abaixo=<10000) — RN103",
    ),
    ordenacao: Literal["codigo", "nome"] = Query(
        default="codigo",
        description="Ordenação por código ou por nome — RN106",
    ),
    db: Session = Depends(get_db),
    _user: dict = Depends(get_current_user),
) -> RelatorioResponse:
    return svc.listar_devedores(
        db,
        data_corte=data_corte,
        saldo_minimo=saldo_minimo,
        faixa_codigo=faixa_codigo,
        ordenacao=ordenacao,
    )


@router.get(
    "/credores",
    response_model=RelatorioResponse,
    summary="Lista clientes credores com saldo e filtros",
    description=(
        "RN114 — retorna clientes com SUM(Cred) > SUM(Deb) até a data de corte. "
        "RN115 — filtro por faixa de código (todos/acima/abaixo de 10000). "
        "RN116 — filtro por saldo mínimo: saldo credor > saldo_minimo. "
        "RN117 — ordenação por código ou nome. "
        "RN118 — título dinâmico retornado com data e saldo mínimo. "
        "Equivalente à query de frmCredores1_Load no legado."
    ),
)
def listar_credores(
    data_corte: date = Query(default_factory=date.today, description="Data de corte — RN104"),
    saldo_minimo: float = Query(default=0, ge=0, description="Saldo mínimo credor — RN105"),
    faixa_codigo: Literal["todos", "acima", "abaixo"] = Query(
        default="todos",
        description="Faixa de código (todos / acima=>=10000 / abaixo=<10000) — RN103",
    ),
    ordenacao: Literal["codigo", "nome"] = Query(
        default="codigo",
        description="Ordenação por código ou por nome — RN106",
    ),
    db: Session = Depends(get_db),
    _user: dict = Depends(get_current_user),
) -> RelatorioResponse:
    return svc.listar_credores(
        db,
        data_corte=data_corte,
        saldo_minimo=saldo_minimo,
        faixa_codigo=faixa_codigo,
        ordenacao=ordenacao,
    )
