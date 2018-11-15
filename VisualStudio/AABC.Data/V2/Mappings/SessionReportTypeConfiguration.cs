using AABC.Domain2.Hours;
using System.Data.Entity.ModelConfiguration;

namespace AABC.Data.V2.Mappings
{
    class SessionReportTypeConfiguration : EntityTypeConfiguration<SessionReport>
    {
        public SessionReportTypeConfiguration()
        {
            Property(m => m.Report).IsUnicode();
            //HasRequired(m => m.Session)
            //    .WithOptional(m => m.Report)
            //    .WillCascadeOnDelete(true);
        }
    }
}
