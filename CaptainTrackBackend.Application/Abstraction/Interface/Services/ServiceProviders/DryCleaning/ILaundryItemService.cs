using CaptainTrackBackend.Application.DTO;
using CaptainTrackBackend.Application.DTO.ServiceProviders;
using CaptainTrackBackend.Application.DTO.ServiceProviders.DryCleaning;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptainTrackBackend.Application.Abstraction.Interface.Services.ServiceProviders.DryCleaning
{
    public interface ILaundryItemService
    {
       Task<Response<ItemDto>> AddDryCleanerItem(Guid dryCleanerUserid, Guid itemId, decimal price);
       Task<Response<ItemDto>> AddItem(ItemRequestDto request, IFormFile file);
        Task<Response<List<ItemDto>>> GetItems();
        Task<Response<List<ItemDto>>> GetItems(Guid dryCleanerUserid);
    }
}
