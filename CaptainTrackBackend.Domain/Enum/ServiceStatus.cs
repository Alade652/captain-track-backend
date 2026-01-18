using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptainTrackBackend.Domain.Enum
{
    public enum ServiceStatus
    {
        Init = 0,
        Pending,
        Booked,
        InProgress,
        Done,
        Cancelled,
        Rejected,

    }

    public enum ServiceMethod
    {
        Scheduled = 1,
        Immediate,
    }
}
