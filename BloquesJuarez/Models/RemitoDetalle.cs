using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BloquesJuarez.Models
{
    public class RemitoDetalle
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Nombre { get; set; }

        [Required]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]

        public decimal Cantidad { get; set; }

        [Required(ErrorMessage = "El Producto es Obligatorio.")]
        public int ProductoId { get; set; }

        [ForeignKey("ProductoId")]
        public Producto Producto { get; set; }

        [Required(ErrorMessage = "El Remito es Obligatorio.")]
        public int RemitoId { get; set; }

        [ForeignKey("RemitoId")]
        public Remito Remito { get; set; }
    }

}
