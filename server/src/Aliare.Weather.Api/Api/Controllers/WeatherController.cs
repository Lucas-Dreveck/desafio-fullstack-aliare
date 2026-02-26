using Aliare.Weather.Api.Api.DTOs;
using Aliare.Weather.Api.Domain.Entities;
using Aliare.Weather.Api.Services;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Aliare.Weather.Api.Api.Controllers;

[ApiVersion("1.0")]
public class WeatherController(WeatherService weatherService) : BaseApiController
{
    [Authorize]
    [HttpPost("register/by-city")]
    public async Task<IActionResult> RegisterByCity([FromBody] RegisterByCityRequest request)
    {
        try
        {
            WeatherRecord record = await weatherService.RegisterByCityAsync(request.City, request.State, request.Country);
            return Ok(ToResponse(record));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { error = ex.Message });
        }
    }

    [Authorize]
    [HttpPost("register/by-coordinates")]
    public async Task<IActionResult> RegisterByCoordinates([FromBody] RegisterByCoordinatesRequest request)
    {
        try
        {
            WeatherRecord record = await weatherService.RegisterByCoordinatesAsync(request.Latitude, request.Longitude);
            return Ok(ToResponse(record));
        }
        catch (ArgumentOutOfRangeException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { error = ex.Message });
        }
    }

    [HttpGet("history/by-city/{city}")]
    public async Task<IActionResult> GetHistoryByCity(
        string city,
        [FromQuery] string? state = null,
        [FromQuery] string? country = null)
    {
        try
        {
            IEnumerable<WeatherRecord> records = await weatherService.GetHistoryByCityAsync(city, state, country);
            return Ok(records.Select(ToResponse));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpGet("history/by-coordinates")]
    public async Task<IActionResult> GetHistoryByCoordinates([FromQuery] double latitude, [FromQuery] double longitude)
    {
        try
        {
            IEnumerable<WeatherRecord> records = await weatherService.GetHistoryByCoordinatesAsync(latitude, longitude);
            return Ok(records.Select(ToResponse));
        }
        catch (ArgumentOutOfRangeException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    private static WeatherRecordResponse ToResponse(WeatherRecord record)
    {
        return new WeatherRecordResponse(
            record.Id,
            record.City,
            record.State,
            record.Country,
            record.Temperature,
            record.Latitude,
            record.Longitude,
            record.RecordedAt
        );
    }
}
