Imports System.Text

''' <summary>
''' P02_0603 DA
''' </summary>
Public Class P02_0603Da
    Inherits DataAccessorBase

#Region " 定数／変数 "

#End Region

#Region "メソッド"

    ''' <summary>
    ''' 領収書(営業所名)取得用SQL
    ''' </summary>
    ''' <param name="companyCd">会社コード</param>
    ''' <param name="eigyosyoCd">営業所コード</param>
    ''' <returns></returns>
    Public Function getP02_0601EigyosyoName(ByVal companyCd As String, ByVal eigyosyoCd As String) As DataTable

        MyBase.paramClear()

        '必須条件チェック
        If String.IsNullOrWhiteSpace(companyCd) Then
            Return Nothing
        End If

        If String.IsNullOrWhiteSpace(eigyosyoCd) Then
            Return Nothing
        End If

        Dim ent As New EigyosyoMasterEntity
        Dim sb As New StringBuilder

        'パラメータの取得
        Dim paramCompanyCd As String = MyBase.setParam(ent.CompanyCd.PhysicsName, companyCd, ent.CompanyCd.DBType, ent.CompanyCd.IntegerBu)
        Dim paramEigyosyoCd As String = MyBase.setParam(ent.EigyosyoCd.PhysicsName, eigyosyoCd, ent.EigyosyoCd.DBType, ent.EigyosyoCd.IntegerBu)

        Try
            sb.AppendLine(" SELECT ")
            sb.AppendLine("     EIGYOSYO_NAME_1 ")
            sb.AppendLine(" FROM  ")
            sb.AppendLine("     M_EIGYOSYO ")
            sb.AppendLine(" WHERE 1=1")
            sb.AppendLine($"     AND COMPANY_CD = {paramCompanyCd} ")
            sb.AppendLine($"     AND EIGYOSYO_CD  = {paramEigyosyoCd} ")

            Return MyBase.getDataTable(sb.ToString())

        Catch
            Throw
        End Try

    End Function
#End Region

End Class
