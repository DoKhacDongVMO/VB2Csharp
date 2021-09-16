using System;
using Oracle.ManagedDataAccess.Client;


/// <summary>
/// コース台帳（料金）
/// </summary>
/// <remarks></remarks>
[Serializable()]
// コース台帳（料金）エンティティ
public partial class TCrsLedgerChargeEntity
{
    private EntityKoumoku_MojiType _chargeKbn = new EntityKoumoku_MojiType();	// 料金区分
    private EntityKoumoku_MojiType _crsCd = new EntityKoumoku_MojiType();	// コースコード
    private EntityKoumoku_NumberType _deleteDay = new EntityKoumoku_NumberType();	// 削除日
    private EntityKoumoku_NumberType _gousya = new EntityKoumoku_NumberType();	// 号車
    private EntityKoumoku_NumberType _syuptDay = new EntityKoumoku_NumberType();	// 出発日
    private EntityKoumoku_NumberType _kbnNo = new EntityKoumoku_NumberType();	// 区分No
    private EntityKoumoku_MojiType _tokuteiRyokinSet = new EntityKoumoku_MojiType();	// 特定料金設定
    private EntityKoumoku_MojiType _systemEntryPgmid = new EntityKoumoku_MojiType();	// システム登録ＰＧＭＩＤ
    private EntityKoumoku_MojiType _systemEntryPersonCd = new EntityKoumoku_MojiType();	// システム登録者コード
    private EntityKoumoku_YmdType _systemEntryDay = new EntityKoumoku_YmdType();	// システム登録日
    private EntityKoumoku_MojiType _systemUpdatePgmid = new EntityKoumoku_MojiType();	// システム更新ＰＧＭＩＤ
    private EntityKoumoku_MojiType _systemUpdatePersonCd = new EntityKoumoku_MojiType();	// システム更新者コード
    private EntityKoumoku_YmdType _systemUpdateDay = new EntityKoumoku_YmdType();	// システム更新日

    public TCrsLedgerChargeEntity()
    {
        _chargeKbn.PhysicsName = "CHARGE_KBN";
        _crsCd.PhysicsName = "CRS_CD";
        _deleteDay.PhysicsName = "DELETE_DAY";
        _gousya.PhysicsName = "GOUSYA";
        _syuptDay.PhysicsName = "SYUPT_DAY";
        _kbnNo.PhysicsName = "KBN_NO";
        _tokuteiRyokinSet.PhysicsName = "TOKUTEI_RYOKIN_SET";
        _systemEntryPgmid.PhysicsName = "SYSTEM_ENTRY_PGMID";
        _systemEntryPersonCd.PhysicsName = "SYSTEM_ENTRY_PERSON_CD";
        _systemEntryDay.PhysicsName = "SYSTEM_ENTRY_DAY";
        _systemUpdatePgmid.PhysicsName = "SYSTEM_UPDATE_PGMID";
        _systemUpdatePersonCd.PhysicsName = "SYSTEM_UPDATE_PERSON_CD";
        _systemUpdateDay.PhysicsName = "SYSTEM_UPDATE_DAY";
        _chargeKbn.Required = false;
        _crsCd.Required = false;
        _deleteDay.Required = false;
        _gousya.Required = false;
        _syuptDay.Required = false;
        _kbnNo.Required = false;
        _tokuteiRyokinSet.Required = false;
        _systemEntryPgmid.Required = true;
        _systemEntryPersonCd.Required = true;
        _systemEntryDay.Required = true;
        _systemUpdatePgmid.Required = true;
        _systemUpdatePersonCd.Required = true;
        _systemUpdateDay.Required = true;
        _chargeKbn.DBType = OracleDbType.Char;
        _crsCd.DBType = OracleDbType.Char;
        _deleteDay.DBType = OracleDbType.Decimal;
        _gousya.DBType = OracleDbType.Decimal;
        _syuptDay.DBType = OracleDbType.Decimal;
        _kbnNo.DBType = OracleDbType.Decimal;
        _tokuteiRyokinSet.DBType = OracleDbType.Char;
        _systemEntryPgmid.DBType = OracleDbType.Char;
        _systemEntryPersonCd.DBType = OracleDbType.Varchar2;
        _systemEntryDay.DBType = OracleDbType.Date;
        _systemUpdatePgmid.DBType = OracleDbType.Char;
        _systemUpdatePersonCd.DBType = OracleDbType.Varchar2;
        _systemUpdateDay.DBType = OracleDbType.Date;
        _chargeKbn.IntegerBu = 2;
        _crsCd.IntegerBu = 6;
        _deleteDay.IntegerBu = 8;
        _gousya.IntegerBu = 3;
        _syuptDay.IntegerBu = 8;
        _kbnNo.IntegerBu = 1;
        _tokuteiRyokinSet.IntegerBu = 1;
        _systemEntryPgmid.IntegerBu = 8;
        _systemEntryPersonCd.IntegerBu = 20;
        _systemEntryDay.IntegerBu = 0;
        _systemUpdatePgmid.IntegerBu = 8;
        _systemUpdatePersonCd.IntegerBu = 20;
        _systemUpdateDay.IntegerBu = 0;
        _chargeKbn.DecimalBu = 0;
        _crsCd.DecimalBu = 0;
        _deleteDay.DecimalBu = 0;
        _gousya.DecimalBu = 0;
        _syuptDay.DecimalBu = 0;
        _kbnNo.DecimalBu = 0;
        _tokuteiRyokinSet.DecimalBu = 0;
        _systemEntryPgmid.DecimalBu = 0;
        _systemEntryPersonCd.DecimalBu = 0;
        _systemEntryDay.DecimalBu = 0;
        _systemUpdatePgmid.DecimalBu = 0;
        _systemUpdatePersonCd.DecimalBu = 0;
        _systemUpdateDay.DecimalBu = 0;
    }


    /// <summary>
/// chargeKbn
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_MojiType chargeKbn
    {
        get
        {
            return _chargeKbn;
        }

        set
        {
            _chargeKbn = value;
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
/// kbnNo
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_NumberType kbnNo
    {
        get
        {
            return _kbnNo;
        }

        set
        {
            _kbnNo = value;
        }
    }


    /// <summary>
/// tokuteiRyokinSet
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_MojiType tokuteiRyokinSet
    {
        get
        {
            return _tokuteiRyokinSet;
        }

        set
        {
            _tokuteiRyokinSet = value;
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
}