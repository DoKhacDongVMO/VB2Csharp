using System;
using Oracle.ManagedDataAccess.Client;


/// <summary>
/// コース台帳（基本）
/// </summary>
/// <remarks></remarks>
[Serializable()]  // コース台帳（基本）エンティティ
public partial class TCrsLedgerBasicEntityTehaiEx : TCrsLedgerBasicEntity
{

    /// <summary>
    /// 編集フラグ
    /// </summary>
    /// <remarks></remarks>
    public sealed partial class EditFlgType
    {
        public const string On = "1";
        public const string Off = "0";
    }

    private EntityKoumoku_MojiType _dispSyuptDay = new EntityKoumoku_MojiType();  // 出発日(表示)
    private EntityKoumoku_MojiType _jyoSyaTiName = new EntityKoumoku_MojiType();  // 乗車地
    private EntityKoumoku_MojiType _yobiName = new EntityKoumoku_MojiType();      // 曜日
    private EntityKoumoku_MojiType _syuPtTime = new EntityKoumoku_MojiType();     // 出発時間
    private EntityKoumoku_MojiType _unkyuName = new EntityKoumoku_MojiType();     // 運休区分
    private EntityKoumoku_MojiType _editFlg = new EntityKoumoku_MojiType();       // 編集フラグ
    private EntityKoumoku_NumberType _carrierLineNo = new EntityKoumoku_NumberType();  // キャリアライン番号
    private EntityKoumoku_MojiType _koushaEditFlg = new EntityKoumoku_MojiType();    // 降車ヶ所編集フラグ
    private EntityKoumoku_MojiType _sonotaEditFlg = new EntityKoumoku_MojiType();    // その他原価編集フラグ

    #region  子エンティティ 
    private EntityOperation _chargeKbnEntity;   // _コース台帳（基本_料金区分）エンティティ
    private EntityOperation _costPlateEntity;               // _コース台帳原価（プレート）エンティティ
    private EntityOperation _costBasicEntity;               // _コース台帳原価（基本）エンティティ
    private EntityOperation _koshakashoEntity;      // _コース台帳（降車ヶ所）エンティティ

    #endregion

    public TCrsLedgerBasicEntityTehaiEx()
    {
        _dispSyuptDay.PhysicsName = "DISP_SYUPT_DAY";
        _jyoSyaTiName.PhysicsName = "JYOSYA_TI_NAME";
        _yobiName.PhysicsName = "YOBI_NAME";
        _syuPtTime.PhysicsName = "SYUPT_TIME";
        _unkyuName.PhysicsName = "UNKYU_NAME";
        _editFlg.PhysicsName = "EDIT_FLG";
        _carrierLineNo.PhysicsName = "CARRIER_LINENO";
        _koushaEditFlg.PhysicsName = "KOUSHA_EDIT_FLG";
        _sonotaEditFlg.PhysicsName = "SONOTA_EDIT_FLG";
        _dispSyuptDay.Required = false;
        _jyoSyaTiName.Required = false;
        _yobiName.Required = false;
        _syuPtTime.Required = false;
        _unkyuName.Required = false;
        _editFlg.Required = false;
        _carrierLineNo.Required = false;
        _koushaEditFlg.Required = false;
        _sonotaEditFlg.Required = false;
        _dispSyuptDay.DBType = OracleDbType.Varchar2;
        _jyoSyaTiName.DBType = OracleDbType.Varchar2;
        _yobiName.DBType = OracleDbType.Varchar2;
        _syuPtTime.DBType = OracleDbType.Varchar2;
        _unkyuName.DBType = OracleDbType.Varchar2;
        _editFlg.DBType = OracleDbType.Varchar2;
        _carrierLineNo.DBType = OracleDbType.Decimal;
        _koushaEditFlg.DBType = OracleDbType.Varchar2;
        _sonotaEditFlg.DBType = OracleDbType.Varchar2;
        _dispSyuptDay.IntegerBu = 99;
        _jyoSyaTiName.IntegerBu = 22;
        _yobiName.IntegerBu = 64;
        _syuPtTime.IntegerBu = 99;
        _unkyuName.IntegerBu = 64;
        _editFlg.IntegerBu = 1;
        _carrierLineNo.IntegerBu = 3;
        _koushaEditFlg.IntegerBu = 1;
        _sonotaEditFlg.IntegerBu = 1;
        _dispSyuptDay.DecimalBu = 0;
        _jyoSyaTiName.DecimalBu = 0;
        _yobiName.DecimalBu = 0;
        _syuPtTime.DecimalBu = 0;
        _unkyuName.DecimalBu = 0;
        _editFlg.DecimalBu = 0;
        _carrierLineNo.DecimalBu = 0;
        _koushaEditFlg.DecimalBu = 0;
        _sonotaEditFlg.DecimalBu = 0;
        _chargeKbnEntity = new EntityOperation<TCrsLedgerBasicChargeKbnEntityTehaiEx>();
        _costPlateEntity = new EntityOperation<TCrsLedgerCostPlateEntity>();
        _costBasicEntity = new EntityOperation<TCrsLedgerCostBasicEntity>();
        _koshakashoEntity = new EntityOperation<TCrsLedgerKoshakashoEntityTehaiEx>();
    }

    /// <summary>
    /// 出発日(表示)
    /// </summary>
    /// <value></value>
    /// <returns></returns>
    /// <remarks></remarks>
    public EntityKoumoku_MojiType DispSyuptDay  // 出発日(表示)
    {
        get
        {
            return _dispSyuptDay;
        }

        set
        {
            _dispSyuptDay = value;
        }
    }

    /// <summary>
    /// 乗車地
    /// </summary>
    /// <value></value>
    /// <returns></returns>
    /// <remarks></remarks>
    public EntityKoumoku_MojiType JyoSyaTiName  // 乗車地
    {
        get
        {
            return _jyoSyaTiName;
        }

        set
        {
            _jyoSyaTiName = value;
        }
    }

    /// <summary>
    /// 曜日
    /// </summary>
    /// <value></value>
    /// <returns></returns>
    /// <remarks></remarks>
    public EntityKoumoku_MojiType YobiName  // 曜日
    {
        get
        {
            return _yobiName;
        }

        set
        {
            _yobiName = value;
        }
    }

    /// <summary>
    /// 出発時間
    /// </summary>
    /// <value></value>
    /// <returns></returns>
    /// <remarks></remarks>
    public EntityKoumoku_MojiType SyuPtTime  // 出発時間
    {
        get
        {
            return _syuPtTime;
        }

        set
        {
            _syuPtTime = value;
        }
    }

    /// <summary>
    /// 運休区分
    /// </summary>
    /// <value></value>
    /// <returns></returns>
    /// <remarks></remarks>
    public EntityKoumoku_MojiType UnkyuName  // 運休区分
    {
        get
        {
            return _unkyuName;
        }

        set
        {
            _unkyuName = value;
        }
    }

    /// <summary>
    /// 編集フラグ
    /// </summary>
    /// <value></value>
    /// <returns></returns>
    /// <remarks></remarks>
    public EntityKoumoku_MojiType EditFlg  // 編集フラグ
    {
        get
        {
            return _editFlg;
        }

        set
        {
            _editFlg = value;
        }
    }

    /// <summary>
    /// キャリアライン番号
    /// </summary>
    /// <value></value>
    /// <returns></returns>
    /// <remarks></remarks>
    public EntityKoumoku_NumberType CarrierLineNo
    {
        get
        {
            return _carrierLineNo;
        }

        set
        {
            _carrierLineNo = value;
        }
    }

    /// <summary>
    /// 降車ヶ所編集フラグ
    /// </summary>
    /// <value></value>
    /// <returns></returns>
    /// <remarks></remarks>
    public EntityKoumoku_MojiType KoushaEditFlg
    {
        get
        {
            return _koushaEditFlg;
        }

        set
        {
            _koushaEditFlg = value;
        }
    }

    /// <summary>
    /// その他原価編集フラグ
    /// </summary>
    /// <value></value>
    /// <returns></returns>
    /// <remarks></remarks>
    public EntityKoumoku_MojiType SonotaEditFlg
    {
        get
        {
            return _sonotaEditFlg;
        }

        set
        {
            _sonotaEditFlg = value;
        }
    }

    #region  コース台帳（基本_料金区分）エンティティ 
    /// <summary>
    /// コース台帳（基本_料金区分）エンティティ（操作クラス）
    /// ※行No(降車ヶ所) = 0
    /// </summary>
    /// <returns></returns>
    /// <remarks></remarks>
    public EntityOperation ChargeKbnEntity()  // コース台帳（基本_料金区分）エンティティ
    {
        return _chargeKbnEntity;
    }
    /// <summary>
    /// コース台帳（基本_料金区分）エンティティ（エンティティクラス）
    /// ※行No(降車ヶ所) = 0
    /// </summary>
    /// <param name="index">コース台帳（基本_料金区分）エンティティ配列要素を指定</param>
    /// <returns></returns>
    /// <remarks></remarks>
    public TCrsLedgerBasicChargeKbnEntityTehaiEx ChargeKbnEntity(int index)  // コース台帳（基本_料金区分）エンティティ
    {
        return _chargeKbnEntity.EntityData(index);
    }
    #endregion

    #region  コース台帳原価（プレート）エンティティ 
    /// <summary>
    /// コース台帳原価（プレート）エンティティ（操作クラス）
    /// ※行No(降車ヶ所) = 0
    /// </summary>
    /// <returns></returns>
    /// <remarks></remarks>
    public EntityOperation CostPlateEntity()  // コース台帳原価（プレート）エンティティ
    {
        return _costPlateEntity;
    }
    /// <summary>
    /// コース台帳原価（プレート）エンティティ（エンティティクラス）
    /// ※行No(降車ヶ所) = 0
    /// </summary>
    /// <param name="index">コース台帳原価（プレート）エンティティ配列要素を指定</param>
    /// <returns></returns>
    /// <remarks></remarks>
    public TCrsLedgerCostPlateEntity CostPlateEntity(int index)  // コース台帳原価（プレート）エンティティ
    {
        return _costPlateEntity.EntityData(index);
    }
    #endregion

    #region  コース台帳原価（基本）エンティティ 
    /// <summary>
    /// コース台帳原価（基本）エンティティ（操作クラス）
    /// ※行No(降車ヶ所) = 0
    /// </summary>
    /// <returns></returns>
    /// <remarks></remarks>
    public EntityOperation CostBasicEntity()  // コース台帳原価（基本）エンティティ
    {
        return _costBasicEntity;
    }
    /// <summary>
    /// コース台帳原価（基本）エンティティ（エンティティクラス）
    /// ※行No(降車ヶ所) = 0
    /// </summary>
    /// <param name="index">コース台帳原価（基本）エンティティ配列要素を指定</param>
    /// <returns></returns>
    /// <remarks></remarks>
    public TCrsLedgerCostBasicEntity CostBasicEntity(int index)  // コース台帳原価（基本）エンティティ
    {
        return _costBasicEntity.EntityData(index);
    }

    #endregion

    #region  コース台帳（降車ヶ所）エンティティ 
    /// <summary>
    /// コース台帳（降車ヶ所）エンティティ（操作クラス）
    /// ※行No(降車ヶ所) = 0
    /// </summary>
    /// <returns></returns>
    /// <remarks></remarks>
    public EntityOperation KoushakashoEntity()  // コース台帳（降車ヶ所）エンティティ
    {
        return _koshakashoEntity;
    }

    /// <summary>
    /// コース台帳（降車ヶ所）エンティティ（エンティティクラス）
    /// ※行No(降車ヶ所) = 0
    /// </summary>
    /// <param name="index">コース台帳（降車ヶ所）エンティティ配列要素を指定</param>
    /// <returns></returns>
    /// <remarks></remarks>
    public TCrsLedgerKoshakashoEntityTehaiEx KoushakashoEntity(int index)  // コース台帳（降車ヶ所）エンティティ
    {
        return _koshakashoEntity.EntityData(index);
    }
    #endregion

}