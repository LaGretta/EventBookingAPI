using System.Security.Claims;
using EventBookingAPI.DTOs;
using EventBookingAPI.Interfaces.Service;
using EventBookingAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventBookingAPI.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class BookingController : ControllerBase
{
    private readonly IBookingService _bookingService;

    public BookingController(IBookingService bookingService)
    {
        _bookingService = bookingService;
    }
    private int GetUserId()
    {
        return int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateBookingDto dto)
    {
        var userId = GetUserId();
        var booking = await _bookingService.CreateAsync(dto, userId);
        return Ok(booking);
    }

    [HttpGet]
    public async Task<IActionResult> GetPagedAsync(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var userId = GetUserId();
        var bookings = await _bookingService.GetPagedAsync(page, pageSize, userId);
        return Ok(bookings);
    }
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetByIdAsync(int id)
    {
        var userId = GetUserId();
        var find = await _bookingService.GetByIdAsync(id ,  userId);
        return Ok(find);
    }

    [HttpPatch("{id:int}/cancel")]
    public async Task<IActionResult> CancelById(int id)
    {
        var userId = GetUserId();
        var find =  await _bookingService.CancelBookingAsync(id ,  userId);
        return Ok(find);
    }
}
