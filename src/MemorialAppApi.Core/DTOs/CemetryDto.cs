using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemorialAppApi.Core.DTOs
{
    public record CemetryDto
    {
        public Guid? Id { get; init; }
        public string Name { get; init; } = string.Empty;
        public string Location { get; init; } = string.Empty;
        public string StreetAddress { get; init; } = string.Empty;
        public string Longitude { get; init; } = string.Empty;
        public string Latitude { get; init; } = string.Empty;
        public string Description { get; init; } = string.Empty;
        public string AdditionalInfo { get; init; } = string.Empty;
        public string Status { get; init; } = string.Empty;
        public ContactDto? ContactInfo { get; init; }
        public DateTime? CreatedAt { get; init; }
        public DateTime? UpdatedAt { get; init; }
    }
}
