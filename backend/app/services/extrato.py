"""
Lógica de negócio — Extrato (FrmExtrato)
Regras de negócio desacopladas dos routers.

Referências:
  - FrmExtrato: RN78–RN101 (ver docs/FrmExtrato_documentacao.md)
  - D13: saldo via window function
  - D14: edição inline com onBlur → PATCH
"""

from datetime import date
from decimal import Decimal
from typing import Optional

from sqlalchemy import text
from sqlalchemy.orm import Session

from app.services.usuarios import verificar_senha_operacao

# ---------------------------------------------------------------------------
# Listar lançamentos com saldo acumulado
# ---------------------------------------------------------------------------


def listar_extrato(
    db: Session,
    id_cliente: int,
    pasta: Optional[int] = None,
    nd: Optional[str] = None,
    sem_nd: bool = False,
    hist: Optional[str] = None,
) -> dict:
    """
    RN78 — extrato filtrado por IdCliente.
    RN79–RN83 — filtros combinados (AND).
    RN84 — saldo acumulado via window function.
    RN85 — saldo total = último valor de saldo acumulado.
    RN86 — recálculo automático ao aplicar filtros.

    Retorna dict com 'lancamentos' (lista de dicts) e 'saldo_total' (Decimal).
    """
    # Monta WHERE dinâmico com parâmetros nomeados
    conditions = ["IdCliente = :id_cliente"]
    params: dict = {"id_cliente": id_cliente}

    if pasta is not None:
        conditions.append("Conta = :pasta")
        params["pasta"] = pasta

    if sem_nd:
        # RN81 — checkbox "Sem ND"
        conditions.append("ND IS NULL")
    elif nd is not None:
        # RN80 — filtro por ND
        conditions.append("ND = :nd")
        params["nd"] = nd

    if hist is not None and hist.strip():
        # RN82 — filtro por Histórico (LIKE)
        conditions.append("Ref LIKE '%' + :hist + '%'")
        params["hist"] = hist.strip()

    where_clause = " AND ".join(conditions)

    # RN84 — window function para saldo acumulado (D13)
    query = text(
        f"SELECT Dt, Id, Conta, ND, Ref, VValor, DC, Deb, Cred, "  # noqa: S608
        f"SUM(ISNULL(Cred, 0) - ISNULL(Deb, 0)) OVER (ORDER BY Dt, Id) AS Saldo "
        f"FROM Contas "
        f"WHERE {where_clause} "
        f"ORDER BY Dt, Id"
    )

    rows = db.execute(query, params).fetchall()

    lancamentos = []
    saldo_total = Decimal("0")

    for row in rows:
        saldo_total = Decimal(str(row.Saldo)) if row.Saldo is not None else Decimal("0")
        lancamentos.append({
            "id": row.Id,
            "dt": row.Dt,
            "conta": row.Conta,
            "nd": row.ND,
            "ref": row.Ref,
            "vvalor": row.VValor,
            "dc": row.DC,
            "deb": row.Deb,
            "cred": row.Cred,
            "saldo": saldo_total,
        })

    return {"lancamentos": lancamentos, "saldo_total": saldo_total}


# ---------------------------------------------------------------------------
# Edição de lançamento (PATCH)
# ---------------------------------------------------------------------------

# RN88 — campos editáveis (após desbloqueio)
CAMPOS_EDITAVEIS = {
    "dt": "Dt",
    "conta": "Conta",
    "nd": "ND",
    "ref": "Ref",
    "deb": "Deb",
    "cred": "Cred",
}


def atualizar_lancamento(
    db: Session,
    id_lancamento: int,
    dt: Optional[date] = None,
    conta: Optional[int] = None,
    nd: Optional[str] = None,
    ref: Optional[str] = None,
    deb: Optional[Decimal] = None,
    cred: Optional[Decimal] = None,
) -> bool:
    """
    RN90 — atualiza um campo específico de um lançamento.

    Equivalente ao CellEndEdit do frmExtrato.vb que executa
    UPDATE Contas SET <campo> = <valor> WHERE Id = <id>.

    Retorna True se atualizou, False se lançamento não encontrado.
    """
    # Verifica se o lançamento existe
    exists = db.execute(
        text("SELECT Id FROM Contas WHERE Id = :id"),
        {"id": id_lancamento},
    ).fetchone()

    if exists is None:
        return False

    # Monta SET dinâmico com os campos enviados
    campos = {}
    if dt is not None:
        campos["Dt"] = dt
    if conta is not None:
        campos["Conta"] = conta
    if nd is not None:
        campos["ND"] = nd
    if ref is not None:
        campos["Ref"] = ref
    if deb is not None:
        campos["Deb"] = deb
    if cred is not None:
        campos["Cred"] = cred

    if not campos:
        return True  # nada a atualizar

    set_clause = ", ".join(f"{col} = :{col}" for col in campos)
    params = {**campos, "id": id_lancamento}

    db.execute(
        text(f"UPDATE Contas SET {set_clause} WHERE Id = :id"),  # noqa: S608
        params,
    )
    db.commit()
    return True


# ---------------------------------------------------------------------------
# Desbloqueio de edição
# ---------------------------------------------------------------------------


def desbloquear_edicao(db: Session, chave: str) -> bool:
    """
    RN87 — valida senha contra Chaves (Ref = 'Desbloquear lançamentos').
    Reutiliza verificar_senha_operacao do service de usuários.
    """
    return verificar_senha_operacao(db, ref="Desbloquear lançamentos", chave=chave)


# ---------------------------------------------------------------------------
# Transferência de lançamentos
# ---------------------------------------------------------------------------


def transferir_lancamentos(
    db: Session,
    ids: list[int],
    id_cliente_origem: int,
    id_destino: int,
    chave: str,
) -> dict:
    """
    RN93–RN98 — transfere lançamentos selecionados para outro cliente.

    Validações:
      RN94 — senha contra Chaves (Ref = 'Transferência de lançamentos')
      RN95 — ao menos um lançamento selecionado (validado pelo schema)
      RN96 — cliente destino diferente do atual (validado aqui)
      RN97 — UPDATE IdCliente e CodCliente

    Retorna dict com 'ok', 'mensagem' e 'transferidos'.

    Raises:
        ValueError: se destino = origem ou destino não encontrado.
        PermissionError: se senha inválida.
    """
    # RN96 — destino não pode ser o mesmo cliente
    if id_destino == id_cliente_origem:
        raise ValueError("Cliente destino não pode ser o mesmo cliente atual.")

    # RN94 — valida senha
    if not verificar_senha_operacao(db, ref="Transferência de lançamentos", chave=chave):
        raise PermissionError("Senha incorreta.")

    # Busca Código do cliente destino
    destino = db.execute(
        text("SELECT Id, Código FROM Clientes WHERE Id = :id"),
        {"id": id_destino},
    ).fetchone()

    if destino is None:
        raise ValueError(f"Cliente destino id={id_destino} não encontrado.")

    cod_destino = destino.Código

    # RN97 — UPDATE IdCliente e CodCliente nos registros selecionados
    # Usa parâmetros para os ids
    placeholders = ", ".join(f":id_{i}" for i in range(len(ids)))
    id_params = {f"id_{i}": id_val for i, id_val in enumerate(ids)}
    id_params["id_destino"] = id_destino
    id_params["cod_destino"] = cod_destino

    result = db.execute(
        text(
            f"UPDATE Contas SET IdCliente = :id_destino, CodCliente = :cod_destino "  # noqa: S608
            f"WHERE Id IN ({placeholders})"
        ),
        id_params,
    )

    db.commit()
    transferidos = result.rowcount

    return {
        "ok": True,
        "mensagem": f"{transferidos} lançamento(s) transferido(s) com sucesso.",
        "transferidos": transferidos,
    }


# ---------------------------------------------------------------------------
# Sincronização VValor
# ---------------------------------------------------------------------------


def sincronizar_vvalor(db: Session, id_cliente: int) -> None:
    """
    RN99 — ao sair da tela, sincroniza VValor com Deb/Cred.

    Equivalente ao FrmExtrato_Closed:
      UPDATE Contas SET VValor = Deb WHERE IdCliente = :id AND Deb > 0
      UPDATE Contas SET VValor = Cred WHERE IdCliente = :id AND Cred > 0
    """
    db.execute(
        text("UPDATE Contas SET VValor = Deb WHERE IdCliente = :id AND Deb > 0"),
        {"id": id_cliente},
    )
    db.execute(
        text("UPDATE Contas SET VValor = Cred WHERE IdCliente = :id AND Cred > 0"),
        {"id": id_cliente},
    )
    db.commit()


# ---------------------------------------------------------------------------
# Clientes para combo de transferência
# ---------------------------------------------------------------------------


def listar_clientes_destino(db: Session, id_cliente_atual: int) -> list[dict]:
    """
    RN96 — lista clientes excluindo o atual, para o combo de transferência.
    """
    rows = db.execute(
        text(
            "SELECT Id, Código, Cliente FROM Clientes "
            "WHERE Id <> :id ORDER BY Cliente"
        ),
        {"id": id_cliente_atual},
    ).fetchall()

    return [{"id": row.Id, "codigo": row.Código, "cliente": row.Cliente} for row in rows]
