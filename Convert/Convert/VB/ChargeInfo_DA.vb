Imports System.Text

''' <summary>
''' コース台帳一括修正（料金情報）のDAクラス
''' </summary>
Public Class ChargeInfo_DA
    Inherits DataAccessorBase

#Region " 定数／変数 "

    Public Enum accessType As Integer
        getChargeInfoHead           'ヘッダ情報取得
        getChargeInfo               '一覧結果取得検索(定期)
        getChargeInfoKikaku         '一覧結果取得検索(企画)
        executeUpdateChargeInfo     '更新(定期)
        executeUpdateChargeKikakuInfo     '更新(企画)
        executeReturnChargeInfo     '戻し
        getKbnNo                    '区分No取得
        getJininCd                  '人員コード取得
    End Enum

    ''' <summary>
    ''' パラメータキー
    ''' </summary>
    ''' <remarks></remarks>
    Public NotInheritable Class ParamHashKeys
        ''' <summary>
        ''' 定期用(KEY)
        ''' </summary>
        Public Const BASIC_KEYS_TEIKI As String = "BASIC_KEYS_TEIKI"
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
    Public Function accessChargeInfoTehai(ByVal type As accessType, Optional ByVal paramInfoList As Hashtable = Nothing) As DataTable
        'SQL文字列
        Dim sqlString As String = String.Empty
        '戻り値
        Dim returnValue As DataTable = Nothing

        Select Case type
            Case accessType.getChargeInfo
                '一覧結果取得検索
                sqlString = getChargeInfo(paramInfoList)
            Case accessType.getChargeInfoKikaku
                '一覧結果取得検索
                sqlString = getChargeInfoKikaku(paramInfoList)
            Case accessType.getChargeInfoHead
                'ヘッダ情報検索
                sqlString = getChargeInfoHead(paramInfoList)
            Case accessType.getKbnNo
                '区分No取得
                sqlString = getKbnNo(paramInfoList)
            Case accessType.getJininCd
                '人員コード取得
                sqlString = getJininCd(paramInfoList)
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
    ''' ヘッダ情報取得用SELECT
    ''' </summary>
    ''' <param name="paramList"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Protected Overloads Function getChargeInfoHead(ByVal paramList As Hashtable) As String

        Dim sqlString As New StringBuilder
        paramClear()

        'SELECT句
        sqlString.AppendLine("SELECT DISTINCT ")
        sqlString.AppendLine("  LINE_NO ")
        sqlString.AppendLine("  , CHARGE_KBN_JININ_CD ")
        sqlString.AppendLine("  , CHARGE_KBN_JININ_NAME ")
        sqlString.AppendLine("  , TEIKI_KIKAKU_KBN ")
        'FROM句
        sqlString.AppendLine("FROM ")
        sqlString.AppendLine("  ( ")
        sqlString.AppendLine("    SELECT ")
        sqlString.AppendLine("      CJK.CHARGE_KBN_JININ_CD AS CHARGE_KBN_JININ_CD ")
        sqlString.AppendLine("      , MCJK.CHARGE_KBN_JININ_NAME AS CHARGE_KBN_JININ_NAME ")
        sqlString.AppendLine("      , BASIC.TEIKI_KIKAKU_KBN AS TEIKI_KIKAKU_KBN ")
        sqlString.AppendLine("      , CJK.LINE_NO ")
        sqlString.AppendLine("      , RANK() OVER ( ")
        sqlString.AppendLine("        PARTITION BY ")
        sqlString.AppendLine("          CJK.LINE_NO ")
        sqlString.AppendLine("        ORDER BY ")
        sqlString.AppendLine("          CJK.CHARGE_KBN_JININ_CD ")
        sqlString.AppendLine("      ) RNK ")
        sqlString.AppendLine("    FROM ")
        sqlString.AppendLine("      T_CRS_LEDGER_BASIC BASIC ")
        sqlString.AppendLine("      INNER JOIN T_CRS_LEDGER_BASIC_CHARGE_KBN CJK ")
        sqlString.AppendLine("        ON BASIC.SYUPT_DAY = CJK.SYUPT_DAY ")
        sqlString.AppendLine("        AND BASIC.CRS_CD = CJK.CRS_CD ")
        sqlString.AppendLine("        AND BASIC.GOUSYA = CJK.GOUSYA ")
        sqlString.AppendLine("      LEFT JOIN M_CHARGE_JININ_KBN MCJK ")
        sqlString.AppendLine("        ON CJK.CHARGE_KBN_JININ_CD = MCJK.CHARGE_KBN_JININ_CD ")
        'WHERE句
        sqlString.AppendLine("      WHERE ")
        sqlString.AppendLine("        NVL(BASIC.DELETE_DAY,0) = 0 ")
        sqlString.AppendLine("        AND ")
        sqlString.AppendLine("        (BASIC.CRS_CD, BASIC.SYUPT_DAY) IN ( ")
        sqlString.AppendLine("        ").Append(paramList(ParamHashKeys.BASIC_KEYS_TEIKI))
        sqlString.AppendLine("        ) ")

        'ORDER BY句
        sqlString.AppendLine("  ) ")
        sqlString.AppendLine("WHERE ")
        sqlString.AppendLine("  RNK = 1 ")
        sqlString.AppendLine("ORDER BY ")
        sqlString.AppendLine("  LINE_NO ")

        Return sqlString.ToString

    End Function

    ''' <summary>
    ''' 区分No取得用SELECT
    ''' </summary>
    ''' <param name="paramList"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Protected Overloads Function getKbnNo(ByVal paramList As Hashtable) As String

        Dim sqlString As New StringBuilder
        paramClear()

        'SELECT句
        sqlString.AppendLine(" SELECT DISTINCT ")
        sqlString.AppendLine(" KBN_NO ")
        'FROM句
        sqlString.AppendLine(" FROM ")
        sqlString.AppendLine("T_CRS_LEDGER_CHARGE ")
        'WHERE句
        sqlString.AppendLine(" WHERE ")
        sqlString.AppendLine(" CRS_CD = " & setParam("CRS_CD", paramList.Item("CRSCD"), OracleDbType.Char))
        sqlString.AppendLine(" And ")
        sqlString.AppendLine(" SYUPT_DAY = " & setParam("SYUPTDAY", paramList.Item("SYUPTDAY"), OracleDbType.Decimal, 8, 0))
        sqlString.AppendLine(" And ")
        sqlString.AppendLine(" NVL(BASIC.DELETE_DAY,0) = 0 ")
        'ORDER BY句
        sqlString.AppendLine(" ORDER BY ")
        sqlString.AppendLine(" KBN_NO ")

        Return sqlString.ToString

    End Function

    ''' <summary>
    ''' 人員コード取得用SELECT
    ''' </summary>
    ''' <param name="paramList"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Protected Overloads Function getJininCd(ByVal paramList As Hashtable) As String

        Dim sqlString As New StringBuilder
        paramClear()

        sqlString.AppendLine("SELECT DISTINCT ")
        sqlString.AppendLine("  LINE_NO ")
        sqlString.AppendLine("  , LBC.CHARGE_KBN_JININ_CD ")
        sqlString.AppendLine("  , M.CHARGE_KBN_JININ_NAME ")
        sqlString.AppendLine("FROM ")
        sqlString.AppendLine("  T_CRS_LEDGER_BASIC_CHARGE_KBN LBC ")
        sqlString.AppendLine("  LEFT JOIN M_CHARGE_JININ_KBN M ")
        sqlString.AppendLine("    ON M.CHARGE_KBN_JININ_CD = LBC.CHARGE_KBN_JININ_CD ")
        sqlString.AppendLine("WHERE ")
        sqlString.AppendLine("  CRS_CD = " & setParam("CRS_CD", paramList.Item("CRSCD"), OracleDbType.Char))
        sqlString.AppendLine("  And SYUPT_DAY = " & setParam("SYUPTDAY", paramList.Item("SYUPTDAY"), OracleDbType.Decimal, 8, 0))
        sqlString.AppendLine("  And DELETE_DATE = 0 ")
        sqlString.AppendLine("ORDER BY ")
        sqlString.AppendLine("  LINE_NO ")

        Return sqlString.ToString

    End Function

    Protected Function getChargeInfoKikaku(ByVal paramList As Hashtable) As String
        Dim sqlString As New StringBuilder
        paramClear()

        'SELECT句
        sqlString.AppendLine("SELECT ")
        sqlString.AppendLine("  DISP_SYUPT_DAY ")
        sqlString.AppendLine("  , YOBI_NAME ")
        sqlString.AppendLine("  , YOYAKU_NUM ")
        sqlString.AppendLine("  , CHARGE_KBN_JININ_NAME ")
        sqlString.AppendLine("  , CHARGE_1 ")
        sqlString.AppendLine("  , CHARGE_2 ")
        sqlString.AppendLine("  , CHARGE_3 ")
        sqlString.AppendLine("  , CHARGE_4 ")
        sqlString.AppendLine("  , CHARGE_5 ")
        sqlString.AppendLine("  , CRS_CD ")
        sqlString.AppendLine("  , SYUPT_DAY ")
        sqlString.AppendLine("  , GOUSYA ")
        sqlString.AppendLine("  , CHARGE_KBN_JININ_CD ")
        sqlString.AppendLine("  , KBN_NO ")
        sqlString.AppendLine("  , NAME_1 ")
        sqlString.AppendLine("  , NAME_2 ")
        sqlString.AppendLine("  , NAME_3 ")
        sqlString.AppendLine("  , NAME_4 ")
        sqlString.AppendLine("  , NAME_5 ")
        sqlString.AppendLine("  , KEY ")
        sqlString.AppendLine("  , SYSTEM_UPDATE_PGMID ")
        sqlString.AppendLine("  , SYSTEM_UPDATE_PERSON_CD ")
        sqlString.AppendLine("  , SYSTEM_UPDATE_DAY ")
        sqlString.AppendLine("  , '  ' AS CHANGE_FLG ")
        sqlString.AppendLine("  , DISPFLG ")
        sqlString.AppendLine("FROM ")
        sqlString.AppendLine("  ( ")
        sqlString.AppendLine("    SELECT ")
        sqlString.AppendLine("      TO_CHAR(TO_DATE(BASIC.SYUPT_DAY), 'yyyy/MM/dd') AS DISP_SYUPT_DAY ")
        sqlString.AppendLine("      , CODE_Y.CODE_NAME AS YOBI_NAME ")
        sqlString.AppendLine("      , CNT.YOYAKU_NUM ")
        sqlString.AppendLine("      , MJK.CHARGE_KBN_JININ_NAME ")
        sqlString.AppendLine("      , TRIM(TO_CHAR(CHGKBN.CHARGE_1, '9,999,999')) AS CHARGE_1 ")
        sqlString.AppendLine("      , TRIM(TO_CHAR(CHGKBN.CHARGE_2, '9,999,999')) AS CHARGE_2 ")
        sqlString.AppendLine("      , TRIM(TO_CHAR(CHGKBN.CHARGE_3, '9,999,999')) AS CHARGE_3 ")
        sqlString.AppendLine("      , TRIM(TO_CHAR(CHGKBN.CHARGE_4, '9,999,999')) AS CHARGE_4 ")
        sqlString.AppendLine("      , TRIM(TO_CHAR(CHGKBN.CHARGE_5, '9,999,999')) AS CHARGE_5 ")
        sqlString.AppendLine("      , TO_CHAR(BASIC.CRS_CD) AS CRS_CD ")
        sqlString.AppendLine("      , TO_CHAR(BASIC.SYUPT_DAY) AS SYUPT_DAY ")
        sqlString.AppendLine("      , BASIC.GOUSYA ")
        sqlString.AppendLine("      , CK.CHARGE_KBN_JININ_CD ")
        sqlString.AppendLine("      , CHG.KBN_NO ")
        sqlString.AppendLine("      , CHGKBN.NAME_1 ")
        sqlString.AppendLine("      , CHGKBN.NAME_2 ")
        sqlString.AppendLine("      , CHGKBN.NAME_3 ")
        sqlString.AppendLine("      , CHGKBN.NAME_4 ")
        sqlString.AppendLine("      , CHGKBN.NAME_5 ")
        sqlString.AppendLine("      , RANK() OVER ( ")
        sqlString.AppendLine("        PARTITION BY ")
        sqlString.AppendLine("          BASIC.CRS_CD ")
        sqlString.AppendLine("          , BASIC.SYUPT_DAY ")
        sqlString.AppendLine("        ORDER BY ")
        sqlString.AppendLine("          BASIC.GOUSYA ")
        sqlString.AppendLine("      ) RANK ")
        sqlString.AppendLine("      ,BASIC.SYSTEM_UPDATE_PGMID ")
        sqlString.AppendLine("      ,BASIC.SYSTEM_UPDATE_PERSON_CD ")
        sqlString.AppendLine("      ,TO_CHAR(BASIC.SYSTEM_UPDATE_DAY) AS SYSTEM_UPDATE_DAY ")
        sqlString.AppendLine("      , RANK() OVER ( ")
        sqlString.AppendLine("        PARTITION BY ")
        sqlString.AppendLine("          BASIC.CRS_CD ")
        sqlString.AppendLine("        ORDER BY ")
        sqlString.AppendLine("          BASIC.SYUPT_DAY ")
        sqlString.AppendLine("      ) KEY ")
        sqlString.AppendLine("      , RANK() OVER ( ")
        sqlString.AppendLine("        PARTITION BY ")
        sqlString.AppendLine("          BASIC.CRS_CD ")
        sqlString.AppendLine("          , BASIC.SYUPT_DAY ")
        sqlString.AppendLine("        ORDER BY ")
        sqlString.AppendLine("          CK.LINE_NO ")
        sqlString.AppendLine("      ) DISPFLG ")
        sqlString.AppendLine("    FROM ")
        sqlString.AppendLine("      T_CRS_LEDGER_BASIC BASIC ")
        sqlString.AppendLine("      INNER JOIN T_CRS_LEDGER_BASIC_CHARGE_KBN CK ")
        sqlString.AppendLine("        ON BASIC.SYUPT_DAY = CK.SYUPT_DAY ")
        sqlString.AppendLine("        AND BASIC.CRS_CD = CK.CRS_CD ")
        sqlString.AppendLine("        AND BASIC.GOUSYA = CK.GOUSYA ")
        sqlString.AppendLine("      LEFT JOIN M_CHARGE_JININ_KBN MJK ")
        sqlString.AppendLine("        ON MJK.CHARGE_KBN_JININ_CD = CK.CHARGE_KBN_JININ_CD ")
        sqlString.AppendLine("      LEFT JOIN T_CRS_LEDGER_CHARGE CHG ")
        sqlString.AppendLine("        ON BASIC.SYUPT_DAY = CHG.SYUPT_DAY ")
        sqlString.AppendLine("        AND BASIC.CRS_CD = CHG.CRS_CD ")
        sqlString.AppendLine("        AND BASIC.GOUSYA = CHG.GOUSYA ")
        sqlString.AppendLine("      LEFT JOIN T_CRS_LEDGER_CHARGE_CHARGE_KBN CHGKBN ")
        sqlString.AppendLine("        ON CHG.SYUPT_DAY = CHGKBN.SYUPT_DAY ")
        sqlString.AppendLine("        AND CHG.CRS_CD = CHGKBN.CRS_CD ")
        sqlString.AppendLine("        AND CHG.GOUSYA = CHGKBN.GOUSYA ")
        sqlString.AppendLine("        AND CHG.KBN_NO = CHGKBN.KBN_NO ")
        sqlString.AppendLine("        AND CK.CHARGE_KBN_JININ_CD = CHGKBN.CHARGE_KBN_JININ_CD ")
        sqlString.AppendLine("      LEFT JOIN M_CODE CODE_Y ")
        sqlString.AppendLine("        ON BASIC.YOBI_CD = CODE_Y.CODE_VALUE ")
        sqlString.AppendLine("        AND CODE_Y.CODE_BUNRUI = '" & CodeBunrui.yobi & "' ")
        sqlString.AppendLine("      LEFT JOIN ( ")
        sqlString.AppendLine("        SELECT")
        sqlString.AppendLine("            CRS_CD")
        sqlString.AppendLine("          , SYUPT_DAY")
        sqlString.AppendLine("          , SUM(NVL(YOYAKU_NUM_TEISEKI,0) + NVL(YOYAKU_NUM_SUB_SEAT,0)) AS YOYAKU_NUM")
        sqlString.AppendLine("        FROM ")
        sqlString.AppendLine("          T_CRS_LEDGER_BASIC")
        sqlString.AppendLine("        WHERE")
        sqlString.AppendLine("          NVL(DELETE_DAY,0) = 0")
        sqlString.AppendLine("          AND (CRS_CD, SYUPT_DAY, GOUSYA) IN ( ")
        sqlString.AppendLine("          ").Append(paramList(ParamHashKeys.BASIC_KEYS))
        sqlString.AppendLine("          )")
        sqlString.AppendLine("        GROUP BY")
        sqlString.AppendLine("          CRS_CD")
        sqlString.AppendLine("          ,SYUPT_DAY")
        sqlString.AppendLine("      ) CNT")
        sqlString.AppendLine("      ON  BASIC.CRS_CD = CNT.CRS_CD")
        sqlString.AppendLine("      AND BASIC.SYUPT_DAY = CNT.SYUPT_DAY")
        sqlString.AppendLine("    WHERE ")
        sqlString.AppendLine("      NVL(BASIC.DELETE_DAY,0) = 0 ")
        sqlString.AppendLine("      And (BASIC.CRS_CD, BASIC.SYUPT_DAY, BASIC.GOUSYA) In ( ")
        sqlString.AppendLine("      ").Append(paramList(ParamHashKeys.BASIC_KEYS))
        sqlString.AppendLine("      ) ")
        sqlString.AppendLine("      And CHG.KBN_NO = 1 ")
        sqlString.AppendLine("    ORDER BY ")
        sqlString.AppendLine("      BASIC.CRS_CD ")
        sqlString.AppendLine("      , BASIC.SYUPT_DAY ")
        sqlString.AppendLine("      , BASIC.GOUSYA ")
        sqlString.AppendLine("      , CK.LINE_NO ")
        sqlString.AppendLine("  ) ")
        sqlString.AppendLine("WHERE ")
        sqlString.AppendLine("  RANK = 1 ")


        Return sqlString.ToString

    End Function

    ''' <summary>
    ''' 検索用SELECT
    ''' </summary>
    ''' <param name="paramList"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Protected Overloads Function getChargeInfo(ByVal paramList As Hashtable) As String

        Dim sqlString As New StringBuilder
        paramClear()

        'SELECT句
        sqlString.AppendLine(" Select ")
        sqlString.AppendLine($" {Chr(34)}2{Chr(34)} ")
        sqlString.AppendLine($",{Chr(34)}3{Chr(34)} ")
        sqlString.AppendLine($",{Chr(34)}4{Chr(34)} ")
        sqlString.AppendLine(",Case ")
        sqlString.AppendLine("   When SORTKEY = '0CHARGE_NAME' THEN '料金区分' ")
        sqlString.AppendLine("   WHEN SORTKEY = '1TOKUTEI_RYOKIN_SET_NAME' THEN '特定料金設定' ")
        sqlString.AppendLine("   WHEN SORTKEY = '2CHARGE' THEN '料金' ")
        sqlString.AppendLine("   WHEN SORTKEY = '3CARRIAGE' THEN '運賃' ")
        sqlString.AppendLine("   WHEN SORTKEY = '4CHARGE_SUB_SEAT' THEN '料金（補助席）' ")
        sqlString.AppendLine("   WHEN SORTKEY = '5CARRIAGE_SUB_SEAT' THEN '運賃（補助席）' ")
        sqlString.AppendLine($"   ELSE '' END AS {Chr(34)}6{Chr(34)} ")
        sqlString.AppendLine($",A1 AS {Chr(34)}7{Chr(34)} ")
        sqlString.AppendLine($",A2 AS {Chr(34)}8{Chr(34)} ")
        sqlString.AppendLine($",A3 AS {Chr(34)}9{Chr(34)} ")
        sqlString.AppendLine($",A4 AS {Chr(34)}10{Chr(34)} ")
        sqlString.AppendLine($",A5 AS {Chr(34)}11{Chr(34)} ")
        sqlString.AppendLine($",A6 AS {Chr(34)}12{Chr(34)} ")
        sqlString.AppendLine($",A7 AS {Chr(34)}13{Chr(34)} ")
        sqlString.AppendLine($",A8 AS {Chr(34)}14{Chr(34)} ")
        sqlString.AppendLine($",A9 AS {Chr(34)}15{Chr(34)} ")
        sqlString.AppendLine($",A10 AS {Chr(34)}16{Chr(34)} ")
        sqlString.AppendLine($",B1 AS {Chr(34)}17{Chr(34)} ")
        sqlString.AppendLine($",B2 AS {Chr(34)}18{Chr(34)} ")
        sqlString.AppendLine($",B3 AS {Chr(34)}19{Chr(34)} ")
        sqlString.AppendLine($",B4 AS {Chr(34)}20{Chr(34)} ")
        sqlString.AppendLine($",B5 AS {Chr(34)}21{Chr(34)} ")
        sqlString.AppendLine($",B6 AS {Chr(34)}22{Chr(34)} ")
        sqlString.AppendLine($",B7 AS {Chr(34)}23{Chr(34)} ")
        sqlString.AppendLine($",B8 AS {Chr(34)}24{Chr(34)} ")
        sqlString.AppendLine($",B9 AS {Chr(34)}25{Chr(34)} ")
        sqlString.AppendLine($",B10 AS {Chr(34)}26{Chr(34)} ")
        sqlString.AppendLine($",C1 AS {Chr(34)}27{Chr(34)} ")
        sqlString.AppendLine($",C2 AS {Chr(34)}28{Chr(34)} ")
        sqlString.AppendLine($",C3 AS {Chr(34)}29{Chr(34)} ")
        sqlString.AppendLine($",C4 AS {Chr(34)}30{Chr(34)} ")
        sqlString.AppendLine($",C5 AS {Chr(34)}31{Chr(34)} ")
        sqlString.AppendLine($",C6 AS {Chr(34)}32{Chr(34)} ")
        sqlString.AppendLine($",C7 AS {Chr(34)}33{Chr(34)} ")
        sqlString.AppendLine($",C8 AS {Chr(34)}34{Chr(34)} ")
        sqlString.AppendLine($",C9 AS {Chr(34)}35{Chr(34)} ")
        sqlString.AppendLine($",C10 AS {Chr(34)}36{Chr(34)} ")
        sqlString.AppendLine($",D1 AS {Chr(34)}37{Chr(34)} ")
        sqlString.AppendLine($",D2 AS {Chr(34)}38{Chr(34)} ")
        sqlString.AppendLine($",D3 AS {Chr(34)}39{Chr(34)} ")
        sqlString.AppendLine($",D4 AS {Chr(34)}40{Chr(34)} ")
        sqlString.AppendLine($",D5 AS {Chr(34)}41{Chr(34)} ")
        sqlString.AppendLine($",D6 AS {Chr(34)}42{Chr(34)} ")
        sqlString.AppendLine($",D7 AS {Chr(34)}43{Chr(34)} ")
        sqlString.AppendLine($",D8 AS {Chr(34)}44{Chr(34)} ")
        sqlString.AppendLine($",D9 AS {Chr(34)}45{Chr(34)} ")
        sqlString.AppendLine($",D10 AS {Chr(34)}46{Chr(34)} ")
        sqlString.AppendLine($",E1 AS {Chr(34)}47{Chr(34)} ")
        sqlString.AppendLine($",E2 AS {Chr(34)}48{Chr(34)} ")
        sqlString.AppendLine($",E3 AS {Chr(34)}49{Chr(34)} ")
        sqlString.AppendLine($",E4 AS {Chr(34)}50{Chr(34)} ")
        sqlString.AppendLine($",E5 AS {Chr(34)}51{Chr(34)} ")
        sqlString.AppendLine($",E6 AS {Chr(34)}52{Chr(34)} ")
        sqlString.AppendLine($",E7 AS {Chr(34)}53{Chr(34)} ")
        sqlString.AppendLine($",E8 AS {Chr(34)}54{Chr(34)} ")
        sqlString.AppendLine($",E9 AS {Chr(34)}55{Chr(34)} ")
        sqlString.AppendLine($",E10 AS {Chr(34)}56{Chr(34)} ")
        sqlString.AppendLine($",F1 AS {Chr(34)}57{Chr(34)} ")
        sqlString.AppendLine($",F2 AS {Chr(34)}58{Chr(34)} ")
        sqlString.AppendLine($",F3 AS {Chr(34)}59{Chr(34)} ")
        sqlString.AppendLine($",F4 AS {Chr(34)}60{Chr(34)} ")
        sqlString.AppendLine($",F5 AS {Chr(34)}61{Chr(34)} ")
        sqlString.AppendLine($",F6 AS {Chr(34)}62{Chr(34)} ")
        sqlString.AppendLine($",F7 AS {Chr(34)}63{Chr(34)} ")
        sqlString.AppendLine($",F8 AS {Chr(34)}64{Chr(34)} ")
        sqlString.AppendLine($",F9 AS {Chr(34)}65{Chr(34)} ")
        sqlString.AppendLine($",F10 AS {Chr(34)}66{Chr(34)} ")
        sqlString.AppendLine($",G1 AS {Chr(34)}67{Chr(34)} ")
        sqlString.AppendLine($",G2 AS {Chr(34)}68{Chr(34)} ")
        sqlString.AppendLine($",G3 AS {Chr(34)}69{Chr(34)} ")
        sqlString.AppendLine($",G4 AS {Chr(34)}70{Chr(34)} ")
        sqlString.AppendLine($",G5 AS {Chr(34)}71{Chr(34)} ")
        sqlString.AppendLine($",G6 AS {Chr(34)}72{Chr(34)} ")
        sqlString.AppendLine($",G7 AS {Chr(34)}73{Chr(34)} ")
        sqlString.AppendLine($",G8 AS {Chr(34)}74{Chr(34)} ")
        sqlString.AppendLine($",G9 AS {Chr(34)}75{Chr(34)} ")
        sqlString.AppendLine($",G10 AS {Chr(34)}76{Chr(34)} ")
        sqlString.AppendLine($",H1 AS {Chr(34)}77{Chr(34)} ")
        sqlString.AppendLine($",H2 AS {Chr(34)}78{Chr(34)} ")
        sqlString.AppendLine($",H3 AS {Chr(34)}79{Chr(34)} ")
        sqlString.AppendLine($",H4 AS {Chr(34)}80{Chr(34)} ")
        sqlString.AppendLine($",H5 AS {Chr(34)}81{Chr(34)} ")
        sqlString.AppendLine($",H6 AS {Chr(34)}82{Chr(34)} ")
        sqlString.AppendLine($",H7 AS {Chr(34)}83{Chr(34)} ")
        sqlString.AppendLine($",H8 AS {Chr(34)}84{Chr(34)} ")
        sqlString.AppendLine($",H9 AS {Chr(34)}85{Chr(34)} ")
        sqlString.AppendLine($",H10 AS {Chr(34)}86{Chr(34)} ")
        sqlString.AppendLine($",I1 AS {Chr(34)}87{Chr(34)} ")
        sqlString.AppendLine($",I2 AS {Chr(34)}88{Chr(34)} ")
        sqlString.AppendLine($",I3 AS {Chr(34)}89{Chr(34)} ")
        sqlString.AppendLine($",I4 AS {Chr(34)}90{Chr(34)} ")
        sqlString.AppendLine($",I5 AS {Chr(34)}91{Chr(34)} ")
        sqlString.AppendLine($",I6 AS {Chr(34)}92{Chr(34)} ")
        sqlString.AppendLine($",I7 AS {Chr(34)}93{Chr(34)} ")
        sqlString.AppendLine($",I8 AS {Chr(34)}94{Chr(34)} ")
        sqlString.AppendLine($",I9 AS {Chr(34)}95{Chr(34)} ")
        sqlString.AppendLine($",I10 AS {Chr(34)}96{Chr(34)} ")
        sqlString.AppendLine(",UP_PGMID ")
        sqlString.AppendLine(",UP_PERSON_CD ")
        sqlString.AppendLine(",UP_DAY ")
        sqlString.AppendLine(",CRSCD ")
        sqlString.AppendLine(",KEY ")
        sqlString.AppendLine(",'  ' AS CHANGE_FLG ")
        sqlString.AppendLine($",A1_B AS {Chr(34)}1_CHARGE_KBN{Chr(34)} ")
        sqlString.AppendLine($",B1_B AS {Chr(34)}2_CHARGE_KBN{Chr(34)} ")
        sqlString.AppendLine($",C1_B AS {Chr(34)}3_CHARGE_KBN{Chr(34)} ")
        sqlString.AppendLine($",D1_B AS {Chr(34)}4_CHARGE_KBN{Chr(34)} ")
        sqlString.AppendLine($",E1_B AS {Chr(34)}5_CHARGE_KBN{Chr(34)} ")
        sqlString.AppendLine($",F1_B AS {Chr(34)}6_CHARGE_KBN{Chr(34)} ")
        sqlString.AppendLine($",G1_B AS {Chr(34)}7_CHARGE_KBN{Chr(34)} ")
        sqlString.AppendLine($",H1_B AS {Chr(34)}8_CHARGE_KBN{Chr(34)} ")
        sqlString.AppendLine($",I1_B AS {Chr(34)}9_CHARGE_KBN{Chr(34)} ")
        sqlString.AppendLine($",A1_C AS {Chr(34)}1_TOKUTEI_RYOKIN_SET{Chr(34)} ")
        sqlString.AppendLine($",B1_C AS {Chr(34)}2_TOKUTEI_RYOKIN_SET{Chr(34)} ")
        sqlString.AppendLine($",C1_C AS {Chr(34)}3_TOKUTEI_RYOKIN_SET{Chr(34)} ")
        sqlString.AppendLine($",D1_C AS {Chr(34)}4_TOKUTEI_RYOKIN_SET{Chr(34)} ")
        sqlString.AppendLine($",E1_C AS {Chr(34)}5_TOKUTEI_RYOKIN_SET{Chr(34)} ")
        sqlString.AppendLine($",F1_C AS {Chr(34)}6_TOKUTEI_RYOKIN_SET{Chr(34)} ")
        sqlString.AppendLine($",G1_C AS {Chr(34)}7_TOKUTEI_RYOKIN_SET{Chr(34)} ")
        sqlString.AppendLine($",H1_C AS {Chr(34)}8_TOKUTEI_RYOKIN_SET{Chr(34)} ")
        sqlString.AppendLine($",I1_C AS {Chr(34)}9_TOKUTEI_RYOKIN_SET{Chr(34)} ")
        sqlString.AppendLine(" FROM ")
        sqlString.AppendLine(" ( ")
        sqlString.AppendLine(" SELECT * ")
        sqlString.AppendLine(" FROM ")
        sqlString.AppendLine(" ( ")
        sqlString.AppendLine(" SELECT ")
        sqlString.AppendLine($"SYUPT_DAY AS {Chr(34)}2{Chr(34)} ")
        sqlString.AppendLine($",YOBI_CD AS {Chr(34)}3{Chr(34)} ")
        sqlString.AppendLine($",YOYAKU_NUM AS {Chr(34)}4{Chr(34)} ")
        sqlString.AppendLine(",KBN_NO || LPAD(RNK, 2, '0') AS RNK ")
        sqlString.AppendLine(",COALESCE(CHARGE_NAME,CHR(0)) CHARGE_NAME ")
        sqlString.AppendLine(",COALESCE(TOKUTEI_RYOKIN_SET_NAME,CHR(0)) TOKUTEI_RYOKIN_SET_NAME ")
        sqlString.AppendLine(",COALESCE(TRIM(TO_CHAR(CHARGE,'9,999,999')),CHR(0)) AS CHARGE ")
        sqlString.AppendLine(",COALESCE(TRIM(TO_CHAR(CHARGE_SUB_SEAT,'9,999,999')),CHR(0)) AS CHARGE_SUB_SEAT ")
        sqlString.AppendLine(",COALESCE(TRIM(TO_CHAR(CARRIAGE,'9,999,999')),CHR(0)) AS CARRIAGE ")
        sqlString.AppendLine(",COALESCE(TRIM(TO_CHAR(CARRIAGE_SUB_SEAT,'9,999,999')),CHR(0)) AS CARRIAGE_SUB_SEAT ")
        sqlString.AppendLine(",TO_CHAR(PH2_CHARGE_RISEKI) AS PH2_CHARGE_RISEKI ")
        sqlString.AppendLine(",SYSTEM_UPDATE_PGMID AS UP_PGMID ")
        sqlString.AppendLine(",SYSTEM_UPDATE_PERSON_CD AS UP_PERSON_CD ")
        sqlString.AppendLine(",SYSTEM_UPDATE_DAY AS UP_DAY ")
        sqlString.AppendLine(",CRS_CD AS CRSCD ")
        sqlString.AppendLine(",REPLACE(SYUPT_DAY, '/') AS KEY ")
        sqlString.AppendLine(",CHARGE_KBN ")
        sqlString.AppendLine(",TOKUTEI_RYOKIN_SET ")
        sqlString.AppendLine(" FROM ")
        sqlString.AppendLine(" ( ")
        sqlString.AppendLine(" SELECT ")
        sqlString.AppendLine(" BASIC.CRS_CD AS CRS_CD ")
        sqlString.AppendLine(",TO_CHAR(TO_DATE(BASIC.SYUPT_DAY), 'yyyy/MM/dd') AS SYUPT_DAY ")
        sqlString.AppendLine(",CODE_Y.CODE_NAME AS YOBI_CD ")
        'sqlString.AppendLine(",BASIC.YOYAKU_NUM_TEISEKI + BASIC.YOYAKU_NUM_SUB_SEAT AS YOYAKU_NUM ")
        sqlString.AppendLine(",CNT.YOYAKU_NUM ")
        sqlString.AppendLine(",C.KBN_NO ")
        sqlString.AppendLine(",RANK() OVER (PARTITION BY C.CRS_CD, C.SYUPT_DAY, C.GOUSYA, C.KBN_NO ORDER BY K.LINE_NO) AS RNK ")
        sqlString.AppendLine(",K.CHARGE_KBN_JININ_CD ")
        sqlString.AppendLine(",MC.CHARGE_NAME ")
        sqlString.AppendLine(",CASE WHEN C.TOKUTEI_RYOKIN_SET = " & " '" & TokuteiChargeSetType.heijitu & "' " & " THEN ")
        sqlString.AppendLine("'" & CommonKyushuUtil.getEnumValue(GetType(TokuteiChargeSetType), TokuteiChargeSetType.heijitu) & "' ")
        sqlString.AppendLine("   WHEN C.TOKUTEI_RYOKIN_SET = " & " '" & TokuteiChargeSetType.satDay & "' " & " THEN ")
        sqlString.AppendLine("'" & CommonKyushuUtil.getEnumValue(GetType(TokuteiChargeSetType), TokuteiChargeSetType.satDay) & "' ")
        sqlString.AppendLine("   WHEN C.TOKUTEI_RYOKIN_SET = " & " '" & TokuteiChargeSetType.satDaySYUKU & "' " & " THEN ")
        sqlString.AppendLine("'" & CommonKyushuUtil.getEnumValue(GetType(TokuteiChargeSetType), TokuteiChargeSetType.satDaySYUKU) & "' ")
        sqlString.AppendLine("   WHEN C.TOKUTEI_RYOKIN_SET = " & " '" & TokuteiChargeSetType.friDay & "' " & " THEN ")
        sqlString.AppendLine("'" & CommonKyushuUtil.getEnumValue(GetType(TokuteiChargeSetType), TokuteiChargeSetType.friDay) & "' ")
        sqlString.AppendLine("   WHEN C.TOKUTEI_RYOKIN_SET = " & " '" & TokuteiChargeSetType.kyuzenDay & "' " & " THEN ")
        sqlString.AppendLine("'" & CommonKyushuUtil.getEnumValue(GetType(TokuteiChargeSetType), TokuteiChargeSetType.kyuzenDay) & "' ")
        sqlString.AppendLine("   ELSE '' END AS TOKUTEI_RYOKIN_SET_NAME ")
        sqlString.AppendLine(",C.CHARGE_KBN ")
        sqlString.AppendLine(",C.TOKUTEI_RYOKIN_SET ")
        sqlString.AppendLine(",CCK.CHARGE ")
        sqlString.AppendLine(",CCK.CHARGE_SUB_SEAT ")
        sqlString.AppendLine(",CCK.CARRIAGE ")
        sqlString.AppendLine(",CCK.CARRIAGE_SUB_SEAT ")
        sqlString.AppendLine(",'0' AS PH2_CHARGE_RISEKI ")
        sqlString.AppendLine(",'DUMMY' AS SYSTEM_UPDATE_PGMID ")
        sqlString.AppendLine(",'DUMMY' AS SYSTEM_UPDATE_PERSON_CD ")
        sqlString.AppendLine(",'DUMMY' AS SYSTEM_UPDATE_DAY ")
        sqlString.AppendLine(" FROM ")
        sqlString.AppendLine("T_CRS_LEDGER_BASIC BASIC ")
        '曜日と催行確定区分は固定コードの可能性があるためコメント
        sqlString.AppendLine("LEFT JOIN M_CODE CODE_Y")
        sqlString.AppendLine("  ON BASIC.YOBI_CD = CODE_Y.CODE_VALUE")
        sqlString.AppendLine("  AND CODE_Y.CODE_BUNRUI = '" & CodeBunrui.yobi & "' ")
        sqlString.AppendLine("INNER JOIN T_CRS_LEDGER_BASIC_CHARGE_KBN K ")
        sqlString.AppendLine("  ON BASIC.SYUPT_DAY = K.SYUPT_DAY ")
        sqlString.AppendLine("  AND BASIC.CRS_CD = K.CRS_CD")
        sqlString.AppendLine("  AND BASIC.GOUSYA = K.GOUSYA")
        'sqlString.AppendLine("LEFT JOIN M_CODE CODE_S ON BASIC.SAIKOU_KAKUTEI_KBN = CODE_S.CODE_VALUE ")
        sqlString.AppendLine("LEFT JOIN T_CRS_LEDGER_CHARGE C ")
        sqlString.AppendLine("  ON BASIC.SYUPT_DAY = C.SYUPT_DAY ")
        sqlString.AppendLine("  AND BASIC.CRS_CD = C.CRS_CD And BASIC.GOUSYA = C.GOUSYA ")
        sqlString.AppendLine("LEFT JOIN T_CRS_LEDGER_CHARGE_CHARGE_KBN CCK ")
        sqlString.AppendLine("  ON C.SYUPT_DAY = CCK.SYUPT_DAY ")
        sqlString.AppendLine("  AND C.CRS_CD = CCK.CRS_CD ")
        sqlString.AppendLine("  AND C.GOUSYA = CCK.GOUSYA ")
        sqlString.AppendLine("  AND C.KBN_NO = CCK.KBN_NO ")
        sqlString.AppendLine("  AND K.CHARGE_KBN_JININ_CD = CCK.CHARGE_KBN_JININ_CD ")
        sqlString.AppendLine("LEFT JOIN M_CHARGE_KBN MC ")
        sqlString.AppendLine("  ON C.CHARGE_KBN = MC.CHARGE_KBN ")
        sqlString.AppendLine("LEFT JOIN ( ")
        sqlString.AppendLine("  SELECT")
        sqlString.AppendLine("      CRS_CD")
        sqlString.AppendLine("    , SYUPT_DAY")
        sqlString.AppendLine("    , SUM(NVL(YOYAKU_NUM_TEISEKI,0) + NVL(YOYAKU_NUM_SUB_SEAT,0)) AS YOYAKU_NUM")
        sqlString.AppendLine("  FROM ")
        sqlString.AppendLine("    T_CRS_LEDGER_BASIC")
        sqlString.AppendLine("  WHERE")
        sqlString.AppendLine("    NVL(DELETE_DAY,0) = 0")
        sqlString.AppendLine("    AND (CRS_CD, SYUPT_DAY) IN ( ")
        sqlString.AppendLine("    ").Append(paramList(ParamHashKeys.BASIC_KEYS_TEIKI))
        sqlString.AppendLine("    )")
        sqlString.AppendLine("  GROUP BY")
        sqlString.AppendLine("    CRS_CD")
        sqlString.AppendLine("    ,SYUPT_DAY")
        sqlString.AppendLine(") CNT")
        sqlString.AppendLine("  ON")
        sqlString.AppendLine("  BASIC.CRS_CD = CNT.CRS_CD")
        sqlString.AppendLine("  AND")
        sqlString.AppendLine("  BASIC.SYUPT_DAY = CNT.SYUPT_DAY")

        'WHERE句
        sqlString.AppendLine(" WHERE ")
        sqlString.AppendLine(" NVL(BASIC.DELETE_DAY,0) = 0 ")
        sqlString.AppendLine(" AND (BASIC.CRS_CD, BASIC.SYUPT_DAY) IN ( ")
        sqlString.AppendLine(" ").Append(paramList(ParamHashKeys.BASIC_KEYS_TEIKI))
        sqlString.AppendLine(" ) ")

        'ORDER BY句
        sqlString.AppendLine(" ORDER BY ")
        sqlString.AppendLine(" BASIC.SYUPT_DAY ")
        sqlString.AppendLine(",C.KBN_NO ")
        sqlString.AppendLine(" ) ")
        sqlString.AppendLine(" ) UNPIVOT( VALS For SORTKEY In ( ")
        sqlString.AppendLine("   CHARGE_NAME As '0CHARGE_NAME' ")
        sqlString.AppendLine("   ,TOKUTEI_RYOKIN_SET_NAME AS '1TOKUTEI_RYOKIN_SET_NAME' ")
        sqlString.AppendLine("   ,CHARGE AS '2CHARGE' ")
        sqlString.AppendLine("   ,CARRIAGE AS '3CARRIAGE' ")
        sqlString.AppendLine("   ,CHARGE_SUB_SEAT AS '4CHARGE_SUB_SEAT' ")
        sqlString.AppendLine("   ,CARRIAGE_SUB_SEAT AS '5CARRIAGE_SUB_SEAT' ")
        'sqlString.AppendLine("   ,PH2_CHARGE_RISEKI AS '9CHARGE_RISEKI' ")
        sqlString.AppendLine(" ) ")
        sqlString.AppendLine(" ) ")
        sqlString.AppendLine(" ) PIVOT( MAX(VALS) ")
        sqlString.AppendLine(" , MAX(CHARGE_KBN) AS B ")
        sqlString.AppendLine(" , MAX(TOKUTEI_RYOKIN_SET) AS C FOR RNK IN ( ")
        sqlString.AppendLine("   '101' AS A1 ")
        sqlString.AppendLine("   , '102' AS A2 ")
        sqlString.AppendLine("   , '103' AS A3 ")
        sqlString.AppendLine("   , '104' AS A4 ")
        sqlString.AppendLine("   , '105' AS A5 ")
        sqlString.AppendLine("   , '106' AS A6 ")
        sqlString.AppendLine("   , '107' AS A7 ")
        sqlString.AppendLine("   , '108' AS A8 ")
        sqlString.AppendLine("   , '109' AS A9 ")
        sqlString.AppendLine("   , '110' AS A10 ")
        sqlString.AppendLine("   , '201' AS B1 ")
        sqlString.AppendLine("   , '202' AS B2 ")
        sqlString.AppendLine("   , '203' AS B3 ")
        sqlString.AppendLine("   , '204' AS B4 ")
        sqlString.AppendLine("   , '205' AS B5 ")
        sqlString.AppendLine("   , '206' AS B6 ")
        sqlString.AppendLine("   , '207' AS B7 ")
        sqlString.AppendLine("   , '208' AS B8 ")
        sqlString.AppendLine("   , '209' AS B9 ")
        sqlString.AppendLine("   , '210' AS B10 ")
        sqlString.AppendLine("   , '301' AS C1 ")
        sqlString.AppendLine("   , '302' AS C2 ")
        sqlString.AppendLine("   , '303' AS C3 ")
        sqlString.AppendLine("   , '304' AS C4 ")
        sqlString.AppendLine("   , '305' AS C5 ")
        sqlString.AppendLine("   , '306' AS C6 ")
        sqlString.AppendLine("   , '307' AS C7 ")
        sqlString.AppendLine("   , '308' AS C8 ")
        sqlString.AppendLine("   , '309' AS C9 ")
        sqlString.AppendLine("   , '310' AS C10 ")
        sqlString.AppendLine("   , '401' AS D1 ")
        sqlString.AppendLine("   , '402' AS D2 ")
        sqlString.AppendLine("   , '403' AS D3 ")
        sqlString.AppendLine("   , '404' AS D4 ")
        sqlString.AppendLine("   , '405' AS D5 ")
        sqlString.AppendLine("   , '406' AS D6 ")
        sqlString.AppendLine("   , '407' AS D7 ")
        sqlString.AppendLine("   , '408' AS D8 ")
        sqlString.AppendLine("   , '409' AS D9 ")
        sqlString.AppendLine("   , '410' AS D10 ")
        sqlString.AppendLine("   , '501' AS E1 ")
        sqlString.AppendLine("   , '502' AS E2 ")
        sqlString.AppendLine("   , '503' AS E3 ")
        sqlString.AppendLine("   , '504' AS E4 ")
        sqlString.AppendLine("   , '505' AS E5 ")
        sqlString.AppendLine("   , '506' AS E6 ")
        sqlString.AppendLine("   , '507' AS E7 ")
        sqlString.AppendLine("   , '508' AS E8 ")
        sqlString.AppendLine("   , '509' AS E9 ")
        sqlString.AppendLine("   , '510' AS E10 ")
        sqlString.AppendLine("   , '601' AS F1 ")
        sqlString.AppendLine("   , '602' AS F2 ")
        sqlString.AppendLine("   , '603' AS F3 ")
        sqlString.AppendLine("   , '604' AS F4 ")
        sqlString.AppendLine("   , '605' AS F5 ")
        sqlString.AppendLine("   , '606' AS F6 ")
        sqlString.AppendLine("   , '607' AS F7 ")
        sqlString.AppendLine("   , '608' AS F8 ")
        sqlString.AppendLine("   , '609' AS F9 ")
        sqlString.AppendLine("   , '610' AS F10 ")
        sqlString.AppendLine("   , '701' AS G1 ")
        sqlString.AppendLine("   , '702' AS G2 ")
        sqlString.AppendLine("   , '703' AS G3 ")
        sqlString.AppendLine("   , '704' AS G4 ")
        sqlString.AppendLine("   , '705' AS G5 ")
        sqlString.AppendLine("   , '706' AS G6 ")
        sqlString.AppendLine("   , '707' AS G7 ")
        sqlString.AppendLine("   , '708' AS G8 ")
        sqlString.AppendLine("   , '709' AS G9 ")
        sqlString.AppendLine("   , '710' AS G10 ")
        sqlString.AppendLine("   , '801' AS H1 ")
        sqlString.AppendLine("   , '802' AS H2 ")
        sqlString.AppendLine("   , '803' AS H3 ")
        sqlString.AppendLine("   , '804' AS H4 ")
        sqlString.AppendLine("   , '805' AS H5 ")
        sqlString.AppendLine("   , '806' AS H6 ")
        sqlString.AppendLine("   , '807' AS H7 ")
        sqlString.AppendLine("   , '808' AS H8 ")
        sqlString.AppendLine("   , '809' AS H9 ")
        sqlString.AppendLine("   , '810' AS H10 ")
        sqlString.AppendLine("   , '901' AS I1 ")
        sqlString.AppendLine("   , '902' AS I2 ")
        sqlString.AppendLine("   , '903' AS I3 ")
        sqlString.AppendLine("   , '904' AS I4 ")
        sqlString.AppendLine("   , '905' AS I5 ")
        sqlString.AppendLine("   , '906' AS I6 ")
        sqlString.AppendLine("   , '907' AS I7 ")
        sqlString.AppendLine("   , '908' AS I8 ")
        sqlString.AppendLine("   , '909' AS I9 ")
        sqlString.AppendLine("   , '910' AS I10 ")
        sqlString.AppendLine(" ) ")
        sqlString.AppendLine(" ) ")
        sqlString.AppendLine(" ORDER BY ")
        sqlString.AppendLine($" {Chr(34)}2{Chr(34)} ")
        sqlString.AppendLine(",SORTKEY ")

        Return sqlString.ToString

    End Function

#End Region

#Region "メソッド"

    ''' <summary>
    ''' 料金区分データ取得
    ''' </summary>
    ''' <param name="nullRecord"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetMChargeKbnData(Optional ByVal nullRecord As Boolean = False) As DataTable

        Dim sqlString As New StringBuilder

        Try

            '空レコード挿入要否に従い、空行挿入
            If nullRecord = True Then
                sqlString.AppendLine("SELECT ' ' AS CODE_VALUE, '' AS CODE_NAME FROM DUAL UNION ")
            End If
            sqlString.AppendLine("SELECT RTRIM(CHARGE_KBN) AS CODE_VALUE, CHARGE_NAME AS CODE_NAME FROM M_CHARGE_KBN")
            sqlString.AppendLine(" WHERE DELETE_DATE IS NULL")
            sqlString.AppendLine(" ORDER BY CODE_VALUE")

            Return MyBase.getDataTable(sqlString.ToString)

        Catch ex As Exception
            Throw
        End Try

    End Function

#End Region

#Region " UPDATE処理 "

    ''' <summary>
    ''' DB接続用
    ''' </summary>
    ''' <param name="type"></param>
    ''' <param name="paramInfoListArrayList "></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function executeChargeInfoTehaiKikaku(ByVal type As accessType, ByVal paramInfoListArrayList As ArrayList, Optional ByVal paramInfoListArrayListKbn As ArrayList = Nothing) As Integer

        Dim oracleTransaction As OracleTransaction = Nothing
        Dim returnValue As Integer = 0
        Dim sqlString As String = String.Empty
        Dim RET_OK As Integer = 1

        Try
            'トランザクション開始
            oracleTransaction = callBeginTransaction()

            Select Case type
                Case accessType.executeUpdateChargeKikakuInfo
                    Dim crsCd As String = String.Empty
                    Dim syuptDay As String = String.Empty

                    For idx As Integer = 0 To paramInfoListArrayList.Count - 1
                        'コース台帳(料金区分)更新
                        paramClear()
                        sqlString = executeUpdateChargeKbnKikakuData(CType(paramInfoListArrayList(idx), Hashtable))
                        returnValue += execNonQuery(oracleTransaction, sqlString)
                        'コース台帳(基本)更新
                        If syuptDay <> CType(CType(paramInfoListArrayList(idx), Hashtable).Item("SYUPT_DAY"), String) OrElse
                            crsCd <> CType(CType(paramInfoListArrayList(idx), Hashtable).Item("CRS_CD"), String) Then
                            paramClear()
                            sqlString = executeUpdateBasicData(CType(paramInfoListArrayList(idx), Hashtable))
                            returnValue += execNonQuery(oracleTransaction, sqlString)
                        End If

                        syuptDay = CType(CType(paramInfoListArrayList(idx), Hashtable).Item("SYUPT_DAY"), String)
                        crsCd = CType(CType(paramInfoListArrayList(idx), Hashtable).Item("CRS_CD"), String)
                    Next
            End Select

            Call callCommitTransaction(oracleTransaction)

        Catch ex As Exception
            Call callRollbackTransaction(oracleTransaction)
            Throw

        Finally
            Call oracleTransaction.Dispose()
        End Try

        If returnValue > 0 Then
            Return RET_OK
        End If

    End Function

    ''' <summary>
    ''' DB接続用
    ''' </summary>
    ''' <param name="type"></param>
    ''' <param name="paramInfoListArrayList "></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function executeChargeInfoTehaiTeiki(ByVal type As accessType, ByVal paramInfoListArrayList As List(Of TCrsLedgerChargeEntityEx)) As Integer

        Dim oracleTransaction As OracleTransaction = Nothing
        Dim returnValue As Integer = 0
        Dim sqlString As String = String.Empty
        Dim RET_OK As Integer = 1

        Try
            'トランザクション開始
            oracleTransaction = callBeginTransaction()

            Select Case type
                Case accessType.executeUpdateChargeInfo
                    For Each ent As TCrsLedgerChargeEntityEx In paramInfoListArrayList
                        '削除処理
                        '[コース台帳(料金)]
                        paramClear()
                        sqlString = executeDeleteTblData(ent, "T_CRS_LEDGER_CHARGE")
                        returnValue += execNonQuery(oracleTransaction, sqlString)
                        '[コース台帳(料金区分)]
                        paramClear()
                        sqlString = executeDeleteTblData(ent, "T_CRS_LEDGER_CHARGE_CHARGE_KBN")
                        returnValue += execNonQuery(oracleTransaction, sqlString)
                        '[コース台帳(基本)]
                        paramClear()
                        sqlString = executeUpdateBasicData(ent)
                        returnValue += execNonQuery(oracleTransaction, sqlString)
                        executeUpdateBasicData(ent)

                        If ent.MakeFlg.Value = TCrsLedgerChargeEntityEx.MakeFlgType.On Then
                            paramClear()
                            sqlString = executeInsertChargeData(ent)
                            returnValue += execNonQuery(oracleTransaction, sqlString)
                            For Each kbnEnt As TCrsLedgerChargeChargeKbnEntity In ent.ChargeChargeKbnEntity.EntityData
                                If kbnEnt.crsCd.Value <> String.Empty Then
                                    paramClear()
                                    sqlString = executeInsertChargeChargeKbnData(kbnEnt)
                                    returnValue += execNonQuery(oracleTransaction, sqlString)
                                End If
                            Next
                        End If

                    Next
            End Select

            Call callCommitTransaction(oracleTransaction)

        Catch ex As Exception
            Call callRollbackTransaction(oracleTransaction)
            Throw
        Finally
            Call oracleTransaction.Dispose()
        End Try

        If returnValue > 0 Then
            Return RET_OK
        End If

    End Function

    ''' <summary>
    ''' コース台帳（料金）：データ削除 ※定期
    ''' </summary>
    ''' <param name="paramEnt">コース台帳(料金)エンティティ</param>
    ''' <param name="paramTblName">テーブルID</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function executeDeleteTblData(ByVal paramEnt As TCrsLedgerChargeEntityEx, ByVal paramTblName As String) As String

        Dim sqlString As New StringBuilder

        'UPDATE
        sqlString.AppendLine(" DELETE FROM " & paramTblName & " CHARGE ")
        'WHERE句
        sqlString.AppendLine(" WHERE EXISTS ( ")
        sqlString.AppendLine(" SELECT ")
        sqlString.AppendLine(" 1 ")
        sqlString.AppendLine(" FROM ")
        sqlString.AppendLine(" T_CRS_LEDGER_BASIC BASIC ")
        sqlString.AppendLine(" WHERE ")
        sqlString.AppendLine(" " & makeSqlEx(paramEnt.crsCd))
        sqlString.AppendLine(" AND ")
        sqlString.AppendLine(" " & makeSqlEx(paramEnt.syuptDay))
        sqlString.AppendLine(" AND ")
        sqlString.AppendLine(" " & makeSqlEx(paramEnt.kbnNo))
        sqlString.AppendLine(" AND ")
        sqlString.AppendLine(" NVL(MARU_ZOU_MANAGEMENT_KBN,' ') <> 'M' ")
        sqlString.AppendLine(" AND ")
        sqlString.AppendLine(" BASIC.CRS_CD = CHARGE.CRS_CD ")
        sqlString.AppendLine(" AND ")
        sqlString.AppendLine(" BASIC.SYUPT_DAY = CHARGE.SYUPT_DAY ")
        sqlString.AppendLine(" AND ")
        sqlString.AppendLine(" BASIC.GOUSYA = CHARGE.GOUSYA ")
        sqlString.AppendLine(" ) ")

        Return sqlString.ToString
    End Function

    ''' <summary>
    ''' コース台帳（料金）：データ削除 ※定期
    ''' </summary>
    ''' <param name="paramEnt">コース台帳(料金)エンティティ</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function executeInsertChargeData(ByVal paramEnt As TCrsLedgerChargeEntityEx) As String

        Dim sqlString As New StringBuilder

        'INSERT
        sqlString.AppendLine("INSERT INTO T_CRS_LEDGER_CHARGE ")
        sqlString.AppendLine("SELECT ")
        sqlString.AppendLine("    " & setParamEx(paramEnt.chargeKbn))
        sqlString.AppendLine("  , CRS_CD ")
        sqlString.AppendLine("  , DELETE_DAY ")
        sqlString.AppendLine("  , GOUSYA ")
        sqlString.AppendLine("  , SYUPT_DAY ")
        sqlString.AppendLine("  , " & setParamEx(paramEnt.kbnNo))
        sqlString.AppendLine("  , " & setParamEx(paramEnt.tokuteiRyokinSet))
        sqlString.AppendLine("  , " & setParamEx(paramEnt.systemEntryPgmid))
        sqlString.AppendLine("  , " & setParamEx(paramEnt.systemEntryPersonCd))
        sqlString.AppendLine("  , " & setParamEx(paramEnt.systemEntryDay))
        sqlString.AppendLine("  , " & setParamEx(paramEnt.systemUpdatePgmid))
        sqlString.AppendLine("  , " & setParamEx(paramEnt.systemUpdatePersonCd))
        sqlString.AppendLine("  , " & setParamEx(paramEnt.systemUpdateDay))
        sqlString.AppendLine("FROM ")
        sqlString.AppendLine("  T_CRS_LEDGER_BASIC ")
        sqlString.AppendLine("WHERE ")
        sqlString.AppendLine("  " & makeSqlEx(paramEnt.crsCd))
        sqlString.AppendLine("  AND ")
        sqlString.AppendLine("  " & makeSqlEx(paramEnt.syuptDay))
        sqlString.AppendLine(" AND ")
        sqlString.AppendLine(" NVL(MARU_ZOU_MANAGEMENT_KBN,' ') <> 'M'")

        Return sqlString.ToString
    End Function

    ''' <summary>
    ''' コース台帳（料金）：データ削除 ※定期
    ''' </summary>
    ''' <param name="paramEnt">コース台帳(料金)エンティティ</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function executeInsertChargeChargeKbnData(ByVal paramEnt As TCrsLedgerChargeChargeKbnEntity) As String

        Dim sqlString As New StringBuilder

        'INSERT
        sqlString.AppendLine("INSERT INTO T_CRS_LEDGER_CHARGE_CHARGE_KBN ")
        sqlString.AppendLine("SELECT ")
        sqlString.AppendLine("    SYUPT_DAY ")
        sqlString.AppendLine("  , CRS_CD ")
        sqlString.AppendLine("  , GOUSYA ")
        sqlString.AppendLine("  , " & setParamEx(paramEnt.kbnNo))
        sqlString.AppendLine("  , " & setParamEx(paramEnt.chargeKbnJininCd))
        sqlString.AppendLine("  , " & setParamEx(paramEnt.charge))
        sqlString.AppendLine("  , " & setParamEx(paramEnt.chargeSubSeat))
        sqlString.AppendLine("  , " & setParamEx(paramEnt.carriage))
        sqlString.AppendLine("  , " & setParamEx(paramEnt.carriageSubSeat))
        sqlString.AppendLine("  , NULL ")
        sqlString.AppendLine("  , NULL ")
        sqlString.AppendLine("  , NULL ")
        sqlString.AppendLine("  , NULL ")
        sqlString.AppendLine("  , NULL ")
        sqlString.AppendLine("  , 0 ")
        sqlString.AppendLine("  , 0 ")
        sqlString.AppendLine("  , 0 ")
        sqlString.AppendLine("  , 0 ")
        sqlString.AppendLine("  , 0 ")
        sqlString.AppendLine("  , " & setParamEx(paramEnt.systemEntryPgmid))
        sqlString.AppendLine("  , " & setParamEx(paramEnt.systemEntryPersonCd))
        sqlString.AppendLine("  , " & setParamEx(paramEnt.systemEntryDay))
        sqlString.AppendLine("  , " & setParamEx(paramEnt.systemUpdatePgmid))
        sqlString.AppendLine("  , " & setParamEx(paramEnt.systemUpdatePersonCd))
        sqlString.AppendLine("  , " & setParamEx(paramEnt.systemUpdateDay))
        sqlString.AppendLine("  , DELETE_DAY ")
        sqlString.AppendLine("FROM ")
        sqlString.AppendLine("  T_CRS_LEDGER_BASIC ")
        sqlString.AppendLine("WHERE ")
        sqlString.AppendLine("  " & makeSqlEx(paramEnt.crsCd))
        sqlString.AppendLine("  AND ")
        sqlString.AppendLine("  " & makeSqlEx(paramEnt.syuptDay))
        sqlString.AppendLine("  AND ")
        sqlString.AppendLine("  NVL(MARU_ZOU_MANAGEMENT_KBN,' ') <> 'M'")

        Return sqlString.ToString
    End Function

    ''' <summary>
    ''' コース台帳（料金区分）：データ更新用 ※企画専用
    ''' </summary>
    ''' <param name="paramList">SQL引数</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function executeUpdateChargeKbnKikakuData(ByVal paramList As Hashtable) As String

        Dim sqlString As New StringBuilder
        Dim chgKbnEnt As New TCrsLedgerChargeChargeKbnEntity

        'UPDATE
        sqlString.AppendLine(" UPDATE T_CRS_LEDGER_CHARGE_CHARGE_KBN CHARGE ")
        sqlString.AppendLine(" SET ")
        sqlString.AppendLine(" " & makeSqlEx(chgKbnEnt.systemUpdatePgmid, paramList))
        sqlString.AppendLine("," & makeSqlEx(chgKbnEnt.systemUpdatePersonCd, paramList))
        sqlString.AppendLine("," & makeSqlEx(chgKbnEnt.systemUpdateDay, paramList))
        If paramList.ContainsKey(chgKbnEnt.charge1.PhysicsName) Then
            sqlString.AppendLine("," & makeSqlEx(chgKbnEnt.charge1, paramList))
        End If
        If paramList.ContainsKey(chgKbnEnt.charge2.PhysicsName) Then
            sqlString.AppendLine("," & makeSqlEx(chgKbnEnt.charge2, paramList))
        End If
        If paramList.ContainsKey(chgKbnEnt.charge3.PhysicsName) Then
            sqlString.AppendLine("," & makeSqlEx(chgKbnEnt.charge3, paramList))
        End If
        If paramList.ContainsKey(chgKbnEnt.charge4.PhysicsName) Then
            sqlString.AppendLine("," & makeSqlEx(chgKbnEnt.charge4, paramList))
        End If
        If paramList.ContainsKey(chgKbnEnt.charge5.PhysicsName) Then
            sqlString.AppendLine("," & makeSqlEx(chgKbnEnt.charge5, paramList))
        End If
        'WHERE句
        sqlString.AppendLine(" WHERE EXISTS ( ")
        sqlString.AppendLine(" SELECT ")
        sqlString.AppendLine(" 1 ")
        sqlString.AppendLine(" FROM ")
        sqlString.AppendLine(" T_CRS_LEDGER_BASIC BASIC ")
        sqlString.AppendLine(" WHERE ")
        sqlString.AppendLine(" " & makeSqlEx(chgKbnEnt.syuptDay, paramList))
        sqlString.AppendLine(" AND ")
        sqlString.AppendLine(" " & makeSqlEx(chgKbnEnt.crsCd, paramList))
        sqlString.AppendLine(" AND ")
        sqlString.AppendLine("" & makeSqlEx(chgKbnEnt.kbnNo, paramList))
        sqlString.AppendLine(" AND ")
        sqlString.AppendLine(" " & makeSqlEx(chgKbnEnt.chargeKbnJininCd, paramList))
        sqlString.AppendLine(" AND ")
        sqlString.AppendLine(" NVL(MARU_ZOU_MANAGEMENT_KBN,' ') <> 'M' ")
        sqlString.AppendLine(" AND ")
        sqlString.AppendLine(" BASIC.CRS_CD = CHARGE.CRS_CD ")
        sqlString.AppendLine(" AND ")
        sqlString.AppendLine(" BASIC.SYUPT_DAY = CHARGE.SYUPT_DAY ")
        sqlString.AppendLine(" AND ")
        sqlString.AppendLine(" BASIC.GOUSYA = CHARGE.GOUSYA ")
        sqlString.AppendLine(" ) ")

        Return sqlString.ToString

    End Function


    ''' <summary>
    ''' コース台帳（基本）：データ戻し用
    ''' </summary>
    ''' <param name="paramList">SQL引数</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Overloads Function executeUpdateBasicData(ByVal paramList As Hashtable) As String

        Dim sqlString As New StringBuilder

        'UPDATE
        sqlString.AppendLine(" UPDATE T_CRS_LEDGER_BASIC ")
        sqlString.AppendLine(" SET ")
        sqlString.AppendLine(" SYSTEM_UPDATE_DAY = " & setParam("SYSTEM_UPDATE_DAY", paramList.Item("SYSTEM_UPDATE_DAY"), OracleDbType.Date))
        sqlString.AppendLine(",SYSTEM_UPDATE_PGMID = '" & CType(paramList.Item("SYSTEM_UPDATE_PGMID"), String) & "' ")
        sqlString.AppendLine(",SYSTEM_UPDATE_PERSON_CD = '" & CType(paramList.Item("SYSTEM_UPDATE_PERSON_CD"), String) & "' ")
        'WHERE句
        sqlString.AppendLine(" WHERE ")
        sqlString.AppendLine(" SYUPT_DAY = " & CType(paramList.Item("SYUPT_DAY"), String) & "")
        sqlString.AppendLine(" AND ")
        sqlString.AppendLine(" CRS_CD = '" & CType(paramList.Item("CRS_CD"), String) & "' ")
        sqlString.AppendLine(" AND ")
        sqlString.AppendLine(" NVL(MARU_ZOU_MANAGEMENT_KBN,' ') <> 'M'")

        Return sqlString.ToString

    End Function

    ''' <summary>
    ''' コース台帳（基本）：データ戻し用
    ''' </summary>
    ''' <param name="paramEnt">コース台帳(料金)エンティティ</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Overloads Function executeUpdateBasicData(ByVal paramEnt As TCrsLedgerChargeEntityEx) As String

        Dim sqlString As New StringBuilder

        'UPDATE
        sqlString.AppendLine(" UPDATE T_CRS_LEDGER_BASIC ")
        sqlString.AppendLine(" SET ")
        sqlString.AppendLine("  " & makeSqlEx(paramEnt.systemUpdatePgmid))
        sqlString.AppendLine(" ," & makeSqlEx(paramEnt.systemUpdatePersonCd))
        sqlString.AppendLine(" ," & makeSqlEx(paramEnt.systemUpdateDay))
        'WHERE句
        sqlString.AppendLine(" WHERE ")
        sqlString.AppendLine(" " & makeSqlEx(paramEnt.syuptDay))
        sqlString.AppendLine(" AND ")
        sqlString.AppendLine(" " & makeSqlEx(paramEnt.syuptDay))
        sqlString.AppendLine(" AND ")
        sqlString.AppendLine(" NVL(MARU_ZOU_MANAGEMENT_KBN,' ') <> 'M'")

        Return sqlString.ToString

    End Function


    ''' <summary>
    ''' パラメータ設定
    ''' </summary>
    ''' <param name="ent">エンティティ項目</param>
    ''' <param name="paramList">SQLパラメータHashTable</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Overloads Function makeSqlEx(ByVal ent As IEntityKoumokuType, ByVal paramList As Hashtable) As String
        Return ent.PhysicsName & " = " & MyBase.setParam(ent.PhysicsName, paramList(ent.PhysicsName), ent.DBType, ent.IntegerBu, ent.DecimalBu)
    End Function

    ''' <summary>
    ''' パラメータ設定
    ''' </summary>
    ''' <param name="ent">エンティティ項目</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Overloads Function makeSqlEx(ByVal ent As IEntityKoumokuType) As String
        If TypeOf (ent) Is EntityKoumoku_MojiType Then
            Return ent.PhysicsName & " = " & MyBase.setParam(ent.PhysicsName, CType(ent, EntityKoumoku_MojiType).Value, ent.DBType, ent.IntegerBu, ent.DecimalBu)
        ElseIf TypeOf (ent) Is EntityKoumoku_NumberType Then
            Return ent.PhysicsName & " = " & MyBase.setParam(ent.PhysicsName, CType(ent, EntityKoumoku_NumberType).Value, ent.DBType, ent.IntegerBu, ent.DecimalBu)
        ElseIf TypeOf (ent) Is EntityKoumoku_Number_DecimalType Then
            Return ent.PhysicsName & " = " & MyBase.setParam(ent.PhysicsName, CType(ent, EntityKoumoku_Number_DecimalType).Value, ent.DBType, ent.IntegerBu, ent.DecimalBu)
        ElseIf TypeOf (ent) Is EntityKoumoku_YmdType Then
            Return ent.PhysicsName & " = " & MyBase.setParam(ent.PhysicsName, CType(ent, EntityKoumoku_YmdType).Value, ent.DBType, ent.IntegerBu, ent.DecimalBu)
        End If
        Return String.Empty
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
