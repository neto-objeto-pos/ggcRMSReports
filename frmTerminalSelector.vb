Imports ggcAppDriver

Public Class frmTerminalSelector
    Private pn_Loaded As Integer
    Private p_bCancelled As Boolean
    Private p_oDriver As GRider
    Private p_sTerminal As String
    Private p_oIDNumber As String


    Public WriteOnly Property GRider() As GRider
        Set(ByVal foValue As GRider)
            p_oDriver = foValue
        End Set
    End Property

    Public ReadOnly Property Cancelled() As Boolean
        Get
            Return p_bCancelled
        End Get
    End Property

    Public ReadOnly Property IDNumber() As String
        Get
            Return p_oIDNumber
        End Get
    End Property

    Private Function isPOsNoOk() As Boolean
        Dim lsSQL As String
        lsSQL = "SELECT " & _
                        "sIDNumber" & _
                    " FROM Cash_Reg_Machine" & _
                    " WHERE nPOSNumbr = " & strParm(p_sTerminal)
        Dim loDta As DataTable
        loDta = p_oDriver.ExecuteQuery(lsSQL)

        If loDta.Rows.Count = 0 Then
            MsgBox("No POS Machine Detected.", MsgBoxStyle.Information, "Notice")
            Return False
            Exit Function
        Else
            p_oIDNumber = loDta(0)("sIDNumber")
            Return True
        End If
    End Function

    Private Sub cmdButton01_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdButton01.Click
        If isPOsNoOk() Then
            p_bCancelled = False
            Me.Hide()
        End If
    End Sub

    Private Sub cmdButton00_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdButton00.Click
        p_bCancelled = True
        Me.Hide()
    End Sub

    Private Sub frmSalesCriteria_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        txtField00.Text = ""
    End Sub

    Private Sub txtField00_Validated(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtField00.Validated
        p_sTerminal = txtField00.Text
    End Sub
End Class