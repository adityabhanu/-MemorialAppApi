using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemorialAppApi.Core.DTOs
{
    public record BurialDto
    {
        public string Id { get; init; }
        public string BurialType { get; init; } = string.Empty;
        public string PlotNumber { get; init; } = string.Empty;
        public string Longitude { get; init; } = string.Empty;
        public string Latitude { get; init; } = string.Empty;
        public string Inscription { get; init; } = string.Empty;
        public string Gravesite { get; init; } = string.Empty;
        public bool Cenotaph { get; init; } = false;
        public bool Monument { get; init; } = false;
    }
}
