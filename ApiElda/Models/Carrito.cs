namespace ApiElda.Models
{
    //dentro de la clase definicmos la estructura de la respuesta que deseamos obtener.
    public class Carrito
    {
        //esta contiene la  informacion unica de la factura
        public class DetalleFacturaResponse
        {

            public int IdVenta { get; set; }
            public string NombreCliente { get; set; }
            public decimal MontoTotal { get; set; }

            //dentro de Detalles factura tenemos esta clase que hace de arreglo de objetos. donde se almacena toda la informacion que tenga en el carrito un cliente por producto.
            public List<ProductoDetalle> Productos { get; set; }
            
            public class ProductoDetalle
            {
                public int Id_Producto {  get; set; }
                public string NombreProducto { get; set; }
                public int Cantidad { get; set; }
                public decimal Precio { get; set; }
                public string Imagen { get; set; }
            }

        }

    }
}
