using Microsoft.AspNetCore.Mvc.Rendering;

namespace BloquesJuarez.Models.ViewModels
{
    public class ProductoVM
    {
        public Producto Producto { get; set; }
        public IEnumerable<SelectListItem>? CategoriaLista { get; set; }
    }
}
