Imports System.Text
''' <summary>
''' インターネット予約トランザクション管理）のDAクラス
''' <remarks>
''' Author:2018/10/26//QuangTD
''' </remarks>
''' </summary>
Public Class CourseMst_DA
    Inherits DataAccessorBase

#Region " 定数／変数 "
    Private clsTCourseMstEntity As New TCourseMstEntity()
#End Region

    ''' <summary>
    ''' 検索用SELECT
    ''' </summary>
    ''' <param name="paramInfoList"></param>
    ''' <remarks></remarks>
    Public Function getCrsCdExist(paramInfoList As Hashtable) As DataTable
        '戻り値
        Dim returnValue As DataTable = Nothing
        Dim sqlString As New StringBuilder
        With clsTCourseMstEntity
            'SELECT
            sqlString.AppendLine("SELECT")
            sqlString.AppendLine("    CRS_CD ")
            sqlString.AppendLine("FROM")
            sqlString.AppendLine("    T_COURSE_MST ")
            sqlString.AppendLine("WHERE")
            If Not String.IsNullOrEmpty(CType(paramInfoList.Item("crsCd"), String)) Then
                sqlString.AppendLine("    T_COURSE_MST.CRS_CD = " & setParam("crsCd", CType(paramInfoList.Item("crsCd"), String), .crsCd.DBType, .crsCd.IntegerBu, .crsCd.DecimalBu))
            End If
        End With
        Try
            returnValue = getDataTable(sqlString.ToString)
        Catch ex As Exception
            Throw
        End Try
        Return returnValue
    End Function
End Class
