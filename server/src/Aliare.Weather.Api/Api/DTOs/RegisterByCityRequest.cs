namespace Aliare.Weather.Api.Api.DTOs;

public record RegisterByCityRequest(string CityName, string? StateCode = null, string? CountryCode = null);
