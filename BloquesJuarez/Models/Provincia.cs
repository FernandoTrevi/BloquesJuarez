using System.ComponentModel.DataAnnotations;

namespace BloquesJuarez.Models
{
    public class Provincia
    {
        [Key]
        public int Id { get; set; }

       
        public string? NombreProvincia { get; set; }
    }
}
