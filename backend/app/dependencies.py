"""
Dependências compartilhadas — injetadas via Depends() nos routers.

- get_db: sessão do banco SQL Server
- get_current_user: decodifica JWT e retorna dados do usuário autenticado
"""

import os

from fastapi import Depends, HTTPException, status
from fastapi.security import OAuth2PasswordBearer
from jose import JWTError, jwt

from app.database import get_session_local

# ---------------------------------------------------------------------------
# Configuração JWT (compartilhada com auth_router para criação de tokens)
# ---------------------------------------------------------------------------
SECRET_KEY = os.getenv("JWT_SECRET_KEY", "troque-esta-chave-em-producao")
ALGORITHM = "HS256"

oauth2_scheme = OAuth2PasswordBearer(tokenUrl="/auth/login")


# ---------------------------------------------------------------------------
# Dependência: sessão do banco de dados
# ---------------------------------------------------------------------------
def get_db():
    """
    Fornece uma sessão SQLAlchemy para cada request.
    Fecha automaticamente ao final, mesmo em caso de exceção.
    """
    db = get_session_local()()
    try:
        yield db
    finally:
        db.close()


# ---------------------------------------------------------------------------
# Dependência: usuário autenticado via JWT
# ---------------------------------------------------------------------------
async def get_current_user(
    token: str = Depends(oauth2_scheme),
) -> dict:
    """
    Decodifica o JWT e retorna os dados do usuário logado.
    Substitui o acesso às variáveis globais varUsu e varPerfil do sistema desktop.

    Retorna:
        {"usuario": str, "perfil": str}

    Uso nos routers:
        @router.get("/rota")
        async def rota(user: dict = Depends(get_current_user)):
            ...
    """
    credentials_exception = HTTPException(
        status_code=status.HTTP_401_UNAUTHORIZED,
        detail="Não foi possível validar as credenciais",
        headers={"WWW-Authenticate": "Bearer"},
    )
    try:
        payload = jwt.decode(token, SECRET_KEY, algorithms=[ALGORITHM])
        usuario: str = payload.get("sub")
        perfil: str = payload.get("perfil")
        if usuario is None:
            raise credentials_exception
        return {"usuario": usuario, "perfil": perfil}
    except JWTError:
        raise credentials_exception
