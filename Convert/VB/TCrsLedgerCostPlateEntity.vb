Imports Oracle.ManagedDataAccess.Client


''' <summary>
''' コース台帳原価（プレート）
''' </summary>
''' <remarks></remarks>
<Serializable()> _
Public Class TCrsLedgerCostPlateEntity  ' コース台帳原価（プレート）エンティティ


Private _crsCd As New EntityKoumoku_MojiType	' コースコード
Private _deleteDay As New EntityKoumoku_NumberType	' 削除日
Private _gousya As New EntityKoumoku_NumberType	' 号車
Private _kingaku As New EntityKoumoku_NumberType	' 金額
Private _kukanFrom As New EntityKoumoku_MojiType	' 区間自
Private _kukanTo As New EntityKoumoku_MojiType	' 区間至
Private _lineNo As New EntityKoumoku_NumberType	' 行ＮＯ
Private _syuptDay As New EntityKoumoku_NumberType	' 出発日
Private _systemEntryPgmid As New EntityKoumoku_MojiType	' システム登録ＰＧＭＩＤ
Private _systemEntryPersonCd As New EntityKoumoku_MojiType	' システム登録者コード
Private _systemEntryDay As New EntityKoumoku_YmdType	' システム登録日
Private _systemUpdatePgmid As New EntityKoumoku_MojiType	' システム更新ＰＧＭＩＤ
Private _systemUpdatePersonCd As New EntityKoumoku_MojiType	' システム更新者コード
Private _systemUpdateDay As New EntityKoumoku_YmdType	' システム更新日


Sub New()
_crsCd.PhysicsName = "CRS_CD"
_deleteDay.PhysicsName = "DELETE_DAY"
_gousya.PhysicsName = "GOUSYA"
_kingaku.PhysicsName = "KINGAKU"
_kukanFrom.PhysicsName = "KUKAN_FROM"
_kukanTo.PhysicsName = "KUKAN_TO"
_lineNo.PhysicsName = "LINE_NO"
_syuptDay.PhysicsName = "SYUPT_DAY"
_systemEntryPgmid.PhysicsName = "SYSTEM_ENTRY_PGMID"
_systemEntryPersonCd.PhysicsName = "SYSTEM_ENTRY_PERSON_CD"
_systemEntryDay.PhysicsName = "SYSTEM_ENTRY_DAY"
_systemUpdatePgmid.PhysicsName = "SYSTEM_UPDATE_PGMID"
_systemUpdatePersonCd.PhysicsName = "SYSTEM_UPDATE_PERSON_CD"
_systemUpdateDay.PhysicsName = "SYSTEM_UPDATE_DAY"


_crsCd.Required = FALSE
_deleteDay.Required = FALSE
_gousya.Required = FALSE
_kingaku.Required = FALSE
_kukanFrom.Required = FALSE
_kukanTo.Required = FALSE
_lineNo.Required = FALSE
_syuptDay.Required = FALSE
_systemEntryPgmid.Required = TRUE
_systemEntryPersonCd.Required = TRUE
_systemEntryDay.Required = TRUE
_systemUpdatePgmid.Required = TRUE
_systemUpdatePersonCd.Required = TRUE
_systemUpdateDay.Required = TRUE


_crsCd.DBType = OracleDbType.Char
_deleteDay.DBType = OracleDbType.Decimal
_gousya.DBType = OracleDbType.Decimal
_kingaku.DBType = OracleDbType.Decimal
_kukanFrom.DBType = OracleDbType.Varchar2
_kukanTo.DBType = OracleDbType.Varchar2
_lineNo.DBType = OracleDbType.Decimal
_syuptDay.DBType = OracleDbType.Decimal
_systemEntryPgmid.DBType = OracleDbType.Char
_systemEntryPersonCd.DBType = OracleDbType.Varchar2
_systemEntryDay.DBType = OracleDbType.Date
_systemUpdatePgmid.DBType = OracleDbType.Char
_systemUpdatePersonCd.DBType = OracleDbType.Varchar2
_systemUpdateDay.DBType = OracleDbType.Date


_crsCd.IntegerBu = 6
_deleteDay.IntegerBu = 8
_gousya.IntegerBu = 3
_kingaku.IntegerBu = 7
_kukanFrom.IntegerBu = 12
_kukanTo.IntegerBu = 12
_lineNo.IntegerBu = 2
_syuptDay.IntegerBu = 8
_systemEntryPgmid.IntegerBu = 8
_systemEntryPersonCd.IntegerBu = 20
_systemEntryDay.IntegerBu = 0
_systemUpdatePgmid.IntegerBu = 8
_systemUpdatePersonCd.IntegerBu = 20
_systemUpdateDay.IntegerBu = 0


_crsCd.DecimalBu = 0
_deleteDay.DecimalBu = 0
_gousya.DecimalBu = 0
_kingaku.DecimalBu = 0
_kukanFrom.DecimalBu = 0
_kukanTo.DecimalBu = 0
_lineNo.DecimalBu = 0
_syuptDay.DecimalBu = 0
_systemEntryPgmid.DecimalBu = 0
_systemEntryPersonCd.DecimalBu = 0
_systemEntryDay.DecimalBu = 0
_systemUpdatePgmid.DecimalBu = 0
_systemUpdatePersonCd.DecimalBu = 0
_systemUpdateDay.DecimalBu = 0
End Sub


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
''' kingaku
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property kingaku() As EntityKoumoku_NumberType
Get
    Return _kingaku
End Get
Set(ByVal value As EntityKoumoku_NumberType)
    _kingaku = value
End Set
End Property


''' <summary>
''' kukanFrom
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property kukanFrom() As EntityKoumoku_MojiType
Get
    Return _kukanFrom
End Get
Set(ByVal value As EntityKoumoku_MojiType)
    _kukanFrom = value
End Set
End Property


''' <summary>
''' kukanTo
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property kukanTo() As EntityKoumoku_MojiType
Get
    Return _kukanTo
End Get
Set(ByVal value As EntityKoumoku_MojiType)
    _kukanTo = value
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


End Class

