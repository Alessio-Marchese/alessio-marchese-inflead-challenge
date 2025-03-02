using Backend.DTO.EXAPI;

namespace Backend.ExternalApiClients.Interfaces;

public interface IUserExapiClient
{
    public Task<List<ExapiUserDTO>?> GetPaginatedUsers(int quantity);
}
