Imports System.Text

Public Class S05_0501_DA
    Inherits DataAccessorBase

#Region "定数・変数"

    Public Enum accessType As Integer
        getEigyousyoName             ' 営業所名取得
        getNippoKingaku              ' 日報金額取得
        getGridData                  ' 一覧結果取得検索
    End Enum

    ' 精算情報エンティティ
    Private clsSeisanInfoEntity As New SeisanInfoEntity()
    ' 内訳情報（クレジット会社）エンティティ
    Private clsUtiwakeInfoCreditCompanyEntity As New TUtiwakeInfoCreditCompanyEntity()

#End Region

#Region "SELECT処理"

    ''' <summary>
    ''' SELECT用DBアクセス
    ''' </summary>
    ''' <param name="type"></param>
    ''' <param name="paramInfoList"></param>
    ''' <returns></returns>
    Public Function accessS05_0501(ByVal type As accessType, Optional ByVal paramInfoList As Hashtable = Nothing) As DataTable
        'SQL文字列
        Dim sqlString As String = String.Empty
        '戻り値
        Dim returnValue As DataTable = Nothing

        Select Case type
            Case accessType.getEigyousyoName
                ' 営業所名取得
                sqlString = getEigyousyoName(paramInfoList)
            Case accessType.getNippoKingaku
                ' 日報金額取得
                sqlString = getNippoKingaku(paramInfoList)
            Case accessType.getGridData
                ' 一覧結果取得検索
                sqlString = getGridData(paramInfoList)
            Case Else
                ' 該当処理なし
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
    ''' 営業所名取得
    ''' </summary>
    ''' <param name="param">会社コードと営業所コード</param>
    ''' <returns></returns>
    Public Function getEigyousyoName(param As Hashtable) As String
        Dim sql As New StringBuilder
        sql.AppendLine(" SELECT EIGYOSYO_NAME_1")
        sql.AppendLine("   FROM M_EIGYOSYO")
        sql.AppendLine("  WHERE COMPANY_CD=" & setParam("COMPANY_CD", param.Item("CompanyCd"), OracleDbType.Char, 2))
        sql.AppendLine("    AND EIGYOSYO_CD=" & setParam("EIGYOSYO_CD", param.Item("EigyousyoCd"), OracleDbType.Char, 2))
        Return sql.ToString
    End Function

    ''' <summary>
    ''' 日報金額取得
    ''' </summary>
    ''' <param name="param">検索条件</param>
    ''' <returns></returns>
    Public Function getNippoKingaku(param As Hashtable) As String
        Dim sql As New StringBuilder
        'SQL生成
        sql.AppendLine(" SELECT ")
        sql.AppendLine("        SSN.COMPANY_CD ")                        ' 精算情報.会社コード
        sql.AppendLine("       ,SSN.EIGYOSYO_CD ")                       ' 精算情報.営業所コード
        sql.AppendLine("       ,SSN.CREATE_DAY ")                        ' 精算情報.作成日
        sql.AppendLine("       ,SUM(SIUA.KINGAKU) AS NCREDITKINGAKU  ")  ' SUM(精算情報内訳.金額) AS 日報クレジット金額
        sql.AppendLine("       ,SUM(SIUB.KINGAKU) AS NCREDITMODOSHI ")    ' SUM(精算情報内訳.金額) AS 日報クレジット払戻
        sql.AppendLine("   FROM ")
        sql.AppendLine("        T_SEISAN_INFO SSN")  ' 精算情報
        sql.AppendLine("        LEFT JOIN T_SEISAN_INFO_UTIWAKE SIUA ")  ' 精算情報内訳A
        sql.AppendLine("           ON SIUA.SEISAN_INFO_SEQ = SSN.SEISAN_INFO_SEQ ")
        sql.AppendLine("          AND SIUA.SEISAN_KOUMOKU_CD = '" & FixedCd.SeisanItemCd.credit_card & "' ")
        sql.AppendLine("        LEFT JOIN T_SEISAN_INFO_UTIWAKE SIUB ")  ' 精算情報内訳B
        sql.AppendLine("           ON SIUB.SEISAN_INFO_SEQ = SSN.SEISAN_INFO_SEQ ")
        sql.AppendLine("          AND SIUB.SEISAN_KOUMOKU_CD = '" & FixedCd.SeisanItemCd.credit_card_modosi & "' ")
        With clsSeisanInfoEntity
            sql.AppendLine("  WHERE ")
            ' ノーサイン区分
            sql.AppendLine("        NVL(SSN.NOSIGN_KBN,' ') = ' ' ")
            ' 会社コード
            sql.AppendLine("    AND SSN.COMPANY_CD = " & setParam(.companyCd.PhysicsName, param.Item(.companyCd.PhysicsName), OracleDbType.Char))
            ' 必須項目：営業所コード
            sql.AppendLine("    AND SSN.EIGYOSYO_CD = " & setParam(.eigyosyoCd.PhysicsName, param.Item(.eigyosyoCd.PhysicsName), OracleDbType.Char))
            ' 必須項目：作成日
            sql.AppendLine("    AND SSN.CREATE_DAY = " & setParam(.createDay.PhysicsName, param.Item(.createDay.PhysicsName), OracleDbType.Decimal, 8, 0))
        End With
        ' グループ化
        sql.AppendLine(" GROUP BY ")
        sql.AppendLine("     SSN.COMPANY_CD ")   ' 精算情報.会社コード
        sql.AppendLine("   , SSN.EIGYOSYO_CD ")  ' 精算情報.営業所コード
        sql.AppendLine("   , SSN.CREATE_DAY ")   ' 精算情報.作成日
        ' 並び順
        sql.AppendLine(" ORDER BY ")
        sql.AppendLine("     SSN.COMPANY_CD ASC ")   ' 精算情報.会社コード 昇順
        sql.AppendLine("   , SSN.EIGYOSYO_CD ASC ")  ' 精算情報.営業所コード 昇順
        sql.AppendLine("   , SSN.CREATE_DAY ASC ")   ' 精算情報.作成日 昇順
        Return sql.ToString
    End Function

    ''' <summary>
    ''' 一覧情報取得
    ''' </summary>
    ''' <param name="param">検索条件</param>
    ''' <returns></returns>
    Public Function getGridData(param As Hashtable) As String
        Dim sql As New StringBuilder
        'SQL生成
        With clsSeisanInfoEntity
            sql.AppendLine(" SELECT ")
            sql.AppendLine("        KMC.CREDIT_CD AS CODE ")    ' 科目・クレジット会社対応マスタ.クレジット会社コード AS コード
            sql.AppendLine("       ,KMC.CREDIT_NAME AS NAME ")  ' 科目・クレジット会社対応マスタ.クレジット会社名称 AS 名称
            sql.AppendLine("       ,TO_CHAR(NVL(UIC.URIAGE_KENSU,0), '" & CommonFormatType.numberFormatComma & "') AS URIAGE_KENSU ")                ' 内訳情報（クレジット会社）.売上件数
            sql.AppendLine("       ,TO_CHAR(NVL(UIC.URIAGE_KINGAKU,0), '" & CommonFormatType.numberFormatCommaLargeDigit & "') AS URIAGE_KINGAKU ")  ' 内訳情報（クレジット会社）.売上金額
            sql.AppendLine("       ,TO_CHAR(NVL(UIC.RETURN_KENSU,0), '" & CommonFormatType.numberFormatComma & "') AS RETURN_KENSU ")                ' 内訳情報（クレジット会社）.戻し件数
            sql.AppendLine("       ,TO_CHAR(NVL(UIC.RETURN_KINGAKU,0), '" & CommonFormatType.numberFormatCommaLargeDigit & "') AS RETURN_KINGAKU ")  ' 内訳情報（クレジット会社）.戻し金額
            sql.AppendLine("       ,NVL(UIC.COMPANY_CD," & setParam(.companyCd.PhysicsName, param.Item(.companyCd.PhysicsName), OracleDbType.Char) & ") AS COMPANY_CD ")           ' 内訳情報（クレジット会社）.会社コード
            sql.AppendLine("       ,NVL(UIC.EIGYOSYO_CD, " & setParam(.eigyosyoCd.PhysicsName, param.Item(.eigyosyoCd.PhysicsName), OracleDbType.Char) & ") AS EIGYOSYO_CD ")          ' 内訳情報（クレジット会社）.営業所コード
            sql.AppendLine("       ,NVL(UIC.CREATE_DAY, " & setParam(.createDay.PhysicsName, param.Item(.createDay.PhysicsName), OracleDbType.Decimal, 8, 0) & ") AS CREATE_DAY ")           ' 内訳情報（クレジット会社）.作成日
            sql.AppendLine("   FROM ")
            sql.AppendLine("        M_KAMOKU_CREDIT KMC")  ' 科目・クレジット会社対応マスタ
            sql.AppendLine("        LEFT JOIN T_UTIWAKE_INFO_CREDIT_COMPANY UIC ")  ' 内訳情報（クレジット会社）
            sql.AppendLine("           ON UIC.CREDIT_COMPANY_CD = KMC.CREDIT_CD ")
            ' 会社コード
            sql.AppendLine("          AND UIC.COMPANY_CD = " & setParam(.companyCd.PhysicsName, param.Item(.companyCd.PhysicsName), OracleDbType.Char))
            ' 必須項目：営業所コード
            sql.AppendLine("          AND UIC.EIGYOSYO_CD = " & setParam(.eigyosyoCd.PhysicsName, param.Item(.eigyosyoCd.PhysicsName), OracleDbType.Char))
            ' 必須項目：作成日
            sql.AppendLine("          AND UIC.CREATE_DAY = " & setParam(.createDay.PhysicsName, param.Item(.createDay.PhysicsName), OracleDbType.Decimal, 8, 0))
            sql.AppendLine("  WHERE ")
            ' 削除日
            sql.AppendLine("        NVL(KMC.DELETE_DATE,'0') = '0' ")
        End With
        ' 並び順
        sql.AppendLine(" ORDER BY ")
        sql.AppendLine("     KMC.CREDIT_CD ASC ")  ' 科目・クレジット会社対応マスタ.クレジット会社コード 昇順
        Return sql.ToString
    End Function
#End Region

#Region "更新処理"

    ''' <summary>
    ''' 内訳情報(クレジット会社)更新
    ''' </summary>
    ''' <param name="updateInfoList"></param>
    ''' <returns></returns>
    Public Function executeTUtiwakeInfoCreditCompany(ByVal updateInfoList As DataTable) As Integer

        Dim oracleTransaction As OracleTransaction = Nothing
        Dim returnValue As Integer = 0
        Dim sqlString As String = String.Empty

        Try
            'トランザクション開始
            oracleTransaction = callBeginTransaction()

            If (Not updateInfoList Is Nothing) AndAlso (updateInfoList.Rows.Count > 0) Then
                '更新対象情報有り
                For i As Integer = 0 To updateInfoList.Rows.Count - 1
                    sqlString = executeUpdateTUtiwakeInfoCreditCompany(i, updateInfoList)

                    If execNonQuery(oracleTransaction, sqlString) > 0 Then
                        returnValue = 1
                        'パラメータの初期化[更新(前回)のパラメータをクリア]
                        paramClear()
                    Else
                        returnValue = 0
                        Exit For
                    End If
                Next
            End If

            If returnValue = 1 Then
                '一括登録成功
                'コミット
                callCommitTransaction(oracleTransaction)
            Else
                '一括登録失敗
                'ロールバック
                callRollbackTransaction(oracleTransaction)
            End If

        Catch ex As Exception
            Call callRollbackTransaction(oracleTransaction)
            Throw
        Finally
            Call oracleTransaction.Dispose()
        End Try

        Return returnValue

    End Function

    ''' <summary>
    ''' 内訳情報(クレジット会社)更新用
    ''' </summary>
    ''' <param name="targetRowIndex"></param>
    ''' <param name="updateInfoList"></param>
    ''' <returns></returns>
    Private Function executeUpdateTUtiwakeInfoCreditCompany(ByVal targetRowIndex As Integer, ByVal updateInfoList As DataTable) As String

        Dim sqlString As New StringBuilder
        Dim sysDate As Date = CommonDateUtil.getSystemTime

        With Me.clsUtiwakeInfoCreditCompanyEntity

            sqlString.AppendLine(" MERGE INTO T_UTIWAKE_INFO_CREDIT_COMPANY ")
            sqlString.AppendLine(" USING DUAL ON (T_UTIWAKE_INFO_CREDIT_COMPANY.COMPANY_CD = " & setParam(S05_0501.EditedDataTableColName.companyCd, updateInfoList.Rows(targetRowIndex).Item(S05_0501.EditedDataTableColName.companyCd), .CompanyCd.DBType, .CompanyCd.IntegerBu, .CompanyCd.DecimalBu))
            sqlString.AppendLine(" 　　　　　AND  T_UTIWAKE_INFO_CREDIT_COMPANY.EIGYOSYO_CD = " & setParam(S05_0501.EditedDataTableColName.eigyosyoCd, updateInfoList.Rows(targetRowIndex).Item(S05_0501.EditedDataTableColName.eigyosyoCd), .EigyosyoCd.DBType, .EigyosyoCd.IntegerBu, .EigyosyoCd.DecimalBu))
            sqlString.AppendLine(" 　　　　　AND  T_UTIWAKE_INFO_CREDIT_COMPANY.CREATE_DAY = " & setParam(S05_0501.EditedDataTableColName.createDay, updateInfoList.Rows(targetRowIndex).Item(S05_0501.EditedDataTableColName.createDay), .CreateDay.DBType, .CreateDay.IntegerBu, .CreateDay.DecimalBu))
            sqlString.AppendLine(" 　　　　　AND  T_UTIWAKE_INFO_CREDIT_COMPANY.CREDIT_COMPANY_CD = " & setParam(S05_0501.EditedDataTableColName.creditCompanyCd, updateInfoList.Rows(targetRowIndex).Item(S05_0501.EditedDataTableColName.creditCompanyCd), .CreditCompanyCd.DBType, .CreditCompanyCd.IntegerBu, .CreditCompanyCd.DecimalBu) & ") ")
            sqlString.AppendLine(" WHEN MATCHED THEN ")
            sqlString.AppendLine("   UPDATE SET ")
            sqlString.AppendLine("     URIAGE_KENSU = " & setParam(S05_0501.EditedDataTableColName.uriageKensu, updateInfoList.Rows(targetRowIndex).Item(S05_0501.EditedDataTableColName.uriageKensu), .UriageKensu.DBType, .UriageKensu.IntegerBu, .UriageKensu.DecimalBu))
            sqlString.AppendLine("    ,URIAGE_KINGAKU = " & setParam(S05_0501.EditedDataTableColName.uriageKingaku, updateInfoList.Rows(targetRowIndex).Item(S05_0501.EditedDataTableColName.uriageKingaku), .UriageKingaku.DBType, .UriageKingaku.IntegerBu, .UriageKingaku.DecimalBu))
            sqlString.AppendLine("    ,RETURN_KENSU = " & setParam(S05_0501.EditedDataTableColName.returnKensu, updateInfoList.Rows(targetRowIndex).Item(S05_0501.EditedDataTableColName.returnKensu), .ReturnKensu.DBType, .ReturnKensu.IntegerBu, .ReturnKensu.DecimalBu))
            sqlString.AppendLine("    ,RETURN_KINGAKU = " & setParam(S05_0501.EditedDataTableColName.returnKingaku, updateInfoList.Rows(targetRowIndex).Item(S05_0501.EditedDataTableColName.returnKingaku), .UriageKingaku.DBType, .UriageKingaku.IntegerBu, .UriageKingaku.DecimalBu))
            sqlString.AppendLine("    ,UPDATE_PERSON_CD = " & setParam(S05_0501.EditedDataTableColName.updatePersonCd, updateInfoList.Rows(targetRowIndex).Item(S05_0501.EditedDataTableColName.updatePersonCd), .UpdatePersonCd.DBType, .UpdatePersonCd.IntegerBu, .UpdatePersonCd.DecimalBu))
            sqlString.AppendLine("    ,UPDATE_PGMID = " & setParam(S05_0501.EditedDataTableColName.updatePgmid, updateInfoList.Rows(targetRowIndex).Item(S05_0501.EditedDataTableColName.updatePgmid), .UpdatePgmid.DBType, .UpdatePgmid.IntegerBu, .UpdatePgmid.DecimalBu))
            sqlString.AppendLine("    ,UPDATE_DAY = " & setParam(S05_0501.EditedDataTableColName.updateDay, sysDate.ToString(CommonFormatType.dateFormatyyyyMMdd), .UpdateDay.DBType, .UpdateDay.IntegerBu, .UpdateDay.DecimalBu))
            sqlString.AppendLine("    ,UPDATE_TIME = " & setParam(S05_0501.EditedDataTableColName.updateTime, sysDate.ToString("HHmmss"), .UpdateTime.DBType, .UpdateTime.IntegerBu, .UpdateTime.DecimalBu))
            sqlString.AppendLine("    ,SYSTEM_UPDATE_DAY = SYSDATE ")
            sqlString.AppendLine("    ,SYSTEM_UPDATE_PGMID = " & setParam(S05_0501.EditedDataTableColName.systemUpdatePgmid, updateInfoList.Rows(targetRowIndex).Item(S05_0501.EditedDataTableColName.systemUpdatePgmid), .SystemUpdatePgmid.DBType, .SystemUpdatePgmid.IntegerBu, .SystemUpdatePgmid.DecimalBu))
            sqlString.AppendLine("    ,SYSTEM_UPDATE_PERSON_CD = " & setParam(S05_0501.EditedDataTableColName.systemUpdatePerson_cd, updateInfoList.Rows(targetRowIndex).Item(S05_0501.EditedDataTableColName.systemUpdatePerson_cd), .SystemUpdatePersonCd.DBType, .SystemUpdatePersonCd.IntegerBu, .SystemUpdatePersonCd.DecimalBu))
            sqlString.AppendLine(" WHEN NOT MATCHED THEN ")
            sqlString.AppendLine("   INSERT( ")
            sqlString.AppendLine("      COMPANY_CD ")
            sqlString.AppendLine("     ,EIGYOSYO_CD ")
            sqlString.AppendLine("     ,CREATE_DAY ")
            sqlString.AppendLine("     ,CREDIT_COMPANY_CD ")
            sqlString.AppendLine("     ,URIAGE_KENSU ")
            sqlString.AppendLine("     ,URIAGE_KINGAKU ")
            sqlString.AppendLine("     ,RETURN_KENSU ")
            sqlString.AppendLine("     ,RETURN_KINGAKU ")
            sqlString.AppendLine("     ,ENTRY_PERSON_CD ")
            sqlString.AppendLine("     ,ENTRY_PGMID ")
            sqlString.AppendLine("     ,ENTRY_DAY ")
            sqlString.AppendLine("     ,ENTRY_TIME ")
            sqlString.AppendLine("     ,UPDATE_PERSON_CD ")
            sqlString.AppendLine("     ,UPDATE_PGMID ")
            sqlString.AppendLine("     ,UPDATE_DAY ")
            sqlString.AppendLine("     ,UPDATE_TIME ")
            sqlString.AppendLine("     ,SYSTEM_ENTRY_DAY ")
            sqlString.AppendLine("     ,SYSTEM_ENTRY_PGMID ")
            sqlString.AppendLine("     ,SYSTEM_ENTRY_PERSON_CD ")
            sqlString.AppendLine("     ,SYSTEM_UPDATE_DAY ")
            sqlString.AppendLine("     ,SYSTEM_UPDATE_PGMID ")
            sqlString.AppendLine("     ,SYSTEM_UPDATE_PERSON_CD) ")
            sqlString.AppendLine("   VALUES( ")
            sqlString.AppendLine("      " & setParam(S05_0501.EditedDataTableColName.companyCd, updateInfoList.Rows(targetRowIndex).Item(S05_0501.EditedDataTableColName.companyCd), .CompanyCd.DBType, .CompanyCd.IntegerBu, .CompanyCd.DecimalBu))
            sqlString.AppendLine("     ," & setParam(S05_0501.EditedDataTableColName.eigyosyoCd, updateInfoList.Rows(targetRowIndex).Item(S05_0501.EditedDataTableColName.eigyosyoCd), .EigyosyoCd.DBType, .EigyosyoCd.IntegerBu, .EigyosyoCd.DecimalBu))
            sqlString.AppendLine("     ," & setParam(S05_0501.EditedDataTableColName.createDay, updateInfoList.Rows(targetRowIndex).Item(S05_0501.EditedDataTableColName.createDay), .CreateDay.DBType, .CreateDay.IntegerBu, .CreateDay.DecimalBu))
            sqlString.AppendLine("     ," & setParam(S05_0501.EditedDataTableColName.creditCompanyCd, updateInfoList.Rows(targetRowIndex).Item(S05_0501.EditedDataTableColName.creditCompanyCd), .CreditCompanyCd.DBType, .CreditCompanyCd.IntegerBu, .CreditCompanyCd.DecimalBu))
            sqlString.AppendLine("     ," & setParam(S05_0501.EditedDataTableColName.uriageKensu, updateInfoList.Rows(targetRowIndex).Item(S05_0501.EditedDataTableColName.uriageKensu), .UriageKensu.DBType, .UriageKensu.IntegerBu, .UriageKensu.DecimalBu))
            sqlString.AppendLine("     ," & setParam(S05_0501.EditedDataTableColName.uriageKingaku, updateInfoList.Rows(targetRowIndex).Item(S05_0501.EditedDataTableColName.uriageKingaku), .UriageKingaku.DBType, .UriageKingaku.IntegerBu, .UriageKingaku.DecimalBu))
            sqlString.AppendLine("     ," & setParam(S05_0501.EditedDataTableColName.returnKensu, updateInfoList.Rows(targetRowIndex).Item(S05_0501.EditedDataTableColName.returnKensu), .ReturnKensu.DBType, .ReturnKensu.IntegerBu, .ReturnKensu.DecimalBu))
            sqlString.AppendLine("     ," & setParam(S05_0501.EditedDataTableColName.returnKingaku, updateInfoList.Rows(targetRowIndex).Item(S05_0501.EditedDataTableColName.returnKingaku), .UriageKingaku.DBType, .UriageKingaku.IntegerBu, .UriageKingaku.DecimalBu))
            sqlString.AppendLine("     ," & setParam(S05_0501.EditedDataTableColName.updatePersonCd, updateInfoList.Rows(targetRowIndex).Item(S05_0501.EditedDataTableColName.updatePersonCd), .UpdatePersonCd.DBType, .UpdatePersonCd.IntegerBu, .UpdatePersonCd.DecimalBu))
            sqlString.AppendLine("     ," & setParam(S05_0501.EditedDataTableColName.updatePgmid, updateInfoList.Rows(targetRowIndex).Item(S05_0501.EditedDataTableColName.updatePgmid), .UpdatePgmid.DBType, .UpdatePgmid.IntegerBu, .UpdatePgmid.DecimalBu))
            sqlString.AppendLine("     ," & setParam(S05_0501.EditedDataTableColName.updateDay, sysDate.ToString(CommonFormatType.dateFormatyyyyMMdd), .UpdateDay.DBType, .UpdateDay.IntegerBu, .UpdateDay.DecimalBu))
            sqlString.AppendLine("     ," & setParam(S05_0501.EditedDataTableColName.updateTime, sysDate.ToString("HHmmss"), .UpdateTime.DBType, .UpdateTime.IntegerBu, .UpdateTime.DecimalBu))
            sqlString.AppendLine("     ," & setParam(S05_0501.EditedDataTableColName.updatePersonCd, updateInfoList.Rows(targetRowIndex).Item(S05_0501.EditedDataTableColName.updatePersonCd), .UpdatePersonCd.DBType, .UpdatePersonCd.IntegerBu, .UpdatePersonCd.DecimalBu))
            sqlString.AppendLine("     ," & setParam(S05_0501.EditedDataTableColName.updatePgmid, updateInfoList.Rows(targetRowIndex).Item(S05_0501.EditedDataTableColName.updatePgmid), .UpdatePgmid.DBType, .UpdatePgmid.IntegerBu, .UpdatePgmid.DecimalBu))
            sqlString.AppendLine("     ," & setParam(S05_0501.EditedDataTableColName.updateDay, sysDate.ToString(CommonFormatType.dateFormatyyyyMMdd), .UpdateDay.DBType, .UpdateDay.IntegerBu, .UpdateDay.DecimalBu))
            sqlString.AppendLine("     ," & setParam(S05_0501.EditedDataTableColName.updateTime, sysDate.ToString("HHmmss"), .UpdateTime.DBType, .UpdateTime.IntegerBu, .UpdateTime.DecimalBu))
            sqlString.AppendLine("     ,SYSDATE ")
            sqlString.AppendLine("     ," & setParam(S05_0501.EditedDataTableColName.systemUpdatePgmid, updateInfoList.Rows(targetRowIndex).Item(S05_0501.EditedDataTableColName.systemUpdatePgmid), .SystemUpdatePgmid.DBType, .SystemUpdatePgmid.IntegerBu, .SystemUpdatePgmid.DecimalBu))
            sqlString.AppendLine("     ," & setParam(S05_0501.EditedDataTableColName.systemUpdatePerson_cd, updateInfoList.Rows(targetRowIndex).Item(S05_0501.EditedDataTableColName.systemUpdatePerson_cd), .SystemUpdatePersonCd.DBType, .SystemUpdatePersonCd.IntegerBu, .SystemUpdatePersonCd.DecimalBu))
            sqlString.AppendLine("     ,SYSDATE ")
            sqlString.AppendLine("     ," & setParam(S05_0501.EditedDataTableColName.systemUpdatePgmid, updateInfoList.Rows(targetRowIndex).Item(S05_0501.EditedDataTableColName.systemUpdatePgmid), .SystemUpdatePgmid.DBType, .SystemUpdatePgmid.IntegerBu, .SystemUpdatePgmid.DecimalBu))
            sqlString.AppendLine("     ," & setParam(S05_0501.EditedDataTableColName.systemUpdatePerson_cd, updateInfoList.Rows(targetRowIndex).Item(S05_0501.EditedDataTableColName.systemUpdatePerson_cd), .SystemUpdatePersonCd.DBType, .SystemUpdatePersonCd.IntegerBu, .SystemUpdatePersonCd.DecimalBu) & " )")

            ''UPDATE
            'sqlString.AppendLine(" UPDATE T_UTIWAKE_INFO_CREDIT_COMPANY ")
            'sqlString.AppendLine(" SET ")
            'sqlString.AppendLine(" URIAGE_KENSU = " & setParam(S05_0501.EditedDataTableColName.uriageKensu, updateInfoList.Rows(targetRowIndex).Item(S05_0501.EditedDataTableColName.uriageKensu), .UriageKensu.DBType, .UriageKensu.IntegerBu, .UriageKensu.DecimalBu))
            'sqlString.AppendLine(",URIAGE_KINGAKU = " & setParam(S05_0501.EditedDataTableColName.uriageKingaku, updateInfoList.Rows(targetRowIndex).Item(S05_0501.EditedDataTableColName.uriageKingaku), .UriageKingaku.DBType, .UriageKingaku.IntegerBu, .UriageKingaku.DecimalBu))
            'sqlString.AppendLine(",RETURN_KENSU = " & setParam(S05_0501.EditedDataTableColName.returnKensu, updateInfoList.Rows(targetRowIndex).Item(S05_0501.EditedDataTableColName.returnKensu), .ReturnKensu.DBType, .ReturnKensu.IntegerBu, .ReturnKensu.DecimalBu))
            'sqlString.AppendLine(",RETURN_KINGAKU = " & setParam(S05_0501.EditedDataTableColName.returnKingaku, updateInfoList.Rows(targetRowIndex).Item(S05_0501.EditedDataTableColName.returnKingaku), .UriageKingaku.DBType, .UriageKingaku.IntegerBu, .UriageKingaku.DecimalBu))
            'sqlString.AppendLine(",UPDATE_PERSON_CD = " & setParam(S05_0501.EditedDataTableColName.updatePersonCd, updateInfoList.Rows(targetRowIndex).Item(S05_0501.EditedDataTableColName.updatePersonCd), .UpdatePersonCd.DBType, .UpdatePersonCd.IntegerBu, .UpdatePersonCd.DecimalBu))
            'sqlString.AppendLine(",UPDATE_PGMID = " & setParam(S05_0501.EditedDataTableColName.updatePgmid, updateInfoList.Rows(targetRowIndex).Item(S05_0501.EditedDataTableColName.updatePgmid), .UpdatePgmid.DBType, .UpdatePgmid.IntegerBu, .UpdatePgmid.DecimalBu))
            'sqlString.AppendLine(",UPDATE_DAY = TO_NUMBER(TO_CHAR(SYSDATE,'" & CommonFormatType.dateFormatyyyyMMdd & "')) ")
            'sqlString.AppendLine(",UPDATE_TIME = TO_NUMBER(TO_CHAR(SYSDATE,'HH24MISS')) ")
            'sqlString.AppendLine(",SYSTEM_UPDATE_DAY = " & setParam(S05_0501.EditedDataTableColName.systemUpdateDay, CommonDateUtil.getSystemTime, OracleDbType.Date))
            'sqlString.AppendLine(",SYSTEM_UPDATE_PGMID = " & setParam(S05_0501.EditedDataTableColName.systemUpdatePgmid, updateInfoList.Rows(targetRowIndex).Item(S05_0501.EditedDataTableColName.systemUpdatePgmid), .SystemUpdatePgmid.DBType, .SystemUpdatePgmid.IntegerBu, .SystemUpdatePgmid.DecimalBu))
            'sqlString.AppendLine(",SYSTEM_UPDATE_PERSON_CD = " & setParam(S05_0501.EditedDataTableColName.systemUpdatePerson_cd, updateInfoList.Rows(targetRowIndex).Item(S05_0501.EditedDataTableColName.systemUpdatePerson_cd), .SystemUpdatePersonCd.DBType, .SystemUpdatePersonCd.IntegerBu, .SystemUpdatePersonCd.DecimalBu))
            ''WHERE句
            'sqlString.AppendLine(" WHERE ")
            'sqlString.AppendLine(" COMPANY_CD = " & setParam(S05_0501.EditedDataTableColName.companyCd, updateInfoList.Rows(targetRowIndex).Item(S05_0501.EditedDataTableColName.companyCd), .CompanyCd.DBType, .CompanyCd.IntegerBu, .CompanyCd.DecimalBu))
            'sqlString.AppendLine(" AND ")
            'sqlString.AppendLine(" EIGYOSYO_CD = " & setParam(S05_0501.EditedDataTableColName.eigyosyoCd, updateInfoList.Rows(targetRowIndex).Item(S05_0501.EditedDataTableColName.eigyosyoCd), .EigyosyoCd.DBType, .EigyosyoCd.IntegerBu, .EigyosyoCd.DecimalBu))
            'sqlString.AppendLine(" AND ")
            'sqlString.AppendLine(" CREATE_DAY = " & setParam(S05_0501.EditedDataTableColName.createDay, updateInfoList.Rows(targetRowIndex).Item(S05_0501.EditedDataTableColName.createDay), .CreateDay.DBType, .CreateDay.IntegerBu, .CreateDay.DecimalBu))
            'sqlString.AppendLine(" AND ")
            'sqlString.AppendLine(" CREDIT_COMPANY_CD = " & setParam(S05_0501.EditedDataTableColName.creditCompanyCd, updateInfoList.Rows(targetRowIndex).Item(S05_0501.EditedDataTableColName.creditCompanyCd), .CreditCompanyCd.DBType, .CreditCompanyCd.IntegerBu, .CreditCompanyCd.DecimalBu))

            Return sqlString.ToString
        End With

    End Function
#End Region

End Class