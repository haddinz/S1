using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Support.Platform.AsyncDataServices;
using Support.Platform.DTO;
using Support.Platform.Interfaces;
using Support.Platform.Models;
using Support.Platform.Models.Enum;
using Support.Platform.SycnDataServices.Http.Interfaces;

namespace Support.Platform.Controller;

[ApiController]
[Route("/api/[controller]")]
public class PlatformsController(
    ILogger<PlatformsController> logger,
    IPlatformRepo platformRepo,
    IMapper mapper,
    ICommandDataCLient commandDataCLient,
    IMessageBusClient messageBusClient
) : ControllerBase
{
    private readonly ILogger _logger = logger;
    private readonly IPlatformRepo _platformRepo = platformRepo;
    private readonly IMapper _mapper = mapper;
    private readonly ICommandDataCLient _commandDataCLient = commandDataCLient;
    private readonly IMessageBusClient _messageBusClient = messageBusClient;

    [HttpGet]
    public ActionResult<IEnumerable<PlatformReadDTO>> GetPlatform()
    {
        Console.WriteLine("Getting all platform ...");

        IEnumerable<PlatformModel> platformItem = _platformRepo.GetAllPlatforms();

        return Ok(_mapper.Map<IEnumerable<PlatformReadDTO>>(platformItem));
    }

    [HttpGet("{id}", Name = "GetPlatformId")]
    public ActionResult<PlatformReadDTO> GetPlatformId(Guid id)
    {
        Console.WriteLine($"Getting platform by id = {id}");

        PlatformModel platformItem = _platformRepo.GetPlatformById(id);

        if (platformItem != null)
        {
            return Ok(_mapper.Map<PlatformReadDTO>(platformItem));
        }

        return NotFound();
    }

    [HttpPost]
    public async Task<ActionResult<PlatformReadDTO>> CreatePlatform(
        PlatformCreateDTO platformCreate
    )
    {
        PlatformModel platform = _mapper.Map<PlatformModel>(platformCreate);
        _platformRepo.CreatePlatform(platform);
        _platformRepo.SaveChanges();

        PlatformReadDTO platformItem = _mapper.Map<PlatformReadDTO>(platform);

        // Send Sync Message
        // try
        // {
        //     await _commandDataCLient.SendPlatformToComand(platformItem);
        // }
        // catch (Exception ex)
        // {
        //     Console.WriteLine($"Cant connect to command syncronously {ex.Message}");
        // }

        // Send Async Message
        try
        {
            PlatformPublishedDto publishedDto = _mapper.Map<PlatformPublishedDto>(platformItem);
            // publishedDto.Event = "Platform_Published";
            publishedDto.Event = Published.Platform_Published;

            await _messageBusClient.PublishNewPlatform(publishedDto);
        }
        catch (Exception ex)
        {
            _logger.LogError("--> Cant connect to command syncronously {ex.Message}", ex.Message);
        }

        return CreatedAtAction(nameof(GetPlatformId), new { id = platformItem.Id }, platformItem);
    }
}
