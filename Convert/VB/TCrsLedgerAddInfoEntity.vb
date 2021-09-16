Imports Oracle.ManagedDataAccess.Client


''' <summary>
''' ÉRÅ[ÉXë‰í†Åiïtâ¡èÓïÒÅj
''' </summary>
''' <remarks></remarks>
<Serializable()> _
Public Class TCrsLedgerAddInfoEntity  ' ÉRÅ[ÉXë‰í†Åiïtâ¡èÓïÒÅjÉGÉìÉeÉBÉeÉB


Private _controlFlg1 As New EntityKoumoku_MojiType	' êßå‰ÉtÉâÉOÇOÇP
Private _controlFlg10 As New EntityKoumoku_MojiType	' êßå‰ÉtÉâÉOÇPÇO
Private _controlFlg2 As New EntityKoumoku_MojiType	' êßå‰ÉtÉâÉOÇOÇQ
Private _controlFlg3 As New EntityKoumoku_MojiType	' êßå‰ÉtÉâÉOÇOÇR
Private _controlFlg4 As New EntityKoumoku_MojiType	' êßå‰ÉtÉâÉOÇOÇS
Private _controlFlg5 As New EntityKoumoku_MojiType	' êßå‰ÉtÉâÉOÇOÇT
Private _controlFlg6 As New EntityKoumoku_MojiType	' êßå‰ÉtÉâÉOÇOÇU
Private _controlFlg7 As New EntityKoumoku_MojiType	' êßå‰ÉtÉâÉOÇOÇV
Private _controlFlg8 As New EntityKoumoku_MojiType	' êßå‰ÉtÉâÉOÇOÇW
Private _controlFlg9 As New EntityKoumoku_MojiType	' êßå‰ÉtÉâÉOÇOÇX
Private _crsCd As New EntityKoumoku_MojiType	' ÉRÅ[ÉXÉRÅ[Éh
Private _day1 As New EntityKoumoku_NumberType	' ì˙ÇP
Private _day2 As New EntityKoumoku_NumberType	' ì˙ÇQ
Private _day3 As New EntityKoumoku_NumberType	' ì˙ÇR
Private _day4 As New EntityKoumoku_NumberType	' ì˙ÇS
Private _day5 As New EntityKoumoku_NumberType	' ì˙ÇT
Private _deleteDay As New EntityKoumoku_NumberType	' çÌèúì˙
Private _gousya As New EntityKoumoku_NumberType	' çÜé‘
Private _syuptDay As New EntityKoumoku_NumberType	' èoî≠ì˙
Private _text1 As New EntityKoumoku_MojiType	' ÉeÉLÉXÉgÇP
Private _text2 As New EntityKoumoku_MojiType	' ÉeÉLÉXÉgÇQ
Private _text3 As New EntityKoumoku_MojiType	' ÉeÉLÉXÉgÇR
Private _text4 As New EntityKoumoku_MojiType	' ÉeÉLÉXÉgÇS
Private _text5 As New EntityKoumoku_MojiType	' ÉeÉLÉXÉgÇT
Private _uketomeKbnOne1r As New EntityKoumoku_MojiType	' éÛé~ãÊï™ÇPñºÇPÇq
Private _uketomeKbnThree1r As New EntityKoumoku_MojiType	' éÛé~ãÊï™ÇRñºÇPÇq
Private _uketomeKbnTwo1r As New EntityKoumoku_MojiType	' éÛé~ãÊï™ÇQñºÇPÇq
Private _uketomeKbnFour1r As New EntityKoumoku_MojiType	' éÛé~ãÊï™ÇSñºÇPÇq
Private _uketomeKbnFive1r As New EntityKoumoku_MojiType	' éÛé~ãÊï™ÇTñºÇPÇq
Private _systemEntryPgmid As New EntityKoumoku_MojiType	' ÉVÉXÉeÉÄìoò^ÇoÇfÇlÇhÇc
Private _systemEntryPersonCd As New EntityKoumoku_MojiType	' ÉVÉXÉeÉÄìoò^é“ÉRÅ[Éh
Private _systemEntryDay As New EntityKoumoku_YmdType	' ÉVÉXÉeÉÄìoò^ì˙
Private _systemUpdatePgmid As New EntityKoumoku_MojiType	' ÉVÉXÉeÉÄçXêVÇoÇfÇlÇhÇc
Private _systemUpdatePersonCd As New EntityKoumoku_MojiType	' ÉVÉXÉeÉÄçXêVé“ÉRÅ[Éh
Private _systemUpdateDay As New EntityKoumoku_YmdType	' ÉVÉXÉeÉÄçXêVì˙


Sub New()
_controlFlg1.PhysicsName = "CONTROL_FLG_1"
_controlFlg10.PhysicsName = "CONTROL_FLG_10"
_controlFlg2.PhysicsName = "CONTROL_FLG_2"
_controlFlg3.PhysicsName = "CONTROL_FLG_3"
_controlFlg4.PhysicsName = "CONTROL_FLG_4"
_controlFlg5.PhysicsName = "CONTROL_FLG_5"
_controlFlg6.PhysicsName = "CONTROL_FLG_6"
_controlFlg7.PhysicsName = "CONTROL_FLG_7"
_controlFlg8.PhysicsName = "CONTROL_FLG_8"
_controlFlg9.PhysicsName = "CONTROL_FLG_9"
_crsCd.PhysicsName = "CRS_CD"
_day1.PhysicsName = "DAY_1"
_day2.PhysicsName = "DAY_2"
_day3.PhysicsName = "DAY_3"
_day4.PhysicsName = "DAY_4"
_day5.PhysicsName = "DAY_5"
_deleteDay.PhysicsName = "DELETE_DAY"
_gousya.PhysicsName = "GOUSYA"
_syuptDay.PhysicsName = "SYUPT_DAY"
_text1.PhysicsName = "TEXT_1"
_text2.PhysicsName = "TEXT_2"
_text3.PhysicsName = "TEXT_3"
_text4.PhysicsName = "TEXT_4"
_text5.PhysicsName = "TEXT_5"
_uketomeKbnOne1r.PhysicsName = "UKETOME_KBN_ONE_1R"
_uketomeKbnThree1r.PhysicsName = "UKETOME_KBN_THREE_1R"
_uketomeKbnTwo1r.PhysicsName = "UKETOME_KBN_TWO_1R"
_uketomeKbnFour1r.PhysicsName = "UKETOME_KBN_FOUR_1R"
_uketomeKbnFive1r.PhysicsName = "UKETOME_KBN_FIVE_1R"
_systemEntryPgmid.PhysicsName = "SYSTEM_ENTRY_PGMID"
_systemEntryPersonCd.PhysicsName = "SYSTEM_ENTRY_PERSON_CD"
_systemEntryDay.PhysicsName = "SYSTEM_ENTRY_DAY"
_systemUpdatePgmid.PhysicsName = "SYSTEM_UPDATE_PGMID"
_systemUpdatePersonCd.PhysicsName = "SYSTEM_UPDATE_PERSON_CD"
_systemUpdateDay.PhysicsName = "SYSTEM_UPDATE_DAY"


_controlFlg1.Required = FALSE
_controlFlg10.Required = FALSE
_controlFlg2.Required = FALSE
_controlFlg3.Required = FALSE
_controlFlg4.Required = FALSE
_controlFlg5.Required = FALSE
_controlFlg6.Required = FALSE
_controlFlg7.Required = FALSE
_controlFlg8.Required = FALSE
_controlFlg9.Required = FALSE
_crsCd.Required = FALSE
_day1.Required = FALSE
_day2.Required = FALSE
_day3.Required = FALSE
_day4.Required = FALSE
_day5.Required = FALSE
_deleteDay.Required = FALSE
_gousya.Required = FALSE
_syuptDay.Required = FALSE
_text1.Required = FALSE
_text2.Required = FALSE
_text3.Required = FALSE
_text4.Required = FALSE
_text5.Required = FALSE
_uketomeKbnOne1r.Required = FALSE
_uketomeKbnThree1r.Required = FALSE
_uketomeKbnTwo1r.Required = FALSE
_uketomeKbnFour1r.Required = FALSE
_uketomeKbnFive1r.Required = FALSE
_systemEntryPgmid.Required = TRUE
_systemEntryPersonCd.Required = TRUE
_systemEntryDay.Required = TRUE
_systemUpdatePgmid.Required = TRUE
_systemUpdatePersonCd.Required = TRUE
_systemUpdateDay.Required = TRUE


_controlFlg1.DBType = OracleDbType.Char
_controlFlg10.DBType = OracleDbType.Char
_controlFlg2.DBType = OracleDbType.Char
_controlFlg3.DBType = OracleDbType.Char
_controlFlg4.DBType = OracleDbType.Char
_controlFlg5.DBType = OracleDbType.Char
_controlFlg6.DBType = OracleDbType.Char
_controlFlg7.DBType = OracleDbType.Char
_controlFlg8.DBType = OracleDbType.Char
_controlFlg9.DBType = OracleDbType.Char
_crsCd.DBType = OracleDbType.Char
_day1.DBType = OracleDbType.Decimal
_day2.DBType = OracleDbType.Decimal
_day3.DBType = OracleDbType.Decimal
_day4.DBType = OracleDbType.Decimal
_day5.DBType = OracleDbType.Decimal
_deleteDay.DBType = OracleDbType.Decimal
_gousya.DBType = OracleDbType.Decimal
_syuptDay.DBType = OracleDbType.Decimal
_text1.DBType = OracleDbType.Varchar2
_text2.DBType = OracleDbType.Varchar2
_text3.DBType = OracleDbType.Varchar2
_text4.DBType = OracleDbType.Varchar2
_text5.DBType = OracleDbType.Varchar2
_uketomeKbnOne1r.DBType = OracleDbType.Char
_uketomeKbnThree1r.DBType = OracleDbType.Char
_uketomeKbnTwo1r.DBType = OracleDbType.Char
_uketomeKbnFour1r.DBType = OracleDbType.Char
_uketomeKbnFive1r.DBType = OracleDbType.Char
_systemEntryPgmid.DBType = OracleDbType.Char
_systemEntryPersonCd.DBType = OracleDbType.Varchar2
_systemEntryDay.DBType = OracleDbType.Date
_systemUpdatePgmid.DBType = OracleDbType.Char
_systemUpdatePersonCd.DBType = OracleDbType.Varchar2
_systemUpdateDay.DBType = OracleDbType.Date


_controlFlg1.IntegerBu = 1
_controlFlg10.IntegerBu = 1
_controlFlg2.IntegerBu = 1
_controlFlg3.IntegerBu = 1
_controlFlg4.IntegerBu = 1
_controlFlg5.IntegerBu = 1
_controlFlg6.IntegerBu = 1
_controlFlg7.IntegerBu = 1
_controlFlg8.IntegerBu = 1
_controlFlg9.IntegerBu = 1
_crsCd.IntegerBu = 6
_day1.IntegerBu = 1
_day2.IntegerBu = 1
_day3.IntegerBu = 1
_day4.IntegerBu = 1
_day5.IntegerBu = 1
_deleteDay.IntegerBu = 8
_gousya.IntegerBu = 3
_syuptDay.IntegerBu = 8
_text1.IntegerBu = 42
_text2.IntegerBu = 42
_text3.IntegerBu = 42
_text4.IntegerBu = 42
_text5.IntegerBu = 42
_uketomeKbnOne1r.IntegerBu = 1
_uketomeKbnThree1r.IntegerBu = 1
_uketomeKbnTwo1r.IntegerBu = 1
_uketomeKbnFour1r.IntegerBu = 1
_uketomeKbnFive1r.IntegerBu = 1
_systemEntryPgmid.IntegerBu = 8
_systemEntryPersonCd.IntegerBu = 20
_systemEntryDay.IntegerBu = 0
_systemUpdatePgmid.IntegerBu = 8
_systemUpdatePersonCd.IntegerBu = 20
_systemUpdateDay.IntegerBu = 0


_controlFlg1.DecimalBu = 0
_controlFlg10.DecimalBu = 0
_controlFlg2.DecimalBu = 0
_controlFlg3.DecimalBu = 0
_controlFlg4.DecimalBu = 0
_controlFlg5.DecimalBu = 0
_controlFlg6.DecimalBu = 0
_controlFlg7.DecimalBu = 0
_controlFlg8.DecimalBu = 0
_controlFlg9.DecimalBu = 0
_crsCd.DecimalBu = 0
_day1.DecimalBu = 0
_day2.DecimalBu = 0
_day3.DecimalBu = 0
_day4.DecimalBu = 0
_day5.DecimalBu = 0
_deleteDay.DecimalBu = 0
_gousya.DecimalBu = 0
_syuptDay.DecimalBu = 0
_text1.DecimalBu = 0
_text2.DecimalBu = 0
_text3.DecimalBu = 0
_text4.DecimalBu = 0
_text5.DecimalBu = 0
_uketomeKbnOne1r.DecimalBu = 0
_uketomeKbnThree1r.DecimalBu = 0
_uketomeKbnTwo1r.DecimalBu = 0
_uketomeKbnFour1r.DecimalBu = 0
_uketomeKbnFive1r.DecimalBu = 0
_systemEntryPgmid.DecimalBu = 0
_systemEntryPersonCd.DecimalBu = 0
_systemEntryDay.DecimalBu = 0
_systemUpdatePgmid.DecimalBu = 0
_systemUpdatePersonCd.DecimalBu = 0
_systemUpdateDay.DecimalBu = 0
End Sub


''' <summary>
''' controlFlg1
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property controlFlg1() As EntityKoumoku_MojiType
Get
    Return _controlFlg1
End Get
Set(ByVal value As EntityKoumoku_MojiType)
    _controlFlg1 = value
End Set
End Property


''' <summary>
''' controlFlg10
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property controlFlg10() As EntityKoumoku_MojiType
Get
    Return _controlFlg10
End Get
Set(ByVal value As EntityKoumoku_MojiType)
    _controlFlg10 = value
End Set
End Property


''' <summary>
''' controlFlg2
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property controlFlg2() As EntityKoumoku_MojiType
Get
    Return _controlFlg2
End Get
Set(ByVal value As EntityKoumoku_MojiType)
    _controlFlg2 = value
End Set
End Property


''' <summary>
''' controlFlg3
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property controlFlg3() As EntityKoumoku_MojiType
Get
    Return _controlFlg3
End Get
Set(ByVal value As EntityKoumoku_MojiType)
    _controlFlg3 = value
End Set
End Property


''' <summary>
''' controlFlg4
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property controlFlg4() As EntityKoumoku_MojiType
Get
    Return _controlFlg4
End Get
Set(ByVal value As EntityKoumoku_MojiType)
    _controlFlg4 = value
End Set
End Property


''' <summary>
''' controlFlg5
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property controlFlg5() As EntityKoumoku_MojiType
Get
    Return _controlFlg5
End Get
Set(ByVal value As EntityKoumoku_MojiType)
    _controlFlg5 = value
End Set
End Property


''' <summary>
''' controlFlg6
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property controlFlg6() As EntityKoumoku_MojiType
Get
    Return _controlFlg6
End Get
Set(ByVal value As EntityKoumoku_MojiType)
    _controlFlg6 = value
End Set
End Property


''' <summary>
''' controlFlg7
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property controlFlg7() As EntityKoumoku_MojiType
Get
    Return _controlFlg7
End Get
Set(ByVal value As EntityKoumoku_MojiType)
    _controlFlg7 = value
End Set
End Property


''' <summary>
''' controlFlg8
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property controlFlg8() As EntityKoumoku_MojiType
Get
    Return _controlFlg8
End Get
Set(ByVal value As EntityKoumoku_MojiType)
    _controlFlg8 = value
End Set
End Property


''' <summary>
''' controlFlg9
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property controlFlg9() As EntityKoumoku_MojiType
Get
    Return _controlFlg9
End Get
Set(ByVal value As EntityKoumoku_MojiType)
    _controlFlg9 = value
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
''' day1
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property day1() As EntityKoumoku_NumberType
Get
    Return _day1
End Get
Set(ByVal value As EntityKoumoku_NumberType)
    _day1 = value
End Set
End Property


''' <summary>
''' day2
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property day2() As EntityKoumoku_NumberType
Get
    Return _day2
End Get
Set(ByVal value As EntityKoumoku_NumberType)
    _day2 = value
End Set
End Property


''' <summary>
''' day3
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property day3() As EntityKoumoku_NumberType
Get
    Return _day3
End Get
Set(ByVal value As EntityKoumoku_NumberType)
    _day3 = value
End Set
End Property


''' <summary>
''' day4
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property day4() As EntityKoumoku_NumberType
Get
    Return _day4
End Get
Set(ByVal value As EntityKoumoku_NumberType)
    _day4 = value
End Set
End Property


''' <summary>
''' day5
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property day5() As EntityKoumoku_NumberType
Get
    Return _day5
End Get
Set(ByVal value As EntityKoumoku_NumberType)
    _day5 = value
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
''' text1
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property text1() As EntityKoumoku_MojiType
Get
    Return _text1
End Get
Set(ByVal value As EntityKoumoku_MojiType)
    _text1 = value
End Set
End Property


''' <summary>
''' text2
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property text2() As EntityKoumoku_MojiType
Get
    Return _text2
End Get
Set(ByVal value As EntityKoumoku_MojiType)
    _text2 = value
End Set
End Property


''' <summary>
''' text3
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property text3() As EntityKoumoku_MojiType
Get
    Return _text3
End Get
Set(ByVal value As EntityKoumoku_MojiType)
    _text3 = value
End Set
End Property


''' <summary>
''' text4
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property text4() As EntityKoumoku_MojiType
Get
    Return _text4
End Get
Set(ByVal value As EntityKoumoku_MojiType)
    _text4 = value
End Set
End Property


''' <summary>
''' text5
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property text5() As EntityKoumoku_MojiType
Get
    Return _text5
End Get
Set(ByVal value As EntityKoumoku_MojiType)
    _text5 = value
End Set
End Property


''' <summary>
''' uketomeKbnOne1r
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property uketomeKbnOne1r() As EntityKoumoku_MojiType
Get
    Return _uketomeKbnOne1r
End Get
Set(ByVal value As EntityKoumoku_MojiType)
    _uketomeKbnOne1r = value
End Set
End Property


''' <summary>
''' uketomeKbnThree1r
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property uketomeKbnThree1r() As EntityKoumoku_MojiType
Get
    Return _uketomeKbnThree1r
End Get
Set(ByVal value As EntityKoumoku_MojiType)
    _uketomeKbnThree1r = value
End Set
End Property


''' <summary>
''' uketomeKbnTwo1r
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property uketomeKbnTwo1r() As EntityKoumoku_MojiType
Get
    Return _uketomeKbnTwo1r
End Get
Set(ByVal value As EntityKoumoku_MojiType)
    _uketomeKbnTwo1r = value
End Set
End Property


''' <summary>
''' uketomeKbnFour1r
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property uketomeKbnFour1r() As EntityKoumoku_MojiType
Get
    Return _uketomeKbnFour1r
End Get
Set(ByVal value As EntityKoumoku_MojiType)
    _uketomeKbnFour1r = value
End Set
End Property


''' <summary>
''' uketomeKbnFive1r
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property uketomeKbnFive1r() As EntityKoumoku_MojiType
Get
    Return _uketomeKbnFive1r
End Get
Set(ByVal value As EntityKoumoku_MojiType)
    _uketomeKbnFive1r = value
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

