using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiDeployReservas.Controllers.Dtos.Area;
using ApiDeployReservas.Controllers.Helpers;
using ApiDeployReservas.Controllers.Mappers;
using ApiDeployReservas.Data;
using ApiDeployReservas.Repository.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiDeployReservas.Controllers
{
    [Route("api/area")]
    [ApiController]
    public class AreaController : ControllerBase
    {
        // private readonly ApplicationDBContext _context;
        private readonly IAreaRepository _areaRepository;
        private readonly IReservaAreaRepository _IReservaAreaRepository;
        public AreaController(IAreaRepository areaRepository, IReservaAreaRepository reservaAreaRepository)
        {
            _areaRepository = areaRepository;
            _IReservaAreaRepository = reservaAreaRepository;
        }


        [HttpGet("GetAll-areas")]
        [Authorize]
        public async Task<IActionResult> GetAll([FromQuery] QueryObject query)
        {

            var areas = await _areaRepository.GetAllAsync(query);

            var areasDto = areas.Select(a => a.ToAreasDto());

            return Ok(areasDto);
        }

        [HttpGet("GetById-areas{id:guid}")]

        [Authorize]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            var area = await _areaRepository.GetByIdAsync(id);

            if (area == null)
            {
                return NotFound();
            }

            return Ok(area);
        }

        [HttpPost("create-area")]

        [Authorize]
        public async Task<IActionResult> Create([FromBody] CreateAreaRequestDto AreaDto)
        {

            if (!ModelState.IsValid)
                return BadRequest(ModelState);


            var areaModel = AreaDto.ToAreaFromCreateDto();
            await _areaRepository.CreateAsync(areaModel);
            return CreatedAtAction(nameof(GetById), new { id = areaModel.Id }, areaModel.ToAreaDto());
        }

        [HttpPut]
        [Authorize]
        [Route("Update-areas/{id:guid}")]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] CreateAreaRequestDto Area)
        {


            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var areaModel = await _areaRepository.UpdateAsync(id, Area);

            if (areaModel == null)
            {
                return NotFound();
            }


            return Ok(areaModel);
        }

        [HttpDelete]
        [Authorize]
        [Route("Delete-areas/{id:guid}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {

            var isAssignedToReservation = await _IReservaAreaRepository.CountActiveReservationsByUserAsync(id);
            if (isAssignedToReservation != 0)
            {
                return BadRequest("No se puede eliminar, ya que está asignado a una reserva activa.");
            }
            var area = await _areaRepository.DeleteAsync(id);

            if (area == null)
            {
                return NotFound();
            }

            return NoContent();

        }
    }
}