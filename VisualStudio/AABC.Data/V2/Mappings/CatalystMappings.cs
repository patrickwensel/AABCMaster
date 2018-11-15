using AABC.Domain2.Integrations.Catalyst;
using System.Data.Entity;

namespace AABC.Data.V2.Mappings
{
    static class CatalystMappings
    {

        public static void Map(DbModelBuilder modelBuilder) {

            const string CATALYST_SCHEMA_NAME = "cata";

            modelBuilder.Entity<ProviderMapping>().ToTable("ProviderMappings", CATALYST_SCHEMA_NAME);
            modelBuilder.Entity<CaseMapping>().ToTable("CaseMappings", CATALYST_SCHEMA_NAME);

            var tsPreload = modelBuilder.Entity<TimesheetPreloadEntry>();
            tsPreload.ToTable("TimesheetPreload", CATALYST_SCHEMA_NAME);
            tsPreload.Property(x => x.ParentAgreed).HasColumnName("PatientAgreed");
            tsPreload.Ignore(x => x.CaseID);
            tsPreload.Ignore(x => x.ProviderID);
            
            var hasData = modelBuilder.Entity<HasDataEntry>();
            hasData.ToTable("HasDataImportTemp", CATALYST_SCHEMA_NAME);
            hasData.Property(x => x.Name).HasColumnName("ImportName");
            hasData.Property(x => x.Date).HasColumnName("ImportDate");
            hasData.Property(x => x.ProviderInitialsSet).HasColumnName("ImportInitials");


        }
    }
}
