Imports MySql.Data.MySqlClient
Imports ADODB
Imports ggcAppDriver
Imports CrystalDecisions.CrystalReports.Engine

Public Class clsEventLogs

    Private Const xsSignature As String = "08220326"

    Private p_oDriver As ggcAppDriver.GRider
    Private p_oSTRept As DataSet
    Private p_oDTSrce As DataTable

    Private p_nReptType As Integer      '0=Summary;1=Detail
    Private p_sBranchCD As String

    Private p_dFromDate As Date
    Private p_dThruDate As Date

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
                    "  a.sTransNox" & _
                    ", b.sDescript" & _
                    ", a.sRemarksx" & _
                    ", c.sUserName" & _
                    ", a.sSerialNo" & _
                    ", a.sComptrNm" & _
                    ", a.dModified" & _
                    ", c.sUserIDxx" & _
                " FROM Event_Master a" & _
                    " LEFT JOIN EVENTS b" & _
                        " ON a.sEventIDx = b.sEventIDx" & _
                    " LEFT JOIN xxxSysUser c" & _
                        " ON a.sUserIDxx = c.sUserIDxx" & _
                " WHERE a.dModified BETWEEN " & datetimeParm(p_dFromDate & " 00:00:01") & " AND " & datetimeParm(p_dThruDate & " 23:59:30") & _
                " ORDER BY a.sTransNox"

        p_oDTSrce = p_oDriver.ExecuteQuery(lsSQL)

        Dim loDtaTbl As DataTable = getRptTable()
        Dim lnCtr As Integer

        oProg.ShowTitle("LOADING RECORDS")
        oProg.MaxValue = p_oDTSrce.Rows.Count

        For lnCtr = 0 To p_oDTSrce.Rows.Count - 1

            oProg.ShowProcess("Loading " & p_oDTSrce(lnCtr).Item("sTransNox") & "...")

            loDtaTbl.Rows.Add(addRow(lnCtr, loDtaTbl))
        Next

        oProg.ShowSuccess()

        Dim clsRpt As clsReports
        clsRpt = New clsReports
        clsRpt.GRider = p_oDriver
        'Set the Report Source Here
        If Not clsRpt.initReport("EvtLg") Then
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
        loTxtObj.Text = "Activity Log"

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

    Public Function getLogName(ByVal sCashierx As String) As String
        Dim lsSQL As String
        Dim lsLogName As String
        Dim loDta As DataTable

        lsSQL = "SELECT" & _
                    " a.sLogNamex" & _
                    " FROM xxxSysUser a" & _
                    " WHERE a.sUserIDxx = " & strParm(sCashierx)

        loDta = p_oDriver.ExecuteQuery(lsSQL)

        If loDta.Rows.Count = 0 Then
            lsLogName = ""
        Else
            lsLogName = Decrypt(loDta(0).Item("sLogNamex"), xsSignature)
        End If

        loDta = Nothing

        Return lsLogName

    End Function

    Private Function addRow(ByVal lnRow As Integer, ByVal foSchemaTable As DataTable) As DataRow
        'ByVal foDTInclue As DataTable
        Dim loDtaRow As DataRow

        'Create row based on the schema of foSchemaTable
        loDtaRow = foSchemaTable.NewRow

        loDtaRow.Item("nField01") = lnRow + 1
        loDtaRow.Item("sField02") = Right(p_oDTSrce(lnRow).Item("sTransNox"), 8)
        loDtaRow.Item("sField03") = p_oDTSrce(lnRow).Item("sDescript")
        loDtaRow.Item("sField04") = p_oDTSrce(lnRow).Item("sRemarksx")
        loDtaRow.Item("sField05") = p_oDTSrce(lnRow).Item("sSerialNo")
        loDtaRow.Item("sField07") = p_oDTSrce(lnRow).Item("sUserIDxx") & " / " & getLogName(p_oDTSrce(lnRow).Item("sUserIDxx"))
        loDtaRow.Item("sField06") = p_oDTSrce(lnRow).Item("dModified")


        Return loDtaRow
    End Function

    Public Sub New(ByVal foRider As GRider)
        p_oDriver = foRider
        p_oSTRept = Nothing
        p_oDTSrce = Nothing

        p_nReptType = 0
    End Sub
End Class
