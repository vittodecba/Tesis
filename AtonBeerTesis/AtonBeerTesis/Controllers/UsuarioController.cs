using AtonBeerTesis.Application.Interfaces;
using AtonBeerTesis.Application.Dtos;
using AtonBeerTesis.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using AtonBeerTesis.Domain.Interfaces;
using AtonBeerTesis.Application.Dto;

namespace AtonBeerTesis.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        private readonly IUsuarioService _usuarioService;

        public UsuarioController(IUsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] bool mostrarInactivos = false)
        {
            var usuarios = await _usuarioService.GetAllAsync(mostrarInactivos);
            return Ok(usuarios);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var usuario = await _usuarioService.GetByIdAsync(id);
            if (usuario == null) return NotFound();
            return Ok(usuario);
        }

        [HttpPost]
        public async Task<IActionResult> Create(UsuarioCreateDto dto)
        {
            try
            {
                var nuevoUsuario = await _usuarioService.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = nuevoUsuario.Id }, nuevoUsuario);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UsuarioUpdateDto dto)
        {
            try
            {
                await _usuarioService.UpdateAsync(id, dto);
                return Ok(new { message = "Usuario actualizado con éxito" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPatch("{id}/toggle-activo")]
        public async Task<IActionResult> ToggleActivo(int id)
        {
            try
            {
                await _usuarioService.DeleteAsync(id);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePhysical(int id)
        {
            await _usuarioService.DeleteAsync(id);
            return Ok(new { message = "Estado de usuario actualizado" });
        }
    }
}