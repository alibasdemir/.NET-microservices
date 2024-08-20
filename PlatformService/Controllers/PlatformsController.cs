using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PlatformService.AsyncDataServices;
using PlatformService.Data;
using PlatformService.Dtos;
using PlatformService.Models;
using PlatformService.SyncDataServices.Http;

namespace PlatformService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlatformsController : ControllerBase
    {
        private readonly IPlatformRepo _repository;
        private readonly IMapper _mapper;
        private readonly ICommandDataClient _commandDataClient;
        private readonly IMessageBusClient _messageBusClient;

        public PlatformsController(IPlatformRepo repository, IMapper mapper, ICommandDataClient commandDataClient, IMessageBusClient messageBusClient)
        {
            _repository = repository;
            _mapper = mapper;
            _commandDataClient = commandDataClient;
            _messageBusClient = messageBusClient;
        }

        [HttpGet]
        public ActionResult<IEnumerable<PlatformReadDto>> GetPlatforms()
        {
            Console.WriteLine("---> Getting Platforms...");

            var platformItem = _repository.GetAllPlatforms();
            return Ok(_mapper.Map<IEnumerable<PlatformReadDto>>(platformItem));
        }

        [HttpGet("{id}", Name = "GetPlatformById")]
        public ActionResult<PlatformReadDto> GetPlatformById(int id)
        {
            var platformItem = _repository.GetPlatformById(id);

            if (platformItem != null)
            {
                return Ok(_mapper.Map<PlatformReadDto>(platformItem));
            }

            return NotFound();
        }

        [HttpPost]
        // Returns a 201 Created response with a location header pointing to the newly created resource.
        public async Task<ActionResult<PlatformCreateDto>> CreatePlatform(PlatformCreateDto platformCreateDto)
        {
            var platformModel = _mapper.Map<Platform>(platformCreateDto);
            _repository.CreatePlatform(platformModel);
            _repository.SaveChanges();

            var platformReadDto = _mapper.Map<PlatformReadDto>(platformModel);

            // Send Sync Message
            try
            {
                await _commandDataClient.SendPlatformToCommand(platformReadDto);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"---> Could not send synchronously: {ex.Message}");
            };

            // Send Async Message
            try
            {
                var platformPusblishedDto = _mapper.Map<PlatformPublishedDto>(platformReadDto);
                platformPusblishedDto.Event = "Platform_Published";
                _messageBusClient.PublishNewPlatform(platformPusblishedDto);
            }
            catch(Exception ex)
            {
                Console.WriteLine($"---> Could not send asynchronously: {ex.Message}");
            }

            // The 'CreatedAtRoute' method generates the URL of the newly created resource using the 'GetPlatformById' route name.
            // 'nameof(GetPlatformById)' specifies the route name for generating the URL.
            // 'new { Id = platformReadDto.Id }' provides the route parameters needed for the URL, such as the new resource's ID.
            // 'platformReadDto' is the body of the response, containing details of the newly created platform.
            return CreatedAtRoute(nameof(GetPlatformById), new { Id = platformReadDto.Id }, platformReadDto);
        }
    }
}
