using System;
using Oracle.ManagedDataAccess.Client;


/// <summary>
/// コース台帳原価（降車ヶ所）
/// </summary>
/// <remarks></remarks>
[Serializable()]
// コース台帳原価（降車ヶ所）エンティティ
public partial class TCrsLedgerCostKoshakashoEntity
{
    private EntityKoumoku_NumberType _com = new EntityKoumoku_NumberType();	// ＣＯＭ
    private EntityKoumoku_MojiType _crsCd = new EntityKoumoku_MojiType();	// コースコード
    private EntityKoumoku_NumberType _crsItineraryLineNo = new EntityKoumoku_NumberType();	// コース行程行ＮＯ
    private EntityKoumoku_NumberType _deleteDay = new EntityKoumoku_NumberType();	// 削除日
    private EntityKoumoku_NumberType _gousya = new EntityKoumoku_NumberType();	// 号車
    private EntityKoumoku_NumberType _lineNo = new EntityKoumoku_NumberType();	// 行ＮＯ
    private EntityKoumoku_NumberType _riyouDay = new EntityKoumoku_NumberType();	// 利用日
    private EntityKoumoku_MojiType _seisanHoho = new EntityKoumoku_MojiType();	// 精算方法
    private EntityKoumoku_MojiType _siireSakiCd = new EntityKoumoku_MojiType();	// 仕入先コード
    private EntityKoumoku_MojiType _siireSakiEdaban = new EntityKoumoku_MojiType();	// 仕入先枝番
    private EntityKoumoku_MojiType _sokyakFeeCalcHohoKbn = new EntityKoumoku_MojiType();	// 送客手数料計算方法区分
    private EntityKoumoku_MojiType _sokyakFeeGenkinPaymentKbn = new EntityKoumoku_MojiType();	// 送客手数料現金払い区分
    private EntityKoumoku_NumberType _sokyakFeeTankaOrPer = new EntityKoumoku_NumberType();	// 送客手数料  単価又は率
    private EntityKoumoku_NumberType _syuptDay = new EntityKoumoku_NumberType();	// 出発日
    private EntityKoumoku_MojiType _taxKbn = new EntityKoumoku_MojiType();	// 税区分
    private EntityKoumoku_MojiType _teikyubiKbn = new EntityKoumoku_MojiType();	// 定休日区分
    private EntityKoumoku_MojiType _busTani = new EntityKoumoku_MojiType();	// バス単位
    private EntityKoumoku_MojiType _systemEntryPgmid = new EntityKoumoku_MojiType();	// システム登録ＰＧＭＩＤ
    private EntityKoumoku_MojiType _systemEntryPersonCd = new EntityKoumoku_MojiType();	// システム登録者コード
    private EntityKoumoku_YmdType _systemEntryDay = new EntityKoumoku_YmdType();	// システム登録日
    private EntityKoumoku_MojiType _systemUpdatePgmid = new EntityKoumoku_MojiType();	// システム更新ＰＧＭＩＤ
    private EntityKoumoku_MojiType _systemUpdatePersonCd = new EntityKoumoku_MojiType();	// システム更新者コード
    private EntityKoumoku_YmdType _systemUpdateDay = new EntityKoumoku_YmdType();	// システム更新日

    public TCrsLedgerCostKoshakashoEntity()
    {
        _com.PhysicsName = "COM";
        _crsCd.PhysicsName = "CRS_CD";
        _crsItineraryLineNo.PhysicsName = "CRS_ITINERARY_LINE_NO";
        _deleteDay.PhysicsName = "DELETE_DAY";
        _gousya.PhysicsName = "GOUSYA";
        _lineNo.PhysicsName = "LINE_NO";
        _riyouDay.PhysicsName = "RIYOU_DAY";
        _seisanHoho.PhysicsName = "SEISAN_HOHO";
        _siireSakiCd.PhysicsName = "SIIRE_SAKI_CD";
        _siireSakiEdaban.PhysicsName = "SIIRE_SAKI_EDABAN";
        _sokyakFeeCalcHohoKbn.PhysicsName = "SOKYAK_FEE_CALC_HOHO_KBN";
        _sokyakFeeGenkinPaymentKbn.PhysicsName = "SOKYAK_FEE_GENKIN_PAYMENT_KBN";
        _sokyakFeeTankaOrPer.PhysicsName = "SOKYAK_FEE_TANKA_OR_PER";
        _syuptDay.PhysicsName = "SYUPT_DAY";
        _taxKbn.PhysicsName = "TAX_KBN";
        _teikyubiKbn.PhysicsName = "TEIKYUBI_KBN";
        _busTani.PhysicsName = "BUS_TANI";
        _systemEntryPgmid.PhysicsName = "SYSTEM_ENTRY_PGMID";
        _systemEntryPersonCd.PhysicsName = "SYSTEM_ENTRY_PERSON_CD";
        _systemEntryDay.PhysicsName = "SYSTEM_ENTRY_DAY";
        _systemUpdatePgmid.PhysicsName = "SYSTEM_UPDATE_PGMID";
        _systemUpdatePersonCd.PhysicsName = "SYSTEM_UPDATE_PERSON_CD";
        _systemUpdateDay.PhysicsName = "SYSTEM_UPDATE_DAY";
        _com.Required = false;
        _crsCd.Required = false;
        _crsItineraryLineNo.Required = false;
        _deleteDay.Required = false;
        _gousya.Required = false;
        _lineNo.Required = false;
        _riyouDay.Required = false;
        _seisanHoho.Required = false;
        _siireSakiCd.Required = false;
        _siireSakiEdaban.Required = false;
        _sokyakFeeCalcHohoKbn.Required = false;
        _sokyakFeeGenkinPaymentKbn.Required = false;
        _sokyakFeeTankaOrPer.Required = false;
        _syuptDay.Required = false;
        _taxKbn.Required = false;
        _teikyubiKbn.Required = false;
        _busTani.Required = false;
        _systemEntryPgmid.Required = true;
        _systemEntryPersonCd.Required = true;
        _systemEntryDay.Required = true;
        _systemUpdatePgmid.Required = true;
        _systemUpdatePersonCd.Required = true;
        _systemUpdateDay.Required = true;
        _com.DBType = OracleDbType.Decimal;
        _crsCd.DBType = OracleDbType.Char;
        _crsItineraryLineNo.DBType = OracleDbType.Decimal;
        _deleteDay.DBType = OracleDbType.Decimal;
        _gousya.DBType = OracleDbType.Decimal;
        _lineNo.DBType = OracleDbType.Decimal;
        _riyouDay.DBType = OracleDbType.Decimal;
        _seisanHoho.DBType = OracleDbType.Char;
        _siireSakiCd.DBType = OracleDbType.Char;
        _siireSakiEdaban.DBType = OracleDbType.Char;
        _sokyakFeeCalcHohoKbn.DBType = OracleDbType.Char;
        _sokyakFeeGenkinPaymentKbn.DBType = OracleDbType.Char;
        _sokyakFeeTankaOrPer.DBType = OracleDbType.Decimal;
        _syuptDay.DBType = OracleDbType.Decimal;
        _taxKbn.DBType = OracleDbType.Char;
        _teikyubiKbn.DBType = OracleDbType.Char;
        _busTani.DBType = OracleDbType.Char;
        _systemEntryPgmid.DBType = OracleDbType.Char;
        _systemEntryPersonCd.DBType = OracleDbType.Varchar2;
        _systemEntryDay.DBType = OracleDbType.Date;
        _systemUpdatePgmid.DBType = OracleDbType.Char;
        _systemUpdatePersonCd.DBType = OracleDbType.Varchar2;
        _systemUpdateDay.DBType = OracleDbType.Date;
        _com.IntegerBu = 3;
        _crsCd.IntegerBu = 6;
        _crsItineraryLineNo.IntegerBu = 2;
        _deleteDay.IntegerBu = 8;
        _gousya.IntegerBu = 3;
        _lineNo.IntegerBu = 3;
        _riyouDay.IntegerBu = 8;
        _seisanHoho.IntegerBu = 1;
        _siireSakiCd.IntegerBu = 4;
        _siireSakiEdaban.IntegerBu = 2;
        _sokyakFeeCalcHohoKbn.IntegerBu = 1;
        _sokyakFeeGenkinPaymentKbn.IntegerBu = 1;
        _sokyakFeeTankaOrPer.IntegerBu = 9;
        _syuptDay.IntegerBu = 8;
        _taxKbn.IntegerBu = 1;
        _teikyubiKbn.IntegerBu = 1;
        _busTani.IntegerBu = 1;
        _systemEntryPgmid.IntegerBu = 8;
        _systemEntryPersonCd.IntegerBu = 20;
        _systemEntryDay.IntegerBu = 0;
        _systemUpdatePgmid.IntegerBu = 8;
        _systemUpdatePersonCd.IntegerBu = 20;
        _systemUpdateDay.IntegerBu = 0;
        _com.DecimalBu = 0;
        _crsCd.DecimalBu = 0;
        _crsItineraryLineNo.DecimalBu = 0;
        _deleteDay.DecimalBu = 0;
        _gousya.DecimalBu = 0;
        _lineNo.DecimalBu = 0;
        _riyouDay.DecimalBu = 0;
        _seisanHoho.DecimalBu = 0;
        _siireSakiCd.DecimalBu = 0;
        _siireSakiEdaban.DecimalBu = 0;
        _sokyakFeeCalcHohoKbn.DecimalBu = 0;
        _sokyakFeeGenkinPaymentKbn.DecimalBu = 0;
        _sokyakFeeTankaOrPer.DecimalBu = 0;
        _syuptDay.DecimalBu = 0;
        _taxKbn.DecimalBu = 0;
        _teikyubiKbn.DecimalBu = 0;
        _busTani.DecimalBu = 0;
        _systemEntryPgmid.DecimalBu = 0;
        _systemEntryPersonCd.DecimalBu = 0;
        _systemEntryDay.DecimalBu = 0;
        _systemUpdatePgmid.DecimalBu = 0;
        _systemUpdatePersonCd.DecimalBu = 0;
        _systemUpdateDay.DecimalBu = 0;
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
/// sokyakFeeCalcHohoKbn
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_MojiType sokyakFeeCalcHohoKbn
    {
        get
        {
            return _sokyakFeeCalcHohoKbn;
        }

        set
        {
            _sokyakFeeCalcHohoKbn = value;
        }
    }


    /// <summary>
/// sokyakFeeGenkinPaymentKbn
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_MojiType sokyakFeeGenkinPaymentKbn
    {
        get
        {
            return _sokyakFeeGenkinPaymentKbn;
        }

        set
        {
            _sokyakFeeGenkinPaymentKbn = value;
        }
    }


    /// <summary>
/// sokyakFeeTankaOrPer
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_NumberType sokyakFeeTankaOrPer
    {
        get
        {
            return _sokyakFeeTankaOrPer;
        }

        set
        {
            _sokyakFeeTankaOrPer = value;
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
/// teikyubiKbn
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_MojiType teikyubiKbn
    {
        get
        {
            return _teikyubiKbn;
        }

        set
        {
            _teikyubiKbn = value;
        }
    }


    /// <summary>
/// busTani
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_MojiType busTani
    {
        get
        {
            return _busTani;
        }

        set
        {
            _busTani = value;
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