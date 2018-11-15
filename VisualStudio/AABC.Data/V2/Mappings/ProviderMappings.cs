using AABC.Domain2.Providers;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;

namespace AABC.Data.V2.Mappings
{
    internal class ProviderMappings
    {
        internal static void Map(DbModelBuilder modelBuilder)
        {
            MapProvider(modelBuilder);
            MapProviderPortalUsers(modelBuilder);
            MapProviderTypes(modelBuilder);
            MapProviderServiceZipCode(modelBuilder);
        }

        private static void MapProviderPortalUsers(DbModelBuilder modelBuilder)
        {
            var mapping = modelBuilder.Entity<PortalUser>();
            mapping.ToTable("ProviderPortalUsers");
            mapping.Property(x => x.UserNumber).HasColumnName("ProviderUserNumber");
            mapping.Property(x => x.HasAppAccess).HasColumnName("ProviderHasAppAccess");
        }

        private static void MapProvider(DbModelBuilder modelBuilder)
        {
            var mapping = modelBuilder.Entity<Provider>();
            mapping.Property(x => x.LastName).HasColumnName("ProviderLastName");
            mapping.Property(x => x.FirstName).HasColumnName("ProviderFirstName");
            mapping.Property(x => x.CompanyName).HasColumnName("ProviderCompanyName");
            mapping.Property(x => x.Phone).HasColumnName("ProviderPrimaryPhone");
            mapping.Property(x => x.Email).HasColumnName("ProviderPrimaryEmail");
            mapping.Property(x => x.Address1).HasColumnName("ProviderAddress1");
            mapping.Property(x => x.Address2).HasColumnName("ProviderAddress2");
            mapping.Property(x => x.City).HasColumnName("ProviderCity");
            mapping.Property(x => x.State).HasColumnName("ProviderState");
            mapping.Property(x => x.Zip).HasColumnName("ProviderZip");
            mapping.Property(x => x.NPI).HasColumnName("ProviderNPI");
            mapping.Property(x => x.HourlyRate).HasColumnName("ProviderRate");
            mapping.Property(x => x.Phone2).HasColumnName("ProviderPhone2");
            mapping.Property(x => x.Fax).HasColumnName("ProviderFax");
            mapping.Property(x => x.Notes).HasColumnName("ProviderNotes");
            mapping.Property(x => x.Availability).HasColumnName("ProviderAvailability");
            mapping.Property(x => x.HasBackgroundCheck).HasColumnName("ProviderHasBackgroundCheck");
            mapping.Property(x => x.HasReferences).HasColumnName("ProviderHasReferences");
            mapping.Property(x => x.HasResume).HasColumnName("ProviderHasResume");
            mapping.Property(x => x.CanCall).HasColumnName("ProviderCanCall");
            mapping.Property(x => x.CanReachByPhone).HasColumnName("ProviderCanReachByPhone");
            mapping.Property(x => x.CanEmail).HasColumnName("ProviderCanEmail");
            mapping.Property(x => x.DocumentStatus).HasColumnName("ProviderDocumentStatus");
            mapping.Property(x => x.LBA).HasColumnName("ProviderLBA");
            mapping.Property(x => x.CertificationID).HasColumnName("ProviderCertificationID");
            mapping.Property(x => x.CertificationState).HasColumnName("ProviderCertificationState");
            mapping.Property(x => x.CertificationRenewalDate).HasColumnName("ProviderCertificationRenewalDate");
            mapping.Property(x => x.W9Date).HasColumnName("ProviderW9Date");
            mapping.Property(x => x.CAQH).HasColumnName("ProviderCAQH");
            mapping.Property(x => x.Status).HasColumnName("ProviderStatus").IsRequired();
            mapping.Property(x => x.IsHired).HasColumnName("ProviderIsHired");
            mapping.Property(x => x.ProviderTypeID).HasColumnName("ProviderType");
            mapping.Property(x => x.ProviderSubTypeID).HasColumnName("ProviderSubTypeID");
            mapping.Property(x => x.ResumeFileName).HasColumnName("ResumeFileName").IsOptional().HasMaxLength(100);
            mapping.Property(x => x.ResumeLocation).HasColumnName("ResumeLocation").IsOptional().HasMaxLength(50);
            mapping.Property(x => x.ProviderGender).HasColumnName("ProviderGender").IsOptional().HasMaxLength(1).IsFixedLength();
            mapping.Property(x => x.HireDate).HasColumnName("ProviderHireDate").IsOptional();
            mapping.HasOptional(m => m.ProviderSubType)
                .WithMany(m => m.Providers)
                .HasForeignKey(m => m.ProviderSubTypeID)
                .WillCascadeOnDelete(false);
        }

        private static void MapProviderTypes(DbModelBuilder modelBuilder)
        {
            var providerTypeMapping = modelBuilder.Entity<ProviderType>();
            providerTypeMapping.Property(x => x.Code).HasColumnName("ProviderTypeCode");
            providerTypeMapping.Property(x => x.Name).HasColumnName("ProviderTypeName");
            providerTypeMapping.Property(x => x.IsOutsourced).HasColumnName("ProviderTypeIsOutsourced");
            providerTypeMapping.Property(x => x.CanSupervise).HasColumnName("ProviderTypeCanSuperviseCase");
            providerTypeMapping.Property(x => x.DateCreated).HasColumnName("DateCreated").IsRequired();

            var providerSubTypeMapping = modelBuilder.Entity<ProviderSubType>();
            providerSubTypeMapping.HasKey(m => m.ID);
            providerSubTypeMapping.Property(x => x.ID).IsRequired().HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            providerSubTypeMapping.Property(x => x.ParentTypeID).IsRequired().HasColumnName("ProviderParentTypeID");
            providerSubTypeMapping.Property(x => x.Code).IsRequired().IsUnicode().HasMaxLength(32).HasColumnName("ProviderSubTypeCode");
            providerSubTypeMapping.Property(x => x.Name).IsOptional().IsUnicode().HasMaxLength(255).HasColumnName("ProviderSubTypeName");
            providerSubTypeMapping.Property(x => x.DateCreated).HasColumnName("DateCreated").IsRequired();
            providerSubTypeMapping.HasRequired(m => m.ParentType)
                .WithMany()
                .HasForeignKey(m => m.ParentTypeID)
                .WillCascadeOnDelete(false);

        }

        private static void MapProviderServiceZipCode(DbModelBuilder modelBuilder)
        {
            var mapping = modelBuilder.Entity<ProviderServiceZipCode>();
            mapping.HasKey(m => new { m.ID, m.ProviderID });
            mapping.Property(m => m.ID).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            mapping.Property(m => m.DateCreated).IsRequired();
            mapping.Property(m => m.rv).IsRequired().HasMaxLength(8);
            mapping.Property(m => m.IsPrimary).IsRequired();
            mapping.HasRequired(m => m.Provider)
                .WithMany(p => p.ServiceZipCodes)
                .HasForeignKey(m => m.ProviderID);
        }

        private static void MapProviderInsuranceCredential(DbModelBuilder modelBuilder)
        {
            var mapping = modelBuilder.Entity<ProviderInsuranceCredential>();
            mapping.HasKey(m => new { m.ID, m.ProviderID });
            mapping.Property(m => m.ID).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            mapping.Property(m => m.StartDate).IsOptional().HasColumnType("datetime");
            mapping.Property(m => m.EndDate).IsOptional().HasColumnType("datetime");
            mapping.HasRequired(m => m.Provider)
                .WithMany(p => p.InsuranceCredentials)
                .HasForeignKey(m => m.ProviderID);
            mapping.HasRequired(m => m.Insurance)
                .WithMany()
                .HasForeignKey(m => m.InsuranceID);
        }
    }
}
