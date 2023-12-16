<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmTerminalSelector
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
        Me.Label1 = New System.Windows.Forms.Label()
        Me.txtField00 = New System.Windows.Forms.TextBox()
        Me.gbxPanel04 = New System.Windows.Forms.GroupBox()
        Me.cmdButton00 = New System.Windows.Forms.Button()
        Me.cmdButton01 = New System.Windows.Forms.Button()
        Me.gbxPanel04.SuspendLayout()
        Me.SuspendLayout()
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(12, 31)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(87, 13)
        Me.Label1.TabIndex = 0
        Me.Label1.Text = "Terminal Number"
        '
        'txtField00
        '
        Me.txtField00.Location = New System.Drawing.Point(16, 49)
        Me.txtField00.Name = "txtField00"
        Me.txtField00.Size = New System.Drawing.Size(147, 20)
        Me.txtField00.TabIndex = 3
        '
        'gbxPanel04
        '
        Me.gbxPanel04.Controls.Add(Me.txtField00)
        Me.gbxPanel04.Controls.Add(Me.Label1)
        Me.gbxPanel04.Location = New System.Drawing.Point(5, 8)
        Me.gbxPanel04.Name = "gbxPanel04"
        Me.gbxPanel04.Size = New System.Drawing.Size(218, 105)
        Me.gbxPanel04.TabIndex = 8
        Me.gbxPanel04.TabStop = False
        Me.gbxPanel04.Text = "Terminal"
        '
        'cmdButton00
        '
        Me.cmdButton00.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.cmdButton00.Location = New System.Drawing.Point(229, 77)
        Me.cmdButton00.Name = "cmdButton00"
        Me.cmdButton00.Size = New System.Drawing.Size(77, 33)
        Me.cmdButton00.TabIndex = 12
        Me.cmdButton00.Text = "&Cancel"
        Me.cmdButton00.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.cmdButton00.UseVisualStyleBackColor = True
        '
        'cmdButton01
        '
        Me.cmdButton01.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.cmdButton01.Location = New System.Drawing.Point(229, 40)
        Me.cmdButton01.Name = "cmdButton01"
        Me.cmdButton01.Size = New System.Drawing.Size(77, 33)
        Me.cmdButton01.TabIndex = 11
        Me.cmdButton01.Text = "&Ok"
        Me.cmdButton01.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.cmdButton01.UseVisualStyleBackColor = True
        '
        'frmTerminalSelector
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.SystemColors.ActiveCaption
        Me.ClientSize = New System.Drawing.Size(309, 127)
        Me.Controls.Add(Me.cmdButton01)
        Me.Controls.Add(Me.cmdButton00)
        Me.Controls.Add(Me.gbxPanel04)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow
        Me.Name = "frmTerminalSelector"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Terminal Selector"
        Me.gbxPanel04.ResumeLayout(False)
        Me.gbxPanel04.PerformLayout()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents txtField00 As System.Windows.Forms.TextBox
    Friend WithEvents gbxPanel04 As System.Windows.Forms.GroupBox
    Friend WithEvents cmdButton00 As System.Windows.Forms.Button
    Friend WithEvents cmdButton01 As System.Windows.Forms.Button
End Class
