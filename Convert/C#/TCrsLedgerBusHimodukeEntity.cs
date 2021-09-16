//====================================================================================================
//The Free Edition of Instant C# limits conversion output to 100 lines per file.

//To purchase the Premium Edition, visit our website:
//https://www.tangiblesoftwaresolutions.com/order/order-instant-csharp.html
//====================================================================================================

//INSTANT C# NOTE: Formerly VB project-level imports:
using Hatobus.ReservationManagementSystem.ClientCommon;
using Hatobus.ReservationManagementSystem.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Linq;
using System.Threading.Tasks;
using Oracle.ManagedDataAccess.Client;


namespace Hatobus.ReservationManagementSystem.Tehai
{
	/// <summary>
	/// �R�[�X�䒠�i�o�X�R�Â��j
	/// </summary>
	/// <remarks></remarks>
	[Serializable()]
	public class TCrsLedgerBusHimodukeEntity // �R�[�X�䒠�i�o�X�R�Â��j�G���e�B�e�B
	{


	private EntityKoumoku_MojiType _busCompanyCd = new EntityKoumoku_MojiType(); // �o�X��ЃR�[�h
	private EntityKoumoku_NumberType _busCompanyKakuteiDay = new EntityKoumoku_NumberType(); // �o�X��Њm���
	private EntityKoumoku_NumberType _busCompanyKakuteiTime = new EntityKoumoku_NumberType(); // �o�X��Њm�莞��
	private EntityKoumoku_MojiType _crsCd = new EntityKoumoku_MojiType(); // �R�[�X�R�[�h
	private EntityKoumoku_NumberType _gousya = new EntityKoumoku_NumberType(); // ����
	private EntityKoumoku_MojiType _hanbaiStartJiKakuteiFlg = new EntityKoumoku_MojiType(); // �̔��J�n���m��t���O
	private EntityKoumoku_MojiType _oldBusCompanyCd = new EntityKoumoku_MojiType(); // ���o�X��ЃR�[�h
	private EntityKoumoku_NumberType _syuptDay = new EntityKoumoku_NumberType(); // �o����
	private EntityKoumoku_MojiType _systemEntryPgmid = new EntityKoumoku_MojiType(); // �V�X�e���o�^�o�f�l�h�c
	private EntityKoumoku_MojiType _systemEntryPersonCd = new EntityKoumoku_MojiType(); // �V�X�e���o�^�҃R�[�h
	private EntityKoumoku_YmdType _systemEntryDay = new EntityKoumoku_YmdType(); // �V�X�e���o�^��
	private EntityKoumoku_MojiType _systemUpdatePgmid = new EntityKoumoku_MojiType(); // �V�X�e���X�V�o�f�l�h�c
	private EntityKoumoku_MojiType _systemUpdatePersonCd = new EntityKoumoku_MojiType(); // �V�X�e���X�V�҃R�[�h
	private EntityKoumoku_YmdType _systemUpdateDay = new EntityKoumoku_YmdType(); // �V�X�e���X�V��
	private EntityKoumoku_NumberType _deleteDate = new EntityKoumoku_NumberType(); // �폜��


	public TCrsLedgerBusHimodukeEntity()
	{
	_busCompanyCd.PhysicsName = "BUS_COMPANY_CD";
	_busCompanyKakuteiDay.PhysicsName = "BUS_COMPANY_KAKUTEI_DAY";
	_busCompanyKakuteiTime.PhysicsName = "BUS_COMPANY_KAKUTEI_TIME";
	_crsCd.PhysicsName = "CRS_CD";
	_gousya.PhysicsName = "GOUSYA";
	_hanbaiStartJiKakuteiFlg.PhysicsName = "HANBAI_START_JI_KAKUTEI_FLG";
	_oldBusCompanyCd.PhysicsName = "OLD_BUS_COMPANY_CD";
	_syuptDay.PhysicsName = "SYUPT_DAY";
	_systemEntryPgmid.PhysicsName = "SYSTEM_ENTRY_PGMID";
	_systemEntryPersonCd.PhysicsName = "SYSTEM_ENTRY_PERSON_CD";
	_systemEntryDay.PhysicsName = "SYSTEM_ENTRY_DAY";
	_systemUpdatePgmid.PhysicsName = "SYSTEM_UPDATE_PGMID";
	_systemUpdatePersonCd.PhysicsName = "SYSTEM_UPDATE_PERSON_CD";
	_systemUpdateDay.PhysicsName = "SYSTEM_UPDATE_DAY";
	_deleteDate.PhysicsName = "DELETE_DATE";


	_busCompanyCd.Required = false;
	_busCompanyKakuteiDay.Required = false;
	_busCompanyKakuteiTime.Required = false;
	_crsCd.Required = false;
	_gousya.Required = false;
	_hanbaiStartJiKakuteiFlg.Required = false;
	_oldBusCompanyCd.Required = false;
	_syuptDay.Required = false;
	_systemEntryPgmid.Required = true;
	_systemEntryPersonCd.Required = true;
	_systemEntryDay.Required = true;
	_systemUpdatePgmid.Required = true;
	_systemUpdatePersonCd.Required = true;
	_systemUpdateDay.Required = true;
	_deleteDate.Required = false;


	_busCompanyCd.DBType = OracleDbType.Char;
	_busCompanyKakuteiDay.DBType = OracleDbType.Decimal;
	_busCompanyKakuteiTime.DBType = OracleDbType.Decimal;
	_crsCd.DBType = OracleDbType.Char;
	_gousya.DBType = OracleDbType.Decimal;
	_hanbaiStartJiKakuteiFlg.DBType = OracleDbType.Char;
	_oldBusCompanyCd.DBType = OracleDbType.Char;
	_syuptDay.DBType = OracleDbType.Decimal;
	_systemEntryPgmid.DBType = OracleDbType.Char;
	_systemEntryPersonCd.DBType = OracleDbType.Varchar2;
	_systemEntryDay.DBType = OracleDbType.Date;
	_systemUpdatePgmid.DBType = OracleDbType.Char;
	_systemUpdatePersonCd.DBType = OracleDbType.Varchar2;
	_systemUpdateDay.DBType = OracleDbType.Date;
	_deleteDate.DBType = OracleDbType.Decimal;


	_busCompanyCd.IntegerBu = 6;
	_busCompanyKakuteiDay.IntegerBu = 8;
	_busCompanyKakuteiTime.IntegerBu = 6;
	_crsCd.IntegerBu = 6;
	_gousya.IntegerBu = 3;
	_hanbaiStartJiKakuteiFlg.IntegerBu = 1;
	_oldBusCompanyCd.IntegerBu = 6;
	_syuptDay.IntegerBu = 8;
	_systemEntryPgmid.IntegerBu = 8;
	_systemEntryPersonCd.IntegerBu = 20;
	_systemEntryDay.IntegerBu = 0;
	_systemUpdatePgmid.IntegerBu = 8;
	_systemUpdatePersonCd.IntegerBu = 20;
	_systemUpdateDay.IntegerBu = 0;
	_deleteDate.IntegerBu = 8;


	_busCompanyCd.DecimalBu = 0;
	_busCompanyKakuteiDay.DecimalBu = 0;
	_busCompanyKakuteiTime.DecimalBu = 0;
	_crsCd.DecimalBu = 0;
	_gousya.DecimalBu = 0;
	_hanbaiStartJiKakuteiFlg.DecimalBu = 0;
	_oldBusCompanyCd.DecimalBu = 0;
	_syuptDay.DecimalBu = 0;
	_systemEntryPgmid.DecimalBu = 0;

//====================================================================================================
//End of the allowed output for the Free Edition of Instant C#.

//To purchase the Premium Edition, visit our website:
//https://www.tangiblesoftwaresolutions.com/order/order-instant-csharp.html
//====================================================================================================
