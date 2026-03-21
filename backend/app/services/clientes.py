"""
Lógica de negócio — Clientes (FrmPrincipal)
Regras de negócio desacopladas dos routers.
"""

from sqlalchemy import text
from sqlalchemy.orm import Session


def listar_clientes(db: Session) -> list[dict]:
    """
    RN09 — Retorna todos os clientes ordenados por nome.
    Equivalente a ClientesTableAdapter.Fill() + BindingSource.Sort = "Cliente"
    no FrmPrincipal_Load.
    """
    result = db.execute(
        text("SELECT Id, Código, Cliente FROM Clientes ORDER BY Cliente ASC")
    ).fetchall()

    return [
        {
            "id": row.Id,
            "codigo": row.Código,
            "cliente": row.Cliente,
        }
        for row in result
    ]
