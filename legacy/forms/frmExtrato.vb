Public Class FrmExtrato

    Private Sub FrmExtrato_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        'TODO: esta linha de código carrega dados na tabela 'SoloDataSet.Clientes'. Você pode movê-la ou removê-la conforme necessário.
        Me.ClientesTableAdapter.Fill(Me.SoloDataSet.Clientes)
        Me.ClientesBindingSource.Filter = "[Id] <> " & FrmPrincipal.RadGridView1.CurrentRow.Cells(0).Value.ToString

        'TODO: esta linha de código carrega dados na tabela 'SoloDataSet.Extrato'. Você pode movê-la ou removê-la conforme necessário.
        Me.ExtratoTableAdapter.Fill(Me.SoloDataSet.Extrato)
        Me.ExtratoBindingSource.Filter = "[IdCliente] = " & FrmPrincipal.RadGridView1.CurrentRow.Cells(0).Value.ToString

        'Dim cn As New ADODB.Connection()
        'Dim rs As New ADODB.Recordset()
        'Dim cnStr As String
        'Dim cmd As New ADODB.Command()
        'cnStr = strAdoConnection()
        'rs.Open("SELECT Saldo FROM Contas Where [IdCliente] = " & FrmPrincipal.RadGridView1.CurrentRow.Cells(0).Value & " ORDER BY Dt, Id", cnStr, ADODB.CursorTypeEnum.adOpenKeyset, ADODB.LockTypeEnum.adLockOptimistic)
        'rs.MoveLast()
        'Me.lbSaldo.Text = "Saldo: " & FormatNumber(rs("Saldo").Value, 2).ToString
        'If rs("Saldo").Value >= 0 Then Me.lbSaldo.ForeColor = Color.Red Else Me.lbSaldo.ForeColor = Color.Blue

        'Me.ExtratoBindingSource.MoveLast()
        'Me.lbSaldo.Text = "Saldo: " & FormatNumber(Me.MasterTemplate.CurrentRow.Cells(12).Value, 2).ToString
        'If Me.MasterTemplate.CurrentRow.Cells(12).Value >= 0 Then Me.lbSaldo.ForeColor = Color.Red Else Me.lbSaldo.ForeColor = Color.Blue
        'Me.ExtratoBindingSource.MoveFirst()
        'Call update_extrato_2()

        'rs.Close()

        Me.lbCliente.Text = FrmPrincipal.RadGridView1.CurrentRow.Cells(2).Value.ToString & " - " & FrmPrincipal.RadGridView1.CurrentRow.Cells(1).Value.ToString

        Dim varSaldo As Double = 0
        Dim varCred As Double = 0
        Dim varDeb As Double = 0
        For Each linha In MasterTemplate.Rows
            If IsDBNull(linha.Cells(10).Value) Then varDeb = 0 Else varDeb = linha.Cells(10).Value
            If IsDBNull(linha.Cells(11).Value) Then varCred = 0 Else varCred = linha.Cells(11).Value
            varSaldo = varSaldo + varCred - varDeb
            linha.Cells(12).Value = varSaldo
        Next

        Me.lbSaldo.Text = "Saldo: " & FormatNumber(varSaldo, 2).ToString
        If varSaldo >= 0 Then Me.lbSaldo.ForeColor = Color.Red Else Me.lbSaldo.ForeColor = Color.Blue

        FrmPrincipal.Visible = False

        Me.Tag = "1"

    End Sub

    Private Sub ribRetornar_Click(sender As Object, e As EventArgs) Handles ribRetornar.Click
        'Me.Validate()
        'Me.ExtratoBindingSource.EndEdit()
        'Me.TableAdapterManager1.UpdateAll(Me.SoloDataSet)

        Me.Close()

    End Sub

    Private Sub FrmExtrato_Closed(sender As Object, e As System.Windows.Forms.FormClosedEventArgs) Handles MyBase.FormClosed
        '"[IdCliente] = " & FrmPrincipal.RadGridView1.CurrentRow.Cells(0).Value.ToString
        fExecutaText("Update Contas Set VValor = Deb Where IdCliente = " & FrmPrincipal.RadGridView1.CurrentRow.Cells(0).Value & " And Deb > 0")
        fExecutaText("Update Contas Set VValor = Cred Where IdCliente = " & FrmPrincipal.RadGridView1.CurrentRow.Cells(0).Value & " And Cred > 0")
        FrmPrincipal.Visible = True

    End Sub

    Private Sub btnImprimir_Click(sender As Object, e As EventArgs) Handles btnImprimir.Click
        'Me.Validate()
        'Me.ExtratoBindingSource.EndEdit()
        'Me.TableAdapterManager1.UpdateAll(Me.SoloDataSet)

        'FrmExtratoRpt.Show()
        Me.Timer1.Enabled = True

    End Sub

    Private Sub update_extrato()
        On Error Resume Next

        Dim strFiltro As String

        strFiltro = "[IdCliente] = " & FrmPrincipal.RadGridView1.CurrentRow.Cells(0).Value.ToString

        If Me.ftPasta.Text <> "" Then
            strFiltro = strFiltro & " And Conta = " & Me.ftPasta.Text
        End If

        If Me.ftND.Text <> "" And Not Me.ftPsND.Checked Then
            strFiltro = strFiltro & " AND ND = '" & Me.ftND.Text & "'"
        End If

        If Me.ftPsND.Checked Then
            strFiltro = strFiltro & " AND ND IS NULL"
        End If

        If Me.ftHist.Text <> "" Then
            'strSQL = strSQL & " AND Ref LIKE " & Chr$(39) & "%" & Hist & "%" & Chr$(39)
            strFiltro = strFiltro & " AND Ref LIKE '%" & Me.ftHist.Text & "%'"
        End If

        Dim strSQL As String

        strSQL = ";WITH Saldos As (Select Dt, Id, DC, VValor, Deb, Cred, Conta, " & _
         "sum(isnull(Cred,0) - isnull(Deb,0)) over (order by dt, id) As Saldo FROM Contas " & _
         "WHERE " & strFiltro
        '        "WHERE (CodCliente = '" & Me.RadGridView1.CurrentRow.Cells(1).Value.ToString & "')"
        strSQL = strSQL & ")" & Chr(10) & "Update Contas SET Saldo = S.Saldo FROM Contas INNER JOIN Saldos As S " & Chr(10) & "ON Contas.Id = S.Id"

        Dim cnn As ADODB.Connection
        Dim cmd As ADODB.Command
        cnn = fGetAdoConnection()
        cmd = New ADODB.Command
        cmd.ActiveConnection = cnn
        cmd.CommandType = CommandType.Text
        cmd.CommandText = strSQL
        cmd.CommandTimeout = 0
        cmd.Execute()

        Me.ExtratoTableAdapter.Fill(Me.SoloDataSet.Extrato)
        'Me.ExtratoBindingSource.Filter = strFiltro
        Me.ExtratoBindingSource.Filter = Replace(strFiltro, "%", "*")

        Me.ExtratoBindingSource.MoveLast()
        Me.lbSaldo.Text = "Saldo: " & FormatNumber(Me.MasterTemplate.CurrentRow.Cells(12).Value, 2).ToString
        If Me.MasterTemplate.CurrentRow.Cells(12).Value >= 0 Then Me.lbSaldo.ForeColor = Color.Red Else Me.lbSaldo.ForeColor = Color.Blue
        Me.ExtratoBindingSource.MoveFirst()

    End Sub

    Private Sub ftPasta_Validated(sender As Object, e As EventArgs) Handles ftPasta.Validated

        If Me.Tag = "1" Then Call update_extrato_2()

    End Sub

    Private Sub ftHist_Validated(sender As Object, e As EventArgs) Handles ftHist.Validated

        If Me.Tag = "1" Then Call update_extrato_2()

    End Sub

    Private Sub ftND_Validated(sender As Object, e As EventArgs) Handles ftND.Validated

        If Me.Tag = "1" Then Call update_extrato_2()

    End Sub

    Private Sub ftPsND_CheckedChanged(sender As Object, e As EventArgs) Handles ftPsND.CheckedChanged

        'If Me.Tag = "1" Then Call update_extrato()

        Dim strFiltro As String

        strFiltro = "[IdCliente] = " & FrmPrincipal.RadGridView1.CurrentRow.Cells(0).Value.ToString

        If Me.ftPsND.Checked Then
            strFiltro = strFiltro & " AND ND IS NULL"
        End If

        If Me.MasterTemplate.FilterDescriptors.Expression <> "" Then
            strFiltro = strFiltro & " AND " & Me.MasterTemplate.FilterDescriptors.Expression
        End If

        Me.ExtratoBindingSource.Filter = strFiltro
        Dim varSaldo As Double = 0
        Dim varCred As Double = 0
        Dim varDeb As Double = 0
        For Each linha In MasterTemplate.Rows
            If IsDBNull(linha.Cells(10).Value) Then varDeb = 0 Else varDeb = linha.Cells(10).Value
            If IsDBNull(linha.Cells(11).Value) Then varCred = 0 Else varCred = linha.Cells(11).Value
            varSaldo = varSaldo + varCred - varDeb
            linha.Cells(12).Value = varSaldo
        Next

        Me.lbSaldo.Text = "Saldo: " & FormatNumber(varSaldo, 2).ToString
        If varSaldo >= 0 Then Me.lbSaldo.ForeColor = Color.Red Else Me.lbSaldo.ForeColor = Color.Blue

    End Sub

    Private Sub btnTransfere_Click(sender As Object, e As EventArgs) Handles btnTransfere.Click

        'MsgBox(Me.cboCliente.SelectedValue.ToString)
        If Me.cboCliente.Text = "" Then
            MsgBox("Por favor, selecione o cliente para transferir os lançamentos selecionados", vbCritical, "Atenção!")
            Me.cboCliente.Focus()
            Exit Sub
        End If

        If Me.MasterTemplate.SelectedRows.Count > 0 Then
            If MsgBox("Confirma transferência de " & Me.MasterTemplate.SelectedRows.Count & " lançamento(s) para o cliente " & vbLf & Me.cboCliente.Text & " ?", MsgBoxStyle.Information + MsgBoxStyle.YesNo + MsgBoxStyle.DefaultButton2, "Controle de Transferência de Lançamentos") = vbYes Then

                'For i As Integer = 0 To Me.MasterTemplate.SelectedRows.Count - 1
                '    Dim row As Telerik.WinControls.UI.GridViewRowInfo = Me.MasterTemplate.SelectedRows(i)
                '    row.Cells(0).Value = 1
                'Next

                'Dim db As New ContasDataContext
                'Dim query = (From tb In db.GetTable(Of Chaves)() Select tb Where tb.Ref = "Transferência de lançamentos")
                'Dim varSenha = (From c In query Select c.Chave).First

                'If InputBox("Informe a senha:", "Controle de Transferência de Lançamentos", "******") <> varSenha Then
                '    MsgBox("Senha incorreta", vbCritical, "")
                '    Exit Sub
                'End If

                'If Me.MasterTemplate.SelectedRows.Count = 1 Then
                '    If MsgBox("O lançamento será transferido agora para o cliente " & vbLf & Me.cboCliente.Text & ";" & vbLf & "Confirma?", vbYesNo Or vbQuestion Or vbDefaultButton2, "Controle de Transferência de Lançamentos") = vbNo Then Exit Sub
                'Else
                '    If MsgBox("Os " & Me.MasterTemplate.SelectedRows.Count & " lançamentos serão transferidos agora para o cliente " & vbLf & Me.cboCliente.Text & ";" & vbLf & "Confirma?", vbYesNo Or vbQuestion Or vbDefaultButton2, "Controle de Transferência de Lançamentos") = vbNo Then Exit Sub
                'End If

                'Me.Validate()
                'Me.ExtratoBindingSource.EndEdit()
                'Me.TableAdapterManager1.UpdateAll(Me.SoloDataSet)

                'Dim query2 = (From tb2 In db.GetTable(Of Clientes)() Select tb2 Where tb2.Id = Me.cboCliente.SelectedValue.ToString)
                'Dim varCod = (From c In query2 Select c.Código).First

                'Dim strSQL As String
                'strSQL = "UPDATE Contas SET IdCliente = " & Me.cboCliente.SelectedValue.ToString & ", CodCliente = " & varCod & " WHERE Selec = 1; UPDATE Contas SET Selec = 0 WHERE Selec = 1"
                'Dim cnn As ADODB.Connection
                'Dim cmd As ADODB.Command
                'cnn = fGetAdoConnection()
                'cmd = New ADODB.Command
                'cmd.ActiveConnection = cnn
                'cmd.CommandType = CommandType.Text
                'cmd.CommandText = strSQL
                'cmd.CommandTimeout = 0
                'cmd.Execute()

                'Me.ExtratoTableAdapter.Fill(Me.SoloDataSet.Extrato)
                'Me.ExtratoBindingSource.Filter = "[IdCliente] = " & FrmPrincipal.RadGridView1.CurrentRow.Cells(0).Value.ToString

                'Me.cboCliente.Visible = False
                'Me.lblTransfere.Visible = False
                'Me.btnTransfere.Visible = False
                'MsgBox("Transferência realizada com sucesso!", vbInformation)
                varSenha = "3"
                frmSenha.ShowDialog()
            End If
        End If
        'Me.RadGridView1.Rows(1).IsSelected = True

    End Sub

    Private Sub FrmExtrato_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles Me.KeyDown

        If e.KeyCode = Keys.F9 Then
            If Me.btnTransfere.Visible Then
                Me.btnTransfere.Visible = False
                Me.cboCliente.Visible = False
                Me.lblTransfere.Visible = False
            Else
                Me.btnTransfere.Visible = True
                Me.cboCliente.Visible = True
                Me.lblTransfere.Visible = True
                Me.cboCliente.Text = ""
            End If
        ElseIf e.KeyCode = Keys.F10 Then
            If Me.MasterTemplate.Columns(1).ReadOnly Then
                'Dim db As New ContasDataContext
                'Dim query = (From tb In db.GetTable(Of Chaves)() Select tb Where tb.Ref = "Desbloquear lançamentos")
                'Dim varSenha = (From c In query Select c.Chave).First
                'If InputBox("Informe a senha:", "Controle de Lançamentos", "******") <> varSenha Then
                '    MsgBox("Senha incorreta", vbCritical, "")
                'Else
                '    Me.MasterTemplate.Columns(1).ReadOnly = False
                '    Me.MasterTemplate.Columns(5).ReadOnly = False
                '    Me.MasterTemplate.Columns(10).ReadOnly = False
                '    Me.MasterTemplate.Columns(11).ReadOnly = False
                '    Me.MasterTemplate.AllowDeleteRow = True
                'End If
                varSenha = "4"
                frmSenha.ShowDialog()
            Else
                'Me.ftPasta.Focus()
                'Me.Validate()
                'Me.ExtratoBindingSource.EndEdit()
                'Me.TableAdapterManager1.UpdateAll(Me.SoloDataSet)
                'Dim x As Integer = RadGridView1.CurrentRow.Cells(2).Value
                'Dim y As String = RadGridView1.CurrentRow.Cells(9).Value.ToString
                'If y = "D" Then fExecutaText("Update Contas Set VValor = Deb Where Id = " & x) Else fExecutaText("Update Contas Set VValor = Cred Where Id = " & x)
                Call update_extrato_2()
                MsgBox("Campos bloqueados")
                Me.MasterTemplate.Columns(1).ReadOnly = True
                Me.MasterTemplate.Columns(5).ReadOnly = True
                Me.MasterTemplate.Columns(10).ReadOnly = True
                Me.MasterTemplate.Columns(11).ReadOnly = True
                Me.MasterTemplate.AllowDeleteRow = False
            End If
        ElseIf e.KeyCode = Keys.Enter Then
            SendKeys.Send("{Tab}")
        End If

    End Sub

    Private Sub MasterTemplate_FilterExpressionChanged(sender As Object, e As Telerik.WinControls.UI.FilterExpressionChangedEventArgs) Handles MasterTemplate.FilterExpressionChanged

        Dim strFiltro As String = "[IdCliente] = " & FrmPrincipal.RadGridView1.CurrentRow.Cells(0).Value.ToString

        If Me.ftPsND.Checked Then
            strFiltro = strFiltro & " AND ND IS NULL"
        End If

        If Me.MasterTemplate.FilterDescriptors.Expression <> "" Then
            strFiltro = strFiltro & " AND " & Me.MasterTemplate.FilterDescriptors.Expression
        End If

        Me.ExtratoBindingSource.Filter = strFiltro

        Dim varSaldo As Double = 0
        Dim varCred As Double = 0
        Dim varDeb As Double = 0
        For Each linha In MasterTemplate.Rows
            If IsDBNull(linha.Cells(10).Value) Then varDeb = 0 Else varDeb = linha.Cells(10).Value
            If IsDBNull(linha.Cells(11).Value) Then varCred = 0 Else varCred = linha.Cells(11).Value
            varSaldo = varSaldo + varCred - varDeb
            linha.Cells(12).Value = varSaldo
        Next

        Me.lbSaldo.Text = "Saldo: " & FormatNumber(varSaldo, 2).ToString
        If varSaldo >= 0 Then Me.lbSaldo.ForeColor = Color.Red Else Me.lbSaldo.ForeColor = Color.Blue

    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click

        Dim varSaldo As Double = 0
        Dim varCred As Double = 0
        Dim varDeb As Double = 0

        For Each linha In MasterTemplate.Rows
            If IsDBNull(linha.Cells(10).Value) Then varDeb = 0 Else varDeb = linha.Cells(10).Value
            If IsDBNull(linha.Cells(11).Value) Then varCred = 0 Else varCred = linha.Cells(11).Value
            varSaldo = varSaldo + varCred - varDeb
        Next

        Me.lbSaldo.Text = "Saldo: " & FormatNumber(varSaldo, 2).ToString
        If varSaldo >= 0 Then Me.lbSaldo.ForeColor = Color.Red Else Me.lbSaldo.ForeColor = Color.Blue

        'MsgBox(Me.MasterTemplate.FilterDescriptors.Expression)
        Dim strFiltro As String

        strFiltro = "[IdCliente] = " & FrmPrincipal.RadGridView1.CurrentRow.Cells(0).Value.ToString

        If Me.ftPsND.Checked Then
            strFiltro = strFiltro & " AND ND IS NULL"
        End If

        If Me.MasterTemplate.FilterDescriptors.Expression <> "" Then
            strFiltro = strFiltro & " AND " & Me.MasterTemplate.FilterDescriptors.Expression
        End If

        MsgBox(strFiltro)

    End Sub

    Private Sub update_extrato_2()

        Dim strFiltro As String = "[IdCliente] = " & FrmPrincipal.RadGridView1.CurrentRow.Cells(0).Value.ToString

        If Me.ftPsND.Checked Then
            strFiltro = strFiltro & " AND ND IS NULL"
        End If

        If Me.MasterTemplate.FilterDescriptors.Expression <> "" Then
            strFiltro = strFiltro & " AND " & Me.MasterTemplate.FilterDescriptors.Expression
        End If

        Me.ExtratoBindingSource.Filter = strFiltro

        Dim varSaldo As Double = 0
        Dim varCred As Double = 0
        Dim varDeb As Double = 0
        For Each linha In MasterTemplate.Rows
            If IsDBNull(linha.Cells(10).Value) Then varDeb = 0 Else varDeb = linha.Cells(10).Value
            If IsDBNull(linha.Cells(11).Value) Then varCred = 0 Else varCred = linha.Cells(11).Value
            varSaldo = varSaldo + varCred - varDeb
            linha.Cells(12).Value = varSaldo
        Next

        Me.lbSaldo.Text = "Saldo: " & FormatNumber(varSaldo, 2).ToString
        If varSaldo >= 0 Then Me.lbSaldo.ForeColor = Color.Red Else Me.lbSaldo.ForeColor = Color.Blue

    End Sub

    Private Sub btnDown_Click(sender As Object, e As EventArgs) Handles btnDown.Click
        On Error Resume Next

        'MsgBox(Me.MasterTemplate.FilterDescriptors.Expression)

        'For Each linha In Me.MasterTemplate.Rows
        '    linha.IsSelected = True
        '    Exit For
        'Next

        'Me.ftND.Focus()
        SendKeys.Send("{ESC}")
        'Application.DoEvents()
        'SendKeys.Send("{Tab}")
        'Application.DoEvents()
        'SendKeys.Send("{Tab}")
        'Application.DoEvents()

        'Me.ExtratoBindingSource.MoveLast()
        'SendKeys.Send("{Tab 5}")
        'Me.MasterTemplate.Focus()
        'SendKeys.Send("{Enter}")
        'Me.MasterTemplate.Rows(0).IsSelected = True
        'Me.ExtratoBindingSource.MoveFirst()
        'Me.MasterTemplate.ShowFilteringRow = False
        'Application.DoEvents()
        'Me.MasterTemplate.Focus()
        'Me.ExtratoBindingSource.MoveLast()
        'Me.MasterTemplate.ShowFilteringRow = True
        'Me.ExtratoBindingSource.Position = 10

        'Me.MasterTemplate.Rows(2).IsSelected = True
        'Dim tempPos As Point
        'Dim R As Long = GetCursorPos(tempPos)
        'MsgBox(R.ToString)

        'mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0)
        'mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0)

        Me.ExtratoBindingSource.MoveLast()
        ' Me.MasterTemplate.ShowFilteringRow = True
        'Me.Timer1.Enabled = True

    End Sub

    Private Sub btnUp_Click(sender As Object, e As EventArgs) Handles btnUp.Click
        On Error Resume Next

        SendKeys.Send("{ESC}")
        'SendKeys.Send("{Tab}")
        'SendKeys.Send("{Tab}")
        'Me.MasterTemplate.ShowFilteringRow = True
        'Me.ExtratoBindingSource.MoveFirst()
        'Me.MasterTemplate.ShowFilteringRow = True
        'Me.MasterTemplate.Focus()
        Me.ExtratoBindingSource.MoveFirst()

    End Sub

    Private Sub x(sender As Object, e As Telerik.WinControls.UI.GridViewCellEventArgs) Handles MasterTemplate.CellEndEdit
        On Error Resume Next

        'MsgBox(Me.MasterTemplate.CurrentColumn.HeaderText & vbCrLf & Me.MasterTemplate.CurrentRow.Cells(6).Value.ToString)
        If Me.MasterTemplate.CurrentColumn.HeaderText = "N. D." Then
            fExecutaText("Update Contas Set ND = '" & Me.MasterTemplate.CurrentRow.Cells(6).Value.ToString & "' Where Id = " & Me.MasterTemplate.CurrentRow.Cells(2).Value)
        ElseIf Me.MasterTemplate.CurrentColumn.HeaderText = "Histórico" Then
            fExecutaText("Update Contas Set Ref = '" & Me.MasterTemplate.CurrentRow.Cells(7).Value.ToString & "' Where Id = " & Me.MasterTemplate.CurrentRow.Cells(2).Value)
        ElseIf Me.MasterTemplate.CurrentColumn.HeaderText = "Data" Then
            fExecutaText("Update Contas Set Dt = '" & Me.MasterTemplate.CurrentRow.Cells(1).Value.ToString & "' Where Id = " & Me.MasterTemplate.CurrentRow.Cells(2).Value)
        ElseIf Me.MasterTemplate.CurrentColumn.HeaderText = "Pasta" Then
            fExecutaText("Update Contas Set Conta = " & Me.MasterTemplate.CurrentRow.Cells(5).Value & " Where Id = " & Me.MasterTemplate.CurrentRow.Cells(2).Value)
        ElseIf Me.MasterTemplate.CurrentColumn.HeaderText = "Débito" Then
            fExecutaText("Update Contas Set Deb = " & Replace(Me.MasterTemplate.CurrentRow.Cells(10).Value.ToString, ",", ".") & " Where Id = " & Me.MasterTemplate.CurrentRow.Cells(2).Value)
        ElseIf Me.MasterTemplate.CurrentColumn.HeaderText = "Crédito" Then
            fExecutaText("Update Contas Set Cred = " & Replace(Me.MasterTemplate.CurrentRow.Cells(11).Value.ToString, ",", ".") & " Where Id = " & Me.MasterTemplate.CurrentRow.Cells(2).Value)
        End If

    End Sub

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        'Me.ExtratoBindingSource.MoveLast()
        ''SendKeys.Send("{Tab}")
        ''SendKeys.Send("{Tab}")
        ''SendKeys.Send("{Tab}")
        ''SendKeys.Send("{Tab}")
        ''SendKeys.Send("{Tab}")
        'Me.MasterTemplate.Rows(0).IsSelected = True
        'Me.ExtratoBindingSource.MoveFirst()
        'Me.ExtratoBindingSource.MoveLast()
        'Me.Timer1.Enabled = False
        On Error Resume Next

        Me.Timer1.Enabled = False

        Me.CircularProgress1.Visible = True
        Me.CircularProgress1.IsRunning = True

        Me.Validate()
        Me.ExtratoBindingSource.EndEdit()
        Me.TableAdapterManager1.UpdateAll(Me.SoloDataSet)

        FrmExtratoRpt.Show()

    End Sub

    'Private Sub MasterTemplate_CellEndEdit(sender As Object, e As Telerik.WinControls.UI.GridViewCellEventArgs) Handles MasterTemplate.CellEndEdit
    '    'Dim x As Integer = RadGridView1.CurrentRow.Cells(2).Value
    '    'Dim y As String = RadGridView1.CurrentRow.Cells(9).Value.ToString
    '    If MasterTemplate.CurrentRow.Cells(9).Value.ToString = "D" Then MasterTemplate.CurrentRow.Cells(8).Value = MasterTemplate.CurrentRow.Cells(10).Value Else MasterTemplate.CurrentRow.Cells(8).Value = MasterTemplate.CurrentRow.Cells(11).Value

    'End Sub

End Class
