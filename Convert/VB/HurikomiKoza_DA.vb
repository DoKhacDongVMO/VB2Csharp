''' <summary>
''' 振込口座マスタのDAクラス
''' </summary>
Public Class HurikomiKoza_DA
    Inherits DataAccessorBase

#Region " 定数／変数 "

    Private hurikomiKoza As New MHurikomiKozaEntity()

    Public Enum accessType As Integer
        getHuriKomiKoza                      '一覧結果取得検索
    End Enum

#End Region

#Region " SELECT処理 "

    ''' <summary>
    ''' SELECT用DBアクセス
    ''' </summary>
    ''' <param name="type"></param>
    ''' <param name="paramInfoList"></param>
    ''' <returns></returns>
    Public Function accessHorikomiKoza(ByVal type As accessType, Optional ByVal paramInfoList As Hashtable = Nothing) As DataTable
        'SQL文字列
        Dim sqlString As String = String.Empty
        '戻り値
        Dim returnValue As DataTable = Nothing

        Select Case type
            Case accessType.getHuriKomiKoza
                '一覧結果取得検索
                sqlString = getHuriKomiKoza(paramInfoList)
            Case Else
                '該当処理なし
                Return returnValue
        End Select

        Try
            returnValue = getDataTable(sqlString)
        Catch ex As Exception
            Throw
        End Try

        Return returnValue

    End Function

    ''' <summary>
    ''' 検索用SELECT
    ''' </summary>
    ''' <param name="paramList"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Protected Overloads Function getHuriKomiKoza(ByVal paramList As Hashtable) As String

        Dim strSQL As String = String.Empty

        strSQL &= " SELECT HURIKOMI_SAKI_BANK_NAME "
        strSQL &= "     ,HURIKOMI_SAKI_KOZA_NAME "
        strSQL &= "     ,HURIKOMI_SAKI_BRANCH_NAME "
        strSQL &= "     ,KOZA_NO "
        strSQL &= "     ,YOKIN_EVENT "
        strSQL &= " FROM M_HURIKOMI_KOZA "
        strSQL &= " WHERE HURIKOMI_SEIKYUSYO_FOR_FLG = 'S' "

        Return strSQL.ToString
    End Function
#End Region

End Class
