"""
Lógica de negócio — Totais
Regras de negócio desacopladas dos routers.

Referências:
  - frmTotais2: RN59 (data padrão = hoje), RN60 (recalculo ao mudar data),
                RN61–RN67 (credores/devedores com acumulado até a data)

Decisões:
  - D5: Bug de data no legado ("yyyy-dd-MM") corrigido — backend usa ISO 8601.
  - D7: As 4 queries ADODB separadas do legado são consolidadas em 1 query CTE.
"""

from datetime import date
from decimal import Decimal

from sqlalchemy import text
from sqlalchemy.orm import Session


def buscar_totais(db: Session, data: date) -> dict:
    """
    RN61–RN67 — Calcula os 4 totais de clientes credores e devedores
    até a data informada (inclusive).

    Equivalente às 4 queries ADODB executadas em FrmTotais_Load e
    Dt_ValueChanged no frmTotais2.vb, consolidadas em uma única query CTE.

    Retorna dict com:
      - qtde_credores  (RN63): COUNT de clientes onde TC > TD
      - valor_credores (RN64): SUM(TC − TD) para credores
      - qtde_devedores (RN65): COUNT de clientes onde TD > TC
      - valor_devedores(RN66): SUM(TD − TC) para devedores

    RN67 — filtro: WHERE Dt <= :data (acumulado desde o início até a data)

    D5 — a data recebida já é ISO 8601 (yyyy-MM-dd); o legado usava o formato
         invertido "yyyy-dd-MM" que produzia datas erradas — bug corrigido na web.
    D7 — as 4 queries separadas do legado são executadas em uma única CTE para
         reduzir round-trips ao banco e eliminar duplicação de lógica.
    """
    result = db.execute(
        text(
            "WITH saldos AS ( "
            "    SELECT "
            "        CodCliente, "
            "        SUM(CASE WHEN DC = 'D' THEN VValor ELSE 0 END) AS TD, "
            "        SUM(CASE WHEN DC = 'C' THEN VValor ELSE 0 END) AS TC "
            "    FROM Contas "
            "    WHERE Dt <= :data "
            "    GROUP BY CodCliente "
            ") "
            "SELECT "
            "    COUNT(CASE WHEN TC > TD THEN 1 END)                        AS qtde_credores, "
            "    ISNULL(SUM(CASE WHEN TC > TD THEN TC - TD END), 0)         AS valor_credores, "
            "    COUNT(CASE WHEN TD > TC THEN 1 END)                        AS qtde_devedores, "
            "    ISNULL(SUM(CASE WHEN TD > TC THEN TD - TC END), 0)         AS valor_devedores "
            "FROM saldos"
        ),
        {"data": data},
    ).fetchone()

    return {
        "qtde_credores": result.qtde_credores or 0,
        "valor_credores": Decimal(str(result.valor_credores or 0)),
        "qtde_devedores": result.qtde_devedores or 0,
        "valor_devedores": Decimal(str(result.valor_devedores or 0)),
    }
