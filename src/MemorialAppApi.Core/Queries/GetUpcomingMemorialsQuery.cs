using MemorialAppApi.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemorialAppApi.Core.Queries
{
    public record GetUpcomingMemorialsQuery : IRequest<UpcomingMemorialResponseDto>
    {
        public Guid UserId { get; init; }
    }
}
