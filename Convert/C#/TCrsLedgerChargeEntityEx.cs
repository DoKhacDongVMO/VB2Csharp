using System;
using Oracle.ManagedDataAccess.Client;


/// <summary>
/// コース台帳（料金）
/// </summary>
/// <remarks></remarks>
[Serializable()]  // コース台帳（料金）エンティティ
public partial class TCrsLedgerChargeEntityEx : TCrsLedgerChargeEntity
{

    #region  子エンティティ 
    private EntityOperation _chargeChargeKbnEntity;   // _コース台帳（料金_料金区分）エンティティ
    #endregion

    /// <summary>
    /// 編集フラグ
    /// </summary>
    /// <remarks></remarks>
    public sealed partial class MakeFlgType
    {
        public const string On = "1";
        public const string Off = "0";
    }

    private EntityKoumoku_MojiType _makeFlg = new EntityKoumoku_MojiType();  // 

    public TCrsLedgerChargeEntityEx()
    {
        _makeFlg.PhysicsName = "DISP_SYUPT_DAY";
        _makeFlg.Required = false;
        _makeFlg.DBType = OracleDbType.Varchar2;
        _makeFlg.IntegerBu = 99;
        _makeFlg.DecimalBu = 0;
        _makeFlg.Value = MakeFlgType.Off;
        _chargeChargeKbnEntity = new EntityOperation<TCrsLedgerChargeChargeKbnEntity>();
    }


    /// <summary>
    /// 出発日(表示)
    /// </summary>
    /// <value></value>
    /// <returns></returns>
    /// <remarks></remarks>
    public EntityKoumoku_MojiType MakeFlg  // 作成フラグ
    {
        get
        {
            return _makeFlg;
        }

        set
        {
            _makeFlg = value;
        }
    }

    #region  コース台帳（料金_料金区分）エンティティ 
    /// <summary>
    /// コース台帳（料金_料金区分）エンティティ（操作クラス）
    /// </summary>
    /// <returns></returns>
    /// <remarks></remarks>
    public EntityOperation ChargeChargeKbnEntity()  // コース台帳（基本_料金区分）エンティティ
    {
        return _chargeChargeKbnEntity;
    }
    /// <summary>
    /// コース台帳（料金_料金区分）エンティティ（エンティティクラス）
    /// </summary>
    /// <param name="index">コース台帳（料金_料金区分）エンティティ配列要素を指定</param>
    /// <returns></returns>
    /// <remarks></remarks>
    public TCrsLedgerChargeChargeKbnEntity ChargeChargeKbnEntity(int index)  // コース台帳（基本_料金区分）エンティティ
    {
        return _chargeChargeKbnEntity.EntityData(index);
    }
    #endregion


}