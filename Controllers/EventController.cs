using System.Security.Claims;
using EventBookingAPI.DTOs;
using EventBookingAPI.Interfaces.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventBookingAPI.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class EventController : ControllerBase
{
    private readonly IEventsService _service;

    public EventController(IEventsService service)
    {
        _service = service;
    }
    private int GetUserId()
    {
        return int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    }

    [HttpGet]
    public async Task<IActionResult> GetPagedAsync([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var paged = await _service.GetPagedAsync(pageNumber, pageSize);
        return Ok(paged);
    }
    [HttpGet("{id}")]
    public async Task<IActionResult> GetEventById(int id)
    {
        var find = await _service.GetByIdAsync(id);
        return Ok(find);
    }

    [Authorize(Roles =  "Admin")]
    [HttpPost]
    public async Task<IActionResult> CreateAsync(CreateEventDto dto)
    {
        var add = await _service.CreateAsync(dto);
        return Ok(add);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAsync(int id, UpdateEventDto dto)
    {
        var find = await _service.GetByIdAsync(id);
        
        var update = await _service.UpdateAsync(id, dto);
        return Ok(update);
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAsync(int id)
    {
        await _service.GetByIdAsync(id);
        await _service.DeleteAsync(id);
        return NoContent();
    }

    [Authorize(Roles = "Admin")]
    [HttpPatch("{id}/publish")]
    public async Task<IActionResult> PublishEventAsync(int id)
    {
        await _service.GetByIdAsync(id);
        var published = await _service.UpdatePublishAsync(id);
        return Ok(published);
    }

    [Authorize(Roles = "Admin")]
    [HttpPatch("{id}/cancel")]
    public async Task<IActionResult> CancelEventAsync(int id)
    {
        await _service.GetByIdAsync(id);
        var cancel =  await _service.CancelEventAsync(id);
        return Ok(cancel);
    }
}
