Public Class frmLogin

    Private Sub frmLogin_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load
        'TODO: esta linha de código carrega dados na tabela 'SoloDataSet.Usuários'. Você pode movê-la ou removê-la conforme necessário.
        Me.UsuáriosTableAdapter.Fill(Me.SoloDataSet.Usuários)
        Try
            'TODO: esta linha de código carrega dados na tabela 'DSCadastros2.tb01_Usuarios'. Você pode movê-la ou removê-la conforme necessário.
            Me.UsuáriosTableAdapter.Fill(Me.SoloDataSet.Usuários)

            Me.cboUsuario.Text = ""

        Catch ex As Exception
            MsgBox(ex.Message)

        End Try

    End Sub

    Private Sub btnOk_Click(sender As System.Object, e As System.EventArgs) Handles btnOk.Click
        Try
            If Me.cboUsuario.Text = "" Then
                MsgBox("Por favor, informe o usuário", MsgBoxStyle.Critical, "Atenção!")
                Me.cboUsuario.Focus()
            ElseIf Me.txtSenha.Text = "" Then
                MsgBox("Por favor, informe a senha", MsgBoxStyle.Critical, "Atenção!")
                Me.txtSenha.Focus()
            Else
                varUsu = Me.cboUsuario.Text
                varSenha = Me.txtSenha.Text
                Dim dc As New Solo.ContasDataContext()
                Dim query = (From tb In dc.GetTable(Of Usuários)() Select tb Where tb.Usuário = varUsu And tb.Psw = varSenha)
                Dim y = (From c In query Select c.Id).Count
                If y = 0 Then
                    MsgBox("Senha incorreta", MsgBoxStyle.Critical, "Atenção!")
                    Me.txtSenha.Text = ""
                    Me.txtSenha.Focus()
                Else
                    varPerfil = (From c In query Select c.Perfil).First
                    Me.Visible = False
                    'If DLookUp("Cliente", "SELECT Clientes.Cliente, Clientes_1.Cliente AS Cliente2, Contas.Dt, Contas.Conta, Contas.Ref, Contas.VValor, Contas.DC FROM Clientes INNER JOIN Clientes AS Clientes_1 ON Clientes.Cliente <> Clientes_1.Cliente RIGHT OUTER JOIN Contas ON Clientes_1.Código = Contas.CodCliente AND Clientes.Id = Contas.IdCliente WHERE (Clientes.Cliente IS NOT NULL) AND (Contas.Dt >= CONVERT(DATETIME, '2016-01-01 00:00:00', 102))", "", 1) <> "" Then
                    '    MsgBox("Existem erros de registros com clientes trocados", MsgBoxStyle.Information, "ATENÇÃO!")
                    '    FrmErrosCodIdClientes.Show()
                    'Else
                    FrmPrincipal.Show()
                    FrmPrincipal.lblUsuario.Text = "USUÁRIO: " & UCase(Me.cboUsuario.Text)
                    'End If
                End If
            End If

        Catch ex As Exception
            MsgBox(ex.Message)
            Application.Exit()

        End Try


    End Sub

    Private Sub btnCancelar_Click(sender As System.Object, e As System.EventArgs) Handles btnCancelar.Click
        Application.Exit()

    End Sub

    Private Sub frmLogin_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles Me.KeyDown

        Select Case e.KeyCode
            Case Keys.Enter
                SendKeys.Send("{Tab}")
                'Case Keys.F2
        End Select

    End Sub
End Class