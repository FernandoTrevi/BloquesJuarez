using System.ComponentModel.DataAnnotations;

namespace BloquesJuarez.Models
{
    public class Categoria
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Nombre de Categoria es Obligatorio.")]
        public string NombreCategoria { get; set; }
    }
}
