namespace Ventas.AplicacionWeb.Utilidades.Response
{
    public class GenericResponse<TObject>
    {
        public bool estado { get; set; }
        public string? mensaje { get; set; }
        public TObject? objeto { get; set; }
        public List<TObject>? ListaObjecto { get; set; }
    }
}
