using CaptainTrackBackend.Application.DTO;
using CaptainTrackBackend.Application.DTO.ServiceProviders;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptainTrackBackend.Application.Abstraction.Interface.Services.ServiceProviders.DryCleaning
{
    public interface ILaundryPackageService
    {
        Task<Response<PackageDto>> Add(Guid dryCleanerUserid, PackageRequestDto request, IFormFile file);
        Task<Response<List<PackageDto>>> GetPackages(Guid dryCleanerUserid);
        Task<Response<List<PackageDto>>> GetPackages();
        Task<Response<PackageDto>> Add(PackageRequestDto request, IFormFile file);
    }
}
