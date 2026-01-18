using CaptainTrackBackend.Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptainTrackBackend.Domain.Entities.ServiceProviders.DryCleaning
{
    public class LaundryPackage : AuditableEntity
    {
        public Guid? DryCleanerId { get; set; }
        public DryCleaner DryCleaner { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal ExtraCharge { get; set; }
        public string ImageUrl { get; set; }
        public bool ForFreelance { get; set; } = false;
    }
}
