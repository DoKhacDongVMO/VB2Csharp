Imports System.Text
''' <summary>
''' 通知先設定のDAクラス
''' </summary>
Public Class S03_0408DA
    Inherits DataAccessorBase
#Region " 定数／変数 "
    Private ParamNum As Integer = 0
#End Region
#Region " SELECT処理 "
    ''' <summary>
    ''' 予約者（最終確認連絡）検索処理を呼び出す
    ''' </summary>
    ''' <param name="param">検索パラメータ</param>
    ''' <returns>検索SQL</returns>
    Public Function selectYoyakuLastHisDataTable(ByVal param As S03_0408DASelectParam) As DataTable
        Dim crsLedgerBasic As New CrsLedgerBasicEntity
        'SQL文字列
        Dim sb As New StringBuilder
        'パラメータクリア
        clear()

        'SELECT句
        sb.AppendLine("SELECT ")
        sb.AppendLine("    TO_YYYYMMDD_FC(BASIC.SYUPT_DAY) AS SYUPT_DAY_STR")
        sb.AppendLine("    , BASIC.SYUPT_DAY")
        sb.AppendLine("    , BASIC.CRS_CD")
        sb.AppendLine("    , BASIC.GOUSYA")
        sb.AppendLine("    , BASIC.CRS_NAME")
        sb.AppendLine("    , '' AS DAILY, '' AS LINE_NO")
        sb.AppendLine("    , INFO.YOYAKU_KBN")
        sb.AppendLine("    , INFO.YOYAKU_NO")
        sb.AppendLine("    , INFO.YOYAKU_KBN || INFO.YOYAKU_NO AS YOYAKU_NUMBER")
        sb.AppendLine("    , INFO.SURNAME || '　' || INFO.NAME AS YOYAKU_NAME")
        sb.AppendLine("    , '' AS AGENT_CD, '' AS AGENT_NAME")
        sb.AppendLine("    , INFO.YOYAKU_UKETUKE_KBN")
        sb.AppendLine("    , CASE WHEN KAIIN.MAIL_ADDRESS IS NOT NULL THEN '会員'")
        sb.AppendLine("      WHEN INFO.YOYAKU_UKETUKE_KBN = '2' THEN 'インターネット'")
        sb.AppendLine("      END AS YOYAKU_STATU")
        sb.AppendLine("    , '' AS SIIRE_CD, '' AS SIIRE_SAKI_NAME, '' AS SIIRE_KIND")
        sb.AppendLine("    , '1' AS NOTIFICATION_HOHO")
        sb.AppendLine("    , INFO.TEL_NO_1_1 || '-' || INFO.TEL_NO_1_2 || '-' || INFO.TEL_NO_1_3 AS TEL_STR")
        sb.AppendLine("    , INFO.TEL_NO_1 AS TEL1")
        sb.AppendLine("    , INFO.TEL_NO_1_1 AS TEL1_1")
        sb.AppendLine("    , INFO.TEL_NO_1_2 AS TEL1_2")
        sb.AppendLine("    , INFO.TEL_NO_1_3 AS TEL1_3")
        sb.AppendLine("    , '' AS FAX_STR, '' AS FAX1, '' AS FAX1_1, '' AS FAX1_2, '' AS FAX1_3")
        sb.AppendLine("    , INFO.MAIL_ADDRESS AS MAIL")
        sb.AppendLine("    , '' AS NOTIFICATION_HOHO_RIREKI")
        sb.AppendLine("    , TO_CHAR(CONTACT.CONTACT_DAY, 'YYYY/MM/DD HH24:MI') AS CONTACT_DAY")
        sb.AppendLine("    , '3' AS CONTACT_TO")
        'FROM句
        sb.AppendLine("FROM T_CRS_LEDGER_BASIC BASIC")
        sb.AppendLine("INNER JOIN T_YOYAKU_INFO_BASIC INFO")
        sb.AppendLine("    ON BASIC.CRS_CD = INFO.CRS_CD")
        sb.AppendLine("    AND BASIC.SYUPT_DAY = INFO.SYUPT_DAY")
        sb.AppendLine("    AND BASIC.GOUSYA = INFO.GOUSYA")
        sb.AppendLine("LEFT JOIN M_KAIIN KAIIN")
        sb.AppendLine("    ON KAIIN.KAIIN_NO = INFO.KAIIN_NO")
        sb.AppendLine("LEFT JOIN T_LAST_CONTACT_HISTORY CONTACT")
        sb.AppendLine("    ON CONTACT.YOYAKU_KBN = INFO.YOYAKU_KBN")
        sb.AppendLine("    AND CONTACT.YOYAKU_NO = INFO.YOYAKU_NO")
        sb.AppendLine("    AND CONTACT.CONTACT_DAY = (SELECT MAX(CONTACT_DAY) FROM T_LAST_CONTACT_HISTORY")
        sb.AppendLine("        WHERE YOYAKU_KBN = INFO.YOYAKU_KBN AND YOYAKU_NO = INFO.YOYAKU_NO)")
        'WHERE句
        sb.AppendLine("WHERE")
        sb.AppendLine("    NVL(BASIC.MARU_ZOU_MANAGEMENT_KBN,'0')<>'M'")
        sb.AppendLine("    AND NVL(BASIC.DELETE_DAY, 0) = 0")
        sb.AppendLine("    AND INFO.YOYAKU_KBN >= '0' AND INFO.YOYAKU_KBN <= '9'")
        sb.AppendLine("    AND INFO.CANCEL_FLG IS NULL")
        sb.AppendLine("    AND (INFO.YOYAKU_UKETUKE_KBN = '2' OR KAIIN.MAIL_ADDRESS IS NOT NULL)")
        '出発日
        'コースコード
        '号車
        sb.Append("    AND (BASIC.SYUPT_DAY, BASIC.CRS_CD")
        If param.Gousya.Count > 0 Then
            sb.Append(", BASIC.GOUSYA")
        End If
        sb.Append(") IN (")
        For i As Integer = 0 To param.SyuptDay.Count - 1
            sb.Append("(").Append(setSelectParam(param.SyuptDay(i), crsLedgerBasic.syuptDay))
            sb.Append(",").Append(setSelectParam(param.CrsCd(i), crsLedgerBasic.crsCd))
            If param.Gousya.Count > 0 Then
                sb.Append(",").Append(setSelectParam(param.Gousya(i), crsLedgerBasic.gousya))
            End If
            sb.Append("),")
        Next
        sb.Remove(sb.Length - 1, 1)
        sb.Append(")")
        'ORDER句
        sb.AppendLine("ORDER BY")
        sb.AppendLine("      BASIC.SYUPT_DAY")
        sb.AppendLine("    , BASIC.CRS_CD")
        sb.AppendLine("    , BASIC.GOUSYA")
        sb.AppendLine("    , INFO.YOYAKU_KBN")
        sb.AppendLine("    , INFO.YOYAKU_NO")
        Return MyBase.getDataTable(sb.ToString)
    End Function
    ''' <summary>
    ''' 予約者（催行決定中止連絡）検索処理を呼び出す
    ''' </summary>
    ''' <param name="param">検索パラメータ</param>
    ''' <returns>検索SQL</returns>
    Public Function selectYoyakuSaikouHisDataTable(ByVal param As S03_0408DASelectParam) As DataTable
        Dim crsLedgerBasic As New CrsLedgerBasicEntity
        'SQL文字列
        Dim sb As New StringBuilder
        'パラメータクリア
        clear()

        'SELECT句
        sb.AppendLine("SELECT ")
        sb.AppendLine("    TO_YYYYMMDD_FC(BASIC.SYUPT_DAY) AS SYUPT_DAY_STR")
        sb.AppendLine("    , BASIC.SYUPT_DAY")
        sb.AppendLine("    , BASIC.CRS_CD")
        sb.AppendLine("    , BASIC.GOUSYA")
        sb.AppendLine("    , BASIC.CRS_NAME")
        sb.AppendLine("    , '' AS DAILY, '' AS LINE_NO")
        sb.AppendLine("    , INFO.YOYAKU_KBN")
        sb.AppendLine("    , INFO.YOYAKU_NO")
        sb.AppendLine("    , INFO.YOYAKU_KBN || INFO.YOYAKU_NO AS YOYAKU_NUMBER ")
        sb.AppendLine("    , INFO.SURNAME || '　' || INFO.NAME AS YOYAKU_NAME")
        sb.AppendLine("    , '' AS AGENT_CD, '' AS AGENT_NAME")
        sb.AppendLine("    , INFO.YOYAKU_UKETUKE_KBN")
        sb.AppendLine("    , CASE WHEN KAIIN.MAIL_ADDRESS IS NOT NULL THEN '会員'")
        sb.AppendLine("      WHEN INFO.YOYAKU_UKETUKE_KBN = '2' THEN 'インターネット' ")
        sb.AppendLine("      END AS YOYAKU_STATU")
        sb.AppendLine("    , '' AS SIIRE_CD, '' AS SIIRE_SAKI_NAME, '' AS SIIRE_KIND")
        sb.AppendLine("    , CASE WHEN BASIC.HOUJIN_GAIKYAKU_KBN = 'G' THEN '1'")
        sb.AppendLine("      WHEN KAIIN.MAIL_ADDRESS IS NOT NULL THEN '1'")
        sb.AppendLine("      WHEN INFO.YOYAKU_UKETUKE_KBN = '2' THEN '1'")
        sb.AppendLine("      WHEN INFO.MAIL_SENDING_KBN = 'Y' THEN '1'")
        sb.AppendLine("      ELSE '3' END AS NOTIFICATION_HOHO")
        sb.AppendLine("    , INFO.TEL_NO_1_1 || '-' || INFO.TEL_NO_1_2 || '-' || INFO.TEL_NO_1_3 AS TEL_STR")
        sb.AppendLine("    , INFO.TEL_NO_1 AS TEL1")
        sb.AppendLine("    , INFO.TEL_NO_1_1 AS TEL1_1")
        sb.AppendLine("    , INFO.TEL_NO_1_2 AS TEL1_2")
        sb.AppendLine("    , INFO.TEL_NO_1_3 AS TEL1_3")
        sb.AppendLine("    , '' AS FAX_STR, '' AS FAX1, '' AS FAX1_1, '' AS FAX1_2, '' AS FAX1_3")
        sb.AppendLine("    , INFO.MAIL_ADDRESS AS MAIL")
        sb.AppendLine("    , CASE WHEN CONTACT.CONTACT_METHOD = '1' THEN 'メール'")
        sb.AppendLine("      WHEN CONTACT.CONTACT_METHOD = '2' THEN 'FAX'")
        sb.AppendLine("      WHEN CONTACT.CONTACT_METHOD = '3' THEN 'TEL'")
        sb.AppendLine("      WHEN CONTACT.CONTACT_METHOD = '4' THEN '郵送'")
        sb.AppendLine("      END AS NOTIFICATION_HOHO_RIREKI")
        sb.AppendLine("    , TO_CHAR(CONTACT.CONTACT_DAY, 'YYYY/MM/DD HH24:MI') AS CONTACT_DAY")
        sb.AppendLine("    , '3' AS CONTACT_TO")
        'FROM句
        sb.AppendLine("FROM T_CRS_LEDGER_BASIC BASIC")
        sb.AppendLine("INNER JOIN T_YOYAKU_INFO_BASIC INFO")
        sb.AppendLine("    ON BASIC.CRS_CD = INFO.CRS_CD")
        sb.AppendLine("    AND BASIC.SYUPT_DAY = INFO.SYUPT_DAY")
        sb.AppendLine("    AND BASIC.GOUSYA = INFO.GOUSYA")
        sb.AppendLine("LEFT JOIN M_KAIIN KAIIN")
        sb.AppendLine("    ON KAIIN.KAIIN_NO = INFO.KAIIN_NO")
        sb.AppendLine("    AND KAIIN.MAIL_ADDRESS IS NOT NULL")
        sb.AppendLine("LEFT JOIN T_SAIKOU_CONTACT_HISTORY CONTACT")
        sb.AppendLine("    ON CONTACT.YOYAKU_KBN = INFO.YOYAKU_KBN")
        sb.AppendLine("    AND CONTACT.YOYAKU_NO = INFO.YOYAKU_NO")
        sb.AppendLine("    AND CONTACT.CONTACT_DAY = (SELECT MAX(CONTACT_DAY) FROM T_SAIKOU_CONTACT_HISTORY")
        sb.AppendLine("        WHERE YOYAKU_KBN = INFO.YOYAKU_KBN AND YOYAKU_NO = INFO.YOYAKU_NO)")
        'WHERE句
        sb.AppendLine("WHERE")
        sb.AppendLine("    NVL(BASIC.MARU_ZOU_MANAGEMENT_KBN,'0')<>'M'")
        sb.AppendLine("    AND NVL(BASIC.DELETE_DAY, 0) = 0")
        sb.AppendLine("    AND INFO.YOYAKU_KBN >= '0' AND INFO.YOYAKU_KBN <= '9'")
        sb.AppendLine("    AND INFO.CANCEL_FLG IS NULL")
        sb.AppendLine("    AND (INFO.AGENT_CD IS NULL OR SUBSTR(INFO.AGENT_CD,1,4) IN ('0001','0002'))")
        '出発日
        'コースコード
        sb.Append("    AND (BASIC.SYUPT_DAY, BASIC.CRS_CD) IN (")
        For i As Integer = 0 To param.SyuptDay.Count - 1
            sb.Append("(").Append(setSelectParam(param.SyuptDay(i), crsLedgerBasic.syuptDay))
            sb.Append(",").Append(setSelectParam(param.CrsCd(i), crsLedgerBasic.crsCd))
            sb.Append("),")
        Next
        sb.Remove(sb.Length - 1, 1)
        sb.Append(")")
        'ORDER句
        sb.AppendLine("ORDER BY")
        sb.AppendLine("      BASIC.SYUPT_DAY")
        sb.AppendLine("    , BASIC.CRS_CD")
        sb.AppendLine("    , BASIC.GOUSYA")
        sb.AppendLine("    , INFO.YOYAKU_KBN")
        sb.AppendLine("    , INFO.YOYAKU_NO")
        Return MyBase.getDataTable(sb.ToString)
    End Function
    ''' <summary>
    ''' 代理店（催行決定中止連絡）検索処理を呼び出す
    ''' </summary>
    ''' <param name="param">検索パラメータ</param>
    ''' <returns>検索SQL</returns>
    Public Function selectAgentSaikouHisDataTable(ByVal param As S03_0408DASelectParam) As DataTable
        Dim crsLedgerBasic As New CrsLedgerBasicEntity
        'SQL文字列
        Dim sb As New StringBuilder
        'パラメータクリア
        clear()
        'SELECT句
        sb.AppendLine("SELECT ")
        sb.AppendLine("    TO_YYYYMMDD_FC(BASIC.SYUPT_DAY) AS SYUPT_DAY_STR")
        sb.AppendLine("    , BASIC.SYUPT_DAY")
        sb.AppendLine("    , BASIC.CRS_CD")
        sb.AppendLine("    , BASIC.GOUSYA")
        sb.AppendLine("    , BASIC.CRS_NAME")
        sb.AppendLine("    , '' AS DAILY, '' AS LINE_NO")
        sb.AppendLine("    , INFO.YOYAKU_KBN")
        sb.AppendLine("    , INFO.YOYAKU_NO")
        sb.AppendLine("    , INFO.YOYAKU_KBN || INFO.YOYAKU_NO AS YOYAKU_NUMBER ")
        sb.AppendLine("    , INFO.SURNAME || '　' || INFO.NAME AS YOYAKU_NAME")
        sb.AppendLine("    , INFO.AGENT_CD")
        sb.AppendLine("    , AGENT.AGENT_NAME")
        sb.AppendLine("    , INFO.YOYAKU_UKETUKE_KBN")
        sb.AppendLine("    , CASE WHEN INFO.YOYAKU_UKETUKE_KBN = '2' THEN 'インターネット' ")
        sb.AppendLine("      END AS YOYAKU_STATU")
        sb.AppendLine("    , '' AS SIIRE_CD, '' AS SIIRE_SAKI_NAME, '' AS SIIRE_KIND")
        sb.AppendLine("    , CASE WHEN TRIM(AGENT.NOTIFICATION_HOHO) IN ('1','2','3','4') ")
        sb.AppendLine("      THEN TRIM(AGENT.NOTIFICATION_HOHO) END AS NOTIFICATION_HOHO ")
        sb.AppendLine("    , INFO.AGENT_TEL_NO_1 || '-' || INFO.AGENT_TEL_NO_2 || '-' || INFO.AGENT_TEL_NO_3 AS TEL_STR")
        sb.AppendLine("    , INFO.AGENT_TEL_NO AS TEL1")
        sb.AppendLine("    , INFO.AGENT_TEL_NO_1 AS TEL1_1")
        sb.AppendLine("    , INFO.AGENT_TEL_NO_2 AS TEL1_2")
        sb.AppendLine("    , INFO.AGENT_TEL_NO_3 AS TEL1_3")
        sb.AppendLine("    , AGENT.FAX_1 || '-' || AGENT.FAX_2 || '-' || AGENT.FAX_3 AS FAX_STR")
        sb.AppendLine("    , AGENT.FAX AS FAX1")
        sb.AppendLine("    , AGENT.FAX_1 AS FAX1_1")
        sb.AppendLine("    , AGENT.FAX_2 AS FAX1_2")
        sb.AppendLine("    , AGENT.FAX_3 AS FAX1_3")
        sb.AppendLine("    , AGENT.MAIL")
        sb.AppendLine("    , INFO.TEL_NO_1_1 || '-' || INFO.TEL_NO_1_2 || '-' || INFO.TEL_NO_1_3 AS TEL_STR_YOYAKU")
        sb.AppendLine("    , INFO.TEL_NO_1 AS TEL1_YOYAKU")
        sb.AppendLine("    , INFO.TEL_NO_1_1 AS TEL1_1_YOYAKU")
        sb.AppendLine("    , INFO.TEL_NO_1_2 AS TEL1_2_YOYAKU")
        sb.AppendLine("    , INFO.TEL_NO_1_3 AS TEL1_3_YOYAKU")
        sb.AppendLine("    , INFO.MAIL_ADDRESS AS MAIL_STR_YOYAKU")
        sb.AppendLine("    , CASE WHEN CONTACT.CONTACT_METHOD = '1' THEN 'メール'")
        sb.AppendLine("      WHEN CONTACT.CONTACT_METHOD = '2' THEN 'FAX'")
        sb.AppendLine("      WHEN CONTACT.CONTACT_METHOD = '3' THEN 'TEL'")
        sb.AppendLine("      WHEN CONTACT.CONTACT_METHOD = '4' THEN '郵送'")
        sb.AppendLine("      END AS NOTIFICATION_HOHO_RIREKI")
        sb.AppendLine("    , TO_CHAR(CONTACT.CONTACT_DAY, 'YYYY/MM/DD HH24:MI') AS CONTACT_DAY")
        sb.AppendLine("    , '2' AS CONTACT_TO")
        'FROM句
        sb.AppendLine("FROM T_CRS_LEDGER_BASIC BASIC")
        sb.AppendLine("INNER JOIN T_YOYAKU_INFO_BASIC INFO")
        sb.AppendLine("    ON BASIC.CRS_CD = INFO.CRS_CD")
        sb.AppendLine("    AND BASIC.SYUPT_DAY = INFO.SYUPT_DAY")
        sb.AppendLine("    AND BASIC.GOUSYA = INFO.GOUSYA")
        sb.AppendLine("LEFT JOIN M_AGENT AGENT")
        sb.AppendLine("    ON AGENT.AGENT_CD = INFO.AGENT_CD")
        sb.AppendLine("    AND AGENT.DELETE_DATE IS NULL")
        sb.AppendLine("LEFT JOIN T_SAIKOU_CONTACT_HISTORY CONTACT")
        sb.AppendLine("    ON CONTACT.YOYAKU_KBN = INFO.YOYAKU_KBN")
        sb.AppendLine("    AND CONTACT.YOYAKU_NO = INFO.YOYAKU_NO")
        sb.AppendLine("    AND CONTACT.CONTACT_DAY = (SELECT MAX(CONTACT_DAY) FROM T_SAIKOU_CONTACT_HISTORY")
        sb.AppendLine("        WHERE YOYAKU_KBN = INFO.YOYAKU_KBN AND YOYAKU_NO = INFO.YOYAKU_NO)")
        'WHERE句
        sb.AppendLine("WHERE")
        sb.AppendLine("    NVL(BASIC.MARU_ZOU_MANAGEMENT_KBN,'0')<>'M'")
        sb.AppendLine("    AND NVL(BASIC.DELETE_DAY, 0) = 0")
        sb.AppendLine("    AND INFO.YOYAKU_KBN >= '0' AND INFO.YOYAKU_KBN <= '9'")
        sb.AppendLine("    AND INFO.CANCEL_FLG IS NULL")
        sb.AppendLine("    AND (INFO.AGENT_CD IS NOT NULL AND SUBSTR(INFO.AGENT_CD,1,4) NOT IN ('0001','0002'))")

        '出発日
        'コースコード
        sb.Append("    AND (BASIC.SYUPT_DAY, BASIC.CRS_CD) IN (")
        For i As Integer = 0 To param.SyuptDay.Count - 1
            sb.Append("(").Append(setSelectParam(param.SyuptDay(i), crsLedgerBasic.syuptDay))
            sb.Append(",").Append(setSelectParam(param.CrsCd(i), crsLedgerBasic.crsCd))
            sb.Append("),")
        Next
        sb.Remove(sb.Length - 1, 1)
        sb.Append(")")
        'ORDER句
        sb.AppendLine("ORDER BY")
        sb.AppendLine("      BASIC.SYUPT_DAY")
        sb.AppendLine("    , BASIC.CRS_CD")
        sb.AppendLine("    , BASIC.GOUSYA")
        sb.AppendLine("    , INFO.YOYAKU_KBN")
        sb.AppendLine("    , INFO.YOYAKU_NO")
        Return MyBase.getDataTable(sb.ToString)
    End Function
    ''' <summary>
    ''' 仕入先（催行決定中止連絡）検索処理を呼び出す
    ''' </summary>
    ''' <param name="param">検索パラメータ</param>
    ''' <returns>検索SQL</returns>
    Public Function selectSiireSaikouHisDataTable(ByVal param As S03_0408DASelectParam) As DataTable
        Dim crsLedgerBasic As New CrsLedgerBasicEntity
        'SQL文字列
        Dim sb As New StringBuilder
        'パラメータクリア
        clear()

        'SELECT句
        sb.AppendLine("SELECT ")
        sb.AppendLine("    TO_YYYYMMDD_FC(BASIC.SYUPT_DAY) AS SYUPT_DAY_STR")
        sb.AppendLine("    , BASIC.SYUPT_DAY, BASIC.CRS_CD, BASIC.GOUSYA, BASIC.CRS_NAME")
        sb.AppendLine("    , LEDGER.DAILY, LEDGER.LINE_NO")
        sb.AppendLine("    , '' AS YOYAKU_KBN, '' AS YOYAKU_NO, '' AS YOYAKU_NUMBER, '' AS YOYAKU_NAME")
        sb.AppendLine("    , '' AS AGENT_CD, '' AS AGENT_NAME")
        sb.AppendLine("    , '' AS YOYAKU_UKETUKE_KBN, '' AS YOYAKU_STATU")
        sb.AppendLine("    , LEDGER.SIIRE_SAKI_CD || LEDGER.SIIRE_SAKI_NO AS SIIRE_CD")
        sb.AppendLine("    , LEDGER.SIIRE_SAKI_NAME, CODE.CODE_NAME AS SIIRE_KIND")
        sb.AppendLine("    , CASE WHEN TRIM(LEDGER.NOTIFICATION_HOHO) IN ('1','2')")
        sb.AppendLine("      THEN TRIM(LEDGER.NOTIFICATION_HOHO) END AS NOTIFICATION_HOHO")
        sb.AppendLine("    , LEDGER.TELNO_1_1 || '-' || LEDGER.TELNO_1_2 || '-' || LEDGER.TELNO_1_3 AS TEL_STR")
        sb.AppendLine("    , LEDGER.TELNO_1 AS TEL1, LEDGER.TELNO_1_1 AS TEL1_1, LEDGER.TELNO_1_2 AS TEL1_2, LEDGER.TELNO_1_3 AS TEL1_3")
        sb.AppendLine("    , LEDGER.FAX_1_1 || '-' || LEDGER.FAX_1_2 || '-' || LEDGER.FAX_1_3 AS FAX_STR")
        sb.AppendLine("    , LEDGER.FAX_1 AS FAX1, LEDGER.FAX_1_1 AS FAX1_1, LEDGER.FAX_1_2 AS FAX1_2, LEDGER.FAX_1_3 AS FAX1_3")
        sb.AppendLine("    , LEDGER.MAIL")
        sb.AppendLine("    , CASE WHEN CONTACT.CONTACT_METHOD = '1' THEN 'メール'")
        sb.AppendLine("      WHEN CONTACT.CONTACT_METHOD = '2' THEN 'FAX'")
        sb.AppendLine("      WHEN CONTACT.CONTACT_METHOD = '3' THEN 'TEL'")
        sb.AppendLine("      WHEN CONTACT.CONTACT_METHOD = '4' THEN '郵送'")
        sb.AppendLine("      END AS NOTIFICATION_HOHO_RIREKI")
        sb.AppendLine("    , TO_CHAR(CONTACT.CONTACT_DAY, 'YYYY/MM/DD HH24:MI') AS CONTACT_DAY")
        sb.AppendLine("    , '1' AS CONTACT_TO")
        'FROM句
        sb.AppendLine("FROM T_CRS_LEDGER_BASIC BASIC INNER JOIN ")
        sb.AppendLine("(")
        sb.AppendLine("SELECT ")
        sb.AppendLine("    KOSHAKASHO.CRS_CD, KOSHAKASHO.SYUPT_DAY, KOSHAKASHO.GOUSYA")
        sb.AppendLine("    , KOSHAKASHO.DAILY, KOSHAKASHO.LINE_NO, SIIRE.*")
        sb.AppendLine("    , ROW_NUMBER() OVER (PARTITION BY KOSHAKASHO.KOSHAKASHO_CD, KOSHAKASHO.KOSHAKASHO_EDABAN,")
        sb.AppendLine("                   KOSHAKASHO.CRS_CD, KOSHAKASHO.SYUPT_DAY, KOSHAKASHO.GOUSYA ORDER BY LINE_NO) AS ROW_NUM")
        sb.AppendLine("FROM T_CRS_LEDGER_KOSHAKASHO KOSHAKASHO")
        sb.AppendLine("INNER JOIN M_SIIRE_SAKI SIIRE")
        sb.AppendLine("    ON SIIRE.SIIRE_SAKI_CD = KOSHAKASHO.KOSHAKASHO_CD")
        sb.AppendLine("    AND SIIRE.SIIRE_SAKI_NO = KOSHAKASHO.KOSHAKASHO_EDABAN")
        sb.AppendLine("    AND SIIRE.SIIRE_SAKI_KIND_CD NOT IN ('35','50','99')")
        sb.AppendLine("    AND SIIRE.DELETE_DATE IS NULL")
        sb.AppendLine("UNION ALL")
        sb.AppendLine("SELECT ")
        sb.AppendLine("    HOTEL.CRS_CD, HOTEL.SYUPT_DAY, HOTEL.GOUSYA")
        sb.AppendLine("    , HOTEL.DAILY, HOTEL.LINE_NO, SIIRE.*")
        sb.AppendLine("    , ROW_NUMBER() OVER (PARTITION BY HOTEL.SIIRE_SAKI_CD, HOTEL.SIIRE_SAKI_EDABAN, ")
        sb.AppendLine("                   HOTEL.CRS_CD, HOTEL.SYUPT_DAY, HOTEL.GOUSYA ORDER BY LINE_NO) AS ROW_NUM")
        sb.AppendLine("FROM T_CRS_LEDGER_HOTEL HOTEL")
        sb.AppendLine("INNER JOIN M_SIIRE_SAKI SIIRE")
        sb.AppendLine("    ON SIIRE.SIIRE_SAKI_CD = HOTEL.SIIRE_SAKI_CD")
        sb.AppendLine("    AND SIIRE.SIIRE_SAKI_NO = HOTEL.SIIRE_SAKI_EDABAN")
        sb.AppendLine("    AND SIIRE.SIIRE_SAKI_KIND_CD = '30'")
        sb.AppendLine("    AND SIIRE.DELETE_DATE IS NULL")
        sb.AppendLine(")LEDGER")
        sb.AppendLine("    ON BASIC.CRS_CD = LEDGER.CRS_CD")
        sb.AppendLine("    AND BASIC.SYUPT_DAY = LEDGER.SYUPT_DAY")
        sb.AppendLine("    AND BASIC.GOUSYA = LEDGER.GOUSYA")
        sb.AppendLine("LEFT JOIN M_CODE CODE")
        sb.AppendLine("    ON LEDGER.SIIRE_SAKI_KIND_CD = CODE.CODE_VALUE")
        sb.AppendLine("    AND CODE.CODE_BUNRUI = '157'")
        sb.AppendLine("    AND CODE.DELETE_DATE IS NULL")
        sb.AppendLine("LEFT JOIN T_SAIKOU_CONTACT_HISTORY CONTACT")
        sb.AppendLine("    ON CONTACT.CRS_CD = LEDGER.CRS_CD")
        sb.AppendLine("    AND CONTACT.SYUPT_DAY = LEDGER.SYUPT_DAY")
        sb.AppendLine("    AND CONTACT.GOUSYA = LEDGER.GOUSYA")
        sb.AppendLine("    AND NVL(CONTACT.DAILY, 0) = NVL(LEDGER.DAILY, 0)")
        sb.AppendLine("    AND CONTACT.LINE_NO = LEDGER.LINE_NO")
        sb.AppendLine("    AND CONTACT.SIIRE_SAKI_CD = LEDGER.SIIRE_SAKI_CD")
        sb.AppendLine("    AND CONTACT.SIIRE_SAKI_EDABAN = LEDGER.SIIRE_SAKI_NO")
        sb.AppendLine("    AND (CONTACT.CONTACT_DAY = (SELECT MAX(CONTACT_DAY) FROM T_SAIKOU_CONTACT_HISTORY")
        sb.AppendLine("        WHERE CRS_CD = LEDGER.CRS_CD AND SYUPT_DAY = LEDGER.SYUPT_DAY AND GOUSYA = LEDGER.GOUSYA")
        sb.AppendLine("        AND NVL(DAILY, 0) = NVL(LEDGER.DAILY, 0) AND LINE_NO = LEDGER.LINE_NO ")
        sb.AppendLine("        AND SIIRE_SAKI_CD = LEDGER.SIIRE_SAKI_CD AND SIIRE_SAKI_EDABAN = LEDGER.SIIRE_SAKI_NO)) ")
        'WHERE句
        sb.AppendLine("WHERE ")
        sb.AppendLine("    NVL(BASIC.MARU_ZOU_MANAGEMENT_KBN,'0')<>'M'")
        sb.AppendLine("    AND NVL(BASIC.DELETE_DAY, 0) = 0")
        sb.AppendLine("    AND LEDGER.ROW_NUM = 1")
        '出発日
        'コースコード
        sb.Append("    AND (BASIC.SYUPT_DAY, BASIC.CRS_CD) IN (")
        For i As Integer = 0 To param.SyuptDay.Count - 1
            sb.Append("(").Append(setSelectParam(param.SyuptDay(i), crsLedgerBasic.syuptDay))
            sb.Append(",").Append(setSelectParam(param.CrsCd(i), crsLedgerBasic.crsCd))
            sb.Append("),")
        Next
        sb.Remove(sb.Length - 1, 1)
        sb.Append(")")
        'ORDER句
        sb.AppendLine("ORDER BY ")
        sb.AppendLine("      BASIC.SYUPT_DAY")
        sb.AppendLine("    , BASIC.CRS_CD")
        sb.AppendLine("    , BASIC.GOUSYA")
        sb.AppendLine("    , LEDGER.DAILY")
        sb.AppendLine("    , LEDGER.LINE_NO")
        Return MyBase.getDataTable(sb.ToString)
    End Function
    ''' <summary>
    ''' 通知内容取得
    ''' </summary>
    ''' <returns></returns>
    Public Function selectDataTableNotifContent() As DataTable
        Dim code As New MCodeEntity
        'フッター文言取得: 038 (手配通知フッター)
        Dim codeBunrui As String = "038"
        'SQL文字列
        Dim sb As New StringBuilder
        'パラメータクリア
        clear()
        'SELECT句
        sb.AppendLine(" SELECT ")
        sb.AppendLine("   CODE_VALUE AS CONTACT_KIND ")
        sb.AppendLine("   , NAIYO_1 AS NAIYO_1")
        'FROM句
        sb.AppendLine(" FROM M_CODE ")
        'WHERE句
        sb.AppendLine(" WHERE DELETE_DATE IS NULL ")
        'コード分類
        sb.Append("   AND CODE_BUNRUI = ").AppendLine(setSelectParam(codeBunrui, code.CodeBunrui))
        Return MyBase.getDataTable(sb.ToString)
    End Function
    Private Sub clear()
        MyBase.paramClear()
        ParamNum = 0
    End Sub
    Public Function setSelectParam(ByVal value As Object, ByVal ent As IEntityKoumokuType) As String
        Return setParamEx(value, ent, True)
    End Function
    Public Function setInsertParam(ByVal value As Object, ByVal ent As IEntityKoumokuType) As String
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
#Region " INSERT処理 "
    ''' <summary>
    ''' 登録処理（催行決定中止連絡履歴）
    ''' </summary>
    ''' <param name="lsParamInsert">催行決定中止連絡履歴追加パラメータリスト</param>
    ''' <returns>成功レコードの数 </returns>
    Public Function executeInsertSaikouHisOut(ByVal lsParamInsert As List(Of S03_0408DAInsertSaikouHisParam)) As Integer
        Dim returnValue As Integer = 0
        Dim sqlStringInsert As String = String.Empty
        'コネクション開始
        Dim con As OracleConnection = openCon()
        Try
            For row As Integer = 0 To lsParamInsert.Count - 1
                'SQL文字列(登録)
                sqlStringInsert = insertSaikouHisQuery(lsParamInsert(row))
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
    ''' <summary>
    ''' 登録（催行決定中止連絡履歴）
    ''' </summary>
    ''' <param name="param">催行決定中止連絡履歴追加パラメータ</param>
    ''' <returns>登録SQL</returns>
    Private Function insertSaikouHisQuery(ByVal param As S03_0408DAInsertSaikouHisParam) As String
        Dim saikouContactHistoryEtt As New TSaikouContactHistoryEntity
        'SQL文字列
        Dim sb As New StringBuilder
        sb.AppendLine("INSERT INTO T_SAIKOU_CONTACT_HISTORY(")
        sb.AppendLine("    SYUPT_DAY")
        sb.AppendLine("    , CRS_CD")
        sb.AppendLine("    , GOUSYA")
        sb.AppendLine("    , CONTACT_KIND")
        sb.AppendLine("    , CONTACT_TO")
        sb.AppendLine("    , YOYAKU_KBN")
        sb.AppendLine("    , YOYAKU_NO")
        sb.AppendLine("    , AGENT_CD")
        sb.AppendLine("    , SIIRE_SAKI_CD")
        sb.AppendLine("    , SIIRE_SAKI_EDABAN")
        sb.AppendLine("    , DAILY")
        sb.AppendLine("    , LINE_NO")
        sb.AppendLine("    , CONTACT_METHOD")
        sb.AppendLine("    , TEL")
        sb.AppendLine("    , FAX")
        sb.AppendLine("    , MAIL")
        sb.AppendLine("    , CONTACT_RESULT")
        sb.AppendLine("    , CONTACT_DAY")
        sb.AppendLine("    , CONTACT_PERSON_CD")
        sb.AppendLine("    , SYSTEM_ENTRY_PGMID")
        sb.AppendLine("    , SYSTEM_ENTRY_PERSON_CD")
        sb.AppendLine("    , SYSTEM_ENTRY_DAY")
        sb.AppendLine("    , SYSTEM_UPDATE_PGMID")
        sb.AppendLine("    , SYSTEM_UPDATE_PERSON_CD")
        sb.AppendLine("    , SYSTEM_UPDATE_DAY)")
        sb.AppendLine("VALUES")
        '出発日
        sb.AppendLine("  (" & setInsertParam(param.SyuptDay, saikouContactHistoryEtt.SyuptDay))
        'コースコード
        sb.AppendLine("  ," & setInsertParam(param.CrsCd, saikouContactHistoryEtt.CrsCd))
        '号車
        sb.AppendLine("  ," & setInsertParam(param.Gousya, saikouContactHistoryEtt.Gousya))
        '通知種別
        sb.AppendLine("  ," & setInsertParam(param.ContactKind, saikouContactHistoryEtt.ContactKind))
        '通知先
        sb.AppendLine("  ," & setInsertParam(param.ContactTo, saikouContactHistoryEtt.ContactTo))
        '予約区分
        sb.AppendLine("  ," & setInsertParam(param.YoyakuKbn, saikouContactHistoryEtt.YoyakuKbn))
        '予約ＮＯ
        sb.AppendLine("  ," & setInsertParam(param.YoyakuNo, saikouContactHistoryEtt.YoyakuNo))
        '業者コード
        sb.AppendLine("  ," & setInsertParam(param.AgentCd, saikouContactHistoryEtt.AgentCd))
        '仕入先コード
        sb.AppendLine("  ," & setInsertParam(param.SiireSakiCd, saikouContactHistoryEtt.SiireSakiCd))
        '仕入先枝番
        sb.AppendLine("  ," & setInsertParam(param.SiireSakiEdaban, saikouContactHistoryEtt.SiireSakiEdaban))
        '行程日次
        sb.AppendLine("  ," & setInsertParam(param.Daily, saikouContactHistoryEtt.Daily))
        '行No
        sb.AppendLine("  ," & setInsertParam(param.LineNo, saikouContactHistoryEtt.LineNo))
        '通知方法
        sb.AppendLine("  ," & setInsertParam(param.ContactMethod, saikouContactHistoryEtt.ContactMethod))
        'TEL
        sb.AppendLine("  ," & setInsertParam(param.Tel, saikouContactHistoryEtt.Tel))
        'FAX
        sb.AppendLine("  ," & setInsertParam(param.Fax, saikouContactHistoryEtt.Fax))
        'メールアドレス
        sb.AppendLine("  ," & setInsertParam(param.Mail, saikouContactHistoryEtt.Mail))
        '通知結果
        sb.AppendLine("  ," & setInsertParam(param.ContactResult, saikouContactHistoryEtt.ContactResult))
        '送信日時
        sb.AppendLine("  ," & setInsertParam(param.ContactDay, saikouContactHistoryEtt.ContactDay))
        '送信者コード
        sb.AppendLine("  ," & setInsertParam(param.ContactPersonCd, saikouContactHistoryEtt.ContactPersonCd))
        'システム登録ＰＧＭＩＤ
        sb.AppendLine("  ," & setInsertParam(param.SystemEntryPgmid, saikouContactHistoryEtt.SystemEntryPgmid))
        'システム登録者コード
        sb.AppendLine("  ," & setInsertParam(param.SystemEntryPersonCd, saikouContactHistoryEtt.SystemEntryPersonCd))
        'システム登録日
        sb.AppendLine("  ," & setInsertParam(param.SystemEntryDay, saikouContactHistoryEtt.SystemEntryDay))
        'システム更新ＰＧＭＩＤ
        sb.AppendLine("  ," & setInsertParam(param.SystemUpdatePgmid, saikouContactHistoryEtt.SystemUpdatePgmid))
        'システム更新者コード
        sb.AppendLine("  ," & setInsertParam(param.SystemUpdatePersonCd, saikouContactHistoryEtt.SystemUpdatePersonCd))
        'システム更新日
        sb.AppendLine("  ," & setInsertParam(param.SystemUpdateDay, saikouContactHistoryEtt.SystemUpdateDay))
        sb.AppendLine(")")

        Return sb.ToString()
    End Function
    ''' <summary>
    ''' 登録処理（最終確認連絡履歴）
    ''' </summary>
    ''' <param name="lsParamInsert">最終確認連絡履歴追加パラメータリスト</param>
    ''' <returns>成功レコードの数 </returns>
    Public Function executeInsertLastHisOut(ByVal lsParamInsert As List(Of S03_0408DAInsertLastHisParam)) As Integer
        Dim returnValue As Integer = 0
        Dim sqlStringInsert As String = String.Empty
        'コネクション開始
        Dim con As OracleConnection = openCon()
        Try
            For row As Integer = 0 To lsParamInsert.Count - 1
                'SQL文字列(登録)
                sqlStringInsert = insertLastHisQuery(lsParamInsert(row))
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

    ''' <summary>
    ''' 登録（最終確認連絡履歴）
    ''' </summary>
    ''' <param name="param">最終確認連絡履歴追加パラメータ</param>
    ''' <returns>登録SQL</returns>
    Private Function insertLastHisQuery(ByVal param As S03_0408DAInsertLastHisParam) As String
        Dim lastContactHistoryEtt As New TLastContactHistoryEntity
        'SQL文字列
        Dim sb As New StringBuilder
        sb.AppendLine("INSERT INTO T_LAST_CONTACT_HISTORY(")
        sb.AppendLine("    SYUPT_DAY")
        sb.AppendLine("    , CRS_CD")
        sb.AppendLine("    , GOUSYA")
        sb.AppendLine("    , YOYAKU_KBN")
        sb.AppendLine("    , YOYAKU_NO")
        sb.AppendLine("    , MAIL")
        sb.AppendLine("    , CONTACT_RESULT")
        sb.AppendLine("    , CONTACT_DAY")
        sb.AppendLine("    , CONTACT_PERSON_CD")
        sb.AppendLine("    , SYSTEM_ENTRY_PGMID")
        sb.AppendLine("    , SYSTEM_ENTRY_PERSON_CD")
        sb.AppendLine("    , SYSTEM_ENTRY_DAY")
        sb.AppendLine("    , SYSTEM_UPDATE_PGMID")
        sb.AppendLine("    , SYSTEM_UPDATE_PERSON_CD")
        sb.AppendLine("    , SYSTEM_UPDATE_DAY)")
        sb.AppendLine("VALUES")
        '出発日
        sb.AppendLine("  (" & setInsertParam(param.SyuptDay, lastContactHistoryEtt.SyuptDay))
        'コースコード
        sb.AppendLine("  ," & setInsertParam(param.CrsCd, lastContactHistoryEtt.CrsCd))
        '号車
        sb.AppendLine("  ," & setInsertParam(param.Gousya, lastContactHistoryEtt.Gousya))
        '予約区分
        sb.AppendLine("  ," & setInsertParam(param.YoyakuKbn, lastContactHistoryEtt.YoyakuKbn))
        '予約ＮＯ
        sb.AppendLine("  ," & setInsertParam(param.YoyakuNo, lastContactHistoryEtt.YoyakuNo))
        'メールアドレス
        sb.AppendLine("  ," & setInsertParam(param.Mail, lastContactHistoryEtt.Mail))
        '通知結果
        sb.AppendLine("  ," & setInsertParam(param.ContactResult, lastContactHistoryEtt.ContactResult))
        '送信日時
        sb.AppendLine("  ," & setInsertParam(param.ContactDay, lastContactHistoryEtt.ContactDay))
        '送信者コード
        sb.AppendLine("  ," & setInsertParam(param.ContactPersonCd, lastContactHistoryEtt.ContactPersonCd))
        'システム登録ＰＧＭＩＤ
        sb.AppendLine("  ," & setInsertParam(param.SystemEntryPgmid, lastContactHistoryEtt.SystemEntryPgmid))
        'システム登録者コード
        sb.AppendLine("  ," & setInsertParam(param.SystemEntryPersonCd, lastContactHistoryEtt.SystemEntryPersonCd))
        'システム登録日
        sb.AppendLine("  ," & setInsertParam(param.SystemEntryDay, lastContactHistoryEtt.SystemEntryDay))
        'システム更新ＰＧＭＩＤ
        sb.AppendLine("  ," & setInsertParam(param.SystemUpdatePgmid, lastContactHistoryEtt.SystemUpdatePgmid))
        'システム更新者コード
        sb.AppendLine("  ," & setInsertParam(param.SystemUpdatePersonCd, lastContactHistoryEtt.SystemUpdatePersonCd))
        'システム更新日
        sb.AppendLine("  ," & setInsertParam(param.SystemUpdateDay, lastContactHistoryEtt.SystemUpdateDay))
        sb.AppendLine(")")

        Return sb.ToString()
    End Function

#End Region
#Region " パラメータ "
    ''' <summary>
    ''' 検索パラメータ
    ''' </summary>
    Public Class S03_0408DASelectParam
        ''' <summary>
        ''' 出発日
        ''' </summary>
        Public Property SyuptDay As List(Of Integer) = New List(Of Integer)
        ''' <summary>
        ''' コースコード
        ''' </summary>
        Public Property CrsCd As List(Of String) = New List(Of String)
        ''' <summary>
        ''' 号車
        ''' </summary>
        Public Property Gousya As List(Of Integer?) = New List(Of Integer?)
    End Class
    ''' <summary>
    ''' 催行決定中止連絡履歴追加パラメータ
    ''' </summary>
    Public Class S03_0408DAInsertSaikouHisParam
        ''' <summary>
        ''' 出発日
        ''' </summary>
        Public Property SyuptDay As Integer?
        ''' <summary>
        ''' コースコード
        ''' </summary>
        Public Property CrsCd As String
        ''' <summary>
        ''' 号車
        ''' </summary>
        Public Property Gousya As Integer?
        ''' <summary>
        ''' 通知種別
        ''' </summary>
        Public Property ContactKind As String
        ''' <summary>
        ''' 通知先
        ''' </summary>
        Public Property ContactTo As String
        ''' <summary>
        ''' 予約区分
        ''' </summary>
        Public Property YoyakuKbn As String
        ''' <summary>
        ''' 予約ＮＯ
        ''' </summary>
        Public Property YoyakuNo As Integer?
        ''' <summary>
        ''' 業者コード
        ''' </summary>
        Public Property AgentCd As String
        ''' <summary>
        ''' 仕入先コード
        ''' </summary>
        Public Property SiireSakiCd As String
        ''' <summary>
        ''' 仕入先枝番
        ''' </summary>
        Public Property SiireSakiEdaban As String
        ''' <summary>
        ''' 行程日次
        ''' </summary>
        Public Property Daily As Integer?
        ''' <summary>
        ''' 行No
        ''' </summary>
        Public Property LineNo As Integer?
        ''' <summary>
        ''' 通知方法
        ''' </summary>
        Public Property ContactMethod As String
        ''' <summary>
        ''' TEL
        ''' </summary>
        Public Property Tel As String
        ''' <summary>
        ''' FAX
        ''' </summary>
        Public Property Fax As String
        ''' <summary>
        ''' メールアドレス
        ''' </summary>
        Public Property Mail As String
        ''' <summary>
        ''' 通知結果
        ''' </summary>
        Public Property ContactResult As String
        ''' <summary>
        ''' 送信日時
        ''' </summary>
        Public Property ContactDay As Date
        ''' <summary>
        ''' 送信者コード
        ''' </summary>
        Public Property ContactPersonCd As String
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
    ''' <summary>
    ''' 最終確認連絡履歴追加パラメータ
    ''' </summary>
    Public Class S03_0408DAInsertLastHisParam
        ''' <summary>
        ''' 出発日
        ''' </summary>
        Public Property SyuptDay As Integer
        ''' <summary>
        ''' コースコード
        ''' </summary>
        Public Property CrsCd As String
        ''' <summary>
        ''' 号車
        ''' </summary>
        Public Property Gousya As Integer
        ''' <summary>
        ''' 予約区分
        ''' </summary>
        Public Property YoyakuKbn As String
        ''' <summary>
        ''' 予約ＮＯ
        ''' </summary>
        Public Property YoyakuNo As Integer
        ''' <summary>
        ''' メールアドレス
        ''' </summary>
        Public Property Mail As String
        ''' <summary>
        ''' 通知結果
        ''' </summary>
        Public Property ContactResult As String
        ''' <summary>
        ''' 送信日時
        ''' </summary>
        Public Property ContactDay As Date
        ''' <summary>
        ''' 送信者コード
        ''' </summary>
        Public Property ContactPersonCd As String
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
#End Region
End Class
