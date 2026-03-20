<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class FrmPrincipal
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
        Dim GridViewDecimalColumn1 As Telerik.WinControls.UI.GridViewDecimalColumn = New Telerik.WinControls.UI.GridViewDecimalColumn()
        Dim GridViewDecimalColumn2 As Telerik.WinControls.UI.GridViewDecimalColumn = New Telerik.WinControls.UI.GridViewDecimalColumn()
        Dim GridViewTextBoxColumn1 As Telerik.WinControls.UI.GridViewTextBoxColumn = New Telerik.WinControls.UI.GridViewTextBoxColumn()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(FrmPrincipal))
        Me.Office2010BlackTheme1 = New Telerik.WinControls.Themes.Office2010BlackTheme()
        Me.RadGridView1 = New Telerik.WinControls.UI.RadGridView()
        Me.ClientesBindingSource = New System.Windows.Forms.BindingSource(Me.components)
        Me.SoloDataSet = New Solo.SoloDataSet()
        Me.ClientesTableAdapter = New Solo.SoloDataSetTableAdapters.ClientesTableAdapter()
        Me.C1PictureBox1 = New C1.Win.C1Input.C1PictureBox()
        Me.C1Ribbon1 = New C1.Win.C1Ribbon.C1Ribbon()
        Me.RibbonApplicationMenu1 = New C1.Win.C1Ribbon.RibbonApplicationMenu()
        Me.RibbonBottomToolBar1 = New C1.Win.C1Ribbon.RibbonBottomToolBar()
        Me.RibbonConfigToolBar1 = New C1.Win.C1Ribbon.RibbonConfigToolBar()
        Me.RibbonQat1 = New C1.Win.C1Ribbon.RibbonQat()
        Me.RibbonTab1 = New C1.Win.C1Ribbon.RibbonTab()
        Me.RibbonGroup6 = New C1.Win.C1Ribbon.RibbonGroup()
        Me.btnLançar = New C1.Win.C1Ribbon.RibbonButton()
        Me.btnExtrato = New C1.Win.C1Ribbon.RibbonButton()
        Me.RibbonSeparator1 = New C1.Win.C1Ribbon.RibbonSeparator()
        Me.btnAlterar = New C1.Win.C1Ribbon.RibbonButton()
        Me.RibbonGroup2 = New C1.Win.C1Ribbon.RibbonGroup()
        Me.btnNovoCliente = New C1.Win.C1Ribbon.RibbonButton()
        Me.btnRpt = New C1.Win.C1Ribbon.RibbonButton()
        Me.btnTotais = New C1.Win.C1Ribbon.RibbonButton()
        Me.btnBackup = New C1.Win.C1Ribbon.RibbonButton()
        Me.btnUsuarios = New C1.Win.C1Ribbon.RibbonButton()
        Me.RibbonGroup1 = New C1.Win.C1Ribbon.RibbonGroup()
        Me.ribRetornar = New C1.Win.C1Ribbon.RibbonButton()
        Me.RibbonTopToolBar1 = New C1.Win.C1Ribbon.RibbonTopToolBar()
        Me.EllipseShape1 = New Telerik.WinControls.EllipseShape()
        Me.lblUsuario = New System.Windows.Forms.Label()
        Me.txtPesq = New System.Windows.Forms.TextBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Timer1 = New System.Windows.Forms.Timer(Me.components)
        CType(Me.RadGridView1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.RadGridView1.MasterTemplate, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.ClientesBindingSource, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.SoloDataSet, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.C1PictureBox1, System.ComponentModel.ISupportInitialize).BeginInit()
        'CType(Me.C1Ribbon1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'RadGridView1
        '
        Me.RadGridView1.BackColor = System.Drawing.Color.FromArgb(CType(CType(146, Byte), Integer), CType(CType(146, Byte), Integer), CType(CType(146, Byte), Integer))
        Me.RadGridView1.Cursor = System.Windows.Forms.Cursors.Default
        Me.RadGridView1.Font = New System.Drawing.Font("Segoe UI", 9.0!)
        Me.RadGridView1.ForeColor = System.Drawing.SystemColors.ControlText
        Me.RadGridView1.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.RadGridView1.Location = New System.Drawing.Point(157, 186)
        '
        'RadGridView1
        '
        Me.RadGridView1.MasterTemplate.AllowAddNewRow = False
        GridViewDecimalColumn1.DataType = GetType(Integer)
        GridViewDecimalColumn1.EnableExpressionEditor = False
        GridViewDecimalColumn1.FieldName = "Id"
        GridViewDecimalColumn1.HeaderText = "Id"
        GridViewDecimalColumn1.IsAutoGenerated = True
        GridViewDecimalColumn1.IsVisible = False
        GridViewDecimalColumn1.Name = "Id"
        GridViewDecimalColumn1.ReadOnly = True
        GridViewDecimalColumn2.DataType = GetType(Integer)
        GridViewDecimalColumn2.EnableExpressionEditor = False
        GridViewDecimalColumn2.FieldName = "Código"
        GridViewDecimalColumn2.HeaderText = "Código"
        GridViewDecimalColumn2.IsAutoGenerated = True
        GridViewDecimalColumn2.Name = "Código"
        GridViewDecimalColumn2.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter
        GridViewDecimalColumn2.Width = 114
        GridViewTextBoxColumn1.EnableExpressionEditor = False
        GridViewTextBoxColumn1.FieldName = "Cliente"
        GridViewTextBoxColumn1.HeaderText = "Cliente"
        GridViewTextBoxColumn1.IsAutoGenerated = True
        GridViewTextBoxColumn1.Name = "Cliente"
        GridViewTextBoxColumn1.Width = 548
        Me.RadGridView1.MasterTemplate.Columns.AddRange(New Telerik.WinControls.UI.GridViewDataColumn() {GridViewDecimalColumn1, GridViewDecimalColumn2, GridViewTextBoxColumn1})
        Me.RadGridView1.MasterTemplate.DataSource = Me.ClientesBindingSource
        Me.RadGridView1.MasterTemplate.EnableAlternatingRowColor = True
        Me.RadGridView1.MasterTemplate.EnableFiltering = True
        Me.RadGridView1.MasterTemplate.EnableGrouping = False
        Me.RadGridView1.MasterTemplate.ShowFilterCellOperatorText = False
        Me.RadGridView1.Name = "RadGridView1"
        Me.RadGridView1.ReadOnly = True
        Me.RadGridView1.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.RadGridView1.Size = New System.Drawing.Size(698, 470)
        Me.RadGridView1.TabIndex = 1
        Me.RadGridView1.Text = "RadGridView1"
        Me.RadGridView1.ThemeName = "Office2010Black"
        '
        'ClientesBindingSource
        '
        Me.ClientesBindingSource.DataMember = "Clientes"
        Me.ClientesBindingSource.DataSource = Me.SoloDataSet
        Me.ClientesBindingSource.Sort = "Cliente"
        '
        'SoloDataSet
        '
        Me.SoloDataSet.DataSetName = "SoloDataSet"
        Me.SoloDataSet.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema
        '
        'ClientesTableAdapter
        '
        Me.ClientesTableAdapter.ClearBeforeFill = True
        '
        'C1PictureBox1
        '
        Me.C1PictureBox1.Image = CType(resources.GetObject("C1PictureBox1.Image"), System.Drawing.Image)
        Me.C1PictureBox1.Location = New System.Drawing.Point(304, 140)
        Me.C1PictureBox1.Name = "C1PictureBox1"
        Me.C1PictureBox1.Size = New System.Drawing.Size(404, 35)
        Me.C1PictureBox1.TabIndex = 3
        Me.C1PictureBox1.TabStop = False
        '
        'C1Ribbon1
        '
        'Me.C1Ribbon1.ApplicationMenuHolder = Me.RibbonApplicationMenu1
        'Me.C1Ribbon1.BottomToolBarHolder = Me.RibbonBottomToolBar1
        'Me.C1Ribbon1.ConfigToolBarHolder = Me.RibbonConfigToolBar1
        'Me.C1Ribbon1.Font = New System.Drawing.Font("Calibri Light", 11.25!)
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
        Me.RibbonApplicationMenu1.LargeImage = CType(resources.GetObject("RibbonApplicationMenu1.LargeImage"), System.Drawing.Image)
        Me.RibbonApplicationMenu1.Name = "RibbonApplicationMenu1"
        Me.RibbonApplicationMenu1.Visible = False
        '
        'RibbonBottomToolBar1
        '
        Me.RibbonBottomToolBar1.Name = "RibbonBottomToolBar1"
        '
        'RibbonConfigToolBar1
        '
        Me.RibbonConfigToolBar1.Name = "RibbonConfigToolBar1"
        '
        'RibbonQat1
        '
        Me.RibbonQat1.Name = "RibbonQat1"
        Me.RibbonQat1.Visible = False
        '
        'RibbonTab1
        '
        Me.RibbonTab1.Groups.Add(Me.RibbonGroup6)
        Me.RibbonTab1.Groups.Add(Me.RibbonGroup2)
        Me.RibbonTab1.Groups.Add(Me.RibbonGroup1)
        Me.RibbonTab1.Image = CType(resources.GetObject("RibbonTab1.Image"), System.Drawing.Image)
        Me.RibbonTab1.Name = "RibbonTab1"
        Me.RibbonTab1.Text = "Contas Correntes"
        '
        'RibbonGroup6
        '
        Me.RibbonGroup6.Items.Add(Me.btnLançar)
        Me.RibbonGroup6.Items.Add(Me.btnExtrato)
        Me.RibbonGroup6.Items.Add(Me.RibbonSeparator1)
        Me.RibbonGroup6.Items.Add(Me.btnAlterar)
        Me.RibbonGroup6.Name = "RibbonGroup6"
        Me.RibbonGroup6.Text = "Cód."
        '
        'btnLançar
        '
        Me.btnLançar.LargeImage = CType(resources.GetObject("btnLançar.LargeImage"), System.Drawing.Image)
        Me.btnLançar.Name = "btnLançar"
        Me.btnLançar.Text = "Lançamentos"
        Me.btnLançar.TextImageRelation = C1.Win.C1Ribbon.TextImageRelation.ImageAboveText
        '
        'btnExtrato
        '
        Me.btnExtrato.LargeImage = CType(resources.GetObject("btnExtrato.LargeImage"), System.Drawing.Image)
        Me.btnExtrato.Name = "btnExtrato"
        Me.btnExtrato.SmallImage = CType(resources.GetObject("btnExtrato.SmallImage"), System.Drawing.Image)
        Me.btnExtrato.Text = "Conta corrente"
        Me.btnExtrato.TextImageRelation = C1.Win.C1Ribbon.TextImageRelation.ImageAboveText
        '
        'RibbonSeparator1
        '
        Me.RibbonSeparator1.Name = "RibbonSeparator1"
        '
        'btnAlterar
        '
        Me.btnAlterar.LargeImage = CType(resources.GetObject("btnAlterar.LargeImage"), System.Drawing.Image)
        Me.btnAlterar.Name = "btnAlterar"
        Me.btnAlterar.SmallImage = CType(resources.GetObject("btnAlterar.SmallImage"), System.Drawing.Image)
        Me.btnAlterar.Text = "Alterar"
        Me.btnAlterar.TextImageRelation = C1.Win.C1Ribbon.TextImageRelation.ImageAboveText
        '
        'RibbonGroup2
        '
        Me.RibbonGroup2.Items.Add(Me.btnNovoCliente)
        Me.RibbonGroup2.Items.Add(Me.btnRpt)
        Me.RibbonGroup2.Items.Add(Me.btnTotais)
        Me.RibbonGroup2.Items.Add(Me.btnBackup)
        Me.RibbonGroup2.Items.Add(Me.btnUsuarios)
        Me.RibbonGroup2.Name = "RibbonGroup2"
        '
        'btnNovoCliente
        '
        Me.btnNovoCliente.LargeImage = CType(resources.GetObject("btnNovoCliente.LargeImage"), System.Drawing.Image)
        Me.btnNovoCliente.Name = "btnNovoCliente"
        Me.btnNovoCliente.SmallImage = CType(resources.GetObject("btnNovoCliente.SmallImage"), System.Drawing.Image)
        Me.btnNovoCliente.Text = "Novo Cliente"
        Me.btnNovoCliente.TextImageRelation = C1.Win.C1Ribbon.TextImageRelation.ImageAboveText
        '
        'btnRpt
        '
        Me.btnRpt.LargeImage = CType(resources.GetObject("btnRpt.LargeImage"), System.Drawing.Image)
        Me.btnRpt.Name = "btnRpt"
        Me.btnRpt.SmallImage = CType(resources.GetObject("btnRpt.SmallImage"), System.Drawing.Image)
        Me.btnRpt.Text = "Relatórios"
        Me.btnRpt.TextImageRelation = C1.Win.C1Ribbon.TextImageRelation.ImageAboveText
        '
        'btnTotais
        '
        Me.btnTotais.LargeImage = CType(resources.GetObject("btnTotais.LargeImage"), System.Drawing.Image)
        Me.btnTotais.Name = "btnTotais"
        Me.btnTotais.SmallImage = CType(resources.GetObject("btnTotais.SmallImage"), System.Drawing.Image)
        Me.btnTotais.Text = "Totais"
        Me.btnTotais.TextImageRelation = C1.Win.C1Ribbon.TextImageRelation.ImageAboveText
        '
        'btnBackup
        '
        Me.btnBackup.Enabled = False
        Me.btnBackup.LargeImage = CType(resources.GetObject("btnBackup.LargeImage"), System.Drawing.Image)
        Me.btnBackup.Name = "btnBackup"
        Me.btnBackup.SmallImage = CType(resources.GetObject("btnBackup.SmallImage"), System.Drawing.Image)
        Me.btnBackup.Text = "Backup"
        Me.btnBackup.TextImageRelation = C1.Win.C1Ribbon.TextImageRelation.ImageAboveText
        '
        'btnUsuarios
        '
        Me.btnUsuarios.LargeImage = CType(resources.GetObject("btnUsuarios.LargeImage"), System.Drawing.Image)
        Me.btnUsuarios.Name = "btnUsuarios"
        Me.btnUsuarios.SmallImage = CType(resources.GetObject("btnUsuarios.SmallImage"), System.Drawing.Image)
        Me.btnUsuarios.Text = "Usuários e Senhas"
        Me.btnUsuarios.TextImageRelation = C1.Win.C1Ribbon.TextImageRelation.ImageAboveText
        '
        'RibbonGroup1
        '
        Me.RibbonGroup1.Items.Add(Me.ribRetornar)
        Me.RibbonGroup1.Name = "RibbonGroup1"
        '
        'ribRetornar
        '
        Me.ribRetornar.LargeImage = CType(resources.GetObject("ribRetornar.LargeImage"), System.Drawing.Image)
        Me.ribRetornar.Name = "ribRetornar"
        Me.ribRetornar.SmallImage = CType(resources.GetObject("ribRetornar.SmallImage"), System.Drawing.Image)
        Me.ribRetornar.Text = "Sair"
        Me.ribRetornar.TextImageRelation = C1.Win.C1Ribbon.TextImageRelation.ImageBeforeText
        '
        'RibbonTopToolBar1
        '
        Me.RibbonTopToolBar1.Name = "RibbonTopToolBar1"
        Me.RibbonTopToolBar1.Visible = False
        '
        'lblUsuario
        '
        Me.lblUsuario.BackColor = System.Drawing.Color.Gainsboro
        Me.lblUsuario.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblUsuario.Location = New System.Drawing.Point(-1, 676)
        Me.lblUsuario.Name = "lblUsuario"
        Me.lblUsuario.Size = New System.Drawing.Size(1014, 15)
        Me.lblUsuario.TabIndex = 4
        Me.lblUsuario.Text = "Label1"
        '
        'txtPesq
        '
        Me.txtPesq.Font = New System.Drawing.Font("Segoe UI", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtPesq.Location = New System.Drawing.Point(861, 186)
        Me.txtPesq.Name = "txtPesq"
        Me.txtPesq.Size = New System.Drawing.Size(149, 23)
        Me.txtPesq.TabIndex = 0
        Me.txtPesq.TabStop = False
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Font = New System.Drawing.Font("Segoe UI", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label1.ForeColor = System.Drawing.Color.DarkRed
        Me.Label1.Location = New System.Drawing.Point(858, 168)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(99, 15)
        Me.Label1.TabIndex = 9
        Me.Label1.Text = "Localizar cliente:"
        '
        'Timer1
        '
        Me.Timer1.Enabled = True
        Me.Timer1.Interval = 500
        '
        'FrmPrincipal
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.White
        Me.ClientSize = New System.Drawing.Size(1012, 691)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.txtPesq)
        Me.Controls.Add(Me.lblUsuario)
        'Me.Controls.Add(Me.C1Ribbon1)
        Me.Controls.Add(Me.C1PictureBox1)
        Me.Controls.Add(Me.RadGridView1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D
        Me.Name = "FrmPrincipal"
        '
        '
        '
        Me.RootElement.ApplyShapeToControl = True
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "SOLO CONSULTORIA DE IMÓVEIS"
        Me.ThemeName = "Office2010Black"
        CType(Me.RadGridView1.MasterTemplate, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.RadGridView1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.ClientesBindingSource, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.SoloDataSet, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.C1PictureBox1, System.ComponentModel.ISupportInitialize).EndInit()
        'CType(Me.C1Ribbon1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents Office2010BlackTheme1 As Telerik.WinControls.Themes.Office2010BlackTheme
    Friend WithEvents RadGridView1 As Telerik.WinControls.UI.RadGridView
    Friend WithEvents SoloDataSet As Solo.SoloDataSet
    Friend WithEvents ClientesBindingSource As System.Windows.Forms.BindingSource
    Friend WithEvents ClientesTableAdapter As Solo.SoloDataSetTableAdapters.ClientesTableAdapter
    Friend WithEvents C1PictureBox1 As C1.Win.C1Input.C1PictureBox
    Friend WithEvents C1Ribbon1 As C1.Win.C1Ribbon.C1Ribbon
    Friend WithEvents RibbonApplicationMenu1 As C1.Win.C1Ribbon.RibbonApplicationMenu
    Friend WithEvents RibbonConfigToolBar1 As C1.Win.C1Ribbon.RibbonConfigToolBar
    Friend WithEvents RibbonQat1 As C1.Win.C1Ribbon.RibbonQat
    Friend WithEvents RibbonTab1 As C1.Win.C1Ribbon.RibbonTab
    Friend WithEvents RibbonGroup6 As C1.Win.C1Ribbon.RibbonGroup
    Friend WithEvents btnLançar As C1.Win.C1Ribbon.RibbonButton
    Friend WithEvents btnExtrato As C1.Win.C1Ribbon.RibbonButton
    Friend WithEvents RibbonSeparator1 As C1.Win.C1Ribbon.RibbonSeparator
    Friend WithEvents btnAlterar As C1.Win.C1Ribbon.RibbonButton
    Friend WithEvents RibbonGroup2 As C1.Win.C1Ribbon.RibbonGroup
    Friend WithEvents btnNovoCliente As C1.Win.C1Ribbon.RibbonButton
    Friend WithEvents btnRpt As C1.Win.C1Ribbon.RibbonButton
    Friend WithEvents btnTotais As C1.Win.C1Ribbon.RibbonButton
    Friend WithEvents btnBackup As C1.Win.C1Ribbon.RibbonButton
    Friend WithEvents btnUsuarios As C1.Win.C1Ribbon.RibbonButton
    Friend WithEvents RibbonGroup1 As C1.Win.C1Ribbon.RibbonGroup
    Friend WithEvents ribRetornar As C1.Win.C1Ribbon.RibbonButton
    Friend WithEvents EllipseShape1 As Telerik.WinControls.EllipseShape
    Friend WithEvents lblUsuario As System.Windows.Forms.Label
    Friend WithEvents txtPesq As System.Windows.Forms.TextBox
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents Timer1 As System.Windows.Forms.Timer
    Friend WithEvents RibbonBottomToolBar1 As C1.Win.C1Ribbon.RibbonBottomToolBar
    Friend WithEvents RibbonTopToolBar1 As C1.Win.C1Ribbon.RibbonTopToolBar
End Class

