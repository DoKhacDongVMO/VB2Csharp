Imports Oracle.ManagedDataAccess.Client


''' <summary>
''' コース台帳原価（降車ヶ所）
''' </summary>
''' <remarks></remarks>
<Serializable()>
Public Class TCrsLedgerCostKoshakashoEntityTehaiEx  ' コース台帳原価（降車ヶ所）エンティティ
    Inherits TCrsLedgerCostKoshakashoEntity

    ''' <summary>
    ''' 作成フラグ
    ''' </summary>
    ''' <remarks></remarks>
    Public NotInheritable Class MakeFlgType
        ''' <summary>
        ''' 仮作成状態(INSERT対象外)
        ''' </summary>
        Public Const [On] As String = "1"
        ''' <summary>
        ''' 編集済状態(INSERT対象)
        ''' </summary>
        Public Const Off As String = "0"
    End Class

    ''' <summary>
    ''' 送客手数料現金払い区分
    ''' </summary>
    ''' <remarks></remarks>
    Public NotInheritable Class GenkinSeisanKbnType
        Public Const unChecked As String = Nothing
        Public Const checked As String = "Y"
    End Class

    ''' <summary>
    ''' バス単位
    ''' </summary>
    ''' <remarks></remarks>
    Public NotInheritable Class busTaniType
        Public Const unChecked As String = "0"
        Public Const checked As String = "1"
    End Class

#Region " 子エンティティ "

    Private _chargeKbnEntity As EntityOperation(Of TCrsLedgerCostKoshakashoChargeKbnEntity) '_コース台帳原価（降車ヶ所_料金区分）エンティティ
#End Region
    Private _makeFlg As New EntityKoumoku_MojiType    ' コースコード
    Sub New()
        _makeFlg.PhysicsName = "MAKE_FLG"

        _makeFlg.Required = False

        _makeFlg.DBType = OracleDbType.Char

        _makeFlg.IntegerBu = 1

        _makeFlg.DecimalBu = 0

        _chargeKbnEntity = New EntityOperation(Of TCrsLedgerCostKoshakashoChargeKbnEntity)

    End Sub

#Region " コース台帳原価（降車ヶ所_料金区分）エンティティ "
    ''' <summary>
    ''' コース台帳原価（降車ヶ所_料金区分）エンティティ（操作クラス）
    ''' ※行No(降車ヶ所) = 0
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function CostKoshakashoChargeKbnEntity() As EntityOperation(Of TCrsLedgerCostKoshakashoChargeKbnEntity)  '原価マスタ_キャリアエンティティ
        Return _chargeKbnEntity
    End Function
    ''' <summary>
    ''' コース台帳原価（降車ヶ所_料金区分）エンティティ（エンティティクラス）
    ''' ※行No(降車ヶ所) = 0
    ''' </summary>
    ''' <param name="index">コース台帳原価（降車ヶ所_料金区分）エンティティ配列要素を指定</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function CostKoshakashoChargeKbnEntity(ByVal index As Integer) As TCrsLedgerCostKoshakashoChargeKbnEntity  '原価マスタ_キャリアエンティティ
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

