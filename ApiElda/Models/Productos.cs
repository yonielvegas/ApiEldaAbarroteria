using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiElda.Models
{
    [Table("producto")]
    public class Productos
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id_producto { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(100)]
        public string nombre { get; set; }

        [Required(ErrorMessage = "El precio unitario es obligatorio")]
        [Column(TypeName = "decimal(10,2)")]
        public decimal precio_uni { get; set; }

        [StringLength(60, ErrorMessage = "La descripción no puede exceder los 60 caracteres")]
        public string descripcion { get; set; }

        [Required(ErrorMessage = "La cantidad en stock es obligatoria")]
        [Range(0, int.MaxValue, ErrorMessage = "La cantidad en stock no puede ser negativa")]
        public int cantidad_stock { get; set; }

        [StringLength(255, ErrorMessage = "La ruta de la imagen no puede exceder los 255 caracteres")]
        public string imagen { get; set; }

        [ForeignKey("Categoria")]
        [Required(ErrorMessage = "La categoría es obligatoria")]
        public int id_categoria { get; set; }
    }
}
