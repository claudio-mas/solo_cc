"""
Schemas Pydantic — Clientes (FrmPrincipal)
Alinhados ao response do endpoint GET /clientes.
"""

from pydantic import BaseModel


class ClienteListItem(BaseModel):
    """
    Item da lista de clientes exibida no grid da tela principal.
    Equivalente às colunas do RadGridView1 no FrmPrincipal.
    """

    id: int  # Coluna oculta no grid — identificador interno
    codigo: int  # Coluna "Código" — exibida no grid e no header dinâmico
    cliente: str  # Coluna "Cliente" — nome do cliente, ordenação padrão


class ClientesResponse(BaseModel):
    """Response do endpoint GET /clientes."""

    clientes: list[ClienteListItem]
