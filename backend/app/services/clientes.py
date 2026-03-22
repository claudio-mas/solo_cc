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
    row_id = db.execute(text("SELECT MAX(Id) AS max_id FROM Clientes")).fetchone()
    proximo_id = (row_id.max_id or 0) + 1

    # Encontrar primeiro gap >= 10000
    rows = db.execute(
        text("SELECT Código FROM Clientes WHERE Código >= 10000 ORDER BY Código ASC")
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


# ---------------------------------------------------------------------------
# frmAlterar — regras de negócio
# ---------------------------------------------------------------------------


def buscar_cliente(db: Session, id: int) -> dict:
    """
    RN33 — Carrega dados do cliente pelo Id.
    RN42 — Verifica se cliente possui lançamentos em Contas.

    Equivalente a ClientesTableAdapter.Fill() + BindingSource.Filter
    no frmAlterar_Load.
    """
    row = db.execute(
        text("SELECT Id, Código, Cliente FROM Clientes WHERE Id = :id"),
        {"id": id},
    ).fetchone()

    if row is None:
        raise HTTPException(
            status_code=status.HTTP_404_NOT_FOUND,
            detail="Cliente não encontrado",
        )

    total = db.execute(
        text("SELECT COUNT(*) AS total FROM Contas WHERE IdCliente = :id"),
        {"id": id},
    ).fetchone()

    return {
        "id": row.Id,
        "codigo": row.Código,
        "cliente": row.Cliente,
        "tem_lancamentos": (total.total or 0) > 0,
    }


def atualizar_cliente(
    db: Session,
    id: int,
    codigo: int,
    cliente: str,
) -> dict:
    """
    RN36 — Alteração de Código (confirmação de alto risco validada no frontend).
    RN37 — Alteração de Nome (confirmação simples validada no frontend).
    RN38 — Após salvar, frontend retorna campos a ReadOnly.
    RN47 — Nome convertido para maiúsculas.
    RN48 — Código deve ser único excluindo o próprio Id.

    Equivalente a TableAdapterManager.UpdateAll(SoloDataSet)
    no btnRibSalvar_Click.
    """
    # RN48 — unicidade de código excluindo o próprio registro
    existing = db.execute(
        text("SELECT Id FROM Clientes WHERE Código = :codigo AND Id <> :id"),
        {"codigo": codigo, "id": id},
    ).fetchone()

    if existing:
        raise HTTPException(
            status_code=status.HTTP_409_CONFLICT,
            detail="Código já existente",
        )

    # Verificar que o cliente existe
    current = db.execute(
        text("SELECT Id FROM Clientes WHERE Id = :id"),
        {"id": id},
    ).fetchone()

    if current is None:
        raise HTTPException(
            status_code=status.HTTP_404_NOT_FOUND,
            detail="Cliente não encontrado",
        )

    # RN47 — maiúsculas
    cliente_upper = cliente.upper()

    result = db.execute(
        text(
            "UPDATE Clientes "
            "SET Código = :codigo, Cliente = :cliente "
            "OUTPUT INSERTED.Id, INSERTED.Código, INSERTED.Cliente "
            "WHERE Id = :id"
        ),
        {"codigo": codigo, "cliente": cliente_upper, "id": id},
    ).fetchone()

    db.commit()

    return {
        "id": result.Id,
        "codigo": result.Código,
        "cliente": result.Cliente,
    }


def verificar_senha_exclusao(
    db: Session,
    id: int,
    senha: str,
) -> dict:
    """
    RN41 — Valida senha contra Chaves WHERE Ref='Exclusão de cliente'.
    RN42 — Verifica existência de lançamentos em Contas para o cliente.

    Equivalente a frmSenha.ShowDialog() com varSenha="2".
    ADAPTA a senha "4321" hardcoded para consulta à tabela Chaves (resolve D3).
    """
    chave_row = db.execute(
        text("SELECT Chave FROM Chaves WHERE Ref = 'Exclusão de cliente'"),
    ).fetchone()

    if chave_row is None:
        raise HTTPException(
            status_code=status.HTTP_500_INTERNAL_SERVER_ERROR,
            detail=("Configuração de senha de exclusão não encontrada na tabela Chaves"),
        )

    valido = senha == chave_row.Chave

    total = db.execute(
        text("SELECT COUNT(*) AS total FROM Contas WHERE IdCliente = :id"),
        {"id": id},
    ).fetchone()

    return {
        "valido": valido,
        "tem_lancamentos": (total.total or 0) > 0,
    }


def excluir_cliente(db: Session, id: int) -> None:
    """
    RN43 — Se tem lançamentos: DELETE Contas + DELETE Clientes.
    RN44 — DELETE Contas WHERE IdCliente = :id primeiro.
    RN45 — DELETE Clientes WHERE Id = :id (com ou sem lançamentos).

    Equivalente ao código ADODB comentado no btnExcluir_Click.
    Executado em transação para garantir atomicidade.
    """
    cliente = db.execute(
        text("SELECT Id FROM Clientes WHERE Id = :id"),
        {"id": id},
    ).fetchone()

    if cliente is None:
        raise HTTPException(
            status_code=status.HTTP_404_NOT_FOUND,
            detail="Cliente não encontrado",
        )

    # RN44 — excluir lançamentos primeiro (integridade referencial)
    db.execute(
        text("DELETE FROM Contas WHERE IdCliente = :id"),
        {"id": id},
    )

    # RN45 — excluir o cliente
    db.execute(
        text("DELETE FROM Clientes WHERE Id = :id"),
        {"id": id},
    )

    db.commit()
