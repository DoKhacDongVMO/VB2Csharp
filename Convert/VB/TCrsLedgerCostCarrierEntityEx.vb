Imports Oracle.ManagedDataAccess.Client


''' <summary>
''' �R�[�X�䒠�����i�L�����A�j
''' </summary>
''' <remarks></remarks>
<Serializable()>
Public Class TCrsLedgerCostCarrierEntityTehaiEx  ' �R�[�X�䒠�����i�L�����A�j�G���e�B�e�B
    Inherits TCrsLedgerCostCarrierEntity

    ''' <summary>
    ''' �쐬�t���O
    ''' </summary>
    ''' <remarks></remarks>
    Public NotInheritable Class MakeFlgType
        Public Const [On] As String = "1"
        Public Const Off As String = "0"
    End Class

#Region " �q�G���e�B�e�B "

    Private _chargeKbnEntity As EntityOperation(Of TCrsLedgerCostCarrierChargeKbnEntity) '_�R�[�X�䒠�����i�L�����A_�����敪�j�G���e�B�e�B
#End Region
    Private _makeFlg As New EntityKoumoku_MojiType    ' �R�[�X�R�[�h

    Sub New()
        _makeFlg.PhysicsName = "MAKE_FLG"

        _makeFlg.Required = False

        _makeFlg.DBType = OracleDbType.Char

        _makeFlg.IntegerBu = 1

        _makeFlg.DecimalBu = 0

        _chargeKbnEntity = New EntityOperation(Of TCrsLedgerCostCarrierChargeKbnEntity)
    End Sub

#Region " �R�[�X�䒠�����i�L�����A_�����敪�j�G���e�B�e�B "
    ''' <summary>
    ''' �R�[�X�䒠�����i�L�����A_�����敪�j�G���e�B�e�B�i����N���X�j
    ''' ���sNo(�~�ԃ���) = 0
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function CostCarrierChargeKbnEntity() As EntityOperation(Of TCrsLedgerCostCarrierChargeKbnEntity)  '�����}�X�^_�L�����A�G���e�B�e�B
        Return _chargeKbnEntity
    End Function
    ''' <summary>
    ''' �R�[�X�䒠�����i�L�����A_�����敪�j�G���e�B�e�B�i�G���e�B�e�B�N���X�j
    ''' ���sNo(�~�ԃ���) = 0
    ''' </summary>
    ''' <param name="index">�R�[�X�䒠�����i�L�����A_�����敪�j�G���e�B�e�B�z��v�f���w��</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function CostCarrierChargeKbnEntity(ByVal index As Integer) As TCrsLedgerCostCarrierChargeKbnEntity  '�����}�X�^_�L�����A�G���e�B�e�B
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

