"""
Endpoints FastAPI — Autenticação (frmLogin)
Equivalente a: frmLogin.vb (Solo Consultoria de Imóveis)

Dependências:
    pip install fastapi sqlalchemy pyodbc python-jose[cryptography] passlib[bcrypt] python-dotenv
"""

import os
from datetime import datetime, timedelta
from typing import Optional

from fastapi import APIRouter, Depends, HTTPException, status
from jose import jwt
from passlib.context import CryptContext
from pydantic import BaseModel
from sqlalchemy import text
from sqlalchemy.orm import Session

from app.dependencies import get_current_user, get_db  # noqa: F401
from app.schemas.usuarios import (
    VerificarSenhaRequest,
    VerificarSenhaResponse,
)
from app.services.usuarios import verificar_senha_operacao

router = APIRouter(prefix="/auth", tags=["Autenticação"])

# ---------------------------------------------------------------------------
# Configuração
# ---------------------------------------------------------------------------
SECRET_KEY = os.getenv("JWT_SECRET_KEY", "troque-esta-chave-em-producao")
ALGORITHM = "HS256"
ACCESS_TOKEN_EXPIRE_MINUTES = 480  # 8 horas (equivalente a uma sessão de trabalho)

pwd_context = CryptContext(schemes=["bcrypt"], deprecated="auto")


# ---------------------------------------------------------------------------
# Schemas (Pydantic)
# ---------------------------------------------------------------------------
class LoginRequest(BaseModel):
    """
    Corpo da requisição de login.
    Equivalente ao preenchimento de cboUsuario + txtSenha no frmLogin.
    """

    usuario: str
    senha: str


class LoginResponse(BaseModel):
    """
    Resposta de login bem-sucedido.
    Substitui as variáveis globais varUsu, varPerfil do sistema desktop.
    """

    access_token: str
    token_type: str = "bearer"
    usuario: str
    perfil: str


class UsuarioListItem(BaseModel):
    """
    Item da lista de usuários para popular o dropdown.
    Equivalente ao cboUsuario carregado via UsuáriosTableAdapter2.Fill()
    """

    usuario: str


# ---------------------------------------------------------------------------
# Utilitários de autenticação
# ---------------------------------------------------------------------------
def verify_password(plain_password: str, hashed_password: str) -> bool:
    """
    Verifica senha — suporta bcrypt (pós-migração) e texto plano (legado).

    NOTA DE MIGRAÇÃO:
    O sistema original armazena senhas em texto plano (Psw = varSenha).
    Senhas já migradas terão prefixo $2b$ (bcrypt). Após go-live, remover
    o fallback de texto plano e garantir que todas as senhas estejam em bcrypt.
    """
    if hashed_password.startswith("$2b$") or hashed_password.startswith("$2a$"):
        return pwd_context.verify(plain_password, hashed_password)
    # LEGADO: comparação direta para senhas ainda não migradas para bcrypt
    return plain_password == hashed_password


def create_access_token(data: dict, expires_delta: Optional[timedelta] = None) -> str:
    """Gera JWT com os dados do usuário autenticado."""
    to_encode = data.copy()
    expire = datetime.utcnow() + (expires_delta or timedelta(minutes=ACCESS_TOKEN_EXPIRE_MINUTES))
    to_encode.update({"exp": expire})
    return jwt.encode(to_encode, SECRET_KEY, algorithm=ALGORITHM)


# ---------------------------------------------------------------------------
# Endpoints
# ---------------------------------------------------------------------------


@router.get(
    "/usuarios",
    response_model=list[UsuarioListItem],
    summary="Lista usuários para o dropdown de login",
    description=(
        "Retorna a lista de nomes de usuários cadastrados, ordenada alfabeticamente. "
        "Equivalente ao carregamento do cboUsuario via UsuáriosTableAdapter2.Fill() "
        "no evento frmLogin_Load."
    ),
)
async def listar_usuarios(db: Session = Depends(get_db)):
    """
    RN08: Usuários disponíveis ordenados alfabeticamente.
    RN07: Lista carregada do banco na abertura do form.
    """
    try:
        result = db.execute(text("SELECT Usuário FROM Usuários ORDER BY Usuário ASC")).fetchall()
        return [{"usuario": row[0]} for row in result]
    except Exception as e:
        raise HTTPException(
            status_code=status.HTTP_500_INTERNAL_SERVER_ERROR,
            detail=f"Erro ao carregar usuários: {str(e)}",
        )


@router.post(
    "/login",
    response_model=LoginResponse,
    summary="Autentica usuário no sistema",
    description=(
        "Valida as credenciais do usuário. Em caso de sucesso, retorna um JWT "
        "contendo o nome e perfil do usuário. "
        "Equivalente ao evento btnOk_Click do frmLogin."
    ),
)
async def login(payload: LoginRequest, db: Session = Depends(get_db)):
    """
    RN01: Campo usuário obrigatório.
    RN02: Campo senha obrigatório.
    RN03: Combinação usuário + senha deve existir na tabela Usuários.
    RN04: Senha incorreta → não revela qual campo está errado (segurança).
    RN05: Perfil recuperado e incluído no token JWT.
    RN06: Nome do usuário retornado em maiúsculas para exibição na tela principal.
    """

    # RN01 — Usuário obrigatório
    if not payload.usuario or not payload.usuario.strip():
        raise HTTPException(
            status_code=status.HTTP_422_UNPROCESSABLE_ENTITY, detail="Por favor, informe o usuário"
        )

    # RN02 — Senha obrigatória
    if not payload.senha or not payload.senha.strip():
        raise HTTPException(
            status_code=status.HTTP_422_UNPROCESSABLE_ENTITY, detail="Por favor, informe a senha"
        )

    try:
        # Busca usuário no banco
        result = db.execute(
            text("SELECT Id, Usuário, Psw, Perfil FROM Usuários WHERE Usuário = :usuario"),
            {"usuario": payload.usuario},
        ).fetchone()

        # RN03 — Valida credenciais
        # NOTA: verify_password usa bcrypt. Se senhas ainda estiverem em texto plano
        # durante a transição, usar: result.Psw == payload.senha
        if result is None or not verify_password(payload.senha, result.Psw):
            raise HTTPException(
                status_code=status.HTTP_401_UNAUTHORIZED,
                detail="Senha incorreta",
                headers={"WWW-Authenticate": "Bearer"},
            )

        # RN05 — Perfil no token
        # RN06 — Nome em maiúsculas
        token_data = {
            "sub": result.Usuário,
            "perfil": result.Perfil,
        }
        access_token = create_access_token(token_data)

        return LoginResponse(
            access_token=access_token,
            usuario=result.Usuário.upper(),  # RN06
            perfil=result.Perfil,
        )

    except HTTPException:
        raise
    except Exception as e:
        raise HTTPException(
            status_code=status.HTTP_500_INTERNAL_SERVER_ERROR, detail=f"Erro interno: {str(e)}"
        )


@router.post(
    "/verificar-senha",
    response_model=VerificarSenhaResponse,
    summary="Verifica senha de operação crítica",
    description=(
        "Valida a senha fornecida contra a tabela Chaves pelo campo Ref. "
        "RN68 — equivalente ao frmSenha.vb (varSenha='1'): antes de abrir "
        "a tela de usuários, o frontend chama este endpoint com "
        "ref='Alteração de senhas'. Também usado para outras operações "
        "críticas (transferência, desbloqueio). "
        "Requer autenticação JWT."
    ),
)
async def verificar_senha(
    payload: VerificarSenhaRequest,
    db: Session = Depends(get_db),
    _user: dict = Depends(get_current_user),
):
    """
    RN68 — valida senha de operação crítica via tabela Chaves.
    Suporta senhas em texto plano (legado) e bcrypt.
    Retorna {ok: true} se válida; {ok: false} se inválida.
    Nunca revela qual campo está errado (apenas ok: false).
    """
    ok = verificar_senha_operacao(db, ref=payload.ref, chave=payload.chave)
    return VerificarSenhaResponse(ok=ok)
