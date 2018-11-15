using AABC.Domain2.Authorizations;
using AABC.Domain2.Hours;
using System.Data.Entity;

namespace AABC.Data.V2.Mappings
{
    static class AuthorizationMappings
    {

        public static void Map(DbModelBuilder modelBuilder) {

            var auth = modelBuilder.Entity<Authorization>();
            auth.ToTable("CaseAuthCodes");
            auth.Property(x => x.AuthorizationCodeID).HasColumnName("AuthCodeID");
            auth.Property(x => x.AuthorizationClassID).HasColumnName("AuthClassID");
            auth.Property(x => x.StartDate).HasColumnName("AuthStartDate");
            auth.Property(x => x.EndDate).HasColumnName("AuthEndDate");
            auth.Property(x => x.HoursApproved).HasColumnName("AuthTotalHoursApproved");
            auth.HasRequired(x => x.Case)
                .WithMany(x => x.Authorizations)
                .HasForeignKey(x => x.CaseID);
            auth.HasRequired(x => x.AuthorizationClass)
                .WithMany(x => x.Authorizations)
                .HasForeignKey(x => x.AuthorizationClassID);
            auth.HasRequired(x => x.AuthorizationCode)
                .WithMany(x => x.Authorizations)
                .HasForeignKey(x => x.AuthorizationCodeID);



            var breakdown = modelBuilder.Entity<AuthorizationBreakdown>();
            breakdown.ToTable("CaseAuthHoursBreakdown");
            breakdown.Property(x => x.AuthorizationID).HasColumnName("CaseAuthID");
            breakdown
                .HasRequired(x => x.HoursEntry)
                .WithMany(x => x.AuthorizationBreakdowns)
                .HasForeignKey(x => x.HoursID);
            breakdown
                .HasRequired(x => x.Authorization)
                .WithMany(x => x.AuthorizationBreakdowns)
                .HasForeignKey(x => x.AuthorizationID);



            var authClass = modelBuilder.Entity<AuthorizationClass>();
            authClass.ToTable("CaseAuthClasses");
            authClass.Property(x => x.Code).HasColumnName("AuthClassCode");
            authClass.Property(x => x.Name).HasColumnName("AuthClassName");
            authClass.Property(x => x.Description).HasColumnName("AuthClassDescription");



            var authCode = modelBuilder.Entity<AuthorizationCode>();
            authCode.ToTable("AuthCodes");
            authCode.Property(x => x.Code).HasColumnName("CodeCode");
            authCode.Property(x => x.Description).HasColumnName("CodeDescription");



            var matchRule = modelBuilder.Entity<AuthorizationMatchRule>();
            matchRule.ToTable("AuthMatchRules");
            matchRule.Property(x => x.AllowOverlapping).HasColumnName("RuleAllowOverlapping");
            matchRule.Property(x => x.BillingMethod).HasColumnName("RuleBillingMethod");
            matchRule.Property(x => x.FinalAuthorizationID).HasColumnName("RuleFinalAuthID");
            matchRule.Property(x => x.FinalMinimumMinutes).HasColumnName("RuleFinalMinimumMinutes");
            matchRule.Property(x => x.FinalUnitSize).HasColumnName("RuleFinalUnitSize");
            matchRule.Property(x => x.InitialAuthorizationID).HasColumnName("RuleInitialAuthID");
            matchRule.Property(x => x.InitialMinimumMinutes).HasColumnName("RuleInitialMinimumMinutes");
            matchRule.Property(x => x.InitialUnitSize).HasColumnName("RuleInitialUnitSize");
            matchRule.Property(x => x.RequiresAuthorizedBCBA).HasColumnName("RuleRequiresAuthorizedBCBA");
            matchRule.Property(x => x.RequiresPreAuthorization).HasColumnName("RuleRequiresPreAuthorization");
            matchRule
                .HasRequired(x => x.Insurance)
                .WithMany(x => x.AuthorizationMatchRules)
                .HasForeignKey(x => x.InsuranceID);
            matchRule
                .HasRequired(x => x.ProviderType)
                .WithMany(x => x.AuthorizationMatchRules)
                .HasForeignKey(x => x.ProviderTypeID);
            matchRule
                .HasRequired(x => x.Service)
                .WithMany(x => x.AuthorizationMatchRules)
                .HasForeignKey(x => x.ServiceID);
            

        }

    }
}
