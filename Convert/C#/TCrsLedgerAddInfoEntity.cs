using System;
using Oracle.ManagedDataAccess.Client;


/// <summary>
/// コース台帳（付加情報）
/// </summary>
/// <remarks></remarks>
[Serializable()]
// コース台帳（付加情報）エンティティ
public partial class TCrsLedgerAddInfoEntity
{
    private EntityKoumoku_MojiType _controlFlg1 = new EntityKoumoku_MojiType();	// 制御フラグ０１
    private EntityKoumoku_MojiType _controlFlg10 = new EntityKoumoku_MojiType();	// 制御フラグ１０
    private EntityKoumoku_MojiType _controlFlg2 = new EntityKoumoku_MojiType();	// 制御フラグ０２
    private EntityKoumoku_MojiType _controlFlg3 = new EntityKoumoku_MojiType();	// 制御フラグ０３
    private EntityKoumoku_MojiType _controlFlg4 = new EntityKoumoku_MojiType();	// 制御フラグ０４
    private EntityKoumoku_MojiType _controlFlg5 = new EntityKoumoku_MojiType();	// 制御フラグ０５
    private EntityKoumoku_MojiType _controlFlg6 = new EntityKoumoku_MojiType();	// 制御フラグ０６
    private EntityKoumoku_MojiType _controlFlg7 = new EntityKoumoku_MojiType();	// 制御フラグ０７
    private EntityKoumoku_MojiType _controlFlg8 = new EntityKoumoku_MojiType();	// 制御フラグ０８
    private EntityKoumoku_MojiType _controlFlg9 = new EntityKoumoku_MojiType();	// 制御フラグ０９
    private EntityKoumoku_MojiType _crsCd = new EntityKoumoku_MojiType();	// コースコード
    private EntityKoumoku_NumberType _day1 = new EntityKoumoku_NumberType();	// 日１
    private EntityKoumoku_NumberType _day2 = new EntityKoumoku_NumberType();	// 日２
    private EntityKoumoku_NumberType _day3 = new EntityKoumoku_NumberType();	// 日３
    private EntityKoumoku_NumberType _day4 = new EntityKoumoku_NumberType();	// 日４
    private EntityKoumoku_NumberType _day5 = new EntityKoumoku_NumberType();	// 日５
    private EntityKoumoku_NumberType _deleteDay = new EntityKoumoku_NumberType();	// 削除日
    private EntityKoumoku_NumberType _gousya = new EntityKoumoku_NumberType();	// 号車
    private EntityKoumoku_NumberType _syuptDay = new EntityKoumoku_NumberType();	// 出発日
    private EntityKoumoku_MojiType _text1 = new EntityKoumoku_MojiType();	// テキスト１
    private EntityKoumoku_MojiType _text2 = new EntityKoumoku_MojiType();	// テキスト２
    private EntityKoumoku_MojiType _text3 = new EntityKoumoku_MojiType();	// テキスト３
    private EntityKoumoku_MojiType _text4 = new EntityKoumoku_MojiType();	// テキスト４
    private EntityKoumoku_MojiType _text5 = new EntityKoumoku_MojiType();	// テキスト５
    private EntityKoumoku_MojiType _uketomeKbnOne1r = new EntityKoumoku_MojiType();	// 受止区分１名１Ｒ
    private EntityKoumoku_MojiType _uketomeKbnThree1r = new EntityKoumoku_MojiType();	// 受止区分３名１Ｒ
    private EntityKoumoku_MojiType _uketomeKbnTwo1r = new EntityKoumoku_MojiType();	// 受止区分２名１Ｒ
    private EntityKoumoku_MojiType _uketomeKbnFour1r = new EntityKoumoku_MojiType();	// 受止区分４名１Ｒ
    private EntityKoumoku_MojiType _uketomeKbnFive1r = new EntityKoumoku_MojiType();	// 受止区分５名１Ｒ
    private EntityKoumoku_MojiType _systemEntryPgmid = new EntityKoumoku_MojiType();	// システム登録ＰＧＭＩＤ
    private EntityKoumoku_MojiType _systemEntryPersonCd = new EntityKoumoku_MojiType();	// システム登録者コード
    private EntityKoumoku_YmdType _systemEntryDay = new EntityKoumoku_YmdType();	// システム登録日
    private EntityKoumoku_MojiType _systemUpdatePgmid = new EntityKoumoku_MojiType();	// システム更新ＰＧＭＩＤ
    private EntityKoumoku_MojiType _systemUpdatePersonCd = new EntityKoumoku_MojiType();	// システム更新者コード
    private EntityKoumoku_YmdType _systemUpdateDay = new EntityKoumoku_YmdType();	// システム更新日

    public TCrsLedgerAddInfoEntity()
    {
        _controlFlg1.PhysicsName = "CONTROL_FLG_1";
        _controlFlg10.PhysicsName = "CONTROL_FLG_10";
        _controlFlg2.PhysicsName = "CONTROL_FLG_2";
        _controlFlg3.PhysicsName = "CONTROL_FLG_3";
        _controlFlg4.PhysicsName = "CONTROL_FLG_4";
        _controlFlg5.PhysicsName = "CONTROL_FLG_5";
        _controlFlg6.PhysicsName = "CONTROL_FLG_6";
        _controlFlg7.PhysicsName = "CONTROL_FLG_7";
        _controlFlg8.PhysicsName = "CONTROL_FLG_8";
        _controlFlg9.PhysicsName = "CONTROL_FLG_9";
        _crsCd.PhysicsName = "CRS_CD";
        _day1.PhysicsName = "DAY_1";
        _day2.PhysicsName = "DAY_2";
        _day3.PhysicsName = "DAY_3";
        _day4.PhysicsName = "DAY_4";
        _day5.PhysicsName = "DAY_5";
        _deleteDay.PhysicsName = "DELETE_DAY";
        _gousya.PhysicsName = "GOUSYA";
        _syuptDay.PhysicsName = "SYUPT_DAY";
        _text1.PhysicsName = "TEXT_1";
        _text2.PhysicsName = "TEXT_2";
        _text3.PhysicsName = "TEXT_3";
        _text4.PhysicsName = "TEXT_4";
        _text5.PhysicsName = "TEXT_5";
        _uketomeKbnOne1r.PhysicsName = "UKETOME_KBN_ONE_1R";
        _uketomeKbnThree1r.PhysicsName = "UKETOME_KBN_THREE_1R";
        _uketomeKbnTwo1r.PhysicsName = "UKETOME_KBN_TWO_1R";
        _uketomeKbnFour1r.PhysicsName = "UKETOME_KBN_FOUR_1R";
        _uketomeKbnFive1r.PhysicsName = "UKETOME_KBN_FIVE_1R";
        _systemEntryPgmid.PhysicsName = "SYSTEM_ENTRY_PGMID";
        _systemEntryPersonCd.PhysicsName = "SYSTEM_ENTRY_PERSON_CD";
        _systemEntryDay.PhysicsName = "SYSTEM_ENTRY_DAY";
        _systemUpdatePgmid.PhysicsName = "SYSTEM_UPDATE_PGMID";
        _systemUpdatePersonCd.PhysicsName = "SYSTEM_UPDATE_PERSON_CD";
        _systemUpdateDay.PhysicsName = "SYSTEM_UPDATE_DAY";
        _controlFlg1.Required = false;
        _controlFlg10.Required = false;
        _controlFlg2.Required = false;
        _controlFlg3.Required = false;
        _controlFlg4.Required = false;
        _controlFlg5.Required = false;
        _controlFlg6.Required = false;
        _controlFlg7.Required = false;
        _controlFlg8.Required = false;
        _controlFlg9.Required = false;
        _crsCd.Required = false;
        _day1.Required = false;
        _day2.Required = false;
        _day3.Required = false;
        _day4.Required = false;
        _day5.Required = false;
        _deleteDay.Required = false;
        _gousya.Required = false;
        _syuptDay.Required = false;
        _text1.Required = false;
        _text2.Required = false;
        _text3.Required = false;
        _text4.Required = false;
        _text5.Required = false;
        _uketomeKbnOne1r.Required = false;
        _uketomeKbnThree1r.Required = false;
        _uketomeKbnTwo1r.Required = false;
        _uketomeKbnFour1r.Required = false;
        _uketomeKbnFive1r.Required = false;
        _systemEntryPgmid.Required = true;
        _systemEntryPersonCd.Required = true;
        _systemEntryDay.Required = true;
        _systemUpdatePgmid.Required = true;
        _systemUpdatePersonCd.Required = true;
        _systemUpdateDay.Required = true;
        _controlFlg1.DBType = OracleDbType.Char;
        _controlFlg10.DBType = OracleDbType.Char;
        _controlFlg2.DBType = OracleDbType.Char;
        _controlFlg3.DBType = OracleDbType.Char;
        _controlFlg4.DBType = OracleDbType.Char;
        _controlFlg5.DBType = OracleDbType.Char;
        _controlFlg6.DBType = OracleDbType.Char;
        _controlFlg7.DBType = OracleDbType.Char;
        _controlFlg8.DBType = OracleDbType.Char;
        _controlFlg9.DBType = OracleDbType.Char;
        _crsCd.DBType = OracleDbType.Char;
        _day1.DBType = OracleDbType.Decimal;
        _day2.DBType = OracleDbType.Decimal;
        _day3.DBType = OracleDbType.Decimal;
        _day4.DBType = OracleDbType.Decimal;
        _day5.DBType = OracleDbType.Decimal;
        _deleteDay.DBType = OracleDbType.Decimal;
        _gousya.DBType = OracleDbType.Decimal;
        _syuptDay.DBType = OracleDbType.Decimal;
        _text1.DBType = OracleDbType.Varchar2;
        _text2.DBType = OracleDbType.Varchar2;
        _text3.DBType = OracleDbType.Varchar2;
        _text4.DBType = OracleDbType.Varchar2;
        _text5.DBType = OracleDbType.Varchar2;
        _uketomeKbnOne1r.DBType = OracleDbType.Char;
        _uketomeKbnThree1r.DBType = OracleDbType.Char;
        _uketomeKbnTwo1r.DBType = OracleDbType.Char;
        _uketomeKbnFour1r.DBType = OracleDbType.Char;
        _uketomeKbnFive1r.DBType = OracleDbType.Char;
        _systemEntryPgmid.DBType = OracleDbType.Char;
        _systemEntryPersonCd.DBType = OracleDbType.Varchar2;
        _systemEntryDay.DBType = OracleDbType.Date;
        _systemUpdatePgmid.DBType = OracleDbType.Char;
        _systemUpdatePersonCd.DBType = OracleDbType.Varchar2;
        _systemUpdateDay.DBType = OracleDbType.Date;
        _controlFlg1.IntegerBu = 1;
        _controlFlg10.IntegerBu = 1;
        _controlFlg2.IntegerBu = 1;
        _controlFlg3.IntegerBu = 1;
        _controlFlg4.IntegerBu = 1;
        _controlFlg5.IntegerBu = 1;
        _controlFlg6.IntegerBu = 1;
        _controlFlg7.IntegerBu = 1;
        _controlFlg8.IntegerBu = 1;
        _controlFlg9.IntegerBu = 1;
        _crsCd.IntegerBu = 6;
        _day1.IntegerBu = 1;
        _day2.IntegerBu = 1;
        _day3.IntegerBu = 1;
        _day4.IntegerBu = 1;
        _day5.IntegerBu = 1;
        _deleteDay.IntegerBu = 8;
        _gousya.IntegerBu = 3;
        _syuptDay.IntegerBu = 8;
        _text1.IntegerBu = 42;
        _text2.IntegerBu = 42;
        _text3.IntegerBu = 42;
        _text4.IntegerBu = 42;
        _text5.IntegerBu = 42;
        _uketomeKbnOne1r.IntegerBu = 1;
        _uketomeKbnThree1r.IntegerBu = 1;
        _uketomeKbnTwo1r.IntegerBu = 1;
        _uketomeKbnFour1r.IntegerBu = 1;
        _uketomeKbnFive1r.IntegerBu = 1;
        _systemEntryPgmid.IntegerBu = 8;
        _systemEntryPersonCd.IntegerBu = 20;
        _systemEntryDay.IntegerBu = 0;
        _systemUpdatePgmid.IntegerBu = 8;
        _systemUpdatePersonCd.IntegerBu = 20;
        _systemUpdateDay.IntegerBu = 0;
        _controlFlg1.DecimalBu = 0;
        _controlFlg10.DecimalBu = 0;
        _controlFlg2.DecimalBu = 0;
        _controlFlg3.DecimalBu = 0;
        _controlFlg4.DecimalBu = 0;
        _controlFlg5.DecimalBu = 0;
        _controlFlg6.DecimalBu = 0;
        _controlFlg7.DecimalBu = 0;
        _controlFlg8.DecimalBu = 0;
        _controlFlg9.DecimalBu = 0;
        _crsCd.DecimalBu = 0;
        _day1.DecimalBu = 0;
        _day2.DecimalBu = 0;
        _day3.DecimalBu = 0;
        _day4.DecimalBu = 0;
        _day5.DecimalBu = 0;
        _deleteDay.DecimalBu = 0;
        _gousya.DecimalBu = 0;
        _syuptDay.DecimalBu = 0;
        _text1.DecimalBu = 0;
        _text2.DecimalBu = 0;
        _text3.DecimalBu = 0;
        _text4.DecimalBu = 0;
        _text5.DecimalBu = 0;
        _uketomeKbnOne1r.DecimalBu = 0;
        _uketomeKbnThree1r.DecimalBu = 0;
        _uketomeKbnTwo1r.DecimalBu = 0;
        _uketomeKbnFour1r.DecimalBu = 0;
        _uketomeKbnFive1r.DecimalBu = 0;
        _systemEntryPgmid.DecimalBu = 0;
        _systemEntryPersonCd.DecimalBu = 0;
        _systemEntryDay.DecimalBu = 0;
        _systemUpdatePgmid.DecimalBu = 0;
        _systemUpdatePersonCd.DecimalBu = 0;
        _systemUpdateDay.DecimalBu = 0;
    }


    /// <summary>
/// controlFlg1
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_MojiType controlFlg1
    {
        get
        {
            return _controlFlg1;
        }

        set
        {
            _controlFlg1 = value;
        }
    }


    /// <summary>
/// controlFlg10
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_MojiType controlFlg10
    {
        get
        {
            return _controlFlg10;
        }

        set
        {
            _controlFlg10 = value;
        }
    }


    /// <summary>
/// controlFlg2
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_MojiType controlFlg2
    {
        get
        {
            return _controlFlg2;
        }

        set
        {
            _controlFlg2 = value;
        }
    }


    /// <summary>
/// controlFlg3
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_MojiType controlFlg3
    {
        get
        {
            return _controlFlg3;
        }

        set
        {
            _controlFlg3 = value;
        }
    }


    /// <summary>
/// controlFlg4
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_MojiType controlFlg4
    {
        get
        {
            return _controlFlg4;
        }

        set
        {
            _controlFlg4 = value;
        }
    }


    /// <summary>
/// controlFlg5
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_MojiType controlFlg5
    {
        get
        {
            return _controlFlg5;
        }

        set
        {
            _controlFlg5 = value;
        }
    }


    /// <summary>
/// controlFlg6
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_MojiType controlFlg6
    {
        get
        {
            return _controlFlg6;
        }

        set
        {
            _controlFlg6 = value;
        }
    }


    /// <summary>
/// controlFlg7
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_MojiType controlFlg7
    {
        get
        {
            return _controlFlg7;
        }

        set
        {
            _controlFlg7 = value;
        }
    }


    /// <summary>
/// controlFlg8
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_MojiType controlFlg8
    {
        get
        {
            return _controlFlg8;
        }

        set
        {
            _controlFlg8 = value;
        }
    }


    /// <summary>
/// controlFlg9
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_MojiType controlFlg9
    {
        get
        {
            return _controlFlg9;
        }

        set
        {
            _controlFlg9 = value;
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
/// day1
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_NumberType day1
    {
        get
        {
            return _day1;
        }

        set
        {
            _day1 = value;
        }
    }


    /// <summary>
/// day2
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_NumberType day2
    {
        get
        {
            return _day2;
        }

        set
        {
            _day2 = value;
        }
    }


    /// <summary>
/// day3
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_NumberType day3
    {
        get
        {
            return _day3;
        }

        set
        {
            _day3 = value;
        }
    }


    /// <summary>
/// day4
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_NumberType day4
    {
        get
        {
            return _day4;
        }

        set
        {
            _day4 = value;
        }
    }


    /// <summary>
/// day5
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_NumberType day5
    {
        get
        {
            return _day5;
        }

        set
        {
            _day5 = value;
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
/// text1
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_MojiType text1
    {
        get
        {
            return _text1;
        }

        set
        {
            _text1 = value;
        }
    }


    /// <summary>
/// text2
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_MojiType text2
    {
        get
        {
            return _text2;
        }

        set
        {
            _text2 = value;
        }
    }


    /// <summary>
/// text3
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_MojiType text3
    {
        get
        {
            return _text3;
        }

        set
        {
            _text3 = value;
        }
    }


    /// <summary>
/// text4
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_MojiType text4
    {
        get
        {
            return _text4;
        }

        set
        {
            _text4 = value;
        }
    }


    /// <summary>
/// text5
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_MojiType text5
    {
        get
        {
            return _text5;
        }

        set
        {
            _text5 = value;
        }
    }


    /// <summary>
/// uketomeKbnOne1r
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_MojiType uketomeKbnOne1r
    {
        get
        {
            return _uketomeKbnOne1r;
        }

        set
        {
            _uketomeKbnOne1r = value;
        }
    }


    /// <summary>
/// uketomeKbnThree1r
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_MojiType uketomeKbnThree1r
    {
        get
        {
            return _uketomeKbnThree1r;
        }

        set
        {
            _uketomeKbnThree1r = value;
        }
    }


    /// <summary>
/// uketomeKbnTwo1r
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_MojiType uketomeKbnTwo1r
    {
        get
        {
            return _uketomeKbnTwo1r;
        }

        set
        {
            _uketomeKbnTwo1r = value;
        }
    }


    /// <summary>
/// uketomeKbnFour1r
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_MojiType uketomeKbnFour1r
    {
        get
        {
            return _uketomeKbnFour1r;
        }

        set
        {
            _uketomeKbnFour1r = value;
        }
    }


    /// <summary>
/// uketomeKbnFive1r
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_MojiType uketomeKbnFive1r
    {
        get
        {
            return _uketomeKbnFive1r;
        }

        set
        {
            _uketomeKbnFive1r = value;
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