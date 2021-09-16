Imports Oracle.ManagedDataAccess.Client


''' <summary>
''' コース台帳（料金）
''' </summary>
''' <remarks></remarks>
<Serializable()>
Public Class TCrsLedgerChargeEntityEx  ' コース台帳（料金）エンティティ
    Inherits TCrsLedgerChargeEntity

#Region " 子エンティティ "
    Private _chargeChargeKbnEntity As EntityOperation(Of TCrsLedgerChargeChargeKbnEntity)   '_コース台帳（料金_料金区分）エンティティ
#End Region

    ''' <summary>
    ''' 編集フラグ
    ''' </summary>
    ''' <remarks></remarks>
    Public NotInheritable Class MakeFlgType
        Public Const [On] As String = "1"
        Public Const Off As String = "0"
    End Class

    Private _makeFlg As New EntityKoumoku_MojiType  ' 

    Sub New()
        _makeFlg.PhysicsName = "DISP_SYUPT_DAY"
        _makeFlg.Required = False
        _makeFlg.DBType = OracleDbType.Varchar2
        _makeFlg.IntegerBu = 99
        _makeFlg.DecimalBu = 0
        _makeFlg.Value = MakeFlgType.Off

        _chargeChargeKbnEntity = New EntityOperation(Of TCrsLedgerChargeChargeKbnEntity)
    End Sub


    ''' <summary>
    ''' 出発日(表示)
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property MakeFlg() As EntityKoumoku_MojiType  '作成フラグ
        Get
            Return _makeFlg
        End Get
        Set(ByVal value As EntityKoumoku_MojiType)
            _makeFlg = value
        End Set
    End Property

#Region " コース台帳（料金_料金区分）エンティティ "
    ''' <summary>
    ''' コース台帳（料金_料金区分）エンティティ（操作クラス）
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function ChargeChargeKbnEntity() As EntityOperation(Of TCrsLedgerChargeChargeKbnEntity)  'コース台帳（基本_料金区分）エンティティ
        Return _chargeChargeKbnEntity
    End Function
    ''' <summary>
    ''' コース台帳（料金_料金区分）エンティティ（エンティティクラス）
    ''' </summary>
    ''' <param name="index">コース台帳（料金_料金区分）エンティティ配列要素を指定</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function ChargeChargeKbnEntity(ByVal index As Integer) As TCrsLedgerChargeChargeKbnEntity  'コース台帳（基本_料金区分）エンティティ
        Return _chargeChargeKbnEntity.EntityData(index)
    End Function
#End Region


End Class

