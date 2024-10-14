using Microsoft.EntityFrameworkCore;
using Store.DataAccess.Services;
using Store.Models.DTOS.User;
using System.Security.Cryptography;

namespace Store.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AccountController : ControllerBase
{
    private readonly IAuthService authService;

    public AccountController(IAuthService authService)
    {
        this.authService = authService;
    }

    [HttpPost("Register")]
    public async Task<ActionResult<GeneralResponse>> Register(RegisterDTO registerDTO)
    {
        GeneralResponse response = new GeneralResponse();
        if (ModelState.IsValid)
        {
            response = await authService.RegisterAsync(registerDTO);

            return response;
        }
        response.IsSuccess = false;
        response.Data = ModelState;
        return response;
    }

    [HttpPost("Login")]
    public async Task<ActionResult> Login(LoginDTO loginDTO)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        GeneralResponse response = await authService.LoginAsync(loginDTO);
        return Ok(response);
    }

    [HttpPost("RefreshToken")]
    public async Task<IActionResult> RefreshToken(TokenRequest request)
    {
        try
        {
            var result = await authService.RefreshTokenAsync(request);
            return Ok(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(ex.Message);
        }

    }

    [HttpPost("ChangePassword")]
    public async Task<ActionResult<GeneralResponse>> ChangePassword(ResetPasswordDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var response = await authService.ChangePasswordAsync(dto);

        if (response.IsSuccess)
        {
            return Ok(response.Data);
        }

        return BadRequest(response.Data);
    }

    [HttpGet("LogOut")]
    public async Task<ActionResult> LogOut()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
        {
            return Unauthorized();
        }

        var response = await authService.LogOutAsync(userId);

        if (response.IsSuccess)
        {
            return Ok(response);
        }

        return Unauthorized(response.Data);
    }
    [HttpPost("RevokeAllTokens")]
    public ActionResult RevokeAllTokens(TokenRequest tokenRequest)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
        {
            return Unauthorized();
        }

        var response = authService.RevokeAllTokens(userId);

        if (response.IsSuccess)
        {
            return Ok(response.Data);
        }

        return Unauthorized(response.Data);
    }  
}
