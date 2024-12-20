﻿using ApiElda.Contexts;
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


        //este get es para obtener la informacion de todos los clientes. 
        [HttpGet]
        public async Task<ActionResult<List<Clientes>>> GetClientes()
        {
            return await applicationDbContext.Clientes.ToListAsync();
        }


        //Este post se utiliza para crear un nuevo usuario
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



        //Este es para logear a los usuarios
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
                return Unauthorized(new { mensaje = "Usuario o contraseña incorrectos." });
            }

            if (!cliente.estado)
            {
                return Unauthorized(new { mensaje = "La cuenta está bloqueada. Contacte al administrador." });
            }

            string contrasenaDecodificada = DecodificarBase64(cliente.contrasena);

            if (contrasenaDecodificada != loginRequest.Contrasena)
            {
                // Incrementar intentos, pero asegurarse de no exceder el máximo permitido
                cliente.intento = Math.Min(cliente.intento + 1, 3);

                if (cliente.intento >= 3)
                {
                    cliente.estado = false;
                    await applicationDbContext.SaveChangesAsync();
                    return Unauthorized(new { mensaje = "Usuario o contraseña incorrectos. La cuenta ha sido bloqueada." });
                }

                await applicationDbContext.SaveChangesAsync();
                return Unauthorized(new { mensaje = "Usuario o contraseña incorrectos.", intentosRestantes = 3 - cliente.intento });
            }

            // Resetear intentos en caso de éxito
            cliente.intento = 0;
            await applicationDbContext.SaveChangesAsync();

            var usuario2 = DecodificarBase64(cliente.usuario);

            return Ok(new
            {
                mensaje = "Inicio de sesión exitoso.",
                cliente = new
                {
                    cliente.id_cliente,
                    usuario2
                }
            });
        }






        //Este Put se tuiliza para actaulizar la informacion de un cliente.
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCliente(int id, Clientes cliente)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                // Buscar el cliente en la base de datos
                var clienteExistente = await applicationDbContext.Clientes.FindAsync(id);

                if (clienteExistente == null)
                {
                    return NotFound(new { mensaje = "Cliente no encontrado." });
                }

                // Bandera para verificar si algún dato cambió
                bool datosModificados = false;

                // Actualizar cada campo solo si es diferente del actual y se envió un valor
                if (!string.IsNullOrWhiteSpace(cliente.nombre) && cliente.nombre != clienteExistente.nombre)
                {
                    clienteExistente.nombre = cliente.nombre;
                    datosModificados = true;
                }

                if (!string.IsNullOrWhiteSpace(cliente.apellido) && cliente.apellido != clienteExistente.apellido)
                {
                    clienteExistente.apellido = cliente.apellido;
                    datosModificados = true;
                }

                if (!string.IsNullOrWhiteSpace(cliente.cedula) && cliente.cedula != clienteExistente.cedula)
                {
                    clienteExistente.cedula = cliente.cedula;
                    datosModificados = true;
                }

                if (!string.IsNullOrWhiteSpace(cliente.telefono) && cliente.telefono != clienteExistente.telefono)
                {
                    clienteExistente.telefono = cliente.telefono;
                    datosModificados = true;
                }

                if (!string.IsNullOrWhiteSpace(cliente.correo) && cliente.correo != clienteExistente.correo)
                {
                    clienteExistente.correo = cliente.correo;
                    datosModificados = true;
                }

                if (!string.IsNullOrWhiteSpace(cliente.usuario) && CodificarBase64(cliente.usuario) != clienteExistente.usuario)
                {
                    clienteExistente.usuario = CodificarBase64(cliente.usuario);
                    datosModificados = true;
                }

                if (!string.IsNullOrWhiteSpace(cliente.contrasena) && CodificarBase64(cliente.contrasena) != clienteExistente.contrasena)
                {
                    clienteExistente.contrasena = CodificarBase64(cliente.contrasena);
                    datosModificados = true;
                }

                if (cliente.intento >= 0 && cliente.intento != clienteExistente.intento)
                {
                    clienteExistente.intento = cliente.intento;
                    datosModificados = true;
                }

                if (cliente.estado != clienteExistente.estado)
                {
                    clienteExistente.estado = cliente.estado;
                    datosModificados = true;
                }

                // Verificar si no se modificó ningún dato
                if (!datosModificados)
                {
                    return BadRequest(new { mensaje = "No se realizaron modificaciones, todos los datos enviados son iguales a los actuales." });
                }

                // Guardar los cambios en la base de datos
                await applicationDbContext.SaveChangesAsync();

                return Ok(new { mensaje = "Cliente actualizado con éxito.", cliente = clienteExistente });
            }
            catch (DbUpdateConcurrencyException)
            {
                return StatusCode(500, "Ocurrió un error al actualizar el cliente.");
            }
        }


    }
}
