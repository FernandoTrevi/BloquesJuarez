namespace BloquesJuarez.Models.ViewModels
{
    public class ClienteConPendientesVM
    {
        public int Id { get; set; }
        public string NombreCliente { get; set; }
        public string Telefono { get; set; }
        public int RemitosPendientes { get; set; }
        public List<RemitoPendienteVM> ListaRemitosPendientes { get; set; }
    }

    public class RemitoPendienteVM
    {
        public int NroRemito { get; set; }
        public DateTime Fecha { get; set; }
        public List<RemitoDetalleVM> Detalles { get; set; }
    }

    public class RemitoDetalleVM
    {
        public string NombreProducto { get; set; }
        public decimal Cantidad { get; set; }
        public double PrecioUnitario { get; set; }
    }

}
