Imports Telerik.WinControls.UI
Imports DevComponents.DotNetBar.Validator

Public Class frmAlterar

    Private Sub frmAlterar_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try
            'TODO: esta linha de código carrega dados na tabela 'SoloDataSet.Clientes'. Você pode movê-la ou removê-la conforme necessário.
            Me.ClientesTableAdapter.Fill(Me.SoloDataSet.Clientes)
            Me.ClientesBindingSource.Filter = "Id = " & FrmPrincipal.RadGridView1.CurrentRow.Cells(0).Value

            FrmPrincipal.Visible = False

            'Me.btnRibSalvar.Visible = False
            'Me.btnRibUndo.Visible = False

        Catch ex As Exception
            MsgBox(ex.Message)

        End Try

    End Sub

    'Private Sub ribRetornar_Click(sender As Object, e As EventArgs) Handles ribRetornar.Click
    '    Me.Close()

    'End Sub

    Private Sub frmAlterar_Closed(sender As Object, e As System.Windows.Forms.FormClosedEventArgs) Handles MyBase.FormClosed
        If Me.Tag = "1" Then
            FrmPrincipal.ClientesTableAdapter.Fill(FrmPrincipal.SoloDataSet.Clientes)
        End If
        FrmPrincipal.Visible = True

    End Sub

    Private Sub btnRibSalvar_Click(sender As Object, e As EventArgs) Handles btnRibSalvar.Click

        'If Me.CódigoTextBox.Text <> FrmPrincipal.RadGridView1.CurrentRow.Cells(1).Value.ToString Then
        '    If MsgBox("DESEJA MESMO ALTERAR O CAMPO 'CÓDIGO' DO CLIENTE?", MsgBoxStyle.Critical + MsgBoxStyle.YesNo, "ATENÇÃO!!!") = vbYes Then
        '        If MsgBox("TEM CERTEZA QUE DESEJA ALTERAR O CÓDIGO DO CLIENTE" & vbCrLf & vbCrLf & Me.ClienteTextBox.Text, MsgBoxStyle.Question + MsgBoxStyle.YesNo, "ATENÇÃO!") Then

        '        End If
        '    End If
        'End If

        If Me.CódigoTextBox.Text <> FrmPrincipal.RadGridView1.CurrentRow.Cells(1).Value.ToString Then
            If MsgBox("TEM CERTEZA QUE DESEJA ALTERAR O CÓDIGO DO CLIENTE " & Me.ClienteTextBox.Text & " ?", vbYesNo, "A T E N Ç Ã O ! ! !") = vbYes Then
                If MsgBox("A T E N Ç Ã O ! ! ! DESEJA MESMO ALTERAR O CÓDIGO DO CLIENTE " & Me.ClienteTextBox.Text & " ?", vbYesNo, "A T E N Ç Ã O ! ! !") = vbYes Then
                    If MsgBox("TEM CERTEZA QUE DESEJA ALTERAR O CÓDIGO DO CLIENTE " & Me.ClienteTextBox.Text & " ?", vbYesNo, "A T E N Ç Ã O ! ! !") = vbYes Then
                        Me.Validate()
                        Me.ClientesBindingSource.EndEdit()
                        Me.TableAdapterManager.UpdateAll(Me.SoloDataSet)
                        Me.Tag = "1"
                        'Me.ribRetornar.Visible = True
                        'Me.btnRibSalvar.Visible = False
                        'Me.btnRibUndo.Visible = False
                        'Me.btnAlterar.Visible = True
                        'Me.btnExcluir.Visible = True
                        Me.ClienteTextBox.Focus()
                        Me.Highlighter1.SetHighlightOnFocus(Me.CódigoTextBox, False)
                        Me.CódigoTextBox.Focus()
                        Me.Highlighter1.SetHighlightOnFocus(Me.ClienteTextBox, False)
                        Me.ClienteTextBox.Focus()
                        Me.CódigoTextBox.Focus()
                        Me.CódigoTextBox.ReadOnly = True
                        Me.ClienteTextBox.ReadOnly = True
                        Exit Sub
                    End If
                End If
            End If
        ElseIf Me.ClienteTextBox.Text <> FrmPrincipal.RadGridView1.CurrentRow.Cells(2).Value.ToString Then
            Me.Validate()
            Me.ClientesBindingSource.EndEdit()
            Me.TableAdapterManager.UpdateAll(Me.SoloDataSet)
            Me.Tag = "1"
            'Me.ribRetornar.Visible = True
            'Me.btnRibSalvar.Visible = False
            'Me.btnRibUndo.Visible = False
            'Me.btnAlterar.Visible = True
            'Me.btnExcluir.Visible = True
            Me.ClienteTextBox.Focus()
            Me.Highlighter1.SetHighlightOnFocus(Me.CódigoTextBox, False)
            Me.CódigoTextBox.Focus()
            Me.Highlighter1.SetHighlightOnFocus(Me.ClienteTextBox, False)
            Me.ClienteTextBox.Focus()
            Me.CódigoTextBox.Focus()
            Me.CódigoTextBox.ReadOnly = True
            Me.ClienteTextBox.ReadOnly = True
        End If

    End Sub

    Private Sub btnRibUndo_Click(sender As Object, e As EventArgs) Handles btnRibUndo.Click
        Me.ClientesTableAdapter.Fill(Me.SoloDataSet.Clientes)
        Me.ClientesBindingSource.Filter = "Id = " & FrmPrincipal.RadGridView1.CurrentRow.Cells(0).Value

        'Me.ribRetornar.Visible = True
        'Me.btnRibSalvar.Visible = False
        'Me.btnRibUndo.Visible = False
        'Me.btnAlterar.Visible = True
        'Me.btnExcluir.Visible = True
        Me.ClienteTextBox.Focus()
        Me.Highlighter1.SetHighlightOnFocus(Me.CódigoTextBox, False)
        Me.CódigoTextBox.Focus()
        Me.Highlighter1.SetHighlightOnFocus(Me.ClienteTextBox, False)
        Me.ClienteTextBox.Focus()
        Me.CódigoTextBox.Focus()
        Me.CódigoTextBox.ReadOnly = True
        Me.ClienteTextBox.ReadOnly = True

    End Sub

    Private Sub ClientesBindingNavigatorSaveItem_Click(sender As Object, e As EventArgs)
        Me.Validate()
        Me.ClientesBindingSource.EndEdit()
        Me.TableAdapterManager.UpdateAll(Me.SoloDataSet)

    End Sub

    Private Sub btnAlterar_Click(sender As Object, e As EventArgs) Handles btnAlterar.Click
        Me.CódigoTextBox.ReadOnly = False
        Me.ClienteTextBox.ReadOnly = False
        'Me.ribRetornar.Visible = False
        'Me.btnRibSalvar.Visible = True
        'Me.btnRibUndo.Visible = True
        'Me.btnAlterar.Visible = False
        'Me.btnExcluir.Visible = False
        Me.Highlighter1.SetHighlightOnFocus(Me.CódigoTextBox, True)
        Me.Highlighter1.SetHighlightOnFocus(Me.ClienteTextBox, True)
        Me.ClienteTextBox.Focus()
        Me.CódigoTextBox.Focus()
    End Sub

    Private Sub btnExcluir_Click(sender As Object, e As EventArgs) Handles btnExcluir.Click
        'If MsgBox("Confirma exclusão do cliente " & Me.ClienteTextBox.Text & " ?", vbYesNo Or vbQuestion Or vbDefaultButton2, "Atenção!") = vbYes Then

        '    If InputBox("Informe a senha:", "Controle de Exclusão de Cliente", "******") <> "4321" Then
        '        MsgBox("Senha incorreta", vbCritical, "Controle de Exclusão de Cliente")
        '    Else
        '        Dim cn As New ADODB.Connection()
        '        Dim rs As New ADODB.Recordset()
        '        Dim cnStr As String = strAdoConnection()
        '        rs.Open("SELECT * From Contas Where IdCliente = " & CInt(Me.IdTextBox.Text), cnStr, ADODB.CursorTypeEnum.adOpenKeyset, ADODB.LockTypeEnum.adLockOptimistic)
        '        If Not rs.EOF Then
        '            If MsgBox("O cliente " & Me.ClienteTextBox.Text & " tem lançamentos na conta corrente;" & vbCrLf & "Confirma exclusão do cliente e dos lançamentos?", vbYesNo Or vbQuestion Or vbDefaultButton2, "Atenção!") = vbYes Then
        '                fExecutaText("Delete From Contas Where IdCliente = " & CInt(Me.IdTextBox.Text) & " And CodCliente = " & CInt(Me.CódigoTextBox.Text) & "; Delete From Clientes Where Id = " & CInt(Me.IdTextBox.Text))
        '            End If
        '        Else
        '            If MsgBox("Confirma exclusão do cliente " & Me.ClienteTextBox.Text & " ?", vbYesNo Or vbQuestion Or vbDefaultButton2, "Atenção!") = vbYes Then
        '                fExecutaText("Delete From Clientes Where Id = " & CInt(Me.IdTextBox.Text))
        '            End If
        '        End If
        '        rs.Close()
        '        Me.Tag = "1"
        '        Me.Close()
        '    End If

        'End If
        If MsgBox("Confirma exclusão do cliente " & Me.ClienteTextBox.Text & " ?", vbYesNo Or vbQuestion Or vbDefaultButton2, "Atenção!") = vbYes Then
            varSenha = "2"
            frmSenha.ShowDialog()
            frmSenha.txtSenha.Text = ""
        End If

    End Sub
End Class
