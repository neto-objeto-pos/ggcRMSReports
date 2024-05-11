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


Imports MySql.Data.MySqlClient
Imports ADODB
Imports ggcAppDriver
Imports CrystalDecisions.CrystalReports.Engine
Public Class clsDailySalesReport
    Private Const xsSignature As String = "08220326"

    Private p_oDriver As ggcAppDriver.GRider
    Private p_oSTRept As DataSet
    Private p_oDTSrce As DataTable

    Private p_sTerminal As String
    Private p_sMachinex As String
    Private p_sSerial As String
    Private p_sVatReg As String
    Private p_sPermit As String
    Private p_sCRMNumbr As String

    Private p_nReptType As Integer      '0=Summary;1=Detail
    Private p_sBranchCD As String

    Private p_dFromDate As Date
    Private p_dThruDate As Date
    Private p_sFromDate As String
    Private p_sThruDate As String
    Private p_sTranDate As String
    Private p_dTranDate As Date
    Private p_sORNoFrom As String
    Private p_sORNoThru As String

    Dim p_nSalesAmt As Decimal
    Dim p_nDiscount As Decimal
    Dim p_nSCDiscxx As Decimal
    Dim p_nPWDDiscx As Decimal
    Dim p_nNonVATxx As Decimal
    Dim p_nVatDiscx As Decimal
    Dim p_nVATAmtxx As Decimal
    Dim p_nNetSalesx As Decimal

    Public Sub New(ByVal foRider As GRider)
        p_oDriver = foRider
        p_oSTRept = Nothing
        p_oDTSrce = Nothing
    End Sub
    Public Sub New(ByVal foRider As GRider,
                   ByVal foMachineNo As String,
                   ByVal foSerialNo As String,
                   ByVal foPermitNo As String,
                   ByVal foVATReg As String)
        p_oDriver = foRider
        p_oSTRept = Nothing
        p_oDTSrce = Nothing

        p_sMachinex = foMachineNo
        p_sSerial = foSerialNo
        p_sPermit = foPermitNo
        p_sVatReg = foVATReg
    End Sub
    Public Function ReportTrans() As Boolean
        Dim loForm As frmSalesCriteria

        loForm = New frmSalesCriteria
        loForm.GRider = p_oDriver
        loForm.FormType = "1"
        loForm.ShowDialog()

        If loForm.Cancelled Then
            MsgBox("Unable to generate report.", MsgBoxStyle.Information, "Notice")
            loForm = Nothing
            Return False
        End If

        p_sTerminal = loForm.IDNoxxx
        p_dFromDate = loForm.txtField01.Text
        p_dThruDate = loForm.txtField02.Text
        p_sFromDate = loForm.txtField01.Text.Replace("-", "")
        p_sThruDate = loForm.txtField02.Text.Replace("-", "")


        Dim lsSQL As String 'whole statement
        If p_sTerminal <> "" Then
            lsSQL = "SELECT" &
                    "  sTranDate" &
                    ", nSalesAmt" &
                    ", nDiscount" &
                    ", nPWDDiscx" &
                    ", nNonVATxx" &
                    ", nVatDiscx" &
                    ", nVATAmtxx" &
                    ", nVATSales" &
                    ", sORNoFrom" &
                    ", sORNoThru" &
               " FROM Daily_Summary" &
               " WHERE cTranStat = '2'" &
               " ORDER BY sTranDate, dOpenedxx"
        Else
            lsSQL = "SELECT" &
                    "  sTranDate" &
                    ", nSalesAmt" &
                    ", nDiscount" &
                    ", nPWDDiscx" &
                    ", nNonVATxx" &
                    ", nVatDiscx" &
                    ", nVATAmtxx" &
                    ", nVATSales" &
                    ", sORNoFrom" &
                    ", sORNoThru" &
                    " FROM Daily_Summary" &
                    " WHERE cTranStat = '2'" &
                    "AND sTranDate BETWEEN " & (p_sFromDate) & " AND " & (p_sThruDate) &
                    " ORDER BY sTranDate, dOpenedxx"
        End If

        p_oDTSrce = p_oDriver.ExecuteQuery(lsSQL)

        If p_oDTSrce.Rows.Count = 0 Then
            MsgBox("No records found For the given criteria.", MsgBoxStyle.Information, "Notice")
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



        p_sTranDate = p_oDTSrce(lnCtr).Item("sTranDate")
        For lnCtr = 0 To p_oDTSrce.Rows.Count - 1
            oProg.ShowProcess("Loading " & p_oDTSrce(lnCtr).Item("sTranDate") & "...")
            'p_sCRMNumbr = p_oDTSrce(lnCtr).Item("sCRMNumbr")

            If p_sTranDate = p_oDTSrce(lnCtr).Item("sTranDate") Then
                If p_sORNoThru < p_oDTSrce(lnCtr).Item("sORNoThru") Then
                    p_sORNoThru = p_oDTSrce(lnCtr).Item("sORNoThru")
                End If
                p_sTranDate = p_oDTSrce(lnCtr).Item("sTranDate")
                p_nSalesAmt = p_oDTSrce(lnCtr).Item("nSalesAmt")
                p_nDiscount = p_oDTSrce(lnCtr).Item("nDiscount")
                p_nSCDiscxx = "0"
                p_nPWDDiscx = p_oDTSrce(lnCtr).Item("nPWDDiscx")
                p_nNonVATxx = p_oDTSrce(lnCtr).Item("nNonVATxx")
                p_nVatDiscx = p_oDTSrce(lnCtr).Item("nVatDiscx")
                p_nVATAmtxx = p_oDTSrce(lnCtr).Item("nVATAmtxx")
                p_nNetSalesx = p_oDTSrce(lnCtr).Item("nVatSales")
            Else
                p_sTranDate = p_oDTSrce(lnCtr).Item("sTranDate") '.ToString().Replace("-", "")
                p_nSalesAmt = p_oDTSrce(lnCtr).Item("nSalesAmt")
                p_nDiscount = p_oDTSrce(lnCtr).Item("nDiscount")
                p_nSCDiscxx = "0"
                p_nPWDDiscx = p_oDTSrce(lnCtr).Item("nPWDDiscx")
                p_nNonVATxx = p_oDTSrce(lnCtr).Item("nNonVATxx")
                p_nVatDiscx = p_oDTSrce(lnCtr).Item("nVatDiscx")
                p_nVATAmtxx = p_oDTSrce(lnCtr).Item("nVATAmtxx")
                p_nNetSalesx = p_oDTSrce(lnCtr).Item("nVatSales")


            End If

            'p_nEndngBal = p_nEndngBal + p_nBegBalxx
            'If lnCtr <> p_oDTSrce.Rows.Count - 1 Then
            '    If p_sTranDate <> p_oDTSrce(lnCtr + 1).Item("sTranDate") Then
            '        p_nEndngBal = p_nEndngBal + p_nBegBalxx
            '        'loDtaTbl.Rows.Add(addRow(loDtaTbl))
            '    End If
            'Else
            '    p_nEndngBal = p_nEndngBal + p_nBegBalxx
            '    'loDtaTbl.Rows.Add(addRow(loDtaTbl))
            'End If
            loDtaTbl.Rows.Add(addRow(loDtaTbl))
        Next

        oProg.ShowSuccess()

        Dim clsRpt As clsReports
        clsRpt = New clsReports
        clsRpt.GRider = p_oDriver
        'Set the Report Source Here
        If Not clsRpt.initReport("DSleSm") Then
            Return False
        End If

        Dim loRpt As ReportDocument = clsRpt.ReportSource

        Dim loTxtObj As CrystalDecisions.CrystalReports.Engine.TextObject
        loTxtObj = loRpt.ReportDefinition.Sections(0).ReportObjects("txtCompany")
        loTxtObj.Text = p_oDriver.BranchName
        'loTxtObj.Text = "Meet 'n' Eat"


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
    Private Function getRptTable() As DataTable
        'Initialize DataSet
        p_oSTRept = New DataSet

        'Load the data structure of the Dataset
        'Data structure was saved at DataSet1.xsd 
        p_oSTRept.ReadXmlSchema(p_oDriver.AppPath & "\vb.net\RetMgtSys\Reports\DataSet1.xsd")

        'Return the schema of the datatable derive from the DataSet 
        Return p_oSTRept.Tables(0)
    End Function

    Private Function addRow(ByVal foSchemaTable As DataTable) As DataRow
        'ByVal foDTInclue As DataTable
        Dim loDtaRow As DataRow

        'Create row based on the schema of foSchemaTable
        loDtaRow = foSchemaTable.NewRow


        'Dim iString1 As String = CInt(p_sTranDate).ToString("####/##/##")
        'Dim iString2 As String = CInt(p_sTranDate).ToString("####/##/##")
        'Dim oYear As String = Mid("", 4)
        'Dim oMonth As String = iString1.Substring(5, 2)
        'Dim oDay As String = Mid("", 7, 2)

        Dim iString As String = CInt(p_sTranDate).ToString("####/##/##")
        Dim oYear As String = iString.Substring(2, 2)
        Dim oDay As String = iString.Substring(8, 2)
        Dim oMonth As String = Mid(iString, 6, 2)

        Dim amonth() As String = {"Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"}
        Dim x As Integer = Integer.Parse(oMonth)
        Dim sTransDate As String = amonth.GetValue(x - 1) + "/" + oDay + "/" + oYear

        loDtaRow.Item("sField01") = sTransDate
        loDtaRow.Item("nField01") = p_nSalesAmt
        loDtaRow.Item("nField02") = p_nSCDiscxx
        loDtaRow.Item("nField03") = p_nPWDDiscx
        loDtaRow.Item("nField04") = p_nNonVATxx
        loDtaRow.Item("nField05") = p_nVatDiscx
        loDtaRow.Item("nField06") = p_nVATAmtxx
        loDtaRow.Item("nField07") = p_nNetSalesx  'net sales

        Return loDtaRow
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

