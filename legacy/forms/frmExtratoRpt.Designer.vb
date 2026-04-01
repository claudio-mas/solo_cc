<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class FrmExtratoRpt
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(FrmExtratoRpt))
        Me.Office2010BlackTheme1 = New Telerik.WinControls.Themes.Office2010BlackTheme()
        Me.C1Report1 = New C1.C1Report.C1Report()
        Me.C1PrintPreviewControl1 = New C1.Win.C1Preview.C1PrintPreviewControl()
        Me.C1Ribbon1 = New C1.Win.C1Ribbon.C1Ribbon()
        Me.RibbonApplicationMenu1 = New C1.Win.C1Ribbon.RibbonApplicationMenu()
        Me.RibbonConfigToolBar1 = New C1.Win.C1Ribbon.RibbonConfigToolBar()
        Me.RibbonQat1 = New C1.Win.C1Ribbon.RibbonQat()
        Me.RibbonTab1 = New C1.Win.C1Ribbon.RibbonTab()
        Me.RibbonGroup6 = New C1.Win.C1Ribbon.RibbonGroup()
        Me.ribRetornar = New C1.Win.C1Ribbon.RibbonButton()
        Me.RibbonTopToolBar1 = New C1.Win.C1Ribbon.RibbonTopToolBar()
        Me.RibbonBottomToolBar1 = New C1.Win.C1Ribbon.RibbonBottomToolBar()
        CType(Me.C1Report1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.C1PrintPreviewControl1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.C1PrintPreviewControl1.PreviewPane, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.C1PrintPreviewControl1.SuspendLayout()
        'CType(Me.C1Ribbon1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'C1Report1
        '
        Me.C1Report1.ReportDefinition = resources.GetString("C1Report1.ReportDefinition")
        Me.C1Report1.ReportName = "Conta Corrente"
        '
        'C1PrintPreviewControl1
        '
        Me.C1PrintPreviewControl1.AvailablePreviewActions = CType((((((((((((((((((((((C1.Win.C1Preview.C1PreviewActionFlags.FileSave Or C1.Win.C1Preview.C1PreviewActionFlags.PageSetup) _
            Or C1.Win.C1Preview.C1PreviewActionFlags.Print) _
            Or C1.Win.C1Preview.C1PreviewActionFlags.PageSingle) _
            Or C1.Win.C1Preview.C1PreviewActionFlags.PageContinuous) _
            Or C1.Win.C1Preview.C1PreviewActionFlags.PageFacing) _
            Or C1.Win.C1Preview.C1PreviewActionFlags.PageFacingContinuous) _
            Or C1.Win.C1Preview.C1PreviewActionFlags.GoFirst) _
            Or C1.Win.C1Preview.C1PreviewActionFlags.GoPrev) _
            Or C1.Win.C1Preview.C1PreviewActionFlags.GoNext) _
            Or C1.Win.C1Preview.C1PreviewActionFlags.GoLast) _
            Or C1.Win.C1Preview.C1PreviewActionFlags.GoPage) _
            Or C1.Win.C1Preview.C1PreviewActionFlags.HistoryNext) _
            Or C1.Win.C1Preview.C1PreviewActionFlags.HistoryPrev) _
            Or C1.Win.C1Preview.C1PreviewActionFlags.ZoomIn) _
            Or C1.Win.C1Preview.C1PreviewActionFlags.ZoomOut) _
            Or C1.Win.C1Preview.C1PreviewActionFlags.ZoomFactor) _
            Or C1.Win.C1Preview.C1PreviewActionFlags.ZoomInTool) _
            Or C1.Win.C1Preview.C1PreviewActionFlags.ZoomOutTool) _
            Or C1.Win.C1Preview.C1PreviewActionFlags.HandTool) _
            Or C1.Win.C1Preview.C1PreviewActionFlags.SelectTextTool) _
            Or C1.Win.C1Preview.C1PreviewActionFlags.Find), C1.Win.C1Preview.C1PreviewActionFlags)
        Me.C1PrintPreviewControl1.Location = New System.Drawing.Point(0, 133)
        Me.C1PrintPreviewControl1.Name = "C1PrintPreviewControl1"
        Me.C1PrintPreviewControl1.NavigationPanelVisible = False
        '
        'C1PrintPreviewControl1.PreviewPane
        '
        Me.C1PrintPreviewControl1.PreviewPane.Document = Me.C1Report1
        Me.C1PrintPreviewControl1.PreviewPane.IntegrateExternalTools = True
        Me.C1PrintPreviewControl1.PreviewPane.TabIndex = 0
        Me.C1PrintPreviewControl1.PreviewPane.ZoomMode = C1.Win.C1Preview.ZoomModeEnum.ActualSize
        Me.C1PrintPreviewControl1.Size = New System.Drawing.Size(1012, 557)
        Me.C1PrintPreviewControl1.TabIndex = 0
        Me.C1PrintPreviewControl1.Text = "C1PrintPreviewControl1"
        '
        '
        '
        Me.C1PrintPreviewControl1.ToolBars.File.Open.Image = CType(resources.GetObject("C1PrintPreviewControl1.ToolBars.File.Open.Image"), System.Drawing.Image)
        Me.C1PrintPreviewControl1.ToolBars.File.Open.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.C1PrintPreviewControl1.ToolBars.File.Open.Name = "btnFileOpen"
        Me.C1PrintPreviewControl1.ToolBars.File.Open.Size = New System.Drawing.Size(32, 22)
        Me.C1PrintPreviewControl1.ToolBars.File.Open.Tag = "C1PreviewActionEnum.FileOpen"
        Me.C1PrintPreviewControl1.ToolBars.File.Open.ToolTipText = "Open File"
        Me.C1PrintPreviewControl1.ToolBars.File.Open.Visible = False
        '
        '
        '
        Me.C1PrintPreviewControl1.ToolBars.File.Parameters.Image = CType(resources.GetObject("C1PrintPreviewControl1.ToolBars.File.Parameters.Image"), System.Drawing.Image)
        Me.C1PrintPreviewControl1.ToolBars.File.Parameters.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.C1PrintPreviewControl1.ToolBars.File.Parameters.Name = "btnParameters"
        Me.C1PrintPreviewControl1.ToolBars.File.Parameters.Size = New System.Drawing.Size(23, 22)
        Me.C1PrintPreviewControl1.ToolBars.File.Parameters.Tag = "C1PreviewActionEnum.Parameters"
        Me.C1PrintPreviewControl1.ToolBars.File.Parameters.ToolTipText = "Report Parameters"
        Me.C1PrintPreviewControl1.ToolBars.File.Parameters.Visible = False
        '
        '
        '
        Me.C1PrintPreviewControl1.ToolBars.File.Print.Image = CType(resources.GetObject("C1PrintPreviewControl1.ToolBars.File.Print.Image"), System.Drawing.Image)
        Me.C1PrintPreviewControl1.ToolBars.File.Print.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.C1PrintPreviewControl1.ToolBars.File.Print.Name = "btnPrint"
        Me.C1PrintPreviewControl1.ToolBars.File.Print.Size = New System.Drawing.Size(23, 22)
        Me.C1PrintPreviewControl1.ToolBars.File.Print.Tag = "C1PreviewActionEnum.Print"
        Me.C1PrintPreviewControl1.ToolBars.File.Print.ToolTipText = "Print"
        '
        '
        '
        Me.C1PrintPreviewControl1.ToolBars.File.PrintLayout.Image = CType(resources.GetObject("C1PrintPreviewControl1.ToolBars.File.PrintLayout.Image"), System.Drawing.Image)
        Me.C1PrintPreviewControl1.ToolBars.File.PrintLayout.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.C1PrintPreviewControl1.ToolBars.File.PrintLayout.Name = "btnPrintLayout"
        Me.C1PrintPreviewControl1.ToolBars.File.PrintLayout.Size = New System.Drawing.Size(23, 22)
        Me.C1PrintPreviewControl1.ToolBars.File.PrintLayout.Tag = "C1PreviewActionEnum.PrintLayout"
        Me.C1PrintPreviewControl1.ToolBars.File.PrintLayout.ToolTipText = "Print Layout"
        Me.C1PrintPreviewControl1.ToolBars.File.PrintLayout.Visible = False
        '
        '
        '
        Me.C1PrintPreviewControl1.ToolBars.File.Reflow.Image = CType(resources.GetObject("C1PrintPreviewControl1.ToolBars.File.Reflow.Image"), System.Drawing.Image)
        Me.C1PrintPreviewControl1.ToolBars.File.Reflow.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.C1PrintPreviewControl1.ToolBars.File.Reflow.Name = "btnReflow"
        Me.C1PrintPreviewControl1.ToolBars.File.Reflow.Size = New System.Drawing.Size(23, 22)
        Me.C1PrintPreviewControl1.ToolBars.File.Reflow.Tag = "C1PreviewActionEnum.Reflow"
        Me.C1PrintPreviewControl1.ToolBars.File.Reflow.ToolTipText = "Reflow"
        Me.C1PrintPreviewControl1.ToolBars.File.Reflow.Visible = False
        '
        '
        '
        Me.C1PrintPreviewControl1.ToolBars.File.Stop.Image = CType(resources.GetObject("C1PrintPreviewControl1.ToolBars.File.Stop.Image"), System.Drawing.Image)
        Me.C1PrintPreviewControl1.ToolBars.File.Stop.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.C1PrintPreviewControl1.ToolBars.File.Stop.Name = "btnStop"
        Me.C1PrintPreviewControl1.ToolBars.File.Stop.Size = New System.Drawing.Size(23, 22)
        Me.C1PrintPreviewControl1.ToolBars.File.Stop.Tag = "C1PreviewActionEnum.Stop"
        Me.C1PrintPreviewControl1.ToolBars.File.Stop.ToolTipText = "Stop"
        Me.C1PrintPreviewControl1.ToolBars.File.Stop.Visible = False
        '
        '
        '
        Me.C1PrintPreviewControl1.ToolBars.Navigation.GoFirst.Image = CType(resources.GetObject("C1PrintPreviewControl1.ToolBars.Navigation.GoFirst.Image"), System.Drawing.Image)
        Me.C1PrintPreviewControl1.ToolBars.Navigation.GoFirst.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.C1PrintPreviewControl1.ToolBars.Navigation.GoFirst.Name = "btnGoFirst"
        Me.C1PrintPreviewControl1.ToolBars.Navigation.GoFirst.Size = New System.Drawing.Size(23, 22)
        Me.C1PrintPreviewControl1.ToolBars.Navigation.GoFirst.Tag = "C1PreviewActionEnum.GoFirst"
        Me.C1PrintPreviewControl1.ToolBars.Navigation.GoFirst.ToolTipText = "Go To First Page"
        '
        '
        '
        Me.C1PrintPreviewControl1.ToolBars.Navigation.GoLast.Image = CType(resources.GetObject("C1PrintPreviewControl1.ToolBars.Navigation.GoLast.Image"), System.Drawing.Image)
        Me.C1PrintPreviewControl1.ToolBars.Navigation.GoLast.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.C1PrintPreviewControl1.ToolBars.Navigation.GoLast.Name = "btnGoLast"
        Me.C1PrintPreviewControl1.ToolBars.Navigation.GoLast.Size = New System.Drawing.Size(23, 22)
        Me.C1PrintPreviewControl1.ToolBars.Navigation.GoLast.Tag = "C1PreviewActionEnum.GoLast"
        Me.C1PrintPreviewControl1.ToolBars.Navigation.GoLast.ToolTipText = "Go To Last Page"
        '
        '
        '
        Me.C1PrintPreviewControl1.ToolBars.Navigation.GoNext.Image = CType(resources.GetObject("C1PrintPreviewControl1.ToolBars.Navigation.GoNext.Image"), System.Drawing.Image)
        Me.C1PrintPreviewControl1.ToolBars.Navigation.GoNext.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.C1PrintPreviewControl1.ToolBars.Navigation.GoNext.Name = "btnGoNext"
        Me.C1PrintPreviewControl1.ToolBars.Navigation.GoNext.Size = New System.Drawing.Size(23, 22)
        Me.C1PrintPreviewControl1.ToolBars.Navigation.GoNext.Tag = "C1PreviewActionEnum.GoNext"
        Me.C1PrintPreviewControl1.ToolBars.Navigation.GoNext.ToolTipText = "Go To Next Page"
        '
        '
        '
        Me.C1PrintPreviewControl1.ToolBars.Navigation.GoPrev.Image = CType(resources.GetObject("C1PrintPreviewControl1.ToolBars.Navigation.GoPrev.Image"), System.Drawing.Image)
        Me.C1PrintPreviewControl1.ToolBars.Navigation.GoPrev.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.C1PrintPreviewControl1.ToolBars.Navigation.GoPrev.Name = "btnGoPrev"
        Me.C1PrintPreviewControl1.ToolBars.Navigation.GoPrev.Size = New System.Drawing.Size(23, 22)
        Me.C1PrintPreviewControl1.ToolBars.Navigation.GoPrev.Tag = "C1PreviewActionEnum.GoPrev"
        Me.C1PrintPreviewControl1.ToolBars.Navigation.GoPrev.ToolTipText = "Go To Previous Page"
        '
        '
        '
        Me.C1PrintPreviewControl1.ToolBars.Navigation.HistoryNext.Image = CType(resources.GetObject("C1PrintPreviewControl1.ToolBars.Navigation.HistoryNext.Image"), System.Drawing.Image)
        Me.C1PrintPreviewControl1.ToolBars.Navigation.HistoryNext.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.C1PrintPreviewControl1.ToolBars.Navigation.HistoryNext.Name = "btnHistoryNext"
        Me.C1PrintPreviewControl1.ToolBars.Navigation.HistoryNext.Size = New System.Drawing.Size(32, 22)
        Me.C1PrintPreviewControl1.ToolBars.Navigation.HistoryNext.Tag = "C1PreviewActionEnum.HistoryNext"
        Me.C1PrintPreviewControl1.ToolBars.Navigation.HistoryNext.ToolTipText = "Next View"
        '
        '
        '
        Me.C1PrintPreviewControl1.ToolBars.Navigation.HistoryPrev.Image = CType(resources.GetObject("C1PrintPreviewControl1.ToolBars.Navigation.HistoryPrev.Image"), System.Drawing.Image)
        Me.C1PrintPreviewControl1.ToolBars.Navigation.HistoryPrev.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.C1PrintPreviewControl1.ToolBars.Navigation.HistoryPrev.Name = "btnHistoryPrev"
        Me.C1PrintPreviewControl1.ToolBars.Navigation.HistoryPrev.Size = New System.Drawing.Size(32, 22)
        Me.C1PrintPreviewControl1.ToolBars.Navigation.HistoryPrev.Tag = "C1PreviewActionEnum.HistoryPrev"
        Me.C1PrintPreviewControl1.ToolBars.Navigation.HistoryPrev.ToolTipText = "Previous View"
        '
        '
        '
        Me.C1PrintPreviewControl1.ToolBars.Navigation.NavigationPane.Image = CType(resources.GetObject("C1PrintPreviewControl1.ToolBars.Navigation.NavigationPane.Image"), System.Drawing.Image)
        Me.C1PrintPreviewControl1.ToolBars.Navigation.NavigationPane.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.C1PrintPreviewControl1.ToolBars.Navigation.NavigationPane.Name = "btnNavigationPane"
        Me.C1PrintPreviewControl1.ToolBars.Navigation.NavigationPane.Size = New System.Drawing.Size(23, 22)
        Me.C1PrintPreviewControl1.ToolBars.Navigation.NavigationPane.Tag = "C1PreviewActionEnum.NavigationPane"
        Me.C1PrintPreviewControl1.ToolBars.Navigation.NavigationPane.ToolTipText = "Navigation Pane"
        Me.C1PrintPreviewControl1.ToolBars.Navigation.NavigationPane.Visible = False
        Me.C1PrintPreviewControl1.ToolBars.Navigation.ToolTipPageNo = Nothing
        '
        '
        '
        Me.C1PrintPreviewControl1.ToolBars.Search.CloseSearch.Image = CType(resources.GetObject("C1PrintPreviewControl1.ToolBars.Search.CloseSearch.Image"), System.Drawing.Image)
        Me.C1PrintPreviewControl1.ToolBars.Search.CloseSearch.Name = "btnCloseSearch"
        Me.C1PrintPreviewControl1.ToolBars.Search.CloseSearch.Size = New System.Drawing.Size(23, 22)
        Me.C1PrintPreviewControl1.ToolBars.Search.CloseSearch.Tag = "C1PreviewActionEnum.CloseSearch"
        Me.C1PrintPreviewControl1.ToolBars.Search.CloseSearch.ToolTipText = "Close"
        Me.C1PrintPreviewControl1.ToolBars.Search.CloseSearch.Visible = False
        '
        '
        '
        Me.C1PrintPreviewControl1.ToolBars.Search.MatchCase.Name = "btnMatchCase"
        Me.C1PrintPreviewControl1.ToolBars.Search.MatchCase.Size = New System.Drawing.Size(73, 22)
        Me.C1PrintPreviewControl1.ToolBars.Search.MatchCase.Tag = "C1PreviewActionEnum.MatchCase"
        Me.C1PrintPreviewControl1.ToolBars.Search.MatchCase.Text = "Match Case"
        Me.C1PrintPreviewControl1.ToolBars.Search.MatchCase.ToolTipText = "Search with case sensitivity"
        Me.C1PrintPreviewControl1.ToolBars.Search.MatchCase.Visible = False
        '
        '
        '
        Me.C1PrintPreviewControl1.ToolBars.Search.MatchWholeWord.Name = "btnMatchWholeWord"
        Me.C1PrintPreviewControl1.ToolBars.Search.MatchWholeWord.Size = New System.Drawing.Size(77, 22)
        Me.C1PrintPreviewControl1.ToolBars.Search.MatchWholeWord.Tag = "C1PreviewActionEnum.MatchWholeWord"
        Me.C1PrintPreviewControl1.ToolBars.Search.MatchWholeWord.Text = "Whole Word"
        Me.C1PrintPreviewControl1.ToolBars.Search.MatchWholeWord.ToolTipText = "Match whole word only"
        Me.C1PrintPreviewControl1.ToolBars.Search.MatchWholeWord.Visible = False
        '
        '
        '
        Me.C1PrintPreviewControl1.ToolBars.Search.SearchLabel.Name = "lblSearch"
        Me.C1PrintPreviewControl1.ToolBars.Search.SearchLabel.Size = New System.Drawing.Size(33, 22)
        Me.C1PrintPreviewControl1.ToolBars.Search.SearchLabel.Tag = "C1PreviewActionEnum.SearchLabel"
        Me.C1PrintPreviewControl1.ToolBars.Search.SearchLabel.Text = "Find:"
        Me.C1PrintPreviewControl1.ToolBars.Search.SearchLabel.Visible = False
        '
        '
        '
        Me.C1PrintPreviewControl1.ToolBars.Search.SearchNext.Image = CType(resources.GetObject("C1PrintPreviewControl1.ToolBars.Search.SearchNext.Image"), System.Drawing.Image)
        Me.C1PrintPreviewControl1.ToolBars.Search.SearchNext.Name = "btnSearchNext"
        Me.C1PrintPreviewControl1.ToolBars.Search.SearchNext.Size = New System.Drawing.Size(23, 22)
        Me.C1PrintPreviewControl1.ToolBars.Search.SearchNext.Tag = "C1PreviewActionEnum.SearchNext"
        Me.C1PrintPreviewControl1.ToolBars.Search.SearchNext.ToolTipText = "Search Next"
        Me.C1PrintPreviewControl1.ToolBars.Search.SearchNext.Visible = False
        '
        '
        '
        Me.C1PrintPreviewControl1.ToolBars.Search.SearchPrevious.Image = CType(resources.GetObject("C1PrintPreviewControl1.ToolBars.Search.SearchPrevious.Image"), System.Drawing.Image)
        Me.C1PrintPreviewControl1.ToolBars.Search.SearchPrevious.Name = "btnSearchPrevious"
        Me.C1PrintPreviewControl1.ToolBars.Search.SearchPrevious.Size = New System.Drawing.Size(23, 22)
        Me.C1PrintPreviewControl1.ToolBars.Search.SearchPrevious.Tag = "C1PreviewActionEnum.SearchPrevious"
        Me.C1PrintPreviewControl1.ToolBars.Search.SearchPrevious.ToolTipText = "Search Previous"
        Me.C1PrintPreviewControl1.ToolBars.Search.SearchPrevious.Visible = False
        '
        '
        '
        Me.C1PrintPreviewControl1.ToolBars.Search.SearchText.Font = New System.Drawing.Font("Segoe UI", 9.0!)
        Me.C1PrintPreviewControl1.ToolBars.Search.SearchText.Name = "txtSearchText"
        Me.C1PrintPreviewControl1.ToolBars.Search.SearchText.Size = New System.Drawing.Size(200, 25)
        Me.C1PrintPreviewControl1.ToolBars.Search.SearchText.Tag = "C1PreviewActionEnum.SearchText"
        Me.C1PrintPreviewControl1.ToolBars.Search.SearchText.Visible = False
        '
        '
        '
        Me.C1PrintPreviewControl1.ToolBars.Text.Find.Image = CType(resources.GetObject("C1PrintPreviewControl1.ToolBars.Text.Find.Image"), System.Drawing.Image)
        Me.C1PrintPreviewControl1.ToolBars.Text.Find.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.C1PrintPreviewControl1.ToolBars.Text.Find.Name = "btnFind"
        Me.C1PrintPreviewControl1.ToolBars.Text.Find.Size = New System.Drawing.Size(23, 22)
        Me.C1PrintPreviewControl1.ToolBars.Text.Find.Tag = "C1PreviewActionEnum.Find"
        Me.C1PrintPreviewControl1.ToolBars.Text.Find.ToolTipText = "Find Text"
        '
        '
        '
        Me.C1PrintPreviewControl1.ToolBars.Text.Hand.Image = CType(resources.GetObject("C1PrintPreviewControl1.ToolBars.Text.Hand.Image"), System.Drawing.Image)
        Me.C1PrintPreviewControl1.ToolBars.Text.Hand.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.C1PrintPreviewControl1.ToolBars.Text.Hand.Name = "btnHandTool"
        Me.C1PrintPreviewControl1.ToolBars.Text.Hand.Size = New System.Drawing.Size(23, 22)
        Me.C1PrintPreviewControl1.ToolBars.Text.Hand.Tag = "C1PreviewActionEnum.HandTool"
        Me.C1PrintPreviewControl1.ToolBars.Text.Hand.ToolTipText = "Hand Tool"
        Me.C1PrintPreviewControl1.ToolBars.Zoom.ToolTipToolZoomIn = Nothing
        Me.C1PrintPreviewControl1.ToolBars.Zoom.ToolTipToolZoomOut = Nothing
        Me.C1PrintPreviewControl1.ToolBars.Zoom.ToolTipZoomFactor = Nothing
        '
        '
        '
        Me.C1PrintPreviewControl1.ToolBars.Zoom.ZoomInTool.Image = CType(resources.GetObject("C1PrintPreviewControl1.ToolBars.Zoom.ZoomInTool.Image"), System.Drawing.Image)
        Me.C1PrintPreviewControl1.ToolBars.Zoom.ZoomInTool.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.C1PrintPreviewControl1.ToolBars.Zoom.ZoomInTool.Name = "itemZoomInTool"
        Me.C1PrintPreviewControl1.ToolBars.Zoom.ZoomInTool.Size = New System.Drawing.Size(154, 22)
        Me.C1PrintPreviewControl1.ToolBars.Zoom.ZoomInTool.Tag = "C1PreviewActionEnum.ZoomInTool"
        Me.C1PrintPreviewControl1.ToolBars.Zoom.ZoomInTool.Text = "Zoom In Tool"
        '
        '
        '
        Me.C1PrintPreviewControl1.ToolBars.Zoom.ZoomOutTool.Image = CType(resources.GetObject("C1PrintPreviewControl1.ToolBars.Zoom.ZoomOutTool.Image"), System.Drawing.Image)
        Me.C1PrintPreviewControl1.ToolBars.Zoom.ZoomOutTool.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.C1PrintPreviewControl1.ToolBars.Zoom.ZoomOutTool.Name = "itemZoomOutTool"
        Me.C1PrintPreviewControl1.ToolBars.Zoom.ZoomOutTool.Size = New System.Drawing.Size(154, 22)
        Me.C1PrintPreviewControl1.ToolBars.Zoom.ZoomOutTool.Tag = "C1PreviewActionEnum.ZoomOutTool"
        Me.C1PrintPreviewControl1.ToolBars.Zoom.ZoomOutTool.Text = "Zoom Out Tool"
        '
        'C1Ribbon1
        '
        'Me.C1Ribbon1.ApplicationMenuHolder = Nothing
        'Me.C1Ribbon1.BottomToolBarHolder = Nothing
        'Me.C1Ribbon1.ConfigToolBarHolder = Nothing
        'Me.C1Ribbon1.Location = New System.Drawing.Point(0, 0)
        'Me.C1Ribbon1.Name = "C1Ribbon1"
        'Me.C1Ribbon1.QatHolder = Nothing
        'Me.C1Ribbon1.Size = New System.Drawing.Size(0, 0)
        'Me.C1Ribbon1.TopToolBarHolder = Nothing
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
        'RibbonTopToolBar1
        '
        Me.RibbonTopToolBar1.Name = "RibbonTopToolBar1"
        Me.RibbonTopToolBar1.Visible = False
        '
        'RibbonBottomToolBar1
        '
        Me.RibbonBottomToolBar1.Name = "RibbonBottomToolBar1"
        '
        'FrmExtratoRpt
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1012, 691)
        Me.Controls.Add(Me.C1PrintPreviewControl1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D
        Me.Name = "FrmExtratoRpt"
        '
        '
        '
        Me.RootElement.ApplyShapeToControl = True
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "SOLO CONSULTORIA DE IMÓVEIS"
        Me.ThemeName = "Office2010Black"
        CType(Me.C1Report1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.C1PrintPreviewControl1.PreviewPane, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.C1PrintPreviewControl1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.C1PrintPreviewControl1.ResumeLayout(False)
        Me.C1PrintPreviewControl1.PerformLayout()
        'CType(Me.C1Ribbon1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents Office2010BlackTheme1 As Telerik.WinControls.Themes.Office2010BlackTheme
    Friend WithEvents C1Report1 As C1.C1Report.C1Report
    Friend WithEvents C1PrintPreviewControl1 As C1.Win.C1Preview.C1PrintPreviewControl
    Friend WithEvents C1Ribbon1 As C1.Win.C1Ribbon.C1Ribbon
    Friend WithEvents RibbonApplicationMenu1 As C1.Win.C1Ribbon.RibbonApplicationMenu
    Friend WithEvents RibbonConfigToolBar1 As C1.Win.C1Ribbon.RibbonConfigToolBar
    Friend WithEvents RibbonQat1 As C1.Win.C1Ribbon.RibbonQat
    Friend WithEvents RibbonTab1 As C1.Win.C1Ribbon.RibbonTab
    Friend WithEvents RibbonGroup6 As C1.Win.C1Ribbon.RibbonGroup
    Friend WithEvents ribRetornar As C1.Win.C1Ribbon.RibbonButton
    Friend WithEvents RibbonBottomToolBar1 As C1.Win.C1Ribbon.RibbonBottomToolBar
    Friend WithEvents RibbonTopToolBar1 As C1.Win.C1Ribbon.RibbonTopToolBar
End Class

