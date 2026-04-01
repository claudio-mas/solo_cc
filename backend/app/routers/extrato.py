"""
Endpoints FastAPI — Extrato (FrmExtrato)
Equivalente a: frmExtrato.vb (Solo Consultoria de Imóveis)

Regras implementadas:
  RN78 — Extrato filtrado por IdCliente
  RN79–RN83 — Filtros combinados (Pasta, ND, Sem ND, Histórico)
  RN84 — Saldo acumulado via window function
  RN85 — Saldo total retornado para exibição no rodapé
  RN86 — Recálculo do saldo ao aplicar filtros
  RN87 — Desbloqueio de edição via senha (Chaves → 'Desbloquear lançamentos')
  RN88–RN90 — Edição inline de campos específicos com save imediato
  RN93–RN98 — Transferência de lançamentos para outro cliente
  RN99 — Sincronização VValor ao sair da tela
  RN100 — Impressão: pendente decisão D1
  RN101 — Navegação: responsabilidade do frontend

Nota:
  O PATCH /lancamentos/{id} é registrado neste router (não no lancamentos.py)
  porque é funcionalidade exclusiva do extrato (edição inline desbloqueada).
"""

from typing import Optional

from fastapi import APIRouter, Depends, HTTPException, Query, status
from sqlalchemy.orm import Session

from app.dependencies import get_current_user, get_db
from app.schemas.extrato import (
    ClienteDestinoItem,
    DesbloquearRequest,
    DesbloquearResponse,
    ExtratoResponse,
    LancamentoPatch,
    LancamentoPatchResponse,
    SincronizarVValorResponse,
    TransferenciaRequest,
    TransferenciaResponse,
)
from app.services import extrato as svc

router = APIRouter(tags=["Extrato"])


# ---------------------------------------------------------------------------
# GET /extrato/{id_cliente} — lista lançamentos com saldo acumulado
# ---------------------------------------------------------------------------


@router.get(
    "/extrato/{id_cliente}",
    response_model=ExtratoResponse,
    summary="Retorna extrato do cliente com saldo acumulado",
    description=(
        "Lista todos os lançamentos do cliente com saldo acumulado calculado "
        "via window function SQL (RN84). Aceita filtros opcionais: pasta (RN79), "
        "nd (RN80), sem_nd (RN81) e hist (RN82). Filtros são combinados com AND (RN83). "
        "RN78 — apenas lançamentos do cliente informado. "
        "RN85 — saldo_total retornado para exibição no rodapé. "
        "RN86 — saldo recalculado a cada requisição com filtros. "
        "Equivalente à query CTE + ExtratoBindingSource.Filter no frmExtrato.vb."
    ),
)
def listar_extrato(
    id_cliente: int,
    pasta: Optional[int] = Query(None, ge=1, description="Filtro por Pasta (Conta) — RN79"),
    nd: Optional[str] = Query(None, description="Filtro por ND — RN80"),
    sem_nd: bool = Query(False, description="Checkbox Sem ND (ND IS NULL) — RN81"),
    hist: Optional[str] = Query(None, description="Filtro por Histórico (LIKE) — RN82"),
    db: Session = Depends(get_db),
    _user: dict = Depends(get_current_user),
):
    # RN78–RN86 — delegado ao service
    result = svc.listar_extrato(
        db, id_cliente=id_cliente, pasta=pasta, nd=nd, sem_nd=sem_nd, hist=hist
    )
    return result


# ---------------------------------------------------------------------------
# PATCH /lancamentos/{id} — edição inline de campo específico
# ---------------------------------------------------------------------------


@router.patch(
    "/lancamentos/{id_lancamento}",
    response_model=LancamentoPatchResponse,
    summary="Atualiza campo específico de um lançamento",
    description=(
        "Atualiza um ou mais campos editáveis de um lançamento (RN88): "
        "Data (Dt), Pasta (Conta), N.D. (ND), Histórico (Ref), Débito (Deb), Crédito (Cred). "
        "RN90 — cada edição salva imediatamente no banco. "
        "RN89 — campos Id, IdCliente, CodCliente, DC, Saldo nunca são editáveis. "
        "Equivalente ao evento CellEndEdit → fExecutaText(UPDATE) no frmExtrato.vb."
    ),
)
def atualizar_lancamento(
    id_lancamento: int,
    payload: LancamentoPatch,
    db: Session = Depends(get_db),
    _user: dict = Depends(get_current_user),
):
    # RN88/RN90 — atualização imediata
    ok = svc.atualizar_lancamento(
        db,
        id_lancamento=id_lancamento,
        dt=payload.dt,
        conta=payload.conta,
        nd=payload.nd,
        ref=payload.ref,
        deb=payload.deb,
        cred=payload.cred,
    )

    if not ok:
        raise HTTPException(
            status_code=status.HTTP_404_NOT_FOUND,
            detail=f"Lançamento id={id_lancamento} não encontrado.",
        )

    return LancamentoPatchResponse(ok=True, mensagem="Lançamento atualizado com sucesso.")


# ---------------------------------------------------------------------------
# POST /extrato/desbloquear — valida senha de desbloqueio
# ---------------------------------------------------------------------------


@router.post(
    "/extrato/desbloquear",
    response_model=DesbloquearResponse,
    summary="Valida senha para desbloqueio de edição",
    description=(
        "RN87 — valida a senha fornecida contra a tabela Chaves "
        "(Ref = 'Desbloquear lançamentos'). "
        "Equivalente ao F10 → frmSenha (varSenha=4) no frmExtrato.vb. "
        "Retorna {ok: true} se válida; {ok: false} se inválida."
    ),
)
def desbloquear_edicao(
    payload: DesbloquearRequest,
    db: Session = Depends(get_db),
    _user: dict = Depends(get_current_user),
):
    ok = svc.desbloquear_edicao(db, chave=payload.chave)
    return DesbloquearResponse(ok=ok)


# ---------------------------------------------------------------------------
# POST /extrato/transferir — transfere lançamentos para outro cliente
# ---------------------------------------------------------------------------


@router.post(
    "/extrato/transferir",
    response_model=TransferenciaResponse,
    summary="Transfere lançamentos selecionados para outro cliente",
    description=(
        "RN93 — transfere lançamentos selecionados para outro cliente. "
        "RN94 — valida senha contra Chaves (Ref = 'Transferência de lançamentos'). "
        "RN95 — requer ao menos um lançamento selecionado. "
        "RN96 — cliente destino não pode ser o cliente atual. "
        "RN97 — UPDATE IdCliente e CodCliente nos registros selecionados. "
        "RN98 — frontend deve recarregar extrato após sucesso. "
        "Equivalente ao btnTransfere_Click → frmSenha (varSenha=3) no frmExtrato.vb."
    ),
)
def transferir_lancamentos(
    payload: TransferenciaRequest,
    id_cliente_origem: int = Query(
        ..., ge=1, description="Id do cliente de origem (cujo extrato está aberto)"
    ),
    db: Session = Depends(get_db),
    _user: dict = Depends(get_current_user),
):
    try:
        result = svc.transferir_lancamentos(
            db,
            ids=payload.ids,
            id_cliente_origem=id_cliente_origem,
            id_destino=payload.id_destino,
            chave=payload.chave,
        )
        return result
    except PermissionError as exc:
        raise HTTPException(
            status_code=status.HTTP_403_FORBIDDEN,
            detail=str(exc),
        )
    except ValueError as exc:
        raise HTTPException(
            status_code=status.HTTP_400_BAD_REQUEST,
            detail=str(exc),
        )


# ---------------------------------------------------------------------------
# POST /extrato/{id_cliente}/sincronizar-vvalor
# ---------------------------------------------------------------------------


@router.post(
    "/extrato/{id_cliente}/sincronizar-vvalor",
    response_model=SincronizarVValorResponse,
    summary="Sincroniza VValor com Deb/Cred",
    description=(
        "RN99 — chamado ao sair da tela de extrato. "
        "Executa UPDATE VValor = Deb WHERE Deb > 0 e "
        "UPDATE VValor = Cred WHERE Cred > 0 para o cliente. "
        "Equivalente ao evento FrmExtrato_Closed no frmExtrato.vb."
    ),
)
def sincronizar_vvalor(
    id_cliente: int,
    db: Session = Depends(get_db),
    _user: dict = Depends(get_current_user),
):
    svc.sincronizar_vvalor(db, id_cliente=id_cliente)
    return SincronizarVValorResponse(ok=True, mensagem="VValor sincronizado com sucesso.")


# ---------------------------------------------------------------------------
# GET /extrato/{id_cliente}/clientes-destino — lista clientes para transferência
# ---------------------------------------------------------------------------


@router.get(
    "/extrato/{id_cliente}/clientes-destino",
    response_model=list[ClienteDestinoItem],
    summary="Lista clientes disponíveis para transferência",
    description=(
        "RN96 — retorna todos os clientes exceto o cliente atual, "
        "para popular o combo de transferência de lançamentos. "
        "Equivalente ao ClientesBindingSource com filtro [Id] <> id_atual "
        "no frmExtrato.vb."
    ),
)
def listar_clientes_destino(
    id_cliente: int,
    db: Session = Depends(get_db),
    _user: dict = Depends(get_current_user),
):
    return svc.listar_clientes_destino(db, id_cliente_atual=id_cliente)
