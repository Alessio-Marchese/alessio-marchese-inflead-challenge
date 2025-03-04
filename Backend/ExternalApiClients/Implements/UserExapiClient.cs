using Backend.DTO.EXAPI;
using Backend.ExternalApiClients.Interfaces;
using Backend.Utility;

namespace Backend.ExternalApiClients.Implements;

public class UserExapiClient : IUserExapiClient
{
    private readonly HttpClient _httpClient;

    public UserExapiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<Result<List<ExapiUserDTO>>> GetPaginatedUsers(int quantity)
    {
        var response = await _httpClient.GetAsync($"https://random-data-api.com/api/users/random_user?size={quantity}");
        if (response.Content.Headers.ContentType == null || response.Content.Headers.ContentType.MediaType == null || !response.Content.Headers.ContentType.MediaType.Equals("application/json"))
        {
            return Result<List<ExapiUserDTO>>.Failure("Il content type della risposta ricevuta non é 'application/json'");
        }
        var exapiUsers = await response.Content.ReadFromJsonAsync<List<ExapiUserDTO>>();
        if (exapiUsers == null)
        {
            return Result<List<ExapiUserDTO>>.Failure("L'API esterna non ha dato nessun risultato");
        }
        else
        {
            return Result<List<ExapiUserDTO>>.Success(exapiUsers);
        }
    }
}
