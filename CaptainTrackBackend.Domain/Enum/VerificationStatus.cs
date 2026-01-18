using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptainTrackBackend.Domain.Enum
{
    public enum VerificationStatus
    {
        Unverified = 1,
        InProgress,
        Verified,
        Rejected
    }
}
