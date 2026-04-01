<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class FrmExtrato
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(FrmExtrato))
        Dim GridViewDecimalColumn1 As Telerik.WinControls.UI.GridViewDecimalColumn = New Telerik.WinControls.UI.GridViewDecimalColumn()
        Dim GridViewDateTimeColumn1 As Telerik.WinControls.UI.GridViewDateTimeColumn = New Telerik.WinControls.UI.GridViewDateTimeColumn()
        Dim GridViewDecimalColumn2 As Telerik.WinControls.UI.GridViewDecimalColumn = New Telerik.WinControls.UI.GridViewDecimalColumn()
        Dim GridViewDecimalColumn3 As Telerik.WinControls.UI.GridViewDecimalColumn = New Telerik.WinControls.UI.GridViewDecimalColumn()
        Dim GridViewDecimalColumn4 As Telerik.WinControls.UI.GridViewDecimalColumn = New Telerik.WinControls.UI.GridViewDecimalColumn()
        Dim GridViewDecimalColumn5 As Telerik.WinControls.UI.GridViewDecimalColumn = New Telerik.WinControls.UI.GridViewDecimalColumn()
        Dim GridViewTextBoxColumn1 As Telerik.WinControls.UI.GridViewTextBoxColumn = New Telerik.WinControls.UI.GridViewTextBoxColumn()
        Dim GridViewTextBoxColumn2 As Telerik.WinControls.UI.GridViewTextBoxColumn = New Telerik.WinControls.UI.GridViewTextBoxColumn()
        Dim ConditionalFormattingObject1 As Telerik.WinControls.UI.ConditionalFormattingObject = New Telerik.WinControls.UI.ConditionalFormattingObject()
        Dim GridViewDecimalColumn6 As Telerik.WinControls.UI.GridViewDecimalColumn = New Telerik.WinControls.UI.GridViewDecimalColumn()
        Dim GridViewTextBoxColumn3 As Telerik.WinControls.UI.GridViewTextBoxColumn = New Telerik.WinControls.UI.GridViewTextBoxColumn()
        Dim GridViewDecimalColumn7 As Telerik.WinControls.UI.GridViewDecimalColumn = New Telerik.WinControls.UI.GridViewDecimalColumn()
        Dim GridViewDecimalColumn8 As Telerik.WinControls.UI.GridViewDecimalColumn = New Telerik.WinControls.UI.GridViewDecimalColumn()
        Dim GridViewDecimalColumn9 As Telerik.WinControls.UI.GridViewDecimalColumn = New Telerik.WinControls.UI.GridViewDecimalColumn()
        Dim ConditionalFormattingObject2 As Telerik.WinControls.UI.ConditionalFormattingObject = New Telerik.WinControls.UI.ConditionalFormattingObject()
        Dim ConditionalFormattingObject3 As Telerik.WinControls.UI.ConditionalFormattingObject = New Telerik.WinControls.UI.ConditionalFormattingObject()
        Me.C1Ribbon1 = New C1.Win.C1Ribbon.C1Ribbon()
        Me.RibbonApplicationMenu1 = New C1.Win.C1Ribbon.RibbonApplicationMenu()
        Me.RibbonBottomToolBar1 = New C1.Win.C1Ribbon.RibbonBottomToolBar()
        Me.RibbonConfigToolBar1 = New C1.Win.C1Ribbon.RibbonConfigToolBar()
        Me.RibbonQat1 = New C1.Win.C1Ribbon.RibbonQat()
        Me.RibbonTab1 = New C1.Win.C1Ribbon.RibbonTab()
        Me.RibbonGroup6 = New C1.Win.C1Ribbon.RibbonGroup()
        Me.ribRetornar = New C1.Win.C1Ribbon.RibbonButton()
        Me.btnImprimir = New C1.Win.C1Ribbon.RibbonButton()
        Me.RibbonTopToolBar1 = New C1.Win.C1Ribbon.RibbonTopToolBar()
        Me.SoloDataSet = New Solo.SoloDataSet()
        Me.ExtratoBindingSource = New System.Windows.Forms.BindingSource(Me.components)
        Me.ExtratoTableAdapter = New Solo.SoloDataSetTableAdapters.ExtratoTableAdapter()
        Me.MasterTemplate = New Telerik.WinControls.UI.RadGridView()
        Me.Office2010BlackTheme1 = New Telerik.WinControls.Themes.Office2010BlackTheme()
        Me.ftPasta = New System.Windows.Forms.TextBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.ftHist = New System.Windows.Forms.TextBox()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.ftND = New System.Windows.Forms.TextBox()
        Me.lbSaldo = New System.Windows.Forms.Label()
        Me.ftPsND = New System.Windows.Forms.CheckBox()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.lbCliente = New System.Windows.Forms.Label()
        Me.btnTransfere = New System.Windows.Forms.Button()
        Me.TableAdapterManager1 = New Solo.SoloDataSetTableAdapters.TableAdapterManager()
        Me.cboCliente = New System.Windows.Forms.ComboBox()
        Me.ClientesBindingSource = New System.Windows.Forms.BindingSource(Me.components)
        Me.ClientesTableAdapter = New Solo.SoloDataSetTableAdapters.ClientesTableAdapter()
        Me.lblTransfere = New System.Windows.Forms.Label()
        Me.Button1 = New System.Windows.Forms.Button()
        Me.btnDown = New System.Windows.Forms.Button()
        Me.btnUp = New System.Windows.Forms.Button()
        Me.Timer1 = New System.Windows.Forms.Timer(Me.components)
        Me.CircularProgress1 = New DevComponents.DotNetBar.Controls.CircularProgress()
        Me.BalloonTip1 = New DevComponents.DotNetBar.BalloonTip()
        'CType(Me.C1Ribbon1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.SoloDataSet, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.ExtratoBindingSource, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.MasterTemplate, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.MasterTemplate.MasterTemplate, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.ClientesBindingSource, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
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
        Me.RibbonTab1.Image = CType(resources.GetObject("RibbonTab1.Image"), System.Drawing.Image)
        Me.RibbonTab1.Name = "RibbonTab1"
        Me.RibbonTab1.Text = "Contas Correntes"
        '
        'RibbonGroup6
        '
        Me.RibbonGroup6.Items.Add(Me.ribRetornar)
        Me.RibbonGroup6.Items.Add(Me.btnImprimir)
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
        'btnImprimir
        '
        Me.btnImprimir.LargeImage = CType(resources.GetObject("btnImprimir.LargeImage"), System.Drawing.Image)
        Me.btnImprimir.Name = "btnImprimir"
        Me.btnImprimir.SmallImage = CType(resources.GetObject("btnImprimir.SmallImage"), System.Drawing.Image)
        Me.btnImprimir.Text = "Imprimir"
        Me.btnImprimir.TextImageRelation = C1.Win.C1Ribbon.TextImageRelation.ImageAboveText
        '
        'RibbonTopToolBar1
        '
        Me.RibbonTopToolBar1.Name = "RibbonTopToolBar1"
        Me.RibbonTopToolBar1.Visible = False
        '
        'SoloDataSet
        '
        Me.SoloDataSet.DataSetName = "SoloDataSet"
        Me.SoloDataSet.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema
        '
        'ExtratoBindingSource
        '
        Me.ExtratoBindingSource.DataMember = "Extrato"
        Me.ExtratoBindingSource.DataSource = Me.SoloDataSet
        '
        'ExtratoTableAdapter
        '
        Me.ExtratoTableAdapter.ClearBeforeFill = True
        '
        'MasterTemplate
        '
        Me.MasterTemplate.BackColor = System.Drawing.Color.White
        Me.MasterTemplate.Cursor = System.Windows.Forms.Cursors.Default
        Me.MasterTemplate.Font = New System.Drawing.Font("Segoe UI", 9.75!)
        Me.MasterTemplate.ForeColor = System.Drawing.SystemColors.ControlText
        Me.MasterTemplate.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.MasterTemplate.Location = New System.Drawing.Point(8, 161)
        '
        'MasterTemplate
        '
        Me.MasterTemplate.MasterTemplate.AllowAddNewRow = False
        Me.MasterTemplate.MasterTemplate.AllowColumnReorder = False
        Me.MasterTemplate.MasterTemplate.AllowColumnResize = False
        Me.MasterTemplate.MasterTemplate.AllowDeleteRow = False
        Me.MasterTemplate.MasterTemplate.AllowRowResize = False
        GridViewDecimalColumn1.DataType = GetType(Integer)
        GridViewDecimalColumn1.EnableExpressionEditor = False
        GridViewDecimalColumn1.FieldName = "Selec"
        GridViewDecimalColumn1.HeaderText = "Selec"
        GridViewDecimalColumn1.IsAutoGenerated = True
        GridViewDecimalColumn1.IsVisible = False
        GridViewDecimalColumn1.Name = "Selec"
        GridViewDateTimeColumn1.AllowFiltering = False
        GridViewDateTimeColumn1.EnableExpressionEditor = False
        GridViewDateTimeColumn1.FieldName = "Dt"
        GridViewDateTimeColumn1.Format = System.Windows.Forms.DateTimePickerFormat.[Long]
        GridViewDateTimeColumn1.FormatString = "{0:dd/MM/yy}"
        GridViewDateTimeColumn1.HeaderText = "Data"
        GridViewDateTimeColumn1.IsAutoGenerated = True
        GridViewDateTimeColumn1.Name = "Dt"
        GridViewDateTimeColumn1.ReadOnly = True
        GridViewDateTimeColumn1.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter
        GridViewDateTimeColumn1.Width = 78
        GridViewDecimalColumn2.DataType = GetType(Integer)
        GridViewDecimalColumn2.EnableExpressionEditor = False
        GridViewDecimalColumn2.FieldName = "Id"
        GridViewDecimalColumn2.HeaderText = "Id"
        GridViewDecimalColumn2.IsAutoGenerated = True
        GridViewDecimalColumn2.IsVisible = False
        GridViewDecimalColumn2.Name = "Id"
        GridViewDecimalColumn2.ReadOnly = True
        GridViewDecimalColumn3.DataType = GetType(Integer)
        GridViewDecimalColumn3.EnableExpressionEditor = False
        GridViewDecimalColumn3.FieldName = "IdCliente"
        GridViewDecimalColumn3.HeaderText = "IdCliente"
        GridViewDecimalColumn3.IsAutoGenerated = True
        GridViewDecimalColumn3.IsVisible = False
        GridViewDecimalColumn3.Name = "IdCliente"
        GridViewDecimalColumn4.DataType = GetType(Integer)
        GridViewDecimalColumn4.EnableExpressionEditor = False
        GridViewDecimalColumn4.FieldName = "CodCliente"
        GridViewDecimalColumn4.HeaderText = "CodCliente"
        GridViewDecimalColumn4.IsAutoGenerated = True
        GridViewDecimalColumn4.IsVisible = False
        GridViewDecimalColumn4.Name = "CodCliente"
        GridViewDecimalColumn5.DataType = GetType(Integer)
        GridViewDecimalColumn5.EnableExpressionEditor = False
        GridViewDecimalColumn5.FieldName = "Conta"
        GridViewDecimalColumn5.HeaderText = "Pasta"
        GridViewDecimalColumn5.IsAutoGenerated = True
        GridViewDecimalColumn5.Name = "Conta"
        GridViewDecimalColumn5.ReadOnly = True
        GridViewDecimalColumn5.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter
        GridViewDecimalColumn5.Width = 71
        GridViewTextBoxColumn1.EnableExpressionEditor = False
        GridViewTextBoxColumn1.FieldName = "ND"
        GridViewTextBoxColumn1.HeaderText = "N. D."
        GridViewTextBoxColumn1.IsAutoGenerated = True
        GridViewTextBoxColumn1.Name = "ND"
        GridViewTextBoxColumn1.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter
        GridViewTextBoxColumn1.Width = 76
        ConditionalFormattingObject1.CellBackColor = System.Drawing.Color.Empty
        ConditionalFormattingObject1.CellForeColor = System.Drawing.Color.Red
        ConditionalFormattingObject1.Name = "NewCondition"
        ConditionalFormattingObject1.RowBackColor = System.Drawing.Color.Empty
        ConditionalFormattingObject1.RowForeColor = System.Drawing.Color.Empty
        ConditionalFormattingObject1.TValue1 = "AGENCIA"
        ConditionalFormattingObject1.TValue2 = "AGENCIA"
        GridViewTextBoxColumn2.ConditionalFormattingObjectList.Add(ConditionalFormattingObject1)
        GridViewTextBoxColumn2.EnableExpressionEditor = False
        GridViewTextBoxColumn2.FieldName = "Ref"
        GridViewTextBoxColumn2.HeaderText = "Histórico"
        GridViewTextBoxColumn2.IsAutoGenerated = True
        GridViewTextBoxColumn2.Name = "Ref"
        GridViewTextBoxColumn2.Width = 486
        GridViewDecimalColumn6.EnableExpressionEditor = False
        GridViewDecimalColumn6.FieldName = "VValor"
        GridViewDecimalColumn6.HeaderText = "VValor"
        GridViewDecimalColumn6.IsAutoGenerated = True
        GridViewDecimalColumn6.IsVisible = False
        GridViewDecimalColumn6.Name = "VValor"
        GridViewTextBoxColumn3.EnableExpressionEditor = False
        GridViewTextBoxColumn3.FieldName = "DC"
        GridViewTextBoxColumn3.HeaderText = "DC"
        GridViewTextBoxColumn3.IsAutoGenerated = True
        GridViewTextBoxColumn3.IsVisible = False
        GridViewTextBoxColumn3.Name = "DC"
        GridViewDecimalColumn7.AllowFiltering = False
        GridViewDecimalColumn7.EnableExpressionEditor = False
        GridViewDecimalColumn7.FieldName = "Débito"
        GridViewDecimalColumn7.FormatString = "{0:#,###0.00;-#,###0.00;nothing}"
        GridViewDecimalColumn7.HeaderText = "Débito"
        GridViewDecimalColumn7.IsAutoGenerated = True
        GridViewDecimalColumn7.Name = "Débito"
        GridViewDecimalColumn7.ReadOnly = True
        GridViewDecimalColumn7.Width = 85
        GridViewDecimalColumn8.AllowFiltering = False
        GridViewDecimalColumn8.EnableExpressionEditor = False
        GridViewDecimalColumn8.FieldName = "Crédito"
        GridViewDecimalColumn8.FormatString = "{0:#,###0.00;-#,###0.00;nothing}"
        GridViewDecimalColumn8.HeaderText = "Crédito"
        GridViewDecimalColumn8.IsAutoGenerated = True
        GridViewDecimalColumn8.Name = "Crédito"
        GridViewDecimalColumn8.ReadOnly = True
        GridViewDecimalColumn8.Width = 85
        GridViewDecimalColumn9.AllowFiltering = False
        ConditionalFormattingObject2.CellBackColor = System.Drawing.Color.Empty
        ConditionalFormattingObject2.CellForeColor = System.Drawing.Color.Red
        ConditionalFormattingObject2.ConditionType = Telerik.WinControls.UI.ConditionTypes.GreaterOrEqual
        ConditionalFormattingObject2.Name = "NewCondition1"
        ConditionalFormattingObject2.RowBackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer), CType(CType(192, Byte), Integer))
        ConditionalFormattingObject2.RowForeColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(128, Byte), Integer), CType(CType(128, Byte), Integer))
        ConditionalFormattingObject2.TValue1 = "0"
        ConditionalFormattingObject2.TValue2 = "0"
        ConditionalFormattingObject3.CellBackColor = System.Drawing.Color.Empty
        ConditionalFormattingObject3.CellForeColor = System.Drawing.Color.Blue
        ConditionalFormattingObject3.ConditionType = Telerik.WinControls.UI.ConditionTypes.Less
        ConditionalFormattingObject3.Name = "NewCondition2"
        ConditionalFormattingObject3.RowBackColor = System.Drawing.Color.FromArgb(CType(CType(192, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        ConditionalFormattingObject3.RowForeColor = System.Drawing.Color.FromArgb(CType(CType(128, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(128, Byte), Integer))
        ConditionalFormattingObject3.TValue1 = "0"
        ConditionalFormattingObject3.TValue2 = "0"
        GridViewDecimalColumn9.ConditionalFormattingObjectList.Add(ConditionalFormattingObject2)
        GridViewDecimalColumn9.ConditionalFormattingObjectList.Add(ConditionalFormattingObject3)
        GridViewDecimalColumn9.EnableExpressionEditor = False
        GridViewDecimalColumn9.FieldName = "Saldo"
        GridViewDecimalColumn9.FormatString = "{0:#,###0.00;-#,###0.00;""0,00""}"
        GridViewDecimalColumn9.HeaderText = "Saldo"
        GridViewDecimalColumn9.IsAutoGenerated = True
        GridViewDecimalColumn9.Name = "Saldo"
        GridViewDecimalColumn9.ReadOnly = True
        GridViewDecimalColumn9.Width = 85
        Me.MasterTemplate.MasterTemplate.Columns.AddRange(New Telerik.WinControls.UI.GridViewDataColumn() {GridViewDecimalColumn1, GridViewDateTimeColumn1, GridViewDecimalColumn2, GridViewDecimalColumn3, GridViewDecimalColumn4, GridViewDecimalColumn5, GridViewTextBoxColumn1, GridViewTextBoxColumn2, GridViewDecimalColumn6, GridViewTextBoxColumn3, GridViewDecimalColumn7, GridViewDecimalColumn8, GridViewDecimalColumn9})
        Me.MasterTemplate.MasterTemplate.DataSource = Me.ExtratoBindingSource
        Me.MasterTemplate.MasterTemplate.EnableAlternatingRowColor = True
        Me.MasterTemplate.MasterTemplate.EnableFiltering = True
        Me.MasterTemplate.MasterTemplate.EnableGrouping = False
        Me.MasterTemplate.MasterTemplate.EnableSorting = False
        Me.MasterTemplate.MasterTemplate.MultiSelect = True
        Me.MasterTemplate.MasterTemplate.ShowFilterCellOperatorText = False
        Me.MasterTemplate.Name = "MasterTemplate"
        Me.MasterTemplate.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.MasterTemplate.ShowGroupPanel = False
        Me.MasterTemplate.Size = New System.Drawing.Size(997, 470)
        Me.MasterTemplate.TabIndex = 7
        Me.MasterTemplate.Text = "RadGridView1"
        Me.MasterTemplate.ThemeName = "Office2010Black"
        '
        'ftPasta
        '
        Me.ftPasta.Font = New System.Drawing.Font("Segoe UI", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.ftPasta.ForeColor = System.Drawing.Color.DarkRed
        Me.ftPasta.Location = New System.Drawing.Point(147, 498)
        Me.ftPasta.Name = "ftPasta"
        Me.ftPasta.Size = New System.Drawing.Size(134, 23)
        Me.ftPasta.TabIndex = 23
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Font = New System.Drawing.Font("Segoe UI", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label1.ForeColor = System.Drawing.Color.DarkRed
        Me.Label1.Location = New System.Drawing.Point(108, 502)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(35, 15)
        Me.Label1.TabIndex = 24
        Me.Label1.Text = "Pasta"
        Me.Label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Font = New System.Drawing.Font("Segoe UI", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label2.ForeColor = System.Drawing.Color.DarkRed
        Me.Label2.Location = New System.Drawing.Point(88, 529)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(55, 15)
        Me.Label2.TabIndex = 26
        Me.Label2.Text = "Histórico"
        Me.Label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'ftHist
        '
        Me.ftHist.Font = New System.Drawing.Font("Segoe UI", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.ftHist.ForeColor = System.Drawing.Color.DarkRed
        Me.ftHist.Location = New System.Drawing.Point(147, 525)
        Me.ftHist.Name = "ftHist"
        Me.ftHist.Size = New System.Drawing.Size(134, 23)
        Me.ftHist.TabIndex = 25
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Font = New System.Drawing.Font("Segoe UI", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label3.ForeColor = System.Drawing.Color.DarkRed
        Me.Label3.Location = New System.Drawing.Point(110, 556)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(33, 15)
        Me.Label3.TabIndex = 28
        Me.Label3.Text = "N. D."
        Me.Label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'ftND
        '
        Me.ftND.Font = New System.Drawing.Font("Segoe UI", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.ftND.ForeColor = System.Drawing.Color.DarkRed
        Me.ftND.Location = New System.Drawing.Point(147, 552)
        Me.ftND.Name = "ftND"
        Me.ftND.Size = New System.Drawing.Size(134, 23)
        Me.ftND.TabIndex = 27
        '
        'lbSaldo
        '
        Me.lbSaldo.Font = New System.Drawing.Font("Segoe UI Semibold", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lbSaldo.Location = New System.Drawing.Point(827, 635)
        Me.lbSaldo.Name = "lbSaldo"
        Me.lbSaldo.Size = New System.Drawing.Size(162, 19)
        Me.lbSaldo.TabIndex = 29
        Me.lbSaldo.Text = "Saldo: "
        Me.lbSaldo.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'ftPsND
        '
        Me.ftPsND.CheckAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.ftPsND.Font = New System.Drawing.Font("Segoe UI", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.ftPsND.ForeColor = System.Drawing.Color.DarkRed
        Me.ftPsND.Location = New System.Drawing.Point(3, 635)
        Me.ftPsND.Name = "ftPsND"
        Me.ftPsND.Size = New System.Drawing.Size(164, 17)
        Me.ftPsND.TabIndex = 31
        Me.ftPsND.Text = "Somente pastas sem N. D."
        Me.ftPsND.TextAlign = System.Drawing.ContentAlignment.TopCenter
        Me.ftPsND.UseVisualStyleBackColor = True
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Font = New System.Drawing.Font("Segoe UI", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label4.ForeColor = System.Drawing.Color.DarkRed
        Me.Label4.Location = New System.Drawing.Point(12, 512)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(42, 15)
        Me.Label4.TabIndex = 33
        Me.Label4.Text = "Filtros:"
        Me.Label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'lbCliente
        '
        Me.lbCliente.Font = New System.Drawing.Font("Segoe UI Semibold", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lbCliente.ForeColor = System.Drawing.Color.DarkRed
        Me.lbCliente.Location = New System.Drawing.Point(7, 132)
        Me.lbCliente.Name = "lbCliente"
        Me.lbCliente.Size = New System.Drawing.Size(999, 25)
        Me.lbCliente.TabIndex = 21
        Me.lbCliente.Text = "Label1"
        Me.lbCliente.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'btnTransfere
        '
        Me.btnTransfere.Font = New System.Drawing.Font("Segoe UI", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnTransfere.Image = CType(resources.GetObject("btnTransfere.Image"), System.Drawing.Image)
        Me.btnTransfere.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnTransfere.Location = New System.Drawing.Point(667, 665)
        Me.btnTransfere.Name = "btnTransfere"
        Me.btnTransfere.Size = New System.Drawing.Size(92, 24)
        Me.btnTransfere.TabIndex = 35
        Me.btnTransfere.Text = "Transferir"
        Me.btnTransfere.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.btnTransfere.UseVisualStyleBackColor = True
        Me.btnTransfere.Visible = False
        '
        'TableAdapterManager1
        '
        Me.TableAdapterManager1.BackupDataSetBeforeUpdate = False
        Me.TableAdapterManager1.ChavesTableAdapter = Nothing
        Me.TableAdapterManager1.ClientesTableAdapter = Nothing
        Me.TableAdapterManager1.ContasTableAdapter = Nothing
        Me.TableAdapterManager1.ExtratoTableAdapter = Me.ExtratoTableAdapter
        Me.TableAdapterManager1.PerfisTableAdapter = Nothing
        Me.TableAdapterManager1.UpdateOrder = Solo.SoloDataSetTableAdapters.TableAdapterManager.UpdateOrderOption.InsertUpdateDelete
        Me.TableAdapterManager1.UsuáriosTableAdapter = Nothing
        '
        'cboCliente
        '
        Me.cboCliente.DataSource = Me.ClientesBindingSource
        Me.cboCliente.DisplayMember = "Cliente"
        Me.cboCliente.Font = New System.Drawing.Font("Segoe UI", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cboCliente.FormattingEnabled = True
        Me.cboCliente.Location = New System.Drawing.Point(345, 666)
        Me.cboCliente.MaxDropDownItems = 16
        Me.cboCliente.Name = "cboCliente"
        Me.cboCliente.Size = New System.Drawing.Size(312, 23)
        Me.cboCliente.TabIndex = 37
        Me.cboCliente.ValueMember = "Id"
        Me.cboCliente.Visible = False
        '
        'ClientesBindingSource
        '
        Me.ClientesBindingSource.DataMember = "Clientes"
        Me.ClientesBindingSource.DataSource = Me.SoloDataSet
        Me.ClientesBindingSource.Sort = "Cliente"
        '
        'ClientesTableAdapter
        '
        Me.ClientesTableAdapter.ClearBeforeFill = True
        '
        'lblTransfere
        '
        Me.lblTransfere.Font = New System.Drawing.Font("Segoe UI", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblTransfere.ForeColor = System.Drawing.Color.DarkRed
        Me.lblTransfere.Location = New System.Drawing.Point(341, 648)
        Me.lblTransfere.Name = "lblTransfere"
        Me.lblTransfere.Size = New System.Drawing.Size(338, 15)
        Me.lblTransfere.TabIndex = 39
        Me.lblTransfere.Text = "Transferir os lançamentos selecionados para o cliente abaixo:"
        Me.lblTransfere.Visible = False
        '
        'Button1
        '
        Me.Button1.Location = New System.Drawing.Point(226, 665)
        Me.Button1.Name = "Button1"
        Me.Button1.Size = New System.Drawing.Size(75, 23)
        Me.Button1.TabIndex = 41
        Me.Button1.Text = "Button1"
        Me.Button1.UseVisualStyleBackColor = True
        Me.Button1.Visible = False
        '
        'btnDown
        '
        Me.BalloonTip1.SetBalloonCaption(Me.btnDown, "Último registro")
        Me.BalloonTip1.SetBalloonText(Me.btnDown, "Quando não funcionar, clique em qualquer lançamento e tente novamente")
        Me.btnDown.Image = CType(resources.GetObject("btnDown.Image"), System.Drawing.Image)
        Me.btnDown.Location = New System.Drawing.Point(41, 132)
        Me.btnDown.Name = "btnDown"
        Me.btnDown.Size = New System.Drawing.Size(28, 26)
        Me.btnDown.TabIndex = 43
        Me.btnDown.TabStop = False
        Me.btnDown.UseVisualStyleBackColor = True
        '
        'btnUp
        '
        Me.BalloonTip1.SetBalloonCaption(Me.btnUp, "Primeiro registro")
        Me.BalloonTip1.SetBalloonText(Me.btnUp, "Quando não funcionar, clique em qualquer lançamento e tente novamente")
        Me.btnUp.Image = CType(resources.GetObject("btnUp.Image"), System.Drawing.Image)
        Me.btnUp.Location = New System.Drawing.Point(8, 132)
        Me.btnUp.Name = "btnUp"
        Me.btnUp.Size = New System.Drawing.Size(28, 26)
        Me.btnUp.TabIndex = 44
        Me.btnUp.TabStop = False
        Me.btnUp.UseVisualStyleBackColor = True
        '
        'Timer1
        '
        Me.Timer1.Interval = 500
        '
        'CircularProgress1
        '
        Me.CircularProgress1.BackColor = System.Drawing.Color.Transparent
        '
        '
        '
        Me.CircularProgress1.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square
        Me.CircularProgress1.BackgroundStyle.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.CircularProgress1.BackgroundStyle.TextAlignment = DevComponents.DotNetBar.eStyleTextAlignment.Center
        Me.CircularProgress1.BackgroundStyle.TextColor = System.Drawing.Color.Red
        Me.CircularProgress1.Location = New System.Drawing.Point(75, 131)
        Me.CircularProgress1.Name = "CircularProgress1"
        Me.CircularProgress1.ProgressColor = System.Drawing.Color.Teal
        Me.CircularProgress1.ProgressTextColor = System.Drawing.Color.Maroon
        Me.CircularProgress1.ProgressTextFormat = ""
        Me.CircularProgress1.ProgressTextVisible = True
        Me.CircularProgress1.Size = New System.Drawing.Size(44, 26)
        Me.CircularProgress1.Style = DevComponents.DotNetBar.eDotNetBarStyle.OfficeXP
        Me.CircularProgress1.TabIndex = 46
        Me.CircularProgress1.Visible = False
        '
        'BalloonTip1
        '
        Me.BalloonTip1.AlertAnimation = DevComponents.DotNetBar.eAlertAnimation.TopToBottom
        Me.BalloonTip1.AutoCloseTimeOut = 10
        Me.BalloonTip1.CaptionImage = CType(resources.GetObject("BalloonTip1.CaptionImage"), System.Drawing.Image)
        Me.BalloonTip1.InitialDelay = 1000
        Me.BalloonTip1.ShowCloseButton = False
        Me.BalloonTip1.Style = DevComponents.DotNetBar.eBallonStyle.Office2007Alert
        '
        'FrmExtrato
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.White
        Me.ClientSize = New System.Drawing.Size(1012, 691)
        Me.Controls.Add(Me.CircularProgress1)
        Me.Controls.Add(Me.btnUp)
        Me.Controls.Add(Me.btnDown)
        Me.Controls.Add(Me.Button1)
        Me.Controls.Add(Me.lblTransfere)
        Me.Controls.Add(Me.cboCliente)
        Me.Controls.Add(Me.btnTransfere)
        Me.Controls.Add(Me.ftPsND)
        Me.Controls.Add(Me.lbSaldo)
        Me.Controls.Add(Me.lbCliente)
        Me.Controls.Add(Me.MasterTemplate)
        'Me.Controls.Add(Me.C1Ribbon1)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.ftND)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.ftHist)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.ftPasta)
        Me.Controls.Add(Me.Label4)
        Me.KeyPreview = True
        Me.Name = "FrmExtrato"
        '
        '
        '
        Me.RootElement.ApplyShapeToControl = True
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "SOLO CONSULTORIA DE IMÓVEIS"
        Me.ThemeName = "Office2010Black"
        'CType(Me.C1Ribbon1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.SoloDataSet, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.ExtratoBindingSource, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.MasterTemplate.MasterTemplate, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.MasterTemplate, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.ClientesBindingSource, System.ComponentModel.ISupportInitialize).EndInit()
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
    Friend WithEvents btnImprimir As C1.Win.C1Ribbon.RibbonButton
    Friend WithEvents SoloDataSet As Solo.SoloDataSet
    Friend WithEvents ExtratoBindingSource As System.Windows.Forms.BindingSource
    Friend WithEvents ExtratoTableAdapter As Solo.SoloDataSetTableAdapters.ExtratoTableAdapter
    Friend WithEvents RadGridView1 As Telerik.WinControls.UI.RadGridView
    Friend WithEvents MasterTemplate As Telerik.WinControls.UI.RadGridView
    Friend WithEvents Office2010BlackTheme1 As Telerik.WinControls.Themes.Office2010BlackTheme
    Friend WithEvents ftPasta As System.Windows.Forms.TextBox
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents ftHist As System.Windows.Forms.TextBox
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents ftND As System.Windows.Forms.TextBox
    Friend WithEvents lbSaldo As System.Windows.Forms.Label
    Friend WithEvents ftPsND As System.Windows.Forms.CheckBox
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents lbCliente As System.Windows.Forms.Label
    Friend WithEvents btnTransfere As System.Windows.Forms.Button
    Friend WithEvents TableAdapterManager1 As Solo.SoloDataSetTableAdapters.TableAdapterManager
    Friend WithEvents cboCliente As System.Windows.Forms.ComboBox
    Friend WithEvents ClientesBindingSource As System.Windows.Forms.BindingSource
    Friend WithEvents ClientesTableAdapter As Solo.SoloDataSetTableAdapters.ClientesTableAdapter
    Friend WithEvents lblTransfere As System.Windows.Forms.Label
    Friend WithEvents Button1 As System.Windows.Forms.Button
    Friend WithEvents btnDown As System.Windows.Forms.Button
    Friend WithEvents btnUp As System.Windows.Forms.Button
    Friend WithEvents Timer1 As System.Windows.Forms.Timer
    Friend WithEvents CircularProgress1 As DevComponents.DotNetBar.Controls.CircularProgress
    Friend WithEvents BalloonTip1 As DevComponents.DotNetBar.BalloonTip
    Friend WithEvents RibbonBottomToolBar1 As C1.Win.C1Ribbon.RibbonBottomToolBar
    Friend WithEvents RibbonTopToolBar1 As C1.Win.C1Ribbon.RibbonTopToolBar
End Class

