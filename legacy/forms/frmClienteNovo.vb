Imports Telerik.WinControls.UI

Public Class frmClienteNovo

    Private Sub frmClienteNovo_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try
            'TODO: esta linha de código carrega dados na tabela 'SoloDataSet.Clientes'. Você pode movê-la ou removê-la conforme necessário.
            Me.ClientesTableAdapter.Fill(Me.SoloDataSet.Clientes)
            Me.ClientesBindingSource.AddNew()

        Catch ex As Exception
            MsgBox(ex.Message)

        Finally
            FrmPrincipal.Visible = False

            'Me.btnRibSalvar.Visible = False
            'Me.btnRibUndo.Visible = False
            'Me.ribRetornar.Visible = True

        End Try

    End Sub

    'Private Sub ribRetornar_Click(sender As Object, e As EventArgs) Handles ribRetornar.Click
    '    Me.Close()

    'End Sub

    Private Sub frmClienteNovo_Closed(sender As Object, e As System.Windows.Forms.FormClosedEventArgs) Handles MyBase.FormClosed
        If Me.Tag = "1" Then FrmPrincipal.ClientesTableAdapter.Fill(FrmPrincipal.SoloDataSet.Clientes)

        FrmPrincipal.Visible = True

    End Sub

    'Private Sub btnRibSalvar_Click(sender As Object, e As EventArgs) Handles btnRibSalvar.Click

    '    If Me.CódigoTextBox.Text = "" Then
    '        MsgBox("Por favor, informe o código do cliente", MsgBoxStyle.Critical, "Atenção!")
    '        Me.CódigoTextBox.Focus()
    '        Exit Sub
    '    End If
    '    If Me.ClienteTextBox.Text = "" Then
    '        MsgBox("Por favor, informe o nome do cliente", MsgBoxStyle.Critical, "Atenção!")
    '        Me.ClienteTextBox.Focus()
    '        Exit Sub
    '    End If

    '    Dim cn As New ADODB.Connection()
    '    Dim rs As New ADODB.Recordset()
    '    Dim cnStr As String = strAdoConnection()
    '    rs.Open("SELECT Id FROM [Clientes] Where Código = " & CInt(Me.CódigoTextBox.Text), cnStr, ADODB.CursorTypeEnum.adOpenKeyset, ADODB.LockTypeEnum.adLockOptimistic)
    '    If Not rs.EOF Then
    '        MsgBox("Código já existente", vbCritical, "Atenção!")
    '        Exit Sub
    '    End If

    '    Me.CódigoTextBox.Tag = "1"

    '    Me.Validate()
    '    Me.ClientesBindingSource.EndEdit()
    '    Me.TableAdapterManager.UpdateAll(Me.SoloDataSet)
    '    Me.Tag = "1"
    '    Me.Close()

    '    'Me.ribRetornar.Visible = True
    '    'Me.btnRibSalvar.Visible = False
    '    'Me.btnRibUndo.Visible = False
    '    'Me.Tag = "1"

    '    'Me.CódigoTextBox.Focus()

    'End Sub

    'Private Sub btnRibUndo_Click(sender As Object, e As EventArgs) Handles btnRibUndo.Click
    '    'Me.CódigoTextBox.Tag = "1"

    '    Me.ClientesTableAdapter.Fill(Me.SoloDataSet.Clientes)
    '    Me.ClientesBindingSource.AddNew()

    '    'Me.ribRetornar.Visible = True
    '    'Me.btnRibSalvar.Visible = False
    '    'Me.btnRibUndo.Visible = False

    '    Me.CódigoTextBox.Focus()

    'End Sub

    Private Sub ClientesBindingNavigatorSaveItem_Click(sender As Object, e As EventArgs)
        Me.Validate()
        Me.ClientesBindingSource.EndEdit()
        Me.TableAdapterManager.UpdateAll(Me.SoloDataSet)

    End Sub

    Private Sub btnCodigo_Click(sender As Object, e As EventArgs) Handles btnCodigo.Click

        Dim cn As New ADODB.Connection()
        Dim rs As New ADODB.Recordset()
        Dim cnStr As String = strAdoConnection()

        rs.Open("SELECT Id FROM [Clientes] Order By Id Desc", cnStr, ADODB.CursorTypeEnum.adOpenKeyset, ADODB.LockTypeEnum.adLockOptimistic)
        Me.IdTextBox.Text = rs("Id").Value + 1
        rs.Close()

        rs.Open("SELECT Código FROM [Clientes] Where Código >= 10000 Order By Código", cnStr, ADODB.CursorTypeEnum.adOpenKeyset, ADODB.LockTypeEnum.adLockOptimistic)

        Dim i As Long
        For i = 10000 To 50000
            If i <> rs("Código").Value Then Exit For
            rs.MoveNext()
        Next i

        rs.Close()

        Me.CódigoTextBox.Text = i 'DLookup("[Código]", "qpst_NovoCod") + 1
        Me.ClienteTextBox.Focus()
        'Me.btnCodigo.Enabled = False

        'Me.ribRetornar.Visible = False
        'Me.btnRibSalvar.Visible = True
        'Me.btnRibUndo.Visible = True

    End Sub

    Private Sub CódigoTextBox_TextChanged(sender As Object, e As EventArgs) Handles CódigoTextBox.Validated

        If Me.CódigoTextBox.Tag = "1" Then Exit Sub

        If CódigoTextBox.Text <> "" Then
            Dim cn As New ADODB.Connection()
            Dim rs As New ADODB.Recordset()
            Dim cnStr As String = strAdoConnection()
            rs.Open("SELECT Id FROM [Clientes] Where Código = " & CInt(Me.CódigoTextBox.Text), cnStr, ADODB.CursorTypeEnum.adOpenKeyset, ADODB.LockTypeEnum.adLockOptimistic)
            If Not rs.EOF Then
                MsgBox("Código já existente", vbCritical, "Atenção!")
                'Me.Timer1.Enabled = True
                'Me.CódigoTextBox.Text = ""
                'Me.ribRetornar.Visible = True
                'Me.btnRibSalvar.Visible = False
                'Me.btnRibUndo.Visible = False
                Me.CódigoTextBox.Focus()
            Else
                If CDbl(Me.CódigoTextBox.Text) > 20000 Then
                    MsgBox("Tamanho máximo de Código: 20.000", vbCritical, "Atenção!")
                    Me.CódigoTextBox.Text = ""
                    'Me.ribRetornar.Visible = True
                    'Me.btnRibSalvar.Visible = False
                    'Me.btnRibUndo.Visible = False
                    Me.CódigoTextBox.Focus()
                Else
                    'Me.ribRetornar.Visible = False
                    'Me.btnRibSalvar.Visible = True
                    'Me.btnRibUndo.Visible = True
                End If
            End If
            rs.Close()
        Else
            'Me.ribRetornar.Visible = True
            'Me.btnRibSalvar.Visible = False
            'Me.btnRibUndo.Visible = False
        End If


    End Sub

    Private Sub ClienteTextBox_TextChanged(sender As Object, e As EventArgs) Handles ClienteTextBox.TextChanged
        'Me.ribRetornar.Visible = False
        'Me.btnRibSalvar.Visible = True
        'Me.btnRibUndo.Visible = True

    End Sub

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        Me.Timer1.Enabled = False
        Me.CódigoTextBox.Tag = "0"

    End Sub

    Private Sub FrmLanca_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles Me.KeyDown

        Select Case e.KeyCode
            Case Keys.Enter
                SendKeys.Send("{Tab}")

        End Select

    End Sub
End Class
