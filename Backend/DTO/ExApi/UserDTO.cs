using Backend.Entities;

namespace Backend.DTO.EXAPI;

public class UserDTO
{
    public Guid Uid { get; set; }
    public string Id { get; set; }
    public string Email { get; set; }
    public string Username { get; set; }
    public string FullName { get; set; }
    public string ProfilePicUrl { get; set; }
    public string Gender { get; set; }
    public string PhoneNumber { get; set; }
    public string Employment { get; set; }
    public string KeySkill { get; set; }
    public Address Address { get; set; }

    public string City { get; set; }
    public string Street { get; set; }
    public string ZipCode { get; set; }
    public string State { get; set; }
}
