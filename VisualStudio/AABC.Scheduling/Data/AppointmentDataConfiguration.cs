using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace AABC.Scheduling.Data
{
    class AppointmentDataConfiguration : EntityTypeConfiguration<AppointmentData>
    {
        public AppointmentDataConfiguration()
        {

            HasKey(m => m.Id);

            Property(t => t.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            Property(t => t.Date)
                .IsRequired();

            Property(t => t.StartTime)
                .IsRequired();

            Property(t => t.EndTime)
                .IsRequired();

            HasMany(m => m.Cancellations)
                .WithOptional(m => m.RecurringAppointment)
                .HasForeignKey(m => m.RecurringAppointmentId)
                .WillCascadeOnDelete(false);

            ToTable("CaseProviderAppointments");
        }
    }
}
