using FirmaService.Models;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.Metrics;
namespace FirmaService
{
    public class EFCoreDbContext : DbContext
    {
        //Constructor calling the Base DbContext Class Constructor
        public EFCoreDbContext(DbContextOptions<EFCoreDbContext> options) : base(options)
        {
        }

        //OnConfiguring() method is used to select and configure the data source
        protected void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

        }
        public DbSet<FirmaService.Models.Angajati_Servicii> Angajati_Servicii { get; set; } = default!;
         public DbSet<FirmaService.Models.Angajati> Angajati { get; set; } = default!;
        public DbSet<FirmaService.Models.Clienti> Clienti { get; set; } = default!;
        public DbSet<FirmaService.Models.Dispozitive> Dispozitive { get; set; } = default!;
        public DbSet<FirmaService.Models.Servicii> Servicii { get; set; } = default!;
        public DbSet<FirmaService.Models.Piese> Piese { get; set; } = default!;
        protected void OnModelCreating(ModelBuilder modelBuilder)
        {           
        }

      
    }
      
    
}
