Imports System.Text


''' <summary>
''' 座席イメージのDAクラス
''' </summary>
Public Class ZasekiImage_DA
    Inherits DataAccessorBase

#Region " 定数／変数 "

    Public Enum accessType As Integer
        getZasekiBlockInfo              '座席ブロック情報結果取得検索
        getZasekiBlockInfoRjc           '座席ブロック情報結果取得検索
        getCrsLegBasic                  'コース台帳(基本)取得
        getCrsLegReserveBasic           'コース台帳(基本)取得（乗せ合わせ）
        getZasekiImage                  '座席イメージ（バス情報）取得
        updateZasekiBlockInfo           '座席ブロック情報更新
        updateZasekiBlockInfoRjc        '座席ブロック情報更新
    End Enum

    Public Enum updateType As Integer
        regist                          '登録
        reject                          '解除
    End Enum

#End Region

#Region " SELECT処理 "

    ''' <summary>
    ''' SELECT用DBアクセス
    ''' </summary>
    ''' <param name="paramInfoList"></param>
    ''' <returns></returns>
    Public Function accessZasekiBlockInfo(ByVal selectType As accessType, ByVal paramInfoList As Hashtable) As DataTable

        'SQL文字列
        Dim sqlString As String = String.Empty
        '戻り値
        Dim returnValue As DataTable = Nothing

        MyBase.paramClear()

        Select Case selectType
            Case accessType.getZasekiBlockInfo
                sqlString = getZasekiBlockInfo(updateType.regist, paramInfoList)
            Case accessType.getZasekiBlockInfoRjc
                sqlString = getZasekiBlockInfo(updateType.reject, paramInfoList)
            Case accessType.getCrsLegBasic
                sqlString = getCrsLegBasic(paramInfoList)
            Case accessType.getZasekiImage
                sqlString = getZasekiImage(paramInfoList)
            Case accessType.getCrsLegReserveBasic
                sqlString = getCrsLegReserveBasic(paramInfoList)
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
    Protected Overloads Function getZasekiBlockInfo(ByVal type As updateType, ByVal paramList As Hashtable) As String

        Dim sqlString As New StringBuilder
        Dim zasekiEnt As New TZasekiImageEntity
        paramClear()

        'SELECT句
        sqlString.AppendLine("SELECT ")
        sqlString.AppendLine("  INFO.ZASEKI_KAI ")
        sqlString.AppendLine("  , INFO.ZASEKI_LINE ")
        sqlString.AppendLine("  , INFO.ZASEKI_COL ")
        sqlString.AppendLine("  , INFO.ZASEKI_KIND ")
        sqlString.AppendLine("  , INFO.ZASEKI_KBN ")
        sqlString.AppendLine("  , INFO.BLOCK_KBN ")
        sqlString.AppendLine("  , INFO.EIGYOSYO_BLOCK_KBN ")
        sqlString.AppendLine("  , INFO.EIGYOSYO_CD ")
        sqlString.AppendLine("  , EIG.EIGYOSYO_NAME_PRINT_2 AS EIGYOSYO_NM ")
        sqlString.AppendLine("  , INFO.YOYAKU_FLG ")
        sqlString.AppendLine("  , INFO.LADIES_SEAT_FLG ")
        If type = updateType.regist Then
            sqlString.AppendLine("  , CASE ")
            sqlString.AppendLine("    WHEN INFO.ZASEKI_KIND = 'X' ")
            sqlString.AppendLine("    OR INFO.BLOCK_KBN = '2' ")
            'sqlString.AppendLine("    OR ( ")
            'sqlString.AppendLine("      IMAGE.SUB_SEAT_OK_KBN IS NULL ")
            'sqlString.AppendLine("      AND INFO.ZASEKI_KIND = '2' ")
            'sqlString.AppendLine("    ) ")
            sqlString.AppendLine("      THEN 1 ")
            sqlString.AppendLine("    WHEN INFO.BLOCK_KBN = '1' AND INFO.EIGYOSYO_BLOCK_KBN = '1' ")
            sqlString.AppendLine("      THEN 2 ")
            sqlString.AppendLine("    WHEN INFO.YOYAKU_FLG = '1' ")
            sqlString.AppendLine("      THEN 3 ")
            sqlString.AppendLine("    WHEN INFO.LADIES_SEAT_FLG = '1' ")
            sqlString.AppendLine("      THEN 4 ")
            sqlString.AppendLine("    WHEN INFO.EIGYOSYO_BLOCK_KBN = '1' ")
            sqlString.AppendLine("      THEN 5 ")
            sqlString.AppendLine("    ELSE 0 ")
            sqlString.AppendLine("    END EDIT_FLG ")
        Else
            sqlString.AppendLine("  , CASE ")
            sqlString.AppendLine("    WHEN INFO.BLOCK_KBN = '2' ")
            sqlString.AppendLine("      THEN 1 ")
            sqlString.AppendLine("    WHEN INFO.EIGYOSYO_BLOCK_KBN = '1' AND INFO.EIGYOSYO_CD = '" & UserInfoManagement.eigyosyoCd & "' ")
            sqlString.AppendLine("      THEN 0 ")
            sqlString.AppendLine("    WHEN INFO.EIGYOSYO_BLOCK_KBN = '1' AND INFO.EIGYOSYO_CD <> '" & UserInfoManagement.eigyosyoCd & "' ")
            sqlString.AppendLine("      THEN 1 ")
            sqlString.AppendLine("    ELSE 2 ")
            sqlString.AppendLine("    END EDIT_FLG ")
        End If
        sqlString.AppendLine("  , INFO.ZASEKI_STATE")
        sqlString.AppendLine("FROM ")
        sqlString.AppendLine("  T_ZASEKI_IMAGE IMAGE ")
        sqlString.AppendLine("  INNER JOIN T_ZASEKI_IMAGE_INFO INFO ")
        sqlString.AppendLine("    ON IMAGE.BUS_RESERVE_CD = INFO.BUS_RESERVE_CD ")
        sqlString.AppendLine("    AND IMAGE.SYUPT_DAY = INFO.SYUPT_DAY ")
        sqlString.AppendLine("    AND IMAGE.GOUSYA = INFO.GOUSYA ")
        sqlString.AppendLine("    AND INFO.ZASEKI_KIND <> '" & FixedCd.ZasekiKind.dummySeat & "' ")
        sqlString.AppendLine("    AND INFO.ZASEKI_KBN = '" & FixedCd.ZasekiKbnType.zaseki & "' ")
        sqlString.AppendLine("  LEFT JOIN M_EIGYOSYO EIG ")
        sqlString.AppendLine("    ON INFO.EIGYOSYO_CD = EIG.EIGYOSYO_CD ")
        sqlString.AppendLine("    AND EIG.COMPANY_CD = '00' ")
        sqlString.AppendLine("WHERE ")
        sqlString.AppendLine("  IMAGE.BUS_RESERVE_CD = " & setParamEx(zasekiEnt.busReserveCd, paramList))
        sqlString.AppendLine("  AND IMAGE.SYUPT_DAY = " & setParamEx(zasekiEnt.syuptDay, paramList))
        sqlString.AppendLine("  AND IMAGE.GOUSYA = " & setParamEx(zasekiEnt.gousya, paramList))

        Return sqlString.ToString
    End Function

    ''' <summary>
    ''' コース台帳(基本)取得
    ''' </summary>
    ''' <param name="paramList"></param>
    Public Function getCrsLegBasic(paramList As Hashtable) As String

        Dim resultDataTable As New DataTable
        Dim sqlString As New StringBuilder
        'コース台帳（基本）エンティティ
        Dim crsLedgerBasicEnt As New CrsLedgerBasicEntity()

        sqlString.AppendLine(" SELECT")
        sqlString.AppendLine("   BLOCK_KAKUHO_NUM ")      ' コース台帳（基本）.ブロック確保数
        sqlString.AppendLine(" , EI_BLOCK_REGULAR ")      ' コース台帳（基本）.営ブロック定
        sqlString.AppendLine(" , JYOSYA_CAPACITY ")       ' コース台帳（基本）.乗車定員
        sqlString.AppendLine(" , KUSEKI_KAKUHO_NUM ")     ' コース台帳（基本）.空席確保数
        sqlString.AppendLine(" , YOYAKU_NUM_TEISEKI ")    ' コース台帳（基本）.予約数定席
        sqlString.AppendLine(" , YOYAKU_NUM_SUB_SEAT")    ' コース台帳（基本）.予約数補助席

        'FROM句
        sqlString.AppendLine(" FROM ")
        sqlString.AppendLine("   T_CRS_LEDGER_BASIC ")    ' コース台帳（基本）

        With crsLedgerBasicEnt
            'WHERE句
            sqlString.AppendLine(" WHERE ")
            ' コース台帳（基本）.出発日 ＝ パラメータ.出発日
            sqlString.AppendLine(" SYUPT_DAY = " & setParam(.syuptDay.PhysicsName, paramList.Item(.syuptDay.PhysicsName), OracleDbType.Decimal, 8, 0))
            ' かつ コース台帳（基本）.コースコード ＝ パラメータ.コースコード
            sqlString.AppendLine(" And CRS_CD = " & setParam(.crsCd.PhysicsName, paramList.Item(.crsCd.PhysicsName), OracleDbType.Char))
            ' かつ コース台帳（基本）.号車 ＝ パラメータ.号車
            sqlString.AppendLine(" And GOUSYA = " & setParam(.gousya.PhysicsName, paramList.Item(.gousya.PhysicsName), OracleDbType.Decimal, 3, 0))
            sqlString.AppendLine(" And NVL(DELETE_DAY, 0) = 0 ")
        End With
        Return sqlString.ToString
    End Function

    ''' <summary>
    ''' コース台帳(基本)取得（乗せ合わせ）
    ''' </summary>
    ''' <param name="paramList"></param>
    Public Function getCrsLegReserveBasic(paramList As Hashtable) As String

        Dim resultDataTable As New DataTable
        Dim sqlString As New StringBuilder
        'コース台帳（基本）エンティティ
        Dim crsLedgerBasicEnt As New CrsLedgerBasicEntity()

        sqlString.AppendLine(" SELECT")
        sqlString.AppendLine("   BLOCK_KAKUHO_NUM ")      ' コース台帳（基本）.ブロック確保数
        sqlString.AppendLine(" , JYOSYA_CAPACITY ")       ' コース台帳（基本）.乗車定員
        sqlString.AppendLine(" , KUSEKI_KAKUHO_NUM ")     ' コース台帳（基本）.空席確保数
        sqlString.AppendLine(" , YOYAKU_NUM_TEISEKI ")    ' コース台帳（基本）.予約数定席
        sqlString.AppendLine(" , YOYAKU_NUM_SUB_SEAT")    ' コース台帳（基本）.予約数補助席
        sqlString.AppendLine(" , CRS_CD")                 ' コース台帳（基本）.コースコード

        'FROM句
        sqlString.AppendLine(" FROM ")
        sqlString.AppendLine("   T_CRS_LEDGER_BASIC ")    ' コース台帳（基本）

        With crsLedgerBasicEnt
            'WHERE句
            sqlString.AppendLine(" WHERE ")
            ' コース台帳（基本）.出発日 ＝ パラメータ.出発日
            sqlString.AppendLine(" SYUPT_DAY = " & setParam(.syuptDay.PhysicsName, paramList.Item(.syuptDay.PhysicsName), OracleDbType.Decimal, 8, 0))
            ' かつ コース台帳（基本）.バス指定コード ＝ パラメータ.バス指定コード
            sqlString.AppendLine(" And BUS_RESERVE_CD = " & setParam(.busReserveCd.PhysicsName, paramList.Item(.busReserveCd.PhysicsName), OracleDbType.Char))
            ' かつ コース台帳（基本）.号車 ＝ パラメータ.号車
            sqlString.AppendLine(" And GOUSYA = " & setParam(.gousya.PhysicsName, paramList.Item(.gousya.PhysicsName), OracleDbType.Decimal, 3, 0))
            ' かつ コース台帳（基本）.コースコード <> パラメータ.コースコード
            sqlString.AppendLine(" And CRS_CD <> " & setParam(.crsCd.PhysicsName, paramList.Item(.crsCd.PhysicsName), OracleDbType.Char))
            sqlString.AppendLine(" And NVL(DELETE_DAY, 0) = 0 ")
        End With
        Return sqlString.ToString
    End Function


    ''' <summary>
    ''' 座席イメージ(バス情報)取得
    ''' </summary>
    ''' <param name="paramList"></param>
    Public Function getZasekiImage(paramList As Hashtable) As String

        Dim resultDataTable As New DataTable
        Dim sqlString As New StringBuilder
        '座席イメージ（バス情報）エンティティ
        Dim zasekiImageEnt As New ZasekiImageEntity()

        sqlString.AppendLine(" SELECT")
        sqlString.AppendLine(" SUB_SEAT_OK_KBN ")      ' 座席イメージ（バス情報）.補助席可区分
        sqlString.AppendLine(" , TEIKI_KIKAKU_KBN ")      ' 座席イメージ（バス情報）.定期企画区分
        sqlString.AppendLine(" , BLOCK_KAKUHO_NUM ")      ' 座席イメージ（バス情報）.ブロック確保数
        sqlString.AppendLine(" , EI_BLOCK_REGULAR ")      ' 座席イメージ（バス情報）.営ブロック定
        sqlString.AppendLine(" , EI_BLOCK_HO ")           ' 座席イメージ（バス情報）.営ブロック補
        sqlString.AppendLine(" , KUSEKI_KAKUHO_NUM ")     ' 座席イメージ（バス情報）.空席確保数
        sqlString.AppendLine(" , CAPACITY_REGULAR ")      ' 座席イメージ（バス情報）.定員定
        sqlString.AppendLine(" , CAPACITY_HO_1KAI")       ' 座席イメージ（バス情報）.定員補1F

        'FROM句
        sqlString.AppendLine(" FROM ")
        sqlString.AppendLine("   T_ZASEKI_IMAGE ")    ' 座席イメージ（バス情報）

        With zasekiImageEnt
            'WHERE句
            sqlString.AppendLine(" WHERE ")
            ' 座席イメージ（バス情報）.出発日 ＝ パラメータ.出発日
            sqlString.AppendLine(" SYUPT_DAY = " & setParam(.syuptDay.PhysicsName, paramList.Item(.syuptDay.PhysicsName), OracleDbType.Decimal, 8, 0))
            ' かつ 座席イメージ（バス情報）.バス指定コード ＝ パラメータ.バス指定コード
            sqlString.AppendLine(" And BUS_RESERVE_CD = " & setParam(.busReserveCd.PhysicsName, paramList.Item(.busReserveCd.PhysicsName), OracleDbType.Char))
            ' かつ 座席イメージ（バス情報）.号車 ＝ パラメータ.号車
            sqlString.AppendLine(" And GOUSYA = " & setParam(.gousya.PhysicsName, paramList.Item(.gousya.PhysicsName), OracleDbType.Decimal, 3, 0))
            sqlString.AppendLine(" And NVL(DELETE_DAY, 0) = 0 ")
        End With
        Return sqlString.ToString
    End Function

#End Region

#Region " UPDATE処理 "
    ''' <summary>
    ''' UPDATE用DBアクセス
    ''' </summary>
    ''' <param name="selectType"></param>
    ''' <param name="paramEntityList"></param>
    ''' <returns></returns>
    Public Function accessZasekiBlockInfo(ByVal selectType As accessType, ByVal formId As String, ByVal paramEntityList As ArrayList, ByVal paramList As Hashtable, ByVal busReserveFlg As Boolean) As Integer

        Dim trns As OracleTransaction = Nothing
        Dim totalValue As Integer = 0
        Dim returnValue As Integer = 0
        Dim sqlString As String = String.Empty

        Try
            'トランザクション開始
            trns = callBeginTransaction()
            For Each ent As TZasekiImageInfoEntity In paramEntityList
                Select Case selectType
                    Case accessType.updateZasekiBlockInfo
                        sqlString = updZasekiBlockInfo(updateType.regist, formId, ent)
                        totalValue += execNonQuery(trns, sqlString)
                    Case accessType.updateZasekiBlockInfoRjc
                        sqlString = updZasekiBlockInfo(updateType.reject, formId, ent)
                        totalValue += execNonQuery(trns, sqlString)
                End Select
            Next

            ' コース台帳（基本）更新
            sqlString = updCrsLegBasic(formId, paramList)
            returnValue += execNonQuery(trns, sqlString)

            If busReserveFlg = True Then
                ' コース台帳（基本）更新
                sqlString = updCrsLegReserveBasic(formId, paramList)
                returnValue += execNonQuery(trns, sqlString)
            End If

            If returnValue <= 0 Then
                ' ロールバック
                Call callRollbackTransaction(trns)
            End If
            ' 座席イメージ(バス情報)更新
            sqlString = updZasekiImage(formId, paramList)
            returnValue = execNonQuery(trns, sqlString)

            If returnValue <= 0 Then
                ' ロールバック
                Call callRollbackTransaction(trns)
            End If

            Call callCommitTransaction(trns)
        Catch ex As Exception
            Call callRollbackTransaction(trns)
            Throw
        Finally
            Call trns.Dispose()
        End Try

        Return totalValue

    End Function

    ''' <summary>
    ''' 更新用SQL
    ''' </summary>
    ''' <param name="type">更新種別</param>
    ''' <param name="zasekiInfo">座席情報エンティティ</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Protected Overloads Function updZasekiBlockInfo(ByVal type As updateType, ByVal fromId As String, ByVal zasekiInfo As TZasekiImageInfoEntity) As String

        Dim sqlString As New StringBuilder
        paramClear()

        'UPDATE句
        sqlString.AppendLine("UPDATE T_ZASEKI_IMAGE_INFO ")
        sqlString.AppendLine("SET ")
        If type = updateType.regist Then
            sqlString.AppendLine("  EIGYOSYO_BLOCK_KBN = '1' ")
            sqlString.AppendLine("  , EIGYOSYO_CD = " & setParamEx(zasekiInfo.eigyosyoCd))
            sqlString.AppendLine("  , ZASEKI_STATE = '701' ")
        ElseIf type = updateType.reject Then
            sqlString.AppendLine("  EIGYOSYO_BLOCK_KBN = '0' ")
            sqlString.AppendLine("  , EIGYOSYO_CD = NULL")
            sqlString.AppendLine("  , ZASEKI_STATE = '0' ")
        End If
        sqlString.AppendLine("  ,SYSTEM_UPDATE_PGMID = '" & fromId & "' ")
        sqlString.AppendLine("  ,SYSTEM_UPDATE_PERSON_CD = '" & UserInfoManagement.userId & "'")
        sqlString.AppendLine("  ,SYSTEM_UPDATE_DAY = SYSDATE ")
        'WHERE句
        sqlString.AppendLine("WHERE ")
        sqlString.AppendLine("  SYUPT_DAY = " & setParamEx(zasekiInfo.syuptDay))
        sqlString.AppendLine("  AND BUS_RESERVE_CD = " & setParamEx(zasekiInfo.busReserveCd))
        sqlString.AppendLine("  AND GOUSYA = " & setParamEx(zasekiInfo.gousya))
        sqlString.AppendLine("  AND ZASEKI_KAI = " & setParamEx(zasekiInfo.zasekiKai))
        sqlString.AppendLine("  AND ZASEKI_LINE = " & setParamEx(zasekiInfo.zasekiLine))
        sqlString.AppendLine("  AND ZASEKI_COL = " & setParamEx(zasekiInfo.zasekiCol))

        Return sqlString.ToString
    End Function

    ''' <summary>
    ''' コース台帳(基本)更新用SQL
    ''' </summary>
    ''' <param name="fromId">更新元画面ID</param>
    ''' <param name="paramList">パラメータリスト</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function updCrsLegBasic(ByVal fromId As String, ByVal paramList As Hashtable) As String

        Dim sqlString As New StringBuilder
        paramClear()
        'コース台帳（基本）エンティティ
        Dim crsLedgerBasicEnt As New CrsLedgerBasicEntity()

        'UPDATE句
        sqlString.AppendLine("UPDATE T_CRS_LEDGER_BASIC ")
        sqlString.AppendLine("SET ")
        With crsLedgerBasicEnt
            sqlString.AppendLine("    EI_BLOCK_REGULAR = " & setParam(.eiBlockRegular.PhysicsName, paramList.Item(.eiBlockRegular.PhysicsName), OracleDbType.Decimal, 3, 0))
            sqlString.AppendLine("  , EI_BLOCK_HO = " & setParam(.eiBlockHo.PhysicsName, paramList.Item(.eiBlockHo.PhysicsName), OracleDbType.Decimal, 3, 0))
            sqlString.AppendLine("  , KUSEKI_NUM_TEISEKI = " & setParam(.kusekiNumTeiseki.PhysicsName, paramList.Item("kusekiNumTeiseki2"), OracleDbType.Decimal, 3, 0))
            sqlString.AppendLine("  , KUSEKI_NUM_SUB_SEAT = " & setParam(.kusekiNumSubSeat.PhysicsName, paramList.Item("kusekiNumHo2"), OracleDbType.Decimal, 3, 0))
            sqlString.AppendLine("  , USING_FLG = NULL ")
            sqlString.AppendLine("  , SYSTEM_UPDATE_PGMID = '" & fromId & "' ")
            sqlString.AppendLine("  , SYSTEM_UPDATE_PERSON_CD = '" & UserInfoManagement.userId & "'")
            sqlString.AppendLine("  , SYSTEM_UPDATE_DAY = SYSDATE ")
            'WHERE句
            sqlString.AppendLine("WHERE ")
            sqlString.AppendLine("  SYUPT_DAY = " & setParam(.syuptDay.PhysicsName, paramList.Item(.syuptDay.PhysicsName), OracleDbType.Decimal, 8, 0))
            sqlString.AppendLine("  AND BUS_RESERVE_CD = " & setParam(.busReserveCd.PhysicsName, paramList.Item(.busReserveCd.PhysicsName), OracleDbType.Char))
            sqlString.AppendLine("  AND GOUSYA = " & setParam(.gousya.PhysicsName, paramList.Item(.gousya.PhysicsName), OracleDbType.Decimal, 3, 0))
        End With
        Return sqlString.ToString
    End Function

    ''' <summary>
    ''' コース台帳(基本)更新用SQL（乗せ合わせ）
    ''' </summary>
    ''' <param name="fromId">更新元画面ID</param>
    ''' <param name="paramList">パラメータリスト</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function updCrsLegReserveBasic(ByVal fromId As String, ByVal paramList As Hashtable) As String

        Dim sqlString As New StringBuilder
        paramClear()
        'コース台帳（基本）エンティティ
        Dim crsLedgerBasicEnt As New CrsLedgerBasicEntity()

        'UPDATE句
        sqlString.AppendLine("UPDATE T_CRS_LEDGER_BASIC ")
        sqlString.AppendLine("SET ")
        With crsLedgerBasicEnt
            sqlString.AppendLine("  KUSEKI_NUM_TEISEKI = " & setParam(.kusekiNumTeiseki.PhysicsName, paramList.Item("kusekiTeisekiReserve"), OracleDbType.Decimal, 3, 0))
            sqlString.AppendLine("  , KUSEKI_NUM_SUB_SEAT = " & setParam(.kusekiNumSubSeat.PhysicsName, paramList.Item("kusekiHoReserve"), OracleDbType.Decimal, 3, 0))
            sqlString.AppendLine("  , USING_FLG = NULL ")
            sqlString.AppendLine("  , SYSTEM_UPDATE_PGMID = '" & fromId & "' ")
            sqlString.AppendLine("  , SYSTEM_UPDATE_PERSON_CD = '" & UserInfoManagement.userId & "'")
            sqlString.AppendLine("  , SYSTEM_UPDATE_DAY = SYSDATE ")
            'WHERE句
            sqlString.AppendLine("WHERE ")
            sqlString.AppendLine("  SYUPT_DAY = " & setParam(.syuptDay.PhysicsName, paramList.Item(.syuptDay.PhysicsName), OracleDbType.Decimal, 8, 0))
            sqlString.AppendLine("  AND CRS_CD = " & setParam(.crsCd.PhysicsName, paramList.Item("crsCd2"), OracleDbType.Char))
            sqlString.AppendLine("  AND GOUSYA = " & setParam(.gousya.PhysicsName, paramList.Item(.gousya.PhysicsName), OracleDbType.Decimal, 3, 0))
        End With
        Return sqlString.ToString
    End Function

    ''' <summary>
    ''' 座席イメージ(バス情報)更新用SQL
    ''' </summary>
    ''' <param name="fromId">更新元画面ID</param>
    ''' <param name="paramList">パラメータリスト</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function updZasekiImage(ByVal fromId As String, ByVal paramList As Hashtable) As String

        Dim sqlString As New StringBuilder
        paramClear()
        '座席イメージ（バス情報）エンティティ
        Dim zasekiImageEnt As New TZasekiImageEntity()

        'UPDATE句
        sqlString.AppendLine("UPDATE T_ZASEKI_IMAGE ")
        sqlString.AppendLine("SET ")
        With zasekiImageEnt
            sqlString.AppendLine("    EI_BLOCK_REGULAR = " & setParam(.eiBlockRegular.PhysicsName, paramList.Item(.eiBlockRegular.PhysicsName), OracleDbType.Decimal, 3, 0))
            sqlString.AppendLine("  , EI_BLOCK_HO = " & setParam(.eiBlockHo.PhysicsName, paramList.Item(.eiBlockHo.PhysicsName), OracleDbType.Decimal, 3, 0))
            sqlString.AppendLine("  , KUSEKI_NUM_TEISEKI = " & setParam(.kusekiNumTeiseki.PhysicsName, paramList.Item(.kusekiNumTeiseki.PhysicsName), OracleDbType.Decimal, 3, 0))
            sqlString.AppendLine("  , KUSEKI_NUM_SUB_SEAT = " & setParam(.kusekiNumSubSeat.PhysicsName, paramList.Item(.kusekiNumSubSeat.PhysicsName), OracleDbType.Decimal, 3, 0))
            sqlString.AppendLine("  , USING_FLG = NULL ")
            sqlString.AppendLine("  , SYSTEM_UPDATE_PGMID = '" & fromId & "' ")
            sqlString.AppendLine("  , SYSTEM_UPDATE_PERSON_CD = '" & UserInfoManagement.userId & "'")
            sqlString.AppendLine("  , SYSTEM_UPDATE_DAY = SYSDATE ")
            'WHERE句
            sqlString.AppendLine("WHERE ")
            sqlString.AppendLine("  SYUPT_DAY = " & setParam(.syuptDay.PhysicsName, paramList.Item(.syuptDay.PhysicsName), OracleDbType.Decimal, 8, 0))
            sqlString.AppendLine("  AND BUS_RESERVE_CD = " & setParam(.busReserveCd.PhysicsName, paramList.Item(.busReserveCd.PhysicsName), OracleDbType.Char))
            sqlString.AppendLine("  AND GOUSYA = " & setParam(.gousya.PhysicsName, paramList.Item(.gousya.PhysicsName), OracleDbType.Decimal, 3, 0))
        End With
        Return sqlString.ToString
    End Function
#End Region

#Region " Private関数 "
    ''' <summary>
    ''' パラメータ設定
    ''' </summary>
    ''' <param name="ent">エンティティ項目</param>
    ''' <param name="paramList">SQLパラメータHashTable</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Overloads Function setParamEx(ByVal ent As IEntityKoumokuType, ByVal paramList As Hashtable) As String
        Return MyBase.setParam(ent.PhysicsName, paramList(ent.PhysicsName), ent.DBType, ent.IntegerBu, ent.DecimalBu)
    End Function

    ''' <summary>
    ''' パラメータ設定
    ''' </summary>
    ''' <param name="ent">エンティティ項目</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Overloads Function setParamEx(ByVal ent As IEntityKoumokuType) As String
        If TypeOf (ent) Is EntityKoumoku_MojiType Then
            Return MyBase.setParam(ent.PhysicsName, CType(ent, EntityKoumoku_MojiType).Value, ent.DBType, ent.IntegerBu, ent.DecimalBu)
        ElseIf TypeOf (ent) Is EntityKoumoku_NumberType Then
            Return MyBase.setParam(ent.PhysicsName, CType(ent, EntityKoumoku_NumberType).Value, ent.DBType, ent.IntegerBu, ent.DecimalBu)
        ElseIf TypeOf (ent) Is EntityKoumoku_Number_DecimalType Then
            Return MyBase.setParam(ent.PhysicsName, CType(ent, EntityKoumoku_Number_DecimalType).Value, ent.DBType, ent.IntegerBu, ent.DecimalBu)
        ElseIf TypeOf (ent) Is EntityKoumoku_YmdType Then
            Return MyBase.setParam(ent.PhysicsName, CType(ent, EntityKoumoku_YmdType).Value, ent.DBType, ent.IntegerBu, ent.DecimalBu)
        End If
    End Function
#End Region

End Class
