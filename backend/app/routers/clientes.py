"""
Endpoints FastAPI — Clientes
Equivalente a: FrmPrincipal.vb, frmClienteNovo.vb e frmAlterar.vb
(Solo Consultoria de Imóveis)
"""

from fastapi import APIRouter, Depends, HTTPException, status
from sqlalchemy.orm import Session

from app.dependencies import get_current_user, get_db
from app.schemas.clientes import (
    ClienteCreate,
    ClienteCreateResponse,
    ClienteDetail,
    ClientesResponse,
    ClienteUpdate,
    ClienteUpdateResponse,
    ProximoCodigoResponse,
    VerificarSenhaRequest,
    VerificarSenhaResponse,
)
from app.services.clientes import (
    atualizar_cliente,
    buscar_cliente,
    criar_cliente,
    excluir_cliente,
    listar_clientes,
    proximo_codigo,
    verificar_senha_exclusao,
)

router = APIRouter(prefix="/clientes", tags=["Clientes"])


@router.get(
    "/",
    response_model=ClientesResponse,
    summary="Lista todos os clientes",
    description=(
        "Retorna a lista completa de clientes ordenada por nome. "
        "Equivalente ao ClientesTableAdapter.Fill() no "
        "FrmPrincipal_Load."
    ),
)
async def get_clientes(
    db: Session = Depends(get_db),
    user: dict = Depends(get_current_user),
):
    """
    RN09 — Grid de clientes carregado do banco, ordenado por nome.
    RN10 — O primeiro item da lista fornece o código inicial
           exibido no header da seção de ações.
    """
    try:
        clientes = listar_clientes(db)
        return ClientesResponse(clientes=clientes)
    except Exception as e:
        raise HTTPException(
            status_code=status.HTTP_500_INTERNAL_SERVER_ERROR,
            detail=f"Erro ao carregar clientes: {str(e)}",
        )


# ---------------------------------------------------------------------------
# frmClienteNovo — endpoints de criação
# ---------------------------------------------------------------------------


@router.get(
    "/proximo-codigo",
    response_model=ProximoCodigoResponse,
    summary="Próximo código disponível",
    description=(
        "Retorna o primeiro código disponível >= 10.000 e o próximo Id "
        "para criação de novo cliente. Equivalente ao btnCodigo_Click no "
        "frmClienteNovo."
    ),
)
async def get_proximo_codigo(
    db: Session = Depends(get_db),
    user: dict = Depends(get_current_user),
):
    """
    RN23 — Geração automática: primeiro inteiro >= 10.000 não utilizado.
    RN27 — Retorna também o próximo Id disponível.
    """
    return proximo_codigo(db)


@router.post(
    "/",
    response_model=ClienteCreateResponse,
    status_code=status.HTTP_201_CREATED,
    summary="Cria novo cliente",
    description=(
        "Cria um novo cliente com validações de código único, "
        "limite de 20.000 e campos obrigatórios. Equivalente ao "
        "btnRibSalvar_Click no frmClienteNovo."
    ),
)
async def post_cliente(
    body: ClienteCreate,
    db: Session = Depends(get_db),
    user: dict = Depends(get_current_user),
):
    """
    RN21 — Código deve ser único na tabela Clientes.
    RN22 — Código máximo: 20.000.
    RN24 — Campo Cliente obrigatório.
    RN25 — Nome convertido para maiúsculas.
    RN26 — Campo Código obrigatório.
    RN29 — Código duplicado: retorna 409 "Código já existente".
    RN30 — Código > 20.000: retorna 422.
    """
    return criar_cliente(db, codigo=body.codigo, cliente=body.cliente)


# ---------------------------------------------------------------------------
# frmAlterar — endpoints de edição e exclusão
# ---------------------------------------------------------------------------


@router.get(
    "/{id}",
    response_model=ClienteDetail,
    summary="Busca cliente por Id",
    description=(
        "Retorna dados de um cliente específico e indica se possui "
        "lançamentos em Contas. Equivalente ao frmAlterar_Load "
        "(Fill + Filter por Id)."
    ),
)
async def get_cliente(
    id: int,
    db: Session = Depends(get_db),
    user: dict = Depends(get_current_user),
):
    """
    RN33 — Carrega dados do cliente pelo Id.
    RN42 — Verifica existência de lançamentos (tem_lancamentos).
    """
    return buscar_cliente(db, id)


@router.put(
    "/{id}",
    response_model=ClienteUpdateResponse,
    summary="Atualiza cliente",
    description=(
        "Atualiza Código e/ou Nome do cliente. Validações: código "
        "único (excluindo o próprio Id), nome em maiúsculas. "
        "Equivalente a TableAdapterManager.UpdateAll() no "
        "btnRibSalvar_Click do frmAlterar."
    ),
)
async def put_cliente(
    id: int,
    body: ClienteUpdate,
    db: Session = Depends(get_db),
    user: dict = Depends(get_current_user),
):
    """
    RN36 — Alteração de Código (confirmação de alto risco no frontend).
    RN37 — Alteração de Nome (confirmação simples no frontend).
    RN38 — Após salvar, frontend retorna campos a ReadOnly.
    RN47 — Nome convertido para maiúsculas.
    RN48 — Código deve ser único excluindo o próprio Id.
    """
    return atualizar_cliente(db, id=id, codigo=body.codigo, cliente=body.cliente)


@router.post(
    "/{id}/verificar-senha",
    response_model=VerificarSenhaResponse,
    summary="Verifica senha de exclusão",
    description=(
        "Valida a senha fornecida contra a tabela Chaves "
        "(Ref='Exclusão de cliente'). Retorna também se o cliente "
        "possui lançamentos, para o frontend decidir se exibe "
        "confirmação adicional. Equivalente a frmSenha (varSenha=2)."
    ),
)
async def post_verificar_senha(
    id: int,
    body: VerificarSenhaRequest,
    db: Session = Depends(get_db),
    user: dict = Depends(get_current_user),
):
    """
    RN41 — Senha validada contra Chaves WHERE Ref='Exclusão de cliente'.
    RN42 — Retorna tem_lancamentos para controle de fluxo no frontend.
    """
    return verificar_senha_exclusao(db, id=id, senha=body.senha)


@router.delete(
    "/{id}",
    status_code=status.HTTP_204_NO_CONTENT,
    summary="Exclui cliente",
    description=(
        "Exclui o cliente e todos os seus lançamentos em Contas "
        "(se houver), em transação. Equivalente ao código ADODB "
        "comentado no btnExcluir_Click do frmAlterar."
    ),
)
async def delete_cliente(
    id: int,
    db: Session = Depends(get_db),
    user: dict = Depends(get_current_user),
):
    """
    RN43 — Confirmação adicional (se tem lançamentos) validada no frontend.
    RN44 — DELETE Contas + DELETE Clientes em transação.
    RN45 — DELETE Clientes apenas se sem lançamentos.
    RN46 — Frontend invalida cache e navega para /principal após sucesso.
    """
    excluir_cliente(db, id=id)
