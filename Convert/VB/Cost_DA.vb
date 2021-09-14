Imports System.Text

Public Class Cost_DA
    Inherits DataAccessorBase

#Region " 定数／変数 "
    Public Enum accessType As Integer
        getSearchData           '検索結果取得
        updateData              '登録処理
    End Enum

    ''' <summary>
    ''' パラメータキー
    ''' </summary>
    ''' <remarks></remarks>
    Public NotInheritable Class ParamHashKeys
        ''' <summary>
        ''' 企画用(KEY)
        ''' </summary>
        Public Const BASIC_KEYS As String = "BASIC_KEYS"
    End Class
#End Region

#Region " SELECT処理 "

    ''' <summary>
    ''' SELECT用DBアクセス
    ''' </summary>
    ''' <param name="type"></param>
    ''' <param name="paramInfoList"></param>
    ''' <returns></returns>
    Public Function getCostTable(ByVal type As accessType, Optional ByVal paramInfoList As Hashtable = Nothing) As DataTable
        'SQL文字列
        Dim sqlString As String = String.Empty
        '戻り値
        Dim returnValue As DataTable = Nothing

        MyBase.paramClear()

        Select Case type
            Case accessType.getSearchData
                '一覧結果取得検索
                sqlString = getChargeInfoHead(paramInfoList)
            Case Else
                '該当処理なし
                Return returnValue
        End Select

        Try
            returnValue = getDataTable(sqlString)
            For Each col As DataColumn In returnValue.Columns
                col.AllowDBNull = True
            Next
        Catch ex As Exception
            Throw
        End Try

        Return returnValue

    End Function

    ''' <summary>
    ''' ヘッダ情報取得用SELECT
    ''' </summary>
    ''' <param name="paramList"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Protected Overloads Function getChargeInfoHead(ByVal paramList As Hashtable) As String

        Dim sqlString As New StringBuilder

        sqlString.AppendLine("SELECT ")
        sqlString.AppendLine("  DISP_SYUPT_DAY ")
        sqlString.AppendLine("  , YOBI_NAME ")
        sqlString.AppendLine("  , PLACE_NAME ")
        sqlString.AppendLine("  , SYUPT_TIME ")
        sqlString.AppendLine("  , DISP_GOUSYA ")
        sqlString.AppendLine("  , UNKYU_NAME ")
        sqlString.AppendLine("  , UNKYU_KBN ")
        sqlString.AppendLine("  , CASE ")
        sqlString.AppendLine("    WHEN EDABAN = 1 ")
        sqlString.AppendLine("      THEN '変更' ")
        sqlString.AppendLine("    ELSE NULL ")
        sqlString.AppendLine("    END K_BUTTON                                  --降車ヶ所(ボタン) ")
        sqlString.AppendLine("  , DAILY ")
        sqlString.AppendLine("  , SIIRE_SAKI_KIND_CD ")
        sqlString.AppendLine("  , SIIRE_SAKI_KIND_NAME ")
        sqlString.AppendLine("  , CODE ")
        sqlString.AppendLine("  , SIIRE_SAKI_NAME ")
        sqlString.AppendLine("  , MOKUTEKI ")
        sqlString.AppendLine("  , '変更' AS G_BUTTON                              --原価(ボタン) ")
        sqlString.AppendLine("  , G_FLG ")
        sqlString.AppendLine("  , CASE ")
        sqlString.AppendLine("    WHEN EDABAN = 1 ")
        sqlString.AppendLine("      THEN '変更' ")
        sqlString.AppendLine("    ELSE NULL ")
        sqlString.AppendLine("    END S_BUTTON                                  --その他原価(ボタン) ")
        sqlString.AppendLine("  , LINE_NO ")
        sqlString.AppendLine("  , CRS_CD ")
        sqlString.AppendLine("  , GOUSYA ")
        sqlString.AppendLine("  , SYUPT_DAY ")
        sqlString.AppendLine("  , RNK ")
        sqlString.AppendLine("  , EDABAN ")
        sqlString.AppendLine("FROM ")
        sqlString.AppendLine("  ( ")
        sqlString.AppendLine("    SELECT ")
        sqlString.AppendLine("      TO_CHAR(TO_DATE(BASIC.SYUPT_DAY), 'yyyy/MM/dd') AS DISP_SYUPT_DAY --日付 ")
        sqlString.AppendLine("      , CODE_Y.CODE_NAME AS YOBI_NAME             --曜日 ")
        sqlString.AppendLine("      , PLACE.PLACE_NAME_1 AS PLACE_NAME          --乗車地 ")
        sqlString.AppendLine("      ," & CommonKyushuUtil.setSQLTime24Format("BASIC.SYUPT_TIME_1") & "AS SYUPT_TIME --出発時間 ")
        sqlString.AppendLine("      , BASIC.GOUSYA AS DISP_GOUSYA               --号車 ")
        sqlString.AppendLine("      , CASE BASIC.TEIKI_KIKAKU_KBN ")
        sqlString.AppendLine("        WHEN '1' THEN CODE_U_T.CODE_NAME          --定期の場合 ")
        sqlString.AppendLine("        ELSE CODE_U_K.CODE_NAME                   --企画の場合 ")
        sqlString.AppendLine("        END AS UNKYU_NAME                         --運休 ")
        sqlString.AppendLine("      , BASIC.UNKYU_KBN AS UNKYU_KBN              --運休区分 ")
        sqlString.AppendLine("      , KOUSHA.DAILY                              --日目 ")
        sqlString.AppendLine("      , SIIRE.SIIRE_SAKI_KIND_CD                  --降車ヶ所種別 ")
        sqlString.AppendLine("      , CODE_SK.CODE_NAME AS SIIRE_SAKI_KIND_NAME --降車ヶ所種別名 ")
        sqlString.AppendLine("      , KOUSHA.KOSHAKASHO_CD || KOUSHA.KOSHAKASHO_EDABAN AS CODE --コード ")
        sqlString.AppendLine("      , SIIRE.SIIRE_SAKI_NAME                     --降車ヶ所名 ")
        sqlString.AppendLine("      , CODE_S.CODE_NAME AS MOKUTEKI              --精算目的 ")
        sqlString.AppendLine("      , CASE ")
        sqlString.AppendLine("        WHEN GENKA.CNT > 0 ")
        sqlString.AppendLine("        OR GENKA_CARRIER.CNT > 0 ")
        sqlString.AppendLine("          THEN '済' ")
        sqlString.AppendLine("        ELSE '未' ")
        sqlString.AppendLine("        END AS G_FLG                              --原価設定 ")
        sqlString.AppendLine("      , KOUSHA.LINE_NO                            --LINE_NO ")
        sqlString.AppendLine("      , BASIC.CRS_CD                              --コースコード(PK) ")
        sqlString.AppendLine("      , BASIC.GOUSYA                              --号車 ")
        sqlString.AppendLine("      , BASIC.SYUPT_DAY                           --出発日(PK) ")
        sqlString.AppendLine("      , RANK() OVER ( ")
        sqlString.AppendLine("        PARTITION BY ")
        sqlString.AppendLine("          BASIC.CRS_CD ")
        sqlString.AppendLine("        ORDER BY ")
        sqlString.AppendLine("            BASIC.SYUPT_DAY ")
        sqlString.AppendLine("          , BASIC.SYUPT_TIME_1 ")
        sqlString.AppendLine("          , BASIC.GOUSYA ")
        sqlString.AppendLine("      ) RNK                                       --RANK ")
        sqlString.AppendLine("      , ROW_NUMBER() OVER ( ")
        sqlString.AppendLine("        PARTITION BY ")
        sqlString.AppendLine("          BASIC.CRS_CD ")
        sqlString.AppendLine("          , BASIC.SYUPT_DAY ")
        sqlString.AppendLine("          , BASIC.SYUPT_TIME_1 ")
        sqlString.AppendLine("          , BASIC.GOUSYA ")
        sqlString.AppendLine("        ORDER BY ")
        sqlString.AppendLine("          KOUSHA.DAILY ")
        sqlString.AppendLine("          ,KOUSHA.KBN ")
        sqlString.AppendLine("          ,KOUSHA.LINE_NO ")
        sqlString.AppendLine("      ) EDABAN                                    --RANK内順番 ")
        sqlString.AppendLine("    FROM ")
        sqlString.AppendLine("      T_CRS_LEDGER_BASIC BASIC ")
        sqlString.AppendLine("      LEFT JOIN ( ")
        sqlString.AppendLine("        SELECT ")
        sqlString.AppendLine("          KOUSHA.BIN_NAME ")
        sqlString.AppendLine("          , KOUSHA.CRS_CD ")
        sqlString.AppendLine("          , KOUSHA.DAILY ")
        sqlString.AppendLine("          , KOUSHA.DELETE_DAY ")
        sqlString.AppendLine("          , KOUSHA.GOUSYA ")
        sqlString.AppendLine("          , KOUSHA.KOSHAKASHO_CD ")
        sqlString.AppendLine("          , KOUSHA.KOSHAKASHO_EDABAN ")
        sqlString.AppendLine("          , KOUSHA.SEISAN_TGT_CD ")
        sqlString.AppendLine("          , KOUSHA.LINE_NO ")
        sqlString.AppendLine("          , KOUSHA.RIYOU_DAY ")
        sqlString.AppendLine("          , KOUSHA.SYUPT_DAY ")
        sqlString.AppendLine("          , KOUSHA.SYUPT_PLACE_CARRIER ")
        sqlString.AppendLine("          , KOUSHA.SYUPT_PLACE_CD_CARRIER ")
        sqlString.AppendLine("          , KOUSHA.TTYAK_PLACE_CARRIER ")
        sqlString.AppendLine("          , KOUSHA.TTYAK_PLACE_CD_CARRIER ")
        sqlString.AppendLine("          , KOUSHA.SYSTEM_ENTRY_PGMID ")
        sqlString.AppendLine("          , KOUSHA.SYSTEM_ENTRY_PERSON_CD ")
        sqlString.AppendLine("          , KOUSHA.SYSTEM_ENTRY_DAY ")
        sqlString.AppendLine("          , KOUSHA.SYSTEM_UPDATE_PGMID ")
        sqlString.AppendLine("          , KOUSHA.SYSTEM_UPDATE_PERSON_CD ")
        sqlString.AppendLine("          , KOUSHA.SYSTEM_UPDATE_DAY ")
        sqlString.AppendLine("          , 1 AS KBN ")
        sqlString.AppendLine("        FROM ")
        sqlString.AppendLine("          T_CRS_LEDGER_KOSHAKASHO KOUSHA ")
        sqlString.AppendLine("        WHERE ")
        sqlString.AppendLine("          (KOUSHA.CRS_CD, KOUSHA.SYUPT_DAY, KOUSHA.GOUSYA) IN (")
        sqlString.AppendLine("            ").Append(paramList(Me.ParamHashKeys.BASIC_KEYS))
        sqlString.AppendLine("          ) ")
        sqlString.AppendLine("      AND KOUSHA.DELETE_DAY = 0 ")
        sqlString.AppendLine("        UNION ALL ")
        sqlString.AppendLine("        SELECT ")
        sqlString.AppendLine("          NULL ")
        sqlString.AppendLine("          , HOTEL.CRS_CD ")
        sqlString.AppendLine("          , HOTEL.DAILY ")
        sqlString.AppendLine("          , HOTEL.DELETE_DAY ")
        sqlString.AppendLine("          , HOTEL.GOUSYA ")
        sqlString.AppendLine("          , HOTEL.SIIRE_SAKI_CD ")
        sqlString.AppendLine("          , HOTEL.SIIRE_SAKI_EDABAN ")
        sqlString.AppendLine("          , SIIRE.SEISAN_TGT_CD ")
        sqlString.AppendLine("          , HOTEL.LINE_NO ")
        sqlString.AppendLine("          , HOTEL.RIYOU_DAY ")
        sqlString.AppendLine("          , HOTEL.SYUPT_DAY ")
        sqlString.AppendLine("          , NULL ")
        sqlString.AppendLine("          , NULL ")
        sqlString.AppendLine("          , NULL ")
        sqlString.AppendLine("          , NULL ")
        sqlString.AppendLine("          , HOTEL.SYSTEM_ENTRY_PGMID ")
        sqlString.AppendLine("          , HOTEL.SYSTEM_ENTRY_PERSON_CD ")
        sqlString.AppendLine("          , HOTEL.SYSTEM_ENTRY_DAY ")
        sqlString.AppendLine("          , HOTEL.SYSTEM_UPDATE_PGMID ")
        sqlString.AppendLine("          , HOTEL.SYSTEM_UPDATE_PERSON_CD ")
        sqlString.AppendLine("          , HOTEL.SYSTEM_UPDATE_DAY ")
        sqlString.AppendLine("          , 2 AS KBN ")
        sqlString.AppendLine("        FROM ")
        sqlString.AppendLine("          T_CRS_LEDGER_HOTEL HOTEL ")
        sqlString.AppendLine("          LEFT JOIN M_SIIRE_SAKI SIIRE ")
        sqlString.AppendLine("            ON HOTEL.SIIRE_SAKI_CD = SIIRE.SIIRE_SAKI_CD ")
        sqlString.AppendLine("            AND HOTEL.SIIRE_SAKI_EDABAN = SIIRE.SIIRE_SAKI_NO ")
        sqlString.AppendLine("        WHERE ")
        sqlString.AppendLine("          (HOTEL.CRS_CD, HOTEL.SYUPT_DAY, HOTEL.GOUSYA) IN (")
        sqlString.AppendLine("            ").Append(paramList(Me.ParamHashKeys.BASIC_KEYS))
        sqlString.AppendLine("          ) ")
        sqlString.AppendLine("      AND HOTEL.DELETE_DAY = 0 ")
        sqlString.AppendLine("      ) KOUSHA ")
        sqlString.AppendLine("        ON BASIC.CRS_CD = KOUSHA.CRS_CD ")
        sqlString.AppendLine("        AND BASIC.SYUPT_DAY = KOUSHA.SYUPT_DAY ")
        sqlString.AppendLine("        AND BASIC.GOUSYA = KOUSHA.GOUSYA ")
        sqlString.AppendLine("      LEFT JOIN M_SIIRE_SAKI SIIRE ")
        sqlString.AppendLine("        ON KOUSHA.KOSHAKASHO_CD = SIIRE.SIIRE_SAKI_CD ")
        sqlString.AppendLine("        AND KOUSHA.KOSHAKASHO_EDABAN = SIIRE.SIIRE_SAKI_NO ")
        sqlString.AppendLine("        AND SIIRE.DELETE_DATE IS NULL ")
        sqlString.AppendLine("      LEFT JOIN M_PLACE PLACE ")
        sqlString.AppendLine("        ON BASIC.HAISYA_KEIYU_CD_1 = PLACE.PLACE_CD ")
        sqlString.AppendLine("        AND PLACE.DELETE_DATE IS NULL ")
        sqlString.AppendLine("      LEFT JOIN M_CODE CODE_Y ")
        sqlString.AppendLine("        ON BASIC.YOBI_CD = CODE_Y.CODE_VALUE ")
        sqlString.AppendLine("        AND CODE_Y.CODE_BUNRUI = '" & CodeBunrui.yobi & "' ")
        sqlString.AppendLine("        AND CODE_Y.DELETE_DATE IS NULL ")
        sqlString.AppendLine("      LEFT JOIN M_CODE CODE_U_T ")
        sqlString.AppendLine("        ON BASIC.UNKYU_KBN = CODE_U_T.CODE_VALUE ")
        sqlString.AppendLine("        AND CODE_U_T.CODE_BUNRUI = '" & CodeBunrui.unkyu & "' ")
        sqlString.AppendLine("        AND CODE_U_T.DELETE_DATE IS NULL ")
        sqlString.AppendLine("      LEFT JOIN M_CODE CODE_U_K ")
        sqlString.AppendLine("        ON BASIC.SAIKOU_KAKUTEI_KBN = CODE_U_K.CODE_VALUE ")
        sqlString.AppendLine("        AND CODE_U_K.CODE_BUNRUI = '" & CodeBunrui.saikou & "' ")
        sqlString.AppendLine("        AND CODE_U_K.DELETE_DATE IS NULL ")
        sqlString.AppendLine("      LEFT JOIN M_CODE CODE_S ")
        sqlString.AppendLine("        ON KOUSHA.SEISAN_TGT_CD = CODE_S.CODE_VALUE ")
        sqlString.AppendLine("        AND CODE_S.CODE_BUNRUI = '" & CodeBunrui.seisanMokuteki & "' ")
        sqlString.AppendLine("        AND CODE_S.DELETE_DATE IS NULL ")
        sqlString.AppendLine("      LEFT JOIN M_CODE CODE_SK ")
        sqlString.AppendLine("        ON SIIRE.SIIRE_SAKI_KIND_CD = CODE_SK.CODE_VALUE ")
        sqlString.AppendLine("        AND CODE_SK.CODE_BUNRUI = '" & CodeBunrui.siireKind & "' ")
        sqlString.AppendLine("        AND CODE_SK.DELETE_DATE IS NULL ")
        sqlString.AppendLine("      LEFT JOIN ( ")
        sqlString.AppendLine("        SELECT ")
        sqlString.AppendLine("          CRS_CD ")
        sqlString.AppendLine("          , SYUPT_DAY ")
        sqlString.AppendLine("          , GOUSYA ")
        sqlString.AppendLine("          , CRS_ITINERARY_LINE_NO ")
        sqlString.AppendLine("          , SIIRE_SAKI_CD ")
        sqlString.AppendLine("          , SIIRE_SAKI_EDABAN ")
        sqlString.AppendLine("          , COUNT(CRS_CD) CNT ")
        sqlString.AppendLine("        FROM ")
        sqlString.AppendLine("          T_CRS_LEDGER_COST_KOSHAKASHO ")
        sqlString.AppendLine("        WHERE ")
        sqlString.AppendLine("          (CRS_CD, SYUPT_DAY, GOUSYA) IN (")
        sqlString.AppendLine("            ").Append(paramList(Me.ParamHashKeys.BASIC_KEYS))
        sqlString.AppendLine("          ) ")
        sqlString.AppendLine("        GROUP BY ")
        sqlString.AppendLine("          CRS_CD ")
        sqlString.AppendLine("          , SYUPT_DAY ")
        sqlString.AppendLine("          , GOUSYA ")
        sqlString.AppendLine("          , CRS_ITINERARY_LINE_NO ")
        sqlString.AppendLine("          , SIIRE_SAKI_CD ")
        sqlString.AppendLine("          , SIIRE_SAKI_EDABAN ")
        sqlString.AppendLine("      ) GENKA ")
        sqlString.AppendLine("        ON KOUSHA.CRS_CD = GENKA.CRS_CD ")
        sqlString.AppendLine("        AND KOUSHA.SYUPT_DAY = GENKA.SYUPT_DAY ")
        sqlString.AppendLine("        AND KOUSHA.GOUSYA = GENKA.GOUSYA ")
        sqlString.AppendLine("        AND KOUSHA.LINE_NO = GENKA.CRS_ITINERARY_LINE_NO ")
        sqlString.AppendLine("        AND KOUSHA.KOSHAKASHO_CD = GENKA.SIIRE_SAKI_CD ")
        sqlString.AppendLine("        AND KOUSHA.KOSHAKASHO_EDABAN = GENKA.SIIRE_SAKI_EDABAN ")
        sqlString.AppendLine("      LEFT JOIN ( ")
        sqlString.AppendLine("        SELECT ")
        sqlString.AppendLine("          CRS_CD ")
        sqlString.AppendLine("          , SYUPT_DAY ")
        sqlString.AppendLine("          , GOUSYA ")
        sqlString.AppendLine("          , CRS_ITINERARY_LINE_NO ")
        sqlString.AppendLine("          , CARRIER_CD ")
        sqlString.AppendLine("          , CARRIER_EDABAN ")
        sqlString.AppendLine("          , COUNT(CRS_CD) CNT ")
        sqlString.AppendLine("        FROM ")
        sqlString.AppendLine("          T_CRS_LEDGER_COST_CARRIER ")
        sqlString.AppendLine("        WHERE ")
        sqlString.AppendLine("          (CRS_CD, SYUPT_DAY, GOUSYA) IN (")
        sqlString.AppendLine("            ").Append(paramList(Me.ParamHashKeys.BASIC_KEYS))
        sqlString.AppendLine("          ) ")
        sqlString.AppendLine("          AND CRS_ITINERARY_LINE_NO > 0 ")
        sqlString.AppendLine("        GROUP BY ")
        sqlString.AppendLine("          CRS_CD ")
        sqlString.AppendLine("          , SYUPT_DAY ")
        sqlString.AppendLine("          , GOUSYA ")
        sqlString.AppendLine("          , CRS_ITINERARY_LINE_NO ")
        sqlString.AppendLine("          , CARRIER_CD ")
        sqlString.AppendLine("          , CARRIER_EDABAN ")
        sqlString.AppendLine("      ) GENKA_CARRIER ")
        sqlString.AppendLine("        ON KOUSHA.CRS_CD = GENKA_CARRIER.CRS_CD ")
        sqlString.AppendLine("        AND KOUSHA.SYUPT_DAY = GENKA_CARRIER.SYUPT_DAY ")
        sqlString.AppendLine("        AND KOUSHA.GOUSYA = GENKA_CARRIER.GOUSYA ")
        sqlString.AppendLine("        AND KOUSHA.LINE_NO = GENKA_CARRIER.CRS_ITINERARY_LINE_NO ")
        sqlString.AppendLine("        AND KOUSHA.KOSHAKASHO_CD = GENKA_CARRIER.CARRIER_CD ")
        sqlString.AppendLine("        AND KOUSHA.KOSHAKASHO_EDABAN = GENKA_CARRIER.CARRIER_EDABAN ")
        sqlString.AppendLine("    WHERE ")
        sqlString.AppendLine("      BASIC.DELETE_DAY = 0 ")
        sqlString.AppendLine("      AND (BASIC.CRS_CD, BASIC.SYUPT_DAY, BASIC.GOUSYA) IN (")
        sqlString.AppendLine("        ").Append(paramList(Me.ParamHashKeys.BASIC_KEYS))
        sqlString.AppendLine("      ) ")
        sqlString.AppendLine("    ORDER BY ")
        sqlString.AppendLine("      CRS_CD ")
        sqlString.AppendLine("      , SYUPT_DAY ")
        sqlString.AppendLine("      , GOUSYA ")
        sqlString.AppendLine("      , DAILY ")
        sqlString.AppendLine("      , KBN ")
        sqlString.AppendLine("      , LINE_NO ")
        sqlString.AppendLine("  ) ")
        sqlString.AppendLine("ORDER BY ")
        sqlString.AppendLine("  RNK ")
        sqlString.AppendLine("  , EDABAN ")

        'sqlString.AppendLine("SELECT ")
        'sqlString.AppendLine("  TO_CHAR(TO_DATE(BASIC.SYUPT_DAY), 'yyyy/MM/dd') AS DISP_SYUPT_DAY --日付 ")
        'sqlString.AppendLine("  , CODE_Y.CODE_NAME AS YOBI_NAME                 --曜日 ")
        'sqlString.AppendLine("  , PLACE.PLACE_NAME_1 AS PLACE_NAME              --乗車地 ")
        'sqlString.AppendLine("  , SUBSTR(TO_CHAR(LPAD(BASIC.SYUPT_TIME_1, 4, '0')), 0, 2) || ':' || SUBSTR(TO_CHAR(LPAD(BASIC.SYUPT_TIME_1, 4, '0')), 3, 2) ")
        'sqlString.AppendLine("   AS SYUPT_TIME                                  --出発時間 ")
        'sqlString.AppendLine("  , BASIC.GOUSYA AS DISP_GOUSYA                   --号車 ")
        'sqlString.AppendLine("  , CASE BASIC.TEIKI_KIKAKU_KBN ")
        'sqlString.AppendLine("    WHEN '1' THEN CODE_U_T.CODE_NAME              --定期の場合 ")
        'sqlString.AppendLine("    ELSE CODE_U_K.CODE_NAME                       --企画の場合 ")
        'sqlString.AppendLine("    END AS UNKYU_NAME                             --運休 ")
        'sqlString.AppendLine("  , BASIC.UNKYU_KBN AS UNKYU_KBN                  --運休区分 ")
        'sqlString.AppendLine("  , '変更' AS K_BUTTON                            --降車ヶ所(ボタン) ")
        'sqlString.AppendLine("  , KOUSHA.DAILY                                  --日目 ")
        'sqlString.AppendLine("  , SIIRE.SIIRE_SAKI_KIND_CD                      --降車ヶ所種別 ")
        'sqlString.AppendLine("  , CODE_SK.CODE_NAME AS SIIRE_SAKI_KIND_NAME     --降車ヶ所種別名 ")
        'sqlString.AppendLine("  , KOUSHA.KOSHAKASHO_CD || KOUSHA.KOSHAKASHO_EDABAN AS CODE --コード ")
        'sqlString.AppendLine("  , SIIRE.SIIRE_SAKI_NAME                         --降車ヶ所名 ")
        'sqlString.AppendLine("  , CODE_S.CODE_NAME AS MOKUTEKI                  --精算目的 ")
        'sqlString.AppendLine("  , '変更' AS G_BUTTON                              --原価(ボタン) ")
        'sqlString.AppendLine("  , CASE ")
        'sqlString.AppendLine("    WHEN GENKA.CNT > 0 ")
        'sqlString.AppendLine("    OR GENKA_CARRIER.CNT > 0 ")
        'sqlString.AppendLine("      THEN '済' ")
        'sqlString.AppendLine("    ELSE '未' ")
        'sqlString.AppendLine("    END AS G_FLG                                  --原価設定 ")
        'sqlString.AppendLine("  , '変更' AS S_BUTTON                              --その他原価(ボタン) ")
        'sqlString.AppendLine("  , KOUSHA.LINE_NO                                --LINE_NO ")
        'sqlString.AppendLine("  , BASIC.CRS_CD                                  --コースコード(PK) ")
        'sqlString.AppendLine("  , BASIC.GOUSYA                                  --号車 ")
        'sqlString.AppendLine("  , BASIC.SYUPT_DAY                               --出発日(PK) ")
        'sqlString.AppendLine("  , RANK() OVER ( ")
        'sqlString.AppendLine("    PARTITION BY ")
        'sqlString.AppendLine("      BASIC.CRS_CD ")
        'sqlString.AppendLine("    ORDER BY ")
        'sqlString.AppendLine("      BASIC.SYUPT_DAY ")
        'sqlString.AppendLine("      , BASIC.GOUSYA ")
        'sqlString.AppendLine("  ) RNK                                           --RANK ")
        'sqlString.AppendLine("  , ROW_NUMBER() OVER ( ")
        'sqlString.AppendLine("    PARTITION BY ")
        'sqlString.AppendLine("      BASIC.CRS_CD ")
        'sqlString.AppendLine("      , BASIC.SYUPT_DAY ")
        'sqlString.AppendLine("      , BASIC.GOUSYA ")
        'sqlString.AppendLine("    ORDER BY ")
        'sqlString.AppendLine("      BASIC.CRS_CD ")
        'sqlString.AppendLine("  ) EDABAN                                        --RANK内順番 ")
        'sqlString.AppendLine("FROM ")
        'sqlString.AppendLine("  T_CRS_LEDGER_BASIC BASIC ")
        'sqlString.AppendLine("  LEFT JOIN ( ")
        'sqlString.AppendLine("    SELECT ")
        'sqlString.AppendLine("      KOUSHA.BIN_NAME ")
        'sqlString.AppendLine("      , KOUSHA.CRS_CD ")
        'sqlString.AppendLine("      , KOUSHA.DAILY ")
        'sqlString.AppendLine("      , KOUSHA.DELETE_DAY ")
        'sqlString.AppendLine("      , KOUSHA.GOUSYA ")
        'sqlString.AppendLine("      , KOUSHA.KOSHAKASHO_CD ")
        'sqlString.AppendLine("      , KOUSHA.KOSHAKASHO_EDABAN ")
        'sqlString.AppendLine("      , KOUSHA.SEISAN_TGT_CD ")
        'sqlString.AppendLine("      , KOUSHA.LINE_NO ")
        'sqlString.AppendLine("      , KOUSHA.RIYOU_DAY ")
        'sqlString.AppendLine("      , KOUSHA.SYUPT_DAY ")
        'sqlString.AppendLine("      , KOUSHA.SYUPT_PLACE_CARRIER ")
        'sqlString.AppendLine("      , KOUSHA.SYUPT_PLACE_CD_CARRIER ")
        'sqlString.AppendLine("      , KOUSHA.TTYAK_PLACE_CARRIER ")
        'sqlString.AppendLine("      , KOUSHA.TTYAK_PLACE_CD_CARRIER ")
        'sqlString.AppendLine("      , KOUSHA.SYSTEM_ENTRY_PGMID ")
        'sqlString.AppendLine("      , KOUSHA.SYSTEM_ENTRY_PERSON_CD ")
        'sqlString.AppendLine("      , KOUSHA.SYSTEM_ENTRY_DAY ")
        'sqlString.AppendLine("      , KOUSHA.SYSTEM_UPDATE_PGMID ")
        'sqlString.AppendLine("      , KOUSHA.SYSTEM_UPDATE_PERSON_CD ")
        'sqlString.AppendLine("      , KOUSHA.SYSTEM_UPDATE_DAY ")
        'sqlString.AppendLine("      , 1 AS KBN ")
        'sqlString.AppendLine("    FROM ")
        'sqlString.AppendLine("      T_CRS_LEDGER_KOSHAKASHO KOUSHA ")
        'sqlString.AppendLine("    WHERE ")
        'sqlString.AppendLine("      (KOUSHA.CRS_CD, KOUSHA.SYUPT_DAY, KOUSHA.GOUSYA) IN (")
        'sqlString.AppendLine("        ").Append(paramList(Me.ParamHashKeys.BASIC_KEYS))
        'sqlString.AppendLine("      ) ")
        'sqlString.AppendLine("      AND KOUSHA.DELETE_DAY = 0 ")
        'sqlString.AppendLine("    UNION ALL ")
        'sqlString.AppendLine("    SELECT ")
        'sqlString.AppendLine("      NULL ")
        'sqlString.AppendLine("      , HOTEL.CRS_CD ")
        'sqlString.AppendLine("      , HOTEL.DAILY ")
        'sqlString.AppendLine("      , HOTEL.DELETE_DAY ")
        'sqlString.AppendLine("      , HOTEL.GOUSYA ")
        'sqlString.AppendLine("      , HOTEL.SIIRE_SAKI_CD ")
        'sqlString.AppendLine("      , HOTEL.SIIRE_SAKI_EDABAN ")
        'sqlString.AppendLine("      , NULL ")
        'sqlString.AppendLine("      , HOTEL.LINE_NO ")
        'sqlString.AppendLine("      , HOTEL.RIYOU_DAY ")
        'sqlString.AppendLine("      , HOTEL.SYUPT_DAY ")
        'sqlString.AppendLine("      , NULL ")
        'sqlString.AppendLine("      , NULL ")
        'sqlString.AppendLine("      , NULL ")
        'sqlString.AppendLine("      , NULL ")
        'sqlString.AppendLine("      , HOTEL.SYSTEM_ENTRY_PGMID ")
        'sqlString.AppendLine("      , HOTEL.SYSTEM_ENTRY_PERSON_CD ")
        'sqlString.AppendLine("      , HOTEL.SYSTEM_ENTRY_DAY ")
        'sqlString.AppendLine("      , HOTEL.SYSTEM_UPDATE_PGMID ")
        'sqlString.AppendLine("      , HOTEL.SYSTEM_UPDATE_PERSON_CD ")
        'sqlString.AppendLine("      , HOTEL.SYSTEM_UPDATE_DAY ")
        'sqlString.AppendLine("      , 2 AS KBN ")
        'sqlString.AppendLine("    FROM ")
        'sqlString.AppendLine("      T_CRS_LEDGER_HOTEL HOTEL ")
        'sqlString.AppendLine("    WHERE ")
        'sqlString.AppendLine("      (HOTEL.CRS_CD, HOTEL.SYUPT_DAY, HOTEL.GOUSYA) IN (")
        'sqlString.AppendLine("        ").Append(paramList(Me.ParamHashKeys.BASIC_KEYS))
        'sqlString.AppendLine("      ) ")
        'sqlString.AppendLine("      AND HOTEL.DELETE_DAY = 0 ")
        'sqlString.AppendLine("  ) KOUSHA ")
        'sqlString.AppendLine("    ON BASIC.CRS_CD = KOUSHA.CRS_CD ")
        'sqlString.AppendLine("    AND BASIC.SYUPT_DAY = KOUSHA.SYUPT_DAY ")
        'sqlString.AppendLine("    AND BASIC.GOUSYA = KOUSHA.GOUSYA ")
        'sqlString.AppendLine("  LEFT JOIN M_SIIRE_SAKI SIIRE ")
        'sqlString.AppendLine("    ON KOUSHA.KOSHAKASHO_CD = SIIRE.SIIRE_SAKI_CD ")
        'sqlString.AppendLine("    AND KOUSHA.KOSHAKASHO_EDABAN = SIIRE.SIIRE_SAKI_NO ")
        'sqlString.AppendLine("    AND SIIRE.DELETE_DATE IS NULL ")
        'sqlString.AppendLine("  LEFT JOIN M_PLACE PLACE ")
        'sqlString.AppendLine("    ON BASIC.HAISYA_KEIYU_CD_1 = PLACE.PLACE_CD ")
        'sqlString.AppendLine("    AND PLACE.DELETE_DATE IS NULL ")
        'sqlString.AppendLine("  LEFT JOIN M_CODE CODE_Y ")
        'sqlString.AppendLine("    ON BASIC.YOBI_CD = CODE_Y.CODE_VALUE ")
        'sqlString.AppendLine("    AND CODE_Y.CODE_BUNRUI = '" & CodeBunrui.yobi & "' ")
        'sqlString.AppendLine("    AND CODE_Y.DELETE_DATE IS NULL ")
        'sqlString.AppendLine("  LEFT JOIN M_CODE CODE_U_T ")
        'sqlString.AppendLine("    ON BASIC.UNKYU_KBN = CODE_U_T.CODE_VALUE ")
        'sqlString.AppendLine("    AND CODE_U_T.CODE_BUNRUI = '" & CodeBunrui.unkyu & "' ")
        'sqlString.AppendLine("    AND CODE_U_T.DELETE_DATE IS NULL ")
        'sqlString.AppendLine("  LEFT JOIN M_CODE CODE_U_K ")
        'sqlString.AppendLine("    ON BASIC.UNKYU_KBN = CODE_U_K.CODE_VALUE ")
        'sqlString.AppendLine("    AND CODE_U_K.CODE_BUNRUI = '" & CodeBunrui.saikou & "' ")
        'sqlString.AppendLine("    AND CODE_U_K.DELETE_DATE IS NULL ")
        'sqlString.AppendLine("  LEFT JOIN M_CODE CODE_S ")
        'sqlString.AppendLine("    ON KOUSHA.SEISAN_TGT_CD = CODE_S.CODE_VALUE ")
        'sqlString.AppendLine("    AND CODE_S.CODE_BUNRUI = '" & CodeBunrui.seisanMokuteki & "' ")
        'sqlString.AppendLine("    AND CODE_S.DELETE_DATE IS NULL ")
        'sqlString.AppendLine("  LEFT JOIN M_CODE CODE_SK ")
        'sqlString.AppendLine("    ON SIIRE.SIIRE_SAKI_KIND_CD = CODE_SK.CODE_VALUE ")
        'sqlString.AppendLine("    AND CODE_SK.CODE_BUNRUI = '" & CodeBunrui.siireKind & "' ")
        'sqlString.AppendLine("    AND CODE_SK.DELETE_DATE IS NULL ")
        'sqlString.AppendLine("  LEFT JOIN ( ")
        'sqlString.AppendLine("    SELECT ")
        'sqlString.AppendLine("      CRS_CD ")
        'sqlString.AppendLine("      , SYUPT_DAY ")
        'sqlString.AppendLine("      , GOUSYA ")
        'sqlString.AppendLine("      , CRS_ITINERARY_LINE_NO ")
        'sqlString.AppendLine("      , SIIRE_SAKI_CD ")
        'sqlString.AppendLine("      , SIIRE_SAKI_EDABAN ")
        'sqlString.AppendLine("      , COUNT(CRS_CD) CNT ")
        'sqlString.AppendLine("    FROM ")
        'sqlString.AppendLine("      T_CRS_LEDGER_COST_KOSHAKASHO ")
        'sqlString.AppendLine("    WHERE ")
        'sqlString.AppendLine("      (CRS_CD, SYUPT_DAY, GOUSYA) IN (")
        'sqlString.AppendLine("        ").Append(paramList(Me.ParamHashKeys.BASIC_KEYS))
        'sqlString.AppendLine("      ) ")
        'sqlString.AppendLine("    GROUP BY ")
        'sqlString.AppendLine("      CRS_CD ")
        'sqlString.AppendLine("      , SYUPT_DAY ")
        'sqlString.AppendLine("      , GOUSYA ")
        'sqlString.AppendLine("      , CRS_ITINERARY_LINE_NO ")
        'sqlString.AppendLine("      , SIIRE_SAKI_CD ")
        'sqlString.AppendLine("      , SIIRE_SAKI_EDABAN ")
        'sqlString.AppendLine("  ) GENKA ")
        'sqlString.AppendLine("    ON KOUSHA.CRS_CD = GENKA.CRS_CD ")
        'sqlString.AppendLine("    AND KOUSHA.SYUPT_DAY = GENKA.SYUPT_DAY ")
        'sqlString.AppendLine("    AND KOUSHA.GOUSYA = GENKA.GOUSYA ")
        'sqlString.AppendLine("    AND KOUSHA.LINE_NO = GENKA.CRS_ITINERARY_LINE_NO ")
        'sqlString.AppendLine("    AND KOUSHA.KOSHAKASHO_CD = GENKA.SIIRE_SAKI_CD ")
        'sqlString.AppendLine("    AND KOUSHA.KOSHAKASHO_EDABAN = GENKA.SIIRE_SAKI_EDABAN ")
        'sqlString.AppendLine("  LEFT JOIN ( ")
        'sqlString.AppendLine("    SELECT ")
        'sqlString.AppendLine("      CRS_CD ")
        'sqlString.AppendLine("      , SYUPT_DAY ")
        'sqlString.AppendLine("      , GOUSYA ")
        'sqlString.AppendLine("      , CRS_ITINERARY_LINE_NO ")
        'sqlString.AppendLine("      , CARRIER_CD ")
        'sqlString.AppendLine("      , CARRIER_EDABAN ")
        'sqlString.AppendLine("      , COUNT(CRS_CD) CNT ")
        'sqlString.AppendLine("    FROM ")
        'sqlString.AppendLine("      T_CRS_LEDGER_COST_CARRIER ")
        'sqlString.AppendLine("    WHERE ")
        'sqlString.AppendLine("      (CRS_CD, SYUPT_DAY, GOUSYA) IN (")
        'sqlString.AppendLine("        ").Append(paramList(Me.ParamHashKeys.BASIC_KEYS))
        'sqlString.AppendLine("      ) ")
        'sqlString.AppendLine("      AND CRS_ITINERARY_LINE_NO > 0 ")
        'sqlString.AppendLine("    GROUP BY ")
        'sqlString.AppendLine("      CRS_CD ")
        'sqlString.AppendLine("      , SYUPT_DAY ")
        'sqlString.AppendLine("      , GOUSYA ")
        'sqlString.AppendLine("      , CRS_ITINERARY_LINE_NO ")
        'sqlString.AppendLine("      , CARRIER_CD ")
        'sqlString.AppendLine("      , CARRIER_EDABAN ")
        'sqlString.AppendLine("  ) GENKA_CARRIER ")
        'sqlString.AppendLine("    ON KOUSHA.CRS_CD = GENKA_CARRIER.CRS_CD ")
        'sqlString.AppendLine("    AND KOUSHA.SYUPT_DAY = GENKA_CARRIER.SYUPT_DAY ")
        'sqlString.AppendLine("    AND KOUSHA.GOUSYA = GENKA_CARRIER.GOUSYA ")
        'sqlString.AppendLine("    AND KOUSHA.LINE_NO = GENKA_CARRIER.CRS_ITINERARY_LINE_NO ")
        'sqlString.AppendLine("    AND KOUSHA.KOSHAKASHO_CD = GENKA_CARRIER.CARRIER_CD ")
        'sqlString.AppendLine("    AND KOUSHA.KOSHAKASHO_EDABAN = GENKA_CARRIER.CARRIER_EDABAN ")
        'sqlString.AppendLine("WHERE ")
        'sqlString.AppendLine("  BASIC.DELETE_DAY = 0 ")
        'sqlString.AppendLine("  AND (BASIC.CRS_CD, BASIC.SYUPT_DAY, BASIC.GOUSYA) IN (")
        'sqlString.AppendLine("    ").Append(paramList(Me.ParamHashKeys.BASIC_KEYS))
        'sqlString.AppendLine("  ) ")
        'sqlString.AppendLine("ORDER BY ")
        'sqlString.AppendLine("  BASIC.CRS_CD ")
        'sqlString.AppendLine("  , BASIC.SYUPT_DAY ")
        'sqlString.AppendLine("  , BASIC.GOUSYA ")
        'sqlString.AppendLine("  , KOUSHA.DAILY ")
        'sqlString.AppendLine("  , KOUSHA.KBN ")
        'sqlString.AppendLine("  , KOUSHA.LINE_NO ")

        Return sqlString.ToString

    End Function


#End Region

#Region "コース台帳Entity取得"
    ''' <summary>
    ''' コース台帳関連テーブルの読み込みを行う
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function getCrsLedgerBasicInfoEntity(ByVal paramList As Hashtable, ByRef crsLeaderBasicMaster As EntityOperation(Of TCrsLedgerBasicEntityTehaiEx)) As Boolean

        'コース台帳(基本)取得
        If getCrsLeadgerBasicEntity(paramList, crsLeaderBasicMaster) = False Then
            Return False
        End If

        'コース台帳(基本_料金区分)取得
        If getCrsLeadgerBasicChargeKbnEntity(paramList, crsLeaderBasicMaster) = False Then
            Return False
        End If

        'コース台帳原価(プレート)取得
        If getCrsLeadgerCostPlateEntity(paramList, crsLeaderBasicMaster) = False Then
            Return False
        End If

        'コース台帳原価(基本)取得
        If getCrsLeadgerCostBasicEntity(paramList, crsLeaderBasicMaster) = False Then
            Return False
        End If

        'コース台帳（降車ヶ所）取得
        If getCrsLeadgerKoshakashoEntity(paramList, crsLeaderBasicMaster) = False Then
            Return False
        End If

        'コース台帳原価（降車ヶ所）取得
        If getCrsLeadgerCostKoshakashoEntity(paramList, crsLeaderBasicMaster) = False Then
            Return False
        End If

        'コース台帳原価（降車ヶ所_料金区分）取得
        If getCrsLeadgerCostKoshakashoChargeKbnEntity(paramList, crsLeaderBasicMaster) = False Then
            Return False
        End If

        'コース台帳原価（キャリア）取得
        If getCrsLeadgerCostCarrierEntity(paramList, crsLeaderBasicMaster) = False Then
            Return False
        End If

        'コース台帳原価（キャリア_料金区分）取得
        If getCrsLeadgerCostCarrierChargeKbnEntity(paramList, crsLeaderBasicMaster) = False Then
            Return False
        End If

        'コース台帳（ホテル）取得　--テーブル申請待
        'If getCrsLeadgerHotelEntity(paramList, crsLeaderBasicMaster) = False Then
        '    Return False
        'End If

        Return True
    End Function

#Region "コース台帳(基本)"
    ''' <summary>
    ''' コース台帳(基本)を読み込みEntityクラスに格納する
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function getCrsLeadgerBasicEntity(ByVal paramList As Hashtable, ByVal crsLeaderBasic As EntityOperation(Of TCrsLedgerBasicEntityTehaiEx)) As Boolean  'getコースマスタEntity(対象コース情報,コースマスタ)

        Dim dtcrsMaster As DataTable  'コース台帳(基本)
        Try
            dtcrsMaster = GetDataCrsLeadgerBasic(paramList)
        Catch ex As Exception
            Throw
        End Try
        crsLeaderBasic.clear()

        For idx As Integer = 0 To crsLeaderBasic.getPropertyDataLength - 1
            With crsLeaderBasic
                If .getPtyType(idx, .EntityData(0)) Is GetType(EntityKoumoku_YmdType) Then
                    If IsDBNull(dtcrsMaster.Rows(0).Item(DirectCast(.getPtyValue(idx, .EntityData(0)), EntityKoumoku_YmdType).PhysicsName)) = False Then

                        DirectCast(.getPtyValue(idx, .EntityData(0)), EntityKoumoku_YmdType).Value = CDate(dtcrsMaster.Rows(0).Item(DirectCast(.getPtyValue(idx, .EntityData(0)), EntityKoumoku_YmdType).PhysicsName))
                    End If
                ElseIf .getPtyType(idx, .EntityData(0)) Is GetType(EntityKoumoku_NumberType) Then
                    If IsDBNull(dtcrsMaster.Rows(0).Item(DirectCast(.getPtyValue(idx, .EntityData(0)), EntityKoumoku_NumberType).PhysicsName)) = False Then
                        DirectCast(.getPtyValue(idx, .EntityData(0)), EntityKoumoku_NumberType).Value = CInt(dtcrsMaster.Rows(0).Item(DirectCast(.getPtyValue(idx, .EntityData(0)), EntityKoumoku_NumberType).PhysicsName))
                    End If
                ElseIf .getPtyType(idx, .EntityData(0)) Is GetType(EntityKoumoku_Number_DecimalType) Then
                    If IsDBNull(dtcrsMaster.Rows(0).Item(DirectCast(.getPtyValue(idx, .EntityData(0)), EntityKoumoku_Number_DecimalType).PhysicsName)) = False Then
                        DirectCast(.getPtyValue(idx, .EntityData(0)), EntityKoumoku_Number_DecimalType).Value = CDec(dtcrsMaster.Rows(0).Item(DirectCast(.getPtyValue(idx, .EntityData(0)), EntityKoumoku_Number_DecimalType).PhysicsName))
                    End If
                Else
                    DirectCast(.getPtyValue(idx, .EntityData(0)), EntityKoumoku_MojiType).Value = getDBValueForString(dtcrsMaster.Rows(0).Item(DirectCast(.getPtyValue(idx, .EntityData(0)), EntityKoumoku_MojiType).PhysicsName))
                End If
            End With
        Next

        Return True
    End Function

    ''' <summary>
    ''' コース台帳(基本)の読み込みを行う
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function GetDataCrsLeadgerBasic(ByVal paramList As Hashtable) As DataTable
        Dim resultDataTable As New DataTable
        Dim sqlString As New StringBuilder
        Dim entBasic As New TCrsLedgerBasicEntityTehaiEx

        MyBase.paramClear()

        sqlString.AppendLine("SELECT ")
        sqlString.AppendLine("    BASIC.CRS_CD ")
        sqlString.AppendLine("  , BASIC.SYUPT_DAY ")
        sqlString.AppendLine("  , BASIC.GOUSYA ")
        sqlString.AppendLine("  , BASIC.ACCESS_CD ")
        sqlString.AppendLine("  , BASIC.AIBEYA_USE_FLG ")
        sqlString.AppendLine("  , BASIC.AIBEYA_YOYAKU_NINZU_JYOSEI ")
        sqlString.AppendLine("  , BASIC.AIBEYA_YOYAKU_NINZU_MALE ")
        sqlString.AppendLine("  , BASIC.BIN_NAME ")
        sqlString.AppendLine("  , BASIC.BLOCK_KAKUHO_NUM ")
        sqlString.AppendLine("  , BASIC.BUS_COMPANY_CD ")
        sqlString.AppendLine("  , BASIC.BUS_RESERVE_CD ")
        sqlString.AppendLine("  , BASIC.CANCEL_NG_FLG ")
        sqlString.AppendLine("  , BASIC.CANCEL_RYOU_KBN ")
        sqlString.AppendLine("  , BASIC.CANCEL_WAIT_NINZU ")
        sqlString.AppendLine("  , BASIC.CAPACITY_HO_1KAI ")
        sqlString.AppendLine("  , BASIC.CAPACITY_REGULAR ")
        sqlString.AppendLine("  , BASIC.CARRIER_CD ")
        sqlString.AppendLine("  , BASIC.CARRIER_EDABAN ")
        sqlString.AppendLine("  , BASIC.CAR_NO ")
        sqlString.AppendLine("  , BASIC.CAR_TYPE_CD ")
        sqlString.AppendLine("  , BASIC.CAR_TYPE_CD_YOTEI ")
        sqlString.AppendLine("  , BASIC.BUS_COUNT_FLG ")
        sqlString.AppendLine("  , BASIC.CATEGORY_CD_1 ")
        sqlString.AppendLine("  , BASIC.CATEGORY_CD_2 ")
        sqlString.AppendLine("  , BASIC.CATEGORY_CD_3 ")
        sqlString.AppendLine("  , BASIC.CATEGORY_CD_4 ")
        sqlString.AppendLine("  , BASIC.COST_SET_KBN ")
        sqlString.AppendLine("  , BASIC.CRS_BLOCK_CAPACITY ")
        sqlString.AppendLine("  , BASIC.CRS_BLOCK_ONE_1R ")
        sqlString.AppendLine("  , BASIC.CRS_BLOCK_ROOM_NUM ")
        sqlString.AppendLine("  , BASIC.CRS_BLOCK_THREE_1R ")
        sqlString.AppendLine("  , BASIC.CRS_BLOCK_TWO_1R ")
        sqlString.AppendLine("  , BASIC.CRS_BLOCK_FOUR_1R ")
        sqlString.AppendLine("  , BASIC.CRS_BLOCK_FIVE_1R ")
        sqlString.AppendLine("  , BASIC.CRS_KBN_1 ")
        sqlString.AppendLine("  , BASIC.CRS_KBN_2 ")
        sqlString.AppendLine("  , BASIC.CRS_KIND ")
        sqlString.AppendLine("  , BASIC.MANAGEMENT_SEC ")
        sqlString.AppendLine("  , BASIC.GUIDE_GENGO ")
        sqlString.AppendLine("  , BASIC.CRS_NAME ")
        sqlString.AppendLine("  , BASIC.CRS_NAME_KANA ")
        sqlString.AppendLine("  , BASIC.CRS_NAME_RK ")
        sqlString.AppendLine("  , BASIC.CRS_NAME_KANA_RK ")
        sqlString.AppendLine("  , BASIC.DELETE_DAY ")
        sqlString.AppendLine("  , BASIC.EI_BLOCK_HO ")
        sqlString.AppendLine("  , BASIC.EI_BLOCK_REGULAR ")
        sqlString.AppendLine("  , BASIC.END_PLACE_CD ")
        sqlString.AppendLine("  , BASIC.END_TIME ")
        sqlString.AppendLine("  , BASIC.HAISYA_KEIYU_CD_1 ")
        sqlString.AppendLine("  , BASIC.HAISYA_KEIYU_CD_2 ")
        sqlString.AppendLine("  , BASIC.HAISYA_KEIYU_CD_3 ")
        sqlString.AppendLine("  , BASIC.HAISYA_KEIYU_CD_4 ")
        sqlString.AppendLine("  , BASIC.HAISYA_KEIYU_CD_5 ")
        sqlString.AppendLine("  , BASIC.HOMEN_CD ")
        sqlString.AppendLine("  , BASIC.HOUJIN_GAIKYAKU_KBN ")
        sqlString.AppendLine("  , BASIC.HURIKOMI_NG_FLG ")
        sqlString.AppendLine("  , BASIC.ITINERARY_TABLE_CREATE_FLG ")
        sqlString.AppendLine("  , BASIC.JYOSEI_SENYO_SEAT_FLG ")
        sqlString.AppendLine("  , BASIC.JYOSYA_CAPACITY ")
        sqlString.AppendLine("  , BASIC.KAITEI_DAY ")
        sqlString.AppendLine("  , BASIC.KUSEKI_KAKUHO_NUM ")
        sqlString.AppendLine("  , BASIC.KUSEKI_NUM_SUB_SEAT ")
        sqlString.AppendLine("  , BASIC.KUSEKI_NUM_TEISEKI ")
        sqlString.AppendLine("  , BASIC.KYOSAI_UNKOU_KBN ")
        sqlString.AppendLine("  , BASIC.MAEURI_KIGEN ")
        sqlString.AppendLine("  , BASIC.MARU_ZOU_MANAGEMENT_KBN ")
        sqlString.AppendLine("  , BASIC.MEAL_COUNT_MORNING ")
        sqlString.AppendLine("  , BASIC.MEAL_COUNT_NIGHT ")
        sqlString.AppendLine("  , BASIC.MEAL_COUNT_NOON ")
        sqlString.AppendLine("  , BASIC.MEDIA_CHECK_FLG ")
        sqlString.AppendLine("  , BASIC.MEIBO_INPUT_FLG ")
        sqlString.AppendLine("  , BASIC.NINZU_INPUT_FLG_KEIYU_1 ")
        sqlString.AppendLine("  , BASIC.NINZU_INPUT_FLG_KEIYU_2 ")
        sqlString.AppendLine("  , BASIC.NINZU_INPUT_FLG_KEIYU_3 ")
        sqlString.AppendLine("  , BASIC.NINZU_INPUT_FLG_KEIYU_4 ")
        sqlString.AppendLine("  , BASIC.NINZU_INPUT_FLG_KEIYU_5 ")
        sqlString.AppendLine("  , BASIC.NINZU_KEIYU_1_ADULT ")
        sqlString.AppendLine("  , BASIC.NINZU_KEIYU_1_CHILD ")
        sqlString.AppendLine("  , BASIC.NINZU_KEIYU_1_JUNIOR ")
        sqlString.AppendLine("  , BASIC.NINZU_KEIYU_1_S ")
        sqlString.AppendLine("  , BASIC.NINZU_KEIYU_2_ADULT ")
        sqlString.AppendLine("  , BASIC.NINZU_KEIYU_2_CHILD ")
        sqlString.AppendLine("  , BASIC.NINZU_KEIYU_2_JUNIOR ")
        sqlString.AppendLine("  , BASIC.NINZU_KEIYU_2_S ")
        sqlString.AppendLine("  , BASIC.NINZU_KEIYU_3_ADULT ")
        sqlString.AppendLine("  , BASIC.NINZU_KEIYU_3_CHILD ")
        sqlString.AppendLine("  , BASIC.NINZU_KEIYU_3_JUNIOR ")
        sqlString.AppendLine("  , BASIC.NINZU_KEIYU_3_S ")
        sqlString.AppendLine("  , BASIC.NINZU_KEIYU_4_ADULT ")
        sqlString.AppendLine("  , BASIC.NINZU_KEIYU_4_CHILD ")
        sqlString.AppendLine("  , BASIC.NINZU_KEIYU_4_JUNIOR ")
        sqlString.AppendLine("  , BASIC.NINZU_KEIYU_4_S ")
        sqlString.AppendLine("  , BASIC.NINZU_KEIYU_5_ADULT ")
        sqlString.AppendLine("  , BASIC.NINZU_KEIYU_5_CHILD ")
        sqlString.AppendLine("  , BASIC.NINZU_KEIYU_5_JUNIOR ")
        sqlString.AppendLine("  , BASIC.NINZU_KEIYU_5_S ")
        sqlString.AppendLine("  , BASIC.ONE_SANKA_FLG ")
        sqlString.AppendLine("  , BASIC.OPTION_FLG ")
        sqlString.AppendLine("  , BASIC.PICKUP_KBN_1 ")
        sqlString.AppendLine("  , BASIC.PICKUP_KBN_2 ")
        sqlString.AppendLine("  , BASIC.PICKUP_KBN_3 ")
        sqlString.AppendLine("  , BASIC.PICKUP_KBN_4 ")
        sqlString.AppendLine("  , BASIC.PICKUP_KBN_5 ")
        sqlString.AppendLine("  , BASIC.RETURN_DAY ")
        sqlString.AppendLine("  , BASIC.ROOM_ZANSU_ONE_ROOM ")
        sqlString.AppendLine("  , BASIC.ROOM_ZANSU_SOKEI ")
        sqlString.AppendLine("  , BASIC.ROOM_ZANSU_THREE_ROOM ")
        sqlString.AppendLine("  , BASIC.ROOM_ZANSU_TWO_ROOM ")
        sqlString.AppendLine("  , BASIC.ROOM_ZANSU_FOUR_ROOM ")
        sqlString.AppendLine("  , BASIC.ROOM_ZANSU_FIVE_ROOM ")
        sqlString.AppendLine("  , BASIC.MIN_SAIKOU_NINZU ")
        sqlString.AppendLine("  , BASIC.SAIKOU_DAY ")
        sqlString.AppendLine("  , BASIC.SAIKOU_KAKUTEI_KBN ")
        sqlString.AppendLine("  , BASIC.SEASON_CD ")
        sqlString.AppendLine("  , BASIC.SENYO_CRS_KBN ")
        sqlString.AppendLine("  , BASIC.SHANAI_CONTACT_FOR_MESSAGE ")
        sqlString.AppendLine("  , BASIC.SHUKUJITSU_FLG ")
        sqlString.AppendLine("  , BASIC.SINSETU_YM ")
        sqlString.AppendLine("  , BASIC.STAY_DAY ")
        sqlString.AppendLine("  , BASIC.STAY_STAY ")
        sqlString.AppendLine("  , BASIC.SUB_SEAT_OK_KBN ")
        sqlString.AppendLine("  , BASIC.SYOYO_TIME ")
        sqlString.AppendLine("  , BASIC.SYUGO_PLACE_CD_CARRIER ")
        sqlString.AppendLine("  , BASIC.SYUGO_TIME_1 ")
        sqlString.AppendLine("  , BASIC.SYUGO_TIME_2 ")
        sqlString.AppendLine("  , BASIC.SYUGO_TIME_3 ")
        sqlString.AppendLine("  , BASIC.SYUGO_TIME_4 ")
        sqlString.AppendLine("  , BASIC.SYUGO_TIME_5 ")
        sqlString.AppendLine("  , BASIC.SYUGO_TIME_CARRIER ")
        sqlString.AppendLine("  , BASIC.SYUPT_JI_CARRIER_KBN ")
        sqlString.AppendLine("  , BASIC.SYUPT_PLACE_CARRIER ")
        sqlString.AppendLine("  , BASIC.SYUPT_PLACE_CD_CARRIER ")
        sqlString.AppendLine("  , BASIC.SYUPT_TIME_1 ")
        sqlString.AppendLine("  , BASIC.SYUPT_TIME_2 ")
        sqlString.AppendLine("  , BASIC.SYUPT_TIME_3 ")
        sqlString.AppendLine("  , BASIC.SYUPT_TIME_4 ")
        sqlString.AppendLine("  , BASIC.SYUPT_TIME_5 ")
        sqlString.AppendLine("  , BASIC.SYUPT_TIME_CARRIER ")
        sqlString.AppendLine("  , BASIC.TEIINSEI_FLG ")
        sqlString.AppendLine("  , BASIC.TEIKI_CRS_KBN ")
        sqlString.AppendLine("  , BASIC.TEIKI_KIKAKU_KBN ")
        sqlString.AppendLine("  , BASIC.TEJIMAI_CONTACT_KBN ")
        sqlString.AppendLine("  , BASIC.TEJIMAI_DAY ")
        sqlString.AppendLine("  , BASIC.TEJIMAI_KBN ")
        sqlString.AppendLine("  , BASIC.TENJYOIN_CD ")
        sqlString.AppendLine("  , BASIC.TIE_TYAKUYO ")
        sqlString.AppendLine("  , BASIC.TOKUTEI_CHARGE_SET ")
        sqlString.AppendLine("  , BASIC.TOKUTEI_DAY_FLG ")
        sqlString.AppendLine("  , BASIC.TTYAK_PLACE_CARRIER ")
        sqlString.AppendLine("  , BASIC.TTYAK_PLACE_CD_CARRIER ")
        sqlString.AppendLine("  , BASIC.TTYAK_TIME_CARRIER ")
        sqlString.AppendLine("  , BASIC.TYUIJIKOU ")
        sqlString.AppendLine("  , BASIC.TYUIJIKOU_KBN ")
        sqlString.AppendLine("  , BASIC.UKETUKE_GENTEI_NINZU ")
        sqlString.AppendLine("  , BASIC.UKETUKE_START_BI ")
        sqlString.AppendLine("  , BASIC.UKETUKE_START_DAY ")
        sqlString.AppendLine("  , BASIC.UKETUKE_START_KAGETUMAE ")
        sqlString.AppendLine("  , BASIC.UNDER_KINSI_18OLD ")
        sqlString.AppendLine("  , BASIC.UNKYU_CONTACT_DAY ")
        sqlString.AppendLine("  , BASIC.UNKYU_CONTACT_DONE_FLG ")
        sqlString.AppendLine("  , BASIC.UNKYU_CONTACT_TIME ")
        sqlString.AppendLine("  , BASIC.UNKYU_KBN ")
        sqlString.AppendLine("  , BASIC.TOJITU_KOKUCHI_FLG ")
        sqlString.AppendLine("  , BASIC.YUSEN_YOYAKU_FLG ")
        sqlString.AppendLine("  , BASIC.PICKUP_KBN_FLG ")
        sqlString.AppendLine("  , BASIC.KONJYO_OK_FLG ")
        sqlString.AppendLine("  , BASIC.TONARI_FLG ")
        sqlString.AppendLine("  , BASIC.AHEAD_ZASEKI_FLG ")
        sqlString.AppendLine("  , BASIC.YOYAKU_MEDIA_INPUT_FLG ")
        sqlString.AppendLine("  , BASIC.KOKUSEKI_FLG ")
        sqlString.AppendLine("  , BASIC.SEX_BETU_FLG ")
        sqlString.AppendLine("  , BASIC.AGE_FLG ")
        sqlString.AppendLine("  , BASIC.BIRTHDAY_FLG ")
        sqlString.AppendLine("  , BASIC.TEL_FLG ")
        sqlString.AppendLine("  , BASIC.ADDRESS_FLG ")
        sqlString.AppendLine("  , BASIC.USING_FLG ")
        sqlString.AppendLine("  , BASIC.UWAGI_TYAKUYO ")
        sqlString.AppendLine("  , BASIC.YEAR ")
        sqlString.AppendLine("  , BASIC.YOBI_CD ")
        sqlString.AppendLine("  , BASIC.YOYAKU_ALREADY_ROOM_NUM ")
        sqlString.AppendLine("  , BASIC.YOYAKU_KANOU_NUM ")
        sqlString.AppendLine("  , BASIC.YOYAKU_NG_FLG ")
        sqlString.AppendLine("  , BASIC.YOYAKU_NUM_SUB_SEAT ")
        sqlString.AppendLine("  , BASIC.YOYAKU_NUM_TEISEKI ")
        sqlString.AppendLine("  , BASIC.YOYAKU_STOP_FLG ")
        sqlString.AppendLine("  , BASIC.ZASEKI_HIHYOJI_FLG ")
        sqlString.AppendLine("  , BASIC.ZASEKI_RESERVE_KBN ")
        sqlString.AppendLine("  , BASIC.WT_KAKUHO_SEAT_NUM ")
        sqlString.AppendLine("  , BASIC.SYSTEM_ENTRY_PGMID ")
        sqlString.AppendLine("  , BASIC.SYSTEM_ENTRY_PERSON_CD ")
        sqlString.AppendLine("  , BASIC.SYSTEM_ENTRY_DAY ")
        sqlString.AppendLine("  , BASIC.SYSTEM_UPDATE_PGMID ")
        sqlString.AppendLine("  , BASIC.SYSTEM_UPDATE_PERSON_CD ")
        sqlString.AppendLine("  , BASIC.SYSTEM_UPDATE_DAY ")
        '以下マスタ追加項目
        '出発日(表示)
        sqlString.AppendLine("  , TO_CHAR(TO_DATE(BASIC.SYUPT_DAY), 'yyyy/MM/dd') AS DISP_SYUPT_DAY ")
        '乗車地
        sqlString.AppendLine("  , PLACE.PLACE_NAME_1 AS JYOSYA_TI_NAME ")
        '曜日
        sqlString.AppendLine("  , CODE_Y.CODE_NAME AS YOBI_NAME ")
        '出発時間
        sqlString.AppendLine("  , SUBSTR(TO_CHAR(LPAD(BASIC.SYUPT_TIME_1, 4, '0')), 0, 2) || ':' || SUBSTR(TO_CHAR(LPAD(BASIC.SYUPT_TIME_1, 4, '0')), 3, 2) AS SYUPT_TIME ")
        '運休区分
        sqlString.AppendLine("  , CASE BASIC.TEIKI_KIKAKU_KBN")
        sqlString.AppendLine("  WHEN '1' THEN CODE_U_T.CODE_NAME")
        sqlString.AppendLine("  ELSE CODE_U_K.CODE_NAME")
        sqlString.AppendLine("  END As UNKYU_NAME")
        sqlString.AppendLine("  ,'0' AS EDIT_FLG")
        sqlString.AppendLine("  ,COALESCE(COST.LINE_NO,0) AS CARRIER_LINENO")
        sqlString.AppendLine("  ,'0' AS KOUSHA_EDIT_FLG")
        sqlString.AppendLine("  ,'0' AS SONOTA_EDIT_FLG")
        sqlString.AppendLine("FROM ")
        sqlString.AppendLine("  T_CRS_LEDGER_BASIC BASIC")
        sqlString.AppendLine("  LEFT JOIN M_PLACE PLACE ")
        sqlString.AppendLine("  ON BASIC.HAISYA_KEIYU_CD_1 = PLACE.PLACE_CD ")
        sqlString.AppendLine("     AND PLACE.DELETE_DATE IS NULL ")
        sqlString.AppendLine("  LEFT JOIN M_CODE CODE_Y ")
        sqlString.AppendLine("    ON BASIC.YOBI_CD = CODE_Y.CODE_VALUE ")
        sqlString.AppendLine("    AND CODE_Y.CODE_BUNRUI = '" & CodeBunrui.yobi & "' ")
        sqlString.AppendLine("    AND CODE_Y.DELETE_DATE IS NULL ")
        sqlString.AppendLine("  LEFT JOIN M_CODE CODE_U_T ")
        sqlString.AppendLine("    ON BASIC.UNKYU_KBN = CODE_U_T.CODE_VALUE ")
        sqlString.AppendLine("    AND CODE_U_T.CODE_BUNRUI = '" & CodeBunrui.unkyu & "' ")
        sqlString.AppendLine("    AND CODE_U_T.DELETE_DATE IS NULL ")
        sqlString.AppendLine("  LEFT JOIN M_CODE CODE_U_K ")
        sqlString.AppendLine("    ON BASIC.SAIKOU_KAKUTEI_KBN = CODE_U_K.CODE_VALUE ")
        sqlString.AppendLine("    AND CODE_U_K.CODE_BUNRUI = '" & CodeBunrui.saikou & "' ")
        sqlString.AppendLine("    AND CODE_U_K.DELETE_DATE IS NULL ")
        sqlString.AppendLine("  LEFT JOIN T_CRS_LEDGER_COST_CARRIER COST ")
        sqlString.AppendLine("    ON COST.CRS_CD = BASIC.CRS_CD ")
        sqlString.AppendLine("    AND COST.SYUPT_DAY = BASIC.SYUPT_DAY ")
        sqlString.AppendLine("    AND COST.GOUSYA = BASIC.GOUSYA ")
        sqlString.AppendLine("    AND COST.CRS_ITINERARY_LINE_NO = 0 ")
        sqlString.AppendLine("WHERE")
        sqlString.AppendLine("   " & makeSqlEx(entBasic.crsCd, paramList, "BASIC"))
        sqlString.AppendLine("   AND")
        sqlString.AppendLine("   " & makeSqlEx(entBasic.syuptDay, paramList, "BASIC"))
        sqlString.AppendLine("   AND")
        sqlString.AppendLine("   " & makeSqlEx(entBasic.gousya, paramList, "BASIC"))
        sqlString.AppendLine("   AND")
        sqlString.AppendLine("   BASIC.DELETE_DAY = 0")
        Try
            resultDataTable = MyBase.getDataTable(sqlString.ToString)
        Catch ex As Exception
            Throw
        End Try

        Return resultDataTable
    End Function
#End Region

#Region "コース台帳(基本_料金区分)"
    ''' <summary>
    ''' コース台帳(基本_料金区分)を読み込みEntityクラスに格納する
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function getCrsLeadgerBasicChargeKbnEntity(ByVal paramList As Hashtable, ByVal crsLeaderBasic As EntityOperation(Of TCrsLedgerBasicEntityTehaiEx)) As Boolean  'getコースマスタEntity(対象コース情報,コースマスタ)

        Dim dtChargeKbnMaster As DataTable  'コース台帳(基本_料金区分)
        Try
            dtChargeKbnMaster = GetDataCrsLeadgerBasicChargeKbn(paramList)
        Catch ex As Exception
            Throw
        End Try

        For idxRow As Integer = 0 To dtChargeKbnMaster.Rows.Count - 1

            If idxRow > 0 Then
                crsLeaderBasic.EntityData(0).ChargeKbnEntity.add(New TCrsLedgerBasicChargeKbnEntityTehaiEx)
            End If

            For idx As Integer = 0 To crsLeaderBasic.EntityData(0).ChargeKbnEntity.getPropertyDataLength - 1
                With crsLeaderBasic.EntityData(0).ChargeKbnEntity
                    If .getPtyType(idx, .EntityData(idxRow)) Is GetType(EntityKoumoku_YmdType) Then
                        If IsDBNull(dtChargeKbnMaster.Rows(idxRow).Item(DirectCast(.getPtyValue(idx, .EntityData(idxRow)), EntityKoumoku_YmdType).PhysicsName)) = False Then
                            DirectCast(.getPtyValue(idx, .EntityData(idxRow)), EntityKoumoku_YmdType).Value = CDate(dtChargeKbnMaster.Rows(idxRow).Item(DirectCast(.getPtyValue(idx, .EntityData(idxRow)), EntityKoumoku_YmdType).PhysicsName))
                        End If
                    ElseIf .getPtyType(idx, .EntityData(idxRow)) Is GetType(EntityKoumoku_NumberType) Then
                        If IsDBNull(dtChargeKbnMaster.Rows(idxRow).Item(DirectCast(.getPtyValue(idx, .EntityData(idxRow)), EntityKoumoku_NumberType).PhysicsName)) = False Then
                            DirectCast(.getPtyValue(idx, .EntityData(idxRow)), EntityKoumoku_NumberType).Value = CInt(dtChargeKbnMaster.Rows(idxRow).Item(DirectCast(.getPtyValue(idx, .EntityData(idxRow)), EntityKoumoku_NumberType).PhysicsName))
                        End If
                    ElseIf .getPtyType(idx, .EntityData(idxRow)) Is GetType(EntityKoumoku_Number_DecimalType) Then
                        If IsDBNull(dtChargeKbnMaster.Rows(idxRow).Item(DirectCast(.getPtyValue(idx, .EntityData(idxRow)), EntityKoumoku_Number_DecimalType).PhysicsName)) = False Then
                            DirectCast(.getPtyValue(idx, .EntityData(idxRow)), EntityKoumoku_Number_DecimalType).Value = CDec(dtChargeKbnMaster.Rows(idxRow).Item(DirectCast(.getPtyValue(idx, .EntityData(idxRow)), EntityKoumoku_Number_DecimalType).PhysicsName))
                        End If
                    Else
                        DirectCast(.getPtyValue(idx, .EntityData(idxRow)), EntityKoumoku_MojiType).Value = getDBValueForString(dtChargeKbnMaster.Rows(idxRow).Item(DirectCast(.getPtyValue(idx, .EntityData(idxRow)), EntityKoumoku_MojiType).PhysicsName))
                    End If
                End With
            Next
        Next

        Return True
    End Function

    ''' <summary>
    ''' コース台帳(基本_料金区分)の読み込みを行う
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function GetDataCrsLeadgerBasicChargeKbn(ByVal paramList As Hashtable) As DataTable
        Dim resultDataTable As New DataTable
        Dim sqlString As New StringBuilder
        Dim entBasic As New TCrsLedgerBasicEntityTehaiEx
        MyBase.paramClear()

        sqlString.AppendLine("SELECT ")
        sqlString.AppendLine("  CHGKBN.SYUPT_DAY ")
        sqlString.AppendLine("  , CHGKBN.CRS_CD ")
        sqlString.AppendLine("  , CHGKBN.GOUSYA ")
        sqlString.AppendLine("  , CHGKBN.LINE_NO ")
        sqlString.AppendLine("  , CHGKBN.CHARGE_KBN_JININ_CD ")
        sqlString.AppendLine("  , CHGKBN.SYSTEM_ENTRY_PGMID ")
        sqlString.AppendLine("  , CHGKBN.SYSTEM_ENTRY_PERSON_CD ")
        sqlString.AppendLine("  , CHGKBN.SYSTEM_ENTRY_DAY ")
        sqlString.AppendLine("  , CHGKBN.SYSTEM_UPDATE_PGMID ")
        sqlString.AppendLine("  , CHGKBN.SYSTEM_UPDATE_PERSON_CD ")
        sqlString.AppendLine("  , CHGKBN.SYSTEM_UPDATE_DAY ")
        sqlString.AppendLine("  , CHGKBN.DELETE_DATE ")
        sqlString.AppendLine("  , MST.CHARGE_KBN_JININ_NAME ")
        sqlString.AppendLine("  , MST.SHUYAKU_CHARGE_KBN_CD ")
        '以下マスタ追加項目
        sqlString.AppendLine("FROM ")
        sqlString.AppendLine("  T_CRS_LEDGER_BASIC_CHARGE_KBN CHGKBN ")
        sqlString.AppendLine("  LEFT JOIN M_CHARGE_JININ_KBN MST ")
        sqlString.AppendLine("    ON CHGKBN.CHARGE_KBN_JININ_CD = MST.CHARGE_KBN_JININ_CD ")
        sqlString.AppendLine("    AND MST.DELETE_DAY IS NULL ")
        sqlString.AppendLine("WHERE")
        sqlString.AppendLine("   " & makeSqlEx(entBasic.crsCd, paramList, "CHGKBN"))
        sqlString.AppendLine("   AND")
        sqlString.AppendLine("   " & makeSqlEx(entBasic.syuptDay, paramList, "CHGKBN"))
        sqlString.AppendLine("   AND")
        sqlString.AppendLine("   " & makeSqlEx(entBasic.gousya, paramList, "CHGKBN"))
        sqlString.AppendLine("   AND")
        sqlString.AppendLine("   CHGKBN.DELETE_DATE = 0")
        sqlString.AppendLine("ORDER BY ")
        sqlString.AppendLine("  CHGKBN.LINE_NO ")

        Try
            resultDataTable = MyBase.getDataTable(sqlString.ToString)
        Catch ex As Exception
            Throw
        End Try

        Return resultDataTable
    End Function
#End Region

#Region "コース台帳原価(プレート)"
    ''' <summary>
    ''' コース台帳(プレート)を読み込みEntityクラスに格納する
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function getCrsLeadgerCostPlateEntity(ByVal paramList As Hashtable, ByVal crsLeaderBasic As EntityOperation(Of TCrsLedgerBasicEntityTehaiEx)) As Boolean  'getコースマスタEntity(対象コース情報,コースマスタ)

        Dim dtCostPlate As DataTable  'コース台帳(プレート)
        Try
            dtCostPlate = GetDataCrsLeadgerCostPlate(paramList)
        Catch ex As Exception
            Throw
        End Try

        For idxRow As Integer = 0 To dtCostPlate.Rows.Count - 1

            If idxRow > 0 Then
                crsLeaderBasic.EntityData(0).CostPlateEntity.add(New TCrsLedgerCostPlateEntity)
            End If

            For idx As Integer = 0 To crsLeaderBasic.EntityData(0).CostPlateEntity.getPropertyDataLength - 1
                With crsLeaderBasic.EntityData(0).CostPlateEntity
                    If .getPtyType(idx, .EntityData(idxRow)) Is GetType(EntityKoumoku_YmdType) Then
                        If IsDBNull(dtCostPlate.Rows(idxRow).Item(DirectCast(.getPtyValue(idx, .EntityData(idxRow)), EntityKoumoku_YmdType).PhysicsName)) = False Then
                            DirectCast(.getPtyValue(idx, .EntityData(idxRow)), EntityKoumoku_YmdType).Value = CDate(dtCostPlate.Rows(idxRow).Item(DirectCast(.getPtyValue(idx, .EntityData(idxRow)), EntityKoumoku_YmdType).PhysicsName))
                        End If
                    ElseIf .getPtyType(idx, .EntityData(idxRow)) Is GetType(EntityKoumoku_NumberType) Then
                        If IsDBNull(dtCostPlate.Rows(idxRow).Item(DirectCast(.getPtyValue(idx, .EntityData(idxRow)), EntityKoumoku_NumberType).PhysicsName)) = False Then
                            DirectCast(.getPtyValue(idx, .EntityData(idxRow)), EntityKoumoku_NumberType).Value = CInt(dtCostPlate.Rows(idxRow).Item(DirectCast(.getPtyValue(idx, .EntityData(idxRow)), EntityKoumoku_NumberType).PhysicsName))
                        End If
                    ElseIf .getPtyType(idx, .EntityData(idxRow)) Is GetType(EntityKoumoku_Number_DecimalType) Then
                        If IsDBNull(dtCostPlate.Rows(idxRow).Item(DirectCast(.getPtyValue(idx, .EntityData(idxRow)), EntityKoumoku_Number_DecimalType).PhysicsName)) = False Then
                            DirectCast(.getPtyValue(idx, .EntityData(idxRow)), EntityKoumoku_Number_DecimalType).Value = CDec(dtCostPlate.Rows(idxRow).Item(DirectCast(.getPtyValue(idx, .EntityData(idxRow)), EntityKoumoku_Number_DecimalType).PhysicsName))
                        End If
                    Else
                        DirectCast(.getPtyValue(idx, .EntityData(idxRow)), EntityKoumoku_MojiType).Value = getDBValueForString(dtCostPlate.Rows(idxRow).Item(DirectCast(.getPtyValue(idx, .EntityData(idxRow)), EntityKoumoku_MojiType).PhysicsName))
                    End If
                End With
            Next
        Next

        Return True
    End Function

    ''' <summary>
    ''' コース台帳(プレート)の読み込みを行う
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function GetDataCrsLeadgerCostPlate(ByVal paramList As Hashtable) As DataTable
        Dim resultDataTable As New DataTable
        Dim sqlString As New StringBuilder
        Dim entBasic As New TCrsLedgerBasicEntityTehaiEx
        MyBase.paramClear()

        sqlString.AppendLine("SELECT ")
        sqlString.AppendLine("  PLATE.CRS_CD ")
        sqlString.AppendLine("  , PLATE.DELETE_DAY ")
        sqlString.AppendLine("  , PLATE.GOUSYA ")
        sqlString.AppendLine("  , PLATE.KINGAKU ")
        sqlString.AppendLine("  , PLATE.KUKAN_FROM ")
        sqlString.AppendLine("  , PLATE.KUKAN_TO ")
        sqlString.AppendLine("  , PLATE.LINE_NO ")
        sqlString.AppendLine("  , PLATE.SYUPT_DAY ")
        sqlString.AppendLine("  , PLATE.SYSTEM_ENTRY_PGMID ")
        sqlString.AppendLine("  , PLATE.SYSTEM_ENTRY_PERSON_CD ")
        sqlString.AppendLine("  , PLATE.SYSTEM_ENTRY_DAY ")
        sqlString.AppendLine("  , PLATE.SYSTEM_UPDATE_PGMID ")
        sqlString.AppendLine("  , PLATE.SYSTEM_UPDATE_PERSON_CD ")
        sqlString.AppendLine("  , PLATE.SYSTEM_UPDATE_DAY ")
        sqlString.AppendLine("FROM ")
        sqlString.AppendLine("  T_CRS_LEDGER_COST_PLATE PLATE ")
        sqlString.AppendLine("WHERE")
        sqlString.AppendLine("   " & makeSqlEx(entBasic.crsCd, paramList, "PLATE"))
        sqlString.AppendLine("   AND")
        sqlString.AppendLine("   " & makeSqlEx(entBasic.syuptDay, paramList, "PLATE"))
        sqlString.AppendLine("   AND")
        sqlString.AppendLine("   " & makeSqlEx(entBasic.gousya, paramList, "PLATE"))
        sqlString.AppendLine("   AND")
        sqlString.AppendLine("   PLATE.DELETE_DAY = 0 ")
        sqlString.AppendLine("ORDER BY ")
        sqlString.AppendLine("  PLATE.LINE_NO ")

        Try
            resultDataTable = MyBase.getDataTable(sqlString.ToString)
        Catch ex As Exception
            Throw
        End Try

        Return resultDataTable
    End Function
#End Region

#Region "コース台帳原価(基本)"
    ''' <summary>
    ''' コース台帳原価(基本)を読み込みEntityクラスに格納する
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function getCrsLeadgerCostBasicEntity(ByVal paramList As Hashtable, ByVal crsLeaderBasic As EntityOperation(Of TCrsLedgerBasicEntityTehaiEx)) As Boolean  'getコースマスタEntity(対象コース情報,コースマスタ)

        Dim dtCostBasic As DataTable  'コース台帳原価(基本)
        Try
            dtCostBasic = GetDataCrsLeadgerCostBasic(paramList)
        Catch ex As Exception
            Throw
        End Try

        For idxRow As Integer = 0 To dtCostBasic.Rows.Count - 1

            If idxRow > 0 Then
                crsLeaderBasic.EntityData(0).CostBasicEntity.add(New TCrsLedgerCostBasicEntity)
            End If

            For idx As Integer = 0 To crsLeaderBasic.EntityData(0).CostBasicEntity.getPropertyDataLength - 1
                With crsLeaderBasic.EntityData(0).CostBasicEntity
                    If .getPtyType(idx, .EntityData(idxRow)) Is GetType(EntityKoumoku_YmdType) Then
                        If IsDBNull(dtCostBasic.Rows(idxRow).Item(DirectCast(.getPtyValue(idx, .EntityData(idxRow)), EntityKoumoku_YmdType).PhysicsName)) = False Then
                            DirectCast(.getPtyValue(idx, .EntityData(idxRow)), EntityKoumoku_YmdType).Value = CDate(dtCostBasic.Rows(idxRow).Item(DirectCast(.getPtyValue(idx, .EntityData(idxRow)), EntityKoumoku_YmdType).PhysicsName))
                        End If
                    ElseIf .getPtyType(idx, .EntityData(idxRow)) Is GetType(EntityKoumoku_NumberType) Then
                        If IsDBNull(dtCostBasic.Rows(idxRow).Item(DirectCast(.getPtyValue(idx, .EntityData(idxRow)), EntityKoumoku_NumberType).PhysicsName)) = False Then
                            DirectCast(.getPtyValue(idx, .EntityData(idxRow)), EntityKoumoku_NumberType).Value = CInt(dtCostBasic.Rows(idxRow).Item(DirectCast(.getPtyValue(idx, .EntityData(idxRow)), EntityKoumoku_NumberType).PhysicsName))
                        End If
                    ElseIf .getPtyType(idx, .EntityData(idxRow)) Is GetType(EntityKoumoku_Number_DecimalType) Then
                        If IsDBNull(dtCostBasic.Rows(idxRow).Item(DirectCast(.getPtyValue(idx, .EntityData(idxRow)), EntityKoumoku_Number_DecimalType).PhysicsName)) = False Then
                            DirectCast(.getPtyValue(idx, .EntityData(idxRow)), EntityKoumoku_Number_DecimalType).Value = CDec(dtCostBasic.Rows(idxRow).Item(DirectCast(.getPtyValue(idx, .EntityData(idxRow)), EntityKoumoku_Number_DecimalType).PhysicsName))
                        End If
                    Else
                        DirectCast(.getPtyValue(idx, .EntityData(idxRow)), EntityKoumoku_MojiType).Value = getDBValueForString(dtCostBasic.Rows(idxRow).Item(DirectCast(.getPtyValue(idx, .EntityData(idxRow)), EntityKoumoku_MojiType).PhysicsName))
                    End If
                End With
            Next
        Next

        Return True
    End Function

    ''' <summary>
    ''' コース台帳原価(基本)の読み込みを行う
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function GetDataCrsLeadgerCostBasic(ByVal paramList As Hashtable) As DataTable
        Dim resultDataTable As New DataTable
        Dim sqlString As New StringBuilder
        Dim entBasic As New TCrsLedgerBasicEntityTehaiEx
        MyBase.paramClear()

        sqlString.AppendLine("SELECT ")
        sqlString.AppendLine("  BASIC.CRS_CD ")
        sqlString.AppendLine("  , BASIC.DELETE_DAY ")
        sqlString.AppendLine("  , BASIC.GOUSYA ")
        sqlString.AppendLine("  , BASIC.LAST_UPDATE_DAY ")
        sqlString.AppendLine("  , BASIC.LAST_UPDATE_PERSON_CD ")
        sqlString.AppendLine("  , BASIC.LAST_UPDATE_TIME ")
        sqlString.AppendLine("  , BASIC.REV_UMU_KBN ")
        sqlString.AppendLine("  , BASIC.SYUPT_DAY ")
        sqlString.AppendLine("  , BASIC.SISAN_NINZU ")
        sqlString.AppendLine("  , BASIC.SYSTEM_ENTRY_PGMID ")
        sqlString.AppendLine("  , BASIC.SYSTEM_ENTRY_PERSON_CD ")
        sqlString.AppendLine("  , BASIC.SYSTEM_ENTRY_DAY ")
        sqlString.AppendLine("  , BASIC.SYSTEM_UPDATE_PGMID ")
        sqlString.AppendLine("  , BASIC.SYSTEM_UPDATE_PERSON_CD ")
        sqlString.AppendLine("  , BASIC.SYSTEM_UPDATE_DAY ")
        sqlString.AppendLine("FROM ")
        sqlString.AppendLine("  T_CRS_LEDGER_COST_BASIC BASIC ")
        sqlString.AppendLine("WHERE")
        sqlString.AppendLine("   " & makeSqlEx(entBasic.crsCd, paramList, "BASIC"))
        sqlString.AppendLine("   AND")
        sqlString.AppendLine("   " & makeSqlEx(entBasic.syuptDay, paramList, "BASIC"))
        sqlString.AppendLine("   AND")
        sqlString.AppendLine("   " & makeSqlEx(entBasic.gousya, paramList, "BASIC"))
        sqlString.AppendLine("   AND")
        sqlString.AppendLine("   BASIC.DELETE_DAY = 0 ")

        Try
            resultDataTable = MyBase.getDataTable(sqlString.ToString)
        Catch ex As Exception
            Throw
        End Try

        Return resultDataTable
    End Function
#End Region

#Region "コース台帳（降車ヶ所）"
    ''' <summary>
    ''' コース台帳（降車ヶ所）を読み込みEntityクラスに格納する
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function getCrsLeadgerKoshakashoEntity(ByVal paramList As Hashtable, ByVal crsLeaderBasic As EntityOperation(Of TCrsLedgerBasicEntityTehaiEx)) As Boolean  'getコースマスタEntity(対象コース情報,コースマスタ)

        Dim dtKoshakasho As DataTable  'コース台帳（降車ヶ所）
        Try
            dtKoshakasho = GetDataCrsLeadgerKoshakasho(paramList)
        Catch ex As Exception
            Throw
        End Try

        For idxRow As Integer = 0 To dtKoshakasho.Rows.Count - 1

            If idxRow > 0 Then
                crsLeaderBasic.EntityData(0).KoushakashoEntity.add(New TCrsLedgerKoshakashoEntityTehaiEx)
            End If

            For idx As Integer = 0 To crsLeaderBasic.EntityData(0).KoushakashoEntity.getPropertyDataLength - 1
                With crsLeaderBasic.EntityData(0).KoushakashoEntity
                    If .getPtyType(idx, .EntityData(idxRow)) Is GetType(EntityKoumoku_YmdType) Then
                        If IsDBNull(dtKoshakasho.Rows(idxRow).Item(DirectCast(.getPtyValue(idx, .EntityData(idxRow)), EntityKoumoku_YmdType).PhysicsName)) = False Then
                            DirectCast(.getPtyValue(idx, .EntityData(idxRow)), EntityKoumoku_YmdType).Value = CDate(dtKoshakasho.Rows(idxRow).Item(DirectCast(.getPtyValue(idx, .EntityData(idxRow)), EntityKoumoku_YmdType).PhysicsName))
                        End If
                    ElseIf .getPtyType(idx, .EntityData(idxRow)) Is GetType(EntityKoumoku_NumberType) Then
                        If IsDBNull(dtKoshakasho.Rows(idxRow).Item(DirectCast(.getPtyValue(idx, .EntityData(idxRow)), EntityKoumoku_NumberType).PhysicsName)) = False Then
                            DirectCast(.getPtyValue(idx, .EntityData(idxRow)), EntityKoumoku_NumberType).Value = CInt(dtKoshakasho.Rows(idxRow).Item(DirectCast(.getPtyValue(idx, .EntityData(idxRow)), EntityKoumoku_NumberType).PhysicsName))
                        End If
                    ElseIf .getPtyType(idx, .EntityData(idxRow)) Is GetType(EntityKoumoku_Number_DecimalType) Then
                        If IsDBNull(dtKoshakasho.Rows(idxRow).Item(DirectCast(.getPtyValue(idx, .EntityData(idxRow)), EntityKoumoku_Number_DecimalType).PhysicsName)) = False Then
                            DirectCast(.getPtyValue(idx, .EntityData(idxRow)), EntityKoumoku_Number_DecimalType).Value = CDec(dtKoshakasho.Rows(idxRow).Item(DirectCast(.getPtyValue(idx, .EntityData(idxRow)), EntityKoumoku_Number_DecimalType).PhysicsName))
                        End If
                    Else
                        DirectCast(.getPtyValue(idx, .EntityData(idxRow)), EntityKoumoku_MojiType).Value = getDBValueForString(dtKoshakasho.Rows(idxRow).Item(DirectCast(.getPtyValue(idx, .EntityData(idxRow)), EntityKoumoku_MojiType).PhysicsName))
                    End If
                End With
            Next
        Next

        Return True
    End Function

    ''' <summary>
    ''' コース台帳（降車ヶ所）の読み込みを行う
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function GetDataCrsLeadgerKoshakasho(ByVal paramList As Hashtable) As DataTable
        Dim resultDataTable As New DataTable
        Dim sqlString As New StringBuilder
        Dim entBasic As New TCrsLedgerBasicEntityTehaiEx
        MyBase.paramClear()

        'sqlString.AppendLine("SELECT ")
        'sqlString.AppendLine("  KOUSHA.BIN_NAME ")
        'sqlString.AppendLine("  , KOUSHA.CRS_CD ")
        'sqlString.AppendLine("  , KOUSHA.DAILY ")
        'sqlString.AppendLine("  , KOUSHA.DELETE_DAY ")
        'sqlString.AppendLine("  , KOUSHA.GOUSYA ")
        'sqlString.AppendLine("  , KOUSHA.KOSHAKASHO_CD ")
        'sqlString.AppendLine("  , KOUSHA.KOSHAKASHO_EDABAN ")
        'sqlString.AppendLine("  , KOUSHA.SEISAN_TGT_CD ")
        'sqlString.AppendLine("  , KOUSHA.LINE_NO ")
        'sqlString.AppendLine("  , KOUSHA.RIYOU_DAY ")
        'sqlString.AppendLine("  , KOUSHA.SYUPT_DAY ")
        'sqlString.AppendLine("  , KOUSHA.SYUPT_PLACE_CARRIER ")
        'sqlString.AppendLine("  , KOUSHA.SYUPT_PLACE_CD_CARRIER ")
        'sqlString.AppendLine("  , KOUSHA.TTYAK_PLACE_CARRIER ")
        'sqlString.AppendLine("  , KOUSHA.TTYAK_PLACE_CD_CARRIER ")
        'sqlString.AppendLine("  , KOUSHA.SYSTEM_ENTRY_PGMID ")
        'sqlString.AppendLine("  , KOUSHA.SYSTEM_ENTRY_PERSON_CD ")
        'sqlString.AppendLine("  , KOUSHA.SYSTEM_ENTRY_DAY ")
        'sqlString.AppendLine("  , KOUSHA.SYSTEM_UPDATE_PGMID ")
        'sqlString.AppendLine("  , KOUSHA.SYSTEM_UPDATE_PERSON_CD ")
        'sqlString.AppendLine("  , KOUSHA.SYSTEM_UPDATE_DAY ")
        'sqlString.AppendLine("  , SIIRE.SIIRE_SAKI_NAME                         --降車ヶ所名 ")
        'sqlString.AppendLine("  , SIIRE.SIIRE_SAKI_KIND_CD                      --降車ヶ所種別 ")
        'sqlString.AppendLine("  , CODE_SK.CODE_NAME AS SIIRE_SAKI_KIND_NAME     --降車ヶ所種別名 ")
        'sqlString.AppendLine("  , CODE_S.CODE_NAME AS SEISAN_TGT_NAME           --精算目的名 ")
        'sqlString.AppendLine("FROM ")
        'sqlString.AppendLine("  T_CRS_LEDGER_KOSHAKASHO KOUSHA ")
        'sqlString.AppendLine("  LEFT JOIN M_SIIRE_SAKI SIIRE ")
        'sqlString.AppendLine("    ON KOUSHA.KOSHAKASHO_CD = SIIRE.SIIRE_SAKI_CD ")
        'sqlString.AppendLine("    AND KOUSHA.KOSHAKASHO_EDABAN = SIIRE.SIIRE_SAKI_NO ")
        'sqlString.AppendLine("    AND SIIRE.DELETE_DATE IS NULL ")
        'sqlString.AppendLine("  LEFT JOIN M_CODE CODE_SK ")
        'sqlString.AppendLine("    ON SIIRE.SIIRE_SAKI_KIND_CD = CODE_SK.CODE_VALUE ")
        'sqlString.AppendLine("    AND CODE_SK.CODE_BUNRUI = '" & CodeBunrui.siireKind & "' ")
        'sqlString.AppendLine("    AND CODE_SK.DELETE_DATE IS NULL ")
        'sqlString.AppendLine("  LEFT JOIN M_CODE CODE_S ")
        'sqlString.AppendLine("    ON KOUSHA.SEISAN_TGT_CD = CODE_S.CODE_VALUE ")
        'sqlString.AppendLine("    AND CODE_S.CODE_BUNRUI = '" & CodeBunrui.seisanMokuteki & "' ")
        'sqlString.AppendLine("    AND CODE_S.DELETE_DATE IS NULL ")
        'sqlString.AppendLine("WHERE ")
        'sqlString.AppendLine("   " & makeSqlEx(entBasic.crsCd, paramList, "KOUSHA"))
        'sqlString.AppendLine("   AND")
        'sqlString.AppendLine("   " & makeSqlEx(entBasic.syuptDay, paramList, "KOUSHA"))
        'sqlString.AppendLine("   AND")
        'sqlString.AppendLine("   " & makeSqlEx(entBasic.gousya, paramList, "KOUSHA"))
        'sqlString.AppendLine("   AND")
        'sqlString.AppendLine("   KOUSHA.DELETE_DAY = 0 ")
        'sqlString.AppendLine("ORDER BY ")
        'sqlString.AppendLine("  KOUSHA.DAILY ")
        'sqlString.AppendLine("  , KOUSHA.LINE_NO ")

        sqlString.AppendLine("SELECT ")
        sqlString.AppendLine("  KOUSHA.BIN_NAME ")
        sqlString.AppendLine("  , KOUSHA.CRS_CD ")
        sqlString.AppendLine("  , KOUSHA.DAILY ")
        sqlString.AppendLine("  , KOUSHA.DELETE_DAY ")
        sqlString.AppendLine("  , KOUSHA.GOUSYA ")
        sqlString.AppendLine("  , KOUSHA.KOSHAKASHO_CD ")
        sqlString.AppendLine("  , KOUSHA.KOSHAKASHO_EDABAN ")
        sqlString.AppendLine("  , KOUSHA.SEISAN_TGT_CD ")
        sqlString.AppendLine("  , KOUSHA.LINE_NO ")
        sqlString.AppendLine("  , KOUSHA.RIYOU_DAY ")
        sqlString.AppendLine("  , KOUSHA.SYUPT_DAY ")
        sqlString.AppendLine("  , KOUSHA.SYUPT_PLACE_CARRIER ")
        sqlString.AppendLine("  , KOUSHA.SYUPT_PLACE_CD_CARRIER ")
        sqlString.AppendLine("  , KOUSHA.TTYAK_PLACE_CARRIER ")
        sqlString.AppendLine("  , KOUSHA.TTYAK_PLACE_CD_CARRIER ")
        sqlString.AppendLine("  , KOUSHA.SYSTEM_ENTRY_PGMID ")
        sqlString.AppendLine("  , KOUSHA.SYSTEM_ENTRY_PERSON_CD ")
        sqlString.AppendLine("  , KOUSHA.SYSTEM_ENTRY_DAY ")
        sqlString.AppendLine("  , KOUSHA.SYSTEM_UPDATE_PGMID ")
        sqlString.AppendLine("  , KOUSHA.SYSTEM_UPDATE_PERSON_CD ")
        sqlString.AppendLine("  , KOUSHA.SYSTEM_UPDATE_DAY ")
        sqlString.AppendLine("  , SIIRE.SIIRE_SAKI_NAME                         --降車ヶ所名 ")
        sqlString.AppendLine("  , SIIRE.SIIRE_SAKI_KIND_CD                      --降車ヶ所種別 ")
        sqlString.AppendLine("  , CODE_SK.CODE_NAME AS SIIRE_SAKI_KIND_NAME     --降車ヶ所種別名 ")
        sqlString.AppendLine("  , CODE_S.CODE_NAME AS SEISAN_TGT_NAME           --精算目的名 ")
        sqlString.AppendLine("  , ROW_NUMBER() OVER ( ")
        sqlString.AppendLine("    PARTITION BY ")
        sqlString.AppendLine("      KOUSHA.CRS_CD ")
        sqlString.AppendLine("      , KOUSHA.SYUPT_DAY ")
        sqlString.AppendLine("      , KOUSHA.GOUSYA ")
        sqlString.AppendLine("    ORDER BY ")
        sqlString.AppendLine("      KOUSHA.DAILY ")
        sqlString.AppendLine("      , KOUSHA.KBN ")
        sqlString.AppendLine("      , KOUSHA.LINE_NO ")
        sqlString.AppendLine("  ) AS EDABAN ")
        sqlString.AppendLine("  ,'0' AS EDIT_FLG ")
        sqlString.AppendLine("FROM ")
        sqlString.AppendLine("  ( ")
        sqlString.AppendLine("    SELECT ")
        sqlString.AppendLine("      KOUSHA.BIN_NAME ")
        sqlString.AppendLine("      , KOUSHA.CRS_CD ")
        sqlString.AppendLine("      , KOUSHA.DAILY ")
        sqlString.AppendLine("      , KOUSHA.DELETE_DAY ")
        sqlString.AppendLine("      , KOUSHA.GOUSYA ")
        sqlString.AppendLine("      , KOUSHA.KOSHAKASHO_CD ")
        sqlString.AppendLine("      , KOUSHA.KOSHAKASHO_EDABAN ")
        sqlString.AppendLine("      , KOUSHA.SEISAN_TGT_CD ")
        sqlString.AppendLine("      , KOUSHA.LINE_NO ")
        sqlString.AppendLine("      , KOUSHA.RIYOU_DAY ")
        sqlString.AppendLine("      , KOUSHA.SYUPT_DAY ")
        sqlString.AppendLine("      , KOUSHA.SYUPT_PLACE_CARRIER ")
        sqlString.AppendLine("      , KOUSHA.SYUPT_PLACE_CD_CARRIER ")
        sqlString.AppendLine("      , KOUSHA.TTYAK_PLACE_CARRIER ")
        sqlString.AppendLine("      , KOUSHA.TTYAK_PLACE_CD_CARRIER ")
        sqlString.AppendLine("      , KOUSHA.SYSTEM_ENTRY_PGMID ")
        sqlString.AppendLine("      , KOUSHA.SYSTEM_ENTRY_PERSON_CD ")
        sqlString.AppendLine("      , KOUSHA.SYSTEM_ENTRY_DAY ")
        sqlString.AppendLine("      , KOUSHA.SYSTEM_UPDATE_PGMID ")
        sqlString.AppendLine("      , KOUSHA.SYSTEM_UPDATE_PERSON_CD ")
        sqlString.AppendLine("      , KOUSHA.SYSTEM_UPDATE_DAY ")
        sqlString.AppendLine("      , 1 AS KBN ")
        sqlString.AppendLine("    FROM ")
        sqlString.AppendLine("      T_CRS_LEDGER_KOSHAKASHO KOUSHA ")
        sqlString.AppendLine("    WHERE ")
        sqlString.AppendLine("       " & makeSqlEx(entBasic.crsCd, paramList, "KOUSHA"))
        sqlString.AppendLine("       AND")
        sqlString.AppendLine("       " & makeSqlEx(entBasic.syuptDay, paramList, "KOUSHA"))
        sqlString.AppendLine("       AND")
        sqlString.AppendLine("       " & makeSqlEx(entBasic.gousya, paramList, "KOUSHA"))
        sqlString.AppendLine("       AND")
        sqlString.AppendLine("       KOUSHA.DELETE_DAY = 0 ")
        sqlString.AppendLine("    UNION ALL ")
        sqlString.AppendLine("    SELECT ")
        sqlString.AppendLine("      NULL ")
        sqlString.AppendLine("      , HOTEL.CRS_CD ")
        sqlString.AppendLine("      , HOTEL.DAILY ")
        sqlString.AppendLine("      , HOTEL.DELETE_DAY ")
        sqlString.AppendLine("      , HOTEL.GOUSYA ")
        sqlString.AppendLine("      , HOTEL.SIIRE_SAKI_CD ")
        sqlString.AppendLine("      , HOTEL.SIIRE_SAKI_EDABAN ")
        sqlString.AppendLine("      , SIIRE.SEISAN_TGT_CD ")
        sqlString.AppendLine("      , HOTEL.LINE_NO ")
        sqlString.AppendLine("      , HOTEL.RIYOU_DAY ")
        sqlString.AppendLine("      , HOTEL.SYUPT_DAY ")
        sqlString.AppendLine("      , NULL ")
        sqlString.AppendLine("      , NULL ")
        sqlString.AppendLine("      , NULL ")
        sqlString.AppendLine("      , NULL ")
        sqlString.AppendLine("      , HOTEL.SYSTEM_ENTRY_PGMID ")
        sqlString.AppendLine("      , HOTEL.SYSTEM_ENTRY_PERSON_CD ")
        sqlString.AppendLine("      , HOTEL.SYSTEM_ENTRY_DAY ")
        sqlString.AppendLine("      , HOTEL.SYSTEM_UPDATE_PGMID ")
        sqlString.AppendLine("      , HOTEL.SYSTEM_UPDATE_PERSON_CD ")
        sqlString.AppendLine("      , HOTEL.SYSTEM_UPDATE_DAY ")
        sqlString.AppendLine("      , 2 AS KBN ")
        sqlString.AppendLine("    FROM ")
        sqlString.AppendLine("      T_CRS_LEDGER_HOTEL HOTEL ")
        sqlString.AppendLine("      LEFT JOIN M_SIIRE_SAKI SIIRE ")
        sqlString.AppendLine("        ON HOTEL.SIIRE_SAKI_CD = SIIRE.SIIRE_SAKI_CD ")
        sqlString.AppendLine("        AND HOTEL.SIIRE_SAKI_EDABAN = SIIRE.SIIRE_SAKI_NO ")
        sqlString.AppendLine("    WHERE ")
        sqlString.AppendLine("       " & makeSqlEx(entBasic.crsCd, paramList, "HOTEL"))
        sqlString.AppendLine("       AND")
        sqlString.AppendLine("       " & makeSqlEx(entBasic.syuptDay, paramList, "HOTEL"))
        sqlString.AppendLine("       AND")
        sqlString.AppendLine("       " & makeSqlEx(entBasic.gousya, paramList, "HOTEL"))
        sqlString.AppendLine("       AND")
        sqlString.AppendLine("       HOTEL.DELETE_DAY = 0 ")
        sqlString.AppendLine("  ) KOUSHA ")
        sqlString.AppendLine("  LEFT JOIN M_SIIRE_SAKI SIIRE ")
        sqlString.AppendLine("    ON KOUSHA.KOSHAKASHO_CD = SIIRE.SIIRE_SAKI_CD ")
        sqlString.AppendLine("    AND KOUSHA.KOSHAKASHO_EDABAN = SIIRE.SIIRE_SAKI_NO ")
        sqlString.AppendLine("    AND SIIRE.DELETE_DATE IS NULL ")
        sqlString.AppendLine("  LEFT JOIN M_CODE CODE_SK ")
        sqlString.AppendLine("    ON SIIRE.SIIRE_SAKI_KIND_CD = CODE_SK.CODE_VALUE ")
        sqlString.AppendLine("    AND CODE_SK.CODE_BUNRUI = '" & CodeBunrui.siireKind & "' ")
        sqlString.AppendLine("    AND CODE_SK.DELETE_DATE IS NULL ")
        sqlString.AppendLine("  LEFT JOIN M_CODE CODE_S ")
        sqlString.AppendLine("    ON KOUSHA.SEISAN_TGT_CD = CODE_S.CODE_VALUE ")
        sqlString.AppendLine("    AND CODE_S.CODE_BUNRUI = '" & CodeBunrui.seisanMokuteki & "' ")
        sqlString.AppendLine("    AND CODE_S.DELETE_DATE IS NULL ")
        sqlString.AppendLine("ORDER BY ")
        sqlString.AppendLine("  KOUSHA.DAILY ")
        sqlString.AppendLine("  , KOUSHA.KBN ")
        sqlString.AppendLine("  , KOUSHA.LINE_NO ")

        Try
            resultDataTable = MyBase.getDataTable(sqlString.ToString)
        Catch ex As Exception
            Throw
        End Try

        Return resultDataTable
    End Function
#End Region

#Region "コース台帳原価（降車ヶ所）"
    ''' <summary>
    ''' コース台帳原価（降車ヶ所）を読み込みEntityクラスに格納する
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function getCrsLeadgerCostKoshakashoEntity(ByVal paramList As Hashtable, ByVal crsLeaderBasic As EntityOperation(Of TCrsLedgerBasicEntityTehaiEx)) As Boolean  'getコースマスタEntity(対象コース情報,コースマスタ)

        Dim dtCostKoshakasho As DataTable   'コース台帳原価（降車ヶ所）
        Dim itinarylineNo As Integer        '降車ヶ所行No
        Dim koushakashoCd As String        '降車ヶ所コード
        Dim koushakashoEda As String       '降車ヶ所枝番
        Dim idxKousha As Integer = 0

        Try
            dtCostKoshakasho = GetDataCrsLeadgerCostKoshakasho(paramList)
        Catch ex As Exception
            Throw
        End Try

        For idxRow As Integer = 0 To dtCostKoshakasho.Rows.Count - 1

            itinarylineNo = CInt(dtCostKoshakasho.Rows(idxRow).Item("CRS_ITINERARY_LINE_NO"))
            koushakashoCd = dtCostKoshakasho.Rows(idxRow).Item("SIIRE_SAKI_CD").ToString
            koushakashoEda = dtCostKoshakasho.Rows(idxRow).Item("SIIRE_SAKI_EDABAN").ToString

            idxKousha = getKoushaKashoEntityIdx(itinarylineNo, koushakashoCd, koushakashoEda, crsLeaderBasic)

            For idx As Integer = 0 To crsLeaderBasic.EntityData(0).KoushakashoEntity(idxKousha).CostKoshakashoEntity.getPropertyDataLength - 1
                With crsLeaderBasic.EntityData(0).KoushakashoEntity(idxKousha).CostKoshakashoEntity
                    If .getPtyType(idx, .EntityData(0)) Is GetType(EntityKoumoku_YmdType) Then
                        If IsDBNull(dtCostKoshakasho.Rows(idxRow).Item(DirectCast(.getPtyValue(idx, .EntityData(0)), EntityKoumoku_YmdType).PhysicsName)) = False Then
                            DirectCast(.getPtyValue(idx, .EntityData(0)), EntityKoumoku_YmdType).Value = CDate(dtCostKoshakasho.Rows(idxRow).Item(DirectCast(.getPtyValue(idx, .EntityData(0)), EntityKoumoku_YmdType).PhysicsName))
                        End If
                    ElseIf .getPtyType(idx, .EntityData(0)) Is GetType(EntityKoumoku_NumberType) Then
                        If IsDBNull(dtCostKoshakasho.Rows(idxRow).Item(DirectCast(.getPtyValue(idx, .EntityData(0)), EntityKoumoku_NumberType).PhysicsName)) = False Then
                            DirectCast(.getPtyValue(idx, .EntityData(0)), EntityKoumoku_NumberType).Value = CInt(dtCostKoshakasho.Rows(idxRow).Item(DirectCast(.getPtyValue(idx, .EntityData(0)), EntityKoumoku_NumberType).PhysicsName))
                        End If
                    ElseIf .getPtyType(idx, .EntityData(0)) Is GetType(EntityKoumoku_Number_DecimalType) Then
                        If IsDBNull(dtCostKoshakasho.Rows(idxRow).Item(DirectCast(.getPtyValue(idx, .EntityData(0)), EntityKoumoku_Number_DecimalType).PhysicsName)) = False Then
                            DirectCast(.getPtyValue(idx, .EntityData(0)), EntityKoumoku_Number_DecimalType).Value = CDec(dtCostKoshakasho.Rows(idxRow).Item(DirectCast(.getPtyValue(idx, .EntityData(0)), EntityKoumoku_Number_DecimalType).PhysicsName))
                        End If
                    Else
                        DirectCast(.getPtyValue(idx, .EntityData(0)), EntityKoumoku_MojiType).Value = getDBValueForString(dtCostKoshakasho.Rows(idxRow).Item(DirectCast(.getPtyValue(idx, .EntityData(0)), EntityKoumoku_MojiType).PhysicsName))
                    End If
                End With
            Next
        Next

        Return True
    End Function

    Private Function getKoushaKashoEntityIdx(ByVal itineraryLineNo As Integer, ByVal siireCd As String, ByVal siireEda As String, ByVal crsLeaderBasic As EntityOperation(Of TCrsLedgerBasicEntityTehaiEx)) As Integer
        Dim idxKousha As Integer = 0
        For Each ent As TCrsLedgerKoshakashoEntityTehaiEx In crsLeaderBasic.EntityData(0).KoushakashoEntity.EntityData
            If itineraryLineNo.Equals(ent.lineNo.Value) AndAlso siireCd.Equals(ent.koshakashoCd.Value) AndAlso siireEda.Equals(ent.koshakashoEdaban.Value) Then
                Return idxKousha
            End If
            idxKousha += 1
        Next
        Return 0

    End Function

    ''' <summary>
    ''' コース台帳原価（降車ヶ所）の読み込みを行う
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function GetDataCrsLeadgerCostKoshakasho(ByVal paramList As Hashtable) As DataTable
        Dim resultDataTable As New DataTable
        Dim sqlString As New StringBuilder
        Dim entBasic As New TCrsLedgerBasicEntityTehaiEx
        MyBase.paramClear()

        sqlString.AppendLine("SELECT ")
        sqlString.AppendLine("  COST.COM ")
        sqlString.AppendLine("  , COST.CRS_CD ")
        sqlString.AppendLine("  , COST.CRS_ITINERARY_LINE_NO ")
        sqlString.AppendLine("  , COST.DELETE_DAY ")
        sqlString.AppendLine("  , COST.GOUSYA ")
        sqlString.AppendLine("  , COST.LINE_NO ")
        sqlString.AppendLine("  , COST.RIYOU_DAY ")
        sqlString.AppendLine("  , COST.SEISAN_HOHO ")
        sqlString.AppendLine("  , COST.SIIRE_SAKI_CD ")
        sqlString.AppendLine("  , COST.SIIRE_SAKI_EDABAN ")
        sqlString.AppendLine("  , COST.SOKYAK_FEE_CALC_HOHO_KBN ")
        sqlString.AppendLine("  , COST.SOKYAK_FEE_GENKIN_PAYMENT_KBN ")
        sqlString.AppendLine("  , COST.SOKYAK_FEE_TANKA_OR_PER ")
        sqlString.AppendLine("  , COST.SYUPT_DAY ")
        sqlString.AppendLine("  , COST.TAX_KBN ")
        sqlString.AppendLine("  , COST.TEIKYUBI_KBN ")
        sqlString.AppendLine("  , COST.BUS_TANI ")
        sqlString.AppendLine("  , COST.SYSTEM_ENTRY_PGMID ")
        sqlString.AppendLine("  , COST.SYSTEM_ENTRY_PERSON_CD ")
        sqlString.AppendLine("  , COST.SYSTEM_ENTRY_DAY ")
        sqlString.AppendLine("  , COST.SYSTEM_UPDATE_PGMID ")
        sqlString.AppendLine("  , COST.SYSTEM_UPDATE_PERSON_CD ")
        sqlString.AppendLine("  , COST.SYSTEM_UPDATE_DAY ")
        sqlString.AppendLine("  , '0' AS MAKE_FLG ")
        sqlString.AppendLine("FROM ")
        sqlString.AppendLine("  T_CRS_LEDGER_COST_KOSHAKASHO COST ")
        sqlString.AppendLine("WHERE")
        sqlString.AppendLine("   " & makeSqlEx(entBasic.crsCd, paramList, "COST"))
        sqlString.AppendLine("   AND")
        sqlString.AppendLine("   " & makeSqlEx(entBasic.syuptDay, paramList, "COST"))
        sqlString.AppendLine("   AND")
        sqlString.AppendLine("   " & makeSqlEx(entBasic.gousya, paramList, "COST"))
        sqlString.AppendLine("   AND")
        sqlString.AppendLine("   COST.DELETE_DAY = 0 ")
        sqlString.AppendLine("ORDER BY ")
        sqlString.AppendLine("   COST.LINE_NO ")

        Try
            resultDataTable = MyBase.getDataTable(sqlString.ToString)
        Catch ex As Exception
            Throw
        End Try

        Return resultDataTable
    End Function
#End Region

#Region "コース台帳原価（降車ヶ所_料金区分）"
    ''' <summary>
    ''' コース台帳原価（降車ヶ所_料金区分）を読み込みEntityクラスに格納する
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function getCrsLeadgerCostKoshakashoChargeKbnEntity(ByVal paramList As Hashtable, ByVal crsLeaderBasic As EntityOperation(Of TCrsLedgerBasicEntityTehaiEx)) As Boolean  'getコースマスタEntity(対象コース情報,コースマスタ)

        Dim dtCostKoshakashoChgKbn As DataTable  'コース台帳原価（降車ヶ所_料金区分）
        Dim itinarylineNo As Integer = -1 '行No
        Dim koushakashoCd As String        '降車ヶ所コード
        Dim koushakashoEda As String       '降車ヶ所枝番
        Dim idxKousha As Integer = 0
        Dim key As String = String.Empty
        Dim beflineKey As String = String.Empty  '前No
        Dim chgKbnRowIdx As Integer = 0  'idx降車ヶ所料金区分

        Try
            dtCostKoshakashoChgKbn = GetDataCrsLeadgerCostKoshakashoChargeKbn(paramList)
        Catch ex As Exception
            Throw
        End Try

        For idxRow As Integer = 0 To dtCostKoshakashoChgKbn.Rows.Count - 1
            beflineKey = key
            itinarylineNo = CInt(dtCostKoshakashoChgKbn.Rows(idxRow).Item("CRS_ITINERARY_LINE_NO"))
            koushakashoCd = dtCostKoshakashoChgKbn.Rows(idxRow).Item("SIIRE_SAKI_CD").ToString
            koushakashoEda = dtCostKoshakashoChgKbn.Rows(idxRow).Item("SIIRE_SAKI_EDABAN").ToString

            key = String.Concat(itinarylineNo.ToString, "_", koushakashoCd, "_", koushakashoEda)

            If key <> beflineKey Then
                idxKousha = getKoushaKashoEntityIdx(itinarylineNo, koushakashoCd, koushakashoEda, crsLeaderBasic)
                'rowCountを初期化
                chgKbnRowIdx = 0
            Else
                crsLeaderBasic.EntityData(0).KoushakashoEntity(idxKousha).CostKoshakashoEntity(0).CostKoshakashoChargeKbnEntity.add(New TCrsLedgerCostKoshakashoChargeKbnEntity)
                chgKbnRowIdx += 1
            End If

            For idx As Integer = 0 To crsLeaderBasic.EntityData(0).KoushakashoEntity(idxKousha).CostKoshakashoEntity(0).CostKoshakashoChargeKbnEntity.getPropertyDataLength - 1
                With crsLeaderBasic.EntityData(0).KoushakashoEntity(idxKousha).CostKoshakashoEntity(0).CostKoshakashoChargeKbnEntity
                    If .getPtyType(idx, .EntityData(chgKbnRowIdx)) Is GetType(EntityKoumoku_YmdType) Then
                        If IsDBNull(dtCostKoshakashoChgKbn.Rows(idxRow).Item(DirectCast(.getPtyValue(idx, .EntityData(chgKbnRowIdx)), EntityKoumoku_YmdType).PhysicsName)) = False Then
                            DirectCast(.getPtyValue(idx, .EntityData(chgKbnRowIdx)), EntityKoumoku_YmdType).Value = CDate(dtCostKoshakashoChgKbn.Rows(idxRow).Item(DirectCast(.getPtyValue(idx, .EntityData(chgKbnRowIdx)), EntityKoumoku_YmdType).PhysicsName))
                        End If
                    ElseIf .getPtyType(idx, .EntityData(chgKbnRowIdx)) Is GetType(EntityKoumoku_NumberType) Then
                        If IsDBNull(dtCostKoshakashoChgKbn.Rows(idxRow).Item(DirectCast(.getPtyValue(idx, .EntityData(chgKbnRowIdx)), EntityKoumoku_NumberType).PhysicsName)) = False Then
                            DirectCast(.getPtyValue(idx, .EntityData(chgKbnRowIdx)), EntityKoumoku_NumberType).Value = CInt(dtCostKoshakashoChgKbn.Rows(idxRow).Item(DirectCast(.getPtyValue(idx, .EntityData(chgKbnRowIdx)), EntityKoumoku_NumberType).PhysicsName))
                        End If
                    ElseIf .getPtyType(idx, .EntityData(chgKbnRowIdx)) Is GetType(EntityKoumoku_Number_DecimalType) Then
                        If IsDBNull(dtCostKoshakashoChgKbn.Rows(idxRow).Item(DirectCast(.getPtyValue(idx, .EntityData(chgKbnRowIdx)), EntityKoumoku_Number_DecimalType).PhysicsName)) = False Then
                            DirectCast(.getPtyValue(idx, .EntityData(chgKbnRowIdx)), EntityKoumoku_Number_DecimalType).Value = CDec(dtCostKoshakashoChgKbn.Rows(idxRow).Item(DirectCast(.getPtyValue(idx, .EntityData(chgKbnRowIdx)), EntityKoumoku_Number_DecimalType).PhysicsName))
                        End If
                    Else
                        DirectCast(.getPtyValue(idx, .EntityData(chgKbnRowIdx)), EntityKoumoku_MojiType).Value = getDBValueForString(dtCostKoshakashoChgKbn.Rows(idxRow).Item(DirectCast(.getPtyValue(idx, .EntityData(chgKbnRowIdx)), EntityKoumoku_MojiType).PhysicsName))
                    End If
                End With
            Next

        Next

        Return True
    End Function

    ''' <summary>
    ''' コース台帳原価（降車ヶ所_料金区分）の読み込みを行う
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function GetDataCrsLeadgerCostKoshakashoChargeKbn(ByVal paramList As Hashtable) As DataTable
        Dim resultDataTable As New DataTable
        Dim sqlString As New StringBuilder
        Dim entBasic As New TCrsLedgerBasicEntityTehaiEx
        MyBase.paramClear()

        sqlString.AppendLine("SELECT ")
        sqlString.AppendLine("  CHARGE_KBN.SYUPT_DAY ")
        sqlString.AppendLine("  , CHARGE_KBN.CRS_CD ")
        sqlString.AppendLine("  , CHARGE_KBN.GOUSYA ")
        sqlString.AppendLine("  , CHARGE_KBN.CRS_ITINERARY_LINE_NO ")
        sqlString.AppendLine("  , CHARGE_KBN.LINE_NO ")
        sqlString.AppendLine("  , CHARGE_KBN.SIIRE_SAKI_CD ")
        sqlString.AppendLine("  , CHARGE_KBN.SIIRE_SAKI_EDABAN ")
        sqlString.AppendLine("  , CHARGE_KBN.HEIJITU_TOKUTEI_DAY_KBN ")
        sqlString.AppendLine("  , CHARGE_KBN.CHARGE_KBN_JININ_CD ")
        sqlString.AppendLine("  , CHARGE_KBN.BATH_TAX ")
        sqlString.AppendLine("  , CHARGE_KBN.SIHARAI_GAKU ")
        sqlString.AppendLine("  , CHARGE_KBN.KOUSEI_GAKU ")
        sqlString.AppendLine("  , CHARGE_KBN.CHARGE_1 ")
        sqlString.AppendLine("  , CHARGE_KBN.CHARGE_2 ")
        sqlString.AppendLine("  , CHARGE_KBN.CHARGE_3 ")
        sqlString.AppendLine("  , CHARGE_KBN.CHARGE_4 ")
        sqlString.AppendLine("  , CHARGE_KBN.CHARGE_5 ")
        sqlString.AppendLine("  , CHARGE_KBN.SYSTEM_ENTRY_PGMID ")
        sqlString.AppendLine("  , CHARGE_KBN.SYSTEM_ENTRY_PERSON_CD ")
        sqlString.AppendLine("  , CHARGE_KBN.SYSTEM_ENTRY_DAY ")
        sqlString.AppendLine("  , CHARGE_KBN.SYSTEM_UPDATE_PGMID ")
        sqlString.AppendLine("  , CHARGE_KBN.SYSTEM_UPDATE_PERSON_CD ")
        sqlString.AppendLine("  , CHARGE_KBN.SYSTEM_UPDATE_DAY ")
        sqlString.AppendLine("  , CHARGE_KBN.DELETE_DATE ")
        sqlString.AppendLine("FROM ")
        sqlString.AppendLine("  T_CRS_LEDGER_COST_KOSHAKASHO_CHARGE_KBN CHARGE_KBN ")
        sqlString.AppendLine("  LEFT JOIN T_CRS_LEDGER_BASIC_CHARGE_KBN BASIC ")
        sqlString.AppendLine("    ON CHARGE_KBN.CRS_CD = BASIC.CRS_CD ")
        sqlString.AppendLine("    AND  CHARGE_KBN.SYUPT_DAY = BASIC.SYUPT_DAY ")
        sqlString.AppendLine("    AND  CHARGE_KBN.GOUSYA = BASIC.GOUSYA ")
        sqlString.AppendLine("    AND  CHARGE_KBN.CHARGE_KBN_JININ_CD = BASIC.CHARGE_KBN_JININ_CD ")
        sqlString.AppendLine("WHERE ")
        sqlString.AppendLine("   " & makeSqlEx(entBasic.crsCd, paramList, "CHARGE_KBN"))
        sqlString.AppendLine("   AND")
        sqlString.AppendLine("   " & makeSqlEx(entBasic.syuptDay, paramList, "CHARGE_KBN"))
        sqlString.AppendLine("   AND")
        sqlString.AppendLine("   " & makeSqlEx(entBasic.gousya, paramList, "CHARGE_KBN"))
        sqlString.AppendLine("   AND")
        sqlString.AppendLine("   CHARGE_KBN.DELETE_DATE = 0 ")
        sqlString.AppendLine("ORDER BY ")
        sqlString.AppendLine("    CHARGE_KBN.CRS_ITINERARY_LINE_NO ")
        sqlString.AppendLine("  , CHARGE_KBN.SIIRE_SAKI_CD ")
        sqlString.AppendLine("  , CHARGE_KBN.SIIRE_SAKI_EDABAN ")
        sqlString.AppendLine("  , BASIC.LINE_NO ")

        Try
            resultDataTable = MyBase.getDataTable(sqlString.ToString)
        Catch ex As Exception
            Throw
        End Try

        Return resultDataTable
    End Function
#End Region

#Region "コース台帳原価（キャリア）"
    ''' <summary>
    ''' コース台帳原価（キャリア）を読み込みEntityクラスに格納する
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function getCrsLeadgerCostCarrierEntity(ByVal paramList As Hashtable, ByVal crsLeaderBasic As EntityOperation(Of TCrsLedgerBasicEntityTehaiEx)) As Boolean  'getコースマスタEntity(対象コース情報,コースマスタ)

        Dim dtCostCarrier As DataTable  'コース台帳原価（キャリア）
        Dim itinarylineNo As Integer  '行No
        Dim koushakashoCd As String        '降車ヶ所コード
        Dim koushakashoEda As String       '降車ヶ所枝番
        Dim idxKousha As Integer = 0

        Try
            dtCostCarrier = GetDataCrsLeadgerCostCarrier(paramList)
        Catch ex As Exception
            Throw
        End Try

        For idxRow As Integer = 0 To dtCostCarrier.Rows.Count - 1
            itinarylineNo = CInt(dtCostCarrier.Rows(idxRow).Item("CRS_ITINERARY_LINE_NO"))
            koushakashoCd = dtCostCarrier.Rows(idxRow).Item("CARRIER_CD").ToString
            koushakashoEda = dtCostCarrier.Rows(idxRow).Item("CARRIER_EDABAN").ToString

            idxKousha = getKoushaKashoEntityIdx(itinarylineNo, koushakashoCd, koushakashoEda, crsLeaderBasic)

            For idx As Integer = 0 To crsLeaderBasic.EntityData(0).KoushakashoEntity(idxKousha).CostCarrierEntity.getPropertyDataLength - 1
                With crsLeaderBasic.EntityData(0).KoushakashoEntity(idxKousha).CostCarrierEntity
                    If .getPtyType(idx, .EntityData(0)) Is GetType(EntityKoumoku_YmdType) Then
                        If IsDBNull(dtCostCarrier.Rows(idxRow).Item(DirectCast(.getPtyValue(idx, .EntityData(0)), EntityKoumoku_YmdType).PhysicsName)) = False Then
                            DirectCast(.getPtyValue(idx, .EntityData(0)), EntityKoumoku_YmdType).Value = CDate(dtCostCarrier.Rows(idxRow).Item(DirectCast(.getPtyValue(idx, .EntityData(0)), EntityKoumoku_YmdType).PhysicsName))
                        End If
                    ElseIf .getPtyType(idx, .EntityData(0)) Is GetType(EntityKoumoku_NumberType) Then
                        If IsDBNull(dtCostCarrier.Rows(idxRow).Item(DirectCast(.getPtyValue(idx, .EntityData(0)), EntityKoumoku_NumberType).PhysicsName)) = False Then
                            DirectCast(.getPtyValue(idx, .EntityData(0)), EntityKoumoku_NumberType).Value = CInt(dtCostCarrier.Rows(idxRow).Item(DirectCast(.getPtyValue(idx, .EntityData(0)), EntityKoumoku_NumberType).PhysicsName))
                        End If
                    ElseIf .getPtyType(idx, .EntityData(0)) Is GetType(EntityKoumoku_Number_DecimalType) Then
                        If IsDBNull(dtCostCarrier.Rows(idxRow).Item(DirectCast(.getPtyValue(idx, .EntityData(0)), EntityKoumoku_Number_DecimalType).PhysicsName)) = False Then
                            DirectCast(.getPtyValue(idx, .EntityData(0)), EntityKoumoku_Number_DecimalType).Value = CDec(dtCostCarrier.Rows(idxRow).Item(DirectCast(.getPtyValue(idx, .EntityData(0)), EntityKoumoku_Number_DecimalType).PhysicsName))
                        End If
                    Else
                        DirectCast(.getPtyValue(idx, .EntityData(0)), EntityKoumoku_MojiType).Value = getDBValueForString(dtCostCarrier.Rows(idxRow).Item(DirectCast(.getPtyValue(idx, .EntityData(0)), EntityKoumoku_MojiType).PhysicsName))
                    End If
                End With
            Next
        Next

        Return True
    End Function

    ''' <summary>
    ''' コース台帳原価（キャリア）の読み込みを行う
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function GetDataCrsLeadgerCostCarrier(ByVal paramList As Hashtable) As DataTable
        Dim resultDataTable As New DataTable
        Dim sqlString As New StringBuilder
        Dim entBasic As New TCrsLedgerBasicEntityTehaiEx
        MyBase.paramClear()

        sqlString.AppendLine("SELECT ")
        sqlString.AppendLine("  COST.CARRIER_CD ")
        sqlString.AppendLine("  , COST.CARRIER_EDABAN ")
        sqlString.AppendLine("  , COST.CLASS_LINE_NO ")
        sqlString.AppendLine("  , COST.CLASS_WORDING ")
        sqlString.AppendLine("  , COST.COM ")
        sqlString.AppendLine("  , COST.CRS_CD ")
        sqlString.AppendLine("  , COST.CRS_ITINERARY_LINE_NO ")
        sqlString.AppendLine("  , COST.DELETE_DAY ")
        sqlString.AppendLine("  , COST.GOUSYA ")
        sqlString.AppendLine("  , COST.LINE_NO ")
        sqlString.AppendLine("  , COST.RIYOU_DAY ")
        sqlString.AppendLine("  , COST.SEISAN_HOHO ")
        sqlString.AppendLine("  , COST.SYUPT_DAY ")
        sqlString.AppendLine("  , COST.TAX_KBN ")
        sqlString.AppendLine("  , COST.SYSTEM_ENTRY_PGMID ")
        sqlString.AppendLine("  , COST.SYSTEM_ENTRY_PERSON_CD ")
        sqlString.AppendLine("  , COST.SYSTEM_ENTRY_DAY ")
        sqlString.AppendLine("  , COST.SYSTEM_UPDATE_PGMID ")
        sqlString.AppendLine("  , COST.SYSTEM_UPDATE_PERSON_CD ")
        sqlString.AppendLine("  , COST.SYSTEM_UPDATE_DAY ")
        sqlString.AppendLine("  , '0' AS MAKE_FLG ")
        sqlString.AppendLine("FROM ")
        sqlString.AppendLine("  T_CRS_LEDGER_COST_CARRIER COST ")
        sqlString.AppendLine("WHERE")
        sqlString.AppendLine("   " & makeSqlEx(entBasic.crsCd, paramList, "COST"))
        sqlString.AppendLine("   AND")
        sqlString.AppendLine("   " & makeSqlEx(entBasic.syuptDay, paramList, "COST"))
        sqlString.AppendLine("   AND")
        sqlString.AppendLine("   " & makeSqlEx(entBasic.gousya, paramList, "COST"))
        sqlString.AppendLine("   AND")
        sqlString.AppendLine("   COST.DELETE_DAY = 0 ")
        sqlString.AppendLine("   AND")
        sqlString.AppendLine("   CRS_ITINERARY_LINE_NO > 0 ")
        sqlString.AppendLine("ORDER BY ")
        sqlString.AppendLine("  COST.LINE_NO ")

        Try
            resultDataTable = MyBase.getDataTable(sqlString.ToString)
        Catch ex As Exception
            Throw
        End Try

        Return resultDataTable
    End Function
#End Region

#Region "コース台帳原価（キャリア_料金区分）"
    ''' <summary>
    ''' コース台帳原価（キャリア_料金区分）を読み込みEntityクラスに格納する
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function getCrsLeadgerCostCarrierChargeKbnEntity(ByVal paramList As Hashtable, ByVal crsLeaderBasic As EntityOperation(Of TCrsLedgerBasicEntityTehaiEx)) As Boolean  'getコースマスタEntity(対象コース情報,コースマスタ)

        Dim dtCostCarrierChgKbn As DataTable  'コース台帳原価（キャリア_料金区分）
        Dim itinarylineNo As Integer = -1 '行No
        Dim koushakashoCd As String        '降車ヶ所コード
        Dim koushakashoEda As String       '降車ヶ所枝番
        Dim idxKousha As Integer = 0
        Dim key As String = String.Empty
        Dim beflineKey As String = String.Empty  '前No
        Dim chgKbnRowIdx As Integer = 0  'idxキャリア料金区分

        Try
            dtCostCarrierChgKbn = GetDataCrsLeadgerCostCarrierChargeKbn(paramList)
        Catch ex As Exception
            Throw
        End Try

        For idxRow As Integer = 0 To dtCostCarrierChgKbn.Rows.Count - 1
            beflineKey = key
            itinarylineNo = CInt(dtCostCarrierChgKbn.Rows(idxRow).Item("CRS_ITINERARY_LINE_NO"))
            koushakashoCd = dtCostCarrierChgKbn.Rows(idxRow).Item("CARRIER_CD").ToString
            koushakashoEda = dtCostCarrierChgKbn.Rows(idxRow).Item("CARRIER_EDABAN").ToString

            key = String.Concat(itinarylineNo.ToString, "_", koushakashoCd, "_", koushakashoEda)

            If key <> beflineKey Then
                idxKousha = getKoushaKashoEntityIdx(itinarylineNo, koushakashoCd, koushakashoEda, crsLeaderBasic)
                'rowCountを初期化
                chgKbnRowIdx = 0
            Else
                crsLeaderBasic.EntityData(0).KoushakashoEntity(idxKousha).CostCarrierEntity(0).CostCarrierChargeKbnEntity.add(New TCrsLedgerCostCarrierChargeKbnEntity)
                chgKbnRowIdx += 1
            End If

            For idx As Integer = 0 To crsLeaderBasic.EntityData(0).KoushakashoEntity(idxKousha).CostCarrierEntity(0).CostCarrierChargeKbnEntity.getPropertyDataLength - 1
                With crsLeaderBasic.EntityData(0).KoushakashoEntity(idxKousha).CostCarrierEntity(0).CostCarrierChargeKbnEntity
                    If .getPtyType(idx, .EntityData(chgKbnRowIdx)) Is GetType(EntityKoumoku_YmdType) Then
                        If IsDBNull(dtCostCarrierChgKbn.Rows(idxRow).Item(DirectCast(.getPtyValue(idx, .EntityData(chgKbnRowIdx)), EntityKoumoku_YmdType).PhysicsName)) = False Then
                            DirectCast(.getPtyValue(idx, .EntityData(chgKbnRowIdx)), EntityKoumoku_YmdType).Value = CDate(dtCostCarrierChgKbn.Rows(idxRow).Item(DirectCast(.getPtyValue(idx, .EntityData(chgKbnRowIdx)), EntityKoumoku_YmdType).PhysicsName))
                        End If
                    ElseIf .getPtyType(idx, .EntityData(chgKbnRowIdx)) Is GetType(EntityKoumoku_NumberType) Then
                        If IsDBNull(dtCostCarrierChgKbn.Rows(idxRow).Item(DirectCast(.getPtyValue(idx, .EntityData(chgKbnRowIdx)), EntityKoumoku_NumberType).PhysicsName)) = False Then
                            DirectCast(.getPtyValue(idx, .EntityData(chgKbnRowIdx)), EntityKoumoku_NumberType).Value = CInt(dtCostCarrierChgKbn.Rows(idxRow).Item(DirectCast(.getPtyValue(idx, .EntityData(chgKbnRowIdx)), EntityKoumoku_NumberType).PhysicsName))
                        End If
                    ElseIf .getPtyType(idx, .EntityData(chgKbnRowIdx)) Is GetType(EntityKoumoku_Number_DecimalType) Then
                        If IsDBNull(dtCostCarrierChgKbn.Rows(idxRow).Item(DirectCast(.getPtyValue(idx, .EntityData(chgKbnRowIdx)), EntityKoumoku_Number_DecimalType).PhysicsName)) = False Then
                            DirectCast(.getPtyValue(idx, .EntityData(chgKbnRowIdx)), EntityKoumoku_Number_DecimalType).Value = CDec(dtCostCarrierChgKbn.Rows(idxRow).Item(DirectCast(.getPtyValue(idx, .EntityData(chgKbnRowIdx)), EntityKoumoku_Number_DecimalType).PhysicsName))
                        End If
                    Else
                        DirectCast(.getPtyValue(idx, .EntityData(chgKbnRowIdx)), EntityKoumoku_MojiType).Value = getDBValueForString(dtCostCarrierChgKbn.Rows(idxRow).Item(DirectCast(.getPtyValue(idx, .EntityData(chgKbnRowIdx)), EntityKoumoku_MojiType).PhysicsName))
                    End If
                End With
            Next

        Next

        Return True
    End Function

    ''' <summary>
    ''' コース台帳原価（キャリア_料金区分）の読み込みを行う
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function GetDataCrsLeadgerCostCarrierChargeKbn(ByVal paramList As Hashtable) As DataTable
        Dim resultDataTable As New DataTable
        Dim sqlString As New StringBuilder
        Dim entBasic As New TCrsLedgerBasicEntityTehaiEx
        MyBase.paramClear()

        sqlString.AppendLine("SELECT ")
        sqlString.AppendLine("  CHARGE_KBN.SYUPT_DAY ")
        sqlString.AppendLine("  , CHARGE_KBN.CRS_CD ")
        sqlString.AppendLine("  , CHARGE_KBN.GOUSYA ")
        sqlString.AppendLine("  , CHARGE_KBN.CRS_ITINERARY_LINE_NO ")
        sqlString.AppendLine("  , CHARGE_KBN.LINE_NO ")
        sqlString.AppendLine("  , CHARGE_KBN.CARRIER_CD ")
        sqlString.AppendLine("  , CHARGE_KBN.CARRIER_EDABAN ")
        sqlString.AppendLine("  , CHARGE_KBN.CHARGE_KBN_JININ_CD ")
        sqlString.AppendLine("  , CHARGE_KBN.TANKA ")
        sqlString.AppendLine("  , CHARGE_KBN.SYSTEM_ENTRY_PGMID ")
        sqlString.AppendLine("  , CHARGE_KBN.SYSTEM_ENTRY_PERSON_CD ")
        sqlString.AppendLine("  , CHARGE_KBN.SYSTEM_ENTRY_DAY ")
        sqlString.AppendLine("  , CHARGE_KBN.SYSTEM_UPDATE_PGMID ")
        sqlString.AppendLine("  , CHARGE_KBN.SYSTEM_UPDATE_PERSON_CD ")
        sqlString.AppendLine("  , CHARGE_KBN.SYSTEM_UPDATE_DAY ")
        sqlString.AppendLine("  , CHARGE_KBN.DELETE_DATE ")
        sqlString.AppendLine("FROM ")
        sqlString.AppendLine("  T_CRS_LEDGER_COST_CARRIER_CHARGE_KBN CHARGE_KBN")
        sqlString.AppendLine("  LEFT JOIN T_CRS_LEDGER_BASIC_CHARGE_KBN BASIC ")
        sqlString.AppendLine("    ON CHARGE_KBN.CRS_CD = BASIC.CRS_CD ")
        sqlString.AppendLine("    AND  CHARGE_KBN.SYUPT_DAY = BASIC.SYUPT_DAY ")
        sqlString.AppendLine("    AND  CHARGE_KBN.GOUSYA = BASIC.GOUSYA ")
        sqlString.AppendLine("    AND  CHARGE_KBN.CHARGE_KBN_JININ_CD = BASIC.CHARGE_KBN_JININ_CD ")
        sqlString.AppendLine("WHERE ")
        sqlString.AppendLine("   " & makeSqlEx(entBasic.crsCd, paramList, "CHARGE_KBN"))
        sqlString.AppendLine("   AND")
        sqlString.AppendLine("   " & makeSqlEx(entBasic.syuptDay, paramList, "CHARGE_KBN"))
        sqlString.AppendLine("   AND")
        sqlString.AppendLine("   " & makeSqlEx(entBasic.gousya, paramList, "CHARGE_KBN"))
        sqlString.AppendLine("   AND")
        sqlString.AppendLine("   CHARGE_KBN.DELETE_DATE = 0 ")
        sqlString.AppendLine("   AND")
        sqlString.AppendLine("   CRS_ITINERARY_LINE_NO > 0 ")
        sqlString.AppendLine("ORDER BY ")
        sqlString.AppendLine("    CHARGE_KBN.CRS_ITINERARY_LINE_NO ")
        sqlString.AppendLine("  , CHARGE_KBN.CARRIER_CD ")
        sqlString.AppendLine("  , CHARGE_KBN.CARRIER_EDABAN ")
        sqlString.AppendLine("  , BASIC.LINE_NO ")

        Try
            resultDataTable = MyBase.getDataTable(sqlString.ToString)
        Catch ex As Exception
            Throw
        End Try

        Return resultDataTable
    End Function
#End Region

#Region "コース台帳（ホテル）"
    ''' <summary>
    ''' コース台帳（ホテル）を読み込みEntityクラスに格納する
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function getCrsLeadgerHotelEntity(ByVal paramList As Hashtable, ByVal crsLeaderBasic As EntityOperation(Of TCrsLedgerBasicEntityTehaiEx)) As Boolean  'getコースマスタEntity(対象コース情報,コースマスタ)

        Dim dtCostCarrier As DataTable  'コース台帳（ホテル）
        Dim lineNo As Integer  '行No

        Try
            dtCostCarrier = GetDataCrsLeadgerHotel(paramList)
        Catch ex As Exception
            Throw
        End Try

        For idxRow As Integer = 0 To dtCostCarrier.Rows.Count - 1

            lineNo = getKoshakashoIdx(crsLeaderBasic, CInt(dtCostCarrier.Rows(idxRow).Item("LINE_NO")))

            'For idx As Integer = 0 To crsLeaderBasic.EntityData(0).KoushakashoEntity(lineNo).HotelEntity.getPropertyDataLength - 1
            '    With crsLeaderBasic.EntityData(0).KoushakashoEntity(lineNo).HotelEntity
            '        If .getPtyType(idx, .EntityData(0)) Is GetType(EntityKoumoku_YmdType) Then
            '            If IsDBNull(dtCostCarrier.Rows(idxRow).Item(DirectCast(.getPtyValue(idx, .EntityData(0)), EntityKoumoku_YmdType).PhysicsName)) = False Then
            '                DirectCast(.getPtyValue(idx, .EntityData(0)), EntityKoumoku_YmdType).Value = CDate(dtCostCarrier.Rows(idxRow).Item(DirectCast(.getPtyValue(idx, .EntityData(0)), EntityKoumoku_YmdType).PhysicsName))
            '            End If
            '        ElseIf .getPtyType(idx, .EntityData(0)) Is GetType(EntityKoumoku_NumberType) Then
            '            If IsDBNull(dtCostCarrier.Rows(idxRow).Item(DirectCast(.getPtyValue(idx, .EntityData(0)), EntityKoumoku_NumberType).PhysicsName)) = False Then
            '                DirectCast(.getPtyValue(idx, .EntityData(0)), EntityKoumoku_NumberType).Value = CInt(dtCostCarrier.Rows(idxRow).Item(DirectCast(.getPtyValue(idx, .EntityData(0)), EntityKoumoku_NumberType).PhysicsName))
            '            End If
            '        ElseIf .getPtyType(idx, .EntityData(0)) Is GetType(EntityKoumoku_Number_DecimalType) Then
            '            If IsDBNull(dtCostCarrier.Rows(idxRow).Item(DirectCast(.getPtyValue(idx, .EntityData(0)), EntityKoumoku_Number_DecimalType).PhysicsName)) = False Then
            '                DirectCast(.getPtyValue(idx, .EntityData(0)), EntityKoumoku_Number_DecimalType).Value = CDec(dtCostCarrier.Rows(idxRow).Item(DirectCast(.getPtyValue(idx, .EntityData(0)), EntityKoumoku_Number_DecimalType).PhysicsName))
            '            End If
            '        Else
            '            DirectCast(.getPtyValue(idx, .EntityData(0)), EntityKoumoku_MojiType).Value = getDBValueForString(dtCostCarrier.Rows(idxRow).Item(DirectCast(.getPtyValue(idx, .EntityData(0)), EntityKoumoku_MojiType).PhysicsName))
            '        End If
            '    End With
            'Next
        Next

        Return True
    End Function

    ''' <summary>
    ''' コース台帳（ホテル）の読み込みを行う
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function GetDataCrsLeadgerHotel(ByVal paramList As Hashtable) As DataTable
        Dim resultDataTable As New DataTable
        Dim sqlString As New StringBuilder
        Dim entBasic As New TCrsLedgerBasicEntityTehaiEx
        MyBase.paramClear()

        sqlString.AppendLine("SELECT ")
        sqlString.AppendLine("  HOTEL.CRS_CD ")
        sqlString.AppendLine("  , HOTEL.DAILY ")
        sqlString.AppendLine("  , HOTEL.DELETE_DAY ")
        sqlString.AppendLine("  , HOTEL.GOUSYA ")
        sqlString.AppendLine("  , HOTEL.RIYOU_DAY ")
        sqlString.AppendLine("  , HOTEL.SIIRE_SAKI_CD ")
        sqlString.AppendLine("  , HOTEL.SIIRE_SAKI_EDABAN ")
        sqlString.AppendLine("  , HOTEL.SYUPT_DAY ")
        sqlString.AppendLine("  , HOTEL.SYSTEM_ENTRY_PGMID ")
        sqlString.AppendLine("  , HOTEL.SYSTEM_ENTRY_PERSON_CD ")
        sqlString.AppendLine("  , HOTEL.SYSTEM_ENTRY_DAY ")
        sqlString.AppendLine("  , HOTEL.SYSTEM_UPDATE_PGMID ")
        sqlString.AppendLine("  , HOTEL.SYSTEM_UPDATE_PERSON_CD ")
        sqlString.AppendLine("  , HOTEL.SYSTEM_UPDATE_DAY ")
        sqlString.AppendLine("FROM ")
        sqlString.AppendLine("  T_CRS_LEDGER_HOTEL HOTEL ")
        sqlString.AppendLine("WHERE ")
        sqlString.AppendLine("  HOTEL.CRS_CD = :CRS_CD ")
        sqlString.AppendLine("  AND HOTEL.SYUPT_DAY = :SYUPT_DAY ")
        sqlString.AppendLine("  AND HOTEL.GOUSYA = :GOUSYA ")
        sqlString.AppendLine("  AND HOTEL.DELETE_DAY = 0 ")
        sqlString.AppendLine("ORDER BY ")
        sqlString.AppendLine("  HOTEL.LINE_NO ")

        Try
            resultDataTable = MyBase.getDataTable(sqlString.ToString)
        Catch ex As Exception
            Throw
        End Try

        Return resultDataTable
    End Function
#End Region


#End Region


#Region " UPDATE処理 "

    ''' <summary>
    ''' DB接続用
    ''' </summary>
    ''' <param name="type"></param>
    ''' <param name="paramInfoListArrayList "></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function executeChargeInfoTehai(ByVal type As accessType, ByVal paramInfoListArrayList As Hashtable) As Integer

        Dim oracleTransaction As OracleTransaction = Nothing
        Dim returnValue As Integer = 1
        Dim sqlString As String = String.Empty
        Dim RET_OK As Integer = 1

        Try
            'トランザクション開始
            oracleTransaction = callBeginTransaction()

            Select Case type
                Case accessType.updateData
                    For Each ent As EntityOperation(Of TCrsLedgerBasicEntityTehaiEx) In paramInfoListArrayList.Values
                        Call updateCrsLedgerBasicInfoEntity(ent, oracleTransaction)
                    Next
            End Select

            Call callCommitTransaction(oracleTransaction)

        Catch ex As Exception
            Call callRollbackTransaction(oracleTransaction)
            Throw

        Finally
            Call oracleTransaction.Dispose()
        End Try

        Return returnValue

    End Function

    ''' <summary>
    ''' コース台帳関連テーブルの更新を行う
    ''' </summary>
    ''' <param name="crsEnt"></param>
    ''' <param name="paramTrans"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function updateCrsLedgerBasicInfoEntity(ByVal crsEnt As EntityOperation(Of TCrsLedgerBasicEntityTehaiEx), paramTrans As OracleTransaction) As Boolean

        'コース台帳(基本)更新
        Call updateCrsLedgerBasic(crsEnt, paramTrans)
        'コース台帳原価関連テーブル削除
        Call deleteCrsLedgerBasic(crsEnt, paramTrans)
        'コース台帳原価関連テーブル挿入
        Call insertCrsLedgerBasic(crsEnt, paramTrans)
        'コース原価台帳(基本)更新
        Call updateCrsLedgerCostBasic(crsEnt, paramTrans)

        Return True
    End Function

    ''' <summary>
    ''' コース台帳(基本)テーブルの更新を行う
    ''' </summary>
    ''' <param name="crsEnt"></param>
    ''' <param name="paramTrans"></param>
    ''' <remarks></remarks>
    Private Sub updateCrsLedgerBasic(ByVal crsEnt As EntityOperation(Of TCrsLedgerBasicEntityTehaiEx), paramTrans As OracleTransaction)
        Dim sqlString As String = String.Empty
        MyBase.paramClear()
        sqlString = updateCrsLedgerBasicSql(crsEnt)
        execNonQuery(paramTrans, sqlString)
    End Sub

    ''' <summary>
    ''' コース台帳(基本)テーブルの更新を行う
    ''' </summary>
    ''' <param name="crsEnt"></param>
    ''' <param name="paramTrans"></param>
    ''' <remarks></remarks>
    Private Sub updateCrsLedgerCostBasic(ByVal crsEnt As EntityOperation(Of TCrsLedgerBasicEntityTehaiEx), paramTrans As OracleTransaction)
        Dim sqlString As String = String.Empty
        MyBase.paramClear()
        sqlString = updateCrsLedgerCostBasicSql(crsEnt)
        execNonQuery(paramTrans, sqlString)
    End Sub

    Private Sub deleteCrsLedgerBasic(ByVal crsEnt As EntityOperation(Of TCrsLedgerBasicEntityTehaiEx), paramTrans As OracleTransaction)
        Dim sqlString As String
        Dim tblList As ArrayList = New ArrayList

        tblList.Add("T_CRS_LEDGER_KOSHAKASHO")
        tblList.Add("T_CRS_LEDGER_HOTEL")
        tblList.Add("T_CRS_LEDGER_COST_KOSHAKASHO")
        tblList.Add("T_CRS_LEDGER_COST_KOSHAKASHO_CHARGE_KBN")
        tblList.Add("T_CRS_LEDGER_COST_CARRIER")
        tblList.Add("T_CRS_LEDGER_COST_CARRIER_CHARGE_KBN")
        tblList.Add("T_CRS_LEDGER_COST_PLATE")

        For Each tblid As String In tblList
            Call MyBase.paramClear()
            sqlString = deleteCrsLedgerTableSql(tblid, crsEnt)
            Call MyBase.execNonQuery(paramTrans, sqlString)
        Next
    End Sub

    Private Sub insertCrsLedgerBasic(ByVal crsEnt As EntityOperation(Of TCrsLedgerBasicEntityTehaiEx), paramTrans As OracleTransaction)
        'コース台帳(降車ヶ所)更新
        Call exeCuteInsertKoushakashoData(crsEnt, paramTrans)

        'コース台帳(ホテル)更新
        Call exeCuteInsertHotelData(crsEnt, paramTrans)

        'コース台帳原価(降車ヶ所)更新
        Call exeCuteInsertCostKoushakashoData(crsEnt, paramTrans)

        'コース台帳原価(キャリア)更新
        Call exeCuteInsertCostCarrierData(crsEnt, paramTrans)

        ''コース台帳原価(降車ヶ所_料金区分)更新
        'Call exeCuteInsertCostPlateData(crsEnt, trans)

        ''コース台帳原価(キャリア_料金区分)更新
        'Call exeCuteInsertCostPlateData(crsEnt, trans)

        'コース台帳原価(プレート)更新
        Call exeCuteInsertCostPlateData(crsEnt, paramTrans)
    End Sub


    Private Function updateCrsLedgerBasicSql(ByVal crsEnt As EntityOperation(Of TCrsLedgerBasicEntityTehaiEx)) As String
        Dim sqlString As New StringBuilder
        MyBase.paramClear()

        sqlString.AppendLine("UPDATE T_CRS_LEDGER_BASIC ")
        sqlString.AppendLine("SET ")
        sqlString.AppendLine("  COST_SET_KBN = " & setParamEx(crsEnt.EntityData(0).costSetKbn))
        sqlString.AppendLine("  , SYSTEM_UPDATE_PGMID = " & setParamEx(crsEnt.EntityData(0).systemUpdatePgmid))
        sqlString.AppendLine("  , SYSTEM_UPDATE_PERSON_CD = " & setParamEx(crsEnt.EntityData(0).systemUpdatePersonCd))
        sqlString.AppendLine("  , SYSTEM_UPDATE_DAY =" & setParamEx(crsEnt.EntityData(0).systemUpdateDay))
        sqlString.AppendLine("WHERE ")
        sqlString.AppendLine("  CRS_CD = " & setParamEx(crsEnt.EntityData(0).crsCd))
        sqlString.AppendLine("  AND SYUPT_DAY = " & setParamEx(crsEnt.EntityData(0).syuptDay))
        sqlString.AppendLine("  AND GOUSYA = " & setParamEx(crsEnt.EntityData(0).gousya))

        Return sqlString.ToString
    End Function

    Private Function updateCrsLedgerCostBasicSql(ByVal crsEnt As EntityOperation(Of TCrsLedgerBasicEntityTehaiEx)) As String
        Dim sqlString As New StringBuilder
        MyBase.paramClear()

        sqlString.AppendLine("UPDATE T_CRS_LEDGER_COST_BASIC ")
        sqlString.AppendLine("SET ")
        sqlString.AppendLine("    LAST_UPDATE_DAY = " & setParamEx(crsEnt.EntityData(0).CostBasicEntity(0).lastUpdateDay))
        sqlString.AppendLine("  , LAST_UPDATE_PERSON_CD = " & setParamEx(crsEnt.EntityData(0).CostBasicEntity(0).lastUpdatePersonCd))
        sqlString.AppendLine("  , LAST_UPDATE_TIME = " & setParamEx(crsEnt.EntityData(0).CostBasicEntity(0).lastUpdateTime))
        sqlString.AppendLine("  , SYSTEM_UPDATE_PGMID = " & setParamEx(crsEnt.EntityData(0).CostBasicEntity(0).systemUpdatePgmid))
        sqlString.AppendLine("  , SYSTEM_UPDATE_PERSON_CD = " & setParamEx(crsEnt.EntityData(0).CostBasicEntity(0).systemUpdatePersonCd))
        sqlString.AppendLine("  , SYSTEM_UPDATE_DAY = " & setParamEx(crsEnt.EntityData(0).CostBasicEntity(0).systemUpdateDay))
        sqlString.AppendLine("WHERE ")
        sqlString.AppendLine("  CRS_CD = " & setParamEx(crsEnt.EntityData(0).CostBasicEntity(0).crsCd))
        sqlString.AppendLine("  AND SYUPT_DAY = " & setParamEx(crsEnt.EntityData(0).CostBasicEntity(0).syuptDay))
        sqlString.AppendLine("  AND GOUSYA = " & setParamEx(crsEnt.EntityData(0).CostBasicEntity(0).gousya))

        Return sqlString.ToString
    End Function

    Private Function deleteCrsLedgerTableSql(ByVal tblId As String, ByVal paramTrans As EntityOperation(Of TCrsLedgerBasicEntityTehaiEx)) As String
        Dim sqlString As New StringBuilder
        MyBase.paramClear()

        sqlString.AppendLine("DELETE FROM " & tblId)
        sqlString.AppendLine("WHERE ")
        sqlString.AppendLine("  CRS_CD = " & setParamEx(paramTrans.EntityData(0).crsCd))
        sqlString.AppendLine("  AND SYUPT_DAY = " & setParamEx(paramTrans.EntityData(0).syuptDay))
        sqlString.AppendLine("  AND GOUSYA = " & setParamEx(paramTrans.EntityData(0).gousya))
        If tblId = "T_CRS_LEDGER_COST_CARRIER" Then
            sqlString.AppendLine("  AND CRS_ITINERARY_LINE_NO > 0 ")
        End If

        Return sqlString.ToString
    End Function

    ''' <summary>
    ''' コース台帳（降車ヶ所）挿入
    ''' </summary>
    ''' <param name="crsEntity">Entity</param>
    ''' <param name="paramTrans">Oracleトランザクション</param>
    ''' <returns>更新件数(-1:エラー,0以上:正常)</returns>
    ''' <remarks></remarks>
    Private Function exeCuteInsertKoushakashoData(ByVal crsEntity As EntityOperation(Of TCrsLedgerBasicEntityTehaiEx), ByVal paramTrans As OracleTransaction) As Integer
        Dim CrsMasterEntity As EntityOperation(Of TCrsLedgerKoshakashoEntityTehaiEx) = crsEntity.EntityData(0).KoushakashoEntity
        Dim sqlString As String = ""  'sqlString
        Dim sqlInsertString As StringBuilder = New StringBuilder
        Dim sqlValueString As StringBuilder = New StringBuilder
        Dim insertCnt As Integer
        Dim prop As System.Reflection.PropertyInfo
        Dim iEnt As IEntityKoumokuType
        Dim strCnm As String = String.Empty

        For entityCnt As Integer = 0 To CrsMasterEntity.EntityData.Length - 1
            'ホテルのみ処理
            If CrsMasterEntity.EntityData(entityCnt).siireSakiKindCd.Value.Equals(FixedCd.SuppliersKind_Stay) = False Then
                paramClear()
                'データ未設定行はスキップ
                If CrsMasterEntity.EntityData(entityCnt).crsCd.Value = "" Then
                    Continue For
                End If
                strCnm = String.Empty
                'SQL文生成
                sqlInsertString.Clear()
                sqlValueString.Clear()
                sqlInsertString.AppendLine("INSERT INTO T_CRS_LEDGER_KOSHAKASHO ")
                sqlInsertString.AppendLine("(")
                sqlValueString.AppendLine(")VALUES(")

                For idx As Integer = 0 To CrsMasterEntity.getPropertyDataLength - 1
                    With CrsMasterEntity
                        prop = DirectCast(CrsMasterEntity.getProperty(idx), Reflection.PropertyInfo)
                        If prop.DeclaringType Is Nothing = False Then
                            If prop.DeclaringType Is GetType(TCrsLedgerKoshakashoEntity) Then
                                iEnt = DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), IEntityKoumokuType)
                                sqlInsertString.AppendLine(strCnm & iEnt.PhysicsName)
                                sqlValueString.AppendLine(strCnm & setParamEx(iEnt))
                                strCnm = ","
                            End If
                        End If
                    End With
                Next
                sqlValueString.AppendLine(")")
                sqlString = sqlInsertString.ToString & sqlValueString.ToString
                insertCnt += execNonQuery(paramTrans, sqlString)
            End If
        Next
        Return insertCnt
    End Function

    Private Function makeHotelEntity(ByVal crsEntity As EntityOperation(Of TCrsLedgerBasicEntityTehaiEx)) As EntityOperation(Of TCrsLedgerHotelEntity)
        Dim hotels As EntityOperation(Of TCrsLedgerHotelEntity) = New EntityOperation(Of TCrsLedgerHotelEntity)
        Dim hotel As TCrsLedgerHotelEntity
        Dim lineNo As Integer = 1
        For Each ent As TCrsLedgerKoshakashoEntityTehaiEx In crsEntity.EntityData(0).KoushakashoEntity.EntityData
            If ent.siireSakiKindCd.Value.Equals(FixedCd.SuppliersKind_Stay) = True Then
                'Entityの変換
                hotel = New TCrsLedgerHotelEntity
                hotel.crsCd.Value = ent.crsCd.Value
                hotel.daily.Value = ent.daily.Value
                hotel.deleteDay.Value = ent.deleteDay.Value
                hotel.gousya.Value = ent.gousya.Value
                hotel.riyouDay.Value = ent.riyouDay.Value
                hotel.siireSakiCd.Value = ent.koshakashoCd.Value
                hotel.siireSakiEdaban.Value = ent.koshakashoEdaban.Value
                hotel.syuptDay.Value = ent.syuptDay.Value
                hotel.systemEntryPgmid.Value = ent.systemEntryPgmid.Value
                hotel.systemEntryPersonCd.Value = ent.systemEntryPersonCd.Value
                hotel.systemEntryDay.Value = ent.systemEntryDay.Value
                hotel.systemUpdatePgmid.Value = ent.systemUpdatePgmid.Value
                hotel.systemUpdatePersonCd.Value = ent.systemUpdatePersonCd.Value
                hotel.systemUpdateDay.Value = ent.systemUpdateDay.Value
                hotel.lineNo.Value = lineNo
                lineNo += 1
                hotels.add(hotel)
            End If
        Next
        '1行目は不要なため削除
        If hotels.EntityData.Length > 1 Then
            hotels.remove(0)
        End If
        Return hotels
    End Function

    ''' <summary>
    ''' コース台帳（ホテル）挿入
    ''' </summary>
    ''' <param name="crsEntity">Entity</param>
    ''' <param name="paramTrans">Oracleトランザクション</param>
    ''' <returns>更新件数(-1:エラー,0以上:正常)</returns>
    ''' <remarks></remarks>
    Private Function exeCuteInsertHotelData(ByVal crsEntity As EntityOperation(Of TCrsLedgerBasicEntityTehaiEx), ByVal paramTrans As OracleTransaction) As Integer
        Dim CrsMasterEntity As EntityOperation(Of TCrsLedgerHotelEntity) = makeHotelEntity(crsEntity)
        Dim sqlString As String = ""  'sqlString
        Dim sqlInsertString As StringBuilder = New StringBuilder
        Dim sqlValueString As StringBuilder = New StringBuilder
        Dim insertCnt As Integer
        Dim iEnt As IEntityKoumokuType
        Dim strCnm As String = String.Empty

        For entityCnt As Integer = 0 To CrsMasterEntity.EntityData.Length - 1
            'ホテルのみ処理
            paramClear()
            'データ未設定行はスキップ
            If CrsMasterEntity.EntityData(entityCnt).crsCd.Value = "" Then
                Continue For
            End If
            strCnm = String.Empty
            'SQL文生成
            sqlInsertString.Clear()
            sqlValueString.Clear()
            sqlInsertString.AppendLine("INSERT INTO T_CRS_LEDGER_HOTEL ")
            sqlInsertString.AppendLine("(")
            sqlValueString.AppendLine(")VALUES(")

            For idx As Integer = 0 To CrsMasterEntity.getPropertyDataLength - 1
                With CrsMasterEntity
                    iEnt = DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), IEntityKoumokuType)
                    sqlInsertString.AppendLine(strCnm & iEnt.PhysicsName)
                    sqlValueString.AppendLine(strCnm & setParamEx(iEnt))
                    strCnm = ","
                End With
            Next
            sqlValueString.AppendLine(")")
            sqlString = sqlInsertString.ToString & sqlValueString.ToString
            insertCnt += execNonQuery(paramTrans, sqlString)
        Next
        Return insertCnt
    End Function

    ''' <summary>
    ''' コース台帳原価（降車ヶ所）挿入
    ''' </summary>
    ''' <param name="crsEntity">Entity</param>
    ''' <param name="paramTrans">Oracleトランザクション</param>
    ''' <returns>更新件数(-1:エラー,0以上:正常)</returns>
    ''' <remarks></remarks>
    Private Function exeCuteInsertCostKoushakashoData(ByVal crsEntity As EntityOperation(Of TCrsLedgerBasicEntityTehaiEx), ByVal paramTrans As OracleTransaction) As Integer
        Dim CrsMasterEntity As EntityOperation(Of TCrsLedgerCostKoshakashoEntityTehaiEx)
        Dim sqlString As String = ""  'sqlString
        Dim sqlInsertString As StringBuilder = New StringBuilder
        Dim sqlValueString As StringBuilder = New StringBuilder
        Dim insertCnt As Integer
        Dim prop As System.Reflection.PropertyInfo
        Dim iEnt As IEntityKoumokuType
        Dim strCnm As String = String.Empty

        For Each ent As TCrsLedgerKoshakashoEntityTehaiEx In crsEntity.EntityData(0).KoushakashoEntity.EntityData
            'キャリア以外
            If ent.siireSakiKindCd.Value.Equals(FixedCd.SuppliersKind_Carrier) = False Then
                CrsMasterEntity = ent.CostKoshakashoEntity
                For entityCnt As Integer = 0 To CrsMasterEntity.EntityData.Length - 1
                    paramClear()
                    'データ未設定 Or 作成フラグ(PG作成)がONは実行しない
                    If CrsMasterEntity.EntityData(entityCnt).crsCd.Value = "" OrElse CrsMasterEntity.EntityData(entityCnt).makeFlg.Value.Equals(TCrsLedgerCostKoshakashoEntityTehaiEx.MakeFlgType.On) Then
                        Continue For
                    End If
                    strCnm = String.Empty
                    'SQL文生成
                    sqlInsertString.Clear()
                    sqlValueString.Clear()
                    sqlInsertString.AppendLine("INSERT INTO T_CRS_LEDGER_COST_KOSHAKASHO ")
                    sqlInsertString.AppendLine("(")
                    sqlValueString.AppendLine(")VALUES(")

                    For idx As Integer = 0 To CrsMasterEntity.getPropertyDataLength - 1
                        With CrsMasterEntity
                            prop = DirectCast(CrsMasterEntity.getProperty(idx), Reflection.PropertyInfo)
                            If prop.DeclaringType Is Nothing = False Then
                                If prop.DeclaringType Is GetType(TCrsLedgerCostKoshakashoEntity) Then
                                    iEnt = DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), IEntityKoumokuType)
                                    sqlInsertString.AppendLine(strCnm & iEnt.PhysicsName)
                                    sqlValueString.AppendLine(strCnm & setParamEx(iEnt))
                                    strCnm = ","
                                End If
                            End If
                        End With
                    Next
                    sqlValueString.AppendLine(")")
                    sqlString = sqlInsertString.ToString & sqlValueString.ToString
                    insertCnt += execNonQuery(paramTrans, sqlString)

                    '料金区分作成
                    Call exeCuteInsertCostKoushakashoChargeKbnData(CrsMasterEntity.EntityData(entityCnt).CostKoshakashoChargeKbnEntity, paramTrans)
                Next
            End If
        Next
        Return insertCnt
    End Function

    ''' <summary>
    ''' コース台帳原価（降車ヶ所）料金区分挿入
    ''' </summary>
    ''' <param name="crsEntity">Entity</param>
    ''' <param name="paramTrans">Oracleトランザクション</param>
    ''' <returns>更新件数(-1:エラー,0以上:正常)</returns>
    ''' <remarks></remarks>
    Private Function exeCuteInsertCostKoushakashoChargeKbnData(ByVal crsEntity As EntityOperation(Of TCrsLedgerCostKoshakashoChargeKbnEntity), ByVal paramTrans As OracleTransaction) As Integer
        Dim sqlString As String = ""  'sqlString
        Dim sqlInsertString As StringBuilder = New StringBuilder
        Dim sqlValueString As StringBuilder = New StringBuilder
        Dim insertCnt As Integer
        Dim iEnt As IEntityKoumokuType
        Dim strCnm As String = String.Empty

        For entityCnt As Integer = 0 To crsEntity.EntityData.Length - 1
            paramClear()
            'データ未設定 Or 作成フラグ(PG作成)がONは実行しない
            If crsEntity.EntityData(entityCnt).crsCd.Value = "" Then
                Continue For
            End If
            strCnm = String.Empty
            'SQL文生成
            sqlInsertString.Clear()
            sqlValueString.Clear()
            sqlInsertString.AppendLine("INSERT INTO T_CRS_LEDGER_COST_KOSHAKASHO_CHARGE_KBN ")
            sqlInsertString.AppendLine("(")
            sqlValueString.AppendLine(")VALUES(")

            For idx As Integer = 0 To crsEntity.getPropertyDataLength - 1
                With crsEntity
                    iEnt = DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), IEntityKoumokuType)
                    sqlInsertString.AppendLine(strCnm & iEnt.PhysicsName)
                    sqlValueString.AppendLine(strCnm & setParamEx(iEnt))
                    strCnm = ","
                End With
            Next
            sqlValueString.AppendLine(")")
            sqlString = sqlInsertString.ToString & sqlValueString.ToString
            insertCnt += execNonQuery(paramTrans, sqlString)
        Next

        Return insertCnt
    End Function

    ''' <summary>
    ''' コース台帳（キャリア）挿入
    ''' </summary>
    ''' <param name="crsEntity">Entity</param>
    ''' <param name="paramTrans">Oracleトランザクション</param>
    ''' <returns>更新件数(-1:エラー,0以上:正常)</returns>
    ''' <remarks></remarks>
    Private Function exeCuteInsertCostCarrierData(ByVal crsEntity As EntityOperation(Of TCrsLedgerBasicEntityTehaiEx), ByVal paramTrans As OracleTransaction) As Integer
        Dim CrsMasterEntity As EntityOperation(Of TCrsLedgerCostCarrierEntityTehaiEx)
        Dim sqlString As String = ""  'sqlString
        Dim sqlInsertString As StringBuilder = New StringBuilder
        Dim sqlValueString As StringBuilder = New StringBuilder
        Dim insertCnt As Integer
        Dim prop As System.Reflection.PropertyInfo
        Dim iEnt As IEntityKoumokuType
        Dim strCnm As String = String.Empty

        For Each ent As TCrsLedgerKoshakashoEntityTehaiEx In crsEntity.EntityData(0).KoushakashoEntity.EntityData
            'キャリア以外
            If ent.siireSakiKindCd.Value.Equals(FixedCd.SuppliersKind_Carrier) = True Then
                CrsMasterEntity = ent.CostCarrierEntity
                For entityCnt As Integer = 0 To CrsMasterEntity.EntityData.Length - 1
                    paramClear()
                    'データ未設定 Or 作成フラグ(PG作成)がONは実行しない
                    If CrsMasterEntity.EntityData(entityCnt).crsCd.Value = "" OrElse CrsMasterEntity.EntityData(entityCnt).makeFlg.Value.Equals(TCrsLedgerCostKoshakashoEntityTehaiEx.MakeFlgType.On) Then
                        Continue For
                    End If
                    strCnm = String.Empty
                    'SQL文生成
                    sqlInsertString.Clear()
                    sqlValueString.Clear()
                    sqlInsertString.AppendLine("INSERT INTO T_CRS_LEDGER_COST_CARRIER ")
                    sqlInsertString.AppendLine("(")
                    sqlValueString.AppendLine(")VALUES(")

                    For idx As Integer = 0 To CrsMasterEntity.getPropertyDataLength - 1
                        With CrsMasterEntity
                            prop = DirectCast(CrsMasterEntity.getProperty(idx), Reflection.PropertyInfo)
                            If prop.DeclaringType Is Nothing = False Then
                                If prop.DeclaringType Is GetType(TCrsLedgerCostCarrierEntity) Then
                                    iEnt = DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), IEntityKoumokuType)
                                    sqlInsertString.AppendLine(strCnm & iEnt.PhysicsName)
                                    sqlValueString.AppendLine(strCnm & setParamEx(iEnt))
                                    strCnm = ","
                                End If
                            End If
                        End With
                    Next
                    sqlValueString.AppendLine(")")
                    sqlString = sqlInsertString.ToString & sqlValueString.ToString
                    insertCnt += execNonQuery(paramTrans, sqlString)

                    '料金区分作成
                    Call exeCuteInsertCostCarrierChargeKbnData(CrsMasterEntity.EntityData(entityCnt).CostCarrierChargeKbnEntity, paramTrans)

                Next
            End If
        Next
        Return insertCnt
    End Function

    ''' <summary>
    ''' コース台帳原価（キャリア）料金区分挿入
    ''' </summary>
    ''' <param name="crsEntity">Entity</param>
    ''' <param name="paramTrans">Oracleトランザクション</param>
    ''' <returns>更新件数(-1:エラー,0以上:正常)</returns>
    ''' <remarks></remarks>
    Private Function exeCuteInsertCostCarrierChargeKbnData(ByVal crsEntity As EntityOperation(Of TCrsLedgerCostCarrierChargeKbnEntity), ByVal paramTrans As OracleTransaction) As Integer
        Dim sqlString As String = ""  'sqlString
        Dim sqlInsertString As StringBuilder = New StringBuilder
        Dim sqlValueString As StringBuilder = New StringBuilder
        Dim insertCnt As Integer
        Dim iEnt As IEntityKoumokuType
        Dim strCnm As String = String.Empty

        For entityCnt As Integer = 0 To crsEntity.EntityData.Length - 1
            paramClear()
            'データ未設定 Or 作成フラグ(PG作成)がONは実行しない
            If crsEntity.EntityData(entityCnt).crsCd.Value = "" Then
                Continue For
            End If
            strCnm = String.Empty
            'SQL文生成
            sqlInsertString.Clear()
            sqlValueString.Clear()
            sqlInsertString.AppendLine("INSERT INTO T_CRS_LEDGER_COST_CARRIER_CHARGE_KBN ")
            sqlInsertString.AppendLine("(")
            sqlValueString.AppendLine(")VALUES(")

            For idx As Integer = 0 To crsEntity.getPropertyDataLength - 1
                With crsEntity
                    iEnt = DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), IEntityKoumokuType)
                    sqlInsertString.AppendLine(strCnm & iEnt.PhysicsName)
                    sqlValueString.AppendLine(strCnm & setParamEx(iEnt))
                    strCnm = ","
                End With
            Next
            sqlValueString.AppendLine(")")
            sqlString = sqlInsertString.ToString & sqlValueString.ToString
            insertCnt += execNonQuery(paramTrans, sqlString)
        Next

        Return insertCnt
    End Function























    ''' <summary>
    ''' コース台帳原価（プレート）挿入
    ''' </summary>
    ''' <param name="crsEnt">Entity</param>
    ''' <param name="paramTrans">Oracleトランザクション</param>
    ''' <returns>更新件数(-1:エラー,0以上:正常)</returns>
    ''' <remarks></remarks>
    Private Function exeCuteInsertCostPlateData(ByVal crsEnt As EntityOperation(Of TCrsLedgerBasicEntityTehaiEx), ByVal paramTrans As OracleTransaction) As Integer
        Dim CrsMasterEntity As EntityOperation(Of TCrsLedgerCostPlateEntity) = crsEnt.EntityData(0).CostPlateEntity
        Dim sqlString As String = ""  'sqlString
        Dim sqlInsertString As StringBuilder = New StringBuilder
        Dim sqlValueString As StringBuilder = New StringBuilder
        Dim insertCnt As Integer
        Dim iEnt As IEntityKoumokuType
        Dim strCnm As String = String.Empty

        For entityCnt As Integer = 0 To CrsMasterEntity.EntityData.Length - 1
            paramClear()
            'データ未設定行はスキップ
            If CrsMasterEntity.EntityData(entityCnt).crsCd.Value = "" Then
                Continue For
            End If
            strCnm = String.Empty
            'SQL文生成
            sqlInsertString.Clear()
            sqlValueString.Clear()
            sqlInsertString.AppendLine("INSERT INTO " & "T_CRS_LEDGER_COST_PLATE ")
            sqlInsertString.AppendLine("(")
            sqlValueString.AppendLine(")VALUES(")

            For idx As Integer = 0 To CrsMasterEntity.getPropertyDataLength - 1
                With CrsMasterEntity
                    iEnt = DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), IEntityKoumokuType)
                    sqlInsertString.AppendLine(strCnm & iEnt.PhysicsName)
                    sqlValueString.AppendLine(strCnm & setParamEx(iEnt))
                    strCnm = ","
                End With
            Next
            sqlValueString.AppendLine(")")
            sqlString = sqlInsertString.ToString & sqlValueString.ToString
            insertCnt += execNonQuery(paramTrans, sqlString)
        Next
        Return insertCnt
    End Function

#End Region

#Region "Privateメソッド"
    ''' <summary>
    ''' 降車ヶ所エンティティ内の配列番号を取得する
    ''' </summary>
    ''' <param name="crsLeaderBasic">コース台帳Entity</param>
    ''' <param name="lineNo">行No</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function getKoshakashoIdx(ByVal crsLeaderBasic As EntityOperation(Of TCrsLedgerBasicEntityTehaiEx), ByVal lineNo As Integer) As Integer

        Dim koushaIdx As Integer = 0  '降車ヶ所idx

        With crsLeaderBasic.EntityData(0).KoushakashoEntity
            For koshakashoidx = 0 To .EntityData.Length - 1
                If .EntityData(koshakashoidx).lineNo.Value = lineNo Then
                    Return koshakashoidx
                End If
            Next
        End With
    End Function

    ''' <summary>
    ''' DBより取得した値をString型に変換する
    ''' ※DBNullは空文字に変換
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function getDBValueForString(ByVal targetObj As Object) As String

        Dim retValue As String = ""  'retValue

        If IsDBNull(targetObj) = False Then
            retValue = targetObj.ToString
        End If
        Return retValue
    End Function

    ''' <summary>
    ''' パラメータ設定
    ''' </summary>
    ''' <param name="ent">エンティティ項目</param>
    ''' <param name="paramList">SQLパラメータHashTable</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function makeSqlEx(ByVal ent As IEntityKoumokuType, ByVal paramList As Hashtable, Optional ByVal tblName As String = "") As String
        Dim constr As String = String.Empty
        If String.Empty.Equals(tblName) = False Then
            constr = "."
        End If
        Return tblName & constr & ent.PhysicsName & " = " & MyBase.setParam(ent.PhysicsName, paramList(ent.PhysicsName), ent.DBType, ent.IntegerBu, ent.DecimalBu)
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
        Return String.Empty
    End Function

#End Region

End Class
