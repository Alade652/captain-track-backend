using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptainTrackBackend.Application.DTO.ServiceProviders
{
    public class BookingDto<T>
    {
        public T Data { get; set; }
    }
}
