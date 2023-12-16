<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmSalesSummary
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
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.rbtn02 = New System.Windows.Forms.RadioButton()
        Me.rbtn01 = New System.Windows.Forms.RadioButton()
        Me.rbtn00 = New System.Windows.Forms.RadioButton()
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.Button2 = New System.Windows.Forms.Button()
        Me.Button1 = New System.Windows.Forms.Button()
        Me.GroupBox1.SuspendLayout()
        Me.Panel1.SuspendLayout()
        Me.SuspendLayout()
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.rbtn02)
        Me.GroupBox1.Controls.Add(Me.rbtn01)
        Me.GroupBox1.Controls.Add(Me.rbtn00)
        Me.GroupBox1.Location = New System.Drawing.Point(5, 7)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(278, 123)
        Me.GroupBox1.TabIndex = 0
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "Report Selection"
        '
        'rbtn02
        '
        Me.rbtn02.AutoSize = True
        Me.rbtn02.Checked = True
        Me.rbtn02.Location = New System.Drawing.Point(25, 75)
        Me.rbtn02.Name = "rbtn02"
        Me.rbtn02.Size = New System.Drawing.Size(118, 17)
        Me.rbtn02.TabIndex = 2
        Me.rbtn02.TabStop = True
        Me.rbtn02.Text = "BIR Sales Summary"
        Me.rbtn02.UseVisualStyleBackColor = True
        '
        'rbtn01
        '
        Me.rbtn01.AutoSize = True
        Me.rbtn01.Location = New System.Drawing.Point(25, 50)
        Me.rbtn01.Name = "rbtn01"
        Me.rbtn01.Size = New System.Drawing.Size(97, 17)
        Me.rbtn01.TabIndex = 1
        Me.rbtn01.Text = "Sales Summary"
        Me.rbtn01.UseVisualStyleBackColor = True
        '
        'rbtn00
        '
        Me.rbtn00.AutoSize = True
        Me.rbtn00.Location = New System.Drawing.Point(25, 25)
        Me.rbtn00.Name = "rbtn00"
        Me.rbtn00.Size = New System.Drawing.Size(93, 17)
        Me.rbtn00.TabIndex = 0
        Me.rbtn00.Text = "Detailed Sales"
        Me.rbtn00.UseVisualStyleBackColor = True
        '
        'Panel1
        '
        Me.Panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Panel1.Controls.Add(Me.Button2)
        Me.Panel1.Controls.Add(Me.Button1)
        Me.Panel1.Location = New System.Drawing.Point(290, 12)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(87, 118)
        Me.Panel1.TabIndex = 1
        '
        'Button2
        '
        Me.Button2.Location = New System.Drawing.Point(5, 44)
        Me.Button2.Name = "Button2"
        Me.Button2.Size = New System.Drawing.Size(74, 35)
        Me.Button2.TabIndex = 4
        Me.Button2.Text = "Cancel"
        Me.Button2.UseVisualStyleBackColor = True
        '
        'Button1
        '
        Me.Button1.Location = New System.Drawing.Point(5, 4)
        Me.Button1.Name = "Button1"
        Me.Button1.Size = New System.Drawing.Size(74, 35)
        Me.Button1.TabIndex = 3
        Me.Button1.Text = "OK"
        Me.Button1.UseVisualStyleBackColor = True
        '
        'frmSalesSummary
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.SystemColors.ActiveCaption
        Me.ClientSize = New System.Drawing.Size(381, 136)
        Me.Controls.Add(Me.Panel1)
        Me.Controls.Add(Me.GroupBox1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D
        Me.Name = "frmSalesSummary"
        Me.Text = "Sales Report"
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox1.PerformLayout()
        Me.Panel1.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
    Friend WithEvents rbtn02 As System.Windows.Forms.RadioButton
    Friend WithEvents rbtn01 As System.Windows.Forms.RadioButton
    Friend WithEvents rbtn00 As System.Windows.Forms.RadioButton
    Friend WithEvents Panel1 As System.Windows.Forms.Panel
    Friend WithEvents Button2 As System.Windows.Forms.Button
    Friend WithEvents Button1 As System.Windows.Forms.Button
End Class
