using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemorialAppApi.Core.Commands
{
    public record DeleteTimelineCommand : IRequest<bool>
    {
        public Guid Id { get; init; }
        public Guid? UpdatedBy { get; init; }
    }
}
