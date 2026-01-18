using CaptainTrackBackend.Application.Abstraction.Interface.Maps;
using CaptainTrackBackend.Application.DTO;
using CaptainTrackBackend.Application.DTO.MapModels;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptainTrackBackend.Persistence.Map
{
    public class MapServices : IMapServices
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public MapServices(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _apiKey = configuration["GoogleMaps:ApiKey"];
        }

        public async Task<DistanceMatrixResponse> GetDistanceAsync(string origin, string destination)
        {
            string url = $"https://maps.googleapis.com/maps/api/distancematrix/json?origins={origin}&destinations={destination}&key={_apiKey}";

            var response = await _httpClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<DistanceMatrixResponse>(json);
            }

            throw new HttpRequestException("Google Maps API call failed: " + response.ReasonPhrase);
        }

        public async Task<GeocodingResponse> GetCoordinatesAsync(string address)
        {
            string url = $"https://maps.googleapis.com/maps/api/geocode/json?address={address}&key={_apiKey}";
            var response = await _httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<GeocodingResponse>(json);
            }

            throw new HttpRequestException("Google Geocoding API call failed: " + response.ReasonPhrase);
        }

        public async Task<ReverseGeocodingResponse> GetAddressAsync(double lat, double lng)
        {
            string url = $"https://maps.googleapis.com/maps/api/geocode/json?latlng={lat},{lng}&key={_apiKey}";
            var response = await _httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<ReverseGeocodingResponse>(json);
            }

            throw new HttpRequestException("Google Reverse Geocoding API call failed: " + response.ReasonPhrase);
        }

        public async Task<TimeZoneResponse> GetTimeZoneAsync(double lat, double lng, long timestamp)
        {
            string url = $"https://maps.googleapis.com/maps/api/timezone/json?location={lat},{lng}&timestamp={timestamp}&key={_apiKey}";
            var response = await _httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<TimeZoneResponse>(json);
            }

            throw new HttpRequestException("Google Time Zone API call failed: " + response.ReasonPhrase);
        }

        public Task<Response<string>> GetCurrentLocation(string location)
        {

            throw new NotImplementedException();
        }

        public Task<Response<string>> GetDistanceBetweenDriverAndCustomer(string driver, string customer)
        {

            throw new NotImplementedException();
        }

        public async Task<(double DistanceMeters, double DurationSeconds)> GetDistanceAndDurationAsync(string origin, string destination)
        {
            var url = $"https://maps.googleapis.com/maps/api/distancematrix/json?origins={origin}&destinations={destination}&key={_apiKey}";
            var response = await _httpClient.GetAsync(url);
            var json = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var data = JsonConvert.DeserializeObject<DistanceMatrixResponse>(json);

                var element = data.rows.First().elements.First();
                return (element.distance.value, element.duration.value);
            }

            throw new HttpRequestException("Distance Matrix API failed: " + response.ReasonPhrase);
        }

    }
}
