using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Backend.Entities;

public class Address
{
    public Guid Uid { get; set; }
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }                 
    public string City { get; set; }
    public string Street { get; set; }
    public string ZipCode { get; set; }
    public string State { get; set; }

    public DateTime CreationDate { get; set; }

    public Address()
    {
        Uid = Guid.NewGuid();
        CreationDate = DateTime.Now;
    }
}
