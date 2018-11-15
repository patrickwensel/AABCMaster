using System.Data.Entity;

namespace AABC.Scheduling.Data
{
    public class SchedulingDataContext : DbContext
    {
        public SchedulingDataContext() : base("Name=SchedulingDataContext") { }

        public SchedulingDataContext(string connectionString) : base(connectionString) { }


        public IDbSet<AppointmentData> Appointments { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new AppointmentDataConfiguration());
        }
    }
}
   