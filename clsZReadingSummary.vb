

Imports MySql.Data.MySqlClient
Imports ADODB
Imports ggcAppDriver
Imports CrystalDecisions.CrystalReports.Engine
Imports Microsoft.VisualBasic.Devices
Imports System.Reflection

Public Class clsZReadingSummary
    Private p_oDriver As GRider

    Private p_oDTMaster As DataTable

    Private p_sPOSNo As String      'MIN:       14121419321782091
    Private p_sVATReg As String     'TIN:       941-184-389-000
    Private p_sCompny As String     'Company  : MONARK HOTEL

    Private p_sPermit As String     'Permit No: PR122014-004-D004507-000
    Private p_sSerial As String     'Serial No: L9GF261769
    Private p_sAccrdt As String     'Accrdt No: 038-227471337-000028
    Private p_sTermnl As String     'Termnl No: 02
    Private p_nZRdCtr As Integer
    Private p_bWasRLCClt As Boolean


    Private p_oSTRept As DataSet
    Private p_oDTSrce As DataTable

    'maynard 10-02-2024
    Private psCashierNm As String

    Private p_bBackEnd As Boolean = False

    Private Const pxeLFTMGN As Integer = 3

    Private Const p_sMasTable As String = "Daily_Summary"
    Private Const p_sMsgHeadr As String = "Daily Summary"

    Private p_dFromDate As Date
    Private p_dThruDate As Date
    Private p_sTerminal As String


    Dim lnOpenBalx As Decimal = 0
    Dim lnCPullOut As Decimal = 0

    Dim lnCashAmnt As Decimal = 0
    Dim lnSChargex As Decimal = 0
    Dim lnChckAmnt As Decimal = 0
    Dim lnCrdtAmnt As Decimal = 0
    Dim lnChrgAmnt As Decimal = 0
    Dim lnGiftAmnt As Decimal = 0

    Dim lnSalesAmt As Decimal = 0
    Dim lnVATSales As Decimal = 0
    Dim lnVATAmtxx As Decimal = 0
    Dim lnZeroRatd As Decimal = 0
    Dim lnNonVATxx As Decimal = 0   'Non-Vat means Vat Exempt
    Dim lnDiscount As Decimal = 0   'Regular Discount
    Dim lnVatDiscx As Decimal = 0   '12% VAT Discount
    Dim lnPWDDiscx As Decimal = 0   'Senior/PWD Discount

    Dim lnReturnsx As Decimal = 0   'Returns
    Dim lnVoidAmnt As Decimal = 0   'Void Transactions
    Dim lnVoidCntx As Integer = 0

    Dim lnPrevSale As Decimal = 0
    Dim lsBegginingSI As String = ""
    Dim lsEndingSI As String = ""



    Public Function ReportTrans() As Boolean
        Dim lsSQL As String
        Dim lnCtr As Integer
        Dim loForm As frmSalesCriteria

        loForm = New frmSalesCriteria
        loForm.GRider = p_oDriver
        loForm.ShowDialog()

        If loForm.Cancelled Then
            MsgBox("Unable to generate report.", MsgBoxStyle.Information, "Notice")
            loForm = Nothing
            Return False
        End If

        p_sPOSNo = loForm.IDNoxxx
        p_sTermnl = loForm.TerminalNo
        p_dFromDate = loForm.txtField01.Text
        p_dThruDate = loForm.txtField02.Text


        'Get configuration of machine
        If Not initMachine() Then
            Return False
        End If

        lsSQL = "SELECT sTranDate, nZReadCtr FROM Daily_Summary" &
                        " WHERE sTranDate BETWEEN " & Replace(dateParm(p_dFromDate), "-", "") & " AND " & Replace(dateParm(p_dThruDate), "-", "") &
                            " AND sCRMNumbr = " & strParm(p_sPOSNo) &
                            " AND cTranStat IN ('2')" &
                       " ORDER BY sTranDate"
        Debug.Print(lsSQL)
        p_oDTSrce = p_oDriver.ExecuteQuery(lsSQL)

        If p_oDTSrce.Rows.Count = 0 Then
            MsgBox("No records found for the given criteria.", MsgBoxStyle.Information, "Notice")
            Exit Function
        End If


        Dim oProg As frmProgress
        oProg = New frmProgress
        oProg.PistonInfo = p_oDriver.AppPath & "/piston.avi"
        oProg.ShowTitle("EXTRACTING RECORDS FROM DATABASE")
        oProg.ShowProcess("Please wait...")
        oProg.Show()

        Dim loDtaTbl As DataTable = getRptTable()
        Dim lbAdd As Boolean = False

        oProg.ShowTitle("LOADING RECORDS")
        oProg.MaxValue = p_oDTSrce.Rows.Count




        For lnCtr = 0 To p_oDTSrce.Rows.Count - 1
            oProg.ShowProcess("Loading " & p_oDTSrce(lnCtr).Item("sTranDate") & "...")
            If PerformZReading(p_oDTSrce.Rows(lnCtr)("sTranDate"),
                                                    p_oDTSrce.Rows(lnCtr)("sTranDate"),
                                                    Environment.GetEnvironmentVariable("RMS-CRM-No"),
                                                    True, p_oDTSrce.Rows(lnCtr)("nZReadCtr")) Then

            End If
        Next



        lsSQL = "SELECT nAccuSale FROM Daily_Summary" &
                " WHERE sTranDate < " & Replace(dateParm(p_dFromDate), "-", "") &
                    " AND sCRMNumbr = " & strParm(p_sPOSNo) &
                    " AND cTranStat IN ('1', '2')" &
                " ORDER BY dClosedxx DESC LIMIT 1"

        Dim loDT As DataTable
        loDT = p_oDriver.ExecuteQuery(lsSQL)
        If loDT.Rows.Count = 0 Then
            lnPrevSale = 0
        Else
            lnPrevSale = loDT(0)("nAccuSale")
        End If


        oProg.ShowSuccess()




        Dim clsRpt As clsReports
        clsRpt = New clsReports
        clsRpt.GRider = p_oDriver
        'Set the Report Source Here
        If Not clsRpt.initReport("ZRead") Then
            Return False
        End If

        Dim loRpt As ReportDocument = clsRpt.ReportSource

        Dim loTxtObj As CrystalDecisions.CrystalReports.Engine.TextObject
        'Set POS Info Report Information
        loTxtObj = loRpt.ReportDefinition.Sections(0).ReportObjects("txtCompany")
        loTxtObj.Text = "The Monarch Hospitality & Tourism Corp."

        loTxtObj = loRpt.ReportDefinition.Sections(0).ReportObjects("txtBranch")
        loTxtObj.Text = p_oDriver.BranchName

        loTxtObj = loRpt.ReportDefinition.Sections(0).ReportObjects("txtAddress")
        loTxtObj.Text = p_oDriver.Address & ", " & p_oDriver.TownCity & " " & p_oDriver.ZippCode & ", " & p_oDriver.Province

        loTxtObj = loRpt.ReportDefinition.Sections(0).ReportObjects("txtVatRegTin")
        loTxtObj.Text = "VAT REG TIN: " + p_sVATReg

        loTxtObj = loRpt.ReportDefinition.Sections(0).ReportObjects("txtMIN")
        loTxtObj.Text = "MIN : " + p_sPOSNo

        loTxtObj = loRpt.ReportDefinition.Sections(0).ReportObjects("txtPTUNo")
        loTxtObj.Text = "PTU No. : " + p_sPermit

        loTxtObj = loRpt.ReportDefinition.Sections(0).ReportObjects("txtSerialNo")
        loTxtObj.Text = "Serial No. : " + p_sSerial

        'Set Title
        loTxtObj = loRpt.ReportDefinition.Sections(0).ReportObjects("txtHeading1")
        loTxtObj.Text = "Z - READING"

        'Set Date
        loTxtObj = loRpt.ReportDefinition.Sections(1).ReportObjects("txtHeading2")
        loTxtObj.Text = Format(p_dFromDate, xsDATE_MEDIUM) & " to " & Format(p_dThruDate, xsDATE_MEDIUM)
        'Set Terminal No
        loTxtObj = loRpt.ReportDefinition.Sections(1).ReportObjects("txtHeading3")
        loTxtObj.Text = p_sTermnl

        loTxtObj = loRpt.ReportDefinition.Sections(1).ReportObjects("txtField01") 'beginning si
        loTxtObj.Text = lsBegginingSI

        loTxtObj = loRpt.ReportDefinition.Sections(1).ReportObjects("txtField02") 'ending si
        loTxtObj.Text = lsEndingSI

        loTxtObj = loRpt.ReportDefinition.Sections(1).ReportObjects("txtField03") 'beginning balance
        loTxtObj.Text = Format(lnPrevSale, xsDECIMAL)

        loTxtObj = loRpt.ReportDefinition.Sections(1).ReportObjects("txtField04") 'ending balance
        loTxtObj.Text = Format(lnPrevSale + ((lnSalesAmt) - (lnDiscount + lnPWDDiscx + lnVatDiscx)), xsDECIMAL)

        loTxtObj = loRpt.ReportDefinition.Sections(1).ReportObjects("txtField05") 'gross sales
        loTxtObj.Text = Format((lnSalesAmt + lnSChargex + lnDiscount + lnPWDDiscx + lnReturnsx) - lnVatDiscx, xsDECIMAL)
        loTxtObj = loRpt.ReportDefinition.Sections(1).ReportObjects("txtField06") 'service charge
        loTxtObj.Text = Format(lnSChargex, xsDECIMAL)
        loTxtObj = loRpt.ReportDefinition.Sections(1).ReportObjects("txtField07") 'regular disc
        loTxtObj.Text = Format(lnDiscount, xsDECIMAL)
        'loTxtObj = loRpt.ReportDefinition.Sections(1).ReportObjects("txtField08") 'vat sc/pwd
        'loTxtObj.Text = p_sVATReg
        loTxtObj = loRpt.ReportDefinition.Sections(1).ReportObjects("txtField09") '20% sc/pwd disc
        loTxtObj.Text = Format(lnPWDDiscx, xsDECIMAL)
        loTxtObj = loRpt.ReportDefinition.Sections(1).ReportObjects("txtField10") ' return
        loTxtObj.Text = Format(lnReturnsx, xsDECIMAL)

        loTxtObj = loRpt.ReportDefinition.Sections(1).ReportObjects("txtField11") 'net sales
        loTxtObj.Text = Format(lnSalesAmt - lnVatDiscx, xsDECIMAL)

        loTxtObj = loRpt.ReportDefinition.Sections(1).ReportObjects("txtField12") 'vat sales
        loTxtObj.Text = Format(lnVATSales, xsDECIMAL)
        loTxtObj = loRpt.ReportDefinition.Sections(1).ReportObjects("txtField13")  'vat amount
        loTxtObj.Text = Format(lnVATAmtxx - lnVatDiscx, xsDECIMAL)
        loTxtObj = loRpt.ReportDefinition.Sections(1).ReportObjects("txtField14") 'vat exempt sales
        loTxtObj.Text = Format(lnNonVATxx, xsDECIMAL)
        loTxtObj = loRpt.ReportDefinition.Sections(1).ReportObjects("txtField15") 'zero rated sales
        loTxtObj.Text = Format(lnZeroRatd, xsDECIMAL)
        'collection info
        loTxtObj = loRpt.ReportDefinition.Sections(1).ReportObjects("txtField16") 'petty cash
        loTxtObj.Text = Format(lnOpenBalx, xsDECIMAL)
        loTxtObj = loRpt.ReportDefinition.Sections(1).ReportObjects("txtField17") 'withdrawal
        loTxtObj.Text = Format(lnCPullOut, xsDECIMAL)

        loTxtObj = loRpt.ReportDefinition.Sections(1).ReportObjects("txtField18") 'cash
        loTxtObj.Text = Format((lnCashAmnt), xsDECIMAL)
        loTxtObj = loRpt.ReportDefinition.Sections(1).ReportObjects("txtField19") 'cheque
        loTxtObj.Text = Format(lnChckAmnt, xsDECIMAL)
        loTxtObj = loRpt.ReportDefinition.Sections(1).ReportObjects("txtField20") 'credit card
        loTxtObj.Text = Format(lnCrdtAmnt, xsDECIMAL)
        loTxtObj = loRpt.ReportDefinition.Sections(1).ReportObjects("txtField21") 'gift cheque
        loTxtObj.Text = Format(lnGiftAmnt, xsDECIMAL)
        loTxtObj = loRpt.ReportDefinition.Sections(1).ReportObjects("txtField22") 'z-counter
        loTxtObj.Text = p_nZRdCtr.ToString
        loTxtObj = loRpt.ReportDefinition.Sections(1).ReportObjects("txtField23") 'void si count
        loTxtObj.Text = Format(lnVoidCntx, xsINTEGER)
        loTxtObj = loRpt.ReportDefinition.Sections(1).ReportObjects("txtField24") 'void si-amount
        loTxtObj.Text = Format(lnVoidAmnt, xsDECIMAL)


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

        Return loDtaRow
    End Function
    Private Function getSQ_Master() As String
        Return "SELECT a.sTranDate" &
                    ", a.sCRMNumbr" &
                    ", a.sCashierx" &
                    ", a.nOpenBalx" &
                    ", a.nCPullOut" &
                    ", a.nSalesAmt" &
                    ", a.nVATSales" &
                    ", a.nVATAmtxx" &
                    ", a.nNonVATxx" &
                    ", a.nZeroRatd" &
                    ", a.nDiscount" &
                    ", a.nPWDDiscx" &
                    ", a.nVatDiscx" &
                    ", a.nReturnsx" &
                    ", a.nVoidAmnt" &
                    ", a.nAccuSale" &
                    ", a.nCashAmnt" &
                    ", a.nChckAmnt" &
                    ", a.nCrdtAmnt" &
                    ", a.nChrgAmnt" &
                    ", a.nSChargex" &
                    ", a.sORNoFrom" &
                    ", a.sORNoThru" &
                    ", a.nZReadCtr" &
                    ", a.nGiftAmnt" &
                    ", a.cTranStat" &
                    ", a.nVoidCntx" &
                " FROM " & p_sMasTable & " a" &
                " ORDER BY sTranDate ASC"

    End Function

    Private Function initMachine() As Boolean
        If p_sPOSNo = "" Then
            MsgBox("Invalid Machine Identification Info Detected...")
            Return False
        End If

        Dim lsSQL As String
        lsSQL = "SELECT" &
                       "  sAccredtn" &
                       ", sPermitNo" &
                       ", sSerialNo" &
                       ", nPOSNumbr" &
                       ", nZReadCtr" &
                       ", cRLCPOSxx" &
               " FROM Cash_Reg_Machine" &
               " WHERE sIDNumber = " & strParm(p_sPOSNo)

        Dim loDta As DataTable
        loDta = p_oDriver.ExecuteQuery(lsSQL)

        If loDta.Rows.Count <> 1 Then
            MsgBox("Invalid Config for MIN Detected...")
            Return False
        End If

        p_sAccrdt = loDta(0).Item("sAccredtn")
        p_sPermit = loDta(0).Item("sPermitNo")
        p_sSerial = loDta(0).Item("sSerialNo")
        p_sTermnl = loDta(0).Item("nPOSNumbr")
        p_nZRdCtr = loDta(0).Item("nZReadCtr") + 1
        p_sVATReg = Environment.GetEnvironmentVariable("REG-TIN-No")
        Return True
    End Function
    Public Function PerformZReading(ByVal sFromDate As String,
                                   ByVal sThruDate As String,
                                   ByVal sCRMNumbr As String,
                                   ByVal bBackendx As Boolean,
                                   Optional nZReadCtr As Integer = 0) As Boolean

        p_nZRdCtr = nZReadCtr

        'print daily sales
        If Not ComputeZReading(sFromDate, sThruDate, sCRMNumbr) Then
            'MsgBox("Unable to perform Terminal Z Reading!!", , p_sMsgHeadr)
            Return False
        Else
            'Update the reset counter(nZReadCtr) at the Cash_Reg_Machine table
            Dim lsSQL As String
            lsSQL = "UPDATE Cash_Reg_Machine" &
                    " SET nZReadCtr = " & p_nZRdCtr &
                        ", nEODCtrxx = nEODCtrxx + 1" &
                    " WHERE sIDNumber = " & strParm(sCRMNumbr)
            p_oDriver.Execute(lsSQL, "Cash_Reg_Machine")

            lsSQL = "UPDATE Daily_Summary" &
                    " SET cTranStat = '2'" &
                        ", nZReadCtr = nZReadCtr + 1" &
                    " WHERE sCRMNumbr = " & strParm(sCRMNumbr) &
                        " AND sTranDate BETWEEN " & strParm(sFromDate) & " AND " & strParm(sThruDate)

            p_oDriver.Execute(lsSQL, "Daily_Summary")

            lsSQL = "UPDATE Table_Master" &
                               " SET cStatusxx = '0'" &
                                    ", dReserved = NULL" &
                                    ", nOccupnts = 0"
            p_oDriver.Execute(lsSQL, "Table_Master")



            'MsgBox("Z-Reading was perform successfully!!", , p_sMsgHeadr)
            p_oDriver.SaveEvent("0022", "Date: " & sFromDate & " to " & sThruDate, p_sTermnl)

        End If

        Return True
    End Function
    Private Function ComputeZReading(ByVal sFromDate As String, ByVal sThruDate As String, ByVal sCRMNumbr As String) As Boolean
        Dim lsSQL As String
        lsSQL = AddCondition(getSQ_Master, "sTranDate BETWEEN " & strParm(sFromDate) & " AND " & strParm(sThruDate) &
                                      " AND sCRMNumbr = " & strParm(sCRMNumbr) &
                                      " AND cTranStat IN ('1', '2')")
        Debug.Print(lsSQL)
        Dim loDta As DataTable
        loDta = p_oDriver.ExecuteQuery(lsSQL)

        If loDta.Rows.Count = 0 Then
            MsgBox("There are no transaction for this date....", , p_sMsgHeadr)
            Return False
        End If

        Dim lsORNoFrom As String = loDta(0).Item("sORNoFrom")
        If lsBegginingSI = "" Then
            lsBegginingSI = loDta(0).Item("sORNoFrom")

        End If
        'lsBegginingSI = "0"
        For lnCtr = 0 To loDta.Rows.Count - 1
            'Determing Beginning SI for this Terminal X Reading
            If lsBegginingSI > lsORNoFrom And loDta(lnCtr).Item("sORNoFrom") <> "" Then
                lsBegginingSI = loDta(lnCtr).Item("sORNoFrom")
            End If
            'Determing Ending SI for this Terminal X Reading

            If loDta(lnCtr).Item("sORNoThru") > lsEndingSI Then
                lsEndingSI = loDta(lnCtr).Item("sORNoThru")
            End If

            'Compute Gross Sales
            Debug.Print(loDta(lnCtr).Item("nSalesAmt"))
            lnSalesAmt = lnSalesAmt + loDta(lnCtr).Item("nSalesAmt")
            'Compute VAT Related Sales
            lnVATSales = lnVATSales + loDta(lnCtr).Item("nVATSales")
            lnVATAmtxx = lnVATAmtxx + loDta(lnCtr).Item("nVATAmtxx")
            lnZeroRatd = lnZeroRatd + loDta(lnCtr).Item("nZeroRatd")
            'Compute Discounts
            lnDiscount = lnDiscount + loDta(lnCtr).Item("nDiscount")
            lnVatDiscx = lnVatDiscx + loDta(lnCtr).Item("nVatDiscx")
            lnPWDDiscx = lnPWDDiscx + loDta(lnCtr).Item("nPWDDiscx")
            'Compute Returns/Refunds/Void Transactions
            lnReturnsx = lnReturnsx + loDta(lnCtr).Item("nReturnsx")
            lnVoidAmnt = lnVoidAmnt + loDta(lnCtr).Item("nVoidAmnt")
            lnVoidCntx = lnVoidCntx + loDta(lnCtr).Item("nVoidCntx")
            'Compute Cashier Collection Info
            lnOpenBalx = lnOpenBalx + loDta(lnCtr).Item("nOpenBalx")
            lnCPullOut = lnCPullOut + loDta(lnCtr).Item("nCPullOut")
            lnCashAmnt = lnCashAmnt + loDta(lnCtr).Item("nCashAmnt")
            lnSChargex = lnSChargex + loDta(lnCtr).Item("nSChargex")
            lnChckAmnt = lnChckAmnt + loDta(lnCtr).Item("nChckAmnt")
            lnCrdtAmnt = lnCrdtAmnt + loDta(lnCtr).Item("nCrdtAmnt")
            lnChrgAmnt = lnChrgAmnt + loDta(lnCtr).Item("nChrgAmnt")
            lnGiftAmnt = lnGiftAmnt + loDta(lnCtr).Item("nGiftAmnt")
            lnNonVATxx = lnNonVATxx + loDta(lnCtr).Item("nNonVATxx")
        Next





        Return True
    End Function
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

        p_sTerminal = foMachineNo
        p_sSerial = foSerialNo
        p_sPermit = foPermitNo
        p_sVATReg = foVATReg


    End Sub
End Class

