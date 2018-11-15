using AABC.Domain2.Notes;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace AABC.Data.V2.Mappings
{
    abstract class BaseNoteTypeConfiguration<TNote> : EntityTypeConfiguration<TNote> where TNote : BaseNote
    {
        protected BaseNoteTypeConfiguration()
        {
            Property(m => m.ID).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            HasOptional(x => x.FollowupUser)
                .WithMany()
                .HasForeignKey(x => x.FollowupUserID);
        }
    }

    class CaseNoteTypeConfiguration : BaseNoteTypeConfiguration<CaseNote>
    {
        public CaseNoteTypeConfiguration() : base()
        {
            ToTable("CaseNotes");
            HasKey(m => m.ID);
            HasRequired(x => x.Case)
                .WithMany(x => x.Notes)
                .HasForeignKey(x => x.CaseID);
        }
    }


    class ReferralNoteTypeConfiguration : BaseNoteTypeConfiguration<ReferralNote>
    {
        public ReferralNoteTypeConfiguration() : base()
        {
            ToTable("ReferralNotes");
            HasKey(m => m.ID);
            HasRequired(x => x.Referral)
                .WithMany(x => x.Notes)
                .HasForeignKey(x => x.ReferralID);
        }
    }


    class ProviderNoteTypeConfiguration : BaseNoteTypeConfiguration<ProviderNote>
    {
        public ProviderNoteTypeConfiguration() : base()
        {
            ToTable("ProviderNotes");
            HasKey(m => m.ID);
            HasRequired(x => x.Provider)
                .WithMany(x => x.ProviderNotes)
                .HasForeignKey(x => x.ProviderID);
        }
    }
}

