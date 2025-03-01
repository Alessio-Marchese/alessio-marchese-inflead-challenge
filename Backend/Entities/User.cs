namespace Backend.Entities;

public class User
{
    public Guid Uid { get; set; }
    public int Id { get; set; }                  
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

    public User()
    {
        CreationDate = DateTime.Now;
    }
}
