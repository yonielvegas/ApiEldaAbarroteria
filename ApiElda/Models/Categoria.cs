using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ApiElda.Models
{
    [Table("Categoria")]
    public class Categoria
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id_categoria { get; set; }

        [Required(ErrorMessage = "El nombre de la categoría es obligatorio")]
        [StringLength(50, ErrorMessage = "El nombre de la categoría no puede exceder los 50 caracteres")]
        public string nombre { get; set; }

        [Required(ErrorMessage = "El impuesto es obligatorio")]
        [Range(0, 1, ErrorMessage = "El impuesto debe estar entre 0 y 1")]
        public decimal impuesto { get; set; }
    }
}
