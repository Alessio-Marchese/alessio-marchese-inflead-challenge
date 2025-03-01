namespace Backend.Entities;

public class User
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
    public DateTime CreationDate { get; set; }

    public User(Guid uid, string id, string email, string username, string fullName, string profilePicUrl, string gender, string phoneNumber, string employment, string keySkill, Address address)
    {
        Uid = uid;
        Id = id;
        Email = email;
        Username = username;
        FullName = fullName;
        ProfilePicUrl = profilePicUrl;
        Gender = gender;
        PhoneNumber = phoneNumber;
        Employment = employment;
        KeySkill = keySkill;
        Address = address;
        CreationDate = DateTime.Now;
    }
}
