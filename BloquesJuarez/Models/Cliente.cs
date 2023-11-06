using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BloquesJuarez.Models
{
    public class Cliente
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string NombreCliente { get; set; }

        public string Direccion { get; set; }

        [Required(ErrorMessage = "La localidad es Obligatorio.")]
        public string Localidad { get; set; }

        [Required(ErrorMessage = "El teléfono es Obligatorio.")]
        public string Telefono { get; set; }

        [Required(ErrorMessage = "La Provincia es Obligatoria.")]
        public int ProvinciaId { get; set; }

        [ForeignKey("ProvinciaId")]
        public virtual Provincia Provincia { get; set; }

        [Required(ErrorMessage = "La elección de una condición es Obligatorio.")]
        public int CondIvaId { get; set; }

        [ForeignKey("CondIvaId")]
        public virtual CondIva CondIva { get; set; }

        [RegularExpression(@"^[0-9]+$", ErrorMessage = "Solo se permiten números sin espacios en blanco.")]
        public string Cuit { get; set; }
    }
}
