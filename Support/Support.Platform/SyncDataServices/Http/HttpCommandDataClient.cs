using System.Text;
using System.Text.Json;
using Support.Platform.DTO;
using Support.Platform.SycnDataServices.Http.Interfaces;

namespace Support.Platform.SycnDataServices.Http;

public class HttpCommandDataClient(HttpClient httpClient, IConfiguration configuration) : ICommandDataCLient
{

    private readonly HttpClient _htppClient = httpClient;
    private readonly IConfiguration _configuration = configuration;

    public async Task SendPlatformToComand(PlatformReadDTO plat)
    {
        StringContent httpContent = new StringContent(
            JsonSerializer.Serialize(plat), 
            Encoding.UTF8, 
            "aplication/json"
        );

        HttpResponseMessage response = await _htppClient.PostAsync($"{_configuration["Command"]}", httpContent);

        if (response.IsSuccessStatusCode)
        {
            Console.WriteLine("Successfully sync post to Command");
        }
        else
        {
            Console.WriteLine("Failed sync post to Command");
        }
    }
}