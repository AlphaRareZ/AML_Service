using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Analysis> Analyses { get; set; }
    public virtual DbSet<AnalysisFile> AnalysisFiles { get; set; }
    public virtual DbSet<Protein> Proteins { get; set; }
    public virtual DbSet<Ligand> Ligands { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Analysis>(e =>
        {
            e.HasKey(a => a.Id);
            e.Property(a => a.Status).HasConversion<string>();
            e.Property(a => a.Success).HasDefaultValue(false);
            e.Property(a => a.UserID).IsRequired();
            
            e.HasMany(a => a.Proteins)
                .WithOne(p => p.Analysis)
                .HasForeignKey(p => p.AnalysisId)
                .OnDelete(DeleteBehavior.Cascade);
            
            e.HasMany(a => a.Files)
                .WithOne(p => p.Analysis)
                .HasForeignKey(p => p.AnalysisId)
                .OnDelete(DeleteBehavior.Cascade);
            e.Property(a=>a.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
        });

        modelBuilder.Entity<AnalysisFile>(e =>
        {
            e.HasKey(a => a.Id);
            e.HasOne(a => a.Analysis).WithMany(a => a.Files).HasForeignKey(a => a.AnalysisId);
            e.Property(a => a.Type).HasConversion<string>();
        });
        modelBuilder.Entity<Protein>(e =>
        {
            e.HasKey(a => a.Id);
            e.Property(p => p.Top10AdvancedSaveLigandsCsvUrl).IsRequired(false);
            e.Property(p => p.Top10AdvancedSaveLigandsImgUrl).IsRequired(false);
            e.Property(a=>a.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            e.HasOne(p => p.Analysis).WithMany(a => a.Proteins).HasForeignKey(a => a.AnalysisId);
        });
        modelBuilder.Entity<Ligand>(e =>
        {
            e.HasKey(a => a.Id);
            e.HasOne(a => a.Protein).WithMany(x => x.Ligands).HasForeignKey(a => a.ProteinId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}