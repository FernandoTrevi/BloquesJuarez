using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BloquesJuarez.Models
{
    public class Producto
    {
        public Producto()
        {
            TempCantidad = 1;
        }
        [Key]
        public int Id { get; set; }

        [Required]
        public string NombreProducto { get; set; }

        [ForeignKey("CategoriaId")]
        public virtual Categoria Categoria { get; set; }

        [Required(ErrorMessage = "La elección de una categoría es Obligatorio.")]
        public int CategoriaId { get; set; }

        [Required]
        public string UniDeMedida { get; set; }

        [Required(ErrorMessage = "Ingrese el precio del producto")]
        [Range(1, double.MaxValue)]
        [DisplayFormat(DataFormatString = "{0:F2}", ApplyFormatInEditMode = true)]
        public double Precio { get; set; }

        [NotMapped] //No se agrega el campo a la base de datos.
        [Range(1.0, 100.0)]
        public int TempCantidad { get; set; }

    }
}
