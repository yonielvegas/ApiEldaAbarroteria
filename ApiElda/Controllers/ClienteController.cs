using ApiElda.Contexts;
using ApiElda.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiElda.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClienteController : ControllerBase
    {
        private readonly ApplicationDbContext applicationDbContext;

        public ClienteController(ApplicationDbContext applicationDbContext)
        {
            this.applicationDbContext = applicationDbContext;
        }

        [HttpGet]
        public async Task<ActionResult<List<Clientes>>> GetClientes()
        {
            return await applicationDbContext.Clientes.ToListAsync();
        }

        [HttpPost]
        public async Task<ActionResult<Clientes>> CreateCliente(Clientes cliente)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                cliente.intento = 0;       // Intento siempre es 0
                cliente.estado = true;     // Estado siempre es true

                applicationDbContext.Clientes.Add(cliente);
                await applicationDbContext.SaveChangesAsync();

                return CreatedAtAction(nameof(GetClientes), new { id = cliente.id_cliente }, cliente);
            }
            catch (DbUpdateException)
            {
                return StatusCode(500, "Ocurrió un error al crear el cliente");
            }
        }


    }
}
