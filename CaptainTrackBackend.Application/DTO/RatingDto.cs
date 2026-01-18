using CaptainTrackBackend.Domain.Entities;
using CaptainTrackBackend.Domain.Enum;
using CaptainTrackBackend.Domain.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptainTrackBackend.Application.DTO
{
    public class RatingDto
    {
        public Guid Id { get; set; }
        public int Stars { get; set; }
        public string Comment { get; set; }
        public Guid CustomerId { get; set; }
        public Guid VendorUserId { get; set; }
        public string ServiceType { get; set; }
        public Guid CreatedBy { get; set; }
    }

    public class RatingRequest
    {
        public int Stars { get; set; }
        public string Comment { get; set; }
        public Guid CustomerId { get; set; }
        public Guid VendorUserId { get; set; }
        public string ServiceType { get; set; }
    }
}
