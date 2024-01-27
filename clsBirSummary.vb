Imports MySql.Data.MySqlClient
Imports ADODB
Imports ggcAppDriver
Imports CrystalDecisions.CrystalReports.Engine

Public Class clsBirSummary
    Private p_oDriver As GRider
    Private p_oSTRept As DataSet
    Private p_oDTSrce As DataTable

    Private p_sTerminal As String
    Private p_sMachinex As String
    Private p_sSerial As String
    Private p_sVatReg As String
    Private p_sPermit As String

    Private p_nReptType As Integer      '0=Summary;1=Detail
    Private p_sBranchCD As String

    Private p_dFromDate As Date
    Private p_dThruDate As Date

    Private p_sTranDate As String
    Private p_sORNoFrom As String
    Private p_sORNoThru As String
    Private p_nBegBalxx As Decimal
    Private p_nEndngBal As Decimal
    Private p_nNetTotal As Decimal
    Private p_nSCDiscxx As Decimal
    Private p_nRegularx As Decimal
    Private p_nReturnxx As Decimal
    Private p_nVoidxxxx As Decimal
    Private p_nGrossTtl As Decimal
    Private p_nVATablex As Decimal
    Private p_nVATAmntx As Decimal
    Private p_nVATExmpt As Decimal
    Private p_nZeroRatd As Decimal
    Private p_nZCounter As Integer
    Private p_nSrvcCrge As Double

    Public Function ReportTrans() As Boolean
        Dim loForm As frmSalesCriteria

        loForm = New frmSalesCriteria
        loForm.GRider = p_oDriver
        loForm.ShowDialog()

        If loForm.Cancelled Then
            MsgBox("Unable to generate report.", MsgBoxStyle.Information, "Notice")
            loForm = Nothing
            Return False
        End If

        p_sTerminal = loForm.IDNoxxx
        p_dFromDate = loForm.txtField01.Text
        p_dThruDate = loForm.txtField02.Text

        Dim lsSQL As String 'whole statement
        If p_sTerminal <> "" Then
            lsSQL = "SELECT" & _
                    "  sTranDate" & _
                    ", nSalesAmt" & _
                    ", nVATSales" & _
                    ", nVATAmtxx" & _
                    ", nNonVATxx" & _
                    ", nZeroRatd" & _
                    ", nDiscount" & _
                    ", nVatDiscx" & _
                    ", nPWDDiscx" & _
                    ", nSChargex" & _
                    ", nReturnsx" & _
                    ", nVoidAmnt" & _
                    ", sORNoFrom" & _
                    ", sORNoThru" & _
                    ", dOpenedxx" & _
                    ", dClosedxx" & _
               " FROM Daily_Summary" & _
               " WHERE cTranStat = '2'" & _
                   " AND sCRMNumbr LIKE " & strParm("%" + p_sTerminal.Substring(1)) & _
                   " AND sTranDate BETWEEN " & Replace(dateParm(p_dFromDate), "-", "") & " AND " & Replace(dateParm(p_dThruDate), "-", "") & _
               " ORDER BY sTranDate, dOpenedxx"
        Else
            lsSQL = "SELECT" & _
                         "  sTranDate" & _
                         ", nSalesAmt" & _
                         ", nVATSales" & _
                         ", nVATAmtxx" & _
                         ", nNonVATxx" & _
                         ", nZeroRatd" & _
                         ", nDiscount" & _
                         ", nVatDiscx" & _
                         ", nPWDDiscx" & _
                         ", nSChargex" & _
                         ", nReturnsx" & _
                         ", nVoidAmnt" & _
                         ", sORNoFrom" & _
                         ", sORNoThru" & _
                         ", dOpenedxx" & _
                         ", dClosedxx" & _
                    " FROM Daily_Summary" & _
                    " WHERE cTranStat = '2'" & _
                        " AND sCRMNumbr = " & strParm(p_sMachinex) & _
                        " AND sTranDate BETWEEN " & Replace(dateParm(p_dFromDate), "-", "") & " AND " & Replace(dateParm(p_dThruDate), "-", "") & _
                    " ORDER BY sTranDate, dOpenedxx"
        End If

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

        lsSQL = "SELECT dOpenedxx, dClosedxx, nAccuSale FROM Daily_Summary" & _
                    " WHERE dClosedxx < " & datetimeParm(p_oDTSrce(0).Item("dOpenedxx")) & _
                    " ORDER BY dOpenedxx DESC LIMIT 1"
        Dim loDta As DataTable
        loDta = p_oDriver.ExecuteQuery(lsSQL)

        If loDta.Rows.Count = 0 Then
            p_nBegBalxx = 0
            p_nEndngBal = 0
        Else
            p_nBegBalxx = loDta(0)("nAccuSale")
            p_nEndngBal = loDta(0)("nAccuSale")
        End If
        loDta = Nothing

        p_sTranDate = p_oDTSrce(0).Item("sTranDate")
        p_sORNoFrom = p_oDTSrce(0).Item("sORNoFrom")
        p_sORNoThru = p_oDTSrce(0).Item("sORNoThru")
        For lnCtr = 0 To p_oDTSrce.Rows.Count - 1
            oProg.ShowProcess("Loading " & p_oDTSrce(lnCtr).Item("sTranDate") & "...")

            If p_sTranDate = p_oDTSrce(lnCtr).Item("sTranDate") Then
                If p_sORNoThru < p_oDTSrce(lnCtr).Item("sORNoThru") Then
                    p_sORNoThru = p_oDTSrce(lnCtr).Item("sORNoThru")
                End If

                p_nNetTotal += p_oDTSrce(lnCtr).Item("nSalesAmt") - (p_oDTSrce(lnCtr).Item("nPWDDiscx") + p_oDTSrce(lnCtr).Item("nDiscount") + p_oDTSrce(lnCtr).Item("nVatDiscx"))
                p_nSCDiscxx += p_oDTSrce(lnCtr).Item("nPWDDiscx") + p_oDTSrce(lnCtr).Item("nVatDiscx")
                p_nRegularx += p_oDTSrce(lnCtr).Item("nDiscount")
                p_nEndngBal += p_oDTSrce(lnCtr).Item("nSalesAmt") - (p_oDTSrce(lnCtr).Item("nPWDDiscx") + p_oDTSrce(lnCtr).Item("nDiscount") + p_oDTSrce(lnCtr).Item("nVatDiscx"))
                p_nReturnxx += p_oDTSrce(lnCtr).Item("nReturnsx")
                p_nVoidxxxx += p_oDTSrce(lnCtr).Item("nVoidAmnt")
                'p_nGrossTtl += p_oDTSrce(lnCtr).Item("nSalesAmt") + p_oDTSrce(lnCtr).Item("nReturnsx") + p_oDTSrce(lnCtr).Item("nVoidAmnt")
                p_nGrossTtl += p_oDTSrce(lnCtr).Item("nSalesAmt") + p_oDTSrce(lnCtr).Item("nReturnsx") + p_oDTSrce(lnCtr).Item("nSChargex")
                p_nSrvcCrge += p_oDTSrce(lnCtr).Item("nSChargex")
                p_nVATablex += p_oDTSrce(lnCtr).Item("nVATSales")
                p_nVATAmntx += p_oDTSrce(lnCtr).Item("nVATAmtxx")
                p_nVATExmpt += p_oDTSrce(lnCtr).Item("nNonVATxx")
                p_nZeroRatd += p_oDTSrce(lnCtr).Item("nZeroRatd")
            Else
                p_sORNoFrom = p_oDTSrce(lnCtr).Item("sORNoFrom")
                p_sORNoThru = p_oDTSrce(lnCtr).Item("sORNoThru")

                p_nNetTotal = p_oDTSrce(lnCtr).Item("nSalesAmt") - (p_oDTSrce(lnCtr).Item("nPWDDiscx") + p_oDTSrce(lnCtr).Item("nDiscount") + p_oDTSrce(lnCtr).Item("nVatDiscx"))
                p_nSCDiscxx = p_oDTSrce(lnCtr).Item("nPWDDiscx") + p_oDTSrce(lnCtr).Item("nVatDiscx")
                p_nRegularx = p_oDTSrce(lnCtr).Item("nDiscount")
                p_nBegBalxx = p_nEndngBal
                p_nEndngBal = p_oDTSrce(lnCtr).Item("nSalesAmt") - (p_oDTSrce(lnCtr).Item("nPWDDiscx") + p_oDTSrce(lnCtr).Item("nDiscount") + p_oDTSrce(lnCtr).Item("nVatDiscx"))
                p_nReturnxx = p_oDTSrce(lnCtr).Item("nReturnsx")
                p_nVoidxxxx = p_oDTSrce(lnCtr).Item("nVoidAmnt")
                'p_nGrossTtl = p_oDTSrce(lnCtr).Item("nSalesAmt") + p_oDTSrce(lnCtr).Item("nReturnsx") + p_oDTSrce(lnCtr).Item("nVoidAmnt")
                p_nGrossTtl = p_oDTSrce(lnCtr).Item("nSalesAmt") + p_oDTSrce(lnCtr).Item("nReturnsx") + p_oDTSrce(lnCtr).Item("nSChargex")
                p_nSrvcCrge = p_oDTSrce(lnCtr).Item("nSChargex")
                p_nVATablex = p_oDTSrce(lnCtr).Item("nVATSales")
                p_nVATAmntx = p_oDTSrce(lnCtr).Item("nVATAmtxx")
                p_nVATExmpt = p_oDTSrce(lnCtr).Item("nNonVATxx")
                p_nZeroRatd = p_oDTSrce(lnCtr).Item("nZeroRatd")

                p_sTranDate = p_oDTSrce(lnCtr).Item("sTranDate")
            End If

            If lnCtr <> p_oDTSrce.Rows.Count - 1 Then
                If p_sTranDate <> p_oDTSrce(lnCtr + 1).Item("sTranDate") Then
                    p_nZCounter += 1
                    p_nEndngBal = p_nEndngBal + p_nBegBalxx
                    loDtaTbl.Rows.Add(addRow(loDtaTbl))
                End If
            Else
                p_nZCounter += 1
                p_nEndngBal = p_nEndngBal + p_nBegBalxx
                loDtaTbl.Rows.Add(addRow(loDtaTbl))
            End If
        Next

        oProg.ShowSuccess()

        Dim clsRpt As clsReports
        clsRpt = New clsReports
        clsRpt.GRider = p_oDriver
        'Set the Report Source Here
        If Not clsRpt.initReport("BIRSl") Then
            Return False
        End If

        Dim loRpt As ReportDocument = clsRpt.ReportSource

        Dim loTxtObj As CrystalDecisions.CrystalReports.Engine.TextObject
        loTxtObj = loRpt.ReportDefinition.Sections(0).ReportObjects("txtCompany")
        'loTxtObj.Text = p_oDriver.BranchName
        loTxtObj.Text = "Meet 'n' Eat"


        'Set Branch Address
        loTxtObj = loRpt.ReportDefinition.Sections(0).ReportObjects("txtAddress")
        loTxtObj.Text = p_oDriver.Address & ", " & p_oDriver.TownCity & " " & p_oDriver.ZippCode & ", " & p_oDriver.Province

        'Set First Header
        loTxtObj = loRpt.ReportDefinition.Sections(1).ReportObjects("txtHeading1")
        loTxtObj.Text = "BIR SALES SUMMARY REPORT"

        'Set Second Header
        loTxtObj = loRpt.ReportDefinition.Sections(1).ReportObjects("txtHeading2")
        loTxtObj.Text = Format(p_dFromDate, xsDATE_LONG) & " to " & Format(p_dThruDate, xsDATE_LONG)

        loTxtObj = loRpt.ReportDefinition.Sections(3).ReportObjects("txtRptUser")
        loTxtObj.Text = Decrypt(p_oDriver.UserName, "08220326")

        'POS Info
        loTxtObj = loRpt.ReportDefinition.Sections(1).ReportObjects("txtVATReg")
        loTxtObj.Text = p_sVatReg

        loTxtObj = loRpt.ReportDefinition.Sections(1).ReportObjects("txtDateTime")
        loTxtObj.Text = p_oDriver.getSysDate

        loTxtObj = loRpt.ReportDefinition.Sections(1).ReportObjects("txtTerminal")
        loTxtObj.Text = p_sSerial

        loTxtObj = loRpt.ReportDefinition.Sections(3).ReportObjects("txtMIN")
        loTxtObj.Text = p_sMachinex

        loTxtObj = loRpt.ReportDefinition.Sections(3).ReportObjects("txtUserID")
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

    Private Function addRow(ByVal foSchemaTable As DataTable) As DataRow
        'ByVal foDTInclue As DataTable
        Dim loDtaRow As DataRow

        'Create row based on the schema of foSchemaTable
        loDtaRow = foSchemaTable.NewRow

        loDtaRow.Item("sField01") = CInt(p_sTranDate).ToString("####-##-##")
        loDtaRow.Item("sField02") = p_sORNoFrom
        loDtaRow.Item("sField03") = p_sORNoThru
        loDtaRow.Item("lField01") = p_nBegBalxx
        loDtaRow.Item("lField02") = p_nEndngBal
        loDtaRow.Item("lField03") = p_nNetTotal
        loDtaRow.Item("lField04") = p_nSCDiscxx
        loDtaRow.Item("lField05") = p_nRegularx
        loDtaRow.Item("lField06") = p_nReturnxx
        'loDtaRow.Item("lField07") = p_nVoidxxxx
        loDtaRow.Item("lField08") = p_nGrossTtl
        loDtaRow.Item("lField09") = p_nVATablex
        loDtaRow.Item("lField10") = p_nVATAmntx
        loDtaRow.Item("lField11") = p_nVATExmpt
        loDtaRow.Item("lField12") = p_nZeroRatd
        loDtaRow.Item("lField13") = 0
        loDtaRow.Item("lField14") = 0
        loDtaRow.Item("lField15") = p_nNetTotal
        loDtaRow.Item("lField16") = p_nSCDiscxx + p_nRegularx 'total deductions
        loDtaRow.Item("lField17") = p_nSCDiscxx - (p_nSCDiscxx / 1.12) ' vat on special discount
        loDtaRow.Item("lField18") = 0
        loDtaRow.Item("lField19") = 0
        loDtaRow.Item("nField05") = 0
        loDtaRow.Item("nField02") = p_nZCounter
        loDtaRow.Item("nField03") = 0
        loDtaRow.Item("nField06") = 0
        loDtaRow.Item("nField04") = p_nGrossTtl
        loDtaRow.Item("lField20") = p_nSrvcCrge
        loDtaRow.Item("sField04") = ""
        Return loDtaRow
    End Function

    Public Sub New(ByVal foRider As GRider)
        p_oDriver = foRider
        p_oSTRept = Nothing
        p_oDTSrce = Nothing
    End Sub

    Public Sub New(ByVal foRider As GRider, _
                   ByVal foMachineNo As String, _
                   ByVal foSerialNo As String, _
                   ByVal foPermitNo As String, _
                   ByVal foVATReg As String)
        p_oDriver = foRider
        p_oSTRept = Nothing
        p_oDTSrce = Nothing

        p_sMachinex = foMachineNo
        p_sSerial = foSerialNo
        p_sPermit = foPermitNo
        p_sVatReg = foVATReg
    End Sub
End Class

