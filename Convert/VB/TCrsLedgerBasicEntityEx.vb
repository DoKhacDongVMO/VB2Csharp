Imports Oracle.ManagedDataAccess.Client


''' <summary>
''' コース台帳（基本）
''' </summary>
''' <remarks></remarks>
<Serializable()>
Public Class TCrsLedgerBasicEntityTehaiEx  ' コース台帳（基本）エンティティ
    Inherits TCrsLedgerBasicEntity

    ''' <summary>
    ''' 編集フラグ
    ''' </summary>
    ''' <remarks></remarks>
    Public NotInheritable Class EditFlgType
        Public Const [On] As String = "1"
        Public Const Off As String = "0"
    End Class

    Private _dispSyuptDay As New EntityKoumoku_MojiType  ' 出発日(表示)
    Private _jyoSyaTiName As New EntityKoumoku_MojiType  ' 乗車地
    Private _yobiName As New EntityKoumoku_MojiType      ' 曜日
    Private _syuPtTime As New EntityKoumoku_MojiType     ' 出発時間
    Private _unkyuName As New EntityKoumoku_MojiType     ' 運休区分
    Private _editFlg As New EntityKoumoku_MojiType       ' 編集フラグ
    Private _carrierLineNo As New EntityKoumoku_NumberType  'キャリアライン番号
    Private _koushaEditFlg As New EntityKoumoku_MojiType    '降車ヶ所編集フラグ
    Private _sonotaEditFlg As New EntityKoumoku_MojiType    'その他原価編集フラグ

#Region " 子エンティティ "
    Private _chargeKbnEntity As EntityOperation(Of TCrsLedgerBasicChargeKbnEntityTehaiEx)   '_コース台帳（基本_料金区分）エンティティ
    Private _costPlateEntity As EntityOperation(Of TCrsLedgerCostPlateEntity)               '_コース台帳原価（プレート）エンティティ
    Private _costBasicEntity As EntityOperation(Of TCrsLedgerCostBasicEntity)               '_コース台帳原価（基本）エンティティ
    Private _koshakashoEntity As EntityOperation(Of TCrsLedgerKoshakashoEntityTehaiEx)      '_コース台帳（降車ヶ所）エンティティ

#End Region

    Sub New()
        _dispSyuptDay.PhysicsName = "DISP_SYUPT_DAY"
        _jyoSyaTiName.PhysicsName = "JYOSYA_TI_NAME"
        _yobiName.PhysicsName = "YOBI_NAME"
        _syuPtTime.PhysicsName = "SYUPT_TIME"
        _unkyuName.PhysicsName = "UNKYU_NAME"
        _editFlg.PhysicsName = "EDIT_FLG"
        _carrierLineNo.PhysicsName = "CARRIER_LINENO"
        _koushaEditFlg.PhysicsName = "KOUSHA_EDIT_FLG"
        _sonotaEditFlg.PhysicsName = "SONOTA_EDIT_FLG"

        _dispSyuptDay.Required = False
        _jyoSyaTiName.Required = False
        _yobiName.Required = False
        _syuPtTime.Required = False
        _unkyuName.Required = False
        _editFlg.Required = False
        _carrierLineNo.Required = False
        _koushaEditFlg.Required = False
        _sonotaEditFlg.Required = False

        _dispSyuptDay.DBType = OracleDbType.Varchar2
        _jyoSyaTiName.DBType = OracleDbType.Varchar2
        _yobiName.DBType = OracleDbType.Varchar2
        _syuPtTime.DBType = OracleDbType.Varchar2
        _unkyuName.DBType = OracleDbType.Varchar2
        _editFlg.DBType = OracleDbType.Varchar2
        _carrierLineNo.DBType = OracleDbType.Decimal
        _koushaEditFlg.DBType = OracleDbType.Varchar2
        _sonotaEditFlg.DBType = OracleDbType.Varchar2

        _dispSyuptDay.IntegerBu = 99
        _jyoSyaTiName.IntegerBu = 22
        _yobiName.IntegerBu = 64
        _syuPtTime.IntegerBu = 99
        _unkyuName.IntegerBu = 64
        _editFlg.IntegerBu = 1
        _carrierLineNo.IntegerBu = 3
        _koushaEditFlg.IntegerBu = 1
        _sonotaEditFlg.IntegerBu = 1

        _dispSyuptDay.DecimalBu = 0
        _jyoSyaTiName.DecimalBu = 0
        _yobiName.DecimalBu = 0
        _syuPtTime.DecimalBu = 0
        _unkyuName.DecimalBu = 0
        _editFlg.DecimalBu = 0
        _carrierLineNo.DecimalBu = 0
        _koushaEditFlg.DecimalBu = 0
        _sonotaEditFlg.DecimalBu = 0

        _chargeKbnEntity = New EntityOperation(Of TCrsLedgerBasicChargeKbnEntityTehaiEx)
        _costPlateEntity = New EntityOperation(Of TCrsLedgerCostPlateEntity)
        _costBasicEntity = New EntityOperation(Of TCrsLedgerCostBasicEntity)
        _koshakashoEntity = New EntityOperation(Of TCrsLedgerKoshakashoEntityTehaiEx)

    End Sub

    ''' <summary>
    ''' 出発日(表示)
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property DispSyuptDay() As EntityKoumoku_MojiType  '出発日(表示)
        Get
            Return _dispSyuptDay
        End Get
        Set(ByVal value As EntityKoumoku_MojiType)
            _dispSyuptDay = value
        End Set
    End Property

    ''' <summary>
    ''' 乗車地
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property JyoSyaTiName() As EntityKoumoku_MojiType  '乗車地
        Get
            Return _jyoSyaTiName
        End Get
        Set(ByVal value As EntityKoumoku_MojiType)
            _jyoSyaTiName = value
        End Set
    End Property

    ''' <summary>
    ''' 曜日
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property YobiName() As EntityKoumoku_MojiType  '曜日
        Get
            Return _yobiName
        End Get
        Set(ByVal value As EntityKoumoku_MojiType)
            _yobiName = value
        End Set
    End Property

    ''' <summary>
    ''' 出発時間
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property SyuPtTime() As EntityKoumoku_MojiType  '出発時間
        Get
            Return _syuPtTime
        End Get
        Set(ByVal value As EntityKoumoku_MojiType)
            _syuPtTime = value
        End Set
    End Property

    ''' <summary>
    ''' 運休区分
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property UnkyuName() As EntityKoumoku_MojiType  '運休区分
        Get
            Return _unkyuName
        End Get
        Set(ByVal value As EntityKoumoku_MojiType)
            _unkyuName = value
        End Set
    End Property

    ''' <summary>
    ''' 編集フラグ
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property EditFlg() As EntityKoumoku_MojiType  '編集フラグ
        Get
            Return _editFlg
        End Get
        Set(ByVal value As EntityKoumoku_MojiType)
            _editFlg = value
        End Set
    End Property

    ''' <summary>
    ''' キャリアライン番号
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property CarrierLineNo() As EntityKoumoku_NumberType
        Get
            Return _carrierLineNo
        End Get
        Set(ByVal value As EntityKoumoku_NumberType)
            _carrierLineNo = value
        End Set
    End Property

    ''' <summary>
    ''' 降車ヶ所編集フラグ
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property KoushaEditFlg() As EntityKoumoku_MojiType
        Get
            Return _koushaEditFlg
        End Get
        Set(ByVal value As EntityKoumoku_MojiType)
            _koushaEditFlg = value
        End Set
    End Property

    ''' <summary>
    ''' その他原価編集フラグ
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property SonotaEditFlg() As EntityKoumoku_MojiType
        Get
            Return _sonotaEditFlg
        End Get
        Set(ByVal value As EntityKoumoku_MojiType)
            _sonotaEditFlg = value
        End Set
    End Property

#Region " コース台帳（基本_料金区分）エンティティ "
    ''' <summary>
    ''' コース台帳（基本_料金区分）エンティティ（操作クラス）
    ''' ※行No(降車ヶ所) = 0
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function ChargeKbnEntity() As EntityOperation(Of TCrsLedgerBasicChargeKbnEntityTehaiEx)  'コース台帳（基本_料金区分）エンティティ
        Return _chargeKbnEntity
    End Function
    ''' <summary>
    ''' コース台帳（基本_料金区分）エンティティ（エンティティクラス）
    ''' ※行No(降車ヶ所) = 0
    ''' </summary>
    ''' <param name="index">コース台帳（基本_料金区分）エンティティ配列要素を指定</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function ChargeKbnEntity(ByVal index As Integer) As TCrsLedgerBasicChargeKbnEntityTehaiEx  'コース台帳（基本_料金区分）エンティティ
        Return _chargeKbnEntity.EntityData(index)
    End Function
#End Region

#Region " コース台帳原価（プレート）エンティティ "
    ''' <summary>
    ''' コース台帳原価（プレート）エンティティ（操作クラス）
    ''' ※行No(降車ヶ所) = 0
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function CostPlateEntity() As EntityOperation(Of TCrsLedgerCostPlateEntity)  'コース台帳原価（プレート）エンティティ
        Return _costPlateEntity
    End Function
    ''' <summary>
    ''' コース台帳原価（プレート）エンティティ（エンティティクラス）
    ''' ※行No(降車ヶ所) = 0
    ''' </summary>
    ''' <param name="index">コース台帳原価（プレート）エンティティ配列要素を指定</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function CostPlateEntity(ByVal index As Integer) As TCrsLedgerCostPlateEntity  'コース台帳原価（プレート）エンティティ
        Return _costPlateEntity.EntityData(index)
    End Function
#End Region

#Region " コース台帳原価（基本）エンティティ "
    ''' <summary>
    ''' コース台帳原価（基本）エンティティ（操作クラス）
    ''' ※行No(降車ヶ所) = 0
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function CostBasicEntity() As EntityOperation(Of TCrsLedgerCostBasicEntity)  'コース台帳原価（基本）エンティティ
        Return _costBasicEntity
    End Function
    ''' <summary>
    ''' コース台帳原価（基本）エンティティ（エンティティクラス）
    ''' ※行No(降車ヶ所) = 0
    ''' </summary>
    ''' <param name="index">コース台帳原価（基本）エンティティ配列要素を指定</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function CostBasicEntity(ByVal index As Integer) As TCrsLedgerCostBasicEntity  'コース台帳原価（基本）エンティティ
        Return _costBasicEntity.EntityData(index)
    End Function

#End Region

#Region " コース台帳（降車ヶ所）エンティティ "
    ''' <summary>
    ''' コース台帳（降車ヶ所）エンティティ（操作クラス）
    ''' ※行No(降車ヶ所) = 0
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function KoushakashoEntity() As EntityOperation(Of TCrsLedgerKoshakashoEntityTehaiEx)  'コース台帳（降車ヶ所）エンティティ
        Return _koshakashoEntity
    End Function

    ''' <summary>
    ''' コース台帳（降車ヶ所）エンティティ（エンティティクラス）
    ''' ※行No(降車ヶ所) = 0
    ''' </summary>
    ''' <param name="index">コース台帳（降車ヶ所）エンティティ配列要素を指定</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function KoushakashoEntity(ByVal index As Integer) As TCrsLedgerKoshakashoEntityTehaiEx  'コース台帳（降車ヶ所）エンティティ
        Return _koshakashoEntity.EntityData(index)
    End Function
#End Region

End Class

