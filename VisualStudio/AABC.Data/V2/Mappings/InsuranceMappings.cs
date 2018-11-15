using AABC.Domain2.Insurances;
using System.Data.Entity;

namespace AABC.Data.V2.Mappings
{
    static class InsuranceMappings
    {


        public static void Map(DbModelBuilder modelBuilder) {


            var i = modelBuilder.Entity<Insurance>();
            i.ToTable("dbo.Insurances");
            i.Property(x => x.Name).HasColumnName("InsuranceName");


            var isvc = modelBuilder.Entity<InsuranceService>();
            isvc.Property(x => x.DefectiveDate).HasColumnName("ServiceDefectiveDate");
            isvc.Property(x => x.EffectiveDate).HasColumnName("ServiceEffectiveDate");
            isvc.HasRequired(x => x.Insurance)
                .WithMany(x => x.Services)
                .HasForeignKey(x => x.InsuranceID);
            isvc.HasRequired(x => x.Service)
                .WithMany(x => x.Insurances)
                .HasForeignKey(x => x.ServiceID);
            isvc.HasRequired(x => x.ProviderType)
                .WithMany()
                .HasForeignKey(x => x.ProviderTypeID);


            var lc = modelBuilder.Entity<LocalCarrier>();
            lc.ToTable("InsuranceLocalCarriers");
            lc.Property(x => x.Name).HasColumnName("CarrierName");
            lc.HasRequired(x => x.Insurance)
                .WithMany(x => x.LocalCarriers)
                .HasForeignKey(x => x.InsuranceID);


        }

    }
}
