﻿namespace Web.Controllers.ControllersBase
{
    using Entity.Domain.Enums;
    using Entity.DTOs.Base;
    using Microsoft.AspNetCore.Mvc;
    using Utilities.Exceptions;

    namespace Web.Controllers.BaseController
    {
        [ApiController]
        [Route("api/[controller]")]
        public abstract class BaseController<TSelect, TCreate ,TService> : ControllerBase
        where TService : class
        {
            protected readonly TService _service;
            protected readonly ILogger _logger;

            protected BaseController(TService service, ILogger logger)
            {
                _service = service;
                _logger = logger;
            }

            [HttpGet]
            //[ProducesResponseType(typeof(IEnumerable<TDto>), 200)]
            [ProducesResponseType(200)]
            [ProducesResponseType(500)]
            public virtual async Task<IActionResult> Get([FromQuery] GetAllType getAllType = GetAllType.GetAll)
            {
                try
                {
                    var result = await GetAllAsync(getAllType);
                    return Ok(result);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error obteniendo datos");
                    return StatusCode(500, new { message = "Error interno del servidor." });
                }

                //var result = await DeleteAsync(id, deleteType);

                //if (!result)
                //    return NotFound(new { message = "No se pudo eliminar el recurso." });

                //return Ok(new { message = $"Eliminación {deleteType} realizada correctamente." });
            }

            [HttpGet("{id}")]
            //[ProducesResponseType(typeof(TDto), 200)]
            [ProducesResponseType(200)]
            [ProducesResponseType(400)]
            [ProducesResponseType(404)]
            [ProducesResponseType(500)]
            public virtual async Task<IActionResult> GetById(int id)
            {
                try
                {
                    var result = await GetByIdAsync(id);
                    if (result == null)
                        return NotFound(new { message = $"No se encontró el elemento con ID {id}" });

                    return Ok(result);
                }
                catch (ValidationException ex)
                {
                    _logger.LogWarning(ex, "Validación fallida con ID: {Id}", id);
                    return BadRequest(new { message = ex.Message });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error al obtener el ID {Id}", id);
                    return StatusCode(500, new { message = "Error interno del servidor." });
                }
            }


            [HttpPost]
            [ProducesResponseType(200)]
            [ProducesResponseType(400)]
            [ProducesResponseType(500)]
            public virtual async Task<IActionResult> Post([FromBody] TCreate dto)
            {
                try
                {
                    await AddAsync(dto);
                    return Ok(new { message = "Elemento agregado exitosamente" });
                }
                catch (ValidationException ex)
                {
                    _logger.LogWarning(ex, "Validación fallida al agregar");
                    return BadRequest(new { message = ex.Message });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error al agregar elemento");
                    return StatusCode(500, new { message = "Error interno del servidor." });
                }
            }


            [HttpPut("{id}")]
            [ProducesResponseType(200)]
            [ProducesResponseType(400)]
            [ProducesResponseType(404)]
            [ProducesResponseType(500)]
            public virtual async Task<IActionResult> Put(int id, [FromBody] TCreate dto)
            {
                try
                {
                    if (dto is not BaseDto identifiableDto)
                        return BadRequest(new { message = "El DTO no implementa IHasId." });

                    identifiableDto.Id = id;

                    var updated = await UpdateAsync(id, dto);
                    if (updated == null)
                        return NotFound(new { message = $"No se encontró el recurso con ID {id} para actualizar." });

                    return Ok(new { message = "Elemento actualizado exitosamente." });
                }
                catch (ValidationException ex)
                {
                    _logger.LogWarning(ex, "Validación fallida al actualizar ID: {Id}", id);
                    return BadRequest(new { message = ex.Message });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error al actualizar ID: {Id}", id);
                    return StatusCode(500, new { message = "Error interno del servidor." });
                }
            }




            [HttpDelete("{id}")]
            [ProducesResponseType(200)]
            [ProducesResponseType(400)]
            [ProducesResponseType(404)]
            [ProducesResponseType(500)]
            public async Task<IActionResult> Delete(int id, [FromQuery] DeleteType deleteType = DeleteType.Persistent)
            {
                try
                {
                    var result = await DeleteAsync(id, deleteType);

                    if (!result)
                        return NotFound(new { message = "No se pudo eliminar el recurso." });

                    return Ok(new { message = $"Eliminación {deleteType} realizada correctamente." });
                }
                catch (ValidationException ex)
                {
                    _logger.LogWarning(ex, "Validación fallida al eliminar recurso con id: {ResourceId}", id);
                    return BadRequest(new { message = ex.Message });
                }
                catch (EntityNotFoundException ex)
                {
                    _logger.LogInformation(ex, "Recurso no encontrado para eliminar con id: {ResourceId}", id);
                    return NotFound(new { message = ex.Message });
                }
                catch (ExternalServiceException ex)
                {
                    _logger.LogError(ex, "Error en servicio externo al eliminar recurso con id: {ResourceId}", id);
                    return StatusCode(500, new { message = ex.Message });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error inesperado al eliminar recurso con id: {ResourceId}", id);
                    return StatusCode(500, new { message = "Error interno del servidor." });
                }
            }

            [HttpPatch("logical-restore/{id}")]
            [ProducesResponseType(204)]
            [ProducesResponseType(400)]
            [ProducesResponseType(404)]
            [ProducesResponseType(500)]
            public async Task<IActionResult> RestoreLogical(int id)
            {
                try
                {
                    var result = await RestaureAsync(id);
                    if (result)
                        return NoContent();

                    return NotFound(new { message = $"Rol con id {id} no encontrado o no está marcado como eliminado." });
                }
                catch (ValidationException ex)
                {
                    _logger.LogWarning(ex, "Validación fallida al restaurar lógicamente Rol con id: {RolId}", id);
                    return BadRequest(new { message = ex.Message });
                }
                catch (EntityNotFoundException ex)
                {
                    _logger.LogInformation(ex, "Rol no encontrado para restauración lógica con id: {RolId}", id);
                    return NotFound(new { message = ex.Message });
                }
                catch (ExternalServiceException ex)
                {
                    _logger.LogError(ex, "Error al restaurar lógicamente Rol con id: {RolId}", id);
                    return StatusCode(500, new { message = ex.Message });
                }
            }

            // Métodos que las subclases deben implementar
            protected abstract Task<IEnumerable<TSelect>> GetAllAsync(GetAllType getAllType);
            protected abstract Task<TSelect?> GetByIdAsync(int id);
            protected abstract Task AddAsync(TCreate dto);
            protected abstract Task<bool> UpdateAsync(int id, TCreate dto);

            protected abstract Task<bool> DeleteAsync(int id, DeleteType deleteType);
            protected abstract Task<bool> RestaureAsync(int id);
        }


    }

}
