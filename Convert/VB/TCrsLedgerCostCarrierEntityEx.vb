Imports Oracle.ManagedDataAccess.Client


''' <summary>
''' コース台帳原価（キャリア）
''' </summary>
''' <remarks></remarks>
<Serializable()>
Public Class TCrsLedgerCostCarrierEntityTehaiEx  ' コース台帳原価（キャリア）エンティティ
    Inherits TCrsLedgerCostCarrierEntity

    ''' <summary>
    ''' 作成フラグ
    ''' </summary>
    ''' <remarks></remarks>
    Public NotInheritable Class MakeFlgType
        Public Const [On] As String = "1"
        Public Const Off As String = "0"
    End Class

#Region " 子エンティティ "

    Private _chargeKbnEntity As EntityOperation(Of TCrsLedgerCostCarrierChargeKbnEntity) '_コース台帳原価（キャリア_料金区分）エンティティ
#End Region
    Private _makeFlg As New EntityKoumoku_MojiType    ' コースコード

    Sub New()
        _makeFlg.PhysicsName = "MAKE_FLG"

        _makeFlg.Required = False

        _makeFlg.DBType = OracleDbType.Char

        _makeFlg.IntegerBu = 1

        _makeFlg.DecimalBu = 0

        _chargeKbnEntity = New EntityOperation(Of TCrsLedgerCostCarrierChargeKbnEntity)
    End Sub

#Region " コース台帳原価（キャリア_料金区分）エンティティ "
    ''' <summary>
    ''' コース台帳原価（キャリア_料金区分）エンティティ（操作クラス）
    ''' ※行No(降車ヶ所) = 0
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function CostCarrierChargeKbnEntity() As EntityOperation(Of TCrsLedgerCostCarrierChargeKbnEntity)  '原価マスタ_キャリアエンティティ
        Return _chargeKbnEntity
    End Function
    ''' <summary>
    ''' コース台帳原価（キャリア_料金区分）エンティティ（エンティティクラス）
    ''' ※行No(降車ヶ所) = 0
    ''' </summary>
    ''' <param name="index">コース台帳原価（キャリア_料金区分）エンティティ配列要素を指定</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function CostCarrierChargeKbnEntity(ByVal index As Integer) As TCrsLedgerCostCarrierChargeKbnEntity  '原価マスタ_キャリアエンティティ
        Return _chargeKbnEntity.EntityData(index)
    End Function
#End Region

    ''' <summary>
    ''' makeFlg
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property makeFlg() As EntityKoumoku_MojiType
        Get
            Return _makeFlg
        End Get
        Set(ByVal value As EntityKoumoku_MojiType)
            _makeFlg = value
        End Set
    End Property
End Class

