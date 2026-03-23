"""
Endpoints FastAPI — Lançamentos
Equivalente a: FrmLancaData.vb + FrmLanca.vb
(Solo Consultoria de Imóveis)
"""

from fastapi import APIRouter, Depends, Query

from app.dependencies import get_current_user, get_db
from app.schemas.lancamentos import (
    LancamentoCreate,
    LancamentoCreateResponse,
    VerificarPastaResponse,
)
from app.services.lancamentos import registrar_lancamento, verificar_pasta

router = APIRouter(prefix="/lancamentos", tags=["Lançamentos"])


@router.get(
    "/verificar-pasta",
    response_model=VerificarPastaResponse,
    summary="Verifica se pasta existe para o cliente",
    description=(
        "Consulta a tabela Contas para verificar se a pasta (Conta) informada "
        "já existe para o cliente. Retorna `existe=true` se há registros, "
        "`existe=false` se é uma pasta nova. "
        "RN51 — equivalente à query ADODB em txtPasta_Validated no FrmLanca.vb."
    ),
)
def verificar_pasta_endpoint(
    id_cliente: int = Query(..., ge=1, description="Id do cliente"),
    pasta: int = Query(..., ge=1, description="Número da pasta (coluna Conta)"),
    db=Depends(get_db),
    _current_user=Depends(get_current_user),
):
    # RN51 — verifica existência da pasta no banco
    existe = verificar_pasta(db, id_cliente=id_cliente, conta=pasta)
    return VerificarPastaResponse(existe=existe)


@router.post(
    "/",
    response_model=LancamentoCreateResponse,
    status_code=201,
    summary="Registra novo lançamento",
    description=(
        "Registra um novo lançamento de débito ou crédito na conta corrente "
        "do cliente informado. "
        "RN52 — DC aceita apenas 'D' ou 'C'. "
        "RN53 — Se DC='D': Deb=VValor, Cred=null; se DC='C': Cred=VValor, Deb=null. "
        "RN54 — Todos os campos são obrigatórios. "
        "Equivalente a btnRibSalvar_Click + TableAdapterManager.UpdateAll() no FrmLanca.vb."
    ),
)
def registrar_lancamento_endpoint(
    dados: LancamentoCreate,
    db=Depends(get_db),
    _current_user=Depends(get_current_user),
):
    # RN53/RN54 — lógica de DC→Deb/Cred e persistência no service
    novo_id = registrar_lancamento(
        db=db,
        id_cliente=dados.id_cliente,
        cod_cliente=dados.cod_cliente,
        dt=dados.dt,
        conta=dados.conta,
        ref=dados.ref,
        vvalor=dados.vvalor,
        dc=dados.dc,
    )
    return LancamentoCreateResponse(
        id=novo_id,
        mensagem="Lançamento registrado com sucesso.",
    )
