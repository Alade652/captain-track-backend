using CaptainTrackBackend.Application.DTO.ServiceProviders;
using CaptainTrackBackend.Application.DTO.ServiceProviders.CarWash;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptainTrackBackend.Application.Services
{
    public class BookingHub : Hub
    {
        public async Task SendPendingBookingNotification(Guid serviceProviderId)
        {
            await Clients.User(serviceProviderId.ToString()).SendAsync("ReceivePendingBooking");
        }

        public async Task RegisterUser(string userId)
        {
            // map this connection to the userId manually
            await Groups.AddToGroupAsync(Context.ConnectionId, userId);
        }
    }



}
