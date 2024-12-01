using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebSite.Models
{
    public class Medicamentos
    {
        public int Idmedicamento { get; set; }

        [Required(ErrorMessage = "El campo 'Nombre' es obligatorio.")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El campo 'Concentración' es obligatorio.")]
        public string Concentracion { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "El campo 'Precio' debe ser un número mayor a cero y con máximo dos decimales.")]
        [RegularExpression(@"^\d+(\.\d{1,2})?$", ErrorMessage = "El campo 'Precio' debe ser un número decimal con máximo dos decimales.")]
        public decimal Precio { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "El campo 'Stock' debe ser un número entero mayor o igual a cero.")]
        public int Stock { get; set; }

        [Required(ErrorMessage = "El campo 'Presentación' es obligatorio.")]
        public string Presentacion { get; set; }
        public string FormaFarmaceuticaNombre { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Seleccione una forma.")]
        [Required(ErrorMessage = "Seleccione una forma farmacéutica.")]
        public int? Idformafarmaceutica { get; set; }
        [Required(ErrorMessage = "El campo 'Habilitado' es obligatorio.")]
        [Range(0, 1, ErrorMessage = "Seleccione un valor.")]
        public int? Bhabilitado { get; set; }
    }
}