<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmRankingCriteria
    Inherits System.Windows.Forms.Form

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
        Me.gbxPanel04 = New System.Windows.Forms.GroupBox()
        Me.txtField02 = New System.Windows.Forms.TextBox()
        Me.txtField01 = New System.Windows.Forms.TextBox()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.cmdButton01 = New System.Windows.Forms.Button()
        Me.cmdButton00 = New System.Windows.Forms.Button()
        Me.gbxPanel01 = New System.Windows.Forms.GroupBox()
        Me.txtSearch = New System.Windows.Forms.TextBox()
        Me.rbtTypex02 = New System.Windows.Forms.RadioButton()
        Me.rbtTypex01 = New System.Windows.Forms.RadioButton()
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.rbtOdrType03 = New System.Windows.Forms.RadioButton()
        Me.rbtOdrType02 = New System.Windows.Forms.RadioButton()
        Me.rbtOdrType01 = New System.Windows.Forms.RadioButton()
        Me.gbxPanel04.SuspendLayout()
        Me.gbxPanel01.SuspendLayout()
        Me.GroupBox1.SuspendLayout()
        Me.SuspendLayout()
        '
        'gbxPanel04
        '
        Me.gbxPanel04.Controls.Add(Me.txtField02)
        Me.gbxPanel04.Controls.Add(Me.txtField01)
        Me.gbxPanel04.Controls.Add(Me.Label2)
        Me.gbxPanel04.Controls.Add(Me.Label1)
        Me.gbxPanel04.Location = New System.Drawing.Point(22, 12)
        Me.gbxPanel04.Name = "gbxPanel04"
        Me.gbxPanel04.Size = New System.Drawing.Size(245, 72)
        Me.gbxPanel04.TabIndex = 13
        Me.gbxPanel04.TabStop = False
        Me.gbxPanel04.Text = "Range"
        '
        'txtField02
        '
        Me.txtField02.Location = New System.Drawing.Point(83, 41)
        Me.txtField02.Name = "txtField02"
        Me.txtField02.Size = New System.Drawing.Size(151, 20)
        Me.txtField02.TabIndex = 4
        '
        'txtField01
        '
        Me.txtField01.Location = New System.Drawing.Point(83, 17)
        Me.txtField01.Name = "txtField01"
        Me.txtField01.Size = New System.Drawing.Size(151, 20)
        Me.txtField01.TabIndex = 3
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(12, 48)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(29, 13)
        Me.Label2.TabIndex = 2
        Me.Label2.Text = "Thru"
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(12, 24)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(30, 13)
        Me.Label1.TabIndex = 0
        Me.Label1.Text = "From"
        '
        'cmdButton01
        '
        Me.cmdButton01.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.cmdButton01.Location = New System.Drawing.Point(281, 16)
        Me.cmdButton01.Name = "cmdButton01"
        Me.cmdButton01.Size = New System.Drawing.Size(77, 33)
        Me.cmdButton01.TabIndex = 14
        Me.cmdButton01.Text = "&Ok"
        Me.cmdButton01.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.cmdButton01.UseVisualStyleBackColor = True
        '
        'cmdButton00
        '
        Me.cmdButton00.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.cmdButton00.Location = New System.Drawing.Point(281, 50)
        Me.cmdButton00.Name = "cmdButton00"
        Me.cmdButton00.Size = New System.Drawing.Size(77, 33)
        Me.cmdButton00.TabIndex = 15
        Me.cmdButton00.Text = "&Cancel"
        Me.cmdButton00.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.cmdButton00.UseVisualStyleBackColor = True
        '
        'gbxPanel01
        '
        Me.gbxPanel01.Controls.Add(Me.txtSearch)
        Me.gbxPanel01.Controls.Add(Me.rbtTypex02)
        Me.gbxPanel01.Controls.Add(Me.rbtTypex01)
        Me.gbxPanel01.Location = New System.Drawing.Point(22, 90)
        Me.gbxPanel01.Name = "gbxPanel01"
        Me.gbxPanel01.Size = New System.Drawing.Size(245, 72)
        Me.gbxPanel01.TabIndex = 12
        Me.gbxPanel01.TabStop = False
        Me.gbxPanel01.Text = "Report Type"
        '
        'txtSearch
        '
        Me.txtSearch.Location = New System.Drawing.Point(16, 43)
        Me.txtSearch.Name = "txtSearch"
        Me.txtSearch.Size = New System.Drawing.Size(218, 20)
        Me.txtSearch.TabIndex = 2
        '
        'rbtTypex02
        '
        Me.rbtTypex02.AutoSize = True
        Me.rbtTypex02.Location = New System.Drawing.Point(83, 19)
        Me.rbtTypex02.Name = "rbtTypex02"
        Me.rbtTypex02.Size = New System.Drawing.Size(67, 17)
        Me.rbtTypex02.TabIndex = 1
        Me.rbtTypex02.Text = "Category"
        Me.rbtTypex02.UseVisualStyleBackColor = True
        '
        'rbtTypex01
        '
        Me.rbtTypex01.AutoSize = True
        Me.rbtTypex01.Checked = True
        Me.rbtTypex01.Location = New System.Drawing.Point(16, 19)
        Me.rbtTypex01.Name = "rbtTypex01"
        Me.rbtTypex01.Size = New System.Drawing.Size(45, 17)
        Me.rbtTypex01.TabIndex = 0
        Me.rbtTypex01.TabStop = True
        Me.rbtTypex01.Text = "Item"
        Me.rbtTypex01.UseVisualStyleBackColor = True
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.rbtOdrType03)
        Me.GroupBox1.Controls.Add(Me.rbtOdrType02)
        Me.GroupBox1.Controls.Add(Me.rbtOdrType01)
        Me.GroupBox1.Location = New System.Drawing.Point(22, 168)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(245, 53)
        Me.GroupBox1.TabIndex = 14
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "Order Type"
        '
        'rbtOdrType03
        '
        Me.rbtOdrType03.AutoSize = True
        Me.rbtOdrType03.Location = New System.Drawing.Point(156, 22)
        Me.rbtOdrType03.Name = "rbtOdrType03"
        Me.rbtOdrType03.Size = New System.Drawing.Size(70, 17)
        Me.rbtOdrType03.TabIndex = 13
        Me.rbtOdrType03.Text = "Take Out"
        Me.rbtOdrType03.UseVisualStyleBackColor = True
        '
        'rbtOdrType02
        '
        Me.rbtOdrType02.AutoSize = True
        Me.rbtOdrType02.Location = New System.Drawing.Point(83, 22)
        Me.rbtOdrType02.Name = "rbtOdrType02"
        Me.rbtOdrType02.Size = New System.Drawing.Size(59, 17)
        Me.rbtOdrType02.TabIndex = 1
        Me.rbtOdrType02.Text = "Dine In"
        Me.rbtOdrType02.UseVisualStyleBackColor = True
        '
        'rbtOdrType01
        '
        Me.rbtOdrType01.AutoSize = True
        Me.rbtOdrType01.Checked = True
        Me.rbtOdrType01.Location = New System.Drawing.Point(16, 22)
        Me.rbtOdrType01.Name = "rbtOdrType01"
        Me.rbtOdrType01.Size = New System.Drawing.Size(36, 17)
        Me.rbtOdrType01.TabIndex = 0
        Me.rbtOdrType01.TabStop = True
        Me.rbtOdrType01.Text = "All"
        Me.rbtOdrType01.UseVisualStyleBackColor = True
        '
        'frmRankingCriteria
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.SystemColors.ActiveCaption
        Me.ClientSize = New System.Drawing.Size(367, 231)
        Me.Controls.Add(Me.GroupBox1)
        Me.Controls.Add(Me.gbxPanel04)
        Me.Controls.Add(Me.cmdButton01)
        Me.Controls.Add(Me.cmdButton00)
        Me.Controls.Add(Me.gbxPanel01)
        Me.Name = "frmRankingCriteria"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Ranking Criteria"
        Me.gbxPanel04.ResumeLayout(False)
        Me.gbxPanel04.PerformLayout()
        Me.gbxPanel01.ResumeLayout(False)
        Me.gbxPanel01.PerformLayout()
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox1.PerformLayout()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents gbxPanel04 As Windows.Forms.GroupBox
    Friend WithEvents txtField02 As Windows.Forms.TextBox
    Friend WithEvents txtField01 As Windows.Forms.TextBox
    Friend WithEvents Label2 As Windows.Forms.Label
    Friend WithEvents Label1 As Windows.Forms.Label
    Friend WithEvents cmdButton01 As Windows.Forms.Button
    Friend WithEvents cmdButton00 As Windows.Forms.Button
    Friend WithEvents gbxPanel01 As Windows.Forms.GroupBox
    Friend WithEvents rbtTypex02 As Windows.Forms.RadioButton
    Friend WithEvents rbtTypex01 As Windows.Forms.RadioButton
    Friend WithEvents GroupBox1 As Windows.Forms.GroupBox
    Friend WithEvents rbtOdrType03 As Windows.Forms.RadioButton
    Friend WithEvents rbtOdrType02 As Windows.Forms.RadioButton
    Friend WithEvents rbtOdrType01 As Windows.Forms.RadioButton
    Friend WithEvents txtSearch As Windows.Forms.TextBox
End Class
