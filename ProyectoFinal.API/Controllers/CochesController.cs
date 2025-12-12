using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProyectoFinal.Dominio.Interfaces;
using ProyectoFinal.Dominio.Modelos;

namespace ProyectoFinal.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CochesController : ControllerBase
    {
        private readonly IRepositorio<Coche> _repositorio;

        public CochesController(IRepositorio<Coche> repositorio)
        {
            _repositorio = repositorio;
        }

        // LEER TODOS (GET: api/Coches)
        [AllowAnonymous] //libre no autent
        [HttpGet]
        public async Task<ActionResult<List<Coche>>> Get()
        {
            return Ok(await _repositorio.ObtenerTodosAsync());
        }

        // LEER UNO SOLO (GET: api/Coches/5)
        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<ActionResult<Coche>> Get(int id)
        {
            var coche = await _repositorio.ObtenerPorIdAsync(id);
            if (coche == null) return NotFound("Coche no encontrado");
            return Ok(coche);
        }

        // INSERTAR NUEVO (POST: api/Coches)
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] Coche coche)
        {
            await _repositorio.AgregarAsync(coche);
            return Ok("Coche guardado correctamente");
        }

        // ELIMINAR (DELETE: api/Coches/5)
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var existe = await _repositorio.ObtenerPorIdAsync(id);
            if (existe == null) return NotFound("No se puede borrar, el coche no existe");

            await _repositorio.EliminarAsync(id);
            return Ok("Coche eliminado");
        }


        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, [FromBody] Coche coche)
        {
            coche.Id = id;

            var existe = await _repositorio.ObtenerPorIdAsync(id);
            if (existe == null) return NotFound("Coche no encontrado");

            await _repositorio.ActualizarAsync(coche);
            return Ok("Coche actualizado");
        }
    }
}