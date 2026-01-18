using CaptainTrackBackend.Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptainTrackBackend.Application.DTO.ServiceProviders
{
    public class Location : AuditableEntity
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Timestamp { get; set; }
    }
}
