using AABC.Data.V2.Mappings;
using AABC.Domain2.Authorizations;
using AABC.Domain2.Cases;
using AABC.Domain2.Hours;
using AABC.Domain2.Infrastructure;
using AABC.Domain2.Insurances;
using AABC.Domain2.Integrations.Catalyst;
using AABC.Domain2.Notes;
using AABC.Domain2.PatientPortal;
using AABC.Domain2.Patients;
using AABC.Domain2.Payments;
using AABC.Domain2.Providers;
using AABC.Domain2.Referrals;
using AABC.Domain2.Services;
using AABC.Domain2.Staff;
using Dymeng.Framework.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;

namespace AABC.Data.V2
{
    public class CoreContext : DbContext
    {

        public CoreContext()
            : base("CoreConnection")
        {
            Database.SetInitializer<CoreContext>(null);
            CatalystProcedures = new Procedures.CatalystProcedures(this);
        }


        public Procedures.CatalystProcedures CatalystProcedures { get; set; }

        public DbSet<Authorization> Authorizations { get; set; }
        public DbSet<AuthorizationBreakdown> AuthorizationBreakdowns { get; set; }
        public DbSet<AuthorizationClass> AuthorizationClass { get; set; }
        public DbSet<AuthorizationCode> AuthorizationCode { get; set; }
        public DbSet<AuthorizationMatchRule> AuthorizationMatchRules { get; set; }

        public virtual DbSet<Case> Cases { get; set; }
        public DbSet<CaseProvider> CaseProviders { get; set; }
        public DbSet<CaseInsurance> CaseInsurances { get; set; }
        public DbSet<CaseInsuranceMaxOutOfPocket> CaseInsurancesMaxOutOfPocket { get; set; }
        public DbSet<CaseInsurancePayment> CaseInsurancePayments { get; set; }
        public DbSet<CasePaymentPlan> CasePaymentPlans { get; set; }
        public DbSet<CaseBillingCorrespondence> CaseBillingCorrespondences { get; set; }
        public DbSet<CaseBillingCorrespondenceType> CaseBillingCorrespondenceTypes { get; set; }
        public DbSet<CreditCard> CreditCards { get; set; }
        public virtual DbSet<Insurance> Insurances { get; set; }
        public DbSet<LocalCarrier> InsuranceLocalCarriers { get; set; }
        public DbSet<InsuranceService> InsuranceServices { get; set; }
        public DbSet<InsuranceServiceDefault> InsuranceServiceDefaults { get; set; }

        public DbSet<CaseNote> CaseNotes { get; set; }
        public DbSet<CaseNoteTask> CaseNoteTasks { get; set; }
        public DbSet<ReferralNote> ReferralNotes { get; set; }
        public DbSet<ReferralNoteTask> ReferralNoteTasks { get; set; }
        public DbSet<ProviderNote> ProviderNotes { get; set; }
        public DbSet<ProviderNoteTask> ProviderNoteTasks { get; set; }


        public DbSet<ExtendedNote> ExtendedNotes { get; set; }
        public DbSet<ExtendedNoteTemplate> ExtendedNoteTemplates { get; set; }
        public DbSet<ExtendedNoteTemplateGroup> ExtendedNoteTemplateGroups { get; set; }
        public DbSet<Hours> Hours { get; set; }
        public DbSet<HoursFinalization> HoursFinalizations { get; set; }
        public DbSet<SessionReportConfiguration> SessionReportConfigurations { get; set; }
        public DbSet<SessionReport> SessionReports { get; set; }
        public DbSet<SessionSignature> SessionSignatures { get; set; }
        public DbSet<ReportLogItem> HoursReportLogItems { get; set; }
        public DbSet<Language> Languages { get; set; }
        public virtual DbSet<Patient> Patients { get; set; }
        public DbSet<Login> PatientPortalLogins { get; set; }
        public DbSet<PatientPortalSignIn> PatientPortalSignIns { get; set; }
        public DbSet<MonthlyPeriod> MonthlyPeriods { get; set; }
        public DbSet<ParentApproval> ParentApprovals { get; set; }
        public DbSet<ParentSignature> ParentSignatures { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<PaymentCharge> PaymentCharges { get; set; }
        public DbSet<PaymentSchedule> PaymentSchedules { get; set; }
        public DbSet<Provider> Providers { get; set; }
        public DbSet<PortalUser> ProviderPortalUsers { get; set; }
        public DbSet<HasDataEntry> CatalystHasDataEntries { get; set; }
        public DbSet<TimesheetPreloadEntry> CatalystPreloadEntries { get; set; }
        public DbSet<ProviderMapping> CatalystProviderMappings { get; set; }
        public DbSet<CaseMapping> CatalystPatientMappings { get; set; }
        public DbSet<ProviderType> ProviderTypes { get; set; }
        public DbSet<ProviderSubType> ProviderSubTypes { get; set; }
        public DbSet<ProviderTypeService> ProviderTypeServices { get; set; }
        public DbSet<ProviderRate> ProviderRates { get; set; }
        public DbSet<ProviderServiceZipCode> ProviderServiceZipCodes { get; set; }
        public DbSet<ProviderInsuranceCredential> ProviderInsuranceCredentials { get; set; }
        public DbSet<CaseRate> ProviderCaseRates { get; set; }
        public DbSet<ServiceRate> ProviderServiceRates { get; set; }


        public virtual DbSet<Referral> Referrals { get; set; }
        public DbSet<ReferralEnumItem> ReferralEnums { get; set; }
        public DbSet<ReferralDismissalReason> ReferralDismissalReasons { get; set; }
        public DbSet<ReferralSourceType> ReferralSourceTypes { get; set; }
        public virtual DbSet<ReferralChecklistItem> ReferralChecklistItems { get; set; }
        public DbSet<ReferralChecklist> ReferralChecklist { get; set; }

        public DbSet<Service> Services { get; set; }
        //public DbSet<ServiceArea> ServiceAreas { get; set; }
        public DbSet<ServiceLocation> ServiceLocations { get; set; }
        public DbSet<WebMembershipDetail> PatientPortalWebMembershipDetails { get; set; }
        public DbSet<Terms> Terms { get; set; }

        public DbSet<Staff> Staff { get; set; }
        public DbSet<StaffingLog> StaffingLog { get; set; }
        public DbSet<StaffingLogProvider> StaffingLogProviders { get; set; }
        public DbSet<StaffingLogProviderStatus> StaffingLogProviderStatuses { get; set; }
        public DbSet<StaffingLogProviderContactLog> StaffingLogProviderContactLog { get; set; }
        public DbSet<StaffingLogParentContactLog> StaffingLogParentContactLog { get; set; }
        public DbSet<SpecialAttentionNeed> SpecialAttentionNeeds { get; set; }

        public DbSet<FunctioningLevel> FunctioningLevels { get; set; }

        public DbSet<ZipInfo> ZipCodes { get; set; }



        protected override void OnModelCreating(DbModelBuilder mb)
        {
            // We're working on porting the bulk of the mapping code into more
            // organized classes.  New code should go an an appropriate Mapping
            // class, and if you get bored or want to do some cleanup refactoring
            // you can port things from this method to the mappings.

            // new mappings
            AuthorizationMappings.Map(mb);
            CaseMappings.Map(mb);
            CatalystMappings.Map(mb);
            InsuranceMappings.Map(mb);
            ProviderMappings.Map(mb);
            ReferralMappings.Map(mb);
            InfrastructureMappings.Map(mb);


            mb.Configurations.Add(new CaseNoteTypeConfiguration());
            mb.Configurations.Add(new CaseNoteTaskTypeConfiguration());
            mb.Configurations.Add(new ReferralNoteTypeConfiguration());
            mb.Configurations.Add(new ReferralNoteTaskTypeConfiguration());
            mb.Configurations.Add(new ProviderNoteTypeConfiguration());
            mb.Configurations.Add(new ProviderNoteTaskTypeConfiguration());

            mb.Configurations.Add(new SessionReportTypeConfiguration());
            mb.Configurations.Add(new SessionReportConfigurationTypeConfiguration());

            // old stuff that's pending refactoring
            var extendedNoteConfig = mb.Entity<ExtendedNote>();
            extendedNoteConfig.ToTable("CaseAuthHoursNotes");
            extendedNoteConfig.Property(m => m.Answer).HasColumnName("NotesAnswer");
            extendedNoteConfig.Property(m => m.TemplateID).HasColumnName("NotesTemplateID");

            mb.Entity<Insurance>().ToTable("dbo.Insurances");
            mb.Entity<Insurance>().Property(x => x.Name).HasColumnName("InsuranceName");


            var hoursMappingConfig = mb.Entity<Hours>();
            hoursMappingConfig.ToTable("CaseAuthHours");
            hoursMappingConfig.Property(x => x.AuthorizationID).HasColumnName("CaseAuthID");
            hoursMappingConfig.Property(x => x.ProviderID).HasColumnName("CaseProviderID");
            hoursMappingConfig.Property(x => x.Date).HasColumnName("HoursDate");
            hoursMappingConfig.Property(x => x.StartTime).HasColumnName("HoursTimeIn");
            hoursMappingConfig.Property(x => x.EndTime).HasColumnName("HoursTimeOut");
            hoursMappingConfig.Property(x => x.TotalHours).HasColumnName("HoursTotal");
            hoursMappingConfig.Property(x => x.ServiceID).HasColumnName("HoursServiceID");
            hoursMappingConfig.Property(x => x.Memo).HasColumnName("HoursNotes");
            hoursMappingConfig.Property(x => x.CaseID).HasColumnName("CaseID");
            hoursMappingConfig.Property(x => x.Status).HasColumnName("HoursStatus");
            hoursMappingConfig.Property(x => x.BillableHours).HasColumnName("HoursBillable");
            hoursMappingConfig.Property(x => x.PayableHours).HasColumnName("HoursPayable");
            hoursMappingConfig.Property(x => x.BillingRef).HasColumnName("HoursBillingRef");
            hoursMappingConfig.Property(x => x.PayableRef).HasColumnName("HoursPayableRef");
            hoursMappingConfig.Property(x => x.HasCatalystData).HasColumnName("HoursHasCatalystData");
            hoursMappingConfig.Property(x => x.WatchEnabled).HasColumnName("HoursWatchEnabled");
            hoursMappingConfig.Property(x => x.WatchNote).HasColumnName("HoursWatchNote");
            hoursMappingConfig.Property(x => x.SSGParentID).HasColumnName("HoursSSGParentID");
            hoursMappingConfig.Property(x => x.CorrelationID).HasColumnName("HoursCorrelationID");
            hoursMappingConfig.Property(x => x.InternalMemo).HasColumnName("HoursInternalNotes");
            hoursMappingConfig.Property(x => x.IsAdjustment).HasColumnName("IsPayrollOrBillingAdjustment");
            hoursMappingConfig.Property(x => x.IsTrainingEntry).HasColumnName("HoursTrainingEntry");
            hoursMappingConfig.Property(x => x.EntryApp).HasColumnName("HoursEntryApp");
            hoursMappingConfig
                .HasRequired(x => x.Case)
                .WithMany(x => x.Hours)
                .HasForeignKey(x => x.CaseID);
            hoursMappingConfig
                .HasMany(x => x.ReportLog)
                .WithRequired(x => x.Hours)
                .HasForeignKey(x => x.HoursID);
            hoursMappingConfig
                .HasMany(m => m.ExtendedNotes)
                .WithRequired(m => m.HoursEntry)
                .HasForeignKey(m => m.HoursID);
            hoursMappingConfig
                .HasOptional(m => m.SessionSignature)
                .WithRequired();
            hoursMappingConfig
                .HasOptional(m => m.Report)
                .WithRequired(m => m.Session)
                .WillCascadeOnDelete(true);
            //hoursMappingConfig
            //    .HasOptional(x => x.Authorization)
            //    .WithMany(x => x.Hours)
            //    .HasForeignKey(x => x.AuthorizationID);

            var sessionSignatureModelConfig = mb.Entity<SessionSignature>();
            sessionSignatureModelConfig.HasKey(m => m.ID);
            sessionSignatureModelConfig.Property(m => m.ParentSignature).IsUnicode(false);
            sessionSignatureModelConfig.Property(m => m.ParentName).IsUnicode(true).HasMaxLength(300);
            sessionSignatureModelConfig.Property(m => m.ParentSignatureType).IsUnicode(false).HasMaxLength(200);
            sessionSignatureModelConfig.Property(m => m.ProviderSignature).IsUnicode(false);
            sessionSignatureModelConfig.Property(m => m.ProviderName).IsUnicode(true).HasMaxLength(300);
            sessionSignatureModelConfig.Property(m => m.ProviderSignatureType).IsUnicode(false).HasMaxLength(200);


            mb.Entity<HoursFinalization>().ToTable("CaseMonthlyPeriodProviderFinalizations");
            mb.Entity<HoursFinalization>().Property(x => x.PeriodID).HasColumnName("CaseMonthlyPeriodID");
            mb.Entity<HoursFinalization>().Property(x => x.PeriodID).HasColumnName("CaseMonthlyPeriodID");


            mb.Entity<ExtendedNote>().ToTable("CaseAuthHoursNotes");
            mb.Entity<ExtendedNote>().Property(x => x.TemplateID).HasColumnName("NotesTemplateID");
            mb.Entity<ExtendedNote>().Property(x => x.Answer).HasColumnName("NotesAnswer");
            mb.Entity<ExtendedNote>()
                .HasRequired(x => x.Template)
                .WithMany()
                .HasForeignKey(x => x.TemplateID);

            mb.Entity<ExtendedNoteTemplate>().ToTable("HoursNoteTemplates");
            mb.Entity<ExtendedNoteTemplate>().Property(x => x.GroupID).HasColumnName("TemplateGroupID");
            mb.Entity<ExtendedNoteTemplate>().Property(x => x.ProviderTypeID).HasColumnName("TemplateProviderTypeID");
            mb.Entity<ExtendedNoteTemplate>().Property(x => x.Text).HasColumnName("TemplateText");
            mb.Entity<ExtendedNoteTemplate>().Property(x => x.DisplaySequence).HasColumnName("TemplateDisplaySequence");
            mb.Entity<ExtendedNoteTemplate>()
                .HasRequired(x => x.Group)
                .WithMany(x => x.Notes)
                .HasForeignKey(x => x.GroupID);

            mb.Entity<ExtendedNoteTemplateGroup>().ToTable("HoursNoteTemplateGroups");
            mb.Entity<ExtendedNoteTemplateGroup>().Property(x => x.Name).HasColumnName("GroupName");



            mb.Entity<ReportLogItem>().ToTable("CaseAuthHoursReportLog");
            mb.Entity<ReportLogItem>().Property(x => x.LoginID).HasColumnName("LogLoginID");
            mb.Entity<ReportLogItem>().Property(x => x.HoursID).HasColumnName("LogHoursID");
            mb.Entity<ReportLogItem>().Property(x => x.Message).HasColumnName("LogMessage");
            mb.Entity<ReportLogItem>().Property(x => x.ResolvedMessage).HasColumnName("LogResolutionNote");
            mb.Entity<ReportLogItem>().Property(x => x.DateReported).HasColumnName("DateCreated");
            mb.Entity<ReportLogItem>()
                .HasRequired(x => x.ReportedBy)
                .WithMany(x => x.ReportLogs)
                .HasForeignKey(x => x.LoginID);


            mb.Entity<Staff>().ToTable("Staff");



            mb.Entity<Login>().ToTable("PatientPortalLogins");
            mb.Entity<Login>().Property(x => x.Email).HasColumnName("LoginEmail");
            mb.Entity<Login>().Property(x => x.FirstName).HasColumnName("LoginFirstName");
            mb.Entity<Login>().Property(x => x.LastName).HasColumnName("LoginLastName");
            mb.Entity<Login>().Property(x => x.Password).HasColumnName("LoginPassword");
            mb.Entity<Login>()
                .HasMany(x => x.Patients)
                .WithMany(x => x.PatientPortalLogins)
                .Map(m =>
                {
                    m.ToTable("PatientPortalLoginPatients");
                    m.MapLeftKey("LoginID");
                    m.MapRightKey("PatientID");
                });
            mb.Entity<Login>()
               .HasMany(x => x.Payments)
               .WithRequired(x => x.Login)
               .HasForeignKey(x => x.LoginId);


            mb.Entity<PatientPortalSignIn>().ToTable("PatientPortalSignIns");
            mb.Entity<PatientPortalSignIn>()
               .HasRequired(x => x.PatientPortalWebMembership)
               .WithMany(x => x.PatientPortalSignIns)
               .HasForeignKey(x => x.UserId);


            mb.Entity<MonthlyPeriod>().ToTable("CaseMonthlyPeriods");
            mb.Entity<MonthlyPeriod>().Property(x => x.FirstDayOfMonth).HasColumnName("PeriodFirstDayOfMonth");
            mb.Entity<MonthlyPeriod>().Ignore(x => x.Hours);
            mb.Entity<MonthlyPeriod>()
                .HasRequired(x => x.Case)
                .WithMany(x => x.Periods)
                .HasForeignKey(x => x.CaseID);


            mb.Entity<ParentApproval>().ToTable("CaseMonthlyPeriodParentApprovals");
            mb.Entity<ParentApproval>().Property(x => x.ParentLoginID).HasColumnName("PatientPortalLoginID");
            mb.Entity<ParentApproval>().Property(x => x.PeriodID).HasColumnName("MonthlyPeriodID");
            mb.Entity<ParentApproval>().Property(x => x.DateApproved).HasColumnName("ApprovalDate");
            mb.Entity<ParentApproval>()
                .HasRequired(x => x.Period)
                .WithMany(x => x.ParentApprovals)
                .HasForeignKey(x => x.PeriodID);
            mb.Entity<ParentApproval>()
                .HasRequired(x => x.ParentLogin)
                .WithMany(x => x.CasePeriodApprovals)
                .HasForeignKey(x => x.ParentLoginID);



            mb.Entity<ParentSignature>().ToTable("PatientPortalLoginSignatures");
            mb.Entity<ParentSignature>().Property(x => x.Data).HasColumnName("SignatureData");
            mb.Entity<ParentSignature>().Property(x => x.Date).HasColumnName("SignatureDate");
            mb.Entity<ParentSignature>()
                .HasRequired(x => x.Login)
                .WithMany(x => x.Signatures)
                .HasForeignKey(x => x.LoginID);




            mb.Entity<Patient>().Property(x => x.GeneratingReferralID).HasColumnName("PatientGeneratingReferralID");
            mb.Entity<Patient>().Property(x => x.FirstName).HasColumnName("PatientFirstName");
            mb.Entity<Patient>().Property(x => x.LastName).HasColumnName("PatientLastName");
            mb.Entity<Patient>().Property(x => x.DateOfBirth).HasColumnName("PatientDateOfBirth");
            mb.Entity<Patient>().Property(x => x.Gender).HasColumnName("PatientGender");
            mb.Entity<Patient>().Property(x => x.PrimarySpokenLangauge).HasColumnName("PatientPrimarySpokenLangauge");
            mb.Entity<Patient>().Property(x => x.GuardianFirstName).HasColumnName("PatientGuardianFirstName");
            mb.Entity<Patient>().Property(x => x.GuardianLastName).HasColumnName("PatientGuardianLastName");
            mb.Entity<Patient>().Property(x => x.GuardianRelationship).HasColumnName("PatientGuardianRelationship");
            mb.Entity<Patient>().Property(x => x.Email).HasColumnName("PatientEmail");
            mb.Entity<Patient>().Property(x => x.Phone).HasColumnName("PatientPhone");
            mb.Entity<Patient>().Property(x => x.Address1).HasColumnName("PatientAddress1");
            mb.Entity<Patient>().Property(x => x.Address2).HasColumnName("PatientAddress2");
            mb.Entity<Patient>().Property(x => x.City).HasColumnName("PatientCity");
            mb.Entity<Patient>().Property(x => x.State).HasColumnName("PatientState");
            mb.Entity<Patient>().Property(x => x.Zip).HasColumnName("PatientZip");
            mb.Entity<Patient>().Property(x => x.InsuranceCompanyName).HasColumnName("PatientInsuranceCompanyName");
            mb.Entity<Patient>().Property(x => x.InsuranceMemberID).HasColumnName("PatientInsuranceMemberID");
            mb.Entity<Patient>().Property(x => x.InsurancePrimaryCardholderDateOfBirth).HasColumnName("PatientInsurancePrimaryCardholderDateOfBirth");
            mb.Entity<Patient>().Property(x => x.InsuranceCompanyProviderPhone).HasColumnName("PatientInsuranceCompanyProviderPhone");
            mb.Entity<Patient>().Property(x => x.Phone2).HasColumnName("PatientPhone2");
            mb.Entity<Patient>().Property(x => x.PrimarySpokenLanguageID).HasColumnName("PatientPrimarySpokenLanguageID");
            mb.Entity<Patient>().Property(x => x.GuardianRelationshipID).HasColumnName("PatientGuardianRelationshipID");
            mb.Entity<Patient>().Property(x => x.GuardianEmail).HasColumnName("PatientGuardianEmail");
            mb.Entity<Patient>().Property(x => x.GuardianCellPhone).HasColumnName("PatientGuardianCellPhone");
            mb.Entity<Patient>().Property(x => x.GuardianWorkPhone).HasColumnName("PatientGuardianWorkPhone");
            mb.Entity<Patient>().Property(x => x.GuardianHomePhone).HasColumnName("PatientGuardianHomePhone");
            mb.Entity<Patient>().Property(x => x.GuardianNotes).HasColumnName("PatientGuardianNotes");
            mb.Entity<Patient>().Property(x => x.Guardian2FirstName).HasColumnName("PatientGuardian2FirstName");
            mb.Entity<Patient>().Property(x => x.Guardian2LastName).HasColumnName("PatientGuardian2LastName");
            mb.Entity<Patient>().Property(x => x.Guardian2RelationshipID).HasColumnName("PatientGuardian2RelationshipID");
            mb.Entity<Patient>().Property(x => x.Guardian2Email).HasColumnName("PatientGuardian2Email");
            mb.Entity<Patient>().Property(x => x.Guardian2CellPhone).HasColumnName("PatientGuardian2CellPhone");
            mb.Entity<Patient>().Property(x => x.Guardian2HomePhone).HasColumnName("PatientGuardian2HomePhone");
            mb.Entity<Patient>().Property(x => x.Guardian2WorkPhone).HasColumnName("PatientGuardian2WorkPhone");
            mb.Entity<Patient>().Property(x => x.Guardian2Notes).HasColumnName("PatientGuardian2Notes");
            mb.Entity<Patient>().Property(x => x.Guardian3FirstName).HasColumnName("PatientGuardian3FirstName");
            mb.Entity<Patient>().Property(x => x.Guardian3LastName).HasColumnName("PatientGuardian3LastName");
            mb.Entity<Patient>().Property(x => x.Guardian3RelationshipID).HasColumnName("PatientGuardian3RelationshipID");
            mb.Entity<Patient>().Property(x => x.Guardian3Email).HasColumnName("PatientGuardian3Email");
            mb.Entity<Patient>().Property(x => x.Guardian3CellPhone).HasColumnName("PatientGuardian3CellPhone");
            mb.Entity<Patient>().Property(x => x.Guardian3HomePhone).HasColumnName("PatientGuardian3HomePhone");
            mb.Entity<Patient>().Property(x => x.Guardian3WorkPhone).HasColumnName("PatientGuardian3WorkPhone");
            mb.Entity<Patient>().Property(x => x.Guardian3Notes).HasColumnName("PatientGuardian3Notes");
            mb.Entity<Patient>().Property(x => x.Notes).HasColumnName("PatientNotes");
            mb.Entity<Patient>().Property(x => x.PhysicianName).HasColumnName("PatientPhysicianName");
            mb.Entity<Patient>().Property(x => x.PhysicianAddress).HasColumnName("PatientPhysicianAddress");
            mb.Entity<Patient>().Property(x => x.PhysicianPhone).HasColumnName("PatientPhysicianPhone");
            mb.Entity<Patient>().Property(x => x.PhysicianFax).HasColumnName("PatientPhysicianFax");
            mb.Entity<Patient>().Property(x => x.PhysicianEmail).HasColumnName("PatientPhysicianEmail");
            mb.Entity<Patient>().Property(x => x.PhysicianContact).HasColumnName("PatientPhysicianContact");
            mb.Entity<Patient>().Property(x => x.PhysicianNotes).HasColumnName("PatientPhysicianNotes");
            mb.Entity<Patient>().Property(x => x.InsuranceID).HasColumnName("PatientInsuranceID");
            //mb.Entity<Patient>()
            //    .HasRequired(x => x.Insurance)
            //    .WithMany(x => x.Patients)
            //    .HasForeignKey(x => x.InsuranceID);

            mb.Entity<CaseRate>().ToTable("ProviderCaseRates");
            mb.Entity<CaseRate>().Ignore(x => x.Type);
            mb.Entity<CaseRate>().Property(x => x.Rate).HasColumnName("HourlyRate");
            mb.Entity<CaseRate>()
                .HasRequired(x => x.Provider)
                .WithMany(x => x.CaseRates)
                .HasForeignKey(x => x.ProviderID);
            mb.Entity<CaseRate>()
                .HasRequired(x => x.Case)
                .WithMany()
                .HasForeignKey(x => x.CaseID);

            mb.Entity<ServiceRate>().ToTable("ProviderServiceRates");
            mb.Entity<ServiceRate>().Property(x => x.Rate).HasColumnName("HourlyRate");
            mb.Entity<ServiceRate>().Ignore(x => x.Type);
            mb.Entity<ServiceRate>()
                .HasRequired(x => x.Provider)
                .WithMany(x => x.ServiceRates)
                .HasForeignKey(x => x.ProviderID);
            mb.Entity<ServiceRate>()
                .HasRequired(x => x.Service)
                .WithMany()
                .HasForeignKey(x => x.ServiceID);

            mb.Entity<ProviderRate>().Property(x => x.Type).HasColumnName("RateType");
            mb.Entity<ProviderRate>()
                .HasRequired(x => x.Provider)
                .WithMany(x => x.ProviderRates)
                .HasForeignKey(x => x.ProviderID);

            mb.Entity<ProviderTypeService>()
                .HasRequired(x => x.ProviderType)
                .WithMany(x => x.Services)
                .HasForeignKey(x => x.ProviderTypeID);
            mb.Entity<ProviderTypeService>()
                .HasRequired(x => x.Service)
                .WithMany(x => x.ProviderTypeServices)
                .HasForeignKey(x => x.ServiceID);



            mb.Entity<WebMembershipDetail>().ToTable("PatientPortalWebMembership");
            mb.Entity<WebMembershipDetail>().Property(e => e.Password).HasColumnName("MemberPassword");
            mb.Entity<WebMembershipDetail>().Property(e => e.PasswordQuestion).HasColumnName("MemberPasswordQuestion");
            mb.Entity<WebMembershipDetail>().Property(e => e.PasswordAnswer).HasColumnName("MemberPasswordAnswer");
            mb.Entity<WebMembershipDetail>().Property(e => e.IsApproved).HasColumnName("MemberIsApproved");
            mb.Entity<WebMembershipDetail>().Property(e => e.LastActivityDate).HasColumnName("MemberLastActivityDateUTC");
            mb.Entity<WebMembershipDetail>().Property(e => e.LastLoginDate).HasColumnName("MemberLastLoginDateUTC");
            mb.Entity<WebMembershipDetail>().Property(e => e.LastPasswordChangeDate).HasColumnName("MemberLastPasswordChangedDateUTC");
            mb.Entity<WebMembershipDetail>().Property(e => e.CreationDate).HasColumnName("MemberCreationDateUTC");
            mb.Entity<WebMembershipDetail>().Property(e => e.IsLockedOut).HasColumnName("MemberIsLockedOut");
            mb.Entity<WebMembershipDetail>().Property(e => e.LastLockoutDate).HasColumnName("MemberLastLockoutDateUTC");
            mb.Entity<WebMembershipDetail>().Property(e => e.FailedPasswordAttemptCount).HasColumnName("MemberFailedPasswordAttemptCount");
            mb.Entity<WebMembershipDetail>().Property(e => e.FailedPasswordWindowStart).HasColumnName("MemberFailedPasswordWindowStartUTC");
            mb.Entity<WebMembershipDetail>().Property(e => e.FailedPasswordAnswerAttemptCount).HasColumnName("MemberFailedPasswordAnswerAttemptCount");
            mb.Entity<WebMembershipDetail>().Property(e => e.FailedPasswordAnswerAttemptWindowStart).HasColumnName("MemberFailedPasswordAnswerAttemptWindowStartUTC");
            mb.Entity<WebMembershipDetail>()
                .HasOptional(detail => detail.User)
                .WithRequired(user => user.WebMembershipDetail);

            mb.Entity<Terms>().ToTable("PatientPortalTerms")
                .HasMany(c => c.AcceptedTerms)
                .WithRequired()
                .HasForeignKey(c => c.TermsId);

            mb.Entity<AcceptedTerms>()
                .ToTable("PatientPortalAcceptedTerms")
                .HasKey(c => new { c.LoginId, c.TermsId });


            mb.Entity<Login>()
                .HasMany(c => c.AcceptedTerms)
                .WithRequired()
                .HasForeignKey(c => c.LoginId);

            mb.Entity<Service>().Property(x => x.Code).HasColumnName("ServiceCode");
            mb.Entity<Service>().Property(x => x.Name).HasColumnName("ServiceName");
            mb.Entity<Service>().Property(x => x.Description).HasColumnName("ServiceDescription");
            mb.Entity<Service>().Property(x => x.Type).HasColumnName("ServiceTypeID");


            mb.Entity<ServiceLocation>().Property(x => x.Name).HasColumnName("LocationName");
            mb.Entity<ServiceLocation>().Property(x => x.MBHID).HasColumnName("LocationMBHID");


            var paymentEntityConfig = mb.Entity<Payment>();

            paymentEntityConfig
                .HasRequired(x => x.Login)
                .WithMany(x => x.Payments)
                .HasForeignKey(x => x.LoginId);

            paymentEntityConfig
                .HasOptional(x => x.CreditCard)
                .WithMany(x => x.Payments)
                .HasForeignKey(x => x.CreditCardId);

            paymentEntityConfig
                .HasRequired(x => x.Patient)
                .WithMany(x => x.Payments)
                .HasForeignKey(x => x.PatientId);

            paymentEntityConfig
                .HasOptional(m => m.ManagementUser)
                .WithMany()
                .HasForeignKey(m => m.ManagementUserId);


            mb.Entity<PaymentSchedule>()
                .HasRequired(x => x.Payment)
                .WithMany(x => x.PaymentSchedules)
                .HasForeignKey(x => x.PaymentId);

            var paymentChargeConfig = mb.Entity<PaymentCharge>();

            paymentChargeConfig.Property(m => m.Result)
                .IsUnicode(false)
                .HasMaxLength(100);

            paymentChargeConfig.Property(m => m.ResultDetails)
                .IsUnicode(false);

            paymentChargeConfig
                .HasRequired(x => x.Payment)
                .WithMany(x => x.PaymentCharges)
                .HasForeignKey(x => x.PaymentId);


            var creditCardMappingConfig = mb.Entity<CreditCard>();

            creditCardMappingConfig.Property(m => m.ExpiryMonth)
                .IsUnicode(false)
                .HasMaxLength(2);
            creditCardMappingConfig.Property(m => m.ExpiryYear)
                .IsUnicode(false)
                .HasMaxLength(4);
            creditCardMappingConfig
                .HasRequired(x => x.Login)
                .WithMany(x => x.CreditCards)
                .HasForeignKey(x => x.LoginId);

            mb.Entity<CaseInsurance>()
                .HasRequired(x => x.Case)
                .WithMany(x => x.Insurances)
                .HasForeignKey(x => x.CaseID);

            mb.Entity<CaseInsurance>()
                .Property(x => x.CarrierID).HasColumnName("CaseInsuranceCarrierID");

            mb.Entity<CaseInsurance>()
                .HasOptional(x => x.Insurance)
                .WithMany(x => x.Cases)
                .HasForeignKey(x => x.InsuranceID);

            mb.Entity<CaseInsuranceMaxOutOfPocket>()
                .HasRequired(x => x.CaseInsurance)
                .WithMany(x => x.CaseInsurancesMaxOutOfPocket)
                .HasForeignKey(x => x.CaseInsuranceId);

            mb.Entity<CaseInsurancePayment>()
                .HasRequired(x => x.CaseInsurance)
                .WithMany(x => x.CaseInsurancePayments)
                .HasForeignKey(x => x.CaseInsuranceId);

            mb.Entity<CasePaymentPlan>()
                .HasRequired(x => x.Case)
                .WithMany(x => x.PaymentPlans)
                .HasForeignKey(x => x.CaseId);

            mb.Entity<CaseBillingCorrespondence>()
                .HasRequired(x => x.Case)
                .WithMany(x => x.CaseBillingCorrespondences)
                .HasForeignKey(x => x.CaseId);

            mb.Entity<CaseBillingCorrespondence>()
                .HasRequired(x => x.CaseBillingCorrespondenceType)
                .WithMany(x => x.CaseBillingCorrespondences)
                .HasForeignKey(x => x.CorrespondenceTypeId);

            mb.Entity<CaseBillingCorrespondence>()
                .HasRequired(x => x.Staff)
                .WithMany(x => x.BillingCorrespondences)
                .HasForeignKey(x => x.StaffId);

        }

        public IEnumerable<DTOs.ProviderPortalUserAdminListItem> GetProviderPortalUserAdminList()
        {
            return Database.SqlQuery<DTOs.ProviderPortalUserAdminListItem>("SELECT * FROM dbo.ProviderPortalUserAdminList").AsEnumerable();
        }

        public IEnumerable<Case> HoursEntryEligibleCasesWithoutActiveInsurance()
        {
            var caseIDs = Database.SqlQuery<int>("SELECT CaseID FROM hoursEntry.CasesWithoutActiveInsurance");
            return Cases.Where(x => caseIDs.Any(y => y == x.ID));
        }

        public IEnumerable<Case> HoursEntryEligibleCasesWithoutActiveAuthorizations()
        {
            var caseIDs = Database.SqlQuery<int>("SELECT CaseID FROM hoursEntry.CasesWithoutActiveAuthorizations");
            return Cases.Where(x => caseIDs.Any(y => y == x.ID));
        }

        public IEnumerable<Authorization> HoursEntryEligibleAuthorizationsWithoutMatchRules()
        {
            var authIDs = Database.SqlQuery<int>("SELECT CaseAuthorizationID FROM hoursEntry.AuthorizationswithoutRules");
            return Authorizations.Where(x => authIDs.Any(y => y == x.ID));
        }


        public IEnumerable<DTOs.AppliedAuthorizationAndInsuranceMismatchItem> GetAppliedAuthsAndInsuranceMismatches()
        {

            IEnumerable<DTOs.AppliedAuthorizationAndInsuranceMismatchItem> data;

            data = Database.SqlQuery<DTOs.AppliedAuthorizationAndInsuranceMismatchItem>("SELECT * FROM dbo.AppliedAuthorizationAndInsuranceMismatches ORDER BY PatientLastName, PatientFirstName, AuthEndDate");

            var activeOnly = from d in data
                             where (d.AuthEndDate == null || d.AuthEndDate > DateTime.Now)
                                && (d.AuthStartDate == null || d.AuthStartDate <= DateTime.Now)
                             select d;

            return activeOnly;

        }



        public IEnumerable<DTOs.WatchCaseResultItem> GetCasesWithHoursButNoSupervision(DateTime startDate, DateTime endDate)
        {

            IEnumerable<DTOs.WatchCaseResultItem> data = new List<DTOs.WatchCaseResultItem>();

            System.Data.SqlClient.SqlParameter pStartDate = new System.Data.SqlClient.SqlParameter("@StartDate", startDate);
            System.Data.SqlClient.SqlParameter pEndDate = new System.Data.SqlClient.SqlParameter("@EndDate", endDate);

            data = Database.SqlQuery<DTOs.WatchCaseResultItem>(
                "dbo.GetCasesWithHoursButNoSupervision @StartDate, @EndDate",
                pStartDate, pEndDate
                );

            var watch = System.Diagnostics.Stopwatch.StartNew();
            data = data.ToList();
            watch.Stop();
            System.Diagnostics.Debug.WriteLine("NoSuper MS: " + watch.ElapsedMilliseconds);
            return data;
        }

        public List<DTOs.WatchCaseResultItem> GetCasesWithNoBCBAHours(DateTime startDate, DateTime endDate)
        {

            var data = new List<DTOs.WatchCaseResultItem>();

            System.Data.SqlClient.SqlParameter pStartDate = new System.Data.SqlClient.SqlParameter("@StartDate", startDate);
            System.Data.SqlClient.SqlParameter pEndDate = new System.Data.SqlClient.SqlParameter("@EndDate", endDate);

            var watch = System.Diagnostics.Stopwatch.StartNew();



            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection())
            using (SqlCommand cmd = new SqlCommand())
            {

                conn.ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["CoreConnection"].ConnectionString;

                cmd.Connection = conn;

                cmd.CommandText = "dbo.GetCasesWithHoursButNoBCBAHours";
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@StartDate", startDate);
                cmd.Parameters.AddWithValue("@EndDate", endDate);

                System.Diagnostics.Debug.WriteLine("ASDKJSHADF " + startDate.ToString() + "; " + endDate.ToString());
                var table = cmd.GetTable();

                watch.Stop();
                System.Diagnostics.Debug.WriteLine("NoBCBAHours MS: " + watch.ElapsedMilliseconds);


                foreach (DataRow r in table.Rows)
                {
                    data.Add(new DTOs.WatchCaseResultItem()
                    {
                        CaseID = r.ToInt("CaseID"),
                        PatientID = r.ToInt("PatientID"),
                        PatientFirstName = r.ToStringValue("PatientFirstName"),
                        PatientLastName = r.ToStringValue("PatientLastName"),
                        ProviderFirstName = r.ToStringValue("ProviderFirstName"),
                        ProviderLastName = r.ToStringValue("ProviderLastName"),
                        WatchComment = r.ToStringValue("WatchComment"),
                        WatchIgnore = r.ToBool("WatchIgnore")
                    });
                }

            }

            return data;
        }

        public IEnumerable<DTOs.WatchCaseResultItem> GetCasesWithNoAideHours(DateTime startDate, DateTime endDate)
        {

            IEnumerable<DTOs.WatchCaseResultItem> data = new List<DTOs.WatchCaseResultItem>();

            System.Data.SqlClient.SqlParameter pStartDate = new System.Data.SqlClient.SqlParameter("@StartDate", startDate);
            System.Data.SqlClient.SqlParameter pEndDate = new System.Data.SqlClient.SqlParameter("@EndDate", endDate);

            data = Database.SqlQuery<DTOs.WatchCaseResultItem>(
                "dbo.GetCasesWithHoursButNoAideHours @StartDate, @EndDate",
                pStartDate, pEndDate
                );

            var watch = System.Diagnostics.Stopwatch.StartNew();
            data = data.ToList();
            watch.Stop();
            System.Diagnostics.Debug.WriteLine("NoAideHours MS: " + watch.ElapsedMilliseconds);
            return data;
        }

        public IEnumerable<DTOs.WatchCaseResultItem> GetCasesWithAuthButNoHours(DateTime startDate, DateTime endDate)
        {

            IEnumerable<DTOs.WatchCaseResultItem> data = new List<DTOs.WatchCaseResultItem>();

            System.Data.SqlClient.SqlParameter pStartDate = new System.Data.SqlClient.SqlParameter("@StartDate", startDate);
            System.Data.SqlClient.SqlParameter pEndDate = new System.Data.SqlClient.SqlParameter("@EndDate", endDate);

            data = Database.SqlQuery<DTOs.WatchCaseResultItem>(
                "dbo.GetCasesWithAuthsButNotHours @StartDate, @EndDate",
                pStartDate, pEndDate
                );

            var watch = System.Diagnostics.Stopwatch.StartNew();
            data = data.ToList();
            watch.Stop();
            System.Diagnostics.Debug.WriteLine("NoHours MS: " + watch.ElapsedMilliseconds);
            return data;
        }

        public IEnumerable<DTOs.WatchCaseResultItem> GetCasesWithoutHoursBilled(DateTime startDate, DateTime endDate)
        {
            SqlParameter pStartDate = new SqlParameter("@StartDate", startDate);
            SqlParameter pEndDate = new SqlParameter("@EndDate", endDate);

            IEnumerable<DTOs.WatchCaseResultItem> data = Database.SqlQuery<DTOs.WatchCaseResultItem>(
                "dbo.GetCasesWithoutHoursBilled @StartDate, @EndDate",
                pStartDate, pEndDate
            );

            var watch = System.Diagnostics.Stopwatch.StartNew();
            data = data.ToList();
            watch.Stop();
            System.Diagnostics.Debug.WriteLine("dbo.GetCasesWithoutHoursBilled MS: " + watch.ElapsedMilliseconds);
            return data;
        }

    }
}
