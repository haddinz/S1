using AutoMapper;
using Command.Data.Interfaces;
using Command.Dtos;
using Command.Models;
using Microsoft.AspNetCore.Mvc;

namespace Command.Controller;

[Route("api/c/[controller]")]
[ApiController]
public class PlatformController(
    ILogger<PlatformController> logger,
    ICommandsRepo commands,
    IMapper mapper
) : ControllerBase
{
    private readonly ILogger _logger = logger;
    private readonly ICommandsRepo _commands = commands;
    private readonly IMapper _mapper = mapper;

    [HttpGet]
    public ActionResult<IEnumerable<PlatformReadDto>> GetPlatform()
    {
        _logger.LogInformation("--> Hit GetPlatform");
        IEnumerable<Platform> platformItems = _commands.GetAllPlatform();

        return Ok(_mapper.Map<IEnumerable<PlatformReadDto>>(platformItems));
    }

    [HttpPost]
    public ActionResult TestConnetion()
    {
        Console.WriteLine("Trying to connect");

        return Ok("Connected to the Command System");
    }
}
