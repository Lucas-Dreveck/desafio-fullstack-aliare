using Microsoft.AspNetCore.Mvc;

namespace Aliare.Weather.Api.Api.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
public abstract class BaseApiController : ControllerBase;
