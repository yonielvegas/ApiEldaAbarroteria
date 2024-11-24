using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ApiElda.Models
{
    [Table("detalles_venta")]
    public class DetallesVenta
    {
        [ForeignKey("Venta")]
        [Column(Order = 0)]
        public int id_venta { get; set; }

        [ForeignKey("Cliente")]
        [Column(Order = 1)]
        public int id_cliente { get; set; }

        [ForeignKey("Producto")]
        [Column(Order = 2)]
        public int id_producto { get; set; }

        public Venta Venta { get; set; }
        public Productos Producto { get; set; }
        public Clientes Cliente { get; set; }

        [Required(ErrorMessage = "La cantidad es obligatoria")]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0")]
        public int cantidad { get; set; }

        [Required(ErrorMessage = "El estado del carrito es obligatorio")]
        public bool estado_carro { get; set; }
    }
}
