using System;
using Oracle.ManagedDataAccess.Client;


/// <summary>
/// コース台帳原価（降車ヶ所）
/// </summary>
/// <remarks></remarks>
[Serializable()]  // コース台帳原価（降車ヶ所）エンティティ
public partial class TCrsLedgerCostKoshakashoEntityTehaiEx : TCrsLedgerCostKoshakashoEntity
{

    /// <summary>
    /// 作成フラグ
    /// </summary>
    /// <remarks></remarks>
    public sealed partial class MakeFlgType
    {
        /// <summary>
        /// 仮作成状態(INSERT対象外)
        /// </summary>
        public const string On = "1";
        /// <summary>
        /// 編集済状態(INSERT対象)
        /// </summary>
        public const string Off = "0";
    }

    /// <summary>
    /// 送客手数料現金払い区分
    /// </summary>
    /// <remarks></remarks>
    public sealed partial class GenkinSeisanKbnType
    {
        public const string unChecked = null;
        public const string @checked = "Y";
    }

    /// <summary>
    /// バス単位
    /// </summary>
    /// <remarks></remarks>
    public sealed partial class busTaniType
    {
        public const string unChecked = "0";
        public const string @checked = "1";
    }

    #region  子エンティティ 

    private EntityOperation _chargeKbnEntity; // _コース台帳原価（降車ヶ所_料金区分）エンティティ
    #endregion
    private EntityKoumoku_MojiType _makeFlg = new EntityKoumoku_MojiType();    // コースコード

    public TCrsLedgerCostKoshakashoEntityTehaiEx()
    {
        _makeFlg.PhysicsName = "MAKE_FLG";
        _makeFlg.Required = false;
        _makeFlg.DBType = OracleDbType.Char;
        _makeFlg.IntegerBu = 1;
        _makeFlg.DecimalBu = 0;
        _chargeKbnEntity = new EntityOperation<TCrsLedgerCostKoshakashoChargeKbnEntity>();
    }

    #region  コース台帳原価（降車ヶ所_料金区分）エンティティ 
    /// <summary>
    /// コース台帳原価（降車ヶ所_料金区分）エンティティ（操作クラス）
    /// ※行No(降車ヶ所) = 0
    /// </summary>
    /// <returns></returns>
    /// <remarks></remarks>
    public EntityOperation CostKoshakashoChargeKbnEntity()  // 原価マスタ_キャリアエンティティ
    {
        return _chargeKbnEntity;
    }
    /// <summary>
    /// コース台帳原価（降車ヶ所_料金区分）エンティティ（エンティティクラス）
    /// ※行No(降車ヶ所) = 0
    /// </summary>
    /// <param name="index">コース台帳原価（降車ヶ所_料金区分）エンティティ配列要素を指定</param>
    /// <returns></returns>
    /// <remarks></remarks>
    public TCrsLedgerCostKoshakashoChargeKbnEntity CostKoshakashoChargeKbnEntity(int index)  // 原価マスタ_キャリアエンティティ
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