using Microsoft.EntityFrameworkCore;
using BloquesJuarez.Models;


namespace BloquesJuarez.Datos
{
    public class AplicationDBContext:DbContext
    {
        public AplicationDBContext(DbContextOptions<AplicationDBContext>options):base(options)
        {
            
        }
        public DbSet<Categoria> Categoria { get; set; }
        public DbSet<Cliente> Cliente { get; set; }
        public DbSet<CondIva> CondIva{ get; set; }
        public DbSet<Provincia> Provincia { get; set; }
        public DbSet<Producto> Producto { get; set; }
        public DbSet<Remito> Remito { get; set; }
        public DbSet<RemitoDetalle> RemitoDetalle { get; set; }
    }
}
