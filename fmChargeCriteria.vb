Imports System.Windows.Forms
Imports ggcAppDriver

Public Class frmChargeCriteria
    Private pn_Loaded As Integer
    Private p_bCancelled As Boolean
    Private p_oDriver As ggcAppDriver.GRider
    Private p_sTerminal As String
    Private p_sForm As String
    Private p_oIDNumber As String
    Private p_bHasSummary As Boolean = False
    Private p_sReportType As Integer = 1 '0=Summary;1=Detail    

    Public Property FormType As String
        Get
            Return p_sForm
        End Get
        Set(ByVal value As String)
            p_sForm = value
        End Set
    End Property
    Public Property HasSummaryReport As Boolean
        Get
            Return p_bHasSummary
        End Get
        Set(ByVal value As Boolean)
            p_bHasSummary = value
        End Set
    End Property

    Public Function isOkey() As Boolean
        Return Not p_bCancelled
    End Function

    Public WriteOnly Property GRider() As ggcAppDriver.GRider
        Set(ByVal foValue As ggcAppDriver.GRider)
            p_oDriver = foValue
        End Set
    End Property

    Public ReadOnly Property Cancelled() As Boolean
        Get
            Return p_bCancelled
        End Get
    End Property

    Public ReadOnly Property ReportType() As Integer
        Get
            Return p_sReportType
        End Get
    End Property

    Public ReadOnly Property TerminalNo As String
        Get
            Return p_sTerminal
        End Get
    End Property

    Public ReadOnly Property IDNoxxx As String
        Get
            Return p_oIDNumber
        End Get
    End Property

    Private Sub cmdButton01_Click(sender As Object, e As EventArgs) Handles cmdButton01.Click
        If Not (IsDate(txtField00.Text) And
            IsDate(txtField01.Text)) Then
            MsgBox("There are invalid date in the RANGE group." & vbCrLf &
                   "Please check your entry try again!", vbOKOnly, "Parameter Validation")
            Exit Sub
        ElseIf CDate(txtField00.Text) > CDate(txtField01.Text) Then
            MsgBox("FROM parameter seems to be higher than THRU in the RANGE group." & vbCrLf &
                   "Please check your entry try again!", vbOKOnly, "Parameter Validation")
            Exit Sub
        End If

        p_bCancelled = False
        Me.Hide()

    End Sub

    Private Sub cmdButton00_Click(sender As Object, e As EventArgs) Handles cmdButton00.Click
        p_bCancelled = True
        Me.Hide()
    End Sub

    Private Sub txtField00_Validated(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtField00.Validated
        If IsDate(txtField00.Text) Then
            txtField00.Text = Format(CDate(txtField00.Text), "yyyy-MM-dd")
        Else
            txtField00.Text = Format(Now(), "yyyy-MM-dd")
        End If
    End Sub

    Private Sub txtField01_Validated(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtField01.Validated
        If IsDate(txtField01.Text) Then
            txtField01.Text = Format(CDate(txtField01.Text), "yyyy-MM-dd")
        Else
            txtField01.Text = Format(Now(), "yyyy-MM-dd")
        End If
    End Sub

    Private Sub frmSalesCriteria_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        txtField00.Text = Format(Now(), "yyyy-MM-dd")
        txtField01.Text = Format(Now(), "yyyy-MM-dd")

        If p_sForm <> "1" Then
            Me.Hide()
            Call frmTerminalSelector()
            p_bCancelled = False


        End If
        Me.Show()
    End Sub

    Private Sub frmTerminalSelector()
        Dim loForm As frmTerminalSelector

        loForm = New frmTerminalSelector
        loForm.GRider = p_oDriver
        loForm.ShowDialog()

        p_sTerminal = loForm.txtField00.Text
        p_oIDNumber = loForm.IDNumber

        If loForm.Cancelled Then
            MsgBox("Unable to generate report.", MsgBoxStyle.Information, "Notice")
            p_bCancelled = loForm.Cancelled
            loForm = Nothing
            Return
        End If

    End Sub

    Private Sub frmSalesCriteria_Activated(sender As Object, e As EventArgs) Handles Me.Activated
        rbtTypex01.Enabled = p_bHasSummary
        rbtTypex02.Checked = True
    End Sub

    Private Sub rbtTypex01_CheckedChanged(sender As Object, e As EventArgs) Handles rbtTypex01.CheckedChanged, rbtTypex02.CheckedChanged
        If rbtTypex01.Checked Then
            rbtTypex02.Checked = False
            p_sReportType = 0 'Summary
        ElseIf rbtTypex02.Checked Then
            rbtTypex01.Checked = False
            p_sReportType = 1 'Detailed

        End If

    End Sub
End Class
