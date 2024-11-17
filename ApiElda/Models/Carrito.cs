namespace ApiElda.Models
{
    public class Carrito
    {

        public class DetalleFacturaResponse
        {

            public int IdVenta { get; set; }
            public string NombreCliente { get; set; }
            public decimal MontoTotal { get; set; }
            public List<ProductoDetalle> Productos { get; set; }
            

            public class ProductoDetalle
            {
                public string NombreProducto { get; set; }
                public int Cantidad { get; set; }
                public decimal Precio { get; set; }
                public string Imagen { get; set; }
            }

        }

    }
}
