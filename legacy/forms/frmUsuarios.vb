Imports Telerik.WinControls.UI

Public Class FrmUsuarios

    Private Sub FrmUsuarios_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        'TODO: esta linha de código carrega dados na tabela 'SoloDataSet.Perfis'. Você pode movê-la ou removê-la conforme necessário.
        Me.PerfisTableAdapter.Fill(Me.SoloDataSet.Perfis)
        'TODO: esta linha de código carrega dados na tabela 'SoloDataSet.Usuários'. Você pode movê-la ou removê-la conforme necessário.
        Me.UsuáriosTableAdapter.Fill(Me.SoloDataSet.Usuários)
        'TODO: esta linha de código carrega dados na tabela 'SoloDataSet.Chaves'. Você pode movê-la ou removê-la conforme necessário.
        Me.ChavesTableAdapter.Fill(Me.SoloDataSet.Chaves)

        FrmPrincipal.Visible = False
        'frmSenha.Close()

        Me.btnRibSalvar.Visible = False
        Me.btnRibUndo.Visible = False

        If varPerfil <> "Administrador" Then
            Me.C1DockingTabPage2.TabVisible = False
            Me.UsuáriosBindingSource.Filter = "Usuário = '" & varUsu & "'"
            Me.RadGridView2.AllowAddNewRow = False
            Me.RadGridView2.Columns(3).ReadOnly = True
        End If

    End Sub

    'Private Sub ribRetornar_Click(sender As Object, e As EventArgs) Handles ribRetornar.Click
    '    Me.Close()

    'End Sub

    Private Sub FrmUsuarios_Closed(sender As Object, e As System.Windows.Forms.FormClosedEventArgs) Handles MyBase.FormClosed
        FrmPrincipal.Visible = True

    End Sub

    Private Sub RadGridView1_ValueChanged(sender As Object, e As System.EventArgs) Handles RadGridView1.ValueChanged

        Me.ribRetornar.Visible = False
        Me.btnRibSalvar.Visible = True
        Me.btnRibUndo.Visible = True

    End Sub

    'Private Sub btnRibSalvar_Click(sender As Object, e As EventArgs) Handles btnRibSalvar.Click

    '    Me.Validate()
    '    Me.ChavesBindingSource.EndEdit()
    '    Me.UsuáriosBindingSource.EndEdit()
    '    Me.TableAdapterManager.UpdateAll(Me.SoloDataSet)
    '    Me.TableAdapterManager1.UpdateAll(Me.SoloDataSet)

    '    Me.ribRetornar.Visible = True
    '    Me.btnRibSalvar.Visible = False
    '    Me.btnRibUndo.Visible = False

    'End Sub

    'Private Sub btnRibUndo_Click(sender As Object, e As EventArgs) Handles btnRibUndo.Click
    '    Me.ChavesTableAdapter.Fill(Me.SoloDataSet.Chaves)
    '    Me.UsuáriosTableAdapter.Fill(Me.SoloDataSet.Usuários)

    '    Me.ribRetornar.Visible = True
    '    Me.btnRibSalvar.Visible = False
    '    Me.btnRibUndo.Visible = False

    'End Sub

    Private Sub RadGridView2_ValueChanged(sender As Object, e As EventArgs) Handles RadGridView2.ValueChanged

        Me.ribRetornar.Visible = False
        Me.btnRibSalvar.Visible = True
        Me.btnRibUndo.Visible = True

    End Sub
End Class
