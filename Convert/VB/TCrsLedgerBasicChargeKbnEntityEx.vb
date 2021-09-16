Imports Oracle.ManagedDataAccess.Client


''' <summary>
''' コース台帳（基本_料金区分）
''' </summary>
''' <remarks></remarks>
<Serializable()>
Public Class TCrsLedgerBasicChargeKbnEntityTehaiEx  ' コース台帳（基本_料金区分）エンティティ
    Inherits TCrsLedgerBasicChargeKbnEntity

    Private _chargeKbnJininName As New EntityKoumoku_MojiType   '料金区分（人員）名
    Private _shuyakuChargeKbnCd As New EntityKoumoku_MojiType     '集約料金区分


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
    ''' 料金区分（人員）名
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
    ''' 集約料金区分
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

