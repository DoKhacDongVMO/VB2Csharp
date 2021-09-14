Imports System.Text

''' <summary>
''' バス会社一括登録のDAクラス
''' </summary>
Public Class BusCompanyBulk_DA
    Inherits DataAccessorBase

#Region " 定数／変数 "

    Public Enum accessType As Integer
        getBusCompanyBulk                 '一覧結果取得検索
        executeUpdateBusCompanyCd         '更新
        executeReverseBusCompanyCd        '戻し
    End Enum

    Private Const paramSyuptDayFrom As String = "SYUPT_DAY_FROM"
    Private Const paramSyuptDayTo As String = "SYUPT_DAY_TO"
    Private Const paramBusCompanyCdOnlyBlank As String = "BUS_COMPANY_CD_ONLY_BLANK"

#End Region

#Region " SELECT処理 "

    ''' <summary>
    ''' SELECT用DBアクセス
    ''' </summary>
    ''' <param name="type"></param>
    ''' <param name="paramInfoList"></param>
    ''' <returns></returns>
    Public Function accessBusCompanyBulkTehai(ByVal type As accessType, Optional ByVal paramInfoList As Hashtable = Nothing) As DataTable
        'SQL文字列
        Dim sqlString As String = String.Empty
        '戻り値
        Dim returnValue As DataTable = Nothing

        Select Case type
            Case accessType.getBusCompanyBulk
                '一覧結果取得検索
                sqlString = getBusCompanyBulk(paramInfoList)
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
    ''' <returns>SQL分</returns>
    ''' <remarks></remarks>
    Private Function getBusCompanyBulk(ByVal paramList As Hashtable) As String

        Dim clsCrsLedgerBasicEntity As New CrsLedgerBasicEntity

        Dim sqlString As New StringBuilder
        paramClear()

        With clsCrsLedgerBasicEntity

            'パラメータ設定
            setParam(paramSyuptDayFrom, paramList.Item(paramSyuptDayFrom), .syuptDay.DBType, .syuptDay.IntegerBu, .syuptDay.DecimalBu)
            If Not paramList.Item(paramSyuptDayTo) Is Nothing Then
                setParam(paramSyuptDayTo, paramList.Item(paramSyuptDayTo), .syuptDay.DBType, .syuptDay.IntegerBu, .syuptDay.DecimalBu)
            End If
            setParam(.crsCd.PhysicsName, paramList.Item(.crsCd.PhysicsName), .crsCd.DBType, .crsCd.IntegerBu, .crsCd.DecimalBu)

            'SELECT句
            sqlString.AppendLine(" SELECT ")
            sqlString.AppendLine(" TO_CHAR(TO_DATE(BASIC.SYUPT_DAY),'yyyy/MM/dd') AS SYUPT_DAY ")
            sqlString.AppendLine(",BASIC.CRS_CD ")
            sqlString.AppendLine(",BASIC.CRS_NAME ")
            sqlString.AppendLine(",TO_CHAR(BASIC.GOUSYA) AS GOUSYA ")
            sqlString.AppendLine(",CODE_S.CODE_NAME AS SAIKOU_KAKUTEI_KBN")
            sqlString.AppendLine(",PLACE.PLACE_NAME_1 AS PLACE_NAME ")
            sqlString.AppendLine(",BASIC.HAISYA_KEIYU_CD_1 ")
            sqlString.AppendLine("," & CommonKyushuUtil.setSQLTime24Format("BASIC.SYUPT_TIME_1") & " AS SYUPT_TIME ")
            sqlString.AppendLine(",BASIC.BUS_COMPANY_CD ")
            sqlString.AppendLine(",BUS_COMPANY.SIIRE_SAKI_NAME AS BUS_COMPANY_NAME ")
            sqlString.AppendLine(",BASIC.USING_FLG ")
            sqlString.AppendLine(",' ' AS DIFFERENCE_FLG ")
            sqlString.AppendLine(",' ' AS UPDATE_TARGET_FLG ")
            sqlString.AppendLine(",BASIC.TEIKI_KIKAKU_KBN ")
            'FROM句
            sqlString.AppendLine(" FROM ")
            sqlString.AppendLine("T_CRS_LEDGER_BASIC BASIC ")
            sqlString.AppendLine("LEFT JOIN M_PLACE PLACE ON BASIC.HAISYA_KEIYU_CD_1 = PLACE.PLACE_CD ")
            sqlString.AppendLine("LEFT JOIN M_SIIRE_SAKI BUS_COMPANY ON BASIC.BUS_COMPANY_CD = BUS_COMPANY.SIIRE_SAKI_CD || BUS_COMPANY.SIIRE_SAKI_NO ")
            sqlString.AppendLine("AND RTRIM(BUS_COMPANY.DELETE_DATE) IS NULL ")
            sqlString.AppendLine("LEFT JOIN M_CODE CODE_S ON CODE_S.CODE_BUNRUI = '" & CommonType_MojiColValue.CdBunruiType_Value(CdBunruiType.saikouKakuteiKbn) & "' AND BASIC.SAIKOU_KAKUTEI_KBN = CODE_S.CODE_VALUE ")
            'WHERE句
            sqlString.AppendLine(" WHERE ")
            '出発日（To）が設定されていない場合
            If paramList.Item(paramSyuptDayTo) Is Nothing Then
                sqlString.AppendLine(" BASIC.SYUPT_DAY = :   " & paramSyuptDayFrom)
            Else
                sqlString.AppendLine(" BASIC.SYUPT_DAY >= :" & paramSyuptDayFrom)
                sqlString.AppendLine(" AND ")
                sqlString.AppendLine(" BASIC.SYUPT_DAY <= :" & paramSyuptDayTo)
            End If
            sqlString.AppendLine(" AND ")
            sqlString.AppendLine(" BASIC.CRS_CD = :" & .crsCd.PhysicsName)
            'バス会社コード未記入分のみにチェックが入っている場合
            If Not paramList.Item(paramBusCompanyCdOnlyBlank) Is Nothing Then
                sqlString.AppendLine(" AND")
                sqlString.AppendLine(" RTRIM(BASIC.BUS_COMPANY_CD) IS NULL")
            End If
            sqlString.AppendLine(" AND ")
            sqlString.AppendLine(" NVL(BASIC.SAIKOU_KAKUTEI_KBN,'*') NOT IN('" & FixedCd.SaikouKakuteiKbn.Tyushi & "','" & SaikouKakuteiKbn.Haishi & "') ")
            sqlString.AppendLine("     AND NVL(BASIC.MARU_ZOU_MANAGEMENT_KBN,'*') <> '" & FixedCd.MaruzouKanriKbn.Maruzou & "' ")
            sqlString.AppendLine(" AND ")
            sqlString.AppendLine(" NVL(BASIC.SYUPT_JI_CARRIER_KBN,'1') = '" & FixedCd.SyuptJiCarrierKbnType.bus & "'")
            sqlString.AppendLine(" AND ")
            sqlString.AppendLine(" NVL(BASIC.CAR_TYPE_CD,'*') <> '" & FixedCd.CarTypeCdKakuu & "'")
            sqlString.AppendLine(" AND ")
            sqlString.AppendLine(" NVL(BASIC.DELETE_DAY,0) = 0 ")
            'ORDER BY句
            sqlString.AppendLine(" ORDER BY ")
            sqlString.AppendLine(" BASIC.SYUPT_DAY ")
            sqlString.AppendLine(" ,BASIC.CRS_CD ")
            sqlString.AppendLine(" ,BASIC.SYUPT_TIME_1 ")
            sqlString.AppendLine(" ,BASIC.GOUSYA ")
        End With

        Return sqlString.ToString

    End Function

#End Region


#Region " UPDATE処理 "

    ''' <summary>
    ''' 使用中フラグ更新処理
    ''' </summary>
    ''' <param name="paramDatatable"></param>
    ''' <param name="paramPgmId"></param>
    ''' <returns>DataTable</returns>
    ''' <remarks></remarks>
    Public Function updateUsingFlag(ByVal paramDatatable As DataTable, ByVal paramPgmId As String) As DataTable
        Dim oracleTransaction As OracleTransaction = Nothing
        Dim returnValue As New DataTable
        Dim sqlString As String = String.Empty
        Dim clsCrsLedgerBasicEntity As New CrsLedgerBasicEntity

        Try

            'トランザクション開始
            oracleTransaction = callBeginTransaction()

            With clsCrsLedgerBasicEntity
                For Each row As DataRow In paramDatatable.Rows
                    If String.IsNullOrEmpty(CType(row("DIFFERENCE_FLG"), String).Trim()) = False Then
                        If CommonCheckUtil.setUsingFlg_Crs(Replace(CType(row(.syuptDay.PhysicsName), String), "/", ""), CType(row(.crsCd.PhysicsName), String), CType(row(.gousya.PhysicsName), String), paramPgmId, oracleTransaction, True) = True Then
                            row("UPDATE_TARGET_FLG") = S03_0501.Targetflg.Target
                        End If
                    End If
                Next
            End With

            'コミット
            Call callCommitTransaction(oracleTransaction)

        Catch ex As Exception
            Call callRollbackTransaction(oracleTransaction)
            Throw

        Finally
            Call oracleTransaction.Dispose()
        End Try

        Return paramDatatable

    End Function

    ''' <summary>
    ''' 更新処理
    ''' </summary>
    ''' <param name="accessType">処理タイプ</param>
    ''' <param name="paramInfoList"></param>
    ''' <returns>更新処理件数</returns>
    ''' <remarks></remarks>
    Public Function updateBusCompanyCd(ByVal accessType As BusCompanyBulk_DA.accessType, ByVal paramInfoList As Hashtable) As Integer
        Dim oracleTransaction As OracleTransaction = Nothing
        Dim returnValue As Integer = 0
        Dim sqlString As String = String.Empty

        Try

            'トランザクション開始
            oracleTransaction = callBeginTransaction()

            For i As Integer = 1 To 2
                Select Case i
                    Case 1  'コース台帳（基本）
                        sqlString = executeUpdateBasicData(accessType, paramInfoList)
                    Case 2  'コース台帳（バス紐づけ）
                        sqlString = executeUpdateHimodukeData(accessType, paramInfoList)
                End Select

                returnValue += execNonQuery(oracleTransaction, sqlString)
            Next

            If returnValue > 1 Then
                'コミット
                Call callCommitTransaction(oracleTransaction)
            Else
                'ロールバック
                Call callRollbackTransaction(oracleTransaction)
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
    ''' コース台帳（基本）：データ更新用
    ''' </summary>
    ''' <param name="accessType">処理タイプ</param>
    ''' <param name="paramList">SQL引数</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function executeUpdateBasicData(ByVal accessType As accessType, ByVal paramList As Hashtable) As String

        Dim clsCrsLedgerBasicEntity As New CrsLedgerBasicEntity
        Dim sqlString As New StringBuilder
        paramClear()


        With clsCrsLedgerBasicEntity
            'パラメータ設定
            setParam(.syuptDay.PhysicsName, paramList.Item(.syuptDay.PhysicsName), .syuptDay.DBType, .syuptDay.IntegerBu, .syuptDay.DecimalBu)
            setParam(.crsCd.PhysicsName, paramList.Item(.crsCd.PhysicsName), .crsCd.DBType, .crsCd.IntegerBu, .crsCd.DecimalBu)
            setParam(.gousya.PhysicsName, paramList.Item(.gousya.PhysicsName), .gousya.DBType, .gousya.IntegerBu, .gousya.DecimalBu)
            If accessType = accessType.executeUpdateBusCompanyCd Then
                setParam(.busCompanyCd.PhysicsName, paramList.Item(.busCompanyCd.PhysicsName), .busCompanyCd.DBType, .busCompanyCd.IntegerBu, .busCompanyCd.DecimalBu)
            End If
            'setParam(.usingFlg.PhysicsName, paramList.Item(.usingFlg.PhysicsName), .usingFlg.DBType, .usingFlg.IntegerBu, .usingFlg.DecimalBu)
            setParam(.systemUpdateDay.PhysicsName, paramList.Item(.systemUpdateDay.PhysicsName), .systemUpdateDay.DBType, .systemUpdateDay.IntegerBu, .systemUpdateDay.DecimalBu)
            setParam(.systemUpdatePgmid.PhysicsName, paramList.Item(.systemUpdatePgmid.PhysicsName), .systemUpdatePgmid.DBType, .systemUpdatePgmid.IntegerBu, .systemUpdatePgmid.DecimalBu)
            setParam(.systemUpdatePersonCd.PhysicsName, paramList.Item(.systemUpdatePersonCd.PhysicsName), .systemUpdatePersonCd.DBType, .systemUpdatePersonCd.IntegerBu, .systemUpdatePersonCd.DecimalBu)

            'UPDATE
            sqlString.AppendLine(" UPDATE T_CRS_LEDGER_BASIC ")
            sqlString.AppendLine(" SET ")
            sqlString.AppendLine(" USING_FLG = NULL")
            If accessType = accessType.executeUpdateBusCompanyCd Then
                sqlString.AppendLine(",BUS_COMPANY_CD = :" & .busCompanyCd.PhysicsName)
            End If
            sqlString.AppendLine(",SYSTEM_UPDATE_DAY = :" & .systemUpdateDay.PhysicsName)
            sqlString.AppendLine(",SYSTEM_UPDATE_PGMID = :" & .systemUpdatePgmid.PhysicsName)
            sqlString.AppendLine(",SYSTEM_UPDATE_PERSON_CD = :" & .systemUpdatePersonCd.PhysicsName)
            'WHERE句
            sqlString.AppendLine(" WHERE ")
            sqlString.AppendLine(" SYUPT_DAY = :" & .syuptDay.PhysicsName)
            sqlString.AppendLine(" AND ")
            sqlString.AppendLine(" CRS_CD = :" & .crsCd.PhysicsName)
            sqlString.AppendLine(" AND ")
            sqlString.AppendLine(" GOUSYA = :" & .gousya.PhysicsName)
        End With
        Return sqlString.ToString

    End Function

    ''' <summary>
    ''' コース台帳（バス紐づけ）：データ更新用
    ''' </summary>
    ''' <param name="accessType">処理タイプ</param>
    ''' <param name="paramList">SQL引数</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function executeUpdateHimodukeData(ByVal accessType As accessType, ByVal paramList As Hashtable) As String

        Dim clsCrsLedgerHimodukeEntity As New TCrsLedgerBusHimodukeEntity
        Dim sqlString As New StringBuilder
        paramClear()


        With clsCrsLedgerHimodukeEntity
            'パラメータ設定
            setParam(.syuptDay.PhysicsName, paramList.Item(.syuptDay.PhysicsName), .syuptDay.DBType, .syuptDay.IntegerBu, .syuptDay.DecimalBu)
            setParam(.crsCd.PhysicsName, paramList.Item(.crsCd.PhysicsName), .crsCd.DBType, .crsCd.IntegerBu, .crsCd.DecimalBu)
            setParam(.gousya.PhysicsName, paramList.Item(.gousya.PhysicsName), .gousya.DBType, .gousya.IntegerBu, .gousya.DecimalBu)
            If accessType = accessType.executeUpdateBusCompanyCd Then
                setParam(.busCompanyCd.PhysicsName, paramList.Item(.busCompanyCd.PhysicsName), .busCompanyCd.DBType, .busCompanyCd.IntegerBu, .busCompanyCd.DecimalBu)
            End If
            setParam(.systemUpdateDay.PhysicsName, paramList.Item(.systemUpdateDay.PhysicsName), .systemUpdateDay.DBType, .systemUpdateDay.IntegerBu, .systemUpdateDay.DecimalBu)
            setParam(.systemUpdatePgmid.PhysicsName, paramList.Item(.systemUpdatePgmid.PhysicsName), .systemUpdatePgmid.DBType, .systemUpdatePgmid.IntegerBu, .systemUpdatePgmid.DecimalBu)
            setParam(.systemUpdatePersonCd.PhysicsName, paramList.Item(.systemUpdatePersonCd.PhysicsName), .systemUpdatePersonCd.DBType, .systemUpdatePersonCd.IntegerBu, .systemUpdatePersonCd.DecimalBu)

            'UPDATE
            sqlString.AppendLine(" UPDATE T_CRS_LEDGER_BUS_HIMODUKE ")
            sqlString.AppendLine(" SET ")
            If accessType = accessType.executeUpdateBusCompanyCd Then
                sqlString.AppendLine("BUS_COMPANY_CD = :" & .busCompanyCd.PhysicsName)
            End If
            sqlString.AppendLine(",BUS_COMPANY_KAKUTEI_DAY = " & setParam("BUS_COMPANY_KAKUTEI_DAY", paramList.Item("BUS_COMPANY_KAKUTEI_DAY"), OracleDbType.Decimal, 8, 0))
            sqlString.AppendLine(",BUS_COMPANY_KAKUTEI_TIME = " & setParam("BUS_COMPANY_KAKUTEI_TIME", paramList.Item("BUS_COMPANY_KAKUTEI_TIME"), OracleDbType.Decimal, 6, 0))
            sqlString.AppendLine(",OLD_BUS_COMPANY_CD = " & setParam("OLD_BUS_COMPANY_CD", paramList.Item("OLD_BUS_COMPANY_CD"), OracleDbType.Char))
            sqlString.AppendLine(",SYSTEM_UPDATE_DAY = :" & .systemUpdateDay.PhysicsName)
            sqlString.AppendLine(",SYSTEM_UPDATE_PGMID = :" & .systemUpdatePgmid.PhysicsName)
            sqlString.AppendLine(",SYSTEM_UPDATE_PERSON_CD = :" & .systemUpdatePersonCd.PhysicsName)
            'WHERE句
            sqlString.AppendLine(" WHERE ")
            sqlString.AppendLine(" SYUPT_DAY = :" & .syuptDay.PhysicsName)
            sqlString.AppendLine(" AND ")
            sqlString.AppendLine(" CRS_CD = :" & .crsCd.PhysicsName)
            sqlString.AppendLine(" AND ")
            sqlString.AppendLine(" GOUSYA = :" & .gousya.PhysicsName)
        End With
        Return sqlString.ToString

    End Function
#End Region
End Class
