Imports Oracle.ManagedDataAccess.Client


''' <summary>
''' �R�[�X�䒠�i����_�����敪�j
''' </summary>
''' <remarks></remarks>
<Serializable()> _
Public Class TCrsLedgerChargeChargeKbnEntity  ' �R�[�X�䒠�i����_�����敪�j�G���e�B�e�B


Private _syuptDay As New EntityKoumoku_NumberType	' �o����
Private _crsCd As New EntityKoumoku_MojiType	' �R�[�X�R�[�h
Private _gousya As New EntityKoumoku_NumberType	' ����
Private _kbnNo As New EntityKoumoku_NumberType	' �敪No
Private _chargeKbnJininCd As New EntityKoumoku_MojiType	' �����敪�i�l���j�R�[�h
Private _charge As New EntityKoumoku_NumberType	' ����
Private _chargeSubSeat As New EntityKoumoku_NumberType	' ����_�⏕��
Private _carriage As New EntityKoumoku_NumberType	' �^��
Private _carriageSubSeat As New EntityKoumoku_NumberType	' �^��_�⏕��
Private _name1 As New EntityKoumoku_MojiType	' �������̂P
Private _name2 As New EntityKoumoku_MojiType	' �������̂Q
Private _name3 As New EntityKoumoku_MojiType	' �������̂R
Private _name4 As New EntityKoumoku_MojiType	' �������̂S
Private _name5 As New EntityKoumoku_MojiType	' �������̂T
Private _charge1 As New EntityKoumoku_NumberType	' �����P
Private _charge2 As New EntityKoumoku_NumberType	' �����Q
Private _charge3 As New EntityKoumoku_NumberType	' �����R
Private _charge4 As New EntityKoumoku_NumberType	' �����S
Private _charge5 As New EntityKoumoku_NumberType	' �����T
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
_kbnNo.PhysicsName = "KBN_NO"
_chargeKbnJininCd.PhysicsName = "CHARGE_KBN_JININ_CD"
_charge.PhysicsName = "CHARGE"
_chargeSubSeat.PhysicsName = "CHARGE_SUB_SEAT"
_carriage.PhysicsName = "CARRIAGE"
_carriageSubSeat.PhysicsName = "CARRIAGE_SUB_SEAT"
_name1.PhysicsName = "NAME_1"
_name2.PhysicsName = "NAME_2"
_name3.PhysicsName = "NAME_3"
_name4.PhysicsName = "NAME_4"
_name5.PhysicsName = "NAME_5"
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
_kbnNo.Required = FALSE
_chargeKbnJininCd.Required = FALSE
_charge.Required = FALSE
_chargeSubSeat.Required = FALSE
_carriage.Required = FALSE
_carriageSubSeat.Required = FALSE
_name1.Required = FALSE
_name2.Required = FALSE
_name3.Required = FALSE
_name4.Required = FALSE
_name5.Required = FALSE
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
_kbnNo.DBType = OracleDbType.Decimal
_chargeKbnJininCd.DBType = OracleDbType.Char
_charge.DBType = OracleDbType.Decimal
_chargeSubSeat.DBType = OracleDbType.Decimal
_carriage.DBType = OracleDbType.Decimal
_carriageSubSeat.DBType = OracleDbType.Decimal
_name1.DBType = OracleDbType.Varchar2
_name2.DBType = OracleDbType.Varchar2
_name3.DBType = OracleDbType.Varchar2
_name4.DBType = OracleDbType.Varchar2
_name5.DBType = OracleDbType.Varchar2
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
_kbnNo.IntegerBu = 1
_chargeKbnJininCd.IntegerBu = 2
_charge.IntegerBu = 7
_chargeSubSeat.IntegerBu = 7
_carriage.IntegerBu = 7
_carriageSubSeat.IntegerBu = 7
_name1.IntegerBu = 32
_name2.IntegerBu = 32
_name3.IntegerBu = 32
_name4.IntegerBu = 32
_name5.IntegerBu = 32
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
_kbnNo.DecimalBu = 0
_chargeKbnJininCd.DecimalBu = 0
_charge.DecimalBu = 0
_chargeSubSeat.DecimalBu = 0
_carriage.DecimalBu = 0
_carriageSubSeat.DecimalBu = 0
_name1.DecimalBu = 0
_name2.DecimalBu = 0
_name3.DecimalBu = 0
_name4.DecimalBu = 0
_name5.DecimalBu = 0
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
''' kbnNo
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property kbnNo() As EntityKoumoku_NumberType
Get
    Return _kbnNo
End Get
Set(ByVal value As EntityKoumoku_NumberType)
    _kbnNo = value
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
''' charge
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property charge() As EntityKoumoku_NumberType
Get
    Return _charge
End Get
Set(ByVal value As EntityKoumoku_NumberType)
    _charge = value
End Set
End Property


''' <summary>
''' chargeSubSeat
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property chargeSubSeat() As EntityKoumoku_NumberType
Get
    Return _chargeSubSeat
End Get
Set(ByVal value As EntityKoumoku_NumberType)
    _chargeSubSeat = value
End Set
End Property


''' <summary>
''' carriage
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property carriage() As EntityKoumoku_NumberType
Get
    Return _carriage
End Get
Set(ByVal value As EntityKoumoku_NumberType)
    _carriage = value
End Set
End Property


''' <summary>
''' carriageSubSeat
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property carriageSubSeat() As EntityKoumoku_NumberType
Get
    Return _carriageSubSeat
End Get
Set(ByVal value As EntityKoumoku_NumberType)
    _carriageSubSeat = value
End Set
End Property


''' <summary>
''' name1
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property name1() As EntityKoumoku_MojiType
Get
    Return _name1
End Get
Set(ByVal value As EntityKoumoku_MojiType)
    _name1 = value
End Set
End Property


''' <summary>
''' name2
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property name2() As EntityKoumoku_MojiType
Get
    Return _name2
End Get
Set(ByVal value As EntityKoumoku_MojiType)
    _name2 = value
End Set
End Property


''' <summary>
''' name3
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property name3() As EntityKoumoku_MojiType
Get
    Return _name3
End Get
Set(ByVal value As EntityKoumoku_MojiType)
    _name3 = value
End Set
End Property


''' <summary>
''' name4
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property name4() As EntityKoumoku_MojiType
Get
    Return _name4
End Get
Set(ByVal value As EntityKoumoku_MojiType)
    _name4 = value
End Set
End Property


''' <summary>
''' name5
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property name5() As EntityKoumoku_MojiType
Get
    Return _name5
End Get
Set(ByVal value As EntityKoumoku_MojiType)
    _name5 = value
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

