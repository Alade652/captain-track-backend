using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptainTrackBackend.Persistence.RealTime
{
    public class LocationHub : Hub
    {
        public async Task SendLocation(Guid userId, double lat, double lng)
        {
            await Clients.Others.SendAsync("ReceiveLocation", userId, lat, lng);
        }

        public async Task JoinGroup(Guid userId)
        {
            
            await Groups.AddToGroupAsync(Context.ConnectionId, userId.ToString());
        }
        public async Task LeaveGroup(Guid userId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, userId.ToString());
        }


    }
}
