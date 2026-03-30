using MemorialAppApi.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemorialAppApi.Core.Commands
{
    public record UpdateTimelineCommand : IRequest<TimelineEntryDto>
    {
        public Guid Id { get; init; }
        public string? Title { get; init; }
        public DateTime? Date { get; init; }
        public string? Description { get; init; }
        public TimelineMediaDto? Media { get; init; }
        public Guid? UpdatedBy { get; init; }
    }
}
