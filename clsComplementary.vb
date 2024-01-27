

Imports MySql.Data.MySqlClient
Imports ADODB
Imports ggcAppDriver
Imports CrystalDecisions.CrystalReports.Engine

Public Class clsComplementary
    Private p_oDriver As GRider
    Private p_oSTRept As DataSet
    Private p_oDTSrce As DataTable

    Private p_sTerminal As String
    Private p_sMachinex As String
    Private p_nReptType As Integer      '0=Summary;1=Detail
    Private p_sBranchCD As String

    Private p_sInvoicex As String
    Private p_dFromDate As Date
    Private p_dThruDate As Date
    Private p_sTransNox As String
    Private p_sTransact As String
    Private p_nCanceldx As Integer
    Private p_nTotCncld As Integer

    Dim p_nNetTotal As Decimal
    Dim p_nDiscount As Decimal
    Dim p_nSCDiscxx As Decimal
    Dim p_nSubTotl As Decimal
    Dim p_nDiscTtl As Decimal
    Dim p_nNetTotl As Decimal
    Dim p_nVATSale As Decimal
    Dim p_nVATAmtx As Decimal
    Dim p_nVATExmp As Decimal
    Dim p_nZeroRtd As Decimal

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

        lsSQL = "SELECT" & _
                  " h.nAmountxx," & _
                  " h.sTransNox," & _
                  " a.nContrlNo," & _
                  " a.dTransact," & _
                  " b.nQuantity," & _
                  " b.nUnitPrce," & _
                  " b.nDiscount, " & _
                  " b.nAddDiscx, " & _
                  " c.sBriefDsc, " & _
                  " d.sUserName    `sCashierx`, " & _
                  " e.sDiscCard, " & _
                  " e.nNoClient, " & _
                        " e.nWithDisc, " & _
                  " f.nDiscRate," & _
                  " f.nDiscAmtx" & _
                " FROM Complementary h " & _
                  " LEFT JOIN SO_Master a " & _
                    " ON h.sSourceNo = a.sTransNox " & _
                  " LEFT JOIN xxxSysUser d " & _
                    " ON a.sCashierx = d.sUserIDxx, " & _
                        " SO_Detail b " & _
                  " LEFT JOIN Inventory c " & _
                    " ON b.sStockIDx = c.sStockIDx " & _
                  " LEFT JOIN Discount e " & _
                    " ON e.sSourceCd = 'SO' " & _
                      " AND b.sTransNox = e.sSourceNo " & _
                      " LEFT JOIN Discount_Card_Detail f" & _
                      " ON e.sDiscCard = f.sCardIDxx" & _
                        " WHERE(h.sSourceNo = a.sTransNox) " & _
                    " AND a.sTransNox = b.sTransNox " & _
                    " AND a.cTranStat = '2' " & _
                    " AND b.cReversed <> '1'" & _
                    " AND h.sTransNox LIKE " & strParm(p_oDriver.BranchCode + p_sTerminal + "%") & _
                    " AND a.dTransact BETWEEN" & dateParm(p_dFromDate) & _
                    " AND " & dateParm(p_dThruDate) & _
                    " AND b.nComplmnt <> '0'" & _
                " UNION SELECT " & _
                        " h.nAmountxx, " & _
                        " h.sTransNox, " & _
                        " a.nContrlNo, " & _
                        " a.dTransact, " & _
                        " b.nQuantity, " & _
                        " b.nUnitPrce, " & _
                        " b.nDiscount, " & _
                        " b.nAddDiscx, " & _
                        " c.sBriefDsc, " & _
                        " d.sUserName    `sCashierx`, " & _
                        " e.sDiscCard, " & _
                        " e.nNoClient, " & _
                        " e.nWithDisc, " & _
                              " f.nDiscRate," & _
                              " f.nDiscAmtx" & _
                      " FROM Complementary h " & _
                        " LEFT JOIN SO_Master a " & _
                          " ON h.sSourceNo = a.sTransNox " & _
                        " LEFT JOIN xxxSysUser d" & _
                          " ON a.sCashierx = d.sUserIDxx," & _
                             " SO_Detail b" & _
                        " LEFT JOIN Inventory c" & _
                          " ON b.sStockIDx = c.sStockIDx" & _
                        " LEFT JOIN Discount e" & _
                          " ON e.sSourceCd = 'SO'" & _
                            " AND b.sTransNox = e.sSourceNo" & _
                            " LEFT JOIN Discount_Card_Detail f" & _
                            " ON e.sDiscCard = f.sCardIDxx" & _
                              " WHERE(h.sSourceNo = a.sTransNox)" & _
                          " AND a.sTransNox = b.sTransNox" & _
                          " AND a.cTranStat = '2'" & _
                          " AND h.sTransNox LIKE " & strParm(p_oDriver.BranchCode + p_sTerminal + "%") & _
                          " AND a.dTransact BETWEEN " & dateParm(p_dFromDate) & _
                          " AND " & dateParm(p_dThruDate) & _
                          " AND b.nComplmnt <> '0'" & _
                          " ORDER BY sTransNox;"


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

        For lnCtr = 0 To p_oDTSrce.Rows.Count - 1
            oProg.ShowProcess("Loading " & p_oDTSrce(lnCtr).Item("sTransNox") & "...")
            loDtaTbl.Rows.Add(addRow(lnCtr, loDtaTbl, lbAdd))
        Next

        oProg.ShowSuccess()

        Dim clsRpt As clsReports
        clsRpt = New clsReports
        clsRpt.GRider = p_oDriver
        'Set the Report Source Here
        If Not clsRpt.initReport("Compl") Then
            Return False
        End If

        Dim loRpt As ReportDocument = clsRpt.ReportSource

        Dim loTxtObj As CrystalDecisions.CrystalReports.Engine.TextObject
        loTxtObj = loRpt.ReportDefinition.Sections(0).ReportObjects("txtCompany")
        loTxtObj.Text = "Meet 'n' Eat"

        'Set Branch Address
        loTxtObj = loRpt.ReportDefinition.Sections(0).ReportObjects("txtAddress")
        loTxtObj.Text = p_oDriver.BranchName & vbCrLf & p_oDriver.Address & vbCrLf & p_oDriver.TownCity & " " & p_oDriver.ZippCode & vbCrLf & p_oDriver.Province

        'Set First Header
        loTxtObj = loRpt.ReportDefinition.Sections(1).ReportObjects("txtHeading1")
        loTxtObj.Text = "Complementary Sales Report"

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

    Private Function addRow(ByVal lnRow As Integer, ByVal foSchemaTable As DataTable, Optional ByVal lbAddFoot As Boolean = False) As DataRow
        'ByVal foDTInclue As DataTable
        Dim loDtaRow As DataRow
        Dim loDiscount As Double

        'Create row based on the schema of foSchemaTable
        loDtaRow = foSchemaTable.NewRow

        loDtaRow.Item("nField01") = lnRow + 1
        loDtaRow.Item("nField02") = p_oDTSrce(lnRow).Item("nContrlNo")
        loDtaRow.Item("sField10") = Format(p_oDTSrce(lnRow).Item("dTransact"), "yyyy-MM-dd") & " - " & Format(p_oDTSrce(lnRow).Item("dTransact"), "dddd")
        loDtaRow.Item("sField11") = "Day Total for " & loDtaRow.Item("sField10")
        loDtaRow.Item("sField12") = "Transaction Summary for " & Format(p_dFromDate, xsDATE_LONG) & " to " & Format(p_dThruDate, xsDATE_LONG) & " Transaction"
        loDtaRow.Item("sField01") = Strings.Right(p_oDTSrce(lnRow).Item("sTransNox"), 10)

        loDtaRow.Item("sField07") = Decrypt(p_oDTSrce(lnRow).Item("sCashierx"), "08220326")

        loDtaRow.Item("nField01") = p_oDTSrce(lnRow).Item("nQuantity")
        loDtaRow.Item("sField02") = p_oDTSrce(lnRow).Item("sBriefDsc")
        loDtaRow.Item("lField01") = p_oDTSrce(lnRow).Item("nUnitPrce")
        loDtaRow.Item("lField02") = p_oDTSrce(lnRow).Item("nUnitPrce") * p_oDTSrce(lnRow).Item("nQuantity")
        If (IFNull(p_oDTSrce(lnRow).Item("nDiscRate"), 0) = "1604") Then
            loDiscount =
            loDtaRow.Item("lField03") = IFNull(p_oDTSrce(lnRow).Item("nDiscRate"), 0)
        Else
            loDiscount = p_oDTSrce(lnRow).Item("nUnitPrce") * IFNull(p_oDTSrce(lnRow).Item("nDiscRate"), 0)
            loDtaRow.Item("lField03") = loDiscount - p_oDTSrce(lnRow).Item("nUnitPrce")
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
