using CaptainTrackBackend.Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptainTrackBackend.Domain.Entities.ServiceProviders
{
    public class Price : AuditableEntity
    {
        public decimal GasPriceperKg { get; set; }
        public decimal DieselPricePerLitre { get; set; }  
        public decimal AmbulancePrice { get; set; }
        public decimal WaterPricePerLitre { get; set; }
    }
}
