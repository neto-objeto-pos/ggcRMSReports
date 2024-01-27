Imports MySql.Data.MySqlClient
Imports ADODB
Imports ggcAppDriver
Imports CrystalDecisions.CrystalReports.Engine

Public Class clsSOASummarized
    Private Const xsSignature As String = "08220326"

    Private p_oDriver As ggcAppDriver.GRider
    Private p_oSTRept As DataSet
    Private p_oDTSrce As DataTable

    Private p_sMachinex As String
    Private p_nReptType As Integer      '0=Summary;1=Detail
    Private p_sBranchCD As String

    Private p_dFromDate As Date
    Private p_dThruDate As Date

    Private p_sRiderID As String

    Public Function ReportTrans() As Boolean
        Dim loForm As frmSOACriteria
        loForm = New frmSOACriteria
        loForm.GRider = p_oDriver
        loForm.ShowDialog()

        If Not loForm.isOkey Then
            MsgBox("Unable to generate report.", MsgBoxStyle.Information, "Notice")
            loForm = Nothing
            Return False
        End If


        p_sRiderID = loForm.txtField00.Tag
        p_dFromDate = loForm.txtField01.Text
        p_dThruDate = loForm.txtField02.Text

        Dim oProg As frmProgress
        oProg = New frmProgress
        oProg.PistonInfo = p_oDriver.AppPath & "/piston.avi"
        oProg.ShowTitle("EXTRACTING RECORDS FROM DATABASE")
        oProg.ShowProcess("Please wait...")
        oProg.Show()

        Dim lsSQL As String

        lsSQL = " SELECT " &
                    "   a.sTransNox sTransNox " &
                    " , a.dTransact dTransact " &
                    " , d.sCompnyNm sCompnyNm " &
                    " , a.sSourceCd sSourceCd " &
                    " , a.cTranStat cTranStat " &
                    " , a.sRemarksx sRemarksx " &
                    " , a.nTranTotl nTranTotl " &
                    "  FROM Billing_Master a " &
                    " INNER JOIN Billing_Detail b " &
                    " ON a.sTransNox = b.sTransNox " &
                    " LEFT JOIN Delivery_Service_Trans c " &
                    " ON b.sSourceNo = c.sTransNox " &
                   " LEFT JOIN Client_Master d " &
                    " ON a.sClientID = d.sClientID " &
                    " LEFT JOIN Delivery_Service e " &
                    " ON c.sRiderIDx = e.sRiderIDx " &
                    " WHERE a.dTransact BETWEEN " & dateParm(p_dFromDate) & " And " & dateParm(p_dThruDate) &
                    " AND c.sRiderIDx = " & strParm(p_sRiderID) &
                    "GROUP BY a.sTransNox ORDER BY a.sTransNox "

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
            oProg.ShowProcess("Loading " & p_oDTSrce(lnCtr).Item("sCompnyNm") & "...")


            loDtaTbl.Rows.Add(addRow(lnCtr, loDtaTbl))
        Next

        oProg.ShowSuccess()

        Dim clsRpt As clsReports
        clsRpt = New clsReports
        clsRpt.GRider = p_oDriver
        'Set the Report Source Here
        If Not clsRpt.initReport("SOASum") Then
            Return False
        End If

        Dim loRpt As ReportDocument = clsRpt.ReportSource

        Dim loTxtObj As CrystalDecisions.CrystalReports.Engine.TextObject
        'loTxtObj = loRpt.ReportDefinition.Sections(0).ReportObjects("txtCompany")
        'loTxtObj.Text = "Meet 'n' Eat"

        ''Set Branch Address
        'loTxtObj = loRpt.ReportDefinition.Sections(0).ReportObjects("txtAddress")
        'loTxtObj.Text = p_oDriver.BranchName & vbCrLf & p_oDriver.Address & vbCrLf & p_oDriver.TownCity & " " & p_oDriver.ZippCode & vbCrLf & p_oDriver.Province

        ''Set First Header
        loTxtObj = loRpt.ReportDefinition.Sections(1).ReportObjects("txtHeading1")
        loTxtObj.Text = "Statement of Account Summarized Report"

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

        loDtaRow.Item("sField01") = p_oDTSrce(lnRow).Item("sTransNox")
        loDtaRow.Item("sField02") = Format(p_oDTSrce(lnRow).Item("dTransact"), "MMMM dd, yyyy")
        loDtaRow.Item("sField03") = p_oDTSrce(lnRow).Item("sCompnyNm")
        loDtaRow.Item("sField04") = IIf(p_oDTSrce(lnRow).Item("sSourceCd") = "DS", "Delivery Service", "Charge Invoice")
        loDtaRow.Item("sField05") = TranStatus(p_oDTSrce(lnRow).Item("cTranStat"))
        loDtaRow.Item("lField01") = CDbl(p_oDTSrce(lnRow).Item("nTranTotl"))
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

