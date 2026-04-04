Imports Telerik.WinControls.UI

Public Class frmReports

    Private Sub frmReports_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        FrmPrincipal.Visible = False

    End Sub

    'Private Sub ribRetornar_Click(sender As Object, e As EventArgs) Handles ribRetornar.Click
    '    Me.Close()

    'End Sub

    Private Sub frmReports_Closed(sender As Object, e As System.Windows.Forms.FormClosedEventArgs) Handles MyBase.FormClosed
        FrmPrincipal.Visible = True

    End Sub

    'Private Sub btnTotais_Click(sender As Object, e As EventArgs) Handles btnTotais.Click
    '    Dim cn As New ADODB.Connection()
    '    Dim rs As New ADODB.Recordset()
    '    Dim cnStr As String = strAdoConnection()

    '    Dim varVal As Double = 0

    '    If Me.Op3.Checked Then
    '        If Me.Op21.Checked Then
    '            rs.Open("Select Sum(Saldo) As S From (SELECT a.CodCliente, dbo.Clientes.Cliente, a.TD - a.TC AS Saldo FROM (SELECT CodCliente, SUM(TotDeb) AS TD, SUM(TotCred) AS TC FROM (SELECT CodCliente, CASE DC WHEN 'D' THEN VValor ELSE 0 END AS TotDeb, CASE DC WHEN 'C' THEN VValor ELSE 0 END AS TotCred FROM (SELECT IdCliente, CodCliente, Conta, Dt, Ref, VValor, DC, ND, Saldo, Deb, Cred, IdLote FROM Contas WHERE Dt <= '" & Me.Dt.Value.ToString("yyyy-dd-MM") & "') AS X) AS C GROUP BY CodCliente) AS a INNER JOIN dbo.Clientes ON a.CodCliente = dbo.Clientes.Código WHERE (a.TD > a.TC) And (a.TD - a.TC) > " & Me.Saldo.Value & ") As z", cnStr, ADODB.CursorTypeEnum.adOpenKeyset, ADODB.LockTypeEnum.adLockOptimistic)
    '            If IsDBNull(rs("S").Value) Then varVal = 0 Else varVal = rs("S").Value
    '            MsgBox("Total Devedores: " & FormatCurrency(varVal, 2), MsgBoxStyle.Information, "Banco de dados inteiro")
    '        ElseIf Me.Op22.Checked Then
    '            rs.Open("Select Sum(Saldo) As S From (SELECT a.CodCliente, dbo.Clientes.Cliente, a.TD - a.TC AS Saldo FROM (SELECT CodCliente, SUM(TotDeb) AS TD, SUM(TotCred) AS TC FROM (SELECT CodCliente, CASE DC WHEN 'D' THEN VValor ELSE 0 END AS TotDeb, CASE DC WHEN 'C' THEN VValor ELSE 0 END AS TotCred FROM (SELECT IdCliente, CodCliente, Conta, Dt, Ref, VValor, DC, ND, Saldo, Deb, Cred, IdLote FROM Contas WHERE Dt <= '" & Me.Dt.Value.ToString("yyyy-dd-MM") & "') AS X) AS C GROUP BY CodCliente) AS a INNER JOIN dbo.Clientes ON a.CodCliente = dbo.Clientes.Código WHERE (a.TD > a.TC) And a.CodCliente >= 10000 And (a.TD - a.TC) > " & Me.Saldo.Value & ") As z", cnStr, ADODB.CursorTypeEnum.adOpenKeyset, ADODB.LockTypeEnum.adLockOptimistic)
    '            If IsDBNull(rs("S").Value) Then varVal = 0 Else varVal = rs("S").Value
    '            MsgBox("Total Devedores: " & FormatCurrency(varVal, 2), MsgBoxStyle.Information, "A partir de 10000")
    '        Else
    '            rs.Open("Select Sum(Saldo) As S From (SELECT a.CodCliente, dbo.Clientes.Cliente, a.TD - a.TC AS Saldo FROM (SELECT CodCliente, SUM(TotDeb) AS TD, SUM(TotCred) AS TC FROM (SELECT CodCliente, CASE DC WHEN 'D' THEN VValor ELSE 0 END AS TotDeb, CASE DC WHEN 'C' THEN VValor ELSE 0 END AS TotCred FROM (SELECT IdCliente, CodCliente, Conta, Dt, Ref, VValor, DC, ND, Saldo, Deb, Cred, IdLote FROM Contas WHERE Dt <= '" & Me.Dt.Value.ToString("yyyy-dd-MM") & "') AS X) AS C GROUP BY CodCliente) AS a INNER JOIN dbo.Clientes ON a.CodCliente = dbo.Clientes.Código WHERE (a.TD > a.TC) And a.CodCliente < 10000 And (a.TD - a.TC) > " & Me.Saldo.Value & ") As z", cnStr, ADODB.CursorTypeEnum.adOpenKeyset, ADODB.LockTypeEnum.adLockOptimistic)
    '            If IsDBNull(rs("S").Value) Then varVal = 0 Else varVal = rs("S").Value
    '            MsgBox("Total Devedores: " & FormatCurrency(varVal, 2), MsgBoxStyle.Information, "Abaixo de 10000")
    '        End If
    '    End If

    '    If Me.Op4.Checked Then
    '        If Me.Op21.Checked Then
    '            rs.Open("Select Sum(Saldo) As S From (SELECT a.CodCliente, dbo.Clientes.Cliente, a.TC - a.TD AS Saldo FROM (SELECT CodCliente, SUM(TotDeb) AS TD, SUM(TotCred) AS TC FROM (SELECT CodCliente, CASE DC WHEN 'D' THEN VValor ELSE 0 END AS TotDeb, CASE DC WHEN 'C' THEN VValor ELSE 0 END AS TotCred FROM (SELECT IdCliente, CodCliente, Conta, Dt, Ref, VValor, DC, ND, Saldo, Deb, Cred, IdLote FROM Contas WHERE Dt <= '" & Me.Dt.Value.ToString("yyyy-dd-MM") & "') AS X) AS C GROUP BY CodCliente) AS a INNER JOIN dbo.Clientes ON a.CodCliente = dbo.Clientes.Código WHERE (a.TC > a.TD) And (a.TC - a.TD) > " & Me.Saldo.Value & ") As z", cnStr, ADODB.CursorTypeEnum.adOpenKeyset, ADODB.LockTypeEnum.adLockOptimistic)
    '            If IsDBNull(rs("S").Value) Then varVal = 0 Else varVal = rs("S").Value
    '            MsgBox("Total Credores: " & FormatCurrency(varVal, 2), MsgBoxStyle.Information, "Banco de dados inteiro")
    '        ElseIf Me.Op22.Checked Then
    '            rs.Open("Select Sum(Saldo) As S From (SELECT a.CodCliente, dbo.Clientes.Cliente, a.TC - a.TD AS Saldo FROM (SELECT CodCliente, SUM(TotDeb) AS TD, SUM(TotCred) AS TC FROM (SELECT CodCliente, CASE DC WHEN 'D' THEN VValor ELSE 0 END AS TotDeb, CASE DC WHEN 'C' THEN VValor ELSE 0 END AS TotCred FROM (SELECT IdCliente, CodCliente, Conta, Dt, Ref, VValor, DC, ND, Saldo, Deb, Cred, IdLote FROM Contas WHERE Dt <= '" & Me.Dt.Value.ToString("yyyy-dd-MM") & "') AS X) AS C GROUP BY CodCliente) AS a INNER JOIN dbo.Clientes ON a.CodCliente = dbo.Clientes.Código WHERE (a.TC > a.TD) And a.CodCliente >= 10000 And (a.TC - a.TD) > " & Me.Saldo.Value & ") As z", cnStr, ADODB.CursorTypeEnum.adOpenKeyset, ADODB.LockTypeEnum.adLockOptimistic)
    '            If IsDBNull(rs("S").Value) Then varVal = 0 Else varVal = rs("S").Value
    '            MsgBox("Total Credores: " & FormatCurrency(varVal, 2), MsgBoxStyle.Information, "A partir de 10000")
    '        Else
    '            rs.Open("Select Sum(Saldo) As S From (SELECT a.CodCliente, dbo.Clientes.Cliente, a.TC - a.TD AS Saldo FROM (SELECT CodCliente, SUM(TotDeb) AS TD, SUM(TotCred) AS TC FROM (SELECT CodCliente, CASE DC WHEN 'D' THEN VValor ELSE 0 END AS TotDeb, CASE DC WHEN 'C' THEN VValor ELSE 0 END AS TotCred FROM (SELECT IdCliente, CodCliente, Conta, Dt, Ref, VValor, DC, ND, Saldo, Deb, Cred, IdLote FROM Contas WHERE Dt <= '" & Me.Dt.Value.ToString("yyyy-dd-MM") & "') AS X) AS C GROUP BY CodCliente) AS a INNER JOIN dbo.Clientes ON a.CodCliente = dbo.Clientes.Código WHERE (a.TC > a.TD) And a.CodCliente < 10000 And (a.TC - a.TD) > " & Me.Saldo.Value & ") As z", cnStr, ADODB.CursorTypeEnum.adOpenKeyset, ADODB.LockTypeEnum.adLockOptimistic)
    '            If IsDBNull(rs("S").Value) Then varVal = 0 Else varVal = rs("S").Value
    '            MsgBox("Total Credores: " & FormatCurrency(varVal, 2), MsgBoxStyle.Information, "Abaixo de 10000")
    '        End If
    '    End If

    '    rs.Close()


    'End Sub

    'Private Sub btnRibUndo_Click(sender As Object, e As EventArgs) Handles btnRibUndo.Click

    '    If Me.Op3.Checked Then  'devedores

    '        frmRptDevedores1.Show()


    '    Else    'credores

    '        frmCredores1.Show()

    '    End If

    'End Sub

    Private Sub frmReports_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles Me.KeyDown

        Select Case e.KeyCode
            Case Keys.Enter
                SendKeys.Send("{Tab}")

        End Select

    End Sub

End Class
