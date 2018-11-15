using AABC.Domain2.Hours;
using System.Data.Entity.ModelConfiguration;

namespace AABC.Data.V2.Mappings
{
    class SessionReportConfigurationTypeConfiguration : EntityTypeConfiguration<SessionReportConfiguration>
    {
        public SessionReportConfigurationTypeConfiguration()
        {
            HasKey(m => new { m.ProviderTypeID, m.ServiceID });
            Property(m => m.Configuration).IsUnicode();
        }
    }
}
