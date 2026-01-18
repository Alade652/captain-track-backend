using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptainTrackBackend.Domain.Entities.ServiceProviders.RideHailing
{
    public enum TripStatus
    {
        Initiated = 0,
        Pending,
        Accepted,
        InProgress,
        Completed,
        Cancelled
    }
}
