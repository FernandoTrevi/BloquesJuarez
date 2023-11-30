using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BloquesJuarez.Models
{
    public class Remito
    {
        [Key]
        public int Id { get; set; }

        public int NroRemito { get; set; } // Se autoincrementará en la base de datos

        public DateTime Fecha { get; set; }

        // Relación con el cliente
        [Required(ErrorMessage = "El Cliente es Obligatorio.")]
        public int ClienteId { get; set; }

        [ForeignKey("ClienteId")]
        public virtual Cliente Cliente { get; set; }

        // Lugar de entrega
        public string LugarEntrega { get; set; }

        public string Observaciones { get; set; }

        [Required]
        public EstadoRemito Estado { get; set; }

        public virtual List<RemitoDetalle> Detalles { get; set; }
    }
    public enum EstadoRemito
    {
        Pendiente,
        Pagado
    }


}
