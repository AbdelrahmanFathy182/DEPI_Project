using System.Xml;
using Microsoft.EntityFrameworkCore;
using SmartHomeDashboard.Models;

namespace SmartHomeDashboard.Data
{
    public class AppDbContext: DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { } 
        public DbSet<Account> Accounts { get; set; }

        public DbSet<Room> rooms { get; set; }

        public DbSet<Device> devices { get; set; }

        public DbSet<ActionLog> actions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Device>()
                .Property(d => d.deviceType)
                .HasConversion<string>(); // store enum as string

            modelBuilder.Entity<ActionLog>()
                .Property(d => d.Type)
                .HasConversion<string>(); // store enum as string

            modelBuilder.Entity<ActionLog>()
               .Property(d => d.TrgtAttribute)
               .HasConversion<string>();

            modelBuilder.Entity<Room>()
               .Property(d => d.Type)
               .HasConversion<string>();

            modelBuilder.Entity<Account>()
           .Property(e => e.Id)
           .ValueGeneratedOnAdd(); // Ensures it's treated as auto-increment
            modelBuilder.Entity<Room>()
           .Property(e => e.Id)
           .ValueGeneratedOnAdd(); // Ensures it's treated as auto-increment
            modelBuilder.Entity<Device>()
           .Property(e => e.Id)
           .ValueGeneratedOnAdd(); // Ensures it's treated as auto-increment
            modelBuilder.Entity<ActionLog>()
           .Property(e => e.Id)
           .ValueGeneratedOnAdd(); // Ensures it's treated as auto-increment

            base.OnModelCreating(modelBuilder);
        }
    }
}
