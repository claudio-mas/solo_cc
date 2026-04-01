<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class FrmUsuarios
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(FrmUsuarios))
        Dim GridViewDecimalColumn1 As Telerik.WinControls.UI.GridViewDecimalColumn = New Telerik.WinControls.UI.GridViewDecimalColumn()
        Dim GridViewTextBoxColumn1 As Telerik.WinControls.UI.GridViewTextBoxColumn = New Telerik.WinControls.UI.GridViewTextBoxColumn()
        Dim GridViewTextBoxColumn2 As Telerik.WinControls.UI.GridViewTextBoxColumn = New Telerik.WinControls.UI.GridViewTextBoxColumn()
        Dim GridViewComboBoxColumn1 As Telerik.WinControls.UI.GridViewComboBoxColumn = New Telerik.WinControls.UI.GridViewComboBoxColumn()
        Dim GridViewDecimalColumn2 As Telerik.WinControls.UI.GridViewDecimalColumn = New Telerik.WinControls.UI.GridViewDecimalColumn()
        Dim GridViewTextBoxColumn3 As Telerik.WinControls.UI.GridViewTextBoxColumn = New Telerik.WinControls.UI.GridViewTextBoxColumn()
        Dim GridViewTextBoxColumn4 As Telerik.WinControls.UI.GridViewTextBoxColumn = New Telerik.WinControls.UI.GridViewTextBoxColumn()
        Me.PerfisBindingSource = New System.Windows.Forms.BindingSource(Me.components)
        Me.SoloDataSet = New Solo.SoloDataSet()
        Me.C1Ribbon1 = New C1.Win.C1Ribbon.C1Ribbon()
        Me.RibbonApplicationMenu1 = New C1.Win.C1Ribbon.RibbonApplicationMenu()
        Me.RibbonConfigToolBar1 = New C1.Win.C1Ribbon.RibbonConfigToolBar()
        Me.RibbonQat1 = New C1.Win.C1Ribbon.RibbonQat()
        Me.RibbonTab1 = New C1.Win.C1Ribbon.RibbonTab()
        Me.RibbonGroup6 = New C1.Win.C1Ribbon.RibbonGroup()
        Me.ribRetornar = New C1.Win.C1Ribbon.RibbonButton()
        Me.btnRibSalvar = New C1.Win.C1Ribbon.RibbonButton()
        Me.btnRibUndo = New C1.Win.C1Ribbon.RibbonButton()
        Me.RibbonTopToolBar1 = New C1.Win.C1Ribbon.RibbonTopToolBar()
        Me.RibbonBottomToolBar1 = New C1.Win.C1Ribbon.RibbonBottomToolBar()
        Me.C1DockingTab1 = New C1.Win.C1Command.C1DockingTab()
        Me.C1DockingTabPage1 = New C1.Win.C1Command.C1DockingTabPage()
        Me.RadGridView2 = New Telerik.WinControls.UI.RadGridView()
        Me.UsuáriosBindingSource = New System.Windows.Forms.BindingSource(Me.components)
        Me.C1DockingTabPage2 = New C1.Win.C1Command.C1DockingTabPage()
        Me.RadGridView1 = New Telerik.WinControls.UI.RadGridView()
        Me.ChavesBindingSource = New System.Windows.Forms.BindingSource(Me.components)
        Me.C1PictureBox1 = New C1.Win.C1Input.C1PictureBox()
        Me.ChavesTableAdapter = New Solo.SoloDataSetTableAdapters.ChavesTableAdapter()
        Me.TableAdapterManager = New Solo.SoloDataSetTableAdapters.TableAdapterManager()
        Me.UsuáriosTableAdapter = New Solo.SoloDataSetTableAdapters.UsuáriosTableAdapter()
        Me.Office2010BlackTheme1 = New Telerik.WinControls.Themes.Office2010BlackTheme()
        Me.TableAdapterManager1 = New Solo.SoloDataSetTableAdapters.TableAdapterManager()
        Me.PerfisTableAdapter = New Solo.SoloDataSetTableAdapters.PerfisTableAdapter()
        CType(Me.PerfisBindingSource, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.SoloDataSet, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.C1Ribbon1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.C1DockingTab1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.C1DockingTab1.SuspendLayout()
        Me.C1DockingTabPage1.SuspendLayout()
        CType(Me.RadGridView2, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.RadGridView2.MasterTemplate, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.UsuáriosBindingSource, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.C1DockingTabPage2.SuspendLayout()
        CType(Me.RadGridView1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.RadGridView1.MasterTemplate, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.ChavesBindingSource, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.C1PictureBox1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'PerfisBindingSource
        '
        Me.PerfisBindingSource.DataMember = "Perfis"
        Me.PerfisBindingSource.DataSource = Me.SoloDataSet
        '
        'SoloDataSet
        '
        Me.SoloDataSet.DataSetName = "SoloDataSet"
        Me.SoloDataSet.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema
        '
        'C1Ribbon1
        '
        Me.C1Ribbon1.ApplicationMenuHolder = Me.RibbonApplicationMenu1
        Me.C1Ribbon1.BottomToolBarHolder = Me.RibbonBottomToolBar1
        Me.C1Ribbon1.ConfigToolBarHolder = Me.RibbonConfigToolBar1
        Me.C1Ribbon1.Font = New System.Drawing.Font("Calibri Light", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.C1Ribbon1.Location = New System.Drawing.Point(0, 0)
        Me.C1Ribbon1.Name = "C1Ribbon1"
        Me.C1Ribbon1.QatHolder = Me.RibbonQat1
        Me.C1Ribbon1.Size = New System.Drawing.Size(1012, 130)
        Me.C1Ribbon1.Tabs.Add(Me.RibbonTab1)
        Me.C1Ribbon1.TopToolBarHolder = Me.RibbonTopToolBar1
        Me.C1Ribbon1.VisualStyle = C1.Win.C1Ribbon.VisualStyle.Office2007Black
        '
        'RibbonApplicationMenu1
        '
        Me.RibbonApplicationMenu1.LargeImage = CType(resources.GetObject("RibbonApplicationMenu1.LargeImage"), System.Drawing.Image)
        Me.RibbonApplicationMenu1.Name = "RibbonApplicationMenu1"
        Me.RibbonApplicationMenu1.Visible = False
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
        Me.RibbonTab1.Image = CType(resources.GetObject("RibbonTab1.Image"), System.Drawing.Image)
        Me.RibbonTab1.Name = "RibbonTab1"
        Me.RibbonTab1.Text = "Contas Correntes"
        '
        'RibbonGroup6
        '
        Me.RibbonGroup6.Items.Add(Me.ribRetornar)
        Me.RibbonGroup6.Items.Add(Me.btnRibSalvar)
        Me.RibbonGroup6.Items.Add(Me.btnRibUndo)
        Me.RibbonGroup6.Name = "RibbonGroup6"
        '
        'ribRetornar
        '
        Me.ribRetornar.LargeImage = CType(resources.GetObject("ribRetornar.LargeImage"), System.Drawing.Image)
        Me.ribRetornar.Name = "ribRetornar"
        Me.ribRetornar.SmallImage = CType(resources.GetObject("ribRetornar.SmallImage"), System.Drawing.Image)
        Me.ribRetornar.Text = "Retornar"
        Me.ribRetornar.TextImageRelation = C1.Win.C1Ribbon.TextImageRelation.ImageAboveText
        '
        'btnRibSalvar
        '
        Me.btnRibSalvar.LargeImage = CType(resources.GetObject("btnRibSalvar.LargeImage"), System.Drawing.Image)
        Me.btnRibSalvar.Name = "btnRibSalvar"
        Me.btnRibSalvar.SmallImage = CType(resources.GetObject("btnRibSalvar.SmallImage"), System.Drawing.Image)
        Me.btnRibSalvar.Text = "Salvar"
        Me.btnRibSalvar.TextImageRelation = C1.Win.C1Ribbon.TextImageRelation.ImageAboveText
        '
        'btnRibUndo
        '
        Me.btnRibUndo.LargeImage = CType(resources.GetObject("btnRibUndo.LargeImage"), System.Drawing.Image)
        Me.btnRibUndo.Name = "btnRibUndo"
        Me.btnRibUndo.SmallImage = CType(resources.GetObject("btnRibUndo.SmallImage"), System.Drawing.Image)
        Me.btnRibUndo.Text = "Cancelar"
        Me.btnRibUndo.TextImageRelation = C1.Win.C1Ribbon.TextImageRelation.ImageAboveText
        '
        'RibbonTopToolBar1
        '
        Me.RibbonTopToolBar1.Name = "RibbonTopToolBar1"
        Me.RibbonTopToolBar1.Visible = False
        '
        'RibbonBottomToolBar1
        '
        Me.RibbonBottomToolBar1.Name = "RibbonBottomToolBar1"
        '
        'C1DockingTab1
        '
        Me.C1DockingTab1.Controls.Add(Me.C1DockingTabPage1)
        Me.C1DockingTab1.Controls.Add(Me.C1DockingTabPage2)
        Me.C1DockingTab1.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.C1DockingTab1.Location = New System.Drawing.Point(229, 225)
        Me.C1DockingTab1.Name = "C1DockingTab1"
        Me.C1DockingTab1.Size = New System.Drawing.Size(555, 393)
        Me.C1DockingTab1.TabIndex = 3
        Me.C1DockingTab1.TabsSpacing = 5
        Me.C1DockingTab1.TabStyle = C1.Win.C1Command.TabStyleEnum.Office2010
        Me.C1DockingTab1.VisualStyle = C1.Win.C1Command.VisualStyle.Custom
        Me.C1DockingTab1.VisualStyleBase = C1.Win.C1Command.VisualStyle.Office2010Blue
        '
        'C1DockingTabPage1
        '
        Me.C1DockingTabPage1.Controls.Add(Me.RadGridView2)
        Me.C1DockingTabPage1.Location = New System.Drawing.Point(1, 29)
        Me.C1DockingTabPage1.Name = "C1DockingTabPage1"
        Me.C1DockingTabPage1.Size = New System.Drawing.Size(553, 363)
        Me.C1DockingTabPage1.TabIndex = 0
        Me.C1DockingTabPage1.Text = "Usuários"
        '
        'RadGridView2
        '
        Me.RadGridView2.BackColor = System.Drawing.Color.White
        Me.RadGridView2.Cursor = System.Windows.Forms.Cursors.Default
        Me.RadGridView2.Font = New System.Drawing.Font("Segoe UI", 9.75!)
        Me.RadGridView2.ForeColor = System.Drawing.SystemColors.ControlText
        Me.RadGridView2.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.RadGridView2.Location = New System.Drawing.Point(43, 74)
        '
        'RadGridView2
        '
        Me.RadGridView2.MasterTemplate.AddNewRowPosition = Telerik.WinControls.UI.SystemRowPosition.Bottom
        Me.RadGridView2.MasterTemplate.AllowColumnResize = False
        Me.RadGridView2.MasterTemplate.AllowDeleteRow = False
        Me.RadGridView2.MasterTemplate.AllowRowResize = False
        Me.RadGridView2.MasterTemplate.AutoGenerateColumns = False
        GridViewDecimalColumn1.DataType = GetType(Integer)
        GridViewDecimalColumn1.EnableExpressionEditor = False
        GridViewDecimalColumn1.FieldName = "Id"
        GridViewDecimalColumn1.HeaderText = "Id"
        GridViewDecimalColumn1.IsAutoGenerated = True
        GridViewDecimalColumn1.IsVisible = False
        GridViewDecimalColumn1.Name = "Id"
        GridViewDecimalColumn1.ReadOnly = True
        GridViewTextBoxColumn1.EnableExpressionEditor = False
        GridViewTextBoxColumn1.FieldName = "Usuário"
        GridViewTextBoxColumn1.HeaderText = "Login"
        GridViewTextBoxColumn1.IsAutoGenerated = True
        GridViewTextBoxColumn1.Name = "Usuário"
        GridViewTextBoxColumn1.Width = 203
        GridViewTextBoxColumn2.EnableExpressionEditor = False
        GridViewTextBoxColumn2.FieldName = "Psw"
        GridViewTextBoxColumn2.HeaderText = "Senha"
        GridViewTextBoxColumn2.IsAutoGenerated = True
        GridViewTextBoxColumn2.Name = "Psw"
        GridViewTextBoxColumn2.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter
        GridViewTextBoxColumn2.Width = 94
        GridViewComboBoxColumn1.DataSource = Me.PerfisBindingSource
        GridViewComboBoxColumn1.DisplayMember = "Perfil"
        GridViewComboBoxColumn1.EnableExpressionEditor = False
        GridViewComboBoxColumn1.FieldName = "Perfil"
        GridViewComboBoxColumn1.HeaderText = "Perfil"
        GridViewComboBoxColumn1.Name = "Perfil"
        GridViewComboBoxColumn1.ValueMember = "Perfil"
        GridViewComboBoxColumn1.Width = 128
        Me.RadGridView2.MasterTemplate.Columns.AddRange(New Telerik.WinControls.UI.GridViewDataColumn() {GridViewDecimalColumn1, GridViewTextBoxColumn1, GridViewTextBoxColumn2, GridViewComboBoxColumn1})
        Me.RadGridView2.MasterTemplate.DataSource = Me.UsuáriosBindingSource
        Me.RadGridView2.Name = "RadGridView2"
        Me.RadGridView2.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.RadGridView2.ShowGroupPanel = False
        Me.RadGridView2.Size = New System.Drawing.Size(466, 216)
        Me.RadGridView2.TabIndex = 0
        Me.RadGridView2.Text = "RadGridView2"
        Me.RadGridView2.ThemeName = "Office2010Black"
        '
        'UsuáriosBindingSource
        '
        Me.UsuáriosBindingSource.DataMember = "Usuários"
        Me.UsuáriosBindingSource.DataSource = Me.SoloDataSet
        '
        'C1DockingTabPage2
        '
        Me.C1DockingTabPage2.Controls.Add(Me.RadGridView1)
        Me.C1DockingTabPage2.Location = New System.Drawing.Point(1, 29)
        Me.C1DockingTabPage2.Name = "C1DockingTabPage2"
        Me.C1DockingTabPage2.Size = New System.Drawing.Size(553, 363)
        Me.C1DockingTabPage2.TabIndex = 1
        Me.C1DockingTabPage2.Text = "Senhas"
        '
        'RadGridView1
        '
        Me.RadGridView1.BackColor = System.Drawing.Color.White
        Me.RadGridView1.Cursor = System.Windows.Forms.Cursors.Default
        Me.RadGridView1.Font = New System.Drawing.Font("Segoe UI", 9.75!)
        Me.RadGridView1.ForeColor = System.Drawing.SystemColors.ControlText
        Me.RadGridView1.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.RadGridView1.Location = New System.Drawing.Point(88, 74)
        '
        'RadGridView1
        '
        Me.RadGridView1.MasterTemplate.AllowAddNewRow = False
        Me.RadGridView1.MasterTemplate.AllowDeleteRow = False
        GridViewDecimalColumn2.DataType = GetType(Integer)
        GridViewDecimalColumn2.EnableExpressionEditor = False
        GridViewDecimalColumn2.FieldName = "Id"
        GridViewDecimalColumn2.HeaderText = "Id"
        GridViewDecimalColumn2.IsAutoGenerated = True
        GridViewDecimalColumn2.IsVisible = False
        GridViewDecimalColumn2.Name = "Id"
        GridViewDecimalColumn2.ReadOnly = True
        GridViewTextBoxColumn3.EnableExpressionEditor = False
        GridViewTextBoxColumn3.FieldName = "Ref"
        GridViewTextBoxColumn3.HeaderText = " "
        GridViewTextBoxColumn3.IsAutoGenerated = True
        GridViewTextBoxColumn3.Name = "Ref"
        GridViewTextBoxColumn3.ReadOnly = True
        GridViewTextBoxColumn3.Width = 253
        GridViewTextBoxColumn4.EnableExpressionEditor = False
        GridViewTextBoxColumn4.FieldName = "Chave"
        GridViewTextBoxColumn4.HeaderText = "Senha"
        GridViewTextBoxColumn4.IsAutoGenerated = True
        GridViewTextBoxColumn4.Name = "Chave"
        GridViewTextBoxColumn4.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter
        GridViewTextBoxColumn4.Width = 84
        Me.RadGridView1.MasterTemplate.Columns.AddRange(New Telerik.WinControls.UI.GridViewDataColumn() {GridViewDecimalColumn2, GridViewTextBoxColumn3, GridViewTextBoxColumn4})
        Me.RadGridView1.MasterTemplate.DataSource = Me.ChavesBindingSource
        Me.RadGridView1.MasterTemplate.EnableAlternatingRowColor = True
        Me.RadGridView1.Name = "RadGridView1"
        Me.RadGridView1.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.RadGridView1.ShowGroupPanel = False
        Me.RadGridView1.Size = New System.Drawing.Size(377, 215)
        Me.RadGridView1.TabIndex = 0
        Me.RadGridView1.Text = "RadGridView1"
        Me.RadGridView1.ThemeName = "Office2010Black"
        '
        'ChavesBindingSource
        '
        Me.ChavesBindingSource.DataMember = "Chaves"
        Me.ChavesBindingSource.DataSource = Me.SoloDataSet
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
        'ChavesTableAdapter
        '
        Me.ChavesTableAdapter.ClearBeforeFill = True
        '
        'TableAdapterManager
        '
        Me.TableAdapterManager.BackupDataSetBeforeUpdate = False
        Me.TableAdapterManager.ChavesTableAdapter = Me.ChavesTableAdapter
        Me.TableAdapterManager.ClientesTableAdapter = Nothing
        Me.TableAdapterManager.ContasTableAdapter = Nothing
        Me.TableAdapterManager.ExtratoTableAdapter = Nothing
        Me.TableAdapterManager.PerfisTableAdapter = Nothing
        Me.TableAdapterManager.UpdateOrder = Solo.SoloDataSetTableAdapters.TableAdapterManager.UpdateOrderOption.InsertUpdateDelete
        Me.TableAdapterManager.UsuáriosTableAdapter = Nothing
        '
        'UsuáriosTableAdapter
        '
        Me.UsuáriosTableAdapter.ClearBeforeFill = True
        '
        'TableAdapterManager1
        '
        Me.TableAdapterManager1.BackupDataSetBeforeUpdate = False
        Me.TableAdapterManager1.ChavesTableAdapter = Nothing
        Me.TableAdapterManager1.ClientesTableAdapter = Nothing
        Me.TableAdapterManager1.ContasTableAdapter = Nothing
        Me.TableAdapterManager1.ExtratoTableAdapter = Nothing
        Me.TableAdapterManager1.PerfisTableAdapter = Nothing
        Me.TableAdapterManager1.UpdateOrder = Solo.SoloDataSetTableAdapters.TableAdapterManager.UpdateOrderOption.InsertUpdateDelete
        Me.TableAdapterManager1.UsuáriosTableAdapter = Me.UsuáriosTableAdapter
        '
        'PerfisTableAdapter
        '
        Me.PerfisTableAdapter.ClearBeforeFill = True
        '
        'FrmUsuarios
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.White
        Me.ClientSize = New System.Drawing.Size(1012, 691)
        Me.Controls.Add(Me.C1PictureBox1)
        Me.Controls.Add(Me.C1DockingTab1)
        Me.Controls.Add(Me.C1Ribbon1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D
        Me.Name = "FrmUsuarios"
        '
        '
        '
        Me.RootElement.ApplyShapeToControl = True
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "SOLO CONSULTORIA DE IMÓVEIS"
        Me.ThemeName = "Office2010Black"
        CType(Me.PerfisBindingSource, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.SoloDataSet, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.C1Ribbon1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.C1DockingTab1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.C1DockingTab1.ResumeLayout(False)
        Me.C1DockingTabPage1.ResumeLayout(False)
        CType(Me.RadGridView2.MasterTemplate, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.RadGridView2, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.UsuáriosBindingSource, System.ComponentModel.ISupportInitialize).EndInit()
        Me.C1DockingTabPage2.ResumeLayout(False)
        CType(Me.RadGridView1.MasterTemplate, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.RadGridView1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.ChavesBindingSource, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.C1PictureBox1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents C1Ribbon1 As C1.Win.C1Ribbon.C1Ribbon
    Friend WithEvents RibbonApplicationMenu1 As C1.Win.C1Ribbon.RibbonApplicationMenu
    Friend WithEvents RibbonConfigToolBar1 As C1.Win.C1Ribbon.RibbonConfigToolBar
    Friend WithEvents RibbonQat1 As C1.Win.C1Ribbon.RibbonQat
    Friend WithEvents RibbonTab1 As C1.Win.C1Ribbon.RibbonTab
    Friend WithEvents RibbonGroup6 As C1.Win.C1Ribbon.RibbonGroup
    Friend WithEvents ribRetornar As C1.Win.C1Ribbon.RibbonButton
    Friend WithEvents btnRibSalvar As C1.Win.C1Ribbon.RibbonButton
    Friend WithEvents btnRibUndo As C1.Win.C1Ribbon.RibbonButton
    Friend WithEvents C1DockingTab1 As C1.Win.C1Command.C1DockingTab
    Friend WithEvents C1DockingTabPage1 As C1.Win.C1Command.C1DockingTabPage
    Friend WithEvents C1DockingTabPage2 As C1.Win.C1Command.C1DockingTabPage
    Friend WithEvents C1PictureBox1 As C1.Win.C1Input.C1PictureBox
    Friend WithEvents RadGridView1 As Telerik.WinControls.UI.RadGridView
    Friend WithEvents SoloDataSet As Solo.SoloDataSet
    Friend WithEvents ChavesBindingSource As System.Windows.Forms.BindingSource
    Friend WithEvents ChavesTableAdapter As Solo.SoloDataSetTableAdapters.ChavesTableAdapter
    Friend WithEvents TableAdapterManager As Solo.SoloDataSetTableAdapters.TableAdapterManager
    Friend WithEvents UsuáriosBindingSource As System.Windows.Forms.BindingSource
    Friend WithEvents UsuáriosTableAdapter As Solo.SoloDataSetTableAdapters.UsuáriosTableAdapter
    Friend WithEvents RadGridView2 As Telerik.WinControls.UI.RadGridView
    Friend WithEvents Office2010BlackTheme1 As Telerik.WinControls.Themes.Office2010BlackTheme
    Friend WithEvents TableAdapterManager1 As Solo.SoloDataSetTableAdapters.TableAdapterManager
    Friend WithEvents PerfisBindingSource As System.Windows.Forms.BindingSource
    Friend WithEvents PerfisTableAdapter As Solo.SoloDataSetTableAdapters.PerfisTableAdapter
    Friend WithEvents RibbonBottomToolBar1 As C1.Win.C1Ribbon.RibbonBottomToolBar
    Friend WithEvents RibbonTopToolBar1 As C1.Win.C1Ribbon.RibbonTopToolBar
End Class

