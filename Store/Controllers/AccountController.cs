namespace Store.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AccountController : ControllerBase
{
    private readonly UserManager<ApplicationUser> userManager;
    private readonly SignInManager<ApplicationUser> signInManager;
    private readonly IConfiguration configuration;

    public AccountController(UserManager<ApplicationUser> userManager
        ,SignInManager<ApplicationUser> signInManager,IConfiguration configuration)
    {
        this.userManager = userManager;
        this.signInManager = signInManager;
        this.configuration = configuration;
    }

    [HttpPost("Register")]
    public async Task<ActionResult<GeneralResponse>> Register(RegisterDTO registerDTO)
    {
        GeneralResponse response = new GeneralResponse();
        if (ModelState.IsValid)
        {
            ApplicationUser user = new ApplicationUser();
            user.Email = registerDTO.Email;
            user.UserName = registerDTO.UserName;
            var result = await userManager.CreateAsync(user, registerDTO.Password);
            if (result.Succeeded)
            {
                response.IsSuccess = true;
                response.Data = "Created";
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
            ApplicationUser? user = await userManager.FindByNameAsync(loginDTO.UserName);
            if (user != null)
            {
                bool result = await userManager.CheckPasswordAsync(user, loginDTO.Password);
                if (result)
                {
                    //Generate token

                    List<Claim> userclaims = new();
                    userclaims.Add(new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()));
                    userclaims.Add(new Claim(ClaimTypes.NameIdentifier,user.Id));
                    userclaims.Add(new Claim(ClaimTypes.Name,user.UserName));
                    var Roles= await userManager.GetRolesAsync(user);
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
                    response.IsSuccess = true;
                    response.Data = "You are loged in";
                    
                    return Ok(new
                    {
                        response,
                        token = new JwtSecurityTokenHandler().WriteToken(token),
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
    [HttpGet("LogOut")]
    public async Task<ActionResult> LogOut()
    {
        await signInManager.SignOutAsync();
        GeneralResponse response = new GeneralResponse();
        response.IsSuccess = true;
        response.Data = "SignOut Succesfuly";
        return Ok(response);
    }
}
