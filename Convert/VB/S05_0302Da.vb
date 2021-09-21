Imports System.Text

''' <summary>
''' 現払登録
''' 現払登録対象コース照会画面から「現払登録」ボタンの押下で現払登録画面に遷移する。
''' 現払登録画面では、本社で出金された準備金に対し、旅行終了後に添乗中の現金支払情報の登録、
''' 降車ヶ所から収受した送客手数料（オレ伝、現金）、参加者からの代金収受の情報、
''' 運休となった降車ヶ所に運休の登録を行う
''' </summary>
''' <remarks>
''' Author:2020/04/01//DLSE 劉祥軍
''' </remarks>
Public Class S05_0302Da
    Inherits Hatobus.ReservationManagementSystem.Common.DataAccessorBase

#Region "定数／変数"

    ''' <summary>
    ''' Y
    ''' </summary>
    Private Const Y As String = "Y"

#End Region

#Region "検索"

    ''' <summary>
    ''' 営業所、添乗員等データを取得(IO項目定義:NO1)
    ''' </summary>
    ''' <param name="paramInfoList">検索条件</param>
    ''' <returns></returns>
    Public Function GetEiGyouDataByCondition(Optional ByVal paramInfoList As Hashtable = Nothing) As DataTable

        '戻り値
        Dim returnValue As DataTable = Nothing
        Dim sqlString As New StringBuilder

        ' SELECT句
        sqlString.AppendLine(" SELECT ")

        sqlString.AppendLine(" (CASE WHEN GE.WITHDRAWAL_EIGYOSYO_CD IS NULL THEN PLA.EIGYOSYO_CD ELSE  ")
        ' 出金営業所コード
        sqlString.AppendLine(" GE.WITHDRAWAL_EIGYOSYO_CD END) AS WITHDRAWAL_EIGYOSYO_CD, ")
        sqlString.AppendLine(" (SELECT ES.EIGYOSYO_NAME_1 FROM M_EIGYOSYO ES WHERE ES.COMPANY_CD = '00' AND ES.EIGYOSYO_CD =  ")
        sqlString.AppendLine(" (CASE WHEN GE.WITHDRAWAL_EIGYOSYO_CD IS NULL THEN PLA.EIGYOSYO_CD ELSE  ")
        ' 出金営業所名
        sqlString.AppendLine(" GE.WITHDRAWAL_EIGYOSYO_CD END)) AS WITHDRAWAL_EIGYOSYO_NAME, ")
        ' 帰着予定営業所コード
        sqlString.AppendLine(" GE.RETURN_YOTEI_EIGYOSYO_CD, ")
        sqlString.AppendLine(" (SELECT ES.EIGYOSYO_NAME_1 FROM M_EIGYOSYO ES WHERE ES.COMPANY_CD = '00'  ")
        ' 帰着予定営業所名
        sqlString.AppendLine(" AND ES.EIGYOSYO_CD = GE.RETURN_YOTEI_EIGYOSYO_CD) AS RETURN_YOTEI_EIGYOSYO_NAME, ")
        ' 添乗員コード
        sqlString.AppendLine(" (CASE WHEN GE.TENJYOIN_NAME IS NULL THEN CLB.TENJYOIN_CD ELSE NULL END) AS  TENJYOIN_CD, ")
        ' 添乗員名
        sqlString.AppendLine(" (CASE WHEN GE.TENJYOIN_NAME IS NOT NULL THEN GE.TENJYOIN_NAME ELSE TEN.TENJYOIN_NAME END) AS TENJYOIN_NAME,  ")
        ' 確定日
        sqlString.AppendLine(" GE.KAKUTEI_DAY,  ")
        ' 確定者コード
        sqlString.AppendLine(" GE.KAKUTEI_PERSON_CD,  ")
        ' 確認日１
        sqlString.AppendLine(" GE.KAKUNIN_DATE_1,  ")
        ' 確認者コード１
        sqlString.AppendLine(" GE.KAKUNIN_PERSON_CD_1,  ")
        ' 確認日２
        sqlString.AppendLine(" GE.KAKUNIN_DATE_2,  ")
        ' 確認者コード２
        sqlString.AppendLine(" GE.KAKUNIN_PERSON_CD_2,  ")
        ' 年
        sqlString.AppendLine(" CLB.YEAR,  ")
        ' 季コード
        sqlString.AppendLine(" CLB.SEASON_CD,  ")
        ' 年月
        sqlString.AppendLine(" SUBSTR(CLB.SYUPT_DAY,1,6) AS YM,  ")
        ' バス会社
        sqlString.AppendLine(" (CASE NVL(CLB.KYOSAI_UNKOU_KBN,'') WHEN '' THEN SS.SIIRE_SAKI_NAME_RK ELSE KUK.KYOSAI_UNKOU_KBN_NAME END) AS BUSCOMPANY  ")

        ' コース台帳（基本）
        sqlString.AppendLine(" FROM T_CRS_LEDGER_BASIC CLB ")
        ' 現払登録
        sqlString.AppendLine(" INNER JOIN T_GENBARAI_ENTRY GE ")
        ' コース台帳（基本）.コースコード = 現払登録.コースコード
        sqlString.AppendLine(" ON CLB.CRS_CD = GE.CRS_CD ")
        ' コース台帳（基本）.出発日 = 現払登録.出発日
        sqlString.AppendLine(" AND CLB.SYUPT_DAY = GE.SYUPT_DAY ")
        ' コース台帳（基本）.号車 = 現払登録.号車
        sqlString.AppendLine(" AND CLB.GOUSYA = GE.GOUSYA ")
        ' 仕入先マスタ
        sqlString.AppendLine(" LEFT JOIN M_SIIRE_SAKI SS ")
        ' コース台帳（基本）.バス会社コードの頭4桁 = 仕入先マスタ.仕入先コード
        sqlString.AppendLine(" ON SUBSTR(CLB.BUS_COMPANY_CD,1,4) = SS.SIIRE_SAKI_CD ")
        ' コース台帳（基本）.バス会社コードの下2桁 = 仕入先マスタ.仕入先枝番
        sqlString.AppendLine(" AND SUBSTR(CLB.BUS_COMPANY_CD,5,2) = SS.SIIRE_SAKI_NO ")
        ' 共催運行区分マスタ
        sqlString.AppendLine(" LEFT JOIN M_KYOSAI_UNKOU_KBN KUK ")
        ' コース台帳（基本）.共催運行区分 = 共催運行区分マスタ.共催運行区分
        sqlString.AppendLine(" ON CLB.KYOSAI_UNKOU_KBN = KUK.KYOSAI_UNKOU_KBN ")
        ' 添乗員マスタ
        sqlString.AppendLine(" LEFT JOIN M_TENJYOIN TEN ")
        ' コース台帳(基本）.添乗員コード = 添乗マスタ.添乗員コード
        sqlString.AppendLine(" ON CLB.TENJYOIN_CD = TEN.TENJYOIN_CD ")
        ' 場所マスタ
        sqlString.AppendLine(" LEFT JOIN M_PLACE PLA ")
        ' コース台帳(基本）.配車経由コード１ = 場所マスタ.場所コード
        sqlString.AppendLine(" ON CLB.HAISYA_KEIYU_CD_1 = PLA.PLACE_CD ")

        sqlString.AppendLine(" WHERE ")

        Dim crsLedgerBasicEntity As New TCrsLedgerBasicEntity

        ' コース台帳（基本）.コースコード = パラメータ.コースコード
        sqlString.AppendLine(" CLB.CRS_CD = ")
        sqlString.AppendLine(MyBase.setParam(crsLedgerBasicEntity.CrsCd.PhysicsName,
                                                paramInfoList.Item("CRS_CD").ToString,
                                                crsLedgerBasicEntity.CrsCd.DBType,
                                                crsLedgerBasicEntity.CrsCd.IntegerBu))

        ' コース台帳（基本）.出発日 = パラメータ.出発日
        sqlString.AppendLine(" AND CLB.SYUPT_DAY = ")
        sqlString.AppendLine(MyBase.setParam(crsLedgerBasicEntity.SyuptDay.PhysicsName,
                                                paramInfoList.Item("SYUPT_DAY").ToString,
                                                crsLedgerBasicEntity.SyuptDay.DBType,
                                                crsLedgerBasicEntity.SyuptDay.IntegerBu))

        '  コース台帳（基本）.号車 = パラメータ.号車
        sqlString.AppendLine(" AND CLB.GOUSYA = ")
        sqlString.AppendLine(MyBase.setParam(crsLedgerBasicEntity.Gousya.PhysicsName,
                                                paramInfoList.Item("GOUSYA").ToString,
                                                crsLedgerBasicEntity.Gousya.DBType,
                                                crsLedgerBasicEntity.Gousya.IntegerBu))

        Try
            returnValue = getDataTable(sqlString.ToString)
        Catch ex As Oracle.ManagedDataAccess.Client.OracleException
            Throw
        Catch ex As Exception
            Throw
        End Try

        Return returnValue

    End Function

    ''' <summary>
    ''' 現金支払内訳(IO項目定義:NO2)
    ''' </summary>
    ''' <param name="paramInfoList">検索条件</param>
    ''' <returns></returns>
    Public Function GetCostKakuteiGenkinUtiwakeByCondition(Optional ByVal paramInfoList As Hashtable = Nothing) As DataTable

        '戻り値
        Dim returnValue As DataTable = Nothing
        Dim sqlString As New StringBuilder

        ' SELECT句
        sqlString.AppendLine(" SELECT ")
        ' 利用日
        sqlString.AppendLine(" TO_CHAR(TO_DATE(CKGU.RIYOU_DAY,'yyyy-MM-dd'),'yyyy/MM/dd') AS RIYOU_DAY, ")
        ' 仕入先名（略称）
        sqlString.AppendLine(" SS.SIIRE_SAKI_NAME_RK, ")
        ' 原価支払内訳名
        sqlString.AppendLine(" CKGU.COST_SIHARAI_UTIWAKE_NAME, ")
        ' 仕訳区分
        sqlString.AppendLine(" CKGU.SIWAKE_KBN, ")
        ' 利用数
        sqlString.AppendLine(" CKGU.RIYOU_NUM, ")
        ' 税区分
        sqlString.AppendLine(" CKGU.TAX_KBN, ")
        ' 入湯税
        sqlString.AppendLine(" CKGU.BATH_TAX, ")
        ' 率
        sqlString.AppendLine(" CKGU.PER, ")
        ' 支出金額
        sqlString.AppendLine(" CKGU.SHISHUTSU_KINGAKU, ")
        ' 収入金額
        sqlString.AppendLine(" CKGU.SHUNYU_KINGAKU, ")
        ' メモ
        sqlString.AppendLine(" CKGU.MEMO, ")
        ' 行番号
        sqlString.AppendLine(" CKGU.LINE_NO, ")
        ' グループ番号
        sqlString.AppendLine(" CKGU.GROUP_NO, ")
        ' ＳＥＱ
        sqlString.AppendLine(" CKGU.SEQ, ")
        ' 原価支払内訳ｺｰﾄﾞ
        sqlString.AppendLine(" CKGU.COST_SIHARAI_UTIWAKE_CD, ")
        ' 新規区分
        sqlString.AppendLine(" CKGU.NEW_KBN, ")
        ' 仕入先ｺｰﾄﾞ
        sqlString.AppendLine(" CKGU.SIIRE_SAKI_CD, ")
        ' 仕入先枝番
        sqlString.AppendLine(" CKGU.SIIRE_SAKI_EDABAN, ")
        ' 単価区分
        sqlString.AppendLine(" CKGU.TANKA_KBN, ")
        ' 支払方法
        sqlString.AppendLine(" CKGU.SIHARAI_HOHO, ")
        ' 精算目的ｺｰﾄﾞ
        sqlString.AppendLine(" CKGU.SEISAN_MOKUTEKI_CD ")
        ' 原価確定現金内訳
        sqlString.AppendLine(" FROM T_COST_KAKUTEI_GENKIN_UTIWAKE CKGU ")
        ' 仕入先マスタ
        sqlString.AppendLine(" INNER JOIN M_SIIRE_SAKI SS ")
        ' 原価確定現金内訳.仕入先コード = 仕入先マスタ.仕入先コード
        sqlString.AppendLine(" ON CKGU.SIIRE_SAKI_CD = SS.SIIRE_SAKI_CD ")
        ' 原価確定現金内訳.仕入先枝番 = 仕入先マスタ.仕入先枝番
        sqlString.AppendLine(" AND CKGU.SIIRE_SAKI_EDABAN = SS.SIIRE_SAKI_NO ")
        sqlString.AppendLine(" WHERE ")

        Dim crsLedgerBasicEntity As New TCrsLedgerBasicEntity

        ' 原価確定現金内訳.コースコード = パラメータ.コースコード
        sqlString.AppendLine(" CKGU.CRS_CD =  ")
        sqlString.AppendLine(MyBase.setParam(crsLedgerBasicEntity.CrsCd.PhysicsName,
                                                paramInfoList.Item("CRS_CD").ToString,
                                                crsLedgerBasicEntity.CrsCd.DBType,
                                                crsLedgerBasicEntity.CrsCd.IntegerBu))

        ' 原価確定現金内訳.出発日 = パラメータ.出発日
        sqlString.AppendLine(" AND CKGU.SYUPT_DAY =  ")
        sqlString.AppendLine(MyBase.setParam(crsLedgerBasicEntity.SyuptDay.PhysicsName,
                                                paramInfoList.Item("SYUPT_DAY").ToString,
                                                crsLedgerBasicEntity.SyuptDay.DBType,
                                                crsLedgerBasicEntity.SyuptDay.IntegerBu))

        ' 原価確定現金内訳.号車 = パラメータ.号車
        sqlString.AppendLine(" AND CKGU.GOUSYA =  ")
        sqlString.AppendLine(MyBase.setParam(crsLedgerBasicEntity.Gousya.PhysicsName,
                                                paramInfoList.Item("GOUSYA").ToString,
                                                crsLedgerBasicEntity.Gousya.DBType,
                                                crsLedgerBasicEntity.Gousya.IntegerBu))

        ' 原価確定現金内訳.仕訳区分 <> '11' (準備金)
        sqlString.AppendLine(" AND CKGU.SIWAKE_KBN <> '11' ")

        ' 原価確定現金内訳.仕訳区分 <> '26' (送客手数料)
        sqlString.AppendLine(" AND CKGU.SIWAKE_KBN <> '26' ")
        sqlString.AppendLine(" ORDER BY CKGU.SEQ ")

        Try
            returnValue = getDataTable(sqlString.ToString)
        Catch ex As Oracle.ManagedDataAccess.Client.OracleException
            Throw
        Catch ex As Exception
            Throw
        End Try

        Return returnValue

    End Function

    ''' <summary>
    ''' （現金支払内訳）（降車ヶ所(徴証)）(IO項目定義:NO3)
    ''' </summary>
    ''' <param name="paramInfoList"></param>
    ''' <param name="kbn">Y:画面一覧（現金支払内訳）の場合 N:画面一覧（降車ヶ所(徴証)）の場合</param>
    ''' <returns></returns>
    Public Function GetCostKakuteiGenkinUtiwakeKoshakashoByCondition(Optional ByVal paramInfoList As Hashtable = Nothing,
                                                                     Optional ByVal kbn As String = Nothing) As DataTable

        '戻り値
        Dim returnValue As DataTable = Nothing
        Dim sqlString As New StringBuilder
        Dim crsLedgerBasicEntity As New TCrsLedgerBasicEntity

        ' SELECT句
        sqlString.AppendLine(" SELECT  ")
        ' 利用日
        sqlString.AppendLine(" TO_CHAR(TO_DATE(K.RIYOU_DAY,'yyyy-MM-dd'),'yyyy/MM/dd') AS RIYOU_DAY,  ")
        ' 仕入先名（略称）
        sqlString.AppendLine(" SS.SIIRE_SAKI_NAME_RK,  ")
        ' 数量
        sqlString.AppendLine(" K.SUYROU,  ")
        ' 単価数量
        sqlString.AppendLine(" K.TANKA_SUYROU,  ")
        ' 税区分
        sqlString.AppendLine(" K.TAX_KBN,  ")
        ' 入湯税
        sqlString.AppendLine(" K.BATH_TAX,  ")
        ' ＣＯＭ
        sqlString.AppendLine(" K.COM,  ")
        ' 行番号
        sqlString.AppendLine(" K.LINE_NO,  ")
        ' グループ番号
        sqlString.AppendLine(" K.GROUP_NO,  ")
        ' 仕入先コード
        sqlString.AppendLine(" K.SIIRE_SAKI_CD,  ")
        ' 仕入先枝番
        sqlString.AppendLine(" K.SIIRE_SAKI_EDABAN,  ")
        ' 精算方法
        sqlString.AppendLine(" K.SEISAN_HOHO,  ")
        ' バス単位
        sqlString.AppendLine(" K.BUS_TANI,  ")
        ' 単価区分
        sqlString.AppendLine(" K.TANKA_KBN,  ")
        ' 支払方法
        sqlString.AppendLine(" K.PAYMENT_HOHO,  ")
        ' 精算目的コード
        sqlString.AppendLine(" SS.SEISAN_TGT_CD,  ")
        ' 仕入先種別コード
        sqlString.AppendLine(" SS.SIIRE_SAKI_KIND_CD,  ")
        ' 被請求先目的
        sqlString.AppendLine(" SS.HI_SEIKYU_SAKI_TGT,  ")
        ' 被請求先コード
        sqlString.AppendLine(" SS.HI_SEIKYU_SAKI_CD,  ")
        ' 被請求先枝番
        sqlString.AppendLine(" SS.HI_SEIKYU_SAKI_NO  ")
        sqlString.AppendLine(" FROM  ")
        sqlString.AppendLine(" (  ")
        sqlString.AppendLine(" SELECT  ")
        sqlString.AppendLine("  CLCK.RIYOU_DAY,  ")
        sqlString.AppendLine("  CLCK.TAX_KBN,  ")
        sqlString.AppendLine("  CLCK.COM,  ")
        sqlString.AppendLine("  CLCK.CRS_ITINERARY_LINE_NO AS LINE_NO,  ")
        sqlString.AppendLine("  NULL AS GROUP_NO,  ")
        sqlString.AppendLine("  CLCK.SIIRE_SAKI_CD,  ")
        sqlString.AppendLine("  CLCK.SIIRE_SAKI_EDABAN,  ")
        sqlString.AppendLine("  '' AS TANKA_KBN,  ")
        sqlString.AppendLine("  '' AS PAYMENT_HOHO,  ")
        sqlString.AppendLine("  CLCK.SEISAN_HOHO,  ")
        sqlString.AppendLine("  CLCK.BUS_TANI,  ")
        sqlString.AppendLine("  NVL(SUM(  ")
        sqlString.AppendLine("    CASE  ")
        sqlString.AppendLine("    WHEN CLCK.BUS_TANI = '1'  ")
        sqlString.AppendLine("    THEN  1  ")
        sqlString.AppendLine("    ELSE   ")
        sqlString.AppendLine("           NVL(A.CHARGE_APPLICATION_NINZU_1,0)  ")
        sqlString.AppendLine("         + NVL(A.CHARGE_APPLICATION_NINZU_2,0)  ")
        sqlString.AppendLine("         + NVL(A.CHARGE_APPLICATION_NINZU_3,0)  ")
        sqlString.AppendLine("         + NVL(A.CHARGE_APPLICATION_NINZU_4,0)  ")
        sqlString.AppendLine("         + NVL(A.CHARGE_APPLICATION_NINZU_5,0)  ")
        sqlString.AppendLine("    END  ")
        sqlString.AppendLine("  ), 0) AS SUYROU,  ")
        sqlString.AppendLine("  NVL(SUM(CASE  ")
        sqlString.AppendLine("  WHEN SS.SIIRE_SAKI_KIND_CD <> '30'  ")
        sqlString.AppendLine("  THEN  ")
        sqlString.AppendLine("    CASE  ")
        sqlString.AppendLine("    WHEN CLCK.BUS_TANI = '1'  ")
        sqlString.AppendLine("    THEN NVL(CLCKCK.SIHARAI_GAKU,0)  ")
        sqlString.AppendLine("    ELSE NVL(CLCKCK.SIHARAI_GAKU,0) *   ")
        sqlString.AppendLine("             NVL(A.CHARGE_APPLICATION_NINZU_1,0)  ")
        sqlString.AppendLine("    END  ")
        sqlString.AppendLine("  ELSE  ")
        sqlString.AppendLine("    CASE  ")
        sqlString.AppendLine("    WHEN CLCK.BUS_TANI = '1'  ")
        sqlString.AppendLine("    THEN NVL(CLCKCK.CHARGE_1,0)  ")
        sqlString.AppendLine("    ELSE NVL(CLCKCK.CHARGE_1,0) *   ")
        sqlString.AppendLine("            NVL(A.CHARGE_APPLICATION_NINZU_1,0)  ")
        sqlString.AppendLine("          + NVL(CLCKCK.CHARGE_2,0) *   ")
        sqlString.AppendLine("            NVL(A.CHARGE_APPLICATION_NINZU_2,0)  ")
        sqlString.AppendLine("          + NVL(CLCKCK.CHARGE_3,0) *   ")
        sqlString.AppendLine("            NVL(A.CHARGE_APPLICATION_NINZU_3,0)  ")
        sqlString.AppendLine("          + NVL(CLCKCK.CHARGE_4,0) *   ")
        sqlString.AppendLine("            NVL(A.CHARGE_APPLICATION_NINZU_4,0)  ")
        sqlString.AppendLine("          + NVL(CLCKCK.CHARGE_5,0) *   ")
        sqlString.AppendLine("            NVL(A.CHARGE_APPLICATION_NINZU_5,0)  ")
        sqlString.AppendLine("    END  ")
        sqlString.AppendLine("  END), 0) AS TANKA_SUYROU,  ")
        sqlString.AppendLine("  NVL(SUM(CASE  ")
        sqlString.AppendLine("    WHEN SS.SIIRE_SAKI_KIND_CD <> '30'  ")
        sqlString.AppendLine("    THEN  ")
        sqlString.AppendLine("      NVL(CLCKCK.BATH_TAX,0) *   ")
        sqlString.AppendLine("      NVL(A.CHARGE_APPLICATION_NINZU_1,0)  ")
        sqlString.AppendLine("    ELSE  ")
        sqlString.AppendLine("      NVL(CLCKCK.BATH_TAX,0) *   ")
        sqlString.AppendLine("      (NVL(A.CHARGE_APPLICATION_NINZU_1,0)  ")
        sqlString.AppendLine("    + NVL(A.CHARGE_APPLICATION_NINZU_2,0)  ")
        sqlString.AppendLine("    + NVL(A.CHARGE_APPLICATION_NINZU_3,0)  ")
        sqlString.AppendLine("    + NVL(A.CHARGE_APPLICATION_NINZU_4,0)  ")
        sqlString.AppendLine("    + NVL(A.CHARGE_APPLICATION_NINZU_5,0))  ")
        sqlString.AppendLine("    END), 0) AS BATH_TAX   ")
        sqlString.AppendLine(" FROM   ")
        sqlString.AppendLine(" T_CRS_LEDGER_COST_KOSHAKASHO CLCK  ")
        sqlString.AppendLine(" INNER JOIN T_CRS_LEDGER_COST_KOSHAKASHO_CHARGE_KBN CLCKCK  ")
        sqlString.AppendLine(" ON CLCK.CRS_CD = CLCKCK.CRS_CD  ")
        sqlString.AppendLine(" AND CLCK.GOUSYA = CLCKCK.GOUSYA  ")
        sqlString.AppendLine(" AND CLCK.SYUPT_DAY = CLCKCK.SYUPT_DAY  ")
        sqlString.AppendLine(" AND CLCK.CRS_ITINERARY_LINE_NO = CLCKCK.CRS_ITINERARY_LINE_NO  ")
        sqlString.AppendLine(" AND CLCK.SIIRE_SAKI_CD = CLCKCK.SIIRE_SAKI_CD  ")
        sqlString.AppendLine(" AND CLCK.SIIRE_SAKI_EDABAN = CLCKCK.SIIRE_SAKI_EDABAN  ")
        sqlString.AppendLine(" INNER JOIN M_SIIRE_SAKI SS  ")
        sqlString.AppendLine(" ON CLCK.SIIRE_SAKI_CD = SS.SIIRE_SAKI_CD  ")
        sqlString.AppendLine(" AND CLCK.SIIRE_SAKI_EDABAN = SS.SIIRE_SAKI_NO  ")
        sqlString.AppendLine(" LEFT JOIN  ")
        sqlString.AppendLine(" (SELECT  ")
        sqlString.AppendLine(" YIB.CRS_CD,  ")
        sqlString.AppendLine(" YIB.GOUSYA,  ")
        sqlString.AppendLine(" YIB.SYUPT_DAY,  ")
        sqlString.AppendLine(" YICC.CHARGE_KBN_JININ_CD,  ")
        sqlString.AppendLine(" SUM(NVL(YICC.CHARGE_APPLICATION_NINZU_1,0)) AS CHARGE_APPLICATION_NINZU_1,  ")
        sqlString.AppendLine(" SUM(NVL(YICC.CHARGE_APPLICATION_NINZU_2,0)) AS CHARGE_APPLICATION_NINZU_2,  ")
        sqlString.AppendLine(" SUM(NVL(YICC.CHARGE_APPLICATION_NINZU_3,0)) AS CHARGE_APPLICATION_NINZU_3,  ")
        sqlString.AppendLine(" SUM(NVL(YICC.CHARGE_APPLICATION_NINZU_4,0)) AS CHARGE_APPLICATION_NINZU_4,  ")
        sqlString.AppendLine(" SUM(NVL(YICC.CHARGE_APPLICATION_NINZU_5,0)) AS CHARGE_APPLICATION_NINZU_5  ")
        sqlString.AppendLine(" FROM T_YOYAKU_INFO_BASIC YIB  ")
        sqlString.AppendLine(" INNER JOIN T_YOYAKU_INFO_CRS_CHARGE_CHARGE_KBN YICC  ")
        sqlString.AppendLine(" ON YIB.YOYAKU_KBN = YICC.YOYAKU_KBN  ")
        sqlString.AppendLine(" AND YIB.YOYAKU_NO = YICC.YOYAKU_NO  ")
        sqlString.AppendLine(" WHERE  ")
        sqlString.AppendLine(" YIB.CRS_CD =   ")
        sqlString.AppendLine(MyBase.setParam(crsLedgerBasicEntity.CrsCd.PhysicsName,
                                                paramInfoList.Item("CRS_CD").ToString,
                                                crsLedgerBasicEntity.CrsCd.DBType,
                                                crsLedgerBasicEntity.CrsCd.IntegerBu))
        sqlString.AppendLine(" AND YIB.GOUSYA =   ")
        sqlString.AppendLine(MyBase.setParam(crsLedgerBasicEntity.Gousya.PhysicsName,
                                                paramInfoList.Item("GOUSYA").ToString,
                                                crsLedgerBasicEntity.Gousya.DBType,
                                                crsLedgerBasicEntity.Gousya.IntegerBu))
        sqlString.AppendLine(" AND YIB.SYUPT_DAY =  ")
        sqlString.AppendLine(MyBase.setParam(crsLedgerBasicEntity.SyuptDay.PhysicsName,
                                            paramInfoList.Item("SYUPT_DAY").ToString,
                                            crsLedgerBasicEntity.SyuptDay.DBType,
                                            crsLedgerBasicEntity.SyuptDay.IntegerBu))
        sqlString.AppendLine(" AND NVL(YIB.CANCEL_FLG,' ') = ' '  ")
        sqlString.AppendLine(" GROUP BY YIB.CRS_CD,YIB.GOUSYA,YIB.SYUPT_DAY,YICC.CHARGE_KBN_JININ_CD) A  ")
        sqlString.AppendLine(" ON CLCK.CRS_CD = A.CRS_CD  ")
        sqlString.AppendLine(" AND CLCK.GOUSYA = A.GOUSYA  ")
        sqlString.AppendLine(" AND CLCK.SYUPT_DAY = A.SYUPT_DAY  ")
        sqlString.AppendLine(" WHERE  ")
        sqlString.AppendLine(" CLCK.CRS_CD =   ")
        sqlString.AppendLine(MyBase.setParam(crsLedgerBasicEntity.CrsCd.PhysicsName,
                                                paramInfoList.Item("CRS_CD").ToString,
                                                crsLedgerBasicEntity.CrsCd.DBType,
                                                crsLedgerBasicEntity.CrsCd.IntegerBu))
        sqlString.AppendLine(" AND CLCK.GOUSYA =   ")
        sqlString.AppendLine(MyBase.setParam(crsLedgerBasicEntity.Gousya.PhysicsName,
                                                paramInfoList.Item("GOUSYA").ToString,
                                                crsLedgerBasicEntity.Gousya.DBType,
                                                crsLedgerBasicEntity.Gousya.IntegerBu))
        sqlString.AppendLine(" AND CLCK.SYUPT_DAY =  ")
        sqlString.AppendLine(MyBase.setParam(crsLedgerBasicEntity.SyuptDay.PhysicsName,
                                            paramInfoList.Item("SYUPT_DAY").ToString,
                                            crsLedgerBasicEntity.SyuptDay.DBType,
                                            crsLedgerBasicEntity.SyuptDay.IntegerBu))
        If kbn.Equals(Y) = True Then
            ' 画面一覧（現金支払内訳）の場合
            sqlString.AppendLine(" AND CLCK.SEISAN_HOHO = '4'  ")
        Else
            ' 画面一覧（降車ヶ所(徴証)）の場合
            sqlString.AppendLine(" AND CLCK.SEISAN_HOHO <> '4'  ")
        End If

        sqlString.AppendLine(" GROUP BY  ")
        sqlString.AppendLine(" CLCK.RIYOU_DAY,  ")
        sqlString.AppendLine(" CLCK.TAX_KBN,  ")
        sqlString.AppendLine(" CLCK.COM,  ")
        sqlString.AppendLine(" CLCK.CRS_ITINERARY_LINE_NO,  ")
        sqlString.AppendLine(" CLCK.SIIRE_SAKI_CD,  ")
        sqlString.AppendLine(" CLCK.SIIRE_SAKI_EDABAN,  ")
        sqlString.AppendLine(" CLCK.SEISAN_HOHO,  ")
        sqlString.AppendLine(" CLCK.BUS_TANI  ")
        sqlString.AppendLine(" UNION ALL  ")
        sqlString.AppendLine("  SELECT   ")
        sqlString.AppendLine("  CLB.SYUPT_DAY AS RIYOU_DAY,  ")
        sqlString.AppendLine("  CLO.TAX_KBN,  ")
        sqlString.AppendLine("  CLO.COM,  ")
        sqlString.AppendLine("  CLO.LINE_NO,  ")
        sqlString.AppendLine("  CLO.GROUP_NO,  ")
        sqlString.AppendLine("  CLO.SIIRE_SAKI_CD,  ")
        sqlString.AppendLine("  CLO.SIIRE_SAKI_EDABAN,  ")
        sqlString.AppendLine("  CLOG.TANKA_KBN,  ")
        sqlString.AppendLine("  CLO.PAYMENT_HOHO,  ")
        sqlString.AppendLine("  CLO.SEISAN_HOHO,  ")
        sqlString.AppendLine("  '' AS BUS_TANI,  ")
        sqlString.AppendLine("  NVL(B.RIYOSU,0) AS SUYROU,  ")
        sqlString.AppendLine("  NVL(CLO.SIIRE_TANKA,0) * NVL(B.RIYOSU,0) AS TANKA_SUYROU,  ")
        sqlString.AppendLine("  0 AS BATH_TAX  ")
        sqlString.AppendLine("  FROM T_CRS_LEDGER_OPTION CLO  ")
        sqlString.AppendLine("  INNER JOIN T_CRS_LEDGER_OPTION_GROUP CLOG  ")
        sqlString.AppendLine("  ON CLO.CRS_CD = CLOG.CRS_CD  ")
        sqlString.AppendLine("  AND CLO.SYUPT_DAY = CLOG.SYUPT_DAY  ")
        sqlString.AppendLine("  AND CLO.GOUSYA = CLOG.GOUSYA  ")
        sqlString.AppendLine("  AND CLO.GROUP_NO = CLOG.GROUP_NO  ")
        sqlString.AppendLine("  INNER JOIN M_SIIRE_SAKI SS  ")
        sqlString.AppendLine("  ON CLO.SIIRE_SAKI_CD = SS.SIIRE_SAKI_CD  ")
        sqlString.AppendLine("  AND CLO.SIIRE_SAKI_EDABAN = SS.SIIRE_SAKI_NO  ")
        sqlString.AppendLine("  INNER JOIN T_CRS_LEDGER_BASIC CLB  ")
        sqlString.AppendLine("  ON CLO.CRS_CD = CLB.CRS_CD  ")
        sqlString.AppendLine("  AND CLO.SYUPT_DAY = CLB.SYUPT_DAY  ")
        sqlString.AppendLine("  AND CLO.GOUSYA = CLB.GOUSYA  ")
        sqlString.AppendLine("  LEFT JOIN   ")
        sqlString.AppendLine("  (SELECT  ")
        sqlString.AppendLine("  YIB.CRS_CD,  ")
        sqlString.AppendLine("  YIB.GOUSYA,  ")
        sqlString.AppendLine("  YIB.SYUPT_DAY,  ")
        sqlString.AppendLine("  YIO.GROUP_NO,  ")
        sqlString.AppendLine("  YIO.LINE_NO,  ")
        sqlString.AppendLine("  SUM(NVL(YIO.ADD_CHARGE_APPLICATION_NINZU,0)) AS RIYOSU  ")
        sqlString.AppendLine("  FROM T_YOYAKU_INFO_OPTION YIO  ")
        sqlString.AppendLine("  INNER JOIN T_YOYAKU_INFO_BASIC YIB  ")
        sqlString.AppendLine("  ON YIO.YOYAKU_KBN = YIB.YOYAKU_KBN  ")
        sqlString.AppendLine("  AND YIO.YOYAKU_NO = YIB.YOYAKU_NO  ")
        sqlString.AppendLine("  WHERE  ")
        sqlString.AppendLine("  YIB.CRS_CD =   ")
        sqlString.AppendLine(MyBase.setParam(crsLedgerBasicEntity.CrsCd.PhysicsName,
                                                paramInfoList.Item("CRS_CD").ToString,
                                                crsLedgerBasicEntity.CrsCd.DBType,
                                                crsLedgerBasicEntity.CrsCd.IntegerBu))
        sqlString.AppendLine("  AND YIB.GOUSYA =   ")
        sqlString.AppendLine(MyBase.setParam(crsLedgerBasicEntity.Gousya.PhysicsName,
                                                paramInfoList.Item("GOUSYA").ToString,
                                                crsLedgerBasicEntity.Gousya.DBType,
                                                crsLedgerBasicEntity.Gousya.IntegerBu))
        sqlString.AppendLine("  AND YIB.SYUPT_DAY =  ")
        sqlString.AppendLine(MyBase.setParam(crsLedgerBasicEntity.SyuptDay.PhysicsName,
                                            paramInfoList.Item("SYUPT_DAY").ToString,
                                            crsLedgerBasicEntity.SyuptDay.DBType,
                                            crsLedgerBasicEntity.SyuptDay.IntegerBu))
        sqlString.AppendLine("  AND NVL(YIB.CANCEL_FLG,' ') = ' '  ")
        sqlString.AppendLine("  GROUP BY YIB.CRS_CD,  ")
        sqlString.AppendLine("  YIB.GOUSYA,  ")
        sqlString.AppendLine("  YIB.SYUPT_DAY,  ")
        sqlString.AppendLine("  YIO.GROUP_NO,  ")
        sqlString.AppendLine("  YIO.LINE_NO) B  ")
        sqlString.AppendLine("  ON CLO.CRS_CD = B.CRS_CD  ")
        sqlString.AppendLine("  AND CLO.SYUPT_DAY = B.SYUPT_DAY  ")
        sqlString.AppendLine("  AND CLO.GOUSYA = B.GOUSYA  ")
        sqlString.AppendLine("  AND CLO.GROUP_NO = B.GROUP_NO  ")
        sqlString.AppendLine("  AND CLO.LINE_NO = B.LINE_NO  ")
        sqlString.AppendLine("  WHERE  ")
        sqlString.AppendLine("  CLO.CRS_CD =   ")
        sqlString.AppendLine(MyBase.setParam(crsLedgerBasicEntity.CrsCd.PhysicsName,
                                                paramInfoList.Item("CRS_CD").ToString,
                                                crsLedgerBasicEntity.CrsCd.DBType,
                                                crsLedgerBasicEntity.CrsCd.IntegerBu))
        sqlString.AppendLine("  AND CLO.SYUPT_DAY =   ")
        sqlString.AppendLine(MyBase.setParam(crsLedgerBasicEntity.SyuptDay.PhysicsName,
                                            paramInfoList.Item("SYUPT_DAY").ToString,
                                            crsLedgerBasicEntity.SyuptDay.DBType,
                                            crsLedgerBasicEntity.SyuptDay.IntegerBu))
        sqlString.AppendLine("  AND CLO.GOUSYA =  ")
        sqlString.AppendLine(MyBase.setParam(crsLedgerBasicEntity.Gousya.PhysicsName,
                                                paramInfoList.Item("GOUSYA").ToString,
                                                crsLedgerBasicEntity.Gousya.DBType,
                                                crsLedgerBasicEntity.Gousya.IntegerBu))
        If kbn.Equals(Y) = True Then
            ' 画面一覧（現金支払内訳）の場合
            sqlString.AppendLine(" AND CLO.SEISAN_HOHO = '4'  ")
        Else
            ' 画面一覧（降車ヶ所(徴証)）の場合
            sqlString.AppendLine(" AND CLO.SEISAN_HOHO <> '4'  ")
        End If

        sqlString.AppendLine("  ) K  ")
        ' 仕入先マスタ
        sqlString.AppendLine("  INNER JOIN M_SIIRE_SAKI SS  ")
        ' K.仕入先コード = 仕入先マスタ.仕入先コード
        sqlString.AppendLine("  ON K.SIIRE_SAKI_CD = SS.SIIRE_SAKI_CD  ")
        ' K.仕入先枝番 = 仕入先マスタ.仕入先枝番
        sqlString.AppendLine("  AND K.SIIRE_SAKI_EDABAN = SS.SIIRE_SAKI_NO  ")
        ' ORDER BY K.グループ番号,K.行番号
        sqlString.AppendLine("  ORDER BY K.GROUP_NO,K.LINE_NO  ")

        Try
            returnValue = getDataTable(sqlString.ToString)
        Catch ex As Oracle.ManagedDataAccess.Client.OracleException
            Throw
        Catch ex As Exception
            Throw
        End Try

        Return returnValue

    End Function

    ''' <summary>
    ''' 送客手数料(IO項目定義:NO4)
    ''' </summary>
    ''' <param name="paramInfoList"></param>
    ''' <returns></returns>
    Public Function GetCostKakuteiSokyakFee(Optional ByVal paramInfoList As Hashtable = Nothing) As DataTable

        '戻り値
        Dim returnValue As DataTable = Nothing
        Dim sqlString As New StringBuilder

        Dim costKakuteiSokyakFeeEntity As New TCostKakuteiSokyakFeeEntity

        ' SELECT句
        sqlString.AppendLine(" SELECT  ")
        ' 利用日
        sqlString.AppendLine(" TO_CHAR(TO_DATE(CKSF.RIYOU_DAY,'yyyy-MM-dd'),'yyyy/MM/dd') AS RIYOU_DAY, ")
        ' 仕入先名（略称）
        sqlString.AppendLine(" SS.SIIRE_SAKI_NAME_RK, ")
        ' 送客手数料現金払い区分
        sqlString.AppendLine(" CKSF.SOKYAK_FEE_GENKIN_PAYMENT_KBN, ")
        ' 伝票ＮＯ
        sqlString.AppendLine(" CKSF.DENPYOU_NO, ")
        ' 計算区分
        sqlString.AppendLine(" CKSF.SOKYAK_FEE_CALC_HOHO_KBN, ")
        sqlString.AppendLine(" （CASE CKSF.SOKYAK_FEE_CALC_HOHO_KBN ")
        sqlString.AppendLine("   WHEN 'A' ")
        sqlString.AppendLine("     THEN CKSF.DAISU ")
        sqlString.AppendLine("   WHEN 'B' ")
        sqlString.AppendLine("     THEN CKSF.NINZU ")
        sqlString.AppendLine(" ELSE NULL ")
        ' 数量
        sqlString.AppendLine(" END  ）AS SURYOU, ")
        sqlString.AppendLine(" （CASE CKSF.SOKYAK_FEE_CALC_HOHO_KBN ")
        sqlString.AppendLine("   WHEN 'A' ")
        sqlString.AppendLine("     THEN CKSF.TANKA ")
        sqlString.AppendLine("   WHEN 'B' ")
        sqlString.AppendLine("     THEN CKSF.TANKA ")
        sqlString.AppendLine("   WHEN 'C' ")
        sqlString.AppendLine("     THEN CKSF.URIAGE ")
        sqlString.AppendLine(" ELSE NULL ")
        ' 単価売上
        sqlString.AppendLine(" END  ）AS TANKAURIAGE, ")
        sqlString.AppendLine(" （CASE CKSF.SOKYAK_FEE_CALC_HOHO_KBN ")
        sqlString.AppendLine("   WHEN 'C' ")
        sqlString.AppendLine("     THEN CKSF.PER ")
        sqlString.AppendLine(" ELSE NULL ")
        ' 率
        sqlString.AppendLine(" END  ）AS PER,  ")
        ' 確認額
        sqlString.AppendLine(" CKSF.KAKUNIN_GAKU, ")
        ' 行番号
        sqlString.AppendLine(" CKSF.LINE_NO, ")
        ' ＳＥＱ
        sqlString.AppendLine(" CKSF.SEQ, ")
        ' 新規区分
        sqlString.AppendLine(" CKSF.NEW_KBN, ")
        ' 仕入先コード
        sqlString.AppendLine(" CKSF.SIIRE_SAKI_CD, ")
        ' 仕入先枝番
        sqlString.AppendLine(" CKSF.SIIRE_SAKI_EDABAN, ")
        ' 仕入先種別コード
        sqlString.AppendLine(" CKSF.SIIRE_SAKI_KIND_CD, ")
        ' 被請求先精算目的
        sqlString.AppendLine(" CKSF.HI_SEIKYU_SAKI_SEISAN_MOKUTEKI, ")
        ' 被請求先コード
        sqlString.AppendLine(" CKSF.HI_SEIKYU_SAKI_CD, ")
        ' 被請求先枝番
        sqlString.AppendLine(" CKSF.HI_SEIKYU_SAKI_EDABAN ")
        ' 原価確定送客手数料
        sqlString.AppendLine(" FROM T_COST_KAKUTEI_SOKYAK_FEE CKSF ")
        ' 仕入先マスタ
        sqlString.AppendLine(" INNER JOIN M_SIIRE_SAKI SS ")
        ' 原価確定送客手数料.仕入先コード = 仕入先マスタ.仕入先コード
        sqlString.AppendLine(" ON CKSF.SIIRE_SAKI_CD = SS.SIIRE_SAKI_CD ")
        ' 原価確定送客手数料.仕入先枝番 = 仕入先マスタ.仕入先枝番
        sqlString.AppendLine(" AND CKSF.SIIRE_SAKI_EDABAN = SS.SIIRE_SAKI_NO ")
        sqlString.AppendLine(" WHERE ")
        ' 原価確定送客手数料.コースコード = パラメータ.コースコード
        sqlString.AppendLine(" CKSF.CRS_CD =   ")
        sqlString.AppendLine(MyBase.setParam(costKakuteiSokyakFeeEntity.CrsCd.PhysicsName,
                                                paramInfoList.Item("CRS_CD").ToString,
                                                costKakuteiSokyakFeeEntity.CrsCd.DBType,
                                                costKakuteiSokyakFeeEntity.CrsCd.IntegerBu))
        ' 原価確定送客手数料.出発日 = パラメータ.出発日
        sqlString.AppendLine(" AND CKSF.SYUPT_DAY =  ")
        sqlString.AppendLine(MyBase.setParam(costKakuteiSokyakFeeEntity.SyuptDay.PhysicsName,
                                                paramInfoList.Item("SYUPT_DAY").ToString,
                                                costKakuteiSokyakFeeEntity.SyuptDay.DBType,
                                                costKakuteiSokyakFeeEntity.SyuptDay.IntegerBu))
        ' 原価確定送客手数料.号車 = パラメータ.号車
        sqlString.AppendLine(" AND CKSF.GOUSYA =   ")
        sqlString.AppendLine(MyBase.setParam(costKakuteiSokyakFeeEntity.Gousya.PhysicsName,
                                                paramInfoList.Item("GOUSYA").ToString,
                                                costKakuteiSokyakFeeEntity.Gousya.DBType,
                                                costKakuteiSokyakFeeEntity.Gousya.IntegerBu))

        ' ORDER BY 原価確定送客手数料.ＳＥＱ
        sqlString.AppendLine(" ORDER BY CKSF.SEQ  ")

        Try
            returnValue = getDataTable(sqlString.ToString)
        Catch ex As Oracle.ManagedDataAccess.Client.OracleException
            Throw
        Catch ex As Exception
            Throw
        End Try

        Return returnValue

    End Function

    ''' <summary>
    ''' 送客手数料(IO項目定義:NO5)
    ''' </summary>
    ''' <param name="paramInfoList"></param>
    ''' <returns></returns>
    Public Function GetCrsLedgerCostKoshakasho(Optional ByVal paramInfoList As Hashtable = Nothing) As DataTable

        '戻り値
        Dim returnValue As DataTable = Nothing
        Dim sqlString As New StringBuilder

        Dim crsLedgerCostKoshakashoEntity As New TCrsLedgerCostKoshakashoEntity

        ' SELECT句
        sqlString.AppendLine(" SELECT  ")
        ' 利用日
        sqlString.AppendLine(" TO_CHAR(TO_DATE(CLCK.RIYOU_DAY,'yyyy-MM-dd'),'yyyy/MM/dd') AS RIYOU_DAY,  ")
        ' 降車ヶ所
        sqlString.AppendLine(" SS.SIIRE_SAKI_NAME_RK,  ")
        ' 精算方法
        sqlString.AppendLine(" CLCK.SOKYAK_FEE_GENKIN_PAYMENT_KBN,  ")
        ' 計算区分
        sqlString.AppendLine(" CLCK.SOKYAK_FEE_CALC_HOHO_KBN,  ")
        sqlString.AppendLine(" (CASE CLCK.SOKYAK_FEE_CALC_HOHO_KBN  ")
        sqlString.AppendLine("   WHEN 'A'  ")
        sqlString.AppendLine("     THEN 1  ")
        sqlString.AppendLine("   WHEN 'B'  ")
        sqlString.AppendLine("     THEN A.NINSU  ")
        sqlString.AppendLine(" ELSE NULL  ")
        ' 数量
        sqlString.AppendLine(" END) AS SURYOU,  ")
        sqlString.AppendLine(" (CASE CLCK.SOKYAK_FEE_CALC_HOHO_KBN  ")
        sqlString.AppendLine("   WHEN 'A'  ")
        sqlString.AppendLine("     THEN CLCK.SOKYAK_FEE_TANKA_OR_PER  ")
        sqlString.AppendLine("   WHEN 'B'  ")
        sqlString.AppendLine("     THEN CLCK.SOKYAK_FEE_TANKA_OR_PER  ")
        sqlString.AppendLine(" ELSE NULL  ")
        ' 単価売上
        sqlString.AppendLine(" END) AS TANKAURIAGE,  ")
        sqlString.AppendLine(" (CASE CLCK.SOKYAK_FEE_CALC_HOHO_KBN  ")
        sqlString.AppendLine("   WHEN 'C'  ")
        sqlString.AppendLine("     THEN CLCK.SOKYAK_FEE_TANKA_OR_PER  ")
        sqlString.AppendLine(" ELSE NULL  ")
        ' ％
        sqlString.AppendLine(" END  ）AS PER,  ")
        ' 行番号
        sqlString.AppendLine(" CLCK.CRS_ITINERARY_LINE_NO,  ")
        ' 仕入先コード
        sqlString.AppendLine(" CLCK.SIIRE_SAKI_CD,  ")
        ' 仕入先枝番
        sqlString.AppendLine(" CLCK.SIIRE_SAKI_EDABAN,  ")
        ' 仕入先種別コード
        sqlString.AppendLine(" SS.SIIRE_SAKI_KIND_CD,  ")
        ' 被請求先精算目的
        sqlString.AppendLine(" SS.HI_SEIKYU_SAKI_TGT,  ")
        ' 被請求先コード
        sqlString.AppendLine(" SS.HI_SEIKYU_SAKI_CD,  ")
        ' 被請求先枝番
        sqlString.AppendLine(" SS.HI_SEIKYU_SAKI_NO  ")
        sqlString.AppendLine(" FROM T_CRS_LEDGER_COST_KOSHAKASHO CLCK  ")
        sqlString.AppendLine(" INNER JOIN M_SIIRE_SAKI SS   ")
        sqlString.AppendLine(" ON CLCK.SIIRE_SAKI_CD = SS.SIIRE_SAKI_CD  ")
        sqlString.AppendLine(" AND CLCK.SIIRE_SAKI_EDABAN = SS.SIIRE_SAKI_NO  ")
        sqlString.AppendLine(" LEFT JOIN  ")
        sqlString.AppendLine(" (SELECT   ")
        sqlString.AppendLine(" YIB.CRS_CD,  ")
        sqlString.AppendLine(" YIB.SYUPT_DAY,  ")
        sqlString.AppendLine(" YIB.GOUSYA,  ")
        sqlString.AppendLine(" SUM(NVL(YICCCK.CHARGE_APPLICATION_NINZU_1,0)   ")
        sqlString.AppendLine(" + NVL(YICCCK.CHARGE_APPLICATION_NINZU_2,0)   ")
        sqlString.AppendLine(" + NVL(YICCCK.CHARGE_APPLICATION_NINZU_3,0)   ")
        sqlString.AppendLine(" + NVL(YICCCK.CHARGE_APPLICATION_NINZU_4,0)   ")
        sqlString.AppendLine(" + NVL(YICCCK.CHARGE_APPLICATION_NINZU_5,0)) AS NINSU  ")
        sqlString.AppendLine(" FROM T_YOYAKU_INFO_BASIC YIB  ")
        sqlString.AppendLine(" INNER JOIN T_YOYAKU_INFO_CRS_CHARGE_CHARGE_KBN YICCCK  ")
        sqlString.AppendLine(" ON YIB.YOYAKU_KBN = YICCCK.YOYAKU_KBN  ")
        sqlString.AppendLine(" AND YIB.YOYAKU_NO = YICCCK.YOYAKU_NO  ")
        sqlString.AppendLine(" WHERE  ")
        ' 予約情報（基本）.コースコード = パラメータ.コースコード
        sqlString.AppendLine(" YIB.CRS_CD =  ")
        sqlString.AppendLine(MyBase.setParam(crsLedgerCostKoshakashoEntity.CrsCd.PhysicsName,
                                                paramInfoList.Item("CRS_CD").ToString,
                                                crsLedgerCostKoshakashoEntity.CrsCd.DBType,
                                                crsLedgerCostKoshakashoEntity.CrsCd.IntegerBu))
        ' 予約情報（基本）.出発日 = パラメータ.出発日
        sqlString.AppendLine(" AND YIB.SYUPT_DAY =  ")
        sqlString.AppendLine(MyBase.setParam(crsLedgerCostKoshakashoEntity.SyuptDay.PhysicsName,
                                                paramInfoList.Item("SYUPT_DAY").ToString,
                                                crsLedgerCostKoshakashoEntity.SyuptDay.DBType,
                                                crsLedgerCostKoshakashoEntity.SyuptDay.IntegerBu))
        ' 予約情報（基本）.号車 = パラメータ.号車
        sqlString.AppendLine(" AND YIB.GOUSYA =   ")
        sqlString.AppendLine(MyBase.setParam(crsLedgerCostKoshakashoEntity.Gousya.PhysicsName,
                                                paramInfoList.Item("GOUSYA").ToString,
                                                crsLedgerCostKoshakashoEntity.Gousya.DBType,
                                                crsLedgerCostKoshakashoEntity.Gousya.IntegerBu))
        ' NVL(予約情報（基本）.キャンセルフラグ, ' ') = ' '：半角スペース
        sqlString.AppendLine(" AND NVL(YIB.CANCEL_FLG, ' ') = ' '  ")
        sqlString.AppendLine(" GROUP BY   ")
        sqlString.AppendLine(" YIB.CRS_CD,  ")
        sqlString.AppendLine(" YIB.SYUPT_DAY,  ")
        sqlString.AppendLine(" YIB.GOUSYA) A  ")
        sqlString.AppendLine(" ON CLCK.CRS_CD = A.CRS_CD  ")
        sqlString.AppendLine(" AND CLCK.SYUPT_DAY = A.SYUPT_DAY  ")
        sqlString.AppendLine(" AND CLCK.GOUSYA = A.GOUSYA  ")
        sqlString.AppendLine(" WHERE  ")
        ' コース台帳原価（降車ヶ所）.コースコード = パラメータ.コースコード
        sqlString.AppendLine(" CLCK.CRS_CD =  ")
        sqlString.AppendLine(MyBase.setParam(crsLedgerCostKoshakashoEntity.CrsCd.PhysicsName,
                                                paramInfoList.Item("CRS_CD").ToString,
                                                crsLedgerCostKoshakashoEntity.CrsCd.DBType,
                                                crsLedgerCostKoshakashoEntity.CrsCd.IntegerBu))
        ' コース台帳原価（降車ヶ所）.出発日 = パラメータ.出発日
        sqlString.AppendLine(" AND CLCK.SYUPT_DAY =  ")
        sqlString.AppendLine(MyBase.setParam(crsLedgerCostKoshakashoEntity.SyuptDay.PhysicsName,
                                                paramInfoList.Item("SYUPT_DAY").ToString,
                                                crsLedgerCostKoshakashoEntity.SyuptDay.DBType,
                                                crsLedgerCostKoshakashoEntity.SyuptDay.IntegerBu))
        ' コース台帳原価（降車ヶ所）.号車 = パラメータ.号車
        sqlString.AppendLine(" AND CLCK.GOUSYA =   ")
        sqlString.AppendLine(MyBase.setParam(crsLedgerCostKoshakashoEntity.Gousya.PhysicsName,
                                                paramInfoList.Item("GOUSYA").ToString,
                                                crsLedgerCostKoshakashoEntity.Gousya.DBType,
                                                crsLedgerCostKoshakashoEntity.Gousya.IntegerBu))
        ' (NVL(コース台帳原価（降車ヶ所）.送客手数料 計算方法区分,' ') <> ' '
        sqlString.AppendLine(" AND (NVL(CLCK.SOKYAK_FEE_CALC_HOHO_KBN,' ') <> ' '  ")
        ' NVL(コース台帳原価（降車ヶ所）.送客手数料現金払い区分,' ') <> ' ')
        sqlString.AppendLine(" OR NVL(CLCK.SOKYAK_FEE_GENKIN_PAYMENT_KBN,' ') <> ' ')  ")
        sqlString.AppendLine(" ORDER BY CLCK.RIYOU_DAY  ")

        Try
            returnValue = getDataTable(sqlString.ToString)
        Catch ex As Oracle.ManagedDataAccess.Client.OracleException
            Throw
        Catch ex As Exception
            Throw
        End Try

        Return returnValue

    End Function

    ''' <summary>
    ''' （徴証）(IO項目定義:NO6)
    ''' </summary>
    ''' <param name="paramInfoList"></param>
    ''' <returns></returns>
    Public Function GetCostKakuteiChosho(Optional ByVal paramInfoList As Hashtable = Nothing) As DataTable

        '戻り値
        Dim returnValue As DataTable = Nothing
        Dim sqlString As New StringBuilder

        Dim costKakuteiChoshoEntity As New TCostKakuteiChoshoEntity

        ' SELECT句
        sqlString.AppendLine(" SELECT  ")
        ' 利用日付
        sqlString.AppendLine(" TO_CHAR(TO_DATE(CKC.RIYOU_YMD,'yyyy-MM-dd'),'yyyy/MM/dd') AS RIYOU_DAY, ")
        ' 仕入先名（略称）
        sqlString.AppendLine(" SS.SIIRE_SAKI_NAME_RK, ")
        ' 精算方法
        sqlString.AppendLine(" CKC.SEISAN_HOHO, ")
        ' 人数
        sqlString.AppendLine(" CKC.NINZU, ")
        ' 税区分
        sqlString.AppendLine(" CKC.TAX_KBN, ")
        ' 入湯税
        sqlString.AppendLine(" CKC.BATH_TAX, ")
        ' ＣＯＭ
        sqlString.AppendLine(" CKC.COM, ")
        ' 確認額
        sqlString.AppendLine(" CKC.KAKUNIN_GAKU, ")
        ' 降車ヶ所運休区分
        sqlString.AppendLine(" CKC.KOSHAKASHO_UNKYUU_KBN, ")
        ' メモ
        sqlString.AppendLine(" CKC.MEMO, ")
        ' 行番号
        sqlString.AppendLine(" CKC.LINE_NO, ")
        ' グループ番号
        sqlString.AppendLine(" CKC.GROUP_NO, ")
        ' SEQ
        sqlString.AppendLine(" CKC.SEQ, ")
        ' 新規区分
        sqlString.AppendLine(" CKC.NEW_KBN, ")
        ' 仕入先コード
        sqlString.AppendLine(" CKC.SIIRE_SAKI_CD, ")
        ' 仕入先枝番
        sqlString.AppendLine(" CKC.SIIRE_SAKI_EDABAN, ")
        ' 仕入先識別コード
        sqlString.AppendLine(" CKC.SIIRE_SAKI_SHIKIBETSU_CD, ")
        ' 精算目的コード
        sqlString.AppendLine(" CKC.SEISAN_MOKUTEKI_CD, ")
        ' 消費税
        sqlString.AppendLine(" CKC.SYOHI_TAX, ")
        ' ＣＯＭ額
        sqlString.AppendLine(" CKC.COM_GAKU, ")
        ' ＮＥＴ
        sqlString.AppendLine(" CKC.NET, ")
        ' 被請求先精算目的
        sqlString.AppendLine(" CKC.HI_SEIKYU_SAKI_SEISAN_MOKUTEKI, ")
        ' 被請求先ｺｰﾄﾞ
        sqlString.AppendLine(" CKC.HI_SEIKYU_SAKI_CD, ")
        ' 被請求先枝番
        sqlString.AppendLine(" CKC.HI_SEIKYU_SAKI_EDABAN ")
        ' 原価確定徴証
        sqlString.AppendLine(" FROM T_COST_KAKUTEI_CHOSHO CKC ")
        ' 仕入先マスタ
        sqlString.AppendLine(" INNER JOIN M_SIIRE_SAKI SS ")
        ' 原価確定徴証.仕入先コード = 仕入先マスタ.仕入先コード
        sqlString.AppendLine(" ON CKC.SIIRE_SAKI_CD = SS.SIIRE_SAKI_CD ")
        ' 原価確定徴証.仕入先枝番 = 仕入先マスタ.仕入先枝番
        sqlString.AppendLine(" AND CKC.SIIRE_SAKI_EDABAN = SS.SIIRE_SAKI_NO ")
        sqlString.AppendLine(" WHERE ")
        ' 原価確定徴証.コースコード = パラメータ.コースコード
        sqlString.AppendLine(" CKC.CRS_CD =   ")
        sqlString.AppendLine(MyBase.setParam(costKakuteiChoshoEntity.CrsCd.PhysicsName,
                                                paramInfoList.Item("CRS_CD").ToString,
                                                costKakuteiChoshoEntity.CrsCd.DBType,
                                                costKakuteiChoshoEntity.CrsCd.IntegerBu))
        ' 原価確定徴証.出発日 = パラメータ.出発日
        sqlString.AppendLine(" AND CKC.SYUPT_DAY =  ")
        sqlString.AppendLine(MyBase.setParam(costKakuteiChoshoEntity.SyuptDay.PhysicsName,
                                                paramInfoList.Item("SYUPT_DAY").ToString,
                                                costKakuteiChoshoEntity.SyuptDay.DBType,
                                                costKakuteiChoshoEntity.SyuptDay.IntegerBu))
        ' 原価確定徴証.号車 = パラメータ.号車
        sqlString.AppendLine(" AND CKC.GOUSYA =   ")
        sqlString.AppendLine(MyBase.setParam(costKakuteiChoshoEntity.Gousya.PhysicsName,
                                                paramInfoList.Item("GOUSYA").ToString,
                                                costKakuteiChoshoEntity.Gousya.DBType,
                                                costKakuteiChoshoEntity.Gousya.IntegerBu))

        ' ORDER BY 原価確定徴証.ＳＥＱ
        sqlString.AppendLine(" ORDER BY CKC.SEQ  ")

        Try
            returnValue = getDataTable(sqlString.ToString)
        Catch ex As Oracle.ManagedDataAccess.Client.OracleException
            Throw
        Catch ex As Exception
            Throw
        End Try

        Return returnValue

    End Function

    ''' <summary>
    ''' （徴証）原価確定徴証(料金情報)現払出金履歴(IO項目定義:NO7)
    ''' </summary>
    ''' <param name="paramInfoList"></param>
    ''' <returns></returns>
    Public Function GetCostKakuteiChoshoChargeInfo(Optional ByVal paramInfoList As Hashtable = Nothing) As DataTable

        '戻り値
        Dim returnValue As DataTable = Nothing
        Dim sqlString As New StringBuilder

        Dim costKakuteiChoshoChargeInfoEntity As New TCostKakuteiChoshoChargeInfoEntity

        ' SELECT句
        sqlString.AppendLine(" SELECT  ")
        ' SEQ
        sqlString.AppendLine(" CKCCI.SEQ, ")
        ' 料金区分(人員)コード
        sqlString.AppendLine(" CKCCI.CHARGE_KBN_JININ_CD, ")
        ' 表示順
        sqlString.AppendLine(" CJK.DISPLAY_ORDER, ")
        ' 入湯税単価
        sqlString.AppendLine(" CKCCI.BATH_TAX_TANKA, ")
        ' 単価
        sqlString.AppendLine(" CKCCI.TANKA, ")
        ' 人数
        sqlString.AppendLine(" CKCCI.NINZU, ")
        ' 料金１
        sqlString.AppendLine(" CKCCI.CHARGE_1, ")
        ' 料金２
        sqlString.AppendLine(" CKCCI.CHARGE_2, ")
        ' 料金３
        sqlString.AppendLine(" CKCCI.CHARGE_3, ")
        ' 料金４
        sqlString.AppendLine(" CKCCI.CHARGE_4, ")
        ' 料金５
        sqlString.AppendLine(" CKCCI.CHARGE_5, ")
        ' 人数１
        sqlString.AppendLine(" CKCCI.NINZU_1, ")
        ' 人数２
        sqlString.AppendLine(" CKCCI.NINZU_2, ")
        ' 人数３
        sqlString.AppendLine(" CKCCI.NINZU_3, ")
        ' 人数４
        sqlString.AppendLine(" CKCCI.NINZU_4, ")
        ' 人数５
        sqlString.AppendLine(" CKCCI.NINZU_5 ")
        ' 原価確定徴証(料金情報)
        sqlString.AppendLine(" FROM T_COST_KAKUTEI_CHOSHO_CHARGE_INFO CKCCI ")
        ' 料金区分（人員）マスタ
        sqlString.AppendLine(" LEFT JOIN M_CHARGE_JININ_KBN CJK ")
        ' 原価確定徴証(料金情報).料金区分（人員）コード = 料金区分（人員）マスタ.料金区分（人員）コード
        sqlString.AppendLine(" ON CKCCI.CHARGE_KBN_JININ_CD = CJK.CHARGE_KBN_JININ_CD ")
        sqlString.AppendLine(" WHERE ")
        ' 原価確定徴証(料金情報).コースコード = パラメータ.コースコード
        sqlString.AppendLine(" CKCCI.CRS_CD =   ")
        sqlString.AppendLine(MyBase.setParam(costKakuteiChoshoChargeInfoEntity.CrsCd.PhysicsName,
                                                paramInfoList.Item("CRS_CD").ToString,
                                                costKakuteiChoshoChargeInfoEntity.CrsCd.DBType,
                                                costKakuteiChoshoChargeInfoEntity.CrsCd.IntegerBu))
        ' 原価確定徴証(料金情報).出発日 = パラメータ.出発日
        sqlString.AppendLine(" AND CKCCI.SYUPT_DAY =  ")
        sqlString.AppendLine(MyBase.setParam(costKakuteiChoshoChargeInfoEntity.SyuptDay.PhysicsName,
                                                paramInfoList.Item("SYUPT_DAY").ToString,
                                                costKakuteiChoshoChargeInfoEntity.SyuptDay.DBType,
                                                costKakuteiChoshoChargeInfoEntity.SyuptDay.IntegerBu))
        ' 原価確定徴証(料金情報).号車 = パラメータ.号車
        sqlString.AppendLine(" AND CKCCI.GOUSYA =   ")
        sqlString.AppendLine(MyBase.setParam(costKakuteiChoshoChargeInfoEntity.Gousya.PhysicsName,
                                                paramInfoList.Item("GOUSYA").ToString,
                                                costKakuteiChoshoChargeInfoEntity.Gousya.DBType,
                                                costKakuteiChoshoChargeInfoEntity.Gousya.IntegerBu))

        ' ORDER BY 原価確定徴証.ＳＥＱ
        sqlString.AppendLine(" ORDER BY CKCCI.SEQ,CKCCI.CHARGE_KBN_JININ_CD  ")

        Try
            returnValue = getDataTable(sqlString.ToString)
        Catch ex As Oracle.ManagedDataAccess.Client.OracleException
            Throw
        Catch ex As Exception
            Throw
        End Try

        Return returnValue

    End Function

    ''' <summary>
    ''' 現払徴証料金情報(IO項目定義:NO8)
    ''' </summary>
    ''' <param name="paramInfoList"></param>
    ''' <returns></returns>
    Public Function GetCrsLedgerCostKoshakashoClckck(Optional ByVal paramInfoList As Hashtable = Nothing,
                                                     Optional ByVal kbn As String = Nothing) As DataTable

        '戻り値
        Dim returnValue As DataTable = Nothing
        Dim sqlString As New StringBuilder

        Dim crsLedgerCostKoshakashoEntity As New TCrsLedgerCostKoshakashoEntity

        ' SELECT句
        sqlString.AppendLine(" SELECT  ")
        ' コース行程行ＮＯ
        sqlString.AppendLine(" CLCK.CRS_ITINERARY_LINE_NO,  ")
        ' グループ番号
        sqlString.AppendLine(" NULL AS GROUP_NO,  ")
        ' 仕入先コード
        sqlString.AppendLine(" CLCK.SIIRE_SAKI_CD,  ")
        ' 仕入先枝番
        sqlString.AppendLine(" CLCK.SIIRE_SAKI_EDABAN,  ")
        ' 料金区分(人員)コード
        sqlString.AppendLine(" CLCKCK.CHARGE_KBN_JININ_CD,  ")
        ' 表示順
        sqlString.AppendLine(" CJK.DISPLAY_ORDER,  ")
        ' 入湯税
        sqlString.AppendLine(" NVL(CLCKCK.BATH_TAX,0) AS BATH_TAX,  ")
        sqlString.AppendLine(" NVL(CASE  ")
        sqlString.AppendLine("     WHEN SS.SIIRE_SAKI_KIND_CD <> '30'  ")
        sqlString.AppendLine("     THEN  ")
        sqlString.AppendLine("       NVL(CLCKCK.SIHARAI_GAKU,0)  ")
        sqlString.AppendLine("     ELSE 0  ")
        ' 単価
        sqlString.AppendLine("     END, 0) AS TANKA,  ")
        sqlString.AppendLine(" 	NVL(CASE  ")
        sqlString.AppendLine("         WHEN SS.SIIRE_SAKI_KIND_CD <> '30'  ")
        sqlString.AppendLine("         THEN  ")
        sqlString.AppendLine("           CASE  ")
        sqlString.AppendLine("           WHEN CLCK.BUS_TANI = '1'  ")
        sqlString.AppendLine("           THEN  1  ")
        sqlString.AppendLine("           ELSE   ")
        sqlString.AppendLine("             NVL(A.CHARGE_APPLICATION_NINZU_1,0)  ")
        sqlString.AppendLine("           END  ")
        sqlString.AppendLine("         ELSE 0  ")
        ' 人数
        sqlString.AppendLine("         END, 0) AS NINSUU,  ")
        sqlString.AppendLine(" NVL(CASE  ")
        sqlString.AppendLine("         WHEN SS.SIIRE_SAKI_KIND_CD <> '30'   ")
        sqlString.AppendLine("         THEN 0  ")
        sqlString.AppendLine("         ELSE  ")
        sqlString.AppendLine("           NVL(CLCKCK.CHARGE_1,0)  ")
        ' 料金１
        sqlString.AppendLine("         END, 0) AS CHARGE_1,  ")
        sqlString.AppendLine(" NVL(CASE  ")
        sqlString.AppendLine("         WHEN SS.SIIRE_SAKI_KIND_CD <> '30'   ")
        sqlString.AppendLine("         THEN 0  ")
        sqlString.AppendLine("         ELSE  ")
        sqlString.AppendLine("           NVL(CLCKCK.CHARGE_2,0)  ")
        ' 料金２
        sqlString.AppendLine("         END, 0) AS CHARGE_2,  ")
        sqlString.AppendLine(" NVL(CASE  ")
        sqlString.AppendLine("         WHEN SS.SIIRE_SAKI_KIND_CD <> '30'   ")
        sqlString.AppendLine("         THEN 0  ")
        sqlString.AppendLine("         ELSE  ")
        sqlString.AppendLine("           NVL(CLCKCK.CHARGE_3,0)  ")
        ' 料金３
        sqlString.AppendLine("         END, 0) AS CHARGE_3,  ")
        sqlString.AppendLine(" NVL(CASE  ")
        sqlString.AppendLine("         WHEN SS.SIIRE_SAKI_KIND_CD <> '30'   ")
        sqlString.AppendLine("         THEN 0  ")
        sqlString.AppendLine("         ELSE  ")
        sqlString.AppendLine("           NVL(CLCKCK.CHARGE_4,0)  ")
        ' 料金４
        sqlString.AppendLine("         END, 0) AS CHARGE_4,  ")
        sqlString.AppendLine(" NVL(CASE  ")
        sqlString.AppendLine("         WHEN SS.SIIRE_SAKI_KIND_CD <> '30'   ")
        sqlString.AppendLine("         THEN 0  ")
        sqlString.AppendLine("         ELSE  ")
        sqlString.AppendLine("           NVL(CLCKCK.CHARGE_5,0)  ")
        ' 料金５
        sqlString.AppendLine("         END, 0) AS CHARGE_5,  ")
        sqlString.AppendLine(" NVL(CASE  ")
        sqlString.AppendLine("         WHEN SS.SIIRE_SAKI_KIND_CD <> '30'  ")
        sqlString.AppendLine("         THEN 0  ")
        sqlString.AppendLine("         ELSE  ")
        sqlString.AppendLine("           CASE  ")
        sqlString.AppendLine("           WHEN CLCK.BUS_TANI = '1'  ")
        sqlString.AppendLine("           THEN  1  ")
        sqlString.AppendLine("           ELSE   ")
        sqlString.AppendLine("             NVL(A.CHARGE_APPLICATION_NINZU_1,0)  ")
        sqlString.AppendLine("           END  ")
        ' 人数１
        sqlString.AppendLine("         END, 0) AS NINZU1,  ")
        sqlString.AppendLine(" NVL(CASE  ")
        sqlString.AppendLine("         WHEN SS.SIIRE_SAKI_KIND_CD <> '30'  ")
        sqlString.AppendLine("         THEN 0  ")
        sqlString.AppendLine("         ELSE  ")
        sqlString.AppendLine("           CASE  ")
        sqlString.AppendLine("           WHEN CLCK.BUS_TANI = '1'  ")
        sqlString.AppendLine("           THEN  1  ")
        sqlString.AppendLine("           ELSE   ")
        sqlString.AppendLine("             NVL(A.CHARGE_APPLICATION_NINZU_2,0)  ")
        sqlString.AppendLine("           END  ")
        ' 人数２
        sqlString.AppendLine("         END, 0) AS NINZU2,  ")
        sqlString.AppendLine(" NVL(CASE  ")
        sqlString.AppendLine("         WHEN SS.SIIRE_SAKI_KIND_CD <> '30'  ")
        sqlString.AppendLine("         THEN 0  ")
        sqlString.AppendLine("         ELSE  ")
        sqlString.AppendLine("           CASE  ")
        sqlString.AppendLine("           WHEN CLCK.BUS_TANI = '1'  ")
        sqlString.AppendLine("           THEN  1  ")
        sqlString.AppendLine("           ELSE   ")
        sqlString.AppendLine("             NVL(A.CHARGE_APPLICATION_NINZU_3,0)  ")
        sqlString.AppendLine("           END  ")
        ' 人数３
        sqlString.AppendLine("         END, 0) AS NINZU3,  ")
        sqlString.AppendLine(" NVL(CASE  ")
        sqlString.AppendLine("         WHEN SS.SIIRE_SAKI_KIND_CD <> '30'  ")
        sqlString.AppendLine("         THEN 0  ")
        sqlString.AppendLine("         ELSE  ")
        sqlString.AppendLine("           CASE  ")
        sqlString.AppendLine("           WHEN CLCK.BUS_TANI = '1'  ")
        sqlString.AppendLine("           THEN  1  ")
        sqlString.AppendLine("           ELSE   ")
        sqlString.AppendLine("             NVL(A.CHARGE_APPLICATION_NINZU_4,0)  ")
        sqlString.AppendLine("           END  ")
        ' 人数４
        sqlString.AppendLine("         END, 0) AS NINZU4,  ")
        sqlString.AppendLine(" NVL(CASE  ")
        sqlString.AppendLine("         WHEN SS.SIIRE_SAKI_KIND_CD <> '30'  ")
        sqlString.AppendLine("         THEN 0  ")
        sqlString.AppendLine("         ELSE  ")
        sqlString.AppendLine("           CASE  ")
        sqlString.AppendLine("           WHEN CLCK.BUS_TANI = '1'  ")
        sqlString.AppendLine("           THEN  1  ")
        sqlString.AppendLine("           ELSE   ")
        sqlString.AppendLine("             NVL(A.CHARGE_APPLICATION_NINZU_5,0)  ")
        sqlString.AppendLine("           END  ")
        ' 人数５
        sqlString.AppendLine("         END, 0) AS NINZU5  ")
        ' コース台帳原価（降車ヶ所）
        sqlString.AppendLine(" FROM T_CRS_LEDGER_COST_KOSHAKASHO CLCK  ")
        ' コース台帳原価（降車ヶ所_料金区分）
        sqlString.AppendLine(" INNER JOIN T_CRS_LEDGER_COST_KOSHAKASHO_CHARGE_KBN CLCKCK  ")
        ' コース台帳原価（降車ヶ所）.コースコード = コース台帳原価（降車ヶ所_料金区分）.コースコード
        sqlString.AppendLine(" ON CLCK.CRS_CD = CLCKCK.CRS_CD  ")
        ' コース台帳原価（降車ヶ所）.出発日 = コース台帳原価（降車ヶ所_料金区分）.出発日
        sqlString.AppendLine(" AND CLCK.SYUPT_DAY = CLCKCK.SYUPT_DAY  ")
        ' コース台帳原価（降車ヶ所）.号車 = コース台帳原価（降車ヶ所_料金区分）.号車
        sqlString.AppendLine(" AND CLCK.GOUSYA = CLCKCK.GOUSYA  ")
        ' コース台帳原価（降車ヶ所）.コース行程行ＮＯ = コース台帳原価（降車ヶ所_料金区分）.コース行程行ＮＯ
        sqlString.AppendLine(" AND CLCK.CRS_ITINERARY_LINE_NO = CLCKCK.CRS_ITINERARY_LINE_NO  ")
        ' コース台帳原価（降車ヶ所）.仕入先コード = コース台帳原価（降車ヶ所_料金区分）.仕入先コード
        sqlString.AppendLine(" AND CLCK.SIIRE_SAKI_CD = CLCKCK.SIIRE_SAKI_CD  ")
        ' コース台帳原価（降車ヶ所）.仕入先枝番 = コース台帳原価（降車ヶ所_料金区分）.仕入先枝番
        sqlString.AppendLine(" AND CLCK.SIIRE_SAKI_EDABAN = CLCKCK.SIIRE_SAKI_EDABAN  ")
        ' 仕入先マスタ
        sqlString.AppendLine(" INNER JOIN M_SIIRE_SAKI SS  ")
        ' コース台帳原価（降車ヶ所）.仕入先コード = 仕入先マスタ.仕入先コード
        sqlString.AppendLine(" ON CLCK.SIIRE_SAKI_CD = SS.SIIRE_SAKI_CD  ")
        ' コース台帳原価（降車ヶ所）.仕入先枝番 = 仕入先マスタ.仕入先枝番
        sqlString.AppendLine(" AND CLCK.SIIRE_SAKI_EDABAN = SS.SIIRE_SAKI_NO  ")
        sqlString.AppendLine(" LEFT JOIN  ")
        sqlString.AppendLine(" (SELECT  ")
        sqlString.AppendLine(" YIB.CRS_CD,  ")
        sqlString.AppendLine(" YIB.GOUSYA,  ")
        sqlString.AppendLine(" YIB.SYUPT_DAY,  ")
        sqlString.AppendLine(" YICC.CHARGE_KBN_JININ_CD,  ")
        sqlString.AppendLine(" SUM(NVL(YICC.CHARGE_APPLICATION_NINZU_1,0)) AS CHARGE_APPLICATION_NINZU_1,  ")
        sqlString.AppendLine(" SUM(NVL(YICC.CHARGE_APPLICATION_NINZU_2,0)) AS CHARGE_APPLICATION_NINZU_2,  ")
        sqlString.AppendLine(" SUM(NVL(YICC.CHARGE_APPLICATION_NINZU_3,0)) AS CHARGE_APPLICATION_NINZU_3,  ")
        sqlString.AppendLine(" SUM(NVL(YICC.CHARGE_APPLICATION_NINZU_4,0)) AS CHARGE_APPLICATION_NINZU_4,  ")
        sqlString.AppendLine(" SUM(NVL(YICC.CHARGE_APPLICATION_NINZU_5,0)) AS CHARGE_APPLICATION_NINZU_5  ")
        sqlString.AppendLine(" FROM T_YOYAKU_INFO_BASIC YIB  ")
        sqlString.AppendLine(" INNER JOIN T_YOYAKU_INFO_CRS_CHARGE_CHARGE_KBN YICC  ")
        sqlString.AppendLine(" ON YIB.YOYAKU_KBN = YICC.YOYAKU_KBN  ")
        sqlString.AppendLine(" AND YIB.YOYAKU_NO = YICC.YOYAKU_NO  ")
        sqlString.AppendLine(" WHERE  ")
        sqlString.AppendLine(" YIB.CRS_CD =   ")
        sqlString.AppendLine(MyBase.setParam(crsLedgerCostKoshakashoEntity.CrsCd.PhysicsName,
                                                paramInfoList.Item("CRS_CD").ToString,
                                                crsLedgerCostKoshakashoEntity.CrsCd.DBType,
                                                crsLedgerCostKoshakashoEntity.CrsCd.IntegerBu))
        sqlString.AppendLine(" AND YIB.GOUSYA =   ")
        sqlString.AppendLine(MyBase.setParam(crsLedgerCostKoshakashoEntity.Gousya.PhysicsName,
                                                paramInfoList.Item("GOUSYA").ToString,
                                                crsLedgerCostKoshakashoEntity.Gousya.DBType,
                                                crsLedgerCostKoshakashoEntity.Gousya.IntegerBu))
        sqlString.AppendLine(" AND YIB.SYUPT_DAY =  ")
        sqlString.AppendLine(MyBase.setParam(crsLedgerCostKoshakashoEntity.SyuptDay.PhysicsName,
                                            paramInfoList.Item("SYUPT_DAY").ToString,
                                            crsLedgerCostKoshakashoEntity.SyuptDay.DBType,
                                            crsLedgerCostKoshakashoEntity.SyuptDay.IntegerBu))
        sqlString.AppendLine(" AND NVL(YIB.CANCEL_FLG,' ') = ' '  ")
        ' ※A
        sqlString.AppendLine(" GROUP BY YIB.CRS_CD,YIB.GOUSYA,YIB.SYUPT_DAY,YICC.CHARGE_KBN_JININ_CD) A  ")
        ' コース台帳原価（降車ヶ所）.コースコード = ※A.コースコード
        sqlString.AppendLine(" ON CLCK.CRS_CD = A.CRS_CD  ")
        ' コース台帳原価（降車ヶ所）.出発日 = ※A.出発日
        sqlString.AppendLine(" AND CLCK.GOUSYA = A.GOUSYA  ")
        ' コース台帳原価（降車ヶ所）.号車 = ※A.号車
        sqlString.AppendLine(" AND CLCK.SYUPT_DAY = A.SYUPT_DAY  ")
        ' コース台帳原価（降車ヶ所_料金区分）.料金区分（人員）コード = ※A.料金区分（人員）コード"
        sqlString.AppendLine(" AND CLCKCK.CHARGE_KBN_JININ_CD = A.CHARGE_KBN_JININ_CD  ")
        ' 料金区分（人員）マスタ
        sqlString.AppendLine(" LEFT JOIN M_CHARGE_JININ_KBN CJK  ")
        ' コース台帳原価（降車ヶ所_料金区分）.料金区分（人員）コード = 料金区分（人員）マスタ.料金区分（人員）コード
        sqlString.AppendLine(" ON CLCKCK.CHARGE_KBN_JININ_CD = CJK.CHARGE_KBN_JININ_CD  ")
        sqlString.AppendLine(" WHERE  ")
        ' コース台帳原価（降車ヶ所）.コースコード = パラメータ.コースコード
        sqlString.AppendLine(" CLCK.CRS_CD =   ")
        sqlString.AppendLine(MyBase.setParam(crsLedgerCostKoshakashoEntity.CrsCd.PhysicsName,
                                                paramInfoList.Item("CRS_CD").ToString,
                                                crsLedgerCostKoshakashoEntity.CrsCd.DBType,
                                                crsLedgerCostKoshakashoEntity.CrsCd.IntegerBu))
        ' コース台帳原価（降車ヶ所）.号車 = パラメータ.号車
        sqlString.AppendLine(" AND CLCK.GOUSYA =   ")
        sqlString.AppendLine(MyBase.setParam(crsLedgerCostKoshakashoEntity.Gousya.PhysicsName,
                                                paramInfoList.Item("GOUSYA").ToString,
                                                crsLedgerCostKoshakashoEntity.Gousya.DBType,
                                                crsLedgerCostKoshakashoEntity.Gousya.IntegerBu))
        ' コース台帳原価（降車ヶ所）.出発日 = パラメータ.出発日
        sqlString.AppendLine(" AND CLCK.SYUPT_DAY =  ")
        sqlString.AppendLine(MyBase.setParam(crsLedgerCostKoshakashoEntity.SyuptDay.PhysicsName,
                                            paramInfoList.Item("SYUPT_DAY").ToString,
                                            crsLedgerCostKoshakashoEntity.SyuptDay.DBType,
                                            crsLedgerCostKoshakashoEntity.SyuptDay.IntegerBu))
        If kbn.Equals(Y) = True Then
            ' 画面一覧（現金支払内訳）の場合
            sqlString.AppendLine(" AND CLCK.SEISAN_HOHO = '4'  ")
        Else
            ' 画面一覧（降車ヶ所(徴証)）の場合
            sqlString.AppendLine(" AND CLCK.SEISAN_HOHO <> '4'  ")
        End If

        sqlString.AppendLine(" UNION ALL  ")
        sqlString.AppendLine(" SELECT  * FROM ")

        sqlString.AppendLine(" (SELECT  ")
        ' 行ＮＯ
        sqlString.AppendLine(" CLO.LINE_NO,  ")
        ' グループ番号
        sqlString.AppendLine(" CLO.GROUP_NO,  ")
        ' 仕入先コード
        sqlString.AppendLine(" CLO.SIIRE_SAKI_CD,  ")
        ' 仕入先枝番
        sqlString.AppendLine(" CLO.SIIRE_SAKI_EDABAN,  ")
        ' 料金区分(人員)コード
        sqlString.AppendLine(" '10' AS CHARGE_KBN_JININ_CD,  ")
        ' 表示順
        sqlString.AppendLine(" CJK.DISPLAY_ORDER,  ")
        ' 入湯税単価
        sqlString.AppendLine(" 0 AS BATH_TAX,  ")
        sqlString.AppendLine(" NVL(CASE  ")
        sqlString.AppendLine("         WHEN SS.SIIRE_SAKI_KIND_CD <> '30'  ")
        sqlString.AppendLine("         THEN  ")
        sqlString.AppendLine("           NVL(CLO.SIIRE_TANKA,0)   ")
        sqlString.AppendLine("         ELSE 0  ")
        ' 単価
        sqlString.AppendLine("         END, 0) AS TANKA,  ")
        sqlString.AppendLine(" NVL(CASE  ")
        sqlString.AppendLine("         WHEN SS.SIIRE_SAKI_KIND_CD <> '30'  ")
        sqlString.AppendLine("         THEN  ")
        sqlString.AppendLine("           NVL(B.RIYOSU,0)  ")
        sqlString.AppendLine("         ELSE 0  ")
        ' 人数
        sqlString.AppendLine("         END, 0) AS NINSUU,  ")
        sqlString.AppendLine(" NVL(CASE  ")
        sqlString.AppendLine("         WHEN SS.SIIRE_SAKI_KIND_CD <> '30'  ")
        sqlString.AppendLine("         THEN 0  ")
        sqlString.AppendLine("         ELSE  ")
        sqlString.AppendLine("           NVL(CLO.SIIRE_TANKA,0)   ")
        ' 料金１
        sqlString.AppendLine("         END, 0) AS CHARGE_1,  ")
        ' 料金２
        sqlString.AppendLine(" 0 AS CHARGE_2,  ")
        ' 料金３
        sqlString.AppendLine(" 0 AS CHARGE_3,  ")
        ' 料金４
        sqlString.AppendLine(" 0 AS CHARGE_4,  ")
        ' 料金５
        sqlString.AppendLine(" 0 AS CHARGE_5,  ")
        sqlString.AppendLine(" NVL(CASE  ")
        sqlString.AppendLine("         WHEN SS.SIIRE_SAKI_KIND_CD <> '30'  ")
        sqlString.AppendLine("         THEN 0  ")
        sqlString.AppendLine("         ELSE  ")
        sqlString.AppendLine("           NVL(B.RIYOSU,0)  ")
        ' 人数１
        sqlString.AppendLine("         END, 0) AS NINZU1,  ")
        ' 人数２
        sqlString.AppendLine(" 0 AS NINZU2,  ")
        ' 人数３
        sqlString.AppendLine(" 0 AS NINZU3,  ")
        ' 人数４
        sqlString.AppendLine(" 0 AS NINZU4,  ")
        ' 人数５
        sqlString.AppendLine(" 0 AS NINZU5  ")
        ' コース台帳（オプション）
        sqlString.AppendLine(" FROM T_CRS_LEDGER_OPTION CLO  ")
        ' 仕入先マスタ
        sqlString.AppendLine(" INNER JOIN M_SIIRE_SAKI SS  ")
        ' コース台帳（オプション）.仕入先コード = 仕入先マスタ.仕入先コード
        sqlString.AppendLine(" ON CLO.SIIRE_SAKI_CD = SS.SIIRE_SAKI_CD  ")
        ' コース台帳（オプション）.仕入先枝番 = 仕入先マスタ.仕入先枝番
        sqlString.AppendLine(" AND CLO.SIIRE_SAKI_EDABAN = SS.SIIRE_SAKI_NO  ")
        sqlString.AppendLine(" LEFT JOIN  ")
        ' ※B
        sqlString.AppendLine("  (SELECT  ")
        sqlString.AppendLine("  YIB.CRS_CD,  ")
        sqlString.AppendLine("  YIB.GOUSYA,  ")
        sqlString.AppendLine("  YIB.SYUPT_DAY,  ")
        sqlString.AppendLine("  YIO.GROUP_NO,  ")
        sqlString.AppendLine("  YIO.LINE_NO,  ")
        sqlString.AppendLine("  SUM(NVL(YIO.ADD_CHARGE_APPLICATION_NINZU,0)) AS RIYOSU  ")
        sqlString.AppendLine("  FROM T_YOYAKU_INFO_OPTION YIO  ")
        sqlString.AppendLine("  INNER JOIN T_YOYAKU_INFO_BASIC YIB  ")
        sqlString.AppendLine("  ON YIO.YOYAKU_KBN = YIB.YOYAKU_KBN  ")
        sqlString.AppendLine("  AND YIO.YOYAKU_NO = YIB.YOYAKU_NO  ")
        sqlString.AppendLine("  WHERE  ")
        sqlString.AppendLine("  YIB.CRS_CD =   ")
        sqlString.AppendLine(MyBase.setParam(crsLedgerCostKoshakashoEntity.CrsCd.PhysicsName,
                                                paramInfoList.Item("CRS_CD").ToString,
                                                crsLedgerCostKoshakashoEntity.CrsCd.DBType,
                                                crsLedgerCostKoshakashoEntity.CrsCd.IntegerBu))
        sqlString.AppendLine("  AND YIB.GOUSYA =   ")
        sqlString.AppendLine(MyBase.setParam(crsLedgerCostKoshakashoEntity.Gousya.PhysicsName,
                                                paramInfoList.Item("GOUSYA").ToString,
                                                crsLedgerCostKoshakashoEntity.Gousya.DBType,
                                                crsLedgerCostKoshakashoEntity.Gousya.IntegerBu))
        sqlString.AppendLine("  AND YIB.SYUPT_DAY =  ")
        sqlString.AppendLine(MyBase.setParam(crsLedgerCostKoshakashoEntity.SyuptDay.PhysicsName,
                                            paramInfoList.Item("SYUPT_DAY").ToString,
                                            crsLedgerCostKoshakashoEntity.SyuptDay.DBType,
                                            crsLedgerCostKoshakashoEntity.SyuptDay.IntegerBu))
        sqlString.AppendLine("  AND NVL(YIB.CANCEL_FLG,' ') = ' '  ")
        sqlString.AppendLine("  GROUP BY YIB.CRS_CD,  ")
        sqlString.AppendLine("  YIB.GOUSYA,  ")
        sqlString.AppendLine("  YIB.SYUPT_DAY,  ")
        sqlString.AppendLine("  YIO.GROUP_NO,  ")
        sqlString.AppendLine("  YIO.LINE_NO) B  ")
        ' コース台帳（オプション）.コースコード = ※B.コースコード
        sqlString.AppendLine("  ON CLO.CRS_CD = B.CRS_CD  ")
        ' コース台帳（オプション）.出発日 = ※B.出発日
        sqlString.AppendLine("  AND CLO.SYUPT_DAY = B.SYUPT_DAY  ")
        ' コース台帳（オプション）.号車 = ※B.号車
        sqlString.AppendLine("  AND CLO.GOUSYA = B.GOUSYA  ")
        ' コース台帳（オプション）.グループ番号 = ※B.グループ番号
        sqlString.AppendLine("  AND CLO.GROUP_NO = B.GROUP_NO  ")
        ' コース台帳（オプション）.行No = ※B.行No
        sqlString.AppendLine("  AND CLO.LINE_NO = B.LINE_NO  ")
        ' 料金区分（人員）マスタ
        sqlString.AppendLine(" LEFT JOIN M_CHARGE_JININ_KBN CJK   ")
        ' 料金区分（人員）マスタ.料金区分（人員）コード='10'
        sqlString.AppendLine(" ON CJK.CHARGE_KBN_JININ_CD = '10'  ")
        sqlString.AppendLine(" WHERE  ")
        ' コース台帳（オプション）.コースコード = パラメータ.コースコード
        sqlString.AppendLine(" CLO.CRS_CD =   ")
        sqlString.AppendLine(MyBase.setParam(crsLedgerCostKoshakashoEntity.CrsCd.PhysicsName,
                                                paramInfoList.Item("CRS_CD").ToString,
                                                crsLedgerCostKoshakashoEntity.CrsCd.DBType,
                                                crsLedgerCostKoshakashoEntity.CrsCd.IntegerBu))
        ' コース台帳（オプション）.号車 = パラメータ.号車
        sqlString.AppendLine(" AND CLO.GOUSYA =   ")
        sqlString.AppendLine(MyBase.setParam(crsLedgerCostKoshakashoEntity.Gousya.PhysicsName,
                                                paramInfoList.Item("GOUSYA").ToString,
                                                crsLedgerCostKoshakashoEntity.Gousya.DBType,
                                                crsLedgerCostKoshakashoEntity.Gousya.IntegerBu))
        ' コース台帳（オプション）.出発日 = パラメータ.出発日
        sqlString.AppendLine(" AND CLO.SYUPT_DAY =  ")
        sqlString.AppendLine(MyBase.setParam(crsLedgerCostKoshakashoEntity.SyuptDay.PhysicsName,
                                            paramInfoList.Item("SYUPT_DAY").ToString,
                                            crsLedgerCostKoshakashoEntity.SyuptDay.DBType,
                                            crsLedgerCostKoshakashoEntity.SyuptDay.IntegerBu))
        If kbn.Equals(Y) = True Then
            ' 画面一覧（現金支払内訳）の場合
            sqlString.AppendLine(" AND CLO.SEISAN_HOHO = '4'  ")
        Else
            ' 画面一覧（降車ヶ所(徴証)）の場合
            sqlString.AppendLine(" AND CLO.SEISAN_HOHO <> '4'  ")
        End If

        ' ORDER BY グループ番号,行番号
        sqlString.AppendLine(" ORDER BY CLO.GROUP_NO,CLO.LINE_NO)  ")

        Try
            returnValue = getDataTable(sqlString.ToString)
        Catch ex As Oracle.ManagedDataAccess.Client.OracleException
            Throw
        Catch ex As Exception
            Throw
        End Try

        Return returnValue

    End Function

    ''' <summary>
    ''' 現払出金履歴(IO項目定義:NO9)
    ''' </summary>
    ''' <param name="paramInfoList"></param>
    ''' <returns></returns>
    Public Function GetGenbaraiWithdrawalHistory(Optional ByVal paramInfoList As Hashtable = Nothing) As DataTable

        '戻り値
        Dim returnValue As DataTable = Nothing
        Dim sqlString As New StringBuilder

        Dim genbaraiWithdrawalHistoryEntity As New TGenbaraiWithdrawalHistoryEntity

        ' SELECT句
        sqlString.AppendLine(" SELECT  * FROM ( SELECT  ")
        ' 出金日
        sqlString.AppendLine(" TO_CHAR(TO_DATE(GWH.WITHDRAWAL_DAY,'yyyy/MM/dd'),'yy/MM/dd') AS WITHDRAWAL_DAY ,  ")
        sqlString.AppendLine(" GWH.WITHDRAWAL_DAY AS WITHDRAWALDAY ,  ")
        ' 準備金
        sqlString.AppendLine(" GWH.WITHDRAWAL_GAKU AS JUNBIKIN,  ")
        ' 登録者
        sqlString.AppendLine(" U.USER_NAME,  ")
        sqlString.AppendLine(" U.USER_ID,  ")
        ' SEQ
        sqlString.AppendLine(" GWH.SEQ,  ")
        ' 新規区分
        sqlString.AppendLine(" '1' AS NEW_KBN  ")
        ' 現払出金履歴
        sqlString.AppendLine(" FROM T_GENBARAI_WITHDRAWAL_HISTORY GWH  ")
        ' 利用者情報
        sqlString.AppendLine(" INNER JOIN M_USER U  ")
        ' 利用者情報.会社コード = '0001'
        sqlString.AppendLine(" ON U.COMPANY_CD = '0001'  ")
        ' 利用者情報.ユーザID = 現払出金履歴.システム登録者コード
        sqlString.AppendLine(" AND U.USER_ID = GWH.SYSTEM_ENTRY_PERSON_CD  ")
        sqlString.AppendLine(" WHERE   ")
        sqlString.AppendLine(" GWH.CRS_CD =   ")
        sqlString.AppendLine(MyBase.setParam(genbaraiWithdrawalHistoryEntity.CrsCd.PhysicsName,
                                                paramInfoList.Item("CRS_CD").ToString,
                                                genbaraiWithdrawalHistoryEntity.CrsCd.DBType,
                                                genbaraiWithdrawalHistoryEntity.CrsCd.IntegerBu))
        sqlString.AppendLine(" AND GWH.SYUPT_DAY =  ")
        sqlString.AppendLine(MyBase.setParam(genbaraiWithdrawalHistoryEntity.SyuptDay.PhysicsName,
                                                paramInfoList.Item("SYUPT_DAY").ToString,
                                                genbaraiWithdrawalHistoryEntity.SyuptDay.DBType,
                                                genbaraiWithdrawalHistoryEntity.SyuptDay.IntegerBu))
        sqlString.AppendLine(" AND GWH.GOUSYA =   ")
        sqlString.AppendLine(MyBase.setParam(genbaraiWithdrawalHistoryEntity.Gousya.PhysicsName,
                                                paramInfoList.Item("GOUSYA").ToString,
                                                genbaraiWithdrawalHistoryEntity.Gousya.DBType,
                                                genbaraiWithdrawalHistoryEntity.Gousya.IntegerBu))
        sqlString.AppendLine(" UNION ALL  ")
        sqlString.AppendLine(" SELECT  ")
        ' 出金日
        sqlString.AppendLine(" TO_CHAR(TJ.SYSTEM_ENTRY_DAY,'yy/MM/dd') AS WITHDRAWAL_DAY,  ")
        sqlString.AppendLine(" TO_NUMBER(TO_CHAR(TJ.SYSTEM_ENTRY_DAY,'yyyyMMdd')) AS WITHDRAWALDAY,  ")
        ' 準備金
        sqlString.AppendLine(" TJ.DELIVERY_KINGAKU AS JUNBIKIN,  ")
        ' 登録者
        sqlString.AppendLine(" U.USER_NAME,  ")
        sqlString.AppendLine(" U.USER_ID,  ")
        ' SEQ
        sqlString.AppendLine(" 0 AS SEQ,  ")
        ' 新規区分
        sqlString.AppendLine(" '2' AS NEW_KBN  ")
        ' 添乗準備金
        sqlString.AppendLine(" FROM T_TENJYO_JUNBIKIN TJ  ")
        ' 利用者情報
        sqlString.AppendLine(" INNER JOIN M_USER U  ")
        ' 利用者情報.会社コード = '0001'
        sqlString.AppendLine(" ON U.COMPANY_CD = '0001'  ")
        ' 利用者情報.ユーザID = 添乗準備金.システム登録者コード
        sqlString.AppendLine(" AND U.USER_ID = TJ.SYSTEM_ENTRY_PERSON_CD  ")
        sqlString.AppendLine(" WHERE   ")
        sqlString.AppendLine(" TJ.CRS_CD =   ")
        sqlString.AppendLine(MyBase.setParam(genbaraiWithdrawalHistoryEntity.CrsCd.PhysicsName,
                                                paramInfoList.Item("CRS_CD").ToString,
                                                genbaraiWithdrawalHistoryEntity.CrsCd.DBType,
                                                genbaraiWithdrawalHistoryEntity.CrsCd.IntegerBu))
        sqlString.AppendLine(" AND TJ.SYUPT_DAY =  ")
        sqlString.AppendLine(MyBase.setParam(genbaraiWithdrawalHistoryEntity.SyuptDay.PhysicsName,
                                                paramInfoList.Item("SYUPT_DAY").ToString,
                                                genbaraiWithdrawalHistoryEntity.SyuptDay.DBType,
                                                genbaraiWithdrawalHistoryEntity.SyuptDay.IntegerBu))
        sqlString.AppendLine(" AND TJ.GOUSYA =   ")
        sqlString.AppendLine(MyBase.setParam(genbaraiWithdrawalHistoryEntity.Gousya.PhysicsName,
                                                paramInfoList.Item("GOUSYA").ToString,
                                                genbaraiWithdrawalHistoryEntity.Gousya.DBType,
                                                genbaraiWithdrawalHistoryEntity.Gousya.IntegerBu))
        ' 添乗準備金.削除日 = 0
        sqlString.AppendLine(" AND TJ.DELETE_DAY = 0  ")
        ' 添乗準備金.入力確定フラグ = '1'
        sqlString.AppendLine(" AND TJ.INPUT_KAKUTEI_FLG = '1'  ")

        ' ORDER BY 出金日、ＳＥＱ
        sqlString.AppendLine(" ORDER BY WITHDRAWAL_DAY,SEQ  ")

        sqlString.AppendLine(" ) ORDER BY NEW_KBN DESC  ")

        Try
            returnValue = getDataTable(sqlString.ToString)
        Catch ex As Oracle.ManagedDataAccess.Client.OracleException
            Throw
        Catch ex As Exception
            Throw
        End Try

        Return returnValue

    End Function

    ''' <summary>
    ''' 追加ボタン（降車ヶ所(徴証)(IO項目定義:NO17)
    ''' </summary>
    ''' <param name="paramInfoList"></param>
    ''' <returns></returns>
    Public Function GetAddBtnKoshakashoChoshoByCondition(Optional ByVal paramInfoList As Hashtable = Nothing) As DataTable

        '戻り値
        Dim returnValue As DataTable = Nothing
        Dim sqlString As New StringBuilder

        ' SELECT句
        sqlString.AppendLine(" SELECT  ")
        ' 料金区分（人員）コード
        sqlString.AppendLine(" CLCC.CHARGE_KBN_JININ_CD, ")
        ' 表示順
        sqlString.AppendLine(" CJK.DISPLAY_ORDER ")
        ' コース台帳（料金_料金区分）
        sqlString.AppendLine(" FROM T_CRS_LEDGER_CHARGE_CHARGE_KBN CLCC ")
        ' 料金区分（人員）マスタ
        sqlString.AppendLine(" INNER JOIN M_CHARGE_JININ_KBN MCJK ")
        ' 料金区分（人員）マスタ.料金区分（人員）コード=コース台帳（料金_料金区分）.料金区分（人員）コード
        sqlString.AppendLine(" ON CLCC.CHARGE_KBN_JININ_CD = MCJK.CHARGE_KBN_JININ_CD ")
        sqlString.AppendLine(" WHERE ")

        Dim crsLedgerBasicEntity As New TCrsLedgerBasicEntity

        ' コース台帳（料金_料金区分）.コースコード = パラメータ.コースコード
        sqlString.AppendLine(" CLCC.CRS_CD =  ")
        sqlString.AppendLine(MyBase.setParam(crsLedgerBasicEntity.CrsCd.PhysicsName,
                                                paramInfoList.Item("CRS_CD").ToString,
                                                crsLedgerBasicEntity.CrsCd.DBType,
                                                crsLedgerBasicEntity.CrsCd.IntegerBu))

        ' コース台帳（料金_料金区分）.出発日 = パラメータ.出発日
        sqlString.AppendLine(" AND CLCC.SYUPT_DAY =  ")
        sqlString.AppendLine(MyBase.setParam(crsLedgerBasicEntity.SyuptDay.PhysicsName,
                                                paramInfoList.Item("SYUPT_DAY").ToString,
                                                crsLedgerBasicEntity.SyuptDay.DBType,
                                                crsLedgerBasicEntity.SyuptDay.IntegerBu))

        ' コース台帳（料金_料金区分）.号車 = パラメータ.号車
        sqlString.AppendLine(" AND CLCC.GOUSYA =  ")
        sqlString.AppendLine(MyBase.setParam(crsLedgerBasicEntity.Gousya.PhysicsName,
                                                paramInfoList.Item("GOUSYA").ToString,
                                                crsLedgerBasicEntity.Gousya.DBType,
                                                crsLedgerBasicEntity.Gousya.IntegerBu))

        ' コース台帳（料金_料金区分）.区分No  = '1'
        sqlString.AppendLine(" AND CLCC.KBN_NO = '1' ")

        sqlString.AppendLine(" ORDER BY  ")
        ' 料金区分（人員）マスタ.表示順
        sqlString.AppendLine(" CJK.DISPLAY_ORDER, ")
        ' 料金区分（人員）マスタ.料金区分（人員）コード
        sqlString.AppendLine(" CJK.CHARGE_KBN_JININ_CD ")

        Try
            returnValue = getDataTable(sqlString.ToString)
        Catch ex As Oracle.ManagedDataAccess.Client.OracleException
            Throw
        Catch ex As Exception
            Throw
        End Try

        Return returnValue

    End Function

    ''' <summary>
    ''' 消費税率を取得
    ''' </summary>
    ''' <param name="riyouDay">利用日</param>
    ''' <returns></returns>
    Public Function GetSyohizei(riyouDay As String) As DataTable

        ' 戻り値
        Dim returnValue As DataTable = Nothing
        Dim sqlString As New StringBuilder

        ' SELECT句
        sqlString.AppendLine(" SELECT TAX_PERCENT FROM M_SYOHIZEI  ")

        sqlString.AppendLine(" WHERE  ")
        sqlString.AppendLine(" '" & riyouDay & "' BETWEEN START_DATE AND END_DATE  ")
        sqlString.AppendLine(" AND TEIKI_KIKAKU_KUBUN = '0'  ")
        sqlString.AppendLine(" AND DELETE_DATE IS NULL  ")

        Try
            returnValue = getDataTable(sqlString.ToString)
        Catch ex As Oracle.ManagedDataAccess.Client.OracleException
            Throw
        Catch ex As Exception
            Throw
        End Try

        Return returnValue

    End Function

    ''' <summary>
    ''' 分類コード指定コードマスタデータ取得
    ''' </summary>
    ''' <param name="_codeType"></param>
    ''' <param name="nullRecord"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetCodeMasterData(ByVal codeBunrui As String) As DataTable

        Dim resultDataTable As New DataTable
        Dim strSQL As String = ""

        Try
            strSQL &= "SELECT RTRIM(CODE_VALUE) AS CODE_VALUE, CODE_NAME, NAIYO_2 FROM M_CODE"
            strSQL &= " WHERE CODE_BUNRUI = :BUNRUI"
            strSQL &= " AND DELETE_DATE IS NULL"
            strSQL &= " ORDER BY CODE_VALUE"

            MyBase.setParam("BUNRUI", codeBunrui)
            resultDataTable = MyBase.getDataTable(strSQL)
        Catch ex As Exception
            Throw ex
        End Try

        Return resultDataTable

    End Function

#End Region

#Region "削除"

    ''' <summary>
    ''' 原価確定現金内訳のDELETE(IO項目定義:NO12)
    ''' </summary>
    ''' <param name="paramInfoList"></param>
    ''' <param name="tran"></param>
    ''' <returns></returns>
    Public Function DeleteCostKakuteiGenkinUtiwake(Optional ByVal paramInfoList As Hashtable = Nothing,
                                                    Optional ByVal tran As OracleTransaction = Nothing) As Integer

        Dim costKakuteiGenkinUtiwakeEntity As New TCostKakuteiGenkinUtiwakeEntity

        Dim returnValue As Integer = 0

        Dim sqlString As New StringBuilder

        ' INSERT前にDELTE分を実行
        sqlString.AppendLine(" DELETE FROM T_COST_KAKUTEI_GENKIN_UTIWAKE ")
        sqlString.AppendLine(" WHERE")
        ' コースコード=画面間パラメータ.コースコード 
        sqlString.AppendLine(" CRS_CD =  ")
        sqlString.AppendLine(MyBase.setParam(costKakuteiGenkinUtiwakeEntity.CrsCd.PhysicsName,
                                            paramInfoList.Item("CRS_CD").ToString,
                                            costKakuteiGenkinUtiwakeEntity.CrsCd.DBType,
                                            costKakuteiGenkinUtiwakeEntity.CrsCd.IntegerBu))

        ' 出発日=画面間パラメータ.出発日
        sqlString.AppendLine(" AND SYUPT_DAY =  ")
        sqlString.AppendLine(MyBase.setParam(costKakuteiGenkinUtiwakeEntity.SyuptDay.PhysicsName,
                                            paramInfoList.Item("SYUPT_DAY").ToString,
                                            costKakuteiGenkinUtiwakeEntity.SyuptDay.DBType,
                                            costKakuteiGenkinUtiwakeEntity.SyuptDay.IntegerBu))

        ' 号車=画面間パラメータ.号車」
        sqlString.AppendLine(" AND GOUSYA =  ")
        sqlString.AppendLine(MyBase.setParam(costKakuteiGenkinUtiwakeEntity.Gousya.PhysicsName,
                                            paramInfoList.Item("GOUSYA").ToString,
                                            costKakuteiGenkinUtiwakeEntity.Gousya.DBType,
                                            costKakuteiGenkinUtiwakeEntity.Gousya.IntegerBu))

        Return execNonQuery(tran, sqlString.ToString)

    End Function

    ''' <summary>
    ''' 原価確定送客手数料 のDELETE(IO項目定義:NO13)
    ''' </summary>
    ''' <param name="paramInfoList"></param>
    ''' <param name="tran"></param>
    ''' <returns></returns>
    Public Function DeleteCostKakuteiSokyakFee(Optional ByVal paramInfoList As Hashtable = Nothing,
                                                    Optional ByVal tran As OracleTransaction = Nothing) As Integer

        Dim costKakuteiSokyakFeeEntity As New TCostKakuteiSokyakFeeEntity

        Dim returnValue As Integer = 0

        Dim sqlString As New StringBuilder

        ' INSERT前にDELTE分を実行
        sqlString.AppendLine(" DELETE FROM T_COST_KAKUTEI_SOKYAK_FEE ")
        sqlString.AppendLine(" WHERE")
        ' コースコード=画面間パラメータ.コースコード 
        sqlString.AppendLine(" CRS_CD =  ")
        sqlString.AppendLine(MyBase.setParam(costKakuteiSokyakFeeEntity.CrsCd.PhysicsName,
                                            paramInfoList.Item("CRS_CD").ToString,
                                            costKakuteiSokyakFeeEntity.CrsCd.DBType,
                                            costKakuteiSokyakFeeEntity.CrsCd.IntegerBu))

        ' 出発日=画面間パラメータ.出発日
        sqlString.AppendLine(" AND SYUPT_DAY =  ")
        sqlString.AppendLine(MyBase.setParam(costKakuteiSokyakFeeEntity.SyuptDay.PhysicsName,
                                            paramInfoList.Item("SYUPT_DAY").ToString,
                                            costKakuteiSokyakFeeEntity.SyuptDay.DBType,
                                            costKakuteiSokyakFeeEntity.SyuptDay.IntegerBu))

        ' 号車=画面間パラメータ.号車」
        sqlString.AppendLine(" AND GOUSYA =  ")
        sqlString.AppendLine(MyBase.setParam(costKakuteiSokyakFeeEntity.Gousya.PhysicsName,
                                            paramInfoList.Item("GOUSYA").ToString,
                                            costKakuteiSokyakFeeEntity.Gousya.DBType,
                                            costKakuteiSokyakFeeEntity.Gousya.IntegerBu))

        Return execNonQuery(tran, sqlString.ToString)

    End Function

    ''' <summary>
    ''' 原価確定徴証のDELETE(IO項目定義:NO14)
    ''' </summary>
    ''' <param name="paramInfoList"></param>
    ''' <param name="tran"></param>
    ''' <returns></returns>
    Public Function DeleteCostKakuteiChosho(Optional ByVal paramInfoList As Hashtable = Nothing,
                                                    Optional ByVal tran As OracleTransaction = Nothing) As Integer

        Dim costKakuteiChoshoEntity As New TCostKakuteiChoshoEntity
        Dim returnValue As Integer = 0

        Dim sqlString As New StringBuilder

        ' INSERT前にDELTE分を実行
        sqlString.AppendLine(" DELETE FROM T_COST_KAKUTEI_CHOSHO ")
        sqlString.AppendLine(" WHERE")
        ' コースコード=画面間パラメータ.コースコード 
        sqlString.AppendLine(" CRS_CD =  ")
        sqlString.AppendLine(MyBase.setParam(costKakuteiChoshoEntity.CrsCd.PhysicsName,
                                            paramInfoList.Item("CRS_CD").ToString,
                                            costKakuteiChoshoEntity.CrsCd.DBType,
                                            costKakuteiChoshoEntity.CrsCd.IntegerBu))

        ' 出発日=画面間パラメータ.出発日
        sqlString.AppendLine(" AND SYUPT_DAY =  ")
        sqlString.AppendLine(MyBase.setParam(costKakuteiChoshoEntity.SyuptDay.PhysicsName,
                                            paramInfoList.Item("SYUPT_DAY").ToString,
                                            costKakuteiChoshoEntity.SyuptDay.DBType,
                                            costKakuteiChoshoEntity.SyuptDay.IntegerBu))

        ' 号車=画面間パラメータ.号車」
        sqlString.AppendLine(" AND GOUSYA =  ")
        sqlString.AppendLine(MyBase.setParam(costKakuteiChoshoEntity.Gousya.PhysicsName,
                                            paramInfoList.Item("GOUSYA").ToString,
                                            costKakuteiChoshoEntity.Gousya.DBType,
                                            costKakuteiChoshoEntity.Gousya.IntegerBu))

        Return execNonQuery(tran, sqlString.ToString)

    End Function

    ''' <summary>
    ''' 原価確定徴証（料金情報）のDELETE(IO項目定義:NO14)
    ''' </summary>
    ''' <param name="paramInfoList"></param>
    ''' <param name="tran"></param>
    ''' <returns></returns>
    Public Function DeleteCostKakuteiChoshoChargeInfo(Optional ByVal paramInfoList As Hashtable = Nothing,
                                                    Optional ByVal tran As OracleTransaction = Nothing) As Integer

        Dim costKakuteiChoshoChargeInfoEntity As New TCostKakuteiChoshoChargeInfoEntity
        Dim returnValue As Integer = 0

        Dim sqlString As New StringBuilder

        ' INSERT前にDELTE分を実行
        sqlString.AppendLine(" DELETE FROM T_COST_KAKUTEI_CHOSHO_CHARGE_INFO ")
        sqlString.AppendLine(" WHERE")
        ' コースコード=画面間パラメータ.コースコード 
        sqlString.AppendLine(" CRS_CD =  ")
        sqlString.AppendLine(MyBase.setParam(costKakuteiChoshoChargeInfoEntity.CrsCd.PhysicsName,
                                            paramInfoList.Item("CRS_CD").ToString,
                                            costKakuteiChoshoChargeInfoEntity.CrsCd.DBType,
                                            costKakuteiChoshoChargeInfoEntity.CrsCd.IntegerBu))

        ' 出発日=画面間パラメータ.出発日
        sqlString.AppendLine(" AND SYUPT_DAY =  ")
        sqlString.AppendLine(MyBase.setParam(costKakuteiChoshoChargeInfoEntity.SyuptDay.PhysicsName,
                                            paramInfoList.Item("SYUPT_DAY").ToString,
                                            costKakuteiChoshoChargeInfoEntity.SyuptDay.DBType,
                                            costKakuteiChoshoChargeInfoEntity.SyuptDay.IntegerBu))

        ' 号車=画面間パラメータ.号車」
        sqlString.AppendLine(" AND GOUSYA =  ")
        sqlString.AppendLine(MyBase.setParam(costKakuteiChoshoChargeInfoEntity.Gousya.PhysicsName,
                                            paramInfoList.Item("GOUSYA").ToString,
                                            costKakuteiChoshoChargeInfoEntity.Gousya.DBType,
                                            costKakuteiChoshoChargeInfoEntity.Gousya.IntegerBu))

        Return execNonQuery(tran, sqlString.ToString)

    End Function

    ''' <summary>
    ''' 現払出金履歴のDELETE(IO項目定義:NO15)
    ''' </summary>
    ''' <param name="paramInfoList"></param>
    ''' <param name="tran"></param>
    ''' <returns></returns>
    Public Function DeleteGenbaraiWithdrawalHistory(Optional ByVal paramInfoList As Hashtable = Nothing,
                                                    Optional ByVal tran As OracleTransaction = Nothing) As Integer

        Dim genbaraiWithdrawalHistoryEntity As New TGenbaraiWithdrawalHistoryEntity
        Dim returnValue As Integer = 0

        Dim sqlString As New StringBuilder

        ' INSERT前にDELTE分を実行
        sqlString.AppendLine(" DELETE FROM T_GENBARAI_WITHDRAWAL_HISTORY ")
        sqlString.AppendLine(" WHERE")
        ' コースコード=画面間パラメータ.コースコード 
        sqlString.AppendLine(" CRS_CD =  ")
        sqlString.AppendLine(MyBase.setParam(genbaraiWithdrawalHistoryEntity.CrsCd.PhysicsName,
                                            paramInfoList.Item("CRS_CD").ToString,
                                            genbaraiWithdrawalHistoryEntity.CrsCd.DBType,
                                            genbaraiWithdrawalHistoryEntity.CrsCd.IntegerBu))

        ' 出発日=画面間パラメータ.出発日
        sqlString.AppendLine(" AND SYUPT_DAY =  ")
        sqlString.AppendLine(MyBase.setParam(genbaraiWithdrawalHistoryEntity.SyuptDay.PhysicsName,
                                            paramInfoList.Item("SYUPT_DAY").ToString,
                                            genbaraiWithdrawalHistoryEntity.SyuptDay.DBType,
                                            genbaraiWithdrawalHistoryEntity.SyuptDay.IntegerBu))

        ' 号車=画面間パラメータ.号車」
        sqlString.AppendLine(" AND GOUSYA =  ")
        sqlString.AppendLine(MyBase.setParam(genbaraiWithdrawalHistoryEntity.Gousya.PhysicsName,
                                            paramInfoList.Item("GOUSYA").ToString,
                                            genbaraiWithdrawalHistoryEntity.Gousya.DBType,
                                            genbaraiWithdrawalHistoryEntity.Gousya.IntegerBu))

        Return execNonQuery(tran, sqlString.ToString)

    End Function

#End Region

#Region "更新"

    ''' <summary>
    ''' 現払登録のUPDATE(IO項目定義:NO11_1
    ''' )
    ''' </summary>
    ''' <param name="paramInfoList"></param>
    ''' <param name="tran"></param>
    ''' <param name="kbn"></param>
    ''' <param name="btnKbn"></param>
    ''' <returns></returns>
    Public Function UpdGenbaraiEntry(Optional ByVal paramInfoList As Hashtable = Nothing,
                                                    Optional ByVal tran As OracleTransaction = Nothing,
                                                    Optional ByVal kbn As String = Nothing,
                                                    Optional ByVal btnKbn As String = Nothing) As Integer

        Dim genbaraiEntryEntity As New TGenbaraiEntryEntity
        Dim returnValue As Integer = 0

        Dim sqlString As New StringBuilder

        With genbaraiEntryEntity

            sqlString.AppendLine(" UPDATE T_GENBARAI_ENTRY SET ")

            ' 出金営業所コード
            sqlString.AppendLine(" WITHDRAWAL_EIGYOSYO_CD = '" & paramInfoList.Item(.WithdrawalEigyosyoCd.PhysicsName).ToString & "'")
            ' 帰着予定営業所コード
            sqlString.AppendLine(" ,RETURN_YOTEI_EIGYOSYO_CD = '" & paramInfoList.Item(.ReturnYoteiEigyosyoCd.PhysicsName).ToString & "'")
            ' 添乗員名
            sqlString.AppendLine(" ,TENJYOIN_NAME = '" & paramInfoList.Item(.TenjyoinName.PhysicsName).ToString & "'")

            ' 業務管理課の場合は「確認日」、「確定者コード」、「確定営業所コード」は更新しない
            If kbn.Equals("E") = False Then
                ' 「F10：確定」ボタン場合
                If btnKbn.Equals("K") Then
                    ' 確定日
                    sqlString.AppendLine(" ,KAKUTEI_DAY = " & paramInfoList.Item(.KakuteiDay.PhysicsName).ToString & "")
                    ' 確定者コード
                    sqlString.AppendLine(" ,KAKUTEI_PERSON_CD = '" & paramInfoList.Item(.KakuteiPersonCd.PhysicsName).ToString & "'")
                    ' 確定営業所コード
                    sqlString.AppendLine(" ,KAKUTEI_EIGYOSYO_CD = '" & paramInfoList.Item(.KakuteiEigyosyoCd.PhysicsName).ToString & "'")
                Else
                    ' 確定日
                    sqlString.AppendLine(" ,KAKUTEI_DAY = NULL ")
                    ' 確定者コード
                    sqlString.AppendLine(" ,KAKUTEI_PERSON_CD = NULL ")
                    ' 確定営業所コード
                    sqlString.AppendLine(" ,KAKUTEI_EIGYOSYO_CD = NULL ")
                End If
            Else
            End If

            ' 更新日
            sqlString.AppendLine(" ,UPDATE_DAY = TO_NUMBER(TO_CHAR(SYSDATE,'yyyyMMdd')) ")
            ' 更新者コード
            sqlString.AppendLine(" ,UPDATE_PERSON_CD = '" & UserInfoManagement.userId & "' ")
            ' 更新ＰＧＭＩＤ
            sqlString.AppendLine(" ,UPDATE_PGMID = 'S05_0302' ")
            ' 更新時刻
            sqlString.AppendLine(" ,UPDATE_TIME = TO_NUMBER(TO_CHAR(SYSDATE,'hh24mmss')) ")
            ' システム更新ＰＧＭＩＤ
            sqlString.AppendLine(" ,SYSTEM_UPDATE_PGMID = 'S05_0302' ")
            ' システム更新者コード
            sqlString.AppendLine(" ,SYSTEM_UPDATE_PERSON_CD = '" & UserInfoManagement.userId & "' ")
            ' システム更新日
            sqlString.AppendLine(" ,SYSTEM_UPDATE_DAY = SYSDATE ")

            sqlString.AppendLine(" WHERE ")

            ' コースコード=画面間パラメータ.コースコード 
            sqlString.AppendLine(" CRS_CD =  ")
            sqlString.AppendLine(MyBase.setParam(genbaraiEntryEntity.CrsCd.PhysicsName,
                                                paramInfoList.Item("CRS_CD").ToString,
                                                genbaraiEntryEntity.CrsCd.DBType,
                                                genbaraiEntryEntity.CrsCd.IntegerBu))

            ' 出発日=画面間パラメータ.出発日
            sqlString.AppendLine(" AND SYUPT_DAY =  ")
            sqlString.AppendLine(MyBase.setParam(genbaraiEntryEntity.SyuptDay.PhysicsName,
                                                paramInfoList.Item("SYUPT_DAY").ToString,
                                                genbaraiEntryEntity.SyuptDay.DBType,
                                                genbaraiEntryEntity.SyuptDay.IntegerBu))

            ' 号車=画面間パラメータ.号車」
            sqlString.AppendLine(" AND GOUSYA =  ")
            sqlString.AppendLine(MyBase.setParam(genbaraiEntryEntity.Gousya.PhysicsName,
                                                paramInfoList.Item("GOUSYA").ToString,
                                                genbaraiEntryEntity.Gousya.DBType,
                                                genbaraiEntryEntity.Gousya.IntegerBu))
        End With

        Return execNonQuery(tran, sqlString.ToString)

    End Function

#End Region

#Region "追加"

    ''' <summary>
    ''' 現払登録のINSERT(IO項目定義:NO11)
    ''' 
    ''' </summary>
    ''' <param name="paramInfoList"></param>
    ''' <param name="tran"></param>
    ''' <param name="kbn"></param>
    ''' <param name="btnKbn"></param>
    ''' <returns></returns>
    Public Function InsertGenbaraiEntry(Optional ByVal paramInfoList As Hashtable = Nothing,
                                                    Optional ByVal tran As OracleTransaction = Nothing,
                                                    Optional ByVal kbn As String = Nothing,
                                                    Optional ByVal btnKbn As String = Nothing) As Integer

        Dim genbaraiEntryEntity As New TGenbaraiEntryEntity
        Dim returnValue As Integer = 0

        Dim sqlString As New StringBuilder

        With genbaraiEntryEntity

            ' INSERT
            sqlString.AppendLine(" INSERT INTO ")
            sqlString.AppendLine(" T_GENBARAI_ENTRY ")
            sqlString.AppendLine(" ( ")

            ' コースコード
            sqlString.AppendLine(" CRS_CD, ")
            ' 出発日
            sqlString.AppendLine(" SYUPT_DAY, ")
            ' 号車
            sqlString.AppendLine(" GOUSYA, ")
            ' 出金営業所コード
            sqlString.AppendLine(" WITHDRAWAL_EIGYOSYO_CD, ")
            ' 帰着予定営業所コード
            sqlString.AppendLine(" RETURN_YOTEI_EIGYOSYO_CD, ")
            ' 添乗員名
            sqlString.AppendLine(" TENJYOIN_NAME, ")
            ' 確定日
            sqlString.AppendLine(" KAKUTEI_DAY, ")
            ' 確定者コード
            sqlString.AppendLine(" KAKUTEI_PERSON_CD, ")
            ' 確定営業所コード
            sqlString.AppendLine(" KAKUTEI_EIGYOSYO_CD, ")
            ' 確認日１
            sqlString.AppendLine(" KAKUNIN_DATE_1, ")
            ' 確認者１コード
            sqlString.AppendLine(" KAKUNIN_PERSON_CD_1, ")
            ' 確認日２
            sqlString.AppendLine(" KAKUNIN_DATE_2, ")
            ' 確認者２コード
            sqlString.AppendLine(" KAKUNIN_PERSON_CD_2, ")
            ' 削除日
            sqlString.AppendLine(" DELETE_DAY, ")
            ' 登録日
            sqlString.AppendLine(" ENTRY_DAY, ")
            ' 登録者コード
            sqlString.AppendLine(" ENTRY_PERSON_CD, ")
            ' 登録ＰＧＭＩＤ
            sqlString.AppendLine(" ENTRY_PGMID, ")
            ' 登録時刻
            sqlString.AppendLine(" ENTRY_TIME, ")
            ' 更新日
            sqlString.AppendLine(" UPDATE_DAY, ")
            ' 更新者コード
            sqlString.AppendLine(" UPDATE_PERSON_CD, ")
            ' 更新ＰＧＭＩＤ
            sqlString.AppendLine(" UPDATE_PGMID, ")
            ' 更新時刻
            sqlString.AppendLine(" UPDATE_TIME, ")
            ' システム登録ＰＧＭＩＤ
            sqlString.AppendLine(" SYSTEM_ENTRY_PGMID, ")
            ' システム登録者コード
            sqlString.AppendLine(" SYSTEM_ENTRY_PERSON_CD, ")
            ' システム登録日
            sqlString.AppendLine(" SYSTEM_ENTRY_DAY, ")
            ' システム更新ＰＧＭＩＤ
            sqlString.AppendLine(" SYSTEM_UPDATE_PGMID, ")
            ' システム更新者コード
            sqlString.AppendLine(" SYSTEM_UPDATE_PERSON_CD, ")
            ' システム更新日
            sqlString.AppendLine(" SYSTEM_UPDATE_DAY ")

            sqlString.AppendLine(" ) ")
            ' VALUE
            sqlString.AppendLine(" VALUES ")
            sqlString.AppendLine(" ( ")
            sqlString.AppendLine("    '" & paramInfoList.Item(.CrsCd.PhysicsName).ToString & "'")
            sqlString.AppendLine("    , " & paramInfoList.Item(.SyuptDay.PhysicsName).ToString & "")
            sqlString.AppendLine("    ," & paramInfoList.Item(.Gousya.PhysicsName).ToString & "")
            sqlString.AppendLine("    ,'" & paramInfoList.Item(.WithdrawalEigyosyoCd.PhysicsName).ToString & "'")
            sqlString.AppendLine("    ,'" & paramInfoList.Item(.ReturnYoteiEigyosyoCd.PhysicsName).ToString & "'")
            sqlString.AppendLine("    ,'" & paramInfoList.Item(.TenjyoinName.PhysicsName).ToString & "'")

            ' 業務管理課の場合は「確認日」、「確定者コード」、「確定営業所コード」はNULLを設定　
            If kbn.Equals("E") = False Then

                ' 「F10：確定」ボタン場合
                If btnKbn.Equals("K") Then
                    ' 確定日
                    sqlString.AppendLine(" ," & paramInfoList.Item(.KakuteiDay.PhysicsName).ToString & "")
                    ' 確定者コード
                    sqlString.AppendLine(" ,'" & paramInfoList.Item(.KakuteiPersonCd.PhysicsName).ToString & "'")
                    ' 確定営業所コード
                    sqlString.AppendLine(" ,'" & paramInfoList.Item(.KakuteiEigyosyoCd.PhysicsName).ToString & "'")
                Else
                    ' 確定日
                    sqlString.AppendLine(" ,NULL ")
                    ' 確定者コード
                    sqlString.AppendLine(" ,NULL ")
                    ' 確定営業所コード
                    sqlString.AppendLine(" ,NULL ")
                End If
            Else
                ' 確定日
                sqlString.AppendLine(" ,NULL ")
                ' 確定者コード
                sqlString.AppendLine(" ,NULL ")
                ' 確定営業所コード
                sqlString.AppendLine(" ,NULL ")
            End If

            sqlString.AppendLine("    ,NULL")
            sqlString.AppendLine("    ,NULL")
            sqlString.AppendLine("    ,NULL")
            sqlString.AppendLine("    ,NULL")
            sqlString.AppendLine("    ,NULL")
            sqlString.AppendLine("    ,TO_NUMBER(TO_CHAR(SYSDATE,'yyyyMMdd'))")
            sqlString.AppendLine("    ,'" & UserInfoManagement.userId & "'")
            sqlString.AppendLine("    ,'S05_0302'")
            sqlString.AppendLine("    ,TO_NUMBER(TO_CHAR(SYSDATE,'hh24mmss'))")
            sqlString.AppendLine("    ,TO_NUMBER(TO_CHAR(SYSDATE,'yyyyMMdd'))")
            sqlString.AppendLine("    ,'" & UserInfoManagement.userId & "'")
            sqlString.AppendLine("    ,'S05_0302'")
            sqlString.AppendLine("    ,TO_NUMBER(TO_CHAR(SYSDATE,'hh24mmss'))")
            sqlString.AppendLine("    ,'S05_0302'")
            sqlString.AppendLine("    ,'" & UserInfoManagement.userId & "'")
            sqlString.AppendLine("    ,SYSDATE")
            sqlString.AppendLine("    ,'S05_0302'")
            sqlString.AppendLine("    ,'" & UserInfoManagement.userId & "'")
            sqlString.AppendLine("    ,SYSDATE")

            sqlString.AppendLine(" ) ")

        End With

        Return execNonQuery(tran, sqlString.ToString)

    End Function

    ''' <summary>
    ''' 原価確定現金内訳のINSERT(IO項目定義:NO12)
    ''' </summary>
    ''' <param name="paramInfoList"></param>
    ''' <param name="tran"></param>
    ''' <param name="kbn">1:原価確定現金内訳 2:添乗準備金分 3:送客手数料(現金分)</param>
    ''' <returns></returns>
    Public Function InsertCostKakuteiGenkinUtiwake(Optional ByVal paramInfoList As Hashtable = Nothing,
                                                    Optional ByVal tran As OracleTransaction = Nothing,
                                                    Optional ByVal kbn As Integer = Nothing) As Integer

        Dim costKakuteiGenkinUtiwakeEntity As New TCostKakuteiGenkinUtiwakeEntity

        Dim returnValue As Integer = 0

        Dim sqlString As New StringBuilder

        With costKakuteiGenkinUtiwakeEntity

            ' INSERT
            sqlString.AppendLine(" INSERT INTO ")
            sqlString.AppendLine(" T_COST_KAKUTEI_GENKIN_UTIWAKE ")
            sqlString.AppendLine(" ( ")

            ' コースコード
            sqlString.AppendLine(" CRS_CD, ")
            ' 出発日
            sqlString.AppendLine(" SYUPT_DAY, ")
            ' 号車
            sqlString.AppendLine(" GOUSYA, ")
            ' ＳＥＱ
            sqlString.AppendLine(" SEQ, ")
            ' 原価支払内訳コード
            sqlString.AppendLine(" COST_SIHARAI_UTIWAKE_CD, ")
            ' 原価支払内訳名
            sqlString.AppendLine(" COST_SIHARAI_UTIWAKE_NAME, ")
            ' メモ
            sqlString.AppendLine(" MEMO, ")
            ' 新規区分
            sqlString.AppendLine(" NEW_KBN, ")
            ' 利用数
            sqlString.AppendLine(" RIYOU_NUM, ")
            ' 季コード
            sqlString.AppendLine(" SEASON_CD, ")
            ' 支出金額
            sqlString.AppendLine(" SHISHUTSU_KINGAKU, ")
            ' 収入金額
            sqlString.AppendLine(" SHUNYU_KINGAKU, ")
            ' 支払方法
            sqlString.AppendLine(" SIHARAI_HOHO, ")
            ' 仕入先コード
            sqlString.AppendLine(" SIIRE_SAKI_CD, ")
            ' 仕入先枝番
            sqlString.AppendLine(" SIIRE_SAKI_EDABAN, ")
            ' 仕訳区分
            sqlString.AppendLine(" SIWAKE_KBN, ")
            ' 単価区分
            sqlString.AppendLine(" TANKA_KBN, ")
            ' 税区分
            sqlString.AppendLine(" TAX_KBN, ")
            ' 年
            sqlString.AppendLine(" YEAR, ")
            ' 年月
            sqlString.AppendLine(" YM, ")
            ' 利用日
            sqlString.AppendLine(" RIYOU_DAY, ")
            ' 精算目的コード
            sqlString.AppendLine(" SEISAN_MOKUTEKI_CD, ")
            ' 入湯税
            sqlString.AppendLine(" BATH_TAX, ")
            ' 率
            sqlString.AppendLine(" PER, ")
            ' 削除日
            sqlString.AppendLine(" DELETE_DAY, ")
            ' 登録日
            sqlString.AppendLine(" ENTRY_DAY, ")
            ' 登録者コード
            sqlString.AppendLine(" ENTRY_PERSON_CD, ")
            ' 登録ＰＧＭＩＤ
            sqlString.AppendLine(" ENTRY_PGMID, ")
            ' 登録時刻
            sqlString.AppendLine(" ENTRY_TIME, ")
            ' 更新日
            sqlString.AppendLine(" UPDATE_DAY, ")
            ' 更新者コード
            sqlString.AppendLine(" UPDATE_PERSON_CD, ")
            ' 更新ＰＧＭＩＤ
            sqlString.AppendLine(" UPDATE_PGMID, ")
            ' 更新時刻
            sqlString.AppendLine(" UPDATE_TIME, ")
            ' システム登録ＰＧＭＩＤ
            sqlString.AppendLine(" SYSTEM_ENTRY_PGMID, ")
            ' システム登録者コード
            sqlString.AppendLine(" SYSTEM_ENTRY_PERSON_CD, ")
            ' システム登録日
            sqlString.AppendLine(" SYSTEM_ENTRY_DAY, ")
            ' システム更新ＰＧＭＩＤ
            sqlString.AppendLine(" SYSTEM_UPDATE_PGMID, ")
            ' システム更新者コード
            sqlString.AppendLine(" SYSTEM_UPDATE_PERSON_CD, ")
            ' システム更新日
            sqlString.AppendLine(" SYSTEM_UPDATE_DAY, ")
            ' 行番号
            sqlString.AppendLine(" LINE_NO, ")
            ' グループ番号
            sqlString.AppendLine(" GROUP_NO ")

            sqlString.AppendLine(" ) ")
            ' VALUE
            sqlString.AppendLine(" VALUES ")
            sqlString.AppendLine(" ( ")
            sqlString.AppendLine("    '" & paramInfoList.Item(.CrsCd.PhysicsName).ToString & "'")
            sqlString.AppendLine("    , " & paramInfoList.Item(.SyuptDay.PhysicsName).ToString & "")
            sqlString.AppendLine("    ," & paramInfoList.Item(.Gousya.PhysicsName).ToString & "")

            If kbn = 1 OrElse kbn = 3 Then
                sqlString.AppendLine("    , " & paramInfoList.Item(.Seq.PhysicsName).ToString & "")
            Else
                sqlString.AppendLine("    , 0")
            End If

            If kbn = 1 Then
                sqlString.AppendLine("    ,'" & paramInfoList.Item(.CostSiharaiUtiwakeCd.PhysicsName).ToString & "'")
                sqlString.AppendLine("    ,'" & paramInfoList.Item(.CostSiharaiUtiwakeName.PhysicsName).ToString & "'")
                sqlString.AppendLine("    ,'" & paramInfoList.Item(.Memo.PhysicsName).ToString & "'")
            Else
                sqlString.AppendLine("    ,NULL")
                sqlString.AppendLine("    ,NULL")
                sqlString.AppendLine("    ,NULL")
            End If

            If kbn = 1 Then
                sqlString.AppendLine("    ,'" & paramInfoList.Item(.NewKbn.PhysicsName).ToString & "'")
            Else
                ' 半角スペース
                sqlString.AppendLine("    ,' '")
            End If

            If kbn = 1 Then
                sqlString.AppendLine("    ," & paramInfoList.Item(.RiyouNum.PhysicsName).ToString & "")
            Else
                sqlString.AppendLine("    ,NULL")
            End If

            sqlString.AppendLine("    ,'" & paramInfoList.Item(.SeasonCd.PhysicsName).ToString & "'")

            If kbn = 1 Then
                sqlString.AppendLine("    ," & paramInfoList.Item(.ShishutsuKingaku.PhysicsName).ToString & "")
            Else
                sqlString.AppendLine("    ,NULL")
            End If

            sqlString.AppendLine("    ," & paramInfoList.Item(.ShunyuKingaku.PhysicsName).ToString & "")

            If kbn = 1 Then
                sqlString.AppendLine("    ,'" & paramInfoList.Item(.SiharaiHoho.PhysicsName).ToString & "'")
            Else
                sqlString.AppendLine("    ,NULL")
            End If

            If kbn = 1 OrElse kbn = 3 Then
                sqlString.AppendLine("    ,'" & paramInfoList.Item(.SiireSakiCd.PhysicsName).ToString & "'")
                sqlString.AppendLine("    ,'" & paramInfoList.Item(.SiireSakiEdaban.PhysicsName).ToString & "'")
            Else
                sqlString.AppendLine("    ,NULL")
                sqlString.AppendLine("    ,NULL")
            End If

            If kbn = 1 Then
                sqlString.AppendLine("    ,'" & paramInfoList.Item(.SiwakeKbn.PhysicsName).ToString & "'")
            ElseIf kbn = 2 Then
                ' 11:準備金
                sqlString.AppendLine("    ,'11'")
            ElseIf kbn = 3 Then
                ' 26:客手数料
                sqlString.AppendLine("    ,'26'")
            End If

            If kbn = 1 OrElse kbn = 3 Then
                sqlString.AppendLine("    ,'" & paramInfoList.Item(.TankaKbn.PhysicsName).ToString & "'")
                sqlString.AppendLine("    ,'" & paramInfoList.Item(.TaxKbn.PhysicsName).ToString & "'")
            Else
                sqlString.AppendLine("    ,NULL")
                sqlString.AppendLine("    ,NULL")
            End If

            sqlString.AppendLine("    ," & paramInfoList.Item(.Year.PhysicsName).ToString & "")
            sqlString.AppendLine("    ," & paramInfoList.Item(.Ym.PhysicsName).ToString & "")
            sqlString.AppendLine("    ," & paramInfoList.Item(.RiyouDay.PhysicsName).ToString & "")

            If kbn = 1 Then
                sqlString.AppendLine("    ,'" & paramInfoList.Item(.SeisanMokutekiCd.PhysicsName).ToString & "'")
                sqlString.AppendLine("    ," & paramInfoList.Item(.BathTax.PhysicsName).ToString & "")
                sqlString.AppendLine("    ," & paramInfoList.Item(.Per.PhysicsName).ToString & "")
            Else
                sqlString.AppendLine("    ,NULL")
                sqlString.AppendLine("    ,NULL")
                sqlString.AppendLine("    ,NULL")
            End If

            sqlString.AppendLine("    ,NULL")
            sqlString.AppendLine("    ,TO_NUMBER(TO_CHAR(SYSDATE,'yyyyMMdd'))")
            sqlString.AppendLine("    ,'" & UserInfoManagement.userId & "'")
            sqlString.AppendLine("    ,'S05_0302'")
            sqlString.AppendLine("    ,TO_NUMBER(TO_CHAR(SYSDATE,'hh24mmss'))")
            sqlString.AppendLine("    ,TO_NUMBER(TO_CHAR(SYSDATE,'yyyyMMdd'))")
            sqlString.AppendLine("    ,'" & UserInfoManagement.userId & "'")
            sqlString.AppendLine("    ,'S05_0302'")
            sqlString.AppendLine("    ,TO_NUMBER(TO_CHAR(SYSDATE,'hh24mmss'))")
            sqlString.AppendLine("    ,'S05_0302'")
            sqlString.AppendLine("    ,'" & UserInfoManagement.userId & "'")
            sqlString.AppendLine("    ,SYSDATE")
            sqlString.AppendLine("    ,'S05_0302'")
            sqlString.AppendLine("    ,'" & UserInfoManagement.userId & "'")
            sqlString.AppendLine("    ,SYSDATE")

            If kbn = 1 Then
                sqlString.AppendLine("    ," & paramInfoList.Item(.LineNo.PhysicsName).ToString & "")
                sqlString.AppendLine("    ," & paramInfoList.Item(.GroupNo.PhysicsName).ToString & "")
            Else
                sqlString.AppendLine("    ,NULL")
                sqlString.AppendLine("    ,NULL")
            End If

            sqlString.AppendLine(" ) ")
        End With

        Return execNonQuery(tran, sqlString.ToString)

    End Function

    ''' <summary>
    ''' 原価確定送客手数料のInsert(IO項目定義:NO13)
    ''' </summary>
    ''' <param name="paramInfoList"></param>
    ''' <param name="tran"></param>
    ''' <returns></returns>
    Public Function InsertCostKakuteiSokyakFee(Optional ByVal paramInfoList As Hashtable = Nothing,
                                                    Optional ByVal tran As OracleTransaction = Nothing) As Integer

        Dim costKakuteiSokyakFeeEntity As New TCostKakuteiSokyakFeeEntity
        Dim returnValue As Integer = 0

        Dim sqlString As New StringBuilder

        With costKakuteiSokyakFeeEntity

            ' INSERT
            sqlString.AppendLine(" INSERT INTO ")
            sqlString.AppendLine(" T_COST_KAKUTEI_SOKYAK_FEE ")
            sqlString.AppendLine(" ( ")

            ' コースコード
            sqlString.AppendLine(" CRS_CD, ")
            ' 台数
            sqlString.AppendLine(" DAISU, ")
            ' 削除日
            sqlString.AppendLine(" DELETE_DAY, ")
            ' 伝票ＮＯ
            sqlString.AppendLine(" DENPYOU_NO, ")
            ' 登録日
            sqlString.AppendLine(" ENTRY_DAY, ")
            ' 登録者コード
            sqlString.AppendLine(" ENTRY_PERSON_CD, ")
            ' 登録ＰＧＭＩＤ
            sqlString.AppendLine(" ENTRY_PGMID, ")
            ' 登録時刻
            sqlString.AppendLine(" ENTRY_TIME, ")
            ' 号車
            sqlString.AppendLine(" GOUSYA, ")
            ' 被請求先コード
            sqlString.AppendLine(" HI_SEIKYU_SAKI_CD, ")
            ' 被請求先枝番
            sqlString.AppendLine(" HI_SEIKYU_SAKI_EDABAN, ")
            ' 被請求先精算目的
            sqlString.AppendLine(" HI_SEIKYU_SAKI_SEISAN_MOKUTEKI, ")
            ' 確認額
            sqlString.AppendLine(" KAKUNIN_GAKU, ")
            ' ＳＥＱ
            sqlString.AppendLine(" SEQ, ")
            ' 新規区分
            sqlString.AppendLine(" NEW_KBN, ")
            ' 人数
            sqlString.AppendLine(" NINZU, ")
            ' 率
            sqlString.AppendLine(" PER, ")
            ' 利用日
            sqlString.AppendLine(" RIYOU_DAY, ")
            ' 季コード
            sqlString.AppendLine(" SEASON_CD, ")
            ' 仕入先コード
            sqlString.AppendLine(" SIIRE_SAKI_CD, ")
            ' 仕入先枝番
            sqlString.AppendLine(" SIIRE_SAKI_EDABAN, ")
            ' 仕入先種別コード
            sqlString.AppendLine(" SIIRE_SAKI_KIND_CD, ")
            ' 送客手数料計算方法区分
            sqlString.AppendLine(" SOKYAK_FEE_CALC_HOHO_KBN, ")
            ' 送客手数料現金払い区分
            sqlString.AppendLine(" SOKYAK_FEE_GENKIN_PAYMENT_KBN, ")
            ' 出発日
            sqlString.AppendLine(" SYUPT_DAY, ")
            ' 単価
            sqlString.AppendLine(" TANKA, ")
            ' 更新日
            sqlString.AppendLine(" UPDATE_DAY, ")
            ' 更新者コード
            sqlString.AppendLine(" UPDATE_PERSON_CD, ")
            ' 更新ＰＧＭＩＤ
            sqlString.AppendLine(" UPDATE_PGMID, ")
            ' 更新時刻
            sqlString.AppendLine(" UPDATE_TIME, ")
            ' 売上
            sqlString.AppendLine(" URIAGE, ")
            ' 年
            sqlString.AppendLine(" YEAR, ")
            ' 年月
            sqlString.AppendLine(" YM, ")
            ' システム登録ＰＧＭＩＤ
            sqlString.AppendLine(" SYSTEM_ENTRY_PGMID, ")
            ' システム登録者コード
            sqlString.AppendLine(" SYSTEM_ENTRY_PERSON_CD, ")
            ' システム登録日
            sqlString.AppendLine(" SYSTEM_ENTRY_DAY, ")
            ' システム更新ＰＧＭＩＤ
            sqlString.AppendLine(" SYSTEM_UPDATE_PGMID, ")
            ' システム更新者コード
            sqlString.AppendLine(" SYSTEM_UPDATE_PERSON_CD, ")
            ' システム更新日
            sqlString.AppendLine(" SYSTEM_UPDATE_DAY ")

            sqlString.AppendLine(" ) ")
            ' VALUE
            sqlString.AppendLine(" VALUES ")
            sqlString.AppendLine(" ( ")
            sqlString.AppendLine("    '" & paramInfoList.Item(.CrsCd.PhysicsName).ToString & "'")
            ' 画面一覧(送客手数料).計算区分 = 'A'の場合、画面一覧(送客手数料).数量　それ以外null
            If Not String.IsNullOrEmpty(paramInfoList.Item(.Daisu.PhysicsName).ToString) Then
                sqlString.AppendLine("    ," & paramInfoList.Item(.Daisu.PhysicsName).ToString & "")
            Else
                sqlString.AppendLine("    ,NULL")
            End If
            sqlString.AppendLine("    ,NULL")
            sqlString.AppendLine("    ," & paramInfoList.Item(.DenpyouNo.PhysicsName).ToString & "")
            sqlString.AppendLine("    ,TO_NUMBER(TO_CHAR(SYSDATE,'yyyyMMdd'))")
            sqlString.AppendLine("    ,'" & UserInfoManagement.userId & "'")
            sqlString.AppendLine("    ,'S05_0302'")
            sqlString.AppendLine("    ,TO_NUMBER(TO_CHAR(SYSDATE,'hh24mmss'))")
            sqlString.AppendLine("    ," & paramInfoList.Item(.Gousya.PhysicsName).ToString & "")
            sqlString.AppendLine("    ,'" & paramInfoList.Item(.HiSeikyuSakiCd.PhysicsName).ToString & "'")
            sqlString.AppendLine("    ,'" & paramInfoList.Item(.HiSeikyuSakiEdaban.PhysicsName).ToString & "'")
            sqlString.AppendLine("    ,'" & paramInfoList.Item(.HiSeikyuSakiSeisanMokuteki.PhysicsName).ToString & "'")
            sqlString.AppendLine("    ," & paramInfoList.Item(.KakuninGaku.PhysicsName).ToString & "")
            sqlString.AppendLine("    ," & paramInfoList.Item(.Seq.PhysicsName).ToString & "")
            sqlString.AppendLine("    ,'" & paramInfoList.Item(.NewKbn.PhysicsName).ToString & "'")
            ' 画面一覧(送客手数料).計算区分 = 'B'の場合、画面一覧(送客手数料).数量　それ以外null
            If Not String.IsNullOrEmpty(paramInfoList.Item(.Ninzu.PhysicsName).ToString) Then
                sqlString.AppendLine("    ," & paramInfoList.Item(.Ninzu.PhysicsName).ToString & "")
            Else
                sqlString.AppendLine("    ,NULL")
            End If
            sqlString.AppendLine("    ," & paramInfoList.Item(.Per.PhysicsName).ToString & "")
            sqlString.AppendLine("    ," & paramInfoList.Item(.RiyouDay.PhysicsName).ToString & "")
            sqlString.AppendLine("    ,'" & paramInfoList.Item(.SeasonCd.PhysicsName).ToString & "'")
            sqlString.AppendLine("    ,'" & paramInfoList.Item(.SiireSakiCd.PhysicsName).ToString & "'")
            sqlString.AppendLine("    ,'" & paramInfoList.Item(.SiireSakiEdaban.PhysicsName).ToString & "'")
            sqlString.AppendLine("    ,'" & paramInfoList.Item(.SiireSakiKindCd.PhysicsName).ToString & "'")
            sqlString.AppendLine("    ,'" & paramInfoList.Item(.SokyakFeeCalcHohoKbn.PhysicsName).ToString & "'")
            sqlString.AppendLine("    ,'" & paramInfoList.Item(.SokyakFeeGenkinPaymentKbn.PhysicsName).ToString & "'")
            sqlString.AppendLine("    , " & paramInfoList.Item(.SyuptDay.PhysicsName).ToString & "")
            ' 画面一覧(送客手数料).計算区分 = 'A'又は'B'の場合、画面一覧(送客手数料).単価/売上　それ以外null
            If Not String.IsNullOrEmpty(paramInfoList.Item(.Tanka.PhysicsName).ToString) Then
                sqlString.AppendLine("    ," & paramInfoList.Item(.Tanka.PhysicsName).ToString & "")
            Else
                sqlString.AppendLine("    ,NULL")
            End If
            sqlString.AppendLine("    ,TO_NUMBER(TO_CHAR(SYSDATE,'yyyyMMdd'))")
            sqlString.AppendLine("    ,'" & UserInfoManagement.userId & "'")
            sqlString.AppendLine("    ,'S05_0302'")
            sqlString.AppendLine("    ,TO_NUMBER(TO_CHAR(SYSDATE,'hh24mmss'))")
            ' 画面一覧(送客手数料).計算区分 = 'C'の場合、画面一覧(送客手数料).単価/売上　それ以外null
            If Not String.IsNullOrEmpty(paramInfoList.Item(.Uriage.PhysicsName).ToString) Then
                sqlString.AppendLine("    ," & paramInfoList.Item(.Uriage.PhysicsName).ToString & "")
            Else
                sqlString.AppendLine("    ,NULL")
            End If
            sqlString.AppendLine("    ," & paramInfoList.Item(.Year.PhysicsName).ToString & "")
            sqlString.AppendLine("    ," & paramInfoList.Item(.Ym.PhysicsName).ToString & "")
            sqlString.AppendLine("    ,'S05_0302'")
            sqlString.AppendLine("    ,'" & UserInfoManagement.userId & "'")
            sqlString.AppendLine("    ,SYSDATE")
            sqlString.AppendLine("    ,'S05_0302'")
            sqlString.AppendLine("    ,'" & UserInfoManagement.userId & "'")
            sqlString.AppendLine("    ,SYSDATE")
            sqlString.AppendLine("    ," & paramInfoList.Item(.LineNo.PhysicsName).ToString & "")

            sqlString.AppendLine(" ) ")

        End With

        Return execNonQuery(tran, sqlString.ToString)

    End Function

    ''' <summary>
    ''' 原価確定徴証のInsert(IO項目定義:NO14)
    ''' </summary>
    ''' <param name="paramInfoList"></param>
    ''' <param name="tran"></param>
    ''' <returns></returns>
    Public Function InsertCostKakuteiChosho(Optional ByVal paramInfoList As Hashtable = Nothing,
                                                    Optional ByVal tran As OracleTransaction = Nothing) As Integer

        Dim costKakuteiChoshoEntity As New TCostKakuteiChoshoEntity
        Dim returnValue As Integer = 0

        Dim sqlString As New StringBuilder

        With costKakuteiChoshoEntity
            ' INSERT
            sqlString.AppendLine(" INSERT INTO ")
            ' 原価確定徴証
            sqlString.AppendLine(" T_COST_KAKUTEI_CHOSHO ")
            sqlString.AppendLine(" ( ")
            ' コースコード
            sqlString.AppendLine(" CRS_CD, ")
            ' 出発日
            sqlString.AppendLine(" SYUPT_DAY, ")
            ' 号車
            sqlString.AppendLine(" GOUSYA, ")
            ' 徴証区分
            sqlString.AppendLine(" CHOSHO_KBN, ")
            ' SEQ
            sqlString.AppendLine(" SEQ, ")
            ' 入湯税
            sqlString.AppendLine(" BATH_TAX, ")
            ' ＣＯＭ
            sqlString.AppendLine(" COM, ")
            ' ＣＯＭ額
            sqlString.AppendLine(" COM_GAKU, ")
            ' 被請求先コード
            sqlString.AppendLine(" HI_SEIKYU_SAKI_CD, ")
            ' 被請求先枝番
            sqlString.AppendLine(" HI_SEIKYU_SAKI_EDABAN, ")
            ' 被請求先精算目的
            sqlString.AppendLine(" HI_SEIKYU_SAKI_SEISAN_MOKUTEKI, ")
            ' 確認額
            sqlString.AppendLine(" KAKUNIN_GAKU, ")
            ' メモ
            sqlString.AppendLine(" MEMO, ")
            ' ＮＥＴ
            sqlString.AppendLine(" NET, ")
            ' 新規区分
            sqlString.AppendLine(" NEW_KBN, ")
            ' 利用日付
            sqlString.AppendLine(" RIYOU_YMD, ")
            ' 季コード
            sqlString.AppendLine(" SEASON_CD, ")
            ' 精算方法
            sqlString.AppendLine(" SEISAN_HOHO, ")
            ' 精算目的コード
            sqlString.AppendLine(" SEISAN_MOKUTEKI_CD, ")
            ' 仕入先コード
            sqlString.AppendLine(" SIIRE_SAKI_CD, ")
            ' 仕入先枝番
            sqlString.AppendLine(" SIIRE_SAKI_EDABAN, ")
            ' 仕入先識別コード
            sqlString.AppendLine(" SIIRE_SAKI_SHIKIBETSU_CD, ")
            ' 消費税
            sqlString.AppendLine(" SYOHI_TAX, ")
            ' 税区分
            sqlString.AppendLine(" TAX_KBN, ")
            ' 年
            sqlString.AppendLine(" YEAR, ")
            ' 年月
            sqlString.AppendLine(" YM, ")
            ' 人数
            sqlString.AppendLine(" NINZU, ")
            ' 削除日
            sqlString.AppendLine(" DELETE_DAY, ")
            ' 登録日
            sqlString.AppendLine(" ENTRY_DAY, ")
            ' 登録者コード
            sqlString.AppendLine(" ENTRY_PERSON_CD, ")
            ' 登録ＰＧＭＩＤ
            sqlString.AppendLine(" ENTRY_PGMID, ")
            ' 登録時刻
            sqlString.AppendLine(" ENTRY_TIME, ")
            ' 更新日
            sqlString.AppendLine(" UPDATE_DAY, ")
            ' 更新者コード
            sqlString.AppendLine(" UPDATE_PERSON_CD, ")
            ' 更新ＰＧＭＩＤ
            sqlString.AppendLine(" UPDATE_PGMID, ")
            ' 更新時刻
            sqlString.AppendLine(" UPDATE_TIME, ")
            ' システム登録ＰＧＭＩＤ
            sqlString.AppendLine(" SYSTEM_ENTRY_PGMID, ")
            ' システム登録者コード
            sqlString.AppendLine(" SYSTEM_ENTRY_PERSON_CD, ")
            ' システム登録日
            sqlString.AppendLine(" SYSTEM_ENTRY_DAY, ")
            ' システム更新ＰＧＭＩＤ
            sqlString.AppendLine(" SYSTEM_UPDATE_PGMID, ")
            ' システム更新者コード
            sqlString.AppendLine(" SYSTEM_UPDATE_PERSON_CD, ")
            ' システム更新日
            sqlString.AppendLine(" SYSTEM_UPDATE_DAY, ")
            ' 行番号
            sqlString.AppendLine(" LINE_NO, ")
            ' グループ番号
            sqlString.AppendLine(" GROUP_NO, ")
            ' 降車ヶ所運休区分
            sqlString.AppendLine(" KOSHAKASHO_UNKYUU_KBN ")
            sqlString.AppendLine(" ) ")
            ' VALUE
            sqlString.AppendLine(" VALUES ")
            sqlString.AppendLine(" ( ")

            sqlString.AppendLine("    '" & paramInfoList.Item(.CrsCd.PhysicsName).ToString & "'")
            sqlString.AppendLine("    , " & paramInfoList.Item(.SyuptDay.PhysicsName).ToString & "")
            sqlString.AppendLine("    ," & paramInfoList.Item(.Gousya.PhysicsName).ToString & "")
            sqlString.AppendLine("    ,'" & paramInfoList.Item(.ChoshoKbn.PhysicsName).ToString & "'")
            sqlString.AppendLine("    ," & paramInfoList.Item(.Seq.PhysicsName).ToString & "")
            sqlString.AppendLine("    , " & paramInfoList.Item(.BathTax.PhysicsName).ToString & "")
            sqlString.AppendLine("    , " & paramInfoList.Item(.Com.PhysicsName).ToString & "")
            sqlString.AppendLine("    , " & paramInfoList.Item(.ComGaku.PhysicsName).ToString & "")
            sqlString.AppendLine("    ,'" & paramInfoList.Item(.HiSeikyuSakiCd.PhysicsName).ToString & "")
            sqlString.AppendLine("    ,'" & paramInfoList.Item(.HiSeikyuSakiEdaban.PhysicsName).ToString & "")
            sqlString.AppendLine("    ,'" & paramInfoList.Item(.HiSeikyuSakiSeisanMokuteki.PhysicsName).ToString & "")
            sqlString.AppendLine("    , " & paramInfoList.Item(.KakuninGaku.PhysicsName).ToString & "")
            sqlString.AppendLine("    ,'" & paramInfoList.Item(.Memo.PhysicsName).ToString & "")
            sqlString.AppendLine("    , " & paramInfoList.Item(.Net.PhysicsName).ToString & "")
            sqlString.AppendLine("    ,'" & paramInfoList.Item(.NewKbn.PhysicsName).ToString & "")
            sqlString.AppendLine("    , " & paramInfoList.Item(.RiyouYmd.PhysicsName).ToString & "")
            sqlString.AppendLine("    ,'" & paramInfoList.Item(.SeasonCd.PhysicsName).ToString & "")
            sqlString.AppendLine("    ,'" & paramInfoList.Item(.SeisanHoho.PhysicsName).ToString & "")
            sqlString.AppendLine("    ,'" & paramInfoList.Item(.SeisanMokutekiCd.PhysicsName).ToString & "")
            sqlString.AppendLine("    ,'" & paramInfoList.Item(.SiireSakiCd.PhysicsName).ToString & "")
            sqlString.AppendLine("    ,'" & paramInfoList.Item(.SiireSakiEdaban.PhysicsName).ToString & "")
            sqlString.AppendLine("    ,'" & paramInfoList.Item(.SiireSakiShikibetsuCd.PhysicsName).ToString & "")
            sqlString.AppendLine("    , " & paramInfoList.Item(.SyohiTax.PhysicsName).ToString & "")
            sqlString.AppendLine("    ,'" & paramInfoList.Item(.TaxKbn.PhysicsName).ToString & "")
            sqlString.AppendLine("    , " & paramInfoList.Item(.Year.PhysicsName).ToString & "")
            sqlString.AppendLine("    , " & paramInfoList.Item(.Ym.PhysicsName).ToString & "")
            sqlString.AppendLine("    , " & paramInfoList.Item(.Ninzu.PhysicsName).ToString & "")
            sqlString.AppendLine("    ,NULL")
            sqlString.AppendLine("    ,TO_NUMBER(TO_CHAR(SYSDATE,'yyyyMMdd'))")
            sqlString.AppendLine("    ,'" & UserInfoManagement.userId & "'")
            sqlString.AppendLine("    ,'S05_0302'")
            sqlString.AppendLine("    ,TO_NUMBER(TO_CHAR(SYSDATE,'hh24mmss'))")
            sqlString.AppendLine("    ,TO_NUMBER(TO_CHAR(SYSDATE,'yyyyMMdd'))")
            sqlString.AppendLine("    ,'" & UserInfoManagement.userId & "'")
            sqlString.AppendLine("    ,'S05_0302'")
            sqlString.AppendLine("    ,TO_NUMBER(TO_CHAR(SYSDATE,'hh24mmss'))")
            sqlString.AppendLine("    ,'S05_0302'")
            sqlString.AppendLine("    ,'" & UserInfoManagement.userId & "'")
            sqlString.AppendLine("    ,SYSDATE")
            sqlString.AppendLine("    ,'S05_0302'")
            sqlString.AppendLine("    ,'" & UserInfoManagement.userId & "'")
            sqlString.AppendLine("    ,SYSDATE")
            sqlString.AppendLine("    ," & paramInfoList.Item(.LineNo.PhysicsName).ToString & "")
            sqlString.AppendLine("    ," & paramInfoList.Item(.GroupNo.PhysicsName).ToString & "")
            ' 画面一覧(降車ヶ所(徴証)).運休 ※チェックON時'Y'（運休）を編集　それ以外null
            If Not String.IsNullOrEmpty(paramInfoList.Item(.KoshakashoUnkyuuKbn.PhysicsName).ToString) Then
                sqlString.AppendLine("    ,'Y'")
            Else
                sqlString.AppendLine("    ,NULL")
            End If


            sqlString.AppendLine(" ) ")
        End With

        Return execNonQuery(tran, sqlString.ToString)

    End Function

    ''' <summary>
    ''' 原価確定徴証（料金情報）のInsert(IO項目定義:NO14)
    ''' </summary>
    ''' <param name="paramInfoList"></param>
    ''' <param name="tran"></param>
    ''' <returns></returns>
    Public Function InsertCostKakuteiChoshoChargeInfo(Optional ByVal paramInfoList As Hashtable = Nothing,
                                                    Optional ByVal tran As OracleTransaction = Nothing) As Integer

        Dim costKakuteiChoshoChargeInfoEntity As New TCostKakuteiChoshoChargeInfoEntity
        Dim returnValue As Integer = 0

        Dim sqlString As New StringBuilder

        With costKakuteiChoshoChargeInfoEntity
            ' INSERT
            sqlString.AppendLine(" INSERT INTO ")
            ' 原価確定徴証（料金情報）
            sqlString.AppendLine(" T_COST_KAKUTEI_CHOSHO_CHARGE_INFO ")
            sqlString.AppendLine(" ( ")
            ' コースコード
            sqlString.AppendLine(" CRS_CD, ")
            ' 出発日
            sqlString.AppendLine(" SYUPT_DAY, ")
            ' 号車
            sqlString.AppendLine(" GOUSYA, ")
            ' 徴証区分
            sqlString.AppendLine(" CHOSHO_KBN, ")
            ' SEQ
            sqlString.AppendLine(" SEQ, ")
            ' 料金区分（人員）コード
            sqlString.AppendLine(" CHARGE_KBN_JININ_CD, ")
            ' 入湯税単価
            sqlString.AppendLine(" BATH_TAX_TANKA, ")
            ' 単価
            sqlString.AppendLine(" TANKA, ")
            ' 人数
            sqlString.AppendLine(" NINZU, ")
            ' 料金１
            sqlString.AppendLine(" CHARGE_1, ")
            ' 料金２
            sqlString.AppendLine(" CHARGE_2, ")
            ' 料金３
            sqlString.AppendLine(" CHARGE_3, ")
            ' 料金４
            sqlString.AppendLine(" CHARGE_4, ")
            ' 料金５
            sqlString.AppendLine(" CHARGE_5, ")
            ' 人数１
            sqlString.AppendLine(" NINZU_1, ")
            ' 人数２
            sqlString.AppendLine(" NINZU_2, ")
            ' 人数３
            sqlString.AppendLine(" NINZU_3, ")
            ' 人数４
            sqlString.AppendLine(" NINZU_4, ")
            ' 人数５
            sqlString.AppendLine(" NINZU_5, ")
            ' 削除日
            sqlString.AppendLine(" DELETE_DAY, ")
            ' 登録日
            sqlString.AppendLine(" ENTRY_DAY, ")
            ' 登録者コード
            sqlString.AppendLine(" ENTRY_PERSON_CD, ")
            ' 登録ＰＧＭＩＤ
            sqlString.AppendLine(" ENTRY_PGMID , ")
            ' 登録時刻
            sqlString.AppendLine(" ENTRY_TIME, ")
            ' 更新日
            sqlString.AppendLine(" UPDATE_DAY, ")
            ' 更新者コード
            sqlString.AppendLine(" UPDATE_PERSON_CD, ")
            ' 更新ＰＧＭＩＤ
            sqlString.AppendLine(" UPDATE_PGMID, ")
            ' 更新時刻
            sqlString.AppendLine(" UPDATE_TIME, ")
            ' システム登録ＰＧＭＩＤ
            sqlString.AppendLine(" SYSTEM_ENTRY_PGMID, ")
            ' システム登録者コード
            sqlString.AppendLine(" SYSTEM_ENTRY_PERSON_CD, ")
            ' システム登録日
            sqlString.AppendLine(" SYSTEM_ENTRY_DAY, ")
            ' システム更新ＰＧＭＩＤ
            sqlString.AppendLine(" SYSTEM_UPDATE_PGMID, ")
            ' システム更新者コード
            sqlString.AppendLine(" SYSTEM_UPDATE_PERSON_CD, ")
            ' システム更新日
            sqlString.AppendLine(" SYSTEM_UPDATE_DAY ")
            sqlString.AppendLine(" ) ")
            ' VALUE
            sqlString.AppendLine(" VALUES ")
            sqlString.AppendLine(" ( ")
            sqlString.AppendLine("    '" & paramInfoList.Item(.CrsCd.PhysicsName).ToString & "'")
            sqlString.AppendLine("    , " & paramInfoList.Item(.SyuptDay.PhysicsName).ToString & "")
            sqlString.AppendLine("    ," & paramInfoList.Item(.Gousya.PhysicsName).ToString & "")
            sqlString.AppendLine("    ,'" & paramInfoList.Item(.ChoshoKbn.PhysicsName).ToString & "'")
            sqlString.AppendLine("    ," & paramInfoList.Item(.Seq.PhysicsName).ToString & "")
            sqlString.AppendLine("    ,'" & paramInfoList.Item(.ChargeKbnJininCd.PhysicsName).ToString & "'")
            sqlString.AppendLine("    ," & paramInfoList.Item(.BathTaxTanka.PhysicsName).ToString & "")
            sqlString.AppendLine("    ," & paramInfoList.Item(.Tanka.PhysicsName).ToString & "")
            sqlString.AppendLine("    ," & paramInfoList.Item(.Ninzu.PhysicsName).ToString & "")
            sqlString.AppendLine("    ," & paramInfoList.Item(.Charge1.PhysicsName).ToString & "")
            sqlString.AppendLine("    ," & paramInfoList.Item(.Charge2.PhysicsName).ToString & "")
            sqlString.AppendLine("    ," & paramInfoList.Item(.Charge3.PhysicsName).ToString & "")
            sqlString.AppendLine("    ," & paramInfoList.Item(.Charge4.PhysicsName).ToString & "")
            sqlString.AppendLine("    ," & paramInfoList.Item(.Charge5.PhysicsName).ToString & "")
            sqlString.AppendLine("    ," & paramInfoList.Item(.Ninzu1.PhysicsName).ToString & "")
            sqlString.AppendLine("    ," & paramInfoList.Item(.Ninzu2.PhysicsName).ToString & "")
            sqlString.AppendLine("    ," & paramInfoList.Item(.Ninzu3.PhysicsName).ToString & "")
            sqlString.AppendLine("    ," & paramInfoList.Item(.Ninzu4.PhysicsName).ToString & "")
            sqlString.AppendLine("    ," & paramInfoList.Item(.Ninzu5.PhysicsName).ToString & "")
            sqlString.AppendLine("    ,NULL")
            sqlString.AppendLine("    ,TO_NUMBER(TO_CHAR(SYSDATE,'yyyyMMdd'))")
            sqlString.AppendLine("    ,'" & UserInfoManagement.userId & "'")
            sqlString.AppendLine("    ,'S05_0302'")
            sqlString.AppendLine("    ,TO_NUMBER(TO_CHAR(SYSDATE,'hh24mmss'))")
            sqlString.AppendLine("    ,TO_NUMBER(TO_CHAR(SYSDATE,'yyyyMMdd'))")
            sqlString.AppendLine("    ,'" & UserInfoManagement.userId & "'")
            sqlString.AppendLine("    ,'S05_0302'")
            sqlString.AppendLine("    ,TO_NUMBER(TO_CHAR(SYSDATE,'hh24mmss'))")
            sqlString.AppendLine("    ,'S05_0302'")
            sqlString.AppendLine("    ,'" & UserInfoManagement.userId & "'")
            sqlString.AppendLine("    ,SYSDATE")
            sqlString.AppendLine("    ,'S05_0302'")
            sqlString.AppendLine("    ,'" & UserInfoManagement.userId & "'")
            sqlString.AppendLine("    ,SYSDATE")
            sqlString.AppendLine(" ) ")
        End With

        Return execNonQuery(tran, sqlString.ToString)

    End Function

    ''' <summary>
    ''' 現払出金履歴のINSERT(IO項目定義:NO15)
    ''' </summary>
    ''' <param name="paramInfoList"></param>
    ''' <returns></returns>
    Public Function InsertGenbaraiWithdrawalHistory(Optional ByVal paramInfoList As Hashtable = Nothing,
                                                    Optional ByVal tran As OracleTransaction = Nothing) As Integer

        Dim genbaraiWithdrawalHistoryEntity As New TGenbaraiWithdrawalHistoryEntity
        Dim returnValue As Integer = 0

        Dim sqlString As New StringBuilder

        With genbaraiWithdrawalHistoryEntity
            ' INSERT
            sqlString.AppendLine(" INSERT INTO ")
            ' 現払出金履歴
            sqlString.AppendLine(" T_GENBARAI_WITHDRAWAL_HISTORY ( ")
            ' コースコード
            sqlString.AppendLine(" CRS_CD, ")
            ' 出発日
            sqlString.AppendLine(" SYUPT_DAY, ")
            ' 号車
            sqlString.AppendLine(" GOUSYA, ")
            ' ＳＥＱ
            sqlString.AppendLine(" SEQ, ")
            ' 出金日
            sqlString.AppendLine(" WITHDRAWAL_DAY, ")
            ' 出金額
            sqlString.AppendLine(" WITHDRAWAL_GAKU, ")
            ' 削除日
            sqlString.AppendLine(" DELETE_DAY, ")
            ' 登録日
            sqlString.AppendLine(" ENTRY_DAY, ")
            ' 登録者コード
            sqlString.AppendLine(" ENTRY_PERSON_CD, ")
            ' 登録ＰＧＭＩＤ
            sqlString.AppendLine(" ENTRY_PGMID, ")
            ' 登録時刻
            sqlString.AppendLine(" ENTRY_TIME, ")
            ' 更新日
            sqlString.AppendLine(" UPDATE_DAY, ")
            ' 更新者コード
            sqlString.AppendLine(" UPDATE_PERSON_CD, ")
            ' 更新ＰＧＭＩＤ
            sqlString.AppendLine(" UPDATE_PGMID, ")
            ' 更新時刻
            sqlString.AppendLine(" UPDATE_TIME, ")
            ' システム登録ＰＧＭＩＤ
            sqlString.AppendLine(" SYSTEM_ENTRY_PGMID, ")
            ' システム登録者コード
            sqlString.AppendLine(" SYSTEM_ENTRY_PERSON_CD, ")
            ' システム登録日
            sqlString.AppendLine(" SYSTEM_ENTRY_DAY, ")
            ' システム更新ＰＧＭＩＤ
            sqlString.AppendLine(" SYSTEM_UPDATE_PGMID, ")
            ' システム更新者コード
            sqlString.AppendLine(" SYSTEM_UPDATE_PERSON_CD, ")
            ' システム更新日
            sqlString.AppendLine(" SYSTEM_UPDATE_DAY ")
            sqlString.AppendLine(" ) ")

            ' VALUE
            sqlString.AppendLine(" VALUES ")
            sqlString.AppendLine(" ( ")
            sqlString.AppendLine("    '" & paramInfoList.Item(.CrsCd.PhysicsName).ToString & "'")
            sqlString.AppendLine("    , " & paramInfoList.Item(.SyuptDay.PhysicsName).ToString & "")
            sqlString.AppendLine("    ," & paramInfoList.Item(.Gousya.PhysicsName).ToString & "")
            sqlString.AppendLine("    ," & paramInfoList.Item(.Seq.PhysicsName).ToString & "")
            sqlString.AppendLine("    ," & paramInfoList.Item(.WithdrawalDay.PhysicsName).ToString & "")
            sqlString.AppendLine("    ," & paramInfoList.Item(.WithdrawalGaku.PhysicsName).ToString & "")
            sqlString.AppendLine("    ,NULL")
            sqlString.AppendLine("    ,TO_NUMBER(TO_CHAR(SYSDATE,'yyyyMMdd'))")
            sqlString.AppendLine("    ,'" & UserInfoManagement.userId & "'")
            sqlString.AppendLine("    ,'S05_0302'")
            sqlString.AppendLine("    ,TO_NUMBER(TO_CHAR(SYSDATE,'hh24mmss'))")
            sqlString.AppendLine("    ,TO_NUMBER(TO_CHAR(SYSDATE,'yyyyMMdd'))")
            sqlString.AppendLine("    ,'" & UserInfoManagement.userId & "'")
            sqlString.AppendLine("    ,'S05_0302'")
            sqlString.AppendLine("    ,TO_NUMBER(TO_CHAR(SYSDATE,'hh24mmss'))")
            sqlString.AppendLine("    ,'S05_0302'")
            sqlString.AppendLine("    ,'" & UserInfoManagement.userId & "'")
            sqlString.AppendLine("    ,SYSDATE")
            sqlString.AppendLine("    ,'S05_0302'")
            sqlString.AppendLine("    ,'" & UserInfoManagement.userId & "'")
            sqlString.AppendLine("    ,SYSDATE")
            sqlString.AppendLine(" ) ")
        End With

        Return execNonQuery(tran, sqlString.ToString)

    End Function

#End Region

End Class
