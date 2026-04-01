Public Class FrmExtratoRpt


    Private Sub FrmExtratoRpt_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        Dim strFiltro As String

        strFiltro = "[IdCliente] = " & FrmPrincipal.RadGridView1.CurrentRow.Cells(0).Value.ToString

        'If FrmExtrato.ftPasta.Text <> "" Then
        '    strFiltro = strFiltro & " And Conta = " & FrmExtrato.ftPasta.Text
        'End If

        'If FrmExtrato.ftND.Text <> "" And Not FrmExtrato.ftPsND.Checked Then
        '    strFiltro = strFiltro & " AND ND = '" & FrmExtrato.ftND.Text & "'"
        'End If

        If FrmExtrato.ftPsND.Checked Then
            strFiltro = strFiltro & " AND ND IS NULL"
        End If

        If FrmExtrato.MasterTemplate.FilterDescriptors.Expression <> "" Then
            strFiltro = strFiltro & " AND " & FrmExtrato.MasterTemplate.FilterDescriptors.Expression
        End If

        'If FrmExtrato.ftHist.Text <> "" Then
        '    'strSQL = strSQL & " AND Ref LIKE " & Chr$(39) & "%" & Hist & "%" & Chr$(39)
        '    strFiltro = strFiltro & " AND Ref LIKE '%" & FrmExtrato.ftHist.Text & "%'"
        'End If

        Dim strSql As String
        'strSql = "SELECT Dt, Id, IdCliente, CodCliente, Conta, Ref, VValor, DC, ND, Saldo, Deb AS Débito, Cred AS Crédito FROM Contas Where IdCliente = " & FrmPrincipal.RadGridView1.CurrentRow.Cells(0).Value.ToString & " Order By Dt, Id"
        strSql = "SELECT Dt, Id, IdCliente, CodCliente, Conta, Ref, VValor, DC, ND, Saldo, '" & FrmExtrato.lbCliente.Text & "' As Titulo, Deb AS Débito, Cred AS Crédito FROM Contas Where " & strFiltro & " Order By Dt, Id"

        Me.C1Report1.DataSource.RecordSource = strSql
        Me.C1PrintPreviewControl1.Document = ""
        Me.C1PrintPreviewControl1.Document = C1Report1.C1Document
        Me.C1PrintPreviewControl1.Refresh()

        FrmExtrato.Visible = False

    End Sub

    Private Sub ribRetornar_Click(sender As Object, e As EventArgs) Handles ribRetornar.Click
        Me.Close()

    End Sub

    Private Sub FrmExtratoRpt_Closed(sender As Object, e As System.Windows.Forms.FormClosedEventArgs) Handles MyBase.FormClosed
        FrmExtrato.Visible = True
        FrmExtrato.CircularProgress1.Visible = False
        FrmExtrato.CircularProgress1.IsRunning = False

    End Sub

    Private Sub C1Report1_PrintSection(sender As System.Object, e As C1.C1Report.ReportEventArgs) Handles C1Report1.PrintSection

        'Dim f As C1.C1Report.Field

        'For Each f In C1Report1.Fields
        '    If f.Name = "S" Then
        '        MsgBox(f.Index.ToString)
        '    End If
        'Next

        'MsgBox(C1Report1.Fields(1).Name.ToString)

        If Me.C1Report1.Fields(15).Value >= 0 Then
            Me.C1Report1.Fields(15).ForeColor = ColorTranslator.FromOle(RGB(255, 0, 0)) 'RGB(255, 0, 0)
        Else
            Me.C1Report1.Fields(15).ForeColor = ColorTranslator.FromHtml("#0000ff")
        End If

    End Sub
End Class
