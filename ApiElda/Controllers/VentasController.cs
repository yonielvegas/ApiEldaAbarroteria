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
                .Where(dv => dv.id_cliente == idCliente && dv.estado_carro == false) // Asegúrate de filtrar correctamente por EstadoCarro
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
                    PrecioUnitario = dv.Producto.precio_uni
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

    }
}
