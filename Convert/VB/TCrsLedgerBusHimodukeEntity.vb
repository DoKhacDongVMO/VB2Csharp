Imports Oracle.ManagedDataAccess.Client


''' <summary>
''' �R�[�X�䒠�i�o�X�R�Â��j
''' </summary>
''' <remarks></remarks>
<Serializable()> _
Public Class TCrsLedgerBusHimodukeEntity  ' �R�[�X�䒠�i�o�X�R�Â��j�G���e�B�e�B


Private _busCompanyCd As New EntityKoumoku_MojiType	' �o�X��ЃR�[�h
Private _busCompanyKakuteiDay As New EntityKoumoku_NumberType	' �o�X��Њm���
Private _busCompanyKakuteiTime As New EntityKoumoku_NumberType	' �o�X��Њm�莞��
Private _crsCd As New EntityKoumoku_MojiType	' �R�[�X�R�[�h
Private _gousya As New EntityKoumoku_NumberType	' ����
Private _hanbaiStartJiKakuteiFlg As New EntityKoumoku_MojiType	' �̔��J�n���m��t���O
Private _oldBusCompanyCd As New EntityKoumoku_MojiType	' ���o�X��ЃR�[�h
Private _syuptDay As New EntityKoumoku_NumberType	' �o����
Private _systemEntryPgmid As New EntityKoumoku_MojiType	' �V�X�e���o�^�o�f�l�h�c
Private _systemEntryPersonCd As New EntityKoumoku_MojiType	' �V�X�e���o�^�҃R�[�h
Private _systemEntryDay As New EntityKoumoku_YmdType	' �V�X�e���o�^��
Private _systemUpdatePgmid As New EntityKoumoku_MojiType	' �V�X�e���X�V�o�f�l�h�c
Private _systemUpdatePersonCd As New EntityKoumoku_MojiType	' �V�X�e���X�V�҃R�[�h
Private _systemUpdateDay As New EntityKoumoku_YmdType	' �V�X�e���X�V��
Private _deleteDate As New EntityKoumoku_NumberType	' �폜��


Sub New()
_busCompanyCd.PhysicsName = "BUS_COMPANY_CD"
_busCompanyKakuteiDay.PhysicsName = "BUS_COMPANY_KAKUTEI_DAY"
_busCompanyKakuteiTime.PhysicsName = "BUS_COMPANY_KAKUTEI_TIME"
_crsCd.PhysicsName = "CRS_CD"
_gousya.PhysicsName = "GOUSYA"
_hanbaiStartJiKakuteiFlg.PhysicsName = "HANBAI_START_JI_KAKUTEI_FLG"
_oldBusCompanyCd.PhysicsName = "OLD_BUS_COMPANY_CD"
_syuptDay.PhysicsName = "SYUPT_DAY"
_systemEntryPgmid.PhysicsName = "SYSTEM_ENTRY_PGMID"
_systemEntryPersonCd.PhysicsName = "SYSTEM_ENTRY_PERSON_CD"
_systemEntryDay.PhysicsName = "SYSTEM_ENTRY_DAY"
_systemUpdatePgmid.PhysicsName = "SYSTEM_UPDATE_PGMID"
_systemUpdatePersonCd.PhysicsName = "SYSTEM_UPDATE_PERSON_CD"
_systemUpdateDay.PhysicsName = "SYSTEM_UPDATE_DAY"
_deleteDate.PhysicsName = "DELETE_DATE"


_busCompanyCd.Required = FALSE
_busCompanyKakuteiDay.Required = FALSE
_busCompanyKakuteiTime.Required = FALSE
_crsCd.Required = FALSE
_gousya.Required = FALSE
_hanbaiStartJiKakuteiFlg.Required = FALSE
_oldBusCompanyCd.Required = FALSE
_syuptDay.Required = FALSE
_systemEntryPgmid.Required = TRUE
_systemEntryPersonCd.Required = TRUE
_systemEntryDay.Required = TRUE
_systemUpdatePgmid.Required = TRUE
_systemUpdatePersonCd.Required = TRUE
_systemUpdateDay.Required = TRUE
_deleteDate.Required = FALSE


_busCompanyCd.DBType = OracleDbType.Char
_busCompanyKakuteiDay.DBType = OracleDbType.Decimal
_busCompanyKakuteiTime.DBType = OracleDbType.Decimal
_crsCd.DBType = OracleDbType.Char
_gousya.DBType = OracleDbType.Decimal
_hanbaiStartJiKakuteiFlg.DBType = OracleDbType.Char
_oldBusCompanyCd.DBType = OracleDbType.Char
_syuptDay.DBType = OracleDbType.Decimal
_systemEntryPgmid.DBType = OracleDbType.Char
_systemEntryPersonCd.DBType = OracleDbType.Varchar2
_systemEntryDay.DBType = OracleDbType.Date
_systemUpdatePgmid.DBType = OracleDbType.Char
_systemUpdatePersonCd.DBType = OracleDbType.Varchar2
_systemUpdateDay.DBType = OracleDbType.Date
_deleteDate.DBType = OracleDbType.Decimal


_busCompanyCd.IntegerBu = 6
_busCompanyKakuteiDay.IntegerBu = 8
_busCompanyKakuteiTime.IntegerBu = 6
_crsCd.IntegerBu = 6
_gousya.IntegerBu = 3
_hanbaiStartJiKakuteiFlg.IntegerBu = 1
_oldBusCompanyCd.IntegerBu = 6
_syuptDay.IntegerBu = 8
_systemEntryPgmid.IntegerBu = 8
_systemEntryPersonCd.IntegerBu = 20
_systemEntryDay.IntegerBu = 0
_systemUpdatePgmid.IntegerBu = 8
_systemUpdatePersonCd.IntegerBu = 20
_systemUpdateDay.IntegerBu = 0
_deleteDate.IntegerBu = 8


_busCompanyCd.DecimalBu = 0
_busCompanyKakuteiDay.DecimalBu = 0
_busCompanyKakuteiTime.DecimalBu = 0
_crsCd.DecimalBu = 0
_gousya.DecimalBu = 0
_hanbaiStartJiKakuteiFlg.DecimalBu = 0
_oldBusCompanyCd.DecimalBu = 0
_syuptDay.DecimalBu = 0
_systemEntryPgmid.DecimalBu = 0
_systemEntryPersonCd.DecimalBu = 0
_systemEntryDay.DecimalBu = 0
_systemUpdatePgmid.DecimalBu = 0
_systemUpdatePersonCd.DecimalBu = 0
_systemUpdateDay.DecimalBu = 0
_deleteDate.DecimalBu = 0
End Sub


''' <summary>
''' busCompanyCd
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property busCompanyCd() As EntityKoumoku_MojiType
Get
    Return _busCompanyCd
End Get
Set(ByVal value As EntityKoumoku_MojiType)
    _busCompanyCd = value
End Set
End Property


''' <summary>
''' busCompanyKakuteiDay
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property busCompanyKakuteiDay() As EntityKoumoku_NumberType
Get
    Return _busCompanyKakuteiDay
End Get
Set(ByVal value As EntityKoumoku_NumberType)
    _busCompanyKakuteiDay = value
End Set
End Property


''' <summary>
''' busCompanyKakuteiTime
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property busCompanyKakuteiTime() As EntityKoumoku_NumberType
Get
    Return _busCompanyKakuteiTime
End Get
Set(ByVal value As EntityKoumoku_NumberType)
    _busCompanyKakuteiTime = value
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
''' hanbaiStartJiKakuteiFlg
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property hanbaiStartJiKakuteiFlg() As EntityKoumoku_MojiType
Get
    Return _hanbaiStartJiKakuteiFlg
End Get
Set(ByVal value As EntityKoumoku_MojiType)
    _hanbaiStartJiKakuteiFlg = value
End Set
End Property


''' <summary>
''' oldBusCompanyCd
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property oldBusCompanyCd() As EntityKoumoku_MojiType
Get
    Return _oldBusCompanyCd
End Get
Set(ByVal value As EntityKoumoku_MojiType)
    _oldBusCompanyCd = value
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

