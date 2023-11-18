using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace BloquesJuarez.Models.ViewModels
{
    public class RemitoVM
    {
        public Remito Remito { get; set; }
        public List<RemitoDetalle> RemitoDetalle { get; set; }
        public IEnumerable<SelectListItem> ClienteLista { get; set; } 
        public IEnumerable<SelectListItem> ProductoLista { get; set; }
        public string ProductoNombre { get; set; } // Nombre del producto seleccionado

        public decimal? ProductoCantidad { get; set; } // Cambiado a float? (nullable)
        public DateTime FechaActual { get; set; }


    }


}
