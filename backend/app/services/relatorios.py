"""
Service — Relatórios (frmRptDevedores1 / frmCredores1)

Regras implementadas:
  RN108 — Devedores: SUM(Deb) > SUM(Cred) até a data de corte
  RN109 — Filtro por faixa de código (todos / acima >= 10000 / abaixo < 10000)
  RN110 — Filtro por saldo mínimo (saldo devedor > :saldo_minimo)
  RN111 — Ordenação por código ou por nome
  RN112 — Título dinâmico de devedores
  RN113 — Saldo retornado positivo (diferença TD - TC)
  RN114 — Credores: SUM(Cred) > SUM(Deb) até a data de corte
  RN115 — Filtro por faixa de código (mesmo que RN109)
  RN116 — Filtro por saldo mínimo para credores
  RN117 — Ordenação por código ou nome (mesmo que RN111)
  RN118 — Título dinâmico de credores
  RN119 — Saldo retornado positivo (diferença TC - TD)

Nota D18: usa colunas Deb/Cred em vez de CASE DC/VValor — equivalente e consistente
com o serviço de extrato.
Nota D19: data passada como parâmetro tipado SQLAlchemy — elimina o bug yyyy-dd-MM
do legado.
"""

from datetime import date
from typing import Literal

from sqlalchemy import text
from sqlalchemy.orm import Session

from app.schemas.relatorios import ClienteRelatorioItem, RelatorioResponse


def _montar_query_base(
    tipo: Literal["devedores", "credores"],
    faixa_codigo: Literal["todos", "acima", "abaixo"],
    ordenacao: Literal["codigo", "nome"],
) -> str:
    """
    Monta a query SQL para devedores ou credores.

    Devedores: SUM(Deb) - SUM(Cred) > saldo_minimo  (TD > TC)
    Credores:  SUM(Cred) - SUM(Deb) > saldo_minimo  (TC > TD)
    """
    if tipo == "devedores":
        saldo_expr = "SUM(ISNULL(c.Deb, 0)) - SUM(ISNULL(c.Cred, 0))"
    else:
        saldo_expr = "SUM(ISNULL(c.Cred, 0)) - SUM(ISNULL(c.Deb, 0))"

    faixa_clause = ""
    if faixa_codigo == "acima":
        faixa_clause = "AND cl.Código >= 10000"
    elif faixa_codigo == "abaixo":
        faixa_clause = "AND cl.Código < 10000"

    order_col = "cl.Código" if ordenacao == "codigo" else "cl.Cliente"

    return f"""
        SELECT
            cl.Id              AS id,
            cl.Código          AS CodCliente,
            cl.Cliente         AS Cliente,
            {saldo_expr}       AS saldo
        FROM Clientes cl
        INNER JOIN Contas c ON c.CodCliente = cl.Código
        WHERE c.Dt <= :data_corte
          {faixa_clause}
        GROUP BY cl.Id, cl.Código, cl.Cliente
        HAVING {saldo_expr} > :saldo_minimo
        ORDER BY {order_col}
    """


def listar_devedores(
    db: Session,
    data_corte: date,
    saldo_minimo: float,
    faixa_codigo: Literal["todos", "acima", "abaixo"],
    ordenacao: Literal["codigo", "nome"],
) -> RelatorioResponse:
    """
    RN108–RN113 — retorna clientes cujo SUM(Deb) supera SUM(Cred) até data_corte.
    Equivalente à query de frmRptDevedores1_Load no legado.
    """
    sql = _montar_query_base("devedores", faixa_codigo, ordenacao)
    rows = db.execute(
        text(sql),
        {"data_corte": data_corte, "saldo_minimo": saldo_minimo},
    ).fetchall()

    itens = [
        ClienteRelatorioItem(
            id=row.id,
            cod_cliente=row.CodCliente,
            cliente=row.Cliente,
            saldo=float(row.saldo),
        )
        for row in rows
    ]

    # RN112 — título dinâmico
    titulo = (
        f"Clientes devedores em {data_corte.strftime('%d/%m/%Y')} "
        f"com saldo maior ou igual a R$ {saldo_minimo:,.2f}"
    )

    return RelatorioResponse(
        titulo=titulo,
        data_corte=data_corte,
        saldo_minimo=saldo_minimo,
        faixa_codigo=faixa_codigo,
        ordenacao=ordenacao,
        itens=itens,
    )


def listar_credores(
    db: Session,
    data_corte: date,
    saldo_minimo: float,
    faixa_codigo: Literal["todos", "acima", "abaixo"],
    ordenacao: Literal["codigo", "nome"],
) -> RelatorioResponse:
    """
    RN114–RN119 — retorna clientes cujo SUM(Cred) supera SUM(Deb) até data_corte.
    Equivalente à query de frmCredores1_Load no legado.
    """
    sql = _montar_query_base("credores", faixa_codigo, ordenacao)
    rows = db.execute(
        text(sql),
        {"data_corte": data_corte, "saldo_minimo": saldo_minimo},
    ).fetchall()

    itens = [
        ClienteRelatorioItem(
            id=row.id,
            cod_cliente=row.CodCliente,
            cliente=row.Cliente,
            saldo=float(row.saldo),
        )
        for row in rows
    ]

    # RN118 — título dinâmico
    titulo = (
        f"Clientes credores em {data_corte.strftime('%d/%m/%Y')} "
        f"com saldo maior ou igual a R$ {saldo_minimo:,.2f}"
    )

    return RelatorioResponse(
        titulo=titulo,
        data_corte=data_corte,
        saldo_minimo=saldo_minimo,
        faixa_codigo=faixa_codigo,
        ordenacao=ordenacao,
        itens=itens,
    )
