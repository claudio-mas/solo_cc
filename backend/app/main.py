"""
Entrypoint FastAPI — Solo Consultoria de Imóveis · Contas Correntes Web

Comando para desenvolvimento:
    uvicorn app.main:app --reload
"""

from dotenv import load_dotenv

load_dotenv()  # carrega backend/.env antes de qualquer import que leia os.getenv()

from fastapi import FastAPI
from fastapi.middleware.cors import CORSMiddleware

from app.routers.auth_router import router as auth_router
from app.routers.clientes import router as clientes_router
from app.routers.lancamentos import router as lancamentos_router

app = FastAPI(
    title="Solo CC API",
    description="API do sistema de Contas Correntes — Solo Consultoria de Imóveis",
    version="0.1.0",
)

# ---------------------------------------------------------------------------
# CORS — permite chamadas do frontend em desenvolvimento
# ---------------------------------------------------------------------------
app.add_middleware(
    CORSMiddleware,
    allow_origins=["http://localhost:5173", "http://localhost:5174", "http://localhost:3000"],
    allow_credentials=True,
    allow_methods=["*"],
    allow_headers=["*"],
)

# ---------------------------------------------------------------------------
# Routers
# ---------------------------------------------------------------------------
app.include_router(auth_router)
app.include_router(clientes_router)
app.include_router(lancamentos_router)
