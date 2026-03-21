"""
Endpoints FastAPI — Clientes
Equivalente a: FrmPrincipal.vb e frmClienteNovo.vb (Solo Consultoria de Imóveis)
"""

from fastapi import APIRouter, Depends, HTTPException, status
from sqlalchemy.orm import Session

from app.dependencies import get_current_user, get_db
from app.schemas.clientes import (
    ClienteCreate,
    ClienteCreateResponse,
    ClientesResponse,
    ProximoCodigoResponse,
)
from app.services.clientes import criar_cliente, listar_clientes, proximo_codigo

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
