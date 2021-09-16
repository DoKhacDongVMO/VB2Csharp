using System;
using Oracle.ManagedDataAccess.Client;


/// <summary>
/// コース台帳原価（降車ヶ所_料金区分）
/// </summary>
/// <remarks></remarks>
[Serializable()]
// コース台帳原価（降車ヶ所_料金区分）エンティティ
public partial class TCrsLedgerCostKoshakashoChargeKbnEntity
{
    private EntityKoumoku_NumberType _syuptDay = new EntityKoumoku_NumberType();	// 出発日
    private EntityKoumoku_MojiType _crsCd = new EntityKoumoku_MojiType();	// コースコード
    private EntityKoumoku_NumberType _gousya = new EntityKoumoku_NumberType();	// 号車
    private EntityKoumoku_NumberType _crsItineraryLineNo = new EntityKoumoku_NumberType();	// コース行程行ＮＯ
    private EntityKoumoku_NumberType _lineNo = new EntityKoumoku_NumberType();	// 行ＮＯ
    private EntityKoumoku_MojiType _siireSakiCd = new EntityKoumoku_MojiType();	// 仕入先コード
    private EntityKoumoku_MojiType _siireSakiEdaban = new EntityKoumoku_MojiType();	// 仕入先枝番
    private EntityKoumoku_MojiType _heijituTokuteiDayKbn = new EntityKoumoku_MojiType();	// 平日／特定日区分
    private EntityKoumoku_MojiType _chargeKbnJininCd = new EntityKoumoku_MojiType();	// 料金区分（人員）コード
    private EntityKoumoku_NumberType _bathTax = new EntityKoumoku_NumberType();	// 入湯税
    private EntityKoumoku_NumberType _siharaiGaku = new EntityKoumoku_NumberType();	// 支払額
    private EntityKoumoku_NumberType _kouseiGaku = new EntityKoumoku_NumberType();	// 構成額
    private EntityKoumoku_NumberType _charge1 = new EntityKoumoku_NumberType();	// 料金１
    private EntityKoumoku_NumberType _charge2 = new EntityKoumoku_NumberType();	// 料金２
    private EntityKoumoku_NumberType _charge3 = new EntityKoumoku_NumberType();	// 料金３
    private EntityKoumoku_NumberType _charge4 = new EntityKoumoku_NumberType();	// 料金４
    private EntityKoumoku_NumberType _charge5 = new EntityKoumoku_NumberType();	// 料金５
    private EntityKoumoku_MojiType _systemEntryPgmid = new EntityKoumoku_MojiType();	// システム登録ＰＧＭＩＤ
    private EntityKoumoku_MojiType _systemEntryPersonCd = new EntityKoumoku_MojiType();	// システム登録者コード
    private EntityKoumoku_YmdType _systemEntryDay = new EntityKoumoku_YmdType();	// システム登録日
    private EntityKoumoku_MojiType _systemUpdatePgmid = new EntityKoumoku_MojiType();	// システム更新ＰＧＭＩＤ
    private EntityKoumoku_MojiType _systemUpdatePersonCd = new EntityKoumoku_MojiType();	// システム更新者コード
    private EntityKoumoku_YmdType _systemUpdateDay = new EntityKoumoku_YmdType();	// システム更新日
    private EntityKoumoku_NumberType _deleteDate = new EntityKoumoku_NumberType();	// 削除日

    public TCrsLedgerCostKoshakashoChargeKbnEntity()
    {
        _syuptDay.PhysicsName = "SYUPT_DAY";
        _crsCd.PhysicsName = "CRS_CD";
        _gousya.PhysicsName = "GOUSYA";
        _crsItineraryLineNo.PhysicsName = "CRS_ITINERARY_LINE_NO";
        _lineNo.PhysicsName = "LINE_NO";
        _siireSakiCd.PhysicsName = "SIIRE_SAKI_CD";
        _siireSakiEdaban.PhysicsName = "SIIRE_SAKI_EDABAN";
        _heijituTokuteiDayKbn.PhysicsName = "HEIJITU_TOKUTEI_DAY_KBN";
        _chargeKbnJininCd.PhysicsName = "CHARGE_KBN_JININ_CD";
        _bathTax.PhysicsName = "BATH_TAX";
        _siharaiGaku.PhysicsName = "SIHARAI_GAKU";
        _kouseiGaku.PhysicsName = "KOUSEI_GAKU";
        _charge1.PhysicsName = "CHARGE_1";
        _charge2.PhysicsName = "CHARGE_2";
        _charge3.PhysicsName = "CHARGE_3";
        _charge4.PhysicsName = "CHARGE_4";
        _charge5.PhysicsName = "CHARGE_5";
        _systemEntryPgmid.PhysicsName = "SYSTEM_ENTRY_PGMID";
        _systemEntryPersonCd.PhysicsName = "SYSTEM_ENTRY_PERSON_CD";
        _systemEntryDay.PhysicsName = "SYSTEM_ENTRY_DAY";
        _systemUpdatePgmid.PhysicsName = "SYSTEM_UPDATE_PGMID";
        _systemUpdatePersonCd.PhysicsName = "SYSTEM_UPDATE_PERSON_CD";
        _systemUpdateDay.PhysicsName = "SYSTEM_UPDATE_DAY";
        _deleteDate.PhysicsName = "DELETE_DATE";
        _syuptDay.Required = false;
        _crsCd.Required = false;
        _gousya.Required = false;
        _crsItineraryLineNo.Required = false;
        _lineNo.Required = false;
        _siireSakiCd.Required = false;
        _siireSakiEdaban.Required = false;
        _heijituTokuteiDayKbn.Required = false;
        _chargeKbnJininCd.Required = false;
        _bathTax.Required = false;
        _siharaiGaku.Required = false;
        _kouseiGaku.Required = false;
        _charge1.Required = false;
        _charge2.Required = false;
        _charge3.Required = false;
        _charge4.Required = false;
        _charge5.Required = false;
        _systemEntryPgmid.Required = true;
        _systemEntryPersonCd.Required = true;
        _systemEntryDay.Required = true;
        _systemUpdatePgmid.Required = true;
        _systemUpdatePersonCd.Required = true;
        _systemUpdateDay.Required = true;
        _deleteDate.Required = false;
        _syuptDay.DBType = OracleDbType.Decimal;
        _crsCd.DBType = OracleDbType.Char;
        _gousya.DBType = OracleDbType.Decimal;
        _crsItineraryLineNo.DBType = OracleDbType.Decimal;
        _lineNo.DBType = OracleDbType.Decimal;
        _siireSakiCd.DBType = OracleDbType.Char;
        _siireSakiEdaban.DBType = OracleDbType.Char;
        _heijituTokuteiDayKbn.DBType = OracleDbType.Char;
        _chargeKbnJininCd.DBType = OracleDbType.Char;
        _bathTax.DBType = OracleDbType.Decimal;
        _siharaiGaku.DBType = OracleDbType.Decimal;
        _kouseiGaku.DBType = OracleDbType.Decimal;
        _charge1.DBType = OracleDbType.Decimal;
        _charge2.DBType = OracleDbType.Decimal;
        _charge3.DBType = OracleDbType.Decimal;
        _charge4.DBType = OracleDbType.Decimal;
        _charge5.DBType = OracleDbType.Decimal;
        _systemEntryPgmid.DBType = OracleDbType.Char;
        _systemEntryPersonCd.DBType = OracleDbType.Varchar2;
        _systemEntryDay.DBType = OracleDbType.Date;
        _systemUpdatePgmid.DBType = OracleDbType.Char;
        _systemUpdatePersonCd.DBType = OracleDbType.Varchar2;
        _systemUpdateDay.DBType = OracleDbType.Date;
        _deleteDate.DBType = OracleDbType.Decimal;
        _syuptDay.IntegerBu = 8;
        _crsCd.IntegerBu = 6;
        _gousya.IntegerBu = 3;
        _crsItineraryLineNo.IntegerBu = 2;
        _lineNo.IntegerBu = 3;
        _siireSakiCd.IntegerBu = 4;
        _siireSakiEdaban.IntegerBu = 2;
        _heijituTokuteiDayKbn.IntegerBu = 1;
        _chargeKbnJininCd.IntegerBu = 2;
        _bathTax.IntegerBu = 7;
        _siharaiGaku.IntegerBu = 7;
        _kouseiGaku.IntegerBu = 7;
        _charge1.IntegerBu = 7;
        _charge2.IntegerBu = 7;
        _charge3.IntegerBu = 7;
        _charge4.IntegerBu = 7;
        _charge5.IntegerBu = 7;
        _systemEntryPgmid.IntegerBu = 8;
        _systemEntryPersonCd.IntegerBu = 20;
        _systemEntryDay.IntegerBu = 0;
        _systemUpdatePgmid.IntegerBu = 8;
        _systemUpdatePersonCd.IntegerBu = 20;
        _systemUpdateDay.IntegerBu = 0;
        _deleteDate.IntegerBu = 8;
        _syuptDay.DecimalBu = 0;
        _crsCd.DecimalBu = 0;
        _gousya.DecimalBu = 0;
        _crsItineraryLineNo.DecimalBu = 0;
        _lineNo.DecimalBu = 0;
        _siireSakiCd.DecimalBu = 0;
        _siireSakiEdaban.DecimalBu = 0;
        _heijituTokuteiDayKbn.DecimalBu = 0;
        _chargeKbnJininCd.DecimalBu = 0;
        _bathTax.DecimalBu = 0;
        _siharaiGaku.DecimalBu = 0;
        _kouseiGaku.DecimalBu = 0;
        _charge1.DecimalBu = 0;
        _charge2.DecimalBu = 0;
        _charge3.DecimalBu = 0;
        _charge4.DecimalBu = 0;
        _charge5.DecimalBu = 0;
        _systemEntryPgmid.DecimalBu = 0;
        _systemEntryPersonCd.DecimalBu = 0;
        _systemEntryDay.DecimalBu = 0;
        _systemUpdatePgmid.DecimalBu = 0;
        _systemUpdatePersonCd.DecimalBu = 0;
        _systemUpdateDay.DecimalBu = 0;
        _deleteDate.DecimalBu = 0;
    }


    /// <summary>
/// syuptDay
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_NumberType syuptDay
    {
        get
        {
            return _syuptDay;
        }

        set
        {
            _syuptDay = value;
        }
    }


    /// <summary>
/// crsCd
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_MojiType crsCd
    {
        get
        {
            return _crsCd;
        }

        set
        {
            _crsCd = value;
        }
    }


    /// <summary>
/// gousya
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_NumberType gousya
    {
        get
        {
            return _gousya;
        }

        set
        {
            _gousya = value;
        }
    }


    /// <summary>
/// crsItineraryLineNo
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_NumberType crsItineraryLineNo
    {
        get
        {
            return _crsItineraryLineNo;
        }

        set
        {
            _crsItineraryLineNo = value;
        }
    }


    /// <summary>
/// lineNo
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_NumberType lineNo
    {
        get
        {
            return _lineNo;
        }

        set
        {
            _lineNo = value;
        }
    }


    /// <summary>
/// siireSakiCd
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_MojiType siireSakiCd
    {
        get
        {
            return _siireSakiCd;
        }

        set
        {
            _siireSakiCd = value;
        }
    }


    /// <summary>
/// siireSakiEdaban
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_MojiType siireSakiEdaban
    {
        get
        {
            return _siireSakiEdaban;
        }

        set
        {
            _siireSakiEdaban = value;
        }
    }


    /// <summary>
/// heijituTokuteiDayKbn
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_MojiType heijituTokuteiDayKbn
    {
        get
        {
            return _heijituTokuteiDayKbn;
        }

        set
        {
            _heijituTokuteiDayKbn = value;
        }
    }


    /// <summary>
/// chargeKbnJininCd
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_MojiType chargeKbnJininCd
    {
        get
        {
            return _chargeKbnJininCd;
        }

        set
        {
            _chargeKbnJininCd = value;
        }
    }


    /// <summary>
/// bathTax
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_NumberType bathTax
    {
        get
        {
            return _bathTax;
        }

        set
        {
            _bathTax = value;
        }
    }


    /// <summary>
/// siharaiGaku
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_NumberType siharaiGaku
    {
        get
        {
            return _siharaiGaku;
        }

        set
        {
            _siharaiGaku = value;
        }
    }


    /// <summary>
/// kouseiGaku
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_NumberType kouseiGaku
    {
        get
        {
            return _kouseiGaku;
        }

        set
        {
            _kouseiGaku = value;
        }
    }


    /// <summary>
/// charge1
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_NumberType charge1
    {
        get
        {
            return _charge1;
        }

        set
        {
            _charge1 = value;
        }
    }


    /// <summary>
/// charge2
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_NumberType charge2
    {
        get
        {
            return _charge2;
        }

        set
        {
            _charge2 = value;
        }
    }


    /// <summary>
/// charge3
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_NumberType charge3
    {
        get
        {
            return _charge3;
        }

        set
        {
            _charge3 = value;
        }
    }


    /// <summary>
/// charge4
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_NumberType charge4
    {
        get
        {
            return _charge4;
        }

        set
        {
            _charge4 = value;
        }
    }


    /// <summary>
/// charge5
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_NumberType charge5
    {
        get
        {
            return _charge5;
        }

        set
        {
            _charge5 = value;
        }
    }


    /// <summary>
/// systemEntryPgmid
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_MojiType systemEntryPgmid
    {
        get
        {
            return _systemEntryPgmid;
        }

        set
        {
            _systemEntryPgmid = value;
        }
    }


    /// <summary>
/// systemEntryPersonCd
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_MojiType systemEntryPersonCd
    {
        get
        {
            return _systemEntryPersonCd;
        }

        set
        {
            _systemEntryPersonCd = value;
        }
    }


    /// <summary>
/// systemEntryDay
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_YmdType systemEntryDay
    {
        get
        {
            return _systemEntryDay;
        }

        set
        {
            _systemEntryDay = value;
        }
    }


    /// <summary>
/// systemUpdatePgmid
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_MojiType systemUpdatePgmid
    {
        get
        {
            return _systemUpdatePgmid;
        }

        set
        {
            _systemUpdatePgmid = value;
        }
    }


    /// <summary>
/// systemUpdatePersonCd
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_MojiType systemUpdatePersonCd
    {
        get
        {
            return _systemUpdatePersonCd;
        }

        set
        {
            _systemUpdatePersonCd = value;
        }
    }


    /// <summary>
/// systemUpdateDay
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_YmdType systemUpdateDay
    {
        get
        {
            return _systemUpdateDay;
        }

        set
        {
            _systemUpdateDay = value;
        }
    }


    /// <summary>
/// deleteDate
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_NumberType deleteDate
    {
        get
        {
            return _deleteDate;
        }

        set
        {
            _deleteDate = value;
        }
    }
}