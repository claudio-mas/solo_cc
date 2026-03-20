Public Class FrmPrincipal

    Private Sub FrmPrincipal_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        fExecutaText(";WITH CTE AS (SELECT dbo.Clientes.Código, a.CodCliente FROM dbo.Clientes INNER JOIN (SELECT dbo.Contas.IdCliente, dbo.Contas.CodCliente FROM dbo.Clientes AS Clientes_2 INNER JOIN dbo.Clientes AS Clientes_1 ON Clientes_2.Cliente <> Clientes_1.Cliente RIGHT OUTER JOIN dbo.Contas ON Clientes_1.Código = dbo.Contas.CodCliente AND Clientes_2.Id = dbo.Contas.IdCliente WHERE (Clientes_2.Cliente IS NOT NULL) AND (dbo.Contas.Dt >= CONVERT(DATETIME, '2016-01-01 00:00:00', 102))) AS a ON dbo.Clientes.Id = a.IdCliente WHERE (dbo.Clientes.Cliente IS NOT NULL)) UPDATE CTE SET CodCliente = Código")

        Me.ClientesTableAdapter.Fill(Me.SoloDataSet.Clientes)

        Dim db As New ContasDataContext
        Dim query = (From tb In db.GetTable(Of Clientes)() Select tb Order By tb.Cliente)
        Dim varcod = (From c In query Select c.Código).First
        Me.RibbonGroup6.Text = "CÓD. " & varcod.ToString

        Me.txtPesq.Focus()

    End Sub

    Private Sub ribRetornar_Click(sender As Object, e As EventArgs) Handles ribRetornar.Click
        Application.Exit()
        'Dim strPw
        'strPw = GetPassword("Please enter your password:")
    End Sub

    Private Sub ClientesBindingSource_CurrentChanged(sender As Object, e As EventArgs) Handles ClientesBindingSource.CurrentChanged
        On Error Resume Next

        'Me.btnLançar.Text = "Lançamentos Cód. " & Me.RadGridView1.CurrentRow.Cells(1).Value.ToString
        'Me.btnAlterar.Text = "Alterar Cód. " & Me.RadGridView1.CurrentRow.Cells(1).Value.ToString
        If Me.RadGridView1.CurrentRow IsNot Nothing Then
            Me.RibbonGroup6.Text = "CÓD. " & Me.RadGridView1.CurrentRow.Cells(1).Value.ToString
        Else
            Me.RibbonGroup6.Text = "CÓD."
        End If

    End Sub

    Private Sub btnUsuarios_Click(sender As Object, e As EventArgs) Handles btnUsuarios.Click
        'Dim pwd As String = "", strPassword As String = ""
        'pwd = DLookup("[Chave]", "qpst_Chaves", "[Ref] = 'Alteração de senhas'")
        'strPassword = InputBoxDK("Informe a senha:", "Controle de Senhas", dhXORText("", "SCC"))
        'If StrPtr(strPassword) = 0 Then
        'Exit Sub
        'End If
        'If strPassword <> pwd Then
        '    MsgBox("Senha incorreta", vbCritical)
        'Else
        '    FrmUsuarios.Show()
        'End If

        'FrmUsuarios.Show()
        varSenha = "1"
        frmSenha.ShowDialog()

    End Sub

    Private Sub btnLançar_Click(sender As Object, e As EventArgs) Handles btnLançar.Click

        If Me.RadGridView1.Tag = "" Then
            FrmLancaData.ShowDialog()
        Else
            FrmLanca.Show()
        End If

    End Sub

    Private Sub btnExtrato_Click(sender As Object, e As EventArgs) Handles btnExtrato.Click
        On Error Resume Next

        'Dim strSQL As String

        'strSQL = ";WITH Saldos As (Select Dt, Id, DC, VValor, Deb, Cred, Conta, " & _
        ' "sum(isnull(Cred,0) - isnull(Deb,0)) over (order by dt, id) As Saldo FROM Contas " & _
        ' "WHERE (CodCliente = '" & Me.RadGridView1.CurrentRow.Cells(1).Value.ToString & "')"
        'strSQL = strSQL & ")" & Chr(10) & "Update Contas SET Saldo = S.Saldo FROM Contas INNER JOIN Saldos As S " & Chr(10) & "ON Contas.Id = S.Id"

        'Dim cnn As ADODB.Connection
        'Dim cmd As ADODB.Command
        'cnn = fGetAdoConnection()
        'cmd = New ADODB.Command
        'cmd.ActiveConnection = cnn
        'cmd.CommandType = CommandType.Text
        'cmd.CommandText = strSQL
        'cmd.CommandTimeout = 0
        'cmd.Execute()

        'cmd.Parameters.Append(cmd.CreateParameter("Cliente", ADODB.DataTypeEnum.adChar, ADODB.ParameterDirectionEnum.adParamInput, 100, Me.RadGridView1.CurrentRow.Cells(1).Value.ToString))

        FrmExtrato.Show()

    End Sub

    Private Sub RadGridView1_Click(sender As Object, e As EventArgs) Handles RadGridView1.Click
        On Error Resume Next

        Me.RibbonGroup6.Text = "CÓD. " & Me.RadGridView1.CurrentRow.Cells(1).Value.ToString

    End Sub

    Private Sub btnNovoCliente_Click(sender As Object, e As EventArgs) Handles btnNovoCliente.Click
        frmClienteNovo.Show()

    End Sub

    Private Sub btnTotais_Click(sender As Object, e As EventArgs) Handles btnTotais.Click
        frmTotais2.ShowDialog()

    End Sub

    Private Sub btnRpt_Click(sender As Object, e As EventArgs) Handles btnRpt.Click
        frmReports.Show()

    End Sub

    Private Sub btnAlterar_Click(sender As Object, e As EventArgs) Handles btnAlterar.Click
        frmAlterar.Show()

    End Sub

    Private Sub btnBackup_Click(sender As Object, e As EventArgs) Handles btnBackup.Click
        Try
            Dim arq As String = "C:\CONTAS CORRENTES\Backup\Backup" & Date.Now.ToString("ddMMyyyy") & "_" & Date.Now.ToString("HHmm") & ".BAK"
            'arq = "H:\Sistema\Solo\Backup_" & Format$(Date, "ddmmyyyy") & "_" & Format$(time(), "hhnnss") & ".BAK"

            fExecutaText("Backup DATABASE Solo TO DISK = '" & arq & "' WITH COPY_ONLY")

            Call MsgBox("Backup realizado com sucesso!" _
                        & vbCrLf & "" _
                        & vbCrLf & arq _
                        , vbInformation, "Contas Correntes")

        Catch ex As Exception
            MsgBox(ex.Message)

        End Try

    End Sub

    Private Sub RadGridView1_DoubleClick(sender As Object, e As EventArgs) Handles RadGridView1.DoubleClick
        FrmExtrato.Show()

    End Sub

    Private Sub txtPesq_TextChanged(sender As Object, e As System.EventArgs) Handles txtPesq.TextChanged
        'Me.ClientesBindingSource.Position = 1
        For Each linha In Me.RadGridView1.Rows
            If UCase(Microsoft.VisualBasic.Left(linha.Cells(2).Value.ToString, Len(Me.txtPesq.Text))) = UCase(Me.txtPesq.Text) Or Microsoft.VisualBasic.Left(linha.Cells(1).Value.ToString, Len(Me.txtPesq.Text)) = Me.txtPesq.Text Then
                Me.ClientesBindingSource.Position = linha.Index
                Exit For
            End If
        Next

    End Sub

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        Me.txtPesq.Focus()
        Me.Timer1.Enabled = False

    End Sub

End Class
