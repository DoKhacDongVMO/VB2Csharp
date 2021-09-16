Imports Oracle.ManagedDataAccess.Client


''' <summary>
''' �R�[�X�䒠�i��{�j
''' </summary>
''' <remarks></remarks>
<Serializable()>
Public Class TCrsLedgerBasicEntityTehaiEx  ' �R�[�X�䒠�i��{�j�G���e�B�e�B
    Inherits TCrsLedgerBasicEntity

    ''' <summary>
    ''' �ҏW�t���O
    ''' </summary>
    ''' <remarks></remarks>
    Public NotInheritable Class EditFlgType
        Public Const [On] As String = "1"
        Public Const Off As String = "0"
    End Class

    Private _dispSyuptDay As New EntityKoumoku_MojiType  ' �o����(�\��)
    Private _jyoSyaTiName As New EntityKoumoku_MojiType  ' ��Ԓn
    Private _yobiName As New EntityKoumoku_MojiType      ' �j��
    Private _syuPtTime As New EntityKoumoku_MojiType     ' �o������
    Private _unkyuName As New EntityKoumoku_MojiType     ' �^�x�敪
    Private _editFlg As New EntityKoumoku_MojiType       ' �ҏW�t���O
    Private _carrierLineNo As New EntityKoumoku_NumberType  '�L�����A���C���ԍ�
    Private _koushaEditFlg As New EntityKoumoku_MojiType    '�~�ԃ����ҏW�t���O
    Private _sonotaEditFlg As New EntityKoumoku_MojiType    '���̑������ҏW�t���O

#Region " �q�G���e�B�e�B "
    Private _chargeKbnEntity As EntityOperation(Of TCrsLedgerBasicChargeKbnEntityTehaiEx)   '_�R�[�X�䒠�i��{_�����敪�j�G���e�B�e�B
    Private _costPlateEntity As EntityOperation(Of TCrsLedgerCostPlateEntity)               '_�R�[�X�䒠�����i�v���[�g�j�G���e�B�e�B
    Private _costBasicEntity As EntityOperation(Of TCrsLedgerCostBasicEntity)               '_�R�[�X�䒠�����i��{�j�G���e�B�e�B
    Private _koshakashoEntity As EntityOperation(Of TCrsLedgerKoshakashoEntityTehaiEx)      '_�R�[�X�䒠�i�~�ԃ����j�G���e�B�e�B

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
    ''' �o����(�\��)
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property DispSyuptDay() As EntityKoumoku_MojiType  '�o����(�\��)
        Get
            Return _dispSyuptDay
        End Get
        Set(ByVal value As EntityKoumoku_MojiType)
            _dispSyuptDay = value
        End Set
    End Property

    ''' <summary>
    ''' ��Ԓn
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property JyoSyaTiName() As EntityKoumoku_MojiType  '��Ԓn
        Get
            Return _jyoSyaTiName
        End Get
        Set(ByVal value As EntityKoumoku_MojiType)
            _jyoSyaTiName = value
        End Set
    End Property

    ''' <summary>
    ''' �j��
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property YobiName() As EntityKoumoku_MojiType  '�j��
        Get
            Return _yobiName
        End Get
        Set(ByVal value As EntityKoumoku_MojiType)
            _yobiName = value
        End Set
    End Property

    ''' <summary>
    ''' �o������
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property SyuPtTime() As EntityKoumoku_MojiType  '�o������
        Get
            Return _syuPtTime
        End Get
        Set(ByVal value As EntityKoumoku_MojiType)
            _syuPtTime = value
        End Set
    End Property

    ''' <summary>
    ''' �^�x�敪
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property UnkyuName() As EntityKoumoku_MojiType  '�^�x�敪
        Get
            Return _unkyuName
        End Get
        Set(ByVal value As EntityKoumoku_MojiType)
            _unkyuName = value
        End Set
    End Property

    ''' <summary>
    ''' �ҏW�t���O
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property EditFlg() As EntityKoumoku_MojiType  '�ҏW�t���O
        Get
            Return _editFlg
        End Get
        Set(ByVal value As EntityKoumoku_MojiType)
            _editFlg = value
        End Set
    End Property

    ''' <summary>
    ''' �L�����A���C���ԍ�
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
    ''' �~�ԃ����ҏW�t���O
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
    ''' ���̑������ҏW�t���O
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

#Region " �R�[�X�䒠�i��{_�����敪�j�G���e�B�e�B "
    ''' <summary>
    ''' �R�[�X�䒠�i��{_�����敪�j�G���e�B�e�B�i����N���X�j
    ''' ���sNo(�~�ԃ���) = 0
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function ChargeKbnEntity() As EntityOperation(Of TCrsLedgerBasicChargeKbnEntityTehaiEx)  '�R�[�X�䒠�i��{_�����敪�j�G���e�B�e�B
        Return _chargeKbnEntity
    End Function
    ''' <summary>
    ''' �R�[�X�䒠�i��{_�����敪�j�G���e�B�e�B�i�G���e�B�e�B�N���X�j
    ''' ���sNo(�~�ԃ���) = 0
    ''' </summary>
    ''' <param name="index">�R�[�X�䒠�i��{_�����敪�j�G���e�B�e�B�z��v�f���w��</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function ChargeKbnEntity(ByVal index As Integer) As TCrsLedgerBasicChargeKbnEntityTehaiEx  '�R�[�X�䒠�i��{_�����敪�j�G���e�B�e�B
        Return _chargeKbnEntity.EntityData(index)
    End Function
#End Region

#Region " �R�[�X�䒠�����i�v���[�g�j�G���e�B�e�B "
    ''' <summary>
    ''' �R�[�X�䒠�����i�v���[�g�j�G���e�B�e�B�i����N���X�j
    ''' ���sNo(�~�ԃ���) = 0
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function CostPlateEntity() As EntityOperation(Of TCrsLedgerCostPlateEntity)  '�R�[�X�䒠�����i�v���[�g�j�G���e�B�e�B
        Return _costPlateEntity
    End Function
    ''' <summary>
    ''' �R�[�X�䒠�����i�v���[�g�j�G���e�B�e�B�i�G���e�B�e�B�N���X�j
    ''' ���sNo(�~�ԃ���) = 0
    ''' </summary>
    ''' <param name="index">�R�[�X�䒠�����i�v���[�g�j�G���e�B�e�B�z��v�f���w��</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function CostPlateEntity(ByVal index As Integer) As TCrsLedgerCostPlateEntity  '�R�[�X�䒠�����i�v���[�g�j�G���e�B�e�B
        Return _costPlateEntity.EntityData(index)
    End Function
#End Region

#Region " �R�[�X�䒠�����i��{�j�G���e�B�e�B "
    ''' <summary>
    ''' �R�[�X�䒠�����i��{�j�G���e�B�e�B�i����N���X�j
    ''' ���sNo(�~�ԃ���) = 0
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function CostBasicEntity() As EntityOperation(Of TCrsLedgerCostBasicEntity)  '�R�[�X�䒠�����i��{�j�G���e�B�e�B
        Return _costBasicEntity
    End Function
    ''' <summary>
    ''' �R�[�X�䒠�����i��{�j�G���e�B�e�B�i�G���e�B�e�B�N���X�j
    ''' ���sNo(�~�ԃ���) = 0
    ''' </summary>
    ''' <param name="index">�R�[�X�䒠�����i��{�j�G���e�B�e�B�z��v�f���w��</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function CostBasicEntity(ByVal index As Integer) As TCrsLedgerCostBasicEntity  '�R�[�X�䒠�����i��{�j�G���e�B�e�B
        Return _costBasicEntity.EntityData(index)
    End Function

#End Region

#Region " �R�[�X�䒠�i�~�ԃ����j�G���e�B�e�B "
    ''' <summary>
    ''' �R�[�X�䒠�i�~�ԃ����j�G���e�B�e�B�i����N���X�j
    ''' ���sNo(�~�ԃ���) = 0
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function KoushakashoEntity() As EntityOperation(Of TCrsLedgerKoshakashoEntityTehaiEx)  '�R�[�X�䒠�i�~�ԃ����j�G���e�B�e�B
        Return _koshakashoEntity
    End Function

    ''' <summary>
    ''' �R�[�X�䒠�i�~�ԃ����j�G���e�B�e�B�i�G���e�B�e�B�N���X�j
    ''' ���sNo(�~�ԃ���) = 0
    ''' </summary>
    ''' <param name="index">�R�[�X�䒠�i�~�ԃ����j�G���e�B�e�B�z��v�f���w��</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function KoushakashoEntity(ByVal index As Integer) As TCrsLedgerKoshakashoEntityTehaiEx  '�R�[�X�䒠�i�~�ԃ����j�G���e�B�e�B
        Return _koshakashoEntity.EntityData(index)
    End Function
#End Region

End Class

