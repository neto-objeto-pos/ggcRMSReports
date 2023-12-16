'########################################################################################'
'#        ___          ___          ___           ___       ___                         #'
'#       /\  \        /\  \        /\  \         /\  \     /\  \         ___            #'
'#       \:\  \      /::\  \      /::\  \        \:\  \   /::\  \       /\  \           #'
'#        \:\  \    /:/\:\  \    /:/\:\  \   ___ /::\__\ /:/\:\  \      \:\  \          #'
'#        /::\  \  /::\~\:\  \  /::\~\:\  \ /\  /:/\/__//::\~\:\  \     /::\__\         #'
'#       /:/\:\__\/:/\:\ \:\__\/:/\:\ \:\__\\:\/:/  /  /:/\:\ \:\__\ __/:/\/__/         #'
'#      /:/  \/__/\:\~\:\ \/__/\:\~\:\ \/__/ \::/  /   \:\~\:\ \/__//\/:/  /            #'
'#     /:/  /      \:\ \:\__\   \:\ \:\__\    \/__/     \:\ \:\__\  \::/__/             #'
'#     \/__/        \:\ \/__/    \:\ \/__/               \:\ \/__/   \:\__\             #'
'#                   \:\__\       \:\__\                  \:\__\      \/__/             #'
'#                    \/__/        \/__/                   \/__/                        #'
'#                                                                                      #'
'#                                 DATE CREATED 07-01-2022                              #'
'#                                 DATE LAST MODIFIED 07-02-2022                        #'
'########################################################################################'




Imports System.Windows.Forms
Imports ggcAppDriver
Public Class frmRankingCriteria
    Private pn_Loaded As Integer
    Private p_bCancelled As Boolean
    Private p_oDriver As ggcAppDriver.GRider
    Private p_sTerminal As String
    Private p_sForm As String
    Private p_oIDNumber As String
    Private p_sTxtSearch As String
    Private p_sSrchText As String

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
        'If p_sForm <> "1" Then Call frmTerminalSelector()

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

    Private Sub rbtTypex01_CheckedChanged(sender As Object, e As EventArgs) Handles rbtTypex01.CheckedChanged

    End Sub

    Private Sub txtSearch_TextChanged(sender As Object, e As EventArgs) Handles txtSearch.TextChanged

    End Sub

    Private Sub txtSearch_KeyDown(sender As Object, e As KeyEventArgs) Handles txtSearch.KeyDown
        If e.KeyCode = Keys.F3 Or e.KeyCode = Keys.Enter Then
            'Dim loTxt As TextBox
            'loTxt = CType(sender, System.Windows.Forms.TextBox)
            'Dim loIndex As Integer
            'loIndex = Val(Mid(loTxt.Name, 9))

            'If Mid(loTxt.Name, 1, 8) = "txtField" Then
            '    Select Case loIndex
            '        Case 1
            '    End Select
            'End If
            p_sTxtSearch = txtSearch.Text
            If rbtTypex01.Checked = True Then
                isReportTypeItem(fsValue:=p_sTxtSearch)
            ElseIf rbtTypex02.Checked = True Then
                isReportTypeCategory(fsValue:=p_sTxtSearch)
            End If

        End If
    End Sub

    Private Function isReportTypeItem(ByVal fsValue As String _
                                , Optional ByVal fbByCode As Boolean = False) As Boolean

        Dim lsSQL As String
        lsSQL = " SELECT" &
                   "  sStockIDx " &
                   " ,sBarcodex " &
                   " , sDescript " &
                   " , sBriefDsc " &
               " FROM Inventory " &
               " WHERE cRecdStat = '1'" &
               " AND sDescript LIKE " & "'" & p_sTxtSearch & "%'"
        Dim lsFilter As String
        ' Dim loDta As DataTable
        If fbByCode Then
            lsFilter = "sBarCodex = " & strParm(fsValue)
        Else
            lsFilter = "sDescript LIKE " & strParm(fsValue & "%")
        End If
        Dim loDta As DataRow = KwikSearch(p_oDriver _
                                        , lsSQL _
                                        , False _
                                        , lsFilter _
                                        , "sBarCodex»sBriefDsc»sDescript" _
                                        , "Barcode»sBriefDsc»Description",
                                        , "sBarCodex»sBriefDsc»sDescript" _
                                        , IIf(fbByCode, 0, 1))


        If IsNothing(loDta) Then
            MsgBox("No Inventory found.", MsgBoxStyle.Information, "Notice")
            Return False
        Else
            txtSearch.Text = loDta.Item("sDescript")
            p_sSrchText = loDta.Item("sStockIDx")
            Return True
        End If
    End Function
    Private Function isReportTypeCategory(ByVal fsValue As String _
                                , Optional ByVal fbByCode As Boolean = False) As Boolean

        Dim lsSQL As String
        lsSQL = " SELECT" &
                   "  sCategrCd " &
                   " , sDescript " &
               " FROM Product_Category " &
               " WHERE cRecdStat = '1'" &
               " AND sDescript LIKE " & "'" & p_sTxtSearch & "%'"
        Dim lsFilter As String
        ' Dim loDta As DataTable
        If fbByCode Then
            lsFilter = "sBarCodex = " & strParm(fsValue)
        Else
            lsFilter = "sDescript LIKE " & strParm(fsValue & "%")
        End If
        Dim loDta As DataRow = KwikSearch(p_oDriver _
                                        , lsSQL _
                                        , False _
                                        , lsFilter _
                                        , "sCategrCd»sDescript" _
                                        , "Category Code»Description",
                                        , "sCategrCd»sDescript" _
                                        , IIf(fbByCode, 0, 1))


        If IsNothing(loDta) Then
            MsgBox("No Inventory found.", MsgBoxStyle.Information, "Notice")
            Return False
        Else
            txtSearch.Text = loDta.Item("sDescript")
            p_sSrchText = loDta.Item("sCategrCd")
            Return True
        End If
    End Function
End Class
'########################################################################################'
'#        ___          ___          ___           ___       ___                         #'
'#       /\  \        /\  \        /\  \         /\  \     /\  \         ___            #'
'#       \:\  \      /::\  \      /::\  \        \:\  \   /::\  \       /\  \           #'
'#        \:\  \    /:/\:\  \    /:/\:\  \   ___ /::\__\ /:/\:\  \      \:\  \          #'
'#        /::\  \  /::\~\:\  \  /::\~\:\  \ /\  /:/\/__//::\~\:\  \     /::\__\         #'
'#       /:/\:\__\/:/\:\ \:\__\/:/\:\ \:\__\\:\/:/  /  /:/\:\ \:\__\ __/:/\/__/         #'
'#      /:/  \/__/\:\~\:\ \/__/\:\~\:\ \/__/ \::/  /   \:\~\:\ \/__//\/:/  /            #'
'#     /:/  /      \:\ \:\__\   \:\ \:\__\    \/__/     \:\ \:\__\  \::/__/             #'
'#     \/__/        \:\ \/__/    \:\ \/__/               \:\ \/__/   \:\__\             #'
'#                   \:\__\       \:\__\                  \:\__\      \/__/             #'
'#                    \/__/        \/__/                   \/__/                        #'
'#                                                                                      #'
'#                                 DATE CREATED 07-01-2022                              #'
'#                                 DATE LAST MODIFIED 07-02-2022                        #'
'########################################################################################'