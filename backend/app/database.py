"""
Configuração do SQLAlchemy — Engine e SessionLocal.

Conecta ao SQL Server existente do sistema Solo Consultoria de Imóveis.
O schema do banco é legado e NÃO deve ser alterado sem aprovação.
"""

import os

from sqlalchemy import create_engine
from sqlalchemy.orm import sessionmaker

DATABASE_URL = os.getenv(
    "DATABASE_URL",
    "mssql+pyodbc://user:pass@host/db?driver=ODBC+Driver+17+for+SQL+Server",
)


def _get_engine():
    return create_engine(DATABASE_URL)


engine = None
SessionLocal = None


def init_db():
    """Inicializa engine e SessionLocal. Chamado no startup da aplicação."""
    global engine, SessionLocal  # noqa: PLW0603
    engine = _get_engine()
    SessionLocal = sessionmaker(autocommit=False, autoflush=False, bind=engine)


def get_session_local():
    """Retorna SessionLocal, inicializando se necessário."""
    if SessionLocal is None:
        init_db()
    return SessionLocal
