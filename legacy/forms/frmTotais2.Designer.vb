<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmTotais2
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
        Me.Office2010BlackTheme1 = New Telerik.WinControls.Themes.Office2010BlackTheme()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.D2 = New System.Windows.Forms.Label()
        Me.D1 = New System.Windows.Forms.Label()
        Me.C2 = New System.Windows.Forms.Label()
        Me.C1 = New System.Windows.Forms.Label()
        Me.Dt = New System.Windows.Forms.DateTimePicker()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Label3 = New System.Windows.Forms.Label()
        CType(Me, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label1.Location = New System.Drawing.Point(83, 221)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(211, 17)
        Me.Label1.TabIndex = 12
        Me.Label1.Text = "Somente clientes a partir de 10000"
        '
        'D2
        '
        Me.D2.Font = New System.Drawing.Font("Segoe UI Semibold", 11.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.D2.ForeColor = System.Drawing.Color.Red
        Me.D2.Location = New System.Drawing.Point(206, 137)
        Me.D2.Name = "D2"
        Me.D2.Size = New System.Drawing.Size(123, 20)
        Me.D2.TabIndex = 11
        Me.D2.Text = "Label4"
        '
        'D1
        '
        Me.D1.Font = New System.Drawing.Font("Segoe UI Semibold", 11.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.D1.ForeColor = System.Drawing.Color.Red
        Me.D1.Location = New System.Drawing.Point(134, 137)
        Me.D1.Name = "D1"
        Me.D1.Size = New System.Drawing.Size(66, 20)
        Me.D1.TabIndex = 10
        Me.D1.Text = "Label3"
        '
        'C2
        '
        Me.C2.Font = New System.Drawing.Font("Segoe UI Semibold", 11.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.C2.ForeColor = System.Drawing.SystemColors.HotTrack
        Me.C2.Location = New System.Drawing.Point(205, 100)
        Me.C2.Name = "C2"
        Me.C2.Size = New System.Drawing.Size(123, 20)
        Me.C2.TabIndex = 9
        Me.C2.Text = "Label2"
        '
        'C1
        '
        Me.C1.Font = New System.Drawing.Font("Segoe UI Semibold", 11.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.C1.ForeColor = System.Drawing.SystemColors.HotTrack
        Me.C1.Location = New System.Drawing.Point(133, 100)
        Me.C1.Name = "C1"
        Me.C1.Size = New System.Drawing.Size(66, 20)
        Me.C1.TabIndex = 8
        Me.C1.Text = "Label1"
        '
        'Dt
        '
        Me.Dt.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Dt.Format = System.Windows.Forms.DateTimePickerFormat.[Short]
        Me.Dt.Location = New System.Drawing.Point(129, 34)
        Me.Dt.Name = "Dt"
        Me.Dt.Size = New System.Drawing.Size(119, 26)
        Me.Dt.TabIndex = 7
        Me.Dt.TabStop = False
        '
        'Label2
        '
        Me.Label2.Font = New System.Drawing.Font("Segoe UI Semibold", 11.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label2.ForeColor = System.Drawing.SystemColors.HotTrack
        Me.Label2.Location = New System.Drawing.Point(51, 100)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(77, 20)
        Me.Label2.TabIndex = 13
        Me.Label2.Text = "Credores:"
        Me.Label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label3
        '
        Me.Label3.Font = New System.Drawing.Font("Segoe UI Semibold", 11.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label3.ForeColor = System.Drawing.Color.Red
        Me.Label3.Location = New System.Drawing.Point(44, 137)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(84, 20)
        Me.Label3.TabIndex = 14
        Me.Label3.Text = "Devedores:"
        Me.Label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'frmTotais2
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.LemonChiffon
        Me.ClientSize = New System.Drawing.Size(376, 255)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.D2)
        Me.Controls.Add(Me.D1)
        Me.Controls.Add(Me.C2)
        Me.Controls.Add(Me.C1)
        Me.Controls.Add(Me.Dt)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "frmTotais2"
        '
        '
        '
        Me.RootElement.ApplyShapeToControl = True
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "TOTAIS DE CLIENTES"
        Me.ThemeName = "Office2010Black"
        Me.TopMost = True
        CType(Me, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents Office2010BlackTheme1 As Telerik.WinControls.Themes.Office2010BlackTheme
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents D2 As System.Windows.Forms.Label
    Friend WithEvents D1 As System.Windows.Forms.Label
    Friend WithEvents C2 As System.Windows.Forms.Label
    Friend WithEvents C1 As System.Windows.Forms.Label
    Friend WithEvents Dt As System.Windows.Forms.DateTimePicker
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents Label3 As System.Windows.Forms.Label
End Class

