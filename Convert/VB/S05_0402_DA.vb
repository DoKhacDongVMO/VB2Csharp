Imports System.Text
Imports System.String
Imports System.Reflection
Imports Oracle.ManagedDataAccess.Client
Imports Hatobus.ReservationManagementSystem.Yoyaku

Public Class S05_0402_DA
    Inherits DataAccessorBase

#Region "定数・変数"

    Public Enum accessType As Integer
        getSeisanKomoku                 '精算情報
        getSeisanInfo                   '精算情報
        getSeisanInfoAll                '精算情報(全Field)
        getSeisanUriage                 '精算情報売上
        getSeisanKaisyu                 '精算情報内訳（回収先）
        getOtherUriageSyohin            'その他売上商品マスタ

    End Enum

    '''' <summary>
    '''' 精算区分コード(その他商品売上)
    '''' </summary>
    'Private Const strSeisankbn As String = "40" ':精算区分(その他商品売上)

    ' 精算情報エンティティ
    Private clsSeisanInfoEntity As New SeisanInfoEntity()

#End Region

#Region "SELECT処理"

    ''' <summary>
    ''' SELECT用DBアクセス
    ''' </summary>
    ''' <param name="type"></param>
    ''' <param name="paramInfoList"></param>
    ''' <returns></returns>
    Public Function accessS05_0402(ByVal type As accessType, ByVal paramInfoList As P05_0401ParamData) As DataTable
        'SQL文字列
        Dim sqlString As String = String.Empty
        '戻り値
        Dim returnValue As DataTable = Nothing

        Select Case type
            Case accessType.getOtherUriageSyohin
                'その他売上商品マスタ
                sqlString = getOtherUriageSyohin(paramInfoList)
            Case accessType.getSeisanKomoku
                '精算項目
                sqlString = getSeisanKomoku(paramInfoList)
            Case accessType.getSeisanInfoAll
                '精算情報(全項目)
                sqlString = getSeisanInfoAll(paramInfoList)
            Case accessType.getSeisanInfo
                '精算情報
                sqlString = getSeisanInfo(paramInfoList)
            Case accessType.getSeisanUriage
                '精算情報内訳
                sqlString = getSeisanUriage(paramInfoList)
            Case accessType.getSeisanKaisyu
                '精算情報内訳（回収先）
                sqlString = getSeisanKaisyu(paramInfoList)
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
    ''' 精算項目マスタ取得
    ''' </summary>
    ''' <param name="param"></param>
    ''' <returns></returns>
    Public Function getSeisanKomoku(param As P05_0401ParamData) As String

        Dim sql As New StringBuilder
        'SQL生成

        sql.AppendLine(" SELECT ")
        sql.AppendLine("        SK.SEISAN_KOUMOKU_CD ")         '精算項目コード
        sql.AppendLine("       ,SK.SEISAN_KOUMOKU_NAME ")       '精算項目名
        sql.AppendLine("       ,SK.TAISYAKU_KBN ")              '貸借区分
        sql.AppendLine("   FROM M_SEISAN_KOUMOKU SK")
        sql.AppendLine("  WHERE SK.SONOTA_SYOHIN_FLG = 1 ")     'その他商品フラグ(0:OFF 1:ON)
        sql.AppendLine("  ORDER BY SK.SEISAN_KOUMOKU_CD ")

        Return sql.ToString
    End Function

    ''' <summary>
    ''' 精算情報内訳（回収先）取得
    ''' </summary>
    ''' <param name="param"></param>
    ''' <returns></returns>
    Public Function getSeisanKaisyu(param As P05_0401ParamData) As String
        Dim sql As New StringBuilder
        'SQL生成
        sql.AppendLine(" SELECT ")
        sql.AppendLine("     SI.RECOVERY_SAKI_DAY ")                            '回収先日
        sql.AppendLine("    ,SI.RECOVERY_SAKI_YOYAKU_KBN ")                     '回収先予約区分
        sql.AppendLine("    ,SI.RECOVERY_SAKI_YOYAKU_NO ")                      '回収先予約ＮＯ
        sql.AppendLine("    ,SI.RECOVERY_SAKI_TYPE ")                           '回収先種類
        sql.AppendLine("    ,SI.OTHER_URIAGE_NAME_BF ")                         '他売上名前
        sql.AppendLine("    ,SI.OTHER_URIAGE_AITESAKI ")                        '他売上相手先
        sql.AppendLine("    ,SI.OTHER_URIAGE_SYOHIN_BIKO ")                     '他売上商品備考
        sql.AppendLine("    ,SI.OTHER_URIAGE_SYOHIN_BIKO_2 ")                   '他売上商品備考２
        sql.AppendLine("    ,SI.OTHER_URIAGE_SYOHIN_H_G ")                      '他売上商品邦人／外客区分
        sql.AppendLine("    ,YIB.SURNAME_KJ || YIB.NAME_KJ AS SEIMEI")       '姓（漢字）＋名（漢字）
        sql.AppendLine("    ,SI.CRS_CD ")                                       'コースコード
        '                                                                        コース名略称
        sql.AppendLine("    ,(SELECT DISTINCT NVL(CRS_NAME_RK,CRS_NAME) FROM T_CRS_LEDGER_BASIC WHERE CRS_CD = SI.CRS_CD AND SYUPT_DAY = SI.RECOVERY_SAKI_DAY ) AS CRS_NAME_RK ")
        sql.AppendLine("    ,SUS.OTHER_URIAGE_SYOHIN_NAME AS KAISYU_OTHER_URIAGE_SYOHIN_NAME")      '回収先:他売上商品名
        sql.AppendLine("    ,SIA.OTHER_URIAGE_SYOHIN_BIKO AS KAISYU_OTHER_URIAGE_SYOHIN_BIKO")      '回収先:他売上商品備考
        sql.AppendLine("    ,SIA.OTHER_URIAGE_SYOHIN_BIKO_2 AS KAISYU_OTHER_URIAGE_SYOHIN_BIKO_2")  '回収先:他売上商品備考２
        sql.AppendLine("    ,SUS.OTHER_URIAGE_SYOHIN_NAME ")                    '他売上商品名
        sql.AppendLine(" FROM   ")
        sql.AppendLine("     T_SEISAN_INFO SI ")                            '精算情報
        sql.AppendLine(" LEFT JOIN T_SEISAN_INFO SIA  ")                    '精算情報A(回収先)
        sql.AppendLine("   ON SI.RECOVERY_SAKI_YOYAKU_KBN = SIA.YOYAKU_KBN ")
        sql.AppendLine("  AND SI.RECOVERY_SAKI_YOYAKU_NO = SIA.YOYAKU_NO ")
        sql.AppendLine("  AND SI.RECOVERY_SAKI_SEQ = SIA.SEISAN_INFO_SEQ ")
        sql.AppendLine(" LEFT JOIN  T_YOYAKU_INFO_BASIC YIB ")              '予約情報（基本）
        sql.AppendLine("   ON SI.RECOVERY_SAKI_YOYAKU_KBN = YIB.YOYAKU_KBN ")
        sql.AppendLine("  AND SI.RECOVERY_SAKI_YOYAKU_NO = YIB.YOYAKU_NO ")
        sql.AppendLine(" LEFT JOIN  M_SONOTA_URIAGE_SYOHIN SUS ")           'その他売上商品マスタ
        sql.AppendLine("   ON SI.OTHER_URIAGE_SYOHIN_CD_1 = SUS.OTHER_URIAGE_SYOHIN_CD_1 ")
        sql.AppendLine("  AND SI.OTHER_URIAGE_SYOHIN_CD_2 = SUS.OTHER_URIAGE_SYOHIN_CD_2 ")
        sql.AppendLine("  AND SI.OTHER_URIAGE_SYOHIN_H_G = SUS.HOUJIN_GAIKYAKU_KBN ")
        sql.AppendLine(" WHERE ")
        With clsSeisanInfoEntity
            sql.AppendLine("      SI.SEISAN_KBN = '" & CStr(FixedCd.SeisanKbn.SonotaSyohinUriage) & "' ") '精算区分 = 40(その他商品売上)
            sql.AppendLine("  AND SI.KENNO = " & setParam(.kenno.PhysicsName, param.KenNO, OracleDbType.Char))                            '券番
            sql.AppendLine("  AND SI.SEISAN_INFO_SEQ = " & setParam(.seisanInfoSeq.PhysicsName, param.SEQ, OracleDbType.Char))   '精算情報SEQ
            'sql.AppendLine("  AND SI.SEQ = '" & setParam(.seisanInfoSeq.PhysicsName, param.SEQ, OracleDbType.Char) & "'")
        End With

        Return sql.ToString
    End Function

    ''' <summary>
    ''' 精算情報取得
    ''' </summary>
    ''' <param name="param"></param>
    ''' <returns></returns>
    Public Function getSeisanInfoAll(param As P05_0401ParamData) As String
        Dim sql As New StringBuilder
        sql.AppendLine(" SELECT ")
        sql.AppendLine("        * ")
        sql.AppendLine("   FROM ")
        sql.AppendLine("        T_SEISAN_INFO SSN")
        With clsSeisanInfoEntity
            sql.AppendLine("  WHERE ")
            ' 会社コード
            sql.AppendLine("        COMPANY_CD = " & setParam(.companyCd.PhysicsName, param.CompanyCd, OracleDbType.Char))
            ' 必須項目：営業所コード
            sql.AppendLine("    AND EIGYOSYO_CD = " & setParam(.eigyosyoCd.PhysicsName, param.EigyosyoCd, OracleDbType.Char))
            ' 必須項目：作成日
            sql.AppendLine("    AND CREATE_DAY = " & setParam(.createDay.PhysicsName, param.ProcessDate, OracleDbType.Decimal, 8, 0))
            ' 時刻
            'sql.AppendLine("    AND SSN.CREATE_TIME = " & setParam(.createTime.PhysicsName, param.ProcessDate, OracleDbType.Decimal, 8, 0))

            '商品コード
            sql.AppendLine(" AND OTHER_URIAGE_SYOHIN_CD_1 = " & setParam(.otherUriageSyohinCd1.PhysicsName, param.SyohinCd1, OracleDbType.Char))
            sql.AppendLine(" AND OTHER_URIAGE_SYOHIN_CD_2 = " & setParam(.otherUriageSyohinCd2.PhysicsName, param.SyohinCd2, OracleDbType.Char))

        End With
        Return Sql.ToString
    End Function


    ''' <summary>
    ''' 精算情報取得
    ''' </summary>
    ''' <param name="seisanInfoSeq"></param>
    ''' <returns></returns>
    Public Function getSeisanInfoUtiwakeAll(seisanInfoSeq As Integer) As String
        Dim sql As New StringBuilder
        sql.AppendLine(" SELECT ")
        sql.AppendLine("        * ")
        sql.AppendLine("   FROM ")
        sql.AppendLine("        T_SEISAN_INFO_UTIWAKE ")
        With clsSeisanInfoEntity
            sql.AppendLine("  WHERE ")
            ' 会社コード
            sql.AppendLine("        SEISAN_INFO_SEQ = " & setParam(.seisanInfoSeq.PhysicsName, seisanInfoSeq, OracleDbType.Int16))
        End With
        Return sql.ToString
    End Function

    ''' <summary>
    ''' 精算情報取得
    ''' </summary>
    ''' <param name="param"></param>
    ''' <returns></returns>
    Public Function getSeisanInfo(param As P05_0401ParamData) As String
        Dim sql As New StringBuilder
        'SQL生成

        sql.AppendLine(" SELECT ")
        sql.AppendLine("        SSN.SEISAN_INFO_SEQ ")
        sql.AppendLine("       ,SSN.OTHER_URIAGE_SYOHIN_CD_1 ")
        sql.AppendLine("       ,SSN.OTHER_URIAGE_SYOHIN_CD_2 ")
        sql.AppendLine("       ,LPAD(' ',4)  AS SYOHIN_CD ")
        sql.AppendLine("       ,SSN.OTHER_URIAGE_SYOHIN_H_G ")
        sql.AppendLine("       ,SUS.TAISYAKU_KBN ")                      ' その他売上商品マスタ.貸借区分(1:借方、2:貸方)
        sql.AppendLine("       ,SUS.OTHER_URIAGE_SYOHIN_NAME ")          ' その他売上商品マスタ.他売上商品名
        sql.AppendLine("       ,SSN.SEISAN_KBN ")
        sql.AppendLine("       ,SSN.KENNO ")

        sql.AppendLine("       ,SSN.CREATE_DAY ") '作成日	

        sql.AppendLine("       ,TO_CHAR(TO_DATE(LPAD(SSN.CREATE_TIME, 6,'0'),'HH24:MI:SS'),'HH24:MI:SS') AS TIME ")
        sql.AppendLine("       ,SSN.ENTRY_PERSON_CD ")
        sql.AppendLine("       ,SSN.SIGNON_TIME ")
        sql.AppendLine("       ,CASE WHEN SSN.URIAGE_KBN='" & FixedCd.UriageKbnType.HaraiModoshi & "' THEN ")
        sql.AppendLine("             SSN.OTHER_URIAGE_SYOHIN_MODOSI ")
        sql.AppendLine("        ELSE SSN.OTHER_URIAGE_SYOHIN_URIAGE ")
        sql.AppendLine("        END AS TOTAL ")
        sql.AppendLine("       ,SSN.OTHER_URIAGE_SYOHIN_MODOSI ")
        sql.AppendLine("       ,SSN.OTHER_URIAGE_SYOHIN_URIAGE ")
        sql.AppendLine("       ,SSN.OTHER_URIAGE_SYOHIN_QUANTITY ") '他売上商品数量
        sql.AppendLine("       ,SSN.OTHER_URIAGE_SYOHIN_TANKA ") '他売上商品単価


        sql.AppendLine("       ,SSN.OTHER_URIAGE_SYOHIN_BIKO ")
        sql.AppendLine("       ,SSN.OTHER_URIAGE_SYOHIN_BIKO_2 ")
        sql.AppendLine("       ,LPAD(' ',8) AS URIAGEINPUT ")
        sql.AppendLine("   FROM ")
        sql.AppendLine("        T_SEISAN_INFO SSN")
        sql.AppendLine("        INNER JOIN M_SONOTA_URIAGE_SYOHIN SUS ")
        sql.AppendLine("           ON SUS.OTHER_URIAGE_SYOHIN_CD_1 = SSN.OTHER_URIAGE_SYOHIN_CD_1 ")
        sql.AppendLine("          AND SUS.OTHER_URIAGE_SYOHIN_CD_2 = SSN.OTHER_URIAGE_SYOHIN_CD_2 ")
        sql.AppendLine("          AND SUS.HOUJIN_GAIKYAKU_KBN = SSN.OTHER_URIAGE_SYOHIN_H_G ")
        With clsSeisanInfoEntity
            sql.AppendLine("  WHERE ")
            ' 会社コード
            sql.AppendLine("        SSN.COMPANY_CD = " & setParam(.companyCd.PhysicsName, param.CompanyCd, OracleDbType.Char))
            ' 必須項目：営業所コード
            sql.AppendLine("    AND SSN.EIGYOSYO_CD = " & setParam(.eigyosyoCd.PhysicsName, param.EigyosyoCd, OracleDbType.Char))
            ' 必須項目：作成日
            sql.AppendLine("    AND SSN.CREATE_DAY = " & setParam(.createDay.PhysicsName, param.ProcessDate, OracleDbType.Decimal, 8, 0))
            ' 精算情報SEQ
            sql.AppendLine("    AND SSN.SEISAN_INFO_SEQ = " & setParam(.seq.PhysicsName, param.SEQ, OracleDbType.Decimal, 12, 0))
            ' 券番
            sql.AppendLine("    AND SSN.KENNO = " & setParam(.kenno.PhysicsName, param.KenNO, OracleDbType.Char))
            ' 商品コード
            sql.AppendLine("    AND SSN.OTHER_URIAGE_SYOHIN_CD_1 = " & setParam(.otherUriageSyohinCd1.PhysicsName, param.SyohinCd1, OracleDbType.Char))
            sql.AppendLine("    AND SSN.OTHER_URIAGE_SYOHIN_CD_2 = " & setParam(.otherUriageSyohinCd2.PhysicsName, param.SyohinCd2, OracleDbType.Char))

        End With
        '' 並び順
        'sql.AppendLine(" ORDER BY ")
        'sql.AppendLine("     SSN.OTHER_URIAGE_SYOHIN_CD_1 ASC ")
        'sql.AppendLine("   , SSN.OTHER_URIAGE_SYOHIN_CD_2 ASC ")
        'sql.AppendLine("   , SSN.CREATE_TIME ASC ")
        Return sql.ToString
    End Function

    ''' <summary>
    ''' 精算情報(内訳)取得
    ''' </summary>
    ''' <param name="param"></param>
    ''' <returns></returns>
    Public Function getSeisanUriage(param As P05_0401ParamData) As String
        Dim sql As New StringBuilder
        'SQL生成
        sql.AppendLine(" SELECT")
        sql.AppendLine("     SK.SEISAN_KOUMOKU_CD")
        sql.AppendLine("   , SK.SEISAN_KOUMOKU_NAME")
        sql.AppendLine("   , SK.TAISYAKU_KBN")
        sql.AppendLine("   , SU.BIKO")
        sql.AppendLine("   , SU.HURIKOMI_KBN")
        sql.AppendLine("   , NVL(SUM(SU.KINGAKU),0) AS URIAGE")
        sql.AppendLine("   , SU.ISSUE_COMPANY_CD")
        sql.AppendLine(" FROM")
        sql.AppendLine("   M_SEISAN_KOUMOKU SK")
        sql.AppendLine(" LEFT JOIN T_SEISAN_INFO_UTIWAKE SU")
        sql.AppendLine("   ON SK.SEISAN_KOUMOKU_CD = SU.SEISAN_KOUMOKU_CD")
        With clsSeisanInfoEntity
            sql.AppendLine("   AND SU.SEISAN_KBN = " & setParam(.seisanKbn.PhysicsName, CStr(FixedCd.SeisanKbn.SonotaSyohinUriage).Trim, OracleDbType.Char))
            sql.AppendLine("   AND SU.KENNO = " & setParam(.kenno.PhysicsName, param.KenNO, OracleDbType.Char))     '40:精算区分(その他商品売上)
            sql.AppendLine("   AND SU.SEISAN_INFO_SEQ = " & setParam(.seisanInfoSeq.PhysicsName, param.SEQ, OracleDbType.Char))
        End With
        sql.AppendLine(" LEFT JOIN T_SEISAN_INFO SI")
        sql.AppendLine(" ON SU.SEISAN_INFO_SEQ = SI.SEISAN_INFO_SEQ")
        sql.AppendLine(" AND SU.SEISAN_KBN = SI.SEISAN_KBN")
        sql.AppendLine(" AND SU.KENNO = SI.KENNO  ")
        sql.AppendLine(" GROUP BY")
        sql.AppendLine("     SK.SEISAN_KOUMOKU_CD")
        sql.AppendLine("   , SK.SEISAN_KOUMOKU_NAME")
        sql.AppendLine("   , SK.TAISYAKU_KBN")
        sql.AppendLine("   , SU.BIKO")
        sql.AppendLine("   , SU.HURIKOMI_KBN")
        sql.AppendLine("   , SU.ISSUE_COMPANY_CD")
        sql.AppendLine(" ORDER BY")
        sql.AppendLine("     SK.TAISYAKU_KBN ")
        sql.AppendLine("    ,SK.SEISAN_KOUMOKU_CD ")
        Return sql.ToString
    End Function

    ''' <summary>
    ''' その他売上商品マスタ取得
    ''' </summary>
    ''' <param name="param">その他商品コード1,2</param>
    ''' <returns></returns>
    Public Function getOtherUriageSyohin(param As P05_0401ParamData) As String
        Dim sql As New StringBuilder
        sql.AppendLine(" SELECT ")
        sql.AppendLine("    HOUJIN_GAIKYAKU_KBN")               ' 邦人外客区分
        sql.AppendLine("   ,OTHER_URIAGE_SYOHIN_NAME")          ' 他売上商品名
        sql.AppendLine("   ,TAISYAKU_KBN ")                     ' その他売上商品マスタ.貸借区分
        sql.AppendLine(" FROM ")
        sql.AppendLine("    M_SONOTA_URIAGE_SYOHIN")
        With clsSeisanInfoEntity
            sql.AppendLine("  WHERE ")
            'その他商品コード1,2
            sql.AppendLine("        OTHER_URIAGE_SYOHIN_CD_1 = " & setParam(.otherUriageSyohinCd1.PhysicsName, param.SyohinCd1, OracleDbType.Char))
            sql.AppendLine("    AND OTHER_URIAGE_SYOHIN_CD_2 = " & setParam(.otherUriageSyohinCd2.PhysicsName, param.SyohinCd2, OracleDbType.Char))
        End With
        Return sql.ToString
    End Function

    ''' <summary>
    ''' SEQ（精算情報）の取得（精算区分（'40'）、券番単位で採番)
    ''' </summary>
    ''' <param name="kenNo"></param>
    ''' <returns></returns>
    Public Function getMaxSeq(kenNo As String) As DataTable
        MyBase.paramClear()
        Dim ent As New SeisanInfoEntity
        Dim sb As New StringBuilder

        'パラメータの取得
        Dim prmKenNo As String = Me.prepareParam("KENNO", kenNo, ent.yoyakuKbn)
        Dim prmSeisanKbn As String = Me.prepareParam("SEISAN_KBN", CStr(FixedCd.SeisanKbn.SonotaSyohinUriage).Trim, ent.seisanKbn)

        Try
            sb.AppendLine(" SELECT")
            sb.AppendLine("     NVL(MAX(SEQ), 0) AS SEQ")
            sb.AppendLine(" FROM")
            sb.AppendLine("     T_SEISAN_INFO")
            sb.AppendLine(" WHERE ")
            sb.AppendLine(" ")
            sb.AppendLine($"         KENNO = {prmKenNo}")
            sb.AppendLine($"     AND SEISAN_KBN = {prmSeisanKbn}")
            sb.AppendLine(" ORDER BY")
            sb.AppendLine("     YOYAKU_KBN, YOYAKU_NO")

            Return MyBase.getDataTable(sb.ToString())

        Catch ex As Exception
            Throw
        End Try
    End Function
#End Region

#Region "登録/更新処理"

    ''' <summary>
    ''' 登録用SQLの作成
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="nameTable"></param>
    ''' <param name="entity"></param>
    ''' <returns></returns>
    Private Function createSqlInsert(Of T As Class)(nameTable As String, entity As T) As String
        Dim properties() As PropertyInfo = GetType(T).GetProperties()

        Dim insertSql As New StringBuilder()
        Dim valueSql As New StringBuilder()

        Dim idx As Integer = 0
        Dim comma As String = ""
        Dim physicsName As String = ""

        Dim param As String = ""

        'プロパティを取り出し、プロパティの型に応じて登録SQLのカラム名とVALUE部分を生成
        For Each prop As PropertyInfo In properties

            If prop.PropertyType Is GetType(EntityKoumoku_YmdType) Then
                '日付型の場合
                'プロパティ名取得
                Dim propYmdType As EntityKoumoku_YmdType _
                    = DirectCast(prop.GetValue(entity), EntityKoumoku_YmdType)
                physicsName = propYmdType.PhysicsName

                param = MyBase.setParam(physicsName, propYmdType.Value, propYmdType.DBType)

            ElseIf prop.PropertyType Is GetType(EntityKoumoku_NumberType) Then
                '数字型の場合
                'プロパティ名取得
                Dim propNumberType As EntityKoumoku_NumberType _
                    = DirectCast(prop.GetValue(entity), EntityKoumoku_NumberType)
                physicsName = propNumberType.PhysicsName

                param = Me.prepareParam(physicsName, propNumberType.Value, propNumberType)

            ElseIf prop.PropertyType Is GetType(EntityKoumoku_Number_DecimalType) Then
                'Decimal型の場合
                'プロパティ名取得
                Dim propNumberDecimaltype As EntityKoumoku_Number_DecimalType _
                    = DirectCast(prop.GetValue(entity), EntityKoumoku_Number_DecimalType)
                physicsName = propNumberDecimaltype.PhysicsName

                param = Me.prepareParam(physicsName, propNumberDecimaltype.Value, propNumberDecimaltype)

            Else
                '文字型の場合
                'プロパティ名取得
                Dim propMojiType As EntityKoumoku_MojiType _
                    = DirectCast(prop.GetValue(entity), EntityKoumoku_MojiType)
                physicsName = propMojiType.PhysicsName

                param = Me.prepareParam(physicsName, propMojiType.Value, propMojiType)
            End If

            'SQL生成
            insertSql.AppendLine($"{comma} {physicsName}")
            valueSql.AppendLine($"{comma} {param}")

            comma = ","
            idx += 1
        Next


        ' INSERT文作成
        Dim sb As New StringBuilder()
        sb.AppendLine($" INSERT INTO {nameTable}")
        sb.AppendLine(" ( ")
        sb.AppendLine(insertSql.ToString())
        sb.AppendLine(" ) VALUES ( ")
        sb.AppendLine(valueSql.ToString())
        sb.AppendLine(" ) ")

        Return sb.ToString()

    End Function

    ''' <summary>
    ''' 精算情報の更新SQLの作成 
    ''' </summary>
    ''' <param name="ent"></param>
    ''' <returns></returns>
    Private Function createSqlUpdateSeisanInfo(ent As SeisanInfoEntity) As String
        'パラメータの設定
        Dim prmCompanyCd As String = Me.prepareParam("COMPANY_CD", ent.companyCd.Value, ent.companyCd)
        Dim prmEigyosyoCd As String = Me.prepareParam("EIGYOSYO_CD", ent.eigyosyoCd.Value, ent.eigyosyoCd)
        Dim prmCrsCd As String = Me.prepareParam("CRS_CD", ent.crsCd.Value, ent.crsCd)
        Dim prmNosignKbn As String = Me.prepareParam("NOSIGN_KBN", ent.nosignKbn.Value, ent.nosignKbn)
        Dim prmOtherUriageAitesaki As String = Me.prepareParam("OTHER_URIAGE_AITESAKI", ent.otherUriageAitesaki.Value, ent.otherUriageAitesaki)
        Dim prmOtherUriageNameBf As String = Me.prepareParam("OTHER_URIAGE_NAME_BF", ent.otherUriageNameBf.Value, ent.otherUriageNameBf)
        Dim prmOtherUriageSyohinBiko As String = Me.prepareParam("OTHER_URIAGE_SYOHIN_BIKO", ent.otherUriageSyohinBiko.Value, ent.otherUriageSyohinBiko)
        Dim prmOtherUriageSyohinBiko2 As String = Me.prepareParam("OTHER_URIAGE_SYOHIN_BIKO_2", ent.otherUriageSyohinBiko2.Value, ent.otherUriageSyohinBiko2)
        Dim prmOtherUriageSyohinQuantity As String = Me.prepareParam("OTHER_URIAGE_SYOHIN_QUANTITY", ent.otherUriageSyohinQuantity.Value, ent.otherUriageSyohinQuantity)
        Dim prmOtherUriageSyohinTanka As String = Me.prepareParam("OTHER_URIAGE_SYOHIN_TANKA", ent.otherUriageSyohinTanka.Value, ent.otherUriageSyohinTanka)
        Dim prmOtherUriageSyohinUriage As String = Me.prepareParam("OTHER_URIAGE_SYOHIN_URIAGE", ent.otherUriageSyohinUriage.Value, ent.otherUriageSyohinUriage)
        Dim prmOtherUriageSyohinModosi As String = Me.prepareParam("OTHER_URIAGE_SYOHIN_MODOSI", ent.otherUriageSyohinModosi.Value, ent.otherUriageSyohinModosi)
        Dim prmRecoverySakiDay As String = Me.prepareParam("RECOVERY_SAKI_DAY", ent.recoverySakiDay.Value, ent.recoverySakiDay)
        Dim prmRecoverySakiSeq As String = Me.prepareParam("RECOVERY_SAKI_SEQ", ent.recoverySakiSeq.Value, ent.recoverySakiSeq)
        Dim prmRecoverySakiType As String = Me.prepareParam("RECOVERY_SAKI_TYPE", ent.recoverySakiType.Value, ent.recoverySakiType)
        Dim prmRecoverySakiKenno As String = Me.prepareParam("RECOVERY_SAKI_KENNO", ent.recoverySakiKenno.Value, ent.recoverySakiKenno)
        Dim prmRecoverySakiYoyakuKbn As String = Me.prepareParam("RECOVERY_SAKI_YOYAKU_KBN", ent.recoverySakiYoyakuKbn.Value, ent.recoverySakiYoyakuKbn)
        Dim prmRecoverySakiYoyakuNo As String = Me.prepareParam("RECOVERY_SAKI_YOYAKU_NO", ent.recoverySakiYoyakuNo.Value, ent.recoverySakiYoyakuNo)
        Dim prmSeisanKbn As String = Me.prepareParam("SEISAN_KBN", ent.seisanKbn.Value, ent.seisanKbn)
        Dim prmSignonTime As String = Me.prepareParam("SIGNON_TIME", ent.signonTime.Value, ent.signonTime)
        Dim prmUpdateDay As String = Me.prepareParam("UPDATE_DAY", ent.updateDay.Value, ent.updateDay)
        Dim prmUpdatePersonCd As String = Me.prepareParam("UPDATE_PERSON_CD", ent.updatePersonCd.Value, ent.updatePersonCd)
        Dim prmUpdatePgmid As String = Me.prepareParam("UPDATE_PGMID", ent.updatePgmid.Value, ent.updatePgmid)
        Dim prmUpdateTime As String = Me.prepareParam("UPDATE_TIME", ent.updateTime.Value, ent.updateTime)
        Dim prmUriagekBN As String = Me.prepareParam("URIAGE_KBN", ent.uriageKbn.Value, ent.uriageKbn)
        Dim prmSystemUpdatePgmId As String = Me.prepareParam("SYSTEM_UPDATE_PGMID", ent.systemUpdatePgmid.Value, ent.systemUpdatePgmid)
        Dim prmSystemUpdatePersonCd As String = Me.prepareParam("SYSTEM_UPDATE_PERSON_CD", ent.systemUpdatePersonCd.Value, ent.systemUpdatePersonCd)
        Dim prmSystemUpdateDay As String = Me.prepareParam("SYSTEM_UPDATE_DAY", ent.systemUpdateDay.Value, ent.systemUpdateDay)
        Dim prmKenNo As String = Me.prepareParam("KENNO", ent.kenno.Value, ent.kenno)           '券番
        Dim prmSeq As String = Me.prepareParam("SEQ", ent.seq.Value, ent.seq)                   'ＳＥＱ

        Dim sb As New StringBuilder
        sb.AppendLine("UPDATE")
        sb.AppendLine("    T_SEISAN_INFO ")
        sb.AppendLine("SET ")
        sb.AppendLine($"     COMPANY_CD = {prmCompanyCd}")                                      '会社コード
        sb.AppendLine($"    ,EIGYOSYO_CD = {prmEigyosyoCd}")                                    '営業所コード
        sb.AppendLine($"    ,CRS_CD = {prmCrsCd}")                                              'コースコード
        sb.AppendLine($"    ,NOSIGN_KBN = {prmNosignKbn}")                                      'ノーサイン区分
        sb.AppendLine($"    ,OTHER_URIAGE_AITESAKI = {prmOtherUriageAitesaki}")                 '他売上相手先
        sb.AppendLine($"    ,OTHER_URIAGE_NAME_BF = {prmOtherUriageNameBf}")                    '他売上名前
        sb.AppendLine($"    ,OTHER_URIAGE_SYOHIN_BIKO = {prmOtherUriageSyohinBiko}")            '他売上商品備考 
        sb.AppendLine($"    ,OTHER_URIAGE_SYOHIN_BIKO_2 = {prmOtherUriageSyohinBiko2}")         '他売上商品備考２
        sb.AppendLine($"    ,OTHER_URIAGE_SYOHIN_QUANTITY = {prmOtherUriageSyohinQuantity}")    '他売上商品数量
        sb.AppendLine($"    ,OTHER_URIAGE_SYOHIN_TANKA = {prmOtherUriageSyohinTanka}")          '他売上商品単価
        sb.AppendLine($"    ,OTHER_URIAGE_SYOHIN_URIAGE = {prmOtherUriageSyohinUriage}")        '他売上商品売上
        sb.AppendLine($"    ,OTHER_URIAGE_SYOHIN_MODOSI = {prmOtherUriageSyohinModosi}")        '他売上商品払戻
        sb.AppendLine($"    ,RECOVERY_SAKI_DAY = {prmRecoverySakiDay}")                         '回収先日	
        sb.AppendLine($"    ,RECOVERY_SAKI_KENNO = {prmRecoverySakiKenno}")                     '回収先券番
        sb.AppendLine($"    ,RECOVERY_SAKI_SEQ = {prmRecoverySakiSeq}")                         '回収先ＳＥＱ
        sb.AppendLine($"    ,RECOVERY_SAKI_TYPE = {prmRecoverySakiType}")                       '回収先種類       
        sb.AppendLine($"    ,RECOVERY_SAKI_YOYAKU_KBN = {prmRecoverySakiYoyakuKbn}")            '回収先予約区分
        sb.AppendLine($"    ,RECOVERY_SAKI_YOYAKU_NO = {prmRecoverySakiYoyakuNo}")              '回収先予約ＮＯ
        sb.AppendLine($"    ,SIGNON_TIME = {prmSignonTime}")                                    'サインオン時刻
        sb.AppendLine($"    ,UPDATE_DAY = {prmUpdateDay}")                                      '更新日
        sb.AppendLine($"    ,UPDATE_PERSON_CD = {prmUpdatePersonCd}")                           '更新者コード
        sb.AppendLine($"    ,UPDATE_PGMID = {prmUpdatePgmid}")                                  '更新ＰＧＭＩＤ
        sb.AppendLine($"    ,UPDATE_TIME = {prmUpdateTime}")                                    '更新時刻
        sb.AppendLine($"    ,URIAGE_KBN = {prmUriagekBN}")                                      '更新時刻
        sb.AppendLine($"    ,SYSTEM_UPDATE_PGMID = {prmSystemUpdatePgmId}")                     'システム更新ＰＧＭＩＤ
        sb.AppendLine($"    ,SYSTEM_UPDATE_PERSON_CD = {prmSystemUpdatePersonCd}")              'システム更新者コード
        sb.AppendLine($"    ,SYSTEM_UPDATE_DAY = {prmSystemUpdateDay}")                         'システム更新日
        sb.AppendLine(" WHERE 1=1")
        sb.AppendLine($" AND SEISAN_KBN = {prmSeisanKbn}")                                      '精算区分
        sb.AppendLine($" AND KENNO  = {prmKenNo}")                                              '券番
        sb.AppendLine($" AND SEQ  = {prmSeq}")                                                  'ＳＥＱ
        Return sb.ToString
    End Function

    ''' <summary>
    ''' 精算情報内訳の削除SQLの作成
    ''' </summary>
    ''' <param name="seisanInfoSeq"></param>
    ''' 精算情報SEQ
    ''' <returns></returns>
    Private Function createSqlDeleteSeisanInfoUtiwake(seisanInfoSeq As Integer) As String
        Dim ent As New SeisanInfoUtiwakeEntity
        Dim prmSeisanInfoSeq As String = Me.prepareParam("SEISAN_INFO_SEQ", seisanInfoSeq, ent.seisanInfoSeq)

        Dim sb As New StringBuilder
        sb.AppendLine("DELETE")
        sb.AppendLine("    T_SEISAN_INFO_UTIWAKE")
        sb.AppendLine("WHERE 1=1")
        sb.AppendLine($"    And SEISAN_INFO_SEQ  = {prmSeisanInfoSeq}")

        Return sb.ToString
    End Function

    ''' <summary>
    ''' その他商品・その他精算の登録
    ''' </summary>
    ''' <param name="s05_0402Entity"></param>
    ''' <returns></returns>
    Public Function insertOtherSeisanGroup(s05_0402Entity As S05_0402Entity) As Boolean
        Dim oraTran As OracleTransaction = Nothing
        Dim returnValue As Integer = 0

        Try
            'トランザクション開始
            oraTran = MyBase.callBeginTransaction()
            Dim sql As String = ""
            Dim updateCount As Integer = 0 '更新件数

            '精算情報
            For Each ent As SeisanInfoEntity In s05_0402Entity.SeisanInfoEntityList
                MyBase.paramClear()
                Dim insertSeisanInfoQuery As String = Me.createSqlInsert(Of SeisanInfoEntity)(CommonHakken.NameTblSeisanInfo, ent)
                updateCount = MyBase.execNonQuery(oraTran, insertSeisanInfoQuery)
                If updateCount <= 0 Then
                    MyBase.callRollbackTransaction(oraTran)
                    Return False
                End If
            Next

            '精算情報内訳
            For Each ent As SeisanInfoUtiwakeEntity In s05_0402Entity.SeisanInfoUtiwakeEntityList
                MyBase.paramClear()
                Dim insertSeisanInfoUtiwakeQuery As String = Me.createSqlInsert(Of SeisanInfoUtiwakeEntity)(CommonHakken.NameTblSeisanInfoUtiwake, ent)
                updateCount = MyBase.execNonQuery(oraTran, insertSeisanInfoUtiwakeQuery)
                If updateCount <= 0 Then
                    MyBase.callRollbackTransaction(oraTran)
                    Return False
                End If
            Next

            'トランザクションコミット
            MyBase.callCommitTransaction(oraTran)

        Catch ex As Exception
            MyBase.callRollbackTransaction(oraTran)
            Throw

        Finally
            oraTran.Dispose()
        End Try

        Return True

    End Function

    ''' <summary>
    ''' その他商品・その他精算の更新
    ''' </summary>
    ''' <returns></returns>
    Public Function updateOtherSeisanGroup(s05_0402Entity As S05_0402Entity, param As P05_0401ParamData) As Boolean
        Dim oraTran As OracleTransaction = Nothing
        Dim updateCount As Integer = 0

        Try
            oraTran = MyBase.callBeginTransaction()

            '精算情報
            For Each ent As SeisanInfoEntity In s05_0402Entity.SeisanInfoEntityList
                MyBase.paramClear()
                Dim updateSeisanInfoQuery As String = Me.createSqlUpdateSeisanInfo(ent)
                updateCount = MyBase.execNonQuery(oraTran, updateSeisanInfoQuery)
                If updateCount <= 0 Then
                    MyBase.callRollbackTransaction(oraTran)
                    Return False
                End If
            Next
            '精算情報内訳
            Dim deleteSeisanInfoQuery As String = createSqlDeleteSeisanInfoUtiwake(param.SEQ)
            '   修正前データ削除
            updateCount = MyBase.execNonQuery(oraTran, deleteSeisanInfoQuery)
            '   新規追加
            Dim seisanInfoEntityList As List(Of SeisanInfoUtiwakeEntity) = s05_0402Entity.SeisanInfoUtiwakeEntityList
            For Each ent As SeisanInfoUtiwakeEntity In s05_0402Entity.SeisanInfoUtiwakeEntityList
                MyBase.paramClear()
                Dim insertSeisanInfoUtiwakeQuery As String = Me.createSqlInsert(Of SeisanInfoUtiwakeEntity)(CommonHakken.NameTblSeisanInfoUtiwake, ent)
                updateCount = MyBase.execNonQuery(oraTran, insertSeisanInfoUtiwakeQuery)
                If updateCount <= 0 Then
                    MyBase.callRollbackTransaction(oraTran)
                    Return False
                End If
            Next

            MyBase.callCommitTransaction(oraTran)

        Catch ex As Exception
            MyBase.callRollbackTransaction(oraTran)
            Throw

        Finally
            oraTran.Dispose()
        End Try

        Return True
    End Function

    ''' <summary>
    ''' パラメータの用意
    ''' </summary>
    ''' <param name="name">パラメータ名（重複不可）</param>
    ''' <param name="value">パラメータ</param>
    ''' <param name="entKoumoku">予約情報（基本）の項目</param>
    Private Function prepareParam(ByVal name As String,
                                           ByVal value As Object,
                                           ByVal entKoumoku As IEntityKoumokuType) As String

        Return MyBase.setParam(name,
                               value,
                               entKoumoku.DBType,
                               entKoumoku.IntegerBu,
                               entKoumoku.DecimalBu)
    End Function

#End Region

End Class