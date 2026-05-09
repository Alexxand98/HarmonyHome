using HarmonyHome.Api.Models.Entity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HarmonyHome.Api.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Producto> Productos { get; set; }

        public DbSet<Ubicacion> Ubicaciones { get; set; }

        public DbSet<Cliente> Clientes { get; set; }

        public DbSet<StockUbicacion> StockUbicaciones { get; set; }

        public DbSet<PedidoVenta> PedidosVenta { get; set; }

        public DbSet<LineaPedidoVenta> LineasPedidoVenta { get; set; }

        public DbSet<OrdenRecogida> OrdenesRecogida { get; set; }

        public DbSet<OrdenReposicion> OrdenesReposicion { get; set; }

        public DbSet<LineaOrdenReposicion> LineasOrdenReposicion { get; set; }

        public DbSet<MovimientoStock> MovimientosStock { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<StockUbicacion>()
                .HasIndex(s => new { s.ProductoId, s.UbicacionId })
                .IsUnique();
        }
    }
}