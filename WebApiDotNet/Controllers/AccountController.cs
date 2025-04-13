using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebApiDotNet.Dtos;
using WebApiDotNet.Models;

namespace WebApiDotNet.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly Microsoft.AspNetCore.Identity.UserManager <ApplicationUser> userManager;
        private readonly IMapper mapper;
        private readonly IConfiguration config;

        public AccountController(UserManager<ApplicationUser> userManager, IMapper mapper, IConfiguration config)
        {
            this.userManager = userManager;
            this.mapper = mapper;
            this.config = config;
        }
        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromForm] RegisterDTO registerDTO)
        {
            var existingUser = await userManager.FindByNameAsync(registerDTO.Name);
            if (existingUser != null)
            {
                return BadRequest("Name is already taken. Please choose a different name.");
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = mapper.Map<ApplicationUser>(registerDTO);
            var result = await userManager.CreateAsync(user, registerDTO.Password);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok("User registered successfully.");
        }
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromForm] LoginDTO loginDTO)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser user = await userManager.FindByEmailAsync(loginDTO.Email);
                if (user != null)
                {
                    bool found = await userManager.CheckPasswordAsync(user, loginDTO.Password);
                    if (found)
                    {
                        //generate token<==

                        List<Claim> UserClaims = new List<Claim>();

                        //Token Genrated id change (JWT Predefind Claims )
                        UserClaims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
                        UserClaims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id));
                        UserClaims.Add(new Claim(ClaimTypes.Name, user.UserName));

                        var UserRoles = await userManager.GetRolesAsync(user);

                        foreach (var roleNAme in UserRoles)
                        {
                            UserClaims.Add(new Claim(ClaimTypes.Role, roleNAme));
                        }

                        var SignInKey =
                            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                                config["JWT:SecritKey"]));

                        SigningCredentials signingCred =
                            new SigningCredentials
                            (SignInKey, SecurityAlgorithms.HmacSha256);

                        //design token
                        JwtSecurityToken mytoken = new JwtSecurityToken(
                            audience: config["JWT:AudienceIP"],
                            issuer: config["JWT:IssuerIP"],
                            expires: DateTime.Now.AddHours(1),
                            claims: UserClaims,
                            signingCredentials: signingCred

                            );
                       // generate token response

                        return Ok(new
                        {
                            token = new JwtSecurityTokenHandler().WriteToken(mytoken),
                            expiration = DateTime.Now.AddHours(1)//mytoken.ValidTo
                            //
                        });
                    }
                }
                ModelState.AddModelError("Email", "Email or Password is not Correct");
            }
            return BadRequest(ModelState);
        }
    }
}
