using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MVC.Models.Entities;

namespace MVC.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<DailyLog> DailyLogs { get; set; }
    public DbSet<DailyMacros> DailyMacros { get; set; }
    public DbSet<DailyMovement> DailyMovements { get; set; }
    public DbSet<CoachAssignment> CoachAssignments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ApplicationUser>(
        entity =>
        {
            entity.Property(au => au.FirstName).HasMaxLength(50).IsRequired();
            entity.Property(au => au.LastName).HasMaxLength(50).IsRequired();
            entity.Property(au => au.UserName).HasMaxLength(50).IsRequired();
            entity.Property(au => au.Email).HasMaxLength(100).IsRequired();
        });

        modelBuilder.Entity<DailyLog>(entity =>
        {
            entity.Property(dl => dl.WeightKg).HasPrecision(5, 2);
            entity.HasIndex(dl => new { dl.Date, dl.ApplicationUserId }).IsUnique();
            entity.ToTable(t => {
                t.HasCheckConstraint("DailyLog_CK01", "[WeightKg] >= 0");
            });
        });

        modelBuilder.Entity<DailyMacros>(entity =>
        {
            entity.Property(dm => dm.Fat).HasPrecision(7, 2);
            entity.Property(dm => dm.Carbs).HasPrecision(7, 2);
            entity.Property(dm => dm.Protein).HasPrecision(7, 2);
            entity.HasIndex(dm => dm.DailyLogId).IsUnique();
            entity.ToTable(t => {
                t.HasCheckConstraint("DailyMacros_CK01", "[Fat] >= 0");
                t.HasCheckConstraint("DailyMacros_CK02", "[Carbs] >= 0");
                t.HasCheckConstraint("DailyMacros_CK03", "[Protein] >= 0");
                t.HasCheckConstraint("DailyMacros_CK04", "[Calories] >= 0");
            });
        });

        modelBuilder.Entity<DailyMovement>(entity =>
        {
            entity.Property(dm => dm.DistanceKm).HasPrecision(6, 2);
            entity.HasIndex(dm => dm.DailyLogId).IsUnique();
            entity.ToTable(t =>
            {
                t.HasCheckConstraint("DailyMovement_CK01", "[DistanceKm] >= 0");
                t.HasCheckConstraint("DailyMovement_CK02", "[StepCount] >= 0");
            });
        });

        modelBuilder.Entity<CoachAssignment>(entity =>
        {
            entity.HasOne(x => x.Member)
            .WithMany(m => m.CoachAssignmentsAsMember)
            .HasForeignKey(x => x.MemberId)
            .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.Coach)
            .WithMany(c => c.CoachAssignmentsAsCoach)
            .HasForeignKey(x => x.CoachId)
            .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(ca => new { ca.MemberId, ca.CoachId }).IsUnique();
            entity.ToTable(t =>
            {
                t.HasCheckConstraint("CoachAssignment_CK01", "[MemberId] <> [CoachId]");
            });
        });
    }
}
