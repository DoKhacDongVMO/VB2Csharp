Imports System.Text

''' <summary>
''' バス会社確定通知/記録照会のDAクラス
''' </summary>
Public Class S03_0502DA
    Inherits DataAccessorBase

#Region " 定数／変数 "
    Private ParamNum As Integer = 0
#End Region

#Region " SELECT処理 "
    ''' <summary>
    ''' 検索（確定通知）
    ''' </summary>
    ''' <param name="param"></param>
    ''' <returns>DataTable</returns>
    Public Function selectDataTableConfirmNotice(ByVal param As S03_0502DASelectParam) As DataTable
        Dim crsLedgerBasic As New CrsLedgerBasicEntity
        'SQL文字列
        Dim sb As New StringBuilder

        'パラメータクリア
        clear()

        'SELECT句
        sb.AppendLine("SELECT ")
        sb.AppendLine("       TO_YYYYMMDD_FC(BASIC.SYUPT_DAY) AS SYUPT_DAY_STR ")
        sb.AppendLine("     , BASIC.SYUPT_DAY AS SYUPT_DAY ")
        sb.AppendLine("     , BASIC.CRS_CD AS CRS_CD ")
        sb.AppendLine("     , BASIC.CRS_NAME AS CRS_NAME ")
        sb.AppendLine("     , TO_HHMM_FC(BASIC.SYUPT_TIME_1) AS SYUPT_TIME_STR ")
        sb.AppendLine("     , BASIC.GOUSYA AS GOUSYA ")
        sb.AppendLine("     , BASIC.BUS_COMPANY_CD AS BUS_COMPANY_CD ")
        sb.AppendLine("     , SIIRE.SIIRE_SAKI_NAME AS BUS_COMPANY_NAME ")
        sb.AppendLine("     , INFO.YOYAKU_KBN AS YOYAKU_KBN ")
        sb.AppendLine("     , INFO.YOYAKU_NO AS YOYAKU_NO ")
        sb.AppendLine("     , TO_CHAR(INFO.YOYAKU_KBN || INFO.YOYAKU_NO, 'FM0,999,999,999') AS YOYAKU_NUMBER ")
        sb.AppendLine("     , INFO.SURNAME || INFO.NAME AS YOYAKU_NAME ")
        sb.AppendLine("     , CHARGE.NINZU AS NINZU ")
        sb.AppendLine("     , INFO.AGENT_CD AS AGENT_CD ")
        sb.AppendLine("     , INFO.AGENT_NM AS AGENT_NM ")
        sb.AppendLine("     , AGENT.AGENT_FORMAL_NAME AS AGENT_FORMAL_NAME ")
        sb.AppendLine("     , CASE WHEN INFO.MAIL_SENDING_KBN = 'Y' THEN '1' ELSE '3' END AS NOTIFICATION_HOHO ")
        sb.AppendLine("     , TRIM(AGENT.BUS_NOTIFICATION_HOHO) AS BUS_NOTIFICATION_HOHO ")
        sb.AppendLine("     , '' AS NOTIFICATION_HOHO_RIREKI ")
        sb.AppendLine("     , TO_YYYYMMDD_FC(NOTIFICATION.KAKUTEI_NOTIFICATION_DAY) AS KAKUTEI_NOTIFICATION_DAY ")
        sb.AppendLine("     , TO_CHAR(INFO.SYSTEM_ENTRY_DAY, 'YYYY/MM/DD') AS YOYAKU_ENTRY_DAY ")
        sb.AppendLine("     , INFO.MAIL_ADDRESS AS MAIL_ADDRESS ")
        sb.AppendLine("     , AGENT.MAIL AS MAIL_ADDRESS2 ")
        sb.AppendLine("     , AGENT.FAX AS FAX ")
        sb.AppendLine("     , INFO.YUBIN_NO AS YUBIN_NO ")
        sb.AppendLine("     , AGENT.YUBIN_NO AS YUBIN_NO2 ")
        sb.AppendLine("     , INFO.ADDRESS_1 AS ADDRESS_1 ")
        sb.AppendLine("     , INFO.ADDRESS_2 AS ADDRESS_2 ")
        sb.AppendLine("     , INFO.ADDRESS_3 AS ADDRESS_3 ")
        sb.AppendLine("     , AGENT.ADDRESS_1 AS ADDRESS2_1 ")
        sb.AppendLine("     , AGENT.ADDRESS_2 AS ADDRESS2_2 ")
        sb.AppendLine("     , AGENT.ADDRESS_3 AS ADDRESS2_3 ")
        sb.AppendLine("     , INFO.YOYAKU_UKETUKE_KBN AS YOYAKU_UKETUKE_KBN ")
        sb.AppendLine("     , CASE INFO.YOYAKU_UKETUKE_KBN WHEN '2' THEN 'インターネット' ELSE '' END AS YOYAKU_STATU ")
        sb.AppendLine("     , NOTIFICATION.BUS_COMPANY_CD AS BUS_COMPANY_CD_RIREKI ")
        sb.AppendLine("     , AGENT.OTA_KBN AS OTA_KBN")
        sb.AppendLine("     , BASIC.SYUPT_JI_CARRIER_KBN AS SYUPT_JI_CARRIER_KBN")
        sb.AppendLine("     , PLACE_1.PLACE_NAME_1 AS SYUGO_PLACE1")
        sb.AppendLine("     , PLACE_2.PLACE_NAME_1 AS SYUGO_PLACE2")
        sb.AppendLine("     , PLACE_3.PLACE_NAME_1 AS SYUGO_PLACE3")
        sb.AppendLine("     , PLACE_4.PLACE_NAME_1 AS SYUGO_PLACE4")
        sb.AppendLine("     , PLACE_5.PLACE_NAME_1 AS SYUGO_PLACE5")
        sb.AppendLine("     , PLACE_6.PLACE_NAME_1 AS SYUGO_PLACE_CARRIER")
        sb.AppendLine("     , TO_HHMM_FC(BASIC.SYUGO_TIME_1) AS SYUGO_TIME1")
        sb.AppendLine("     , TO_HHMM_FC(BASIC.SYUGO_TIME_2) AS SYUGO_TIME2")
        sb.AppendLine("     , TO_HHMM_FC(BASIC.SYUGO_TIME_3) AS SYUGO_TIME3")
        sb.AppendLine("     , TO_HHMM_FC(BASIC.SYUGO_TIME_4) AS SYUGO_TIME4")
        sb.AppendLine("     , TO_HHMM_FC(BASIC.SYUGO_TIME_5) AS SYUGO_TIME5")
        sb.AppendLine("     , TO_HHMM_FC(BASIC.SYUGO_TIME_CARRIER) AS SYUGO_TIME_CARRIER")
        sb.AppendLine("     , TO_HHMM_FC(BASIC.SYUPT_TIME_1) AS SYUPT_TIME1")
        sb.AppendLine("     , TO_HHMM_FC(BASIC.SYUPT_TIME_2) AS SYUPT_TIME2")
        sb.AppendLine("     , TO_HHMM_FC(BASIC.SYUPT_TIME_3) AS SYUPT_TIME3")
        sb.AppendLine("     , TO_HHMM_FC(BASIC.SYUPT_TIME_4) AS SYUPT_TIME4")
        sb.AppendLine("     , TO_HHMM_FC(BASIC.SYUPT_TIME_5) AS SYUPT_TIME5")
        sb.AppendLine("     , TO_HHMM_FC(BASIC.SYUPT_TIME_CARRIER) AS SYUPT_TIME_CARRIER")
        sb.AppendLine("     , INFO.ZASEKI AS ZASEKI")
        sb.AppendLine("     , INFO.JYOSYA_NINZU_1 AS JYOSYA_NINZU_1")
        sb.AppendLine("     , INFO.JYOSYA_NINZU_2 AS JYOSYA_NINZU_2")
        sb.AppendLine("     , INFO.JYOSYA_NINZU_3 AS JYOSYA_NINZU_3")
        sb.AppendLine("     , INFO.JYOSYA_NINZU_4 AS JYOSYA_NINZU_4")
        sb.AppendLine("     , INFO.JYOSYA_NINZU_5 AS JYOSYA_NINZU_5")
        sb.AppendLine("     , '' AS NYUUKIN_SITUATION_KBN ")
        sb.AppendLine("     , '' AS CONTACT_FLG ")
        sb.AppendLine("     , '' AS STATU ")
        sb.AppendLine("     , '' AS TEIKI_KIKAKU_KBN ")
        sb.AppendLine("     , '' AS SAIKOU_KAKUTEI_KBN ")
        sb.AppendLine("     , '' AS CANCEL_FLG ")

        'FROM句
        sb.AppendLine("    FROM ")

        'コース台帳(基本) BASIC
        sb.AppendLine("    T_CRS_LEDGER_BASIC BASIC")

        '内部結合
        sb.AppendLine("    INNER JOIN ")

        '予約情報（基本）
        sb.AppendLine("        T_YOYAKU_INFO_BASIC INFO")
        sb.AppendLine("        ON BASIC.CRS_CD = INFO.CRS_CD ")
        sb.AppendLine("        AND BASIC.SYUPT_DAY = INFO.SYUPT_DAY ")
        sb.AppendLine("        AND BASIC.GOUSYA = INFO.GOUSYA ")
        sb.AppendLine("        AND INFO.YOYAKU_KBN IN ('0','1','2','3','4','5','6','7','8','9') ")
        sb.AppendLine("        AND INFO.CANCEL_FLG IS NULL ")
        sb.AppendLine("        AND INFO.NYUUKIN_SITUATION_KBN = '1' ")

        '内部結合
        sb.AppendLine("    INNER JOIN ")

        '予約情報（コース料金_料金区分）CHARGE
        sb.AppendLine("        ( ")
        sb.AppendLine("        SELECT")
        sb.AppendLine("              YOYAKU_KBN ")
        sb.AppendLine("            , YOYAKU_NO ")
        sb.AppendLine("            , SUM(CHARGE_APPLICATION_NINZU) AS NINZU ")
        sb.AppendLine("        FROM ")
        sb.AppendLine("            T_YOYAKU_INFO_CRS_CHARGE_CHARGE_KBN ")
        sb.AppendLine("        GROUP BY ")
        sb.AppendLine("              YOYAKU_KBN ")
        sb.AppendLine("            , YOYAKU_NO ")
        sb.AppendLine("        ) CHARGE ")
        sb.AppendLine("        ON INFO.YOYAKU_KBN = CHARGE.YOYAKU_KBN ")
        sb.AppendLine("        AND INFO.YOYAKU_NO = CHARGE.YOYAKU_NO ")

        '左結合
        sb.AppendLine("    LEFT JOIN ")

        '仕入先マスタ
        sb.AppendLine("        M_SIIRE_SAKI SIIRE")
        sb.AppendLine("        ON SUBSTR(BASIC.BUS_COMPANY_CD, 1, 4) = SIIRE.SIIRE_SAKI_CD ")
        sb.AppendLine("        AND SUBSTR(BASIC.BUS_COMPANY_CD, LENGTH(BASIC.BUS_COMPANY_CD) - 1, 2) = SIIRE.SIIRE_SAKI_NO ")
        sb.AppendLine("        AND SIIRE.DELETE_DATE IS NULL ")

        '左結合
        sb.AppendLine("    LEFT JOIN ")

        'バス会社確定通知出力記録
        sb.AppendLine("        T_BUS_COMPANY_KAKUTEI_NOTIFICATION_OUT NOTIFICATION")
        sb.AppendLine("        ON BASIC.CRS_CD = NOTIFICATION.CRS_CD ")
        sb.AppendLine("        AND BASIC.SYUPT_DAY = NOTIFICATION.SYUPT_DAY ")
        sb.AppendLine("        AND BASIC.GOUSYA = NOTIFICATION.GOUSYA ")
        sb.AppendLine("        AND INFO.YOYAKU_KBN = NOTIFICATION.YOYAKU_KBN ")
        sb.AppendLine("        AND INFO.YOYAKU_NO = NOTIFICATION.YOYAKU_NO ")

        '左結合
        sb.AppendLine("    LEFT JOIN ")

        '代理店マスタ
        sb.AppendLine("        M_AGENT AGENT")
        sb.AppendLine("        ON INFO.AGENT_CD = AGENT.AGENT_CD ")
        sb.AppendLine("        AND AGENT.DELETE_DATE IS NULL ")

        '左結合
        sb.AppendLine("    LEFT JOIN ")

        '場所マスタ PLACE_1
        sb.AppendLine("        M_PLACE PLACE_1")
        sb.AppendLine("        ON BASIC.HAISYA_KEIYU_CD_1 = PLACE_1.PLACE_CD ")
        sb.AppendLine("        AND PLACE_1.DELETE_DATE IS NULL ")

        '左結合
        sb.AppendLine("    LEFT JOIN ")

        '場所マスタ PLACE_2
        sb.AppendLine("        M_PLACE PLACE_2")
        sb.AppendLine("        ON BASIC.HAISYA_KEIYU_CD_2 = PLACE_2.PLACE_CD ")
        sb.AppendLine("        AND PLACE_2.DELETE_DATE IS NULL ")

        '左結合
        sb.AppendLine("    LEFT JOIN ")

        '場所マスタ PLACE_3
        sb.AppendLine("        M_PLACE PLACE_3")
        sb.AppendLine("        ON BASIC.HAISYA_KEIYU_CD_3 = PLACE_3.PLACE_CD ")
        sb.AppendLine("        AND PLACE_3.DELETE_DATE IS NULL ")

        '左結合
        sb.AppendLine("    LEFT JOIN ")

        '場所マスタ PLACE_4
        sb.AppendLine("        M_PLACE PLACE_4")
        sb.AppendLine("        ON BASIC.HAISYA_KEIYU_CD_4 = PLACE_4.PLACE_CD ")
        sb.AppendLine("        AND PLACE_4.DELETE_DATE IS NULL ")

        '左結合
        sb.AppendLine("    LEFT JOIN ")

        '場所マスタ PLACE_5
        sb.AppendLine("        M_PLACE PLACE_5")
        sb.AppendLine("        ON BASIC.HAISYA_KEIYU_CD_5 = PLACE_5.PLACE_CD ")
        sb.AppendLine("        AND PLACE_5.DELETE_DATE IS NULL ")

        '左結合
        sb.AppendLine("    LEFT JOIN ")

        '場所マスタ PLACE_6
        sb.AppendLine("        M_PLACE PLACE_6")
        sb.AppendLine("        ON BASIC.SYUGO_PLACE_CD_CARRIER = PLACE_6.PLACE_CD ")
        sb.AppendLine("        AND PLACE_6.DELETE_DATE IS NULL ")

        '内部結合
        sb.AppendLine("    INNER JOIN ")

        'コース台帳（バス紐づけ）
        sb.AppendLine("        T_CRS_LEDGER_BUS_HIMODUKE HIMODUKE")
        sb.AppendLine("        ON BASIC.CRS_CD = HIMODUKE.CRS_CD ")
        sb.AppendLine("        AND BASIC.SYUPT_DAY = HIMODUKE.SYUPT_DAY ")
        sb.AppendLine("        AND BASIC.GOUSYA = HIMODUKE.GOUSYA ")
        sb.AppendLine("        AND HIMODUKE.DELETE_DATE = 0 ")
        sb.AppendLine("        AND HIMODUKE.BUS_COMPANY_KAKUTEI_DAY <> 0 ")
        sb.AppendLine("        AND HIMODUKE.HANBAI_START_JI_KAKUTEI_FLG IS NULL ")

        'WHERE句
        sb.AppendLine("    WHERE ")
        sb.AppendLine("        NVL(BASIC.MARU_ZOU_MANAGEMENT_KBN, '*') <> '" & MaruzouKanriKbn.Maruzou & "'")
        sb.AppendLine("        AND NVL(BASIC.SAIKOU_KAKUTEI_KBN, '*') = '" & SaikouKakuteiKbn.Saikou & "'")
        sb.AppendLine("        AND BASIC.DELETE_DAY = 0 ")
        sb.AppendLine("        AND NVL(BASIC.HOUJIN_GAIKYAKU_KBN, '*') = '" & HoujinGaikyakuKbnType.Houjin & "'")
        sb.AppendLine("        AND BASIC.BUS_COMPANY_CD IS NOT NULL")
        sb.AppendLine("        AND (INFO.ENTRY_DAY < HIMODUKE.BUS_COMPANY_KAKUTEI_DAY OR (INFO.ENTRY_DAY = HIMODUKE.BUS_COMPANY_KAKUTEI_DAY AND INFO.ENTRY_TIME < 160000) ) ")
        '出発日FROM
        If Not param.SyuptDayFrom Is Nothing Then
            sb.AppendLine("    AND BASIC.SYUPT_DAY >= ").Append(setSelectParam(param.SyuptDayFrom, crsLedgerBasic.syuptDay))
        End If

        '出発日TO
        If Not param.SyuptDayTo Is Nothing Then
            sb.AppendLine("    AND BASIC.SYUPT_DAY <= ").Append(setSelectParam(param.SyuptDayTo, crsLedgerBasic.syuptDay))
        End If

        'コースコード
        If Not String.IsNullOrEmpty(param.CrsCd) Then
            sb.AppendLine("    AND BASIC.CRS_CD = ").Append(setSelectParam(param.CrsCd, crsLedgerBasic.crsCd))
        End If

        '号車
        If param.Gousya <> 0 Then
            sb.AppendLine("    AND BASIC.GOUSYA = ").Append(setSelectParam(param.Gousya, crsLedgerBasic.gousya))
        End If

        '条件WHERE(※１）
        If param.YoyakuMember = True OrElse param.YoyakuAgent = True Then
            sb.AppendLine("    AND ( 1 = 1 ")

            '◆通知先＝予約者の場合
            If param.YoyakuMember = True Then
                sb.AppendLine("    AND (INFO.AGENT_CD IS NULL OR (SUBSTR(INFO.AGENT_CD, 1, 4) IN ('0001', '0002')) ) ")
            End If
            '◆通知先＝代理店の場合
            If param.YoyakuAgent = True Then
                sb.AppendLine("    AND (INFO.AGENT_CD IS NOT NULL AND (SUBSTR(INFO.AGENT_CD, 1, 4) NOT IN ('0001', '0002')) ) ")
            End If

            sb.AppendLine("    ) ")
        End If

        '条件（WHERE）(※２）
        If param.SendMail = True OrElse param.SendFax = True OrElse param.SendPost = True OrElse param.SendNothing = True Then
            sb.AppendLine("    AND ( 1 <> 1 ")

            '◆通知方法＝メールの場合
            If param.SendMail = True Then
                sb.AppendLine("    OR ( INFO.MAIL_SENDING_KBN IS NOT NULL ")
                '条件WHERE(※２-１）

                sb.AppendLine("    OR ( 1 = 1 ")
                '◆通知方法＝メールの場合
                If param.SendMail = True Then
                    sb.AppendLine("    AND  AGENT.BUS_NOTIFICATION_HOHO = '" & CInt(NotificationHohoBusCompany.Mail) & "'")
                End If
                '◆通知方法＝郵送の場合
                If param.SendPost = True Then
                    sb.AppendLine("    AND  AGENT.BUS_NOTIFICATION_HOHO = '" & CInt(NotificationHohoBusCompany.Yusou) & "'")
                End If
                sb.AppendLine("    ) )")
            End If

            '◆通知方法＝FAXの場合
            If param.SendFax = True Then
                '条件WHERE(※２-１）
                sb.AppendLine("    OR ( ( 1 = 1 ")
                '◆通知方法＝メールの場合
                If param.SendMail = True Then
                    sb.AppendLine("    AND  AGENT.BUS_NOTIFICATION_HOHO = '" & CInt(NotificationHohoBusCompany.Mail) & "'")
                End If
                '◆通知方法＝郵送の場合
                If param.SendPost = True Then
                    sb.AppendLine("    AND  AGENT.BUS_NOTIFICATION_HOHO = '" & CInt(NotificationHohoBusCompany.Yusou) & "'")
                End If
                sb.AppendLine("    ) ")
                sb.AppendLine("        AND AGENT.BUS_NOTIFICATION_HOHO = '" & CInt(NotificationHohoBusCompany.Fax) & "')")
            End If

            '◆通知方法＝郵送の場合
            If param.SendPost = True Then
                sb.AppendLine("    OR ( INFO.MAIL_SENDING_KBN IS NULL ")
                '条件WHERE(※２-１）
                sb.AppendLine("    OR (  1 = 1  ")
                If param.SendMail = True Then
                    sb.AppendLine("    AND  AGENT.BUS_NOTIFICATION_HOHO = '" & CInt(NotificationHohoBusCompany.Mail) & "'")
                End If
                If param.SendPost = True Then
                    sb.AppendLine("    AND  AGENT.BUS_NOTIFICATION_HOHO = '" & CInt(NotificationHohoBusCompany.Yusou) & "'")
                End If
                sb.AppendLine("   ) )")
            End If

            '◆通知方法＝不要の場合
            '※　使用未確定のため実装不要

            sb.AppendLine("    ) ")
        End If

        '条件（WHERE）(※３）
        If param.SendFinish = True OrElse param.NotSend = True Then
            sb.AppendLine("    AND ( 1 <> 1 ")

            '◆通知済の場合
            If param.SendFinish = True Then
                sb.AppendLine("   OR NOTIFICATION.KAKUTEI_NOTIFICATION_DAY IS NOT NULL  ")
            End If

            '◆未通知の場合
            If param.NotSend = True Then
                sb.AppendLine("   OR NOTIFICATION.KAKUTEI_NOTIFICATION_DAY IS NULL  ")
            End If

            sb.AppendLine("    ) ")
        End If

        'ORDER句
        sb.AppendLine("  ORDER BY ")
        sb.AppendLine("      BASIC.SYUPT_DAY ")
        sb.AppendLine("    , BASIC.CRS_CD ")
        sb.AppendLine("    , BASIC.GOUSYA ")
        sb.AppendLine("    , INFO.YOYAKU_KBN ")
        sb.AppendLine("    , INFO.MOTO_YOYAKU_NO ")

        Return MyBase.getDataTable(sb.ToString)
    End Function

    ''' <summary>
    ''' 検索（通知記録照会）
    ''' </summary>
    ''' <param name="param"></param>
    ''' <returns></returns>
    Public Function selectDataTableInquiry(ByVal param As S03_0502DASelectParam) As DataTable
        Dim crsLedgerBasic As New CrsLedgerBasicEntity
        Dim yoyakuInfoBasic As New YoyakuInfoBasicEntity
        'SQL文字列
        Dim sb As New StringBuilder

        'パラメータクリア
        clear()

        'SELECT句
        sb.AppendLine("SELECT ")
        sb.AppendLine("       TO_YYYYMMDD_FC(BASIC.SYUPT_DAY) AS SYUPT_DAY_STR ")
        sb.AppendLine("     , BASIC.SYUPT_DAY AS SYUPT_DAY ")
        sb.AppendLine("     , BASIC.CRS_CD AS CRS_CD ")
        sb.AppendLine("     , '' AS CRS_NAME ")
        sb.AppendLine("     , TO_HHMM_FC(BASIC.SYUPT_TIME_1) AS SYUPT_TIME_STR ")
        sb.AppendLine("     , BASIC.GOUSYA AS GOUSYA ")
        sb.AppendLine("     , BASIC.BUS_COMPANY_CD AS BUS_COMPANY_CD ")
        sb.AppendLine("     , SIIRE.SIIRE_SAKI_NAME AS BUS_COMPANY_NAME ")
        sb.AppendLine("     , INFO.YOYAKU_KBN AS YOYAKU_KBN ")
        sb.AppendLine("     , INFO.YOYAKU_NO AS YOYAKU_NO ")
        sb.AppendLine("     , TO_CHAR(INFO.YOYAKU_KBN || INFO.YOYAKU_NO, 'FM0,999,999,999') AS YOYAKU_NUMBER ")
        sb.AppendLine("     , INFO.SURNAME || INFO.NAME AS YOYAKU_NAME ")
        sb.AppendLine("     , '' AS NINZU ")
        sb.AppendLine("     , INFO.AGENT_CD AS AGENT_CD ")
        sb.AppendLine("     , INFO.AGENT_NM AS AGENT_NM ")
        sb.AppendLine("     , '' AS AGENT_FORMAL_NAME ")
        sb.AppendLine("     , '' AS NOTIFICATION_HOHO ")
        sb.AppendLine("     , '' AS BUS_NOTIFICATION_HOHO ")
        sb.AppendLine("     , CASE TRIM(NOTIFICATION.NOTIFICATION_HOHO) WHEN '1' THEN 'メール' WHEN '2' THEN 'FAX' WHEN '3' THEN '郵送' ELSE '不要' END AS NOTIFICATION_HOHO_RIREKI ")
        sb.AppendLine("     , TO_YYYYMMDD_FC(NOTIFICATION.KAKUTEI_NOTIFICATION_DAY) AS KAKUTEI_NOTIFICATION_DAY ")
        sb.AppendLine("     , '' AS YOYAKU_ENTRY_DAY ")
        sb.AppendLine("     , '' AS MAIL_ADDRESS ")
        sb.AppendLine("     , '' AS MAIL_ADDRESS2 ")
        sb.AppendLine("     , '' AS FAX ")
        sb.AppendLine("     , '' AS YUBIN_NO ")
        sb.AppendLine("     , '' AS YUBIN_NO2 ")
        sb.AppendLine("     , '' AS ADDRESS_1 ")
        sb.AppendLine("     , '' AS ADDRESS_2 ")
        sb.AppendLine("     , '' AS ADDRESS_3 ")
        sb.AppendLine("     , '' AS ADDRESS2_1 ")
        sb.AppendLine("     , '' AS ADDRESS2_2 ")
        sb.AppendLine("     , '' AS ADDRESS2_3 ")
        sb.AppendLine("     , INFO.YOYAKU_UKETUKE_KBN AS YOYAKU_UKETUKE_KBN ")
        sb.AppendLine("     , CASE INFO.YOYAKU_UKETUKE_KBN WHEN '2' THEN 'インターネット' ELSE '' END AS YOYAKU_STATU ")
        sb.AppendLine("     , NOTIFICATION.BUS_COMPANY_CD AS BUS_COMPANY_CD_RIREKI ")
        sb.AppendLine("     , '' AS OTA_KBN")
        sb.AppendLine("     , '' AS SYUPT_JI_CARRIER_KBN")
        sb.AppendLine("     , '' AS SYUGO_PLACE1")
        sb.AppendLine("     , '' AS SYUGO_PLACE2")
        sb.AppendLine("     , '' AS SYUGO_PLACE3")
        sb.AppendLine("     , '' AS SYUGO_PLACE4")
        sb.AppendLine("     , '' AS SYUGO_PLACE5")
        sb.AppendLine("     , '' AS SYUGO_PLACE_CARRIER")
        sb.AppendLine("     , '' AS SYUGO_TIME1")
        sb.AppendLine("     , '' AS SYUGO_TIME2")
        sb.AppendLine("     , '' AS SYUGO_TIME3")
        sb.AppendLine("     , '' AS SYUGO_TIME4")
        sb.AppendLine("     , '' AS SYUGO_TIME5")
        sb.AppendLine("     , '' AS SYUGO_TIME_CARRIER")
        sb.AppendLine("     , '' AS SYUPT_TIME1")
        sb.AppendLine("     , '' AS SYUPT_TIME2")
        sb.AppendLine("     , '' AS SYUPT_TIME3")
        sb.AppendLine("     , '' AS SYUPT_TIME4")
        sb.AppendLine("     , '' AS SYUPT_TIME5")
        sb.AppendLine("     , '' AS SYUPT_TIME_CARRIER")
        sb.AppendLine("     , '' AS ZASEKI")
        sb.AppendLine("     , '' AS JYOSYA_NINZU_1")
        sb.AppendLine("     , '' AS JYOSYA_NINZU_2")
        sb.AppendLine("     , '' AS JYOSYA_NINZU_3")
        sb.AppendLine("     , '' AS JYOSYA_NINZU_4")
        sb.AppendLine("     , '' AS JYOSYA_NINZU_5")
        sb.AppendLine("     , INFO.NYUUKIN_SITUATION_KBN AS NYUUKIN_SITUATION_KBN ")
        sb.AppendLine("     , '' AS CONTACT_FLG ")
        sb.AppendLine("     , CASE WHEN INFO.CANCEL_FLG IS NOT NULL THEN 'キャンセル済' ")
        sb.AppendLine("            WHEN INFO.NYUUKIN_SITUATION_KBN = '0' THEN '未入金' ")
        sb.AppendLine("            WHEN BASIC.BUS_COMPANY_CD <> NOTIFICATION.BUS_COMPANY_CD THEN '変更後未通知' ")
        sb.AppendLine("            WHEN INFO.ENTRY_DAY > HIMODUKE.BUS_COMPANY_KAKUTEI_DAY OR (INFO.ENTRY_DAY = HIMODUKE.BUS_COMPANY_KAKUTEI_DAY AND INFO.ENTRY_TIME > 160000) THEN '確定後受付' ")
        sb.AppendLine("            WHEN HIMODUKE.HANBAI_START_JI_KAKUTEI_FLG IS NOT NULL THEN '販売時確定済' ")
        sb.AppendLine("            ELSE '' ")
        sb.AppendLine("       END AS STATU ")
        sb.AppendLine("     , '' AS TEIKI_KIKAKU_KBN ")
        sb.AppendLine("     , '' AS SAIKOU_KAKUTEI_KBN ")
        sb.AppendLine("     , '' AS CANCEL_FLG ")

        'FROM句
        sb.AppendLine("    FROM ")

        'コース台帳(基本) BASIC
        sb.AppendLine("    T_CRS_LEDGER_BASIC BASIC")

        '内部結合
        sb.AppendLine("    INNER JOIN ")

        '予約情報（基本）INFO
        sb.AppendLine("        T_YOYAKU_INFO_BASIC INFO")
        sb.AppendLine("        ON BASIC.CRS_CD = INFO.CRS_CD ")
        sb.AppendLine("        AND BASIC.SYUPT_DAY = INFO.SYUPT_DAY ")
        sb.AppendLine("        AND BASIC.GOUSYA = INFO.GOUSYA ")
        sb.AppendLine("        AND INFO.YOYAKU_KBN IN ('0','1','2','3','4','5','6','7','8','9') ")

        '左結合
        sb.AppendLine("    LEFT JOIN ")

        'バス会社確定通知出力記録
        sb.AppendLine("        T_BUS_COMPANY_KAKUTEI_NOTIFICATION_OUT NOTIFICATION")
        sb.AppendLine("        ON INFO.YOYAKU_KBN = NOTIFICATION.YOYAKU_KBN ")
        sb.AppendLine("        AND INFO.YOYAKU_NO = NOTIFICATION.YOYAKU_NO ")

        '左結合
        sb.AppendLine("    LEFT JOIN ")

        '仕入先マスタ
        sb.AppendLine("        M_SIIRE_SAKI SIIRE")
        sb.AppendLine("        ON SUBSTR(BASIC.BUS_COMPANY_CD, 1, 4) = SIIRE.SIIRE_SAKI_CD ")
        sb.AppendLine("        AND SUBSTR(BASIC.BUS_COMPANY_CD, LENGTH(BASIC.BUS_COMPANY_CD) - 1, 2) = SIIRE.SIIRE_SAKI_NO ")
        sb.AppendLine("        AND SIIRE.DELETE_DATE IS NULL ")

        '左結合
        sb.AppendLine("    LEFT JOIN ")

        'コース台帳（バス紐づけ）
        sb.AppendLine("        T_CRS_LEDGER_BUS_HIMODUKE HIMODUKE")
        sb.AppendLine("        ON BASIC.CRS_CD = HIMODUKE.CRS_CD ")
        sb.AppendLine("        AND BASIC.SYUPT_DAY = HIMODUKE.SYUPT_DAY ")
        sb.AppendLine("        AND BASIC.GOUSYA = HIMODUKE.GOUSYA ")
        sb.AppendLine("        AND HIMODUKE.DELETE_DATE = 0 ")
        sb.AppendLine("        AND HIMODUKE.BUS_COMPANY_KAKUTEI_DAY <> 0 ")

        'WHERE句
        sb.AppendLine("    WHERE ")
        sb.AppendLine("        NVL(BASIC.MARU_ZOU_MANAGEMENT_KBN, '*') <> '" & MaruzouKanriKbn.Maruzou & "'")
        sb.AppendLine("        AND NVL(BASIC.SAIKOU_KAKUTEI_KBN, '*') = '" & SaikouKakuteiKbn.Saikou & "'")
        sb.AppendLine("        AND BASIC.DELETE_DAY = 0 ")
        sb.AppendLine("        AND NVL(BASIC.HOUJIN_GAIKYAKU_KBN, '*') = '" & HoujinGaikyakuKbnType.Houjin & "'")
        sb.AppendLine("        AND BASIC.BUS_COMPANY_CD IS NOT NULL")
        '出発日FROM
        If Not param.SyuptDayFrom Is Nothing Then
            sb.AppendLine("    AND BASIC.SYUPT_DAY >= ").Append(setSelectParam(param.SyuptDayFrom, crsLedgerBasic.syuptDay))
        End If

        '出発日TO
        If Not param.SyuptDayTo Is Nothing Then
            sb.AppendLine("    AND BASIC.SYUPT_DAY <= ").Append(setSelectParam(param.SyuptDayTo, crsLedgerBasic.syuptDay))
        End If

        'コースコード
        If Not String.IsNullOrEmpty(param.CrsCd) Then
            sb.AppendLine("    AND BASIC.CRS_CD = ").Append(setSelectParam(param.CrsCd, crsLedgerBasic.crsCd))
        End If

        '予約区分
        If Not param.YoyakuKbn Is Nothing Then
            sb.AppendLine("     AND INFO.YOYAKU_KBN = ").Append(setSelectParam(param.YoyakuKbn, yoyakuInfoBasic.yoyakuKbn))
        End If

        '予約ＮＯ
        If param.YoyakuNo <> 0 Then
            sb.AppendLine("     AND INFO.YOYAKU_NO = ").Append(setSelectParam(param.YoyakuNo, yoyakuInfoBasic.yoyakuNo))
        End If

        '号車
        If param.Gousya <> 0 Then
            sb.AppendLine("    AND BASIC.GOUSYA = ").Append(setSelectParam(param.Gousya, crsLedgerBasic.gousya))
        End If

        '条件WHERE(※１）
        If param.YoyakuMember = True OrElse param.YoyakuAgent = True Then
            sb.AppendLine("    AND ( 1 = 1 ")

            '◆通知先＝予約者の場合
            If param.YoyakuMember = True Then
                sb.AppendLine("  AND ( INFO.AGENT_CD IS NULL OR (SUBSTR(INFO.AGENT_CD,1,4) IN ('0001','0002')) ) ")
            End If
            '◆通知先＝代理店の場合
            If param.YoyakuAgent Then
                sb.AppendLine("    AND ( INFO.AGENT_CD IS NOT NULL AND (SUBSTR(INFO.AGENT_CD,1,4) NOT IN ('0001','0002')) ) ")
            End If

            sb.AppendLine("    ) ")
        End If

        '条件（WHERE）(※２）
        If param.SendMail = True OrElse param.SendFax = True OrElse param.SendPost = True Then
            sb.AppendLine("    AND ( 1 <> 1 ")

            '◆通知方法＝メールの場合
            If param.SendMail = True Then
                sb.AppendLine("     OR NOTIFICATION.NOTIFICATION_HOHO = '" & CInt(NotificationHohoBusCompany.Mail) & "'")
            End If
            '◆通知方法＝FAXの場合
            If param.SendFax = True Then
                sb.AppendLine("     OR NOTIFICATION.NOTIFICATION_HOHO = '" & CInt(NotificationHohoBusCompany.Fax) & "'")
            End If
            '◆通知方法＝郵送の場合
            If param.SendPost = True Then
                sb.AppendLine("     OR NOTIFICATION.NOTIFICATION_HOHO = '" & CInt(NotificationHohoBusCompany.Yusou) & "'")
            End If

            sb.AppendLine("    ) ")
        End If

        'ORDER句
        sb.AppendLine("  ORDER BY ")
        sb.AppendLine("      BASIC.SYUPT_DAY ")
        sb.AppendLine("    , BASIC.CRS_CD ")
        sb.AppendLine("    , BASIC.GOUSYA ")
        sb.AppendLine("    , INFO.YOYAKU_KBN ")
        sb.AppendLine("    , INFO.MOTO_YOYAKU_NO ")

        Return MyBase.getDataTable(sb.ToString)
    End Function

    ''' <summary>
    ''' 検索（予約番号入力時）
    ''' </summary>
    ''' <param name="param"></param>
    ''' <returns></returns>
    Public Function selectDataTableReservation(ByVal param As S03_0502DASelectParam) As DataTable
        Dim crsLedgerBasic As New CrsLedgerBasicEntity
        Dim yoyakuInfoBasic As New YoyakuInfoBasicEntity
        Dim yoyakuInfoCrsChargeChargeKbn As New YoyakuInfoCrsChargeChargeKbnEntity
        Dim siireSaki As New SiireSakiEntity
        Dim crsLedgerBusHimoduke As New TCrsLedgerBusHimodukeEntity

        'SQL文字列
        Dim sb As New StringBuilder

        'パラメータクリア
        clear()

        'SELECT句
        sb.AppendLine("SELECT ")
        sb.AppendLine("       TO_YYYYMMDD_FC(BASIC.SYUPT_DAY) AS SYUPT_DAY_STR ")
        sb.AppendLine("     , BASIC.SYUPT_DAY AS SYUPT_DAY ")
        sb.AppendLine("     , BASIC.CRS_CD AS CRS_CD ")
        sb.AppendLine("     , BASIC.CRS_NAME AS CRS_NAME ")
        sb.AppendLine("     , TO_HHMM_FC(BASIC.SYUPT_TIME_1) AS SYUPT_TIME_STR ")
        sb.AppendLine("     , BASIC.GOUSYA AS GOUSYA ")
        sb.AppendLine("     , BASIC.BUS_COMPANY_CD AS BUS_COMPANY_CD ")
        sb.AppendLine("     , SIIRE.SIIRE_SAKI_NAME AS BUS_COMPANY_NAME ")
        sb.AppendLine("     , INFO.YOYAKU_KBN AS YOYAKU_KBN ")
        sb.AppendLine("     , INFO.YOYAKU_NO AS YOYAKU_NO ")
        sb.AppendLine("     , TO_CHAR(INFO.YOYAKU_KBN || INFO.YOYAKU_NO, 'FM0,999,999,999') AS YOYAKU_NUMBER ")
        sb.AppendLine("     , INFO.SURNAME || INFO.NAME AS YOYAKU_NAME ")
        sb.AppendLine("     , CHARGE.NINZU AS NINZU ")
        sb.AppendLine("     , INFO.AGENT_CD AS AGENT_CD ")
        sb.AppendLine("     , INFO.AGENT_NM AS AGENT_NM ")
        sb.AppendLine("     , AGENT.AGENT_FORMAL_NAME AS AGENT_FORMAL_NAME ")
        sb.AppendLine("     , CASE WHEN INFO.MAIL_SENDING_KBN = 'Y' THEN '1' ELSE '3' END AS NOTIFICATION_HOHO ")
        sb.AppendLine("     , TRIM(AGENT.BUS_NOTIFICATION_HOHO) AS BUS_NOTIFICATION_HOHO ")
        sb.AppendLine("     , '' AS NOTIFICATION_HOHO_RIREKI ")
        sb.AppendLine("     , TO_YYYYMMDD_FC(NOTIFICATION.KAKUTEI_NOTIFICATION_DAY) AS KAKUTEI_NOTIFICATION_DAY ")
        sb.AppendLine("     , TO_CHAR(INFO.SYSTEM_ENTRY_DAY, 'YYYY/MM/DD') AS YOYAKU_ENTRY_DAY ")
        sb.AppendLine("     , INFO.MAIL_ADDRESS AS MAIL_ADDRESS ")
        sb.AppendLine("     , AGENT.MAIL AS MAIL_ADDRESS2 ")
        sb.AppendLine("     , AGENT.FAX AS FAX ")
        sb.AppendLine("     , INFO.YUBIN_NO AS YUBIN_NO ")
        sb.AppendLine("     , AGENT.YUBIN_NO AS YUBIN_NO2 ")
        sb.AppendLine("     , INFO.ADDRESS_1 AS ADDRESS_1 ")
        sb.AppendLine("     , INFO.ADDRESS_2 AS ADDRESS_2 ")
        sb.AppendLine("     , INFO.ADDRESS_3 AS ADDRESS_3 ")
        sb.AppendLine("     , AGENT.ADDRESS_1 AS ADDRESS2_1 ")
        sb.AppendLine("     , AGENT.ADDRESS_2 AS ADDRESS2_2 ")
        sb.AppendLine("     , AGENT.ADDRESS_3 AS ADDRESS2_3 ")
        sb.AppendLine("     , INFO.YOYAKU_UKETUKE_KBN AS YOYAKU_UKETUKE_KBN ")
        sb.AppendLine("     , CASE INFO.YOYAKU_UKETUKE_KBN WHEN '2' THEN 'インターネット' ELSE '' END AS YOYAKU_STATU ")
        sb.AppendLine("     , NOTIFICATION.BUS_COMPANY_CD AS BUS_COMPANY_CD_RIREKI ")
        sb.AppendLine("     , AGENT.OTA_KBN AS OTA_KBN")
        sb.AppendLine("     , BASIC.SYUPT_JI_CARRIER_KBN AS SYUPT_JI_CARRIER_KBN")
        sb.AppendLine("     , PLACE_1.PLACE_NAME_1 AS SYUGO_PLACE1")
        sb.AppendLine("     , PLACE_2.PLACE_NAME_1 AS SYUGO_PLACE2")
        sb.AppendLine("     , PLACE_3.PLACE_NAME_1 AS SYUGO_PLACE3")
        sb.AppendLine("     , PLACE_4.PLACE_NAME_1 AS SYUGO_PLACE4")
        sb.AppendLine("     , PLACE_5.PLACE_NAME_1 AS SYUGO_PLACE5")
        sb.AppendLine("     , PLACE_6.PLACE_NAME_1 AS SYUGO_PLACE_CARRIER")
        sb.AppendLine("     , TO_HHMM_FC(BASIC.SYUGO_TIME_1) AS SYUGO_TIME1")
        sb.AppendLine("     , TO_HHMM_FC(BASIC.SYUGO_TIME_2) AS SYUGO_TIME2")
        sb.AppendLine("     , TO_HHMM_FC(BASIC.SYUGO_TIME_3) AS SYUGO_TIME3")
        sb.AppendLine("     , TO_HHMM_FC(BASIC.SYUGO_TIME_4) AS SYUGO_TIME4")
        sb.AppendLine("     , TO_HHMM_FC(BASIC.SYUGO_TIME_5) AS SYUGO_TIME5")
        sb.AppendLine("     , TO_HHMM_FC(BASIC.SYUGO_TIME_CARRIER) AS SYUGO_TIME_CARRIER")
        sb.AppendLine("     , TO_HHMM_FC(BASIC.SYUPT_TIME_1) AS SYUPT_TIME1")
        sb.AppendLine("     , TO_HHMM_FC(BASIC.SYUPT_TIME_2) AS SYUPT_TIME2")
        sb.AppendLine("     , TO_HHMM_FC(BASIC.SYUPT_TIME_3) AS SYUPT_TIME3")
        sb.AppendLine("     , TO_HHMM_FC(BASIC.SYUPT_TIME_4) AS SYUPT_TIME4")
        sb.AppendLine("     , TO_HHMM_FC(BASIC.SYUPT_TIME_5) AS SYUPT_TIME5")
        sb.AppendLine("     , TO_HHMM_FC(BASIC.SYUPT_TIME_CARRIER) AS SYUPT_TIME_CARRIER")
        sb.AppendLine("     , INFO.ZASEKI AS ZASEKI")
        sb.AppendLine("     , INFO.JYOSYA_NINZU_1 AS JYOSYA_NINZU_1")
        sb.AppendLine("     , INFO.JYOSYA_NINZU_2 AS JYOSYA_NINZU_2")
        sb.AppendLine("     , INFO.JYOSYA_NINZU_3 AS JYOSYA_NINZU_3")
        sb.AppendLine("     , INFO.JYOSYA_NINZU_4 AS JYOSYA_NINZU_4")
        sb.AppendLine("     , INFO.JYOSYA_NINZU_5 AS JYOSYA_NINZU_5")
        sb.AppendLine("     , INFO.NYUUKIN_SITUATION_KBN AS NYUUKIN_SITUATION_KBN ")
        sb.AppendLine("     , '' AS CONTACT_FLG ")
        sb.AppendLine("     , '' AS STATU ")
        sb.AppendLine("     , BASIC.TEIKI_KIKAKU_KBN AS TEIKI_KIKAKU_KBN ")
        sb.AppendLine("     , BASIC.SAIKOU_KAKUTEI_KBN AS SAIKOU_KAKUTEI_KBN ")
        sb.AppendLine("     , INFO.CANCEL_FLG AS CANCEL_FLG ")

        'FROM句
        sb.AppendLine("    FROM ")

        '予約情報（基本）INFO
        sb.AppendLine("    T_YOYAKU_INFO_BASIC INFO")

        '内部結合
        sb.AppendLine("    INNER JOIN ")

        '予約情報（コース料金_料金区分） CHARGE
        sb.AppendLine("        ( ")
        sb.AppendLine("        SELECT")
        sb.AppendLine("              YOYAKU_KBN ")
        sb.AppendLine("            , YOYAKU_NO ")
        sb.AppendLine("            , SUM(CHARGE_APPLICATION_NINZU) AS NINZU ")
        sb.AppendLine("        FROM ")
        sb.AppendLine("            T_YOYAKU_INFO_CRS_CHARGE_CHARGE_KBN ")
        sb.AppendLine("        GROUP BY ")
        sb.AppendLine("              YOYAKU_KBN ")
        sb.AppendLine("            , YOYAKU_NO ")
        sb.AppendLine("        ) CHARGE ")
        sb.AppendLine("        ON INFO.YOYAKU_KBN = CHARGE.YOYAKU_KBN ")
        sb.AppendLine("        AND INFO.YOYAKU_NO = CHARGE.YOYAKU_NO ")

        '左結合
        sb.AppendLine("    LEFT JOIN ")

        '代理店マスタ
        sb.AppendLine("        M_AGENT AGENT")
        sb.AppendLine("        ON INFO.AGENT_CD = AGENT.AGENT_CD ")
        sb.AppendLine("        AND AGENT.DELETE_DATE IS NULL ")

        '左結合
        sb.AppendLine("    LEFT JOIN ")

        'コース台帳(基本)
        sb.AppendLine("        T_CRS_LEDGER_BASIC BASIC")
        sb.AppendLine("        ON INFO.CRS_CD = BASIC.CRS_CD ")
        sb.AppendLine("        AND INFO.SYUPT_DAY = BASIC.SYUPT_DAY ")
        sb.AppendLine("        AND INFO.GOUSYA = BASIC.GOUSYA ")
        sb.AppendLine("        AND NVL(BASIC.HOUJIN_GAIKYAKU_KBN, '*') = '" & HoujinGaikyakuKbnType.Houjin & "'")
        sb.AppendLine("        AND NVL(BASIC.MARU_ZOU_MANAGEMENT_KBN, '*') <> '" & MaruzouKanriKbn.Maruzou & "'")
        sb.AppendLine("        AND BASIC.DELETE_DAY = 0")

        '左結合
        sb.AppendLine("    LEFT JOIN ")

        '仕入先マスタ
        sb.AppendLine("        M_SIIRE_SAKI SIIRE")
        sb.AppendLine("        ON SUBSTR(BASIC.BUS_COMPANY_CD, 1, 4) = SIIRE.SIIRE_SAKI_CD ")
        sb.AppendLine("        AND SUBSTR(BASIC.BUS_COMPANY_CD, LENGTH(BASIC.BUS_COMPANY_CD) - 1, 2) = SIIRE.SIIRE_SAKI_NO ")
        sb.AppendLine("        AND SIIRE.DELETE_DATE IS NULL ")

        '左結合
        sb.AppendLine("    LEFT JOIN ")

        'バス会社確定通知出力記録
        sb.AppendLine("        T_BUS_COMPANY_KAKUTEI_NOTIFICATION_OUT NOTIFICATION")
        sb.AppendLine("        ON BASIC.CRS_CD = NOTIFICATION.CRS_CD ")
        sb.AppendLine("        AND BASIC.SYUPT_DAY = NOTIFICATION.SYUPT_DAY ")
        sb.AppendLine("        AND BASIC.GOUSYA = NOTIFICATION.GOUSYA ")
        sb.AppendLine("        AND INFO.YOYAKU_KBN = NOTIFICATION.YOYAKU_KBN ")
        sb.AppendLine("        AND INFO.YOYAKU_NO = NOTIFICATION.YOYAKU_NO ")

        '左結合
        sb.AppendLine("    LEFT JOIN ")

        '場所マスタ PLACE_1
        sb.AppendLine("        M_PLACE PLACE_1")
        sb.AppendLine("        ON BASIC.HAISYA_KEIYU_CD_1 = PLACE_1.PLACE_CD ")
        sb.AppendLine("        AND PLACE_1.DELETE_DATE IS NULL ")

        '左結合
        sb.AppendLine("    LEFT JOIN ")

        '場所マスタ PLACE_2
        sb.AppendLine("        M_PLACE PLACE_2")
        sb.AppendLine("        ON BASIC.HAISYA_KEIYU_CD_2 = PLACE_2.PLACE_CD ")
        sb.AppendLine("        AND PLACE_2.DELETE_DATE IS NULL ")

        '左結合
        sb.AppendLine("    LEFT JOIN ")

        '場所マスタ PLACE_3
        sb.AppendLine("        M_PLACE PLACE_3")
        sb.AppendLine("        ON BASIC.HAISYA_KEIYU_CD_3 = PLACE_3.PLACE_CD ")
        sb.AppendLine("        AND PLACE_3.DELETE_DATE IS NULL ")

        '左結合
        sb.AppendLine("    LEFT JOIN ")

        '場所マスタ PLACE_4
        sb.AppendLine("        M_PLACE PLACE_4")
        sb.AppendLine("        ON BASIC.HAISYA_KEIYU_CD_4 = PLACE_4.PLACE_CD ")
        sb.AppendLine("        AND PLACE_4.DELETE_DATE IS NULL ")

        '左結合
        sb.AppendLine("    LEFT JOIN ")

        '場所マスタ PLACE_5
        sb.AppendLine("        M_PLACE PLACE_5")
        sb.AppendLine("        ON BASIC.HAISYA_KEIYU_CD_5 = PLACE_5.PLACE_CD ")
        sb.AppendLine("        AND PLACE_5.DELETE_DATE IS NULL ")

        '左結合
        sb.AppendLine("    LEFT JOIN ")

        '場所マスタ PLACE_6
        sb.AppendLine("        M_PLACE PLACE_6")
        sb.AppendLine("        ON BASIC.SYUGO_PLACE_CD_CARRIER = PLACE_6.PLACE_CD ")
        sb.AppendLine("        AND PLACE_6.DELETE_DATE IS NULL ")

        '左結合
        sb.AppendLine("    LEFT JOIN ")

        'コース台帳（バス紐づけ）
        sb.AppendLine("        T_CRS_LEDGER_BUS_HIMODUKE HIMODUKE")
        sb.AppendLine("        ON BASIC.CRS_CD = HIMODUKE.CRS_CD ")
        sb.AppendLine("        AND BASIC.SYUPT_DAY = HIMODUKE.SYUPT_DAY ")
        sb.AppendLine("        AND BASIC.GOUSYA = HIMODUKE.GOUSYA ")
        sb.AppendLine("        AND HIMODUKE.DELETE_DATE = 0 ")
        sb.AppendLine("        AND HIMODUKE.BUS_COMPANY_KAKUTEI_DAY <> 0 ")
        sb.AppendLine("        AND HIMODUKE.HANBAI_START_JI_KAKUTEI_FLG IS NULL ")

        'WHERE句
        sb.AppendLine("    WHERE ")
        sb.AppendLine("        INFO.YOYAKU_KBN IN ('0','1','2','3','4','5','6','7','8','9') ")
        '予約区分
        sb.AppendLine("    AND INFO.YOYAKU_KBN = ").Append(setSelectParam(param.YoyakuKbn, yoyakuInfoBasic.yoyakuKbn))
        '予約No
        sb.AppendLine("    AND INFO.YOYAKU_NO = ").Append(setSelectParam(param.YoyakuNo, yoyakuInfoBasic.yoyakuNo))

        'ORDER句
        sb.AppendLine("  ORDER BY ")
        sb.AppendLine("      BASIC.SYUPT_DAY ")
        sb.AppendLine("    , BASIC.CRS_CD ")
        sb.AppendLine("    , BASIC.GOUSYA ")
        sb.AppendLine("    , INFO.YOYAKU_KBN ")
        sb.AppendLine("    , INFO.MOTO_YOYAKU_NO ")

        Return MyBase.getDataTable(sb.ToString)
    End Function

    ''' <summary>
    ''' 通知内容取得
    ''' </summary>
    ''' <param name="param"></param>
    ''' <returns></returns>
    Public Function selectDataDataTableNotificationContent(ByVal param As S03_0502DAGetNotifiContentParam) As DataTable
        Dim code As New MCodeEntity
        'SQL文字列
        Dim sb As New StringBuilder
        'パラメータクリア
        clear()
        'SELECT句
        sb.AppendLine(" SELECT ")
        sb.AppendLine("   CODE_VALUE AS CONTACT_KIND ")
        sb.AppendLine("   , NAIYO_1 AS NAIYO_1")
        'FROM句
        sb.AppendLine(" FROM ")
        sb.AppendLine("   M_CODE ")
        'WHERE句
        sb.AppendLine("WHERE ")
        sb.AppendLine("   DELETE_DATE IS NULL")
        'コード分類
        sb.AppendLine("   AND CODE_BUNRUI = ").Append(setSelectParam(param.CodeBunrui, code.CodeBunrui))
        Return MyBase.getDataTable(sb.ToString)
    End Function

    Private Sub clear()
        MyBase.paramClear()
        ParamNum = 0
    End Sub
    Public Function setSelectParam(ByVal value As Object, ByVal ent As IEntityKoumokuType) As String
        Return setParamEx(value, ent, True)
    End Function
    Public Function setUpdateParam(ByVal value As Object, ByVal ent As IEntityKoumokuType) As String
        Return setParamEx(value, ent, True)
    End Function
    Private Function setParamEx(ByVal value As Object, ByVal ent As IEntityKoumokuType, ByVal selFlg As Boolean) As String
        ParamNum += 1
        If selFlg = True AndAlso TypeOf ent Is EntityKoumoku_MojiType Then
            Return MyBase.setParam(ParamNum.ToString, value, ent.DBType)
        Else
            Return MyBase.setParam(ParamNum.ToString, value, ent.DBType, ent.IntegerBu, ent.DecimalBu)
        End If
    End Function
#End Region

#Region "DELETE/INSERT 処理 "

    ''' <summary>
    ''' 削除/登録 (バス会社確定通知出力記録)
    ''' </summary>
    ''' <param name="lsParamDelete"></param>
    ''' <param name="lsParamInsert"></param>
    ''' <returns></returns>
    Public Function executeDeleteInsertNotificationOut(ByVal lsParamDelete As List(Of S03_0502DADeleteParam), ByVal lsParamInsert As List(Of S03_0502DAInsertParam)) As Integer
        Dim returnValue As Integer = 0
        Dim sqlStringDelete As String = String.Empty
        Dim sqlStringInsert As String = String.Empty
        Dim con As OracleConnection = Nothing

        'コネクション開始
        con = openCon()
        Try
            Dim sizeData As Integer = lsParamDelete.Count
            For row As Integer = 0 To sizeData - 1
                'SQL文字列(削除)
                sqlStringDelete = getDeleteNotificationOutQuery(lsParamDelete(row))
                returnValue += execNonQuery(con, sqlStringDelete)

                'SQL文字列(登録)
                sqlStringInsert = getInsertNotificationOutQuery(lsParamInsert(row))
                returnValue += execNonQuery(con, sqlStringInsert)
            Next

        Catch ex As Exception
            Throw
        Finally
            If con IsNot Nothing Then
                If con.State = ConnectionState.Open Then
                    con.Close()
                End If
                If con IsNot Nothing Then
                    con.Dispose()
                End If
            End If
        End Try
        Return returnValue

    End Function
#End Region

#Region "DELETE処理 "

    ''' <summary>
    ''' 削除（バス会社確定通知出力記録）
    ''' </summary>
    ''' <param name="param"></param>
    ''' <returns></returns>
    Private Function getDeleteNotificationOutQuery(ByVal param As S03_0502DADeleteParam) As String
        Dim busComKakuteiNotifiEtt As New TBusCompanyKakuteiNotificationOutEntity

        'SQL文字列
        Dim sb As New StringBuilder

        'パラメータクリア
        clear()
        sb.AppendLine("DELETE FROM T_BUS_COMPANY_KAKUTEI_NOTIFICATION_OUT ")
        'WHERE
        sb.AppendLine("WHERE ")
        '予約ＮＯ
        sb.AppendLine("  YOYAKU_KBN = ").Append(setUpdateParam(param.YoyakuKbn, busComKakuteiNotifiEtt.YoyakuKbn))
        '仕入先コード
        sb.AppendLine("  AND ")
        sb.AppendLine("  YOYAKU_NO = ").Append(setUpdateParam(param.YoyakuNo, busComKakuteiNotifiEtt.YoyakuNo))
        Return sb.ToString
    End Function
#End Region

#Region "INSERT処理 "

    ''' <summary>
    ''' 登録（バス会社確定通知出力記録）
    ''' </summary>
    ''' <param name="param"></param>
    ''' <returns></returns>
    Private Function getInsertNotificationOutQuery(ByVal param As S03_0502DAInsertParam) As String
        Dim busComKakuteiNotifiEtt As New TBusCompanyKakuteiNotificationOutEntity

        'SQL文字列
        Dim sb As New StringBuilder

        sb.AppendLine("  INSERT INTO T_BUS_COMPANY_KAKUTEI_NOTIFICATION_OUT (")
        sb.AppendLine("        AGENT_CD ")
        sb.AppendLine("      , AGENT_NM ")
        sb.AppendLine("      , BUS_COMPANY_CD ")
        sb.AppendLine("      , CRS_CD ")
        sb.AppendLine("      , GOUSYA ")
        sb.AppendLine("      , KAKUTEI_NOTIFICATION_DAY ")
        sb.AppendLine("      , NOTIFICATION_HOHO ")
        sb.AppendLine("      , SYUPT_DAY ")
        sb.AppendLine("      , YEAR ")
        sb.AppendLine("      , YOYAKU_KBN ")
        sb.AppendLine("      , YOYAKU_NO ")
        sb.AppendLine("      , SYSTEM_ENTRY_PGMID ")
        sb.AppendLine("      , SYSTEM_ENTRY_PERSON_CD ")
        sb.AppendLine("      , SYSTEM_ENTRY_DAY ")
        sb.AppendLine("      , SYSTEM_UPDATE_PGMID ")
        sb.AppendLine("      , SYSTEM_UPDATE_PERSON_CD ")
        sb.AppendLine("      , SYSTEM_UPDATE_DAY)")
        'INSERT 登録
        sb.AppendLine("  VALUES")
        '業者コード
        sb.AppendLine("  (" & setUpdateParam(param.AgentCd, busComKakuteiNotifiEtt.AgentCd))
        '業者名称
        sb.AppendLine("  ," & setUpdateParam(param.AgentNm, busComKakuteiNotifiEtt.AgentNm))
        'バス会社コード
        sb.AppendLine("  ," & setUpdateParam(param.BusCompanyCd, busComKakuteiNotifiEtt.BusCompanyCd))
        'コースコード
        sb.AppendLine("  ," & setUpdateParam(param.CrsCd, busComKakuteiNotifiEtt.CrsCd))
        '号車
        sb.AppendLine("  ," & setUpdateParam(param.Gousya, busComKakuteiNotifiEtt.Gousya))
        '確定通知日
        sb.AppendLine("  ," & setUpdateParam(param.KakuteiNotificationDay, busComKakuteiNotifiEtt.KakuteiNotificationDay))
        '通知方法
        sb.AppendLine("  ," & setUpdateParam(param.NotificationHoho, busComKakuteiNotifiEtt.NotificationHoho))
        '出発日
        sb.AppendLine("  ," & setUpdateParam(param.SyuptDay, busComKakuteiNotifiEtt.SyuptDay))
        '年
        sb.AppendLine("  ," & setUpdateParam(param.Year, busComKakuteiNotifiEtt.Year))
        '予約区分
        sb.AppendLine("  ," & setUpdateParam(param.YoyakuKbn, busComKakuteiNotifiEtt.YoyakuKbn))
        '予約ＮＯ
        sb.AppendLine("  ," & setUpdateParam(param.YoyakuNo, busComKakuteiNotifiEtt.YoyakuNo))
        'システム登録ＰＧＭＩＤ
        sb.AppendLine("  ," & setUpdateParam(param.SystemEntryPgmid, busComKakuteiNotifiEtt.SystemEntryPgmid))
        'システム登録者コード
        sb.AppendLine("  ," & setUpdateParam(param.SystemEntryPersonCd, busComKakuteiNotifiEtt.SystemEntryPersonCd))
        'システム登録日
        sb.AppendLine("  ," & setUpdateParam(param.SystemEntryDay, busComKakuteiNotifiEtt.SystemEntryDay))
        'システム更新ＰＧＭＩＤ
        sb.AppendLine("  ," & setUpdateParam(param.SystemUpdatePgmid, busComKakuteiNotifiEtt.SystemUpdatePgmid))
        'システム更新者コード
        sb.AppendLine("  ," & setUpdateParam(param.SystemUpdatePersonCd, busComKakuteiNotifiEtt.SystemUpdatePersonCd))
        'システム更新日
        sb.AppendLine("  ," & setUpdateParam(param.SystemUpdateDay, busComKakuteiNotifiEtt.SystemUpdateDay))
        sb.AppendLine("  )")
        Return sb.ToString()
    End Function
#End Region

#Region " パラメータ "
    Public Class S03_0502DASelectParam
        ''' <summary>
        ''' 出発日FROM
        ''' </summary>
        Public Property SyuptDayFrom As Integer?
        ''' <summary>
        ''' 出発日TO
        ''' </summary>
        Public Property SyuptDayTo As Integer?
        ''' <summary>
        ''' コースコード
        ''' </summary>
        Public Property CrsCd As String
        ''' <summary>
        ''' 予約区分
        ''' </summary>
        Public Property YoyakuKbn As String
        ''' <summary>
        ''' 予約No
        ''' </summary>
        Public Property YoyakuNo As Integer
        ''' <summary>
        ''' 号車
        ''' </summary>
        Public Property Gousya As Integer
        ''' <summary>
        ''' 予約者
        ''' </summary>
        Public Property YoyakuMember As Boolean
        ''' <summary>
        ''' 代理店
        ''' </summary>
        Public Property YoyakuAgent As Boolean
        ''' <summary>
        ''' メール
        ''' </summary>
        Public Property SendMail As Boolean
        ''' <summary>
        ''' FAX
        ''' </summary>
        Public Property SendFax As Boolean
        ''' <summary>
        ''' 郵送
        ''' </summary>
        Public Property SendPost As Boolean
        ''' <summary>
        ''' 不要
        ''' </summary>
        Public Property SendNothing As Boolean
        ''' <summary>
        ''' 通知済
        ''' </summary>
        Public Property SendFinish As Boolean
        ''' <summary>
        ''' 未通知
        ''' </summary>
        Public Property NotSend As Boolean
    End Class
    Public Class S03_0502DADeleteParam
        ''' <summary>
        ''' コースコード
        ''' </summary>
        Public Property CrsCd As String
        ''' <summary>
        ''' 号車
        ''' </summary>
        Public Property Gousya As Integer
        ''' <summary>
        ''' 出発日
        ''' </summary>
        Public Property SyuptDay As Integer
        ''' <summary>
        ''' 予約区分
        ''' </summary>
        Public Property YoyakuKbn As String
        ''' <summary>
        ''' 予約No
        ''' </summary>
        Public Property YoyakuNo As Integer
    End Class
    Public Class S03_0502DAInsertParam
        ''' <summary>
        ''' 業者コード
        ''' </summary>
        Public Property AgentCd As String
        ''' <summary>
        ''' 業者名称
        ''' </summary>
        Public Property AgentNm As String
        ''' <summary>
        ''' バス会社コード
        ''' </summary>
        Public Property BusCompanyCd As String
        ''' <summary>
        ''' コースコード
        ''' </summary>
        Public Property CrsCd As String
        ''' <summary>
        ''' 号車
        ''' </summary>
        Public Property Gousya As Integer?
        ''' <summary>
        ''' 確定通知日
        ''' </summary>
        Public Property KakuteiNotificationDay As Integer?
        ''' <summary>
        ''' 通知方法
        ''' </summary>
        Public Property NotificationHoho As String
        ''' <summary>
        ''' 出発日
        ''' </summary>
        Public Property SyuptDay As Integer?
        ''' <summary>
        ''' 年
        ''' </summary>
        Public Property Year As Integer?
        ''' <summary>
        ''' 予約区分
        ''' </summary>
        Public Property YoyakuKbn As String
        ''' <summary>
        ''' 予約ＮＯ
        ''' </summary>
        Public Property YoyakuNo As Integer
        ''' <summary>
        ''' システム登録ＰＧＭＩＤ
        ''' </summary>
        Public Property SystemEntryPgmid As String
        ''' <summary>
        ''' システム登録者コード
        ''' </summary>
        Public Property SystemEntryPersonCd As String
        ''' <summary>
        ''' システム登録日
        ''' </summary>
        Public Property SystemEntryDay As Date
        ''' <summary>
        ''' システム更新ＰＧＭＩＤ
        ''' </summary>
        Public Property SystemUpdatePgmid As String
        ''' <summary>
        ''' システム更新者コード
        ''' </summary>
        Public Property SystemUpdatePersonCd As String
        ''' <summary>
        ''' システム更新日
        ''' </summary>
        Public Property SystemUpdateDay As Date
    End Class
    Public Class S03_0502DAGetNotifiContentParam
        ''' <summary>
        ''' コード分類
        ''' </summary>
        Public Property CodeBunrui As String
    End Class
#End Region

End Class