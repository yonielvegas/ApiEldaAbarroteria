using ApiElda.Contexts;
using ApiElda.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;

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

        // Método para codificar en base64
        public static string CodificarBase64(string valor)
        {
            var valorBytes = System.Text.Encoding.UTF8.GetBytes(valor);
            return Convert.ToBase64String(valorBytes);
        }

        // Método para decodificar desde base64
        public static string DecodificarBase64(string valorCodificado)
        {
            var valorBytes = Convert.FromBase64String(valorCodificado);
            return System.Text.Encoding.UTF8.GetString(valorBytes);
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
                // Codificar usuario y contraseña antes de almacenarlos
                cliente.usuario = CodificarBase64(cliente.usuario);
                cliente.contrasena = CodificarBase64(cliente.contrasena);

                // Asignar valores por defecto
                cliente.intento = 0;
                cliente.estado = true;

                applicationDbContext.Clientes.Add(cliente);
                await applicationDbContext.SaveChangesAsync();

                return CreatedAtAction(nameof(GetClientes), new { id = cliente.id_cliente }, cliente);
            }
            catch (DbUpdateException)
            {
                return StatusCode(500, "Ocurrió un error al crear el cliente");
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult<object>> ValidarLogin([FromBody] LoginRequest loginRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var cliente = await applicationDbContext.Clientes
                .FirstOrDefaultAsync(c => c.usuario == CodificarBase64(loginRequest.Usuario));

            if (cliente == null)
            {
                return Unauthorized("Usuario o contraseña incorrectos.");
            }

            string contrasenaDecodificada = DecodificarBase64(cliente.contrasena);

            if (contrasenaDecodificada != loginRequest.Contrasena)
            {
                return Unauthorized("Usuario o contraseña incorrectos.");
            }

            return Ok(new { cliente.id_cliente, cliente.nombre, cliente.apellido });
        }
    }
}
