using System;
using Oracle.ManagedDataAccess.Client;


/// <summary>
/// 座席番号マスタ
/// </summary>
/// <remarks></remarks>
[Serializable()]
// 座席番号マスタエンティティ
public partial class MZasekiNoEntity
{
    private EntityKoumoku_MojiType _carCd = new EntityKoumoku_MojiType();	// 車種コード
    private EntityKoumoku_NumberType _zasekiKai = new EntityKoumoku_NumberType();	// 座席階
    private EntityKoumoku_NumberType _zasekiLine = new EntityKoumoku_NumberType();	// 座席行
    private EntityKoumoku_NumberType _zasekiCol = new EntityKoumoku_NumberType();	// 座席列
    private EntityKoumoku_MojiType _zasekiName = new EntityKoumoku_MojiType();	// 座席名
    private EntityKoumoku_MojiType _zasekiKind = new EntityKoumoku_MojiType();	// 座席種別
    private EntityKoumoku_MojiType _systemEntryPgmid = new EntityKoumoku_MojiType();	// システム登録ＰＧＭＩＤ
    private EntityKoumoku_MojiType _systemEntryPersonCd = new EntityKoumoku_MojiType();	// システム登録者コード
    private EntityKoumoku_YmdType _systemEntryDay = new EntityKoumoku_YmdType();	// システム登録日
    private EntityKoumoku_MojiType _systemUpdatePgmid = new EntityKoumoku_MojiType();	// システム更新ＰＧＭＩＤ
    private EntityKoumoku_MojiType _systemUpdatePersonCd = new EntityKoumoku_MojiType();	// システム更新者コード
    private EntityKoumoku_YmdType _systemUpdateDay = new EntityKoumoku_YmdType();	// システム更新日

    public MZasekiNoEntity()
    {
        _carCd.PhysicsName = "CAR_CD";
        _zasekiKai.PhysicsName = "ZASEKI_KAI";
        _zasekiLine.PhysicsName = "ZASEKI_LINE";
        _zasekiCol.PhysicsName = "ZASEKI_COL";
        _zasekiName.PhysicsName = "ZASEKI_NAME";
        _zasekiKind.PhysicsName = "ZASEKI_KIND";
        _systemEntryPgmid.PhysicsName = "SYSTEM_ENTRY_PGMID";
        _systemEntryPersonCd.PhysicsName = "SYSTEM_ENTRY_PERSON_CD";
        _systemEntryDay.PhysicsName = "SYSTEM_ENTRY_DAY";
        _systemUpdatePgmid.PhysicsName = "SYSTEM_UPDATE_PGMID";
        _systemUpdatePersonCd.PhysicsName = "SYSTEM_UPDATE_PERSON_CD";
        _systemUpdateDay.PhysicsName = "SYSTEM_UPDATE_DAY";
        _carCd.Required = false;
        _zasekiKai.Required = false;
        _zasekiLine.Required = false;
        _zasekiCol.Required = false;
        _zasekiName.Required = false;
        _zasekiKind.Required = false;
        _systemEntryPgmid.Required = true;
        _systemEntryPersonCd.Required = true;
        _systemEntryDay.Required = true;
        _systemUpdatePgmid.Required = true;
        _systemUpdatePersonCd.Required = true;
        _systemUpdateDay.Required = true;
        _carCd.DBType = OracleDbType.Char;
        _zasekiKai.DBType = OracleDbType.Decimal;
        _zasekiLine.DBType = OracleDbType.Decimal;
        _zasekiCol.DBType = OracleDbType.Decimal;
        _zasekiName.DBType = OracleDbType.Varchar2;
        _zasekiKind.DBType = OracleDbType.Char;
        _systemEntryPgmid.DBType = OracleDbType.Char;
        _systemEntryPersonCd.DBType = OracleDbType.Varchar2;
        _systemEntryDay.DBType = OracleDbType.Date;
        _systemUpdatePgmid.DBType = OracleDbType.Char;
        _systemUpdatePersonCd.DBType = OracleDbType.Varchar2;
        _systemUpdateDay.DBType = OracleDbType.Date;
        _carCd.IntegerBu = 2;
        _zasekiKai.IntegerBu = 1;
        _zasekiLine.IntegerBu = 2;
        _zasekiCol.IntegerBu = 2;
        _zasekiName.IntegerBu = 3;
        _zasekiKind.IntegerBu = 1;
        _systemEntryPgmid.IntegerBu = 8;
        _systemEntryPersonCd.IntegerBu = 20;
        _systemEntryDay.IntegerBu = 0;
        _systemUpdatePgmid.IntegerBu = 8;
        _systemUpdatePersonCd.IntegerBu = 20;
        _systemUpdateDay.IntegerBu = 0;
        _carCd.DecimalBu = 0;
        _zasekiKai.DecimalBu = 0;
        _zasekiLine.DecimalBu = 0;
        _zasekiCol.DecimalBu = 0;
        _zasekiName.DecimalBu = 0;
        _zasekiKind.DecimalBu = 0;
        _systemEntryPgmid.DecimalBu = 0;
        _systemEntryPersonCd.DecimalBu = 0;
        _systemEntryDay.DecimalBu = 0;
        _systemUpdatePgmid.DecimalBu = 0;
        _systemUpdatePersonCd.DecimalBu = 0;
        _systemUpdateDay.DecimalBu = 0;
    }


    /// <summary>
/// carCd
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_MojiType carCd
    {
        get
        {
            return _carCd;
        }

        set
        {
            _carCd = value;
        }
    }


    /// <summary>
/// zasekiKai
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_NumberType zasekiKai
    {
        get
        {
            return _zasekiKai;
        }

        set
        {
            _zasekiKai = value;
        }
    }


    /// <summary>
/// zasekiLine
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_NumberType zasekiLine
    {
        get
        {
            return _zasekiLine;
        }

        set
        {
            _zasekiLine = value;
        }
    }


    /// <summary>
/// zasekiCol
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_NumberType zasekiCol
    {
        get
        {
            return _zasekiCol;
        }

        set
        {
            _zasekiCol = value;
        }
    }


    /// <summary>
/// zasekiName
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_MojiType zasekiName
    {
        get
        {
            return _zasekiName;
        }

        set
        {
            _zasekiName = value;
        }
    }


    /// <summary>
/// zasekiKind
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_MojiType zasekiKind
    {
        get
        {
            return _zasekiKind;
        }

        set
        {
            _zasekiKind = value;
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