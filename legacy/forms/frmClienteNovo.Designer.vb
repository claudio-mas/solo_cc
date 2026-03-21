<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmClienteNovo
    Inherits Telerik.WinControls.UI.RadForm

    'Form overrides dispose to clean up the component list.
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

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim CódigoLabel As System.Windows.Forms.Label
        Dim ClienteLabel As System.Windows.Forms.Label
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmClienteNovo))
        'Me.C1Ribbon1 = New C1.Win.C1Ribbon.C1Ribbon()
        'Me.RibbonApplicationMenu1 = New C1.Win.C1Ribbon.RibbonApplicationMenu()
        'Me.RibbonConfigToolBar1 = New C1.Win.C1Ribbon.RibbonConfigToolBar()
        'Me.RibbonQat1 = New C1.Win.C1Ribbon.RibbonQat()
        'Me.RibbonTab1 = New C1.Win.C1Ribbon.RibbonTab()
        'Me.RibbonGroup6 = New C1.Win.C1Ribbon.RibbonGroup()
        'Me.ribRetornar = New C1.Win.C1Ribbon.RibbonButton()
        'Me.btnRibSalvar = New C1.Win.C1Ribbon.RibbonButton()
        'Me.btnRibUndo = New C1.Win.C1Ribbon.RibbonButton()
        'Me.RibbonTopToolBar1 = New C1.Win.C1Ribbon.RibbonTopToolBar()
        'Me.RibbonBottomToolBar1 = New C1.Win.C1Ribbon.RibbonBottomToolBar()
        Me.C1PictureBox1 = New C1.Win.C1Input.C1PictureBox()
        Me.Office2010BlackTheme1 = New Telerik.WinControls.Themes.Office2010BlackTheme()
        Me.SoloDataSet = New Solo.SoloDataSet()
        Me.ClientesBindingSource = New System.Windows.Forms.BindingSource(Me.components)
        Me.ClientesTableAdapter = New Solo.SoloDataSetTableAdapters.ClientesTableAdapter()
        Me.TableAdapterManager = New Solo.SoloDataSetTableAdapters.TableAdapterManager()
        Me.btnCodigo = New System.Windows.Forms.Button()
        Me.Highlighter1 = New DevComponents.DotNetBar.Validator.Highlighter()
        Me.CódigoTextBox = New System.Windows.Forms.TextBox()
        Me.ClienteTextBox = New System.Windows.Forms.TextBox()
        Me.IdTextBox = New System.Windows.Forms.TextBox()
        Me.Timer1 = New System.Windows.Forms.Timer(Me.components)
        Me.lbCliente = New System.Windows.Forms.Label()
        CódigoLabel = New System.Windows.Forms.Label()
        ClienteLabel = New System.Windows.Forms.Label()
        'CType(Me.C1Ribbon1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.C1PictureBox1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.SoloDataSet, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.ClientesBindingSource, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'CódigoLabel
        '
        CódigoLabel.AutoSize = True
        CódigoLabel.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        CódigoLabel.Location = New System.Drawing.Point(264, 330)
        CódigoLabel.Name = "CódigoLabel"
        CódigoLabel.Size = New System.Drawing.Size(51, 17)
        CódigoLabel.TabIndex = 8
        CódigoLabel.Text = "Código"
        CódigoLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'ClienteLabel
        '
        ClienteLabel.AutoSize = True
        ClienteLabel.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        ClienteLabel.Location = New System.Drawing.Point(271, 362)
        ClienteLabel.Name = "ClienteLabel"
        ClienteLabel.Size = New System.Drawing.Size(44, 17)
        ClienteLabel.TabIndex = 9
        ClienteLabel.Text = "Nome"
        ClienteLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'C1Ribbon1
        '
        'Me.C1Ribbon1.ApplicationMenuHolder = Me.RibbonApplicationMenu1
        'Me.C1Ribbon1.BottomToolBarHolder = Me.RibbonBottomToolBar1
        'Me.C1Ribbon1.ConfigToolBarHolder = Me.RibbonConfigToolBar1
        'Me.C1Ribbon1.Font = New System.Drawing.Font("Calibri Light", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        'Me.C1Ribbon1.Location = New System.Drawing.Point(0, 0)
        'Me.C1Ribbon1.Name = "C1Ribbon1"
        'Me.C1Ribbon1.QatHolder = Me.RibbonQat1
        'Me.C1Ribbon1.Size = New System.Drawing.Size(1012, 130)
        'Me.C1Ribbon1.Tabs.Add(Me.RibbonTab1)
        'Me.C1Ribbon1.TopToolBarHolder = Me.RibbonTopToolBar1
        'Me.C1Ribbon1.VisualStyle = C1.Win.C1Ribbon.VisualStyle.Office2007Black
        '
        'RibbonApplicationMenu1
        '
        'Me.RibbonApplicationMenu1.LargeImage = CType(resources.GetObject("RibbonApplicationMenu1.LargeImage"), System.Drawing.Image)
        'Me.RibbonApplicationMenu1.Name = "RibbonApplicationMenu1"
        'Me.RibbonApplicationMenu1.Visible = False
        ''
        ''RibbonConfigToolBar1
        ''
        'Me.RibbonConfigToolBar1.Name = "RibbonConfigToolBar1"
        ''
        ''RibbonQat1
        ''
        'Me.RibbonQat1.Name = "RibbonQat1"
        'Me.RibbonQat1.Visible = False
        ''
        ''RibbonTab1
        ''
        'Me.RibbonTab1.Groups.Add(Me.RibbonGroup6)
        'Me.RibbonTab1.Image = CType(resources.GetObject("RibbonTab1.Image"), System.Drawing.Image)
        'Me.RibbonTab1.Name = "RibbonTab1"
        'Me.RibbonTab1.Text = "NOVO CLIENTE"
        ''
        ''RibbonGroup6
        ''
        'Me.RibbonGroup6.Items.Add(Me.ribRetornar)
        'Me.RibbonGroup6.Items.Add(Me.btnRibSalvar)
        'Me.RibbonGroup6.Items.Add(Me.btnRibUndo)
        'Me.RibbonGroup6.Name = "RibbonGroup6"
        ''
        ''ribRetornar
        ''
        'Me.ribRetornar.LargeImage = CType(resources.GetObject("ribRetornar.LargeImage"), System.Drawing.Image)
        'Me.ribRetornar.Name = "ribRetornar"
        'Me.ribRetornar.SmallImage = CType(resources.GetObject("ribRetornar.SmallImage"), System.Drawing.Image)
        'Me.ribRetornar.Text = "Retornar"
        'Me.ribRetornar.TextImageRelation = C1.Win.C1Ribbon.TextImageRelation.ImageAboveText
        ''
        ''btnRibSalvar
        ''
        'Me.btnRibSalvar.LargeImage = CType(resources.GetObject("btnRibSalvar.LargeImage"), System.Drawing.Image)
        'Me.btnRibSalvar.Name = "btnRibSalvar"
        'Me.btnRibSalvar.SmallImage = CType(resources.GetObject("btnRibSalvar.SmallImage"), System.Drawing.Image)
        'Me.btnRibSalvar.Text = "Salvar"
        'Me.btnRibSalvar.TextImageRelation = C1.Win.C1Ribbon.TextImageRelation.ImageAboveText
        ''
        ''btnRibUndo
        ''
        'Me.btnRibUndo.LargeImage = CType(resources.GetObject("btnRibUndo.LargeImage"), System.Drawing.Image)
        'Me.btnRibUndo.Name = "btnRibUndo"
        'Me.btnRibUndo.SmallImage = CType(resources.GetObject("btnRibUndo.SmallImage"), System.Drawing.Image)
        'Me.btnRibUndo.Text = "Cancelar"
        'Me.btnRibUndo.TextImageRelation = C1.Win.C1Ribbon.TextImageRelation.ImageAboveText
        '
        'RibbonTopToolBar1
        '
        'Me.RibbonTopToolBar1.Name = "RibbonTopToolBar1"
        'Me.RibbonTopToolBar1.Visible = False
        ''
        ''RibbonBottomToolBar1
        ''
        'Me.RibbonBottomToolBar1.Name = "RibbonBottomToolBar1"
        '
        'C1PictureBox1
        '
        Me.C1PictureBox1.Image = CType(resources.GetObject("C1PictureBox1.Image"), System.Drawing.Image)
        Me.C1PictureBox1.Location = New System.Drawing.Point(304, 140)
        Me.C1PictureBox1.Name = "C1PictureBox1"
        Me.C1PictureBox1.Size = New System.Drawing.Size(404, 35)
        Me.C1PictureBox1.TabIndex = 4
        Me.C1PictureBox1.TabStop = False
        '
        'SoloDataSet
        '
        Me.SoloDataSet.DataSetName = "SoloDataSet"
        Me.SoloDataSet.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema
        '
        'ClientesBindingSource
        '
        Me.ClientesBindingSource.DataMember = "Clientes"
        Me.ClientesBindingSource.DataSource = Me.SoloDataSet
        '
        'ClientesTableAdapter
        '
        Me.ClientesTableAdapter.ClearBeforeFill = True
        '
        'TableAdapterManager
        '
        Me.TableAdapterManager.BackupDataSetBeforeUpdate = False
        Me.TableAdapterManager.ChavesTableAdapter = Nothing
        Me.TableAdapterManager.ClientesTableAdapter = Me.ClientesTableAdapter
        Me.TableAdapterManager.ContasTableAdapter = Nothing
        Me.TableAdapterManager.ExtratoTableAdapter = Nothing
        Me.TableAdapterManager.PerfisTableAdapter = Nothing
        Me.TableAdapterManager.UpdateOrder = Solo.SoloDataSetTableAdapters.TableAdapterManager.UpdateOrderOption.InsertUpdateDelete
        Me.TableAdapterManager.UsuáriosTableAdapter = Nothing
        '
        'btnCodigo
        '
        Me.btnCodigo.Location = New System.Drawing.Point(428, 327)
        Me.btnCodigo.Name = "btnCodigo"
        Me.btnCodigo.Size = New System.Drawing.Size(87, 23)
        Me.btnCodigo.TabIndex = 8
        Me.btnCodigo.TabStop = False
        Me.btnCodigo.Text = "Gerar código"
        Me.btnCodigo.UseVisualStyleBackColor = True
        '
        'Highlighter1
        '
        Me.Highlighter1.ContainerControl = Me
        Me.Highlighter1.FocusHighlightColor = DevComponents.DotNetBar.Validator.eHighlightColor.Red
        '
        'CódigoTextBox
        '
        Me.CódigoTextBox.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.ClientesBindingSource, "Código", True))
        Me.CódigoTextBox.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Highlighter1.SetHighlightOnFocus(Me.CódigoTextBox, True)
        Me.CódigoTextBox.Location = New System.Drawing.Point(319, 327)
        Me.CódigoTextBox.Name = "CódigoTextBox"
        Me.CódigoTextBox.Size = New System.Drawing.Size(100, 25)
        Me.CódigoTextBox.TabIndex = 9
        Me.CódigoTextBox.Tag = "1"
        '
        'ClienteTextBox
        '
        Me.ClienteTextBox.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper
        Me.ClienteTextBox.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.ClientesBindingSource, "Cliente", True))
        Me.ClienteTextBox.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Highlighter1.SetHighlightOnFocus(Me.ClienteTextBox, True)
        Me.ClienteTextBox.Location = New System.Drawing.Point(319, 359)
        Me.ClienteTextBox.Name = "ClienteTextBox"
        Me.ClienteTextBox.Size = New System.Drawing.Size(431, 25)
        Me.ClienteTextBox.TabIndex = 10
        '
        'IdTextBox
        '
        Me.IdTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.IdTextBox.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.ClientesBindingSource, "Id", True))
        Me.IdTextBox.ForeColor = System.Drawing.Color.White
        Me.IdTextBox.Location = New System.Drawing.Point(800, 246)
        Me.IdTextBox.Name = "IdTextBox"
        Me.IdTextBox.Size = New System.Drawing.Size(100, 13)
        Me.IdTextBox.TabIndex = 12
        Me.IdTextBox.TabStop = False
        '
        'Timer1
        '
        Me.Timer1.Enabled = True
        Me.Timer1.Interval = 1000
        '
        'lbCliente
        '
        Me.lbCliente.Font = New System.Drawing.Font("Segoe UI Semibold", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lbCliente.ForeColor = System.Drawing.Color.DarkRed
        Me.lbCliente.Location = New System.Drawing.Point(7, 190)
        Me.lbCliente.Name = "lbCliente"
        Me.lbCliente.Size = New System.Drawing.Size(999, 25)
        Me.lbCliente.TabIndex = 22
        Me.lbCliente.Text = "NOVO CLIENTE"
        Me.lbCliente.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'frmClienteNovo
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.White
        Me.ClientSize = New System.Drawing.Size(1012, 691)
        Me.Controls.Add(Me.lbCliente)
        Me.Controls.Add(Me.IdTextBox)
        Me.Controls.Add(ClienteLabel)
        Me.Controls.Add(Me.ClienteTextBox)
        Me.Controls.Add(CódigoLabel)
        Me.Controls.Add(Me.CódigoTextBox)
        Me.Controls.Add(Me.btnCodigo)
        Me.Controls.Add(Me.C1PictureBox1)
        'Me.Controls.Add(Me.C1Ribbon1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D
        Me.KeyPreview = True
        Me.Name = "frmClienteNovo"
        '
        '
        '
        Me.RootElement.ApplyShapeToControl = True
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "SOLO CONSULTORIA DE IMÓVEIS"
        Me.ThemeName = "Office2010Black"
        'CType(Me.C1Ribbon1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.C1PictureBox1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.SoloDataSet, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.ClientesBindingSource, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    'Friend WithEvents C1Ribbon1 As C1.Win.C1Ribbon.C1Ribbon
    'Friend WithEvents RibbonApplicationMenu1 As C1.Win.C1Ribbon.RibbonApplicationMenu
    'Friend WithEvents RibbonConfigToolBar1 As C1.Win.C1Ribbon.RibbonConfigToolBar
    'Friend WithEvents RibbonQat1 As C1.Win.C1Ribbon.RibbonQat
    'Friend WithEvents RibbonTab1 As C1.Win.C1Ribbon.RibbonTab
    'Friend WithEvents RibbonGroup6 As C1.Win.C1Ribbon.RibbonGroup
    'Friend WithEvents ribRetornar As C1.Win.C1Ribbon.RibbonButton
    'Friend WithEvents btnRibSalvar As C1.Win.C1Ribbon.RibbonButton
    'Friend WithEvents btnRibUndo As C1.Win.C1Ribbon.RibbonButton
    Friend WithEvents C1PictureBox1 As C1.Win.C1Input.C1PictureBox
    Friend WithEvents Office2010BlackTheme1 As Telerik.WinControls.Themes.Office2010BlackTheme
    Friend WithEvents SoloDataSet As Solo.SoloDataSet
    Friend WithEvents ClientesBindingSource As System.Windows.Forms.BindingSource
    Friend WithEvents ClientesTableAdapter As Solo.SoloDataSetTableAdapters.ClientesTableAdapter
    Friend WithEvents TableAdapterManager As Solo.SoloDataSetTableAdapters.TableAdapterManager
    Friend WithEvents btnCodigo As System.Windows.Forms.Button
    Friend WithEvents Highlighter1 As DevComponents.DotNetBar.Validator.Highlighter
    Friend WithEvents ClienteTextBox As System.Windows.Forms.TextBox
    Friend WithEvents CódigoTextBox As System.Windows.Forms.TextBox
    Friend WithEvents IdTextBox As System.Windows.Forms.TextBox
    Friend WithEvents Timer1 As System.Windows.Forms.Timer
    Friend WithEvents lbCliente As System.Windows.Forms.Label
    'Friend WithEvents RibbonBottomToolBar1 As C1.Win.C1Ribbon.RibbonBottomToolBar
    'Friend WithEvents RibbonTopToolBar1 As C1.Win.C1Ribbon.RibbonTopToolBar
End Class

