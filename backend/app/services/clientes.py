"""
Lógica de negócio — Clientes
Regras de negócio desacopladas dos routers.

Referências:
  - FrmPrincipal: RN09 (listagem)
  - frmClienteNovo: RN21-RN26 (criação de cliente)
"""

from fastapi import HTTPException, status
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


# ---------------------------------------------------------------------------
# frmClienteNovo — regras de negócio
# ---------------------------------------------------------------------------


def proximo_codigo(db: Session) -> dict:
    """
    RN23 — Gera o próximo código disponível >= 10000.

    Equivalente ao btnCodigo_Click no frmClienteNovo.vb:
      1. SELECT Id FROM [Clientes] ORDER BY Id DESC → próximo Id
      2. SELECT Código FROM [Clientes] WHERE Código >= 10000 ORDER BY Código
         → percorre de 10000 a 50000, encontra primeiro gap
    """
    # Próximo Id
    row_id = db.execute(
        text("SELECT MAX(Id) AS max_id FROM Clientes")
    ).fetchone()
    proximo_id = (row_id.max_id or 0) + 1

    # Encontrar primeiro gap >= 10000
    rows = db.execute(
        text(
            "SELECT Código FROM Clientes "
            "WHERE Código >= 10000 "
            "ORDER BY Código ASC"
        )
    ).fetchall()

    codigos = {row.Código for row in rows}
    proximo_codigo = 10000
    for i in range(10000, 50001):
        if i not in codigos:
            proximo_codigo = i
            break

    return {"proximo_codigo": proximo_codigo, "proximo_id": proximo_id}


def criar_cliente(db: Session, codigo: int, cliente: str) -> dict:
    """
    RN21, RN22, RN24, RN26 — Cria novo cliente com validações.

    Validações:
      - RN21: Código deve ser único
      - RN22: Código <= 20000 (validação primária no schema Pydantic)
      - RN24: Cliente não pode ser vazio (validação primária no schema)
      - RN26: Código obrigatório (validação primária no schema)
    """
    # RN22 — validação adicional no service (defesa em profundidade)
    if codigo > 20000:
        raise HTTPException(
            status_code=status.HTTP_422_UNPROCESSABLE_ENTITY,
            detail="Tamanho máximo de Código: 20.000",
        )

    # RN21 — unicidade de código
    existing = db.execute(
        text("SELECT Id FROM Clientes WHERE Código = :codigo"),
        {"codigo": codigo},
    ).fetchone()

    if existing:
        raise HTTPException(
            status_code=status.HTTP_409_CONFLICT,
            detail="Código já existente",
        )

    # RN25 — nome em maiúsculas
    cliente_upper = cliente.upper()

    # Inserir novo cliente
    result = db.execute(
        text(
            "INSERT INTO Clientes (Código, Cliente) "
            "OUTPUT INSERTED.Id, INSERTED.Código, INSERTED.Cliente "
            "VALUES (:codigo, :cliente)"
        ),
        {"codigo": codigo, "cliente": cliente_upper},
    ).fetchone()

    db.commit()

    return {
        "id": result.Id,
        "codigo": result.Código,
        "cliente": result.Cliente,
    }
