using System.Text.Json.Serialization;

namespace Backend.DTO.EXAPI;

public class ExapiAddressDTO
{
    public string City { get; set; }
    [JsonPropertyName("street_name")]
    public string StreetName { get; set; }
    [JsonPropertyName("street_address")]
    public string StreetAddress { get; set; }
    [JsonPropertyName("zip_code")]
    public string ZipCode { get; set; }
    public string State { get; set; }
}
