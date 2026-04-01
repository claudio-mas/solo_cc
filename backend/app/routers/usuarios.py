"""
Endpoints FastAPI — Usuários e Chaves (FrmUsuarios)
Equivalente a: FrmUsuarios.vb (Solo Consultoria de Imóveis)

Regras implementadas:
  RN68 — Acesso à tela exige validação via POST /auth/verificar-senha (no auth_router)
  RN69 — Administrador vê todos; perfil comum vê apenas o próprio
  RN70 — Apenas Administrador pode criar usuários
  RN71 — Perfil comum não pode alterar o campo Perfil
  RN72 — Chaves visíveis apenas para Administrador
  RN73 — Senha armazenada como bcrypt
  RN74 — Controle de visibilidade Salvar/Cancelar: responsabilidade do frontend
  RN75 — Ref em Chaves é somente leitura
  RN76 — Chaves: sem criação nem exclusão
  RN77 — Id nunca exposto nos requests; oculto na UI

Nota de segurança (D12):
  Psw nunca incluída em responses — garantido pelos schemas UsuarioResponse/ChaveResponse.
"""

from fastapi import APIRouter, Depends, HTTPException, status
from sqlalchemy.orm import Session

from app.dependencies import get_current_user, get_db
from app.schemas.usuarios import (
    ChaveResponse,
    ChaveUpdate,
    PerfilItem,
    UsuarioCreate,
    UsuarioResponse,
    UsuarioUpdate,
)
from app.services import usuarios as svc

router = APIRouter(tags=["Usuários"])


# ---------------------------------------------------------------------------
# Perfis (suporte ao combobox — B19)
# ---------------------------------------------------------------------------


@router.get(
    "/perfis",
    response_model=list[PerfilItem],
    summary="Lista perfis disponíveis",
    description=(
        "Retorna todos os perfis cadastrados na tabela Perfis, ordenados alfabeticamente. "
        "Usado para popular o dropdown de Perfil na tela /usuarios. "
        "Equivalente ao PerfisBindingSource carregado no FrmUsuarios_Load."
    ),
)
async def listar_perfis(
    db: Session = Depends(get_db),
    _user: dict = Depends(get_current_user),
):
    return svc.listar_perfis(db)


# ---------------------------------------------------------------------------
# Usuários
# ---------------------------------------------------------------------------


@router.get(
    "/usuarios",
    response_model=list[UsuarioResponse],
    summary="Lista usuários",
    description=(
        "Administrador recebe todos os usuários (RN69). "
        "Perfil comum recebe apenas o próprio registro (RN69). "
        "O campo Psw nunca é retornado (D12). "
        "Equivalente ao UsuáriosTableAdapter.Fill + filtro de BindingSource no FrmUsuarios_Load."
    ),
)
async def listar_usuarios(
    db: Session = Depends(get_db),
    user: dict = Depends(get_current_user),
):
    # RN69 — filtro por perfil aplicado no service
    rows = svc.listar_usuarios(db, user["usuario"], user["perfil"])
    return rows


@router.post(
    "/usuarios",
    response_model=UsuarioResponse,
    status_code=status.HTTP_201_CREATED,
    summary="Cria novo usuário",
    description=(
        "Apenas Administrador pode criar usuários (RN70). "
        "A senha é armazenada como bcrypt (RN73). "
        "Retorna o usuário criado sem o campo Psw (D12). "
        "Equivalente a AllowAddNewRow no RadGridView2 + UpdateAll no FrmUsuarios."
    ),
)
async def criar_usuario(
    payload: UsuarioCreate,
    db: Session = Depends(get_db),
    user: dict = Depends(get_current_user),
):
    try:
        # RN70 + RN73 — validados no service
        row = svc.criar_usuario(
            db,
            usuario=payload.usuario,
            senha=payload.senha,
            perfil=payload.perfil,
            perfil_logado=user["perfil"],
        )
        return row
    except PermissionError as exc:
        raise HTTPException(status_code=status.HTTP_403_FORBIDDEN, detail=str(exc))
    except ValueError as exc:
        raise HTTPException(status_code=status.HTTP_409_CONFLICT, detail=str(exc))


@router.put(
    "/usuarios/{id_usuario}",
    response_model=UsuarioResponse,
    summary="Atualiza usuário",
    description=(
        "Perfil comum só pode alterar o próprio registro (RN69). "
        "Perfil comum não pode alterar o campo Perfil (RN71). "
        "Nova senha é armazenada como bcrypt (RN73). "
        "Retorna o usuário atualizado sem o campo Psw (D12). "
        "Equivalente à edição inline no RadGridView2 + UpdateAll no FrmUsuarios."
    ),
)
async def atualizar_usuario(
    id_usuario: int,
    payload: UsuarioUpdate,
    db: Session = Depends(get_db),
    user: dict = Depends(get_current_user),
):
    try:
        # RN69, RN71, RN73 — validados no service
        row = svc.atualizar_usuario(
            db,
            id_usuario=id_usuario,
            usuario_logado=user["usuario"],
            perfil_logado=user["perfil"],
            novo_usuario=payload.usuario,
            nova_senha=payload.senha,
            novo_perfil=payload.perfil,
        )
        return row
    except PermissionError as exc:
        raise HTTPException(status_code=status.HTTP_403_FORBIDDEN, detail=str(exc))
    except ValueError as exc:
        raise HTTPException(status_code=status.HTTP_404_NOT_FOUND, detail=str(exc))


# ---------------------------------------------------------------------------
# Chaves
# ---------------------------------------------------------------------------


@router.get(
    "/chaves",
    response_model=list[ChaveResponse],
    summary="Lista chaves de operações críticas",
    description=(
        "Apenas Administrador pode acessar as chaves (RN72). "
        "Retorna Id, Ref e Chave — sem criação nem exclusão de linhas (RN76). "
        "Equivalente à aba 'Senhas' (C1DockingTabPage2) no FrmUsuarios, "
        "visível apenas para Administrador."
    ),
)
async def listar_chaves(
    db: Session = Depends(get_db),
    user: dict = Depends(get_current_user),
):
    try:
        # RN72 — validado no service
        return svc.listar_chaves(db, user["perfil"])
    except PermissionError as exc:
        raise HTTPException(status_code=status.HTTP_403_FORBIDDEN, detail=str(exc))


@router.put(
    "/chaves/{id_chave}",
    response_model=ChaveResponse,
    summary="Atualiza chave de operação crítica",
    description=(
        "Apenas Administrador pode alterar chaves (RN72). "
        "Somente o campo Chave é atualizado — Ref é somente leitura (RN75). "
        "Não é possível criar nem excluir linhas (RN76). "
        "Equivalente à edição inline no RadGridView1 + UpdateAll no FrmUsuarios."
    ),
)
async def atualizar_chave(
    id_chave: int,
    payload: ChaveUpdate,
    db: Session = Depends(get_db),
    user: dict = Depends(get_current_user),
):
    try:
        # RN72, RN75, RN76 — validados no service
        return svc.atualizar_chave(db, id_chave, payload.chave, user["perfil"])
    except PermissionError as exc:
        raise HTTPException(status_code=status.HTTP_403_FORBIDDEN, detail=str(exc))
    except ValueError as exc:
        raise HTTPException(status_code=status.HTTP_404_NOT_FOUND, detail=str(exc))
