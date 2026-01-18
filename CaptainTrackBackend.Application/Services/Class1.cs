/*using Afiari.Application.Models.ResponseDTO;
using DocumentFormat.OpenXml.Office2021.Excel.RichDataWebImage;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Afiari.Application.Hubs
{
    public interface IActivityHub
    {
        Task GetAllActivities(List<ActivityLogDTO> groupName);
        Task GetRecentActivities(List<ActivityLogDTO> groupName);

        Task JoinOrExitGroup(bool clientJoined);

        Task RecieveMessage(string message);
    }

    public sealed class ActivityHub : Hub<IActivityHub>
    {
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
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task BroadcastAllStoreActivities(string groupName, List<ActivityLogDTO> message)
        {
            await this.Clients.Group(groupName).GetAllActivities(message);
        }

        public async Task OnJoinOrExit(string groupName, bool clientJoined)
        {
            await this.Clients.Group(groupName).JoinOrExitGroup(clientJoined);
        }

        public async Task BroadcastStoreRecentActivities(string groupName, List<ActivityLogDTO> messsage)
        {
            await this.Clients.Group(groupName).GetRecentActivities(messsage);
        }

        #endregion
    }
}
*/