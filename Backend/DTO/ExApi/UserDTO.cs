using Backend.Entities;
using System.Text.Json.Serialization;

namespace Backend.DTO.EXAPI;

public class UserDTO
{
    public Guid Uid { get; set; }
    public int Id { get; set; }
    public string Email { get; set; }
    public string Username { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    [JsonPropertyName("avatar")]
    public string ProfilePicUrl { get; set; }
    public string Gender { get; set; }
    public string PhoneNumber { get; set; }
    public EmploymentDTO Employment { get; set; }
    public string KeySkill { get; set; }
    public AddressDTO Address { get; set; }
}
