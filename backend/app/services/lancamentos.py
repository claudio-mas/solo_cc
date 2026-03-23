"""
Lógica de negócio — Lançamentos
Regras de negócio desacopladas dos routers.

Referências:
  - FrmLancaData: RN49, RN50 (seleção de data e passagem de contexto)
  - FrmLanca: RN51 (verificação de pasta), RN52-RN54 (validação e save),
              RN53 (DC → Deb/Cred)
"""

from datetime import date
from decimal import Decimal

from sqlalchemy import text
from sqlalchemy.orm import Session


def verificar_pasta(db: Session, id_cliente: int, conta: int) -> bool:
    """
    RN51 — Verifica se a pasta (Conta) já existe para o cliente informado.

    Equivalente à query ADODB em txtPasta_Validated no FrmLanca.vb:
      SELECT * FROM Contas WHERE Conta = c AND IdCliente = i

    Retorna True se a pasta existe, False se é nova.
    """
    result = db.execute(
        text(
            "SELECT COUNT(*) AS total "
            "FROM Contas "
            "WHERE Conta = :conta AND IdCliente = :id_cliente"
        ),
        {"conta": conta, "id_cliente": id_cliente},
    ).fetchone()

    return result.total > 0


def registrar_lancamento(
    db: Session,
    id_cliente: int,
    cod_cliente: int,
    dt: date,
    conta: int,
    ref: str,
    vvalor: Decimal,
    dc: str,
) -> int:
    """
    RN53 — Se DC='D': Deb = VValor, Cred = null.
           Se DC='C': Cred = VValor, Deb = null.
    RN54 — Campos obrigatórios validados pelo schema Pydantic antes de chegar aqui.

    Equivalente a TableAdapterManager.UpdateAll() após preenchimento do BindingSource
    em btnRibSalvar_Click no FrmLanca.vb.

    Retorna o Id do registro inserido.
    """
    # RN53 — lógica DC → Deb/Cred
    deb = vvalor if dc == "D" else None
    cred = vvalor if dc == "C" else None

    result = db.execute(
        text(
            "INSERT INTO Contas (IdCliente, CodCliente, Conta, Dt, Ref, VValor, DC, Deb, Cred) "
            "OUTPUT INSERTED.Id "
            "VALUES (:id_cliente, :cod_cliente, :conta, :dt, :ref, :vvalor, :dc, :deb, :cred)"
        ),
        {
            "id_cliente": id_cliente,
            "cod_cliente": cod_cliente,
            "conta": conta,
            "dt": dt,
            "ref": ref,
            "vvalor": vvalor,
            "dc": dc,
            "deb": deb,
            "cred": cred,
        },
    ).fetchone()

    db.commit()
    return result.Id
