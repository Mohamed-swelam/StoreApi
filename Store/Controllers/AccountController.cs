using Microsoft.EntityFrameworkCore;
using Store.Models.DTOS.User;
using System.Security.Cryptography;

namespace Store.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AccountController : ControllerBase
{
    private readonly UserManager<ApplicationUser> userManager;
    private readonly SignInManager<ApplicationUser> signInManager;
    private readonly IConfiguration configuration;
    private readonly IUnitOfWork unitOfWork;

    public AccountController(UserManager<ApplicationUser> userManager
        ,SignInManager<ApplicationUser> signInManager,IConfiguration configuration,IUnitOfWork unitOfWork)
    {
        this.userManager = userManager;
        this.signInManager = signInManager;
        this.configuration = configuration;
        this.unitOfWork = unitOfWork;
    }

    [HttpPost("Register")]
    public async Task<ActionResult<GeneralResponse>> Register(RegisterDTO registerDTO)
    {
        GeneralResponse response = new GeneralResponse();
        if (ModelState.IsValid)
        {
             var userfromdb= await userManager.FindByEmailAsync(registerDTO.Email);
            if (userfromdb != null)
            {
                response.IsSuccess = false;
                response.Data = "this Email is exist, please change your email";
                return BadRequest(response);
            }

            ApplicationUser user = new ApplicationUser();
            user.Email = registerDTO.Email;
            user.UserName = registerDTO.UserName;
            user.EmailConfirmed = false;
            var result = await userManager.CreateAsync(user, registerDTO.Password);
            if (result.Succeeded)
            {

                response.IsSuccess = true;
                response.Data = "registration succeful , please check email to confirm your account";
                return response;
            }
            foreach (var item in result.Errors)
            {
                ModelState.AddModelError("Password", item.Description);
            }
        }
        response.IsSuccess = false;
        response.Data = ModelState;
        return response;

    }

    [HttpPost("Login")]
    public async Task<ActionResult> Login(LoginDTO loginDTO)
    {
        GeneralResponse response = new GeneralResponse();

        if (ModelState.IsValid)
        {
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
                    response.Data = "You are loged in";
                    
                    return Ok(new
                    {
                        response,
                        token,
                        refreshToken,
                        exipiration = DateTime.Now.AddHours(1)
                    });
                }
            }
            ModelState.AddModelError("", "The UserName Or Password is invalid");
        }
        response.IsSuccess = false;
        response.Data = ModelState;
        return NotFound(response);
    }
    [HttpPost("RefreshToken")]
    public async Task<IActionResult> RefreshToken(TokenRequest request)
    {
        var user =unitOfWork.User.Get(e=>e.refreshTokens.Any(t=>t.Token == request.RefreshToken));

        if(user == null)
            return Unauthorized();

        var refreshtoken = user.refreshTokens.Single(e=>e.Token ==  request.RefreshToken);
        if (!refreshtoken.IsActive)
            return Unauthorized();

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
        return Ok(tokenRequest);

    }

    [HttpPost("ChangePassword")]
    public async Task<ActionResult<GeneralResponse>> ChangePassword(ResetPasswordDto dto)
    {
        if (ModelState.IsValid)
        {
            var user = await userManager.FindByEmailAsync(dto.Email);
            if (user == null)
                return NotFound();

            var oldresult= await userManager.CheckPasswordAsync(user, dto.OldPassword);
            if(oldresult == false)
                return NotFound("the password not correct");
           
            var result= await userManager.ChangePasswordAsync(user,dto.OldPassword,dto.NewPassword);
            if (result.Succeeded)
                return Ok("The Password Change succesfuly");

            foreach (var item in result.Errors)
                ModelState.AddModelError("",item.Description);

        }
        
        return BadRequest(ModelState);
    
    }

    [HttpGet("LogOut")]
    public async Task<ActionResult> LogOut()
    {
        var UserId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

        var user = unitOfWork.User.Get(e => e.Id == UserId, IncludeProperites: "refreshTokens");
        if (user == null)
            return Unauthorized();

        foreach(var item in user.refreshTokens)
            item.Revoked = DateTime.UtcNow;

        unitOfWork.Save();

        await signInManager.SignOutAsync();
        GeneralResponse response = new GeneralResponse();
        response.IsSuccess = true;
        response.Data = "SignOut Succesfuly";
        return Ok(response);
    }
    [HttpPost("RevokeAllTokens")]
    public ActionResult RevokeAllTokens(TokenRequest tokenRequest)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
        var user = unitOfWork.User.Get(e=>e.Id == userId,IncludeProperites: "refreshTokens");
        if (user == null)
            return Unauthorized();

        foreach(var item in user.refreshTokens)
            item.Revoked = DateTime.UtcNow;
        unitOfWork.Save();

        return Ok("All token has been revoked");

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
    private string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }
}
