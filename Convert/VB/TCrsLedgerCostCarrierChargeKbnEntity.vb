Imports Oracle.ManagedDataAccess.Client


''' <summary>
''' �R�[�X�䒠�����i�L�����A_�����敪�j
''' </summary>
''' <remarks></remarks>
<Serializable()> _
Public Class TCrsLedgerCostCarrierChargeKbnEntity  ' �R�[�X�䒠�����i�L�����A_�����敪�j�G���e�B�e�B


Private _syuptDay As New EntityKoumoku_NumberType	' �o����
Private _crsCd As New EntityKoumoku_MojiType	' �R�[�X�R�[�h
Private _gousya As New EntityKoumoku_NumberType	' ����
Private _crsItineraryLineNo As New EntityKoumoku_NumberType	' �R�[�X�s���s�m�n
Private _lineNo As New EntityKoumoku_NumberType	' �s�m�n
Private _carrierCd As New EntityKoumoku_MojiType	' �L�����A�R�[�h
Private _carrierEdaban As New EntityKoumoku_MojiType	' �L�����A�}��
Private _chargeKbnJininCd As New EntityKoumoku_MojiType	' �����敪�i�l���j�R�[�h
Private _tanka As New EntityKoumoku_NumberType	' �P��
Private _systemEntryPgmid As New EntityKoumoku_MojiType	' �V�X�e���o�^�o�f�l�h�c
Private _systemEntryPersonCd As New EntityKoumoku_MojiType	' �V�X�e���o�^�҃R�[�h
Private _systemEntryDay As New EntityKoumoku_YmdType	' �V�X�e���o�^��
Private _systemUpdatePgmid As New EntityKoumoku_MojiType	' �V�X�e���X�V�o�f�l�h�c
Private _systemUpdatePersonCd As New EntityKoumoku_MojiType	' �V�X�e���X�V�҃R�[�h
Private _systemUpdateDay As New EntityKoumoku_YmdType	' �V�X�e���X�V��
Private _deleteDate As New EntityKoumoku_NumberType	' �폜��


Sub New()
_syuptDay.PhysicsName = "SYUPT_DAY"
_crsCd.PhysicsName = "CRS_CD"
_gousya.PhysicsName = "GOUSYA"
_crsItineraryLineNo.PhysicsName = "CRS_ITINERARY_LINE_NO"
_lineNo.PhysicsName = "LINE_NO"
_carrierCd.PhysicsName = "CARRIER_CD"
_carrierEdaban.PhysicsName = "CARRIER_EDABAN"
_chargeKbnJininCd.PhysicsName = "CHARGE_KBN_JININ_CD"
_tanka.PhysicsName = "TANKA"
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
_carrierCd.Required = FALSE
_carrierEdaban.Required = FALSE
_chargeKbnJininCd.Required = FALSE
_tanka.Required = FALSE
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
_carrierCd.DBType = OracleDbType.Char
_carrierEdaban.DBType = OracleDbType.Char
_chargeKbnJininCd.DBType = OracleDbType.Char
_tanka.DBType = OracleDbType.Decimal
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
_carrierCd.IntegerBu = 4
_carrierEdaban.IntegerBu = 2
_chargeKbnJininCd.IntegerBu = 2
_tanka.IntegerBu = 7
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
_carrierCd.DecimalBu = 0
_carrierEdaban.DecimalBu = 0
_chargeKbnJininCd.DecimalBu = 0
_tanka.DecimalBu = 0
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
''' tanka
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property tanka() As EntityKoumoku_NumberType
Get
    Return _tanka
End Get
Set(ByVal value As EntityKoumoku_NumberType)
    _tanka = value
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

