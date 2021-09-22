using System;
using System.Collections;
using System.Text;

public partial class RemarksMemoListDisplay_DA : DataAccessorBase
{

    #region  定数／変数 

    // 予約情報（基本）
    private TYoyakuInfoBasicEntity tYoyakuInfoBasicEntity = new TYoyakuInfoBasicEntity();
    // 予約情報（メモ）
    private TYoyakuInfoMemoEntity tYoyakuInfoMemoEntity = new TYoyakuInfoMemoEntity();
    // コース台帳（リマークス）
    private TCrsLedgerRemarksEntity tCrsLedgerRemarksEntity = new TCrsLedgerRemarksEntity();
    // WT_リクエスト情報
    private TWtRequestInfoEntity tWtRequestInfoEntity = new TWtRequestInfoEntity();
    // WT_リクエスト情報（メモ）
    private TWtRequestInfoMemoEntity tWtRequestInfoMemoEntity = new TWtRequestInfoMemoEntity();

    #endregion

    #region  UNION ALL用 
    /// <summary>
    /// SELECT INFO T_YOYAKU_INFO_MEMO
    /// </summary>
    /// <returns></returns>
    protected StringBuilder getSubQueryReserInfo(Hashtable paramInfoList)
    {
        var commandText = new StringBuilder();
        // T_YOYAKU_INFO_MEMO
        commandText.AppendLine("SELECT ");
        commandText.AppendLine("    YMEMO.EDABAN AS EDABAN ");
        commandText.AppendLine("    ,YMEMO.YOYAKU_KBN ||','|| TRIM(TO_CHAR(YMEMO.YOYAKU_NO, '000,000,000')) AS RESER_NM");
        commandText.AppendLine("    ,YMEMO.YOYAKU_KBN || TRIM(TO_CHAR(YMEMO.YOYAKU_NO, '000000000')) AS RESER_NM_SEARCH ");
        commandText.AppendLine("    ,(YBASIC.SURNAME ||' '|| YBASIC.NAME) AS NAME_BASIC ");
        commandText.AppendLine("    ,YMEMO.MEMO_KBN AS MEMO_KBN");
        commandText.AppendLine("    ,YMEMO.MEMO_BUNRUI AS MEMO_BUNRUI");
        commandText.AppendLine("    ,YMEMO.KOSHAKASHO_CD AS KOSHAKASHO_CD");
        commandText.AppendLine("    ,YMEMO.KOSHAKASHO_EDABAN AS KOSHAKASHO_EDABAN");
        commandText.AppendLine("    ,YMEMO.STAFF_SHARE_TAISYO AS STAFF_SHARE_TAISYO ");
        commandText.AppendLine("    ,YMEMO.NAIYO AS NAIYO");
        commandText.AppendLine("    ,YMEMO.SYSTEM_UPDATE_DAY AS SYSTEM_UPDATE_DAY");
        commandText.AppendLine("    ,YMEMO.SYSTEM_UPDATE_PERSON_CD AS SYSTEM_UPDATE_PERSON_CD");
        commandText.AppendLine("FROM T_YOYAKU_INFO_MEMO YMEMO ");
        commandText.AppendLine("INNER JOIN T_YOYAKU_INFO_BASIC  YBASIC");
        commandText.AppendLine("    ON YBASIC.YOYAKU_KBN = YMEMO.YOYAKU_KBN ");
        commandText.AppendLine("   AND YBASIC.YOYAKU_NO = YMEMO.YOYAKU_NO ");
        {
            var withBlock = tYoyakuInfoBasicEntity;
            commandText.AppendLine("    AND YBASIC.CRS_CD = " + setParam(CommonKyushuUtil.getEnumValue(typeof(ConditionSearchMemoIchiranHyoji), ConditionSearchMemoIchiranHyoji.CRS_CD), paramInfoList(CommonKyushuUtil.getEnumValue(typeof(ConditionSearchMemoIchiranHyoji), ConditionSearchMemoIchiranHyoji.CRS_CD)), withBlock.CrsCd.DBType, withBlock.CrsCd.IntegerBu, withBlock.CrsCd.DecimalBu));
            commandText.AppendLine("    AND YBASIC.SYUPT_DAY = " + setParam(CommonKyushuUtil.getEnumValue(typeof(ConditionSearchMemoIchiranHyoji), ConditionSearchMemoIchiranHyoji.SYUPT_DAY), paramInfoList(CommonKyushuUtil.getEnumValue(typeof(ConditionSearchMemoIchiranHyoji), ConditionSearchMemoIchiranHyoji.SYUPT_DAY)), withBlock.SyuptDay.DBType, withBlock.SyuptDay.IntegerBu, withBlock.SyuptDay.DecimalBu));
            commandText.AppendLine("    AND YBASIC.GOUSYA = " + setParam(CommonKyushuUtil.getEnumValue(typeof(ConditionSearchMemoIchiranHyoji), ConditionSearchMemoIchiranHyoji.GOUSYA), paramInfoList(CommonKyushuUtil.getEnumValue(typeof(ConditionSearchMemoIchiranHyoji), ConditionSearchMemoIchiranHyoji.GOUSYA)), withBlock.Gousya.DBType, withBlock.Gousya.IntegerBu, withBlock.Gousya.DecimalBu));
            commandText.AppendLine("    AND NVL(YBASIC.CANCEL_FLG, ' ')　= ' ' ");
        }

        commandText.AppendLine("WHERE NVL(YMEMO.DELETE_DATE, 0) = 0 ");
        return commandText;
    }

    /// <summary>
    /// SELECT INFO T_CRS_LEDGER_REMARKS
    /// </summary>
    /// <param name="paramInfoList"></param>
    /// <returns></returns>
    protected StringBuilder getSubQueryCourseLedger(Hashtable paramInfoList)
    {
        var commandText = new StringBuilder();
        {
            var withBlock = tCrsLedgerRemarksEntity;
            commandText.AppendLine("SELECT ");
            commandText.AppendLine("    CREMARKS.EDABAN AS  EDABAN");
            commandText.AppendLine("    ,'-' AS RESER_NM");
            commandText.AppendLine("    ,'-' AS RESER_NM_SEARCH");
            commandText.AppendLine("    ,'-' AS NAME_BASIC");
            commandText.AppendLine("    ,CREMARKS.MEMO_KBN AS MEMO_KBN");
            commandText.AppendLine("    ,CREMARKS.MEMO_BUNRUI AS MEMO_BUNRUI");
            commandText.AppendLine("    ,CREMARKS.KOSHAKASHO_CD AS KOSHAKASHO_CD");
            commandText.AppendLine("    ,CREMARKS.KOSHAKASHO_EDABAN AS KOSHAKASHO_EDABAN");
            commandText.AppendLine("    ,CREMARKS.STAFF_SHARE_TAISYO AS STAFF_SHARE_TAISYO");
            commandText.AppendLine("    ,CREMARKS.NAIYO As NAIYO");
            commandText.AppendLine("    ,CREMARKS.SYSTEM_UPDATE_DAY As SYSTEM_UPDATE_DAY");
            commandText.AppendLine("    ,CREMARKS.SYSTEM_UPDATE_PERSON_CD As SYSTEM_UPDATE_PERSON_CD");
            commandText.AppendLine("FROM T_CRS_LEDGER_REMARKS  CREMARKS ");
            commandText.AppendLine("WHERE CREMARKS.CRS_CD =" + setParam(CommonKyushuUtil.getEnumValue(typeof(ConditionSearchMemoIchiranHyoji), ConditionSearchMemoIchiranHyoji.CRS_CD), paramInfoList(CommonKyushuUtil.getEnumValue(typeof(ConditionSearchMemoIchiranHyoji), ConditionSearchMemoIchiranHyoji.CRS_CD)), withBlock.crsCd.DBType, withBlock.crsCd.IntegerBu, withBlock.crsCd.DecimalBu));
            commandText.AppendLine("    AND CREMARKS.SYUPT_DAY = " + setParam(CommonKyushuUtil.getEnumValue(typeof(ConditionSearchMemoIchiranHyoji), ConditionSearchMemoIchiranHyoji.SYUPT_DAY), paramInfoList(CommonKyushuUtil.getEnumValue(typeof(ConditionSearchMemoIchiranHyoji), ConditionSearchMemoIchiranHyoji.SYUPT_DAY)), withBlock.syuptDay.DBType, withBlock.syuptDay.IntegerBu, withBlock.syuptDay.DecimalBu));
            commandText.AppendLine("    AND CREMARKS.GOUSYA = " + setParam(CommonKyushuUtil.getEnumValue(typeof(ConditionSearchMemoIchiranHyoji), ConditionSearchMemoIchiranHyoji.GOUSYA), paramInfoList(CommonKyushuUtil.getEnumValue(typeof(ConditionSearchMemoIchiranHyoji), ConditionSearchMemoIchiranHyoji.GOUSYA)), withBlock.gousya.DBType, withBlock.gousya.IntegerBu, withBlock.gousya.DecimalBu));
            commandText.AppendLine("    AND NVL(CREMARKS.DELETE_DATE, 0) = 0 ");
        }

        return commandText;
    }

    /// <summary>
    /// SELECT T_WT_REQUEST_INFO_MEMO
    /// </summary>
    /// <returns></returns>
    protected StringBuilder getSubQueryWTRequestInfo(Hashtable paramInfoList)
    {
        var commandText = new StringBuilder();
        // T_WT_REQUEST_INFO_MEMO
        commandText.AppendLine("SELECT ");
        commandText.AppendLine("    WTRMEMO.EDABAN AS EDABAN ");
        commandText.AppendLine("    ,WTRMEMO.MANAGEMENT_KBN ||','|| TRIM(TO_CHAR(WTRMEMO.MANAGEMENT_NO, '000,000,000')) AS RESER_NM ");
        commandText.AppendLine("    ,WTRMEMO.MANAGEMENT_KBN || TRIM(TO_CHAR(WTRMEMO.MANAGEMENT_NO, '000000000')) AS RESER_NM_SEARCH ");
        commandText.AppendLine("    ,(WTRINFO.SURNAME ||' '|| WTRINFO.NAME) AS NAME_BASIC ");
        commandText.AppendLine("    ,WTRMEMO.MEMO_KBN AS MEMO_KBN ");
        commandText.AppendLine("    ,WTRMEMO.MEMO_BUNRUI AS MEMO_BUNRUI ");
        commandText.AppendLine("    ,WTRMEMO.KOSHAKASHO_CD AS KOSHAKASHO_CD ");
        commandText.AppendLine("    ,WTRMEMO.KOSHAKASHO_EDABAN AS KOSHAKASHO_EDABAN ");
        commandText.AppendLine("    ,WTRMEMO.STAFF_SHARE_TAISYO AS STAFF_SHARE_TAISYO ");
        commandText.AppendLine("    ,WTRMEMO.NAIYO AS NAIYO ");
        commandText.AppendLine("    ,WTRMEMO.SYSTEM_UPDATE_DAY AS SYSTEM_UPDATE_DAY ");
        commandText.AppendLine("    ,WTRMEMO.SYSTEM_UPDATE_PERSON_CD AS SYSTEM_UPDATE_PERSON_CD ");
        commandText.AppendLine("FROM T_WT_REQUEST_INFO_MEMO WTRMEMO ");
        commandText.AppendLine("INNER JOIN T_WT_REQUEST_INFO  WTRINFO ");
        commandText.AppendLine("    ON WTRINFO.MANAGEMENT_KBN = WTRMEMO.MANAGEMENT_KBN ");
        commandText.AppendLine("    AND WTRINFO.MANAGEMENT_NO = WTRMEMO.MANAGEMENT_NO ");
        {
            var withBlock = tWtRequestInfoEntity;
            commandText.AppendLine("    AND WTRINFO.CRS_CD = " + setParam(CommonKyushuUtil.getEnumValue(typeof(ConditionSearchMemoIchiranHyoji), ConditionSearchMemoIchiranHyoji.CRS_CD), paramInfoList(CommonKyushuUtil.getEnumValue(typeof(ConditionSearchMemoIchiranHyoji), ConditionSearchMemoIchiranHyoji.CRS_CD)), withBlock.CrsCd.DBType, withBlock.CrsCd.IntegerBu, withBlock.CrsCd.DecimalBu));
            commandText.AppendLine("    AND WTRINFO.SYUPT_DAY = " + setParam(CommonKyushuUtil.getEnumValue(typeof(ConditionSearchMemoIchiranHyoji), ConditionSearchMemoIchiranHyoji.SYUPT_DAY), paramInfoList(CommonKyushuUtil.getEnumValue(typeof(ConditionSearchMemoIchiranHyoji), ConditionSearchMemoIchiranHyoji.SYUPT_DAY)), withBlock.SyuptDay.DBType, withBlock.SyuptDay.IntegerBu, withBlock.SyuptDay.DecimalBu));
            commandText.AppendLine("    AND WTRINFO.GOUSYA = " + setParam(CommonKyushuUtil.getEnumValue(typeof(ConditionSearchMemoIchiranHyoji), ConditionSearchMemoIchiranHyoji.GOUSYA), paramInfoList(CommonKyushuUtil.getEnumValue(typeof(ConditionSearchMemoIchiranHyoji), ConditionSearchMemoIchiranHyoji.GOUSYA)), withBlock.Gousya.DBType, withBlock.Gousya.IntegerBu, withBlock.Gousya.DecimalBu));
            commandText.AppendLine("    AND NVL(WTRINFO.STATE, ' ') = ' ' ");
        }

        commandText.AppendLine("WHERE NVL(WTRMEMO.DELETE_DATE, 0) = 0 ");
        return commandText;
    }
    #endregion


    #region getSubQuery
    /// <summary>
    /// ※No2、No3、No4をuion allで結合したテーブル / 「F8：検索」ボタン
    /// </summary>
    /// <param name="paramInfoList"></param>
    /// <returns></returns>
    public StringBuilder getSubQuerySearch(Hashtable paramInfoList)
    {
        var commandText = new StringBuilder();
        commandText.AppendLine("SELECT ");
        commandText.AppendLine("    INFOLST.EDABAN AS EDABAN");
        commandText.AppendLine("    ,INFOLST.RESER_NM AS RESER_NM");
        commandText.AppendLine("    ,INFOLST.RESER_NM_SEARCH AS RESER_NM_SEARCH");
        commandText.AppendLine("    ,INFOLST.NAME_BASIC AS NAME_BASIC");
        commandText.AppendLine("    ,INFOLST.MEMO_KBN AS MEMO_KBN");
        commandText.AppendLine("    ,INFOLST.MEMO_BUNRUI AS MEMO_BUNRUI");
        commandText.AppendLine("    ,MCN.CODE_NAME AS CODE_NAME1");
        commandText.AppendLine("    ,NCN.CODE_NAME AS CODE_NAME2");
        commandText.AppendLine("    ,INFOLST.KOSHAKASHO_CD AS KOSHAKASHO_CD");
        commandText.AppendLine("    ,INFOLST.KOSHAKASHO_EDABAN AS KOSHAKASHO_EDABAN");
        commandText.AppendLine("    ,MSAKI.SIIRE_SAKI_NAME AS SIIRE_SAKI_NAME");
        commandText.AppendLine("    ,SPN.CODE_NAME AS CODE_NAME3");
        commandText.AppendLine("    ,INFOLST.STAFF_SHARE_TAISYO AS STAFF_SHARE_TAISYO");
        commandText.AppendLine("    ,SSN.CODE_NAME AS CODE_NAME4");
        commandText.AppendLine("    ,INFOLST.NAIYO AS NAIYO");
        commandText.AppendLine("    ,INFOLST.SYSTEM_UPDATE_DAY AS SYSTEM_UPDATE_DAY");
        commandText.AppendLine("    ,INFOLST.SYSTEM_UPDATE_PERSON_CD AS SYSTEM_UPDATE_PERSON_CD");
        commandText.AppendLine("    ,M_USER.USER_NAME AS USER_NAME");
        commandText.AppendLine("FROM ");
        commandText.AppendLine("    (" + getSubQueryReserInfo(paramInfoList).ToString());
        commandText.AppendLine("    UNION ALL");
        commandText.AppendLine("    " + getSubQueryCourseLedger(paramInfoList).ToString());
        commandText.AppendLine("    UNION ALL");
        commandText.AppendLine("    " + getSubQueryWTRequestInfo(paramInfoList).ToString());
        commandText.AppendLine("    )" + "  INFOLST");
        commandText.AppendLine("LEFT OUTER JOIN M_SIIRE_SAKI  MSAKI");
        commandText.AppendLine("    ON MSAKI.SIIRE_SAKI_CD = INFOLST.KOSHAKASHO_CD");
        commandText.AppendLine("    AND MSAKI.SIIRE_SAKI_NO = INFOLST.KOSHAKASHO_EDABAN");
        commandText.AppendLine("LEFT OUTER JOIN M_CODE  SPN");
        commandText.AppendLine("    ON SPN.CODE_BUNRUI = " + CommonType_MojiColValue.CdBunruiType_Value(CdBunruiType.seisanMokutekiMaster));
        commandText.AppendLine("    AND SPN.CODE_VALUE = MSAKI.SEISAN_TGT_CD");
        commandText.AppendLine("LEFT OUTER JOIN M_CODE  MCN");
        commandText.AppendLine("    ON MCN.CODE_BUNRUI = " + CommonType_MojiColValue.CdBunruiType_Value(CdBunruiType.memokbn));
        commandText.AppendLine("    AND MCN.CODE_VALUE = INFOLST.MEMO_KBN");
        commandText.AppendLine("LEFT OUTER JOIN M_CODE  NCN");
        commandText.AppendLine("    ON NCN.CODE_BUNRUI = " + CommonType_MojiColValue.CdBunruiType_Value(CdBunruiType.memobunrui));
        commandText.AppendLine("    AND NCN.CODE_VALUE = INFOLST.MEMO_BUNRUI");
        commandText.AppendLine("LEFT OUTER JOIN M_CODE  SSN");
        commandText.AppendLine("    ON SSN.CODE_BUNRUI = " + MemoClassification.staff_Sharing);
        commandText.AppendLine("    AND SSN.CODE_VALUE = INFOLST.STAFF_SHARE_TAISYO");
        commandText.AppendLine("LEFT OUTER JOIN M_USER  M_USER");
        commandText.AppendLine("    ON M_USER.COMPANY_CD = '0001'");
        commandText.AppendLine("    AND M_USER.USER_ID = INFOLST.SYSTEM_UPDATE_PERSON_CD");
        if (paramInfoList(CommonKyushuUtil.getEnumValue(typeof(ConditionSearchMemoIchiranHyoji), ConditionSearchMemoIchiranHyoji.SHARED_ITEMS)).ToString == "True" | paramInfoList(CommonKyushuUtil.getEnumValue(typeof(ConditionSearchMemoIchiranHyoji), ConditionSearchMemoIchiranHyoji.BUSINESS_HISTORY)).ToString() == "True")
        {
            commandText.AppendLine("WHERE ((INFOLST.MEMO_KBN = " + NoteClassification.Shared_Items + " AND " + setParam(CommonKyushuUtil.getEnumValue(typeof(ConditionSearchMemoIchiranHyoji), ConditionSearchMemoIchiranHyoji.SHARED_ITEMS), paramInfoList(CommonKyushuUtil.getEnumValue(typeof(ConditionSearchMemoIchiranHyoji), ConditionSearchMemoIchiranHyoji.SHARED_ITEMS))) + "= 'True')");
            commandText.AppendLine("    AND ((INFOLST.MEMO_BUNRUI = " + MemoClassification.memo + " AND " + setParam(CommonKyushuUtil.getEnumValue(typeof(ConditionSearchMemoIchiranHyoji), ConditionSearchMemoIchiranHyoji.MEMO), paramInfoList(CommonKyushuUtil.getEnumValue(typeof(ConditionSearchMemoIchiranHyoji), ConditionSearchMemoIchiranHyoji.MEMO))) + "= 'True')");
            commandText.AppendLine("    OR (INFOLST.MEMO_BUNRUI =  " + MemoClassification.disembarkation_Place_Report + " AND " + setParam(CommonKyushuUtil.getEnumValue(typeof(ConditionSearchMemoIchiranHyoji), ConditionSearchMemoIchiranHyoji.DISEMBARKATION_PLACE_REPORT), paramInfoList(CommonKyushuUtil.getEnumValue(typeof(ConditionSearchMemoIchiranHyoji), ConditionSearchMemoIchiranHyoji.DISEMBARKATION_PLACE_REPORT))) + "= 'True')");
            commandText.AppendLine("    OR (INFOLST.MEMO_BUNRUI =  " + MemoClassification.staff_Sharing + " AND " + setParam(CommonKyushuUtil.getEnumValue(typeof(ConditionSearchMemoIchiranHyoji), ConditionSearchMemoIchiranHyoji.STAFF_SHARING), paramInfoList(CommonKyushuUtil.getEnumValue(typeof(ConditionSearchMemoIchiranHyoji), ConditionSearchMemoIchiranHyoji.STAFF_SHARING))) + "= 'True'))");
            commandText.AppendLine("    OR (INFOLST.MEMO_KBN = " + NoteClassification.Business_History + " AND " + setParam(CommonKyushuUtil.getEnumValue(typeof(ConditionSearchMemoIchiranHyoji), ConditionSearchMemoIchiranHyoji.BUSINESS_HISTORY), paramInfoList(CommonKyushuUtil.getEnumValue(typeof(ConditionSearchMemoIchiranHyoji), ConditionSearchMemoIchiranHyoji.BUSINESS_HISTORY))) + "= 'True')");
            commandText.AppendLine("    AND ((INFOLST.MEMO_BUNRUI = " + MemoClassification.payment_Contact + " AND " + setParam(CommonKyushuUtil.getEnumValue(typeof(ConditionSearchMemoIchiranHyoji), ConditionSearchMemoIchiranHyoji.PAYMENT_CONTACT), paramInfoList(CommonKyushuUtil.getEnumValue(typeof(ConditionSearchMemoIchiranHyoji), ConditionSearchMemoIchiranHyoji.PAYMENT_CONTACT))) + "= 'True')");
            commandText.AppendLine("    OR (INFOLST.MEMO_BUNRUI = " + MemoClassification.booking_Confirmation + " AND " + setParam(CommonKyushuUtil.getEnumValue(typeof(ConditionSearchMemoIchiranHyoji), ConditionSearchMemoIchiranHyoji.BOOKING_CONFIRMATION), paramInfoList(CommonKyushuUtil.getEnumValue(typeof(ConditionSearchMemoIchiranHyoji), ConditionSearchMemoIchiranHyoji.BOOKING_CONFIRMATION))) + "= 'True')");
            commandText.AppendLine("    OR (INFOLST.MEMO_BUNRUI = " + MemoClassification.NOSHOW_Confirmation + " AND " + setParam(CommonKyushuUtil.getEnumValue(typeof(ConditionSearchMemoIchiranHyoji), ConditionSearchMemoIchiranHyoji.NOSHOW_CONFIRMATION), paramInfoList(CommonKyushuUtil.getEnumValue(typeof(ConditionSearchMemoIchiranHyoji), ConditionSearchMemoIchiranHyoji.NOSHOW_CONFIRMATION))) + "= 'True')");
            commandText.AppendLine("    OR (INFOLST.MEMO_BUNRUI = " + MemoClassification.cancellation_Of_Liaison + " AND " + setParam(CommonKyushuUtil.getEnumValue(typeof(ConditionSearchMemoIchiranHyoji), ConditionSearchMemoIchiranHyoji.CANCELLATION_OF_LIAISON), paramInfoList(CommonKyushuUtil.getEnumValue(typeof(ConditionSearchMemoIchiranHyoji), ConditionSearchMemoIchiranHyoji.CANCELLATION_OF_LIAISON))) + "= 'True')");
            commandText.AppendLine("    OR (INFOLST.MEMO_BUNRUI = " + MemoClassification.dubricity_Check + " AND " + setParam(CommonKyushuUtil.getEnumValue(typeof(ConditionSearchMemoIchiranHyoji), ConditionSearchMemoIchiranHyoji.DUBRICITY_CHECK), paramInfoList(CommonKyushuUtil.getEnumValue(typeof(ConditionSearchMemoIchiranHyoji), ConditionSearchMemoIchiranHyoji.DUBRICITY_CHECK))) + "= 'True')");
            commandText.AppendLine("    OR (INFOLST.MEMO_BUNRUI = " + MemoClassification.weight + " AND " + setParam(CommonKyushuUtil.getEnumValue(typeof(ConditionSearchMemoIchiranHyoji), ConditionSearchMemoIchiranHyoji.WEIGHT), paramInfoList(CommonKyushuUtil.getEnumValue(typeof(ConditionSearchMemoIchiranHyoji), ConditionSearchMemoIchiranHyoji.WEIGHT))) + "= 'True')");
            commandText.AppendLine("    OR (INFOLST.MEMO_BUNRUI = " + MemoClassification.request + " AND " + setParam(CommonKyushuUtil.getEnumValue(typeof(ConditionSearchMemoIchiranHyoji), ConditionSearchMemoIchiranHyoji.REQUEST), paramInfoList(CommonKyushuUtil.getEnumValue(typeof(ConditionSearchMemoIchiranHyoji), ConditionSearchMemoIchiranHyoji.REQUEST))) + "= 'True')))");
            if (default
#error Cannot convert BinaryExpressionSyntax - see comment for details
/* Cannot convert BinaryExpressionSyntax, System.NullReferenceException: Object reference not set to an instance of an object.
   at ICSharpCode.CodeConverter.CSharp.VisualBasicEqualityComparison.VbCoerceToNonNullString(ExpressionSyntax vbNode, ExpressionSyntax csNode, TypeInfo typeInfo, Boolean knownNotNull) in D:\a\CodeConverter\CodeConverter\CodeConverter\CSharp\VisualBasicEqualityComparison.cs:line 102
   at ICSharpCode.CodeConverter.CSharp.VisualBasicEqualityComparison.VbCoerceToNonNullString(ExpressionSyntax vbLeft, ExpressionSyntax csLeft, TypeInfo lhsTypeInfo, Boolean leftKnownNotNull, ExpressionSyntax vbRight, ExpressionSyntax csRight, TypeInfo rhsTypeInfo, Boolean rightKnownNotNull) in D:\a\CodeConverter\CodeConverter\CodeConverter\CSharp\VisualBasicEqualityComparison.cs:line 84
   at ICSharpCode.CodeConverter.CSharp.VisualBasicEqualityComparison.AdjustForVbStringComparison(ExpressionSyntax vbLeft, ExpressionSyntax csLeft, TypeInfo lhsTypeInfo, Boolean leftKnownNotNull, ExpressionSyntax vbRight, ExpressionSyntax csRight, TypeInfo rhsTypeInfo, Boolean rightKnownNotNull) in D:\a\CodeConverter\CodeConverter\CodeConverter\CSharp\VisualBasicEqualityComparison.cs:line 219
   at ICSharpCode.CodeConverter.CSharp.ExpressionNodeVisitor.VisitBinaryExpression(BinaryExpressionSyntax node) in D:\a\CodeConverter\CodeConverter\CodeConverter\CSharp\ExpressionNodeVisitor.cs:line 786
   at ICSharpCode.CodeConverter.CSharp.CommentConvertingVisitorWrapper.ConvertHandledAsync[T](VisualBasicSyntaxNode vbNode, SourceTriviaMapKind sourceTriviaMap) in D:\a\CodeConverter\CodeConverter\CodeConverter\CSharp\CommentConvertingVisitorWrapper.cs:line 63

Input:
paramInfoList.Item(CommonKyushuUtil.getEnumValue(GetType(ConditionSearchMemoIchiranHyoji), ConditionSearchMemoIchiranHyoji.RESER_NM)).ToString <> "" 
 */
)
            {
                commandText.AppendLine("    AND INFOLST.RESER_NM_SEARCH = '" + paramInfoList(CommonKyushuUtil.getEnumValue(typeof(ConditionSearchMemoIchiranHyoji), ConditionSearchMemoIchiranHyoji.RESER_NM)).ToString() + "'");
            }
        }
        // commandText.AppendLine("ORDER BY INFOLST.RESER_NM, INFOLST.SYSTEM_UPDATE_DAY ASC FETCH FIRST 101 ROWS ONLY")
        commandText.AppendLine("ORDER BY INFOLST.RESER_NM, INFOLST.SYSTEM_UPDATE_DAY ASC");
        return commandText;
    }
    #endregion

    #region getSubQuery
    /// <summary>
    /// ※No2、No3、No4をuion allで結合したテーブル / Load
    /// </summary>
    /// <param name="paramInfoList"></param>
    /// <returns></returns>
    public StringBuilder getSubQueryLoad(Hashtable paramInfoList)
    {
        var commandText = new StringBuilder();
        commandText.AppendLine("Select ");
        commandText.AppendLine("    INFOLST.EDABAN As EDABAN");
        commandText.AppendLine("    , INFOLST.RESER_NM As RESER_NM");
        commandText.AppendLine("    , INFOLST.NAME_BASIC As NAME_BASIC");
        commandText.AppendLine("    , INFOLST.MEMO_KBN As MEMO_KBN");
        commandText.AppendLine("    , INFOLST.MEMO_BUNRUI As MEMO_BUNRUI");
        commandText.AppendLine("    , MCN.CODE_NAME As CODE_NAME1");
        commandText.AppendLine("    , NCN.CODE_NAME As CODE_NAME2");
        commandText.AppendLine("    , INFOLST.KOSHAKASHO_CD As KOSHAKASHO_CD");
        commandText.AppendLine("    , INFOLST.KOSHAKASHO_EDABAN As KOSHAKASHO_EDABAN");
        commandText.AppendLine("    , MSAKI.SIIRE_SAKI_NAME As SIIRE_SAKI_NAME");
        commandText.AppendLine("    , SPN.CODE_NAME As CODE_NAME3");
        commandText.AppendLine("    , INFOLST.STAFF_SHARE_TAISYO As STAFF_SHARE_TAISYO");
        commandText.AppendLine("    , SSN.CODE_NAME As CODE_NAME4");
        commandText.AppendLine("    , INFOLST.NAIYO As NAIYO");
        commandText.AppendLine("    , INFOLST.SYSTEM_UPDATE_DAY As SYSTEM_UPDATE_DAY");
        commandText.AppendLine("    , INFOLST.SYSTEM_UPDATE_PERSON_CD As SYSTEM_UPDATE_PERSON_CD");
        commandText.AppendLine("    , M_USER.USER_NAME As USER_NAME");
        commandText.AppendLine("FROM ");
        commandText.AppendLine("    (" + getSubQueryReserInfo(paramInfoList).ToString());
        commandText.AppendLine("    UNION ALL");
        commandText.AppendLine("    " + getSubQueryCourseLedger(paramInfoList).ToString());
        commandText.AppendLine("    UNION ALL");
        commandText.AppendLine("    " + getSubQueryWTRequestInfo(paramInfoList).ToString());
        commandText.AppendLine("    )" + "  INFOLST");
        commandText.AppendLine("LEFT OUTER JOIN M_SIIRE_SAKI  MSAKI");
        commandText.AppendLine("    On MSAKI.SIIRE_SAKI_CD = INFOLST.KOSHAKASHO_CD");
        commandText.AppendLine("    And MSAKI.SIIRE_SAKI_NO = INFOLST.KOSHAKASHO_EDABAN");
        commandText.AppendLine("LEFT OUTER JOIN M_CODE  SPN");
        commandText.AppendLine("    On SPN.CODE_BUNRUI =" + CommonType_MojiColValue.CdBunruiType_Value(CdBunruiType.seisanMokutekiMaster));
        commandText.AppendLine(" And SPN.CODE_VALUE = MSAKI.SEISAN_TGT_CD");
        commandText.AppendLine("LEFT OUTER JOIN M_CODE  MCN");
        commandText.AppendLine("    On MCN.CODE_BUNRUI = " + CommonType_MojiColValue.CdBunruiType_Value(CdBunruiType.memokbn));
        commandText.AppendLine("    And MCN.CODE_VALUE = INFOLST.MEMO_KBN");
        commandText.AppendLine("LEFT OUTER JOIN M_CODE  NCN");
        commandText.AppendLine("    On NCN.CODE_BUNRUI = " + CommonType_MojiColValue.CdBunruiType_Value(CdBunruiType.memobunrui));
        commandText.AppendLine(" And NCN.CODE_VALUE = INFOLST.MEMO_BUNRUI");
        commandText.AppendLine("LEFT OUTER JOIN M_CODE  SSN");
        commandText.AppendLine("    On SSN.CODE_BUNRUI = " + MemoClassification.staff_Sharing); // CommonType_MojiColValue.CdBunruiType_Value(CdBunruiType.staff_Sharing))
        commandText.AppendLine("    And SSN.CODE_VALUE = INFOLST.STAFF_SHARE_TAISYO");
        commandText.AppendLine("LEFT OUTER JOIN M_USER  M_USER");
        commandText.AppendLine("    On M_USER.COMPANY_CD = '0001'");
        commandText.AppendLine("    AND M_USER.USER_ID = INFOLST.SYSTEM_UPDATE_PERSON_CD");
        if (paramInfoList(CommonKyushuUtil.getEnumValue(typeof(ConditionSearchMemoIchiranHyoji), ConditionSearchMemoIchiranHyoji.SHARED_ITEMS)).ToString == "True" | paramInfoList(CommonKyushuUtil.getEnumValue(typeof(ConditionSearchMemoIchiranHyoji), ConditionSearchMemoIchiranHyoji.BUSINESS_HISTORY)).ToString() == "True")
        {
            commandText.AppendLine("WHERE ((INFOLST.MEMO_KBN = " + NoteClassification.Shared_Items + " AND " + setParam(CommonKyushuUtil.getEnumValue(typeof(ConditionSearchMemoIchiranHyoji), ConditionSearchMemoIchiranHyoji.SHARED_ITEMS), paramInfoList(CommonKyushuUtil.getEnumValue(typeof(ConditionSearchMemoIchiranHyoji), ConditionSearchMemoIchiranHyoji.SHARED_ITEMS))) + "= 'True')");
            commandText.AppendLine("    AND ((INFOLST.MEMO_BUNRUI = " + MemoClassification.memo + " AND " + setParam(CommonKyushuUtil.getEnumValue(typeof(ConditionSearchMemoIchiranHyoji), ConditionSearchMemoIchiranHyoji.MEMO), paramInfoList(CommonKyushuUtil.getEnumValue(typeof(ConditionSearchMemoIchiranHyoji), ConditionSearchMemoIchiranHyoji.MEMO))) + "= 'True'" + ")");
            commandText.AppendLine("    OR (INFOLST.MEMO_BUNRUI = " + MemoClassification.disembarkation_Place_Report + " AND " + setParam(CommonKyushuUtil.getEnumValue(typeof(ConditionSearchMemoIchiranHyoji), ConditionSearchMemoIchiranHyoji.DISEMBARKATION_PLACE_REPORT), paramInfoList(CommonKyushuUtil.getEnumValue(typeof(ConditionSearchMemoIchiranHyoji), ConditionSearchMemoIchiranHyoji.DISEMBARKATION_PLACE_REPORT))) + "= 'True'" + ")");
            commandText.AppendLine("    OR (INFOLST.MEMO_BUNRUI =  " + MemoClassification.staff_Sharing + " AND " + setParam(CommonKyushuUtil.getEnumValue(typeof(ConditionSearchMemoIchiranHyoji), ConditionSearchMemoIchiranHyoji.STAFF_SHARING), paramInfoList(CommonKyushuUtil.getEnumValue(typeof(ConditionSearchMemoIchiranHyoji), ConditionSearchMemoIchiranHyoji.STAFF_SHARING))) + "= 'True'" + ")))");
            commandText.AppendLine("    OR ((INFOLST.MEMO_KBN = " + NoteClassification.Business_History + " AND " + setParam(CommonKyushuUtil.getEnumValue(typeof(ConditionSearchMemoIchiranHyoji), ConditionSearchMemoIchiranHyoji.BUSINESS_HISTORY), paramInfoList(CommonKyushuUtil.getEnumValue(typeof(ConditionSearchMemoIchiranHyoji), ConditionSearchMemoIchiranHyoji.BUSINESS_HISTORY))) + "= 'True'" + ")");
            commandText.AppendLine("    AND ((INFOLST.MEMO_BUNRUI = " + MemoClassification.payment_Contact + " AND " + setParam(CommonKyushuUtil.getEnumValue(typeof(ConditionSearchMemoIchiranHyoji), ConditionSearchMemoIchiranHyoji.PAYMENT_CONTACT), paramInfoList(CommonKyushuUtil.getEnumValue(typeof(ConditionSearchMemoIchiranHyoji), ConditionSearchMemoIchiranHyoji.PAYMENT_CONTACT))) + "= 'True'" + ")");
            commandText.AppendLine("    OR (INFOLST.MEMO_BUNRUI = " + MemoClassification.booking_Confirmation + " AND " + setParam(CommonKyushuUtil.getEnumValue(typeof(ConditionSearchMemoIchiranHyoji), ConditionSearchMemoIchiranHyoji.BOOKING_CONFIRMATION), paramInfoList(CommonKyushuUtil.getEnumValue(typeof(ConditionSearchMemoIchiranHyoji), ConditionSearchMemoIchiranHyoji.BOOKING_CONFIRMATION))) + "= 'True'" + ")");
            commandText.AppendLine("    OR (INFOLST.MEMO_BUNRUI = " + MemoClassification.NOSHOW_Confirmation + " AND " + setParam(CommonKyushuUtil.getEnumValue(typeof(ConditionSearchMemoIchiranHyoji), ConditionSearchMemoIchiranHyoji.NOSHOW_CONFIRMATION), paramInfoList(CommonKyushuUtil.getEnumValue(typeof(ConditionSearchMemoIchiranHyoji), ConditionSearchMemoIchiranHyoji.NOSHOW_CONFIRMATION))) + "= 'True'" + ")");
            commandText.AppendLine("    OR (INFOLST.MEMO_BUNRUI = " + MemoClassification.cancellation_Of_Liaison + " AND " + setParam(CommonKyushuUtil.getEnumValue(typeof(ConditionSearchMemoIchiranHyoji), ConditionSearchMemoIchiranHyoji.CANCELLATION_OF_LIAISON), paramInfoList(CommonKyushuUtil.getEnumValue(typeof(ConditionSearchMemoIchiranHyoji), ConditionSearchMemoIchiranHyoji.CANCELLATION_OF_LIAISON))) + "= 'True'" + ")");
            commandText.AppendLine("    OR (INFOLST.MEMO_BUNRUI = " + MemoClassification.dubricity_Check + " AND " + setParam(CommonKyushuUtil.getEnumValue(typeof(ConditionSearchMemoIchiranHyoji), ConditionSearchMemoIchiranHyoji.DUBRICITY_CHECK), paramInfoList(CommonKyushuUtil.getEnumValue(typeof(ConditionSearchMemoIchiranHyoji), ConditionSearchMemoIchiranHyoji.DUBRICITY_CHECK))) + "= 'True'" + ")");
            commandText.AppendLine("    OR (INFOLST.MEMO_BUNRUI = " + MemoClassification.weight + " AND " + setParam(CommonKyushuUtil.getEnumValue(typeof(ConditionSearchMemoIchiranHyoji), ConditionSearchMemoIchiranHyoji.WEIGHT), paramInfoList(CommonKyushuUtil.getEnumValue(typeof(ConditionSearchMemoIchiranHyoji), ConditionSearchMemoIchiranHyoji.WEIGHT))) + "= 'True'" + ")");
            commandText.AppendLine("    OR (INFOLST.MEMO_BUNRUI = " + MemoClassification.request + " AND " + setParam(CommonKyushuUtil.getEnumValue(typeof(ConditionSearchMemoIchiranHyoji), ConditionSearchMemoIchiranHyoji.REQUEST), paramInfoList(CommonKyushuUtil.getEnumValue(typeof(ConditionSearchMemoIchiranHyoji), ConditionSearchMemoIchiranHyoji.REQUEST))) + "= 'True'" + ")))");
        }
        // commandText.AppendLine("ORDER BY INFOLST.RESER_NM, INFOLST.SYSTEM_UPDATE_DAY ASC FETCH FIRST 101 ROWS ONLY")
        commandText.AppendLine("ORDER BY INFOLST.RESER_NM, INFOLST.SYSTEM_UPDATE_DAY ASC ");
        return commandText;
    }
    #endregion

    #region executeSelectList
    /// <summary>
    /// DB接続用
    /// </summary>
    /// <param name="_sqlString"></param>
    /// <returns></returns>
    /// <remarks></remarks>
    public DataTable executeSelectList(string _sqlString)
    {
        OracleTransaction oracleTransaction = default;
        DataTable returnValue = default;
        string sqlString = _sqlString;
        try
        {
            // SQL実施
            returnValue = getDataTable(sqlString);
        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
        }

        return returnValue;
    }
    #endregion

    #region getDataSource
    /// <summary>
    /// getDataSource
    /// </summary>
    /// <param name="_paramInfoList"></param>
    /// <returns></returns>
    public DataTable getDataSource(Hashtable _paramInfoList, string _sqlString)
    {
        string sqlString = _sqlString;
        var dataSourc = executeSelectList(sqlString);
        return dataSourc;
    }
    #endregion
}