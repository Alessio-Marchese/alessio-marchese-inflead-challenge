using Backend.DTO.EXAPI;
using Backend.Utility;

namespace Backend.ExternalApiClients.Interfaces;

public interface IUserExapiClient
{
    public Task<Result<List<ExapiUserDTO>>> GetPaginatedUsers(int quantity);
}
