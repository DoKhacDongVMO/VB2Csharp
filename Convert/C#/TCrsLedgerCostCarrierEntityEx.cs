using System;
using Oracle.ManagedDataAccess.Client;


/// <summary>
/// コース台帳原価（キャリア）
/// </summary>
/// <remarks></remarks>
[Serializable()]  // コース台帳原価（キャリア）エンティティ
public partial class TCrsLedgerCostCarrierEntityTehaiEx : TCrsLedgerCostCarrierEntity
{

    /// <summary>
    /// 作成フラグ
    /// </summary>
    /// <remarks></remarks>
    public sealed partial class MakeFlgType
    {
        public const string On = "1";
        public const string Off = "0";
    }

    #region  子エンティティ 

    private EntityOperation _chargeKbnEntity; // _コース台帳原価（キャリア_料金区分）エンティティ
    #endregion
    private EntityKoumoku_MojiType _makeFlg = new EntityKoumoku_MojiType();    // コースコード

    public TCrsLedgerCostCarrierEntityTehaiEx()
    {
        _makeFlg.PhysicsName = "MAKE_FLG";
        _makeFlg.Required = false;
        _makeFlg.DBType = OracleDbType.Char;
        _makeFlg.IntegerBu = 1;
        _makeFlg.DecimalBu = 0;
        _chargeKbnEntity = new EntityOperation<TCrsLedgerCostCarrierChargeKbnEntity>();
    }

    #region  コース台帳原価（キャリア_料金区分）エンティティ 
    /// <summary>
    /// コース台帳原価（キャリア_料金区分）エンティティ（操作クラス）
    /// ※行No(降車ヶ所) = 0
    /// </summary>
    /// <returns></returns>
    /// <remarks></remarks>
    public EntityOperation CostCarrierChargeKbnEntity()  // 原価マスタ_キャリアエンティティ
    {
        return _chargeKbnEntity;
    }
    /// <summary>
    /// コース台帳原価（キャリア_料金区分）エンティティ（エンティティクラス）
    /// ※行No(降車ヶ所) = 0
    /// </summary>
    /// <param name="index">コース台帳原価（キャリア_料金区分）エンティティ配列要素を指定</param>
    /// <returns></returns>
    /// <remarks></remarks>
    public TCrsLedgerCostCarrierChargeKbnEntity CostCarrierChargeKbnEntity(int index)  // 原価マスタ_キャリアエンティティ
    {
        return _chargeKbnEntity.EntityData(index);
    }
    #endregion

    /// <summary>
    /// makeFlg
    /// </summary>
    /// <value></value>
    /// <returns></returns>
    /// <remarks></remarks>
    public EntityKoumoku_MojiType makeFlg
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
}