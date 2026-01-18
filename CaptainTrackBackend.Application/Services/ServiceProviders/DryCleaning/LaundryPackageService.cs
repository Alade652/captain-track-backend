using CaptainTrackBackend.Application.Abstraction.Interface.Repository;
using CaptainTrackBackend.Application.Abstraction.Interface.Services.ServiceProviders.DryCleaning;
using CaptainTrackBackend.Application.DTO;
using CaptainTrackBackend.Application.DTO.ServiceProviders;
using CaptainTrackBackend.Application.DTO.ServiceProviders.DryCleaning;
using CaptainTrackBackend.Domain.Entities.ServiceProviders.DryCleaning;
using CaptainTrackBackend.Domain.Enum;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptainTrackBackend.Application.Services.ServiceProviders.DryCleaning
{
    public class LaundryPackageService : ILaundryPackageService
    {
        private readonly IUnitofWork _unitOfWork;
        public LaundryPackageService(IUnitofWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<Response<PackageDto>> Add(Guid dryCleanerUserid, PackageRequestDto request, IFormFile file)
        {
            var dryCleaner = await _unitOfWork.DryCleaner.GetAsync(x => x.UserId == dryCleanerUserid);
            var package = await _unitOfWork.LaundryPackage.GetAllByExpression(x => x.DryCleanerId == dryCleaner.Id);
            var exist = package.Any(x => x.Name.ToLower() == request.Name.ToLower());
            if (exist)
            {
                return new Response<PackageDto>
                {
                    Message = "Package already exists",
                    Success = false
                };
            }

            if (file != null)
            {
                request.ImageUrl = await _unitOfWork.FileUpload.UploadAsync(file);
            }
            var laundryPackage = new LaundryPackage
            {
                DryCleanerId = dryCleaner.Id,
                Name = request.Name,
                Description = request.Description,
                ImageUrl = request.ImageUrl,
                ExtraCharge = request.ExtraCharge
            };
            await _unitOfWork.LaundryPackage.AddAsync(laundryPackage);
            return new Response<PackageDto>
            {
                Message = "Package added successfully",
                Success = true,
                Data = new PackageDto
                {
                    Id = laundryPackage.Id,
                    Name = laundryPackage.Name,
                    Description = laundryPackage.Description,
                    ImageUrl = laundryPackage.ImageUrl,
                    ExtraCharge = laundryPackage.ExtraCharge
                }
            };
        }

        public async Task<Response<PackageDto>> Add(PackageRequestDto request, IFormFile file)
        {
            var package = await _unitOfWork.LaundryPackage.GetAllByExpression(x => x.DryCleanerId == null);
            var exist = package.Any(x => x.Name.ToLower() == request.Name.ToLower());
            if (exist)
            {
                return new Response<PackageDto>
                {
                    Message = "Package already exists",
                    Success = false
                };
            }

            if (file != null)
            {
                request.ImageUrl = await _unitOfWork.FileUpload.UploadAsync(file);
            }
            var laundryPackage = new LaundryPackage
            {
                Name = request.Name,
                Description = request.Description,
                ImageUrl = request.ImageUrl,
                ExtraCharge = request.ExtraCharge,
                ForFreelance = true
            };
            await _unitOfWork.LaundryPackage.AddAsync(laundryPackage);
            return new Response<PackageDto>
            {
                Message = "Package added successfully",
                Success = true,
                Data = new PackageDto
                {
                    Id = laundryPackage.Id,
                    Name = laundryPackage.Name,
                    Description = laundryPackage.Description,
                    ImageUrl = laundryPackage.ImageUrl,
                    ExtraCharge = laundryPackage.ExtraCharge
                }
            };  
        }

        public async Task<Response<List<PackageDto>>> GetPackages(Guid dryCleanerUserid)
        {
            var dryCleaner = await _unitOfWork.DryCleaner.GetAsync(x => x.UserId == dryCleanerUserid);
            var packages = await _unitOfWork.LaundryPackage.GetAllByExpression(x => x.DryCleanerId == dryCleaner.Id);
            var packageDtos = packages.Select(x => new PackageDto
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description,
                ImageUrl = x.ImageUrl,
                spId = x.DryCleanerId,
                ExtraCharge = x.ExtraCharge
            }).ToList();

            return new Response<List<PackageDto>>
            {
                Message = "Packages retrieved successfully",
                Success = true,
                Data = packageDtos
            };
        }

        public async Task<Response<List<PackageDto>>> GetPackages()
        {
            var response = new Response<List<PackageDto>>();
            var packages = await _unitOfWork.LaundryPackage.GetAllByExpression(x => x.DryCleanerId == null);
            if(packages == null)
            {
                response.Success = false;
                response.Message = "No packages";
                return response;
            }
            response.Data = packages.Select(x => new PackageDto
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description,
                ImageUrl = x.ImageUrl,
                spId = x.DryCleanerId,
                ExtraCharge = x.ExtraCharge
            }).ToList();
            response.Success = true;
            response.Message = "Packages gotten";
            return response;
        }
    }
}
