using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemorialAppApi.Core.DTOs
{
    public record ContactDto
    {
        public string Email { get; init; } = string.Empty;
        public string Phone { get; init; } = string.Empty;
        public string Website { get; init; } = string.Empty;
        public string OfficeAddress { get; init; } = string.Empty;
    }
}
