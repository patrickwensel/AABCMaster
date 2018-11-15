using DevExpress.XtraReports.UI;

/// <summary>
/// Summary description for PatientHoursReport
/// </summary>
public class ParentApprovalReport_alt : DevExpress.XtraReports.UI.XtraReport
{
    private DevExpress.XtraReports.UI.DetailBand Detail;
    private DevExpress.XtraReports.UI.TopMarginBand TopMargin;
    private DevExpress.XtraReports.UI.BottomMarginBand BottomMargin;
    private XRLabel xrLabel11;
    private XRLabel xrLabel12;
    private XRLabel xrLabel15;
    private XRLabel xrLabel16;
    private PageHeaderBand pageHeaderBand1;
    private XRLabel xrLabel3;
    private XRLabel xrLabel4;
    private XRLabel xrLabel5;
    private XRLabel xrLabel7;
    private XRLabel xrLabel8;
    private XRLine xrLine1;
    private XRLine xrLine2;
    private GroupHeaderBand groupHeaderBand1;
    private PageFooterBand pageFooterBand1;
    private XRPageInfo xrPageInfo2;
    private ReportHeaderBand reportHeaderBand1;
    private XRLabel xrLabel17;
    private XRControlStyle Title;
    private XRControlStyle FieldCaption;
    private XRControlStyle PageInfo;
    private XRControlStyle DataField;
    private XRLabel xrLabel2;
    private CalculatedField PatientName;
    private CalculatedField ProviderName;
    private XRLabel xrLabel6;
    private XRLabel xrLabel9;
    private XRLabel xrLabel1;
    private XRLabel xrLabel13;
    private XRLabel xrLabel10;
    private XRLabel xrLabel14;
    private XRLabel xrLabel18;
    private XRPictureBox xrPictureBox1;
    private XRLabel xrLabel19;
    private XRLabel xrLabel22;
    private XRLabel xrLabel21;
    private XRLabel xrLabel20;
    private DevExpress.DataAccess.Sql.SqlDataSource sqlDataSource1;
    private DevExpress.XtraReports.Parameters.Parameter CaseID;
    private DevExpress.XtraReports.Parameters.Parameter StartDate;
    private DevExpress.XtraReports.Parameters.Parameter EndDate;
    private XRLabel xrLabel24;
    private XRLabel xrLabel23;
    private CalculatedField BCBAName;
    private XRLabel xrLabel27;
    private XRLabel xrLabel26;
    private XRLine xrLine3;
    private XRLabel xrLabel25;
    private CalculatedField BCBASignature;
    private DevExpress.XtraReports.Parameters.Parameter ParentApprovalID;

    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    public ParentApprovalReport_alt()
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
        System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ParentApprovalReport_alt));
        DevExpress.DataAccess.Sql.StoredProcQuery storedProcQuery1 = new DevExpress.DataAccess.Sql.StoredProcQuery();
        DevExpress.DataAccess.Sql.QueryParameter queryParameter1 = new DevExpress.DataAccess.Sql.QueryParameter();
        DevExpress.DataAccess.Sql.QueryParameter queryParameter2 = new DevExpress.DataAccess.Sql.QueryParameter();
        DevExpress.DataAccess.Sql.QueryParameter queryParameter3 = new DevExpress.DataAccess.Sql.QueryParameter();
        DevExpress.DataAccess.Sql.StoredProcQuery storedProcQuery2 = new DevExpress.DataAccess.Sql.StoredProcQuery();
        DevExpress.DataAccess.Sql.QueryParameter queryParameter4 = new DevExpress.DataAccess.Sql.QueryParameter();
        this.Detail = new DevExpress.XtraReports.UI.DetailBand();
        this.xrLabel14 = new DevExpress.XtraReports.UI.XRLabel();
        this.xrLabel9 = new DevExpress.XtraReports.UI.XRLabel();
        this.xrLabel1 = new DevExpress.XtraReports.UI.XRLabel();
        this.xrLabel6 = new DevExpress.XtraReports.UI.XRLabel();
        this.xrLabel11 = new DevExpress.XtraReports.UI.XRLabel();
        this.xrLabel12 = new DevExpress.XtraReports.UI.XRLabel();
        this.xrLabel15 = new DevExpress.XtraReports.UI.XRLabel();
        this.xrLabel16 = new DevExpress.XtraReports.UI.XRLabel();
        this.TopMargin = new DevExpress.XtraReports.UI.TopMarginBand();
        this.BottomMargin = new DevExpress.XtraReports.UI.BottomMarginBand();
        this.pageHeaderBand1 = new DevExpress.XtraReports.UI.PageHeaderBand();
        this.xrLabel18 = new DevExpress.XtraReports.UI.XRLabel();
        this.xrLabel13 = new DevExpress.XtraReports.UI.XRLabel();
        this.xrLabel10 = new DevExpress.XtraReports.UI.XRLabel();
        this.xrLabel3 = new DevExpress.XtraReports.UI.XRLabel();
        this.xrLabel4 = new DevExpress.XtraReports.UI.XRLabel();
        this.xrLabel5 = new DevExpress.XtraReports.UI.XRLabel();
        this.xrLabel7 = new DevExpress.XtraReports.UI.XRLabel();
        this.xrLabel8 = new DevExpress.XtraReports.UI.XRLabel();
        this.xrLine1 = new DevExpress.XtraReports.UI.XRLine();
        this.xrLine2 = new DevExpress.XtraReports.UI.XRLine();
        this.groupHeaderBand1 = new DevExpress.XtraReports.UI.GroupHeaderBand();
        this.pageFooterBand1 = new DevExpress.XtraReports.UI.PageFooterBand();
        this.xrLabel27 = new DevExpress.XtraReports.UI.XRLabel();
        this.xrLabel26 = new DevExpress.XtraReports.UI.XRLabel();
        this.xrLine3 = new DevExpress.XtraReports.UI.XRLine();
        this.xrLabel25 = new DevExpress.XtraReports.UI.XRLabel();
        this.xrPageInfo2 = new DevExpress.XtraReports.UI.XRPageInfo();
        this.reportHeaderBand1 = new DevExpress.XtraReports.UI.ReportHeaderBand();
        this.xrLabel24 = new DevExpress.XtraReports.UI.XRLabel();
        this.xrLabel23 = new DevExpress.XtraReports.UI.XRLabel();
        this.xrLabel22 = new DevExpress.XtraReports.UI.XRLabel();
        this.xrLabel21 = new DevExpress.XtraReports.UI.XRLabel();
        this.xrLabel20 = new DevExpress.XtraReports.UI.XRLabel();
        this.xrPictureBox1 = new DevExpress.XtraReports.UI.XRPictureBox();
        this.xrLabel19 = new DevExpress.XtraReports.UI.XRLabel();
        this.xrLabel2 = new DevExpress.XtraReports.UI.XRLabel();
        this.xrLabel17 = new DevExpress.XtraReports.UI.XRLabel();
        this.Title = new DevExpress.XtraReports.UI.XRControlStyle();
        this.FieldCaption = new DevExpress.XtraReports.UI.XRControlStyle();
        this.PageInfo = new DevExpress.XtraReports.UI.XRControlStyle();
        this.DataField = new DevExpress.XtraReports.UI.XRControlStyle();
        this.PatientName = new DevExpress.XtraReports.UI.CalculatedField();
        this.ProviderName = new DevExpress.XtraReports.UI.CalculatedField();
        this.sqlDataSource1 = new DevExpress.DataAccess.Sql.SqlDataSource(this.components);
        this.CaseID = new DevExpress.XtraReports.Parameters.Parameter();
        this.StartDate = new DevExpress.XtraReports.Parameters.Parameter();
        this.EndDate = new DevExpress.XtraReports.Parameters.Parameter();
        this.BCBAName = new DevExpress.XtraReports.UI.CalculatedField();
        this.BCBASignature = new DevExpress.XtraReports.UI.CalculatedField();
        this.ParentApprovalID = new DevExpress.XtraReports.Parameters.Parameter();
        ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
        // 
        // Detail
        // 
        this.Detail.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrLabel14,
            this.xrLabel9,
            this.xrLabel1,
            this.xrLabel6,
            this.xrLabel11,
            this.xrLabel12,
            this.xrLabel15,
            this.xrLabel16});
        this.Detail.Dpi = 100F;
        this.Detail.HeightF = 23F;
        this.Detail.Name = "Detail";
        this.Detail.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100F);
        this.Detail.StyleName = "DataField";
        this.Detail.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
        // 
        // xrLabel14
        // 
        this.xrLabel14.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "webreports_ParentApprovalHoursReport.HoursTotal")});
        this.xrLabel14.Dpi = 100F;
        this.xrLabel14.LocationFloat = new DevExpress.Utils.PointFloat(183.5359F, 0F);
        this.xrLabel14.Name = "xrLabel14";
        this.xrLabel14.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
        this.xrLabel14.SizeF = new System.Drawing.SizeF(33.48204F, 15F);
        this.xrLabel14.Text = "HoursTotalDetail";
        // 
        // xrLabel9
        // 
        this.xrLabel9.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "webreports_ParentApprovalHoursReport.HoursTimeOut", "{0:h:mm tt}")});
        this.xrLabel9.Dpi = 100F;
        this.xrLabel9.LocationFloat = new DevExpress.Utils.PointFloat(118.9525F, 0F);
        this.xrLabel9.Name = "xrLabel9";
        this.xrLabel9.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
        this.xrLabel9.SizeF = new System.Drawing.SizeF(64.58336F, 15F);
        this.xrLabel9.StylePriority.UseTextAlignment = false;
        this.xrLabel9.Text = "HoursTimeOutDetail";
        this.xrLabel9.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopCenter;
        // 
        // xrLabel1
        // 
        this.xrLabel1.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "webreports_ParentApprovalHoursReport.HoursTimeIn", "{0:h:mm tt}")});
        this.xrLabel1.Dpi = 100F;
        this.xrLabel1.LocationFloat = new DevExpress.Utils.PointFloat(61.31353F, 0F);
        this.xrLabel1.Name = "xrLabel1";
        this.xrLabel1.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
        this.xrLabel1.SizeF = new System.Drawing.SizeF(57.63891F, 15F);
        this.xrLabel1.StylePriority.UseTextAlignment = false;
        this.xrLabel1.Text = "HoursTimeInDetail";
        this.xrLabel1.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopCenter;
        // 
        // xrLabel6
        // 
        this.xrLabel6.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "webreports_ParentApprovalHoursReport.ProviderFirstName")});
        this.xrLabel6.Dpi = 100F;
        this.xrLabel6.LocationFloat = new DevExpress.Utils.PointFloat(217.0179F, 0F);
        this.xrLabel6.Name = "xrLabel6";
        this.xrLabel6.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
        this.xrLabel6.SizeF = new System.Drawing.SizeF(143.5061F, 15F);
        this.xrLabel6.Text = "ProviderDetail";
        // 
        // xrLabel11
        // 
        this.xrLabel11.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "webreports_ParentApprovalHoursReport.HoursDate", "{0:M/d/yyyy}")});
        this.xrLabel11.Dpi = 100F;
        this.xrLabel11.LocationFloat = new DevExpress.Utils.PointFloat(5.99998F, 0F);
        this.xrLabel11.Name = "xrLabel11";
        this.xrLabel11.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
        this.xrLabel11.SizeF = new System.Drawing.SizeF(55.31354F, 15F);
        this.xrLabel11.StylePriority.UseTextAlignment = false;
        this.xrLabel11.Text = "hoursDateDetail";
        this.xrLabel11.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopCenter;
        // 
        // xrLabel12
        // 
        this.xrLabel12.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "webreports_ParentApprovalHoursReport.HoursNotes")});
        this.xrLabel12.Dpi = 100F;
        this.xrLabel12.LocationFloat = new DevExpress.Utils.PointFloat(458.8784F, 0F);
        this.xrLabel12.Name = "xrLabel12";
        this.xrLabel12.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
        this.xrLabel12.SizeF = new System.Drawing.SizeF(181.1216F, 15F);
        this.xrLabel12.Text = "HoursNotesDetail";
        // 
        // xrLabel15
        // 
        this.xrLabel15.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "webreports_ParentApprovalHoursReport.ProviderType")});
        this.xrLabel15.Dpi = 100F;
        this.xrLabel15.LocationFloat = new DevExpress.Utils.PointFloat(360.524F, 0F);
        this.xrLabel15.Name = "xrLabel15";
        this.xrLabel15.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
        this.xrLabel15.SizeF = new System.Drawing.SizeF(36.22137F, 15F);
        this.xrLabel15.StylePriority.UseTextAlignment = false;
        this.xrLabel15.Text = "ProviderTypeDetail";
        this.xrLabel15.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopCenter;
        // 
        // xrLabel16
        // 
        this.xrLabel16.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "webreports_ParentApprovalHoursReport.ServiceName")});
        this.xrLabel16.Dpi = 100F;
        this.xrLabel16.LocationFloat = new DevExpress.Utils.PointFloat(396.7454F, 0F);
        this.xrLabel16.Name = "xrLabel16";
        this.xrLabel16.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
        this.xrLabel16.SizeF = new System.Drawing.SizeF(62.13306F, 15F);
        this.xrLabel16.StylePriority.UseTextAlignment = false;
        this.xrLabel16.Text = "ServiceNameDetail";
        this.xrLabel16.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopCenter;
        // 
        // TopMargin
        // 
        this.TopMargin.Dpi = 100F;
        this.TopMargin.HeightF = 100F;
        this.TopMargin.Name = "TopMargin";
        this.TopMargin.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100F);
        this.TopMargin.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
        // 
        // BottomMargin
        // 
        this.BottomMargin.Dpi = 100F;
        this.BottomMargin.HeightF = 100F;
        this.BottomMargin.Name = "BottomMargin";
        this.BottomMargin.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100F);
        this.BottomMargin.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
        // 
        // pageHeaderBand1
        // 
        this.pageHeaderBand1.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrLabel18,
            this.xrLabel13,
            this.xrLabel10,
            this.xrLabel3,
            this.xrLabel4,
            this.xrLabel5,
            this.xrLabel7,
            this.xrLabel8,
            this.xrLine1,
            this.xrLine2});
        this.pageHeaderBand1.Dpi = 100F;
        this.pageHeaderBand1.HeightF = 36F;
        this.pageHeaderBand1.Name = "pageHeaderBand1";
        // 
        // xrLabel18
        // 
        this.xrLabel18.Dpi = 100F;
        this.xrLabel18.LocationFloat = new DevExpress.Utils.PointFloat(183.5359F, 10.00005F);
        this.xrLabel18.Name = "xrLabel18";
        this.xrLabel18.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
        this.xrLabel18.SizeF = new System.Drawing.SizeF(33.48204F, 18.37179F);
        this.xrLabel18.StyleName = "FieldCaption";
        this.xrLabel18.Text = "Hrs";
        // 
        // xrLabel13
        // 
        this.xrLabel13.Dpi = 100F;
        this.xrLabel13.LocationFloat = new DevExpress.Utils.PointFloat(118.9525F, 9.999994F);
        this.xrLabel13.Name = "xrLabel13";
        this.xrLabel13.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
        this.xrLabel13.SizeF = new System.Drawing.SizeF(64.58337F, 18.37179F);
        this.xrLabel13.StyleName = "FieldCaption";
        this.xrLabel13.Text = "Time Out";
        // 
        // xrLabel10
        // 
        this.xrLabel10.Dpi = 100F;
        this.xrLabel10.LocationFloat = new DevExpress.Utils.PointFloat(61.31354F, 10.00002F);
        this.xrLabel10.Name = "xrLabel10";
        this.xrLabel10.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
        this.xrLabel10.SizeF = new System.Drawing.SizeF(57.63892F, 18.37179F);
        this.xrLabel10.StyleName = "FieldCaption";
        this.xrLabel10.Text = "Time In";
        // 
        // xrLabel3
        // 
        this.xrLabel3.Dpi = 100F;
        this.xrLabel3.LocationFloat = new DevExpress.Utils.PointFloat(5.999989F, 10.00002F);
        this.xrLabel3.Name = "xrLabel3";
        this.xrLabel3.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
        this.xrLabel3.SizeF = new System.Drawing.SizeF(55.31354F, 18.37179F);
        this.xrLabel3.StyleName = "FieldCaption";
        this.xrLabel3.Text = "Date";
        // 
        // xrLabel4
        // 
        this.xrLabel4.Dpi = 100F;
        this.xrLabel4.LocationFloat = new DevExpress.Utils.PointFloat(458.8784F, 10.00002F);
        this.xrLabel4.Name = "xrLabel4";
        this.xrLabel4.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
        this.xrLabel4.SizeF = new System.Drawing.SizeF(185.1216F, 18.37179F);
        this.xrLabel4.StyleName = "FieldCaption";
        this.xrLabel4.Text = "Notes";
        // 
        // xrLabel5
        // 
        this.xrLabel5.Dpi = 100F;
        this.xrLabel5.LocationFloat = new DevExpress.Utils.PointFloat(217.018F, 10.00005F);
        this.xrLabel5.Name = "xrLabel5";
        this.xrLabel5.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
        this.xrLabel5.SizeF = new System.Drawing.SizeF(143.506F, 18.37179F);
        this.xrLabel5.StyleName = "FieldCaption";
        this.xrLabel5.Text = "Provider";
        // 
        // xrLabel7
        // 
        this.xrLabel7.Dpi = 100F;
        this.xrLabel7.LocationFloat = new DevExpress.Utils.PointFloat(360.524F, 10.00005F);
        this.xrLabel7.Name = "xrLabel7";
        this.xrLabel7.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
        this.xrLabel7.SizeF = new System.Drawing.SizeF(36.22137F, 18.3718F);
        this.xrLabel7.StyleName = "FieldCaption";
        this.xrLabel7.Text = "Type";
        // 
        // xrLabel8
        // 
        this.xrLabel8.Dpi = 100F;
        this.xrLabel8.LocationFloat = new DevExpress.Utils.PointFloat(396.7454F, 10.00005F);
        this.xrLabel8.Name = "xrLabel8";
        this.xrLabel8.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
        this.xrLabel8.SizeF = new System.Drawing.SizeF(62.13306F, 18.37178F);
        this.xrLabel8.StyleName = "FieldCaption";
        this.xrLabel8.StylePriority.UseTextAlignment = false;
        this.xrLabel8.Text = "Service";
        this.xrLabel8.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopCenter;
        // 
        // xrLine1
        // 
        this.xrLine1.Dpi = 100F;
        this.xrLine1.LocationFloat = new DevExpress.Utils.PointFloat(6F, 5F);
        this.xrLine1.Name = "xrLine1";
        this.xrLine1.SizeF = new System.Drawing.SizeF(638F, 2F);
        // 
        // xrLine2
        // 
        this.xrLine2.Dpi = 100F;
        this.xrLine2.LocationFloat = new DevExpress.Utils.PointFloat(5.99998F, 34F);
        this.xrLine2.Name = "xrLine2";
        this.xrLine2.SizeF = new System.Drawing.SizeF(638F, 2F);
        // 
        // groupHeaderBand1
        // 
        this.groupHeaderBand1.Dpi = 100F;
        this.groupHeaderBand1.GroupFields.AddRange(new DevExpress.XtraReports.UI.GroupField[] {
            new DevExpress.XtraReports.UI.GroupField("PatientFirstName", DevExpress.XtraReports.UI.XRColumnSortOrder.Ascending),
            new DevExpress.XtraReports.UI.GroupField("PatientLastName", DevExpress.XtraReports.UI.XRColumnSortOrder.Ascending)});
        this.groupHeaderBand1.HeightF = 0F;
        this.groupHeaderBand1.Name = "groupHeaderBand1";
        this.groupHeaderBand1.StyleName = "DataField";
        // 
        // pageFooterBand1
        // 
        this.pageFooterBand1.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrLabel27,
            this.xrLabel26,
            this.xrLine3,
            this.xrLabel25,
            this.xrPageInfo2});
        this.pageFooterBand1.Dpi = 100F;
        this.pageFooterBand1.HeightF = 61.29163F;
        this.pageFooterBand1.Name = "pageFooterBand1";
        // 
        // xrLabel27
        // 
        this.xrLabel27.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "webreports_PatientHoursReportDetail.BCBASignature")});
        this.xrLabel27.Dpi = 100F;
        this.xrLabel27.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F);
        this.xrLabel27.LocationFloat = new DevExpress.Utils.PointFloat(430.1251F, 5.371793F);
        this.xrLabel27.Name = "xrLabel27";
        this.xrLabel27.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
        this.xrLabel27.SizeF = new System.Drawing.SizeF(209.8749F, 23F);
        this.xrLabel27.StylePriority.UseFont = false;
        this.xrLabel27.StylePriority.UseTextAlignment = false;
        this.xrLabel27.Text = "xrLabel27";
        this.xrLabel27.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopCenter;
        // 
        // xrLabel26
        // 
        this.xrLabel26.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "webreports_PatientHoursReportDetail.SignoffDate", "{0:M/d/yyyy}")});
        this.xrLabel26.Dpi = 100F;
        this.xrLabel26.LocationFloat = new DevExpress.Utils.PointFloat(540F, 34.54164F);
        this.xrLabel26.Name = "xrLabel26";
        this.xrLabel26.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
        this.xrLabel26.SizeF = new System.Drawing.SizeF(100F, 23F);
        this.xrLabel26.Text = "xrLabel26";
        // 
        // xrLine3
        // 
        this.xrLine3.Dpi = 100F;
        this.xrLine3.LocationFloat = new DevExpress.Utils.PointFloat(430.1251F, 28.37175F);
        this.xrLine3.Name = "xrLine3";
        this.xrLine3.SizeF = new System.Drawing.SizeF(209.875F, 3.041668F);
        // 
        // xrLabel25
        // 
        this.xrLabel25.Dpi = 100F;
        this.xrLabel25.Font = new System.Drawing.Font("Times New Roman", 10F);
        this.xrLabel25.LocationFloat = new DevExpress.Utils.PointFloat(183.5359F, 9.999974F);
        this.xrLabel25.Name = "xrLabel25";
        this.xrLabel25.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
        this.xrLabel25.SizeF = new System.Drawing.SizeF(240.4683F, 18.37179F);
        this.xrLabel25.StyleName = "FieldCaption";
        this.xrLabel25.StylePriority.UseFont = false;
        this.xrLabel25.Text = "- electronically approved and finalized by";
        // 
        // xrPageInfo2
        // 
        this.xrPageInfo2.Dpi = 100F;
        this.xrPageInfo2.Format = "Page {0} of {1}";
        this.xrPageInfo2.LocationFloat = new DevExpress.Utils.PointFloat(0F, 9.999974F);
        this.xrPageInfo2.Name = "xrPageInfo2";
        this.xrPageInfo2.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
        this.xrPageInfo2.SizeF = new System.Drawing.SizeF(171.3333F, 23F);
        this.xrPageInfo2.StyleName = "PageInfo";
        this.xrPageInfo2.StylePriority.UseTextAlignment = false;
        this.xrPageInfo2.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
        // 
        // reportHeaderBand1
        // 
        this.reportHeaderBand1.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrLabel24,
            this.xrLabel23,
            this.xrLabel22,
            this.xrLabel21,
            this.xrLabel20,
            this.xrPictureBox1,
            this.xrLabel19,
            this.xrLabel2,
            this.xrLabel17});
        this.reportHeaderBand1.Dpi = 100F;
        this.reportHeaderBand1.HeightF = 216.5238F;
        this.reportHeaderBand1.Name = "reportHeaderBand1";
        // 
        // xrLabel24
        // 
        this.xrLabel24.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "webreports_PatientHoursReportDetail.BCBAName")});
        this.xrLabel24.Dpi = 100F;
        this.xrLabel24.Font = new System.Drawing.Font("Times New Roman", 12F);
        this.xrLabel24.LocationFloat = new DevExpress.Utils.PointFloat(463.3844F, 167.1905F);
        this.xrLabel24.Name = "xrLabel24";
        this.xrLabel24.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
        this.xrLabel24.SizeF = new System.Drawing.SizeF(180.6155F, 23F);
        this.xrLabel24.StylePriority.UseFont = false;
        this.xrLabel24.Text = "xrLabel24";
        // 
        // xrLabel23
        // 
        this.xrLabel23.Dpi = 100F;
        this.xrLabel23.Font = new System.Drawing.Font("Times New Roman", 12F);
        this.xrLabel23.LocationFloat = new DevExpress.Utils.PointFloat(392.3604F, 167.1905F);
        this.xrLabel23.Name = "xrLabel23";
        this.xrLabel23.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
        this.xrLabel23.SizeF = new System.Drawing.SizeF(66.51801F, 23.9584F);
        this.xrLabel23.StyleName = "Title";
        this.xrLabel23.StylePriority.UseFont = false;
        this.xrLabel23.Text = "BCBA:";
        // 
        // xrLabel22
        // 
        this.xrLabel22.Dpi = 100F;
        this.xrLabel22.Font = new System.Drawing.Font("Times New Roman", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        this.xrLabel22.LocationFloat = new DevExpress.Utils.PointFloat(458.8784F, 143.5655F);
        this.xrLabel22.Name = "xrLabel22";
        this.xrLabel22.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
        this.xrLabel22.SizeF = new System.Drawing.SizeF(51.86224F, 20.66664F);
        this.xrLabel22.StyleName = "Title";
        this.xrLabel22.StylePriority.UseFont = false;
        this.xrLabel22.StylePriority.UseTextAlignment = false;
        this.xrLabel22.Text = "through";
        this.xrLabel22.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopCenter;
        // 
        // xrLabel21
        // 
        this.xrLabel21.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "webreports_PatientHoursReportDetail.StartDate", "{0:M/d/yyyy}")});
        this.xrLabel21.Dpi = 100F;
        this.xrLabel21.LocationFloat = new DevExpress.Utils.PointFloat(357.4861F, 143.5655F);
        this.xrLabel21.Name = "xrLabel21";
        this.xrLabel21.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
        this.xrLabel21.SizeF = new System.Drawing.SizeF(100F, 23F);
        this.xrLabel21.StylePriority.UseTextAlignment = false;
        this.xrLabel21.Text = "xrLabel21";
        this.xrLabel21.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopRight;
        // 
        // xrLabel20
        // 
        this.xrLabel20.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "webreports_PatientHoursReportDetail.EndDate", "{0:M/d/yyyy}")});
        this.xrLabel20.Dpi = 100F;
        this.xrLabel20.LocationFloat = new DevExpress.Utils.PointFloat(510.7407F, 143.5655F);
        this.xrLabel20.Name = "xrLabel20";
        this.xrLabel20.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
        this.xrLabel20.SizeF = new System.Drawing.SizeF(100F, 23F);
        this.xrLabel20.Text = "xrLabel20";
        // 
        // xrPictureBox1
        // 
        this.xrPictureBox1.Dpi = 100F;
        this.xrPictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("xrPictureBox1.Image")));
        this.xrPictureBox1.LocationFloat = new DevExpress.Utils.PointFloat(6.000019F, 1.999991F);
        this.xrPictureBox1.Name = "xrPictureBox1";
        this.xrPictureBox1.SizeF = new System.Drawing.SizeF(154.1667F, 136.25F);
        this.xrPictureBox1.Sizing = DevExpress.XtraPrinting.ImageSizeMode.StretchImage;
        // 
        // xrLabel19
        // 
        this.xrLabel19.Dpi = 100F;
        this.xrLabel19.LocationFloat = new DevExpress.Utils.PointFloat(174.75F, 18.66668F);
        this.xrLabel19.Multiline = true;
        this.xrLabel19.Name = "xrLabel19";
        this.xrLabel19.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
        this.xrLabel19.SizeF = new System.Drawing.SizeF(308.9399F, 105.2917F);
        this.xrLabel19.StylePriority.UseFont = false;
        this.xrLabel19.Text = "Applied Behavioral Mental Health Counseling P.C.\r\n1970 52nd St, Bmt, Brooklyn, NY" +
" 11204\r\nPhone: 718-360-9548\r\nFax: 718-874-0052\r\ntimesheets@appliedabc.com\r\nwww.a" +
"ppliedabc.com";
        // 
        // xrLabel2
        // 
        this.xrLabel2.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "webreports_PatientHoursReportDetail.PatientName")});
        this.xrLabel2.Dpi = 100F;
        this.xrLabel2.Font = new System.Drawing.Font("Times New Roman", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        this.xrLabel2.LocationFloat = new DevExpress.Utils.PointFloat(6.45593F, 182.5655F);
        this.xrLabel2.Name = "xrLabel2";
        this.xrLabel2.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
        this.xrLabel2.SizeF = new System.Drawing.SizeF(276.4063F, 31F);
        this.xrLabel2.StylePriority.UseFont = false;
        this.xrLabel2.StylePriority.UseTextAlignment = false;
        this.xrLabel2.Text = "xrLabel2";
        this.xrLabel2.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopCenter;
        // 
        // xrLabel17
        // 
        this.xrLabel17.Dpi = 100F;
        this.xrLabel17.LocationFloat = new DevExpress.Utils.PointFloat(6.00001F, 143.5655F);
        this.xrLabel17.Name = "xrLabel17";
        this.xrLabel17.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
        this.xrLabel17.SizeF = new System.Drawing.SizeF(276.8622F, 38.99998F);
        this.xrLabel17.StyleName = "Title";
        this.xrLabel17.Text = "Patient Hours Detail";
        // 
        // Title
        // 
        this.Title.BackColor = System.Drawing.Color.Transparent;
        this.Title.BorderColor = System.Drawing.Color.Black;
        this.Title.Borders = DevExpress.XtraPrinting.BorderSide.None;
        this.Title.BorderWidth = 1F;
        this.Title.Font = new System.Drawing.Font("Times New Roman", 24F);
        this.Title.ForeColor = System.Drawing.Color.Black;
        this.Title.Name = "Title";
        // 
        // FieldCaption
        // 
        this.FieldCaption.BackColor = System.Drawing.Color.Transparent;
        this.FieldCaption.BorderColor = System.Drawing.Color.Black;
        this.FieldCaption.Borders = DevExpress.XtraPrinting.BorderSide.None;
        this.FieldCaption.BorderWidth = 1F;
        this.FieldCaption.Font = new System.Drawing.Font("Times New Roman", 10F, System.Drawing.FontStyle.Bold);
        this.FieldCaption.ForeColor = System.Drawing.Color.Black;
        this.FieldCaption.Name = "FieldCaption";
        // 
        // PageInfo
        // 
        this.PageInfo.BackColor = System.Drawing.Color.Transparent;
        this.PageInfo.BorderColor = System.Drawing.Color.Black;
        this.PageInfo.Borders = DevExpress.XtraPrinting.BorderSide.None;
        this.PageInfo.BorderWidth = 1F;
        this.PageInfo.Font = new System.Drawing.Font("Times New Roman", 8F);
        this.PageInfo.ForeColor = System.Drawing.Color.Black;
        this.PageInfo.Name = "PageInfo";
        // 
        // DataField
        // 
        this.DataField.BackColor = System.Drawing.Color.Transparent;
        this.DataField.BorderColor = System.Drawing.Color.Black;
        this.DataField.Borders = DevExpress.XtraPrinting.BorderSide.None;
        this.DataField.BorderWidth = 1F;
        this.DataField.Font = new System.Drawing.Font("Times New Roman", 8F);
        this.DataField.ForeColor = System.Drawing.Color.Black;
        this.DataField.Name = "DataField";
        this.DataField.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
        // 
        // PatientName
        // 
        this.PatientName.DataMember = "webreports_PatientHoursReportDetail";
        this.PatientName.Expression = "[PatientLastName] + \', \' + [PatientFirstName]";
        this.PatientName.Name = "PatientName";
        // 
        // ProviderName
        // 
        this.ProviderName.DataMember = "webreports_PatientHoursReportDetail";
        this.ProviderName.Expression = "[ProviderLastName] + \', \' + [ProviderFirstName]";
        this.ProviderName.Name = "ProviderName";
        // 
        // sqlDataSource1
        // 
        this.sqlDataSource1.ConnectionName = "CoreConnection";
        this.sqlDataSource1.Name = "sqlDataSource1";
        storedProcQuery1.Name = "webreports_PatientHoursReportDetail";
        queryParameter1.Name = "@CaseID";
        queryParameter1.Type = typeof(DevExpress.DataAccess.Expression);
        queryParameter1.Value = new DevExpress.DataAccess.Expression("[Parameters.CaseID]", typeof(int));
        queryParameter2.Name = "@StartDate";
        queryParameter2.Type = typeof(DevExpress.DataAccess.Expression);
        queryParameter2.Value = new DevExpress.DataAccess.Expression("[Parameters.StartDate]", typeof(System.DateTime));
        queryParameter3.Name = "@EndDate";
        queryParameter3.Type = typeof(DevExpress.DataAccess.Expression);
        queryParameter3.Value = new DevExpress.DataAccess.Expression("[Parameters.EndDate]", typeof(System.DateTime));
        storedProcQuery1.Parameters.Add(queryParameter1);
        storedProcQuery1.Parameters.Add(queryParameter2);
        storedProcQuery1.Parameters.Add(queryParameter3);
        storedProcQuery1.StoredProcName = "webreports.PatientHoursReportDetail";
        storedProcQuery2.Name = "webreports_ParentApprovalHoursReport";
        queryParameter4.Name = "@ParentApprovalID";
        queryParameter4.Type = typeof(int);
        queryParameter4.ValueInfo = "0";
        storedProcQuery2.Parameters.Add(queryParameter4);
        storedProcQuery2.StoredProcName = "webreports.ParentApprovalHoursReport";
        this.sqlDataSource1.Queries.AddRange(new DevExpress.DataAccess.Sql.SqlQuery[] {
            storedProcQuery2,
            storedProcQuery1});
        this.sqlDataSource1.ResultSchemaSerializable = resources.GetString("sqlDataSource1.ResultSchemaSerializable");
        // 
        // CaseID
        // 
        this.CaseID.Description = "Case ID";
        this.CaseID.Name = "CaseID";
        this.CaseID.Type = typeof(int);
        this.CaseID.ValueInfo = "0";
        // 
        // StartDate
        // 
        this.StartDate.Description = "Start Date";
        this.StartDate.Name = "StartDate";
        this.StartDate.Type = typeof(System.DateTime);
        // 
        // EndDate
        // 
        this.EndDate.Description = "End Date";
        this.EndDate.Name = "EndDate";
        this.EndDate.Type = typeof(System.DateTime);
        // 
        // BCBAName
        // 
        this.BCBAName.DataMember = "webreports_PatientHoursReportDetail";
        this.BCBAName.Expression = "[reportedBCBALastName] + \', \' + [reportedBCBAFirstName]";
        this.BCBAName.Name = "BCBAName";
        // 
        // BCBASignature
        // 
        this.BCBASignature.DataMember = "webreports_PatientHoursReportDetail";
        this.BCBASignature.Expression = "[reportedBCBAFirstName] + \' \' + [reportedBCBALastName]";
        this.BCBASignature.Name = "BCBASignature";
        // 
        // ParentApprovalID
        // 
        this.ParentApprovalID.Description = "Parent Approval ID";
        this.ParentApprovalID.Name = "ParentApprovalID";
        this.ParentApprovalID.Type = typeof(int);
        this.ParentApprovalID.ValueInfo = "21";
        // 
        // ParentApprovalReport4
        // 
        this.Bands.AddRange(new DevExpress.XtraReports.UI.Band[] {
            this.Detail,
            this.TopMargin,
            this.BottomMargin,
            this.pageHeaderBand1,
            this.groupHeaderBand1,
            this.pageFooterBand1,
            this.reportHeaderBand1});
        this.CalculatedFields.AddRange(new DevExpress.XtraReports.UI.CalculatedField[] {
            this.PatientName,
            this.ProviderName,
            this.BCBAName,
            this.BCBASignature});
        this.ComponentStorage.AddRange(new System.ComponentModel.IComponent[] {
            this.sqlDataSource1});
        this.DataMember = "webreports_ParentApprovalHoursReport";
        this.DataSource = this.sqlDataSource1;
        this.Parameters.AddRange(new DevExpress.XtraReports.Parameters.Parameter[] {
            this.CaseID,
            this.StartDate,
            this.EndDate,
            this.ParentApprovalID});
        this.StyleSheet.AddRange(new DevExpress.XtraReports.UI.XRControlStyle[] {
            this.Title,
            this.FieldCaption,
            this.PageInfo,
            this.DataField});
        this.Version = "16.1";
        ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

    }

    #endregion
}
