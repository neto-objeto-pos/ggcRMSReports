Imports MySql.Data.MySqlClient
Imports ADODB
Imports ggcAppDriver
Imports CrystalDecisions.CrystalReports.Engine

Public Class clsSalesReport
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

    Dim p_nSubTotl As Decimal
    Dim p_nDiscTtl As Decimal
    Dim p_nNetTotl As Decimal
    Dim p_nVATSale As Decimal
    Dim p_nVATAmtx As Decimal
    Dim p_nVATExmp As Decimal
    Dim p_nZeroRtd As Decimal
    Dim p_nSrvCrge As Decimal

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

        p_sTerminal = loForm.TerminalNo
        p_dFromDate = loForm.txtField01.Text
        p_dThruDate = loForm.txtField02.Text

        Dim oProg As frmProgress
        oProg = New frmProgress
        oProg.PistonInfo = p_oDriver.AppPath & "/piston.avi"
        oProg.ShowTitle("EXTRACTING RECORDS FROM DATABASE")
        oProg.ShowProcess("Please wait...")
        oProg.Show()

        Dim lsSQL As String 'whole statement

        lsSQL = "SELECT" &
                    " a.sTransNox      `sTransNox`," &
                    " a.dTransact      `dTransact`," &
                    " a.cTranStat       `cTranStat`," &
                    " CONCAT('OR', a.sORNumber)      `sInvceNox`," &
                    " a.nSalesAmt      `nNetAmntx`," &
                    " a.nVATSales      `nVATSales`," &
                    " a.nVATAmtxx      `nVATAmtxx`," &
                    " a.nZeroRatd      `nZeroRatd`," &
                    " a.nDiscount      `nDiscount`," &
                    " a.nVatDiscx      `nVatDiscx`," &
                    " a.nPWDDiscx      `nPWDDiscx`," &
                    " a.nTendered      `nTendered`," &
                    " a.nCashAmtx      `nCashAmtx`," &
                    " a.sSourceCd      `sSourceCd`," &
                    " a.sSourceNo      `sSourceNo`," &
                    " a.nSChargex      `nSChargex`," &
                    " b.sTableNox      `sTableNox`," &
                    " g.sUserName      `sCashierx`," &
                    " c.nEntryNox      `nEntryNox`," &
                    " c.sStockIDx      `sStockIDx`," &
                    " d.sBriefDsc      `sBriefDsc`," &
                    " c.cReversex      `cReversex`," &
                    " c.nQuantity      `nQuantity`," &
                    " c.nUnitPrce      `nUnitPrce`," &
                    " c.nComplmnt      `nComplmnt`," &
                    " c.cDetailxx      `cDetailxx`," &
                    " 'Sales Order'    `xRemarksx`," &
                    " f.sCardDesc      `sCardDesc`," &
                    " e.nNoClient      `nNoClient`," &
                    " e.nWithDisc      `nWithDisc`," &
                    " e.nAddDiscx      `nAddDiscx`," &
                    " e.nDiscRate      `nDiscRate`" &
                " FROM Receipt_Master a" &
                        " LEFT JOIN xxxSysUser g" &
                            " ON a.sCashierx = g.sUserIDxx," &
                    " SO_Master b" &
                        " LEFT JOIN SO_Detail c" &
                            " ON b.sTransNox = c.sTransNox" &
                        " LEFT JOIN Inventory d" &
                            " ON c.sStockIDx = d.sStockIDx" &
                        " LEFT JOIN Discount e" &
                            " ON e.sSourceCd = 'SO'" &
                            " AND b.sTransNox = e.sSourceNo" &
                        " LEFT JOIN Discount_Card f" &
                            " ON e.sDiscCard = f.sCardIDxx" &
                " WHERE a.sSourceNo = b.sTransNox" &
                    " AND a.sSourceCd = 'SO'" &
                    " AND c.cReversed <> '1'" &
                    " AND a.sTransNox LIKE " & strParm(p_oDriver.BranchCode + p_sTerminal + "%") &
                    " AND a.dTransact BETWEEN " & dateParm(p_dFromDate) & " AND " & dateParm(p_dThruDate) &
                " UNION SELECT" &
                    " a.sTransNox       `sTransNox`," &
                    " a.dTransact       `dTransact`," &
                    " a.cTranStat       `cTranStat`," &
                    " CONCAT('OR', a.sORNumber)       `sInvceNox`," &
                    " a.nSalesAmt       `nNetAmntx`," &
                    " a.nVATSales       `nVATSales`," &
                    " a.nVATAmtxx       `nVATAmtxx`," &
                    " a.nZeroRatd       `nZeroRatd`," &
                    " a.nDiscount       `nDiscount`," &
                    " a.nVatDiscx       `nVatDiscx`," &
                    " a.nPWDDiscx       `nPWDDiscx`," &
                    " a.nTendered       `nTendered`," &
                    " a.nCashAmtx       `nCashAmtx`," &
                    " a.sSourceCd       `sSourceCd`," &
                    " a.sSourceNo       `sSourceNo`," &
                    " a.nSChargex      `nSChargex`," &
                    " 'n/a'             `sTableNox`," &
                    " e.sUserName       `sCashierx`," &
                    " c.nEntryNox       `nEntryNox`," &
                    " c.sStockIDx       `sStockIDx`," &
                    " IFNULL(d.sBriefDsc, 'Meals')    `sBriefDsc`," &
                    " ''                `cReversex`," &
                    " c.nQuantity       `nQuantity`," &
                    " c.nUnitPrce       `nUnitPrce`," &
                    " '0'               `nComplmnt`," &
                    " ''                `cDetailxx`," &
                    " 'Split Order'     `xRemarksx`," &
                    " ''                `sCardDesc`," &
                    " ''                `nNoClient`," &
                    " ''                `nWithDisc`," &
                    " '0.00'            `nAddDiscx`," &
                    " '0.00'            `nDiscRate`" &
                " FROM Receipt_Master a" &
                        " LEFT JOIN xxxSysUser e" &
                            " ON a.sCashierx = e.sUserIDxx," &
                    " Order_Split b" &
                        " LEFT JOIN Order_Split_Detail c" &
                            " ON b.sTransNox = c.sTransNox" &
                        " LEFT JOIN Inventory d" &
                            " ON c.sStockIDx = d.sStockIDx" &
                " WHERE a.sSourceNo = b.sTransNox" &
                    " AND a.sSourceCd = 'SOSp'" &
                    " AND a.sTransNox LIKE " & strParm(p_oDriver.BranchCode + p_sTerminal + "%") &
                    " AND a.dTransact BETWEEN " & dateParm(p_dFromDate) & " AND " & dateParm(p_dThruDate)
        '" ORDER BY `sInvceNox`"


        p_oDTSrce = p_oDriver.ExecuteQuery(lsSQL)

        If p_oDTSrce.Rows.Count = 0 Then
            MsgBox("No records found for the given criteria.", MsgBoxStyle.Information, "Notice")
            Return False
        End If

        Dim loDtaTbl As DataTable = getRptTable()
        Dim lnCtr As Integer
        Dim lbAdd As Boolean = False

        oProg.ShowTitle("LOADING RECORDS")
        oProg.MaxValue = p_oDTSrce.Rows.Count

        p_sInvoicex = ""
        For lnCtr = 0 To p_oDTSrce.Rows.Count - 1
            oProg.ShowProcess("Loading " & p_oDTSrce(lnCtr).Item("sInvceNox") & "...")
            If p_sInvoicex <> p_oDTSrce.Rows(lnCtr).Item("sInvceNox") Then
                If p_oDTSrce.Rows(lnCtr).Item("cTranStat") = "3" Then
                    p_nSubTotl = (p_oDTSrce(lnCtr).Item("nNetAmntx") + p_oDTSrce(lnCtr).Item("nDiscount") + p_oDTSrce(lnCtr).Item("nVatDiscx") + p_oDTSrce(lnCtr).Item("nPWDDiscx")) * 0
                    p_nDiscTtl = (p_oDTSrce(lnCtr).Item("nDiscount") + p_oDTSrce(lnCtr).Item("nVatDiscx") + p_oDTSrce(lnCtr).Item("nPWDDiscx")) * 0
                    p_nNetTotl = p_oDTSrce(lnCtr).Item("nNetAmntx") * 0
                    p_nVATSale = p_oDTSrce(lnCtr).Item("nVATSales") * 0
                    p_nVATAmtx = p_oDTSrce(lnCtr).Item("nVATAmtxx") * 0
                    p_nVATExmp = (p_oDTSrce(lnCtr).Item("nNetAmntx") - (p_oDTSrce(lnCtr).Item("nVATSales") + p_oDTSrce(lnCtr).Item("nVATAmtxx"))) * 0
                    p_nZeroRtd = IFNull(p_oDTSrce(lnCtr).Item("nZeroRatd"), 0) * 0
                    p_nSrvCrge = IFNull(p_oDTSrce(lnCtr).Item("nSChargex"), 0) * 0
                    p_sInvoicex = p_oDTSrce.Rows(lnCtr).Item("sInvceNox")
                Else
                    p_nSubTotl = p_oDTSrce(lnCtr).Item("nNetAmntx") + p_oDTSrce(lnCtr).Item("nDiscount") + p_oDTSrce(lnCtr).Item("nVatDiscx") + p_oDTSrce(lnCtr).Item("nPWDDiscx")
                    p_nDiscTtl = p_oDTSrce(lnCtr).Item("nDiscount") + p_oDTSrce(lnCtr).Item("nVatDiscx") + p_oDTSrce(lnCtr).Item("nPWDDiscx")
                    p_nNetTotl = p_oDTSrce(lnCtr).Item("nNetAmntx")
                    p_nVATSale = p_oDTSrce(lnCtr).Item("nVATSales")
                    p_nVATAmtx = p_oDTSrce(lnCtr).Item("nVATAmtxx")
                    p_nVATExmp = p_oDTSrce(lnCtr).Item("nNetAmntx") - (p_oDTSrce(lnCtr).Item("nVATSales") + p_oDTSrce(lnCtr).Item("nVATAmtxx"))
                    p_nZeroRtd = IFNull(p_oDTSrce(lnCtr).Item("nZeroRatd"), 0)
                    p_nSrvCrge = IFNull(p_oDTSrce(lnCtr).Item("nSChargex"), 0)
                    p_sInvoicex = p_oDTSrce.Rows(lnCtr).Item("sInvceNox")
                End If
            End If

                'check if the next or is not the same as the current
                If lnCtr <> p_oDTSrce.Rows.Count - 1 Then
                lbAdd = p_sInvoicex <> p_oDTSrce(lnCtr + 1).Item("sInvceNox")
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
        If Not clsRpt.initReport("SleSm") Then
            Return False
        End If

        Dim loRpt As ReportDocument = clsRpt.ReportSource

        Dim loTxtObj As CrystalDecisions.CrystalReports.Engine.TextObject
        loTxtObj = loRpt.ReportDefinition.Sections(0).ReportObjects("txtCompany")
        'loTxtObj.Text = "Meet 'n' Eat"
        loTxtObj.Text = p_oDriver.BranchName

        'Set Branch Address
        loTxtObj = loRpt.ReportDefinition.Sections(0).ReportObjects("txtAddress")
        loTxtObj.Text = p_oDriver.BranchName & vbCrLf & p_oDriver.Address & vbCrLf & p_oDriver.TownCity & " " & p_oDriver.ZippCode & vbCrLf & p_oDriver.Province

        'Set First Header
        loTxtObj = loRpt.ReportDefinition.Sections(1).ReportObjects("txtHeading1")
        loTxtObj.Text = "Sales Summary per Invoice"

        'Set Second Header
        loTxtObj = loRpt.ReportDefinition.Sections(1).ReportObjects("txtHeading2")
        loTxtObj.Text = Format(p_dFromDate, xsDATE_LONG) & " to " & Format(p_dThruDate, xsDATE_LONG)

        loTxtObj = loRpt.ReportDefinition.Sections(3).ReportObjects("txtRptUser")
        loTxtObj.Text = Decrypt(p_oDriver.UserName, "08220326")

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
        p_oSTRept.ReadXmlSchema(p_oDriver.AppPath & "\vb.net\RetMgtSys\Reports\DataSet1.xsd")

        'Return the schema of the datatable derive from the DataSet 
        Return p_oSTRept.Tables(0)
    End Function

    Private Function addRow(ByVal lnRow As Integer, ByVal foSchemaTable As DataTable, Optional ByVal lbAddFoot As Boolean = False) As DataRow
        'ByVal foDTInclue As DataTable
        Dim loDtaRow As DataRow

        'Create row based on the schema of foSchemaTable
        loDtaRow = foSchemaTable.NewRow

        loDtaRow.Item("nField01") = lnRow + 1
        loDtaRow.Item("sField10") = Format(p_oDTSrce(lnRow).Item("dTransact"), "yyyy-MM-dd") & " - " & Format(p_oDTSrce(lnRow).Item("dTransact"), "dddd")
        loDtaRow.Item("sField11") = "Day Total for " & loDtaRow.Item("sField10")
        loDtaRow.Item("sField12") = "Transaction Summary for " & Format(p_dFromDate, xsDATE_LONG) & " to " & Format(p_dThruDate, xsDATE_LONG) & " Sales"
        loDtaRow.Item("sField01") = p_oDTSrce(lnRow).Item("sInvceNox")

        loDtaRow.Item("nField01") = IIf(p_oDTSrce(lnRow).Item("nQuantity") = 0, 1, p_oDTSrce(lnRow).Item("nQuantity"))
        If p_oDTSrce(lnRow).Item("nComplmnt") > 0 Then
            loDtaRow.Item("sField03") = "Complementary : " & p_oDTSrce(lnRow).Item("nComplmnt") & "pc(s) x " & p_oDTSrce(lnRow).Item("nUnitPrce") & " = " & Format(p_oDTSrce(lnRow).Item("nComplmnt") * p_oDTSrce(lnRow).Item("nUnitPrce"), xsDECIMAL)
        End If

        Dim lsRemarksx As String

        lsRemarksx = IIf(IFNull(p_oDTSrce(lnRow).Item("nAddDiscx"), 0) = 0, "", "P" & p_oDTSrce(lnRow).Item("nAddDiscx") & "/") & _
                        IIf(IFNull(p_oDTSrce(lnRow).Item("nDiscRate"), 0) = 0, "", p_oDTSrce(lnRow).Item("nDiscRate") & "% Disc. - ") & _
                        IFNull(p_oDTSrce(lnRow).Item("sCardDesc"), "")

        loDtaRow.Item("sField07") = IIf(lsRemarksx = "", "", "with " & lsRemarksx)

        loDtaRow.Item("sField02") = p_oDTSrce(lnRow).Item("sBriefDsc")
        loDtaRow.Item("sField04") = getCashier(IFNull(p_oDTSrce(lnRow).Item("sCashierx"), ""))
        loDtaRow.Item("sField05") = p_oDTSrce(lnRow).Item("xRemarksx")
        loDtaRow.Item("sField06") = IIf(p_oDTSrce(lnRow).Item("sTableNox") = "", "n/ a", p_oDTSrce(lnRow).Item("sTableNox"))

        loDtaRow.Item("sField08") = IIf(p_oDTSrce(lnRow).Item("cTranStat") = "3", "Cancelled OR", "")
        loDtaRow.Item("lField01") = p_oDTSrce(lnRow).Item("nUnitPrce")
        

        If lbAddFoot Then
            loDtaRow.Item("lField02") = p_nSubTotl + p_nSrvCrge
            loDtaRow.Item("lField03") = p_nDiscTtl
            loDtaRow.Item("lField04") = p_nNetTotl
            loDtaRow.Item("lField10") = p_nNetTotl - p_nSrvCrge
            loDtaRow.Item("lField05") = p_nVATSale
            loDtaRow.Item("lField06") = p_nVATAmtx
            loDtaRow.Item("lField07") = p_nVATExmp
            loDtaRow.Item("lField08") = p_nZeroRtd
            loDtaRow.Item("lField09") = p_nSrvCrge
        Else
            loDtaRow.Item("lField02") = 0
            loDtaRow.Item("lField03") = 0
            loDtaRow.Item("lField04") = 0
            loDtaRow.Item("lField05") = 0
            loDtaRow.Item("lField06") = 0
            loDtaRow.Item("lField07") = 0
            loDtaRow.Item("lField08") = 0
            loDtaRow.Item("lField09") = 0
            loDtaRow.Item("lField10") = 0
        End If

        Return loDtaRow
    End Function

    Public Sub New(ByVal foRider As GRider)
        p_oDriver = foRider
        p_oSTRept = Nothing
        p_oDTSrce = Nothing
    End Sub

    Public Sub New(ByVal foRider As GRider, _
                   ByVal foMachineNo As String)
        p_oDriver = foRider
        p_oSTRept = Nothing
        p_oDTSrce = Nothing

        p_sMachinex = foMachineNo
    End Sub
End Class

