using System;
using Oracle.ManagedDataAccess.Client;


/// <summary>
/// コース台帳原価（基本）
/// </summary>
/// <remarks></remarks>
[Serializable()]
// コース台帳原価（基本）エンティティ
public partial class TCrsLedgerCostBasicEntity
{
    private EntityKoumoku_MojiType _crsCd = new EntityKoumoku_MojiType();	// コースコード
    private EntityKoumoku_NumberType _deleteDay = new EntityKoumoku_NumberType();	// 削除日
    private EntityKoumoku_NumberType _gousya = new EntityKoumoku_NumberType();	// 号車
    private EntityKoumoku_NumberType _lastUpdateDay = new EntityKoumoku_NumberType();	// 最終更新日
    private EntityKoumoku_MojiType _lastUpdatePersonCd = new EntityKoumoku_MojiType();	// 最終更新者コード
    private EntityKoumoku_NumberType _lastUpdateTime = new EntityKoumoku_NumberType();	// 最終更新時刻
    private EntityKoumoku_MojiType _revUmuKbn = new EntityKoumoku_MojiType();	// 修正有無区分
    private EntityKoumoku_NumberType _syuptDay = new EntityKoumoku_NumberType();	// 出発日
    private EntityKoumoku_NumberType _sisanNinzu = new EntityKoumoku_NumberType();	// 試算人数
    private EntityKoumoku_MojiType _systemEntryPgmid = new EntityKoumoku_MojiType();	// システム登録ＰＧＭＩＤ
    private EntityKoumoku_MojiType _systemEntryPersonCd = new EntityKoumoku_MojiType();	// システム登録者コード
    private EntityKoumoku_YmdType _systemEntryDay = new EntityKoumoku_YmdType();	// システム登録日
    private EntityKoumoku_MojiType _systemUpdatePgmid = new EntityKoumoku_MojiType();	// システム更新ＰＧＭＩＤ
    private EntityKoumoku_MojiType _systemUpdatePersonCd = new EntityKoumoku_MojiType();	// システム更新者コード
    private EntityKoumoku_YmdType _systemUpdateDay = new EntityKoumoku_YmdType();	// システム更新日

    public TCrsLedgerCostBasicEntity()
    {
        _crsCd.PhysicsName = "CRS_CD";
        _deleteDay.PhysicsName = "DELETE_DAY";
        _gousya.PhysicsName = "GOUSYA";
        _lastUpdateDay.PhysicsName = "LAST_UPDATE_DAY";
        _lastUpdatePersonCd.PhysicsName = "LAST_UPDATE_PERSON_CD";
        _lastUpdateTime.PhysicsName = "LAST_UPDATE_TIME";
        _revUmuKbn.PhysicsName = "REV_UMU_KBN";
        _syuptDay.PhysicsName = "SYUPT_DAY";
        _sisanNinzu.PhysicsName = "SISAN_NINZU";
        _systemEntryPgmid.PhysicsName = "SYSTEM_ENTRY_PGMID";
        _systemEntryPersonCd.PhysicsName = "SYSTEM_ENTRY_PERSON_CD";
        _systemEntryDay.PhysicsName = "SYSTEM_ENTRY_DAY";
        _systemUpdatePgmid.PhysicsName = "SYSTEM_UPDATE_PGMID";
        _systemUpdatePersonCd.PhysicsName = "SYSTEM_UPDATE_PERSON_CD";
        _systemUpdateDay.PhysicsName = "SYSTEM_UPDATE_DAY";
        _crsCd.Required = false;
        _deleteDay.Required = false;
        _gousya.Required = false;
        _lastUpdateDay.Required = false;
        _lastUpdatePersonCd.Required = false;
        _lastUpdateTime.Required = false;
        _revUmuKbn.Required = false;
        _syuptDay.Required = false;
        _sisanNinzu.Required = false;
        _systemEntryPgmid.Required = true;
        _systemEntryPersonCd.Required = true;
        _systemEntryDay.Required = true;
        _systemUpdatePgmid.Required = true;
        _systemUpdatePersonCd.Required = true;
        _systemUpdateDay.Required = true;
        _crsCd.DBType = OracleDbType.Char;
        _deleteDay.DBType = OracleDbType.Decimal;
        _gousya.DBType = OracleDbType.Decimal;
        _lastUpdateDay.DBType = OracleDbType.Decimal;
        _lastUpdatePersonCd.DBType = OracleDbType.Varchar2;
        _lastUpdateTime.DBType = OracleDbType.Decimal;
        _revUmuKbn.DBType = OracleDbType.Char;
        _syuptDay.DBType = OracleDbType.Decimal;
        _sisanNinzu.DBType = OracleDbType.Decimal;
        _systemEntryPgmid.DBType = OracleDbType.Char;
        _systemEntryPersonCd.DBType = OracleDbType.Varchar2;
        _systemEntryDay.DBType = OracleDbType.Date;
        _systemUpdatePgmid.DBType = OracleDbType.Char;
        _systemUpdatePersonCd.DBType = OracleDbType.Varchar2;
        _systemUpdateDay.DBType = OracleDbType.Date;
        _crsCd.IntegerBu = 6;
        _deleteDay.IntegerBu = 8;
        _gousya.IntegerBu = 3;
        _lastUpdateDay.IntegerBu = 8;
        _lastUpdatePersonCd.IntegerBu = 20;
        _lastUpdateTime.IntegerBu = 6;
        _revUmuKbn.IntegerBu = 1;
        _syuptDay.IntegerBu = 8;
        _sisanNinzu.IntegerBu = 3;
        _systemEntryPgmid.IntegerBu = 8;
        _systemEntryPersonCd.IntegerBu = 20;
        _systemEntryDay.IntegerBu = 0;
        _systemUpdatePgmid.IntegerBu = 8;
        _systemUpdatePersonCd.IntegerBu = 20;
        _systemUpdateDay.IntegerBu = 0;
        _crsCd.DecimalBu = 0;
        _deleteDay.DecimalBu = 0;
        _gousya.DecimalBu = 0;
        _lastUpdateDay.DecimalBu = 0;
        _lastUpdatePersonCd.DecimalBu = 0;
        _lastUpdateTime.DecimalBu = 0;
        _revUmuKbn.DecimalBu = 0;
        _syuptDay.DecimalBu = 0;
        _sisanNinzu.DecimalBu = 0;
        _systemEntryPgmid.DecimalBu = 0;
        _systemEntryPersonCd.DecimalBu = 0;
        _systemEntryDay.DecimalBu = 0;
        _systemUpdatePgmid.DecimalBu = 0;
        _systemUpdatePersonCd.DecimalBu = 0;
        _systemUpdateDay.DecimalBu = 0;
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
/// lastUpdateDay
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_NumberType lastUpdateDay
    {
        get
        {
            return _lastUpdateDay;
        }

        set
        {
            _lastUpdateDay = value;
        }
    }


    /// <summary>
/// lastUpdatePersonCd
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_MojiType lastUpdatePersonCd
    {
        get
        {
            return _lastUpdatePersonCd;
        }

        set
        {
            _lastUpdatePersonCd = value;
        }
    }


    /// <summary>
/// lastUpdateTime
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_NumberType lastUpdateTime
    {
        get
        {
            return _lastUpdateTime;
        }

        set
        {
            _lastUpdateTime = value;
        }
    }


    /// <summary>
/// revUmuKbn
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_MojiType revUmuKbn
    {
        get
        {
            return _revUmuKbn;
        }

        set
        {
            _revUmuKbn = value;
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
/// sisanNinzu
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_NumberType sisanNinzu
    {
        get
        {
            return _sisanNinzu;
        }

        set
        {
            _sisanNinzu = value;
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