using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiElda.Models
{
    [Table("clientes")]
    public class Clientes
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id_cliente { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        public string nombre { get; set; }

        [Required(ErrorMessage = "El apellido es obligatorio")]
        public string apellido { get; set; }

        [Required(ErrorMessage = "La cédula es obligatoria")]
        public string cedula { get; set; }

        [Required(ErrorMessage = "El teléfono es obligatorio")]
        public string telefono { get; set; }

        [Required(ErrorMessage = "El correo es obligatorio")]
        public string correo { get; set; }

        [Required(ErrorMessage = "El usuario es obligatorio")]
        public string usuario { get; set; }

        [Required(ErrorMessage = "La contraseña es obligatoria")]
        public string contrasena { get; set; }

        [Range(0, 3, ErrorMessage = "Los intentos deben estar entre 0 y 3")]
        public int intento { get; set; }

        public bool estado { get; set; }
    }

    public class LoginRequest
    {
        [Required]
        public string Usuario { get; set; }

        [Required]
        public string Contrasena { get; set; }
    }

}
