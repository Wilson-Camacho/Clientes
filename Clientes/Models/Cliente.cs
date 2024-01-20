using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Clientes.Models
{
    public class Cliente
    {
        [Column("IdCliente")]
        public int IdCliente { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [Column("Nombre")]
        public string? Nombre { get; set; }

        [Required(ErrorMessage = "El Apellido es obligatorio")]
        [Column("Apellido")]
        public string? Apellido { get; set; }

        [Required(ErrorMessage = "El Email es obligatorio")]
        [Column("Email")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "El Telefono es obligatorio")]
        [Column("Telefono")]
        public string? Telefono { get; set; }

        [Required(ErrorMessage = "El Pais es obligatorio")]
        [Column("PaisResidencia")]
        public string? PaisResidencia { get; set; }

        [Required(ErrorMessage = "El Documento de identificación es obligatorio")]
        [Column("DocumentoIdentificacion")]
        public string? DocumentoIdentificacion { get; set; }

        [Column("FechaNacimiento")]
        public DateTime? FechaNacimiento { get; set; }
        
        [Column("NombreEmpresa")]
        public string? NombreEmpresa { get; set; }

        [Column("CuentaTwitter")]
        public string? CuentaTwitter { get; set; }
        
        [Column("Genero")]
        public string? Genero { get; set; }
        
        [Column("UrlGenerado")]
        public string? UrlGenerado { get; set; }

        public ClienteInteres? ClienteInteres { get; set; }
    }
}
 