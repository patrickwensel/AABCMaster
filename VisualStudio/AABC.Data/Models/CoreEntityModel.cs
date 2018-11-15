namespace AABC.Data.Models
{
    using System.Data.Entity;

    public partial class CoreEntityModel : DbContext
    {
        public CoreEntityModel()
            : base("name=CoreConnection")
        {
        }

        public virtual DbSet<AuthCode> AuthCodes { get; set; }
        public virtual DbSet<CaseAuthClass> CaseAuthClasses { get; set; }
        public virtual DbSet<CaseAuthCodeGeneralHour> CaseAuthCodeGeneralHours { get; set; }
        public virtual DbSet<CaseAuthCode> CaseAuthCodes { get; set; }
        public virtual DbSet<CaseAuthHour> CaseAuthHours { get; set; }
        public virtual DbSet<CaseAuthHoursNote> CaseAuthHoursNotes { get; set; }
        public virtual DbSet<CaseAuthHoursStatus> CaseAuthHoursStatuses { get; set; }
        public virtual DbSet<CaseBillingReport> CaseBillingReports { get; set; }
        public virtual DbSet<CaseMonthlyPeriodProviderFinalization> CaseMonthlyPeriodProviderFinalizations { get; set; }
        public virtual DbSet<CaseMonthlyPeriod> CaseMonthlyPeriods { get; set; }
        public virtual DbSet<CasePayableReport> CasePayableReports { get; set; }
        public virtual DbSet<CaseProviderNote> CaseProviderNotes { get; set; }
        public virtual DbSet<CaseProvider> CaseProviders { get; set; }
        public virtual DbSet<Case> Cases { get; set; }
        public virtual DbSet<CaseStatus> CaseStatuses { get; set; }
        public virtual DbSet<CaseStatusReason> CaseStatusReasons { get; set; }
        public virtual DbSet<CaseTask> CaseTasks { get; set; }
        public virtual DbSet<GuardianRelationship> GuardianRelationships { get; set; }
        public virtual DbSet<HoursNoteTemplateGroup> HoursNoteTemplateGroups { get; set; }
        public virtual DbSet<HoursNoteTemplate> HoursNoteTemplates { get; set; }
        public virtual DbSet<Insurance> Insurances { get; set; }
        public virtual DbSet<ISO639_2_Lang> ISO639_2_Lang { get; set; }
        public virtual DbSet<Language> Languages { get; set; }
        public virtual DbSet<LanguageStrengthType> LanguageStrengthTypes { get; set; }
        public virtual DbSet<Number> Numbers { get; set; }
        public virtual DbSet<Patient> Patients { get; set; }
        public virtual DbSet<ProviderInsuranceBlacklist> ProviderInsuranceBlacklists { get; set; }
        public virtual DbSet<ProviderLanguage> ProviderLanguages { get; set; }
        public virtual DbSet<ProviderPortalUser> ProviderPortalUsers { get; set; }
        public virtual DbSet<Provider> Providers { get; set; }
        public virtual DbSet<ProviderServiceZipCode> ProviderServiceZipCodes { get; set; }
        public virtual DbSet<ProviderType> ProviderTypes { get; set; }
        public virtual DbSet<ProviderTypeService> ProviderTypeServices { get; set; }
        public virtual DbSet<ReferralChecklist> ReferralChecklists { get; set; }
        public virtual DbSet<ReferralChecklistItem> ReferralChecklistItems { get; set; }
        public virtual DbSet<ReferralDismissalReason> ReferralDismissalReasons { get; set; }
        public virtual DbSet<Referral> Referrals { get; set; }
        public virtual DbSet<ReferralEmail> ReferralEmails { get; set; }
        public virtual DbSet<ReferralSetting> ReferralSettings { get; set; }
        public virtual DbSet<ReferralSourceType> ReferralSourceTypes { get; set; }
        public virtual DbSet<ReferralStatus> ReferralStatuses { get; set; }
        public virtual DbSet<ServiceLocation> ServiceLocations { get; set; }
        public virtual DbSet<Service> Services { get; set; }
        public virtual DbSet<SettingValueType> SettingValueTypes { get; set; }
        public virtual DbSet<SMTPAccount> SMTPAccounts { get; set; }
        public virtual DbSet<Staff> Staffs { get; set; }
        public virtual DbSet<WebOptionGroup> WebOptionGroups { get; set; }
        public virtual DbSet<WebOption> WebOptions { get; set; }
        public virtual DbSet<WebPermissionGroup> WebPermissionGroups { get; set; }
        public virtual DbSet<WebPermission> WebPermissions { get; set; }
        public virtual DbSet<WebUserOption> WebUserOptions { get; set; }
        public virtual DbSet<WebUserPermission> WebUserPermissions { get; set; }
        public virtual DbSet<WebUser> WebUsers { get; set; }
        public virtual DbSet<ZipCode> ZipCodes { get; set; }
        public virtual DbSet<AllLanguage> AllLanguages { get; set; }
        public virtual DbSet<CommonLanguage> CommonLanguages { get; set; }
        public virtual DbSet<HoursProviderCost> HoursProviderCosts { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AuthCode>()
                .Property(e => e.rv)
                .IsFixedLength();

            modelBuilder.Entity<CaseAuthClass>()
                .Property(e => e.rv)
                .IsFixedLength();

            modelBuilder.Entity<CaseAuthClass>()
                .HasMany(e => e.CaseAuthCodes)
                .WithRequired(e => e.CaseAuthClass)
                .HasForeignKey(e => e.AuthClassID);

            modelBuilder.Entity<CaseAuthClass>()
                .HasMany(e => e.ProviderTypeServices)
                .WithOptional(e => e.CaseAuthClass)
                .HasForeignKey(e => e.AssociatedAuthClassID);

            modelBuilder.Entity<CaseAuthCodeGeneralHour>()
                .Property(e => e.rv)
                .IsFixedLength();

            modelBuilder.Entity<CaseAuthCodeGeneralHour>()
                .Property(e => e.HoursApplied)
                .HasPrecision(6, 2);

            modelBuilder.Entity<CaseAuthCode>()
                .Property(e => e.rv)
                .IsFixedLength();

            modelBuilder.Entity<CaseAuthCode>()
                .Property(e => e.AuthTotalHoursApproved)
                .HasPrecision(6, 2);

            modelBuilder.Entity<CaseAuthCode>()
                .HasMany(e => e.CaseAuthCodeGeneralHours)
                .WithRequired(e => e.CaseAuthCode)
                .HasForeignKey(e => e.CaseAuthID);

            modelBuilder.Entity<CaseAuthHour>()
                .Property(e => e.rv)
                .IsFixedLength();

            modelBuilder.Entity<CaseAuthHour>()
                .Property(e => e.HoursTotal)
                .HasPrecision(6, 2);

            modelBuilder.Entity<CaseAuthHour>()
                .Property(e => e.HoursBillable)
                .HasPrecision(6, 2);

            modelBuilder.Entity<CaseAuthHour>()
                .Property(e => e.HoursPayable)
                .HasPrecision(6, 2);

            modelBuilder.Entity<CaseAuthHour>()
                .HasMany(e => e.CaseAuthHoursNotes)
                .WithRequired(e => e.CaseAuthHour)
                .HasForeignKey(e => e.HoursID);

            modelBuilder.Entity<CaseAuthHoursNote>()
                .Property(e => e.rv)
                .IsFixedLength();

            modelBuilder.Entity<CaseBillingReport>()
                .Property(e => e.rv)
                .IsFixedLength();

            modelBuilder.Entity<CaseMonthlyPeriodProviderFinalization>()
                .Property(e => e.rv)
                .IsFixedLength();

            modelBuilder.Entity<CaseMonthlyPeriod>()
                .Property(e => e.rv)
                .IsFixedLength();

            modelBuilder.Entity<CaseMonthlyPeriod>()
                .HasMany(e => e.CaseBillingReports)
                .WithRequired(e => e.CaseMonthlyPeriod)
                .HasForeignKey(e => e.ReportPeriodID);

            modelBuilder.Entity<CasePayableReport>()
                .Property(e => e.rv)
                .IsFixedLength();

            modelBuilder.Entity<CaseProviderNote>()
                .Property(e => e.rv)
                .IsFixedLength();

            modelBuilder.Entity<CaseProvider>()
                .Property(e => e.rv)
                .IsFixedLength();

            modelBuilder.Entity<Case>()
                .Property(e => e.rv)
                .IsFixedLength();

            modelBuilder.Entity<CaseTask>()
                .Property(e => e.rv)
                .IsFixedLength();

            modelBuilder.Entity<GuardianRelationship>()
                .HasMany(e => e.Patients)
                .WithOptional(e => e.GuardianRelationship)
                .HasForeignKey(e => e.PatientGuardianRelationshipID);

            modelBuilder.Entity<HoursNoteTemplateGroup>()
                .Property(e => e.rv)
                .IsFixedLength();

            modelBuilder.Entity<HoursNoteTemplateGroup>()
                .HasMany(e => e.HoursNoteTemplates)
                .WithRequired(e => e.HoursNoteTemplateGroup)
                .HasForeignKey(e => e.TemplateGroupID);

            modelBuilder.Entity<HoursNoteTemplate>()
                .Property(e => e.rv)
                .IsFixedLength();

            modelBuilder.Entity<HoursNoteTemplate>()
                .HasMany(e => e.CaseAuthHoursNotes)
                .WithRequired(e => e.HoursNoteTemplate)
                .HasForeignKey(e => e.NotesTemplateID)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Insurance>()
                .Property(e => e.rv)
                .IsFixedLength();

            modelBuilder.Entity<ISO639_2_Lang>()
                .HasMany(e => e.ProviderLanguages)
                .WithRequired(e => e.ISO639_2_Lang)
                .HasForeignKey(e => e.LanguageID);

            modelBuilder.Entity<Language>()
                .Property(e => e.rv)
                .IsFixedLength();

            modelBuilder.Entity<LanguageStrengthType>()
                .HasMany(e => e.ProviderLanguages)
                .WithRequired(e => e.LanguageStrengthType)
                .HasForeignKey(e => e.LanguageStrength);

            modelBuilder.Entity<Patient>()
                .Property(e => e.rv)
                .IsFixedLength();

            modelBuilder.Entity<ProviderInsuranceBlacklist>()
                .Property(e => e.rv)
                .IsFixedLength();

            modelBuilder.Entity<ProviderLanguage>()
                .Property(e => e.rv)
                .IsFixedLength();

            modelBuilder.Entity<ProviderPortalUser>()
                .Property(e => e.rv)
                .IsFixedLength();

            modelBuilder.Entity<Provider>()
                .Property(e => e.rv)
                .IsFixedLength();

            modelBuilder.Entity<Provider>()
                .Property(e => e.ProviderRate)
                .HasPrecision(6, 2);

            modelBuilder.Entity<ProviderServiceZipCode>()
                .Property(e => e.rv)
                .IsFixedLength();

            modelBuilder.Entity<ProviderType>()
                .Property(e => e.rv)
                .IsFixedLength();

            modelBuilder.Entity<ProviderType>()
                .HasMany(e => e.Providers)
                .WithOptional(e => e.ProviderType1)
                .HasForeignKey(e => e.ProviderType);

            modelBuilder.Entity<ProviderTypeService>()
                .Property(e => e.rv)
                .IsFixedLength();

            modelBuilder.Entity<ReferralChecklist>()
                .Property(e => e.rv)
                .IsFixedLength();

            modelBuilder.Entity<ReferralChecklistItem>()
                .Property(e => e.rv)
                .IsFixedLength();

            modelBuilder.Entity<ReferralChecklistItem>()
                .HasMany(e => e.ReferralChecklists)
                .WithRequired(e => e.ReferralChecklistItem)
                .HasForeignKey(e => e.ChecklistItemID);

            modelBuilder.Entity<ReferralDismissalReason>()
                .Property(e => e.rv)
                .IsFixedLength();

            modelBuilder.Entity<Referral>()
                .Property(e => e.rv)
                .IsFixedLength();

            modelBuilder.Entity<ReferralSetting>()
                .Property(e => e.rv)
                .IsFixedLength();

            modelBuilder.Entity<ReferralSourceType>()
                .Property(e => e.rv)
                .IsFixedLength();

            modelBuilder.Entity<ReferralSourceType>()
                .HasMany(e => e.Referrals)
                .WithOptional(e => e.ReferralSourceType1)
                .HasForeignKey(e => e.ReferralSourceType);

            modelBuilder.Entity<ReferralStatus>()
                .Property(e => e.rv)
                .IsFixedLength();

            modelBuilder.Entity<ServiceLocation>()
                .Property(e => e.rv)
                .IsFixedLength();

            modelBuilder.Entity<Service>()
                .Property(e => e.rv)
                .IsFixedLength();

            modelBuilder.Entity<SettingValueType>()
                .HasMany(e => e.ReferralSettings)
                .WithRequired(e => e.SettingValueType1)
                .HasForeignKey(e => e.SettingValueType);

            modelBuilder.Entity<SMTPAccount>()
                .Property(e => e.rv)
                .IsFixedLength();

            modelBuilder.Entity<Staff>()
                .Property(e => e.rv)
                .IsFixedLength();

            modelBuilder.Entity<Staff>()
                .HasMany(e => e.CaseTasks)
                .WithOptional(e => e.Staff)
                .HasForeignKey(e => e.TaskCompletedByStaffID);

            modelBuilder.Entity<Staff>()
                .HasMany(e => e.Referrals)
                .WithOptional(e => e.Staff)
                .HasForeignKey(e => e.ReferralEnteredByStaffID);

            modelBuilder.Entity<WebPermission>()
                .HasMany(e => e.WebUserPermissions)
                .WithRequired(e => e.WebPermission)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<WebUserOption>()
                .Property(e => e.rv)
                .IsFixedLength();

            modelBuilder.Entity<WebUserPermission>()
                .Property(e => e.rv)
                .IsFixedLength();

            modelBuilder.Entity<WebUser>()
                .Property(e => e.rv)
                .IsFixedLength();

            modelBuilder.Entity<WebUser>()
                .HasMany(e => e.CaseBillingReports)
                .WithOptional(e => e.WebUser)
                .HasForeignKey(e => e.ReportGeneratedByUserID);

            modelBuilder.Entity<WebUser>()
                .HasMany(e => e.WebUserOptions)
                .WithRequired(e => e.WebUser)
                .HasForeignKey(e => e.WebUserID)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<WebUser>()
                .HasMany(e => e.WebUserOptions1)
                .WithRequired(e => e.WebUser1)
                .HasForeignKey(e => e.WebUserID);

            modelBuilder.Entity<WebUser>()
                .HasMany(e => e.WebUserPermissions)
                .WithRequired(e => e.WebUser)
                .HasForeignKey(e => e.WebUserID)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<WebUser>()
                .HasMany(e => e.WebUserPermissions1)
                .WithRequired(e => e.WebUser1)
                .HasForeignKey(e => e.WebUserID);

            modelBuilder.Entity<ZipCode>()
                .Property(e => e.ZipLatitude)
                .HasPrecision(20, 10);

            modelBuilder.Entity<ZipCode>()
                .Property(e => e.ZipLongitude)
                .HasPrecision(20, 10);

            modelBuilder.Entity<ZipCode>()
                .HasMany(e => e.ProviderServiceZipCodes)
                .WithRequired(e => e.ZipCode1)
                .HasForeignKey(e => e.ZipCode);

            modelBuilder.Entity<HoursProviderCost>()
                .Property(e => e.ProviderRate)
                .HasPrecision(6, 2);

            modelBuilder.Entity<HoursProviderCost>()
                .Property(e => e.HoursTotal)
                .HasPrecision(6, 2);

            modelBuilder.Entity<HoursProviderCost>()
                .Property(e => e.ProviderCost)
                .HasPrecision(13, 4);
        }
    }
}
