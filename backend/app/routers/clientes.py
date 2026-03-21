"""
Endpoints FastAPI — Clientes (FrmPrincipal)
Equivalente a: FrmPrincipal.vb (Solo Consultoria de Imóveis)
"""

from fastapi import APIRouter, Depends, HTTPException, status
from sqlalchemy.orm import Session

from app.dependencies import get_current_user, get_db
from app.schemas.clientes import ClientesResponse
from app.services.clientes import listar_clientes

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
