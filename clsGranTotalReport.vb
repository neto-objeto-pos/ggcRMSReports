Imports MySql.Data.MySqlClient
Imports ADODB
Imports ggcAppDriver
Imports CrystalDecisions.CrystalReports.Engine

Public Class clsGranTotalReport
    Private p_oDriver As ggcAppDriver.GRider
    Private p_oSTRept As DataSet
    Private p_oDTSrce As DataTable

    Private p_sMachinex As String
    Private p_nReptType As Integer      '0=Summary;1=Detail
    Private p_sBranchCD As String

    Private p_dFromDate As Date
    Private p_dThruDate As Date
    Private p_sTerminal As String

    Private p_nCashTotl As Decimal
    Private p_nCredtTtl As Decimal
    Private p_nCheckTtl As Decimal
    Private p_nGCertTtl As Decimal

    Public Function ReportTrans() As Boolean

        Dim loForm As frmTerminalSelector
        loForm = New frmTerminalSelector
        loForm.GRider = p_oDriver
        loForm.ShowDialog()

        If loForm.Cancelled Then
            MsgBox("Unable to generate report.", MsgBoxStyle.Information, "Notice")
            loForm = Nothing
            Return False
        End If

        p_sTerminal = loForm.txtField00.Text

        'Dim oProg As frmProgress
        'oProg = New frmProgress
        'oProg.PistonInfo = p_oDriver.AppPath & "/piston.avi"
        'oProg.ShowTitle("EXTRACTING RECORDS FROM DATABASE")
        'oProg.ShowProcess("Please wait...")
        'oProg.Show()

        Dim lsSQL As String 'whole statement

        lsSQL = "SELECT SUM(nCashAmtx) `nAmountxx` FROM Receipt_Master  WHERE cTranStat NOT IN ('3', '4') AND sSourceNo LIKE " & strParm(p_oDriver.BranchCode + p_sTerminal + "%")

        p_oDTSrce = p_oDriver.ExecuteQuery(lsSQL)
        If Not IsNothing(p_oDTSrce) Then
            p_nCashTotl = IFNull(p_oDTSrce(0)("nAmountxx"), 0)
            p_oDTSrce = Nothing
        End If

        lsSQL = "SELECT SUM(nAmountxx) `nAmountxx` FROM Check_Payment_Trans  WHERE cTranStat = '0' AND sSourceNo LIKE " & strParm(p_oDriver.BranchCode + p_sTerminal + "%")
        Debug.Print(lsSQL)
        p_oDTSrce = p_oDriver.ExecuteQuery(lsSQL)
        If Not IsNothing(p_oDTSrce) Then
            p_nCheckTtl = IFNull(p_oDTSrce(0)("nAmountxx"), 0)
            p_oDTSrce = Nothing
        End If

        lsSQL = "SELECT SUM(nAmountxx) `nAmountxx` FROM Gift_Certificate_Trans  WHERE cTranStat = '0' AND sSourceNo LIKE " & strParm(p_oDriver.BranchCode + p_sTerminal + "%")
        Debug.Print(lsSQL)
        p_oDTSrce = p_oDriver.ExecuteQuery(lsSQL)
        If Not IsNothing(p_oDTSrce) Then
            p_nGCertTtl = IFNull(p_oDTSrce(0)("nAmountxx"), 0)
            p_oDTSrce = Nothing
        End If

        lsSQL = "SELECT SUM(nAmountxx) `nAmountxx` FROM Credit_Card_Trans  WHERE cTranStat = '0' AND sSourceNo LIKE " & strParm(p_oDriver.BranchCode + p_sTerminal + "%")
        Debug.Print(lsSQL)
        p_oDTSrce = p_oDriver.ExecuteQuery(lsSQL)
        If Not IsNothing(p_oDTSrce) Then
            p_nCredtTtl = IFNull(p_oDTSrce(0)("nAmountxx"), 0)
            p_oDTSrce = Nothing
        End If

        Dim loDtaTbl As DataTable = getRptTable()
        Dim lbAdd As Boolean = False


        loDtaTbl.Rows.Add(addRow(loDtaTbl))


        Dim clsRpt As clsReports
        clsRpt = New clsReports
        clsRpt.GRider = p_oDriver
        'Set the Report Source Here
        If Not clsRpt.initReport("AccGT") Then
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
        loTxtObj.Text = "Accumulated Grand Total Report"

        'Set Second Header
        'loTxtObj = loRpt.ReportDefinition.Sections(1).ReportObjects("txtHeading2")
        'loTxtObj.Text = Format(p_dFromDate, xsDATE_LONG) & " to " & Format(p_dThruDate, xsDATE_LONG)

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

    Private Function addRow(ByVal foSchemaTable As DataTable) As DataRow
        'ByVal foDTInclue As DataTable
        Dim loDtaRow As DataRow

        'Create row based on the schema of foSchemaTable
        loDtaRow = foSchemaTable.NewRow

        loDtaRow.Item("lField01") = p_nCashTotl
        loDtaRow.Item("lField02") = p_nCredtTtl
        loDtaRow.Item("lField03") = p_nCheckTtl
        loDtaRow.Item("lField04") = p_nGCertTtl
        loDtaRow.Item("lField05") = p_nCashTotl + p_nCredtTtl + p_nCheckTtl + p_nGCertTtl

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
