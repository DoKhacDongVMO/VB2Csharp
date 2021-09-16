Imports Oracle.ManagedDataAccess.Client


''' <summary>
''' �R�[�X�䒠�i��{_�����敪�j
''' </summary>
''' <remarks></remarks>
<Serializable()>
Public Class TCrsLedgerBasicChargeKbnEntityTehaiEx  ' �R�[�X�䒠�i��{_�����敪�j�G���e�B�e�B
    Inherits TCrsLedgerBasicChargeKbnEntity

    Private _chargeKbnJininName As New EntityKoumoku_MojiType   '�����敪�i�l���j��
    Private _shuyakuChargeKbnCd As New EntityKoumoku_MojiType     '�W�񗿋��敪


    Sub New()
        _chargeKbnJininName.PhysicsName = "CHARGE_KBN_JININ_NAME"
        _shuyakuChargeKbnCd.PhysicsName = "SHUYAKU_CHARGE_KBN_CD"

        _chargeKbnJininName.Required = False
        _shuyakuChargeKbnCd.Required = False

        _chargeKbnJininName.DBType = OracleDbType.Varchar2
        _shuyakuChargeKbnCd.DBType = OracleDbType.Char

        _chargeKbnJininName.IntegerBu = 12
        _shuyakuChargeKbnCd.IntegerBu = 2

        _chargeKbnJininName.DecimalBu = 0
        _shuyakuChargeKbnCd.DecimalBu = 0

    End Sub


    ''' <summary>
    ''' �����敪�i�l���j��
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property ChargeKbnJininName() As EntityKoumoku_MojiType
        Get
            Return _chargeKbnJininName
        End Get
        Set(ByVal value As EntityKoumoku_MojiType)
            _chargeKbnJininName = value
        End Set
    End Property

    ''' <summary>
    ''' �W�񗿋��敪
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property ShuyakuChargeKbnCd() As EntityKoumoku_MojiType
        Get
            Return _ShuyakuChargeKbnCd
        End Get
        Set(ByVal value As EntityKoumoku_MojiType)
            _ShuyakuChargeKbnCd = value
        End Set
    End Property



End Class

