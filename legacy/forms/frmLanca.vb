Imports System.Linq
Imports System.Data.Linq.SqlClient.SqlMethods


Public Class FrmLanca
    Private Declare Sub mouse_event Lib "user32.dll" (ByVal dwFlags As Integer, ByVal dx As Integer, ByVal dy As Integer, ByVal cButtons As Integer, ByVal dwExtraInfo As IntPtr)

    Private Sub ContasBindingNavigatorSaveItem_Click(sender As Object, e As EventArgs)
        Me.Validate()
        Me.ContasBindingSource.EndEdit()
        Me.TableAdapterManager.UpdateAll(Me.SoloDataSet)

    End Sub

    Private Sub FrmLanca_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        'TODO: esta linha de código carrega dados na tabela 'SoloDataSet.Contas'. Você pode movê-la ou removê-la conforme necessário.
        Me.ContasTableAdapter.Fill(Me.SoloDataSet.Contas)
        Me.ContasBindingSource.AddNew()

        If FrmLancaData.IsLoaded Then FrmLancaData.Close()
        FrmPrincipal.Visible = False

        Me.btnRibSalvar.Visible = False
        Me.btnRibUndo.Visible = False
        Me.btnNovo.Visible = False
        Me.ribRetornar.Visible = True

        Me.lbCliente.Text = FrmPrincipal.RadGridView1.CurrentRow.Cells(2).Value.ToString
        Me.txtIdCliente.Text = FrmPrincipal.RadGridView1.CurrentRow.Cells(0).Value.ToString
        Me.txtCodCliente.Text = FrmPrincipal.RadGridView1.CurrentRow.Cells(1).Value.ToString

        Me.dtpDt.Value = FrmPrincipal.RadGridView1.Tag
        Me.txtDT.Text = FrmPrincipal.RadGridView1.Tag   'FormatDateTime(Today, DateFormat.ShortDate) 

        'Me.txtRef.Focus()
        'PerformMouseClick("LClick")
        'Application.DoEvents()
        Me.txtPasta.Focus()


    End Sub

    Private Sub ribRetornar_Click(sender As Object, e As EventArgs) Handles ribRetornar.Click
        Me.Close()

    End Sub

    Private Sub FrmLanca_Closed(sender As Object, e As System.Windows.Forms.FormClosedEventArgs) Handles MyBase.FormClosed
        FrmPrincipal.Visible = True

    End Sub

    Private Sub txtPasta_Validated(sender As Object, e As System.EventArgs) Handles txtPasta.Validated

        If Me.txtPasta.Text = "" Then
            Exit Sub
        End If

        If Me.txtPasta.Tag = "1" Then Exit Sub

        Me.txtPasta.Tag = "1"

        Dim i, c
        i = CLng(Me.txtIdCliente.Text)
        c = CLng(Me.txtPasta.Text)

        'Dim db As New ContasDataContext
        'Dim query = (From tb In db.GetTable(Of Contas)() Select tb Where tb.Conta = c And tb.IdCliente = i)
        'Dim varcod = (From c2 In query Select c2.Id)
        'Dim varcod = Aggregate cust In db.Contas Where cust.Conta = c And cust.IdCliente = i Into Count()

        'If varcod = 0 Then
        '    If MsgBox("É uma nova pasta?", MsgBoxStyle.Question + MsgBoxStyle.YesNo, "Atenção!") = vbNo Then
        '        MsgBox(0)
        '    Else
        '        MsgBox(1)
        '    End If
        'End If

        Dim cn As New ADODB.Connection()
        Dim rs As New ADODB.Recordset()
        Dim cnStr As String
        Dim cmd As New ADODB.Command()
        cnStr = strAdoConnection()
        rs.Open("Select * from Contas Where Conta = " & c & " And IdCliente = " & i, cnStr, ADODB.CursorTypeEnum.adOpenKeyset, ADODB.LockTypeEnum.adLockOptimistic)
        If rs.EOF Then
            If MsgBox("É uma nova pasta?", MsgBoxStyle.Question + MsgBoxStyle.YesNo, "Atenção!") = vbNo Then
                Me.dtpDt.Focus()
                Me.txtPasta.Text = ""
                Me.txtPasta.Focus()
            End If
        End If
        rs.Close()

    End Sub

    Private Sub FrmLanca_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles Me.KeyDown

        Select Case e.KeyCode
            Case Keys.Enter
                If Not Me.txtRef.Focused Then SendKeys.Send("{Tab}") Else SendKeys.Send("{BACKSPACE}") : SendKeys.Send("{Tab}")

        End Select

    End Sub

    Private Sub txtDC_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txtDC.KeyPress

        'Dim allowedChars As String = ""
        'If allowedChars.IndexOf(e.KeyChar) = -1 Then
        '    e.Handled = True
        'End If

        If UCase(e.KeyChar) <> "C" And UCase(e.KeyChar) <> "D" Then
            e.Handled = True
        Else
            Me.ribRetornar.Visible = False
            Me.btnNovo.Visible = False
            Me.btnRibSalvar.Visible = True
            Me.btnRibUndo.Visible = True
        End If

    End Sub

    Private Sub dtpDt_Validated(sender As Object, e As EventArgs) Handles dtpDt.Validated

        Me.ribRetornar.Visible = False
        Me.btnNovo.Visible = False
        Me.btnRibSalvar.Visible = True
        Me.btnRibUndo.Visible = True
        Me.txtDT.Text = FormatDateTime(dtpDt.Value.Date, DateFormat.ShortDate)

    End Sub

    Private Sub txtRef_TextChanged(sender As Object, e As EventArgs) Handles txtRef.TextChanged

        If Me.txtRef.Tag = "" Then
            If Trim(Me.txtRef.Text) <> "" & vbCrLf & "" And Trim(Me.txtRef.Text) <> "" Then
                Me.ribRetornar.Visible = False
                Me.btnNovo.Visible = False
                Me.btnRibSalvar.Visible = True
                Me.btnRibUndo.Visible = True
                'Else
                '   Me.txtRef.Text = ""
            End If
        Else
            Me.ribRetornar.Visible = False
            Me.btnNovo.Visible = False
            Me.btnRibSalvar.Visible = True
            Me.btnRibUndo.Visible = True
        End If
        Me.txtRef.Tag = Me.txtRef.Text

    End Sub
    Private Sub txtVValor_TextChanged(sender As Object, e As EventArgs) Handles txtVValor.TextChanged

        Me.ribRetornar.Visible = False
        Me.btnNovo.Visible = False
        Me.btnRibSalvar.Visible = True
        Me.btnRibUndo.Visible = True

    End Sub

    Private Sub btnRibUndo_Click(sender As Object, e As System.EventArgs) Handles btnRibUndo.Click

        'Me.btnRibSalvar.Visible = False
        'Me.btnRibUndo.Visible = False
        'Me.btnNovo.Visible = False
        'Me.ribRetornar.Visible = True

        Me.ContasTableAdapter.Fill(Me.SoloDataSet.Contas)
        Me.ContasBindingSource.AddNew()

        Me.dtpDt.Value = FrmPrincipal.RadGridView1.Tag
        Me.txtDT.Text = FrmPrincipal.RadGridView1.Tag   'FormatDateTime(Today, DateFormat.ShortDate) 

        Me.txtPasta.Focus()

    End Sub

    Private Sub txtPasta_TextChanged(sender As Object, e As EventArgs) Handles txtPasta.TextChanged

        Me.ribRetornar.Visible = False
        Me.btnNovo.Visible = False
        Me.btnRibSalvar.Visible = True
        Me.btnRibUndo.Visible = True

    End Sub

    Private Sub dtpDt_ValueChanged(sender As Object, e As EventArgs) Handles dtpDt.ValueChanged

        If Me.txtDT.Text = "" Then Me.txtDT.Text = Me.dtpDt.Value

        'Me.ribRetornar.Visible = False
        'Me.btnNovo.Visible = False
        'Me.btnRibSalvar.Visible = True
        'Me.btnRibUndo.Visible = True

    End Sub

    Private Sub ContasBindingSource_CurrentChanged(sender As Object, e As EventArgs) Handles ContasBindingSource.CurrentChanged

        Me.ribRetornar.Visible = True
        Me.btnNovo.Visible = False
        Me.btnRibSalvar.Visible = False
        Me.btnRibUndo.Visible = False

    End Sub

    Private Sub btnRibSalvar_Click(sender As Object, e As EventArgs) Handles btnRibSalvar.Click

        If Me.txtPasta.Text = "" Then
            MsgBox("Informe a pasta", MsgBoxStyle.Critical, "Atenção!")
            Me.txtPasta.Focus()
            Exit Sub
        End If

        If Me.txtRef.Text = "" Then
            MsgBox("Informe o lançamento", MsgBoxStyle.Critical, "Atenção!")
            Me.txtRef.Focus()
            Exit Sub
        End If

        If Me.txtVValor.Text = "" Then
            MsgBox("Informe o valor", MsgBoxStyle.Critical, "Atenção!")
            Me.txtVValor.Focus()
            Exit Sub
        End If

        If Me.txtDC.Text = "" Then
            MsgBox("Informe D ou C", MsgBoxStyle.Critical, "Atenção!")
            Me.txtDC.Focus()
            Exit Sub
        End If

        If Me.txtDC.Text = "D" Then
            Me.txtDeb.Text = Me.txtVValor.Text
        Else
            Me.txtCred.Text = Me.txtVValor.Text
        End If

        Me.Validate()
        Me.ContasBindingSource.EndEdit()
        Me.TableAdapterManager.UpdateAll(Me.SoloDataSet)

        Me.ribRetornar.Visible = True
        Me.btnNovo.Visible = True
        Me.btnRibSalvar.Visible = False
        Me.btnRibUndo.Visible = False

        Me.txtPasta.Focus()

    End Sub

    Private Sub PerformMouseClick(ByVal LClick_RClick_DClick As String)
        Const MOUSEEVENTF_LEFTDOWN As Integer = 2
        Const MOUSEEVENTF_LEFTUP As Integer = 4
        Const MOUSEEVENTF_RIGHTDOWN As Integer = 6
        Const MOUSEEVENTF_RIGHTUP As Integer = 8
        If LClick_RClick_DClick = "RClick" Then
            mouse_event(MOUSEEVENTF_RIGHTDOWN, 0, 0, 0, IntPtr.Zero)
            mouse_event(MOUSEEVENTF_RIGHTUP, 0, 0, 0, IntPtr.Zero)
        ElseIf LClick_RClick_DClick = "LClick" Then
            mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, IntPtr.Zero)
            mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, IntPtr.Zero)
        ElseIf LClick_RClick_DClick = "DClick" Then
            mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, IntPtr.Zero)
            mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, IntPtr.Zero)
            mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, IntPtr.Zero)
            mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, IntPtr.Zero)
        End If
    End Sub

End Class
