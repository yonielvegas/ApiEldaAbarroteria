using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiElda.Models
{
    [Table("venta")]
    public class Venta
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id_venta { get; set; }

        [Required]
        [Column(TypeName = "timestamp")]
        public DateTime fecha { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "El monto es obligatorio")]
        [Range(0, double.MaxValue, ErrorMessage = "El monto debe ser mayor o igual a 0")]
        public decimal monto { get; set; }
    }
}
