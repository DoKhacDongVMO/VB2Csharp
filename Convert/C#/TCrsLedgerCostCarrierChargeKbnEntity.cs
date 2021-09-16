using System;
using Oracle.ManagedDataAccess.Client;


/// <summary>
/// コース台帳原価（キャリア_料金区分）
/// </summary>
/// <remarks></remarks>
[Serializable()]
// コース台帳原価（キャリア_料金区分）エンティティ
public partial class TCrsLedgerCostCarrierChargeKbnEntity
{
    private EntityKoumoku_NumberType _syuptDay = new EntityKoumoku_NumberType();	// 出発日
    private EntityKoumoku_MojiType _crsCd = new EntityKoumoku_MojiType();	// コースコード
    private EntityKoumoku_NumberType _gousya = new EntityKoumoku_NumberType();	// 号車
    private EntityKoumoku_NumberType _crsItineraryLineNo = new EntityKoumoku_NumberType();	// コース行程行ＮＯ
    private EntityKoumoku_NumberType _lineNo = new EntityKoumoku_NumberType();	// 行ＮＯ
    private EntityKoumoku_MojiType _carrierCd = new EntityKoumoku_MojiType();	// キャリアコード
    private EntityKoumoku_MojiType _carrierEdaban = new EntityKoumoku_MojiType();	// キャリア枝番
    private EntityKoumoku_MojiType _chargeKbnJininCd = new EntityKoumoku_MojiType();	// 料金区分（人員）コード
    private EntityKoumoku_NumberType _tanka = new EntityKoumoku_NumberType();	// 単価
    private EntityKoumoku_MojiType _systemEntryPgmid = new EntityKoumoku_MojiType();	// システム登録ＰＧＭＩＤ
    private EntityKoumoku_MojiType _systemEntryPersonCd = new EntityKoumoku_MojiType();	// システム登録者コード
    private EntityKoumoku_YmdType _systemEntryDay = new EntityKoumoku_YmdType();	// システム登録日
    private EntityKoumoku_MojiType _systemUpdatePgmid = new EntityKoumoku_MojiType();	// システム更新ＰＧＭＩＤ
    private EntityKoumoku_MojiType _systemUpdatePersonCd = new EntityKoumoku_MojiType();	// システム更新者コード
    private EntityKoumoku_YmdType _systemUpdateDay = new EntityKoumoku_YmdType();	// システム更新日
    private EntityKoumoku_NumberType _deleteDate = new EntityKoumoku_NumberType();	// 削除日

    public TCrsLedgerCostCarrierChargeKbnEntity()
    {
        _syuptDay.PhysicsName = "SYUPT_DAY";
        _crsCd.PhysicsName = "CRS_CD";
        _gousya.PhysicsName = "GOUSYA";
        _crsItineraryLineNo.PhysicsName = "CRS_ITINERARY_LINE_NO";
        _lineNo.PhysicsName = "LINE_NO";
        _carrierCd.PhysicsName = "CARRIER_CD";
        _carrierEdaban.PhysicsName = "CARRIER_EDABAN";
        _chargeKbnJininCd.PhysicsName = "CHARGE_KBN_JININ_CD";
        _tanka.PhysicsName = "TANKA";
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
        _carrierCd.Required = false;
        _carrierEdaban.Required = false;
        _chargeKbnJininCd.Required = false;
        _tanka.Required = false;
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
        _carrierCd.DBType = OracleDbType.Char;
        _carrierEdaban.DBType = OracleDbType.Char;
        _chargeKbnJininCd.DBType = OracleDbType.Char;
        _tanka.DBType = OracleDbType.Decimal;
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
        _carrierCd.IntegerBu = 4;
        _carrierEdaban.IntegerBu = 2;
        _chargeKbnJininCd.IntegerBu = 2;
        _tanka.IntegerBu = 7;
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
        _carrierCd.DecimalBu = 0;
        _carrierEdaban.DecimalBu = 0;
        _chargeKbnJininCd.DecimalBu = 0;
        _tanka.DecimalBu = 0;
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
/// carrierCd
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_MojiType carrierCd
    {
        get
        {
            return _carrierCd;
        }

        set
        {
            _carrierCd = value;
        }
    }


    /// <summary>
/// carrierEdaban
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_MojiType carrierEdaban
    {
        get
        {
            return _carrierEdaban;
        }

        set
        {
            _carrierEdaban = value;
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
/// tanka
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_NumberType tanka
    {
        get
        {
            return _tanka;
        }

        set
        {
            _tanka = value;
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