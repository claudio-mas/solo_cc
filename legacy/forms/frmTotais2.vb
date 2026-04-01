Imports Telerik.WinControls.UI

Public Class frmTotais2

    Private Sub FrmTotais_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        Dim cn As New ADODB.Connection()
        Dim rs As New ADODB.Recordset()
        Dim cnStr As String = strAdoConnection()
        rs.Open("SELECT COUNT(CodCliente) AS QtdeC FROM (SELECT CodCliente, SUM(TotDeb) AS TD, SUM(TotCred) AS TC FROM (SELECT CodCliente, CASE DC WHEN 'D' THEN VValor ELSE 0 END AS TotDeb, CASE DC WHEN 'C' THEN VValor ELSE 0 END AS TotCred FROM (SELECT IdCliente, CodCliente, Conta, Dt, Ref, VValor, DC, ND, Saldo, Deb, Cred, IdLote FROM Contas WHERE Dt <= '" & Date.Today.ToString("yyyy-dd-MM") & "') AS X) AS C GROUP BY CodCliente) AS a WHERE (TC > TD)", cnStr, ADODB.CursorTypeEnum.adOpenKeyset, ADODB.LockTypeEnum.adLockOptimistic)
        Me.C1.Text = FormatNumber(rs("QtdeC").Value, 0)
        rs.Close()

        rs.Open("SELECT SUM(TC) - SUM(TD) AS TotC FROM (SELECT CodCliente, SUM(TotDeb) AS TD, SUM(TotCred) AS TC FROM (SELECT CodCliente, CASE DC WHEN 'D' THEN VValor ELSE 0 END AS TotDeb, CASE DC WHEN 'C' THEN VValor ELSE 0 END AS TotCred FROM (SELECT IdCliente, CodCliente, Conta, Dt, Ref, VValor, DC, ND, Saldo, Deb, Cred, IdLote FROM Contas WHERE Dt <= '" & Date.Today.ToString("yyyy-dd-MM") & "') as x) AS C GROUP BY CodCliente) AS a WHERE (TC > TD)", cnStr, ADODB.CursorTypeEnum.adOpenKeyset, ADODB.LockTypeEnum.adLockOptimistic)
        Me.C2.Text = FormatCurrency(rs("TotC").Value, 2)
        rs.Close()

        rs.Open("SELECT COUNT(CodCliente) AS QtdeD FROM (SELECT CodCliente, SUM(TotDeb) AS TD, SUM(TotCred) AS TC FROM (SELECT CodCliente, CASE DC WHEN 'D' THEN VValor ELSE 0 END AS TotDeb, CASE DC WHEN 'C' THEN VValor ELSE 0 END AS TotCred FROM (SELECT IdCliente, CodCliente, Conta, Dt, Ref, VValor, DC, ND, Saldo, Deb, Cred, IdLote FROM Contas WHERE Dt <= '" & Date.Today.ToString("yyyy-dd-MM") & "') as x) AS C GROUP BY CodCliente) AS a WHERE (TD > TC)", cnStr, ADODB.CursorTypeEnum.adOpenKeyset, ADODB.LockTypeEnum.adLockOptimistic)
        Me.D1.Text = FormatNumber(rs("QtdeD").Value, 0)
        rs.Close()

        rs.Open("SELECT SUM(TD) - SUM(TC) AS TotD FROM (SELECT CodCliente, SUM(TotDeb) AS TD, SUM(TotCred) AS TC FROM  (SELECT CodCliente, CASE DC WHEN 'D' THEN VValor ELSE 0 END AS TotDeb, CASE DC WHEN 'C' THEN VValor ELSE 0 END AS TotCred FROM (SELECT IdCliente, CodCliente, Conta, Dt, Ref, VValor, DC, ND, Saldo, Deb, Cred, IdLote FROM Contas WHERE Dt <= '" & Date.Today.ToString("yyyy-dd-MM") & "') as x) AS C GROUP BY CodCliente) AS a WHERE (TD > TC)", cnStr, ADODB.CursorTypeEnum.adOpenKeyset, ADODB.LockTypeEnum.adLockOptimistic)
        Me.D2.Text = FormatCurrency(rs("TotD").Value, 2)
        rs.Close()

    End Sub

    Private Sub Dt_ValueChanged(sender As Object, e As EventArgs) Handles Dt.ValueChanged

        Dim cn As New ADODB.Connection()
        Dim rs As New ADODB.Recordset()
        Dim cnStr As String = strAdoConnection()
        rs.Open("SELECT COUNT(CodCliente) AS QtdeC FROM (SELECT CodCliente, SUM(TotDeb) AS TD, SUM(TotCred) AS TC FROM (SELECT CodCliente, CASE DC WHEN 'D' THEN VValor ELSE 0 END AS TotDeb, CASE DC WHEN 'C' THEN VValor ELSE 0 END AS TotCred FROM (SELECT IdCliente, CodCliente, Conta, Dt, Ref, VValor, DC, ND, Saldo, Deb, Cred, IdLote FROM Contas WHERE Dt <= '" & Me.Dt.Value.ToString("yyyy-dd-MM") & "') AS X) AS C GROUP BY CodCliente) AS a WHERE (TC > TD)", cnStr, ADODB.CursorTypeEnum.adOpenKeyset, ADODB.LockTypeEnum.adLockOptimistic)
        Me.C1.Text = FormatNumber(rs("QtdeC").Value, 0)
        rs.Close()

        rs.Open("SELECT SUM(TC) - SUM(TD) AS TotC FROM (SELECT CodCliente, SUM(TotDeb) AS TD, SUM(TotCred) AS TC FROM (SELECT CodCliente, CASE DC WHEN 'D' THEN VValor ELSE 0 END AS TotDeb, CASE DC WHEN 'C' THEN VValor ELSE 0 END AS TotCred FROM (SELECT IdCliente, CodCliente, Conta, Dt, Ref, VValor, DC, ND, Saldo, Deb, Cred, IdLote FROM Contas WHERE Dt <= '" & Me.Dt.Value.ToString("yyyy-dd-MM") & "') as x) AS C GROUP BY CodCliente) AS a WHERE (TC > TD)", cnStr, ADODB.CursorTypeEnum.adOpenKeyset, ADODB.LockTypeEnum.adLockOptimistic)
        Me.C2.Text = FormatCurrency(rs("TotC").Value, 2)
        rs.Close()

        rs.Open("SELECT COUNT(CodCliente) AS QtdeD FROM (SELECT CodCliente, SUM(TotDeb) AS TD, SUM(TotCred) AS TC FROM (SELECT CodCliente, CASE DC WHEN 'D' THEN VValor ELSE 0 END AS TotDeb, CASE DC WHEN 'C' THEN VValor ELSE 0 END AS TotCred FROM (SELECT IdCliente, CodCliente, Conta, Dt, Ref, VValor, DC, ND, Saldo, Deb, Cred, IdLote FROM Contas WHERE Dt <= '" & Me.Dt.Value.ToString("yyyy-dd-MM") & "') as x) AS C GROUP BY CodCliente) AS a WHERE (TD > TC)", cnStr, ADODB.CursorTypeEnum.adOpenKeyset, ADODB.LockTypeEnum.adLockOptimistic)
        Me.D1.Text = FormatNumber(rs("QtdeD").Value, 0)
        rs.Close()

        rs.Open("SELECT SUM(TD) - SUM(TC) AS TotD FROM (SELECT CodCliente, SUM(TotDeb) AS TD, SUM(TotCred) AS TC FROM  (SELECT CodCliente, CASE DC WHEN 'D' THEN VValor ELSE 0 END AS TotDeb, CASE DC WHEN 'C' THEN VValor ELSE 0 END AS TotCred FROM (SELECT IdCliente, CodCliente, Conta, Dt, Ref, VValor, DC, ND, Saldo, Deb, Cred, IdLote FROM Contas WHERE Dt <= '" & Me.Dt.Value.ToString("yyyy-dd-MM") & "') as x) AS C GROUP BY CodCliente) AS a WHERE (TD > TC)", cnStr, ADODB.CursorTypeEnum.adOpenKeyset, ADODB.LockTypeEnum.adLockOptimistic)
        Me.D2.Text = FormatCurrency(rs("TotD").Value, 2)
        rs.Close()

    End Sub

End Class
