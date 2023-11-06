using Microsoft.AspNetCore.Mvc.Rendering;

namespace BloquesJuarez.Models.ViewModels
{
    public class ClienteVM
    {
        public Cliente Cliente { get; set; }
        public IEnumerable<SelectListItem> ProvinciaLista { get; set; }
        public IEnumerable<SelectListItem> CondIvaLista { get; set; }
    }
}
