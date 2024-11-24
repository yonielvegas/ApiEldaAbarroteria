using ApiElda.Contexts;
using ApiElda.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiElda.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductoController : ControllerBase
    {
        private readonly ApplicationDbContext applicationDbContext;

        public ProductoController(ApplicationDbContext applicationDbContext)
        {
            this.applicationDbContext = applicationDbContext;
        }

        //este metodo es para traer los porductos por categoria

        [HttpGet("categoria/{id_categoria}")]
        public async Task<ActionResult<List<Productos>>> GetProductosPorCategoria(int id_categoria)
        {
            var productos = await applicationDbContext.Productos
                .Where(p => p.id_categoria == id_categoria && p.cantidad_stock > 0) // Filtrar productos con stock mayor a 0
                .ToListAsync();

            if (productos == null || productos.Count == 0)
            {
                return NotFound($"No se encontraron productos disponibles para la categoría con ID {id_categoria}");
            }

            return Ok(productos);
        }

    }
}
