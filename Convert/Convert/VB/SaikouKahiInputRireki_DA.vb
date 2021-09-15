Imports System.Text

''' <summary>
''' 催行可否入力履歴のDAクラス
''' </summary>
Public Class SaikouKahiInputRireki_DA
    Inherits DataAccessorBase

#Region " 定数／変数 "

    Public Enum accessType As Integer
        getSaikouKahiInputRireki                   '一覧結果取得検索
    End Enum

#End Region

#Region " SELECT処理 "

    ''' <summary>
    ''' SELECT用DBアクセス
    ''' </summary>
    ''' <param name="type"></param>
    ''' <param name="paramInfoList"></param>
    ''' <returns></returns>
    Public Function accessSaikouKahiRirekiTehai(ByVal type As accessType, Optional ByVal paramInfoList As Hashtable = Nothing) As DataTable

        'SQL文字列
        Dim sqlString As String = String.Empty
        '戻り値
        Dim returnValue As DataTable = Nothing

        Select Case type
            Case accessType.getSaikouKahiInputRireki
                '一覧結果取得検索
                sqlString = getSaikouKahiInputRireki(paramInfoList)
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
    Protected Overloads Function getSaikouKahiInputRireki(ByVal paramList As Hashtable) As String

        Dim sqlString As New StringBuilder
        paramClear()

        'SELECT句
        sqlString.AppendLine(" SELECT ")
        sqlString.AppendLine(" CASE WHEN TRIM(T_SDCH.SAIKOU_KAKUTEI_KBN) IS NULL THEN ")
        sqlString.AppendLine("'未設定' ")
        sqlString.AppendLine("   WHEN T_SDCH.SAIKOU_KAKUTEI_KBN = " & " '" & FixedCd.SaikouKakuteiKbn.Saikou & "' " & " THEN ")
        sqlString.AppendLine("'催行' ")
        sqlString.AppendLine("   WHEN T_SDCH.SAIKOU_KAKUTEI_KBN = " & " '" & FixedCd.SaikouKakuteiKbn.Tyushi & "' " & " THEN ")
        sqlString.AppendLine("'中止' ")
        sqlString.AppendLine("   WHEN T_SDCH.SAIKOU_KAKUTEI_KBN = " & " '" & FixedCd.SaikouKakuteiKbn.Haishi & "' " & " THEN ")
        sqlString.AppendLine("'廃止' ")
        sqlString.AppendLine("   ELSE '' END AS SAIKOU_KAKUTEI_KBN ") '催行確定区分
        sqlString.AppendLine(",CASE WHEN TRIM(T_SDCH.TEJIMAI_KBN) IS NULL THEN ")
        sqlString.AppendLine("'未' ")
        sqlString.AppendLine("   WHEN T_SDCH.TEJIMAI_KBN = " & " '" & FixedCd.TejimaiKbn.Zumi & "' " & " THEN ")
        sqlString.AppendLine("'済' ")
        sqlString.AppendLine("   WHEN T_SDCH.TEJIMAI_KBN = " & " '" & FixedCd.TejimaiKbn.Mi & "' " & " THEN ")
        sqlString.AppendLine("'未' ")
        sqlString.AppendLine("   ELSE '' END AS TEJIMAI_KBN ") '手仕舞区分
        sqlString.AppendLine(",TO_CHAR(TO_DATE(T_SDCH.SYUPT_DAY),'yyyy/MM/dd') AS SYUPT_DAY ") '出発日
        sqlString.AppendLine(",T_SDCH.CRS_CD ") 'コースコード
        sqlString.AppendLine(",T_SDCH.CRS_NAME ") 'コース名
        sqlString.AppendLine(",TO_CHAR(T_SDCH.SYSTEM_UPDATE_DAY, 'YYYY/MM/DD') AS SYSTEM_UPDATE_DAY ") 'システム更新日
        sqlString.AppendLine(",TO_CHAR(T_SDCH.SYSTEM_UPDATE_DAY, 'HH24:MI:SS') AS SYSTEM_UPDATE_TIME ") 'システム更新時間
        sqlString.AppendLine(",CASE WHEN T_SDCH.DELETE_DAY = 0 THEN ")
        sqlString.AppendLine("'' ")
        sqlString.AppendLine("   WHEN T_SDCH.DELETE_DAY IS NULL THEN ")
        sqlString.AppendLine("'' ")
        sqlString.AppendLine("   ELSE TO_CHAR(TO_DATE(TO_CHAR(T_SDCH.DELETE_DAY), 'yyyyMMdd'), 'yyyy/MM/dd') END AS DELETE_DAY ") '削除日
        sqlString.AppendLine(",M_U.USER_NAME AS UPDATE_PERSON_NAME ") '更新者名
        'FROM句
        sqlString.AppendLine(" FROM ")
        sqlString.AppendLine(" T_SAIKOU_DECISION_CRS_HISTORY T_SDCH ")
        sqlString.AppendLine("LEFT JOIN M_USER M_U ON T_SDCH.SYSTEM_UPDATE_PERSON_CD = M_U.USER_ID ")
        'WHERE句
        sqlString.AppendLine(" WHERE ")
        If CType(paramList.Item("CRSCD"), String) <> String.Empty Then
            sqlString.AppendLine(" T_SDCH.CRS_CD = " & setParam("CRS_CD", paramList.Item("CRSCD"), OracleDbType.Char))
            sqlString.AppendLine(" AND ")
        End If
        sqlString.AppendLine(" T_SDCH.SYUPT_DAY >= " & setParam("SYUPTDAYFROM", paramList.Item("SYUPTDAYFROM"), OracleDbType.Decimal, 8, 0))
        sqlString.AppendLine(" AND ")
        sqlString.AppendLine(" T_SDCH.SYUPT_DAY <= " & setParam("SYUPTDAYTO", paramList.Item("SYUPTDAYTO"), OracleDbType.Decimal, 8, 0))
        'ORDER BY句
        sqlString.AppendLine(" ORDER BY ")
        sqlString.AppendLine(" T_SDCH.SYSTEM_UPDATE_DAY DESC ")

        Return sqlString.ToString

    End Function

#End Region

End Class
