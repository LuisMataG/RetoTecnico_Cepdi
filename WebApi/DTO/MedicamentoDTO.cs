namespace WebApi.DTO
{
    public class MedicamentoDTO
    {
        public int Idmedicamento { get; set; }
        public string? Nombre { get; set; }
        public string? Concentracion { get; set; }
        public decimal? Precio { get; set; }
        public int? Stock { get; set; }
        public string? Presentacion { get; set; }
        public string? FormaFarmaceuticaNombre { get; set; }
        public int? Idformafarmaceutica { get; set; }
        public int? Bhabilitado { get; set; }
    }
}
