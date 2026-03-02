using System.Text.Json.Serialization;

namespace MemorialAppApi.Core.DTOs
{
    public class PassingDetail
    {
        [JsonPropertyName("passingDate")]
        public string? PassingDate { get; set; }

        [JsonPropertyName("passingPlace")]
        public PassingPlace? PassingPlace { get; set; }
    }

    public class PassingPlace
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
