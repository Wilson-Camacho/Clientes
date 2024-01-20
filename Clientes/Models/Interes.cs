using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations.Schema;

namespace Clientes.Models
{
    public class Interes
    {
        [Column("idInteres")]
        public int IdInteres { get; set; }

        [Column("Descripcion")]
        public string? Descripcion { get; set; }

    }
}
