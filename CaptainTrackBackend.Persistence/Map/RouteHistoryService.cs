using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptainTrackBackend.Persistence.Map
{
    public class RouteHistoryService
    {
        public string DriverLocation { get; set; }
        private readonly Dictionary<Guid, List<DriverLocation>> _history = new();

        public async Task<string> Log(Guid driverId, double lat, double lng)
        {
            if (!_history.ContainsKey(driverId))
                _history[driverId] = new List<DriverLocation>();

            _history[driverId].Add(new DriverLocation
            {
                DriverId = driverId,
                Latitude = lat,
                Longitude = lng,
                Timestamp = DateTime.UtcNow
            });

            string message = "location logged";
            return message;

        }

        public async Task<List<DriverLocation>> GetHistory(Guid driverId)
        {
            return _history.ContainsKey(driverId) ? _history[driverId] : new List<DriverLocation>();
        }

    }
}
