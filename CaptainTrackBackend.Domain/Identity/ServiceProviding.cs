using CaptainTrackBackend.Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptainTrackBackend.Domain.Identity
{
    public class ServiceProviding : AuditableEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public List<ServiceProviding> ServiceProvidings { get; set; } = new List<ServiceProviding>();
    }
}
