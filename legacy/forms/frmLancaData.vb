Public Class FrmLancaData

    Private Sub RadCalendar1_SelectionChanged(sender As Object, e As EventArgs) Handles RadCalendar1.SelectionChanged
        'MsgBox(FormatDateTime(Me.RadCalendar1.SelectedDate, DateFormat.ShortDate))
        Me.lbData.Text = FormatDateTime(Me.RadCalendar1.SelectedDate, DateFormat.ShortDate)

    End Sub


    Private Sub btnCancelar_Click(sender As Object, e As EventArgs) Handles btnCancelar.Click
        Me.Close()

    End Sub

    Private Sub btnOk_Click(sender As Object, e As EventArgs) Handles btnOk.Click
        FrmPrincipal.RadGridView1.Tag = Me.lbData.Text
        FrmLanca.Show()

    End Sub

    Private Sub FrmLancaData_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.lbData.Text = FormatDateTime(Today, DateFormat.ShortDate)

    End Sub
End Class
