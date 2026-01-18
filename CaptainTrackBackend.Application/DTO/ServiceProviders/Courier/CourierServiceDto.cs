using CaptainTrackBackend.Domain.Entities.ServiceProviders.Courier;
using CaptainTrackBackend.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptainTrackBackend.Application.DTO.ServiceProviders.Courier
{
    public class CourierServiceDto
    {
        public Guid CourierServiceId { get; set; }
        public Guid CustomerId { get; set; }
        public Guid? RiderorParkId { get; set; }
        public string Vehicle { get; set; }
        public string PickupLocation { get; set; }
        public string Destination { get; set; }
        public string ServiceStatus { get; set; }
        public string Distance { get; set; }
        public string Duration { get; set; }
        public string? DistanceToPickupLocation { get; set; }
        public string? DurationToPickupLocation { get; set; }
        public decimal EstimatedPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal NegotiatedPrice { get; set; }
        public string DeliveryType { get; set; }
        public int ProviderNotified { get; set; }
    }

    public class CourierServiceRequest
    {
        public Guid VehicleId { get; set; }
        public string PickupLocation { get; set; }
        public string Destination { get; set; }
        public DeliveryType DeliveryType { get; set; }
    }
}
