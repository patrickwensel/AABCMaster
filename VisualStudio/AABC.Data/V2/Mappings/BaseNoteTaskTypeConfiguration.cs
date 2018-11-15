using AABC.Domain2.Notes;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace AABC.Data.V2.Mappings
{
    abstract class BaseNoteTaskTypeConfiguration<TNoteTask> : EntityTypeConfiguration<TNoteTask> where TNoteTask : BaseNoteTask
    {
        protected BaseNoteTaskTypeConfiguration()
        {
            HasKey(m => new { m.ID, m.NoteID });
            Property(m => m.ID).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            HasOptional(x => x.CompletedByUser)
                .WithMany()
                .HasForeignKey(x => x.CompletedByUserID);
        }
    }

    class CaseNoteTaskTypeConfiguration : BaseNoteTaskTypeConfiguration<CaseNoteTask>
    {
        public CaseNoteTaskTypeConfiguration()
        {
            ToTable("CaseNoteTasks");
            Property(m => m.AssignedToStaffID);
            Property(m => m.CompletedByUserID);

            HasOptional(x => x.AssignedToStaff)
                .WithMany(x => x.CaseNoteTasks)
                .HasForeignKey(x => x.AssignedToStaffID);

            HasRequired(x => x.Note)
               .WithMany(x => x.NoteTasks)
               .HasForeignKey(x => x.NoteID);
        }
    }


    class ReferralNoteTaskTypeConfiguration : BaseNoteTaskTypeConfiguration<ReferralNoteTask>
    {
        public ReferralNoteTaskTypeConfiguration()
        {
            ToTable("ReferralNoteTasks");

            HasOptional(x => x.AssignedToStaff)
                .WithMany(x=> x.ReferralNoteTasks)
                .HasForeignKey(x => x.AssignedToStaffID);

            HasRequired(x => x.Note)
                .WithMany(x => x.NoteTasks)
                .HasForeignKey(x => x.NoteID);
        }
    }


    class ProviderNoteTaskTypeConfiguration : BaseNoteTaskTypeConfiguration<ProviderNoteTask>
    {
        public ProviderNoteTaskTypeConfiguration()
        {
            ToTable("ProviderNoteTasks");

            HasOptional(x => x.AssignedToStaff)
                .WithMany(x => x.ProviderNoteTasks)
                .HasForeignKey(x => x.AssignedToStaffID);

            HasRequired(x => x.Note)
                .WithMany(x => x.NoteTasks)
                .HasForeignKey(x => x.NoteID);
        }
    }
}
