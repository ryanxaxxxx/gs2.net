using Microsoft.EntityFrameworkCore;
using GreenWay.Models;

namespace GreenWay.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Colaborador> Colaboradores { get; set; }
        public DbSet<Carona> Caronas { get; set; }
        public DbSet<ImpactoAmbiental> ImpactosAmbientais { get; set; }
        public DbSet<RotaSustentavel> RotasSustentaveis { get; set; }
    }
}
