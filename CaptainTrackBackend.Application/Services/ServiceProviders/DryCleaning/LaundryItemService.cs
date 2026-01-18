using CaptainTrackBackend.Application.Abstraction.Interface.Repository;
using CaptainTrackBackend.Application.Abstraction.Interface.Services.ServiceProviders.DryCleaning;
using CaptainTrackBackend.Application.Context;
using CaptainTrackBackend.Application.DTO;
using CaptainTrackBackend.Application.DTO.ServiceProviders;
using CaptainTrackBackend.Application.DTO.ServiceProviders.DryCleaning;
using CaptainTrackBackend.Application.Services.FileUpload;
using CaptainTrackBackend.Domain.Entities.ServiceProviders;
using CaptainTrackBackend.Domain.Entities.ServiceProviders.DryCleaning;
using CaptainTrackBackend.Domain.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptainTrackBackend.Application.Services.ServiceProviders.DryCleaning
{
    public class LaundryItemService : ILaundryItemService
    {
        private readonly IUnitofWork _unitOfWork;
        private readonly ApplicationDbContext _context;
       
        public LaundryItemService(ApplicationDbContext context, IUnitofWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _context = context;
        }

        public async Task<Response<ItemDto>> AddDryCleanerItem(Guid dryCleanerUserid, Guid itemId, decimal price)
        {
            var dryCleaner = await _unitOfWork.DryCleaner.GetAsync(x => x.UserId == dryCleanerUserid);
            var check = await _unitOfWork.LaundryItem.ExistsAsync(x => x.Id == itemId);
            if (!check)
            {
                return new Response<ItemDto>
                {
                    Message = "LaundryItem does not exists",
                    Success = false
                };
            }
            var item = await _context.DryCleanerLaundryItems
                .FirstOrDefaultAsync(x => x.DrycleanerId == dryCleaner.Id && x.LaundryItemId == itemId);
            if (item!= null)
            {
                return new Response<ItemDto>
                {
                    Message = "Item already added",
                    Success = false
                };
            }
            var dryCleanerItem = new DryCleanerLaundryItem
            {
                DrycleanerId = dryCleaner.Id,
                LaundryItemId = itemId,
                Price = price,
            };
            await _context.DryCleanerLaundryItems.AddAsync(dryCleanerItem);
            await _context.SaveChangesAsync();    
            return new Response<ItemDto>
            {
                Message = "LaundryItem added successfully",
                Success = true,
            };
        }

        public async Task<Response<ItemDto>> AddItem(ItemRequestDto request, IFormFile file)
        {
            var exist = await _unitOfWork.LaundryItem.ExistsAsync(x => x.Name.ToLower() == request.Name.ToLower());
            if (exist)
            {
                return new Response<ItemDto>
                {
                    Message = "LaundryItem already exists",
                    Success = false                 
                };
            };

            if (file != null)
            {
                request.ImageUrl = await _unitOfWork.FileUpload.UploadAsync(file);
            }

            var item = new LaundryItem
            {
                Name = request.Name,
                Price = request.Price,
                ImageUrl = request.ImageUrl
            };
            await _unitOfWork.LaundryItem.AddAsync(item);

            return new Response<ItemDto>
            {
                Message = "LaundryItem added",
                Success = true,
                Data = new ItemDto
                {
                    Id = item.Id,
                    Name = item.Name,
                    Price = item.Price,
                    ImageUrl = item.ImageUrl
                }
            };
        }

        public async Task<Response<List<ItemDto>>> GetItems(Guid dryCleanerUserid)
        {
            var sp = await _unitOfWork.DryCleaner.GetAsync(x => x.UserId == dryCleanerUserid);
            var items = await _context.DryCleanerLaundryItems.Include(x => x.LaundryItem).Where(x => x.DrycleanerId == sp.Id).ToListAsync();
            var itemDtos = items.Select(x => new ItemDto
            {
                Id = x.Id,
                Name = x.LaundryItem.Name,
                Price = x.Price,
                ImageUrl = x.LaundryItem.ImageUrl,
                ServiceProviderId = x.DrycleanerId
            }).ToList();

            return new Response<List<ItemDto>>
            {
                Message = "Items retrieved successfully",
                Success = true,
                Data = itemDtos
            };
        }

        public async Task<Response<List<ItemDto>>> GetItems()
        {
            var items = await _unitOfWork.LaundryItem.GetAllAsync();
            var itemsDto = items.Select(x => new ItemDto
            {
                Id = x.Id,
                Name = x.Name,
                Price = x.Price,
                ImageUrl = x.ImageUrl,
            }).ToList();
            return new Response<List<ItemDto>>
            {
                Message = "Items gotten",
                Success = true,
                Data = itemsDto
            };
        }
    }
}
