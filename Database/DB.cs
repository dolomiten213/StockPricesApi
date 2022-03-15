using Microsoft.EntityFrameworkCore;

using RealTimePrices.Models;

namespace RealTimePrices.Database
{
    public class DB: DbContext
    {
        public DbSet<Paper> Papers { get; set; }

        static DB()
        {
            Npgsql.NpgsqlConnection.GlobalTypeMapper.MapEnum<Currency>();
            Npgsql.NpgsqlConnection.GlobalTypeMapper.MapEnum<InstrumentType>();
        }

        public DB()
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Paper>().HasKey(u => u.Figi);
            modelBuilder.HasPostgresEnum<Currency>();
            modelBuilder.HasPostgresEnum<InstrumentType>();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(Environment.GetEnvironmentVariable("BROKER_UNIFIER_CONNECTION_STRING") ?? throw new Exception("No connection string"));
        }
    }
}
