using ApiElda.Contexts;
using ApiElda.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiElda.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VentasController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;

        public VentasController(ApplicationDbContext dbContext)
        {
            this._dbContext = dbContext;
        }

        [HttpGet("Carrito/{idCliente}")]
        public async Task<ActionResult<Carrito.DetalleFacturaResponse>> ObtenerCarrito(int idCliente)
        {
            // Obtener detalles de la venta, filtrados por el estado del carrito y cliente
            var detallesVenta = await _dbContext.DetallesVenta
                .Where(dv => dv.id_cliente == idCliente && dv.estado_carro == false) // Filtrar correctamente por EstadoCarro
                .Include(dv => dv.Venta) // Incluir la venta relacionada
                .Include(dv => dv.Producto) // Incluir el producto relacionado
                .ToListAsync();

            // Verificar si el cliente tiene detalles de venta
            if (!detallesVenta.Any())
            {
                return NotFound(new { mensaje = "No se encontraron productos en el carrito para este cliente." });
            }

            // Obtener el cliente
            var cliente = await _dbContext.Clientes
                .FirstOrDefaultAsync(c => c.id_cliente == idCliente);

            if (cliente == null)
            {
                return NotFound(new { mensaje = "Cliente no encontrado" });
            }

            // Crear la respuesta con la información del carrito
            var response = new Carrito.DetalleFacturaResponse
            {
                IdVenta = detallesVenta.FirstOrDefault()?.id_venta ?? 0, // Obtener el IdVenta de los detalles
                NombreCliente = $"{cliente.nombre} {cliente.apellido}",
                MontoTotal = detallesVenta.Sum(d => d.cantidad * d.Producto.precio_uni), // Calcular el monto total
                Productos = detallesVenta.Select(dv => new Carrito.DetalleFacturaResponse.ProductoDetalle
                {
                    NombreProducto = dv.Producto.nombre,
                    Cantidad = dv.cantidad,
                    Precio = dv.cantidad * dv.Producto.precio_uni, // Calcula el precio total
                    Imagen = dv.Producto.imagen // Incluir la imagen del producto
                }).ToList()
            };

            // Devolver la respuesta en formato JSON
            return Ok(response);
        }


        [HttpGet("obtener-carrito/{idCliente}")]
        public async Task<IActionResult> ObjetosCarrito(int idCliente)
        {
            try
            {
                // Obtén la suma de las cantidades de los productos en el carrito de un cliente
                var cantidadTotal = await _dbContext.DetallesVenta
                                                  .Where(c => c.id_cliente == idCliente && c.estado_carro == false)
                                                  .SumAsync(c => c.cantidad);

                return Ok(cantidadTotal);
            }
            catch (Exception ex)
            {
                // Manejo de errores
                return StatusCode(500, $"Error al obtener el carrito: {ex.Message}");
            }
        }


        [HttpPost("AgregarAlCarrito")]
        public async Task<IActionResult> AgregarAlCarrito(int idCliente, int idProducto, int cantidad)
        {
            try
            {
                // Validar cliente
                var cliente = await _dbContext.Clientes.FindAsync(idCliente);
                if (cliente == null)
                {
                    return NotFound(new { mensaje = "Cliente no encontrado." });
                }

                // Validar producto
                var producto = await _dbContext.Productos.FindAsync(idProducto);
                if (producto == null)
                {
                    return NotFound(new { mensaje = "Producto no encontrado." });
                }

                if (producto.cantidad_stock < cantidad)
                {
                    return BadRequest(new { mensaje = "No hay suficiente stock disponible." });
                }

                // Buscar un carrito activo para el cliente
                var ventaActiva = await _dbContext.Venta
                    .FirstOrDefaultAsync(v => _dbContext.DetallesVenta.Any(d => d.id_cliente == idCliente && !d.estado_carro && d.id_venta == v.id_venta));

                if (ventaActiva == null)
                {
                    // Crear una nueva venta (carrito)
                    ventaActiva = new Venta
                    {
                        monto = 0, // Inicialmente sin monto
                        fecha = DateTime.Now
                    };

                    await _dbContext.Venta.AddAsync(ventaActiva);
                    await _dbContext.SaveChangesAsync(); // Guardar para generar el ID
                }

                // Verificar si el producto ya está en el carrito
                var detalleExistente = await _dbContext.DetallesVenta
                    .FirstOrDefaultAsync(d => d.id_venta == ventaActiva.id_venta && d.id_producto == idProducto);

                if (detalleExistente != null)
                {
                    // Actualizar cantidad y monto
                    detalleExistente.cantidad += cantidad;
                }
                else
                {
                    // Agregar un nuevo detalle de venta
                    var nuevoDetalle = new DetallesVenta
                    {
                        id_venta = ventaActiva.id_venta,
                        id_cliente = idCliente,
                        id_producto = idProducto,
                        cantidad = cantidad,
                        estado_carro = false
                    };

                    await _dbContext.DetallesVenta.AddAsync(nuevoDetalle);
                }

                // Actualizar el monto total de la venta
                ventaActiva.monto += producto.precio_uni * cantidad;

                // Reducir la cantidad en stock del producto
                producto.cantidad_stock -= cantidad;

                // Guardar cambios
                await _dbContext.SaveChangesAsync();

                return Ok(new { mensaje = "Producto agregado al carrito con éxito." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = $"Error al agregar producto al carrito: {ex.Message}" });
            }
        }



    }
}
