"""
Lógica de negócio — Usuários e Chaves
Regras de negócio desacopladas dos routers.

Referências:
  - FrmUsuarios: RN68–RN77 (ver docs/FrmUsuarios_documentacao.md)
  - D8: duas abas → duas seções; D9: modal de senha; D10: bcrypt; D11: edição inline; D12: Psw nunca retornada

Nota de segurança:
  - RN73 / D10: senhas sempre armazenadas como bcrypt via passlib.
  - D12: campo Psw nunca incluído em resultados retornados.
"""

from passlib.context import CryptContext
from sqlalchemy import text
from sqlalchemy.orm import Session

pwd_context = CryptContext(schemes=["bcrypt"], deprecated="auto")


# ---------------------------------------------------------------------------
# Helpers
# ---------------------------------------------------------------------------


def _hash_password(plain: str) -> str:
    """RN73 — converte senha em texto plano para bcrypt antes de salvar."""
    return pwd_context.hash(plain)


def _verificar_senha_chave(plain: str, stored: str) -> bool:
    """
    Verifica senha da tabela Chaves.
    As chaves do sistema podem ainda estar em texto plano (legado).
    """
    if stored.startswith("$2b$") or stored.startswith("$2a$"):
        return pwd_context.verify(plain, stored)
    return plain == stored


# ---------------------------------------------------------------------------
# Verificação de senha de acesso (frmSenha — varSenha = "1")
# ---------------------------------------------------------------------------


def verificar_senha_operacao(db: Session, ref: str, chave: str) -> bool:
    """
    RN68 — valida a senha de uma operação crítica consultando a tabela Chaves.

    Equivalente ao frmSenha.vb que consulta Chaves WHERE Ref = varRef.
    Suporta senhas em texto plano (legado) e bcrypt.

    Args:
        ref:   Contexto da operação (ex: 'Alteração de senhas').
        chave: Senha fornecida pelo usuário.

    Returns:
        True se a senha for válida; False caso contrário.
    """
    result = db.execute(
        text("SELECT Chave FROM Chaves WHERE Ref = :ref"),
        {"ref": ref},
    ).fetchone()

    if result is None:
        return False

    return _verificar_senha_chave(chave, result.Chave)


# ---------------------------------------------------------------------------
# Perfis
# ---------------------------------------------------------------------------


def listar_perfis(db: Session) -> list[dict]:
    """Retorna a lista de perfis disponíveis para o combobox (RN71 — B19)."""
    rows = db.execute(
        text("SELECT Perfil FROM Perfis ORDER BY Perfil ASC")
    ).fetchall()
    return [{"perfil": row.Perfil} for row in rows]


# ---------------------------------------------------------------------------
# Usuários
# ---------------------------------------------------------------------------


def listar_usuarios(db: Session, usuario_logado: str, perfil_logado: str) -> list[dict]:
    """
    RN69 — Administrador vê todos os usuários; perfil comum vê apenas o próprio.
    D12 — Psw nunca incluída no resultado.

    Returns lista de dicts com id, usuario, perfil.
    """
    if perfil_logado == "Administrador":
        rows = db.execute(
            text("SELECT Id, Usuário, Perfil FROM Usuários ORDER BY Usuário ASC")
        ).fetchall()
    else:
        rows = db.execute(
            text("SELECT Id, Usuário, Perfil FROM Usuários WHERE Usuário = :usuario"),
            {"usuario": usuario_logado},
        ).fetchall()

    return [{"id": row.Id, "usuario": row.Usuário, "perfil": row.Perfil} for row in rows]


def criar_usuario(db: Session, usuario: str, senha: str, perfil: str, perfil_logado: str) -> dict:
    """
    RN70 — apenas Administrador pode criar usuários.
    RN73 — senha armazenada como bcrypt.
    D12 — Psw nunca retornada.

    Raises:
        PermissionError: se o solicitante não for Administrador.
        ValueError: se o login já existir.
    """
    # RN70 — guard de perfil
    if perfil_logado != "Administrador":
        raise PermissionError("Apenas o Administrador pode criar usuários.")

    # Verifica unicidade do login
    exists = db.execute(
        text("SELECT Id FROM Usuários WHERE Usuário = :usuario"),
        {"usuario": usuario},
    ).fetchone()
    if exists:
        raise ValueError(f"Usuário '{usuario}' já existe.")

    psw_hash = _hash_password(senha)  # RN73

    db.execute(
        text("INSERT INTO Usuários (Usuário, Psw, Perfil) VALUES (:usuario, :psw, :perfil)"),
        {"usuario": usuario, "psw": psw_hash, "perfil": perfil},
    )
    db.commit()

    row = db.execute(
        text("SELECT Id, Usuário, Perfil FROM Usuários WHERE Usuário = :usuario"),
        {"usuario": usuario},
    ).fetchone()

    return {"id": row.Id, "usuario": row.Usuário, "perfil": row.Perfil}


def atualizar_usuario(
    db: Session,
    id_usuario: int,
    usuario_logado: str,
    perfil_logado: str,
    novo_usuario: str | None,
    nova_senha: str | None,
    novo_perfil: str | None,
) -> dict:
    """
    RN69 — perfil comum só pode atualizar o próprio registro.
    RN71 — perfil comum não pode alterar o campo Perfil.
    RN73 — nova senha armazenada como bcrypt.
    D12 — Psw nunca retornada.

    Raises:
        PermissionError: violação de RN69 ou RN71.
        ValueError: usuário não encontrado.
    """
    # Busca o registro alvo
    row = db.execute(
        text("SELECT Id, Usuário, Psw, Perfil FROM Usuários WHERE Id = :id"),
        {"id": id_usuario},
    ).fetchone()

    if row is None:
        raise ValueError(f"Usuário id={id_usuario} não encontrado.")

    # RN69 — perfil comum só pode alterar o próprio
    if perfil_logado != "Administrador" and row.Usuário != usuario_logado:
        raise PermissionError("Você só pode alterar o próprio registro.")

    # RN71 — perfil comum não pode alterar Perfil
    if novo_perfil is not None and perfil_logado != "Administrador":
        raise PermissionError("Apenas o Administrador pode alterar o campo Perfil.")

    # Monta campos a atualizar
    campos = {}
    if novo_usuario is not None:
        campos["Usuário"] = novo_usuario
    if nova_senha is not None:
        campos["Psw"] = _hash_password(nova_senha)  # RN73
    if novo_perfil is not None:
        campos["Perfil"] = novo_perfil

    if not campos:
        # Nenhum campo enviado — retorna o estado atual sem UPDATE
        return {"id": row.Id, "usuario": row.Usuário, "perfil": row.Perfil}

    set_clause = ", ".join(f"{col} = :{col}" for col in campos)
    params = {**campos, "id": id_usuario}

    db.execute(
        text(f"UPDATE Usuários SET {set_clause} WHERE Id = :id"),  # noqa: S608
        params,
    )
    db.commit()

    updated = db.execute(
        text("SELECT Id, Usuário, Perfil FROM Usuários WHERE Id = :id"),
        {"id": id_usuario},
    ).fetchone()

    return {"id": updated.Id, "usuario": updated.Usuário, "perfil": updated.Perfil}


# ---------------------------------------------------------------------------
# Chaves
# ---------------------------------------------------------------------------


def listar_chaves(db: Session, perfil_logado: str) -> list[dict]:
    """
    RN72 — apenas Administrador pode ver as chaves.

    Raises:
        PermissionError: se o solicitante não for Administrador.
    """
    if perfil_logado != "Administrador":
        raise PermissionError("Apenas o Administrador pode acessar as chaves do sistema.")

    rows = db.execute(
        text("SELECT Id, Ref, Chave FROM Chaves ORDER BY Id ASC")
    ).fetchall()

    return [{"id": row.Id, "ref": row.Ref, "chave": row.Chave} for row in rows]


def atualizar_chave(db: Session, id_chave: int, nova_chave: str, perfil_logado: str) -> dict:
    """
    RN72 — apenas Administrador pode alterar chaves.
    RN75 — Ref não é alterada; somente Chave.
    RN76 — sem criação ou exclusão de linhas.

    Raises:
        PermissionError: se o solicitante não for Administrador.
        ValueError: se a chave não for encontrada.
    """
    if perfil_logado != "Administrador":
        raise PermissionError("Apenas o Administrador pode alterar chaves do sistema.")

    row = db.execute(
        text("SELECT Id, Ref, Chave FROM Chaves WHERE Id = :id"),
        {"id": id_chave},
    ).fetchone()

    if row is None:
        raise ValueError(f"Chave id={id_chave} não encontrada.")

    db.execute(
        text("UPDATE Chaves SET Chave = :chave WHERE Id = :id"),
        {"chave": nova_chave, "id": id_chave},
    )
    db.commit()

    updated = db.execute(
        text("SELECT Id, Ref, Chave FROM Chaves WHERE Id = :id"),
        {"id": id_chave},
    ).fetchone()

    return {"id": updated.Id, "ref": updated.Ref, "chave": updated.Chave}
