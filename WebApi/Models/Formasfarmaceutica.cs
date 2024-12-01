using System;
using System.Collections.Generic;

namespace WebApi.Models;

public partial class Formasfarmaceutica
{
    public int Idformafarmaceutica { get; set; }

    public string? Nombre { get; set; }

    public int? Habilitado { get; set; }

    public virtual ICollection<Medicamento> Medicamentos { get; set; } = new List<Medicamento>();
}
