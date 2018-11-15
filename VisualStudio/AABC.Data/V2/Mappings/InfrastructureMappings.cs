using AABC.Domain2.Infrastructure;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;

namespace AABC.Data.V2.Mappings
{
    static class InfrastructureMappings
    {
        public static void Map(DbModelBuilder modelBuilder)
        {
            MapLanguage(modelBuilder);
        }

        private static void MapLanguage(DbModelBuilder modelBuilder)
        { 
            var config = modelBuilder.Entity<Language>();
            config.ToTable("Languages");
            config.HasKey(m => m.ID);
            config.Property(m => m.ID).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            config.Property(m => m.DateCreated).IsRequired();
            config.Property(m => m.Active).HasColumnName("LangIsActive").IsRequired();
            config.Property(m => m.Code).IsUnicode(true).IsRequired().HasMaxLength(2).HasColumnName("LangCommonCode");
            config.Property(m => m.Description).IsUnicode().HasMaxLength(255).IsRequired().HasColumnName("LangEnglishName");
        }
    }
}
