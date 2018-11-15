using DevExpress.XtraReports.UI;

/// <summary>
/// Summary description for PatientHoursReport
/// </summary>
public class PatientHoursReport : DevExpress.XtraReports.UI.XtraReport
{
    private DevExpress.XtraReports.UI.DetailBand Detail;
    private DevExpress.XtraReports.UI.TopMarginBand TopMargin;
    private DevExpress.XtraReports.UI.BottomMarginBand BottomMargin;
    private XRLabel xrLabel3;
    private XRLabel xrLabel4;
    private XRLabel xrLabel8;
    private XRLine xrLine1;
    private XRLine xrLine2;
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
    private XRLabel xrLabel13;
    private XRLabel xrLabel10;
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
    private CalculatedField BCBAName;
    private CalculatedField BCBASignature;
    private CalculatedField ProviderSignature;
    private DetailReportBand DetailReport1;
    private DetailBand Detail2;
    private XRLabel xrLabel28;
    private XRLabel xrLabel14;
    private XRLabel xrLabel12;
    private XRLabel xrLabel9;
    private XRLabel xrLabel6;
    private XRLabel xrLabel1;
    private CalculatedField calculatedField1;
    private GroupHeaderBand GroupHeader2;
    private XRLabel xrLabel16;
    private XRLabel xrLabel15;
    private GroupFooterBand GroupFooter1;
    private XRLine xrLine3;
    private XRLabel xrLabel27;
    private XRLabel xrLabel26;
    private XRLabel xrLabel31;
    private XRLabel xrLabel30;
    private CalculatedField FinalizationID;
    private DevExpress.XtraReports.Parameters.Parameter ProviderID;
    private ReportHeaderBand ReportHeader;
    private XRLabel xrLabel24;
    private XRLabel xrLabel23;
    private CalculatedField ProviderTypeLabel;
    private XRLine xrLine4;
    private XRLabel xrLabel5;

    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    public PatientHoursReport() {
        InitializeComponent();
        //
        // TODO: Add constructor logic here
        //
    }

    /// <summary> 
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing) {
        if (disposing && (components != null)) {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    #region Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PatientHoursReport));
            DevExpress.DataAccess.Sql.StoredProcQuery storedProcQuery1 = new DevExpress.DataAccess.Sql.StoredProcQuery();
            DevExpress.DataAccess.Sql.QueryParameter queryParameter1 = new DevExpress.DataAccess.Sql.QueryParameter();
            DevExpress.DataAccess.Sql.QueryParameter queryParameter2 = new DevExpress.DataAccess.Sql.QueryParameter();
            DevExpress.DataAccess.Sql.QueryParameter queryParameter3 = new DevExpress.DataAccess.Sql.QueryParameter();
            DevExpress.DataAccess.Sql.StoredProcQuery storedProcQuery2 = new DevExpress.DataAccess.Sql.StoredProcQuery();
            DevExpress.DataAccess.Sql.QueryParameter queryParameter4 = new DevExpress.DataAccess.Sql.QueryParameter();
            DevExpress.DataAccess.Sql.QueryParameter queryParameter5 = new DevExpress.DataAccess.Sql.QueryParameter();
            DevExpress.DataAccess.Sql.QueryParameter queryParameter6 = new DevExpress.DataAccess.Sql.QueryParameter();
            this.Detail = new DevExpress.XtraReports.UI.DetailBand();
            this.TopMargin = new DevExpress.XtraReports.UI.TopMarginBand();
            this.BottomMargin = new DevExpress.XtraReports.UI.BottomMarginBand();
            this.xrLabel18 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel13 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel10 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel3 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel4 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel8 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLine1 = new DevExpress.XtraReports.UI.XRLine();
            this.xrLine2 = new DevExpress.XtraReports.UI.XRLine();
            this.pageFooterBand1 = new DevExpress.XtraReports.UI.PageFooterBand();
            this.xrPageInfo2 = new DevExpress.XtraReports.UI.XRPageInfo();
            this.reportHeaderBand1 = new DevExpress.XtraReports.UI.ReportHeaderBand();
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
            this.ProviderSignature = new DevExpress.XtraReports.UI.CalculatedField();
            this.DetailReport1 = new DevExpress.XtraReports.UI.DetailReportBand();
            this.Detail2 = new DevExpress.XtraReports.UI.DetailBand();
            this.xrLabel16 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel14 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel12 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel9 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel6 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel1 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel28 = new DevExpress.XtraReports.UI.XRLabel();
            this.GroupHeader2 = new DevExpress.XtraReports.UI.GroupHeaderBand();
            this.xrLabel15 = new DevExpress.XtraReports.UI.XRLabel();
            this.GroupFooter1 = new DevExpress.XtraReports.UI.GroupFooterBand();
            this.xrLabel31 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel30 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLine3 = new DevExpress.XtraReports.UI.XRLine();
            this.xrLabel27 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel26 = new DevExpress.XtraReports.UI.XRLabel();
            this.ReportHeader = new DevExpress.XtraReports.UI.ReportHeaderBand();
            this.xrLabel23 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel24 = new DevExpress.XtraReports.UI.XRLabel();
            this.calculatedField1 = new DevExpress.XtraReports.UI.CalculatedField();
            this.FinalizationID = new DevExpress.XtraReports.UI.CalculatedField();
            this.ProviderID = new DevExpress.XtraReports.Parameters.Parameter();
            this.ProviderTypeLabel = new DevExpress.XtraReports.UI.CalculatedField();
            this.xrLine4 = new DevExpress.XtraReports.UI.XRLine();
            this.xrLabel5 = new DevExpress.XtraReports.UI.XRLabel();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // Detail
            // 
            this.Detail.Dpi = 100F;
            this.Detail.HeightF = 0F;
            this.Detail.Name = "Detail";
            this.Detail.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100F);
            this.Detail.StyleName = "DataField";
            this.Detail.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            this.Detail.Visible = false;
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
            // xrLabel18
            // 
            this.xrLabel18.Dpi = 100F;
            this.xrLabel18.LocationFloat = new DevExpress.Utils.PointFloat(288.3769F, 9.999974F);
            this.xrLabel18.Name = "xrLabel18";
            this.xrLabel18.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel18.SizeF = new System.Drawing.SizeF(33.48204F, 18.37179F);
            this.xrLabel18.StyleName = "FieldCaption";
            this.xrLabel18.Text = "Hrs";
            // 
            // xrLabel13
            // 
            this.xrLabel13.Dpi = 100F;
            this.xrLabel13.LocationFloat = new DevExpress.Utils.PointFloat(213.3768F, 9.999974F);
            this.xrLabel13.Name = "xrLabel13";
            this.xrLabel13.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel13.SizeF = new System.Drawing.SizeF(75.00002F, 18.37179F);
            this.xrLabel13.StyleName = "FieldCaption";
            this.xrLabel13.Text = "Time Out";
            // 
            // xrLabel10
            // 
            this.xrLabel10.Dpi = 100F;
            this.xrLabel10.LocationFloat = new DevExpress.Utils.PointFloat(142.1962F, 9.999974F);
            this.xrLabel10.Name = "xrLabel10";
            this.xrLabel10.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel10.SizeF = new System.Drawing.SizeF(71.18059F, 18.37179F);
            this.xrLabel10.StyleName = "FieldCaption";
            this.xrLabel10.Text = "Time In";
            // 
            // xrLabel3
            // 
            this.xrLabel3.Dpi = 100F;
            this.xrLabel3.LocationFloat = new DevExpress.Utils.PointFloat(69.50741F, 9.999974F);
            this.xrLabel3.Name = "xrLabel3";
            this.xrLabel3.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel3.SizeF = new System.Drawing.SizeF(72.68874F, 18.37179F);
            this.xrLabel3.StyleName = "FieldCaption";
            this.xrLabel3.Text = "Date";
            // 
            // xrLabel4
            // 
            this.xrLabel4.Dpi = 100F;
            this.xrLabel4.LocationFloat = new DevExpress.Utils.PointFloat(378.8173F, 9.999974F);
            this.xrLabel4.Name = "xrLabel4";
            this.xrLabel4.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel4.SizeF = new System.Drawing.SizeF(323.1829F, 18.37179F);
            this.xrLabel4.StyleName = "FieldCaption";
            this.xrLabel4.Text = "Notes";
            // 
            // xrLabel8
            // 
            this.xrLabel8.Dpi = 100F;
            this.xrLabel8.LocationFloat = new DevExpress.Utils.PointFloat(321.8589F, 9.999974F);
            this.xrLabel8.Name = "xrLabel8";
            this.xrLabel8.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel8.SizeF = new System.Drawing.SizeF(56.95837F, 18.37178F);
            this.xrLabel8.StyleName = "FieldCaption";
            this.xrLabel8.StylePriority.UseTextAlignment = false;
            this.xrLabel8.Text = "Service";
            this.xrLabel8.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopCenter;
            // 
            // xrLine1
            // 
            this.xrLine1.Dpi = 100F;
            this.xrLine1.LocationFloat = new DevExpress.Utils.PointFloat(6.00001F, 1.041635F);
            this.xrLine1.Name = "xrLine1";
            this.xrLine1.SizeF = new System.Drawing.SizeF(696F, 5.95832F);
            // 
            // xrLine2
            // 
            this.xrLine2.Dpi = 100F;
            this.xrLine2.LocationFloat = new DevExpress.Utils.PointFloat(5.999947F, 28.37181F);
            this.xrLine2.Name = "xrLine2";
            this.xrLine2.SizeF = new System.Drawing.SizeF(696F, 2F);
            // 
            // pageFooterBand1
            // 
            this.pageFooterBand1.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrPageInfo2});
            this.pageFooterBand1.Dpi = 100F;
            this.pageFooterBand1.HeightF = 45.66663F;
            this.pageFooterBand1.Name = "pageFooterBand1";
            // 
            // xrPageInfo2
            // 
            this.xrPageInfo2.Dpi = 100F;
            this.xrPageInfo2.Format = "Page {0} of {1}";
            this.xrPageInfo2.LocationFloat = new DevExpress.Utils.PointFloat(0F, 9.999974F);
            this.xrPageInfo2.Name = "xrPageInfo2";
            this.xrPageInfo2.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrPageInfo2.SizeF = new System.Drawing.SizeF(81.74996F, 23F);
            this.xrPageInfo2.StyleName = "PageInfo";
            this.xrPageInfo2.StylePriority.UseTextAlignment = false;
            this.xrPageInfo2.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // reportHeaderBand1
            // 
            this.reportHeaderBand1.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrLabel22,
            this.xrLabel21,
            this.xrLabel20,
            this.xrPictureBox1,
            this.xrLabel19,
            this.xrLabel2,
            this.xrLabel17});
            this.reportHeaderBand1.Dpi = 100F;
            this.reportHeaderBand1.HeightF = 207.1488F;
            this.reportHeaderBand1.Name = "reportHeaderBand1";
            // 
            // xrLabel22
            // 
            this.xrLabel22.Dpi = 100F;
            this.xrLabel22.Font = new System.Drawing.Font("Times New Roman", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrLabel22.LocationFloat = new DevExpress.Utils.PointFloat(512.6127F, 174.5655F);
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
            this.xrLabel21.LocationFloat = new DevExpress.Utils.PointFloat(411.2205F, 174.5655F);
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
            this.xrLabel20.LocationFloat = new DevExpress.Utils.PointFloat(564.4752F, 174.5655F);
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
            this.xrLabel19.Font = new System.Drawing.Font("Trebuchet MS", 9.75F);
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
            this.xrLabel2.LocationFloat = new DevExpress.Utils.PointFloat(401.0257F, 143.5655F);
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
            this.xrLabel17.LocationFloat = new DevExpress.Utils.PointFloat(5.999883F, 143.5655F);
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
            storedProcQuery2.Name = "webreports_PatientHoursReportDetailSignatures";
            queryParameter4.Name = "@CaseID";
            queryParameter4.Type = typeof(DevExpress.DataAccess.Expression);
            queryParameter4.Value = new DevExpress.DataAccess.Expression("[Parameters.CaseID]", typeof(int));
            queryParameter5.Name = "@StartDate";
            queryParameter5.Type = typeof(DevExpress.DataAccess.Expression);
            queryParameter5.Value = new DevExpress.DataAccess.Expression("[Parameters.StartDate]", typeof(System.DateTime));
            queryParameter6.Name = "@EndDate";
            queryParameter6.Type = typeof(DevExpress.DataAccess.Expression);
            queryParameter6.Value = new DevExpress.DataAccess.Expression("[Parameters.EndDate]", typeof(System.DateTime));
            storedProcQuery2.Parameters.Add(queryParameter4);
            storedProcQuery2.Parameters.Add(queryParameter5);
            storedProcQuery2.Parameters.Add(queryParameter6);
            storedProcQuery2.StoredProcName = "webreports.PatientHoursReportDetailSignatures";
            this.sqlDataSource1.Queries.AddRange(new DevExpress.DataAccess.Sql.SqlQuery[] {
            storedProcQuery1,
            storedProcQuery2});
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
            // ProviderSignature
            // 
            this.ProviderSignature.DataMember = "webreports_PatientHoursReportDetail";
            this.ProviderSignature.Expression = "[ProviderFirstName] + \' \' + [ProviderLastName]";
            this.ProviderSignature.Name = "ProviderSignature";
            // 
            // DetailReport1
            // 
            this.DetailReport1.Bands.AddRange(new DevExpress.XtraReports.UI.Band[] {
            this.Detail2,
            this.GroupHeader2,
            this.GroupFooter1,
            this.ReportHeader});
            this.DetailReport1.DataMember = "webreports_PatientHoursReportDetail";
            this.DetailReport1.DataSource = this.sqlDataSource1;
            this.DetailReport1.Dpi = 100F;
            this.DetailReport1.FilterString = "[ProviderID] = ?ProviderID";
            this.DetailReport1.Level = 0;
            this.DetailReport1.Name = "DetailReport1";
            // 
            // Detail2
            // 
            this.Detail2.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrLabel16,
            this.xrLabel14,
            this.xrLabel12,
            this.xrLabel9,
            this.xrLabel6,
            this.xrLabel1,
            this.xrLabel28});
            this.Detail2.Dpi = 100F;
            this.Detail2.HeightF = 23F;
            this.Detail2.Name = "Detail2";
            // 
            // xrLabel16
            // 
            this.xrLabel16.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "webreports_PatientHoursReportDetail.ID")});
            this.xrLabel16.Dpi = 100F;
            this.xrLabel16.Font = new System.Drawing.Font("Times New Roman", 8.5F);
            this.xrLabel16.LocationFloat = new DevExpress.Utils.PointFloat(9.999974F, 0F);
            this.xrLabel16.Name = "xrLabel16";
            this.xrLabel16.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel16.SizeF = new System.Drawing.SizeF(59.50743F, 23F);
            this.xrLabel16.StylePriority.UseFont = false;
            this.xrLabel16.Text = "xrLabel16";
            // 
            // xrLabel14
            // 
            this.xrLabel14.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "webreports_PatientHoursReportDetail.HoursNotes")});
            this.xrLabel14.Dpi = 100F;
            this.xrLabel14.Font = new System.Drawing.Font("Times New Roman", 8.5F);
            this.xrLabel14.LocationFloat = new DevExpress.Utils.PointFloat(378.8173F, 0F);
            this.xrLabel14.Name = "xrLabel14";
            this.xrLabel14.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel14.SizeF = new System.Drawing.SizeF(323.1826F, 23F);
            this.xrLabel14.StylePriority.UseFont = false;
            this.xrLabel14.Text = "xrLabel14";
            // 
            // xrLabel12
            // 
            this.xrLabel12.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "webreports_PatientHoursReportDetail.ServiceCode")});
            this.xrLabel12.Dpi = 100F;
            this.xrLabel12.Font = new System.Drawing.Font("Times New Roman", 8.5F);
            this.xrLabel12.LocationFloat = new DevExpress.Utils.PointFloat(321.8589F, 0F);
            this.xrLabel12.Name = "xrLabel12";
            this.xrLabel12.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel12.SizeF = new System.Drawing.SizeF(56.95837F, 23F);
            this.xrLabel12.StylePriority.UseFont = false;
            this.xrLabel12.Text = "xrLabel12";
            // 
            // xrLabel9
            // 
            this.xrLabel9.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "webreports_PatientHoursReportDetail.HoursTotal")});
            this.xrLabel9.Dpi = 100F;
            this.xrLabel9.Font = new System.Drawing.Font("Times New Roman", 8.5F);
            this.xrLabel9.LocationFloat = new DevExpress.Utils.PointFloat(288.3769F, 0F);
            this.xrLabel9.Name = "xrLabel9";
            this.xrLabel9.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel9.SizeF = new System.Drawing.SizeF(33.48203F, 23F);
            this.xrLabel9.StylePriority.UseFont = false;
            this.xrLabel9.Text = "xrLabel9";
            // 
            // xrLabel6
            // 
            this.xrLabel6.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "webreports_PatientHoursReportDetail.HoursTimeOut", "{0:h:mm tt}")});
            this.xrLabel6.Dpi = 100F;
            this.xrLabel6.Font = new System.Drawing.Font("Times New Roman", 8.5F);
            this.xrLabel6.LocationFloat = new DevExpress.Utils.PointFloat(213.3769F, 0F);
            this.xrLabel6.Name = "xrLabel6";
            this.xrLabel6.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel6.SizeF = new System.Drawing.SizeF(75.00002F, 23F);
            this.xrLabel6.StylePriority.UseFont = false;
            this.xrLabel6.Text = "xrLabel6";
            // 
            // xrLabel1
            // 
            this.xrLabel1.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "webreports_PatientHoursReportDetail.HoursTimeIn", "{0:h:mm tt}")});
            this.xrLabel1.Dpi = 100F;
            this.xrLabel1.Font = new System.Drawing.Font("Times New Roman", 8.5F);
            this.xrLabel1.LocationFloat = new DevExpress.Utils.PointFloat(142.1962F, 0F);
            this.xrLabel1.Name = "xrLabel1";
            this.xrLabel1.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel1.SizeF = new System.Drawing.SizeF(71.1806F, 23F);
            this.xrLabel1.StylePriority.UseFont = false;
            this.xrLabel1.Text = "xrLabel1";
            // 
            // xrLabel28
            // 
            this.xrLabel28.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "webreports_PatientHoursReportDetail.HoursDate", "{0:M/d/yyyy}")});
            this.xrLabel28.Dpi = 100F;
            this.xrLabel28.Font = new System.Drawing.Font("Times New Roman", 8.5F);
            this.xrLabel28.LocationFloat = new DevExpress.Utils.PointFloat(69.50747F, 0F);
            this.xrLabel28.Name = "xrLabel28";
            this.xrLabel28.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel28.SizeF = new System.Drawing.SizeF(72.68874F, 15F);
            this.xrLabel28.StylePriority.UseFont = false;
            this.xrLabel28.StylePriority.UseTextAlignment = false;
            this.xrLabel28.Text = "xrLabel11";
            this.xrLabel28.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // GroupHeader2
            // 
            this.GroupHeader2.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrLabel15,
            this.xrLabel8,
            this.xrLabel3,
            this.xrLabel10,
            this.xrLabel13,
            this.xrLabel18,
            this.xrLine1,
            this.xrLabel4,
            this.xrLine2});
            this.GroupHeader2.Dpi = 100F;
            this.GroupHeader2.HeightF = 31.25F;
            this.GroupHeader2.Name = "GroupHeader2";
            this.GroupHeader2.RepeatEveryPage = true;
            // 
            // xrLabel15
            // 
            this.xrLabel15.Dpi = 100F;
            this.xrLabel15.LocationFloat = new DevExpress.Utils.PointFloat(9.999974F, 9.999974F);
            this.xrLabel15.Name = "xrLabel15";
            this.xrLabel15.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel15.SizeF = new System.Drawing.SizeF(59.50743F, 18.37179F);
            this.xrLabel15.StyleName = "FieldCaption";
            this.xrLabel15.Text = "ID";
            // 
            // GroupFooter1
            // 
            this.GroupFooter1.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrLabel5,
            this.xrLine4,
            this.xrLabel31,
            this.xrLabel30,
            this.xrLine3,
            this.xrLabel27,
            this.xrLabel26});
            this.GroupFooter1.Dpi = 100F;
            this.GroupFooter1.HeightF = 86.12499F;
            this.GroupFooter1.Name = "GroupFooter1";
            // 
            // xrLabel31
            // 
            this.xrLabel31.Dpi = 100F;
            this.xrLabel31.Font = new System.Drawing.Font("Times New Roman", 9.75F);
            this.xrLabel31.LocationFloat = new DevExpress.Utils.PointFloat(510.7407F, 55.25004F);
            this.xrLabel31.Name = "xrLabel31";
            this.xrLabel31.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel31.SizeF = new System.Drawing.SizeF(89.58331F, 23F);
            this.xrLabel31.StyleName = "Title";
            this.xrLabel31.StylePriority.UseFont = false;
            this.xrLabel31.Text = "Signature ID:";
            // 
            // xrLabel30
            // 
            this.xrLabel30.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "webreports_PatientHoursReportDetail.FinalizationID")});
            this.xrLabel30.Dpi = 100F;
            this.xrLabel30.LocationFloat = new DevExpress.Utils.PointFloat(600.324F, 55.25004F);
            this.xrLabel30.Name = "xrLabel30";
            this.xrLabel30.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel30.SizeF = new System.Drawing.SizeF(101.6758F, 23F);
            this.xrLabel30.Text = "xrLabel30";
            // 
            // xrLine3
            // 
            this.xrLine3.Dpi = 100F;
            this.xrLine3.LocationFloat = new DevExpress.Utils.PointFloat(5.999883F, 9.999974F);
            this.xrLine3.Name = "xrLine3";
            this.xrLine3.SizeF = new System.Drawing.SizeF(696F, 5.95832F);
            // 
            // xrLabel27
            // 
            this.xrLabel27.Dpi = 100F;
            this.xrLabel27.Font = new System.Drawing.Font("Times New Roman", 12F);
            this.xrLabel27.LocationFloat = new DevExpress.Utils.PointFloat(0F, 35.37502F);
            this.xrLabel27.Name = "xrLabel27";
            this.xrLabel27.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel27.SizeF = new System.Drawing.SizeF(160.1667F, 23F);
            this.xrLabel27.StyleName = "Title";
            this.xrLabel27.StylePriority.UseFont = false;
            this.xrLabel27.Text = "Electronically signed:";
            // 
            // xrLabel26
            // 
            this.xrLabel26.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "webreports_PatientHoursReportDetail.LatestSignoffDate")});
            this.xrLabel26.Dpi = 100F;
            this.xrLabel26.LocationFloat = new DevExpress.Utils.PointFloat(510.7407F, 32.25002F);
            this.xrLabel26.Name = "xrLabel26";
            this.xrLabel26.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel26.SizeF = new System.Drawing.SizeF(191.2593F, 23F);
            this.xrLabel26.Text = "xrLabel26";
            // 
            // ReportHeader
            // 
            this.ReportHeader.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrLabel23,
            this.xrLabel24});
            this.ReportHeader.Dpi = 100F;
            this.ReportHeader.HeightF = 69.79166F;
            this.ReportHeader.Name = "ReportHeader";
            // 
            // xrLabel23
            // 
            this.xrLabel23.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "webreports_PatientHoursReportDetail.ProviderTypeLabel")});
            this.xrLabel23.Dpi = 100F;
            this.xrLabel23.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold);
            this.xrLabel23.LocationFloat = new DevExpress.Utils.PointFloat(0F, 10.00001F);
            this.xrLabel23.Name = "xrLabel23";
            this.xrLabel23.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel23.SizeF = new System.Drawing.SizeF(157.9349F, 23F);
            this.xrLabel23.StylePriority.UseFont = false;
            this.xrLabel23.Text = "xrLabel23";
            // 
            // xrLabel24
            // 
            this.xrLabel24.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "webreports_PatientHoursReportDetail.ProviderName")});
            this.xrLabel24.Dpi = 100F;
            this.xrLabel24.Font = new System.Drawing.Font("Times New Roman", 12F);
            this.xrLabel24.LocationFloat = new DevExpress.Utils.PointFloat(0F, 32.99999F);
            this.xrLabel24.Name = "xrLabel24";
            this.xrLabel24.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel24.SizeF = new System.Drawing.SizeF(231.25F, 23F);
            this.xrLabel24.StylePriority.UseFont = false;
            this.xrLabel24.Text = "xrLabel24";
            // 
            // calculatedField1
            // 
            this.calculatedField1.DataMember = "webreports_PatientHoursReportDetailSignatures";
            this.calculatedField1.DisplayName = "ProviderSignature";
            this.calculatedField1.Expression = "[ProviderFirstName] + \' \' + [ProviderLastName]";
            this.calculatedField1.Name = "calculatedField1";
            // 
            // FinalizationID
            // 
            this.FinalizationID.DataMember = "webreports_PatientHoursReportDetail";
            this.FinalizationID.Expression = "ToInt(Rnd() * 100000)";
            this.FinalizationID.Name = "FinalizationID";
            // 
            // ProviderID
            // 
            this.ProviderID.Description = "Provider ID";
            this.ProviderID.Name = "ProviderID";
            this.ProviderID.Type = typeof(int);
            this.ProviderID.ValueInfo = "0";
            // 
            // ProviderTypeLabel
            // 
            this.ProviderTypeLabel.DataMember = "webreports_PatientHoursReportDetail";
            this.ProviderTypeLabel.Expression = "Iif([ProviderTypeCode] = \'BCBA\', \'BCBA\', \'Behavior Technician\')";
            this.ProviderTypeLabel.Name = "ProviderTypeLabel";
            // 
            // xrLine4
            // 
            this.xrLine4.Dpi = 100F;
            this.xrLine4.LocationFloat = new DevExpress.Utils.PointFloat(145.3212F, 67.75004F);
            this.xrLine4.Name = "xrLine4";
            this.xrLine4.SizeF = new System.Drawing.SizeF(233.496F, 5.958321F);
            // 
            // xrLabel5
            // 
            this.xrLabel5.Dpi = 100F;
            this.xrLabel5.Font = new System.Drawing.Font("Times New Roman", 12F);
            this.xrLabel5.ForeColor = System.Drawing.Color.White;
            this.xrLabel5.LocationFloat = new DevExpress.Utils.PointFloat(173.7083F, 40.58335F);
            this.xrLabel5.Name = "xrLabel5";
            this.xrLabel5.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel5.SizeF = new System.Drawing.SizeF(175.8206F, 23F);
            this.xrLabel5.StyleName = "Title";
            this.xrLabel5.StylePriority.UseFont = false;
            this.xrLabel5.StylePriority.UseForeColor = false;
            this.xrLabel5.Text = "SIGN_HERE_PLEASE";
            // 
            // PatientHoursReport
            // 
            this.Bands.AddRange(new DevExpress.XtraReports.UI.Band[] {
            this.Detail,
            this.TopMargin,
            this.BottomMargin,
            this.pageFooterBand1,
            this.reportHeaderBand1,
            this.DetailReport1});
            this.CalculatedFields.AddRange(new DevExpress.XtraReports.UI.CalculatedField[] {
            this.PatientName,
            this.ProviderName,
            this.BCBAName,
            this.BCBASignature,
            this.ProviderSignature,
            this.calculatedField1,
            this.FinalizationID,
            this.ProviderTypeLabel});
            this.ComponentStorage.AddRange(new System.ComponentModel.IComponent[] {
            this.sqlDataSource1});
            this.DataSource = this.sqlDataSource1;
            this.Margins = new System.Drawing.Printing.Margins(74, 64, 100, 100);
            this.Parameters.AddRange(new DevExpress.XtraReports.Parameters.Parameter[] {
            this.CaseID,
            this.StartDate,
            this.EndDate,
            this.ProviderID});
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
