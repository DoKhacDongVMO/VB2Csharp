Imports System.Text

''' <summary>
''' 増発DAクラス
''' </summary>
Public Class Zouhatsu_DA
    Inherits DataAccessorBase

#Region " 定数／変数 "
    Public Enum accessType As Integer
        getPlaceMaster        ' 場所マスタ検索
        getCarNoMaster        ' 車番マスタ検索
        getSameCrsLedgerBasic ' コース台帳（基本）重複データ検索
        getGousya             ' 空き番検索
        getZasekiImageCnt     ' 座席イメージ作成済検索
    End Enum

    Private Const maruZou As String = "○増"
    Private Const ERRCNT As Integer = -1

    'コース台帳（基本）エンティティ
    Private clsCrsLedgerBasicEntity As New TCrsLedgerBasicEntity()

    ''' <summary>
    ''' 共通設定カラム名
    ''' </summary>
    ''' <remarks></remarks>
    Public NotInheritable Class CommonColName
        Public Const SystemEntryPgmid As String = "SYSTEM_ENTRY_PGMID"
        Public Const SystemEntryPersonCd As String = "SYSTEM_ENTRY_PERSON_CD"
        Public Const SystemEntryDay As String = "SYSTEM_ENTRY_DAY"
        Public Const SystemUpdatePgmid As String = "SYSTEM_UPDATE_PGMID"
        Public Const SystemUpdatePersonCd As String = "SYSTEM_UPDATE_PERSON_CD"
        Public Const SystemUpdateDay As String = "SYSTEM_UPDATE_DAY"
        Public Const DeleteDay As String = "DELETE_DAY"
        Public Const DeleteDate As String = "DELETE_DATE"
        Public Const Gousya As String = "GOUSYA"
    End Class

    ''' <summary>
    ''' パラメータキー
    ''' </summary>
    ''' <remarks></remarks>
    Public NotInheritable Class UpdateParamKeys
        ''' <summary>
        ''' 定員（定）
        ''' </summary>
        Public Const CAR_CAPACITY As String = "CAR_CAPACITY"
        ''' <summary>
        ''' 定員（補・１階）
        ''' </summary>
        Public Const CAR_EMG_CAPACITY As String = "CAR_EMG_CAPACITY"
        ''' <summary>
        ''' 車種コード
        ''' </summary>
        Public Const CAR_NO As String = "CAR_NO"
        ''' <summary>
        ''' 号車
        ''' </summary>
        Public Const GOUSYA As String = "GOUSYA"
        ''' <summary>
        ''' 車種(入力)
        ''' </summary>
        Public Const CAR_TYPE_CD As String = "CAR_TYPE_CD"
        ''' <summary>
        ''' 車種(基本)
        ''' </summary>
        Public Const CAR_TYPE_CD_BEF As String = "CAR_TYPE_CD_BEF"
        ''' <summary>
        ''' 配車経由地1
        ''' </summary>
        Public Const HAISYA_KEIYU_CD_1 As String = "HAISYA_KEIYU_CD_1"
        ''' <summary>
        ''' 配車経由地2
        ''' </summary>
        Public Const HAISYA_KEIYU_CD_2 As String = "HAISYA_KEIYU_CD_2"
        ''' <summary>
        ''' 配車経由地3
        ''' </summary>
        Public Const HAISYA_KEIYU_CD_3 As String = "HAISYA_KEIYU_CD_3"
        ''' <summary>
        ''' 配車経由地4
        ''' </summary>
        Public Const HAISYA_KEIYU_CD_4 As String = "HAISYA_KEIYU_CD_4"
        ''' <summary>
        ''' 配車経由地5
        ''' </summary>
        Public Const HAISYA_KEIYU_CD_5 As String = "HAISYA_KEIYU_CD_5"
        ''' <summary>
        ''' 出発日
        ''' </summary>
        Public Const SYUPT_DAY As String = "SYUPT_DAY"
        ''' <summary>
        ''' バス指定コード
        ''' </summary>
        Public Const BUS_RESERVE_CD As String = "BUS_RESERVE_CD"
        ''' <summary>
        ''' 出発時間１
        ''' </summary>
        Public Const SYUPT_TIME_1 As String = "SYUPT_TIME_1"
        ''' <summary>
        ''' 出発時間２
        ''' </summary>
        Public Const SYUPT_TIME_2 As String = "SYUPT_TIME_2"
        ''' <summary>
        ''' 出発時間３
        ''' </summary>
        Public Const SYUPT_TIME_3 As String = "SYUPT_TIME_3"
        ''' <summary>
        ''' 出発時間４
        ''' </summary>
        Public Const SYUPT_TIME_4 As String = "SYUPT_TIME_4"
        ''' <summary>
        ''' 出発時間５
        ''' </summary>
        Public Const SYUPT_TIME_5 As String = "SYUPT_TIME_5"
        ''' <summary>
        ''' 集合時間１
        ''' </summary>
        Public Const SYUGO_TIME_1 As String = "SYUGO_TIME_1"
        ''' <summary>
        ''' 集合時間２
        ''' </summary>
        Public Const SYUGO_TIME_2 As String = "SYUGO_TIME_2"
        ''' <summary>
        ''' 集合時間３
        ''' </summary>
        Public Const SYUGO_TIME_3 As String = "SYUGO_TIME_3"
        ''' <summary>
        ''' 集合時間４
        ''' </summary>
        Public Const SYUGO_TIME_4 As String = "SYUGO_TIME_4"
        ''' <summary>
        ''' 集合時間５
        ''' </summary>
        Public Const SYUGO_TIME_5 As String = "SYUGO_TIME_5"

    End Class

    ''' <summary>
    ''' DataSetテーブルNo
    ''' </summary>
    ''' <remarks></remarks>
    Public Enum crsLedgerTblId
        crsLeaderBasic = 0          'コース台帳（基本
        'コース台帳（降車ヶ所）
        crsLeaderKoshakasho
        'コース台帳（コース情報）
        crsLeaderCrsInfo
        'コース台帳（メッセージ）
        crsLeaderMessage
        'コース台帳（料金）
        crsLeaderCharge
        'コース台帳（ホテル）
        crsLeaderHotel
        'コース台帳（付加情報）
        crsLeaderAddInfo
        'コース台帳（バス紐づけ）
        crsLeaderBusHimoduke
        'コース台帳（オプション）
        crsLeaderOption
        'コース台帳（ダイヤ）
        crsLeaderDia
        'コース台帳（基本_料金区分）
        crsLeaderBasicChargeKbn
        'コース台帳（基本_料金区分）
        crsLeaderChargeChargeKbn
        'コース台帳（オプショングループ）
        crsLeaderOptionGroup
        'コース台帳（販売課所）
        crsLeaderKasho
        'コース台帳（リマークス）
        crsLeaderRemarks
        'コース台帳原価（基本）
        crsLeaderCostBasic
        'コース台帳原価（降車ヶ所）
        crsLeaderCostKoshakasho
        'コース台帳原価（キャリア）
        crsLeaderCostCarrier
        'コース台帳原価（プレート）
        crsLeaderCostPlate
        'コース台帳原価（キャリア_料金区分）
        crsLeaderCostCarrierChargeKbn
        'コース台帳原価（降車ヶ所_料金区分）
        crsLeaderCostKoshakashoChargeKbn
        '座席イメージ（バス情報）
        zasekiImage
        '座席イメージ（座席情報）
        zasekiImageInfo
        '座席番号マスタ
        zasekiNoMst
    End Enum

#End Region

#Region " SELECT処理 "

    ''' <summary>
    ''' SELECT用DBアクセス
    ''' </summary>
    ''' <param name="type"></param>
    ''' <param name="paramInfoList"></param>
    ''' <returns></returns>
    Public Function accessZouhatsuTehai(ByVal type As accessType, Optional ByVal paramInfoList As Hashtable = Nothing) As DataTable
        'SQL文字列
        Dim sqlString As String = String.Empty
        '戻り値
        Dim returnValue As DataTable = Nothing

        Select Case type
            Case accessType.getPlaceMaster
                ' 場所マスタ検索
                sqlString = GetPlaceMaster(paramInfoList)
            Case accessType.getCarNoMaster
                ' 車番マスタ検索
                sqlString = GetCarNoMaster(paramInfoList)
            Case accessType.getSameCrsLedgerBasic
                ' コース台帳（基本）重複データ検索
                sqlString = getSameCrsLedgerBasic(paramInfoList)
            Case accessType.getGousya
                ' 空き番検索
                sqlString = getGousya(paramInfoList)
            Case accessType.getZasekiImageCnt
                '座席イメージ作成済チェック
                sqlString = getZasekiImageCnt(paramInfoList)
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
    ''' 場所マスタ検索
    ''' </summary>
    ''' <param name="paramInfoList"></param>
    Public Function GetPlaceMaster(paramInfoList As Hashtable) As String

        Dim resultDataTable As New DataTable
        Dim sqlString As New StringBuilder

        '空レコード挿入要否に従い、空行挿入
        If CType(paramInfoList.Item("NullRecord"), Boolean) = True Then
            sqlString.AppendLine(" SELECT")
            sqlString.AppendLine(" ' ' AS CODE_VALUE ")
            sqlString.AppendLine(",'' AS CODE_NAME ")
            'FROM句
            sqlString.AppendLine(" FROM ")
            sqlString.AppendLine("DUAL UNION ")
        End If
        sqlString.AppendLine(" SELECT")
        sqlString.AppendLine(" RTRIM(PLACE_CD) AS CODE_VALUE ")
        sqlString.AppendLine(",PLACE_NAME_1 AS CODE_NAME ")
        'FROM句
        sqlString.AppendLine(" FROM ")
        sqlString.AppendLine("M_PLACE ")
        'WHERE句
        sqlString.AppendLine(" WHERE NVL(DELETE_DATE,0) = 0 ")
        'ORDER BY句
        sqlString.AppendLine(" ORDER BY ")
        sqlString.AppendLine(" CODE_VALUE ASC ")

        Return sqlString.ToString
    End Function

    ''' <summary>
    ''' 車番マスタ検索
    ''' </summary>
    ''' <param name="paramInfoList"></param>
    Public Function GetCarNoMaster(paramInfoList As Hashtable) As String

        Dim resultDataTable As New DataTable
        Dim sqlString As New StringBuilder

        sqlString.AppendLine(" SELECT")
        sqlString.AppendLine("   CN.CAR_TYPE_CD ")         ' 車番マスタ.車種コード
        sqlString.AppendLine(" , CK.CAR_CAPACITY ")        ' 車種マスタ.定員（定）
        sqlString.AppendLine(" , CK.CAR_EMG_CAPACITY ")    ' 車種マスタ.定員（補・１階）
        sqlString.AppendLine(" , ZN.ZASEKI_KAI ")          ' 座席番号マスタ.座席階
        sqlString.AppendLine(" , ZN.ZASEKI_LINE ")         ' 座席番号マスタ.座席行
        sqlString.AppendLine(" , ZN.ZASEKI_COL ")          ' 座席番号マスタ.座席列
        sqlString.AppendLine(" , ZN.ZASEKI_KIND ")         ' 座席番号マスタ.座席種別

        'FROM句
        sqlString.AppendLine(" FROM ")
        sqlString.AppendLine("M_CAR_NO CN ")                                                          ' 車番マスタ
        sqlString.AppendLine("INNER JOIN M_CAR_KIND CK ON TRIM(CN.CAR_TYPE_CD) = TRIM(CK.CAR_CD) ")    ' 車種マスタ
        sqlString.AppendLine("LEFT JOIN M_ZASEKI_NO ZN ON TRIM(CK.CAR_CD) = TRIM(ZN.CAR_CD) ")        ' 座席番号マスタ

        'WHERE句
        sqlString.AppendLine(" WHERE CN.APPLICATION_DAY_FROM <= " & setParam("SyuptDay", paramInfoList.Item("SyuptDay"), OracleDbType.Decimal, 8, 0))
        sqlString.AppendLine(" AND NVL(CN.APPLICATION_DAY_TO,99991231) >= " & setParam("SyuptDay", paramInfoList.Item("SyuptDay"), OracleDbType.Decimal, 8, 0))
        sqlString.AppendLine(" AND CN.CAR_NO = " & setParam("CarNo", paramInfoList.Item("CarNo"), OracleDbType.Decimal, 3, 0))
        sqlString.AppendLine(" AND NVL(CN.DELETE_DAY,0) = 0 ")
        sqlString.AppendLine(" AND NVL(CK.DELETE_DATE,0) = 0 ")

        Return sqlString.ToString
    End Function

    ''' <summary>
    ''' コース台帳（基本）重複データ検索
    ''' </summary>
    ''' <param name="paramList"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Protected Overloads Function getSameCrsLedgerBasic(ByVal paramList As Hashtable) As String

        Dim sqlString As New StringBuilder

        sqlString.AppendLine(" SELECT")
        sqlString.AppendLine(" COUNT(*) AS COUNT ")

        ' FROM句
        sqlString.AppendLine(" FROM ")
        sqlString.AppendLine("T_CRS_LEDGER_BASIC ") ' コース台帳（基本）

        ' WHERE句
        sqlString.AppendLine(" WHERE SYUPT_DAY = " & setParam("SyuptDay", paramList.Item("SyuptDay"), OracleDbType.Decimal, 8, 0))
        sqlString.AppendLine(" AND CRS_CD = " & setParam("CrsCd", paramList.Item("CrsCd"), OracleDbType.Char))
        sqlString.AppendLine(" AND GOUSYA = " & setParam("Gousya", paramList.Item("Gousya"), OracleDbType.Decimal, 3, 0))
        'sqlString.AppendLine(" AND NVL(DELETE_DAY,0) = 0 ")

        Return sqlString.ToString
    End Function

    ''' <summary>
    ''' 空き番検索
    ''' </summary>
    ''' <param name="paramList"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Protected Overloads Function getGousya(ByVal paramList As Hashtable) As String

        Dim sqlString As New StringBuilder

        sqlString.AppendLine(" SELECT")
        sqlString.AppendLine(" MIN(t1.GOUSYA) + 1 AS GOUSYA ")

        ' FROM句
        sqlString.AppendLine(" FROM ")
        sqlString.AppendLine("T_CRS_LEDGER_BASIC t1 ") ' コース台帳（基本）t1

        ' WHERE句
        sqlString.AppendLine(" WHERE (t1.GOUSYA +1) NOT IN ( ")
        sqlString.AppendLine("   SELECT")
        sqlString.AppendLine("     t2.GOUSYA ")
        sqlString.AppendLine("   FROM ")
        sqlString.AppendLine("     T_CRS_LEDGER_BASIC t2 ") ' コース台帳（基本）t2
        sqlString.AppendLine("   WHERE t2.SYUPT_DAY = " & setParam("SyuptDay", paramList.Item("SyuptDay"), OracleDbType.Decimal, 8, 0))
        sqlString.AppendLine("   AND t2.CRS_CD = " & setParam("CrsCd", paramList.Item("CrsCd"), OracleDbType.Char))
        'sqlString.AppendLine("   AND NVL(t2.DELETE_DAY,0) = 0")
        sqlString.AppendLine(" ) ")
        sqlString.AppendLine(" AND t1.GOUSYA >= " & setParam("Gousya", paramList.Item("Gousya"), OracleDbType.Decimal, 3, 0))

        Return sqlString.ToString
    End Function

    ''' <summary>
    ''' 座席イメージ作成済チェックデータ取得
    ''' </summary>
    ''' <param name="paramList"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Protected Overloads Function getZasekiImageCnt(ByVal paramList As Hashtable) As String

        Dim sqlString As New StringBuilder

        sqlString.AppendLine(" SELECT")
        sqlString.AppendLine(" COUNT(*) AS COUNT ")

        ' FROM句
        sqlString.AppendLine(" FROM ")
        sqlString.AppendLine("T_ZASEKI_IMAGE ") ' 座席イメージ（バス情報）

        ' WHERE句
        sqlString.AppendLine(" WHERE SYUPT_DAY = " & setParam("SyuptDay", paramList.Item("SyuptDay"), OracleDbType.Decimal, 8, 0))
        sqlString.AppendLine(" AND BUS_RESERVE_CD = " & setParam("BusReserveCd", paramList.Item("BusReserveCd"), OracleDbType.Char))
        sqlString.AppendLine(" AND GOUSYA = " & setParam("Gousya", paramList.Item("Gousya"), OracleDbType.Decimal, 3, 0))
        'sqlString.AppendLine(" AND NVL(DELETE_DAY,0) = 0 ")

        Return sqlString.ToString
    End Function


    ''' <summary>
    ''' 増発元データ取得処理
    ''' </summary>
    ''' <param name="paramInfoList">パラメータリスト</param>
    ''' <returns>DataSet</returns>
    ''' <remarks></remarks>
    Public Function getZouhatsutOrigData(ByVal paramInfoList As Hashtable) As DataSet
        Dim con As OracleConnection = Nothing
        Dim ds As DataSet = New DataSet()
        Dim dtAfStayYoyakuSgl As DataTable = New DataTable()
        Dim dtAfStayYoyakuTwin As DataTable = New DataTable()

        'ログ出力用
        Dim logmsg(3) As String

        Try
            'コネクション作成
            con = openCon()

            ' 増発元データ取得(コース台帳（基本）)
            ds.Tables.Add(getOrigCrsLedgerBasic(paramInfoList, con))
            ' 増発元データ取得(コース台帳（降車ヶ所）)
            ds.Tables.Add(getOrigCrsLedgerKoshakasho(paramInfoList, con))
            ' 増発元データ取得(コース台帳（コース情報）)
            ds.Tables.Add(getOrigCrsLedgerCrsInfo(paramInfoList, con))
            ' 増発元データ取得(コース台帳（メッセージ）)
            ds.Tables.Add(getOrigCrsLedgerMessage(paramInfoList, con))
            ' 増発元データ取得(コース台帳（料金）)
            ds.Tables.Add(getOrigCrsLedgerCharge(paramInfoList, con))
            ' 増発元データ取得(コース台帳（ホテル）)
            ds.Tables.Add(getOrigCrsLedgerHotel(paramInfoList, con))
            ' 増発元データ取得(コース台帳（付加情報）)
            ds.Tables.Add(getOrigCrsLedgerAddInfo(paramInfoList, con))
            ' 増発元データ取得(コース台帳（バス紐づけ）)
            ds.Tables.Add(getOrigCrsLedgerBusHimoduke(paramInfoList, con))
            ' 増発元データ取得(コース台帳（オプション）)
            ds.Tables.Add(getOrigCrsLedgerOption(paramInfoList, con))
            ' 増発元データ取得(コース台帳（ダイヤ）)
            ds.Tables.Add(getOrigCrsLedgerDia(paramInfoList, con))
            ' 増発元データ取得(コース台帳（基本_料金区分）)
            ds.Tables.Add(getOrigCrsLedgerBasicChargeKbn(paramInfoList, con))
            ' 増発元データ取得(コース台帳（料金_料金区分）)
            ds.Tables.Add(getOrigCrsLedgerChargeChargeKbn(paramInfoList, con))
            ' 増発元データ取得(コース台帳（オプショングループ）)
            ds.Tables.Add(getOrigCrsLedgerOptionGroup(paramInfoList, con))
            ' 増発元データ取得(コース台帳（販売課所）)
            ds.Tables.Add(getOrigCrsLedgerKasho(paramInfoList, con))
            ' 増発元データ取得(コース台帳（リマークス）)
            ds.Tables.Add(getOrigCrsLedgerRemarks(paramInfoList, con))
            ' 増発元データ取得(コース台帳原価（基本）)
            ds.Tables.Add(getOrigCrsLedgerCostBasic(paramInfoList, con))
            ' 増発元データ取得(コース台帳原価（降車ヶ所）)
            ds.Tables.Add(getOrigCrsLedgerCostKoshakasho(paramInfoList, con))
            ' 増発元データ取得(コース台帳原価（キャリア）)
            ds.Tables.Add(getOrigCrsLedgerCostCarrier(paramInfoList, con))
            ' 増発元データ取得(コース台帳原価（プレート）)
            ds.Tables.Add(getOrigCrsLedgerCostPlate(paramInfoList, con))
            ' 増発元データ取得(コース台帳原価（キャリア_料金区分）)
            ds.Tables.Add(getOrigCrsLedgerCostCarrierChargeKbn(paramInfoList, con))
            ' 増発元データ取得(コース台帳原価（降車ヶ所_料金区分）)
            ds.Tables.Add(getOrigCrsLedgerCostKoshakashoChargeKbn(paramInfoList, con))
            ' 増発元データ取得(座席イメージ（バス情報）)
            ds.Tables.Add(getOrigZasekiImage(paramInfoList, con))
            ' 増発元データ取得(座席イメージ（座席情報）)
            ds.Tables.Add(getOrigZasekiImageInfo(paramInfoList, con))
            ' 座席番号マスタ
            ds.Tables.Add(getZasekiMst(paramInfoList, con))

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

        Return ds

    End Function

#Region "増発元取得(23TBL)"
    ''' <summary>
    ''' 増発元データ取得(コース台帳（基本）)
    ''' </summary>
    ''' <param name="paramList">検索用パラメータ</param>
    ''' <param name="connection">Oracleコネクション</param>
    ''' <returns>DataTable</returns>
    ''' <remarks></remarks>
    Public Function getOrigCrsLedgerBasic(ByVal paramList As Hashtable, connection As OracleConnection) As DataTable

        Dim resultDataTable As New DataTable
        Dim sqlColString As New StringBuilder
        Dim CrsMasterEntity As EntityOperation(Of TCrsLedgerBasicEntity) = New EntityOperation(Of TCrsLedgerBasicEntity)

        Try
            'Entityから項目情報生成
            For idxItem As Integer = 0 To CrsMasterEntity.getPropertyDataLength - 1
                If idxItem > 0 Then
                    sqlColString.Append(",")
                End If
                With CrsMasterEntity
                    If .getPtyType(idxItem, .EntityData(0)) Is GetType(EntityKoumoku_YmdType) Then
                        sqlColString.AppendLine(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_YmdType).PhysicsName)
                    ElseIf .getPtyType(idxItem, .EntityData(0)) Is GetType(EntityKoumoku_NumberType) Then
                        sqlColString.AppendLine("COALESCE (" & DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_NumberType).PhysicsName & ", 0) AS " & DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_NumberType).PhysicsName)
                    ElseIf .getPtyType(idxItem, .EntityData(0)) Is GetType(EntityKoumoku_Number_DecimalType) Then
                        sqlColString.AppendLine("COALESCE (" & DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_Number_DecimalType).PhysicsName & ", 0) AS " & DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_Number_DecimalType).PhysicsName)
                    Else
                        sqlColString.AppendLine(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_MojiType).PhysicsName)
                    End If
                End With
            Next
            resultDataTable = getCrsLeaderTableData(paramList, connection, sqlColString.ToString, "T_CRS_LEDGER_BASIC")
        Catch ex As Exception
            Throw
        End Try

        Return resultDataTable
    End Function

    Public Function getCrsLeaderTableData(ByVal paramList As Hashtable, connection As OracleConnection, ByVal paramColInfo As String, ByVal paramTblId As String, Optional ByVal paramDeleteColId As String = "DELETE_DAY") As DataTable
        Dim resultDataTable As New DataTable
        Dim sqlString As New StringBuilder

        With clsCrsLedgerBasicEntity
            'パラメータクリア
            paramClear()

            'パラメータ設定
            ' 出発日
            setParam(.syuptDay.PhysicsName, paramList.Item(.syuptDay.PhysicsName), .syuptDay.DBType, .syuptDay.IntegerBu, .syuptDay.DecimalBu)
            ' コースコード
            setParam(.crsCd.PhysicsName, paramList.Item(.crsCd.PhysicsName), .crsCd.DBType, .crsCd.IntegerBu, .crsCd.DecimalBu)
            ' 号車
            setParam(.gousya.PhysicsName, paramList.Item(.gousya.PhysicsName), .gousya.DBType, .gousya.IntegerBu, .gousya.DecimalBu)

            sqlString.AppendLine(" SELECT")
            sqlString.Append(paramColInfo)
            ' FROM句
            sqlString.AppendLine(" FROM ")
            sqlString.AppendLine(paramTblId)

            ' WHERE句
            sqlString.AppendLine(" WHERE SYUPT_DAY = :" & .syuptDay.PhysicsName)
            sqlString.AppendLine(" AND CRS_CD = :" & .crsCd.PhysicsName)
            sqlString.AppendLine(" AND GOUSYA = :" & .gousya.PhysicsName)
            sqlString.AppendLine(" AND NVL(").Append(paramDeleteColId).Append(",0) = 0 ")

            'If paramTblId.Equals("T_CRS_LEDGER_BASIC") Then
            '    sqlString.AppendLine(" AND CRS_CD = BUS_RESERVE_CD")
            'End If

        End With

        resultDataTable = MyBase.getDataTable(connection, sqlString.ToString())

        Return resultDataTable
    End Function

    Public Function getZasekiTableData(ByVal paramList As Hashtable, connection As OracleConnection, ByVal paramColInfo As String, ByVal paramTblId As String, Optional ByVal paramDeleteColId As String = "DELETE_DAY") As DataTable
        Dim resultDataTable As New DataTable
        Dim sqlString As New StringBuilder

        With clsCrsLedgerBasicEntity
            'パラメータクリア
            paramClear()

            'パラメータ設定
            ' 出発日
            setParam(.syuptDay.PhysicsName, paramList.Item(.syuptDay.PhysicsName), .syuptDay.DBType, .syuptDay.IntegerBu, .syuptDay.DecimalBu)
            ' コースコード
            setParam(.crsCd.PhysicsName, paramList.Item(.crsCd.PhysicsName), .crsCd.DBType, .crsCd.IntegerBu, .crsCd.DecimalBu)
            ' 号車
            setParam(.gousya.PhysicsName, paramList.Item(.gousya.PhysicsName), .gousya.DBType, .gousya.IntegerBu, .gousya.DecimalBu)
            ' バス指定コード
            setParam(.busReserveCd.PhysicsName, paramList.Item(.busReserveCd.PhysicsName), .busReserveCd.DBType, .busReserveCd.IntegerBu, .busReserveCd.DecimalBu)

            sqlString.AppendLine(" SELECT")
            sqlString.Append(paramColInfo)
            ' FROM句
            sqlString.AppendLine(" FROM ")
            sqlString.AppendLine(paramTblId)

            ' WHERE句
            sqlString.AppendLine(" WHERE SYUPT_DAY = :" & .syuptDay.PhysicsName)
            sqlString.AppendLine(" AND BUS_RESERVE_CD = :" & .busReserveCd.PhysicsName)
            sqlString.AppendLine(" AND GOUSYA = :" & .gousya.PhysicsName)

            sqlString.AppendLine(" AND NVL(").Append(paramDeleteColId).Append(",0) = 0 ")
        End With

        resultDataTable = MyBase.getDataTable(connection, sqlString.ToString())

        Return resultDataTable
    End Function

    ''' <summary>
    ''' 増発元データ取得(コース台帳（降車ヶ所）)
    ''' </summary>
    ''' <param name="paramList">検索用パラメータ</param>
    ''' <param name="connection">Oracleコネクション</param>
    ''' <returns>DataTable</returns>
    ''' <remarks></remarks>
    Public Function getOrigCrsLedgerKoshakasho(ByVal paramList As Hashtable, connection As OracleConnection) As DataTable

        Dim resultDataTable As New DataTable
        Dim sqlColString As New StringBuilder
        Dim CrsMasterEntity As EntityOperation(Of TCrsLedgerKoshakashoEntity) = New EntityOperation(Of TCrsLedgerKoshakashoEntity)

        Try
            'Entityから項目情報生成
            For idxItem As Integer = 0 To CrsMasterEntity.getPropertyDataLength - 1
                If idxItem > 0 Then
                    sqlColString.Append(",")
                End If
                With CrsMasterEntity
                    If .getPtyType(idxItem, .EntityData(0)) Is GetType(EntityKoumoku_YmdType) Then
                        sqlColString.AppendLine(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_YmdType).PhysicsName)
                    ElseIf .getPtyType(idxItem, .EntityData(0)) Is GetType(EntityKoumoku_NumberType) Then
                        sqlColString.AppendLine("COALESCE (" & DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_NumberType).PhysicsName & ", 0) AS " & DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_NumberType).PhysicsName)
                    ElseIf .getPtyType(idxItem, .EntityData(0)) Is GetType(EntityKoumoku_Number_DecimalType) Then
                        sqlColString.AppendLine("COALESCE (" & DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_Number_DecimalType).PhysicsName & ", 0) AS " & DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_Number_DecimalType).PhysicsName)
                    Else
                        sqlColString.AppendLine(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_MojiType).PhysicsName)
                    End If
                End With
            Next

            resultDataTable = getCrsLeaderTableData(paramList, connection, sqlColString.ToString, "T_CRS_LEDGER_KOSHAKASHO")
        Catch ex As Exception
            Throw
        End Try

        Return resultDataTable
    End Function

    ''' <summary>
    ''' 増発元データ取得(コース台帳（コース情報）)
    ''' </summary>
    ''' <param name="paramList">検索用パラメータ</param>
    ''' <param name="connection">Oracleコネクション</param>
    ''' <returns>DataTable</returns>
    ''' <remarks></remarks>
    Public Function getOrigCrsLedgerCrsInfo(ByVal paramList As Hashtable, connection As OracleConnection) As DataTable

        Dim resultDataTable As New DataTable
        Dim sqlColString As New StringBuilder
        Dim CrsMasterEntity As EntityOperation(Of TCrsLedgerCrsInfoEntity) = New EntityOperation(Of TCrsLedgerCrsInfoEntity)

        Try
            'Entityから項目情報生成
            For idxItem As Integer = 0 To CrsMasterEntity.getPropertyDataLength - 1
                If idxItem > 0 Then
                    sqlColString.Append(",")
                End If
                With CrsMasterEntity
                    If .getPtyType(idxItem, .EntityData(0)) Is GetType(EntityKoumoku_YmdType) Then
                        sqlColString.AppendLine(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_YmdType).PhysicsName)
                    ElseIf .getPtyType(idxItem, .EntityData(0)) Is GetType(EntityKoumoku_NumberType) Then
                        sqlColString.AppendLine("COALESCE (" & DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_NumberType).PhysicsName & ", 0) AS " & DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_NumberType).PhysicsName)
                    ElseIf .getPtyType(idxItem, .EntityData(0)) Is GetType(EntityKoumoku_Number_DecimalType) Then
                        sqlColString.AppendLine("COALESCE (" & DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_Number_DecimalType).PhysicsName & ", 0) AS " & DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_Number_DecimalType).PhysicsName)
                    Else
                        sqlColString.AppendLine(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_MojiType).PhysicsName)
                    End If
                End With
            Next

            resultDataTable = getCrsLeaderTableData(paramList, connection, sqlColString.ToString, "T_CRS_LEDGER_CRS_INFO")
        Catch ex As Exception
            Throw
        End Try

        Return resultDataTable
    End Function

    ''' <summary>
    ''' 増発元データ取得(コース台帳（メッセージ）)
    ''' </summary>
    ''' <param name="paramList">検索用パラメータ</param>
    ''' <param name="connection">Oracleコネクション</param>
    ''' <returns>DataTable</returns>
    ''' <remarks></remarks>
    Public Function getOrigCrsLedgerMessage(ByVal paramList As Hashtable, connection As OracleConnection) As DataTable

        Dim resultDataTable As New DataTable
        Dim sqlColString As New StringBuilder
        Dim CrsMasterEntity As EntityOperation(Of TCrsLedgerMessageEntity) = New EntityOperation(Of TCrsLedgerMessageEntity)

        Try
            'Entityから項目情報生成
            For idxItem As Integer = 0 To CrsMasterEntity.getPropertyDataLength - 1
                If idxItem > 0 Then
                    sqlColString.Append(",")
                End If
                With CrsMasterEntity
                    If .getPtyType(idxItem, .EntityData(0)) Is GetType(EntityKoumoku_YmdType) Then
                        sqlColString.AppendLine(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_YmdType).PhysicsName)
                    ElseIf .getPtyType(idxItem, .EntityData(0)) Is GetType(EntityKoumoku_NumberType) Then
                        sqlColString.AppendLine("COALESCE (" & DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_NumberType).PhysicsName & ", 0) AS " & DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_NumberType).PhysicsName)
                    ElseIf .getPtyType(idxItem, .EntityData(0)) Is GetType(EntityKoumoku_Number_DecimalType) Then
                        sqlColString.AppendLine("COALESCE (" & DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_Number_DecimalType).PhysicsName & ", 0) AS " & DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_Number_DecimalType).PhysicsName)
                    Else
                        sqlColString.AppendLine(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_MojiType).PhysicsName)
                    End If
                End With
            Next

            resultDataTable = getCrsLeaderTableData(paramList, connection, sqlColString.ToString, "T_CRS_LEDGER_MESSAGE")
        Catch ex As Exception
            Throw
        End Try

        Return resultDataTable
    End Function

    ''' <summary>
    ''' 増発元データ取得(コース台帳（料金）)
    ''' </summary>
    ''' <param name="paramList">検索用パラメータ</param>
    ''' <param name="connection">Oracleコネクション</param>
    ''' <returns>DataTable</returns>
    ''' <remarks></remarks>
    Public Function getOrigCrsLedgerCharge(ByVal paramList As Hashtable, connection As OracleConnection) As DataTable

        Dim resultDataTable As New DataTable
        Dim sqlColString As New StringBuilder
        Dim CrsMasterEntity As EntityOperation(Of TCrsLedgerChargeEntity) = New EntityOperation(Of TCrsLedgerChargeEntity)

        Try
            'Entityから項目情報生成
            For idxItem As Integer = 0 To CrsMasterEntity.getPropertyDataLength - 1
                If idxItem > 0 Then
                    sqlColString.Append(",")
                End If
                With CrsMasterEntity
                    If .getPtyType(idxItem, .EntityData(0)) Is GetType(EntityKoumoku_YmdType) Then
                        sqlColString.AppendLine(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_YmdType).PhysicsName)
                    ElseIf .getPtyType(idxItem, .EntityData(0)) Is GetType(EntityKoumoku_NumberType) Then
                        sqlColString.AppendLine("COALESCE (" & DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_NumberType).PhysicsName & ", 0) AS " & DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_NumberType).PhysicsName)
                    ElseIf .getPtyType(idxItem, .EntityData(0)) Is GetType(EntityKoumoku_Number_DecimalType) Then
                        sqlColString.AppendLine("COALESCE (" & DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_Number_DecimalType).PhysicsName & ", 0) AS " & DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_Number_DecimalType).PhysicsName)
                    Else
                        sqlColString.AppendLine(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_MojiType).PhysicsName)
                    End If
                End With
            Next

            resultDataTable = getCrsLeaderTableData(paramList, connection, sqlColString.ToString, "T_CRS_LEDGER_CHARGE")
        Catch ex As Exception
            Throw
        End Try

        Return resultDataTable
    End Function

    ''' <summary>
    ''' 増発元データ取得(コース台帳（ホテル）)
    ''' </summary>
    ''' <param name="paramList">検索用パラメータ</param>
    ''' <param name="connection">Oracleコネクション</param>
    ''' <returns>DataTable</returns>
    ''' <remarks></remarks>
    Public Function getOrigCrsLedgerHotel(ByVal paramList As Hashtable, connection As OracleConnection) As DataTable

        Dim resultDataTable As New DataTable
        Dim sqlColString As New StringBuilder
        Dim CrsMasterEntity As EntityOperation(Of TCrsLedgerHotelEntity) = New EntityOperation(Of TCrsLedgerHotelEntity)

        Try
            'Entityから項目情報生成
            For idxItem As Integer = 0 To CrsMasterEntity.getPropertyDataLength - 1
                If idxItem > 0 Then
                    sqlColString.Append(",")
                End If
                With CrsMasterEntity
                    If .getPtyType(idxItem, .EntityData(0)) Is GetType(EntityKoumoku_YmdType) Then
                        sqlColString.AppendLine(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_YmdType).PhysicsName)
                    ElseIf .getPtyType(idxItem, .EntityData(0)) Is GetType(EntityKoumoku_NumberType) Then
                        sqlColString.AppendLine("COALESCE (" & DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_NumberType).PhysicsName & ", 0) AS " & DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_NumberType).PhysicsName)
                    ElseIf .getPtyType(idxItem, .EntityData(0)) Is GetType(EntityKoumoku_Number_DecimalType) Then
                        sqlColString.AppendLine("COALESCE (" & DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_Number_DecimalType).PhysicsName & ", 0) AS " & DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_Number_DecimalType).PhysicsName)
                    Else
                        sqlColString.AppendLine(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_MojiType).PhysicsName)
                    End If
                End With
            Next

            resultDataTable = getCrsLeaderTableData(paramList, connection, sqlColString.ToString, "T_CRS_LEDGER_HOTEL")
        Catch ex As Exception
            Throw
        End Try

        Return resultDataTable
    End Function

    ''' <summary>
    ''' 増発元データ取得(コース台帳（付加情報）)
    ''' </summary>
    ''' <param name="paramList">検索用パラメータ</param>
    ''' <param name="connection">Oracleコネクション</param>
    ''' <returns>DataTable</returns>
    ''' <remarks></remarks>
    Public Function getOrigCrsLedgerAddInfo(ByVal paramList As Hashtable, connection As OracleConnection) As DataTable

        Dim resultDataTable As New DataTable
        Dim sqlColString As New StringBuilder
        Dim CrsMasterEntity As EntityOperation(Of TCrsLedgerAddInfoEntity) = New EntityOperation(Of TCrsLedgerAddInfoEntity)

        Try
            'Entityから項目情報生成
            For idxItem As Integer = 0 To CrsMasterEntity.getPropertyDataLength - 1
                If idxItem > 0 Then
                    sqlColString.Append(",")
                End If
                With CrsMasterEntity
                    If .getPtyType(idxItem, .EntityData(0)) Is GetType(EntityKoumoku_YmdType) Then
                        sqlColString.AppendLine(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_YmdType).PhysicsName)
                    ElseIf .getPtyType(idxItem, .EntityData(0)) Is GetType(EntityKoumoku_NumberType) Then
                        sqlColString.AppendLine("COALESCE (" & DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_NumberType).PhysicsName & ", 0) AS " & DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_NumberType).PhysicsName)
                    ElseIf .getPtyType(idxItem, .EntityData(0)) Is GetType(EntityKoumoku_Number_DecimalType) Then
                        sqlColString.AppendLine("COALESCE (" & DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_Number_DecimalType).PhysicsName & ", 0) AS " & DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_Number_DecimalType).PhysicsName)
                    Else
                        sqlColString.AppendLine(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_MojiType).PhysicsName)
                    End If
                End With
            Next

            resultDataTable = getCrsLeaderTableData(paramList, connection, sqlColString.ToString, "T_CRS_LEDGER_ADD_INFO")
        Catch ex As Exception
            Throw
        End Try

        Return resultDataTable
    End Function

    ''' <summary>
    ''' 増発元データ取得(コース台帳（バス紐づけ）)
    ''' </summary>
    ''' <param name="paramList">検索用パラメータ</param>
    ''' <param name="connection">Oracleコネクション</param>
    ''' <returns>DataTable</returns>
    ''' <remarks></remarks>
    Public Function getOrigCrsLedgerBusHimoduke(ByVal paramList As Hashtable, connection As OracleConnection) As DataTable

        Dim resultDataTable As New DataTable
        Dim sqlColString As New StringBuilder
        Dim CrsMasterEntity As EntityOperation(Of TCrsLedgerBusHimodukeEntity) = New EntityOperation(Of TCrsLedgerBusHimodukeEntity)

        Try
            'Entityから項目情報生成
            For idxItem As Integer = 0 To CrsMasterEntity.getPropertyDataLength - 1
                If idxItem > 0 Then
                    sqlColString.Append(",")
                End If
                With CrsMasterEntity
                    If .getPtyType(idxItem, .EntityData(0)) Is GetType(EntityKoumoku_YmdType) Then
                        sqlColString.AppendLine(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_YmdType).PhysicsName)
                    ElseIf .getPtyType(idxItem, .EntityData(0)) Is GetType(EntityKoumoku_NumberType) Then
                        sqlColString.AppendLine("COALESCE (" & DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_NumberType).PhysicsName & ", 0) AS " & DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_NumberType).PhysicsName)
                    ElseIf .getPtyType(idxItem, .EntityData(0)) Is GetType(EntityKoumoku_Number_DecimalType) Then
                        sqlColString.AppendLine("COALESCE (" & DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_Number_DecimalType).PhysicsName & ", 0) AS " & DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_Number_DecimalType).PhysicsName)
                    Else
                        sqlColString.AppendLine(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_MojiType).PhysicsName)
                    End If
                End With
            Next

            resultDataTable = getCrsLeaderTableData(paramList, connection, sqlColString.ToString, "T_CRS_LEDGER_BUS_HIMODUKE", CrsMasterEntity.EntityData(0).deleteDate.PhysicsName)
        Catch ex As Exception
            Throw
        End Try

        Return resultDataTable
    End Function

    ''' <summary>
    ''' 増発元データ取得(コース台帳（オプション）)
    ''' </summary>
    ''' <param name="paramList">検索用パラメータ</param>
    ''' <param name="connection">Oracleコネクション</param>
    ''' <returns>DataTable</returns>
    ''' <remarks></remarks>
    Public Function getOrigCrsLedgerOption(ByVal paramList As Hashtable, connection As OracleConnection) As DataTable

        Dim resultDataTable As New DataTable
        Dim sqlColString As New StringBuilder
        Dim CrsMasterEntity As EntityOperation(Of TCrsLedgerOptionEntity) = New EntityOperation(Of TCrsLedgerOptionEntity)

        Try
            'Entityから項目情報生成
            For idxItem As Integer = 0 To CrsMasterEntity.getPropertyDataLength - 1
                If idxItem > 0 Then
                    sqlColString.Append(",")
                End If
                With CrsMasterEntity
                    If .getPtyType(idxItem, .EntityData(0)) Is GetType(EntityKoumoku_YmdType) Then
                        sqlColString.AppendLine(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_YmdType).PhysicsName)
                    ElseIf .getPtyType(idxItem, .EntityData(0)) Is GetType(EntityKoumoku_NumberType) Then
                        sqlColString.AppendLine("COALESCE (" & DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_NumberType).PhysicsName & ", 0) AS " & DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_NumberType).PhysicsName)
                    ElseIf .getPtyType(idxItem, .EntityData(0)) Is GetType(EntityKoumoku_Number_DecimalType) Then
                        sqlColString.AppendLine("COALESCE (" & DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_Number_DecimalType).PhysicsName & ", 0) AS " & DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_Number_DecimalType).PhysicsName)
                    Else
                        sqlColString.AppendLine(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_MojiType).PhysicsName)
                    End If
                End With
            Next

            resultDataTable = getCrsLeaderTableData(paramList, connection, sqlColString.ToString, "T_CRS_LEDGER_OPTION")
        Catch ex As Exception
            Throw
        End Try

        Return resultDataTable
    End Function

    ''' <summary>
    ''' 増発元データ取得(コース台帳（ダイヤ）)
    ''' </summary>
    ''' <param name="paramList">検索用パラメータ</param>
    ''' <param name="connection">Oracleコネクション</param>
    ''' <returns>DataTable</returns>
    ''' <remarks></remarks>
    Public Function getOrigCrsLedgerDia(ByVal paramList As Hashtable, connection As OracleConnection) As DataTable

        Dim resultDataTable As New DataTable
        Dim sqlColString As New StringBuilder
        Dim CrsMasterEntity As EntityOperation(Of TCrsLedgerDiaEntity) = New EntityOperation(Of TCrsLedgerDiaEntity)

        Try
            'Entityから項目情報生成
            For idxItem As Integer = 0 To CrsMasterEntity.getPropertyDataLength - 1
                If idxItem > 0 Then
                    sqlColString.Append(",")
                End If
                With CrsMasterEntity
                    If .getPtyType(idxItem, .EntityData(0)) Is GetType(EntityKoumoku_YmdType) Then
                        sqlColString.AppendLine(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_YmdType).PhysicsName)
                    ElseIf .getPtyType(idxItem, .EntityData(0)) Is GetType(EntityKoumoku_NumberType) Then
                        sqlColString.AppendLine("COALESCE (" & DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_NumberType).PhysicsName & ", 0) AS " & DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_NumberType).PhysicsName)
                    ElseIf .getPtyType(idxItem, .EntityData(0)) Is GetType(EntityKoumoku_Number_DecimalType) Then
                        sqlColString.AppendLine("COALESCE (" & DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_Number_DecimalType).PhysicsName & ", 0) AS " & DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_Number_DecimalType).PhysicsName)
                    Else
                        sqlColString.AppendLine(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_MojiType).PhysicsName)
                    End If
                End With
            Next

            resultDataTable = getCrsLeaderTableData(paramList, connection, sqlColString.ToString, "T_CRS_LEDGER_DIA")
        Catch ex As Exception
            Throw
        End Try

        Return resultDataTable
    End Function

    ''' <summary>
    ''' 増発元データ取得(コース台帳（基本_料金区分）)
    ''' </summary>
    ''' <param name="paramList">検索用パラメータ</param>
    ''' <param name="connection">Oracleコネクション</param>
    ''' <returns>DataTable</returns>
    ''' <remarks></remarks>
    Public Function getOrigCrsLedgerBasicChargeKbn(ByVal paramList As Hashtable, connection As OracleConnection) As DataTable

        Dim resultDataTable As New DataTable
        Dim sqlColString As New StringBuilder
        Dim CrsMasterEntity As EntityOperation(Of TCrsLedgerBasicChargeKbnEntity) = New EntityOperation(Of TCrsLedgerBasicChargeKbnEntity)

        Try
            'Entityから項目情報生成
            For idxItem As Integer = 0 To CrsMasterEntity.getPropertyDataLength - 1
                If idxItem > 0 Then
                    sqlColString.Append(",")
                End If
                With CrsMasterEntity
                    If .getPtyType(idxItem, .EntityData(0)) Is GetType(EntityKoumoku_YmdType) Then
                        sqlColString.AppendLine(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_YmdType).PhysicsName)
                    ElseIf .getPtyType(idxItem, .EntityData(0)) Is GetType(EntityKoumoku_NumberType) Then
                        sqlColString.AppendLine("COALESCE (" & DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_NumberType).PhysicsName & ", 0) AS " & DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_NumberType).PhysicsName)
                    ElseIf .getPtyType(idxItem, .EntityData(0)) Is GetType(EntityKoumoku_Number_DecimalType) Then
                        sqlColString.AppendLine("COALESCE (" & DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_Number_DecimalType).PhysicsName & ", 0) AS " & DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_Number_DecimalType).PhysicsName)
                    Else
                        sqlColString.AppendLine(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_MojiType).PhysicsName)
                    End If
                End With
            Next

            resultDataTable = getCrsLeaderTableData(paramList, connection, sqlColString.ToString, "T_CRS_LEDGER_BASIC_CHARGE_KBN", CrsMasterEntity.EntityData(0).deleteDate.PhysicsName)
        Catch ex As Exception
            Throw
        End Try

        Return resultDataTable
    End Function

    ''' <summary>
    ''' 増発元データ取得(コース台帳（料金_料金区分）)
    ''' </summary>
    ''' <param name="paramList">検索用パラメータ</param>
    ''' <param name="connection">Oracleコネクション</param>
    ''' <returns>DataTable</returns>
    ''' <remarks></remarks>
    Public Function getOrigCrsLedgerChargeChargeKbn(ByVal paramList As Hashtable, connection As OracleConnection) As DataTable

        Dim resultDataTable As New DataTable
        Dim sqlColString As New StringBuilder
        Dim CrsMasterEntity As EntityOperation(Of TCrsLedgerChargeChargeKbnEntity) = New EntityOperation(Of TCrsLedgerChargeChargeKbnEntity)

        Try
            'Entityから項目情報生成
            For idxItem As Integer = 0 To CrsMasterEntity.getPropertyDataLength - 1
                If idxItem > 0 Then
                    sqlColString.Append(",")
                End If
                With CrsMasterEntity
                    If .getPtyType(idxItem, .EntityData(0)) Is GetType(EntityKoumoku_YmdType) Then
                        sqlColString.AppendLine(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_YmdType).PhysicsName)
                    ElseIf .getPtyType(idxItem, .EntityData(0)) Is GetType(EntityKoumoku_NumberType) Then
                        sqlColString.AppendLine("COALESCE (" & DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_NumberType).PhysicsName & ", 0) AS " & DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_NumberType).PhysicsName)
                    ElseIf .getPtyType(idxItem, .EntityData(0)) Is GetType(EntityKoumoku_Number_DecimalType) Then
                        sqlColString.AppendLine("COALESCE (" & DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_Number_DecimalType).PhysicsName & ", 0) AS " & DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_Number_DecimalType).PhysicsName)
                    Else
                        sqlColString.AppendLine(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_MojiType).PhysicsName)
                    End If
                End With
            Next

            resultDataTable = getCrsLeaderTableData(paramList, connection, sqlColString.ToString, "T_CRS_LEDGER_CHARGE_CHARGE_KBN", CrsMasterEntity.EntityData(0).deleteDate.PhysicsName)
        Catch ex As Exception
            Throw
        End Try

        Return resultDataTable
    End Function

    ''' <summary>
    ''' 増発元データ取得(コース台帳（オプショングループ）)
    ''' </summary>
    ''' <param name="paramList">検索用パラメータ</param>
    ''' <param name="connection">Oracleコネクション</param>
    ''' <returns>DataTable</returns>
    ''' <remarks></remarks>
    Public Function getOrigCrsLedgerOptionGroup(ByVal paramList As Hashtable, connection As OracleConnection) As DataTable

        Dim resultDataTable As New DataTable
        Dim sqlColString As New StringBuilder
        Dim CrsMasterEntity As EntityOperation(Of TCrsLedgerOptionGroupEntity) = New EntityOperation(Of TCrsLedgerOptionGroupEntity)

        Try
            'Entityから項目情報生成
            For idxItem As Integer = 0 To CrsMasterEntity.getPropertyDataLength - 1
                If idxItem > 0 Then
                    sqlColString.Append(",")
                End If
                With CrsMasterEntity
                    If .getPtyType(idxItem, .EntityData(0)) Is GetType(EntityKoumoku_YmdType) Then
                        sqlColString.AppendLine(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_YmdType).PhysicsName)
                    ElseIf .getPtyType(idxItem, .EntityData(0)) Is GetType(EntityKoumoku_NumberType) Then
                        sqlColString.AppendLine("COALESCE (" & DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_NumberType).PhysicsName & ", 0) AS " & DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_NumberType).PhysicsName)
                    ElseIf .getPtyType(idxItem, .EntityData(0)) Is GetType(EntityKoumoku_Number_DecimalType) Then
                        sqlColString.AppendLine("COALESCE (" & DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_Number_DecimalType).PhysicsName & ", 0) AS " & DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_Number_DecimalType).PhysicsName)
                    Else
                        sqlColString.AppendLine(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_MojiType).PhysicsName)
                    End If
                End With
            Next

            resultDataTable = getCrsLeaderTableData(paramList, connection, sqlColString.ToString, "T_CRS_LEDGER_OPTION_GROUP", CrsMasterEntity.EntityData(0).deleteDate.PhysicsName)
        Catch ex As Exception
            Throw
        End Try

        Return resultDataTable
    End Function

    ''' <summary>
    ''' 増発元データ取得(コース台帳（販売課所）)
    ''' </summary>
    ''' <param name="paramList">検索用パラメータ</param>
    ''' <param name="connection">Oracleコネクション</param>
    ''' <returns>DataTable</returns>
    ''' <remarks></remarks>
    Public Function getOrigCrsLedgerKasho(ByVal paramList As Hashtable, connection As OracleConnection) As DataTable

        Dim resultDataTable As New DataTable
        Dim sqlColString As New StringBuilder
        Dim CrsMasterEntity As EntityOperation(Of TCrsLedgerKashoEntity) = New EntityOperation(Of TCrsLedgerKashoEntity)

        Try
            'Entityから項目情報生成
            For idxItem As Integer = 0 To CrsMasterEntity.getPropertyDataLength - 1
                If idxItem > 0 Then
                    sqlColString.Append(",")
                End If
                With CrsMasterEntity
                    If .getPtyType(idxItem, .EntityData(0)) Is GetType(EntityKoumoku_YmdType) Then
                        sqlColString.AppendLine(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_YmdType).PhysicsName)
                    ElseIf .getPtyType(idxItem, .EntityData(0)) Is GetType(EntityKoumoku_NumberType) Then
                        sqlColString.AppendLine("COALESCE (" & DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_NumberType).PhysicsName & ", 0) AS " & DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_NumberType).PhysicsName)
                    ElseIf .getPtyType(idxItem, .EntityData(0)) Is GetType(EntityKoumoku_Number_DecimalType) Then
                        sqlColString.AppendLine("COALESCE (" & DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_Number_DecimalType).PhysicsName & ", 0) AS " & DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_Number_DecimalType).PhysicsName)
                    Else
                        sqlColString.AppendLine(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_MojiType).PhysicsName)
                    End If
                End With
            Next

            resultDataTable = getCrsLeaderTableData(paramList, connection, sqlColString.ToString, "T_CRS_LEDGER_KASHO")
        Catch ex As Exception
            Throw
        End Try

        Return resultDataTable
    End Function

    ''' <summary>
    ''' 増発元データ取得(コース台帳（リマークス）)
    ''' </summary>
    ''' <param name="paramList">検索用パラメータ</param>
    ''' <param name="connection">Oracleコネクション</param>
    ''' <returns>DataTable</returns>
    ''' <remarks></remarks>
    Public Function getOrigCrsLedgerRemarks(ByVal paramList As Hashtable, connection As OracleConnection) As DataTable

        Dim resultDataTable As New DataTable
        Dim sqlColString As New StringBuilder
        Dim CrsMasterEntity As EntityOperation(Of TCrsLedgerRemarksEntity) = New EntityOperation(Of TCrsLedgerRemarksEntity)

        Try
            'Entityから項目情報生成
            For idxItem As Integer = 0 To CrsMasterEntity.getPropertyDataLength - 1
                If idxItem > 0 Then
                    sqlColString.Append(",")
                End If
                With CrsMasterEntity
                    If .getPtyType(idxItem, .EntityData(0)) Is GetType(EntityKoumoku_YmdType) Then
                        sqlColString.AppendLine(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_YmdType).PhysicsName)
                    ElseIf .getPtyType(idxItem, .EntityData(0)) Is GetType(EntityKoumoku_NumberType) Then
                        sqlColString.AppendLine("COALESCE (" & DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_NumberType).PhysicsName & ", 0) AS " & DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_NumberType).PhysicsName)
                    ElseIf .getPtyType(idxItem, .EntityData(0)) Is GetType(EntityKoumoku_Number_DecimalType) Then
                        sqlColString.AppendLine("COALESCE (" & DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_Number_DecimalType).PhysicsName & ", 0) AS " & DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_Number_DecimalType).PhysicsName)
                    Else
                        sqlColString.AppendLine(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_MojiType).PhysicsName)
                    End If
                End With
            Next

            resultDataTable = getCrsLeaderTableData(paramList, connection, sqlColString.ToString, "T_CRS_LEDGER_REMARKS", CrsMasterEntity.EntityData(0).deleteDate.PhysicsName)
        Catch ex As Exception
            Throw
        End Try

        Return resultDataTable
    End Function

    ''' <summary>
    ''' 増発元データ取得(コース台帳原価（基本）)
    ''' </summary>
    ''' <param name="paramList">検索用パラメータ</param>
    ''' <param name="connection">Oracleコネクション</param>
    ''' <returns>DataTable</returns>
    ''' <remarks></remarks>
    Public Function getOrigCrsLedgerCostBasic(ByVal paramList As Hashtable, connection As OracleConnection) As DataTable

        Dim resultDataTable As New DataTable
        Dim sqlColString As New StringBuilder
        Dim CrsMasterEntity As EntityOperation(Of TCrsLedgerCostBasicEntity) = New EntityOperation(Of TCrsLedgerCostBasicEntity)

        Try
            'Entityから項目情報生成
            For idxItem As Integer = 0 To CrsMasterEntity.getPropertyDataLength - 1
                If idxItem > 0 Then
                    sqlColString.Append(",")
                End If
                With CrsMasterEntity
                    If .getPtyType(idxItem, .EntityData(0)) Is GetType(EntityKoumoku_YmdType) Then
                        sqlColString.AppendLine(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_YmdType).PhysicsName)
                    ElseIf .getPtyType(idxItem, .EntityData(0)) Is GetType(EntityKoumoku_NumberType) Then
                        sqlColString.AppendLine("COALESCE (" & DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_NumberType).PhysicsName & ", 0) AS " & DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_NumberType).PhysicsName)
                    ElseIf .getPtyType(idxItem, .EntityData(0)) Is GetType(EntityKoumoku_Number_DecimalType) Then
                        sqlColString.AppendLine("COALESCE (" & DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_Number_DecimalType).PhysicsName & ", 0) AS " & DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_Number_DecimalType).PhysicsName)
                    Else
                        sqlColString.AppendLine(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_MojiType).PhysicsName)
                    End If
                End With
            Next

            resultDataTable = getCrsLeaderTableData(paramList, connection, sqlColString.ToString, "T_CRS_LEDGER_COST_BASIC")
        Catch ex As Exception
            Throw
        End Try

        Return resultDataTable
    End Function

    ''' <summary>
    ''' 増発元データ取得(コース台帳原価（降車ヶ所）)
    ''' </summary>
    ''' <param name="paramList">検索用パラメータ</param>
    ''' <param name="connection">Oracleコネクション</param>
    ''' <returns>DataTable</returns>
    ''' <remarks></remarks>
    Public Function getOrigCrsLedgerCostKoshakasho(ByVal paramList As Hashtable, connection As OracleConnection) As DataTable

        Dim resultDataTable As New DataTable
        Dim sqlColString As New StringBuilder
        Dim CrsMasterEntity As EntityOperation(Of TCrsLedgerCostKoshakashoEntity) = New EntityOperation(Of TCrsLedgerCostKoshakashoEntity)

        Try
            'Entityから項目情報生成
            For idxItem As Integer = 0 To CrsMasterEntity.getPropertyDataLength - 1
                If idxItem > 0 Then
                    sqlColString.Append(",")
                End If
                With CrsMasterEntity
                    If .getPtyType(idxItem, .EntityData(0)) Is GetType(EntityKoumoku_YmdType) Then
                        sqlColString.AppendLine(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_YmdType).PhysicsName)
                    ElseIf .getPtyType(idxItem, .EntityData(0)) Is GetType(EntityKoumoku_NumberType) Then
                        sqlColString.AppendLine("COALESCE (" & DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_NumberType).PhysicsName & ", 0) AS " & DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_NumberType).PhysicsName)
                    ElseIf .getPtyType(idxItem, .EntityData(0)) Is GetType(EntityKoumoku_Number_DecimalType) Then
                        sqlColString.AppendLine("COALESCE (" & DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_Number_DecimalType).PhysicsName & ", 0) AS " & DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_Number_DecimalType).PhysicsName)
                    Else
                        sqlColString.AppendLine(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_MojiType).PhysicsName)
                    End If
                End With
            Next

            resultDataTable = getCrsLeaderTableData(paramList, connection, sqlColString.ToString, "T_CRS_LEDGER_COST_KOSHAKASHO")
        Catch ex As Exception
            Throw
        End Try

        Return resultDataTable
    End Function

    ''' <summary>
    ''' 増発元データ取得(コース台帳原価（キャリア）)
    ''' </summary>
    ''' <param name="paramList">検索用パラメータ</param>
    ''' <param name="connection">Oracleコネクション</param>
    ''' <returns>DataTable</returns>
    ''' <remarks></remarks>
    Public Function getOrigCrsLedgerCostCarrier(ByVal paramList As Hashtable, connection As OracleConnection) As DataTable

        Dim resultDataTable As New DataTable
        Dim sqlColString As New StringBuilder
        Dim CrsMasterEntity As EntityOperation(Of TCrsLedgerCostCarrierEntity) = New EntityOperation(Of TCrsLedgerCostCarrierEntity)

        Try
            'Entityから項目情報生成
            For idxItem As Integer = 0 To CrsMasterEntity.getPropertyDataLength - 1
                If idxItem > 0 Then
                    sqlColString.Append(",")
                End If
                With CrsMasterEntity
                    If .getPtyType(idxItem, .EntityData(0)) Is GetType(EntityKoumoku_YmdType) Then
                        sqlColString.AppendLine(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_YmdType).PhysicsName)
                    ElseIf .getPtyType(idxItem, .EntityData(0)) Is GetType(EntityKoumoku_NumberType) Then
                        sqlColString.AppendLine("COALESCE (" & DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_NumberType).PhysicsName & ", 0) AS " & DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_NumberType).PhysicsName)
                    ElseIf .getPtyType(idxItem, .EntityData(0)) Is GetType(EntityKoumoku_Number_DecimalType) Then
                        sqlColString.AppendLine("COALESCE (" & DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_Number_DecimalType).PhysicsName & ", 0) AS " & DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_Number_DecimalType).PhysicsName)
                    Else
                        sqlColString.AppendLine(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_MojiType).PhysicsName)
                    End If
                End With
            Next

            resultDataTable = getCrsLeaderTableData(paramList, connection, sqlColString.ToString, "T_CRS_LEDGER_COST_CARRIER")
        Catch ex As Exception
            Throw
        End Try

        Return resultDataTable
    End Function

    ''' <summary>
    ''' 増発元データ取得(コース台帳原価（プレート）)
    ''' </summary>
    ''' <param name="paramList">検索用パラメータ</param>
    ''' <param name="connection">Oracleコネクション</param>
    ''' <returns>DataTable</returns>
    ''' <remarks></remarks>
    Public Function getOrigCrsLedgerCostPlate(ByVal paramList As Hashtable, connection As OracleConnection) As DataTable

        Dim resultDataTable As New DataTable
        Dim sqlColString As New StringBuilder
        Dim CrsMasterEntity As EntityOperation(Of TCrsLedgerCostPlateEntity) = New EntityOperation(Of TCrsLedgerCostPlateEntity)

        Try
            'Entityから項目情報生成
            For idxItem As Integer = 0 To CrsMasterEntity.getPropertyDataLength - 1
                If idxItem > 0 Then
                    sqlColString.Append(",")
                End If
                With CrsMasterEntity
                    If .getPtyType(idxItem, .EntityData(0)) Is GetType(EntityKoumoku_YmdType) Then
                        sqlColString.AppendLine(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_YmdType).PhysicsName)
                    ElseIf .getPtyType(idxItem, .EntityData(0)) Is GetType(EntityKoumoku_NumberType) Then
                        sqlColString.AppendLine("COALESCE (" & DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_NumberType).PhysicsName & ", 0) AS " & DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_NumberType).PhysicsName)
                    ElseIf .getPtyType(idxItem, .EntityData(0)) Is GetType(EntityKoumoku_Number_DecimalType) Then
                        sqlColString.AppendLine("COALESCE (" & DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_Number_DecimalType).PhysicsName & ", 0) AS " & DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_Number_DecimalType).PhysicsName)
                    Else
                        sqlColString.AppendLine(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_MojiType).PhysicsName)
                    End If
                End With
            Next

            resultDataTable = getCrsLeaderTableData(paramList, connection, sqlColString.ToString, "T_CRS_LEDGER_COST_PLATE")
        Catch ex As Exception
            Throw
        End Try

        Return resultDataTable
    End Function

    ''' <summary>
    ''' 増発元データ取得(コース台帳原価（キャリア_料金区分）)
    ''' </summary>
    ''' <param name="paramList">検索用パラメータ</param>
    ''' <param name="connection">Oracleコネクション</param>
    ''' <returns>DataTable</returns>
    ''' <remarks></remarks>
    Public Function getOrigCrsLedgerCostCarrierChargeKbn(ByVal paramList As Hashtable, connection As OracleConnection) As DataTable

        Dim resultDataTable As New DataTable
        Dim sqlColString As New StringBuilder
        Dim CrsMasterEntity As EntityOperation(Of TCrsLedgerCostCarrierChargeKbnEntity) = New EntityOperation(Of TCrsLedgerCostCarrierChargeKbnEntity)

        Try
            'Entityから項目情報生成
            For idxItem As Integer = 0 To CrsMasterEntity.getPropertyDataLength - 1
                If idxItem > 0 Then
                    sqlColString.Append(",")
                End If
                With CrsMasterEntity
                    If .getPtyType(idxItem, .EntityData(0)) Is GetType(EntityKoumoku_YmdType) Then
                        sqlColString.AppendLine(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_YmdType).PhysicsName)
                    ElseIf .getPtyType(idxItem, .EntityData(0)) Is GetType(EntityKoumoku_NumberType) Then
                        sqlColString.AppendLine("COALESCE (" & DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_NumberType).PhysicsName & ", 0) AS " & DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_NumberType).PhysicsName)
                    ElseIf .getPtyType(idxItem, .EntityData(0)) Is GetType(EntityKoumoku_Number_DecimalType) Then
                        sqlColString.AppendLine("COALESCE (" & DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_Number_DecimalType).PhysicsName & ", 0) AS " & DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_Number_DecimalType).PhysicsName)
                    Else
                        sqlColString.AppendLine(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_MojiType).PhysicsName)
                    End If
                End With
            Next

            resultDataTable = getCrsLeaderTableData(paramList, connection, sqlColString.ToString, "T_CRS_LEDGER_COST_CARRIER_CHARGE_KBN", CrsMasterEntity.EntityData(0).deleteDate.PhysicsName)
        Catch ex As Exception
            Throw
        End Try

        Return resultDataTable
    End Function

    ''' <summary>
    ''' 増発元データ取得(コース台帳原価（降車ヶ所_料金区分）)
    ''' </summary>
    ''' <param name="paramList">検索用パラメータ</param>
    ''' <param name="connection">Oracleコネクション</param>
    ''' <returns>DataTable</returns>
    ''' <remarks></remarks>
    Public Function getOrigCrsLedgerCostKoshakashoChargeKbn(ByVal paramList As Hashtable, connection As OracleConnection) As DataTable

        Dim resultDataTable As New DataTable
        Dim sqlColString As New StringBuilder
        Dim CrsMasterEntity As EntityOperation(Of TCrsLedgerCostKoshakashoChargeKbnEntity) = New EntityOperation(Of TCrsLedgerCostKoshakashoChargeKbnEntity)

        Try
            'Entityから項目情報生成
            For idxItem As Integer = 0 To CrsMasterEntity.getPropertyDataLength - 1
                If idxItem > 0 Then
                    sqlColString.Append(",")
                End If
                With CrsMasterEntity
                    If .getPtyType(idxItem, .EntityData(0)) Is GetType(EntityKoumoku_YmdType) Then
                        sqlColString.AppendLine(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_YmdType).PhysicsName)
                    ElseIf .getPtyType(idxItem, .EntityData(0)) Is GetType(EntityKoumoku_NumberType) Then
                        sqlColString.AppendLine("COALESCE (" & DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_NumberType).PhysicsName & ", 0) AS " & DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_NumberType).PhysicsName)
                    ElseIf .getPtyType(idxItem, .EntityData(0)) Is GetType(EntityKoumoku_Number_DecimalType) Then
                        sqlColString.AppendLine("COALESCE (" & DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_Number_DecimalType).PhysicsName & ", 0) AS " & DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_Number_DecimalType).PhysicsName)
                    Else
                        sqlColString.AppendLine(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_MojiType).PhysicsName)
                    End If
                End With
            Next

            resultDataTable = getCrsLeaderTableData(paramList, connection, sqlColString.ToString, "T_CRS_LEDGER_COST_KOSHAKASHO_CHARGE_KBN", CrsMasterEntity.EntityData(0).deleteDate.PhysicsName)
        Catch ex As Exception
            Throw
        End Try

        Return resultDataTable
    End Function

    ''' <summary>
    ''' 増発元データ取得(座席イメージ（バス情報）)
    ''' </summary>
    ''' <param name="paramList">検索用パラメータ</param>
    ''' <param name="connection">Oracleコネクション</param>
    ''' <returns>DataTable</returns>
    ''' <remarks></remarks>
    Public Function getOrigZasekiImage(ByVal paramList As Hashtable, connection As OracleConnection) As DataTable

        Dim resultDataTable As New DataTable
        Dim sqlColString As New StringBuilder
        Dim CrsMasterEntity As EntityOperation(Of TZasekiImageEntity) = New EntityOperation(Of TZasekiImageEntity)

        Try
            'Entityから項目情報生成
            For idxItem As Integer = 0 To CrsMasterEntity.getPropertyDataLength - 1
                If idxItem > 0 Then
                    sqlColString.Append(",")
                End If
                With CrsMasterEntity
                    If .getPtyType(idxItem, .EntityData(0)) Is GetType(EntityKoumoku_YmdType) Then
                        sqlColString.AppendLine(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_YmdType).PhysicsName)
                    ElseIf .getPtyType(idxItem, .EntityData(0)) Is GetType(EntityKoumoku_NumberType) Then
                        sqlColString.AppendLine("COALESCE (" & DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_NumberType).PhysicsName & ", 0) AS " & DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_NumberType).PhysicsName)
                    ElseIf .getPtyType(idxItem, .EntityData(0)) Is GetType(EntityKoumoku_Number_DecimalType) Then
                        sqlColString.AppendLine("COALESCE (" & DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_Number_DecimalType).PhysicsName & ", 0) AS " & DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_Number_DecimalType).PhysicsName)
                    Else
                        sqlColString.AppendLine(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_MojiType).PhysicsName)
                    End If
                End With
            Next

            resultDataTable = getZasekiTableData(paramList, connection, sqlColString.ToString, "T_ZASEKI_IMAGE")
        Catch ex As Exception
            Throw
        End Try

        Return resultDataTable
    End Function

    ''' <summary>
    ''' 増発元データ取得(座席イメージ（座席情報）)
    ''' </summary>
    ''' <param name="paramList">検索用パラメータ</param>
    ''' <param name="connection">Oracleコネクション</param>
    ''' <returns>DataTable</returns>
    ''' <remarks></remarks>
    Public Function getOrigZasekiImageInfo(ByVal paramList As Hashtable, connection As OracleConnection) As DataTable

        Dim resultDataTable As New DataTable
        Dim sqlColString As New StringBuilder
        Dim CrsMasterEntity As EntityOperation(Of TZasekiImageInfoEntity) = New EntityOperation(Of TZasekiImageInfoEntity)

        Try
            'Entityから項目情報生成
            For idxItem As Integer = 0 To CrsMasterEntity.getPropertyDataLength - 1
                If idxItem > 0 Then
                    sqlColString.Append(",")
                End If
                With CrsMasterEntity
                    If .getPtyType(idxItem, .EntityData(0)) Is GetType(EntityKoumoku_YmdType) Then
                        sqlColString.AppendLine(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_YmdType).PhysicsName)
                    ElseIf .getPtyType(idxItem, .EntityData(0)) Is GetType(EntityKoumoku_NumberType) Then
                        sqlColString.AppendLine("COALESCE (" & DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_NumberType).PhysicsName & ", 0) AS " & DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_NumberType).PhysicsName)
                    ElseIf .getPtyType(idxItem, .EntityData(0)) Is GetType(EntityKoumoku_Number_DecimalType) Then
                        sqlColString.AppendLine("COALESCE (" & DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_Number_DecimalType).PhysicsName & ", 0) AS " & DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_Number_DecimalType).PhysicsName)
                    Else
                        sqlColString.AppendLine(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_MojiType).PhysicsName)
                    End If
                End With
            Next

            resultDataTable = getZasekiTableData(paramList, connection, sqlColString.ToString, "T_ZASEKI_IMAGE_INFO")
        Catch ex As Exception
            Throw
        End Try

        Return resultDataTable
    End Function

    ''' <summary>
    ''' マスタデータ取得(座席マスタ（座席情報）)
    ''' </summary>
    ''' <param name="paramList">検索用パラメータ</param>
    ''' <param name="connection">Oracleコネクション</param>
    ''' <returns>DataTable</returns>
    ''' <remarks></remarks>
    Public Function getZasekiMst(ByVal paramList As Hashtable, connection As OracleConnection) As DataTable

        Dim resultDataTable As New DataTable
        Dim sqlColString As New StringBuilder
        Dim sqlString As New StringBuilder
        Dim MasterEntity As EntityOperation(Of MZasekiNoEntity) = New EntityOperation(Of MZasekiNoEntity)

        Try
            'Entityから項目情報生成
            For idxItem As Integer = 0 To MasterEntity.getPropertyDataLength - 1
                If idxItem > 0 Then
                    sqlColString.Append(",")
                End If
                With MasterEntity
                    If .getPtyType(idxItem, .EntityData(0)) Is GetType(EntityKoumoku_YmdType) Then
                        sqlColString.AppendLine(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_YmdType).PhysicsName)
                    ElseIf .getPtyType(idxItem, .EntityData(0)) Is GetType(EntityKoumoku_NumberType) Then
                        sqlColString.AppendLine(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_NumberType).PhysicsName)
                    ElseIf .getPtyType(idxItem, .EntityData(0)) Is GetType(EntityKoumoku_Number_DecimalType) Then
                        sqlColString.AppendLine(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_Number_DecimalType).PhysicsName)
                    Else
                        sqlColString.AppendLine(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_MojiType).PhysicsName)
                    End If
                End With
            Next
            'パラメータクリア
            paramClear()

            With MasterEntity
                sqlString.AppendLine(" SELECT")
                sqlString.Append(sqlColString)
                ' FROM句
                sqlString.AppendLine(" FROM ")
                sqlString.AppendLine(" M_ZASEKI_NO")

                ' WHERE句
                sqlString.AppendLine(" WHERE CAR_CD = " & setParam(.EntityData(0).carCd.PhysicsName, paramList(UpdateParamKeys.CAR_TYPE_CD), .EntityData(0).carCd.DBType, .EntityData(0).carCd.IntegerBu, .EntityData(0).carCd.DecimalBu))
            End With

            resultDataTable = MyBase.getDataTable(connection, sqlString.ToString())

        Catch ex As Exception
            Throw
        End Try

        Return resultDataTable
    End Function
#End Region
#End Region

#Region " INSERT処理(23TBL) "

    ''' <summary>
    ''' コース台帳登録処理
    ''' </summary>
    ''' <param name="paramInfoList"></param>
    ''' <param name="zouhatsutOrigData"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function executeLedgerInsertTehai(ByVal paramInfoList As Hashtable, ByVal zouhatsutOrigData As DataSet, ByVal zasekiImageFlg As Boolean) As Integer
        Dim oracleTransaction As OracleTransaction = Nothing
        Dim returnValue As Integer = 0
        Dim updateValue As Integer = 0
        Dim sqlString As String = String.Empty

        Try
            'コース台帳(基本)
            editDataBasicTable(zouhatsutOrigData, paramInfoList)
            'コース台帳(降車ヶ所)
            editDataKoushakashoTable(zouhatsutOrigData, paramInfoList)
            '座席イメージ情報
            editDataZasekiImageInfoTable(zouhatsutOrigData, paramInfoList)
            '座席イメージ
            editDataZasekiImageTable(zouhatsutOrigData, paramInfoList)
            'ブロック数
            editDataZasekiBlockCnt(zouhatsutOrigData, paramInfoList)
            'データ編集
            editCommonValue(zouhatsutOrigData, paramInfoList)

            'トランザクション開始
            oracleTransaction = callBeginTransaction()
            'コース台帳（基本）
            updateValue = exeCuteInsertBasicData(zouhatsutOrigData.Tables(crsLedgerTblId.crsLeaderBasic), oracleTransaction)
            If updateValue = ERRCNT Then
                'ロールバック
                Call callRollbackTransaction(oracleTransaction)
                Return ERRCNT
            Else
                returnValue += updateValue
            End If
            ' コース台帳（降車ヶ所）
            updateValue = exeCuteInsertKoshakashoData(zouhatsutOrigData.Tables(crsLedgerTblId.crsLeaderKoshakasho), oracleTransaction)
            If updateValue = ERRCNT Then
                'ロールバック
                Call callRollbackTransaction(oracleTransaction)
                Return ERRCNT
            Else
                returnValue += updateValue
            End If
            ' コース台帳（コース情報）
            updateValue = exeCuteInsertCrsInfoData(zouhatsutOrigData.Tables(crsLedgerTblId.crsLeaderCrsInfo), oracleTransaction)
            If updateValue = ERRCNT Then
                'ロールバック
                Call callRollbackTransaction(oracleTransaction)
                Return ERRCNT
            Else
                returnValue += updateValue
            End If
            ' コース台帳（メッセージ）
            updateValue = exeCuteInsertMessageData(zouhatsutOrigData.Tables(crsLedgerTblId.crsLeaderMessage), oracleTransaction)
            If updateValue = ERRCNT Then
                'ロールバック
                Call callRollbackTransaction(oracleTransaction)
                Return ERRCNT
            Else
                returnValue += updateValue
            End If
            ' コース台帳（料金）
            updateValue = exeCuteInsertChargeData(zouhatsutOrigData.Tables(crsLedgerTblId.crsLeaderCharge), oracleTransaction)
            If updateValue = ERRCNT Then
                'ロールバック
                Call callRollbackTransaction(oracleTransaction)
                Return ERRCNT
            Else
                returnValue += updateValue
            End If
            ' コース台帳（ホテル）
            updateValue = exeCuteInsertHotelData(zouhatsutOrigData.Tables(crsLedgerTblId.crsLeaderHotel), oracleTransaction)
            If updateValue = ERRCNT Then
                'ロールバック
                Call callRollbackTransaction(oracleTransaction)
                Return ERRCNT
            Else
                returnValue += updateValue
            End If
            ' コース台帳（付加情報）
            updateValue = exeCuteInsertAddInfoData(zouhatsutOrigData.Tables(crsLedgerTblId.crsLeaderAddInfo), oracleTransaction)
            If updateValue = ERRCNT Then
                'ロールバック
                Call callRollbackTransaction(oracleTransaction)
                Return ERRCNT
            Else
                returnValue += updateValue
            End If
            ' コース台帳（バス紐づけ）
            updateValue = exeCuteInsertBusHimodukeData(zouhatsutOrigData.Tables(crsLedgerTblId.crsLeaderBusHimoduke), oracleTransaction)
            If updateValue = ERRCNT Then
                'ロールバック
                Call callRollbackTransaction(oracleTransaction)
                Return ERRCNT
            Else
                returnValue += updateValue
            End If
            ' コース台帳（オプション）
            updateValue = exeCuteInsertOptionData(zouhatsutOrigData.Tables(crsLedgerTblId.crsLeaderOption), oracleTransaction)
            If updateValue = ERRCNT Then
                'ロールバック
                Call callRollbackTransaction(oracleTransaction)
                Return ERRCNT
            Else
                returnValue += updateValue
            End If
            ' コース台帳（ダイヤ）
            updateValue = exeCuteInsertDiaData(zouhatsutOrigData.Tables(crsLedgerTblId.crsLeaderDia), oracleTransaction)
            If updateValue = ERRCNT Then
                'ロールバック
                Call callRollbackTransaction(oracleTransaction)
                Return ERRCNT
            Else
                returnValue += updateValue
            End If
            ' コース台帳（基本_料金区分）
            updateValue = exeCuteInsertBasicChargeKbnData(zouhatsutOrigData.Tables(crsLedgerTblId.crsLeaderBasicChargeKbn), oracleTransaction)
            If updateValue = ERRCNT Then
                'ロールバック
                Call callRollbackTransaction(oracleTransaction)
                Return ERRCNT
            Else
                returnValue += updateValue
            End If
            ' コース台帳（料金_料金区分）
            updateValue = exeCuteInsertChargeChargeKbnData(zouhatsutOrigData.Tables(crsLedgerTblId.crsLeaderChargeChargeKbn), oracleTransaction)
            If updateValue = ERRCNT Then
                'ロールバック
                Call callRollbackTransaction(oracleTransaction)
                Return ERRCNT
            Else
                returnValue += updateValue
            End If
            ' コース台帳（オプショングループ）
            updateValue = exeCuteInsertOptionGroupData(zouhatsutOrigData.Tables(crsLedgerTblId.crsLeaderOptionGroup), oracleTransaction)
            If updateValue = ERRCNT Then
                'ロールバック
                Call callRollbackTransaction(oracleTransaction)
                Return ERRCNT
            Else
                returnValue += updateValue
            End If
            ' コース台帳（販売課所）
            updateValue = exeCuteInsertKashoData(zouhatsutOrigData.Tables(crsLedgerTblId.crsLeaderKasho), oracleTransaction)
            If updateValue = ERRCNT Then
                'ロールバック
                Call callRollbackTransaction(oracleTransaction)
                Return ERRCNT
            Else
                returnValue += updateValue
            End If
            ' コース台帳（リマークス）
            updateValue = exeCuteInsertRemarksData(zouhatsutOrigData.Tables(crsLedgerTblId.crsLeaderRemarks), oracleTransaction)
            If updateValue = ERRCNT Then
                'ロールバック
                Call callRollbackTransaction(oracleTransaction)
                Return ERRCNT
            Else
                returnValue += updateValue
            End If
            ' コース台帳原価（基本）
            updateValue = exeCuteInsertCostBasicData(zouhatsutOrigData.Tables(crsLedgerTblId.crsLeaderCostBasic), oracleTransaction)
            If updateValue = ERRCNT Then
                'ロールバック
                Call callRollbackTransaction(oracleTransaction)
                Return ERRCNT
            Else
                returnValue += updateValue
            End If
            ' コース台帳原価（降車ヶ所）
            updateValue = exeCuteInsertCostKoshakashoData(zouhatsutOrigData.Tables(crsLedgerTblId.crsLeaderCostKoshakasho), oracleTransaction)
            If updateValue = ERRCNT Then
                'ロールバック
                Call callRollbackTransaction(oracleTransaction)
                Return ERRCNT
            Else
                returnValue += updateValue
            End If
            ' コース台帳原価（キャリア）
            updateValue = exeCuteInsertCostCarrierData(zouhatsutOrigData.Tables(crsLedgerTblId.crsLeaderCostCarrier), oracleTransaction)
            If updateValue = ERRCNT Then
                'ロールバック
                Call callRollbackTransaction(oracleTransaction)
                Return ERRCNT
            Else
                returnValue += updateValue
            End If
            ' コース台帳原価（プレート）
            updateValue = exeCuteInsertCostPlateData(zouhatsutOrigData.Tables(crsLedgerTblId.crsLeaderCostPlate), oracleTransaction)
            If updateValue = ERRCNT Then
                'ロールバック
                Call callRollbackTransaction(oracleTransaction)
                Return ERRCNT
            Else
                returnValue += updateValue
            End If
            ' コース台帳原価（キャリア_料金区分）
            updateValue = exeCuteInsertCostCarrierChargeKbnData(zouhatsutOrigData.Tables(crsLedgerTblId.crsLeaderCostCarrierChargeKbn), oracleTransaction)
            If updateValue = ERRCNT Then
                'ロールバック
                Call callRollbackTransaction(oracleTransaction)
                Return ERRCNT
            Else
                returnValue += updateValue
            End If
            ' コース台帳原価（降車ヶ所_料金区分）
            updateValue = exeCuteInsertCostKoshakashoChargeKbnData(zouhatsutOrigData.Tables(crsLedgerTblId.crsLeaderCostKoshakashoChargeKbn), oracleTransaction)
            If updateValue = ERRCNT Then
                'ロールバック
                Call callRollbackTransaction(oracleTransaction)
                Return ERRCNT
            Else
                returnValue += updateValue
            End If

            ' 座席イメージが作成されていない場合作成
            If zasekiImageFlg = False Then
                ' 座席イメージ（バス情報）
                updateValue = exeCuteInsertZasekiImageoData(zouhatsutOrigData.Tables(crsLedgerTblId.zasekiImage), oracleTransaction)
                If updateValue = ERRCNT Then
                    'ロールバック
                    Call callRollbackTransaction(oracleTransaction)
                    Return ERRCNT
                Else
                    returnValue += updateValue
                End If
                ' 座席イメージ（座席情報）
                updateValue = exeCuteInsertZasekiImageInfoData(zouhatsutOrigData.Tables(crsLedgerTblId.zasekiImageInfo), oracleTransaction)
                If updateValue = ERRCNT Then
                    'ロールバック
                    Call callRollbackTransaction(oracleTransaction)
                    Return ERRCNT
                Else
                    returnValue += updateValue
                End If
            End If

            If returnValue > 0 Then
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
    ''' コース台帳（基本）挿入
    ''' </summary>
    ''' <param name="paramDt">DataTable</param>
    ''' <param name="paramTrans">Oracleトランザクション</param>
    ''' <returns>更新件数(-1:エラー,0以上:正常)</returns>
    ''' <remarks></remarks>
    Private Function exeCuteInsertBasicData(ByVal paramDt As DataTable, ByVal paramTrans As OracleTransaction) As Integer

        Dim CrsMasterEntity As EntityOperation(Of TCrsLedgerBasicEntity) = New EntityOperation(Of TCrsLedgerBasicEntity)
        Dim sqlString As String = ""  'sqlString
        Dim sqlInsertString As String = ""  'sqlInsertString
        Dim sqlValueString As String = ""  'sqlValueString
        Dim retValue As Integer  'retValue
        Dim insertCnt As Integer

        'DataTableをEntityに詰める
        For idx As Integer = 0 To paramDt.Rows.Count - 1
            If idx > 0 Then
                Dim entity As New TCrsLedgerBasicEntity  'entity
                CrsMasterEntity.add(entity)
            End If
            For idxItem As Integer = 0 To CrsMasterEntity.getPropertyDataLength - 1
                With CrsMasterEntity
                    If .getPtyType(idxItem, .EntityData(0)) Is GetType(EntityKoumoku_YmdType) Then
                        If IsDBNull(paramDt.Rows(0).Item(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_YmdType).PhysicsName)) = False Then
                            DirectCast(.getPtyValue(idxItem, .EntityData(idx)), EntityKoumoku_YmdType).Value = CDate(paramDt.Rows(idx).Item(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_YmdType).PhysicsName))
                        End If
                    ElseIf .getPtyType(idxItem, .EntityData(0)) Is GetType(EntityKoumoku_NumberType) Then
                        If IsDBNull(paramDt.Rows(0).Item(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_NumberType).PhysicsName)) = False Then
                            DirectCast(.getPtyValue(idxItem, .EntityData(idx)), EntityKoumoku_NumberType).Value = CInt(paramDt.Rows(idx).Item(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_NumberType).PhysicsName))
                        End If
                    ElseIf .getPtyType(idxItem, .EntityData(0)) Is GetType(EntityKoumoku_Number_DecimalType) Then
                        If IsDBNull(paramDt.Rows(0).Item(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_Number_DecimalType).PhysicsName)) = False Then
                            DirectCast(.getPtyValue(idxItem, .EntityData(idx)), EntityKoumoku_Number_DecimalType).Value = CDec(paramDt.Rows(idx).Item(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_Number_DecimalType).PhysicsName))
                        End If
                    Else
                        DirectCast(.getPtyValue(idxItem, .EntityData(idx)), EntityKoumoku_MojiType).Value = getDBValueForString(paramDt.Rows(idx).Item(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_MojiType).PhysicsName))
                    End If
                End With
            Next
        Next

        For entityCnt As Integer = 0 To CrsMasterEntity.EntityData.Length - 1
            paramClear()
            'データ未設定行はスキップ
            If CrsMasterEntity.EntityData(entityCnt).crsCd.Value = "" Then
                Continue For
            End If

            'SQL文生成
            sqlInsertString = "INSERT INTO " & "T_CRS_LEDGER_BASIC ("
            sqlValueString = "VALUES("

            For idx As Integer = 0 To CrsMasterEntity.getPropertyDataLength - 1
                If idx > 0 Then
                    sqlInsertString &= ","
                    sqlValueString &= ","
                End If
                With CrsMasterEntity
                    If .getPtyType(idx, .EntityData(entityCnt)) Is GetType(EntityKoumoku_YmdType) Then
                        sqlInsertString &= DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_YmdType).PhysicsName
                        sqlValueString &= setParam(DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_YmdType).PhysicsName,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_YmdType).Value,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_YmdType).DBType)
                    ElseIf .getPtyType(idx, .EntityData(entityCnt)) Is GetType(EntityKoumoku_NumberType) Then
                        sqlInsertString &= DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_NumberType).PhysicsName
                        sqlValueString &= setParam(DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_NumberType).PhysicsName,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_NumberType).Value,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_NumberType).DBType,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_NumberType).IntegerBu,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_NumberType).DecimalBu)
                    ElseIf .getPtyType(idx, .EntityData(entityCnt)) Is GetType(EntityKoumoku_Number_DecimalType) Then
                        sqlInsertString &= DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_Number_DecimalType).PhysicsName
                        sqlValueString &= setParam(DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_Number_DecimalType).PhysicsName,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_Number_DecimalType).Value,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_Number_DecimalType).DBType,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_Number_DecimalType).IntegerBu,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_Number_DecimalType).DecimalBu)
                    Else
                        sqlInsertString &= DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_MojiType).PhysicsName
                        sqlValueString &= setParam(DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_MojiType).PhysicsName,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_MojiType).Value,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_MojiType).DBType,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_MojiType).IntegerBu)
                    End If
                End With
            Next
            sqlString = sqlInsertString & ") " & sqlValueString & ")"
            insertCnt = execNonQuery(paramTrans, sqlString)

            '一意制約違反等でInsertエラーの場合
            If insertCnt = 0 Then
                '更新元エラーコードで返却
                Return ERRCNT
            Else
                retValue += insertCnt
            End If
        Next

        Return retValue
    End Function

    ''' <summary>
    ''' コース台帳（降車ヶ所）挿入
    ''' </summary>
    ''' <param name="paramDt">DataTable</param>
    ''' <param name="paramTrans">Oracleトランザクション</param>
    ''' <returns>更新件数(-1:エラー,0以上:正常)</returns>
    ''' <remarks></remarks>
    Private Function exeCuteInsertKoshakashoData(ByVal paramDt As DataTable, ByVal paramTrans As OracleTransaction) As Integer

        Dim CrsMasterEntity As EntityOperation(Of TCrsLedgerKoshakashoEntity) = New EntityOperation(Of TCrsLedgerKoshakashoEntity)
        Dim sqlString As String = ""  'sqlString
        Dim sqlInsertString As String = ""  'sqlInsertString
        Dim sqlValueString As String = ""  'sqlValueString
        Dim retValue As Integer  'retValue
        Dim insertCnt As Integer

        'DataTableをEntityに詰める
        For idx As Integer = 0 To paramDt.Rows.Count - 1
            If idx > 0 Then
                Dim entity As New TCrsLedgerKoshakashoEntity  'entity
                CrsMasterEntity.add(entity)
            End If
            For idxItem As Integer = 0 To CrsMasterEntity.getPropertyDataLength - 1
                With CrsMasterEntity
                    If .getPtyType(idxItem, .EntityData(0)) Is GetType(EntityKoumoku_YmdType) Then
                        If IsDBNull(paramDt.Rows(0).Item(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_YmdType).PhysicsName)) = False Then
                            DirectCast(.getPtyValue(idxItem, .EntityData(idx)), EntityKoumoku_YmdType).Value = CDate(paramDt.Rows(idx).Item(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_YmdType).PhysicsName))
                        End If
                    ElseIf .getPtyType(idxItem, .EntityData(0)) Is GetType(EntityKoumoku_NumberType) Then
                        If IsDBNull(paramDt.Rows(0).Item(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_NumberType).PhysicsName)) = False Then
                            DirectCast(.getPtyValue(idxItem, .EntityData(idx)), EntityKoumoku_NumberType).Value = CInt(paramDt.Rows(idx).Item(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_NumberType).PhysicsName))
                        End If
                    ElseIf .getPtyType(idxItem, .EntityData(0)) Is GetType(EntityKoumoku_Number_DecimalType) Then
                        If IsDBNull(paramDt.Rows(0).Item(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_Number_DecimalType).PhysicsName)) = False Then
                            DirectCast(.getPtyValue(idxItem, .EntityData(idx)), EntityKoumoku_Number_DecimalType).Value = CDec(paramDt.Rows(idx).Item(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_Number_DecimalType).PhysicsName))
                        End If
                    Else
                        DirectCast(.getPtyValue(idxItem, .EntityData(idx)), EntityKoumoku_MojiType).Value = getDBValueForString(paramDt.Rows(idx).Item(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_MojiType).PhysicsName))
                    End If
                End With
            Next
        Next

        For entityCnt As Integer = 0 To CrsMasterEntity.EntityData.Length - 1
            paramClear()
            'データ未設定行はスキップ
            If CrsMasterEntity.EntityData(entityCnt).crsCd.Value = "" Then
                Continue For
            End If

            'SQL文生成
            sqlInsertString = "INSERT INTO " & "T_CRS_LEDGER_KOSHAKASHO ("
            sqlValueString = "VALUES("

            For idx As Integer = 0 To CrsMasterEntity.getPropertyDataLength - 1
                If idx > 0 Then
                    sqlInsertString &= ","
                    sqlValueString &= ","
                End If
                With CrsMasterEntity
                    If .getPtyType(idx, .EntityData(entityCnt)) Is GetType(EntityKoumoku_YmdType) Then
                        sqlInsertString &= DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_YmdType).PhysicsName
                        sqlValueString &= setParam(DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_YmdType).PhysicsName,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_YmdType).Value,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_YmdType).DBType)
                    ElseIf .getPtyType(idx, .EntityData(entityCnt)) Is GetType(EntityKoumoku_NumberType) Then
                        sqlInsertString &= DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_NumberType).PhysicsName
                        sqlValueString &= setParam(DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_NumberType).PhysicsName,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_NumberType).Value,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_NumberType).DBType,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_NumberType).IntegerBu,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_NumberType).DecimalBu)
                    ElseIf .getPtyType(idx, .EntityData(entityCnt)) Is GetType(EntityKoumoku_Number_DecimalType) Then
                        sqlInsertString &= DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_Number_DecimalType).PhysicsName
                        sqlValueString &= setParam(DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_Number_DecimalType).PhysicsName,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_Number_DecimalType).Value,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_Number_DecimalType).DBType,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_Number_DecimalType).IntegerBu,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_Number_DecimalType).DecimalBu)
                    Else
                        sqlInsertString &= DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_MojiType).PhysicsName
                        sqlValueString &= setParam(DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_MojiType).PhysicsName,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_MojiType).Value,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_MojiType).DBType,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_MojiType).IntegerBu)
                    End If
                End With
            Next
            sqlString = sqlInsertString & ") " & sqlValueString & ")"
            insertCnt = execNonQuery(paramTrans, sqlString)

            '一意制約違反等でInsertエラーの場合
            If insertCnt = 0 Then
                '更新元エラーコードで返却
                Return ERRCNT
            Else
                retValue += insertCnt
            End If
        Next

        Return retValue
    End Function

    ''' <summary>
    ''' コース台帳（コース情報）挿入
    ''' </summary>
    ''' <param name="paramDt">DataTable</param>
    ''' <param name="paramTrans">Oracleトランザクション</param>
    ''' <returns>更新件数(-1:エラー,0以上:正常)</returns>
    ''' <remarks></remarks>
    Private Function exeCuteInsertCrsInfoData(ByVal paramDt As DataTable, ByVal paramTrans As OracleTransaction) As Integer
        Dim CrsMasterEntity As EntityOperation(Of TCrsLedgerCrsInfoEntity) = New EntityOperation(Of TCrsLedgerCrsInfoEntity)
        Dim sqlString As String = ""  'sqlString
        Dim sqlInsertString As String = ""  'sqlInsertString
        Dim sqlValueString As String = ""  'sqlValueString
        Dim retValue As Integer  'retValue
        Dim insertCnt As Integer

        'DataTableをEntityに詰める
        For idx As Integer = 0 To paramDt.Rows.Count - 1
            If idx > 0 Then
                Dim entity As New TCrsLedgerCrsInfoEntity  'entity
                CrsMasterEntity.add(entity)
            End If
            For idxItem As Integer = 0 To CrsMasterEntity.getPropertyDataLength - 1
                With CrsMasterEntity
                    If .getPtyType(idxItem, .EntityData(0)) Is GetType(EntityKoumoku_YmdType) Then
                        If IsDBNull(paramDt.Rows(0).Item(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_YmdType).PhysicsName)) = False Then
                            DirectCast(.getPtyValue(idxItem, .EntityData(idx)), EntityKoumoku_YmdType).Value = CDate(paramDt.Rows(idx).Item(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_YmdType).PhysicsName))
                        End If
                    ElseIf .getPtyType(idxItem, .EntityData(0)) Is GetType(EntityKoumoku_NumberType) Then
                        If IsDBNull(paramDt.Rows(0).Item(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_NumberType).PhysicsName)) = False Then
                            DirectCast(.getPtyValue(idxItem, .EntityData(idx)), EntityKoumoku_NumberType).Value = CInt(paramDt.Rows(idx).Item(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_NumberType).PhysicsName))
                        End If
                    ElseIf .getPtyType(idxItem, .EntityData(0)) Is GetType(EntityKoumoku_Number_DecimalType) Then
                        If IsDBNull(paramDt.Rows(0).Item(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_Number_DecimalType).PhysicsName)) = False Then
                            DirectCast(.getPtyValue(idxItem, .EntityData(idx)), EntityKoumoku_Number_DecimalType).Value = CDec(paramDt.Rows(idx).Item(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_Number_DecimalType).PhysicsName))
                        End If
                    Else
                        DirectCast(.getPtyValue(idxItem, .EntityData(idx)), EntityKoumoku_MojiType).Value = getDBValueForString(paramDt.Rows(idx).Item(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_MojiType).PhysicsName))
                    End If
                End With
            Next
        Next

        For entityCnt As Integer = 0 To CrsMasterEntity.EntityData.Length - 1
            paramClear()
            'データ未設定行はスキップ
            If CrsMasterEntity.EntityData(entityCnt).crsCd.Value = "" Then
                Continue For
            End If

            'SQL文生成
            sqlInsertString = "INSERT INTO " & "T_CRS_LEDGER_CRS_INFO ("
            sqlValueString = "VALUES("

            For idx As Integer = 0 To CrsMasterEntity.getPropertyDataLength - 1
                If idx > 0 Then
                    sqlInsertString &= ","
                    sqlValueString &= ","
                End If
                With CrsMasterEntity
                    If .getPtyType(idx, .EntityData(entityCnt)) Is GetType(EntityKoumoku_YmdType) Then
                        sqlInsertString &= DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_YmdType).PhysicsName
                        sqlValueString &= setParam(DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_YmdType).PhysicsName,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_YmdType).Value,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_YmdType).DBType)
                    ElseIf .getPtyType(idx, .EntityData(entityCnt)) Is GetType(EntityKoumoku_NumberType) Then
                        sqlInsertString &= DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_NumberType).PhysicsName
                        sqlValueString &= setParam(DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_NumberType).PhysicsName,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_NumberType).Value,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_NumberType).DBType,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_NumberType).IntegerBu,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_NumberType).DecimalBu)
                    ElseIf .getPtyType(idx, .EntityData(entityCnt)) Is GetType(EntityKoumoku_Number_DecimalType) Then
                        sqlInsertString &= DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_Number_DecimalType).PhysicsName
                        sqlValueString &= setParam(DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_Number_DecimalType).PhysicsName,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_Number_DecimalType).Value,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_Number_DecimalType).DBType,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_Number_DecimalType).IntegerBu,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_Number_DecimalType).DecimalBu)
                    Else
                        sqlInsertString &= DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_MojiType).PhysicsName
                        sqlValueString &= setParam(DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_MojiType).PhysicsName,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_MojiType).Value,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_MojiType).DBType,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_MojiType).IntegerBu)
                    End If
                End With
            Next
            sqlString = sqlInsertString & ") " & sqlValueString & ")"
            insertCnt = execNonQuery(paramTrans, sqlString)

            '一意制約違反等でInsertエラーの場合
            If insertCnt = 0 Then
                '更新元エラーコードで返却
                Return ERRCNT
            Else
                retValue += insertCnt
            End If
        Next

        Return retValue
    End Function

    ''' <summary>
    ''' コース台帳（メッセージ）挿入
    ''' </summary>
    ''' <param name="paramDt">DataTable</param>
    ''' <param name="paramTrans">Oracleトランザクション</param>
    ''' <returns>更新件数(-1:エラー,0以上:正常)</returns>
    ''' <remarks></remarks>
    Private Function exeCuteInsertMessageData(ByVal paramDt As DataTable, ByVal paramTrans As OracleTransaction) As Integer
        Dim CrsMasterEntity As EntityOperation(Of TCrsLedgerMessageEntity) = New EntityOperation(Of TCrsLedgerMessageEntity)
        Dim sqlString As String = ""  'sqlString
        Dim sqlInsertString As String = ""  'sqlInsertString
        Dim sqlValueString As String = ""  'sqlValueString
        Dim retValue As Integer  'retValue
        Dim insertCnt As Integer

        'DataTableをEntityに詰める
        For idx As Integer = 0 To paramDt.Rows.Count - 1
            If idx > 0 Then
                Dim entity As New TCrsLedgerMessageEntity  'entity
                CrsMasterEntity.add(entity)
            End If
            For idxItem As Integer = 0 To CrsMasterEntity.getPropertyDataLength - 1
                With CrsMasterEntity
                    If .getPtyType(idxItem, .EntityData(0)) Is GetType(EntityKoumoku_YmdType) Then
                        If IsDBNull(paramDt.Rows(0).Item(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_YmdType).PhysicsName)) = False Then
                            DirectCast(.getPtyValue(idxItem, .EntityData(idx)), EntityKoumoku_YmdType).Value = CDate(paramDt.Rows(idx).Item(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_YmdType).PhysicsName))
                        End If
                    ElseIf .getPtyType(idxItem, .EntityData(0)) Is GetType(EntityKoumoku_NumberType) Then
                        If IsDBNull(paramDt.Rows(0).Item(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_NumberType).PhysicsName)) = False Then
                            DirectCast(.getPtyValue(idxItem, .EntityData(idx)), EntityKoumoku_NumberType).Value = CInt(paramDt.Rows(idx).Item(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_NumberType).PhysicsName))
                        End If
                    ElseIf .getPtyType(idxItem, .EntityData(0)) Is GetType(EntityKoumoku_Number_DecimalType) Then
                        If IsDBNull(paramDt.Rows(0).Item(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_Number_DecimalType).PhysicsName)) = False Then
                            DirectCast(.getPtyValue(idxItem, .EntityData(idx)), EntityKoumoku_Number_DecimalType).Value = CDec(paramDt.Rows(idx).Item(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_Number_DecimalType).PhysicsName))
                        End If
                    Else
                        DirectCast(.getPtyValue(idxItem, .EntityData(idx)), EntityKoumoku_MojiType).Value = getDBValueForString(paramDt.Rows(idx).Item(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_MojiType).PhysicsName))
                    End If
                End With
            Next
        Next

        For entityCnt As Integer = 0 To CrsMasterEntity.EntityData.Length - 1
            paramClear()
            'データ未設定行はスキップ
            If CrsMasterEntity.EntityData(entityCnt).crsCd.Value = "" Then
                Continue For
            End If

            'SQL文生成
            sqlInsertString = "INSERT INTO " & "T_CRS_LEDGER_MESSAGE ("
            sqlValueString = "VALUES("

            For idx As Integer = 0 To CrsMasterEntity.getPropertyDataLength - 1
                If idx > 0 Then
                    sqlInsertString &= ","
                    sqlValueString &= ","
                End If
                With CrsMasterEntity
                    If .getPtyType(idx, .EntityData(entityCnt)) Is GetType(EntityKoumoku_YmdType) Then
                        sqlInsertString &= DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_YmdType).PhysicsName
                        sqlValueString &= setParam(DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_YmdType).PhysicsName,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_YmdType).Value,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_YmdType).DBType)
                    ElseIf .getPtyType(idx, .EntityData(entityCnt)) Is GetType(EntityKoumoku_NumberType) Then
                        sqlInsertString &= DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_NumberType).PhysicsName
                        sqlValueString &= setParam(DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_NumberType).PhysicsName,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_NumberType).Value,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_NumberType).DBType,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_NumberType).IntegerBu,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_NumberType).DecimalBu)
                    ElseIf .getPtyType(idx, .EntityData(entityCnt)) Is GetType(EntityKoumoku_Number_DecimalType) Then
                        sqlInsertString &= DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_Number_DecimalType).PhysicsName
                        sqlValueString &= setParam(DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_Number_DecimalType).PhysicsName,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_Number_DecimalType).Value,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_Number_DecimalType).DBType,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_Number_DecimalType).IntegerBu,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_Number_DecimalType).DecimalBu)
                    Else
                        sqlInsertString &= DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_MojiType).PhysicsName
                        sqlValueString &= setParam(DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_MojiType).PhysicsName,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_MojiType).Value,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_MojiType).DBType,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_MojiType).IntegerBu)
                    End If
                End With
            Next
            sqlString = sqlInsertString & ") " & sqlValueString & ")"
            insertCnt = execNonQuery(paramTrans, sqlString)

            '一意制約違反等でInsertエラーの場合
            If insertCnt = 0 Then
                '更新元エラーコードで返却
                Return ERRCNT
            Else
                retValue += insertCnt
            End If
        Next

        Return retValue
    End Function

    ''' <summary>
    ''' コース台帳（料金）挿入
    ''' </summary>
    ''' <param name="paramDt">DataTable</param>
    ''' <param name="paramTrans">Oracleトランザクション</param>
    ''' <returns>更新件数(-1:エラー,0以上:正常)</returns>
    ''' <remarks></remarks>
    Private Function exeCuteInsertChargeData(ByVal paramDt As DataTable, ByVal paramTrans As OracleTransaction) As Integer
        Dim CrsMasterEntity As EntityOperation(Of TCrsLedgerChargeEntity) = New EntityOperation(Of TCrsLedgerChargeEntity)
        Dim sqlString As String = ""  'sqlString
        Dim sqlInsertString As String = ""  'sqlInsertString
        Dim sqlValueString As String = ""  'sqlValueString
        Dim retValue As Integer  'retValue
        Dim insertCnt As Integer

        'DataTableをEntityに詰める
        For idx As Integer = 0 To paramDt.Rows.Count - 1
            If idx > 0 Then
                Dim entity As New TCrsLedgerChargeEntity  'entity
                CrsMasterEntity.add(entity)
            End If
            For idxItem As Integer = 0 To CrsMasterEntity.getPropertyDataLength - 1
                With CrsMasterEntity
                    If .getPtyType(idxItem, .EntityData(0)) Is GetType(EntityKoumoku_YmdType) Then
                        If IsDBNull(paramDt.Rows(0).Item(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_YmdType).PhysicsName)) = False Then
                            DirectCast(.getPtyValue(idxItem, .EntityData(idx)), EntityKoumoku_YmdType).Value = CDate(paramDt.Rows(idx).Item(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_YmdType).PhysicsName))
                        End If
                    ElseIf .getPtyType(idxItem, .EntityData(0)) Is GetType(EntityKoumoku_NumberType) Then
                        If IsDBNull(paramDt.Rows(0).Item(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_NumberType).PhysicsName)) = False Then
                            DirectCast(.getPtyValue(idxItem, .EntityData(idx)), EntityKoumoku_NumberType).Value = CInt(paramDt.Rows(idx).Item(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_NumberType).PhysicsName))
                        End If
                    ElseIf .getPtyType(idxItem, .EntityData(0)) Is GetType(EntityKoumoku_Number_DecimalType) Then
                        If IsDBNull(paramDt.Rows(0).Item(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_Number_DecimalType).PhysicsName)) = False Then
                            DirectCast(.getPtyValue(idxItem, .EntityData(idx)), EntityKoumoku_Number_DecimalType).Value = CDec(paramDt.Rows(idx).Item(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_Number_DecimalType).PhysicsName))
                        End If
                    Else
                        DirectCast(.getPtyValue(idxItem, .EntityData(idx)), EntityKoumoku_MojiType).Value = getDBValueForString(paramDt.Rows(idx).Item(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_MojiType).PhysicsName))
                    End If
                End With
            Next
        Next

        For entityCnt As Integer = 0 To CrsMasterEntity.EntityData.Length - 1
            paramClear()
            'データ未設定行はスキップ
            If CrsMasterEntity.EntityData(entityCnt).crsCd.Value = "" Then
                Continue For
            End If

            'SQL文生成
            sqlInsertString = "INSERT INTO " & "T_CRS_LEDGER_CHARGE ("
            sqlValueString = "VALUES("

            For idx As Integer = 0 To CrsMasterEntity.getPropertyDataLength - 1
                If idx > 0 Then
                    sqlInsertString &= ","
                    sqlValueString &= ","
                End If
                With CrsMasterEntity
                    If .getPtyType(idx, .EntityData(entityCnt)) Is GetType(EntityKoumoku_YmdType) Then
                        sqlInsertString &= DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_YmdType).PhysicsName
                        sqlValueString &= setParam(DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_YmdType).PhysicsName,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_YmdType).Value,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_YmdType).DBType)
                    ElseIf .getPtyType(idx, .EntityData(entityCnt)) Is GetType(EntityKoumoku_NumberType) Then
                        sqlInsertString &= DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_NumberType).PhysicsName
                        sqlValueString &= setParam(DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_NumberType).PhysicsName,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_NumberType).Value,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_NumberType).DBType,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_NumberType).IntegerBu,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_NumberType).DecimalBu)
                    ElseIf .getPtyType(idx, .EntityData(entityCnt)) Is GetType(EntityKoumoku_Number_DecimalType) Then
                        sqlInsertString &= DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_Number_DecimalType).PhysicsName
                        sqlValueString &= setParam(DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_Number_DecimalType).PhysicsName,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_Number_DecimalType).Value,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_Number_DecimalType).DBType,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_Number_DecimalType).IntegerBu,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_Number_DecimalType).DecimalBu)
                    Else
                        sqlInsertString &= DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_MojiType).PhysicsName
                        sqlValueString &= setParam(DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_MojiType).PhysicsName,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_MojiType).Value,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_MojiType).DBType,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_MojiType).IntegerBu)
                    End If
                End With
            Next
            sqlString = sqlInsertString & ") " & sqlValueString & ")"
            insertCnt = execNonQuery(paramTrans, sqlString)

            '一意制約違反等でInsertエラーの場合
            If insertCnt = 0 Then
                '更新元エラーコードで返却
                Return ERRCNT
            Else
                retValue += insertCnt
            End If
        Next

        Return retValue
    End Function

    ''' <summary>
    ''' コース台帳（ホテル）挿入
    ''' </summary>
    ''' <param name="paramDt">DataTable</param>
    ''' <param name="paramTrans">Oracleトランザクション</param>
    ''' <returns>更新件数(-1:エラー,0以上:正常)</returns>
    ''' <remarks></remarks>
    Private Function exeCuteInsertHotelData(ByVal paramDt As DataTable, ByVal paramTrans As OracleTransaction) As Integer
        Dim CrsMasterEntity As EntityOperation(Of TCrsLedgerHotelEntity) = New EntityOperation(Of TCrsLedgerHotelEntity)
        Dim sqlString As String = ""  'sqlString
        Dim sqlInsertString As String = ""  'sqlInsertString
        Dim sqlValueString As String = ""  'sqlValueString
        Dim retValue As Integer  'retValue
        Dim insertCnt As Integer

        'DataTableをEntityに詰める
        For idx As Integer = 0 To paramDt.Rows.Count - 1
            If idx > 0 Then
                Dim entity As New TCrsLedgerHotelEntity  'entity
                CrsMasterEntity.add(entity)
            End If
            For idxItem As Integer = 0 To CrsMasterEntity.getPropertyDataLength - 1
                With CrsMasterEntity
                    If .getPtyType(idxItem, .EntityData(0)) Is GetType(EntityKoumoku_YmdType) Then
                        If IsDBNull(paramDt.Rows(0).Item(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_YmdType).PhysicsName)) = False Then
                            DirectCast(.getPtyValue(idxItem, .EntityData(idx)), EntityKoumoku_YmdType).Value = CDate(paramDt.Rows(idx).Item(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_YmdType).PhysicsName))
                        End If
                    ElseIf .getPtyType(idxItem, .EntityData(0)) Is GetType(EntityKoumoku_NumberType) Then
                        If IsDBNull(paramDt.Rows(0).Item(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_NumberType).PhysicsName)) = False Then
                            DirectCast(.getPtyValue(idxItem, .EntityData(idx)), EntityKoumoku_NumberType).Value = CInt(paramDt.Rows(idx).Item(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_NumberType).PhysicsName))
                        End If
                    ElseIf .getPtyType(idxItem, .EntityData(0)) Is GetType(EntityKoumoku_Number_DecimalType) Then
                        If IsDBNull(paramDt.Rows(0).Item(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_Number_DecimalType).PhysicsName)) = False Then
                            DirectCast(.getPtyValue(idxItem, .EntityData(idx)), EntityKoumoku_Number_DecimalType).Value = CDec(paramDt.Rows(idx).Item(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_Number_DecimalType).PhysicsName))
                        End If
                    Else
                        DirectCast(.getPtyValue(idxItem, .EntityData(idx)), EntityKoumoku_MojiType).Value = getDBValueForString(paramDt.Rows(idx).Item(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_MojiType).PhysicsName))
                    End If
                End With
            Next
        Next

        For entityCnt As Integer = 0 To CrsMasterEntity.EntityData.Length - 1
            paramClear()
            'データ未設定行はスキップ
            If CrsMasterEntity.EntityData(entityCnt).crsCd.Value = "" Then
                Continue For
            End If

            'SQL文生成
            sqlInsertString = "INSERT INTO " & "T_CRS_LEDGER_HOTEL ("
            sqlValueString = "VALUES("

            For idx As Integer = 0 To CrsMasterEntity.getPropertyDataLength - 1
                If idx > 0 Then
                    sqlInsertString &= ","
                    sqlValueString &= ","
                End If
                With CrsMasterEntity
                    If .getPtyType(idx, .EntityData(entityCnt)) Is GetType(EntityKoumoku_YmdType) Then
                        sqlInsertString &= DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_YmdType).PhysicsName
                        sqlValueString &= setParam(DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_YmdType).PhysicsName,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_YmdType).Value,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_YmdType).DBType)
                    ElseIf .getPtyType(idx, .EntityData(entityCnt)) Is GetType(EntityKoumoku_NumberType) Then
                        sqlInsertString &= DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_NumberType).PhysicsName
                        sqlValueString &= setParam(DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_NumberType).PhysicsName,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_NumberType).Value,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_NumberType).DBType,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_NumberType).IntegerBu,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_NumberType).DecimalBu)
                    ElseIf .getPtyType(idx, .EntityData(entityCnt)) Is GetType(EntityKoumoku_Number_DecimalType) Then
                        sqlInsertString &= DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_Number_DecimalType).PhysicsName
                        sqlValueString &= setParam(DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_Number_DecimalType).PhysicsName,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_Number_DecimalType).Value,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_Number_DecimalType).DBType,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_Number_DecimalType).IntegerBu,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_Number_DecimalType).DecimalBu)
                    Else
                        sqlInsertString &= DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_MojiType).PhysicsName
                        sqlValueString &= setParam(DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_MojiType).PhysicsName,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_MojiType).Value,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_MojiType).DBType,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_MojiType).IntegerBu)
                    End If
                End With
            Next
            sqlString = sqlInsertString & ") " & sqlValueString & ")"
            insertCnt = execNonQuery(paramTrans, sqlString)

            '一意制約違反等でInsertエラーの場合
            If insertCnt = 0 Then
                '更新元エラーコードで返却
                Return ERRCNT
            Else
                retValue += insertCnt
            End If
        Next

        Return retValue
    End Function

    ''' <summary>
    ''' コース台帳（付加情報）挿入
    ''' </summary>
    ''' <param name="paramDt">DataTable</param>
    ''' <param name="paramTrans">Oracleトランザクション</param>
    ''' <returns>更新件数(-1:エラー,0以上:正常)</returns>
    ''' <remarks></remarks>
    Private Function exeCuteInsertAddInfoData(ByVal paramDt As DataTable, ByVal paramTrans As OracleTransaction) As Integer
        Dim CrsMasterEntity As EntityOperation(Of TCrsLedgerAddInfoEntity) = New EntityOperation(Of TCrsLedgerAddInfoEntity)
        Dim sqlString As String = ""  'sqlString
        Dim sqlInsertString As String = ""  'sqlInsertString
        Dim sqlValueString As String = ""  'sqlValueString
        Dim retValue As Integer  'retValue
        Dim insertCnt As Integer

        'DataTableをEntityに詰める
        For idx As Integer = 0 To paramDt.Rows.Count - 1
            If idx > 0 Then
                Dim entity As New TCrsLedgerAddInfoEntity  'entity
                CrsMasterEntity.add(entity)
            End If
            For idxItem As Integer = 0 To CrsMasterEntity.getPropertyDataLength - 1
                With CrsMasterEntity
                    If .getPtyType(idxItem, .EntityData(0)) Is GetType(EntityKoumoku_YmdType) Then
                        If IsDBNull(paramDt.Rows(0).Item(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_YmdType).PhysicsName)) = False Then
                            DirectCast(.getPtyValue(idxItem, .EntityData(idx)), EntityKoumoku_YmdType).Value = CDate(paramDt.Rows(idx).Item(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_YmdType).PhysicsName))
                        End If
                    ElseIf .getPtyType(idxItem, .EntityData(0)) Is GetType(EntityKoumoku_NumberType) Then
                        If IsDBNull(paramDt.Rows(0).Item(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_NumberType).PhysicsName)) = False Then
                            DirectCast(.getPtyValue(idxItem, .EntityData(idx)), EntityKoumoku_NumberType).Value = CInt(paramDt.Rows(idx).Item(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_NumberType).PhysicsName))
                        End If
                    ElseIf .getPtyType(idxItem, .EntityData(0)) Is GetType(EntityKoumoku_Number_DecimalType) Then
                        If IsDBNull(paramDt.Rows(0).Item(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_Number_DecimalType).PhysicsName)) = False Then
                            DirectCast(.getPtyValue(idxItem, .EntityData(idx)), EntityKoumoku_Number_DecimalType).Value = CDec(paramDt.Rows(idx).Item(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_Number_DecimalType).PhysicsName))
                        End If
                    Else
                        DirectCast(.getPtyValue(idxItem, .EntityData(idx)), EntityKoumoku_MojiType).Value = getDBValueForString(paramDt.Rows(idx).Item(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_MojiType).PhysicsName))
                    End If
                End With
            Next
        Next

        For entityCnt As Integer = 0 To CrsMasterEntity.EntityData.Length - 1
            paramClear()
            'データ未設定行はスキップ
            If CrsMasterEntity.EntityData(entityCnt).crsCd.Value = "" Then
                Continue For
            End If

            'SQL文生成
            sqlInsertString = "INSERT INTO " & "T_CRS_LEDGER_ADD_INFO ("
            sqlValueString = "VALUES("

            For idx As Integer = 0 To CrsMasterEntity.getPropertyDataLength - 1
                If idx > 0 Then
                    sqlInsertString &= ","
                    sqlValueString &= ","
                End If
                With CrsMasterEntity
                    If .getPtyType(idx, .EntityData(entityCnt)) Is GetType(EntityKoumoku_YmdType) Then
                        sqlInsertString &= DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_YmdType).PhysicsName
                        sqlValueString &= setParam(DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_YmdType).PhysicsName,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_YmdType).Value,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_YmdType).DBType)
                    ElseIf .getPtyType(idx, .EntityData(entityCnt)) Is GetType(EntityKoumoku_NumberType) Then
                        sqlInsertString &= DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_NumberType).PhysicsName
                        sqlValueString &= setParam(DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_NumberType).PhysicsName,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_NumberType).Value,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_NumberType).DBType,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_NumberType).IntegerBu,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_NumberType).DecimalBu)
                    ElseIf .getPtyType(idx, .EntityData(entityCnt)) Is GetType(EntityKoumoku_Number_DecimalType) Then
                        sqlInsertString &= DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_Number_DecimalType).PhysicsName
                        sqlValueString &= setParam(DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_Number_DecimalType).PhysicsName,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_Number_DecimalType).Value,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_Number_DecimalType).DBType,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_Number_DecimalType).IntegerBu,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_Number_DecimalType).DecimalBu)
                    Else
                        sqlInsertString &= DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_MojiType).PhysicsName
                        sqlValueString &= setParam(DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_MojiType).PhysicsName,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_MojiType).Value,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_MojiType).DBType,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_MojiType).IntegerBu)
                    End If
                End With
            Next
            sqlString = sqlInsertString & ") " & sqlValueString & ")"
            insertCnt = execNonQuery(paramTrans, sqlString)

            '一意制約違反等でInsertエラーの場合
            If insertCnt = 0 Then
                '更新元エラーコードで返却
                Return ERRCNT
            Else
                retValue += insertCnt
            End If
        Next

        Return retValue
    End Function

    ''' <summary>
    ''' コース台帳（バス紐づけ）挿入
    ''' </summary>
    ''' <param name="paramDt">DataTable</param>
    ''' <param name="paramTrans">Oracleトランザクション</param>
    ''' <returns>更新件数(-1:エラー,0以上:正常)</returns>
    ''' <remarks></remarks>
    Private Function exeCuteInsertBusHimodukeData(ByVal paramDt As DataTable, ByVal paramTrans As OracleTransaction) As Integer
        Dim CrsMasterEntity As EntityOperation(Of TCrsLedgerBusHimodukeEntity) = New EntityOperation(Of TCrsLedgerBusHimodukeEntity)
        Dim sqlString As String = ""  'sqlString
        Dim sqlInsertString As String = ""  'sqlInsertString
        Dim sqlValueString As String = ""  'sqlValueString
        Dim retValue As Integer  'retValue
        Dim insertCnt As Integer

        'DataTableをEntityに詰める
        For idx As Integer = 0 To paramDt.Rows.Count - 1
            If idx > 0 Then
                Dim entity As New TCrsLedgerBusHimodukeEntity  'entity
                CrsMasterEntity.add(entity)
            End If
            For idxItem As Integer = 0 To CrsMasterEntity.getPropertyDataLength - 1
                With CrsMasterEntity
                    If .getPtyType(idxItem, .EntityData(0)) Is GetType(EntityKoumoku_YmdType) Then
                        If IsDBNull(paramDt.Rows(0).Item(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_YmdType).PhysicsName)) = False Then
                            DirectCast(.getPtyValue(idxItem, .EntityData(idx)), EntityKoumoku_YmdType).Value = CDate(paramDt.Rows(idx).Item(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_YmdType).PhysicsName))
                        End If
                    ElseIf .getPtyType(idxItem, .EntityData(0)) Is GetType(EntityKoumoku_NumberType) Then
                        If IsDBNull(paramDt.Rows(0).Item(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_NumberType).PhysicsName)) = False Then
                            DirectCast(.getPtyValue(idxItem, .EntityData(idx)), EntityKoumoku_NumberType).Value = CInt(paramDt.Rows(idx).Item(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_NumberType).PhysicsName))
                        End If
                    ElseIf .getPtyType(idxItem, .EntityData(0)) Is GetType(EntityKoumoku_Number_DecimalType) Then
                        If IsDBNull(paramDt.Rows(0).Item(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_Number_DecimalType).PhysicsName)) = False Then
                            DirectCast(.getPtyValue(idxItem, .EntityData(idx)), EntityKoumoku_Number_DecimalType).Value = CDec(paramDt.Rows(idx).Item(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_Number_DecimalType).PhysicsName))
                        End If
                    Else
                        DirectCast(.getPtyValue(idxItem, .EntityData(idx)), EntityKoumoku_MojiType).Value = getDBValueForString(paramDt.Rows(idx).Item(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_MojiType).PhysicsName))
                    End If
                End With
            Next
        Next

        For entityCnt As Integer = 0 To CrsMasterEntity.EntityData.Length - 1
            paramClear()
            'データ未設定行はスキップ
            If CrsMasterEntity.EntityData(entityCnt).crsCd.Value = "" Then
                Continue For
            End If

            'SQL文生成
            sqlInsertString = "INSERT INTO " & "T_CRS_LEDGER_BUS_HIMODUKE ("
            sqlValueString = "VALUES("

            For idx As Integer = 0 To CrsMasterEntity.getPropertyDataLength - 1
                If idx > 0 Then
                    sqlInsertString &= ","
                    sqlValueString &= ","
                End If
                With CrsMasterEntity
                    If .getPtyType(idx, .EntityData(entityCnt)) Is GetType(EntityKoumoku_YmdType) Then
                        sqlInsertString &= DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_YmdType).PhysicsName
                        sqlValueString &= setParam(DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_YmdType).PhysicsName,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_YmdType).Value,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_YmdType).DBType)
                    ElseIf .getPtyType(idx, .EntityData(entityCnt)) Is GetType(EntityKoumoku_NumberType) Then
                        sqlInsertString &= DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_NumberType).PhysicsName
                        sqlValueString &= setParam(DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_NumberType).PhysicsName,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_NumberType).Value,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_NumberType).DBType,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_NumberType).IntegerBu,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_NumberType).DecimalBu)
                    ElseIf .getPtyType(idx, .EntityData(entityCnt)) Is GetType(EntityKoumoku_Number_DecimalType) Then
                        sqlInsertString &= DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_Number_DecimalType).PhysicsName
                        sqlValueString &= setParam(DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_Number_DecimalType).PhysicsName,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_Number_DecimalType).Value,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_Number_DecimalType).DBType,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_Number_DecimalType).IntegerBu,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_Number_DecimalType).DecimalBu)
                    Else
                        sqlInsertString &= DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_MojiType).PhysicsName
                        sqlValueString &= setParam(DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_MojiType).PhysicsName,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_MojiType).Value,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_MojiType).DBType,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_MojiType).IntegerBu)
                    End If
                End With
            Next
            sqlString = sqlInsertString & ") " & sqlValueString & ")"
            insertCnt = execNonQuery(paramTrans, sqlString)

            '一意制約違反等でInsertエラーの場合
            If insertCnt = 0 Then
                '更新元エラーコードで返却
                Return ERRCNT
            Else
                retValue += insertCnt
            End If
        Next

        Return retValue
    End Function

    ''' <summary>
    ''' コース台帳（オプション）挿入
    ''' </summary>
    ''' <param name="paramDt">DataTable</param>
    ''' <param name="paramTrans">Oracleトランザクション</param>
    ''' <returns>更新件数(-1:エラー,0以上:正常)</returns>
    ''' <remarks></remarks>
    Private Function exeCuteInsertOptionData(ByVal paramDt As DataTable, ByVal paramTrans As OracleTransaction) As Integer
        Dim CrsMasterEntity As EntityOperation(Of TCrsLedgerOptionEntity) = New EntityOperation(Of TCrsLedgerOptionEntity)
        Dim sqlString As String = ""  'sqlString
        Dim sqlInsertString As String = ""  'sqlInsertString
        Dim sqlValueString As String = ""  'sqlValueString
        Dim retValue As Integer  'retValue
        Dim insertCnt As Integer

        'DataTableをEntityに詰める
        For idx As Integer = 0 To paramDt.Rows.Count - 1
            If idx > 0 Then
                Dim entity As New TCrsLedgerOptionEntity  'entity
                CrsMasterEntity.add(entity)
            End If
            For idxItem As Integer = 0 To CrsMasterEntity.getPropertyDataLength - 1
                With CrsMasterEntity
                    If .getPtyType(idxItem, .EntityData(0)) Is GetType(EntityKoumoku_YmdType) Then
                        If IsDBNull(paramDt.Rows(0).Item(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_YmdType).PhysicsName)) = False Then
                            DirectCast(.getPtyValue(idxItem, .EntityData(idx)), EntityKoumoku_YmdType).Value = CDate(paramDt.Rows(idx).Item(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_YmdType).PhysicsName))
                        End If
                    ElseIf .getPtyType(idxItem, .EntityData(0)) Is GetType(EntityKoumoku_NumberType) Then
                        If IsDBNull(paramDt.Rows(0).Item(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_NumberType).PhysicsName)) = False Then
                            DirectCast(.getPtyValue(idxItem, .EntityData(idx)), EntityKoumoku_NumberType).Value = CInt(paramDt.Rows(idx).Item(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_NumberType).PhysicsName))
                        End If
                    ElseIf .getPtyType(idxItem, .EntityData(0)) Is GetType(EntityKoumoku_Number_DecimalType) Then
                        If IsDBNull(paramDt.Rows(0).Item(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_Number_DecimalType).PhysicsName)) = False Then
                            DirectCast(.getPtyValue(idxItem, .EntityData(idx)), EntityKoumoku_Number_DecimalType).Value = CDec(paramDt.Rows(idx).Item(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_Number_DecimalType).PhysicsName))
                        End If
                    Else
                        DirectCast(.getPtyValue(idxItem, .EntityData(idx)), EntityKoumoku_MojiType).Value = getDBValueForString(paramDt.Rows(idx).Item(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_MojiType).PhysicsName))
                    End If
                End With
            Next
        Next

        For entityCnt As Integer = 0 To CrsMasterEntity.EntityData.Length - 1
            paramClear()
            'データ未設定行はスキップ
            If CrsMasterEntity.EntityData(entityCnt).crsCd.Value = "" Then
                Continue For
            End If

            'SQL文生成
            sqlInsertString = "INSERT INTO " & "T_CRS_LEDGER_OPTION ("
            sqlValueString = "VALUES("

            For idx As Integer = 0 To CrsMasterEntity.getPropertyDataLength - 1
                If idx > 0 Then
                    sqlInsertString &= ","
                    sqlValueString &= ","
                End If
                With CrsMasterEntity
                    If .getPtyType(idx, .EntityData(entityCnt)) Is GetType(EntityKoumoku_YmdType) Then
                        sqlInsertString &= DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_YmdType).PhysicsName
                        sqlValueString &= setParam(DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_YmdType).PhysicsName,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_YmdType).Value,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_YmdType).DBType)
                    ElseIf .getPtyType(idx, .EntityData(entityCnt)) Is GetType(EntityKoumoku_NumberType) Then
                        sqlInsertString &= DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_NumberType).PhysicsName
                        sqlValueString &= setParam(DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_NumberType).PhysicsName,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_NumberType).Value,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_NumberType).DBType,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_NumberType).IntegerBu,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_NumberType).DecimalBu)
                    ElseIf .getPtyType(idx, .EntityData(entityCnt)) Is GetType(EntityKoumoku_Number_DecimalType) Then
                        sqlInsertString &= DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_Number_DecimalType).PhysicsName
                        sqlValueString &= setParam(DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_Number_DecimalType).PhysicsName,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_Number_DecimalType).Value,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_Number_DecimalType).DBType,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_Number_DecimalType).IntegerBu,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_Number_DecimalType).DecimalBu)
                    Else
                        sqlInsertString &= DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_MojiType).PhysicsName
                        sqlValueString &= setParam(DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_MojiType).PhysicsName,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_MojiType).Value,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_MojiType).DBType,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_MojiType).IntegerBu)
                    End If
                End With
            Next
            sqlString = sqlInsertString & ") " & sqlValueString & ")"
            insertCnt = execNonQuery(paramTrans, sqlString)

            '一意制約違反等でInsertエラーの場合
            If insertCnt = 0 Then
                '更新元エラーコードで返却
                Return ERRCNT
            Else
                retValue += insertCnt
            End If
        Next

        Return retValue
    End Function

    ''' <summary>
    ''' コース台帳（ダイヤ）挿入
    ''' </summary>
    ''' <param name="paramDt">DataTable</param>
    ''' <param name="paramTrans">Oracleトランザクション</param>
    ''' <returns>更新件数(-1:エラー,0以上:正常)</returns>
    ''' <remarks></remarks>
    Private Function exeCuteInsertDiaData(ByVal paramDt As DataTable, ByVal paramTrans As OracleTransaction) As Integer
        Dim CrsMasterEntity As EntityOperation(Of TCrsLedgerDiaEntity) = New EntityOperation(Of TCrsLedgerDiaEntity)
        Dim sqlString As String = ""  'sqlString
        Dim sqlInsertString As String = ""  'sqlInsertString
        Dim sqlValueString As String = ""  'sqlValueString
        Dim retValue As Integer  'retValue
        Dim insertCnt As Integer

        'DataTableをEntityに詰める
        For idx As Integer = 0 To paramDt.Rows.Count - 1
            If idx > 0 Then
                Dim entity As New TCrsLedgerDiaEntity  'entity
                CrsMasterEntity.add(entity)
            End If
            For idxItem As Integer = 0 To CrsMasterEntity.getPropertyDataLength - 1
                With CrsMasterEntity
                    If .getPtyType(idxItem, .EntityData(0)) Is GetType(EntityKoumoku_YmdType) Then
                        If IsDBNull(paramDt.Rows(0).Item(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_YmdType).PhysicsName)) = False Then
                            DirectCast(.getPtyValue(idxItem, .EntityData(idx)), EntityKoumoku_YmdType).Value = CDate(paramDt.Rows(idx).Item(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_YmdType).PhysicsName))
                        End If
                    ElseIf .getPtyType(idxItem, .EntityData(0)) Is GetType(EntityKoumoku_NumberType) Then
                        If IsDBNull(paramDt.Rows(0).Item(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_NumberType).PhysicsName)) = False Then
                            DirectCast(.getPtyValue(idxItem, .EntityData(idx)), EntityKoumoku_NumberType).Value = CInt(paramDt.Rows(idx).Item(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_NumberType).PhysicsName))
                        End If
                    ElseIf .getPtyType(idxItem, .EntityData(0)) Is GetType(EntityKoumoku_Number_DecimalType) Then
                        If IsDBNull(paramDt.Rows(0).Item(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_Number_DecimalType).PhysicsName)) = False Then
                            DirectCast(.getPtyValue(idxItem, .EntityData(idx)), EntityKoumoku_Number_DecimalType).Value = CDec(paramDt.Rows(idx).Item(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_Number_DecimalType).PhysicsName))
                        End If
                    Else
                        DirectCast(.getPtyValue(idxItem, .EntityData(idx)), EntityKoumoku_MojiType).Value = getDBValueForString(paramDt.Rows(idx).Item(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_MojiType).PhysicsName))
                    End If
                End With
            Next
        Next

        For entityCnt As Integer = 0 To CrsMasterEntity.EntityData.Length - 1
            paramClear()
            'データ未設定行はスキップ
            If CrsMasterEntity.EntityData(entityCnt).crsCd.Value = "" Then
                Continue For
            End If

            'SQL文生成
            sqlInsertString = "INSERT INTO " & "T_CRS_LEDGER_DIA ("
            sqlValueString = "VALUES("

            For idx As Integer = 0 To CrsMasterEntity.getPropertyDataLength - 1
                If idx > 0 Then
                    sqlInsertString &= ","
                    sqlValueString &= ","
                End If
                With CrsMasterEntity
                    If .getPtyType(idx, .EntityData(entityCnt)) Is GetType(EntityKoumoku_YmdType) Then
                        sqlInsertString &= DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_YmdType).PhysicsName
                        sqlValueString &= setParam(DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_YmdType).PhysicsName,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_YmdType).Value,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_YmdType).DBType)
                    ElseIf .getPtyType(idx, .EntityData(entityCnt)) Is GetType(EntityKoumoku_NumberType) Then
                        sqlInsertString &= DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_NumberType).PhysicsName
                        sqlValueString &= setParam(DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_NumberType).PhysicsName,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_NumberType).Value,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_NumberType).DBType,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_NumberType).IntegerBu,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_NumberType).DecimalBu)
                    ElseIf .getPtyType(idx, .EntityData(entityCnt)) Is GetType(EntityKoumoku_Number_DecimalType) Then
                        sqlInsertString &= DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_Number_DecimalType).PhysicsName
                        sqlValueString &= setParam(DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_Number_DecimalType).PhysicsName,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_Number_DecimalType).Value,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_Number_DecimalType).DBType,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_Number_DecimalType).IntegerBu,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_Number_DecimalType).DecimalBu)
                    Else
                        sqlInsertString &= DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_MojiType).PhysicsName
                        sqlValueString &= setParam(DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_MojiType).PhysicsName,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_MojiType).Value,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_MojiType).DBType,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_MojiType).IntegerBu)
                    End If
                End With
            Next
            sqlString = sqlInsertString & ") " & sqlValueString & ")"
            insertCnt = execNonQuery(paramTrans, sqlString)

            '一意制約違反等でInsertエラーの場合
            If insertCnt = 0 Then
                '更新元エラーコードで返却
                Return ERRCNT
            Else
                retValue += insertCnt
            End If
        Next

        Return retValue
    End Function

    ''' <summary>
    ''' コース台帳（基本_料金区分）挿入
    ''' </summary>
    ''' <param name="paramDt">DataTable</param>
    ''' <param name="paramTrans">Oracleトランザクション</param>
    ''' <returns>更新件数(-1:エラー,0以上:正常)</returns>
    ''' <remarks></remarks>
    Private Function exeCuteInsertBasicChargeKbnData(ByVal paramDt As DataTable, ByVal paramTrans As OracleTransaction) As Integer
        Dim CrsMasterEntity As EntityOperation(Of TCrsLedgerBasicChargeKbnEntity) = New EntityOperation(Of TCrsLedgerBasicChargeKbnEntity)
        Dim sqlString As String = ""  'sqlString
        Dim sqlInsertString As String = ""  'sqlInsertString
        Dim sqlValueString As String = ""  'sqlValueString
        Dim retValue As Integer  'retValue
        Dim insertCnt As Integer

        'DataTableをEntityに詰める
        For idx As Integer = 0 To paramDt.Rows.Count - 1
            If idx > 0 Then
                Dim entity As New TCrsLedgerBasicChargeKbnEntity  'entity
                CrsMasterEntity.add(entity)
            End If
            For idxItem As Integer = 0 To CrsMasterEntity.getPropertyDataLength - 1
                With CrsMasterEntity
                    If .getPtyType(idxItem, .EntityData(0)) Is GetType(EntityKoumoku_YmdType) Then
                        If IsDBNull(paramDt.Rows(0).Item(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_YmdType).PhysicsName)) = False Then
                            DirectCast(.getPtyValue(idxItem, .EntityData(idx)), EntityKoumoku_YmdType).Value = CDate(paramDt.Rows(idx).Item(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_YmdType).PhysicsName))
                        End If
                    ElseIf .getPtyType(idxItem, .EntityData(0)) Is GetType(EntityKoumoku_NumberType) Then
                        If IsDBNull(paramDt.Rows(0).Item(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_NumberType).PhysicsName)) = False Then
                            DirectCast(.getPtyValue(idxItem, .EntityData(idx)), EntityKoumoku_NumberType).Value = CInt(paramDt.Rows(idx).Item(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_NumberType).PhysicsName))
                        End If
                    ElseIf .getPtyType(idxItem, .EntityData(0)) Is GetType(EntityKoumoku_Number_DecimalType) Then
                        If IsDBNull(paramDt.Rows(0).Item(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_Number_DecimalType).PhysicsName)) = False Then
                            DirectCast(.getPtyValue(idxItem, .EntityData(idx)), EntityKoumoku_Number_DecimalType).Value = CDec(paramDt.Rows(idx).Item(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_Number_DecimalType).PhysicsName))
                        End If
                    Else
                        DirectCast(.getPtyValue(idxItem, .EntityData(idx)), EntityKoumoku_MojiType).Value = getDBValueForString(paramDt.Rows(idx).Item(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_MojiType).PhysicsName))
                    End If
                End With
            Next
        Next

        For entityCnt As Integer = 0 To CrsMasterEntity.EntityData.Length - 1
            paramClear()
            'データ未設定行はスキップ
            If CrsMasterEntity.EntityData(entityCnt).crsCd.Value = "" Then
                Continue For
            End If

            'SQL文生成
            sqlInsertString = "INSERT INTO " & "T_CRS_LEDGER_BASIC_CHARGE_KBN ("
            sqlValueString = "VALUES("

            For idx As Integer = 0 To CrsMasterEntity.getPropertyDataLength - 1
                If idx > 0 Then
                    sqlInsertString &= ","
                    sqlValueString &= ","
                End If
                With CrsMasterEntity
                    If .getPtyType(idx, .EntityData(entityCnt)) Is GetType(EntityKoumoku_YmdType) Then
                        sqlInsertString &= DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_YmdType).PhysicsName
                        sqlValueString &= setParam(DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_YmdType).PhysicsName,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_YmdType).Value,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_YmdType).DBType)
                    ElseIf .getPtyType(idx, .EntityData(entityCnt)) Is GetType(EntityKoumoku_NumberType) Then
                        sqlInsertString &= DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_NumberType).PhysicsName
                        sqlValueString &= setParam(DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_NumberType).PhysicsName,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_NumberType).Value,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_NumberType).DBType,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_NumberType).IntegerBu,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_NumberType).DecimalBu)
                    ElseIf .getPtyType(idx, .EntityData(entityCnt)) Is GetType(EntityKoumoku_Number_DecimalType) Then
                        sqlInsertString &= DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_Number_DecimalType).PhysicsName
                        sqlValueString &= setParam(DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_Number_DecimalType).PhysicsName,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_Number_DecimalType).Value,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_Number_DecimalType).DBType,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_Number_DecimalType).IntegerBu,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_Number_DecimalType).DecimalBu)
                    Else
                        sqlInsertString &= DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_MojiType).PhysicsName
                        sqlValueString &= setParam(DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_MojiType).PhysicsName,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_MojiType).Value,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_MojiType).DBType,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_MojiType).IntegerBu)
                    End If
                End With
            Next
            sqlString = sqlInsertString & ") " & sqlValueString & ")"
            insertCnt = execNonQuery(paramTrans, sqlString)

            '一意制約違反等でInsertエラーの場合
            If insertCnt = 0 Then
                '更新元エラーコードで返却
                Return ERRCNT
            Else
                retValue += insertCnt
            End If
        Next

        Return retValue
    End Function

    ''' <summary>
    ''' コース台帳（料金_料金区分）挿入
    ''' </summary>
    ''' <param name="paramDt">DataTable</param>
    ''' <param name="paramTrans">Oracleトランザクション</param>
    ''' <returns>更新件数(-1:エラー,0以上:正常)</returns>
    ''' <remarks></remarks>
    Private Function exeCuteInsertChargeChargeKbnData(ByVal paramDt As DataTable, ByVal paramTrans As OracleTransaction) As Integer
        Dim CrsMasterEntity As EntityOperation(Of TCrsLedgerChargeChargeKbnEntity) = New EntityOperation(Of TCrsLedgerChargeChargeKbnEntity)
        Dim sqlString As String = ""  'sqlString
        Dim sqlInsertString As String = ""  'sqlInsertString
        Dim sqlValueString As String = ""  'sqlValueString
        Dim retValue As Integer  'retValue
        Dim insertCnt As Integer

        'DataTableをEntityに詰める
        For idx As Integer = 0 To paramDt.Rows.Count - 1
            If idx > 0 Then
                Dim entity As New TCrsLedgerChargeChargeKbnEntity  'entity
                CrsMasterEntity.add(entity)
            End If
            For idxItem As Integer = 0 To CrsMasterEntity.getPropertyDataLength - 1
                With CrsMasterEntity
                    If .getPtyType(idxItem, .EntityData(0)) Is GetType(EntityKoumoku_YmdType) Then
                        If IsDBNull(paramDt.Rows(0).Item(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_YmdType).PhysicsName)) = False Then
                            DirectCast(.getPtyValue(idxItem, .EntityData(idx)), EntityKoumoku_YmdType).Value = CDate(paramDt.Rows(idx).Item(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_YmdType).PhysicsName))
                        End If
                    ElseIf .getPtyType(idxItem, .EntityData(0)) Is GetType(EntityKoumoku_NumberType) Then
                        If IsDBNull(paramDt.Rows(0).Item(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_NumberType).PhysicsName)) = False Then
                            DirectCast(.getPtyValue(idxItem, .EntityData(idx)), EntityKoumoku_NumberType).Value = CInt(paramDt.Rows(idx).Item(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_NumberType).PhysicsName))
                        End If
                    ElseIf .getPtyType(idxItem, .EntityData(0)) Is GetType(EntityKoumoku_Number_DecimalType) Then
                        If IsDBNull(paramDt.Rows(0).Item(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_Number_DecimalType).PhysicsName)) = False Then
                            DirectCast(.getPtyValue(idxItem, .EntityData(idx)), EntityKoumoku_Number_DecimalType).Value = CDec(paramDt.Rows(idx).Item(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_Number_DecimalType).PhysicsName))
                        End If
                    Else
                        DirectCast(.getPtyValue(idxItem, .EntityData(idx)), EntityKoumoku_MojiType).Value = getDBValueForString(paramDt.Rows(idx).Item(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_MojiType).PhysicsName))
                    End If
                End With
            Next
        Next

        For entityCnt As Integer = 0 To CrsMasterEntity.EntityData.Length - 1
            paramClear()
            'データ未設定行はスキップ
            If CrsMasterEntity.EntityData(entityCnt).crsCd.Value = "" Then
                Continue For
            End If

            'SQL文生成
            sqlInsertString = "INSERT INTO " & "T_CRS_LEDGER_CHARGE_CHARGE_KBN ("
            sqlValueString = "VALUES("

            For idx As Integer = 0 To CrsMasterEntity.getPropertyDataLength - 1
                If idx > 0 Then
                    sqlInsertString &= ","
                    sqlValueString &= ","
                End If
                With CrsMasterEntity
                    If .getPtyType(idx, .EntityData(entityCnt)) Is GetType(EntityKoumoku_YmdType) Then
                        sqlInsertString &= DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_YmdType).PhysicsName
                        sqlValueString &= setParam(DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_YmdType).PhysicsName,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_YmdType).Value,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_YmdType).DBType)
                    ElseIf .getPtyType(idx, .EntityData(entityCnt)) Is GetType(EntityKoumoku_NumberType) Then
                        sqlInsertString &= DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_NumberType).PhysicsName
                        sqlValueString &= setParam(DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_NumberType).PhysicsName,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_NumberType).Value,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_NumberType).DBType,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_NumberType).IntegerBu,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_NumberType).DecimalBu)
                    ElseIf .getPtyType(idx, .EntityData(entityCnt)) Is GetType(EntityKoumoku_Number_DecimalType) Then
                        sqlInsertString &= DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_Number_DecimalType).PhysicsName
                        sqlValueString &= setParam(DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_Number_DecimalType).PhysicsName,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_Number_DecimalType).Value,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_Number_DecimalType).DBType,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_Number_DecimalType).IntegerBu,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_Number_DecimalType).DecimalBu)
                    Else
                        sqlInsertString &= DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_MojiType).PhysicsName
                        sqlValueString &= setParam(DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_MojiType).PhysicsName,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_MojiType).Value,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_MojiType).DBType,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_MojiType).IntegerBu)
                    End If
                End With
            Next
            sqlString = sqlInsertString & ") " & sqlValueString & ")"
            insertCnt = execNonQuery(paramTrans, sqlString)

            '一意制約違反等でInsertエラーの場合
            If insertCnt = 0 Then
                '更新元エラーコードで返却
                Return ERRCNT
            Else
                retValue += insertCnt
            End If
        Next

        Return retValue
    End Function

    ''' <summary>
    ''' コース台帳（オプショングループ）挿入
    ''' </summary>
    ''' <param name="paramDt">DataTable</param>
    ''' <param name="paramTrans">Oracleトランザクション</param>
    ''' <returns>更新件数(-1:エラー,0以上:正常)</returns>
    ''' <remarks></remarks>
    Private Function exeCuteInsertOptionGroupData(ByVal paramDt As DataTable, ByVal paramTrans As OracleTransaction) As Integer
        Dim CrsMasterEntity As EntityOperation(Of TCrsLedgerOptionGroupEntity) = New EntityOperation(Of TCrsLedgerOptionGroupEntity)
        Dim sqlString As String = ""  'sqlString
        Dim sqlInsertString As String = ""  'sqlInsertString
        Dim sqlValueString As String = ""  'sqlValueString
        Dim retValue As Integer  'retValue
        Dim insertCnt As Integer

        'DataTableをEntityに詰める
        For idx As Integer = 0 To paramDt.Rows.Count - 1
            If idx > 0 Then
                Dim entity As New TCrsLedgerOptionGroupEntity  'entity
                CrsMasterEntity.add(entity)
            End If
            For idxItem As Integer = 0 To CrsMasterEntity.getPropertyDataLength - 1
                With CrsMasterEntity
                    If .getPtyType(idxItem, .EntityData(0)) Is GetType(EntityKoumoku_YmdType) Then
                        If IsDBNull(paramDt.Rows(0).Item(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_YmdType).PhysicsName)) = False Then
                            DirectCast(.getPtyValue(idxItem, .EntityData(idx)), EntityKoumoku_YmdType).Value = CDate(paramDt.Rows(idx).Item(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_YmdType).PhysicsName))
                        End If
                    ElseIf .getPtyType(idxItem, .EntityData(0)) Is GetType(EntityKoumoku_NumberType) Then
                        If IsDBNull(paramDt.Rows(0).Item(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_NumberType).PhysicsName)) = False Then
                            DirectCast(.getPtyValue(idxItem, .EntityData(idx)), EntityKoumoku_NumberType).Value = CInt(paramDt.Rows(idx).Item(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_NumberType).PhysicsName))
                        End If
                    ElseIf .getPtyType(idxItem, .EntityData(0)) Is GetType(EntityKoumoku_Number_DecimalType) Then
                        If IsDBNull(paramDt.Rows(0).Item(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_Number_DecimalType).PhysicsName)) = False Then
                            DirectCast(.getPtyValue(idxItem, .EntityData(idx)), EntityKoumoku_Number_DecimalType).Value = CDec(paramDt.Rows(idx).Item(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_Number_DecimalType).PhysicsName))
                        End If
                    Else
                        DirectCast(.getPtyValue(idxItem, .EntityData(idx)), EntityKoumoku_MojiType).Value = getDBValueForString(paramDt.Rows(idx).Item(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_MojiType).PhysicsName))
                    End If
                End With
            Next
        Next

        For entityCnt As Integer = 0 To CrsMasterEntity.EntityData.Length - 1
            paramClear()
            'データ未設定行はスキップ
            If CrsMasterEntity.EntityData(entityCnt).crsCd.Value = "" Then
                Continue For
            End If

            'SQL文生成
            sqlInsertString = "INSERT INTO " & "T_CRS_LEDGER_OPTION_GROUP ("
            sqlValueString = "VALUES("

            For idx As Integer = 0 To CrsMasterEntity.getPropertyDataLength - 1
                If idx > 0 Then
                    sqlInsertString &= ","
                    sqlValueString &= ","
                End If
                With CrsMasterEntity
                    If .getPtyType(idx, .EntityData(entityCnt)) Is GetType(EntityKoumoku_YmdType) Then
                        sqlInsertString &= DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_YmdType).PhysicsName
                        sqlValueString &= setParam(DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_YmdType).PhysicsName,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_YmdType).Value,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_YmdType).DBType)
                    ElseIf .getPtyType(idx, .EntityData(entityCnt)) Is GetType(EntityKoumoku_NumberType) Then
                        sqlInsertString &= DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_NumberType).PhysicsName
                        sqlValueString &= setParam(DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_NumberType).PhysicsName,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_NumberType).Value,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_NumberType).DBType,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_NumberType).IntegerBu,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_NumberType).DecimalBu)
                    ElseIf .getPtyType(idx, .EntityData(entityCnt)) Is GetType(EntityKoumoku_Number_DecimalType) Then
                        sqlInsertString &= DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_Number_DecimalType).PhysicsName
                        sqlValueString &= setParam(DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_Number_DecimalType).PhysicsName,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_Number_DecimalType).Value,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_Number_DecimalType).DBType,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_Number_DecimalType).IntegerBu,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_Number_DecimalType).DecimalBu)
                    Else
                        sqlInsertString &= DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_MojiType).PhysicsName
                        sqlValueString &= setParam(DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_MojiType).PhysicsName,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_MojiType).Value,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_MojiType).DBType,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_MojiType).IntegerBu)
                    End If
                End With
            Next
            sqlString = sqlInsertString & ") " & sqlValueString & ")"
            insertCnt = execNonQuery(paramTrans, sqlString)

            '一意制約違反等でInsertエラーの場合
            If insertCnt = 0 Then
                '更新元エラーコードで返却
                Return ERRCNT
            Else
                retValue += insertCnt
            End If
        Next

        Return retValue
    End Function

    ''' <summary>
    ''' コース台帳（販売課所）挿入
    ''' </summary>
    ''' <param name="paramDt">DataTable</param>
    ''' <param name="paramTrans">Oracleトランザクション</param>
    ''' <returns>更新件数(-1:エラー,0以上:正常)</returns>
    ''' <remarks></remarks>
    Private Function exeCuteInsertKashoData(ByVal paramDt As DataTable, ByVal paramTrans As OracleTransaction) As Integer
        Dim CrsMasterEntity As EntityOperation(Of TCrsLedgerKashoEntity) = New EntityOperation(Of TCrsLedgerKashoEntity)
        Dim sqlString As String = ""  'sqlString
        Dim sqlInsertString As String = ""  'sqlInsertString
        Dim sqlValueString As String = ""  'sqlValueString
        Dim retValue As Integer  'retValue
        Dim insertCnt As Integer

        'DataTableをEntityに詰める
        For idx As Integer = 0 To paramDt.Rows.Count - 1
            If idx > 0 Then
                Dim entity As New TCrsLedgerKashoEntity  'entity
                CrsMasterEntity.add(entity)
            End If
            For idxItem As Integer = 0 To CrsMasterEntity.getPropertyDataLength - 1
                With CrsMasterEntity
                    If .getPtyType(idxItem, .EntityData(0)) Is GetType(EntityKoumoku_YmdType) Then
                        If IsDBNull(paramDt.Rows(0).Item(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_YmdType).PhysicsName)) = False Then
                            DirectCast(.getPtyValue(idxItem, .EntityData(idx)), EntityKoumoku_YmdType).Value = CDate(paramDt.Rows(idx).Item(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_YmdType).PhysicsName))
                        End If
                    ElseIf .getPtyType(idxItem, .EntityData(0)) Is GetType(EntityKoumoku_NumberType) Then
                        If IsDBNull(paramDt.Rows(0).Item(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_NumberType).PhysicsName)) = False Then
                            DirectCast(.getPtyValue(idxItem, .EntityData(idx)), EntityKoumoku_NumberType).Value = CInt(paramDt.Rows(idx).Item(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_NumberType).PhysicsName))
                        End If
                    ElseIf .getPtyType(idxItem, .EntityData(0)) Is GetType(EntityKoumoku_Number_DecimalType) Then
                        If IsDBNull(paramDt.Rows(0).Item(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_Number_DecimalType).PhysicsName)) = False Then
                            DirectCast(.getPtyValue(idxItem, .EntityData(idx)), EntityKoumoku_Number_DecimalType).Value = CDec(paramDt.Rows(idx).Item(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_Number_DecimalType).PhysicsName))
                        End If
                    Else
                        DirectCast(.getPtyValue(idxItem, .EntityData(idx)), EntityKoumoku_MojiType).Value = getDBValueForString(paramDt.Rows(idx).Item(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_MojiType).PhysicsName))
                    End If
                End With
            Next
        Next

        For entityCnt As Integer = 0 To CrsMasterEntity.EntityData.Length - 1
            paramClear()
            'データ未設定行はスキップ
            If CrsMasterEntity.EntityData(entityCnt).crsCd.Value = "" Then
                Continue For
            End If

            'SQL文生成
            sqlInsertString = "INSERT INTO " & "T_CRS_LEDGER_KASHO ("
            sqlValueString = "VALUES("

            For idx As Integer = 0 To CrsMasterEntity.getPropertyDataLength - 1
                If idx > 0 Then
                    sqlInsertString &= ","
                    sqlValueString &= ","
                End If
                With CrsMasterEntity
                    If .getPtyType(idx, .EntityData(entityCnt)) Is GetType(EntityKoumoku_YmdType) Then
                        sqlInsertString &= DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_YmdType).PhysicsName
                        sqlValueString &= setParam(DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_YmdType).PhysicsName,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_YmdType).Value,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_YmdType).DBType)
                    ElseIf .getPtyType(idx, .EntityData(entityCnt)) Is GetType(EntityKoumoku_NumberType) Then
                        sqlInsertString &= DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_NumberType).PhysicsName
                        sqlValueString &= setParam(DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_NumberType).PhysicsName,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_NumberType).Value,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_NumberType).DBType,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_NumberType).IntegerBu,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_NumberType).DecimalBu)
                    ElseIf .getPtyType(idx, .EntityData(entityCnt)) Is GetType(EntityKoumoku_Number_DecimalType) Then
                        sqlInsertString &= DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_Number_DecimalType).PhysicsName
                        sqlValueString &= setParam(DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_Number_DecimalType).PhysicsName,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_Number_DecimalType).Value,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_Number_DecimalType).DBType,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_Number_DecimalType).IntegerBu,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_Number_DecimalType).DecimalBu)
                    Else
                        sqlInsertString &= DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_MojiType).PhysicsName
                        sqlValueString &= setParam(DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_MojiType).PhysicsName,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_MojiType).Value,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_MojiType).DBType,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_MojiType).IntegerBu)
                    End If
                End With
            Next
            sqlString = sqlInsertString & ") " & sqlValueString & ")"
            insertCnt = execNonQuery(paramTrans, sqlString)

            '一意制約違反等でInsertエラーの場合
            If insertCnt = 0 Then
                '更新元エラーコードで返却
                Return ERRCNT
            Else
                retValue += insertCnt
            End If
        Next

        Return retValue
    End Function

    ''' <summary>
    ''' コース台帳（リマークス）挿入
    ''' </summary>
    ''' <param name="paramDt">DataTable</param>
    ''' <param name="paramTrans">Oracleトランザクション</param>
    ''' <returns>更新件数(-1:エラー,0以上:正常)</returns>
    ''' <remarks></remarks>
    Private Function exeCuteInsertRemarksData(ByVal paramDt As DataTable, ByVal paramTrans As OracleTransaction) As Integer
        Dim CrsMasterEntity As EntityOperation(Of TCrsLedgerRemarksEntity) = New EntityOperation(Of TCrsLedgerRemarksEntity)
        Dim sqlString As String = ""  'sqlString
        Dim sqlInsertString As String = ""  'sqlInsertString
        Dim sqlValueString As String = ""  'sqlValueString
        Dim retValue As Integer  'retValue
        Dim insertCnt As Integer

        'DataTableをEntityに詰める
        For idx As Integer = 0 To paramDt.Rows.Count - 1
            If idx > 0 Then
                Dim entity As New TCrsLedgerRemarksEntity  'entity
                CrsMasterEntity.add(entity)
            End If
            For idxItem As Integer = 0 To CrsMasterEntity.getPropertyDataLength - 1
                With CrsMasterEntity
                    If .getPtyType(idxItem, .EntityData(0)) Is GetType(EntityKoumoku_YmdType) Then
                        If IsDBNull(paramDt.Rows(0).Item(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_YmdType).PhysicsName)) = False Then
                            DirectCast(.getPtyValue(idxItem, .EntityData(idx)), EntityKoumoku_YmdType).Value = CDate(paramDt.Rows(idx).Item(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_YmdType).PhysicsName))
                        End If
                    ElseIf .getPtyType(idxItem, .EntityData(0)) Is GetType(EntityKoumoku_NumberType) Then
                        If IsDBNull(paramDt.Rows(0).Item(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_NumberType).PhysicsName)) = False Then
                            DirectCast(.getPtyValue(idxItem, .EntityData(idx)), EntityKoumoku_NumberType).Value = CInt(paramDt.Rows(idx).Item(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_NumberType).PhysicsName))
                        End If
                    ElseIf .getPtyType(idxItem, .EntityData(0)) Is GetType(EntityKoumoku_Number_DecimalType) Then
                        If IsDBNull(paramDt.Rows(0).Item(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_Number_DecimalType).PhysicsName)) = False Then
                            DirectCast(.getPtyValue(idxItem, .EntityData(idx)), EntityKoumoku_Number_DecimalType).Value = CDec(paramDt.Rows(idx).Item(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_Number_DecimalType).PhysicsName))
                        End If
                    Else
                        DirectCast(.getPtyValue(idxItem, .EntityData(idx)), EntityKoumoku_MojiType).Value = getDBValueForString(paramDt.Rows(idx).Item(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_MojiType).PhysicsName))
                    End If
                End With
            Next
        Next

        For entityCnt As Integer = 0 To CrsMasterEntity.EntityData.Length - 1
            paramClear()
            'データ未設定行はスキップ
            If CrsMasterEntity.EntityData(entityCnt).crsCd.Value = "" Then
                Continue For
            End If

            'SQL文生成
            sqlInsertString = "INSERT INTO " & "T_CRS_LEDGER_REMARKS ("
            sqlValueString = "VALUES("

            For idx As Integer = 0 To CrsMasterEntity.getPropertyDataLength - 1
                If idx > 0 Then
                    sqlInsertString &= ","
                    sqlValueString &= ","
                End If
                With CrsMasterEntity
                    If .getPtyType(idx, .EntityData(entityCnt)) Is GetType(EntityKoumoku_YmdType) Then
                        sqlInsertString &= DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_YmdType).PhysicsName
                        sqlValueString &= setParam(DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_YmdType).PhysicsName,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_YmdType).Value,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_YmdType).DBType)
                    ElseIf .getPtyType(idx, .EntityData(entityCnt)) Is GetType(EntityKoumoku_NumberType) Then
                        sqlInsertString &= DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_NumberType).PhysicsName
                        sqlValueString &= setParam(DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_NumberType).PhysicsName,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_NumberType).Value,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_NumberType).DBType,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_NumberType).IntegerBu,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_NumberType).DecimalBu)
                    ElseIf .getPtyType(idx, .EntityData(entityCnt)) Is GetType(EntityKoumoku_Number_DecimalType) Then
                        sqlInsertString &= DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_Number_DecimalType).PhysicsName
                        sqlValueString &= setParam(DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_Number_DecimalType).PhysicsName,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_Number_DecimalType).Value,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_Number_DecimalType).DBType,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_Number_DecimalType).IntegerBu,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_Number_DecimalType).DecimalBu)
                    Else
                        sqlInsertString &= DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_MojiType).PhysicsName
                        sqlValueString &= setParam(DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_MojiType).PhysicsName,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_MojiType).Value,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_MojiType).DBType,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_MojiType).IntegerBu)
                    End If
                End With
            Next
            sqlString = sqlInsertString & ") " & sqlValueString & ")"
            insertCnt = execNonQuery(paramTrans, sqlString)

            '一意制約違反等でInsertエラーの場合
            If insertCnt = 0 Then
                '更新元エラーコードで返却
                Return ERRCNT
            Else
                retValue += insertCnt
            End If
        Next

        Return retValue
    End Function

    ''' <summary>
    ''' コース台帳原価（基本）挿入
    ''' </summary>
    ''' <param name="paramDt">DataTable</param>
    ''' <param name="paramTrans">Oracleトランザクション</param>
    ''' <returns>更新件数(-1:エラー,0以上:正常)</returns>
    ''' <remarks></remarks>
    Private Function exeCuteInsertCostBasicData(ByVal paramDt As DataTable, ByVal paramTrans As OracleTransaction) As Integer
        Dim CrsMasterEntity As EntityOperation(Of TCrsLedgerCostBasicEntity) = New EntityOperation(Of TCrsLedgerCostBasicEntity)
        Dim sqlString As String = ""  'sqlString
        Dim sqlInsertString As String = ""  'sqlInsertString
        Dim sqlValueString As String = ""  'sqlValueString
        Dim retValue As Integer  'retValue
        Dim insertCnt As Integer

        'DataTableをEntityに詰める
        For idx As Integer = 0 To paramDt.Rows.Count - 1
            If idx > 0 Then
                Dim entity As New TCrsLedgerCostBasicEntity  'entity
                CrsMasterEntity.add(entity)
            End If
            For idxItem As Integer = 0 To CrsMasterEntity.getPropertyDataLength - 1
                With CrsMasterEntity
                    If .getPtyType(idxItem, .EntityData(0)) Is GetType(EntityKoumoku_YmdType) Then
                        If IsDBNull(paramDt.Rows(0).Item(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_YmdType).PhysicsName)) = False Then
                            DirectCast(.getPtyValue(idxItem, .EntityData(idx)), EntityKoumoku_YmdType).Value = CDate(paramDt.Rows(idx).Item(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_YmdType).PhysicsName))
                        End If
                    ElseIf .getPtyType(idxItem, .EntityData(0)) Is GetType(EntityKoumoku_NumberType) Then
                        If IsDBNull(paramDt.Rows(0).Item(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_NumberType).PhysicsName)) = False Then
                            DirectCast(.getPtyValue(idxItem, .EntityData(idx)), EntityKoumoku_NumberType).Value = CInt(paramDt.Rows(idx).Item(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_NumberType).PhysicsName))
                        End If
                    ElseIf .getPtyType(idxItem, .EntityData(0)) Is GetType(EntityKoumoku_Number_DecimalType) Then
                        If IsDBNull(paramDt.Rows(0).Item(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_Number_DecimalType).PhysicsName)) = False Then
                            DirectCast(.getPtyValue(idxItem, .EntityData(idx)), EntityKoumoku_Number_DecimalType).Value = CDec(paramDt.Rows(idx).Item(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_Number_DecimalType).PhysicsName))
                        End If
                    Else
                        DirectCast(.getPtyValue(idxItem, .EntityData(idx)), EntityKoumoku_MojiType).Value = getDBValueForString(paramDt.Rows(idx).Item(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_MojiType).PhysicsName))
                    End If
                End With
            Next
        Next

        For entityCnt As Integer = 0 To CrsMasterEntity.EntityData.Length - 1
            paramClear()
            'データ未設定行はスキップ
            If CrsMasterEntity.EntityData(entityCnt).crsCd.Value = "" Then
                Continue For
            End If

            'SQL文生成
            sqlInsertString = "INSERT INTO " & "T_CRS_LEDGER_COST_BASIC ("
            sqlValueString = "VALUES("

            For idx As Integer = 0 To CrsMasterEntity.getPropertyDataLength - 1
                If idx > 0 Then
                    sqlInsertString &= ","
                    sqlValueString &= ","
                End If
                With CrsMasterEntity
                    If .getPtyType(idx, .EntityData(entityCnt)) Is GetType(EntityKoumoku_YmdType) Then
                        sqlInsertString &= DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_YmdType).PhysicsName
                        sqlValueString &= setParam(DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_YmdType).PhysicsName,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_YmdType).Value,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_YmdType).DBType)
                    ElseIf .getPtyType(idx, .EntityData(entityCnt)) Is GetType(EntityKoumoku_NumberType) Then
                        sqlInsertString &= DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_NumberType).PhysicsName
                        sqlValueString &= setParam(DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_NumberType).PhysicsName,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_NumberType).Value,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_NumberType).DBType,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_NumberType).IntegerBu,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_NumberType).DecimalBu)
                    ElseIf .getPtyType(idx, .EntityData(entityCnt)) Is GetType(EntityKoumoku_Number_DecimalType) Then
                        sqlInsertString &= DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_Number_DecimalType).PhysicsName
                        sqlValueString &= setParam(DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_Number_DecimalType).PhysicsName,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_Number_DecimalType).Value,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_Number_DecimalType).DBType,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_Number_DecimalType).IntegerBu,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_Number_DecimalType).DecimalBu)
                    Else
                        sqlInsertString &= DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_MojiType).PhysicsName
                        sqlValueString &= setParam(DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_MojiType).PhysicsName,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_MojiType).Value,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_MojiType).DBType,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_MojiType).IntegerBu)
                    End If
                End With
            Next
            sqlString = sqlInsertString & ") " & sqlValueString & ")"
            insertCnt = execNonQuery(paramTrans, sqlString)

            '一意制約違反等でInsertエラーの場合
            If insertCnt = 0 Then
                '更新元エラーコードで返却
                Return ERRCNT
            Else
                retValue += insertCnt
            End If
        Next

        Return retValue
    End Function

    ''' <summary>
    ''' コース台帳原価（降車ヶ所）挿入
    ''' </summary>
    ''' <param name="paramDt">DataTable</param>
    ''' <param name="paramTrans">Oracleトランザクション</param>
    ''' <returns>更新件数(-1:エラー,0以上:正常)</returns>
    ''' <remarks></remarks>
    Private Function exeCuteInsertCostKoshakashoData(ByVal paramDt As DataTable, ByVal paramTrans As OracleTransaction) As Integer
        Dim CrsMasterEntity As EntityOperation(Of TCrsLedgerCostKoshakashoEntity) = New EntityOperation(Of TCrsLedgerCostKoshakashoEntity)
        Dim sqlString As String = ""  'sqlString
        Dim sqlInsertString As String = ""  'sqlInsertString
        Dim sqlValueString As String = ""  'sqlValueString
        Dim retValue As Integer  'retValue
        Dim insertCnt As Integer

        'DataTableをEntityに詰める
        For idx As Integer = 0 To paramDt.Rows.Count - 1
            If idx > 0 Then
                Dim entity As New TCrsLedgerCostKoshakashoEntity  'entity
                CrsMasterEntity.add(entity)
            End If
            For idxItem As Integer = 0 To CrsMasterEntity.getPropertyDataLength - 1
                With CrsMasterEntity
                    If .getPtyType(idxItem, .EntityData(0)) Is GetType(EntityKoumoku_YmdType) Then
                        If IsDBNull(paramDt.Rows(0).Item(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_YmdType).PhysicsName)) = False Then
                            DirectCast(.getPtyValue(idxItem, .EntityData(idx)), EntityKoumoku_YmdType).Value = CDate(paramDt.Rows(idx).Item(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_YmdType).PhysicsName))
                        End If
                    ElseIf .getPtyType(idxItem, .EntityData(0)) Is GetType(EntityKoumoku_NumberType) Then
                        If IsDBNull(paramDt.Rows(0).Item(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_NumberType).PhysicsName)) = False Then
                            DirectCast(.getPtyValue(idxItem, .EntityData(idx)), EntityKoumoku_NumberType).Value = CInt(paramDt.Rows(idx).Item(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_NumberType).PhysicsName))
                        End If
                    ElseIf .getPtyType(idxItem, .EntityData(0)) Is GetType(EntityKoumoku_Number_DecimalType) Then
                        If IsDBNull(paramDt.Rows(0).Item(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_Number_DecimalType).PhysicsName)) = False Then
                            DirectCast(.getPtyValue(idxItem, .EntityData(idx)), EntityKoumoku_Number_DecimalType).Value = CDec(paramDt.Rows(idx).Item(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_Number_DecimalType).PhysicsName))
                        End If
                    Else
                        DirectCast(.getPtyValue(idxItem, .EntityData(idx)), EntityKoumoku_MojiType).Value = getDBValueForString(paramDt.Rows(idx).Item(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_MojiType).PhysicsName))
                    End If
                End With
            Next
        Next

        For entityCnt As Integer = 0 To CrsMasterEntity.EntityData.Length - 1
            paramClear()
            'データ未設定行はスキップ
            If CrsMasterEntity.EntityData(entityCnt).crsCd.Value = "" Then
                Continue For
            End If

            'SQL文生成
            sqlInsertString = "INSERT INTO " & "T_CRS_LEDGER_COST_KOSHAKASHO ("
            sqlValueString = "VALUES("

            For idx As Integer = 0 To CrsMasterEntity.getPropertyDataLength - 1
                If idx > 0 Then
                    sqlInsertString &= ","
                    sqlValueString &= ","
                End If
                With CrsMasterEntity
                    If .getPtyType(idx, .EntityData(entityCnt)) Is GetType(EntityKoumoku_YmdType) Then
                        sqlInsertString &= DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_YmdType).PhysicsName
                        sqlValueString &= setParam(DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_YmdType).PhysicsName,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_YmdType).Value,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_YmdType).DBType)
                    ElseIf .getPtyType(idx, .EntityData(entityCnt)) Is GetType(EntityKoumoku_NumberType) Then
                        sqlInsertString &= DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_NumberType).PhysicsName
                        sqlValueString &= setParam(DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_NumberType).PhysicsName,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_NumberType).Value,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_NumberType).DBType,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_NumberType).IntegerBu,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_NumberType).DecimalBu)
                    ElseIf .getPtyType(idx, .EntityData(entityCnt)) Is GetType(EntityKoumoku_Number_DecimalType) Then
                        sqlInsertString &= DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_Number_DecimalType).PhysicsName
                        sqlValueString &= setParam(DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_Number_DecimalType).PhysicsName,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_Number_DecimalType).Value,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_Number_DecimalType).DBType,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_Number_DecimalType).IntegerBu,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_Number_DecimalType).DecimalBu)
                    Else
                        sqlInsertString &= DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_MojiType).PhysicsName
                        sqlValueString &= setParam(DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_MojiType).PhysicsName,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_MojiType).Value,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_MojiType).DBType,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_MojiType).IntegerBu)
                    End If
                End With
            Next
            sqlString = sqlInsertString & ") " & sqlValueString & ")"
            insertCnt = execNonQuery(paramTrans, sqlString)

            '一意制約違反等でInsertエラーの場合
            If insertCnt = 0 Then
                '更新元エラーコードで返却
                Return ERRCNT
            Else
                retValue += insertCnt
            End If
        Next

        Return retValue
    End Function

    ''' <summary>
    ''' コース台帳原価（キャリア）挿入
    ''' </summary>
    ''' <param name="paramDt">DataTable</param>
    ''' <param name="paramTrans">Oracleトランザクション</param>
    ''' <returns>更新件数(-1:エラー,0以上:正常)</returns>
    ''' <remarks></remarks>
    Private Function exeCuteInsertCostCarrierData(ByVal paramDt As DataTable, ByVal paramTrans As OracleTransaction) As Integer
        Dim CrsMasterEntity As EntityOperation(Of TCrsLedgerCostCarrierEntity) = New EntityOperation(Of TCrsLedgerCostCarrierEntity)
        Dim sqlString As String = ""  'sqlString
        Dim sqlInsertString As String = ""  'sqlInsertString
        Dim sqlValueString As String = ""  'sqlValueString
        Dim retValue As Integer  'retValue
        Dim insertCnt As Integer

        'DataTableをEntityに詰める
        For idx As Integer = 0 To paramDt.Rows.Count - 1
            If idx > 0 Then
                Dim entity As New TCrsLedgerCostCarrierEntity  'entity
                CrsMasterEntity.add(entity)
            End If
            For idxItem As Integer = 0 To CrsMasterEntity.getPropertyDataLength - 1
                With CrsMasterEntity
                    If .getPtyType(idxItem, .EntityData(0)) Is GetType(EntityKoumoku_YmdType) Then
                        If IsDBNull(paramDt.Rows(0).Item(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_YmdType).PhysicsName)) = False Then
                            DirectCast(.getPtyValue(idxItem, .EntityData(idx)), EntityKoumoku_YmdType).Value = CDate(paramDt.Rows(idx).Item(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_YmdType).PhysicsName))
                        End If
                    ElseIf .getPtyType(idxItem, .EntityData(0)) Is GetType(EntityKoumoku_NumberType) Then
                        If IsDBNull(paramDt.Rows(0).Item(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_NumberType).PhysicsName)) = False Then
                            DirectCast(.getPtyValue(idxItem, .EntityData(idx)), EntityKoumoku_NumberType).Value = CInt(paramDt.Rows(idx).Item(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_NumberType).PhysicsName))
                        End If
                    ElseIf .getPtyType(idxItem, .EntityData(0)) Is GetType(EntityKoumoku_Number_DecimalType) Then
                        If IsDBNull(paramDt.Rows(0).Item(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_Number_DecimalType).PhysicsName)) = False Then
                            DirectCast(.getPtyValue(idxItem, .EntityData(idx)), EntityKoumoku_Number_DecimalType).Value = CDec(paramDt.Rows(idx).Item(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_Number_DecimalType).PhysicsName))
                        End If
                    Else
                        DirectCast(.getPtyValue(idxItem, .EntityData(idx)), EntityKoumoku_MojiType).Value = getDBValueForString(paramDt.Rows(idx).Item(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_MojiType).PhysicsName))
                    End If
                End With
            Next
        Next

        For entityCnt As Integer = 0 To CrsMasterEntity.EntityData.Length - 1
            paramClear()
            'データ未設定行はスキップ
            If CrsMasterEntity.EntityData(entityCnt).crsCd.Value = "" Then
                Continue For
            End If

            'SQL文生成
            sqlInsertString = "INSERT INTO " & "T_CRS_LEDGER_COST_CARRIER ("
            sqlValueString = "VALUES("

            For idx As Integer = 0 To CrsMasterEntity.getPropertyDataLength - 1
                If idx > 0 Then
                    sqlInsertString &= ","
                    sqlValueString &= ","
                End If
                With CrsMasterEntity
                    If .getPtyType(idx, .EntityData(entityCnt)) Is GetType(EntityKoumoku_YmdType) Then
                        sqlInsertString &= DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_YmdType).PhysicsName
                        sqlValueString &= setParam(DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_YmdType).PhysicsName,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_YmdType).Value,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_YmdType).DBType)
                    ElseIf .getPtyType(idx, .EntityData(entityCnt)) Is GetType(EntityKoumoku_NumberType) Then
                        sqlInsertString &= DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_NumberType).PhysicsName
                        sqlValueString &= setParam(DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_NumberType).PhysicsName,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_NumberType).Value,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_NumberType).DBType,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_NumberType).IntegerBu,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_NumberType).DecimalBu)
                    ElseIf .getPtyType(idx, .EntityData(entityCnt)) Is GetType(EntityKoumoku_Number_DecimalType) Then
                        sqlInsertString &= DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_Number_DecimalType).PhysicsName
                        sqlValueString &= setParam(DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_Number_DecimalType).PhysicsName,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_Number_DecimalType).Value,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_Number_DecimalType).DBType,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_Number_DecimalType).IntegerBu,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_Number_DecimalType).DecimalBu)
                    Else
                        sqlInsertString &= DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_MojiType).PhysicsName
                        sqlValueString &= setParam(DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_MojiType).PhysicsName,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_MojiType).Value,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_MojiType).DBType,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_MojiType).IntegerBu)
                    End If
                End With
            Next
            sqlString = sqlInsertString & ") " & sqlValueString & ")"
            insertCnt = execNonQuery(paramTrans, sqlString)

            '一意制約違反等でInsertエラーの場合
            If insertCnt = 0 Then
                '更新元エラーコードで返却
                Return ERRCNT
            Else
                retValue += insertCnt
            End If
        Next

        Return retValue
    End Function

    ''' <summary>
    ''' コース台帳原価（プレート）挿入
    ''' </summary>
    ''' <param name="paramDt">DataTable</param>
    ''' <param name="paramTrans">Oracleトランザクション</param>
    ''' <returns>更新件数(-1:エラー,0以上:正常)</returns>
    ''' <remarks></remarks>
    Private Function exeCuteInsertCostPlateData(ByVal paramDt As DataTable, ByVal paramTrans As OracleTransaction) As Integer
        Dim CrsMasterEntity As EntityOperation(Of TCrsLedgerCostPlateEntity) = New EntityOperation(Of TCrsLedgerCostPlateEntity)
        Dim sqlString As String = ""  'sqlString
        Dim sqlInsertString As String = ""  'sqlInsertString
        Dim sqlValueString As String = ""  'sqlValueString
        Dim retValue As Integer  'retValue
        Dim insertCnt As Integer

        'DataTableをEntityに詰める
        For idx As Integer = 0 To paramDt.Rows.Count - 1
            If idx > 0 Then
                Dim entity As New TCrsLedgerCostPlateEntity  'entity
                CrsMasterEntity.add(entity)
            End If
            For idxItem As Integer = 0 To CrsMasterEntity.getPropertyDataLength - 1
                With CrsMasterEntity
                    If .getPtyType(idxItem, .EntityData(0)) Is GetType(EntityKoumoku_YmdType) Then
                        If IsDBNull(paramDt.Rows(0).Item(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_YmdType).PhysicsName)) = False Then
                            DirectCast(.getPtyValue(idxItem, .EntityData(idx)), EntityKoumoku_YmdType).Value = CDate(paramDt.Rows(idx).Item(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_YmdType).PhysicsName))
                        End If
                    ElseIf .getPtyType(idxItem, .EntityData(0)) Is GetType(EntityKoumoku_NumberType) Then
                        If IsDBNull(paramDt.Rows(0).Item(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_NumberType).PhysicsName)) = False Then
                            DirectCast(.getPtyValue(idxItem, .EntityData(idx)), EntityKoumoku_NumberType).Value = CInt(paramDt.Rows(idx).Item(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_NumberType).PhysicsName))
                        End If
                    ElseIf .getPtyType(idxItem, .EntityData(0)) Is GetType(EntityKoumoku_Number_DecimalType) Then
                        If IsDBNull(paramDt.Rows(0).Item(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_Number_DecimalType).PhysicsName)) = False Then
                            DirectCast(.getPtyValue(idxItem, .EntityData(idx)), EntityKoumoku_Number_DecimalType).Value = CDec(paramDt.Rows(idx).Item(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_Number_DecimalType).PhysicsName))
                        End If
                    Else
                        DirectCast(.getPtyValue(idxItem, .EntityData(idx)), EntityKoumoku_MojiType).Value = getDBValueForString(paramDt.Rows(idx).Item(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_MojiType).PhysicsName))
                    End If
                End With
            Next
        Next

        For entityCnt As Integer = 0 To CrsMasterEntity.EntityData.Length - 1
            paramClear()
            'データ未設定行はスキップ
            If CrsMasterEntity.EntityData(entityCnt).crsCd.Value = "" Then
                Continue For
            End If

            'SQL文生成
            sqlInsertString = "INSERT INTO " & "T_CRS_LEDGER_COST_PLATE ("
            sqlValueString = "VALUES("

            For idx As Integer = 0 To CrsMasterEntity.getPropertyDataLength - 1
                If idx > 0 Then
                    sqlInsertString &= ","
                    sqlValueString &= ","
                End If
                With CrsMasterEntity
                    If .getPtyType(idx, .EntityData(entityCnt)) Is GetType(EntityKoumoku_YmdType) Then
                        sqlInsertString &= DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_YmdType).PhysicsName
                        sqlValueString &= setParam(DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_YmdType).PhysicsName,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_YmdType).Value,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_YmdType).DBType)
                    ElseIf .getPtyType(idx, .EntityData(entityCnt)) Is GetType(EntityKoumoku_NumberType) Then
                        sqlInsertString &= DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_NumberType).PhysicsName
                        sqlValueString &= setParam(DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_NumberType).PhysicsName,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_NumberType).Value,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_NumberType).DBType,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_NumberType).IntegerBu,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_NumberType).DecimalBu)
                    ElseIf .getPtyType(idx, .EntityData(entityCnt)) Is GetType(EntityKoumoku_Number_DecimalType) Then
                        sqlInsertString &= DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_Number_DecimalType).PhysicsName
                        sqlValueString &= setParam(DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_Number_DecimalType).PhysicsName,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_Number_DecimalType).Value,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_Number_DecimalType).DBType,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_Number_DecimalType).IntegerBu,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_Number_DecimalType).DecimalBu)
                    Else
                        sqlInsertString &= DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_MojiType).PhysicsName
                        sqlValueString &= setParam(DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_MojiType).PhysicsName,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_MojiType).Value,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_MojiType).DBType,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_MojiType).IntegerBu)
                    End If
                End With
            Next
            sqlString = sqlInsertString & ") " & sqlValueString & ")"
            insertCnt = execNonQuery(paramTrans, sqlString)

            '一意制約違反等でInsertエラーの場合
            If insertCnt = 0 Then
                '更新元エラーコードで返却
                Return ERRCNT
            Else
                retValue += insertCnt
            End If
        Next

        Return retValue
    End Function

    ''' <summary>
    ''' コース台帳原価（キャリア_料金区分）挿入
    ''' </summary>
    ''' <param name="paramDt">DataTable</param>
    ''' <param name="paramTrans">Oracleトランザクション</param>
    ''' <returns>更新件数(-1:エラー,0以上:正常)</returns>
    ''' <remarks></remarks>
    Private Function exeCuteInsertCostCarrierChargeKbnData(ByVal paramDt As DataTable, ByVal paramTrans As OracleTransaction) As Integer
        Dim CrsMasterEntity As EntityOperation(Of TCrsLedgerCostCarrierChargeKbnEntity) = New EntityOperation(Of TCrsLedgerCostCarrierChargeKbnEntity)
        Dim sqlString As String = ""  'sqlString
        Dim sqlInsertString As String = ""  'sqlInsertString
        Dim sqlValueString As String = ""  'sqlValueString
        Dim retValue As Integer  'retValue
        Dim insertCnt As Integer

        'DataTableをEntityに詰める
        For idx As Integer = 0 To paramDt.Rows.Count - 1
            If idx > 0 Then
                Dim entity As New TCrsLedgerCostCarrierChargeKbnEntity  'entity
                CrsMasterEntity.add(entity)
            End If
            For idxItem As Integer = 0 To CrsMasterEntity.getPropertyDataLength - 1
                With CrsMasterEntity
                    If .getPtyType(idxItem, .EntityData(0)) Is GetType(EntityKoumoku_YmdType) Then
                        If IsDBNull(paramDt.Rows(0).Item(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_YmdType).PhysicsName)) = False Then
                            DirectCast(.getPtyValue(idxItem, .EntityData(idx)), EntityKoumoku_YmdType).Value = CDate(paramDt.Rows(idx).Item(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_YmdType).PhysicsName))
                        End If
                    ElseIf .getPtyType(idxItem, .EntityData(0)) Is GetType(EntityKoumoku_NumberType) Then
                        If IsDBNull(paramDt.Rows(0).Item(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_NumberType).PhysicsName)) = False Then
                            DirectCast(.getPtyValue(idxItem, .EntityData(idx)), EntityKoumoku_NumberType).Value = CInt(paramDt.Rows(idx).Item(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_NumberType).PhysicsName))
                        End If
                    ElseIf .getPtyType(idxItem, .EntityData(0)) Is GetType(EntityKoumoku_Number_DecimalType) Then
                        If IsDBNull(paramDt.Rows(0).Item(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_Number_DecimalType).PhysicsName)) = False Then
                            DirectCast(.getPtyValue(idxItem, .EntityData(idx)), EntityKoumoku_Number_DecimalType).Value = CDec(paramDt.Rows(idx).Item(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_Number_DecimalType).PhysicsName))
                        End If
                    Else
                        DirectCast(.getPtyValue(idxItem, .EntityData(idx)), EntityKoumoku_MojiType).Value = getDBValueForString(paramDt.Rows(idx).Item(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_MojiType).PhysicsName))
                    End If
                End With
            Next
        Next

        For entityCnt As Integer = 0 To CrsMasterEntity.EntityData.Length - 1
            paramClear()
            'データ未設定行はスキップ
            If CrsMasterEntity.EntityData(entityCnt).crsCd.Value = "" Then
                Continue For
            End If

            'SQL文生成
            sqlInsertString = "INSERT INTO " & "T_CRS_LEDGER_COST_CARRIER_CHARGE_KBN ("
            sqlValueString = "VALUES("

            For idx As Integer = 0 To CrsMasterEntity.getPropertyDataLength - 1
                If idx > 0 Then
                    sqlInsertString &= ","
                    sqlValueString &= ","
                End If
                With CrsMasterEntity
                    If .getPtyType(idx, .EntityData(entityCnt)) Is GetType(EntityKoumoku_YmdType) Then
                        sqlInsertString &= DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_YmdType).PhysicsName
                        sqlValueString &= setParam(DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_YmdType).PhysicsName,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_YmdType).Value,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_YmdType).DBType)
                    ElseIf .getPtyType(idx, .EntityData(entityCnt)) Is GetType(EntityKoumoku_NumberType) Then
                        sqlInsertString &= DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_NumberType).PhysicsName
                        sqlValueString &= setParam(DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_NumberType).PhysicsName,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_NumberType).Value,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_NumberType).DBType,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_NumberType).IntegerBu,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_NumberType).DecimalBu)
                    ElseIf .getPtyType(idx, .EntityData(entityCnt)) Is GetType(EntityKoumoku_Number_DecimalType) Then
                        sqlInsertString &= DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_Number_DecimalType).PhysicsName
                        sqlValueString &= setParam(DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_Number_DecimalType).PhysicsName,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_Number_DecimalType).Value,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_Number_DecimalType).DBType,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_Number_DecimalType).IntegerBu,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_Number_DecimalType).DecimalBu)
                    Else
                        sqlInsertString &= DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_MojiType).PhysicsName
                        sqlValueString &= setParam(DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_MojiType).PhysicsName,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_MojiType).Value,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_MojiType).DBType,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_MojiType).IntegerBu)
                    End If
                End With
            Next
            sqlString = sqlInsertString & ") " & sqlValueString & ")"
            insertCnt = execNonQuery(paramTrans, sqlString)

            '一意制約違反等でInsertエラーの場合
            If insertCnt = 0 Then
                '更新元エラーコードで返却
                Return ERRCNT
            Else
                retValue += insertCnt
            End If
        Next

        Return retValue
    End Function

    ''' <summary>
    ''' コース台帳原価（降車ヶ所_料金区分）挿入
    ''' </summary>
    ''' <param name="paramDt">DataTable</param>
    ''' <param name="paramTrans">Oracleトランザクション</param>
    ''' <returns>更新件数(-1:エラー,0以上:正常)</returns>
    ''' <remarks></remarks>
    Private Function exeCuteInsertCostKoshakashoChargeKbnData(ByVal paramDt As DataTable, ByVal paramTrans As OracleTransaction) As Integer
        Dim CrsMasterEntity As EntityOperation(Of TCrsLedgerCostKoshakashoChargeKbnEntity) = New EntityOperation(Of TCrsLedgerCostKoshakashoChargeKbnEntity)
        Dim sqlString As String = ""  'sqlString
        Dim sqlInsertString As String = ""  'sqlInsertString
        Dim sqlValueString As String = ""  'sqlValueString
        Dim retValue As Integer  'retValue
        Dim insertCnt As Integer

        'DataTableをEntityに詰める
        For idx As Integer = 0 To paramDt.Rows.Count - 1
            If idx > 0 Then
                Dim entity As New TCrsLedgerCostKoshakashoChargeKbnEntity  'entity
                CrsMasterEntity.add(entity)
            End If
            For idxItem As Integer = 0 To CrsMasterEntity.getPropertyDataLength - 1
                With CrsMasterEntity
                    If .getPtyType(idxItem, .EntityData(0)) Is GetType(EntityKoumoku_YmdType) Then
                        If IsDBNull(paramDt.Rows(0).Item(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_YmdType).PhysicsName)) = False Then
                            DirectCast(.getPtyValue(idxItem, .EntityData(idx)), EntityKoumoku_YmdType).Value = CDate(paramDt.Rows(idx).Item(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_YmdType).PhysicsName))
                        End If
                    ElseIf .getPtyType(idxItem, .EntityData(0)) Is GetType(EntityKoumoku_NumberType) Then
                        If IsDBNull(paramDt.Rows(0).Item(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_NumberType).PhysicsName)) = False Then
                            DirectCast(.getPtyValue(idxItem, .EntityData(idx)), EntityKoumoku_NumberType).Value = CInt(paramDt.Rows(idx).Item(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_NumberType).PhysicsName))
                        End If
                    ElseIf .getPtyType(idxItem, .EntityData(0)) Is GetType(EntityKoumoku_Number_DecimalType) Then
                        If IsDBNull(paramDt.Rows(0).Item(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_Number_DecimalType).PhysicsName)) = False Then
                            DirectCast(.getPtyValue(idxItem, .EntityData(idx)), EntityKoumoku_Number_DecimalType).Value = CDec(paramDt.Rows(idx).Item(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_Number_DecimalType).PhysicsName))
                        End If
                    Else
                        DirectCast(.getPtyValue(idxItem, .EntityData(idx)), EntityKoumoku_MojiType).Value = getDBValueForString(paramDt.Rows(idx).Item(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_MojiType).PhysicsName))
                    End If
                End With
            Next
        Next

        For entityCnt As Integer = 0 To CrsMasterEntity.EntityData.Length - 1
            paramClear()
            'データ未設定行はスキップ
            If CrsMasterEntity.EntityData(entityCnt).crsCd.Value = "" Then
                Continue For
            End If

            'SQL文生成
            sqlInsertString = "INSERT INTO " & "T_CRS_LEDGER_COST_KOSHAKASHO_CHARGE_KBN ("
            sqlValueString = "VALUES("

            For idx As Integer = 0 To CrsMasterEntity.getPropertyDataLength - 1
                If idx > 0 Then
                    sqlInsertString &= ","
                    sqlValueString &= ","
                End If
                With CrsMasterEntity
                    If .getPtyType(idx, .EntityData(entityCnt)) Is GetType(EntityKoumoku_YmdType) Then
                        sqlInsertString &= DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_YmdType).PhysicsName
                        sqlValueString &= setParam(DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_YmdType).PhysicsName,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_YmdType).Value,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_YmdType).DBType)
                    ElseIf .getPtyType(idx, .EntityData(entityCnt)) Is GetType(EntityKoumoku_NumberType) Then
                        sqlInsertString &= DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_NumberType).PhysicsName
                        sqlValueString &= setParam(DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_NumberType).PhysicsName,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_NumberType).Value,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_NumberType).DBType,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_NumberType).IntegerBu,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_NumberType).DecimalBu)
                    ElseIf .getPtyType(idx, .EntityData(entityCnt)) Is GetType(EntityKoumoku_Number_DecimalType) Then
                        sqlInsertString &= DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_Number_DecimalType).PhysicsName
                        sqlValueString &= setParam(DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_Number_DecimalType).PhysicsName,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_Number_DecimalType).Value,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_Number_DecimalType).DBType,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_Number_DecimalType).IntegerBu,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_Number_DecimalType).DecimalBu)
                    Else
                        sqlInsertString &= DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_MojiType).PhysicsName
                        sqlValueString &= setParam(DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_MojiType).PhysicsName,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_MojiType).Value,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_MojiType).DBType,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_MojiType).IntegerBu)
                    End If
                End With
            Next
            sqlString = sqlInsertString & ") " & sqlValueString & ")"
            insertCnt = execNonQuery(paramTrans, sqlString)

            '一意制約違反等でInsertエラーの場合
            If insertCnt = 0 Then
                '更新元エラーコードで返却
                Return ERRCNT
            Else
                retValue += insertCnt
            End If
        Next

        Return retValue
    End Function

    ''' <summary>
    ''' 座席イメージ（バス情報）挿入
    ''' </summary>
    ''' <param name="paramDt">DataTable</param>
    ''' <param name="paramTrans">Oracleトランザクション</param>
    ''' <returns>更新件数(-1:エラー,0以上:正常)</returns>
    ''' <remarks></remarks>
    Private Function exeCuteInsertZasekiImageoData(ByVal paramDt As DataTable, ByVal paramTrans As OracleTransaction) As Integer
        Dim CrsMasterEntity As EntityOperation(Of TZasekiImageEntity) = New EntityOperation(Of TZasekiImageEntity)
        Dim sqlString As String = ""  'sqlString
        Dim sqlInsertString As String = ""  'sqlInsertString
        Dim sqlValueString As String = ""  'sqlValueString
        Dim retValue As Integer  'retValue
        Dim insertCnt As Integer

        'DataTableをEntityに詰める
        For idx As Integer = 0 To paramDt.Rows.Count - 1
            If idx > 0 Then
                Dim entity As New TZasekiImageEntity  'entity
                CrsMasterEntity.add(entity)
            End If
            For idxItem As Integer = 0 To CrsMasterEntity.getPropertyDataLength - 1
                With CrsMasterEntity
                    If .getPtyType(idxItem, .EntityData(0)) Is GetType(EntityKoumoku_YmdType) Then
                        If IsDBNull(paramDt.Rows(0).Item(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_YmdType).PhysicsName)) = False Then
                            DirectCast(.getPtyValue(idxItem, .EntityData(idx)), EntityKoumoku_YmdType).Value = CDate(paramDt.Rows(idx).Item(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_YmdType).PhysicsName))
                        End If
                    ElseIf .getPtyType(idxItem, .EntityData(0)) Is GetType(EntityKoumoku_NumberType) Then
                        If IsDBNull(paramDt.Rows(0).Item(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_NumberType).PhysicsName)) = False Then
                            DirectCast(.getPtyValue(idxItem, .EntityData(idx)), EntityKoumoku_NumberType).Value = CInt(paramDt.Rows(idx).Item(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_NumberType).PhysicsName))
                        End If
                    ElseIf .getPtyType(idxItem, .EntityData(0)) Is GetType(EntityKoumoku_Number_DecimalType) Then
                        If IsDBNull(paramDt.Rows(0).Item(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_Number_DecimalType).PhysicsName)) = False Then
                            DirectCast(.getPtyValue(idxItem, .EntityData(idx)), EntityKoumoku_Number_DecimalType).Value = CDec(paramDt.Rows(idx).Item(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_Number_DecimalType).PhysicsName))
                        End If
                    Else
                        DirectCast(.getPtyValue(idxItem, .EntityData(idx)), EntityKoumoku_MojiType).Value = getDBValueForString(paramDt.Rows(idx).Item(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_MojiType).PhysicsName))
                    End If
                End With
            Next
        Next

        For entityCnt As Integer = 0 To CrsMasterEntity.EntityData.Length - 1
            paramClear()
            'データ未設定行はスキップ
            If CrsMasterEntity.EntityData(entityCnt).busReserveCd.Value = "" Then
                Continue For
            End If

            'SQL文生成
            sqlInsertString = "INSERT INTO " & "T_ZASEKI_IMAGE ("
            sqlValueString = "VALUES("

            For idx As Integer = 0 To CrsMasterEntity.getPropertyDataLength - 1
                If idx > 0 Then
                    sqlInsertString &= ","
                    sqlValueString &= ","
                End If
                With CrsMasterEntity
                    If .getPtyType(idx, .EntityData(entityCnt)) Is GetType(EntityKoumoku_YmdType) Then
                        sqlInsertString &= DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_YmdType).PhysicsName
                        sqlValueString &= setParam(DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_YmdType).PhysicsName,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_YmdType).Value,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_YmdType).DBType)
                    ElseIf .getPtyType(idx, .EntityData(entityCnt)) Is GetType(EntityKoumoku_NumberType) Then
                        sqlInsertString &= DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_NumberType).PhysicsName
                        sqlValueString &= setParam(DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_NumberType).PhysicsName,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_NumberType).Value,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_NumberType).DBType,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_NumberType).IntegerBu,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_NumberType).DecimalBu)
                    ElseIf .getPtyType(idx, .EntityData(entityCnt)) Is GetType(EntityKoumoku_Number_DecimalType) Then
                        sqlInsertString &= DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_Number_DecimalType).PhysicsName
                        sqlValueString &= setParam(DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_Number_DecimalType).PhysicsName,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_Number_DecimalType).Value,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_Number_DecimalType).DBType,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_Number_DecimalType).IntegerBu,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_Number_DecimalType).DecimalBu)
                    Else
                        sqlInsertString &= DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_MojiType).PhysicsName
                        sqlValueString &= setParam(DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_MojiType).PhysicsName,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_MojiType).Value,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_MojiType).DBType,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_MojiType).IntegerBu)
                    End If
                End With
            Next
            sqlString = sqlInsertString & ") " & sqlValueString & ")"
            insertCnt = execNonQuery(paramTrans, sqlString)

            '一意制約違反等でInsertエラーの場合
            If insertCnt = 0 Then
                '更新元エラーコードで返却
                Return ERRCNT
            Else
                retValue += insertCnt
            End If
        Next

        Return retValue
    End Function

    ''' <summary>
    ''' 座席イメージ（座席情報）挿入
    ''' </summary>
    ''' <param name="paramDt">DataTable</param>
    ''' <param name="paramTrans">Oracleトランザクション</param>
    ''' <returns>更新件数(-1:エラー,0以上:正常)</returns>
    ''' <remarks></remarks>
    Private Function exeCuteInsertZasekiImageInfoData(ByVal paramDt As DataTable, ByVal paramTrans As OracleTransaction) As Integer
        Dim CrsMasterEntity As EntityOperation(Of TZasekiImageInfoEntity) = New EntityOperation(Of TZasekiImageInfoEntity)
        Dim sqlString As String = ""  'sqlString
        Dim sqlInsertString As String = ""  'sqlInsertString
        Dim sqlValueString As String = ""  'sqlValueString
        Dim retValue As Integer  'retValue
        Dim insertCnt As Integer

        'DataTableをEntityに詰める
        For idx As Integer = 0 To paramDt.Rows.Count - 1
            If idx > 0 Then
                Dim entity As New TZasekiImageInfoEntity  'entity
                CrsMasterEntity.add(entity)
            End If
            For idxItem As Integer = 0 To CrsMasterEntity.getPropertyDataLength - 1
                With CrsMasterEntity
                    If .getPtyType(idxItem, .EntityData(0)) Is GetType(EntityKoumoku_YmdType) Then
                        If IsDBNull(paramDt.Rows(0).Item(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_YmdType).PhysicsName)) = False Then
                            DirectCast(.getPtyValue(idxItem, .EntityData(idx)), EntityKoumoku_YmdType).Value = CDate(paramDt.Rows(idx).Item(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_YmdType).PhysicsName))
                        End If
                    ElseIf .getPtyType(idxItem, .EntityData(0)) Is GetType(EntityKoumoku_NumberType) Then
                        If IsDBNull(paramDt.Rows(0).Item(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_NumberType).PhysicsName)) = False Then
                            DirectCast(.getPtyValue(idxItem, .EntityData(idx)), EntityKoumoku_NumberType).Value = CInt(paramDt.Rows(idx).Item(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_NumberType).PhysicsName))
                        End If
                    ElseIf .getPtyType(idxItem, .EntityData(0)) Is GetType(EntityKoumoku_Number_DecimalType) Then
                        If IsDBNull(paramDt.Rows(0).Item(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_Number_DecimalType).PhysicsName)) = False Then
                            DirectCast(.getPtyValue(idxItem, .EntityData(idx)), EntityKoumoku_Number_DecimalType).Value = CDec(paramDt.Rows(idx).Item(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_Number_DecimalType).PhysicsName))
                        End If
                    Else
                        DirectCast(.getPtyValue(idxItem, .EntityData(idx)), EntityKoumoku_MojiType).Value = getDBValueForString(paramDt.Rows(idx).Item(DirectCast(.getPtyValue(idxItem, .EntityData(0)), EntityKoumoku_MojiType).PhysicsName))
                    End If
                End With
            Next
        Next

        For entityCnt As Integer = 0 To CrsMasterEntity.EntityData.Length - 1
            paramClear()
            'データ未設定行はスキップ
            If CrsMasterEntity.EntityData(entityCnt).busReserveCd.Value = "" Then
                Continue For
            End If

            'SQL文生成
            sqlInsertString = "INSERT INTO " & "T_ZASEKI_IMAGE_INFO ("
            sqlValueString = "VALUES("

            For idx As Integer = 0 To CrsMasterEntity.getPropertyDataLength - 1
                If idx > 0 Then
                    sqlInsertString &= ","
                    sqlValueString &= ","
                End If
                With CrsMasterEntity
                    If .getPtyType(idx, .EntityData(entityCnt)) Is GetType(EntityKoumoku_YmdType) Then
                        sqlInsertString &= DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_YmdType).PhysicsName
                        sqlValueString &= setParam(DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_YmdType).PhysicsName,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_YmdType).Value,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_YmdType).DBType)
                    ElseIf .getPtyType(idx, .EntityData(entityCnt)) Is GetType(EntityKoumoku_NumberType) Then
                        sqlInsertString &= DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_NumberType).PhysicsName
                        sqlValueString &= setParam(DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_NumberType).PhysicsName,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_NumberType).Value,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_NumberType).DBType,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_NumberType).IntegerBu,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_NumberType).DecimalBu)
                    ElseIf .getPtyType(idx, .EntityData(entityCnt)) Is GetType(EntityKoumoku_Number_DecimalType) Then
                        sqlInsertString &= DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_Number_DecimalType).PhysicsName
                        sqlValueString &= setParam(DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_Number_DecimalType).PhysicsName,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_Number_DecimalType).Value,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_Number_DecimalType).DBType,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_Number_DecimalType).IntegerBu,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_Number_DecimalType).DecimalBu)
                    Else
                        sqlInsertString &= DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_MojiType).PhysicsName
                        sqlValueString &= setParam(DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_MojiType).PhysicsName,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_MojiType).Value,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_MojiType).DBType,
                                                           DirectCast(.getPtyValue(idx, .EntityData(entityCnt)), EntityKoumoku_MojiType).IntegerBu)
                    End If
                End With
            Next
            sqlString = sqlInsertString & ") " & sqlValueString & ")"
            insertCnt = execNonQuery(paramTrans, sqlString)

            '一意制約違反等でInsertエラーの場合
            If insertCnt = 0 Then
                '更新元エラーコードで返却
                Return ERRCNT
            Else
                retValue += insertCnt
            End If
        Next

        Return retValue
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
            retValue = targetObj.ToString 'RTrim(targetObj.ToString)
        End If
        Return retValue
    End Function

    ''' <summary>
    ''' コース台帳(基本)編集処理
    ''' </summary>
    ''' <param name="zouhatsutOrigData"></param>
    ''' <param name="ParamList"></param>
    Private Sub editDataBasicTable(ByRef zouhatsutOrigData As DataSet, ByVal ParamList As Hashtable)
        Dim basicDt As DataTable
        Dim crsLeaderBasicEnt As TCrsLedgerBasicEntity = New TCrsLedgerBasicEntity
        basicDt = zouhatsutOrigData.Tables(crsLedgerTblId.crsLeaderBasic)
        For Each dr As DataRow In basicDt.Rows
            'ブロック確保数	BLOCK_KAKUHO_NUM	NUMBER(3, 0)			初期値
            dr.Item(crsLeaderBasicEnt.blockKakuhoNum.PhysicsName) = 0
            'キャンセル待ち人数	CANCEL_WAIT_NINZU	NUMBER(2, 0)			初期値
            dr.Item(crsLeaderBasicEnt.cancelWaitNinzu.PhysicsName) = 0
            '定員補１階	CAPACITY_HO_1KAI	NUMBER(3, 0)			取得④
            If IsDBNull(dr.Item(crsLeaderBasicEnt.subSeatOkKbn.PhysicsName)) OrElse
                DirectCast(dr.Item(crsLeaderBasicEnt.subSeatOkKbn.PhysicsName), String) = "N" Then
                dr.Item(crsLeaderBasicEnt.capacityHo1kai.PhysicsName) = 0
            Else
                dr.Item(crsLeaderBasicEnt.capacityHo1kai.PhysicsName) = ParamList(UpdateParamKeys.CAR_EMG_CAPACITY)
            End If
            '定員定	CAPACITY_REGULAR	NUMBER(3, 0)			取得③
            dr.Item(crsLeaderBasicEnt.capacityRegular.PhysicsName) = ParamList(UpdateParamKeys.CAR_CAPACITY)
            '車番	CAR_NO	NUMBER(3, 0)			入力値
            dr.Item(crsLeaderBasicEnt.carNo.PhysicsName) = ParamList(UpdateParamKeys.CAR_NO)
            '車種コード	CAR_TYPE_CD	CHAR(2 BYTE)			入力値
            dr.Item(crsLeaderBasicEnt.carTypeCd.PhysicsName) = ParamList(UpdateParamKeys.CAR_TYPE_CD)
            '台数カウントフラグ	BUS_COUNT_FLG	CHAR(1 BYTE)			カウントしない
            dr.Item(crsLeaderBasicEnt.busCountFlg.PhysicsName) = BusCountFlg.NoCount
            'コースブロック定員	CRS_BLOCK_CAPACITY	NUMBER(5, 0)			初期値
            dr.Item(crsLeaderBasicEnt.crsBlockCapacity.PhysicsName) = 0
            'コースブロック１名１Ｒ	CRS_BLOCK_ONE_1R	NUMBER(3, 0)			初期値
            dr.Item(crsLeaderBasicEnt.crsBlockOne1r.PhysicsName) = 0
            'コースブロックルーム数	CRS_BLOCK_ROOM_NUM	NUMBER(3, 0)			初期値
            dr.Item(crsLeaderBasicEnt.crsBlockRoomNum.PhysicsName) = 0
            'コースブロック３名１Ｒ	CRS_BLOCK_THREE_1R	NUMBER(3, 0)			初期値
            dr.Item(crsLeaderBasicEnt.crsBlockThree1r.PhysicsName) = 0
            'コースブロック２名１Ｒ	CRS_BLOCK_TWO_1R	NUMBER(3, 0)			初期値
            dr.Item(crsLeaderBasicEnt.crsBlockTwo1r.PhysicsName) = 0
            'コースブロック４名１Ｒ	CRS_BLOCK_FOUR_1R	NUMBER(3, 0)			初期値
            dr.Item(crsLeaderBasicEnt.crsBlockFour1r.PhysicsName) = 0
            'コースブロック５名１Ｒ	CRS_BLOCK_FIVE_1R	NUMBER(3, 0)			初期値
            dr.Item(crsLeaderBasicEnt.crsBlockFive1r.PhysicsName) = 0
            '営ブロック補	EI_BLOCK_HO	NUMBER(3, 0)			"増発先の座席種別が2（補助席）かつ、営業所ブロック区分が1（ブロックあり）の数をカウント"
            '営ブロック定	EI_BLOCK_REGULAR	NUMBER(3, 0)			"増発先の座席種別が1（通常席）かつ、営業所ブロック区分が1（ブロックあり）の数をカウント"
            '⇒別プロシージャで設定するように変更
            '配車経由コード１	HAISYA_KEIYU_CD_1	CHAR(3 BYTE)			入力値
            dr.Item(crsLeaderBasicEnt.haisyaKeiyuCd1.PhysicsName) = ParamList(UpdateParamKeys.HAISYA_KEIYU_CD_1)
            '配車経由コード２	HAISYA_KEIYU_CD_2	CHAR(3 BYTE)			入力値
            dr.Item(crsLeaderBasicEnt.haisyaKeiyuCd2.PhysicsName) = ParamList(UpdateParamKeys.HAISYA_KEIYU_CD_2)
            '配車経由コード３	HAISYA_KEIYU_CD_3	CHAR(3 BYTE)			入力値
            dr.Item(crsLeaderBasicEnt.haisyaKeiyuCd3.PhysicsName) = ParamList(UpdateParamKeys.HAISYA_KEIYU_CD_3)
            '配車経由コード４	HAISYA_KEIYU_CD_4	CHAR(3 BYTE)			入力値
            dr.Item(crsLeaderBasicEnt.haisyaKeiyuCd4.PhysicsName) = ParamList(UpdateParamKeys.HAISYA_KEIYU_CD_4)
            '配車経由コード５	HAISYA_KEIYU_CD_5	CHAR(3 BYTE)			入力値
            dr.Item(crsLeaderBasicEnt.haisyaKeiyuCd5.PhysicsName) = ParamList(UpdateParamKeys.HAISYA_KEIYU_CD_5)
            '○増管理区分	MARU_ZOU_MANAGEMENT_KBN	CHAR(1 BYTE)			取得②
            If Not IsDBNull(basicDt.Rows(0).Item(54)) Then
                '邦人外客区分チェック
                If basicDt.Rows(0).Item(54).ToString = FixedCd.HoujinGaikyakuKbnType.Houjin Then
                    '邦人の場合、号車が900以上の場合○増
                    If DirectCast(ParamList(UpdateParamKeys.GOUSYA), Integer) >= 900 Then
                        dr.Item(crsLeaderBasicEnt.maruZouManagementKbn.PhysicsName) = FixedCd.MaruzouKanriKbn.Maruzou
                    End If
                Else
                    '外客の場合、号車が990以上の場合○増
                    If DirectCast(ParamList(UpdateParamKeys.GOUSYA), Integer) >= 990 Then
                        dr.Item(crsLeaderBasicEnt.maruZouManagementKbn.PhysicsName) = FixedCd.MaruzouKanriKbn.Maruzou
                    End If
                End If
            End If

            '乗車人数入力済フラグ配車経由１	NINZU_INPUT_FLG_KEIYU_1	CHAR(1 BYTE)			初期値
            dr.Item(crsLeaderBasicEnt.ninzuInputFlgKeiyu1.PhysicsName) = String.Empty
            '乗車人数入力済フラグ配車経由２	NINZU_INPUT_FLG_KEIYU_2	CHAR(1 BYTE)			初期値
            dr.Item(crsLeaderBasicEnt.ninzuInputFlgKeiyu2.PhysicsName) = String.Empty
            '乗車人数入力済フラグ配車経由３	NINZU_INPUT_FLG_KEIYU_3	CHAR(1 BYTE)			初期値
            dr.Item(crsLeaderBasicEnt.ninzuInputFlgKeiyu3.PhysicsName) = String.Empty
            '乗車人数入力済フラグ配車経由４	NINZU_INPUT_FLG_KEIYU_4	CHAR(1 BYTE)			初期値
            dr.Item(crsLeaderBasicEnt.ninzuInputFlgKeiyu4.PhysicsName) = String.Empty
            '乗車人数入力済フラグ配車経由５	NINZU_INPUT_FLG_KEIYU_5	CHAR(1 BYTE)			初期値
            dr.Item(crsLeaderBasicEnt.ninzuInputFlgKeiyu5.PhysicsName) = String.Empty
            '乗車人数配車経由１大人	NINZU_KEIYU_1_ADULT	NUMBER(3, 0)			初期値
            dr.Item(crsLeaderBasicEnt.ninzuKeiyu1Adult.PhysicsName) = 0
            '乗車人数配車経由１小人	NINZU_KEIYU_1_CHILD	NUMBER(3, 0)			初期値
            dr.Item(crsLeaderBasicEnt.ninzuKeiyu1Child.PhysicsName) = 0
            '乗車人数配車経由１中人	NINZU_KEIYU_1_JUNIOR	NUMBER(3, 0)			初期値
            dr.Item(crsLeaderBasicEnt.ninzuKeiyu1Junior.PhysicsName) = 0
            '乗車人数配車経由１招	NINZU_KEIYU_1_S	NUMBER(3, 0)			初期値
            dr.Item(crsLeaderBasicEnt.ninzuKeiyu1S.PhysicsName) = 0
            '乗車人数配車経由２大人	NINZU_KEIYU_2_ADULT	NUMBER(3, 0)			初期値
            dr.Item(crsLeaderBasicEnt.ninzuKeiyu2Adult.PhysicsName) = 0
            '乗車人数配車経由２小人	NINZU_KEIYU_2_CHILD	NUMBER(3, 0)			初期値
            dr.Item(crsLeaderBasicEnt.ninzuKeiyu2Child.PhysicsName) = 0
            '乗車人数配車経由２中人	NINZU_KEIYU_2_JUNIOR	NUMBER(3, 0)			初期値
            dr.Item(crsLeaderBasicEnt.ninzuKeiyu2Junior.PhysicsName) = 0
            '乗車人数配車経由２招	NINZU_KEIYU_2_S	NUMBER(3, 0)			初期値
            dr.Item(crsLeaderBasicEnt.ninzuKeiyu2S.PhysicsName) = 0
            '乗車人数配車経由３大人	NINZU_KEIYU_3_ADULT	NUMBER(3, 0)			初期値
            dr.Item(crsLeaderBasicEnt.ninzuKeiyu3Adult.PhysicsName) = 0
            '乗車人数配車経由３小人	NINZU_KEIYU_3_CHILD	NUMBER(3, 0)			初期値
            dr.Item(crsLeaderBasicEnt.ninzuKeiyu3Child.PhysicsName) = 0
            '乗車人数配車経由３中人	NINZU_KEIYU_3_JUNIOR	NUMBER(3, 0)			初期値
            dr.Item(crsLeaderBasicEnt.ninzuKeiyu3Junior.PhysicsName) = 0
            '乗車人数配車経由３招	NINZU_KEIYU_3_S	NUMBER(3, 0)			初期値
            dr.Item(crsLeaderBasicEnt.ninzuKeiyu3S.PhysicsName) = 0
            '乗車人数配車経由４大人	NINZU_KEIYU_4_ADULT	NUMBER(3, 0)			初期値
            dr.Item(crsLeaderBasicEnt.ninzuKeiyu4Adult.PhysicsName) = 0
            '乗車人数配車経由４小人	NINZU_KEIYU_4_CHILD	NUMBER(3, 0)			初期値
            dr.Item(crsLeaderBasicEnt.ninzuKeiyu4Child.PhysicsName) = 0
            '乗車人数配車経由４中人	NINZU_KEIYU_4_JUNIOR	NUMBER(3, 0)			初期値
            dr.Item(crsLeaderBasicEnt.ninzuKeiyu4Junior.PhysicsName) = 0
            '乗車人数配車経由４招	NINZU_KEIYU_4_S	NUMBER(3, 0)			初期値
            dr.Item(crsLeaderBasicEnt.ninzuKeiyu4S.PhysicsName) = 0
            '乗車人数配車経由５大人	NINZU_KEIYU_5_ADULT	NUMBER(3, 0)			初期値
            dr.Item(crsLeaderBasicEnt.ninzuKeiyu5Adult.PhysicsName) = 0
            '乗車人数配車経由５小人	NINZU_KEIYU_5_CHILD	NUMBER(3, 0)			初期値
            dr.Item(crsLeaderBasicEnt.ninzuKeiyu5Child.PhysicsName) = 0
            '乗車人数配車経由５中人	NINZU_KEIYU_5_JUNIOR	NUMBER(3, 0)			初期値
            dr.Item(crsLeaderBasicEnt.ninzuKeiyu5Junior.PhysicsName) = 0
            '乗車人数配車経由５招	NINZU_KEIYU_5_S	NUMBER(3, 0)			初期値
            dr.Item(crsLeaderBasicEnt.ninzuKeiyu5S.PhysicsName) = 0
            '部屋残数１人部屋	ROOM_ZANSU_ONE_ROOM	NUMBER(3, 0)			初期値
            dr.Item(crsLeaderBasicEnt.roomZansuOneRoom.PhysicsName) = 0
            '部屋残数総計	ROOM_ZANSU_SOKEI	NUMBER(3, 0)			初期値
            dr.Item(crsLeaderBasicEnt.roomZansuSokei.PhysicsName) = 0
            '部屋残数３人部屋	ROOM_ZANSU_THREE_ROOM	NUMBER(3, 0)			初期値
            dr.Item(crsLeaderBasicEnt.roomZansuThreeRoom.PhysicsName) = 0
            '部屋残数２人部屋	ROOM_ZANSU_TWO_ROOM	NUMBER(3, 0)			初期値
            dr.Item(crsLeaderBasicEnt.roomZansuTwoRoom.PhysicsName) = 0
            '部屋残数４人部屋	ROOM_ZANSU_FOUR_ROOM	NUMBER(3, 0)			初期値
            dr.Item(crsLeaderBasicEnt.roomZansuFourRoom.PhysicsName) = 0
            '部屋残数５人部屋	ROOM_ZANSU_FIVE_ROOM	NUMBER(3, 0)			初期値
            dr.Item(crsLeaderBasicEnt.roomZansuFiveRoom.PhysicsName) = 0
            '催行確定区分	SAIKOU_KAKUTEI_KBN	CHAR(1 BYTE)			"企画の場合、廃止（非表示）を設定 それ以外の場合、増発元データを設定"
            If CStr(FixedCd.TeikiKikakuKbn.Kikaku).Equals(dr.Item(crsLeaderBasicEnt.teikiKikakuKbn.PhysicsName)) Then
                dr.Item(crsLeaderBasicEnt.saikouKakuteiKbn.PhysicsName) = SaikouKakuteiKbn.Haishi
            End If
            If (Not IsDBNull(basicDt.Rows(0).Item(129)) AndAlso basicDt.Rows(0).Item(129) IsNot Nothing) AndAlso
                CType(basicDt.Rows(0).Item(129), Integer) = FixedCd.SyuptJiCarrierKbnType.sonota Then
                ' 集合時間１	SYUGO_TIME_1	NUMBER(4, 0)			入力.出発時間1-（増発元.出発時間1-増発元.集合時間1)
                dr.Item(crsLeaderBasicEnt.syugoTimeCarrier.PhysicsName) = ParamList(UpdateParamKeys.SYUGO_TIME_1)
                '出発時間１	SYUPT_TIME_1	NUMBER(4, 0)			入力値
                dr.Item(crsLeaderBasicEnt.syuptTimeCarrier.PhysicsName) = ParamList(UpdateParamKeys.SYUPT_TIME_1)
            End If
            ' 集合時間１	SYUGO_TIME_1	NUMBER(4, 0)			入力.出発時間1-（増発元.出発時間1-増発元.集合時間1)
            dr.Item(crsLeaderBasicEnt.syugoTime1.PhysicsName) = ParamList(UpdateParamKeys.SYUGO_TIME_1)
            '集合時間２	SYUGO_TIME_2	NUMBER(4, 0)			入力.出発時間2-（増発元.出発時間2-増発元.集合時間2)
            dr.Item(crsLeaderBasicEnt.syugoTime2.PhysicsName) = ParamList(UpdateParamKeys.SYUGO_TIME_2)
            '集合時間３	SYUGO_TIME_3	NUMBER(4, 0)			入力.出発時間3-（増発元.出発時間3-増発元.集合時間3)
            dr.Item(crsLeaderBasicEnt.syugoTime3.PhysicsName) = ParamList(UpdateParamKeys.SYUGO_TIME_3)
            '集合時間４	SYUGO_TIME_4	NUMBER(4, 0)			入力.出発時間4-（増発元.出発時間4-増発元.集合時間4)
            dr.Item(crsLeaderBasicEnt.syugoTime4.PhysicsName) = ParamList(UpdateParamKeys.SYUGO_TIME_4)
            '集合時間５	SYUGO_TIME_5	NUMBER(4, 0)			入力.出発時間5-（増発元.出発時間5-増発元.集合時間5)
            dr.Item(crsLeaderBasicEnt.syugoTime5.PhysicsName) = ParamList(UpdateParamKeys.SYUGO_TIME_5)
            '出発時間１	SYUPT_TIME_1	NUMBER(4, 0)			入力値
            dr.Item(crsLeaderBasicEnt.syuptTime1.PhysicsName) = ParamList(UpdateParamKeys.SYUPT_TIME_1)
            '出発時間２	SYUPT_TIME_2	NUMBER(4, 0)			入力値
            dr.Item(crsLeaderBasicEnt.syuptTime2.PhysicsName) = ParamList(UpdateParamKeys.SYUPT_TIME_2)
            '出発時間３	SYUPT_TIME_3	NUMBER(4, 0)			入力値
            dr.Item(crsLeaderBasicEnt.syuptTime3.PhysicsName) = ParamList(UpdateParamKeys.SYUPT_TIME_3)
            '出発時間４	SYUPT_TIME_4	NUMBER(4, 0)			入力値
            dr.Item(crsLeaderBasicEnt.syuptTime4.PhysicsName) = ParamList(UpdateParamKeys.SYUPT_TIME_4)
            '出発時間５	SYUPT_TIME_5	NUMBER(4, 0)			入力値
            dr.Item(crsLeaderBasicEnt.syuptTime5.PhysicsName) = ParamList(UpdateParamKeys.SYUPT_TIME_5)
            '運休区分	UNKYU_KBN	CHAR(1 BYTE)			"定期の場合、廃止（非表示）を設定。それ以外の場合、増発元データを設定"
            If CStr(FixedCd.TeikiKikakuKbn.Teiki).Equals(dr.Item(crsLeaderBasicEnt.teikiKikakuKbn.PhysicsName)) Then
                dr.Item(crsLeaderBasicEnt.unkyuKbn.PhysicsName) = UnkyuKbn.Haishi
            End If
            '使用中フラグ	USING_FLG	CHAR(1 BYTE)			初期値
            dr.Item(crsLeaderBasicEnt.usingFlg.PhysicsName) = String.Empty
            '予約済ＲＯＯＭ数	YOYAKU_ALREADY_ROOM_NUM	NUMBER(3, 0)			初期値
            dr.Item(crsLeaderBasicEnt.yoyakuAlreadyRoomNum.PhysicsName) = 0
            '予約数補助席	YOYAKU_NUM_SUB_SEAT	NUMBER(3, 0)			取得⑦(初期値)
            dr.Item(crsLeaderBasicEnt.yoyakuNumSubSeat.PhysicsName) = 0
            '予約数定席	YOYAKU_NUM_TEISEKI	NUMBER(3, 0)			取得⑥(初期値)
            dr.Item(crsLeaderBasicEnt.yoyakuNumTeiseki.PhysicsName) = 0
            '増発元号車 ZOUHATSUMOTO_GOUSYA NUMBER(3, 0) 増発元の号車
            dr.Item(crsLeaderBasicEnt.zouhatsumotogousya.PhysicsName) = dr.Item(CommonColName.Gousya)
            '増発日 ZOUHATSU_DAY NUMBER(8, 0) システム日付の日付部
            dr.Item(crsLeaderBasicEnt.zouhatsuday.PhysicsName) = CType(Replace(CType(CommonDateUtil.getSystemTime().ToShortDateString(), String), "/", ""), Decimal)
            '増発実施者 ZOUHATSU_ENTRY_PERSON_CD VARCHAR2(20 BYTE) ユーザー情報(UserID)
            dr.Item(crsLeaderBasicEnt.zouhatsuentrypersoncd.PhysicsName) = UserInfoManagement.userId
            'ＷＴ確保席数	WT_KAKUHO_SEAT_NUM	NUMBER(3, 0)			初期値
            dr.Item(crsLeaderBasicEnt.wtKakuhoSeatNum.PhysicsName) = 0
            '予約可能数	YOYAKU_KANOU_NUM	NUMBER(3, 0)			取得⑤
            '取得した座席数<　コース台帳（基本）.乗車定員のとき		
            '  予約可能数 = 取得した座席数
            'それ以外の場合
            '  予約可能数 = 乗車定員
            If IsDBNull(dr.Item(crsLeaderBasicEnt.jyosyaCapacity.PhysicsName)) = False Then
                If (CInt(ParamList(UpdateParamKeys.CAR_CAPACITY)) < CInt(dr.Item(crsLeaderBasicEnt.jyosyaCapacity.PhysicsName))) Then
                    dr.Item(crsLeaderBasicEnt.yoyakuKanouNum.PhysicsName) = ParamList(UpdateParamKeys.CAR_CAPACITY)
                Else
                    dr.Item(crsLeaderBasicEnt.yoyakuKanouNum.PhysicsName) = dr.Item(crsLeaderBasicEnt.jyosyaCapacity.PhysicsName)
                End If
            Else
                dr.Item(crsLeaderBasicEnt.yoyakuKanouNum.PhysicsName) = DBNull.Value
            End If

            '空席数定席	KUSEKI_NUM_TEISEKI	NUMBER(3, 0)			取得⑧
            '空席数補助席	KUSEKI_NUM_SUB_SEAT	NUMBER(3, 0)			取得⑧
            '架空車種の場合、
            '  空席数・定席　=　乗車定員
            '  予約可能数 = 乗車定員
            '架空車種以外
            ' 予約可能数 >= 取得した座席数のとき          
            '   空席数・定席　=　取得した座席数　-　コース台帳（基本）.ブロック確保数　-　コース台帳（基本）.空席確保数
            '	空席数・補助席　=　予約可能数　-　取得した座席数
            '	空席数・補助席　>　定員・補/1Fのとき
            '		空席数・補助席　=　定員・補/1F	
            ' 上記以外の場合
            '   予約可能数 <　定員・定のとき←この条件いるの？(上で定員・定には、座席数を設定している)
            '       空席数・定席　=　取得した座席数	
            '       空席数・補助席　=　0	
            If IsDBNull(dr.Item(crsLeaderBasicEnt.carTypeCd.PhysicsName)) = False AndAlso CType(dr.Item(crsLeaderBasicEnt.carTypeCd.PhysicsName), String) = FixedCd.CarTypeCdKakuu Then
                dr.Item(crsLeaderBasicEnt.kusekiNumTeiseki.PhysicsName) = dr.Item(crsLeaderBasicEnt.jyosyaCapacity.PhysicsName)
                dr.Item(crsLeaderBasicEnt.yoyakuKanouNum.PhysicsName) = dr.Item(crsLeaderBasicEnt.jyosyaCapacity.PhysicsName)
            Else
                If IsDBNull(dr.Item(crsLeaderBasicEnt.yoyakuKanouNum.PhysicsName)) = True OrElse
                   CInt(dr.Item(crsLeaderBasicEnt.yoyakuKanouNum.PhysicsName)) < CInt(ParamList(UpdateParamKeys.CAR_CAPACITY)) Then
                    dr.Item(crsLeaderBasicEnt.kusekiNumTeiseki.PhysicsName) = ParamList(UpdateParamKeys.CAR_CAPACITY)
                    dr.Item(crsLeaderBasicEnt.kusekiNumSubSeat.PhysicsName) = 0
                Else
                    If IsDBNull(dr.Item(crsLeaderBasicEnt.kusekiKakuhoNum.PhysicsName)) = True Then
                        dr.Item(crsLeaderBasicEnt.kusekiNumTeiseki.PhysicsName) = CInt(ParamList(UpdateParamKeys.CAR_CAPACITY)) - CInt(dr.Item(crsLeaderBasicEnt.blockKakuhoNum.PhysicsName)) '←ブロック確保は0のはず
                    Else
                        dr.Item(crsLeaderBasicEnt.kusekiNumTeiseki.PhysicsName) = CInt(ParamList(UpdateParamKeys.CAR_CAPACITY)) - CInt(dr.Item(crsLeaderBasicEnt.blockKakuhoNum.PhysicsName)) - CInt(dr.Item(crsLeaderBasicEnt.kusekiKakuhoNum.PhysicsName)) '←ブロック確保は0のはず
                    End If
                    If IsDBNull(dr.Item(crsLeaderBasicEnt.yoyakuKanouNum.PhysicsName)) = True Then
                        dr.Item(crsLeaderBasicEnt.kusekiNumSubSeat.PhysicsName) = 0 '←TODO:予約可能がNullの場合は0でよいのか？(確認)
                    Else
                        dr.Item(crsLeaderBasicEnt.kusekiNumSubSeat.PhysicsName) = CInt(dr.Item(crsLeaderBasicEnt.yoyakuKanouNum.PhysicsName)) - CInt(ParamList(UpdateParamKeys.CAR_CAPACITY))
                    End If
                    If IsDBNull(dr.Item(crsLeaderBasicEnt.capacityHo1kai.PhysicsName)) = False AndAlso CInt(dr.Item(crsLeaderBasicEnt.kusekiNumSubSeat.PhysicsName)) > CInt(dr.Item(crsLeaderBasicEnt.capacityHo1kai.PhysicsName)) Then
                        dr.Item(crsLeaderBasicEnt.kusekiNumSubSeat.PhysicsName) = dr.Item(crsLeaderBasicEnt.capacityHo1kai.PhysicsName)
                    End If
                End If
            End If
        Next
    End Sub

    ''' <summary>
    ''' コース台帳(降車ヶ所)編集処理
    ''' </summary>
    ''' <param name="zouhatsutOrigData"></param>
    ''' <param name="ParamList"></param>
    Private Sub editDataKoushakashoTable(ByRef zouhatsutOrigData As DataSet, ByVal ParamList As Hashtable)
        Dim koushakashoDt As DataTable
        Dim crsLeaderKoushaEnt As TCrsLedgerKoshakashoEntity = New TCrsLedgerKoshakashoEntity
        koushakashoDt = zouhatsutOrigData.Tables(crsLedgerTblId.crsLeaderKoshakasho)
        '出発時キャリア判定用
        Dim basicData As DataTable
        basicData = zouhatsutOrigData.Tables(crsLedgerTblId.crsLeaderBasic)

        For Each dr As DataRow In koushakashoDt.Rows
            '利用日付
            dr.Item(crsLeaderKoushaEnt.riyouDay.PhysicsName) = dr.Item(crsLeaderKoushaEnt.syuptDay.PhysicsName)
            If (Not IsDBNull(basicData.Rows(0).Item(129)) AndAlso basicData.Rows(0).Item(129) IsNot Nothing) AndAlso
                CType(basicData.Rows(0).Item(129), Integer) = FixedCd.SyuptJiCarrierKbnType.sonota Then
                '出発場所コード・キャリア
                dr.Item(crsLeaderKoushaEnt.syuptPlaceCdCarrier.PhysicsName) = dr.Item(crsLeaderKoushaEnt.syuptPlaceCdCarrier.PhysicsName)
                '出発場所キャリア
                dr.Item(crsLeaderKoushaEnt.syuptPlaceCarrier.PhysicsName) = dr.Item(crsLeaderKoushaEnt.syuptPlaceCarrier.PhysicsName)
                '到着場所コード・キャリア
                dr.Item(crsLeaderKoushaEnt.ttyakPlaceCdCarrier.PhysicsName) = dr.Item(crsLeaderKoushaEnt.ttyakPlaceCdCarrier.PhysicsName)
                '到着場所キャリア
                dr.Item(crsLeaderKoushaEnt.ttyakPlaceCarrier.PhysicsName) = dr.Item(crsLeaderKoushaEnt.ttyakPlaceCarrier.PhysicsName)
                '便名キャリア
                dr.Item(crsLeaderKoushaEnt.binName.PhysicsName) = dr.Item(crsLeaderKoushaEnt.binName.PhysicsName)
            Else
                '出発場所コード・キャリア
                dr.Item(crsLeaderKoushaEnt.syuptPlaceCdCarrier.PhysicsName) = String.Empty
                '出発場所キャリア
                dr.Item(crsLeaderKoushaEnt.syuptPlaceCarrier.PhysicsName) = String.Empty
                '到着場所コード・キャリア
                dr.Item(crsLeaderKoushaEnt.ttyakPlaceCdCarrier.PhysicsName) = String.Empty
                '到着場所キャリア
                dr.Item(crsLeaderKoushaEnt.ttyakPlaceCarrier.PhysicsName) = String.Empty
                '便名キャリア
                dr.Item(crsLeaderKoushaEnt.binName.PhysicsName) = String.Empty
            End If
        Next
    End Sub

    ''' <summary>
    ''' 座席イメージ編集処理
    ''' </summary>
    ''' <param name="zouhatsutOrigData"></param>
    ''' <param name="ParamList"></param>
    Private Sub editDataZasekiImageTable(ByRef zouhatsutOrigData As DataSet, ByVal ParamList As Hashtable)
        Dim zasekiImageDt As DataTable
        Dim crsLedgerBasicDt As DataTable
        Dim zasekiImageEnt As TZasekiImageEntity = New TZasekiImageEntity
        Dim crsLeaderEnt As TCrsLedgerBasicEntity = New TCrsLedgerBasicEntity
        Dim blockHoCnt As Integer = 0
        Dim blockTeiCnt As Integer = 0

        zasekiImageDt = zouhatsutOrigData.Tables(crsLedgerTblId.zasekiImage)
        crsLedgerBasicDt = zouhatsutOrigData.Tables(crsLedgerTblId.crsLeaderBasic)

        For Each dr As DataRow In zasekiImageDt.Rows
            'ブロック確保数	台帳(基本)と同じ値
            dr.Item(zasekiImageEnt.blockKakuhoNum.PhysicsName) = crsLedgerBasicDt.Rows(0).Item(crsLeaderEnt.blockKakuhoNum.PhysicsName)
            '定員補１階	台帳(基本)と同じ値
            dr.Item(zasekiImageEnt.capacityHo1kai.PhysicsName) = crsLedgerBasicDt.Rows(0).Item(crsLeaderEnt.capacityHo1kai.PhysicsName)
            '定員定	台帳(基本)と同じ値
            dr.Item(zasekiImageEnt.capacityRegular.PhysicsName) = crsLedgerBasicDt.Rows(0).Item(crsLeaderEnt.capacityRegular.PhysicsName)
            '車種コード	入力値
            dr.Item(zasekiImageEnt.carTypeCd.PhysicsName) = ParamList.Item(UpdateParamKeys.CAR_TYPE_CD)
            '営ブロック補	"増発先の座席種別が2（補助席）かつ、営業所ブロック区分が1（ブロックあり）の数をカウント"
            '営ブロック定	"増発先の座席種別が1（通常席）かつ、営業所ブロック区分が1（ブロックあり）の数をカウント"
            '空席数定・空席数補
            '⇒外だし
            'グループＮＯＭＡＸ	初期値
            dr.Item(zasekiImageEnt.groupNoMax.PhysicsName) = 0
            '補助席可区分	台帳(基本)と同じ値
            dr.Item(zasekiImageEnt.subSeatOkKbn.PhysicsName) = crsLedgerBasicDt.Rows(0).Item(crsLeaderEnt.subSeatOkKbn.PhysicsName)
            '使用中フラグ	初期値
            dr.Item(zasekiImageEnt.usingFlg.PhysicsName) = String.Empty
        Next
    End Sub


    ''' <summary>
    ''' ブロック数カウント
    ''' </summary>
    Private Sub editDataZasekiBlockCnt(ByRef zouhatsutOrigData As DataSet, ByVal ParamList As Hashtable)
        Dim zasekiImageDt As DataTable
        Dim zasekiImageInfoDt As DataTable
        Dim crsLedgerBasicDt As DataTable

        Dim zasekiImageEnt As TZasekiImageEntity = New TZasekiImageEntity
        Dim crsLeaderEnt As TCrsLedgerBasicEntity = New TCrsLedgerBasicEntity

        Dim blockHoCnt As Integer = 0
        Dim blockTeiCnt As Integer = 0

        zasekiImageDt = zouhatsutOrigData.Tables(crsLedgerTblId.zasekiImage)
        crsLedgerBasicDt = zouhatsutOrigData.Tables(crsLedgerTblId.crsLeaderBasic)
        zasekiImageInfoDt = zouhatsutOrigData.Tables(crsLedgerTblId.zasekiImageInfo)

        ' 営ブロック補算出
        blockHoCnt = zasekiImageInfoDt.Select("ZASEKI_KIND = '" & FixedCd.ZasekiKind.hojoSeat & "' AND EIGYOSYO_BLOCK_KBN = '" & FixedCd.EigyosyoBlockKbn.blockYes & "'").Count

        ' 営ブロック定算出
        blockTeiCnt = zasekiImageInfoDt.Select("ZASEKI_KIND = '" & FixedCd.ZasekiKind.normalSeat & "' AND EIGYOSYO_BLOCK_KBN = '" & FixedCd.EigyosyoBlockKbn.blockYes & "'").Count

        '台帳(基本)
        For Each drBasic As DataRow In crsLedgerBasicDt.Rows
            '営ブロック補	"増発先の座席種別が2（補助席）かつ、営業所ブロック区分が1（ブロックあり）の数をカウント"
            drBasic.Item(crsLeaderEnt.eiBlockHo.PhysicsName) = blockHoCnt
            '営ブロック定	"増発先の座席種別が1（通常席）かつ、営業所ブロック区分が1（ブロックあり）の数をカウント"
            drBasic.Item(crsLeaderEnt.eiBlockRegular.PhysicsName) = blockTeiCnt
            '営業所ブロック数の考慮
            '予約可能数 ＞＝ 定員定の場合
            '空席数定 ＝ 空席数定 - 営ブロック定
            If CType(drBasic.Item(crsLeaderEnt.yoyakuKanouNum.PhysicsName), Integer) >= CType(drBasic.Item(crsLeaderEnt.capacityRegular.PhysicsName), Integer) Then
                drBasic.Item(crsLeaderEnt.kusekiNumTeiseki.PhysicsName) = CType(drBasic.Item(crsLeaderEnt.kusekiNumTeiseki.PhysicsName), Integer) - CType(drBasic.Item(crsLeaderEnt.eiBlockRegular.PhysicsName), Integer)
                ' 空席数補 ＝ 予約可能数 - 定員・定 - 営ブロック補
                drBasic.Item(crsLeaderEnt.kusekiNumSubSeat.PhysicsName) = CType(drBasic.Item(crsLeaderEnt.yoyakuKanouNum.PhysicsName), Integer) - CType(drBasic.Item(crsLeaderEnt.capacityRegular.PhysicsName), Integer) - CType(drBasic.Item(crsLeaderEnt.eiBlockHo.PhysicsName), Integer)
                ' 空席数補 > 定員・補
                If CType(drBasic.Item(crsLeaderEnt.kusekiNumSubSeat.PhysicsName), Integer) > CType(drBasic.Item(crsLeaderEnt.capacityHo1kai.PhysicsName), Integer) Then
                    ' 空席数補 = 定員・補
                    drBasic.Item(crsLeaderEnt.kusekiNumSubSeat.PhysicsName) = drBasic.Item(crsLeaderEnt.capacityHo1kai.PhysicsName)
                End If
            End If
        Next

        '座席イメージ
        For Each drZaseki As DataRow In zasekiImageDt.Rows
            '営ブロック補	"増発先の座席種別が2（補助席）かつ、営業所ブロック区分が1（ブロックあり）の数をカウント"
            drZaseki.Item(zasekiImageEnt.eiBlockHo.PhysicsName) = blockHoCnt
            '空席数補助席	定員補 - 営ブロ補
            If CType(drZaseki.Item(zasekiImageEnt.subSeatOkKbn.PhysicsName), String) = "Y" Then
                drZaseki.Item(zasekiImageEnt.kusekiNumSubSeat.PhysicsName) = CType(drZaseki.Item(zasekiImageEnt.capacityHo1kai.PhysicsName), Integer) - CType(drZaseki.Item(zasekiImageEnt.eiBlockHo.PhysicsName), Integer)
            Else
                drZaseki.Item(zasekiImageEnt.kusekiNumSubSeat.PhysicsName) = 0
            End If
            '営ブロック定	"増発先の座席種別が1（通常席）かつ、営業所ブロック区分が1（ブロックあり）の数をカウント"
            drZaseki.Item(zasekiImageEnt.eiBlockRegular.PhysicsName) = blockTeiCnt
            '空席数定席	定員定 - ブロック確保数 - 空席確保数 - 営ブロック定
            drZaseki.Item(zasekiImageEnt.kusekiNumTeiseki.PhysicsName) = CType(drZaseki.Item(zasekiImageEnt.capacityRegular.PhysicsName), Integer) - CType(drZaseki.Item(zasekiImageEnt.blockKakuhoNum.PhysicsName), Integer) - CType(drZaseki.Item(zasekiImageEnt.kusekiKakuhoNum.PhysicsName), Integer) - CType(drZaseki.Item(zasekiImageEnt.eiBlockRegular.PhysicsName), Integer)
        Next


    End Sub

    ''' <summary>
    ''' 座席イメージ情報編集処理
    ''' </summary>
    ''' <param name="zouhatsutOrigData"></param>
    ''' <param name="ParamList"></param>
    Private Sub editDataZasekiImageInfoTable(ByRef zouhatsutOrigData As DataSet, ByVal ParamList As Hashtable)
        Dim zasekiImageInfoDt As DataTable
        Dim zasekiImageInfoDtClone As DataTable
        Dim zasekiImageEnt As TZasekiImageInfoEntity = New TZasekiImageInfoEntity
        Dim mstZasekiDt As DataTable
        Dim zasekiMstEnt As MZasekiNoEntity = New MZasekiNoEntity
        Dim carCd As String = CType(ParamList(UpdateParamKeys.CAR_TYPE_CD), String)
        Dim carCdBef As String = CType(ParamList(UpdateParamKeys.CAR_TYPE_CD_BEF), String)
        Dim drSelect As DataRow()
        mstZasekiDt = zouhatsutOrigData.Tables(crsLedgerTblId.zasekiNoMst)
        zasekiImageInfoDt = zouhatsutOrigData.Tables(crsLedgerTblId.zasekiImageInfo)
        zasekiImageInfoDtClone = zasekiImageInfoDt.Copy

        If carCd.Equals(carCdBef) Then
            '車種コードが同一の場合
            For Each dr As DataRow In zasekiImageInfoDt.Rows
                '予約が入っていた席の場合
                If CType(dr.Item(zasekiImageEnt.YoyakuFlg.PhysicsName), Integer) = 1 Then
                    '座席状態に0を設定
                    dr.Item(zasekiImageEnt.ZasekiState.PhysicsName) = 0
                End If
                '予約フラグ	0を設定
                dr.Item(zasekiImageEnt.YoyakuFlg.PhysicsName) = 0
                '予約状態	初期値
                dr.Item(zasekiImageEnt.YoyakuStatus.PhysicsName) = String.Empty
                '予約グループNo	初期値
                dr.Item(zasekiImageEnt.GroupNo.PhysicsName) = 0
                '予約区分	初期値
                dr.Item(zasekiImageEnt.YoyakuKbn.PhysicsName) = String.Empty
                '予約番号	初期値
                dr.Item(zasekiImageEnt.YoyakuNo.PhysicsName) = 0
                '連番
                dr.Item(zasekiImageEnt.Nlgsqn.PhysicsName) = 0

            Next
        Else
            '車種コードが異なる場合
            zasekiImageInfoDt.Clear()
            '新車種マスタからデータを作成する
            For Each rowZaseki As DataRow In mstZasekiDt.Rows
                Dim drZasekiImage As DataRow = zasekiImageInfoDt.NewRow
                '出発日
                drZasekiImage(zasekiImageEnt.syuptDay.PhysicsName) = ParamList(UpdateParamKeys.SYUPT_DAY)
                'バス指定コード
                drZasekiImage.Item(zasekiImageEnt.busReserveCd.PhysicsName) = ParamList(UpdateParamKeys.BUS_RESERVE_CD)
                '号車
                drZasekiImage.Item(zasekiImageEnt.gousya.PhysicsName) = ParamList(UpdateParamKeys.GOUSYA)
                '座席階
                drZasekiImage.Item(zasekiImageEnt.zasekiKai.PhysicsName) = rowZaseki.Item(zasekiMstEnt.zasekiKai.PhysicsName)
                '座席行
                drZasekiImage.Item(zasekiImageEnt.zasekiLine.PhysicsName) = rowZaseki.Item(zasekiMstEnt.zasekiLine.PhysicsName)
                '座席列
                drZasekiImage.Item(zasekiImageEnt.zasekiCol.PhysicsName) = rowZaseki.Item(zasekiMstEnt.zasekiCol.PhysicsName)
                '座席種別
                drZasekiImage.Item(zasekiImageEnt.zasekiKind.PhysicsName) = rowZaseki.Item(zasekiMstEnt.zasekiKind.PhysicsName)
                '座席区分
                drZasekiImage.Item(zasekiImageEnt.zasekiKbn.PhysicsName) = String.Empty
                '座席ブロック区分
                drZasekiImage.Item(zasekiImageEnt.blockKbn.PhysicsName) = CStr(0)
                '営業所ブロック区分
                drZasekiImage.Item(zasekiImageEnt.eigyosyoBlockKbn.PhysicsName) = CStr(0)
                '営業所コード
                drZasekiImage.Item(zasekiImageEnt.eigyosyoCd.PhysicsName) = String.Empty
                '女性専用席フラグ
                drZasekiImage.Item(zasekiImageEnt.LadiesSeatFlg.PhysicsName) = CStr(0)
                '座席状態
                If CType(drZasekiImage.Item(zasekiImageEnt.ZasekiKind.PhysicsName), String) = FixedCd.ZasekiKind.notUseSeat Then
                    drZasekiImage.Item(zasekiImageEnt.ZasekiState.PhysicsName) = CStr(901)
                Else
                    drZasekiImage.Item(zasekiImageEnt.ZasekiState.PhysicsName) = CStr(0)
                End If


                If Not FixedCd.ZasekiKind.notUseSeat.ToString.Equals(rowZaseki.Item(zasekiMstEnt.zasekiKind.PhysicsName)) Then
                    'DataTableを取得
                    drSelect = zasekiImageInfoDtClone.Select(zasekiMstEnt.zasekiKai.PhysicsName & "=" & rowZaseki.Item(zasekiMstEnt.zasekiKai.PhysicsName).ToString & " AND " & zasekiMstEnt.zasekiLine.PhysicsName & "=" & rowZaseki.Item(zasekiMstEnt.zasekiLine.PhysicsName).ToString & " AND " & zasekiMstEnt.zasekiCol.PhysicsName & "=" & rowZaseki.Item(zasekiMstEnt.zasekiCol.PhysicsName).ToString)
                    If drSelect.Count > 0 Then
                        '座席区分
                        drZasekiImage.Item(zasekiImageEnt.zasekiKbn.PhysicsName) = drSelect(0).Item(zasekiImageEnt.zasekiKbn.PhysicsName)
                        '座席ブロック区分
                        drZasekiImage.Item(zasekiImageEnt.blockKbn.PhysicsName) = drSelect(0).Item(zasekiImageEnt.blockKbn.PhysicsName)
                        '営業所ブロック区分
                        drZasekiImage.Item(zasekiImageEnt.eigyosyoBlockKbn.PhysicsName) = drSelect(0).Item(zasekiImageEnt.eigyosyoBlockKbn.PhysicsName)
                        '営業所コード
                        drZasekiImage.Item(zasekiImageEnt.eigyosyoCd.PhysicsName) = drSelect(0).Item(zasekiImageEnt.eigyosyoCd.PhysicsName)
                        '女性専用席フラグ
                        drZasekiImage.Item(zasekiImageEnt.LadiesSeatFlg.PhysicsName) = drSelect(0).Item(zasekiImageEnt.LadiesSeatFlg.PhysicsName)
                        '座席状態
                        If CType(drZasekiImage.Item(zasekiImageEnt.EigyosyoBlockKbn.PhysicsName), String) = "1" Then
                            drZasekiImage.Item(zasekiImageEnt.ZasekiState.PhysicsName) = CStr(701)
                        End If
                    End If
                End If

                'Null対策のための一時値設定
                drZasekiImage.Item(CommonColName.SystemEntryPersonCd) = "Dummy"
                drZasekiImage.Item(CommonColName.SystemEntryPgmid) = "Dummy"
                drZasekiImage.Item(CommonColName.SystemUpdateDay) = DateTime.Now
                drZasekiImage.Item(CommonColName.SystemEntryDay) = DateTime.Now
                drZasekiImage.Item(CommonColName.SystemUpdatePersonCd) = "Dummy"
                drZasekiImage.Item(CommonColName.SystemUpdatePgmid) = "Dummy"

                zasekiImageInfoDt.Rows.Add(drZasekiImage)
            Next
        End If
    End Sub

    ''' <summary>
    ''' 共通項目編集処理
    ''' </summary>
    ''' <param name="zouhatsutOrigData"></param>
    ''' <param name="ParamList"></param>
    Private Sub editCommonValue(ByRef zouhatsutOrigData As DataSet, ByVal ParamList As Hashtable)
        Dim dateTime As Date = CommonDateUtil.getSystemTime()
        Dim userId As String = UserInfoManagement.userId
        Dim prmId As String = getMotoGamenId()

        For Each dt As DataTable In zouhatsutOrigData.Tables
            For Each dr As DataRow In dt.Rows
                'システム登録日
                If dt.Columns.Contains(CommonColName.SystemEntryDay) Then
                    dr.Item(CommonColName.SystemEntryDay) = dateTime
                End If
                'システム登録者
                If dt.Columns.Contains(CommonColName.SystemEntryPersonCd) Then
                    dr.Item(CommonColName.SystemEntryPersonCd) = userId
                End If
                'システム登録PGM-ID
                If dt.Columns.Contains(CommonColName.SystemEntryPgmid) Then
                    dr.Item(CommonColName.SystemEntryPgmid) = prmId
                End If
                'システム更新日
                If dt.Columns.Contains(CommonColName.SystemUpdateDay) Then
                    dr.Item(CommonColName.SystemUpdateDay) = dateTime
                End If
                'システム更新者
                If dt.Columns.Contains(CommonColName.SystemUpdatePersonCd) Then
                    dr.Item(CommonColName.SystemUpdatePersonCd) = userId
                End If
                'システム更新PGM-ID
                If dt.Columns.Contains(CommonColName.SystemUpdatePgmid) Then
                    dr.Item(CommonColName.SystemUpdatePgmid) = prmId
                End If
                '削除日
                If dt.Columns.Contains(CommonColName.DeleteDate) Then
                    dr.Item(CommonColName.DeleteDate) = 0
                End If
                '削除日
                If dt.Columns.Contains(CommonColName.DeleteDay) Then
                    dr.Item(CommonColName.DeleteDay) = 0
                End If
                '号車
                If dt.Columns.Contains(CommonColName.Gousya) Then
                    dr.Item(CommonColName.Gousya) = ParamList.Item(UpdateParamKeys.GOUSYA)
                End If
            Next
        Next
    End Sub

    ''' <summary>
    ''' 呼び出し元画面名取得
    ''' </summary>
    ''' <returns></returns>
    Public Function getMotoGamenId() As String
        '画面名
        For i = 1 To 10
            Dim StFrame As New StackFrame(i)
            If InStr(StFrame.GetMethod.DeclaringType.BaseType.FullName, "Hatobus.ReservationManagementSystem.ClientCommon.FormBase") > 0 Then
                Return StFrame.GetMethod.DeclaringType.Name
            End If
        Next

        Return "ZouhatsuDA"
    End Function

#End Region

End Class
