using Microsoft.AspNetCore.Identity;
using Store.Models.DTOS.User;
using Store.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.DataAccess.Services
{
    public interface IAuthService
    {
        public Task<GeneralResponse> RegisterAsync(RegisterDTO registerDTO);
        public Task<GeneralResponse> LoginAsync(LoginDTO loginDTO); 
        public Task<TokenRequest> RefreshTokenAsync(TokenRequest request);
        public Task<GeneralResponse> ChangePasswordAsync(ResetPasswordDto dto);
        public Task<GeneralResponse> LogOutAsync(string userId);
        public GeneralResponse RevokeAllTokens(string userId);
    }
}
