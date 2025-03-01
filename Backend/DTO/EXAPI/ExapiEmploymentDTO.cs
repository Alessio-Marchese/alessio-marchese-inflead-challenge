using System.Text.Json.Serialization;

namespace Backend.DTO.EXAPI;

public class ExapiEmploymentDTO
{
    public string Title { get; set; }
    [JsonPropertyName("key_skill")]
    public string KeySkill { get; set; }
}
