Imports Oracle.ManagedDataAccess.Client


''' <summary>
''' コース台帳原価（キャリア）
''' </summary>
''' <remarks></remarks>
<Serializable()> _
Public Class TCrsLedgerCostCarrierEntity  ' コース台帳原価（キャリア）エンティティ


Private _carrierCd As New EntityKoumoku_MojiType	' キャリアコード
Private _carrierEdaban As New EntityKoumoku_MojiType	' キャリア枝番
Private _classLineNo As New EntityKoumoku_NumberType	' クラス行ＮＯ
Private _classWording As New EntityKoumoku_MojiType	' クラス文言
Private _com As New EntityKoumoku_NumberType	' ＣＯＭ
Private _crsCd As New EntityKoumoku_MojiType	' コースコード
Private _crsItineraryLineNo As New EntityKoumoku_NumberType	' コース行程行ＮＯ
Private _deleteDay As New EntityKoumoku_NumberType	' 削除日
Private _gousya As New EntityKoumoku_NumberType	' 号車
Private _lineNo As New EntityKoumoku_NumberType	' 行ＮＯ
Private _riyouDay As New EntityKoumoku_NumberType	' 利用日
Private _seisanHoho As New EntityKoumoku_MojiType	' 精算方法
Private _syuptDay As New EntityKoumoku_NumberType	' 出発日
Private _taxKbn As New EntityKoumoku_MojiType	' 税区分
Private _systemEntryPgmid As New EntityKoumoku_MojiType	' システム登録ＰＧＭＩＤ
Private _systemEntryPersonCd As New EntityKoumoku_MojiType	' システム登録者コード
Private _systemEntryDay As New EntityKoumoku_YmdType	' システム登録日
Private _systemUpdatePgmid As New EntityKoumoku_MojiType	' システム更新ＰＧＭＩＤ
Private _systemUpdatePersonCd As New EntityKoumoku_MojiType	' システム更新者コード
Private _systemUpdateDay As New EntityKoumoku_YmdType	' システム更新日


Sub New()
_carrierCd.PhysicsName = "CARRIER_CD"
_carrierEdaban.PhysicsName = "CARRIER_EDABAN"
_classLineNo.PhysicsName = "CLASS_LINE_NO"
_classWording.PhysicsName = "CLASS_WORDING"
_com.PhysicsName = "COM"
_crsCd.PhysicsName = "CRS_CD"
_crsItineraryLineNo.PhysicsName = "CRS_ITINERARY_LINE_NO"
_deleteDay.PhysicsName = "DELETE_DAY"
_gousya.PhysicsName = "GOUSYA"
_lineNo.PhysicsName = "LINE_NO"
_riyouDay.PhysicsName = "RIYOU_DAY"
_seisanHoho.PhysicsName = "SEISAN_HOHO"
_syuptDay.PhysicsName = "SYUPT_DAY"
_taxKbn.PhysicsName = "TAX_KBN"
_systemEntryPgmid.PhysicsName = "SYSTEM_ENTRY_PGMID"
_systemEntryPersonCd.PhysicsName = "SYSTEM_ENTRY_PERSON_CD"
_systemEntryDay.PhysicsName = "SYSTEM_ENTRY_DAY"
_systemUpdatePgmid.PhysicsName = "SYSTEM_UPDATE_PGMID"
_systemUpdatePersonCd.PhysicsName = "SYSTEM_UPDATE_PERSON_CD"
_systemUpdateDay.PhysicsName = "SYSTEM_UPDATE_DAY"


_carrierCd.Required = FALSE
_carrierEdaban.Required = FALSE
_classLineNo.Required = FALSE
_classWording.Required = FALSE
_com.Required = FALSE
_crsCd.Required = FALSE
_crsItineraryLineNo.Required = FALSE
_deleteDay.Required = FALSE
_gousya.Required = FALSE
_lineNo.Required = FALSE
_riyouDay.Required = FALSE
_seisanHoho.Required = FALSE
_syuptDay.Required = FALSE
_taxKbn.Required = FALSE
_systemEntryPgmid.Required = TRUE
_systemEntryPersonCd.Required = TRUE
_systemEntryDay.Required = TRUE
_systemUpdatePgmid.Required = TRUE
_systemUpdatePersonCd.Required = TRUE
_systemUpdateDay.Required = TRUE


_carrierCd.DBType = OracleDbType.Char
_carrierEdaban.DBType = OracleDbType.Char
_classLineNo.DBType = OracleDbType.Decimal
_classWording.DBType = OracleDbType.Varchar2
_com.DBType = OracleDbType.Decimal
_crsCd.DBType = OracleDbType.Char
_crsItineraryLineNo.DBType = OracleDbType.Decimal
_deleteDay.DBType = OracleDbType.Decimal
_gousya.DBType = OracleDbType.Decimal
_lineNo.DBType = OracleDbType.Decimal
_riyouDay.DBType = OracleDbType.Decimal
_seisanHoho.DBType = OracleDbType.Char
_syuptDay.DBType = OracleDbType.Decimal
_taxKbn.DBType = OracleDbType.Char
_systemEntryPgmid.DBType = OracleDbType.Char
_systemEntryPersonCd.DBType = OracleDbType.Varchar2
_systemEntryDay.DBType = OracleDbType.Date
_systemUpdatePgmid.DBType = OracleDbType.Char
_systemUpdatePersonCd.DBType = OracleDbType.Varchar2
_systemUpdateDay.DBType = OracleDbType.Date


_carrierCd.IntegerBu = 4
_carrierEdaban.IntegerBu = 2
_classLineNo.IntegerBu = 1
_classWording.IntegerBu = 16
_com.IntegerBu = 3
_crsCd.IntegerBu = 6
_crsItineraryLineNo.IntegerBu = 2
_deleteDay.IntegerBu = 8
_gousya.IntegerBu = 3
_lineNo.IntegerBu = 3
_riyouDay.IntegerBu = 8
_seisanHoho.IntegerBu = 1
_syuptDay.IntegerBu = 8
_taxKbn.IntegerBu = 1
_systemEntryPgmid.IntegerBu = 8
_systemEntryPersonCd.IntegerBu = 20
_systemEntryDay.IntegerBu = 0
_systemUpdatePgmid.IntegerBu = 8
_systemUpdatePersonCd.IntegerBu = 20
_systemUpdateDay.IntegerBu = 0


_carrierCd.DecimalBu = 0
_carrierEdaban.DecimalBu = 0
_classLineNo.DecimalBu = 0
_classWording.DecimalBu = 0
_com.DecimalBu = 0
_crsCd.DecimalBu = 0
_crsItineraryLineNo.DecimalBu = 0
_deleteDay.DecimalBu = 0
_gousya.DecimalBu = 0
_lineNo.DecimalBu = 0
_riyouDay.DecimalBu = 0
_seisanHoho.DecimalBu = 0
_syuptDay.DecimalBu = 0
_taxKbn.DecimalBu = 0
_systemEntryPgmid.DecimalBu = 0
_systemEntryPersonCd.DecimalBu = 0
_systemEntryDay.DecimalBu = 0
_systemUpdatePgmid.DecimalBu = 0
_systemUpdatePersonCd.DecimalBu = 0
_systemUpdateDay.DecimalBu = 0
End Sub


''' <summary>
''' carrierCd
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property carrierCd() As EntityKoumoku_MojiType
Get
    Return _carrierCd
End Get
Set(ByVal value As EntityKoumoku_MojiType)
    _carrierCd = value
End Set
End Property


''' <summary>
''' carrierEdaban
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property carrierEdaban() As EntityKoumoku_MojiType
Get
    Return _carrierEdaban
End Get
Set(ByVal value As EntityKoumoku_MojiType)
    _carrierEdaban = value
End Set
End Property


''' <summary>
''' classLineNo
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property classLineNo() As EntityKoumoku_NumberType
Get
    Return _classLineNo
End Get
Set(ByVal value As EntityKoumoku_NumberType)
    _classLineNo = value
End Set
End Property


''' <summary>
''' classWording
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property classWording() As EntityKoumoku_MojiType
Get
    Return _classWording
End Get
Set(ByVal value As EntityKoumoku_MojiType)
    _classWording = value
End Set
End Property


''' <summary>
''' com
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property com() As EntityKoumoku_NumberType
Get
    Return _com
End Get
Set(ByVal value As EntityKoumoku_NumberType)
    _com = value
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
''' deleteDay
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property deleteDay() As EntityKoumoku_NumberType
Get
    Return _deleteDay
End Get
Set(ByVal value As EntityKoumoku_NumberType)
    _deleteDay = value
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
''' riyouDay
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property riyouDay() As EntityKoumoku_NumberType
Get
    Return _riyouDay
End Get
Set(ByVal value As EntityKoumoku_NumberType)
    _riyouDay = value
End Set
End Property


''' <summary>
''' seisanHoho
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property seisanHoho() As EntityKoumoku_MojiType
Get
    Return _seisanHoho
End Get
Set(ByVal value As EntityKoumoku_MojiType)
    _seisanHoho = value
End Set
End Property


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
''' taxKbn
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property taxKbn() As EntityKoumoku_MojiType
Get
    Return _taxKbn
End Get
Set(ByVal value As EntityKoumoku_MojiType)
    _taxKbn = value
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


End Class

