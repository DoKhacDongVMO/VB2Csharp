Imports Oracle.ManagedDataAccess.Client


''' <summary>
''' コース台帳原価（降車ヶ所_料金区分）
''' </summary>
''' <remarks></remarks>
<Serializable()> _
Public Class TCrsLedgerCostKoshakashoChargeKbnEntity  ' コース台帳原価（降車ヶ所_料金区分）エンティティ


Private _syuptDay As New EntityKoumoku_NumberType	' 出発日
Private _crsCd As New EntityKoumoku_MojiType	' コースコード
Private _gousya As New EntityKoumoku_NumberType	' 号車
Private _crsItineraryLineNo As New EntityKoumoku_NumberType	' コース行程行ＮＯ
Private _lineNo As New EntityKoumoku_NumberType	' 行ＮＯ
Private _siireSakiCd As New EntityKoumoku_MojiType	' 仕入先コード
Private _siireSakiEdaban As New EntityKoumoku_MojiType	' 仕入先枝番
Private _heijituTokuteiDayKbn As New EntityKoumoku_MojiType	' 平日／特定日区分
Private _chargeKbnJininCd As New EntityKoumoku_MojiType	' 料金区分（人員）コード
Private _bathTax As New EntityKoumoku_NumberType	' 入湯税
Private _siharaiGaku As New EntityKoumoku_NumberType	' 支払額
Private _kouseiGaku As New EntityKoumoku_NumberType	' 構成額
Private _charge1 As New EntityKoumoku_NumberType	' 料金１
Private _charge2 As New EntityKoumoku_NumberType	' 料金２
Private _charge3 As New EntityKoumoku_NumberType	' 料金３
Private _charge4 As New EntityKoumoku_NumberType	' 料金４
Private _charge5 As New EntityKoumoku_NumberType	' 料金５
Private _systemEntryPgmid As New EntityKoumoku_MojiType	' システム登録ＰＧＭＩＤ
Private _systemEntryPersonCd As New EntityKoumoku_MojiType	' システム登録者コード
Private _systemEntryDay As New EntityKoumoku_YmdType	' システム登録日
Private _systemUpdatePgmid As New EntityKoumoku_MojiType	' システム更新ＰＧＭＩＤ
Private _systemUpdatePersonCd As New EntityKoumoku_MojiType	' システム更新者コード
Private _systemUpdateDay As New EntityKoumoku_YmdType	' システム更新日
Private _deleteDate As New EntityKoumoku_NumberType	' 削除日


Sub New()
_syuptDay.PhysicsName = "SYUPT_DAY"
_crsCd.PhysicsName = "CRS_CD"
_gousya.PhysicsName = "GOUSYA"
_crsItineraryLineNo.PhysicsName = "CRS_ITINERARY_LINE_NO"
_lineNo.PhysicsName = "LINE_NO"
_siireSakiCd.PhysicsName = "SIIRE_SAKI_CD"
_siireSakiEdaban.PhysicsName = "SIIRE_SAKI_EDABAN"
_heijituTokuteiDayKbn.PhysicsName = "HEIJITU_TOKUTEI_DAY_KBN"
_chargeKbnJininCd.PhysicsName = "CHARGE_KBN_JININ_CD"
_bathTax.PhysicsName = "BATH_TAX"
_siharaiGaku.PhysicsName = "SIHARAI_GAKU"
_kouseiGaku.PhysicsName = "KOUSEI_GAKU"
_charge1.PhysicsName = "CHARGE_1"
_charge2.PhysicsName = "CHARGE_2"
_charge3.PhysicsName = "CHARGE_3"
_charge4.PhysicsName = "CHARGE_4"
_charge5.PhysicsName = "CHARGE_5"
_systemEntryPgmid.PhysicsName = "SYSTEM_ENTRY_PGMID"
_systemEntryPersonCd.PhysicsName = "SYSTEM_ENTRY_PERSON_CD"
_systemEntryDay.PhysicsName = "SYSTEM_ENTRY_DAY"
_systemUpdatePgmid.PhysicsName = "SYSTEM_UPDATE_PGMID"
_systemUpdatePersonCd.PhysicsName = "SYSTEM_UPDATE_PERSON_CD"
_systemUpdateDay.PhysicsName = "SYSTEM_UPDATE_DAY"
_deleteDate.PhysicsName = "DELETE_DATE"


_syuptDay.Required = FALSE
_crsCd.Required = FALSE
_gousya.Required = FALSE
_crsItineraryLineNo.Required = FALSE
_lineNo.Required = FALSE
_siireSakiCd.Required = FALSE
_siireSakiEdaban.Required = FALSE
_heijituTokuteiDayKbn.Required = FALSE
_chargeKbnJininCd.Required = FALSE
_bathTax.Required = FALSE
_siharaiGaku.Required = FALSE
_kouseiGaku.Required = FALSE
_charge1.Required = FALSE
_charge2.Required = FALSE
_charge3.Required = FALSE
_charge4.Required = FALSE
_charge5.Required = FALSE
_systemEntryPgmid.Required = TRUE
_systemEntryPersonCd.Required = TRUE
_systemEntryDay.Required = TRUE
_systemUpdatePgmid.Required = TRUE
_systemUpdatePersonCd.Required = TRUE
_systemUpdateDay.Required = TRUE
_deleteDate.Required = FALSE


_syuptDay.DBType = OracleDbType.Decimal
_crsCd.DBType = OracleDbType.Char
_gousya.DBType = OracleDbType.Decimal
_crsItineraryLineNo.DBType = OracleDbType.Decimal
_lineNo.DBType = OracleDbType.Decimal
_siireSakiCd.DBType = OracleDbType.Char
_siireSakiEdaban.DBType = OracleDbType.Char
_heijituTokuteiDayKbn.DBType = OracleDbType.Char
_chargeKbnJininCd.DBType = OracleDbType.Char
_bathTax.DBType = OracleDbType.Decimal
_siharaiGaku.DBType = OracleDbType.Decimal
_kouseiGaku.DBType = OracleDbType.Decimal
_charge1.DBType = OracleDbType.Decimal
_charge2.DBType = OracleDbType.Decimal
_charge3.DBType = OracleDbType.Decimal
_charge4.DBType = OracleDbType.Decimal
_charge5.DBType = OracleDbType.Decimal
_systemEntryPgmid.DBType = OracleDbType.Char
_systemEntryPersonCd.DBType = OracleDbType.Varchar2
_systemEntryDay.DBType = OracleDbType.Date
_systemUpdatePgmid.DBType = OracleDbType.Char
_systemUpdatePersonCd.DBType = OracleDbType.Varchar2
_systemUpdateDay.DBType = OracleDbType.Date
_deleteDate.DBType = OracleDbType.Decimal


_syuptDay.IntegerBu = 8
_crsCd.IntegerBu = 6
_gousya.IntegerBu = 3
_crsItineraryLineNo.IntegerBu = 2
_lineNo.IntegerBu = 3
_siireSakiCd.IntegerBu = 4
_siireSakiEdaban.IntegerBu = 2
_heijituTokuteiDayKbn.IntegerBu = 1
_chargeKbnJininCd.IntegerBu = 2
_bathTax.IntegerBu = 7
_siharaiGaku.IntegerBu = 7
_kouseiGaku.IntegerBu = 7
_charge1.IntegerBu = 7
_charge2.IntegerBu = 7
_charge3.IntegerBu = 7
_charge4.IntegerBu = 7
_charge5.IntegerBu = 7
_systemEntryPgmid.IntegerBu = 8
_systemEntryPersonCd.IntegerBu = 20
_systemEntryDay.IntegerBu = 0
_systemUpdatePgmid.IntegerBu = 8
_systemUpdatePersonCd.IntegerBu = 20
_systemUpdateDay.IntegerBu = 0
_deleteDate.IntegerBu = 8


_syuptDay.DecimalBu = 0
_crsCd.DecimalBu = 0
_gousya.DecimalBu = 0
_crsItineraryLineNo.DecimalBu = 0
_lineNo.DecimalBu = 0
_siireSakiCd.DecimalBu = 0
_siireSakiEdaban.DecimalBu = 0
_heijituTokuteiDayKbn.DecimalBu = 0
_chargeKbnJininCd.DecimalBu = 0
_bathTax.DecimalBu = 0
_siharaiGaku.DecimalBu = 0
_kouseiGaku.DecimalBu = 0
_charge1.DecimalBu = 0
_charge2.DecimalBu = 0
_charge3.DecimalBu = 0
_charge4.DecimalBu = 0
_charge5.DecimalBu = 0
_systemEntryPgmid.DecimalBu = 0
_systemEntryPersonCd.DecimalBu = 0
_systemEntryDay.DecimalBu = 0
_systemUpdatePgmid.DecimalBu = 0
_systemUpdatePersonCd.DecimalBu = 0
_systemUpdateDay.DecimalBu = 0
_deleteDate.DecimalBu = 0
End Sub


''' <summary>
''' syuptDay
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property syuptDay() As EntityKoumoku_NumberType
Get
    Return _syuptDay
End Get
Set(ByVal value As EntityKoumoku_NumberType)
    _syuptDay = value
End Set
End Property


''' <summary>
''' crsCd
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property crsCd() As EntityKoumoku_MojiType
Get
    Return _crsCd
End Get
Set(ByVal value As EntityKoumoku_MojiType)
    _crsCd = value
End Set
End Property


''' <summary>
''' gousya
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property gousya() As EntityKoumoku_NumberType
Get
    Return _gousya
End Get
Set(ByVal value As EntityKoumoku_NumberType)
    _gousya = value
End Set
End Property


''' <summary>
''' crsItineraryLineNo
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property crsItineraryLineNo() As EntityKoumoku_NumberType
Get
    Return _crsItineraryLineNo
End Get
Set(ByVal value As EntityKoumoku_NumberType)
    _crsItineraryLineNo = value
End Set
End Property


''' <summary>
''' lineNo
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property lineNo() As EntityKoumoku_NumberType
Get
    Return _lineNo
End Get
Set(ByVal value As EntityKoumoku_NumberType)
    _lineNo = value
End Set
End Property


''' <summary>
''' siireSakiCd
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property siireSakiCd() As EntityKoumoku_MojiType
Get
    Return _siireSakiCd
End Get
Set(ByVal value As EntityKoumoku_MojiType)
    _siireSakiCd = value
End Set
End Property


''' <summary>
''' siireSakiEdaban
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property siireSakiEdaban() As EntityKoumoku_MojiType
Get
    Return _siireSakiEdaban
End Get
Set(ByVal value As EntityKoumoku_MojiType)
    _siireSakiEdaban = value
End Set
End Property


''' <summary>
''' heijituTokuteiDayKbn
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property heijituTokuteiDayKbn() As EntityKoumoku_MojiType
Get
    Return _heijituTokuteiDayKbn
End Get
Set(ByVal value As EntityKoumoku_MojiType)
    _heijituTokuteiDayKbn = value
End Set
End Property


''' <summary>
''' chargeKbnJininCd
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property chargeKbnJininCd() As EntityKoumoku_MojiType
Get
    Return _chargeKbnJininCd
End Get
Set(ByVal value As EntityKoumoku_MojiType)
    _chargeKbnJininCd = value
End Set
End Property


''' <summary>
''' bathTax
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property bathTax() As EntityKoumoku_NumberType
Get
    Return _bathTax
End Get
Set(ByVal value As EntityKoumoku_NumberType)
    _bathTax = value
End Set
End Property


''' <summary>
''' siharaiGaku
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property siharaiGaku() As EntityKoumoku_NumberType
Get
    Return _siharaiGaku
End Get
Set(ByVal value As EntityKoumoku_NumberType)
    _siharaiGaku = value
End Set
End Property


''' <summary>
''' kouseiGaku
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property kouseiGaku() As EntityKoumoku_NumberType
Get
    Return _kouseiGaku
End Get
Set(ByVal value As EntityKoumoku_NumberType)
    _kouseiGaku = value
End Set
End Property


''' <summary>
''' charge1
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property charge1() As EntityKoumoku_NumberType
Get
    Return _charge1
End Get
Set(ByVal value As EntityKoumoku_NumberType)
    _charge1 = value
End Set
End Property


''' <summary>
''' charge2
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property charge2() As EntityKoumoku_NumberType
Get
    Return _charge2
End Get
Set(ByVal value As EntityKoumoku_NumberType)
    _charge2 = value
End Set
End Property


''' <summary>
''' charge3
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property charge3() As EntityKoumoku_NumberType
Get
    Return _charge3
End Get
Set(ByVal value As EntityKoumoku_NumberType)
    _charge3 = value
End Set
End Property


''' <summary>
''' charge4
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property charge4() As EntityKoumoku_NumberType
Get
    Return _charge4
End Get
Set(ByVal value As EntityKoumoku_NumberType)
    _charge4 = value
End Set
End Property


''' <summary>
''' charge5
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property charge5() As EntityKoumoku_NumberType
Get
    Return _charge5
End Get
Set(ByVal value As EntityKoumoku_NumberType)
    _charge5 = value
End Set
End Property


''' <summary>
''' systemEntryPgmid
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property systemEntryPgmid() As EntityKoumoku_MojiType
Get
    Return _systemEntryPgmid
End Get
Set(ByVal value As EntityKoumoku_MojiType)
    _systemEntryPgmid = value
End Set
End Property


''' <summary>
''' systemEntryPersonCd
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property systemEntryPersonCd() As EntityKoumoku_MojiType
Get
    Return _systemEntryPersonCd
End Get
Set(ByVal value As EntityKoumoku_MojiType)
    _systemEntryPersonCd = value
End Set
End Property


''' <summary>
''' systemEntryDay
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property systemEntryDay() As EntityKoumoku_YmdType
Get
    Return _systemEntryDay
End Get
Set(ByVal value As EntityKoumoku_YmdType)
    _systemEntryDay = value
End Set
End Property


''' <summary>
''' systemUpdatePgmid
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property systemUpdatePgmid() As EntityKoumoku_MojiType
Get
    Return _systemUpdatePgmid
End Get
Set(ByVal value As EntityKoumoku_MojiType)
    _systemUpdatePgmid = value
End Set
End Property


''' <summary>
''' systemUpdatePersonCd
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property systemUpdatePersonCd() As EntityKoumoku_MojiType
Get
    Return _systemUpdatePersonCd
End Get
Set(ByVal value As EntityKoumoku_MojiType)
    _systemUpdatePersonCd = value
End Set
End Property


''' <summary>
''' systemUpdateDay
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property systemUpdateDay() As EntityKoumoku_YmdType
Get
    Return _systemUpdateDay
End Get
Set(ByVal value As EntityKoumoku_YmdType)
    _systemUpdateDay = value
End Set
End Property


''' <summary>
''' deleteDate
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property deleteDate() As EntityKoumoku_NumberType
Get
    Return _deleteDate
End Get
Set(ByVal value As EntityKoumoku_NumberType)
    _deleteDate = value
End Set
End Property


End Class

