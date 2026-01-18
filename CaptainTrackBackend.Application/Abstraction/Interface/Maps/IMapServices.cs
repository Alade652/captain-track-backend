using CaptainTrackBackend.Application.DTO;
using CaptainTrackBackend.Application.DTO.MapModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptainTrackBackend.Application.Abstraction.Interface.Maps
{
    public interface IMapServices
    {
        Task<DistanceMatrixResponse> GetDistanceAsync(string origin, string destination);
        Task<GeocodingResponse> GetCoordinatesAsync(string address);
        Task<ReverseGeocodingResponse> GetAddressAsync(double lat, double lng);
        Task<TimeZoneResponse> GetTimeZoneAsync(double lat, double lng, long timestamp);
        Task<Response<string>> GetDistanceBetweenDriverAndCustomer(string driver, string customer);
        Task<(double DistanceMeters, double DurationSeconds)> GetDistanceAndDurationAsync(string origin, string destination);
    }
}
