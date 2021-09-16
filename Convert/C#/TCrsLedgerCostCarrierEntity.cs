using System;
using Oracle.ManagedDataAccess.Client;


/// <summary>
/// コース台帳原価（キャリア）
/// </summary>
/// <remarks></remarks>
[Serializable()]
// コース台帳原価（キャリア）エンティティ
public partial class TCrsLedgerCostCarrierEntity
{
    private EntityKoumoku_MojiType _carrierCd = new EntityKoumoku_MojiType();	// キャリアコード
    private EntityKoumoku_MojiType _carrierEdaban = new EntityKoumoku_MojiType();	// キャリア枝番
    private EntityKoumoku_NumberType _classLineNo = new EntityKoumoku_NumberType();	// クラス行ＮＯ
    private EntityKoumoku_MojiType _classWording = new EntityKoumoku_MojiType();	// クラス文言
    private EntityKoumoku_NumberType _com = new EntityKoumoku_NumberType();	// ＣＯＭ
    private EntityKoumoku_MojiType _crsCd = new EntityKoumoku_MojiType();	// コースコード
    private EntityKoumoku_NumberType _crsItineraryLineNo = new EntityKoumoku_NumberType();	// コース行程行ＮＯ
    private EntityKoumoku_NumberType _deleteDay = new EntityKoumoku_NumberType();	// 削除日
    private EntityKoumoku_NumberType _gousya = new EntityKoumoku_NumberType();	// 号車
    private EntityKoumoku_NumberType _lineNo = new EntityKoumoku_NumberType();	// 行ＮＯ
    private EntityKoumoku_NumberType _riyouDay = new EntityKoumoku_NumberType();	// 利用日
    private EntityKoumoku_MojiType _seisanHoho = new EntityKoumoku_MojiType();	// 精算方法
    private EntityKoumoku_NumberType _syuptDay = new EntityKoumoku_NumberType();	// 出発日
    private EntityKoumoku_MojiType _taxKbn = new EntityKoumoku_MojiType();	// 税区分
    private EntityKoumoku_MojiType _systemEntryPgmid = new EntityKoumoku_MojiType();	// システム登録ＰＧＭＩＤ
    private EntityKoumoku_MojiType _systemEntryPersonCd = new EntityKoumoku_MojiType();	// システム登録者コード
    private EntityKoumoku_YmdType _systemEntryDay = new EntityKoumoku_YmdType();	// システム登録日
    private EntityKoumoku_MojiType _systemUpdatePgmid = new EntityKoumoku_MojiType();	// システム更新ＰＧＭＩＤ
    private EntityKoumoku_MojiType _systemUpdatePersonCd = new EntityKoumoku_MojiType();	// システム更新者コード
    private EntityKoumoku_YmdType _systemUpdateDay = new EntityKoumoku_YmdType();	// システム更新日

    public TCrsLedgerCostCarrierEntity()
    {
        _carrierCd.PhysicsName = "CARRIER_CD";
        _carrierEdaban.PhysicsName = "CARRIER_EDABAN";
        _classLineNo.PhysicsName = "CLASS_LINE_NO";
        _classWording.PhysicsName = "CLASS_WORDING";
        _com.PhysicsName = "COM";
        _crsCd.PhysicsName = "CRS_CD";
        _crsItineraryLineNo.PhysicsName = "CRS_ITINERARY_LINE_NO";
        _deleteDay.PhysicsName = "DELETE_DAY";
        _gousya.PhysicsName = "GOUSYA";
        _lineNo.PhysicsName = "LINE_NO";
        _riyouDay.PhysicsName = "RIYOU_DAY";
        _seisanHoho.PhysicsName = "SEISAN_HOHO";
        _syuptDay.PhysicsName = "SYUPT_DAY";
        _taxKbn.PhysicsName = "TAX_KBN";
        _systemEntryPgmid.PhysicsName = "SYSTEM_ENTRY_PGMID";
        _systemEntryPersonCd.PhysicsName = "SYSTEM_ENTRY_PERSON_CD";
        _systemEntryDay.PhysicsName = "SYSTEM_ENTRY_DAY";
        _systemUpdatePgmid.PhysicsName = "SYSTEM_UPDATE_PGMID";
        _systemUpdatePersonCd.PhysicsName = "SYSTEM_UPDATE_PERSON_CD";
        _systemUpdateDay.PhysicsName = "SYSTEM_UPDATE_DAY";
        _carrierCd.Required = false;
        _carrierEdaban.Required = false;
        _classLineNo.Required = false;
        _classWording.Required = false;
        _com.Required = false;
        _crsCd.Required = false;
        _crsItineraryLineNo.Required = false;
        _deleteDay.Required = false;
        _gousya.Required = false;
        _lineNo.Required = false;
        _riyouDay.Required = false;
        _seisanHoho.Required = false;
        _syuptDay.Required = false;
        _taxKbn.Required = false;
        _systemEntryPgmid.Required = true;
        _systemEntryPersonCd.Required = true;
        _systemEntryDay.Required = true;
        _systemUpdatePgmid.Required = true;
        _systemUpdatePersonCd.Required = true;
        _systemUpdateDay.Required = true;
        _carrierCd.DBType = OracleDbType.Char;
        _carrierEdaban.DBType = OracleDbType.Char;
        _classLineNo.DBType = OracleDbType.Decimal;
        _classWording.DBType = OracleDbType.Varchar2;
        _com.DBType = OracleDbType.Decimal;
        _crsCd.DBType = OracleDbType.Char;
        _crsItineraryLineNo.DBType = OracleDbType.Decimal;
        _deleteDay.DBType = OracleDbType.Decimal;
        _gousya.DBType = OracleDbType.Decimal;
        _lineNo.DBType = OracleDbType.Decimal;
        _riyouDay.DBType = OracleDbType.Decimal;
        _seisanHoho.DBType = OracleDbType.Char;
        _syuptDay.DBType = OracleDbType.Decimal;
        _taxKbn.DBType = OracleDbType.Char;
        _systemEntryPgmid.DBType = OracleDbType.Char;
        _systemEntryPersonCd.DBType = OracleDbType.Varchar2;
        _systemEntryDay.DBType = OracleDbType.Date;
        _systemUpdatePgmid.DBType = OracleDbType.Char;
        _systemUpdatePersonCd.DBType = OracleDbType.Varchar2;
        _systemUpdateDay.DBType = OracleDbType.Date;
        _carrierCd.IntegerBu = 4;
        _carrierEdaban.IntegerBu = 2;
        _classLineNo.IntegerBu = 1;
        _classWording.IntegerBu = 16;
        _com.IntegerBu = 3;
        _crsCd.IntegerBu = 6;
        _crsItineraryLineNo.IntegerBu = 2;
        _deleteDay.IntegerBu = 8;
        _gousya.IntegerBu = 3;
        _lineNo.IntegerBu = 3;
        _riyouDay.IntegerBu = 8;
        _seisanHoho.IntegerBu = 1;
        _syuptDay.IntegerBu = 8;
        _taxKbn.IntegerBu = 1;
        _systemEntryPgmid.IntegerBu = 8;
        _systemEntryPersonCd.IntegerBu = 20;
        _systemEntryDay.IntegerBu = 0;
        _systemUpdatePgmid.IntegerBu = 8;
        _systemUpdatePersonCd.IntegerBu = 20;
        _systemUpdateDay.IntegerBu = 0;
        _carrierCd.DecimalBu = 0;
        _carrierEdaban.DecimalBu = 0;
        _classLineNo.DecimalBu = 0;
        _classWording.DecimalBu = 0;
        _com.DecimalBu = 0;
        _crsCd.DecimalBu = 0;
        _crsItineraryLineNo.DecimalBu = 0;
        _deleteDay.DecimalBu = 0;
        _gousya.DecimalBu = 0;
        _lineNo.DecimalBu = 0;
        _riyouDay.DecimalBu = 0;
        _seisanHoho.DecimalBu = 0;
        _syuptDay.DecimalBu = 0;
        _taxKbn.DecimalBu = 0;
        _systemEntryPgmid.DecimalBu = 0;
        _systemEntryPersonCd.DecimalBu = 0;
        _systemEntryDay.DecimalBu = 0;
        _systemUpdatePgmid.DecimalBu = 0;
        _systemUpdatePersonCd.DecimalBu = 0;
        _systemUpdateDay.DecimalBu = 0;
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
/// classLineNo
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_NumberType classLineNo
    {
        get
        {
            return _classLineNo;
        }

        set
        {
            _classLineNo = value;
        }
    }


    /// <summary>
/// classWording
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_MojiType classWording
    {
        get
        {
            return _classWording;
        }

        set
        {
            _classWording = value;
        }
    }


    /// <summary>
/// com
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_NumberType com
    {
        get
        {
            return _com;
        }

        set
        {
            _com = value;
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
/// deleteDay
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_NumberType deleteDay
    {
        get
        {
            return _deleteDay;
        }

        set
        {
            _deleteDay = value;
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
/// riyouDay
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_NumberType riyouDay
    {
        get
        {
            return _riyouDay;
        }

        set
        {
            _riyouDay = value;
        }
    }


    /// <summary>
/// seisanHoho
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_MojiType seisanHoho
    {
        get
        {
            return _seisanHoho;
        }

        set
        {
            _seisanHoho = value;
        }
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
/// taxKbn
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_MojiType taxKbn
    {
        get
        {
            return _taxKbn;
        }

        set
        {
            _taxKbn = value;
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
}