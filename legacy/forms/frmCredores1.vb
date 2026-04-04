Public Class frmCredores1


    Private Sub frmCredores1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        frmReports.Visible = False

        Dim strSql As String

        If frmReports.Op21.Checked Then
            strSql = "SELECT a.CodCliente, dbo.Clientes.Cliente, a.TC - a.TD AS Saldo FROM (SELECT CodCliente, SUM(TotDeb) AS TD, SUM(TotCred) AS TC FROM (SELECT CodCliente, CASE DC WHEN 'D' THEN VValor ELSE 0 END AS TotDeb, CASE DC WHEN 'C' THEN VValor ELSE 0 END AS TotCred FROM (SELECT IdCliente, CodCliente, Conta, Dt, Ref, VValor, DC, ND, Saldo, Deb, Cred, IdLote FROM Contas WHERE Dt <= '" & frmReports.Dt.Value.ToString("yyyy-dd-MM") & "') AS X) AS C GROUP BY CodCliente) AS a INNER JOIN dbo.Clientes ON a.CodCliente = dbo.Clientes.Código WHERE (a.TC > a.TD) And (a.TC - a.TD) > " & frmReports.Saldo.Value
        ElseIf frmReports.Op22.Checked Then
            strSql = "SELECT a.CodCliente, dbo.Clientes.Cliente, a.TC - a.TD AS Saldo FROM (SELECT CodCliente, SUM(TotDeb) AS TD, SUM(TotCred) AS TC FROM (SELECT CodCliente, CASE DC WHEN 'D' THEN VValor ELSE 0 END AS TotDeb, CASE DC WHEN 'C' THEN VValor ELSE 0 END AS TotCred FROM (SELECT IdCliente, CodCliente, Conta, Dt, Ref, VValor, DC, ND, Saldo, Deb, Cred, IdLote FROM Contas WHERE Dt <= '" & frmReports.Dt.Value.ToString("yyyy-dd-MM") & "') AS X) AS C GROUP BY CodCliente) AS a INNER JOIN dbo.Clientes ON a.CodCliente = dbo.Clientes.Código WHERE (a.TC > a.TD) And a.CodCliente >= 10000 And (a.TC - a.TD) > " & frmReports.Saldo.Value
        Else
            strSql = "SELECT a.CodCliente, dbo.Clientes.Cliente, a.TC - a.TD AS Saldo FROM (SELECT CodCliente, SUM(TotDeb) AS TD, SUM(TotCred) AS TC FROM (SELECT CodCliente, CASE DC WHEN 'D' THEN VValor ELSE 0 END AS TotDeb, CASE DC WHEN 'C' THEN VValor ELSE 0 END AS TotCred FROM (SELECT IdCliente, CodCliente, Conta, Dt, Ref, VValor, DC, ND, Saldo, Deb, Cred, IdLote FROM Contas WHERE Dt <= '" & frmReports.Dt.Value.ToString("yyyy-dd-MM") & "') AS X) AS C GROUP BY CodCliente) AS a INNER JOIN dbo.Clientes ON a.CodCliente = dbo.Clientes.Código WHERE (a.TC > a.TD) And a.CodCliente < 10000 And (a.TC - a.TD) > " & frmReports.Saldo.Value
        End If

        If frmReports.OpOrd1.Checked Then
            Me.C1Report1.Groups(0).GroupBy = "CodCliente"
        Else
            Me.C1Report1.Groups(0).GroupBy = "Cliente"
        End If

        Me.C1Report1.Groups(0).Sort = C1.C1Report.SortEnum.Ascending
        Me.C1Report1.DataSource.RecordSource = strSql
        Me.C1PrintPreviewControl1.Document = ""
        Me.C1PrintPreviewControl1.Document = C1Report1.C1Document
        Me.C1PrintPreviewControl1.Refresh()

    End Sub

    Private Sub ribRetornar_Click(sender As Object, e As EventArgs) Handles ribRetornar.Click
        Me.Close()

    End Sub

    Private Sub frmCredores1_Closed(sender As Object, e As System.Windows.Forms.FormClosedEventArgs) Handles MyBase.FormClosed
        frmReports.Visible = True

    End Sub

    Private Sub C1Report1_PrintSection(sender As System.Object, e As C1.C1Report.ReportEventArgs) Handles C1Report1.PrintSection

        Me.C1Report1.Fields("rtTitulo").Text = "Clientes credores em " & frmReports.Dt.Value.ToShortDateString & " com saldo maior ou igual a " & FormatCurrency(frmReports.Saldo.Value, 2)
        If frmReports.Op21.Checked Then Me.C1Report1.Fields("rtTitulo2").Text = "Banco de dados inteiro"

        'Dim f As C1.C1Report.Field

        'For Each f In C1Report1.Fields
        '    If f.Name = "S" Then
        '        MsgBox(f.Index.ToString)
        '    End If
        'Next

        'MsgBox(C1Report1.Fields(1).Name.ToString)

        'If Me.C1Report1.Fields(15).Value >= 0 Then
        '    Me.C1Report1.Fields(15).ForeColor = ColorTranslator.FromOle(RGB(255, 0, 0)) 'RGB(255, 0, 0)
        'Else
        '    Me.C1Report1.Fields(15).ForeColor = ColorTranslator.FromHtml("#0000ff")
        'End If

    End Sub
End Class
