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

engine = create_engine(DATABASE_URL)

SessionLocal = sessionmaker(autocommit=False, autoflush=False, bind=engine)
