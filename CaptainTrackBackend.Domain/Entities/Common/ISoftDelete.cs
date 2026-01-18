using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptainTrackBackend.Domain.Entities.Common
{
    public interface ISoftDelete
    {
        DateTime? IsDeleteOn { get; set; }
        Guid IsDeleteBy { get; set; }
        bool IsDeleted { get; set; }
    }
}
