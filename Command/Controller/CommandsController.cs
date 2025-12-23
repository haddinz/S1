using AutoMapper;
using Command.Data.Interfaces;
using Command.Dtos;
using Command.Models;
using Microsoft.AspNetCore.Mvc;

namespace Command.Controller;

[Route("api/c/platform/{platformId}/[controller]")]
[ApiController]
public class CommandsController(
    ILogger<CommandsController> logger,
    IMapper mapper,
    ICommandsRepo repository
) : ControllerBase
{
    private readonly ILogger _logger = logger;
    private readonly IMapper _mapper = mapper;
    private readonly ICommandsRepo _repository = repository;

    [HttpGet]
    public ActionResult<IEnumerable<CommandsReadDto>> GetCommandsForPlatfrom(Guid platformId)
    {
        _logger.LogInformation("==> Hit GetCommandsFromPlatfrom {platformId}", platformId);

        if (!_repository.PlatformExist(platformId))
        {
            _logger.LogWarning(
                "==> PlatformId GetCommandsFromPlatfrom {platformId} Not Found",
                platformId
            );
            return NotFound();
        }

        IEnumerable<Commands> commands = _repository.GetCommandsForPlatform(platformId);
        return Ok(_mapper.Map<IEnumerable<CommandsReadDto>>(commands));
    }

    [HttpGet("{commandsId}", Name = "GetCommandForPlatform")]
    public ActionResult<CommandsReadDto> GetCommandForPlatform(Guid platformId, Guid commandsId)
    {
        _logger.LogInformation("==> Hit GetCommandFromPlatfrom {platformId}", platformId);

        if (!_repository.PlatformExist(platformId))
        {
            _logger.LogWarning(
                "==> PlatformId GetCommandFromPlatfrom {platformId} / {commandsId} Not Found",
                platformId,
                commandsId
            );
            return NotFound();
        }

        Commands commands = _repository.GetCommand(platformId, commandsId);
        if (commands == null)
        {
            _logger.LogWarning(
                "==> Commands GetCommandFromPlatform {commands} Not Found",
                commands
            );
            return NotFound();
        }

        return Ok(_mapper.Map<CommandsReadDto>(commands));
    }

    [HttpPost]
    public ActionResult<CommandsReadDto> CreateCommandsForPlatform(
        Guid platformId,
        CommandsCreateDto commandsDto
    )
    {
        _logger.LogInformation("==> Hit CreateCommandsForPlatform {platformId}", platformId);

        if (!_repository.PlatformExist(platformId))
        {
            _logger.LogWarning(
                "==> PlatformId CreateCommandsForPlatform {platformId} Not Found",
                platformId
            );
            return NotFound();
        }

        Commands commands = _mapper.Map<Commands>(commandsDto);

        _repository.CreateCommand(platformId, commands);
        _repository.SaveChanges();

        CommandsReadDto commandsReadDto = _mapper.Map<CommandsReadDto>(commands);

        return CreatedAtRoute(
            nameof(GetCommandForPlatform),
            new { platformId = platformId, commandsId = commandsReadDto.Id },
            commandsReadDto
        );
    }
}
