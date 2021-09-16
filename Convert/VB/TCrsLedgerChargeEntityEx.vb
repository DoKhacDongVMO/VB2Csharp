Imports Oracle.ManagedDataAccess.Client


''' <summary>
''' �R�[�X�䒠�i�����j
''' </summary>
''' <remarks></remarks>
<Serializable()>
Public Class TCrsLedgerChargeEntityEx  ' �R�[�X�䒠�i�����j�G���e�B�e�B
    Inherits TCrsLedgerChargeEntity

#Region " �q�G���e�B�e�B "
    Private _chargeChargeKbnEntity As EntityOperation(Of TCrsLedgerChargeChargeKbnEntity)   '_�R�[�X�䒠�i����_�����敪�j�G���e�B�e�B
#End Region

    ''' <summary>
    ''' �ҏW�t���O
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
    ''' �o����(�\��)
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property MakeFlg() As EntityKoumoku_MojiType  '�쐬�t���O
        Get
            Return _makeFlg
        End Get
        Set(ByVal value As EntityKoumoku_MojiType)
            _makeFlg = value
        End Set
    End Property

#Region " �R�[�X�䒠�i����_�����敪�j�G���e�B�e�B "
    ''' <summary>
    ''' �R�[�X�䒠�i����_�����敪�j�G���e�B�e�B�i����N���X�j
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function ChargeChargeKbnEntity() As EntityOperation(Of TCrsLedgerChargeChargeKbnEntity)  '�R�[�X�䒠�i��{_�����敪�j�G���e�B�e�B
        Return _chargeChargeKbnEntity
    End Function
    ''' <summary>
    ''' �R�[�X�䒠�i����_�����敪�j�G���e�B�e�B�i�G���e�B�e�B�N���X�j
    ''' </summary>
    ''' <param name="index">�R�[�X�䒠�i����_�����敪�j�G���e�B�e�B�z��v�f���w��</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Function ChargeChargeKbnEntity(ByVal index As Integer) As TCrsLedgerChargeChargeKbnEntity  '�R�[�X�䒠�i��{_�����敪�j�G���e�B�e�B
        Return _chargeChargeKbnEntity.EntityData(index)
    End Function
#End Region


End Class

