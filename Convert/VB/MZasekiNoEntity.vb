Imports Oracle.ManagedDataAccess.Client


''' <summary>
''' 座席番号マスタ
''' </summary>
''' <remarks></remarks>
<Serializable()> _
Public Class MZasekiNoEntity  ' 座席番号マスタエンティティ


Private _carCd As New EntityKoumoku_MojiType	' 車種コード
Private _zasekiKai As New EntityKoumoku_NumberType	' 座席階
Private _zasekiLine As New EntityKoumoku_NumberType	' 座席行
Private _zasekiCol As New EntityKoumoku_NumberType	' 座席列
Private _zasekiName As New EntityKoumoku_MojiType	' 座席名
Private _zasekiKind As New EntityKoumoku_MojiType	' 座席種別
Private _systemEntryPgmid As New EntityKoumoku_MojiType	' システム登録ＰＧＭＩＤ
Private _systemEntryPersonCd As New EntityKoumoku_MojiType	' システム登録者コード
Private _systemEntryDay As New EntityKoumoku_YmdType	' システム登録日
Private _systemUpdatePgmid As New EntityKoumoku_MojiType	' システム更新ＰＧＭＩＤ
Private _systemUpdatePersonCd As New EntityKoumoku_MojiType	' システム更新者コード
Private _systemUpdateDay As New EntityKoumoku_YmdType	' システム更新日


Sub New()
_carCd.PhysicsName = "CAR_CD"
_zasekiKai.PhysicsName = "ZASEKI_KAI"
_zasekiLine.PhysicsName = "ZASEKI_LINE"
_zasekiCol.PhysicsName = "ZASEKI_COL"
_zasekiName.PhysicsName = "ZASEKI_NAME"
_zasekiKind.PhysicsName = "ZASEKI_KIND"
_systemEntryPgmid.PhysicsName = "SYSTEM_ENTRY_PGMID"
_systemEntryPersonCd.PhysicsName = "SYSTEM_ENTRY_PERSON_CD"
_systemEntryDay.PhysicsName = "SYSTEM_ENTRY_DAY"
_systemUpdatePgmid.PhysicsName = "SYSTEM_UPDATE_PGMID"
_systemUpdatePersonCd.PhysicsName = "SYSTEM_UPDATE_PERSON_CD"
_systemUpdateDay.PhysicsName = "SYSTEM_UPDATE_DAY"


_carCd.Required = FALSE
_zasekiKai.Required = FALSE
_zasekiLine.Required = FALSE
_zasekiCol.Required = FALSE
_zasekiName.Required = FALSE
_zasekiKind.Required = FALSE
_systemEntryPgmid.Required = TRUE
_systemEntryPersonCd.Required = TRUE
_systemEntryDay.Required = TRUE
_systemUpdatePgmid.Required = TRUE
_systemUpdatePersonCd.Required = TRUE
_systemUpdateDay.Required = TRUE


_carCd.DBType = OracleDbType.Char
_zasekiKai.DBType = OracleDbType.Decimal
_zasekiLine.DBType = OracleDbType.Decimal
_zasekiCol.DBType = OracleDbType.Decimal
_zasekiName.DBType = OracleDbType.Varchar2
_zasekiKind.DBType = OracleDbType.Char
_systemEntryPgmid.DBType = OracleDbType.Char
_systemEntryPersonCd.DBType = OracleDbType.Varchar2
_systemEntryDay.DBType = OracleDbType.Date
_systemUpdatePgmid.DBType = OracleDbType.Char
_systemUpdatePersonCd.DBType = OracleDbType.Varchar2
_systemUpdateDay.DBType = OracleDbType.Date


_carCd.IntegerBu = 2
_zasekiKai.IntegerBu = 1
_zasekiLine.IntegerBu = 2
_zasekiCol.IntegerBu = 2
_zasekiName.IntegerBu = 3
_zasekiKind.IntegerBu = 1
_systemEntryPgmid.IntegerBu = 8
_systemEntryPersonCd.IntegerBu = 20
_systemEntryDay.IntegerBu = 0
_systemUpdatePgmid.IntegerBu = 8
_systemUpdatePersonCd.IntegerBu = 20
_systemUpdateDay.IntegerBu = 0


_carCd.DecimalBu = 0
_zasekiKai.DecimalBu = 0
_zasekiLine.DecimalBu = 0
_zasekiCol.DecimalBu = 0
_zasekiName.DecimalBu = 0
_zasekiKind.DecimalBu = 0
_systemEntryPgmid.DecimalBu = 0
_systemEntryPersonCd.DecimalBu = 0
_systemEntryDay.DecimalBu = 0
_systemUpdatePgmid.DecimalBu = 0
_systemUpdatePersonCd.DecimalBu = 0
_systemUpdateDay.DecimalBu = 0
End Sub


''' <summary>
''' carCd
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property carCd() As EntityKoumoku_MojiType
Get
    Return _carCd
End Get
Set(ByVal value As EntityKoumoku_MojiType)
    _carCd = value
End Set
End Property


''' <summary>
''' zasekiKai
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property zasekiKai() As EntityKoumoku_NumberType
Get
    Return _zasekiKai
End Get
Set(ByVal value As EntityKoumoku_NumberType)
    _zasekiKai = value
End Set
End Property


''' <summary>
''' zasekiLine
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property zasekiLine() As EntityKoumoku_NumberType
Get
    Return _zasekiLine
End Get
Set(ByVal value As EntityKoumoku_NumberType)
    _zasekiLine = value
End Set
End Property


''' <summary>
''' zasekiCol
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property zasekiCol() As EntityKoumoku_NumberType
Get
    Return _zasekiCol
End Get
Set(ByVal value As EntityKoumoku_NumberType)
    _zasekiCol = value
End Set
End Property


''' <summary>
''' zasekiName
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property zasekiName() As EntityKoumoku_MojiType
Get
    Return _zasekiName
End Get
Set(ByVal value As EntityKoumoku_MojiType)
    _zasekiName = value
End Set
End Property


''' <summary>
''' zasekiKind
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property zasekiKind() As EntityKoumoku_MojiType
Get
    Return _zasekiKind
End Get
Set(ByVal value As EntityKoumoku_MojiType)
    _zasekiKind = value
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

