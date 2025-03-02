using System.Text.Json.Serialization;

namespace Backend.DTO.EXAPI;

public class ExapiUserDTO
{
    public Guid Uid { get; set; }
    public int Id { get; set; }
    public string Email { get; set; }
    public string Username { get; set; }
    [JsonPropertyName("first_name")]
    public string FirstName { get; set; }
    [JsonPropertyName("last_name")]
    public string LastName { get; set; }
    [JsonPropertyName("avatar")]
    public string ProfilePicUrl { get; set; }
    public string Gender { get; set; }
    [JsonPropertyName("phone_number")]
    public string PhoneNumber { get; set; }
    public ExapiEmploymentDTO Employment { get; set; }
    public string KeySkill { get; set; }
    public int AddressId { get; set; }
    public ExapiAddressDTO Address { get; set; }
}
