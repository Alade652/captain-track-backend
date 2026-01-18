using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptainTrackBackend.Application.DTO.ServiceProviders
{
    public class NegotiationDto
    {
        public decimal? Offer { get; set; }
        public Guid UserId { get; set; }
        public Guid BookingId { get; set; }
        //public TimeSpan TimeRequired { get; set; }
    }

    public class NegotiationRequestDto
    {
        public decimal? Price { get; set; }
        public TimeSpan TimeRequired { get; set; }

    }
}
