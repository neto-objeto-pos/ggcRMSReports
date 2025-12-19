Imports MySql.Data.MySqlClient
Imports ADODB
Imports ggcAppDriver
Imports CrystalDecisions.CrystalReports.Engine
Imports CrystalDecisions.[Shared].Json

Public Class clsChargeInvoice
    Private Const xsSignature As String = "08220326"

    Private p_oDriver As ggcAppDriver.GRider
    Private p_oSTRept As DataSet
    Private p_oDTSrce As DataTable

    Private p_sMachinex As String
    Private p_nReptType As Integer      '0=Summary;1=Detail
    Private p_sBranchCD As String
    Private p_sTerminal As String

    Private p_dFromDate As Date
    Private p_dThruDate As Date

    Private p_sRiderID As String

    Public Function ReportTrans() As Boolean
        Dim loForm As frmChargeCriteria
        loForm = New frmChargeCriteria
        loForm.GRider = p_oDriver
        loForm.FormType = 0
        loForm.HasSummaryReport = True
        loForm.ShowDialog()

        If Not loForm.isOkey Then
            MsgBox("Unable to generate report.", MsgBoxStyle.Information, "Notice")
            loForm = Nothing
            Return False
        End If

        p_sTerminal = loForm.TerminalNo
        p_sRiderID = loForm.txtField00.Tag
        p_dFromDate = loForm.txtField00.Text
        p_dThruDate = loForm.txtField01.Text
        p_nReptType = loForm.ReportType

        Dim oProg As frmProgress
        oProg = New frmProgress
        oProg.PistonInfo = p_oDriver.AppPath & "/piston.avi"
        oProg.ShowTitle("EXTRACTING RECORDS FROM DATABASE")
        oProg.ShowProcess("Please wait...")
        oProg.Show()

        Dim lsSQL As String

        If p_nReptType = 0 Then ' Summary Report
            lsSQL = "SELECT" &
                    " a.`sTransNox`" &
                    ", a.`sChargeNo`" &
                    ", b.`dTransact`" &
                    ", a.`sClientNm`" &
                    ", a.`sAddressx`" &
                    ", a.`nAmountxx`" &
                    ", b.`sCashierx` " &
                    "FROM Charge_Invoice a " &
                    " LEFT JOIN SO_Master b  ON a.`sSourceNo` = b.`sTransNox`" &
                    "          WHERE b.sTransNox LIKE " & strParm(p_oDriver.BranchCode + p_sTerminal + "%") &
                    "            AND b.dTransact BETWEEN " & dateParm(p_dFromDate) & " AND " & dateParm(p_dThruDate)


        Else 'Detailed Report
            lsSQL = "SELECT" &
                    " a.`sTransNox` " &
                    ", a.`sChargeNo`" &
                    ", b.`dTransact`" &
                    ", a.`sClientNm`" &
                    ", d.`sBarcodex`" &
                    ", d.`sDescript`" &
                    ", e.`sDescript` xCategryNm" &
                    ", c.`nUnitPrce`" &
                    ", c.`nQuantity`" &
                    ",(c.`nUnitPrce` * c.`nQuantity`) xTranTotl" &
                    ", b.`sCashierx` " &
                    "FROM Charge_Invoice a " &
                    "    LEFT JOIN SO_Master b  ON a.`sSourceNo` = b.`sTransNox`" &
                    "    LEFT JOIN SO_Detail c ON b.`sTransNox` = c.`sTransNox`" &
                    "    LEFT JOIN Inventory d ON c.`sStockIDx` = d.`sStockIDx`" &
                    "    LEFT JOIN Product_Category e ON d.`sCategrID` = e.`sCategrCd` " &
                    "        WHERE c.`cReversex` = '+'" &
                    "           AND b.sTransNox LIKE " & strParm(p_oDriver.BranchCode + p_sTerminal + "%") &
                    "            AND b.dTransact BETWEEN " & dateParm(p_dFromDate) & " AND " & dateParm(p_dThruDate)
        End If


        Debug.Print(lsSQL)
        p_oDTSrce = p_oDriver.ExecuteQuery(lsSQL)

        If p_oDTSrce.Rows.Count = 0 Then
            MsgBox("No records found For the given criteria.", MsgBoxStyle.Information, "Notice")
            oProg.Close()
            Return False

        End If

        Dim loDtaTbl As DataTable = getRptTable()
        Dim lnCtr As Integer
        Dim lbAdd As Boolean = False

        oProg.ShowTitle("LOADING RECORDS")
        oProg.MaxValue = p_oDTSrce.Rows.Count


        For lnCtr = 0 To p_oDTSrce.Rows.Count - 1
            oProg.ShowProcess("Loading " & p_oDTSrce(lnCtr).Item("sClientNm") & "...")


            loDtaTbl.Rows.Add(addRow(lnCtr, loDtaTbl))
        Next

        oProg.ShowSuccess()

        Dim clsRpt As clsReports
        clsRpt = New clsReports
        clsRpt.GRider = p_oDriver
        'Set the Report Source Here
        If p_nReptType = 0 Then ' Summary Report    
            If Not clsRpt.initReport("CISum") Then
                Return False
            End If
        Else
            If Not clsRpt.initReport("CIDet") Then
                Return False
            End If
        End If
        Dim loRpt As ReportDocument = clsRpt.ReportSource

        Dim loTxtObj As CrystalDecisions.CrystalReports.Engine.TextObject
        loTxtObj = loRpt.ReportDefinition.Sections(0).ReportObjects("txtCompany")
        loTxtObj.Text = "Los Pedritos"

        'Set Branch Address
        loTxtObj = loRpt.ReportDefinition.Sections(0).ReportObjects("txtAddress")
        loTxtObj.Text = p_oDriver.BranchName & vbCrLf & p_oDriver.Address & vbCrLf & p_oDriver.TownCity & " " & p_oDriver.ZippCode & vbCrLf & p_oDriver.Province

        ''Set First Header
        If p_nReptType = 0 Then ' Summary Report    
            loTxtObj = loRpt.ReportDefinition.Sections(1).ReportObjects("txtHeading1")
            loTxtObj.Text = "Charge Invoice Summarized Report"
        Else
            loTxtObj = loRpt.ReportDefinition.Sections(1).ReportObjects("txtHeading1")
            loTxtObj.Text = "Charge Invoice Detailed Report"
        End If

        'Set Second Header
        loTxtObj = loRpt.ReportDefinition.Sections(1).ReportObjects("txtHeading2")
        loTxtObj.Text = Format(p_dFromDate, xsDATE_LONG) & " to " & Format(p_dThruDate, xsDATE_LONG)

        loTxtObj = loRpt.ReportDefinition.Sections(3).ReportObjects("txtRptUser")
        loTxtObj.Text = Decrypt(p_oDriver.UserName, "08220326")

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

    Private Function addRow(ByVal lnRow As Integer, ByVal foSchemaTable As DataTable) As DataRow
        'ByVal foDTInclue As DataTable
        Dim loDtaRow As DataRow



        'Create row based on the schema of foSchemaTable
        loDtaRow = foSchemaTable.NewRow
        If p_nReptType = 0 Then
            loDtaRow.Item("sField01") = p_oDTSrce(lnRow).Item("sTransNox")
            loDtaRow.Item("sField02") = Format(p_oDTSrce(lnRow).Item("dTransact"), "MMMM dd, yyyy")
            loDtaRow.Item("sField03") = p_oDTSrce(lnRow).Item("sChargeNo")
            loDtaRow.Item("sField04") = p_oDTSrce(lnRow).Item("sClientNm")
            loDtaRow.Item("sField05") = p_oDTSrce(lnRow).Item("sAddressx")
            loDtaRow.Item("sField06") = getCashier(p_oDTSrce(lnRow).Item("sCashierx"))
            loDtaRow.Item("nField01") = CDbl(p_oDTSrce(lnRow).Item("nAmountxx"))
            Return loDtaRow
        Else
            loDtaRow.Item("sField01") = p_oDTSrce(lnRow).Item("sTransNox")
            loDtaRow.Item("sField02") = Format(p_oDTSrce(lnRow).Item("dTransact"), "MMMM dd, yyyy")
            loDtaRow.Item("sField03") = p_oDTSrce(lnRow).Item("sChargeNo")
            loDtaRow.Item("sField04") = p_oDTSrce(lnRow).Item("xCategryNm")
            loDtaRow.Item("sField05") = p_oDTSrce(lnRow).Item("sClientNm")
            loDtaRow.Item("sField06") = p_oDTSrce(lnRow).Item("sBarcodex")
            loDtaRow.Item("sField07") = p_oDTSrce(lnRow).Item("sDescript")
            loDtaRow.Item("lField01") = CDbl(p_oDTSrce(lnRow).Item("nUnitPrce"))
            loDtaRow.Item("lField02") = CDbl(p_oDTSrce(lnRow).Item("nQuantity"))
            loDtaRow.Item("lField03") = CDbl(p_oDTSrce(lnRow).Item("xTranTotl"))
            Return loDtaRow
        End If
    End Function

    Public Function getCashier(ByVal sCashierx As String) As String
        Dim lsSQL As String
        Dim lsCashierNm As String
        Dim loDta As DataTable

        lsSQL = "SELECT" &
                    " a.sUserName" &
                    " FROM xxxSysUser a" &
                    " WHERE a.sUserIDxx = " & strParm(sCashierx)

        loDta = p_oDriver.ExecuteQuery(lsSQL)

        If loDta.Rows.Count = 0 Then
            lsCashierNm = ""
        Else
            lsCashierNm = Decrypt(loDta(0).Item("sUserName"), xsSignature)
        End If

        loDta = Nothing

        Return lsCashierNm
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

    Function TranStatus(ByVal fnStatus As Int32) As String
        If fnStatus = 0 Then
            Return "OPEN"
        ElseIf fnStatus = 1 Then
            Return "APPROVED"
        ElseIf fnStatus = 2 Then
            Return "FULLY PAID"
        ElseIf fnStatus = 3 Then
            Return "DISAPPROVED"
        ElseIf fnStatus = 4 Then
            Return "VOID"
        Else
            Return "UNKNOWN"
        End If
    End Function

    Function DetailTranStatus(ByVal fnStatus As Int32) As String
        If fnStatus = 0 Then
            Return "UNPAID"
        ElseIf fnStatus = 1 Then
            Return "PAID"

        Else
            Return "UNKNOWN"
        End If
    End Function
End Class


