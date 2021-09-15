using System;
using System.Collections.Generic;
using System.Text;

/// <summary>
/// バス会社確定通知/記録照会のDAクラス
/// </summary>
public partial class S03_0502DA : DataAccessorBase
{

    #region  定数／変数 
    private int ParamNum = 0;
    #endregion

    #region  SELECT処理 
    /// <summary>
    /// 検索（確定通知）
    /// </summary>
    /// <param name="param"></param>
    /// <returns>DataTable</returns>
    public DataTable selectDataTableConfirmNotice(S03_0502DASelectParam param)
    {
        var crsLedgerBasic = new CrsLedgerBasicEntity();
        // SQL文字列
        var sb = new StringBuilder();

        // パラメータクリア
        clear();

        // SELECT句
        sb.AppendLine("SELECT ");
        sb.AppendLine("       TO_YYYYMMDD_FC(BASIC.SYUPT_DAY) AS SYUPT_DAY_STR ");
        sb.AppendLine("     , BASIC.SYUPT_DAY AS SYUPT_DAY ");
        sb.AppendLine("     , BASIC.CRS_CD AS CRS_CD ");
        sb.AppendLine("     , BASIC.CRS_NAME AS CRS_NAME ");
        sb.AppendLine("     , TO_HHMM_FC(BASIC.SYUPT_TIME_1) AS SYUPT_TIME_STR ");
        sb.AppendLine("     , BASIC.GOUSYA AS GOUSYA ");
        sb.AppendLine("     , BASIC.BUS_COMPANY_CD AS BUS_COMPANY_CD ");
        sb.AppendLine("     , SIIRE.SIIRE_SAKI_NAME AS BUS_COMPANY_NAME ");
        sb.AppendLine("     , INFO.YOYAKU_KBN AS YOYAKU_KBN ");
        sb.AppendLine("     , INFO.YOYAKU_NO AS YOYAKU_NO ");
        sb.AppendLine("     , TO_CHAR(INFO.YOYAKU_KBN || INFO.YOYAKU_NO, 'FM0,999,999,999') AS YOYAKU_NUMBER ");
        sb.AppendLine("     , INFO.SURNAME || INFO.NAME AS YOYAKU_NAME ");
        sb.AppendLine("     , CHARGE.NINZU AS NINZU ");
        sb.AppendLine("     , INFO.AGENT_CD AS AGENT_CD ");
        sb.AppendLine("     , INFO.AGENT_NM AS AGENT_NM ");
        sb.AppendLine("     , AGENT.AGENT_FORMAL_NAME AS AGENT_FORMAL_NAME ");
        sb.AppendLine("     , CASE WHEN INFO.MAIL_SENDING_KBN = 'Y' THEN '1' ELSE '3' END AS NOTIFICATION_HOHO ");
        sb.AppendLine("     , TRIM(AGENT.BUS_NOTIFICATION_HOHO) AS BUS_NOTIFICATION_HOHO ");
        sb.AppendLine("     , '' AS NOTIFICATION_HOHO_RIREKI ");
        sb.AppendLine("     , TO_YYYYMMDD_FC(NOTIFICATION.KAKUTEI_NOTIFICATION_DAY) AS KAKUTEI_NOTIFICATION_DAY ");
        sb.AppendLine("     , TO_CHAR(INFO.SYSTEM_ENTRY_DAY, 'YYYY/MM/DD') AS YOYAKU_ENTRY_DAY ");
        sb.AppendLine("     , INFO.MAIL_ADDRESS AS MAIL_ADDRESS ");
        sb.AppendLine("     , AGENT.MAIL AS MAIL_ADDRESS2 ");
        sb.AppendLine("     , AGENT.FAX AS FAX ");
        sb.AppendLine("     , INFO.YUBIN_NO AS YUBIN_NO ");
        sb.AppendLine("     , AGENT.YUBIN_NO AS YUBIN_NO2 ");
        sb.AppendLine("     , INFO.ADDRESS_1 AS ADDRESS_1 ");
        sb.AppendLine("     , INFO.ADDRESS_2 AS ADDRESS_2 ");
        sb.AppendLine("     , INFO.ADDRESS_3 AS ADDRESS_3 ");
        sb.AppendLine("     , AGENT.ADDRESS_1 AS ADDRESS2_1 ");
        sb.AppendLine("     , AGENT.ADDRESS_2 AS ADDRESS2_2 ");
        sb.AppendLine("     , AGENT.ADDRESS_3 AS ADDRESS2_3 ");
        sb.AppendLine("     , INFO.YOYAKU_UKETUKE_KBN AS YOYAKU_UKETUKE_KBN ");
        sb.AppendLine("     , CASE INFO.YOYAKU_UKETUKE_KBN WHEN '2' THEN 'インターネット' ELSE '' END AS YOYAKU_STATU ");
        sb.AppendLine("     , NOTIFICATION.BUS_COMPANY_CD AS BUS_COMPANY_CD_RIREKI ");
        sb.AppendLine("     , AGENT.OTA_KBN AS OTA_KBN");
        sb.AppendLine("     , BASIC.SYUPT_JI_CARRIER_KBN AS SYUPT_JI_CARRIER_KBN");
        sb.AppendLine("     , PLACE_1.PLACE_NAME_1 AS SYUGO_PLACE1");
        sb.AppendLine("     , PLACE_2.PLACE_NAME_1 AS SYUGO_PLACE2");
        sb.AppendLine("     , PLACE_3.PLACE_NAME_1 AS SYUGO_PLACE3");
        sb.AppendLine("     , PLACE_4.PLACE_NAME_1 AS SYUGO_PLACE4");
        sb.AppendLine("     , PLACE_5.PLACE_NAME_1 AS SYUGO_PLACE5");
        sb.AppendLine("     , PLACE_6.PLACE_NAME_1 AS SYUGO_PLACE_CARRIER");
        sb.AppendLine("     , TO_HHMM_FC(BASIC.SYUGO_TIME_1) AS SYUGO_TIME1");
        sb.AppendLine("     , TO_HHMM_FC(BASIC.SYUGO_TIME_2) AS SYUGO_TIME2");
        sb.AppendLine("     , TO_HHMM_FC(BASIC.SYUGO_TIME_3) AS SYUGO_TIME3");
        sb.AppendLine("     , TO_HHMM_FC(BASIC.SYUGO_TIME_4) AS SYUGO_TIME4");
        sb.AppendLine("     , TO_HHMM_FC(BASIC.SYUGO_TIME_5) AS SYUGO_TIME5");
        sb.AppendLine("     , TO_HHMM_FC(BASIC.SYUGO_TIME_CARRIER) AS SYUGO_TIME_CARRIER");
        sb.AppendLine("     , TO_HHMM_FC(BASIC.SYUPT_TIME_1) AS SYUPT_TIME1");
        sb.AppendLine("     , TO_HHMM_FC(BASIC.SYUPT_TIME_2) AS SYUPT_TIME2");
        sb.AppendLine("     , TO_HHMM_FC(BASIC.SYUPT_TIME_3) AS SYUPT_TIME3");
        sb.AppendLine("     , TO_HHMM_FC(BASIC.SYUPT_TIME_4) AS SYUPT_TIME4");
        sb.AppendLine("     , TO_HHMM_FC(BASIC.SYUPT_TIME_5) AS SYUPT_TIME5");
        sb.AppendLine("     , TO_HHMM_FC(BASIC.SYUPT_TIME_CARRIER) AS SYUPT_TIME_CARRIER");
        sb.AppendLine("     , INFO.ZASEKI AS ZASEKI");
        sb.AppendLine("     , INFO.JYOSYA_NINZU_1 AS JYOSYA_NINZU_1");
        sb.AppendLine("     , INFO.JYOSYA_NINZU_2 AS JYOSYA_NINZU_2");
        sb.AppendLine("     , INFO.JYOSYA_NINZU_3 AS JYOSYA_NINZU_3");
        sb.AppendLine("     , INFO.JYOSYA_NINZU_4 AS JYOSYA_NINZU_4");
        sb.AppendLine("     , INFO.JYOSYA_NINZU_5 AS JYOSYA_NINZU_5");
        sb.AppendLine("     , '' AS NYUUKIN_SITUATION_KBN ");
        sb.AppendLine("     , '' AS CONTACT_FLG ");
        sb.AppendLine("     , '' AS STATU ");
        sb.AppendLine("     , '' AS TEIKI_KIKAKU_KBN ");
        sb.AppendLine("     , '' AS SAIKOU_KAKUTEI_KBN ");
        sb.AppendLine("     , '' AS CANCEL_FLG ");

        // FROM句
        sb.AppendLine("    FROM ");

        // コース台帳(基本) BASIC
        sb.AppendLine("    T_CRS_LEDGER_BASIC BASIC");

        // 内部結合
        sb.AppendLine("    INNER JOIN ");

        // 予約情報（基本）
        sb.AppendLine("        T_YOYAKU_INFO_BASIC INFO");
        sb.AppendLine("        ON BASIC.CRS_CD = INFO.CRS_CD ");
        sb.AppendLine("        AND BASIC.SYUPT_DAY = INFO.SYUPT_DAY ");
        sb.AppendLine("        AND BASIC.GOUSYA = INFO.GOUSYA ");
        sb.AppendLine("        AND INFO.YOYAKU_KBN IN ('0','1','2','3','4','5','6','7','8','9') ");
        sb.AppendLine("        AND INFO.CANCEL_FLG IS NULL ");
        sb.AppendLine("        AND INFO.NYUUKIN_SITUATION_KBN = '1' ");

        // 内部結合
        sb.AppendLine("    INNER JOIN ");

        // 予約情報（コース料金_料金区分）CHARGE
        sb.AppendLine("        ( ");
        sb.AppendLine("        SELECT");
        sb.AppendLine("              YOYAKU_KBN ");
        sb.AppendLine("            , YOYAKU_NO ");
        sb.AppendLine("            , SUM(CHARGE_APPLICATION_NINZU) AS NINZU ");
        sb.AppendLine("        FROM ");
        sb.AppendLine("            T_YOYAKU_INFO_CRS_CHARGE_CHARGE_KBN ");
        sb.AppendLine("        GROUP BY ");
        sb.AppendLine("              YOYAKU_KBN ");
        sb.AppendLine("            , YOYAKU_NO ");
        sb.AppendLine("        ) CHARGE ");
        sb.AppendLine("        ON INFO.YOYAKU_KBN = CHARGE.YOYAKU_KBN ");
        sb.AppendLine("        AND INFO.YOYAKU_NO = CHARGE.YOYAKU_NO ");

        // 左結合
        sb.AppendLine("    LEFT JOIN ");

        // 仕入先マスタ
        sb.AppendLine("        M_SIIRE_SAKI SIIRE");
        sb.AppendLine("        ON SUBSTR(BASIC.BUS_COMPANY_CD, 1, 4) = SIIRE.SIIRE_SAKI_CD ");
        sb.AppendLine("        AND SUBSTR(BASIC.BUS_COMPANY_CD, LENGTH(BASIC.BUS_COMPANY_CD) - 1, 2) = SIIRE.SIIRE_SAKI_NO ");
        sb.AppendLine("        AND SIIRE.DELETE_DATE IS NULL ");

        // 左結合
        sb.AppendLine("    LEFT JOIN ");

        // バス会社確定通知出力記録
        sb.AppendLine("        T_BUS_COMPANY_KAKUTEI_NOTIFICATION_OUT NOTIFICATION");
        sb.AppendLine("        ON BASIC.CRS_CD = NOTIFICATION.CRS_CD ");
        sb.AppendLine("        AND BASIC.SYUPT_DAY = NOTIFICATION.SYUPT_DAY ");
        sb.AppendLine("        AND BASIC.GOUSYA = NOTIFICATION.GOUSYA ");
        sb.AppendLine("        AND INFO.YOYAKU_KBN = NOTIFICATION.YOYAKU_KBN ");
        sb.AppendLine("        AND INFO.YOYAKU_NO = NOTIFICATION.YOYAKU_NO ");

        // 左結合
        sb.AppendLine("    LEFT JOIN ");

        // 代理店マスタ
        sb.AppendLine("        M_AGENT AGENT");
        sb.AppendLine("        ON INFO.AGENT_CD = AGENT.AGENT_CD ");
        sb.AppendLine("        AND AGENT.DELETE_DATE IS NULL ");

        // 左結合
        sb.AppendLine("    LEFT JOIN ");

        // 場所マスタ PLACE_1
        sb.AppendLine("        M_PLACE PLACE_1");
        sb.AppendLine("        ON BASIC.HAISYA_KEIYU_CD_1 = PLACE_1.PLACE_CD ");
        sb.AppendLine("        AND PLACE_1.DELETE_DATE IS NULL ");

        // 左結合
        sb.AppendLine("    LEFT JOIN ");

        // 場所マスタ PLACE_2
        sb.AppendLine("        M_PLACE PLACE_2");
        sb.AppendLine("        ON BASIC.HAISYA_KEIYU_CD_2 = PLACE_2.PLACE_CD ");
        sb.AppendLine("        AND PLACE_2.DELETE_DATE IS NULL ");

        // 左結合
        sb.AppendLine("    LEFT JOIN ");

        // 場所マスタ PLACE_3
        sb.AppendLine("        M_PLACE PLACE_3");
        sb.AppendLine("        ON BASIC.HAISYA_KEIYU_CD_3 = PLACE_3.PLACE_CD ");
        sb.AppendLine("        AND PLACE_3.DELETE_DATE IS NULL ");

        // 左結合
        sb.AppendLine("    LEFT JOIN ");

        // 場所マスタ PLACE_4
        sb.AppendLine("        M_PLACE PLACE_4");
        sb.AppendLine("        ON BASIC.HAISYA_KEIYU_CD_4 = PLACE_4.PLACE_CD ");
        sb.AppendLine("        AND PLACE_4.DELETE_DATE IS NULL ");

        // 左結合
        sb.AppendLine("    LEFT JOIN ");

        // 場所マスタ PLACE_5
        sb.AppendLine("        M_PLACE PLACE_5");
        sb.AppendLine("        ON BASIC.HAISYA_KEIYU_CD_5 = PLACE_5.PLACE_CD ");
        sb.AppendLine("        AND PLACE_5.DELETE_DATE IS NULL ");

        // 左結合
        sb.AppendLine("    LEFT JOIN ");

        // 場所マスタ PLACE_6
        sb.AppendLine("        M_PLACE PLACE_6");
        sb.AppendLine("        ON BASIC.SYUGO_PLACE_CD_CARRIER = PLACE_6.PLACE_CD ");
        sb.AppendLine("        AND PLACE_6.DELETE_DATE IS NULL ");

        // 内部結合
        sb.AppendLine("    INNER JOIN ");

        // コース台帳（バス紐づけ）
        sb.AppendLine("        T_CRS_LEDGER_BUS_HIMODUKE HIMODUKE");
        sb.AppendLine("        ON BASIC.CRS_CD = HIMODUKE.CRS_CD ");
        sb.AppendLine("        AND BASIC.SYUPT_DAY = HIMODUKE.SYUPT_DAY ");
        sb.AppendLine("        AND BASIC.GOUSYA = HIMODUKE.GOUSYA ");
        sb.AppendLine("        AND HIMODUKE.DELETE_DATE = 0 ");
        sb.AppendLine("        AND HIMODUKE.BUS_COMPANY_KAKUTEI_DAY <> 0 ");
        sb.AppendLine("        AND HIMODUKE.HANBAI_START_JI_KAKUTEI_FLG IS NULL ");

        // WHERE句
        sb.AppendLine("    WHERE ");
        sb.AppendLine("        NVL(BASIC.MARU_ZOU_MANAGEMENT_KBN, '*') <> '" + MaruzouKanriKbn.Maruzou + "'");
        sb.AppendLine("        AND NVL(BASIC.SAIKOU_KAKUTEI_KBN, '*') = '" + SaikouKakuteiKbn.Saikou + "'");
        sb.AppendLine("        AND BASIC.DELETE_DAY = 0 ");
        sb.AppendLine("        AND NVL(BASIC.HOUJIN_GAIKYAKU_KBN, '*') = '" + HoujinGaikyakuKbnType.Houjin + "'");
        sb.AppendLine("        AND BASIC.BUS_COMPANY_CD IS NOT NULL");
        sb.AppendLine("        AND (INFO.ENTRY_DAY < HIMODUKE.BUS_COMPANY_KAKUTEI_DAY OR (INFO.ENTRY_DAY = HIMODUKE.BUS_COMPANY_KAKUTEI_DAY AND INFO.ENTRY_TIME < 160000) ) ");
        // 出発日FROM
        if (param.SyuptDayFrom is object)
        {
            sb.AppendLine("    AND BASIC.SYUPT_DAY >= ").Append(setSelectParam(param.SyuptDayFrom, crsLedgerBasic.syuptDay));
        }

        // 出発日TO
        if (param.SyuptDayTo is object)
        {
            sb.AppendLine("    AND BASIC.SYUPT_DAY <= ").Append(setSelectParam(param.SyuptDayTo, crsLedgerBasic.syuptDay));
        }

        // コースコード
        if (!string.IsNullOrEmpty(param.CrsCd))
        {
            sb.AppendLine("    AND BASIC.CRS_CD = ").Append(setSelectParam(param.CrsCd, crsLedgerBasic.crsCd));
        }

        // 号車
        if (param.Gousya != 0)
        {
            sb.AppendLine("    AND BASIC.GOUSYA = ").Append(setSelectParam(param.Gousya, crsLedgerBasic.gousya));
        }

        // 条件WHERE(※１）
        if (param.YoyakuMember == true || param.YoyakuAgent == true)
        {
            sb.AppendLine("    AND ( 1 = 1 ");

            // ◆通知先＝予約者の場合
            if (param.YoyakuMember == true)
            {
                sb.AppendLine("    AND (INFO.AGENT_CD IS NULL OR (SUBSTR(INFO.AGENT_CD, 1, 4) IN ('0001', '0002')) ) ");
            }
            // ◆通知先＝代理店の場合
            if (param.YoyakuAgent == true)
            {
                sb.AppendLine("    AND (INFO.AGENT_CD IS NOT NULL AND (SUBSTR(INFO.AGENT_CD, 1, 4) NOT IN ('0001', '0002')) ) ");
            }

            sb.AppendLine("    ) ");
        }

        // 条件（WHERE）(※２）
        if (param.SendMail == true || param.SendFax == true || param.SendPost == true || param.SendNothing == true)
        {
            sb.AppendLine("    AND ( 1 <> 1 ");

            // ◆通知方法＝メールの場合
            if (param.SendMail == true)
            {
                sb.AppendLine("    OR ( INFO.MAIL_SENDING_KBN IS NOT NULL ");
                // 条件WHERE(※２-１）

                sb.AppendLine("    OR ( 1 = 1 ");
                // ◆通知方法＝メールの場合
                if (param.SendMail == true)
                {
                    sb.AppendLine("    AND  AGENT.BUS_NOTIFICATION_HOHO = '" + NotificationHohoBusCompany.Mail + "'");
                }
                // ◆通知方法＝郵送の場合
                if (param.SendPost == true)
                {
                    sb.AppendLine("    AND  AGENT.BUS_NOTIFICATION_HOHO = '" + NotificationHohoBusCompany.Yusou + "'");
                }

                sb.AppendLine("    ) )");
            }

            // ◆通知方法＝FAXの場合
            if (param.SendFax == true)
            {
                // 条件WHERE(※２-１）
                sb.AppendLine("    OR ( ( 1 = 1 ");
                // ◆通知方法＝メールの場合
                if (param.SendMail == true)
                {
                    sb.AppendLine("    AND  AGENT.BUS_NOTIFICATION_HOHO = '" + NotificationHohoBusCompany.Mail + "'");
                }
                // ◆通知方法＝郵送の場合
                if (param.SendPost == true)
                {
                    sb.AppendLine("    AND  AGENT.BUS_NOTIFICATION_HOHO = '" + NotificationHohoBusCompany.Yusou + "'");
                }

                sb.AppendLine("    ) ");
                sb.AppendLine("        AND AGENT.BUS_NOTIFICATION_HOHO = '" + NotificationHohoBusCompany.Fax + "')");
            }

            // ◆通知方法＝郵送の場合
            if (param.SendPost == true)
            {
                sb.AppendLine("    OR ( INFO.MAIL_SENDING_KBN IS NULL ");
                // 条件WHERE(※２-１）
                sb.AppendLine("    OR (  1 = 1  ");
                if (param.SendMail == true)
                {
                    sb.AppendLine("    AND  AGENT.BUS_NOTIFICATION_HOHO = '" + NotificationHohoBusCompany.Mail + "'");
                }

                if (param.SendPost == true)
                {
                    sb.AppendLine("    AND  AGENT.BUS_NOTIFICATION_HOHO = '" + NotificationHohoBusCompany.Yusou + "'");
                }

                sb.AppendLine("   ) )");
            }

            // ◆通知方法＝不要の場合
            // ※　使用未確定のため実装不要

            sb.AppendLine("    ) ");
        }

        // 条件（WHERE）(※３）
        if (param.SendFinish == true || param.NotSend == true)
        {
            sb.AppendLine("    AND ( 1 <> 1 ");

            // ◆通知済の場合
            if (param.SendFinish == true)
            {
                sb.AppendLine("   OR NOTIFICATION.KAKUTEI_NOTIFICATION_DAY IS NOT NULL  ");
            }

            // ◆未通知の場合
            if (param.NotSend == true)
            {
                sb.AppendLine("   OR NOTIFICATION.KAKUTEI_NOTIFICATION_DAY IS NULL  ");
            }

            sb.AppendLine("    ) ");
        }

        // ORDER句
        sb.AppendLine("  ORDER BY ");
        sb.AppendLine("      BASIC.SYUPT_DAY ");
        sb.AppendLine("    , BASIC.CRS_CD ");
        sb.AppendLine("    , BASIC.GOUSYA ");
        sb.AppendLine("    , INFO.YOYAKU_KBN ");
        sb.AppendLine("    , INFO.MOTO_YOYAKU_NO ");
        return base.getDataTable(sb.ToString());
    }

    /// <summary>
    /// 検索（通知記録照会）
    /// </summary>
    /// <param name="param"></param>
    /// <returns></returns>
    public DataTable selectDataTableInquiry(S03_0502DASelectParam param)
    {
        var crsLedgerBasic = new CrsLedgerBasicEntity();
        var yoyakuInfoBasic = new YoyakuInfoBasicEntity();
        // SQL文字列
        var sb = new StringBuilder();

        // パラメータクリア
        clear();

        // SELECT句
        sb.AppendLine("SELECT ");
        sb.AppendLine("       TO_YYYYMMDD_FC(BASIC.SYUPT_DAY) AS SYUPT_DAY_STR ");
        sb.AppendLine("     , BASIC.SYUPT_DAY AS SYUPT_DAY ");
        sb.AppendLine("     , BASIC.CRS_CD AS CRS_CD ");
        sb.AppendLine("     , '' AS CRS_NAME ");
        sb.AppendLine("     , TO_HHMM_FC(BASIC.SYUPT_TIME_1) AS SYUPT_TIME_STR ");
        sb.AppendLine("     , BASIC.GOUSYA AS GOUSYA ");
        sb.AppendLine("     , BASIC.BUS_COMPANY_CD AS BUS_COMPANY_CD ");
        sb.AppendLine("     , SIIRE.SIIRE_SAKI_NAME AS BUS_COMPANY_NAME ");
        sb.AppendLine("     , INFO.YOYAKU_KBN AS YOYAKU_KBN ");
        sb.AppendLine("     , INFO.YOYAKU_NO AS YOYAKU_NO ");
        sb.AppendLine("     , TO_CHAR(INFO.YOYAKU_KBN || INFO.YOYAKU_NO, 'FM0,999,999,999') AS YOYAKU_NUMBER ");
        sb.AppendLine("     , INFO.SURNAME || INFO.NAME AS YOYAKU_NAME ");
        sb.AppendLine("     , '' AS NINZU ");
        sb.AppendLine("     , INFO.AGENT_CD AS AGENT_CD ");
        sb.AppendLine("     , INFO.AGENT_NM AS AGENT_NM ");
        sb.AppendLine("     , '' AS AGENT_FORMAL_NAME ");
        sb.AppendLine("     , '' AS NOTIFICATION_HOHO ");
        sb.AppendLine("     , '' AS BUS_NOTIFICATION_HOHO ");
        sb.AppendLine("     , CASE TRIM(NOTIFICATION.NOTIFICATION_HOHO) WHEN '1' THEN 'メール' WHEN '2' THEN 'FAX' WHEN '3' THEN '郵送' ELSE '不要' END AS NOTIFICATION_HOHO_RIREKI ");
        sb.AppendLine("     , TO_YYYYMMDD_FC(NOTIFICATION.KAKUTEI_NOTIFICATION_DAY) AS KAKUTEI_NOTIFICATION_DAY ");
        sb.AppendLine("     , '' AS YOYAKU_ENTRY_DAY ");
        sb.AppendLine("     , '' AS MAIL_ADDRESS ");
        sb.AppendLine("     , '' AS MAIL_ADDRESS2 ");
        sb.AppendLine("     , '' AS FAX ");
        sb.AppendLine("     , '' AS YUBIN_NO ");
        sb.AppendLine("     , '' AS YUBIN_NO2 ");
        sb.AppendLine("     , '' AS ADDRESS_1 ");
        sb.AppendLine("     , '' AS ADDRESS_2 ");
        sb.AppendLine("     , '' AS ADDRESS_3 ");
        sb.AppendLine("     , '' AS ADDRESS2_1 ");
        sb.AppendLine("     , '' AS ADDRESS2_2 ");
        sb.AppendLine("     , '' AS ADDRESS2_3 ");
        sb.AppendLine("     , INFO.YOYAKU_UKETUKE_KBN AS YOYAKU_UKETUKE_KBN ");
        sb.AppendLine("     , CASE INFO.YOYAKU_UKETUKE_KBN WHEN '2' THEN 'インターネット' ELSE '' END AS YOYAKU_STATU ");
        sb.AppendLine("     , NOTIFICATION.BUS_COMPANY_CD AS BUS_COMPANY_CD_RIREKI ");
        sb.AppendLine("     , '' AS OTA_KBN");
        sb.AppendLine("     , '' AS SYUPT_JI_CARRIER_KBN");
        sb.AppendLine("     , '' AS SYUGO_PLACE1");
        sb.AppendLine("     , '' AS SYUGO_PLACE2");
        sb.AppendLine("     , '' AS SYUGO_PLACE3");
        sb.AppendLine("     , '' AS SYUGO_PLACE4");
        sb.AppendLine("     , '' AS SYUGO_PLACE5");
        sb.AppendLine("     , '' AS SYUGO_PLACE_CARRIER");
        sb.AppendLine("     , '' AS SYUGO_TIME1");
        sb.AppendLine("     , '' AS SYUGO_TIME2");
        sb.AppendLine("     , '' AS SYUGO_TIME3");
        sb.AppendLine("     , '' AS SYUGO_TIME4");
        sb.AppendLine("     , '' AS SYUGO_TIME5");
        sb.AppendLine("     , '' AS SYUGO_TIME_CARRIER");
        sb.AppendLine("     , '' AS SYUPT_TIME1");
        sb.AppendLine("     , '' AS SYUPT_TIME2");
        sb.AppendLine("     , '' AS SYUPT_TIME3");
        sb.AppendLine("     , '' AS SYUPT_TIME4");
        sb.AppendLine("     , '' AS SYUPT_TIME5");
        sb.AppendLine("     , '' AS SYUPT_TIME_CARRIER");
        sb.AppendLine("     , '' AS ZASEKI");
        sb.AppendLine("     , '' AS JYOSYA_NINZU_1");
        sb.AppendLine("     , '' AS JYOSYA_NINZU_2");
        sb.AppendLine("     , '' AS JYOSYA_NINZU_3");
        sb.AppendLine("     , '' AS JYOSYA_NINZU_4");
        sb.AppendLine("     , '' AS JYOSYA_NINZU_5");
        sb.AppendLine("     , INFO.NYUUKIN_SITUATION_KBN AS NYUUKIN_SITUATION_KBN ");
        sb.AppendLine("     , '' AS CONTACT_FLG ");
        sb.AppendLine("     , CASE WHEN INFO.CANCEL_FLG IS NOT NULL THEN 'キャンセル済' ");
        sb.AppendLine("            WHEN INFO.NYUUKIN_SITUATION_KBN = '0' THEN '未入金' ");
        sb.AppendLine("            WHEN BASIC.BUS_COMPANY_CD <> NOTIFICATION.BUS_COMPANY_CD THEN '変更後未通知' ");
        sb.AppendLine("            WHEN INFO.ENTRY_DAY > HIMODUKE.BUS_COMPANY_KAKUTEI_DAY OR (INFO.ENTRY_DAY = HIMODUKE.BUS_COMPANY_KAKUTEI_DAY AND INFO.ENTRY_TIME > 160000) THEN '確定後受付' ");
        sb.AppendLine("            WHEN HIMODUKE.HANBAI_START_JI_KAKUTEI_FLG IS NOT NULL THEN '販売時確定済' ");
        sb.AppendLine("            ELSE '' ");
        sb.AppendLine("       END AS STATU ");
        sb.AppendLine("     , '' AS TEIKI_KIKAKU_KBN ");
        sb.AppendLine("     , '' AS SAIKOU_KAKUTEI_KBN ");
        sb.AppendLine("     , '' AS CANCEL_FLG ");

        // FROM句
        sb.AppendLine("    FROM ");

        // コース台帳(基本) BASIC
        sb.AppendLine("    T_CRS_LEDGER_BASIC BASIC");

        // 内部結合
        sb.AppendLine("    INNER JOIN ");

        // 予約情報（基本）INFO
        sb.AppendLine("        T_YOYAKU_INFO_BASIC INFO");
        sb.AppendLine("        ON BASIC.CRS_CD = INFO.CRS_CD ");
        sb.AppendLine("        AND BASIC.SYUPT_DAY = INFO.SYUPT_DAY ");
        sb.AppendLine("        AND BASIC.GOUSYA = INFO.GOUSYA ");
        sb.AppendLine("        AND INFO.YOYAKU_KBN IN ('0','1','2','3','4','5','6','7','8','9') ");

        // 左結合
        sb.AppendLine("    LEFT JOIN ");

        // バス会社確定通知出力記録
        sb.AppendLine("        T_BUS_COMPANY_KAKUTEI_NOTIFICATION_OUT NOTIFICATION");
        sb.AppendLine("        ON INFO.YOYAKU_KBN = NOTIFICATION.YOYAKU_KBN ");
        sb.AppendLine("        AND INFO.YOYAKU_NO = NOTIFICATION.YOYAKU_NO ");

        // 左結合
        sb.AppendLine("    LEFT JOIN ");

        // 仕入先マスタ
        sb.AppendLine("        M_SIIRE_SAKI SIIRE");
        sb.AppendLine("        ON SUBSTR(BASIC.BUS_COMPANY_CD, 1, 4) = SIIRE.SIIRE_SAKI_CD ");
        sb.AppendLine("        AND SUBSTR(BASIC.BUS_COMPANY_CD, LENGTH(BASIC.BUS_COMPANY_CD) - 1, 2) = SIIRE.SIIRE_SAKI_NO ");
        sb.AppendLine("        AND SIIRE.DELETE_DATE IS NULL ");

        // 左結合
        sb.AppendLine("    LEFT JOIN ");

        // コース台帳（バス紐づけ）
        sb.AppendLine("        T_CRS_LEDGER_BUS_HIMODUKE HIMODUKE");
        sb.AppendLine("        ON BASIC.CRS_CD = HIMODUKE.CRS_CD ");
        sb.AppendLine("        AND BASIC.SYUPT_DAY = HIMODUKE.SYUPT_DAY ");
        sb.AppendLine("        AND BASIC.GOUSYA = HIMODUKE.GOUSYA ");
        sb.AppendLine("        AND HIMODUKE.DELETE_DATE = 0 ");
        sb.AppendLine("        AND HIMODUKE.BUS_COMPANY_KAKUTEI_DAY <> 0 ");

        // WHERE句
        sb.AppendLine("    WHERE ");
        sb.AppendLine("        NVL(BASIC.MARU_ZOU_MANAGEMENT_KBN, '*') <> '" + MaruzouKanriKbn.Maruzou + "'");
        sb.AppendLine("        AND NVL(BASIC.SAIKOU_KAKUTEI_KBN, '*') = '" + SaikouKakuteiKbn.Saikou + "'");
        sb.AppendLine("        AND BASIC.DELETE_DAY = 0 ");
        sb.AppendLine("        AND NVL(BASIC.HOUJIN_GAIKYAKU_KBN, '*') = '" + HoujinGaikyakuKbnType.Houjin + "'");
        sb.AppendLine("        AND BASIC.BUS_COMPANY_CD IS NOT NULL");
        // 出発日FROM
        if (param.SyuptDayFrom is object)
        {
            sb.AppendLine("    AND BASIC.SYUPT_DAY >= ").Append(setSelectParam(param.SyuptDayFrom, crsLedgerBasic.syuptDay));
        }

        // 出発日TO
        if (param.SyuptDayTo is object)
        {
            sb.AppendLine("    AND BASIC.SYUPT_DAY <= ").Append(setSelectParam(param.SyuptDayTo, crsLedgerBasic.syuptDay));
        }

        // コースコード
        if (!string.IsNullOrEmpty(param.CrsCd))
        {
            sb.AppendLine("    AND BASIC.CRS_CD = ").Append(setSelectParam(param.CrsCd, crsLedgerBasic.crsCd));
        }

        // 予約区分
        if (param.YoyakuKbn is object)
        {
            sb.AppendLine("     AND INFO.YOYAKU_KBN = ").Append(setSelectParam(param.YoyakuKbn, yoyakuInfoBasic.yoyakuKbn));
        }

        // 予約ＮＯ
        if (param.YoyakuNo != 0)
        {
            sb.AppendLine("     AND INFO.YOYAKU_NO = ").Append(setSelectParam(param.YoyakuNo, yoyakuInfoBasic.yoyakuNo));
        }

        // 号車
        if (param.Gousya != 0)
        {
            sb.AppendLine("    AND BASIC.GOUSYA = ").Append(setSelectParam(param.Gousya, crsLedgerBasic.gousya));
        }

        // 条件WHERE(※１）
        if (param.YoyakuMember == true || param.YoyakuAgent == true)
        {
            sb.AppendLine("    AND ( 1 = 1 ");

            // ◆通知先＝予約者の場合
            if (param.YoyakuMember == true)
            {
                sb.AppendLine("  AND ( INFO.AGENT_CD IS NULL OR (SUBSTR(INFO.AGENT_CD,1,4) IN ('0001','0002')) ) ");
            }
            // ◆通知先＝代理店の場合
            if (param.YoyakuAgent)
            {
                sb.AppendLine("    AND ( INFO.AGENT_CD IS NOT NULL AND (SUBSTR(INFO.AGENT_CD,1,4) NOT IN ('0001','0002')) ) ");
            }

            sb.AppendLine("    ) ");
        }

        // 条件（WHERE）(※２）
        if (param.SendMail == true || param.SendFax == true || param.SendPost == true)
        {
            sb.AppendLine("    AND ( 1 <> 1 ");

            // ◆通知方法＝メールの場合
            if (param.SendMail == true)
            {
                sb.AppendLine("     OR NOTIFICATION.NOTIFICATION_HOHO = '" + NotificationHohoBusCompany.Mail + "'");
            }
            // ◆通知方法＝FAXの場合
            if (param.SendFax == true)
            {
                sb.AppendLine("     OR NOTIFICATION.NOTIFICATION_HOHO = '" + NotificationHohoBusCompany.Fax + "'");
            }
            // ◆通知方法＝郵送の場合
            if (param.SendPost == true)
            {
                sb.AppendLine("     OR NOTIFICATION.NOTIFICATION_HOHO = '" + NotificationHohoBusCompany.Yusou + "'");
            }

            sb.AppendLine("    ) ");
        }

        // ORDER句
        sb.AppendLine("  ORDER BY ");
        sb.AppendLine("      BASIC.SYUPT_DAY ");
        sb.AppendLine("    , BASIC.CRS_CD ");
        sb.AppendLine("    , BASIC.GOUSYA ");
        sb.AppendLine("    , INFO.YOYAKU_KBN ");
        sb.AppendLine("    , INFO.MOTO_YOYAKU_NO ");
        return base.getDataTable(sb.ToString());
    }

    /// <summary>
    /// 検索（予約番号入力時）
    /// </summary>
    /// <param name="param"></param>
    /// <returns></returns>
    public DataTable selectDataTableReservation(S03_0502DASelectParam param)
    {
        var crsLedgerBasic = new CrsLedgerBasicEntity();
        var yoyakuInfoBasic = new YoyakuInfoBasicEntity();
        var yoyakuInfoCrsChargeChargeKbn = new YoyakuInfoCrsChargeChargeKbnEntity();
        var siireSaki = new SiireSakiEntity();
        var crsLedgerBusHimoduke = new TCrsLedgerBusHimodukeEntity();

        // SQL文字列
        var sb = new StringBuilder();

        // パラメータクリア
        clear();

        // SELECT句
        sb.AppendLine("SELECT ");
        sb.AppendLine("       TO_YYYYMMDD_FC(BASIC.SYUPT_DAY) AS SYUPT_DAY_STR ");
        sb.AppendLine("     , BASIC.SYUPT_DAY AS SYUPT_DAY ");
        sb.AppendLine("     , BASIC.CRS_CD AS CRS_CD ");
        sb.AppendLine("     , BASIC.CRS_NAME AS CRS_NAME ");
        sb.AppendLine("     , TO_HHMM_FC(BASIC.SYUPT_TIME_1) AS SYUPT_TIME_STR ");
        sb.AppendLine("     , BASIC.GOUSYA AS GOUSYA ");
        sb.AppendLine("     , BASIC.BUS_COMPANY_CD AS BUS_COMPANY_CD ");
        sb.AppendLine("     , SIIRE.SIIRE_SAKI_NAME AS BUS_COMPANY_NAME ");
        sb.AppendLine("     , INFO.YOYAKU_KBN AS YOYAKU_KBN ");
        sb.AppendLine("     , INFO.YOYAKU_NO AS YOYAKU_NO ");
        sb.AppendLine("     , TO_CHAR(INFO.YOYAKU_KBN || INFO.YOYAKU_NO, 'FM0,999,999,999') AS YOYAKU_NUMBER ");
        sb.AppendLine("     , INFO.SURNAME || INFO.NAME AS YOYAKU_NAME ");
        sb.AppendLine("     , CHARGE.NINZU AS NINZU ");
        sb.AppendLine("     , INFO.AGENT_CD AS AGENT_CD ");
        sb.AppendLine("     , INFO.AGENT_NM AS AGENT_NM ");
        sb.AppendLine("     , AGENT.AGENT_FORMAL_NAME AS AGENT_FORMAL_NAME ");
        sb.AppendLine("     , CASE WHEN INFO.MAIL_SENDING_KBN = 'Y' THEN '1' ELSE '3' END AS NOTIFICATION_HOHO ");
        sb.AppendLine("     , TRIM(AGENT.BUS_NOTIFICATION_HOHO) AS BUS_NOTIFICATION_HOHO ");
        sb.AppendLine("     , '' AS NOTIFICATION_HOHO_RIREKI ");
        sb.AppendLine("     , TO_YYYYMMDD_FC(NOTIFICATION.KAKUTEI_NOTIFICATION_DAY) AS KAKUTEI_NOTIFICATION_DAY ");
        sb.AppendLine("     , TO_CHAR(INFO.SYSTEM_ENTRY_DAY, 'YYYY/MM/DD') AS YOYAKU_ENTRY_DAY ");
        sb.AppendLine("     , INFO.MAIL_ADDRESS AS MAIL_ADDRESS ");
        sb.AppendLine("     , AGENT.MAIL AS MAIL_ADDRESS2 ");
        sb.AppendLine("     , AGENT.FAX AS FAX ");
        sb.AppendLine("     , INFO.YUBIN_NO AS YUBIN_NO ");
        sb.AppendLine("     , AGENT.YUBIN_NO AS YUBIN_NO2 ");
        sb.AppendLine("     , INFO.ADDRESS_1 AS ADDRESS_1 ");
        sb.AppendLine("     , INFO.ADDRESS_2 AS ADDRESS_2 ");
        sb.AppendLine("     , INFO.ADDRESS_3 AS ADDRESS_3 ");
        sb.AppendLine("     , AGENT.ADDRESS_1 AS ADDRESS2_1 ");
        sb.AppendLine("     , AGENT.ADDRESS_2 AS ADDRESS2_2 ");
        sb.AppendLine("     , AGENT.ADDRESS_3 AS ADDRESS2_3 ");
        sb.AppendLine("     , INFO.YOYAKU_UKETUKE_KBN AS YOYAKU_UKETUKE_KBN ");
        sb.AppendLine("     , CASE INFO.YOYAKU_UKETUKE_KBN WHEN '2' THEN 'インターネット' ELSE '' END AS YOYAKU_STATU ");
        sb.AppendLine("     , NOTIFICATION.BUS_COMPANY_CD AS BUS_COMPANY_CD_RIREKI ");
        sb.AppendLine("     , AGENT.OTA_KBN AS OTA_KBN");
        sb.AppendLine("     , BASIC.SYUPT_JI_CARRIER_KBN AS SYUPT_JI_CARRIER_KBN");
        sb.AppendLine("     , PLACE_1.PLACE_NAME_1 AS SYUGO_PLACE1");
        sb.AppendLine("     , PLACE_2.PLACE_NAME_1 AS SYUGO_PLACE2");
        sb.AppendLine("     , PLACE_3.PLACE_NAME_1 AS SYUGO_PLACE3");
        sb.AppendLine("     , PLACE_4.PLACE_NAME_1 AS SYUGO_PLACE4");
        sb.AppendLine("     , PLACE_5.PLACE_NAME_1 AS SYUGO_PLACE5");
        sb.AppendLine("     , PLACE_6.PLACE_NAME_1 AS SYUGO_PLACE_CARRIER");
        sb.AppendLine("     , TO_HHMM_FC(BASIC.SYUGO_TIME_1) AS SYUGO_TIME1");
        sb.AppendLine("     , TO_HHMM_FC(BASIC.SYUGO_TIME_2) AS SYUGO_TIME2");
        sb.AppendLine("     , TO_HHMM_FC(BASIC.SYUGO_TIME_3) AS SYUGO_TIME3");
        sb.AppendLine("     , TO_HHMM_FC(BASIC.SYUGO_TIME_4) AS SYUGO_TIME4");
        sb.AppendLine("     , TO_HHMM_FC(BASIC.SYUGO_TIME_5) AS SYUGO_TIME5");
        sb.AppendLine("     , TO_HHMM_FC(BASIC.SYUGO_TIME_CARRIER) AS SYUGO_TIME_CARRIER");
        sb.AppendLine("     , TO_HHMM_FC(BASIC.SYUPT_TIME_1) AS SYUPT_TIME1");
        sb.AppendLine("     , TO_HHMM_FC(BASIC.SYUPT_TIME_2) AS SYUPT_TIME2");
        sb.AppendLine("     , TO_HHMM_FC(BASIC.SYUPT_TIME_3) AS SYUPT_TIME3");
        sb.AppendLine("     , TO_HHMM_FC(BASIC.SYUPT_TIME_4) AS SYUPT_TIME4");
        sb.AppendLine("     , TO_HHMM_FC(BASIC.SYUPT_TIME_5) AS SYUPT_TIME5");
        sb.AppendLine("     , TO_HHMM_FC(BASIC.SYUPT_TIME_CARRIER) AS SYUPT_TIME_CARRIER");
        sb.AppendLine("     , INFO.ZASEKI AS ZASEKI");
        sb.AppendLine("     , INFO.JYOSYA_NINZU_1 AS JYOSYA_NINZU_1");
        sb.AppendLine("     , INFO.JYOSYA_NINZU_2 AS JYOSYA_NINZU_2");
        sb.AppendLine("     , INFO.JYOSYA_NINZU_3 AS JYOSYA_NINZU_3");
        sb.AppendLine("     , INFO.JYOSYA_NINZU_4 AS JYOSYA_NINZU_4");
        sb.AppendLine("     , INFO.JYOSYA_NINZU_5 AS JYOSYA_NINZU_5");
        sb.AppendLine("     , INFO.NYUUKIN_SITUATION_KBN AS NYUUKIN_SITUATION_KBN ");
        sb.AppendLine("     , '' AS CONTACT_FLG ");
        sb.AppendLine("     , '' AS STATU ");
        sb.AppendLine("     , BASIC.TEIKI_KIKAKU_KBN AS TEIKI_KIKAKU_KBN ");
        sb.AppendLine("     , BASIC.SAIKOU_KAKUTEI_KBN AS SAIKOU_KAKUTEI_KBN ");
        sb.AppendLine("     , INFO.CANCEL_FLG AS CANCEL_FLG ");

        // FROM句
        sb.AppendLine("    FROM ");

        // 予約情報（基本）INFO
        sb.AppendLine("    T_YOYAKU_INFO_BASIC INFO");

        // 内部結合
        sb.AppendLine("    INNER JOIN ");

        // 予約情報（コース料金_料金区分） CHARGE
        sb.AppendLine("        ( ");
        sb.AppendLine("        SELECT");
        sb.AppendLine("              YOYAKU_KBN ");
        sb.AppendLine("            , YOYAKU_NO ");
        sb.AppendLine("            , SUM(CHARGE_APPLICATION_NINZU) AS NINZU ");
        sb.AppendLine("        FROM ");
        sb.AppendLine("            T_YOYAKU_INFO_CRS_CHARGE_CHARGE_KBN ");
        sb.AppendLine("        GROUP BY ");
        sb.AppendLine("              YOYAKU_KBN ");
        sb.AppendLine("            , YOYAKU_NO ");
        sb.AppendLine("        ) CHARGE ");
        sb.AppendLine("        ON INFO.YOYAKU_KBN = CHARGE.YOYAKU_KBN ");
        sb.AppendLine("        AND INFO.YOYAKU_NO = CHARGE.YOYAKU_NO ");

        // 左結合
        sb.AppendLine("    LEFT JOIN ");

        // 代理店マスタ
        sb.AppendLine("        M_AGENT AGENT");
        sb.AppendLine("        ON INFO.AGENT_CD = AGENT.AGENT_CD ");
        sb.AppendLine("        AND AGENT.DELETE_DATE IS NULL ");

        // 左結合
        sb.AppendLine("    LEFT JOIN ");

        // コース台帳(基本)
        sb.AppendLine("        T_CRS_LEDGER_BASIC BASIC");
        sb.AppendLine("        ON INFO.CRS_CD = BASIC.CRS_CD ");
        sb.AppendLine("        AND INFO.SYUPT_DAY = BASIC.SYUPT_DAY ");
        sb.AppendLine("        AND INFO.GOUSYA = BASIC.GOUSYA ");
        sb.AppendLine("        AND NVL(BASIC.HOUJIN_GAIKYAKU_KBN, '*') = '" + HoujinGaikyakuKbnType.Houjin + "'");
        sb.AppendLine("        AND NVL(BASIC.MARU_ZOU_MANAGEMENT_KBN, '*') <> '" + MaruzouKanriKbn.Maruzou + "'");
        sb.AppendLine("        AND BASIC.DELETE_DAY = 0");

        // 左結合
        sb.AppendLine("    LEFT JOIN ");

        // 仕入先マスタ
        sb.AppendLine("        M_SIIRE_SAKI SIIRE");
        sb.AppendLine("        ON SUBSTR(BASIC.BUS_COMPANY_CD, 1, 4) = SIIRE.SIIRE_SAKI_CD ");
        sb.AppendLine("        AND SUBSTR(BASIC.BUS_COMPANY_CD, LENGTH(BASIC.BUS_COMPANY_CD) - 1, 2) = SIIRE.SIIRE_SAKI_NO ");
        sb.AppendLine("        AND SIIRE.DELETE_DATE IS NULL ");

        // 左結合
        sb.AppendLine("    LEFT JOIN ");

        // バス会社確定通知出力記録
        sb.AppendLine("        T_BUS_COMPANY_KAKUTEI_NOTIFICATION_OUT NOTIFICATION");
        sb.AppendLine("        ON BASIC.CRS_CD = NOTIFICATION.CRS_CD ");
        sb.AppendLine("        AND BASIC.SYUPT_DAY = NOTIFICATION.SYUPT_DAY ");
        sb.AppendLine("        AND BASIC.GOUSYA = NOTIFICATION.GOUSYA ");
        sb.AppendLine("        AND INFO.YOYAKU_KBN = NOTIFICATION.YOYAKU_KBN ");
        sb.AppendLine("        AND INFO.YOYAKU_NO = NOTIFICATION.YOYAKU_NO ");

        // 左結合
        sb.AppendLine("    LEFT JOIN ");

        // 場所マスタ PLACE_1
        sb.AppendLine("        M_PLACE PLACE_1");
        sb.AppendLine("        ON BASIC.HAISYA_KEIYU_CD_1 = PLACE_1.PLACE_CD ");
        sb.AppendLine("        AND PLACE_1.DELETE_DATE IS NULL ");

        // 左結合
        sb.AppendLine("    LEFT JOIN ");

        // 場所マスタ PLACE_2
        sb.AppendLine("        M_PLACE PLACE_2");
        sb.AppendLine("        ON BASIC.HAISYA_KEIYU_CD_2 = PLACE_2.PLACE_CD ");
        sb.AppendLine("        AND PLACE_2.DELETE_DATE IS NULL ");

        // 左結合
        sb.AppendLine("    LEFT JOIN ");

        // 場所マスタ PLACE_3
        sb.AppendLine("        M_PLACE PLACE_3");
        sb.AppendLine("        ON BASIC.HAISYA_KEIYU_CD_3 = PLACE_3.PLACE_CD ");
        sb.AppendLine("        AND PLACE_3.DELETE_DATE IS NULL ");

        // 左結合
        sb.AppendLine("    LEFT JOIN ");

        // 場所マスタ PLACE_4
        sb.AppendLine("        M_PLACE PLACE_4");
        sb.AppendLine("        ON BASIC.HAISYA_KEIYU_CD_4 = PLACE_4.PLACE_CD ");
        sb.AppendLine("        AND PLACE_4.DELETE_DATE IS NULL ");

        // 左結合
        sb.AppendLine("    LEFT JOIN ");

        // 場所マスタ PLACE_5
        sb.AppendLine("        M_PLACE PLACE_5");
        sb.AppendLine("        ON BASIC.HAISYA_KEIYU_CD_5 = PLACE_5.PLACE_CD ");
        sb.AppendLine("        AND PLACE_5.DELETE_DATE IS NULL ");

        // 左結合
        sb.AppendLine("    LEFT JOIN ");

        // 場所マスタ PLACE_6
        sb.AppendLine("        M_PLACE PLACE_6");
        sb.AppendLine("        ON BASIC.SYUGO_PLACE_CD_CARRIER = PLACE_6.PLACE_CD ");
        sb.AppendLine("        AND PLACE_6.DELETE_DATE IS NULL ");

        // 左結合
        sb.AppendLine("    LEFT JOIN ");

        // コース台帳（バス紐づけ）
        sb.AppendLine("        T_CRS_LEDGER_BUS_HIMODUKE HIMODUKE");
        sb.AppendLine("        ON BASIC.CRS_CD = HIMODUKE.CRS_CD ");
        sb.AppendLine("        AND BASIC.SYUPT_DAY = HIMODUKE.SYUPT_DAY ");
        sb.AppendLine("        AND BASIC.GOUSYA = HIMODUKE.GOUSYA ");
        sb.AppendLine("        AND HIMODUKE.DELETE_DATE = 0 ");
        sb.AppendLine("        AND HIMODUKE.BUS_COMPANY_KAKUTEI_DAY <> 0 ");
        sb.AppendLine("        AND HIMODUKE.HANBAI_START_JI_KAKUTEI_FLG IS NULL ");

        // WHERE句
        sb.AppendLine("    WHERE ");
        sb.AppendLine("        INFO.YOYAKU_KBN IN ('0','1','2','3','4','5','6','7','8','9') ");
        // 予約区分
        sb.AppendLine("    AND INFO.YOYAKU_KBN = ").Append(setSelectParam(param.YoyakuKbn, yoyakuInfoBasic.yoyakuKbn));
        // 予約No
        sb.AppendLine("    AND INFO.YOYAKU_NO = ").Append(setSelectParam(param.YoyakuNo, yoyakuInfoBasic.yoyakuNo));

        // ORDER句
        sb.AppendLine("  ORDER BY ");
        sb.AppendLine("      BASIC.SYUPT_DAY ");
        sb.AppendLine("    , BASIC.CRS_CD ");
        sb.AppendLine("    , BASIC.GOUSYA ");
        sb.AppendLine("    , INFO.YOYAKU_KBN ");
        sb.AppendLine("    , INFO.MOTO_YOYAKU_NO ");
        return base.getDataTable(sb.ToString());
    }

    /// <summary>
    /// 通知内容取得
    /// </summary>
    /// <param name="param"></param>
    /// <returns></returns>
    public DataTable selectDataDataTableNotificationContent(S03_0502DAGetNotifiContentParam param)
    {
        var code = new MCodeEntity();
        // SQL文字列
        var sb = new StringBuilder();
        // パラメータクリア
        clear();
        // SELECT句
        sb.AppendLine(" SELECT ");
        sb.AppendLine("   CODE_VALUE AS CONTACT_KIND ");
        sb.AppendLine("   , NAIYO_1 AS NAIYO_1");
        // FROM句
        sb.AppendLine(" FROM ");
        sb.AppendLine("   M_CODE ");
        // WHERE句
        sb.AppendLine("WHERE ");
        sb.AppendLine("   DELETE_DATE IS NULL");
        // コード分類
        sb.AppendLine("   AND CODE_BUNRUI = ").Append(setSelectParam(param.CodeBunrui, code.CodeBunrui));
        return base.getDataTable(sb.ToString());
    }

    private void clear()
    {
        base.paramClear();
        ParamNum = 0;
    }

    public string setSelectParam(object value, IEntityKoumokuType ent)
    {
        return setParamEx(value, ent, true);
    }

    public string setUpdateParam(object value, IEntityKoumokuType ent)
    {
        return setParamEx(value, ent, true);
    }

    private string setParamEx(object value, IEntityKoumokuType ent, bool selFlg)
    {
        ParamNum += 1;
        if (selFlg == true && ent is EntityKoumoku_MojiType)
        {
            return base.setParam(ParamNum.ToString(), value, ent.DBType);
        }
        else
        {
            return base.setParam(ParamNum.ToString(), value, ent.DBType, ent.IntegerBu, ent.DecimalBu);
        }
    }
    #endregion

    #region DELETE/INSERT 処理 

    /// <summary>
    /// 削除/登録 (バス会社確定通知出力記録)
    /// </summary>
    /// <param name="lsParamDelete"></param>
    /// <param name="lsParamInsert"></param>
    /// <returns></returns>
    public int executeDeleteInsertNotificationOut(List<S03_0502DADeleteParam> lsParamDelete, List<S03_0502DAInsertParam> lsParamInsert)
    {
        int returnValue = 0;
        string sqlStringDelete = string.Empty;
        string sqlStringInsert = string.Empty;
        OracleConnection con = default;

        // コネクション開始
        con = openCon();
        try
        {
            int sizeData = lsParamDelete.Count;
            for (int row = 0, loopTo = sizeData - 1; row <= loopTo; row++)
            {
                // SQL文字列(削除)
                sqlStringDelete = getDeleteNotificationOutQuery(lsParamDelete[row]);
                returnValue += execNonQuery(con, sqlStringDelete);

                // SQL文字列(登録)
                sqlStringInsert = getInsertNotificationOutQuery(lsParamInsert[row]);
                returnValue += execNonQuery(con, sqlStringInsert);
            }
        }
        catch (Exception ex)
        {
            throw;
        }
        finally
        {
            if (con is object)
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }

                if (con is object)
                {
                    con.Dispose();
                }
            }
        }

        return returnValue;
    }
    #endregion

    #region DELETE処理 

    /// <summary>
    /// 削除（バス会社確定通知出力記録）
    /// </summary>
    /// <param name="param"></param>
    /// <returns></returns>
    private string getDeleteNotificationOutQuery(S03_0502DADeleteParam param)
    {
        var busComKakuteiNotifiEtt = new TBusCompanyKakuteiNotificationOutEntity();

        // SQL文字列
        var sb = new StringBuilder();

        // パラメータクリア
        clear();
        sb.AppendLine("DELETE FROM T_BUS_COMPANY_KAKUTEI_NOTIFICATION_OUT ");
        // WHERE
        sb.AppendLine("WHERE ");
        // 予約ＮＯ
        sb.AppendLine("  YOYAKU_KBN = ").Append(setUpdateParam(param.YoyakuKbn, busComKakuteiNotifiEtt.YoyakuKbn));
        // 仕入先コード
        sb.AppendLine("  AND ");
        sb.AppendLine("  YOYAKU_NO = ").Append(setUpdateParam(param.YoyakuNo, busComKakuteiNotifiEtt.YoyakuNo));
        return sb.ToString();
    }
    #endregion

    #region INSERT処理 

    /// <summary>
    /// 登録（バス会社確定通知出力記録）
    /// </summary>
    /// <param name="param"></param>
    /// <returns></returns>
    private string getInsertNotificationOutQuery(S03_0502DAInsertParam param)
    {
        var busComKakuteiNotifiEtt = new TBusCompanyKakuteiNotificationOutEntity();

        // SQL文字列
        var sb = new StringBuilder();
        sb.AppendLine("  INSERT INTO T_BUS_COMPANY_KAKUTEI_NOTIFICATION_OUT (");
        sb.AppendLine("        AGENT_CD ");
        sb.AppendLine("      , AGENT_NM ");
        sb.AppendLine("      , BUS_COMPANY_CD ");
        sb.AppendLine("      , CRS_CD ");
        sb.AppendLine("      , GOUSYA ");
        sb.AppendLine("      , KAKUTEI_NOTIFICATION_DAY ");
        sb.AppendLine("      , NOTIFICATION_HOHO ");
        sb.AppendLine("      , SYUPT_DAY ");
        sb.AppendLine("      , YEAR ");
        sb.AppendLine("      , YOYAKU_KBN ");
        sb.AppendLine("      , YOYAKU_NO ");
        sb.AppendLine("      , SYSTEM_ENTRY_PGMID ");
        sb.AppendLine("      , SYSTEM_ENTRY_PERSON_CD ");
        sb.AppendLine("      , SYSTEM_ENTRY_DAY ");
        sb.AppendLine("      , SYSTEM_UPDATE_PGMID ");
        sb.AppendLine("      , SYSTEM_UPDATE_PERSON_CD ");
        sb.AppendLine("      , SYSTEM_UPDATE_DAY)");
        // INSERT 登録
        sb.AppendLine("  VALUES");
        // 業者コード
        sb.AppendLine("  (" + setUpdateParam(param.AgentCd, busComKakuteiNotifiEtt.AgentCd));
        // 業者名称
        sb.AppendLine("  ," + setUpdateParam(param.AgentNm, busComKakuteiNotifiEtt.AgentNm));
        // バス会社コード
        sb.AppendLine("  ," + setUpdateParam(param.BusCompanyCd, busComKakuteiNotifiEtt.BusCompanyCd));
        // コースコード
        sb.AppendLine("  ," + setUpdateParam(param.CrsCd, busComKakuteiNotifiEtt.CrsCd));
        // 号車
        sb.AppendLine("  ," + setUpdateParam(param.Gousya, busComKakuteiNotifiEtt.Gousya));
        // 確定通知日
        sb.AppendLine("  ," + setUpdateParam(param.KakuteiNotificationDay, busComKakuteiNotifiEtt.KakuteiNotificationDay));
        // 通知方法
        sb.AppendLine("  ," + setUpdateParam(param.NotificationHoho, busComKakuteiNotifiEtt.NotificationHoho));
        // 出発日
        sb.AppendLine("  ," + setUpdateParam(param.SyuptDay, busComKakuteiNotifiEtt.SyuptDay));
        // 年
        sb.AppendLine("  ," + setUpdateParam(param.Year, busComKakuteiNotifiEtt.Year));
        // 予約区分
        sb.AppendLine("  ," + setUpdateParam(param.YoyakuKbn, busComKakuteiNotifiEtt.YoyakuKbn));
        // 予約ＮＯ
        sb.AppendLine("  ," + setUpdateParam(param.YoyakuNo, busComKakuteiNotifiEtt.YoyakuNo));
        // システム登録ＰＧＭＩＤ
        sb.AppendLine("  ," + setUpdateParam(param.SystemEntryPgmid, busComKakuteiNotifiEtt.SystemEntryPgmid));
        // システム登録者コード
        sb.AppendLine("  ," + setUpdateParam(param.SystemEntryPersonCd, busComKakuteiNotifiEtt.SystemEntryPersonCd));
        // システム登録日
        sb.AppendLine("  ," + setUpdateParam(param.SystemEntryDay, busComKakuteiNotifiEtt.SystemEntryDay));
        // システム更新ＰＧＭＩＤ
        sb.AppendLine("  ," + setUpdateParam(param.SystemUpdatePgmid, busComKakuteiNotifiEtt.SystemUpdatePgmid));
        // システム更新者コード
        sb.AppendLine("  ," + setUpdateParam(param.SystemUpdatePersonCd, busComKakuteiNotifiEtt.SystemUpdatePersonCd));
        // システム更新日
        sb.AppendLine("  ," + setUpdateParam(param.SystemUpdateDay, busComKakuteiNotifiEtt.SystemUpdateDay));
        sb.AppendLine("  )");
        return sb.ToString();
    }
    #endregion

    #region  パラメータ 
    public partial class S03_0502DASelectParam
    {
        /// <summary>
        /// 出発日FROM
        /// </summary>
        public int? SyuptDayFrom { get; set; }
        /// <summary>
        /// 出発日TO
        /// </summary>
        public int? SyuptDayTo { get; set; }
        /// <summary>
        /// コースコード
        /// </summary>
        public string CrsCd { get; set; }
        /// <summary>
        /// 予約区分
        /// </summary>
        public string YoyakuKbn { get; set; }
        /// <summary>
        /// 予約No
        /// </summary>
        public int YoyakuNo { get; set; }
        /// <summary>
        /// 号車
        /// </summary>
        public int Gousya { get; set; }
        /// <summary>
        /// 予約者
        /// </summary>
        public bool YoyakuMember { get; set; }
        /// <summary>
        /// 代理店
        /// </summary>
        public bool YoyakuAgent { get; set; }
        /// <summary>
        /// メール
        /// </summary>
        public bool SendMail { get; set; }
        /// <summary>
        /// FAX
        /// </summary>
        public bool SendFax { get; set; }
        /// <summary>
        /// 郵送
        /// </summary>
        public bool SendPost { get; set; }
        /// <summary>
        /// 不要
        /// </summary>
        public bool SendNothing { get; set; }
        /// <summary>
        /// 通知済
        /// </summary>
        public bool SendFinish { get; set; }
        /// <summary>
        /// 未通知
        /// </summary>
        public bool NotSend { get; set; }
    }

    public partial class S03_0502DADeleteParam
    {
        /// <summary>
        /// コースコード
        /// </summary>
        public string CrsCd { get; set; }
        /// <summary>
        /// 号車
        /// </summary>
        public int Gousya { get; set; }
        /// <summary>
        /// 出発日
        /// </summary>
        public int SyuptDay { get; set; }
        /// <summary>
        /// 予約区分
        /// </summary>
        public string YoyakuKbn { get; set; }
        /// <summary>
        /// 予約No
        /// </summary>
        public int YoyakuNo { get; set; }
    }

    public partial class S03_0502DAInsertParam
    {
        /// <summary>
        /// 業者コード
        /// </summary>
        public string AgentCd { get; set; }
        /// <summary>
        /// 業者名称
        /// </summary>
        public string AgentNm { get; set; }
        /// <summary>
        /// バス会社コード
        /// </summary>
        public string BusCompanyCd { get; set; }
        /// <summary>
        /// コースコード
        /// </summary>
        public string CrsCd { get; set; }
        /// <summary>
        /// 号車
        /// </summary>
        public int? Gousya { get; set; }
        /// <summary>
        /// 確定通知日
        /// </summary>
        public int? KakuteiNotificationDay { get; set; }
        /// <summary>
        /// 通知方法
        /// </summary>
        public string NotificationHoho { get; set; }
        /// <summary>
        /// 出発日
        /// </summary>
        public int? SyuptDay { get; set; }
        /// <summary>
        /// 年
        /// </summary>
        public int? Year { get; set; }
        /// <summary>
        /// 予約区分
        /// </summary>
        public string YoyakuKbn { get; set; }
        /// <summary>
        /// 予約ＮＯ
        /// </summary>
        public int YoyakuNo { get; set; }
        /// <summary>
        /// システム登録ＰＧＭＩＤ
        /// </summary>
        public string SystemEntryPgmid { get; set; }
        /// <summary>
        /// システム登録者コード
        /// </summary>
        public string SystemEntryPersonCd { get; set; }
        /// <summary>
        /// システム登録日
        /// </summary>
        public DateTime SystemEntryDay { get; set; }
        /// <summary>
        /// システム更新ＰＧＭＩＤ
        /// </summary>
        public string SystemUpdatePgmid { get; set; }
        /// <summary>
        /// システム更新者コード
        /// </summary>
        public string SystemUpdatePersonCd { get; set; }
        /// <summary>
        /// システム更新日
        /// </summary>
        public DateTime SystemUpdateDay { get; set; }
    }

    public partial class S03_0502DAGetNotifiContentParam
    {
        /// <summary>
        /// コード分類
        /// </summary>
        public string CodeBunrui { get; set; }
    }
    #endregion

}