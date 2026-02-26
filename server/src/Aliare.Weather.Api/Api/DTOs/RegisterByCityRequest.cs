namespace Aliare.Weather.Api.Api.DTOs;

public record RegisterByCityRequest(string City, string? State = null, string? Country = null);
