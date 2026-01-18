using CaptainTrackBackend.Application.Abstraction.Interface.Maps;
using CaptainTrackBackend.Application.Abstraction.Interface.Repository;
using CaptainTrackBackend.Application.DTO;
using Firebase.Database;
using Firebase.Database.Query;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptainTrackBackend.Persistence.Map
{
    public class TrackingHub : ITrackingHub
    {
        private readonly FirebaseClient _firebaseClient;
        private readonly IUnitofWork _unitofWork;
        
        // Constructor: FirebaseClient parameter is optional - creates its own instance if not provided
        public TrackingHub(IUnitofWork unitofWork, FirebaseClient? firebaseClient = null)
        {
            // Use provided FirebaseClient or create new instance with hardcoded URL
            // Note: In production, this URL should come from configuration
            _firebaseClient = firebaseClient ?? new FirebaseClient("https://locationtracking-df77d-default-rtdb.firebaseio.com/");
            _unitofWork = unitofWork;
        }

        public async Task<Response<string>> SendLocation(Guid userId, double lat, double lng)
        {
            var response = new Response<string>();
            var user = await _unitofWork.User.GetAsync(x => x.Id == userId || x.Customer.Id == userId);
            if (user == null)
            {
                response.Message = "User not found";
                response.Success = false;
                return response;
            }
            await _firebaseClient
            .Child("locations")
            .Child(user.Role.ToString())
            .Child(user.Id.ToString())
            .PutAsync(new LocationDto
            {
                Latitude = lat,
                Longitude = lng,
                Timestamp = DateTime.UtcNow
            });
            response.Data = "Location sent successfully";
            response.Success = true;
            response.Message = "Location sent successfully";
            return response;

        }

        public async Task<Response<LocationDto>> GetLocation(Guid userId)
        {
            var response = new Response<LocationDto>();
            var user = await _unitofWork.User.GetAsync(userId);
            if (user == null)
            {
                response.Message = "User not found";
                response.Success = false;
                return response;
            }

            var location = await _firebaseClient
                .Child("locations")
                .Child(user.Role.ToString())
                .Child(userId.ToString())
                .OnceSingleAsync<LocationDto>();
            if (location == null)
            {
                response.Message = "Location not found";
                response.Success = false;
                return response;
            }
            response.Data = location;
            response.Success = true;
            response.Message = "Location retrieved successfully";
            return response;

        }
    }
}
