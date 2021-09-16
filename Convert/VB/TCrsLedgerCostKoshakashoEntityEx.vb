Imports Oracle.ManagedDataAccess.Client


''' <summary>
''' �R�[�X�䒠�����i�~�ԃ����j
''' </summary>
''' <remarks></remarks>
<Serializable()>
Public Class TCrsLedgerCostKoshakashoEntityTehaiEx  ' �R�[�X�䒠�����i�~�ԃ����j�G���e�B�e�B
    Inherits TCrsLedgerCostKoshakashoEntity

    ''' <summary>
    ''' �쐬�t���O
    ''' </summary>
    ''' <remarks></remarks>
    Public NotInheritable Class MakeFlgType
        ''' <summary>
        ''' ���쐬���(INSERT�ΏۊO)
        ''' </summary>
        Public Const [On] As String = "1"
        ''' <summary>
        ''' �ҏW�Ϗ��(INSERT�Ώ�)
        ''' </summary>
        Public Const Off As String = "0"
    End Class

    ''' <summary>
    ''' ���q�萔�����������敪
    ''' </summary>
    ''' <remarks></remarks>
    Public NotInheritable Class GenkinSeisanKbnType
        Public Const unChecked As String = Nothing
        Public Const checked As String = "Y"
    End Class

    ''' <summary>
    ''' �o�X�P��
    ''' </summary>
    ''' <remarks></remarks>
    Public NotInheritable Class busTaniType
        Public Const unChecked As String = "0"
        Public Const checked As String = "1"
    End Class

#Region " �q�G���e�B�e�B "

    Private _chargeKbnEntity As EntityOperation(Of TCrsLedgerCostKoshakashoChargeKbnEntity) '_�R�[�X�䒠�����i�~�ԃ���_�����敪�j�G���e�B�e�B
#End Region
    Private _makeFlg As New EntityKoumoku_MojiType    ' �R�[�X�R�[�h
    Sub New()
        _makeFlg.PhysicsName = "MAKE_FLG"

        _makeFlg.Required = False

        _makeFlg.DBType = OracleDbType.Char

        _makeFlg.IntegerBu = 1

        _makeFlg.DecimalBu = 0

        _chargeKbnEntity = New EntityOperation(Of TCrsLedgerCostKoshakashoChargeKbnEntity)

    End Sub

#Region " �R�[�X�䒠�����i�~�ԃ���_�����敪�j�G���e�B�e�B "
    ''' <summary>
    ''' �R�[�X�䒠�����i�~�ԃ���_�����敪�j�G���e�B�e�B�i����N���X�j
    ''' ���sNo(�~�ԃ���) = 0
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function CostKoshakashoChargeKbnEntity() As EntityOperation(Of TCrsLedgerCostKoshakashoChargeKbnEntity)  '�����}�X�^_�L�����A�G���e�B�e�B
        Return _chargeKbnEntity
    End Function
    ''' <summary>
    ''' �R�[�X�䒠�����i�~�ԃ���_�����敪�j�G���e�B�e�B�i�G���e�B�e�B�N���X�j
    ''' ���sNo(�~�ԃ���) = 0
    ''' </summary>
    ''' <param name="index">�R�[�X�䒠�����i�~�ԃ���_�����敪�j�G���e�B�e�B�z��v�f���w��</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function CostKoshakashoChargeKbnEntity(ByVal index As Integer) As TCrsLedgerCostKoshakashoChargeKbnEntity  '�����}�X�^_�L�����A�G���e�B�e�B
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

