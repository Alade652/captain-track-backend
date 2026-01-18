using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptainTrackBackend.Domain.Enum
{
    public enum NegotiationStatus
    {
        Ongoing = 1,    // Negotiation is ongoing
        Accepted,   // Negotiation has been accepted
        Rejected   // Negotiation has been rejected

    }
}
