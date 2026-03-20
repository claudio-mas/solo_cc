<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmLogin
    Inherits System.Windows.Forms.Form

    'Descartar substituições de formulário para limpar a lista de componentes.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Exigido pelo Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'OBSERVAÇÃO: O procedimento a seguir é exigido pelo Windows Form Designer
    'Ele pode ser modificado usando o Windows Form Designer.  
    'Não o modifique usando o editor de códigos.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmLogin))
        Me.PictureBox2 = New System.Windows.Forms.PictureBox()
        Me.btnOk = New System.Windows.Forms.Button()
        Me.Button2 = New System.Windows.Forms.Button()
        Me.cboUsuario = New System.Windows.Forms.ComboBox()
        Me.UsuáriosBindingSource = New System.Windows.Forms.BindingSource(Me.components)
        Me.SoloDataSet = New Solo.SoloDataSet()
        Me.btnCancelar = New System.Windows.Forms.Button()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.txtSenha = New System.Windows.Forms.TextBox()
        Me.UsuáriosTableAdapter = New Solo.SoloDataSetTableAdapters.UsuáriosTableAdapter()
        Me.Highlighter1 = New DevComponents.DotNetBar.Validator.Highlighter()
        CType(Me.PictureBox2, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.UsuáriosBindingSource, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.SoloDataSet, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'PictureBox2
        '
        Me.PictureBox2.BackColor = System.Drawing.Color.Transparent
        Me.PictureBox2.Image = CType(resources.GetObject("PictureBox2.Image"), System.Drawing.Image)
        Me.PictureBox2.Location = New System.Drawing.Point(33, 25)
        Me.PictureBox2.Name = "PictureBox2"
        Me.PictureBox2.Size = New System.Drawing.Size(96, 96)
        Me.PictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.PictureBox2.TabIndex = 1
        Me.PictureBox2.TabStop = False
        '
        'btnOk
        '
        Me.btnOk.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnOk.Location = New System.Drawing.Point(406, 52)
        Me.btnOk.Name = "btnOk"
        Me.btnOk.Size = New System.Drawing.Size(69, 27)
        Me.btnOk.TabIndex = 7
        Me.btnOk.Text = "Ok"
        Me.btnOk.UseVisualStyleBackColor = True
        '
        'Button2
        '
        Me.Button2.Image = CType(resources.GetObject("Button2.Image"), System.Drawing.Image)
        Me.Button2.Location = New System.Drawing.Point(510, 4)
        Me.Button2.Name = "Button2"
        Me.Button2.Size = New System.Drawing.Size(55, 55)
        Me.Button2.TabIndex = 3
        Me.Button2.UseVisualStyleBackColor = True
        Me.Button2.Visible = False
        '
        'cboUsuario
        '
        Me.cboUsuario.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend
        Me.cboUsuario.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems
        Me.cboUsuario.DataSource = Me.UsuáriosBindingSource
        Me.cboUsuario.DisplayMember = "Usuário"
        Me.cboUsuario.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cboUsuario.FormattingEnabled = True
        Me.Highlighter1.SetHighlightOnFocus(Me.cboUsuario, True)
        Me.cboUsuario.Location = New System.Drawing.Point(260, 52)
        Me.cboUsuario.Name = "cboUsuario"
        Me.cboUsuario.Size = New System.Drawing.Size(126, 24)
        Me.cboUsuario.TabIndex = 4
        Me.cboUsuario.ValueMember = "PWD"
        '
        'UsuáriosBindingSource
        '
        Me.UsuáriosBindingSource.DataMember = "Usuários"
        Me.UsuáriosBindingSource.DataSource = Me.SoloDataSet
        Me.UsuáriosBindingSource.Sort = "Usuário"
        '
        'SoloDataSet
        '
        Me.SoloDataSet.DataSetName = "SoloDataSet"
        Me.SoloDataSet.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema
        '
        'btnCancelar
        '
        Me.btnCancelar.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnCancelar.Location = New System.Drawing.Point(406, 82)
        Me.btnCancelar.Name = "btnCancelar"
        Me.btnCancelar.Size = New System.Drawing.Size(69, 27)
        Me.btnCancelar.TabIndex = 9
        Me.btnCancelar.Text = "Cancelar"
        Me.btnCancelar.UseVisualStyleBackColor = True
        '
        'Label1
        '
        Me.Label1.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label1.Location = New System.Drawing.Point(199, 52)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(58, 23)
        Me.Label1.TabIndex = 6
        Me.Label1.Text = "Usuário"
        Me.Label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label2
        '
        Me.Label2.Font = New System.Drawing.Font("Microsoft Sans Serif", 14.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label2.ForeColor = System.Drawing.Color.DarkRed
        Me.Label2.Location = New System.Drawing.Point(207, 4)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(179, 23)
        Me.Label2.TabIndex = 7
        Me.Label2.Text = "Controle de Acesso"
        Me.Label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'Label3
        '
        Me.Label3.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label3.Location = New System.Drawing.Point(199, 85)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(58, 23)
        Me.Label3.TabIndex = 8
        Me.Label3.Text = "Senha"
        Me.Label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'txtSenha
        '
        Me.txtSenha.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Highlighter1.SetHighlightOnFocus(Me.txtSenha, True)
        Me.txtSenha.Location = New System.Drawing.Point(260, 85)
        Me.txtSenha.Name = "txtSenha"
        Me.txtSenha.PasswordChar = Global.Microsoft.VisualBasic.ChrW(42)
        Me.txtSenha.Size = New System.Drawing.Size(126, 22)
        Me.txtSenha.TabIndex = 5
        '
        'UsuáriosTableAdapter
        '
        Me.UsuáriosTableAdapter.ClearBeforeFill = True
        '
        'Highlighter1
        '
        Me.Highlighter1.ContainerControl = Me
        Me.Highlighter1.FocusHighlightColor = DevComponents.DotNetBar.Validator.eHighlightColor.Red
        '
        'frmLogin
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(567, 146)
        Me.ControlBox = False
        Me.Controls.Add(Me.txtSenha)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.btnCancelar)
        Me.Controls.Add(Me.cboUsuario)
        Me.Controls.Add(Me.Button2)
        Me.Controls.Add(Me.btnOk)
        Me.Controls.Add(Me.PictureBox2)
        Me.Highlighter1.SetHighlightOnFocus(Me, True)
        Me.KeyPreview = True
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "frmLogin"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "SOLO CONSULTORIA DE IMÓVEIS"
        CType(Me.PictureBox2, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.UsuáriosBindingSource, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.SoloDataSet, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents PictureBox2 As System.Windows.Forms.PictureBox
    Friend WithEvents btnOk As System.Windows.Forms.Button
    Friend WithEvents Button2 As System.Windows.Forms.Button
    Friend WithEvents cboUsuario As System.Windows.Forms.ComboBox
    Friend WithEvents btnCancelar As System.Windows.Forms.Button
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents txtSenha As System.Windows.Forms.TextBox
    Friend WithEvents SoloDataSet As Solo.SoloDataSet
    Friend WithEvents UsuáriosBindingSource As System.Windows.Forms.BindingSource
    Friend WithEvents UsuáriosTableAdapter As Solo.SoloDataSetTableAdapters.UsuáriosTableAdapter
    Friend WithEvents Highlighter1 As DevComponents.DotNetBar.Validator.Highlighter
End Class
