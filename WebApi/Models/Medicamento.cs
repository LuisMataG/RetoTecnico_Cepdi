using System;
using System.Collections.Generic;

namespace WebApi.Models;

public partial class Medicamento
{
    public int Idmedicamento { get; set; }

    public string? Nombre { get; set; }

    public string? Concentracion { get; set; }

    public int? Idformafarmaceutica { get; set; }

    public decimal? Precio { get; set; }

    public int? Stock { get; set; }

    public string? Presentacion { get; set; }

    public int? Bhabilitado { get; set; }

    public virtual Formasfarmaceutica? IdformafarmaceuticaNavigation { get; set; }
}
