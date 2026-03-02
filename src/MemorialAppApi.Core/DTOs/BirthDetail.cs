using System.Text.Json.Serialization;

namespace MemorialAppApi.Core.DTOs
{
    public class BirthDetail
    {
        [JsonPropertyName("birthDate")]
        public string? BirthDate { get; set; }

        [JsonPropertyName("birthPlace")]
        public BirthPlace? BirthPlace { get; set; }
    }

    public class BirthPlace
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("latitude")]
        public decimal Latitude { get; set; } 

        [JsonPropertyName("longitude")]
        public decimal Longitude { get; set; }

        [JsonPropertyName("address")]
        public string Address { get; set; } = string.Empty;
    }
}
