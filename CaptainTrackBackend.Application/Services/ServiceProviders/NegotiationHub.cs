using CaptainTrackBackend.Application.DTO.ServiceProviders;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptainTrackBackend.Application.Services.ServiceProviders
{
    public interface INegotiationHub
    {
        Task ReciveOffer(NegotiationDto negotiation);
        //Task GetRecentActivities(Guid userId, Guid bookingId, decimal offer);

        Task JoinOrExitGroup(bool clientJoined);

        Task RecieveMessage(string message);
    }

    public sealed class NegotiationHub : Hub<INegotiationHub>
    {
        /*        public async Task SendOffer(Guid userId, Guid bookingId, decimal offer)
                {
                    //await  this.Clients.All.SendAsync("ReceiveOffer",userId, bookingId, offer);
                }

                public async Task RegisterUser(Guid userId)
                {
                    await Groups.AddToGroupAsync(Context.ConnectionId, userId.ToString());
                }*/

        public override async Task OnConnectedAsync()
        {
            await this.Clients.All.RecieveMessage($"{Context.ConnectionId} has joined");
        }

        #region INVOKE FROM CLIENT
        public async Task JoinGroup(string groupName)
        {
            if (string.IsNullOrWhiteSpace(groupName))
                return;

            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

            await this.Clients.Group(groupName).JoinOrExitGroup(clientJoined: true);

        }

        public async Task LeaveGroup(string groupName)
        {
            if (string.IsNullOrWhiteSpace(groupName))
                return;

            await this.Clients.Group(groupName).JoinOrExitGroup(clientJoined: false);

            await this.Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
        }

        #endregion


        #region LISTEN ON FROM CLIENT
        /// <summary>
        /// send a message from client to server
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="negotiation"></param>
        /// <returns></returns>
        public async Task SendOffer(string groupName, NegotiationDto negotiation)
        {
            await this.Clients.Group(groupName).ReciveOffer(negotiation);
        }

        public async Task OnJoinOrExit(string groupName, bool clientJoined)
        {
            await this.Clients.Group(groupName).JoinOrExitGroup(clientJoined);
        }

/*        public async Task BroadcastStoreRecentActivities(string groupName, Guid userId, Guid bookingId, decimal offer)
        {
            await this.Clients.Group(groupName).GetRecentActivities(userId, bookingId, offer);
        }*/

        #endregion
    }



    public class NotificationResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public int ProvidersNotified { get; set; }
    }
}
