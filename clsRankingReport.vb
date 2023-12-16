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
'#                                 DATE LAST MODIFIED 07-04-2022                        #'
'########################################################################################'
Imports MySql.Data.MySqlClient
Imports ADODB
Imports ggcAppDriver
Imports CrystalDecisions.CrystalReports.Engine

Public Class clsRankingReport
    Private Const xsSignature As String = "08220326"

    Private p_oDriver As ggcAppDriver.GRider
    Private p_oSTRept As DataSet
    Private p_oDTSrce As DataTable

    Private p_sMachinex As String
    Private p_nReptType As Integer      '0=Summary;1=Detail
    Private p_sBranchCD As String


    Private p_dFromDate As Date
    Private p_dThruDate As Date
    Private p_sTerminal As String
    Private p_sInvoicex As String

    Private p_sTransNox As String
    Private p_sTranDate As String
    Private p_sBarrcode As String
    Private p_sCategory As String
    Private p_sDescrption As String
    Private p_sOrderType As String
    Private p_nQty As Integer
    Private p_nUnitPrice As Decimal
    Private p_nTotl As Decimal

    Dim p_sRTCat As Boolean
    Dim p_sRTItem As Boolean
    Dim p_sOTAll As Boolean
    Dim p_sOTDineIn As Boolean
    Dim p_sOTTakeOut As Boolean

    Public Function ReportTrans() As Boolean
        Dim loForm As frmRankingCriteria
        loForm = New frmRankingCriteria
        loForm.GRider = p_oDriver
        loForm.ShowDialog()

        If loForm.Cancelled Then
            MsgBox("Unable to generate report.", MsgBoxStyle.Information, "Notice")
            loForm = Nothing
            Return False
        End If

        p_sTerminal = loForm.TerminalNo
        p_dFromDate = loForm.txtField01.Text
        p_dThruDate = loForm.txtField02.Text

        p_sRTItem = loForm.rbtTypex01.Checked
        p_sRTCat = loForm.rbtTypex02.Checked
        p_sOTAll = loForm.rbtOdrType01.Checked
        p_sOTDineIn = loForm.rbtOdrType02.Checked
        p_sOTTakeOut = loForm.rbtOdrType03.Checked
        Dim p_sTextSearch = loForm.txtSearch.Text
        Dim p_selected As Boolean = True



        Dim lsSQL As String 'whole statement
        lsSQL = "SELECT" &
                    " a.sTransNox `Transaction No.`" &
                    ", a.dTransact `DATE`  " &
                    ", c.sBarcodex AS `Barrcode` " &
                    ", d.sDescript AS `Category` " &
                    ", c.sDescript AS `Item Description` " &
                    ", CASE a.sTableNox " &
                       "WHEN '' THEN 'TAKE OUT' " &
                       " ELSE 'DINE IN'   " &
                       " End `ORDER TYPE` " &
                    ", SUM(b.nQuantity) AS `QTY` " &
                    ", b.nUnitPrce AS `Unit Price` " &
                    ", b.nUnitPrce * SUM(b.nQuantity) AS `Total` " &
                 "From SO_Master a " &
                    ", SO_Detail b " &
                    "  LEFT JOIN Inventory c On b.sStockIDx = c.sStockIDx" &
                    "  LEFT JOIN Product_Category d ON d.sCategrCd = c.sCategrID" &
                " WHERE a.cTranStat = '2' " &
                    " And a.sTransNox = b.sTransNox " &
                    " AND a.dTransact BETWEEN " & dateParm(p_dFromDate) & " AND " & dateParm(p_dThruDate)

        Dim p_oType As Boolean = True
        Select Case p_selected
            Case p_sRTItem
                lsSQL = lsSQL + "AND c.sDescript LIKE " & "'%" & p_sTextSearch & "'"

                Select Case p_selected
                    Case p_sOTAll
                    Case p_sOTDineIn
                        lsSQL = lsSQL + "AND a.sTableNox <> ''"
                    Case p_sOTTakeOut
                        lsSQL = lsSQL + "AND a.sTableNox = ''"
                End Select

                lsSQL = lsSQL + " Group BY a.dTransact, b.sStockIDx " &
                               " ORDER BY  QTY DESC "

            Case p_sRTCat
                lsSQL = lsSQL + "AND d.sDescript LIKE " & "'" & p_sTextSearch & "%'"

                Select Case p_selected
                    Case p_sOTAll
                    Case p_sOTDineIn
                        lsSQL = lsSQL + "AND a.sTableNox <> ''"
                    Case p_sOTTakeOut
                        lsSQL = lsSQL + "AND a.sTableNox = ''"
                End Select
                lsSQL = lsSQL + " Group BY a.dTransact, b.sStockIDx " &
                               " ORDER BY  QTY DESC"
        End Select

        p_oDTSrce = p_oDriver.ExecuteQuery(lsSQL)

        If p_oDTSrce.Rows.Count = 0 Then
            MsgBox("No records found for the given criteria.", MsgBoxStyle.Information, "Notice")
            Return False
        End If


        Dim oProg As frmProgress
        oProg = New frmProgress
        oProg.PistonInfo = p_oDriver.AppPath & "/piston.avi"
        oProg.ShowTitle("EXTRACTING RECORDS FROM DATABASE")
        oProg.ShowProcess("Please wait...")
        oProg.Show()


        Dim loDtaTbl As DataTable = getRptTable()
        Dim lnCtr As Integer
        Dim lbAdd As Boolean = False

        oProg.ShowTitle("LOADING RECORDS")
        oProg.MaxValue = p_oDTSrce.Rows.Count

        p_sTransNox = p_oDTSrce(lnCtr).Item("Transaction No.")
        For lnCtr = 0 To p_oDTSrce.Rows.Count - 1
            oProg.ShowProcess("Loading " & p_oDTSrce(lnCtr).Item("Transaction No.") & "...")
            'p_sCRMNumbr = p_oDTSrce(lnCtr).Item("sCRMNumbr")

            If p_sTransNox = p_oDTSrce(lnCtr).Item("Transaction No.") Then
                p_sTranDate = p_oDTSrce(lnCtr).Item("DATE")
                p_sBarrcode = p_oDTSrce(lnCtr).Item("Barrcode")
                p_sCategory = p_oDTSrce(lnCtr).Item("Category")
                p_sDescrption = p_oDTSrce(lnCtr).Item("Item Description")
                p_sOrderType = p_oDTSrce(lnCtr).Item("ORDER TYPE")
                p_nQty = p_oDTSrce(lnCtr).Item("QTY")
                p_nUnitPrice = p_oDTSrce(lnCtr).Item("Unit Price")
                p_nTotl = p_oDTSrce(lnCtr).Item("QTY") * p_oDTSrce(lnCtr).Item("Unit Price")
            Else
                p_sTranDate = p_oDTSrce(lnCtr).Item("DATE")
                p_sBarrcode = p_oDTSrce(lnCtr).Item("Barrcode")
                p_sCategory = IFNull(p_oDTSrce(lnCtr).Item("Category"), "NONE")
                p_sDescrption = p_oDTSrce(lnCtr).Item("Item Description")
                p_sOrderType = p_oDTSrce(lnCtr).Item("ORDER TYPE")
                p_nQty = p_oDTSrce(lnCtr).Item("QTY")
                p_nUnitPrice = p_oDTSrce(lnCtr).Item("Unit Price")
                p_nTotl = p_oDTSrce(lnCtr).Item("QTY") * p_oDTSrce(lnCtr).Item("Unit Price")
            End If

            'check if the next or is not the same as the current
            If lnCtr <> p_oDTSrce.Rows.Count - 1 Then
                lbAdd = p_sTranDate <> p_oDTSrce(lnCtr + 1).Item("DATE")
            Else
                lbAdd = True
            End If

            loDtaTbl.Rows.Add(addRow(lnCtr, loDtaTbl, lbAdd))
        Next

        oProg.ShowSuccess()

        Dim clsRpt As clsReports
        clsRpt = New clsReports
        clsRpt.GRider = p_oDriver
        'Set the Report Source Here
        If Not clsRpt.initReport("RnkRpt") Then
            Return False
        End If

        Dim loRpt As ReportDocument = clsRpt.ReportSource

        Dim loTxtObj As CrystalDecisions.CrystalReports.Engine.TextObject
        loTxtObj = loRpt.ReportDefinition.Sections(0).ReportObjects("txtCompany")
        loTxtObj.Text = p_oDriver.BranchName

        'Set Branch Address
        loTxtObj = loRpt.ReportDefinition.Sections(0).ReportObjects("txtAddress")
        loTxtObj.Text = p_oDriver.Address & ", " & p_oDriver.TownCity & " " & p_oDriver.ZippCode & ", " & p_oDriver.Province

        'Set First Header
        'loTxtObj = loRpt.ReportDefinition.Sections(1).ReportObjects("txtHeading1") 
        'loTxtObj.Text = "DAILY SALES SUMMARY REPORT"

        'Set Second Header
        loTxtObj = loRpt.ReportDefinition.Sections(1).ReportObjects("txtHeading2")
        loTxtObj.Text = Format(p_dFromDate, xsDATE_LONG) & " to " & Format(p_dThruDate, xsDATE_LONG)

        loTxtObj = loRpt.ReportDefinition.Sections(1).ReportObjects("lblTerminal")
        loTxtObj.Text = "Terminal No. " & p_sMachinex

        loTxtObj = loRpt.ReportDefinition.Sections(3).ReportObjects("txtRptUser")
        loTxtObj.Text = Decrypt(p_oDriver.UserName, "08220326")

        'POS Info
        'loTxtObj = loRpt.ReportDefinition.Sections(1).ReportObjects("txtVATReg")
        'loTxtObj.Text = p_sVatReg

        'loTxtObj = loRpt.ReportDefinition.Sections(1).ReportObjects("txtDateTime")
        'loTxtObj.Text = p_oDriver.getSysDate

        'loTxtObj = loRpt.ReportDefinition.Sections(1).ReportObjects("txtTerminal")
        'loTxtObj.Text = p_sSerial

        'loTxtObj = loRpt.ReportDefinition.Sections(3).ReportObjects("txtMIN")
        'loTxtObj.Text = p_sMachinex

        'loTxtObj = loRpt.ReportDefinition.Sections(3).ReportObjects("txtUserID")
        'loTxtObj.Text = Decrypt(p_oDriver.UserName, "08220326")

        loRpt.SetDataSource(p_oSTRept)
        clsRpt.showReport()

        Return True
    End Function

    Public Function getCashier(ByVal sCashierx As String) As String
        Dim lsCashierNm As String

        lsCashierNm = Decrypt(sCashierx, xsSignature)
        Return lsCashierNm

    End Function

    Private Function getRptTable() As DataTable
        'Initialize DataSet
        p_oSTRept = New DataSet

        'Load the data structure of the Dataset
        'Data structure was saved at DataSet1.xsd 
        p_oSTRept.ReadXmlSchema(p_oDriver.AppPath & "\vb.net\RetMgySys\Reports\DataSet1.xsd")

        'Return the schema of the datatable derive from the DataSet 
        Return p_oSTRept.Tables(0)
    End Function

    Private Function addRow(ByVal lnRow As Integer, ByVal foSchemaTable As DataTable, Optional ByVal lbAddFoot As Boolean = False) As DataRow
        'ByVal foDTInclue As DataTable
        Dim loDtaRow As DataRow

        'Create row based on the schema of foSchemaTable
        loDtaRow = foSchemaTable.NewRow
        'Dim iString As String = CInt(p_sTranDate).ToString("####/##/##")
        'Dim oYear As String = iString.Substring(2, 2)
        'Dim oDay As String = iString.Substring(8, 2)
        'Dim oMonth As String = Mid(iString, 6, 2)

        'Dim amonth() As String = {"Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"}
        'Dim x As Integer = Integer.Parse(oMonth)
        'Dim sTransDate As String = amonth.GetValue(x - 1) + "/" + oDay + "/" + oYear

        loDtaRow.Item("sField01") = Format(xsDATE_MEDIUM, p_sTranDate)
        loDtaRow.Item("sField03") = p_sBarrcode
        loDtaRow.Item("sField05") = p_sCategory
        loDtaRow.Item("sField02") = p_sDescrption
        loDtaRow.Item("sField04") = p_sOrderType
        loDtaRow.Item("nField01") = p_nQty
        loDtaRow.Item("nField02") = p_nUnitPrice
        loDtaRow.Item("nField03") = p_nTotl

        Return loDtaRow
    End Function

    Public Sub New(ByVal foRider As GRider)
        p_oDriver = foRider
        p_oSTRept = Nothing
        p_oDTSrce = Nothing
    End Sub

    Public Sub New(ByVal foRider As GRider,
                   ByVal foMachineNo As String)
        p_oDriver = foRider
        p_oSTRept = Nothing
        p_oDTSrce = Nothing

        p_sMachinex = foMachineNo
    End Sub
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
'#                                 DATE LAST MODIFIED 07-04-2022                        #'
'########################################################################################'