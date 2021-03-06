using System;
using Oracle.ManagedDataAccess.Client;


/// <summary>
/// コース台帳原価（プレート）
/// </summary>
/// <remarks></remarks>
[Serializable()]
// コース台帳原価（プレート）エンティティ
public partial class TCrsLedgerCostPlateEntity
{
    private EntityKoumoku_MojiType _crsCd = new EntityKoumoku_MojiType();	// コースコード
    private EntityKoumoku_NumberType _deleteDay = new EntityKoumoku_NumberType();	// 削除日
    private EntityKoumoku_NumberType _gousya = new EntityKoumoku_NumberType();	// 号車
    private EntityKoumoku_NumberType _kingaku = new EntityKoumoku_NumberType();	// 金額
    private EntityKoumoku_MojiType _kukanFrom = new EntityKoumoku_MojiType();	// 区間自
    private EntityKoumoku_MojiType _kukanTo = new EntityKoumoku_MojiType();	// 区間至
    private EntityKoumoku_NumberType _lineNo = new EntityKoumoku_NumberType();	// 行ＮＯ
    private EntityKoumoku_NumberType _syuptDay = new EntityKoumoku_NumberType();	// 出発日
    private EntityKoumoku_MojiType _systemEntryPgmid = new EntityKoumoku_MojiType();	// システム登録ＰＧＭＩＤ
    private EntityKoumoku_MojiType _systemEntryPersonCd = new EntityKoumoku_MojiType();	// システム登録者コード
    private EntityKoumoku_YmdType _systemEntryDay = new EntityKoumoku_YmdType();	// システム登録日
    private EntityKoumoku_MojiType _systemUpdatePgmid = new EntityKoumoku_MojiType();	// システム更新ＰＧＭＩＤ
    private EntityKoumoku_MojiType _systemUpdatePersonCd = new EntityKoumoku_MojiType();	// システム更新者コード
    private EntityKoumoku_YmdType _systemUpdateDay = new EntityKoumoku_YmdType();	// システム更新日

    public TCrsLedgerCostPlateEntity()
    {
        _crsCd.PhysicsName = "CRS_CD";
        _deleteDay.PhysicsName = "DELETE_DAY";
        _gousya.PhysicsName = "GOUSYA";
        _kingaku.PhysicsName = "KINGAKU";
        _kukanFrom.PhysicsName = "KUKAN_FROM";
        _kukanTo.PhysicsName = "KUKAN_TO";
        _lineNo.PhysicsName = "LINE_NO";
        _syuptDay.PhysicsName = "SYUPT_DAY";
        _systemEntryPgmid.PhysicsName = "SYSTEM_ENTRY_PGMID";
        _systemEntryPersonCd.PhysicsName = "SYSTEM_ENTRY_PERSON_CD";
        _systemEntryDay.PhysicsName = "SYSTEM_ENTRY_DAY";
        _systemUpdatePgmid.PhysicsName = "SYSTEM_UPDATE_PGMID";
        _systemUpdatePersonCd.PhysicsName = "SYSTEM_UPDATE_PERSON_CD";
        _systemUpdateDay.PhysicsName = "SYSTEM_UPDATE_DAY";
        _crsCd.Required = false;
        _deleteDay.Required = false;
        _gousya.Required = false;
        _kingaku.Required = false;
        _kukanFrom.Required = false;
        _kukanTo.Required = false;
        _lineNo.Required = false;
        _syuptDay.Required = false;
        _systemEntryPgmid.Required = true;
        _systemEntryPersonCd.Required = true;
        _systemEntryDay.Required = true;
        _systemUpdatePgmid.Required = true;
        _systemUpdatePersonCd.Required = true;
        _systemUpdateDay.Required = true;
        _crsCd.DBType = OracleDbType.Char;
        _deleteDay.DBType = OracleDbType.Decimal;
        _gousya.DBType = OracleDbType.Decimal;
        _kingaku.DBType = OracleDbType.Decimal;
        _kukanFrom.DBType = OracleDbType.Varchar2;
        _kukanTo.DBType = OracleDbType.Varchar2;
        _lineNo.DBType = OracleDbType.Decimal;
        _syuptDay.DBType = OracleDbType.Decimal;
        _systemEntryPgmid.DBType = OracleDbType.Char;
        _systemEntryPersonCd.DBType = OracleDbType.Varchar2;
        _systemEntryDay.DBType = OracleDbType.Date;
        _systemUpdatePgmid.DBType = OracleDbType.Char;
        _systemUpdatePersonCd.DBType = OracleDbType.Varchar2;
        _systemUpdateDay.DBType = OracleDbType.Date;
        _crsCd.IntegerBu = 6;
        _deleteDay.IntegerBu = 8;
        _gousya.IntegerBu = 3;
        _kingaku.IntegerBu = 7;
        _kukanFrom.IntegerBu = 12;
        _kukanTo.IntegerBu = 12;
        _lineNo.IntegerBu = 2;
        _syuptDay.IntegerBu = 8;
        _systemEntryPgmid.IntegerBu = 8;
        _systemEntryPersonCd.IntegerBu = 20;
        _systemEntryDay.IntegerBu = 0;
        _systemUpdatePgmid.IntegerBu = 8;
        _systemUpdatePersonCd.IntegerBu = 20;
        _systemUpdateDay.IntegerBu = 0;
        _crsCd.DecimalBu = 0;
        _deleteDay.DecimalBu = 0;
        _gousya.DecimalBu = 0;
        _kingaku.DecimalBu = 0;
        _kukanFrom.DecimalBu = 0;
        _kukanTo.DecimalBu = 0;
        _lineNo.DecimalBu = 0;
        _syuptDay.DecimalBu = 0;
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
/// kingaku
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_NumberType kingaku
    {
        get
        {
            return _kingaku;
        }

        set
        {
            _kingaku = value;
        }
    }


    /// <summary>
/// kukanFrom
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_MojiType kukanFrom
    {
        get
        {
            return _kukanFrom;
        }

        set
        {
            _kukanFrom = value;
        }
    }


    /// <summary>
/// kukanTo
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_MojiType kukanTo
    {
        get
        {
            return _kukanTo;
        }

        set
        {
            _kukanTo = value;
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