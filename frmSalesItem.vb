Imports ggcAppDriver

Public Class frmSalesItem
    Private pn_Loaded As Integer
    Private p_bCancelled As Boolean
    Private p_oDriver As ggcAppDriver.GRider
    Private p_sTerminal As String
    Private p_sForm As String
    Private p_oIDNumber As String

    Public Property FormType As String
        Get
            Return p_sForm
        End Get
        Set(ByVal value As String)
            p_sForm = value
        End Set
    End Property

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


    Private Sub cmdButton01_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdButton01.Click
        If Not (IsDate(txtField01.Text) And
                    IsDate(txtField02.Text)) Then
            MsgBox("There are invalid date in the RANGE group." & vbCrLf &
                   "Please check your entry try again!", vbOKOnly, "Parameter Validation")
            Exit Sub
        ElseIf CDate(txtField01.Text) > CDate(txtField02.Text) Then
            MsgBox("FROM parameter seems to be higher than THRU in the RANGE group." & vbCrLf &
                   "Please check your entry try again!", vbOKOnly, "Parameter Validation")
            Exit Sub
        End If

        p_bCancelled = False
        Me.Hide()
        If p_sForm <> "1" Then Call frmTerminalSelector()
    End Sub

    Private Sub cmdButton00_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdButton00.Click
        p_bCancelled = True
        Me.Hide()
    End Sub

    Private Sub txtField01_Validated(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtField01.Validated
        If IsDate(txtField01.Text) Then
            txtField01.Text = Format(CDate(txtField01.Text), "yyyy-MM-dd")
        Else
            txtField01.Text = Format(Now(), "yyyy-MM-dd")
        End If
    End Sub

    Private Sub txtField02_Validated(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtField02.Validated
        If IsDate(txtField02.Text) Then
            txtField02.Text = Format(CDate(txtField02.Text), "yyyy-MM-dd")
        Else
            txtField02.Text = Format(Now(), "yyyy-MM-dd")
        End If
    End Sub

    Private Sub frmSalesCriteria_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        txtField01.Text = Format(Now(), "yyyy-MM-dd")
        txtField02.Text = Format(Now(), "yyyy-MM-dd")
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
End Class
