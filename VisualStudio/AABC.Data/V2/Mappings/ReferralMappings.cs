using AABC.Domain2.Referrals;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;

namespace AABC.Data.V2.Mappings
{
    class ReferralMappings
    {
        public static void Map(DbModelBuilder modelBuilder)
        {
            MapReferral(modelBuilder);
            MapReferralEnum(modelBuilder);
            MapReferralDismissalReason(modelBuilder);
            MapReferralSourceType(modelBuilder);
            MapReferralChecklistItem(modelBuilder);
            MapReferralChecklist(modelBuilder);
        }


        private static void MapReferral(DbModelBuilder modelBuilder)
        {
            var config = modelBuilder.Entity<Referral>();
            config.HasKey(m => m.ID);
            config.Property(m => m.ID).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            config.Property(m => m.DateCreated).IsRequired();
            config.Property(m => m.FirstName).IsUnicode(true).HasMaxLength(64).IsRequired().HasColumnName("ReferralFirstName");
            config.Property(m => m.LastName).IsUnicode(true).HasMaxLength(64).IsRequired().HasColumnName("ReferralLastName");
            config.Property(m => m.DateOfBirth).IsOptional().HasColumnName("ReferralDateOfBirth").HasColumnType("date");
            config.Property(m => m.Gender).IsOptional().IsUnicode(true).HasMaxLength(5).HasColumnName("ReferralGender");
            config.Property(m => m.PrimaryLanguage).IsOptional().IsUnicode(true).HasMaxLength(64).HasColumnName("ReferralPrimarySpokenLangauge");
            config.Property(m => m.Email).IsOptional().IsUnicode(true).HasMaxLength(128).HasColumnName("ReferralEmail");
            config.Property(m => m.Phone).IsOptional().IsUnicode(true).HasMaxLength(40).HasColumnName("ReferralPhone");
            config.Property(m => m.Address1).IsOptional().IsUnicode(true).HasMaxLength(255).HasColumnName("ReferralAddress1");
            config.Property(m => m.Address2).IsOptional().IsUnicode(true).HasMaxLength(255).HasColumnName("ReferralAddress2");
            config.Property(m => m.City).IsOptional().IsUnicode(true).HasMaxLength(255).HasColumnName("ReferralCity");
            config.Property(m => m.State).IsOptional().IsUnicode(true).HasMaxLength(50).HasColumnName("ReferralState");
            config.Property(m => m.ZipCode).IsOptional().IsUnicode(true).HasMaxLength(20).HasColumnName("ReferralZip");
            config.Property(m => m.AreaOfConcern).IsOptional().IsUnicode(true).HasMaxLength(1000).HasColumnName("ReferralAreaOfConcern");
            config.Property(m => m.InsuranceCompanyName).IsOptional().IsUnicode(true).HasMaxLength(255).HasColumnName("ReferralInsuranceCompanyName");
            config.Property(m => m.InsuranceMemberID).IsOptional().IsUnicode(true).HasMaxLength(64).HasColumnName("ReferralInsuranceMemberID");
            config.Property(m => m.InsurancePrimaryCardholderDOB).IsOptional().HasColumnType("date").HasColumnName("ReferralInsurancePrimaryCardholderDateOfBirth");
            config.Property(m => m.InsuranceProviderPhone).IsOptional().IsUnicode(true).HasMaxLength(30).HasColumnName("ReferralInsuranceCompanyProviderPhone");
            config.Property(m => m.SourceTypeID).IsOptional().HasColumnName("ReferralSourceType");
            config.Property(m => m.SourceName).IsOptional().IsUnicode(true).HasMaxLength(255).HasColumnName("ReferralSourceName");
            config.Property(m => m.ReferrerNotes).IsOptional().IsUnicode(true).HasMaxLength(2000).HasColumnName("ReferralReferrerNotes");
            config.Property(m => m.Status).IsRequired().HasColumnName("ReferralStatus");
            config.Property(m => m.DismissalReasonID).IsOptional().HasColumnName("ReferralDismissalReasonID");
            config.Property(m => m.DismissalNote).IsOptional().IsUnicode(true).HasMaxLength(128).HasColumnName("ReferralDismissalReason");
            config.Property(m => m.DismissalNoteExtended).IsOptional().IsUnicode(true).HasMaxLength(2000).HasColumnName("ReferralDismissalReasonNotes");
            config.Property(m => m.BenefitCheck).IsOptional().IsUnicode().HasMaxLength(2000).HasColumnName("ReferralBenefitCheck");
            config.Property(m => m.EnteredByStaffID).IsOptional().HasColumnName("ReferralEnteredByStaffID");
            config.Property(m => m.Followup).IsRequired().HasColumnName("ReferralFollowup");
            config.Property(m => m.FollowupDate).IsOptional().HasColumnType("date").HasColumnName("ReferralFollowupDate");
            config.Property(m => m.AssignedStaffID).IsOptional().HasColumnName("ReferralAssignedStaffID");
            config.Property(m => m.GeneratedCaseID).IsOptional().HasColumnName("ReferralGeneratedCaseID");
            config.Property(m => m.GeneratedPatientID).IsOptional().HasColumnName("ReferralGeneratedPatientID");
            config.Property(m => m.ServicesRequested).IsOptional().IsUnicode(true).HasMaxLength(75).HasColumnName("ReferralServicesRequested");
            config.Property(m => m.InsurancePrimaryCardholderName).IsOptional().IsUnicode(true).HasMaxLength(128).HasColumnName("ReferralPrimaryCardholderName");
            config.Property(m => m.StatusNotes).IsOptional().IsUnicode(true).HasMaxLength(2000).HasColumnName("ReferralStatusNotes");
            config.Property(m => m.Active).IsRequired().HasColumnName("ReferralActive");
            config.Property(m => m.InsuranceStatus).IsOptional().HasColumnName("ReferralInsuranceStatus");
            config.Property(m => m.IntakeStatus).IsOptional().HasColumnName("ReferralIntakeStatus");
            config.Property(m => m.RxStatus).IsOptional().HasColumnName("ReferralRxStatus");
            config.Property(m => m.InsuranceCardStatus).IsOptional().HasColumnName("ReferralInsuranceCardStatus");
            config.Property(m => m.EvaluationStatus).IsOptional().HasColumnName("ReferralEvaluationStatus");
            config.Property(m => m.PolicyBookStatus).IsOptional().HasColumnName("ReferralPolicyBookStatus");
            config.Property(m => m.FundingType).IsOptional().HasMaxLength(20).HasColumnName("ReferralFundingType");
            config.Property(m => m.BenefitType).IsOptional().HasMaxLength(20).HasColumnName("ReferralBenefitType");
            config.Property(m => m.CoPayAmount).IsOptional().HasColumnType("money").HasColumnName("ReferralCoPayAmount");
            config.Property(m => m.CoInsuranceAmount).IsOptional().HasColumnType("money").HasColumnName("ReferralCoInsuranceAmount");
            config.Property(m => m.DeductibleTotal).IsOptional().HasColumnType("money").HasColumnName("ReferralDeductibleTotal");

            config.Property(m => m.GuardianFirstName).IsOptional().IsUnicode(true).HasMaxLength(64).HasColumnName("ReferralGuardianFirstName");
            config.Property(m => m.GuardianLastName).IsOptional().IsUnicode(true).HasMaxLength(64).HasColumnName("ReferralGuardianLastName");
            config.Property(m => m.GuardianRelationship).IsOptional().IsUnicode(true).HasMaxLength(64).HasColumnName("ReferralGuardianRelationship");
            config.Property(x => x.GuardianRelationshipID).HasColumnName("ReferralGuardianRelationshipID");
            config.Property(x => x.GuardianEmail).HasColumnName("ReferralGuardianEmail");
            config.Property(x => x.GuardianCellPhone).HasColumnName("ReferralGuardianCellPhone");
            config.Property(x => x.GuardianWorkPhone).HasColumnName("ReferralGuardianWorkPhone");
            config.Property(x => x.GuardianHomePhone).HasColumnName("ReferralGuardianHomePhone");
            config.Property(x => x.GuardianNotes).HasColumnName("ReferralGuardianNotes");

            config.Property(x => x.Guardian2FirstName).HasColumnName("ReferralGuardian2FirstName");
            config.Property(x => x.Guardian2LastName).HasColumnName("ReferralGuardian2LastName");
            config.Property(x => x.Guardian2RelationshipID).HasColumnName("ReferralGuardian2RelationshipID");
            config.Property(x => x.Guardian2Email).HasColumnName("ReferralGuardian2Email");
            config.Property(x => x.Guardian2CellPhone).HasColumnName("ReferralGuardian2CellPhone");
            config.Property(x => x.Guardian2HomePhone).HasColumnName("ReferralGuardian2HomePhone");
            config.Property(x => x.Guardian2WorkPhone).HasColumnName("ReferralGuardian2WorkPhone");
            config.Property(x => x.Guardian2Notes).HasColumnName("ReferralGuardian2Notes");

            config.Property(x => x.Guardian3FirstName).HasColumnName("ReferralGuardian3FirstName");
            config.Property(x => x.Guardian3LastName).HasColumnName("ReferralGuardian3LastName");
            config.Property(x => x.Guardian3RelationshipID).HasColumnName("ReferralGuardian3RelationshipID");
            config.Property(x => x.Guardian3Email).HasColumnName("ReferralGuardian3Email");
            config.Property(x => x.Guardian3CellPhone).HasColumnName("ReferralGuardian3CellPhone");
            config.Property(x => x.Guardian3HomePhone).HasColumnName("ReferralGuardian3HomePhone");
            config.Property(x => x.Guardian3WorkPhone).HasColumnName("ReferralGuardian3WorkPhone");
            config.Property(x => x.Guardian3Notes).HasColumnName("ReferralGuardian3Notes");

            config.Property(x => x.PhysicianName).HasColumnName("ReferralPhysicianName");
            config.Property(x => x.PhysicianAddress).HasColumnName("ReferralPhysicianAddress");
            config.Property(x => x.PhysicianPhone).HasColumnName("ReferralPhysicianPhone");
            config.Property(x => x.PhysicianFax).HasColumnName("ReferralPhysicianFax");
            config.Property(x => x.PhysicianEmail).HasColumnName("ReferralPhysicianEmail");
            config.Property(x => x.PhysicianContact).HasColumnName("ReferralPhysicianContact");
            config.Property(x => x.PhysicianNotes).HasColumnName("ReferralPhysicianNotes");

            config.HasOptional(m => m.SourceType)
                .WithMany()
                .HasForeignKey(m => m.SourceTypeID);

            config.HasOptional(m => m.DismissalReason)
                .WithMany()
                .HasForeignKey(m => m.DismissalReasonID);

            config.HasOptional(m => m.EnteredByStaff)
                .WithMany()
                .HasForeignKey(m => m.EnteredByStaffID);

            config.HasOptional(m => m.AssignedStaff)
                .WithMany()
                .HasForeignKey(m => m.AssignedStaffID);

            config.HasOptional(m => m.InsuranceCompany)
                .WithMany()
                .HasForeignKey(m => m.InsuranceCompanyID);
        }


        private static void MapReferralEnum(DbModelBuilder modelBuilder)
        {
            var config = modelBuilder.Entity<ReferralEnumItem>();
            config.HasKey(m => m.ID);
            config.Property(m => m.ID).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
            config.Property(m => m.DateCreated).IsRequired();
            config.Property(m => m.StatusType).IsRequired().IsUnicode(false).HasMaxLength(20);
            config.Property(m => m.Label).IsRequired().IsUnicode(true).HasMaxLength(50);
            config.Property(m => m.ColorCode).IsOptional().IsUnicode(false).HasMaxLength(20);
        }


        private static void MapReferralDismissalReason(DbModelBuilder modelBuilder)
        {
            var config = modelBuilder.Entity<ReferralDismissalReason>();
            config.ToTable("ReferralDismissalReasons");
            config.HasKey(m => m.ID);
            config.Property(m => m.ID).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            config.Property(m => m.DateCreated).IsRequired();
            config.Property(m => m.Code).IsRequired().IsUnicode(true).HasMaxLength(10).HasColumnName("ReasonCode");
            config.Property(m => m.Name).IsRequired().IsUnicode(true).HasMaxLength(255).HasColumnName("ReasonName");
        }

        private static void MapReferralSourceType(DbModelBuilder modelBuilder)
        {
            var config = modelBuilder.Entity<ReferralSourceType>();
            config.ToTable("ReferralSourceTypes");
            config.HasKey(m => m.ID);
            config.Property(m => m.ID).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            config.Property(m => m.DateCreated).IsRequired();
            config.Property(m => m.Name).IsRequired().IsUnicode(true).HasMaxLength(64).HasColumnName("TypeName");
        }

        private static void MapReferralChecklistItem(DbModelBuilder modelBuilder)
        {
            var config = modelBuilder.Entity<ReferralChecklistItem>();
            config.ToTable("ReferralChecklistItems");
            config.HasKey(m => m.ID);
            config.Property(m => m.ID).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            config.Property(m => m.DateCreated).IsRequired();
            config.Property(m => m.Description).IsRequired().IsUnicode(true).HasMaxLength(255).HasColumnName("ItemDescription");
        }

        private static void MapReferralChecklist(DbModelBuilder modelBuilder)
        {
            var config = modelBuilder.Entity<ReferralChecklist>();
            config.ToTable("ReferralChecklist");
            config.HasKey(m => m.ID);
            config.Property(m => m.ID).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            config.Property(m => m.DateCreated).IsRequired();
            config.Property(m => m.IsComplete).IsRequired().HasColumnName("ItemIsComplete");
            config.Property(m => m.CompletedByStaffID).IsOptional().HasColumnName("ItemCompletedByStaffID");
            config.HasRequired(m => m.Referral)
                .WithMany(m => m.Checklist)
                .HasForeignKey(m => m.ReferralID);
            config.HasRequired(m => m.ChecklistItem)
                .WithMany()
                .HasForeignKey(m => m.ChecklistItemID);
            config.HasOptional(m => m.CompletedByStaff)
                .WithMany()
                .HasForeignKey(m => m.CompletedByStaffID);
        }

    }
}
