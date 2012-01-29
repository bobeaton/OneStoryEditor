Friend Class Status
	Inherits System.Windows.Forms.Form

#Region " Windows Form Designer generated code "

	Public Sub New()
		MyBase.New()

		'This call is required by the Windows Form Designer.
		InitializeComponent()

		'Add any initialization after the InitializeComponent() call

	End Sub

	'Form overrides dispose to clean up the component list.
	Protected Overloads Overrides Sub Dispose(ByVal disposing As Boolean)
		If disposing Then
			If Not (components Is Nothing) Then
				components.Dispose()
			End If
		End If
		MyBase.Dispose(disposing)
	End Sub

	'Required by the Windows Form Designer
	Private components As System.ComponentModel.IContainer

	'NOTE: The following procedure is required by the Windows Form Designer
	'It can be modified using the Windows Form Designer.  
	'Do not modify it using the code editor.
	Private WithEvents lblTimeRemaining As System.Windows.Forms.Label
	Private WithEvents pbrProgress As System.Windows.Forms.ProgressBar
	Private WithEvents lblStatus As System.Windows.Forms.Label
	Private WithEvents lblTitle As System.Windows.Forms.Label
	Private WithEvents cmdCancel As System.Windows.Forms.Button
	Friend WithEvents picIcon As System.Windows.Forms.PictureBox
	<System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Status))
        Me.picIcon = New System.Windows.Forms.PictureBox
        Me.lblTimeRemaining = New System.Windows.Forms.Label
        Me.lblTitle = New System.Windows.Forms.Label
        Me.pbrProgress = New System.Windows.Forms.ProgressBar
        Me.lblStatus = New System.Windows.Forms.Label
        Me.cmdCancel = New System.Windows.Forms.Button
        CType(Me.picIcon, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'picIcon
        '
        Me.picIcon.Image = CType(resources.GetObject("picIcon.Image"), System.Drawing.Image)
        Me.picIcon.Location = New System.Drawing.Point(28, 16)
        Me.picIcon.Name = "picIcon"
        Me.picIcon.Size = New System.Drawing.Size(32, 32)
        Me.picIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize
        Me.picIcon.TabIndex = 5
        Me.picIcon.TabStop = False
        '
        'lblTimeRemaining
        '
        Me.lblTimeRemaining.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblTimeRemaining.Location = New System.Drawing.Point(84, 92)
        Me.lblTimeRemaining.Name = "lblTimeRemaining"
        Me.lblTimeRemaining.Size = New System.Drawing.Size(306, 16)
        Me.lblTimeRemaining.TabIndex = 1
        '
        'lblTitle
        '
        Me.lblTitle.Font = New System.Drawing.Font("Verdana", 9.75!)
        Me.lblTitle.Location = New System.Drawing.Point(76, 16)
        Me.lblTitle.Name = "lblTitle"
        Me.lblTitle.Size = New System.Drawing.Size(503, 20)
        Me.lblTitle.TabIndex = 2
        '
        'pbrProgress
        '
        Me.pbrProgress.Location = New System.Drawing.Point(84, 68)
        Me.pbrProgress.Name = "pbrProgress"
        Me.pbrProgress.Size = New System.Drawing.Size(417, 25)
        Me.pbrProgress.TabIndex = 3
        '
        'lblStatus
        '
        Me.lblStatus.Font = New System.Drawing.Font("Verdana", 9.75!)
        Me.lblStatus.Location = New System.Drawing.Point(76, 48)
        Me.lblStatus.Name = "lblStatus"
        Me.lblStatus.Size = New System.Drawing.Size(400, 16)
        Me.lblStatus.TabIndex = 4
        '
        'cmdCancel
        '
        Me.cmdCancel.BackColor = System.Drawing.SystemColors.Control
        Me.cmdCancel.Location = New System.Drawing.Point(507, 68)
        Me.cmdCancel.Name = "cmdCancel"
        Me.cmdCancel.Size = New System.Drawing.Size(72, 24)
        Me.cmdCancel.TabIndex = 0
        Me.cmdCancel.Text = "Cancel"
        Me.cmdCancel.UseVisualStyleBackColor = False
        '
        'Status
        '
        Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
        Me.ClientSize = New System.Drawing.Size(615, 123)
        Me.ControlBox = False
        Me.Controls.Add(Me.lblTitle)
        Me.Controls.Add(Me.picIcon)
        Me.Controls.Add(Me.lblTimeRemaining)
        Me.Controls.Add(Me.cmdCancel)
        Me.Controls.Add(Me.pbrProgress)
        Me.Controls.Add(Me.lblStatus)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "Status"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Automatic Upgrade"
        CType(Me.picIcon, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

#End Region

	Private mblnCancel As Boolean

	Public Sub OnProgress(ByVal strProgressString As String, ByVal strCurrentFile As String, ByVal intProgressPercent As Integer, ByVal lngTimeRemaining As Long, ByRef blnCancel As Boolean)

		If lblStatus.Text <> strProgressString Then
			lblStatus.Text = strProgressString
		End If

		If lngTimeRemaining = 0 Then
			lblTimeRemaining.Text = ""
		Else
			lblTimeRemaining.Text = "Estimated time remaining: " & lngTimeRemaining & " seconds."
		End If

		If pbrProgress.Value <> intProgressPercent Then
			If intProgressPercent >= 0 And intProgressPercent <= 100 Then
				pbrProgress.Value = intProgressPercent
			End If
		End If

		' Give status form a chance to process window messages (i.e cancel button)
		System.Windows.Forms.Application.DoEvents()
		blnCancel = mblnCancel
	End Sub

	Public Property ApplicationName() As String
		Get
			Return Me.Text
		End Get
		Set(ByVal Value As String)
			Me.Text = Value
            lblTitle.Text = Value & " is downloading OneStory Editor files."
		End Set
	End Property

	Public ReadOnly Property Cancel() As Boolean
		Get
			Return mblnCancel
		End Get
	End Property

	Private Sub cmdCancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdCancel.Click
		If MsgBox("Are you sure you want to cancel the automatic upgrade?", MsgBoxStyle.Question Or MsgBoxStyle.YesNo, "Cancel") = MsgBoxResult.Yes Then
			mblnCancel = True
		End If
	End Sub

	Private Sub Status_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load
		Me.Show()
	End Sub
End Class

