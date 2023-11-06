using System.ComponentModel.DataAnnotations;

namespace BloquesJuarez.Models
{
    public class CondIva
    {
        [Key]
        public int Id { get; set; }

        
        public string? Iva { get; set; }
    }
}
