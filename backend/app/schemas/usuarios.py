"""
Schemas Pydantic — Usuários e Chaves
Equivalente a: FrmUsuarios.vb (Solo Consultoria de Imóveis)

Nota de segurança (RN73, D12):
  O campo Psw nunca aparece nos schemas de response.
  Senhas só transitam nos schemas de request (para criação/atualização)
  e são sempre armazenadas como bcrypt no service.
"""

from typing import Optional

from pydantic import BaseModel, Field


# ---------------------------------------------------------------------------
# Perfis
# ---------------------------------------------------------------------------


class PerfilItem(BaseModel):
    """Item da lista de perfis para popular o dropdown de Perfil."""

    perfil: str


# ---------------------------------------------------------------------------
# Usuários — Response (nunca inclui Psw)
# ---------------------------------------------------------------------------


class UsuarioResponse(BaseModel):
    """
    Representação de um usuário retornada pela API.

    D12 — campo Psw NUNCA incluído aqui, independentemente do perfil do solicitante.
    Sem aliases — o service já mapeia os nomes das colunas SQL para os nomes dos campos.
    """

    id: int
    usuario: str
    perfil: str


# ---------------------------------------------------------------------------
# Usuários — Request (criação e atualização)
# ---------------------------------------------------------------------------


class UsuarioCreate(BaseModel):
    """
    Payload para POST /usuarios (apenas Administrador — RN70).
    A senha é obrigatória na criação.
    """

    usuario: str = Field(min_length=1, description="Nome de login do usuário")
    senha: str = Field(min_length=1, description="Senha em texto plano — será armazenada como bcrypt")
    perfil: str = Field(min_length=1, description="Perfil do usuário (deve existir na tabela Perfis)")


class UsuarioUpdate(BaseModel):
    """
    Payload para PUT /usuarios/{id}.

    Campos opcionais: somente o que for enviado será atualizado.
    - Perfil só é aceito quando o solicitante é Administrador (RN71 — validado no service).
    - senha: quando presente, armazenada como bcrypt (RN73).
    """

    usuario: Optional[str] = Field(None, min_length=1)
    senha: Optional[str] = Field(None, min_length=1, description="Nova senha em texto plano — será armazenada como bcrypt")
    perfil: Optional[str] = Field(None, min_length=1)


# ---------------------------------------------------------------------------
# Chaves — Response e Request
# ---------------------------------------------------------------------------


class ChaveResponse(BaseModel):
    """
    Representação de uma linha da tabela Chaves.
    Ref é somente leitura (RN75) — retornado mas não aceito em PUT.
    Sem aliases — o service já mapeia os nomes das colunas SQL para os nomes dos campos.
    """

    id: int
    ref: str
    chave: str


class ChaveUpdate(BaseModel):
    """
    Payload para PUT /chaves/{id}.
    Apenas Chave é editável — Ref é somente leitura (RN75).
    """

    chave: str = Field(min_length=1, description="Nova senha para a operação crítica")


# ---------------------------------------------------------------------------
# Verificação de senha de acesso (frmSenha — varSenha = "1")
# ---------------------------------------------------------------------------


class VerificarSenhaRequest(BaseModel):
    """
    Payload para POST /auth/verificar-senha.
    Consulta a tabela Chaves onde Ref = ref e verifica se Chave = chave.

    RN68 — usado pelo frontend antes de exibir a tela /usuarios.
    Também utilizado para outras operações críticas (transferência, desbloqueio).
    """

    ref: str = Field(description="Contexto da operação (ex: 'Alteração de senhas')")
    chave: str = Field(description="Senha fornecida pelo usuário")


class VerificarSenhaResponse(BaseModel):
    """Resposta de POST /auth/verificar-senha."""

    ok: bool
