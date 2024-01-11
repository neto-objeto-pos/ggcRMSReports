
Imports System.Windows.Forms
Imports ggcAppDriver

Public Class frmSOACriteria

    Private pb_FieldOk As Boolean
    Private pn_Loaded As Integer
    Private p_oDriver As GRider

    Private p_bCancelled As Boolean

    Public WriteOnly Property GRider() As GRider
        Set(ByVal foValue As GRider)
            p_oDriver = foValue
        End Set
    End Property

    Public Function isOkey() As Boolean
        Return pb_FieldOk
    End Function


    Private Sub cmdButton01_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdButton01.Click

        If Not (txtField00.Tag <> "") Then
            MsgBox("Filter seems to be empty. " & vbCrLf &
                   "Please check your entry try again!", vbOKOnly, "Parameter Validation")
            Exit Sub

        ElseIf Not (IsDate(txtField01.Text) And
                    IsDate(txtField02.Text)) Then

            MsgBox("There are invalid date in the RANGE group." & vbCrLf &
                   "Please check your entry try again!", vbOKOnly, "Parameter Validation")
            Exit Sub

        ElseIf CDate(txtField01.Text) > CDate(txtField02.Text) Then
            MsgBox("FROM parameter seems to be higher than THRU in the RANGE group." & vbCrLf &
                   "Please check your entry try again!", vbOKOnly, "Parameter Validation")
            Exit Sub
        End If

        pb_FieldOk = True

        Me.Hide()
    End Sub

    Private Sub cmdButton00_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdButton00.Click
        pb_FieldOk = False
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
    Private Sub txtField_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txtField00.KeyDown
        If e.KeyCode = Keys.F3 Or e.KeyCode = Keys.Return Then
            Dim loTxt As TextBox
            loTxt = CType(sender, System.Windows.Forms.TextBox)

            Dim loIndex As Integer
            loIndex = Val(Mid(loTxt.Name, 9))

            If Mid(loTxt.Name, 1, 8) = "txtField" Then
                Select Case loIndex
                    Case 0
                        If (txtField00.Text <> "") Then SearchDeliveryService(loTxt.Text, False, True)
                        If txtField00.Tag <> "" Then pb_FieldOk = True
                End Select
            End If
        End If
    End Sub

    Private Sub frmSOADetailedCriteria_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        txtField01.Text = Format(Now(), "yyyy-MM-dd")
        txtField02.Text = txtField01.Text
        txtField00.Text = ""
        txtField00.Tag = ""


    End Sub


    Public Function SearchDeliveryService(
                ByVal fsValue As String _
              , ByVal fbByCode As Boolean _
                , ByVal fbIsSrch As Boolean) As Boolean

        Dim lsSQL As String

        'Initialize SQL filter
        lsSQL = getSQ_DeliveryService()


        'create Kwiksearch filter
        Dim lsFilter As String
        If fbByCode Then
            lsFilter = "sRiderIDx LIKE " & strParm("%" & fsValue)
        Else
            lsFilter = "sDescript like " & strParm(fsValue & "%")
        End If
        If fbIsSrch Then
            Debug.Print(lsSQL)
            Dim loDta As DataRow = KwikSearch(p_oDriver _
                                            , lsSQL _
                                            , False _
                                             , lsFilter _
                                             , "sDescript»dPartnerx" _
                                             , "Company Name»Partner Date",
                                             , "sDescript»dPartnerx" _
                                             , IIf(fbByCode, 0, 1))
            If IsNothing(loDta) Then

                Return False
            Else
                txtField00.Text = (loDta.Item("sDescript"))
                txtField00.Tag = (loDta.Item("sRiderIDx"))



            End If
        End If
        Return True
    End Function

    Private Function getSQ_DeliveryService() As String
        Return "SELECT sRiderIDx " &
                    ", sDescript " &
                    ", dPartnerx " &
              " FROM Delivery_Service"


    End Function
End Class
