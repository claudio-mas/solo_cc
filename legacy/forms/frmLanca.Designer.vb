<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class FrmLanca
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
        Dim DtLabel As System.Windows.Forms.Label
        Dim ContaLabel As System.Windows.Forms.Label
        Dim RefLabel As System.Windows.Forms.Label
        Dim VValorLabel As System.Windows.Forms.Label
        Dim DCLabel As System.Windows.Forms.Label
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(FrmLanca))
        Me.Office2010BlackTheme1 = New Telerik.WinControls.Themes.Office2010BlackTheme()
        Me.C1Ribbon1 = New C1.Win.C1Ribbon.C1Ribbon()
        Me.RibbonApplicationMenu1 = New C1.Win.C1Ribbon.RibbonApplicationMenu()
        Me.RibbonConfigToolBar1 = New C1.Win.C1Ribbon.RibbonConfigToolBar()
        Me.RibbonQat1 = New C1.Win.C1Ribbon.RibbonQat()
        Me.RibbonTab1 = New C1.Win.C1Ribbon.RibbonTab()
        Me.RibbonGroup6 = New C1.Win.C1Ribbon.RibbonGroup()
        Me.ribRetornar = New C1.Win.C1Ribbon.RibbonButton()
        Me.btnNovo = New C1.Win.C1Ribbon.RibbonButton()
        Me.btnRibSalvar = New C1.Win.C1Ribbon.RibbonButton()
        Me.btnRibUndo = New C1.Win.C1Ribbon.RibbonButton()
        Me.RibbonTopToolBar1 = New C1.Win.C1Ribbon.RibbonTopToolBar()
        Me.RibbonBottomToolBar1 = New C1.Win.C1Ribbon.RibbonBottomToolBar()
        Me.SoloDataSet = New Solo.SoloDataSet()
        Me.ContasBindingSource = New System.Windows.Forms.BindingSource(Me.components)
        Me.ContasTableAdapter = New Solo.SoloDataSetTableAdapters.ContasTableAdapter()
        Me.TableAdapterManager = New Solo.SoloDataSetTableAdapters.TableAdapterManager()
        Me.dtpDt = New System.Windows.Forms.DateTimePicker()
        Me.txtPasta = New System.Windows.Forms.TextBox()
        Me.txtDC = New System.Windows.Forms.TextBox()
        Me.txtIdCliente = New System.Windows.Forms.TextBox()
        Me.txtCodCliente = New System.Windows.Forms.TextBox()
        Me.txtDeb = New System.Windows.Forms.TextBox()
        Me.txtCred = New System.Windows.Forms.TextBox()
        Me.lbCliente = New System.Windows.Forms.Label()
        Me.C1PictureBox1 = New C1.Win.C1Input.C1PictureBox()
        Me.txtDT = New System.Windows.Forms.TextBox()
        Me.txtRef = New C1.Win.C1Input.C1TextBox()
        Me.txtVValor = New C1.Win.C1Input.C1TextBox()
        Me.ShapeContainer1 = New Microsoft.VisualBasic.PowerPacks.ShapeContainer()
        Me.RectangleShape1 = New Microsoft.VisualBasic.PowerPacks.RectangleShape()
        Me.Highlighter1 = New DevComponents.DotNetBar.Validator.Highlighter()
        DtLabel = New System.Windows.Forms.Label()
        ContaLabel = New System.Windows.Forms.Label()
        RefLabel = New System.Windows.Forms.Label()
        VValorLabel = New System.Windows.Forms.Label()
        DCLabel = New System.Windows.Forms.Label()
        CType(Me.C1Ribbon1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.SoloDataSet, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.ContasBindingSource, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.C1PictureBox1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.txtRef, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.txtVValor, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'DtLabel
        '
        DtLabel.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DtLabel.Location = New System.Drawing.Point(288, 327)
        DtLabel.Name = "DtLabel"
        DtLabel.Size = New System.Drawing.Size(76, 19)
        DtLabel.TabIndex = 2
        DtLabel.Text = "Data"
        DtLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'ContaLabel
        '
        ContaLabel.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        ContaLabel.Location = New System.Drawing.Point(589, 324)
        ContaLabel.Name = "ContaLabel"
        ContaLabel.Size = New System.Drawing.Size(76, 19)
        ContaLabel.TabIndex = 4
        ContaLabel.Text = "Pasta"
        ContaLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'RefLabel
        '
        RefLabel.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        RefLabel.Location = New System.Drawing.Point(279, 374)
        RefLabel.Name = "RefLabel"
        RefLabel.Size = New System.Drawing.Size(85, 19)
        RefLabel.TabIndex = 6
        RefLabel.Text = "Lançamento"
        RefLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'VValorLabel
        '
        VValorLabel.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        VValorLabel.Location = New System.Drawing.Point(288, 479)
        VValorLabel.Name = "VValorLabel"
        VValorLabel.Size = New System.Drawing.Size(76, 19)
        VValorLabel.TabIndex = 8
        VValorLabel.Text = "Valor"
        VValorLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'DCLabel
        '
        DCLabel.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DCLabel.Location = New System.Drawing.Point(589, 479)
        DCLabel.Name = "DCLabel"
        DCLabel.Size = New System.Drawing.Size(76, 19)
        DCLabel.TabIndex = 10
        DCLabel.Text = "D ou C"
        DCLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight
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
        Me.RibbonGroup6.Items.Add(Me.btnNovo)
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
        'btnNovo
        '
        Me.btnNovo.LargeImage = CType(resources.GetObject("btnNovo.LargeImage"), System.Drawing.Image)
        Me.btnNovo.Name = "btnNovo"
        Me.btnNovo.SmallImage = CType(resources.GetObject("btnNovo.SmallImage"), System.Drawing.Image)
        Me.btnNovo.Text = "Novo"
        Me.btnNovo.TextImageRelation = C1.Win.C1Ribbon.TextImageRelation.ImageAboveText
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
        'SoloDataSet
        '
        Me.SoloDataSet.DataSetName = "SoloDataSet"
        Me.SoloDataSet.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema
        '
        'ContasBindingSource
        '
        Me.ContasBindingSource.DataMember = "Contas"
        Me.ContasBindingSource.DataSource = Me.SoloDataSet
        '
        'ContasTableAdapter
        '
        Me.ContasTableAdapter.ClearBeforeFill = True
        '
        'TableAdapterManager
        '
        Me.TableAdapterManager.BackupDataSetBeforeUpdate = False
        Me.TableAdapterManager.ChavesTableAdapter = Nothing
        Me.TableAdapterManager.ClientesTableAdapter = Nothing
        Me.TableAdapterManager.ContasTableAdapter = Me.ContasTableAdapter
        Me.TableAdapterManager.ExtratoTableAdapter = Nothing
        Me.TableAdapterManager.PerfisTableAdapter = Nothing
        Me.TableAdapterManager.UpdateOrder = Solo.SoloDataSetTableAdapters.TableAdapterManager.UpdateOrderOption.InsertUpdateDelete
        Me.TableAdapterManager.UsuáriosTableAdapter = Nothing
        '
        'dtpDt
        '
        Me.dtpDt.DataBindings.Add(New System.Windows.Forms.Binding("Value", Me.ContasBindingSource, "Dt", True))
        Me.dtpDt.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.dtpDt.Format = System.Windows.Forms.DateTimePickerFormat.[Short]
        Me.Highlighter1.SetHighlightOnFocus(Me.dtpDt, True)
        Me.dtpDt.Location = New System.Drawing.Point(369, 324)
        Me.dtpDt.Name = "dtpDt"
        Me.dtpDt.Size = New System.Drawing.Size(100, 25)
        Me.dtpDt.TabIndex = 1
        Me.dtpDt.TabStop = False
        '
        'txtPasta
        '
        Me.txtPasta.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper
        Me.txtPasta.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.ContasBindingSource, "Conta", True))
        Me.txtPasta.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Highlighter1.SetHighlightOnFocus(Me.txtPasta, True)
        Me.txtPasta.Location = New System.Drawing.Point(670, 321)
        Me.txtPasta.MaxLength = 5
        Me.txtPasta.Name = "txtPasta"
        Me.txtPasta.Size = New System.Drawing.Size(62, 25)
        Me.txtPasta.TabIndex = 4
        Me.txtPasta.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'txtDC
        '
        Me.txtDC.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper
        Me.txtDC.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.ContasBindingSource, "DC", True))
        Me.txtDC.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Highlighter1.SetHighlightOnFocus(Me.txtDC, True)
        Me.txtDC.Location = New System.Drawing.Point(670, 475)
        Me.txtDC.MaxLength = 1
        Me.txtDC.Name = "txtDC"
        Me.txtDC.Size = New System.Drawing.Size(62, 25)
        Me.txtDC.TabIndex = 11
        Me.txtDC.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'txtIdCliente
        '
        Me.txtIdCliente.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.txtIdCliente.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.ContasBindingSource, "IdCliente", True))
        Me.txtIdCliente.ForeColor = System.Drawing.Color.White
        Me.txtIdCliente.Location = New System.Drawing.Point(959, 197)
        Me.txtIdCliente.Name = "txtIdCliente"
        Me.txtIdCliente.Size = New System.Drawing.Size(41, 13)
        Me.txtIdCliente.TabIndex = 12
        Me.txtIdCliente.TabStop = False
        '
        'txtCodCliente
        '
        Me.txtCodCliente.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.txtCodCliente.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.ContasBindingSource, "CodCliente", True))
        Me.txtCodCliente.ForeColor = System.Drawing.Color.White
        Me.txtCodCliente.Location = New System.Drawing.Point(959, 238)
        Me.txtCodCliente.Name = "txtCodCliente"
        Me.txtCodCliente.Size = New System.Drawing.Size(41, 13)
        Me.txtCodCliente.TabIndex = 14
        Me.txtCodCliente.TabStop = False
        '
        'txtDeb
        '
        Me.txtDeb.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.txtDeb.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.ContasBindingSource, "Deb", True))
        Me.txtDeb.ForeColor = System.Drawing.Color.White
        Me.txtDeb.Location = New System.Drawing.Point(950, 284)
        Me.txtDeb.Name = "txtDeb"
        Me.txtDeb.Size = New System.Drawing.Size(41, 13)
        Me.txtDeb.TabIndex = 16
        Me.txtDeb.TabStop = False
        '
        'txtCred
        '
        Me.txtCred.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.txtCred.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.ContasBindingSource, "Cred", True))
        Me.txtCred.ForeColor = System.Drawing.Color.White
        Me.txtCred.Location = New System.Drawing.Point(950, 326)
        Me.txtCred.Name = "txtCred"
        Me.txtCred.Size = New System.Drawing.Size(41, 13)
        Me.txtCred.TabIndex = 18
        Me.txtCred.TabStop = False
        '
        'lbCliente
        '
        Me.lbCliente.Font = New System.Drawing.Font("Segoe UI Semibold", 14.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lbCliente.ForeColor = System.Drawing.Color.DarkRed
        Me.lbCliente.Location = New System.Drawing.Point(4, 188)
        Me.lbCliente.Name = "lbCliente"
        Me.lbCliente.Size = New System.Drawing.Size(1005, 31)
        Me.lbCliente.TabIndex = 19
        Me.lbCliente.Text = "Label1"
        Me.lbCliente.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'C1PictureBox1
        '
        Me.C1PictureBox1.Image = CType(resources.GetObject("C1PictureBox1.Image"), System.Drawing.Image)
        Me.C1PictureBox1.Location = New System.Drawing.Point(304, 140)
        Me.C1PictureBox1.Name = "C1PictureBox1"
        Me.C1PictureBox1.Size = New System.Drawing.Size(404, 35)
        Me.C1PictureBox1.TabIndex = 20
        Me.C1PictureBox1.TabStop = False
        '
        'txtDT
        '
        Me.txtDT.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.txtDT.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper
        Me.txtDT.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.ContasBindingSource, "Dt", True))
        Me.txtDT.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtDT.ForeColor = System.Drawing.Color.White
        Me.txtDT.Location = New System.Drawing.Point(959, 368)
        Me.txtDT.MaxLength = 5
        Me.txtDT.Name = "txtDT"
        Me.txtDT.Size = New System.Drawing.Size(47, 18)
        Me.txtDT.TabIndex = 22
        Me.txtDT.TabStop = False
        Me.txtDT.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'txtRef
        '
        Me.txtRef.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper
        Me.txtRef.DataBindings.Add(New System.Windows.Forms.Binding("Value", Me.ContasBindingSource, "Ref", True))
        Me.txtRef.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Highlighter1.SetHighlightOnFocus(Me.txtRef, True)
        Me.txtRef.Location = New System.Drawing.Point(370, 373)
        Me.txtRef.MaxLength = 250
        Me.txtRef.Multiline = True
        Me.txtRef.Name = "txtRef"
        Me.txtRef.Size = New System.Drawing.Size(361, 84)
        Me.txtRef.TabIndex = 6
        Me.txtRef.Tag = Nothing
        Me.txtRef.TrimStart = True
        '
        'txtVValor
        '
        Me.txtVValor.DataBindings.Add(New System.Windows.Forms.Binding("Value", Me.ContasBindingSource, "VValor", True))
        Me.txtVValor.DataType = GetType(Decimal)
        Me.txtVValor.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtVValor.FormatType = C1.Win.C1Input.FormatTypeEnum.StandardNumber
        Me.Highlighter1.SetHighlightOnFocus(Me.txtVValor, True)
        Me.txtVValor.Location = New System.Drawing.Point(370, 479)
        Me.txtVValor.Name = "txtVValor"
        Me.txtVValor.Size = New System.Drawing.Size(82, 23)
        Me.txtVValor.TabIndex = 9
        Me.txtVValor.Tag = Nothing
        Me.txtVValor.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'ShapeContainer1
        '
        Me.ShapeContainer1.Location = New System.Drawing.Point(0, 0)
        Me.ShapeContainer1.Margin = New System.Windows.Forms.Padding(0)
        Me.ShapeContainer1.Name = "ShapeContainer1"
        Me.ShapeContainer1.Shapes.AddRange(New Microsoft.VisualBasic.PowerPacks.Shape() {Me.RectangleShape1})
        Me.ShapeContainer1.Size = New System.Drawing.Size(1012, 691)
        Me.ShapeContainer1.TabIndex = 24
        Me.ShapeContainer1.TabStop = False
        '
        'RectangleShape1
        '
        Me.RectangleShape1.BorderColor = System.Drawing.Color.DarkRed
        Me.RectangleShape1.BorderWidth = 3
        Me.RectangleShape1.Location = New System.Drawing.Point(255, 290)
        Me.RectangleShape1.Name = "RectangleShape1"
        Me.RectangleShape1.Size = New System.Drawing.Size(496, 243)
        '
        'Highlighter1
        '
        Me.Highlighter1.ContainerControl = Me
        Me.Highlighter1.FocusHighlightColor = DevComponents.DotNetBar.Validator.eHighlightColor.Red
        '
        'FrmLanca
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.White
        Me.ClientSize = New System.Drawing.Size(1012, 691)
        Me.Controls.Add(Me.txtVValor)
        Me.Controls.Add(Me.txtRef)
        Me.Controls.Add(Me.txtDT)
        Me.Controls.Add(Me.C1PictureBox1)
        Me.Controls.Add(Me.lbCliente)
        Me.Controls.Add(Me.txtCred)
        Me.Controls.Add(Me.txtDeb)
        Me.Controls.Add(Me.txtCodCliente)
        Me.Controls.Add(Me.txtIdCliente)
        Me.Controls.Add(DCLabel)
        Me.Controls.Add(Me.txtDC)
        Me.Controls.Add(VValorLabel)
        Me.Controls.Add(RefLabel)
        Me.Controls.Add(ContaLabel)
        Me.Controls.Add(Me.txtPasta)
        Me.Controls.Add(DtLabel)
        Me.Controls.Add(Me.dtpDt)
        Me.Controls.Add(Me.C1Ribbon1)
        Me.Controls.Add(Me.ShapeContainer1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D
        Me.KeyPreview = True
        Me.Name = "FrmLanca"
        '
        '
        '
        Me.RootElement.ApplyShapeToControl = True
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "SOLO CONSULTORIA DE IMÓVEIS"
        Me.ThemeName = "Office2010Black"
        CType(Me.C1Ribbon1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.SoloDataSet, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.ContasBindingSource, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.C1PictureBox1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.txtRef, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.txtVValor, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents Office2010BlackTheme1 As Telerik.WinControls.Themes.Office2010BlackTheme
    Friend WithEvents C1Ribbon1 As C1.Win.C1Ribbon.C1Ribbon
    Friend WithEvents RibbonApplicationMenu1 As C1.Win.C1Ribbon.RibbonApplicationMenu
    Friend WithEvents RibbonConfigToolBar1 As C1.Win.C1Ribbon.RibbonConfigToolBar
    Friend WithEvents RibbonQat1 As C1.Win.C1Ribbon.RibbonQat
    Friend WithEvents RibbonTab1 As C1.Win.C1Ribbon.RibbonTab
    Friend WithEvents RibbonGroup6 As C1.Win.C1Ribbon.RibbonGroup
    Friend WithEvents ribRetornar As C1.Win.C1Ribbon.RibbonButton
    Friend WithEvents btnRibSalvar As C1.Win.C1Ribbon.RibbonButton
    Friend WithEvents btnRibUndo As C1.Win.C1Ribbon.RibbonButton
    Friend WithEvents SoloDataSet As Solo.SoloDataSet
    Friend WithEvents ContasBindingSource As System.Windows.Forms.BindingSource
    Friend WithEvents ContasTableAdapter As Solo.SoloDataSetTableAdapters.ContasTableAdapter
    Friend WithEvents TableAdapterManager As Solo.SoloDataSetTableAdapters.TableAdapterManager
    Friend WithEvents dtpDt As System.Windows.Forms.DateTimePicker
    Friend WithEvents txtPasta As System.Windows.Forms.TextBox
    Friend WithEvents txtDC As System.Windows.Forms.TextBox
    Friend WithEvents txtIdCliente As System.Windows.Forms.TextBox
    Friend WithEvents txtCodCliente As System.Windows.Forms.TextBox
    Friend WithEvents txtDeb As System.Windows.Forms.TextBox
    Friend WithEvents txtCred As System.Windows.Forms.TextBox
    Friend WithEvents lbCliente As System.Windows.Forms.Label
    Friend WithEvents C1PictureBox1 As C1.Win.C1Input.C1PictureBox
    Friend WithEvents btnNovo As C1.Win.C1Ribbon.RibbonButton
    Friend WithEvents txtDT As System.Windows.Forms.TextBox
    Friend WithEvents txtRef As C1.Win.C1Input.C1TextBox
    Friend WithEvents txtVValor As C1.Win.C1Input.C1TextBox
    Friend WithEvents Highlighter1 As DevComponents.DotNetBar.Validator.Highlighter
    Friend WithEvents RibbonBottomToolBar1 As C1.Win.C1Ribbon.RibbonBottomToolBar
    Friend WithEvents RibbonTopToolBar1 As C1.Win.C1Ribbon.RibbonTopToolBar
    Private WithEvents ShapeContainer1 As Microsoft.VisualBasic.PowerPacks.ShapeContainer
    Private WithEvents RectangleShape1 As Microsoft.VisualBasic.PowerPacks.RectangleShape
End Class

