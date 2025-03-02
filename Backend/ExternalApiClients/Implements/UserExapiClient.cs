using Backend.DTO.EXAPI;
using Backend.ExternalApiClients.Interfaces;

namespace Backend.ExternalApiClients.Implements;

public class UserExapiClient : IUserExapiClient
{
    private readonly HttpClient _httpClient;

    public UserExapiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<ExapiUserDTO>?> GetPaginatedUsers(int quantity)
    {
        var response = await _httpClient.GetAsync($"https://random-data-api.com/api/users/random_user?size={quantity}");
        if (response.Content.Headers.ContentType == null || response.Content.Headers.ContentType.MediaType == null || !response.Content.Headers.ContentType.MediaType.Equals("application/json"))
        {
            throw new InvalidOperationException($"Expected response content type 'application/json'");
        }
        return await response.Content.ReadFromJsonAsync<List<ExapiUserDTO>>();
    }
}
