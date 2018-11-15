using AABC.Domain2.Cases;
using AABC.Domain2.Infrastructure;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;

namespace AABC.Data.V2.Mappings
{
    static class CaseMappings
    {
        public static void Map(DbModelBuilder modelBuilder)
        {

            var c = modelBuilder.Entity<Case>();

            c.Property(x => x.GeneratingReferralID).HasColumnName("CaseGeneratingReferralID");
            c.Property(x => x.Status).HasColumnName("CaseStatus");
            c.Property(x => x.StatusNotes).HasColumnName("CaseStatusNotes");
            c.Property(x => x.StartDate).HasColumnName("CaseStartDate");
            c.Property(x => x.AssignedStaffID).HasColumnName("CaseAssignedStaffID");
            c.Property(x => x.RequiredHoursNotes).HasColumnName("CaseRequiredHoursNotes");
            c.Property(x => x.RequiredServicesNotes).HasColumnName("CaseRequiredServicesNotes");
            c.Property(x => x.HasPrescription).HasColumnName("CaseHasPrescription");
            c.Property(x => x.HasAssessment).HasColumnName("CaseHasAssessment");
            c.Property(x => x.HasIntake).HasColumnName("CaseHasIntake");
            c.Property(x => x.StatusReason).HasColumnName("CaseStatusReason");
            c.Property(x => x.DischargeNotes).HasColumnName("CaseDischargeNotes");
            c.Property(x => x.DefaultServiceLocationID).HasColumnName("DefaultServiceLocationID");
            c.Property(x => x.NeedsRestaffing).HasColumnName("CaseNeedsRestaffing");
            c.Property(x => x.RestaffingReason).HasColumnName("CaseRestaffingReason");
            c.Property(x => x.NeedsStaffing).HasColumnName("CaseNeedsStaffing");
            c.Property(x => x.RestaffReasonID).HasColumnName("CaseRestaffReasonID");
            c.HasRequired(x => x.Patient)
                .WithMany(x => x.Cases)
                .HasForeignKey(x => x.PatientID);
            c.HasOptional(m => m.FunctioningLevel)
                .WithMany()
                .HasForeignKey(m => m.FunctioningLevelID);


            var caseProvider = modelBuilder.Entity<CaseProvider>();
            caseProvider.Property(x => x.IsAuthorizedBCBA).HasColumnName("IsInsuranceAuthorizedBCBA");
            caseProvider.Property(x => x.StartDate).HasColumnName("ActiveStartDate");
            caseProvider.Property(x => x.EndDate).HasColumnName("ActiveEndDate");



            modelBuilder.Entity<CaseInsurance>().ToTable("dbo.CaseInsurances");

            modelBuilder.Entity<CaseInsuranceMaxOutOfPocket>().ToTable("dbo.CaseInsurancesMaxOutOfPocket");

            modelBuilder.Entity<CaseInsurancePayment>().ToTable("dbo.CaseInsurancePayments");

            modelBuilder.Entity<CaseBillingCorrespondence>().ToTable("dbo.CaseBillingCorrespondences");



            var staffingLogMapping = modelBuilder.Entity<StaffingLog>();
            staffingLogMapping.ToTable("StaffingLog");
            staffingLogMapping.HasKey(m => m.ID);
            staffingLogMapping.HasRequired(m => m.Case)
                .WithOptional();
            staffingLogMapping.Property(m => m.ParentalRestaffRequest).IsOptional().IsUnicode(true).HasMaxLength(2000);
            staffingLogMapping.Property(m => m.Active).HasColumnName("StaffingActive");
            staffingLogMapping.Property(m => m.HoursOfABATherapy).IsOptional().HasPrecision(5, 2);
            staffingLogMapping.Property(m => m.AidesRespondingNo).IsOptional().IsUnicode(true).HasMaxLength(2000);
            staffingLogMapping.Property(m => m.AidesRespondingMaybe).IsOptional().IsUnicode(true).HasMaxLength(2000);
            staffingLogMapping.Property(m => m.DateWentToRestaff).IsOptional().HasColumnType("date");
            staffingLogMapping.Property(m => m.ProviderGenderPreference).IsOptional().HasMaxLength(1).IsOptional().IsFixedLength();

            var staffingLogProviderMapping = modelBuilder.Entity<StaffingLogProvider>();
            staffingLogProviderMapping.HasKey(m => m.ID);
            staffingLogProviderMapping.Property(m => m.ID).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
               staffingLogProviderMapping.HasRequired(m => m.StaffingLog)
                                      .WithMany()
                                      .HasForeignKey(m => m.StaffingLogID);
            staffingLogProviderMapping.HasRequired(m => m.Provider)
                        .WithMany()
                        .HasForeignKey(m => m.ProviderID);

            var functioningLevelMapping = modelBuilder.Entity<FunctioningLevel>();
            functioningLevelMapping.HasKey(m => m.ID);
            functioningLevelMapping.Property(m => m.Name).IsRequired().HasMaxLength(50);

            var zipCodes = modelBuilder.Entity<ZipInfo>();
            zipCodes.ToTable("ZipCodes");
            zipCodes.HasKey(m => m.ZipCode);
            zipCodes.Property(m => m.ZipCode).IsRequired().IsUnicode(true).HasMaxLength(5);
            zipCodes.Property(m => m.City).HasColumnName("ZipCity").IsOptional().IsUnicode(true).HasMaxLength(64);
            zipCodes.Property(m => m.County).HasColumnName("ZipCounty").IsOptional().IsUnicode(true).HasMaxLength(100);
            zipCodes.Property(m => m.State).HasColumnName("ZipState").IsOptional().IsUnicode(true).HasMaxLength(2);
            zipCodes.Property(m => m.TimeZone).HasColumnName("ZipTimeZone").IsOptional();
            zipCodes.Property(m => m.DaylightSavings).HasColumnName("ZipDaylightSavings").IsOptional();

            MapStaffingLogProviderStatus(modelBuilder);
            MapStaffingLogProviderContactLog(modelBuilder);
            MapStaffingLogParentContactLog(modelBuilder);
            MapSpecialAttentionNeeds(modelBuilder);
        }

        public static void MapStaffingLogProviderStatus(DbModelBuilder modelBuilder)
        {
            var statusMapping = modelBuilder.Entity<StaffingLogProviderStatus>();
            statusMapping.ToTable("StaffingLogProviderStatuses");
            statusMapping.HasKey(m => m.ID);
            statusMapping.Property(m => m.StatusName).IsRequired().IsUnicode(true).HasMaxLength(50);
            statusMapping.Property(m => m.StatusDescription).IsOptional().IsUnicode(true).HasMaxLength(100);
            statusMapping.Property(m => m.Active).IsRequired();
        }

        public static void MapStaffingLogProviderContactLog(DbModelBuilder modelBuilder)
        {
            var contactLogMapping = modelBuilder.Entity<StaffingLogProviderContactLog>();
            contactLogMapping.ToTable("StaffingLogProviderContactLog");
            contactLogMapping.HasKey(m => m.ID);
            contactLogMapping.Property(m => m.ContactDate).IsRequired();
            contactLogMapping.Property(m => m.Notes).IsOptional().IsUnicode(true).HasMaxLength(2000);
            contactLogMapping.Property(m => m.FollowUpDate).IsOptional();
            contactLogMapping.Property(m => m.DateCreated).IsRequired();
            contactLogMapping.Property(m => m.CreatedByUserID).IsRequired();

            contactLogMapping.HasRequired(x => x.Status)
                .WithMany()
                .HasForeignKey(x => x.StatusID);

            contactLogMapping.HasRequired(x => x.StaffingLogProvider)
                .WithMany(x => x.StaffingLogProviderContactLog)
                .HasForeignKey(x => x.StaffingLogProviderID);
        }


        public static void MapStaffingLogParentContactLog(DbModelBuilder modelBuilder)
        {
            var contactLogMapping = modelBuilder.Entity<StaffingLogParentContactLog>();
            contactLogMapping.ToTable("StaffingLogParentContactLog");
            contactLogMapping.HasKey(m => m.ID);
            contactLogMapping.Property(m => m.ContactDate).IsRequired();
            contactLogMapping.Property(m => m.Notes).IsRequired().IsUnicode(true).HasMaxLength(2000);
            contactLogMapping.Property(m => m.ContactedPersonName).IsOptional().IsUnicode(true).HasMaxLength(100);
            contactLogMapping.Property(m => m.ContactMethodType).IsRequired();
            contactLogMapping.Property(m => m.ContactMethodValue).IsRequired().IsUnicode(true).HasMaxLength(100);
            contactLogMapping.Property(m => m.GuardianRelationshipID).IsRequired();
            contactLogMapping.Property(m => m.DateCreated).IsRequired();
            contactLogMapping.Property(m => m.CreatedByUserID).IsRequired();

            contactLogMapping.HasRequired(x => x.StaffingLog)
                .WithMany(x => x.StaffingLogParentContactLog)
                .HasForeignKey(x => x.StaffingLogID);
        }


        public static void MapSpecialAttentionNeeds(DbModelBuilder modelBuilder)
        {
            var mapping = modelBuilder.Entity<SpecialAttentionNeed>();
            mapping.ToTable("SpecialAttentionNeeds");
            mapping.HasKey(m => m.ID);
            mapping.Property(m => m.Code).IsRequired().IsUnicode(true).HasMaxLength(5);
            mapping.Property(m => m.Name).IsRequired().IsUnicode(true).HasMaxLength(100);
            mapping.Property(m => m.Active).IsRequired();

            mapping.HasMany(x => x.StaffingLog)
                .WithMany(x => x.SpecialAttentionNeeds)
                .Map(m =>
                {
                    m.ToTable("StaffingLogSpecialAttentionNeeds");
                    m.MapRightKey("StaffingLogID");
                    m.MapLeftKey("SpecialAttentionNeedID");
                });
        }
    }
}
