using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;

namespace WebApplication1.Context;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Doctor> Doctors { get; set; }
    public DbSet<Prescription> Prescriptions { get; set; }
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Prescription>()
            .HasOne(p => p.Doctor)
            .WithMany(d => d.Prescriptions)
            .HasForeignKey(p => p.DoctorId);
    }

    public void Seed()
    {
        if (!Doctors.Any())
        {
            Doctors.AddRange(new List<Doctor>
            {
                new Doctor { Name = "John Doe", Specialty = "Cardiology" },
                new Doctor { Name = "Jane Smith", Specialty = "Neurology" }
            });

            SaveChanges();
        }
    }
}
