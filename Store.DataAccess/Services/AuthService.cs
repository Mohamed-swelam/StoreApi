using Azure;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Store.DataAccess.Repositories;
using Store.DataAccess.Repositories.IRepositories;
using Store.Models.DTOS.User;
using Store.Models.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Store.DataAccess.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IUnitOfWork unitOfWork;
        private readonly IConfiguration configuration;
        private readonly SignInManager<ApplicationUser> signInManager;

        public AuthService(UserManager<ApplicationUser> userManager
            , IUnitOfWork unitOfWork, IConfiguration configuration
            , SignInManager<ApplicationUser> signInManager)
        {
            this.userManager = userManager;
            this.unitOfWork = unitOfWork;
            this.configuration = configuration;
            this.signInManager = signInManager;
        }

        public async Task<GeneralResponse> RegisterAsync(RegisterDTO registerDTO)
        {
            GeneralResponse response = new GeneralResponse();

            var userfromdb = await userManager.FindByEmailAsync(registerDTO.Email);
            if (userfromdb != null)
            {
                response.IsSuccess = false;
                response.Data = "This Email already exists, please choose a different email";
                return response;
            }

            ApplicationUser user = new ApplicationUser
            {
                Email = registerDTO.Email,
                UserName = registerDTO.UserName,
                EmailConfirmed = false 
            };

            var result = await userManager.CreateAsync(user, registerDTO.Password);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, "User");
                response.IsSuccess = true;
                response.Data = "registration successfuly";
                return response;
            }

            response.IsSuccess = false;
            response.Data = result.Errors.Select(e => e.Description).ToList();
            return response;
        }

        public async Task<GeneralResponse> LoginAsync(LoginDTO loginDTO)
        {
            GeneralResponse response = new GeneralResponse();
            ApplicationUser? user = await userManager.FindByEmailAsync(loginDTO.Email);

            if (user != null)
            {
                bool result = await userManager.CheckPasswordAsync(user, loginDTO.Password);
                if (result)
                {

                    var token = await GenerateJwtToken(user);
                    var refreshToken = GenerateRefreshToken();

                    user.refreshTokens.Add(new RefreshToken()
                    {
                        Token = refreshToken,
                        Expires = DateTime.UtcNow.AddDays(3),
                        Created = DateTime.UtcNow
                    });
                    unitOfWork.Save();
                    response.IsSuccess = true;
                    response.Data = new
                    {
                        token,
                        refreshToken,
                        exipiration = DateTime.Now.AddHours(1)
                    };
                    return response;
                }
            }
            response.IsSuccess = false;
            response.Data = "Invalid email or password";
            return response;
        }

        public async Task<TokenRequest> RefreshTokenAsync(TokenRequest request)
        {
            var user = unitOfWork.User.Get(e => e.refreshTokens.Any(t => t.Token == request.RefreshToken));

            if (user == null)
                throw new UnauthorizedAccessException("Invalid refresh token.");

            var refreshtoken = user.refreshTokens.Single(e => e.Token == request.RefreshToken);
            if (!refreshtoken.IsActive)
                throw new UnauthorizedAccessException("Refresh token has expired.");

            var newjwttoken = await GenerateJwtToken(user);
            var newRefreshToken = GenerateRefreshToken();
            refreshtoken.Revoked = DateTime.UtcNow;


            user.refreshTokens.Add(new RefreshToken
            {
                Token = newRefreshToken,
                Created = DateTime.UtcNow,
                Expires = DateTime.UtcNow.AddDays(3),
            });

            unitOfWork.Save();

            TokenRequest tokenRequest = new TokenRequest()
            {
                Token = newjwttoken,
                RefreshToken = newRefreshToken,
            };
            return tokenRequest;
        }

        public async Task<GeneralResponse> ChangePasswordAsync(ResetPasswordDto dto)
        {
            GeneralResponse response = new GeneralResponse();

            var user = await userManager.FindByEmailAsync(dto.Email);
            if (user == null)
            {
                response.IsSuccess = false;
                response.Data = "User not found";
                return response;
            }

            var oldresult = await userManager.CheckPasswordAsync(user, dto.OldPassword);
            if (oldresult == false)
            {
                response.IsSuccess = false;
                response.Data = "The old password is incorrect";
                return response;
            }
            var result = await userManager.ChangePasswordAsync(user, dto.OldPassword, dto.NewPassword);
            if (result.Succeeded)
            {
                response.IsSuccess = true;
                response.Data = "Password changed successfully";
                return response;
            }
            response.IsSuccess = false;
            response.Data = result.Errors.Select(e => e.Description).ToList();
            return response;
        }

        public async Task<GeneralResponse> LogOutAsync(string userId)
        {
            GeneralResponse response = new GeneralResponse();

            var user = unitOfWork.User.Get(e => e.Id == userId, IncludeProperites: "refreshTokens");
            if (user == null)
            {
                response.IsSuccess = false;
                response.Data = "Unauthorized";
                return response;
            }

            foreach (var item in user.refreshTokens)
            {
                item.Revoked = DateTime.UtcNow;
            }

            unitOfWork.Save();

            await signInManager.SignOutAsync();

            response.IsSuccess = true;
            response.Data = "SignOut Successfully";
            return response;
        }

        public  GeneralResponse RevokeAllTokens(string userId)
        {
            GeneralResponse response = new GeneralResponse();

            var user = unitOfWork.User.Get(e => e.Id == userId, IncludeProperites: "refreshTokens");
            if (user == null)
            {
                response.IsSuccess = false;
                response.Data = "Unauthorized";
                return response;
            }

            foreach (var item in user.refreshTokens)
            {
                item.Revoked = DateTime.UtcNow;
            }

            unitOfWork.Save();

            response.IsSuccess = true;
            response.Data = "All tokens have been revoked";
            return response;
        }

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }

        private async Task<string> GenerateJwtToken(ApplicationUser user)
        {
            List<Claim> userclaims = new();
            userclaims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
            userclaims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id));
            userclaims.Add(new Claim(ClaimTypes.Name, user.UserName));
            var Roles = await userManager.GetRolesAsync(user);
            foreach (var Role in Roles)
            {
                userclaims.Add(new Claim(ClaimTypes.Role, Role));
            }

            SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:SecritKey"]));
            SigningCredentials credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            JwtSecurityToken token = new JwtSecurityToken(
               issuer: configuration["JWT:IssuerIP"],
               audience: configuration["JWT:AudienceIP"],
               expires: DateTime.Now.AddHours(1),
               claims: userclaims,
               signingCredentials: credentials
            );

            string result = new JwtSecurityTokenHandler().WriteToken(token);
            return result;
        }

       
    }
}
