using ApiElda.Models;
using Microsoft.EntityFrameworkCore;

namespace ApiElda.Contexts
{
    public class ApplicationDbContext:DbContext
    {
        //configurar el contexto (cadena de conexion(
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        //Mapeo de las tablas de la base de datos
        public DbSet<Clientes> Clientes { get; set; }
        public DbSet<Productos> Productos { get; set; }
        public DbSet<Venta> Venta { get; set; }
        public DbSet<DetallesVenta> DetallesVenta { get; set; }
        public DbSet<Categoria> Categoria { get; set; }

        //OnModelCreating es un metodo de Entity Framework para configurar manualmente relaciones. (en este caso crear un llave compuesta)
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Define la clave primaria compuesta
            modelBuilder.Entity<DetallesVenta>()
                .HasKey(d => new { d.id_venta, d.id_cliente, d.id_producto });

            // Configurar las relaciones de claves foráneas si es necesario
            modelBuilder.Entity<DetallesVenta>()
                .HasOne(d => d.Venta)
                .WithMany()
                .HasForeignKey(d => d.id_venta);

            modelBuilder.Entity<DetallesVenta>()
                .HasOne(d => d.Producto)
                .WithMany()
                .HasForeignKey(d => d.id_producto);

            modelBuilder.Entity<DetallesVenta>()
                .HasOne(d => d.Cliente)
                .WithMany()
                .HasForeignKey(d => d.id_cliente);

            base.OnModelCreating(modelBuilder);
        }
    }
}
