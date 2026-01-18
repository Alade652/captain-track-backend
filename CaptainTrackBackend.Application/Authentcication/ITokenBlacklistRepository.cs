using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaptainTrackBackend.Application.Abstraction.Interface.Repository;

namespace CaptainTrackBackend.Application.Authentcication
{
    public interface ITokenBlacklistRepository 
    {
        Task AddAsync(string token, DateTime? expiryDate);
        Task<bool> ExistsAsync(string token);
    }
}
