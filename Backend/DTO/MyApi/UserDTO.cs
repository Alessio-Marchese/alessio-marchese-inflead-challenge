using System.ComponentModel.DataAnnotations;

namespace Backend.DTO.MYAPI
{
    public class UserDTO
    {
        [Key]
        public string Id { get; set; }                  // map from Uid
        public string Email { get; set; }
        public string Username { get; set; }
        public string FullName { get; set; }            // name + surname
        public string ProfilePicUrl { get; set; }
        public string Gender { get; set; }
        public string PhoneNumber { get; set; }
        public string Employment { get; set; }
        public string KeySkill { get; set; }
        public string AddressId { get; set; }

        public DateTime CreationDate { get; set; }      //DB entry date
    }
}
