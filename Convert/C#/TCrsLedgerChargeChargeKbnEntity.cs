using System;
using Oracle.ManagedDataAccess.Client;


/// <summary>
/// コース台帳（料金_料金区分）
/// </summary>
/// <remarks></remarks>
[Serializable()]
// コース台帳（料金_料金区分）エンティティ
public partial class TCrsLedgerChargeChargeKbnEntity
{
    private EntityKoumoku_NumberType _syuptDay = new EntityKoumoku_NumberType();	// 出発日
    private EntityKoumoku_MojiType _crsCd = new EntityKoumoku_MojiType();	// コースコード
    private EntityKoumoku_NumberType _gousya = new EntityKoumoku_NumberType();	// 号車
    private EntityKoumoku_NumberType _kbnNo = new EntityKoumoku_NumberType();	// 区分No
    private EntityKoumoku_MojiType _chargeKbnJininCd = new EntityKoumoku_MojiType();	// 料金区分（人員）コード
    private EntityKoumoku_NumberType _charge = new EntityKoumoku_NumberType();	// 料金
    private EntityKoumoku_NumberType _chargeSubSeat = new EntityKoumoku_NumberType();	// 料金_補助席
    private EntityKoumoku_NumberType _carriage = new EntityKoumoku_NumberType();	// 運賃
    private EntityKoumoku_NumberType _carriageSubSeat = new EntityKoumoku_NumberType();	// 運賃_補助席
    private EntityKoumoku_MojiType _name1 = new EntityKoumoku_MojiType();	// 料金名称１
    private EntityKoumoku_MojiType _name2 = new EntityKoumoku_MojiType();	// 料金名称２
    private EntityKoumoku_MojiType _name3 = new EntityKoumoku_MojiType();	// 料金名称３
    private EntityKoumoku_MojiType _name4 = new EntityKoumoku_MojiType();	// 料金名称４
    private EntityKoumoku_MojiType _name5 = new EntityKoumoku_MojiType();	// 料金名称５
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

    public TCrsLedgerChargeChargeKbnEntity()
    {
        _syuptDay.PhysicsName = "SYUPT_DAY";
        _crsCd.PhysicsName = "CRS_CD";
        _gousya.PhysicsName = "GOUSYA";
        _kbnNo.PhysicsName = "KBN_NO";
        _chargeKbnJininCd.PhysicsName = "CHARGE_KBN_JININ_CD";
        _charge.PhysicsName = "CHARGE";
        _chargeSubSeat.PhysicsName = "CHARGE_SUB_SEAT";
        _carriage.PhysicsName = "CARRIAGE";
        _carriageSubSeat.PhysicsName = "CARRIAGE_SUB_SEAT";
        _name1.PhysicsName = "NAME_1";
        _name2.PhysicsName = "NAME_2";
        _name3.PhysicsName = "NAME_3";
        _name4.PhysicsName = "NAME_4";
        _name5.PhysicsName = "NAME_5";
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
        _kbnNo.Required = false;
        _chargeKbnJininCd.Required = false;
        _charge.Required = false;
        _chargeSubSeat.Required = false;
        _carriage.Required = false;
        _carriageSubSeat.Required = false;
        _name1.Required = false;
        _name2.Required = false;
        _name3.Required = false;
        _name4.Required = false;
        _name5.Required = false;
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
        _kbnNo.DBType = OracleDbType.Decimal;
        _chargeKbnJininCd.DBType = OracleDbType.Char;
        _charge.DBType = OracleDbType.Decimal;
        _chargeSubSeat.DBType = OracleDbType.Decimal;
        _carriage.DBType = OracleDbType.Decimal;
        _carriageSubSeat.DBType = OracleDbType.Decimal;
        _name1.DBType = OracleDbType.Varchar2;
        _name2.DBType = OracleDbType.Varchar2;
        _name3.DBType = OracleDbType.Varchar2;
        _name4.DBType = OracleDbType.Varchar2;
        _name5.DBType = OracleDbType.Varchar2;
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
        _kbnNo.IntegerBu = 1;
        _chargeKbnJininCd.IntegerBu = 2;
        _charge.IntegerBu = 7;
        _chargeSubSeat.IntegerBu = 7;
        _carriage.IntegerBu = 7;
        _carriageSubSeat.IntegerBu = 7;
        _name1.IntegerBu = 32;
        _name2.IntegerBu = 32;
        _name3.IntegerBu = 32;
        _name4.IntegerBu = 32;
        _name5.IntegerBu = 32;
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
        _kbnNo.DecimalBu = 0;
        _chargeKbnJininCd.DecimalBu = 0;
        _charge.DecimalBu = 0;
        _chargeSubSeat.DecimalBu = 0;
        _carriage.DecimalBu = 0;
        _carriageSubSeat.DecimalBu = 0;
        _name1.DecimalBu = 0;
        _name2.DecimalBu = 0;
        _name3.DecimalBu = 0;
        _name4.DecimalBu = 0;
        _name5.DecimalBu = 0;
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
/// charge
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_NumberType charge
    {
        get
        {
            return _charge;
        }

        set
        {
            _charge = value;
        }
    }


    /// <summary>
/// chargeSubSeat
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_NumberType chargeSubSeat
    {
        get
        {
            return _chargeSubSeat;
        }

        set
        {
            _chargeSubSeat = value;
        }
    }


    /// <summary>
/// carriage
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_NumberType carriage
    {
        get
        {
            return _carriage;
        }

        set
        {
            _carriage = value;
        }
    }


    /// <summary>
/// carriageSubSeat
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_NumberType carriageSubSeat
    {
        get
        {
            return _carriageSubSeat;
        }

        set
        {
            _carriageSubSeat = value;
        }
    }


    /// <summary>
/// name1
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_MojiType name1
    {
        get
        {
            return _name1;
        }

        set
        {
            _name1 = value;
        }
    }


    /// <summary>
/// name2
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_MojiType name2
    {
        get
        {
            return _name2;
        }

        set
        {
            _name2 = value;
        }
    }


    /// <summary>
/// name3
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_MojiType name3
    {
        get
        {
            return _name3;
        }

        set
        {
            _name3 = value;
        }
    }


    /// <summary>
/// name4
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_MojiType name4
    {
        get
        {
            return _name4;
        }

        set
        {
            _name4 = value;
        }
    }


    /// <summary>
/// name5
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_MojiType name5
    {
        get
        {
            return _name5;
        }

        set
        {
            _name5 = value;
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