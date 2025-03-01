namespace Backend.Entities;

public class Address
{
    public Guid Uid { get; set; }
    public string Id { get; set; }                 
    public string City { get; set; }
    public string Street { get; set; }              
    public string ZipCode { get; set; }
    public string State { get; set; }

    public DateTime CreationDate { get; set; }

    public Address(string id, string city, string street, string zipCode, string state)
    {
        Uid = Guid.NewGuid();
        Id = id;
        City = city;
        Street = street;
        ZipCode = zipCode;
        State = state;
        CreationDate = DateTime.Now;
    }
}
