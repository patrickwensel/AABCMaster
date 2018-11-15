using DevExpress.XtraReports.UI;

/// <summary>
/// Summary description for BillingReport
/// </summary>
public class BillingReport : DevExpress.XtraReports.UI.XtraReport
{
    private DevExpress.XtraReports.UI.DetailBand Detail;
    private DevExpress.XtraReports.UI.TopMarginBand TopMargin;
    private DevExpress.XtraReports.UI.BottomMarginBand BottomMargin;
    private DevExpress.XtraReports.Parameters.Parameter CaseID;
    private DevExpress.XtraReports.Parameters.Parameter firstDayOfMonthPeriod;
    private ReportHeaderBand ReportHeader;
    private XRLabel xrLabel1;
    private XRPictureBox xrPictureBox1;
    private PageFooterBand PageFooter;
    private XRPageInfo xrPageInfo2;
    private XRLabel xrLabel2;
    private XRPageInfo xrPageInfo1;
    private DevExpress.DataAccess.Sql.SqlDataSource BillingReportDS;
    private DetailReportBand DetailReport;
    private DetailBand Detail1;
    private XRLabel xrLabel5;
    private XRLabel xrLabel9;
    private XRLabel xrLabel8;
    private DetailReportBand DetailReport1;
    private DetailBand Detail2;
    private XRLabel xrLabel16;
    private XRLabel xrLabel15;
    private XRLabel xrLabel14;
    private XRLabel xrLabel13;
    private XRLabel xrLabel12;
    private XRPanel xrPanel1;
    private XRLabel xrLabel6;
    private XRLabel xrLabel4;
    private XRLabel xrLabel3;
    private XRLabel xrLabel10;
    private XRLabel xrLabel11;
    private CalculatedField PatientName;
    private CalculatedField ProviderName;
    private XRPanel xrPanel2;
    private XRLabel xrLabel17;
    private XRLabel xrLabel7;
    private CalculatedField ProviderTypeLabel;
    private XRLine xrLine1;
    private XRLabel xrLabel18;
    private XRLabel xrLabel24;
    private XRLabel xrLabel23;
    private XRLabel xrLabel22;
    private XRLabel xrLabel21;
    private XRLabel xrLabel20;
    private XRLabel xrLabel19;
    private XRLabel xrLabel26;
    public XRLabel lblReportID;
    private XRLabel xrLabel27;
    private DevExpress.XtraReports.Parameters.Parameter BillingRef;

    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    public BillingReport()
    {
        InitializeComponent();
        //
        // TODO: Add constructor logic here
        //
    }

    /// <summary> 
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    #region Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        this.components = new System.ComponentModel.Container();
        System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BillingReport));
        DevExpress.DataAccess.Sql.StoredProcQuery storedProcQuery1 = new DevExpress.DataAccess.Sql.StoredProcQuery();
        DevExpress.DataAccess.Sql.QueryParameter queryParameter1 = new DevExpress.DataAccess.Sql.QueryParameter();
        DevExpress.DataAccess.Sql.StoredProcQuery storedProcQuery2 = new DevExpress.DataAccess.Sql.StoredProcQuery();
        DevExpress.DataAccess.Sql.QueryParameter queryParameter2 = new DevExpress.DataAccess.Sql.QueryParameter();
        DevExpress.DataAccess.Sql.QueryParameter queryParameter3 = new DevExpress.DataAccess.Sql.QueryParameter();
        DevExpress.DataAccess.Sql.QueryParameter queryParameter4 = new DevExpress.DataAccess.Sql.QueryParameter();
        DevExpress.DataAccess.Sql.StoredProcQuery storedProcQuery3 = new DevExpress.DataAccess.Sql.StoredProcQuery();
        DevExpress.DataAccess.Sql.QueryParameter queryParameter5 = new DevExpress.DataAccess.Sql.QueryParameter();
        DevExpress.DataAccess.Sql.QueryParameter queryParameter6 = new DevExpress.DataAccess.Sql.QueryParameter();
        DevExpress.DataAccess.Sql.QueryParameter queryParameter7 = new DevExpress.DataAccess.Sql.QueryParameter();
        DevExpress.DataAccess.Sql.MasterDetailInfo masterDetailInfo1 = new DevExpress.DataAccess.Sql.MasterDetailInfo();
        DevExpress.DataAccess.Sql.RelationColumnInfo relationColumnInfo1 = new DevExpress.DataAccess.Sql.RelationColumnInfo();
        DevExpress.DataAccess.Sql.MasterDetailInfo masterDetailInfo2 = new DevExpress.DataAccess.Sql.MasterDetailInfo();
        DevExpress.DataAccess.Sql.RelationColumnInfo relationColumnInfo2 = new DevExpress.DataAccess.Sql.RelationColumnInfo();
        this.Detail = new DevExpress.XtraReports.UI.DetailBand();
        this.TopMargin = new DevExpress.XtraReports.UI.TopMarginBand();
        this.BottomMargin = new DevExpress.XtraReports.UI.BottomMarginBand();
        this.CaseID = new DevExpress.XtraReports.Parameters.Parameter();
        this.firstDayOfMonthPeriod = new DevExpress.XtraReports.Parameters.Parameter();
        this.ReportHeader = new DevExpress.XtraReports.UI.ReportHeaderBand();
        this.lblReportID = new DevExpress.XtraReports.UI.XRLabel();
        this.xrLabel27 = new DevExpress.XtraReports.UI.XRLabel();
        this.xrLabel1 = new DevExpress.XtraReports.UI.XRLabel();
        this.xrPictureBox1 = new DevExpress.XtraReports.UI.XRPictureBox();
        this.xrLabel26 = new DevExpress.XtraReports.UI.XRLabel();
        this.PageFooter = new DevExpress.XtraReports.UI.PageFooterBand();
        this.xrPageInfo2 = new DevExpress.XtraReports.UI.XRPageInfo();
        this.xrLabel2 = new DevExpress.XtraReports.UI.XRLabel();
        this.xrPageInfo1 = new DevExpress.XtraReports.UI.XRPageInfo();
        this.BillingReportDS = new DevExpress.DataAccess.Sql.SqlDataSource(this.components);
        this.DetailReport = new DevExpress.XtraReports.UI.DetailReportBand();
        this.Detail1 = new DevExpress.XtraReports.UI.DetailBand();
        this.xrLabel24 = new DevExpress.XtraReports.UI.XRLabel();
        this.xrLabel23 = new DevExpress.XtraReports.UI.XRLabel();
        this.xrLabel22 = new DevExpress.XtraReports.UI.XRLabel();
        this.xrLabel21 = new DevExpress.XtraReports.UI.XRLabel();
        this.xrLabel20 = new DevExpress.XtraReports.UI.XRLabel();
        this.xrLabel19 = new DevExpress.XtraReports.UI.XRLabel();
        this.xrLine1 = new DevExpress.XtraReports.UI.XRLine();
        this.xrPanel2 = new DevExpress.XtraReports.UI.XRPanel();
        this.xrLabel17 = new DevExpress.XtraReports.UI.XRLabel();
        this.xrLabel7 = new DevExpress.XtraReports.UI.XRLabel();
        this.xrLabel8 = new DevExpress.XtraReports.UI.XRLabel();
        this.xrLabel5 = new DevExpress.XtraReports.UI.XRLabel();
        this.xrPanel1 = new DevExpress.XtraReports.UI.XRPanel();
        this.xrLabel6 = new DevExpress.XtraReports.UI.XRLabel();
        this.xrLabel4 = new DevExpress.XtraReports.UI.XRLabel();
        this.xrLabel3 = new DevExpress.XtraReports.UI.XRLabel();
        this.xrLabel10 = new DevExpress.XtraReports.UI.XRLabel();
        this.xrLabel11 = new DevExpress.XtraReports.UI.XRLabel();
        this.xrLabel9 = new DevExpress.XtraReports.UI.XRLabel();
        this.DetailReport1 = new DevExpress.XtraReports.UI.DetailReportBand();
        this.Detail2 = new DevExpress.XtraReports.UI.DetailBand();
        this.xrLabel18 = new DevExpress.XtraReports.UI.XRLabel();
        this.xrLabel16 = new DevExpress.XtraReports.UI.XRLabel();
        this.xrLabel15 = new DevExpress.XtraReports.UI.XRLabel();
        this.xrLabel14 = new DevExpress.XtraReports.UI.XRLabel();
        this.xrLabel13 = new DevExpress.XtraReports.UI.XRLabel();
        this.xrLabel12 = new DevExpress.XtraReports.UI.XRLabel();
        this.PatientName = new DevExpress.XtraReports.UI.CalculatedField();
        this.ProviderName = new DevExpress.XtraReports.UI.CalculatedField();
        this.ProviderTypeLabel = new DevExpress.XtraReports.UI.CalculatedField();
        this.BillingRef = new DevExpress.XtraReports.Parameters.Parameter();
        ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
        // 
        // Detail
        // 
        this.Detail.Dpi = 100F;
        this.Detail.HeightF = 0F;
        this.Detail.Name = "Detail";
        this.Detail.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100F);
        this.Detail.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
        // 
        // TopMargin
        // 
        this.TopMargin.Dpi = 100F;
        this.TopMargin.HeightF = 16.75002F;
        this.TopMargin.Name = "TopMargin";
        this.TopMargin.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100F);
        this.TopMargin.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
        // 
        // BottomMargin
        // 
        this.BottomMargin.Dpi = 100F;
        this.BottomMargin.HeightF = 20.83333F;
        this.BottomMargin.Name = "BottomMargin";
        this.BottomMargin.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100F);
        this.BottomMargin.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
        // 
        // CaseID
        // 
        this.CaseID.Description = "CaseID";
        this.CaseID.Name = "CaseID";
        this.CaseID.Type = typeof(int);
        this.CaseID.ValueInfo = "383";
        this.CaseID.Visible = false;
        // 
        // firstDayOfMonthPeriod
        // 
        this.firstDayOfMonthPeriod.Name = "firstDayOfMonthPeriod";
        this.firstDayOfMonthPeriod.Type = typeof(System.DateTime);
        this.firstDayOfMonthPeriod.ValueInfo = "2016-05-01";
        this.firstDayOfMonthPeriod.Visible = false;
        // 
        // ReportHeader
        // 
        this.ReportHeader.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.lblReportID,
            this.xrLabel27,
            this.xrLabel1,
            this.xrPictureBox1,
            this.xrLabel26});
        this.ReportHeader.Dpi = 100F;
        this.ReportHeader.HeightF = 136.25F;
        this.ReportHeader.Name = "ReportHeader";
        // 
        // lblReportID
        // 
        this.lblReportID.Dpi = 100F;
        this.lblReportID.Font = new System.Drawing.Font("Trebuchet MS", 14F);
        this.lblReportID.ForeColor = System.Drawing.Color.Red;
        this.lblReportID.LocationFloat = new DevExpress.Utils.PointFloat(649.5993F, 0F);
        this.lblReportID.Name = "lblReportID";
        this.lblReportID.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
        this.lblReportID.SizeF = new System.Drawing.SizeF(100F, 23F);
        this.lblReportID.StylePriority.UseFont = false;
        this.lblReportID.StylePriority.UseForeColor = false;
        this.lblReportID.Text = "PREVIEW";
        // 
        // xrLabel27
        // 
        this.xrLabel27.Dpi = 100F;
        this.xrLabel27.Font = new System.Drawing.Font("Trebuchet MS", 14F);
        this.xrLabel27.LocationFloat = new DevExpress.Utils.PointFloat(510.8974F, 0F);
        this.xrLabel27.Name = "xrLabel27";
        this.xrLabel27.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
        this.xrLabel27.SizeF = new System.Drawing.SizeF(138.7019F, 23F);
        this.xrLabel27.StylePriority.UseFont = false;
        this.xrLabel27.StylePriority.UseTextAlignment = false;
        this.xrLabel27.Text = "Billing Report:";
        this.xrLabel27.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopRight;
        // 
        // xrLabel1
        // 
        this.xrLabel1.Dpi = 100F;
        this.xrLabel1.Font = new System.Drawing.Font("Trebuchet MS", 9.75F);
        this.xrLabel1.LocationFloat = new DevExpress.Utils.PointFloat(168.75F, 16.66667F);
        this.xrLabel1.Multiline = true;
        this.xrLabel1.Name = "xrLabel1";
        this.xrLabel1.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
        this.xrLabel1.SizeF = new System.Drawing.SizeF(328.2852F, 105.2917F);
        this.xrLabel1.StylePriority.UseFont = false;
        this.xrLabel1.Text = "Applied Behavioral Mental Health Counseling P.C.\r\n1970 52nd St, Bmt, Brooklyn, NY" +
" 11204\r\nPhone: 718-360-9548\r\nFax: 718-874-0052\r\ntimesheets@appliedabc.com\r\nwww.a" +
"ppliedabc.com";
        // 
        // xrPictureBox1
        // 
        this.xrPictureBox1.Dpi = 100F;
        this.xrPictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("xrPictureBox1.Image")));
        this.xrPictureBox1.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0F);
        this.xrPictureBox1.Name = "xrPictureBox1";
        this.xrPictureBox1.SizeF = new System.Drawing.SizeF(154.1667F, 136.25F);
        this.xrPictureBox1.Sizing = DevExpress.XtraPrinting.ImageSizeMode.StretchImage;
        // 
        // xrLabel26
        // 
        this.xrLabel26.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding(this.firstDayOfMonthPeriod, "Text", "{0:MMMM, yyyy}")});
        this.xrLabel26.Dpi = 100F;
        this.xrLabel26.Font = new System.Drawing.Font("Trebuchet MS", 9.75F);
        this.xrLabel26.LocationFloat = new DevExpress.Utils.PointFloat(510.8974F, 23F);
        this.xrLabel26.Name = "xrLabel26";
        this.xrLabel26.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
        this.xrLabel26.SizeF = new System.Drawing.SizeF(138.7019F, 23.00001F);
        this.xrLabel26.StylePriority.UseFont = false;
        this.xrLabel26.StylePriority.UseTextAlignment = false;
        this.xrLabel26.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopRight;
        // 
        // PageFooter
        // 
        this.PageFooter.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrPageInfo2,
            this.xrLabel2,
            this.xrPageInfo1});
        this.PageFooter.Dpi = 100F;
        this.PageFooter.HeightF = 25F;
        this.PageFooter.Name = "PageFooter";
        // 
        // xrPageInfo2
        // 
        this.xrPageInfo2.Dpi = 100F;
        this.xrPageInfo2.Font = new System.Drawing.Font("Trebuchet MS", 9.75F);
        this.xrPageInfo2.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0F);
        this.xrPageInfo2.Name = "xrPageInfo2";
        this.xrPageInfo2.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
        this.xrPageInfo2.PageInfo = DevExpress.XtraPrinting.PageInfo.DateTime;
        this.xrPageInfo2.SizeF = new System.Drawing.SizeF(212.9808F, 23F);
        this.xrPageInfo2.StylePriority.UseFont = false;
        // 
        // xrLabel2
        // 
        this.xrLabel2.Dpi = 100F;
        this.xrLabel2.Font = new System.Drawing.Font("Trebuchet MS", 9.75F);
        this.xrLabel2.LocationFloat = new DevExpress.Utils.PointFloat(640.625F, 0F);
        this.xrLabel2.Name = "xrLabel2";
        this.xrLabel2.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
        this.xrLabel2.SizeF = new System.Drawing.SizeF(35.41667F, 23F);
        this.xrLabel2.StylePriority.UseFont = false;
        this.xrLabel2.Text = "Page";
        // 
        // xrPageInfo1
        // 
        this.xrPageInfo1.Dpi = 100F;
        this.xrPageInfo1.Font = new System.Drawing.Font("Trebuchet MS", 9.75F);
        this.xrPageInfo1.LocationFloat = new DevExpress.Utils.PointFloat(676.0417F, 0F);
        this.xrPageInfo1.Name = "xrPageInfo1";
        this.xrPageInfo1.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
        this.xrPageInfo1.SizeF = new System.Drawing.SizeF(63.95831F, 23F);
        this.xrPageInfo1.StylePriority.UseFont = false;
        // 
        // BillingReportDS
        // 
        this.BillingReportDS.ConnectionName = "CoreConnection";
        this.BillingReportDS.Name = "BillingReportDS";
        storedProcQuery1.Name = "webreports_BillingCaseInfo";
        queryParameter1.Name = "@CaseID";
        queryParameter1.Type = typeof(DevExpress.DataAccess.Expression);
        queryParameter1.Value = new DevExpress.DataAccess.Expression("[Parameters.CaseID]", typeof(int));
        storedProcQuery1.Parameters.Add(queryParameter1);
        storedProcQuery1.StoredProcName = "webreports.BillingCaseInfo";
        storedProcQuery2.Name = "webreports_BillingProviderInfo";
        queryParameter2.Name = "@CaseID";
        queryParameter2.Type = typeof(DevExpress.DataAccess.Expression);
        queryParameter2.Value = new DevExpress.DataAccess.Expression("[Parameters.CaseID]", typeof(int));
        queryParameter3.Name = "@FirstDayOfMonth";
        queryParameter3.Type = typeof(DevExpress.DataAccess.Expression);
        queryParameter3.Value = new DevExpress.DataAccess.Expression("[Parameters.firstDayOfMonthPeriod]", typeof(System.DateTime));
        queryParameter4.Name = "@BillingRef";
        queryParameter4.Type = typeof(DevExpress.DataAccess.Expression);
        queryParameter4.Value = new DevExpress.DataAccess.Expression("[Parameters.BillingRef]", typeof(string));
        storedProcQuery2.Parameters.Add(queryParameter2);
        storedProcQuery2.Parameters.Add(queryParameter3);
        storedProcQuery2.Parameters.Add(queryParameter4);
        storedProcQuery2.StoredProcName = "webreports.BillingProviderInfo";
        storedProcQuery3.Name = "webreports_BillingHoursInfo";
        queryParameter5.Name = "@CaseID";
        queryParameter5.Type = typeof(DevExpress.DataAccess.Expression);
        queryParameter5.Value = new DevExpress.DataAccess.Expression("[Parameters.CaseID]", typeof(int));
        queryParameter6.Name = "@FirstDayOfMonth";
        queryParameter6.Type = typeof(DevExpress.DataAccess.Expression);
        queryParameter6.Value = new DevExpress.DataAccess.Expression("[Parameters.firstDayOfMonthPeriod]", typeof(System.DateTime));
        queryParameter7.Name = "@BillingRef";
        queryParameter7.Type = typeof(DevExpress.DataAccess.Expression);
        queryParameter7.Value = new DevExpress.DataAccess.Expression("[Parameters.BillingRef]", typeof(string));
        storedProcQuery3.Parameters.Add(queryParameter5);
        storedProcQuery3.Parameters.Add(queryParameter6);
        storedProcQuery3.Parameters.Add(queryParameter7);
        storedProcQuery3.StoredProcName = "webreports.BillingHoursInfo";
        this.BillingReportDS.Queries.AddRange(new DevExpress.DataAccess.Sql.SqlQuery[] {
            storedProcQuery1,
            storedProcQuery2,
            storedProcQuery3});
        masterDetailInfo1.DetailQueryName = "webreports_BillingProviderInfo";
        relationColumnInfo1.NestedKeyColumn = "CaseID";
        relationColumnInfo1.ParentKeyColumn = "CaseID";
        masterDetailInfo1.KeyColumns.Add(relationColumnInfo1);
        masterDetailInfo1.MasterQueryName = "webreports_BillingCaseInfo";
        masterDetailInfo2.DetailQueryName = "webreports_BillingHoursInfo";
        relationColumnInfo2.NestedKeyColumn = "ProviderID";
        relationColumnInfo2.ParentKeyColumn = "ProviderID";
        masterDetailInfo2.KeyColumns.Add(relationColumnInfo2);
        masterDetailInfo2.MasterQueryName = "webreports_BillingProviderInfo";
        this.BillingReportDS.Relations.AddRange(new DevExpress.DataAccess.Sql.MasterDetailInfo[] {
            masterDetailInfo1,
            masterDetailInfo2});
        this.BillingReportDS.ResultSchemaSerializable = resources.GetString("BillingReportDS.ResultSchemaSerializable");
        // 
        // DetailReport
        // 
        this.DetailReport.Bands.AddRange(new DevExpress.XtraReports.UI.Band[] {
            this.Detail1,
            this.DetailReport1});
        this.DetailReport.DataMember = "webreports_BillingCaseInfo.webreports_BillingCaseInfowebreports_BillingProviderIn" +
"fo";
        this.DetailReport.DataSource = this.BillingReportDS;
        this.DetailReport.Dpi = 100F;
        this.DetailReport.Level = 0;
        this.DetailReport.Name = "DetailReport";
        // 
        // Detail1
        // 
        this.Detail1.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrLabel24,
            this.xrLabel23,
            this.xrLabel22,
            this.xrLabel21,
            this.xrLabel20,
            this.xrLabel19,
            this.xrLine1,
            this.xrPanel2,
            this.xrPanel1,
            this.xrLabel11,
            this.xrLabel9});
        this.Detail1.Dpi = 100F;
        this.Detail1.HeightF = 128.9295F;
        this.Detail1.Name = "Detail1";
        // 
        // xrLabel24
        // 
        this.xrLabel24.Dpi = 100F;
        this.xrLabel24.Font = new System.Drawing.Font("Trebuchet MS", 9.75F, System.Drawing.FontStyle.Bold);
        this.xrLabel24.LocationFloat = new DevExpress.Utils.PointFloat(480.1284F, 105.9295F);
        this.xrLabel24.Name = "xrLabel24";
        this.xrLabel24.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
        this.xrLabel24.SizeF = new System.Drawing.SizeF(269.8717F, 23F);
        this.xrLabel24.StylePriority.UseFont = false;
        this.xrLabel24.Text = "Notes";
        // 
        // xrLabel23
        // 
        this.xrLabel23.Dpi = 100F;
        this.xrLabel23.Font = new System.Drawing.Font("Trebuchet MS", 9.75F, System.Drawing.FontStyle.Bold);
        this.xrLabel23.LocationFloat = new DevExpress.Utils.PointFloat(328.045F, 105.9295F);
        this.xrLabel23.Name = "xrLabel23";
        this.xrLabel23.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
        this.xrLabel23.SizeF = new System.Drawing.SizeF(152.0833F, 23F);
        this.xrLabel23.StylePriority.UseFont = false;
        this.xrLabel23.Text = "Service";
        // 
        // xrLabel22
        // 
        this.xrLabel22.Dpi = 100F;
        this.xrLabel22.Font = new System.Drawing.Font("Trebuchet MS", 9.75F, System.Drawing.FontStyle.Bold);
        this.xrLabel22.LocationFloat = new DevExpress.Utils.PointFloat(272.9168F, 105.9295F);
        this.xrLabel22.Name = "xrLabel22";
        this.xrLabel22.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
        this.xrLabel22.SizeF = new System.Drawing.SizeF(55.12814F, 23F);
        this.xrLabel22.StylePriority.UseFont = false;
        this.xrLabel22.StylePriority.UseTextAlignment = false;
        this.xrLabel22.Text = "Hours";
        this.xrLabel22.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopCenter;
        // 
        // xrLabel21
        // 
        this.xrLabel21.Dpi = 100F;
        this.xrLabel21.Font = new System.Drawing.Font("Trebuchet MS", 9.75F, System.Drawing.FontStyle.Bold);
        this.xrLabel21.LocationFloat = new DevExpress.Utils.PointFloat(172.9168F, 105.9295F);
        this.xrLabel21.Name = "xrLabel21";
        this.xrLabel21.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
        this.xrLabel21.SizeF = new System.Drawing.SizeF(100F, 23F);
        this.xrLabel21.StylePriority.UseFont = false;
        this.xrLabel21.StylePriority.UseTextAlignment = false;
        this.xrLabel21.Text = "Time Out";
        this.xrLabel21.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopCenter;
        // 
        // xrLabel20
        // 
        this.xrLabel20.Dpi = 100F;
        this.xrLabel20.Font = new System.Drawing.Font("Trebuchet MS", 9.75F, System.Drawing.FontStyle.Bold);
        this.xrLabel20.LocationFloat = new DevExpress.Utils.PointFloat(72.91676F, 105.9295F);
        this.xrLabel20.Name = "xrLabel20";
        this.xrLabel20.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
        this.xrLabel20.SizeF = new System.Drawing.SizeF(100F, 23F);
        this.xrLabel20.StylePriority.UseFont = false;
        this.xrLabel20.StylePriority.UseTextAlignment = false;
        this.xrLabel20.Text = "Time In";
        this.xrLabel20.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopCenter;
        // 
        // xrLabel19
        // 
        this.xrLabel19.Dpi = 100F;
        this.xrLabel19.Font = new System.Drawing.Font("Trebuchet MS", 9.75F, System.Drawing.FontStyle.Bold);
        this.xrLabel19.LocationFloat = new DevExpress.Utils.PointFloat(9.781275E-05F, 105.9295F);
        this.xrLabel19.Name = "xrLabel19";
        this.xrLabel19.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
        this.xrLabel19.SizeF = new System.Drawing.SizeF(72.91681F, 23F);
        this.xrLabel19.StylePriority.UseFont = false;
        this.xrLabel19.StylePriority.UseTextAlignment = false;
        this.xrLabel19.Text = "Date";
        this.xrLabel19.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopCenter;
        // 
        // xrLine1
        // 
        this.xrLine1.Dpi = 100F;
        this.xrLine1.LocationFloat = new DevExpress.Utils.PointFloat(9.781275E-05F, 98.15387F);
        this.xrLine1.Name = "xrLine1";
        this.xrLine1.SizeF = new System.Drawing.SizeF(750F, 2F);
        // 
        // xrPanel2
        // 
        this.xrPanel2.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrLabel17,
            this.xrLabel7,
            this.xrLabel8,
            this.xrLabel5});
        this.xrPanel2.Dpi = 100F;
        this.xrPanel2.LocationFloat = new DevExpress.Utils.PointFloat(400F, 52.15385F);
        this.xrPanel2.Name = "xrPanel2";
        this.xrPanel2.SizeF = new System.Drawing.SizeF(300F, 46F);
        // 
        // xrLabel17
        // 
        this.xrLabel17.Dpi = 100F;
        this.xrLabel17.Font = new System.Drawing.Font("Trebuchet MS", 9.75F, System.Drawing.FontStyle.Bold);
        this.xrLabel17.LocationFloat = new DevExpress.Utils.PointFloat(9.999939F, 22.99998F);
        this.xrLabel17.Name = "xrLabel17";
        this.xrLabel17.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
        this.xrLabel17.SizeF = new System.Drawing.SizeF(70.12836F, 23F);
        this.xrLabel17.StylePriority.UseFont = false;
        this.xrLabel17.StylePriority.UseTextAlignment = false;
        this.xrLabel17.Text = "Phone #:";
        this.xrLabel17.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopRight;
        // 
        // xrLabel7
        // 
        this.xrLabel7.Dpi = 100F;
        this.xrLabel7.Font = new System.Drawing.Font("Trebuchet MS", 9.75F, System.Drawing.FontStyle.Bold);
        this.xrLabel7.LocationFloat = new DevExpress.Utils.PointFloat(9.999939F, 0F);
        this.xrLabel7.Name = "xrLabel7";
        this.xrLabel7.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
        this.xrLabel7.SizeF = new System.Drawing.SizeF(70.12833F, 23F);
        this.xrLabel7.StylePriority.UseFont = false;
        this.xrLabel7.StylePriority.UseTextAlignment = false;
        this.xrLabel7.Text = "Phone #:";
        this.xrLabel7.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopRight;
        // 
        // xrLabel8
        // 
        this.xrLabel8.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "webreports_BillingCaseInfo.webreports_BillingCaseInfowebreports_BillingProviderIn" +
                    "fo.ProviderPrimaryPhone")});
        this.xrLabel8.Dpi = 100F;
        this.xrLabel8.Font = new System.Drawing.Font("Trebuchet MS", 9.75F);
        this.xrLabel8.LocationFloat = new DevExpress.Utils.PointFloat(80.12836F, 22.99998F);
        this.xrLabel8.Name = "xrLabel8";
        this.xrLabel8.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
        this.xrLabel8.SizeF = new System.Drawing.SizeF(209.8716F, 23.00002F);
        this.xrLabel8.StylePriority.UseFont = false;
        this.xrLabel8.Text = "xrLabel8";
        // 
        // xrLabel5
        // 
        this.xrLabel5.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "webreports_BillingCaseInfo.PatientPhone")});
        this.xrLabel5.Dpi = 100F;
        this.xrLabel5.Font = new System.Drawing.Font("Trebuchet MS", 9.75F);
        this.xrLabel5.LocationFloat = new DevExpress.Utils.PointFloat(80.12836F, 0F);
        this.xrLabel5.Name = "xrLabel5";
        this.xrLabel5.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
        this.xrLabel5.SizeF = new System.Drawing.SizeF(209.8716F, 23.00001F);
        this.xrLabel5.StylePriority.UseFont = false;
        this.xrLabel5.Text = "xrLabel5";
        // 
        // xrPanel1
        // 
        this.xrPanel1.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrLabel6,
            this.xrLabel4,
            this.xrLabel3,
            this.xrLabel10});
        this.xrPanel1.Dpi = 100F;
        this.xrPanel1.LocationFloat = new DevExpress.Utils.PointFloat(89.98392F, 52.15385F);
        this.xrPanel1.Name = "xrPanel1";
        this.xrPanel1.SizeF = new System.Drawing.SizeF(300F, 46F);
        // 
        // xrLabel6
        // 
        this.xrLabel6.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "webreports_BillingCaseInfo.webreports_BillingCaseInfowebreports_BillingProviderIn" +
                    "fo.ProviderName")});
        this.xrLabel6.Dpi = 100F;
        this.xrLabel6.Font = new System.Drawing.Font("Trebuchet MS", 9.75F);
        this.xrLabel6.LocationFloat = new DevExpress.Utils.PointFloat(110F, 22.99998F);
        this.xrLabel6.Name = "xrLabel6";
        this.xrLabel6.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
        this.xrLabel6.SizeF = new System.Drawing.SizeF(190F, 23F);
        this.xrLabel6.StylePriority.UseFont = false;
        this.xrLabel6.Text = "xrLabel6";
        // 
        // xrLabel4
        // 
        this.xrLabel4.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "webreports_BillingCaseInfo.PatientName")});
        this.xrLabel4.Dpi = 100F;
        this.xrLabel4.Font = new System.Drawing.Font("Trebuchet MS", 9.75F);
        this.xrLabel4.LocationFloat = new DevExpress.Utils.PointFloat(110F, 0F);
        this.xrLabel4.Name = "xrLabel4";
        this.xrLabel4.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
        this.xrLabel4.SizeF = new System.Drawing.SizeF(190F, 23F);
        this.xrLabel4.StylePriority.UseFont = false;
        this.xrLabel4.Text = "xrLabel4";
        // 
        // xrLabel3
        // 
        this.xrLabel3.Dpi = 100F;
        this.xrLabel3.Font = new System.Drawing.Font("Trebuchet MS", 9.75F, System.Drawing.FontStyle.Bold);
        this.xrLabel3.LocationFloat = new DevExpress.Utils.PointFloat(9.999984F, 0F);
        this.xrLabel3.Name = "xrLabel3";
        this.xrLabel3.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
        this.xrLabel3.SizeF = new System.Drawing.SizeF(100F, 23F);
        this.xrLabel3.StylePriority.UseFont = false;
        this.xrLabel3.StylePriority.UseTextAlignment = false;
        this.xrLabel3.Text = "Patient Name:";
        this.xrLabel3.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopRight;
        // 
        // xrLabel10
        // 
        this.xrLabel10.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "webreports_BillingCaseInfo.webreports_BillingCaseInfowebreports_BillingProviderIn" +
                    "fo.ProviderTypeLabel")});
        this.xrLabel10.Dpi = 100F;
        this.xrLabel10.Font = new System.Drawing.Font("Trebuchet MS", 9.75F, System.Drawing.FontStyle.Bold);
        this.xrLabel10.LocationFloat = new DevExpress.Utils.PointFloat(9.999984F, 22.99998F);
        this.xrLabel10.Name = "xrLabel10";
        this.xrLabel10.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
        this.xrLabel10.SizeF = new System.Drawing.SizeF(100F, 23F);
        this.xrLabel10.StylePriority.UseFont = false;
        this.xrLabel10.StylePriority.UseTextAlignment = false;
        this.xrLabel10.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopRight;
        // 
        // xrLabel11
        // 
        this.xrLabel11.Dpi = 100F;
        this.xrLabel11.Font = new System.Drawing.Font("Trebuchet MS", 16F, System.Drawing.FontStyle.Bold);
        this.xrLabel11.LocationFloat = new DevExpress.Utils.PointFloat(366.7467F, 10.00001F);
        this.xrLabel11.Name = "xrLabel11";
        this.xrLabel11.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
        this.xrLabel11.SizeF = new System.Drawing.SizeF(100F, 23F);
        this.xrLabel11.StylePriority.UseFont = false;
        this.xrLabel11.Text = "Hours";
        // 
        // xrLabel9
        // 
        this.xrLabel9.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "webreports_BillingCaseInfo.webreports_BillingCaseInfowebreports_BillingProviderIn" +
                    "fo.ProviderTypeCode")});
        this.xrLabel9.Dpi = 100F;
        this.xrLabel9.Font = new System.Drawing.Font("Trebuchet MS", 16F, System.Drawing.FontStyle.Bold);
        this.xrLabel9.LocationFloat = new DevExpress.Utils.PointFloat(145.5128F, 10.00001F);
        this.xrLabel9.Name = "xrLabel9";
        this.xrLabel9.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
        this.xrLabel9.SizeF = new System.Drawing.SizeF(221.234F, 31.01283F);
        this.xrLabel9.StylePriority.UseFont = false;
        this.xrLabel9.StylePriority.UseTextAlignment = false;
        this.xrLabel9.Text = "xrLabel9";
        this.xrLabel9.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopRight;
        // 
        // DetailReport1
        // 
        this.DetailReport1.Bands.AddRange(new DevExpress.XtraReports.UI.Band[] {
            this.Detail2});
        this.DetailReport1.DataMember = "webreports_BillingCaseInfo.webreports_BillingCaseInfowebreports_BillingProviderIn" +
"fo.webreports_BillingProviderInfowebreports_BillingHoursInfo";
        this.DetailReport1.DataSource = this.BillingReportDS;
        this.DetailReport1.Dpi = 100F;
        this.DetailReport1.Level = 0;
        this.DetailReport1.Name = "DetailReport1";
        // 
        // Detail2
        // 
        this.Detail2.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrLabel18,
            this.xrLabel16,
            this.xrLabel15,
            this.xrLabel14,
            this.xrLabel13,
            this.xrLabel12});
        this.Detail2.Dpi = 100F;
        this.Detail2.HeightF = 23.80127F;
        this.Detail2.Name = "Detail2";
        // 
        // xrLabel18
        // 
        this.xrLabel18.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "webreports_BillingCaseInfo.webreports_BillingCaseInfowebreports_BillingProviderIn" +
                    "fo.webreports_BillingProviderInfowebreports_BillingHoursInfo.HoursBillable")});
        this.xrLabel18.Dpi = 100F;
        this.xrLabel18.Font = new System.Drawing.Font("Trebuchet MS", 9.75F);
        this.xrLabel18.LocationFloat = new DevExpress.Utils.PointFloat(272.9168F, 0F);
        this.xrLabel18.Name = "xrLabel18";
        this.xrLabel18.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
        this.xrLabel18.SizeF = new System.Drawing.SizeF(55.1282F, 23F);
        this.xrLabel18.StylePriority.UseFont = false;
        this.xrLabel18.StylePriority.UseTextAlignment = false;
        this.xrLabel18.Text = "xrLabel18";
        this.xrLabel18.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopCenter;
        // 
        // xrLabel16
        // 
        this.xrLabel16.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "webreports_BillingCaseInfo.webreports_BillingCaseInfowebreports_BillingProviderIn" +
                    "fo.webreports_BillingProviderInfowebreports_BillingHoursInfo.HoursNotes")});
        this.xrLabel16.Dpi = 100F;
        this.xrLabel16.Font = new System.Drawing.Font("Trebuchet MS", 9.75F);
        this.xrLabel16.LocationFloat = new DevExpress.Utils.PointFloat(480.1283F, 0F);
        this.xrLabel16.Name = "xrLabel16";
        this.xrLabel16.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
        this.xrLabel16.SizeF = new System.Drawing.SizeF(269.8719F, 23F);
        this.xrLabel16.StylePriority.UseFont = false;
        this.xrLabel16.Text = "xrLabel16";
        // 
        // xrLabel15
        // 
        this.xrLabel15.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "webreports_BillingCaseInfo.webreports_BillingCaseInfowebreports_BillingProviderIn" +
                    "fo.webreports_BillingProviderInfowebreports_BillingHoursInfo.ServiceName")});
        this.xrLabel15.Dpi = 100F;
        this.xrLabel15.Font = new System.Drawing.Font("Trebuchet MS", 9.75F);
        this.xrLabel15.LocationFloat = new DevExpress.Utils.PointFloat(328.045F, 0F);
        this.xrLabel15.Name = "xrLabel15";
        this.xrLabel15.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
        this.xrLabel15.SizeF = new System.Drawing.SizeF(152.0833F, 23F);
        this.xrLabel15.StylePriority.UseFont = false;
        this.xrLabel15.Text = "xrLabel15";
        // 
        // xrLabel14
        // 
        this.xrLabel14.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "webreports_BillingCaseInfo.webreports_BillingCaseInfowebreports_BillingProviderIn" +
                    "fo.webreports_BillingProviderInfowebreports_BillingHoursInfo.HoursTimeOut")});
        this.xrLabel14.Dpi = 100F;
        this.xrLabel14.Font = new System.Drawing.Font("Trebuchet MS", 9.75F);
        this.xrLabel14.LocationFloat = new DevExpress.Utils.PointFloat(172.9168F, 0F);
        this.xrLabel14.Name = "xrLabel14";
        this.xrLabel14.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
        this.xrLabel14.SizeF = new System.Drawing.SizeF(100F, 23F);
        this.xrLabel14.StylePriority.UseFont = false;
        this.xrLabel14.StylePriority.UseTextAlignment = false;
        this.xrLabel14.Text = "xrLabel14";
        this.xrLabel14.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopCenter;
        // 
        // xrLabel13
        // 
        this.xrLabel13.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "webreports_BillingCaseInfo.webreports_BillingCaseInfowebreports_BillingProviderIn" +
                    "fo.webreports_BillingProviderInfowebreports_BillingHoursInfo.HoursTimeIn")});
        this.xrLabel13.Dpi = 100F;
        this.xrLabel13.Font = new System.Drawing.Font("Trebuchet MS", 9.75F);
        this.xrLabel13.LocationFloat = new DevExpress.Utils.PointFloat(72.91681F, 0F);
        this.xrLabel13.Name = "xrLabel13";
        this.xrLabel13.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
        this.xrLabel13.SizeF = new System.Drawing.SizeF(100F, 23F);
        this.xrLabel13.StylePriority.UseFont = false;
        this.xrLabel13.StylePriority.UseTextAlignment = false;
        this.xrLabel13.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopCenter;
        // 
        // xrLabel12
        // 
        this.xrLabel12.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "webreports_BillingCaseInfo.webreports_BillingCaseInfowebreports_BillingProviderIn" +
                    "fo.webreports_BillingProviderInfowebreports_BillingHoursInfo.HoursDate", "{0:M/d/yyyy}")});
        this.xrLabel12.Dpi = 100F;
        this.xrLabel12.Font = new System.Drawing.Font("Trebuchet MS", 9.75F);
        this.xrLabel12.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0F);
        this.xrLabel12.Name = "xrLabel12";
        this.xrLabel12.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
        this.xrLabel12.SizeF = new System.Drawing.SizeF(72.91681F, 23F);
        this.xrLabel12.StylePriority.UseFont = false;
        this.xrLabel12.StylePriority.UseTextAlignment = false;
        this.xrLabel12.Text = "xrLabel12";
        this.xrLabel12.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopCenter;
        // 
        // PatientName
        // 
        this.PatientName.DataMember = "webreports_BillingCaseInfo";
        this.PatientName.Expression = "[PatientFirstName] + \' \' + [PatientLastName]";
        this.PatientName.Name = "PatientName";
        // 
        // ProviderName
        // 
        this.ProviderName.DataMember = "webreports_BillingCaseInfo.webreports_BillingCaseInfowebreports_BillingProviderIn" +
"fo";
        this.ProviderName.Expression = "[ProviderFirstName] + \' \' + [ProviderLastName]";
        this.ProviderName.Name = "ProviderName";
        // 
        // ProviderTypeLabel
        // 
        this.ProviderTypeLabel.DataMember = "webreports_BillingCaseInfo.webreports_BillingCaseInfowebreports_BillingProviderIn" +
"fo";
        this.ProviderTypeLabel.Expression = "[ProviderTypeCode] + \':\'";
        this.ProviderTypeLabel.Name = "ProviderTypeLabel";
        // 
        // BillingRef
        // 
        this.BillingRef.Description = "Parameter1";
        this.BillingRef.Name = "BillingRef";
        this.BillingRef.ValueInfo = "PREVIEW";
        this.BillingRef.Visible = false;
        // 
        // BillingReport
        // 
        this.Bands.AddRange(new DevExpress.XtraReports.UI.Band[] {
            this.Detail,
            this.TopMargin,
            this.BottomMargin,
            this.ReportHeader,
            this.PageFooter,
            this.DetailReport});
        this.CalculatedFields.AddRange(new DevExpress.XtraReports.UI.CalculatedField[] {
            this.PatientName,
            this.ProviderName,
            this.ProviderTypeLabel});
        this.ComponentStorage.AddRange(new System.ComponentModel.IComponent[] {
            this.BillingReportDS});
        this.DataMember = "webreports_BillingCaseInfo";
        this.DataSource = this.BillingReportDS;
        this.Margins = new System.Drawing.Printing.Margins(49, 51, 17, 21);
        this.Parameters.AddRange(new DevExpress.XtraReports.Parameters.Parameter[] {
            this.CaseID,
            this.firstDayOfMonthPeriod,
            this.BillingRef});
        this.Version = "15.2";
        ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

    }

    #endregion
}
