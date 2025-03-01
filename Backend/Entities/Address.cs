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

    protected Address()
    {
        CreationDate = DateTime.Now;
    }
}
