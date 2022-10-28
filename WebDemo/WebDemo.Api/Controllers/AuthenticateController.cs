using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using WebDemo.Core.Dtos.Auth;
using WebDemo.Core.RealTimeModels;
using WebDemo.Infrastructure.Data;


namespace WebDemo.Api.Controllers
{   //source: https://www.c-sharpcorner.com/article/jwt-authentication-and-authorization-in-net-6-0-with-identity-framework/
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticateController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly WebApiDbContext _userContext;
       

        public AuthenticateController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration, WebApiDbContext userContext)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _userContext = userContext;
        }

        /// <summary>
        /// Permet de se connecter (seulement si vous avez un compte User)
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (!_userContext.User.Any(u => u.Email == user.Email)) return StatusCode(StatusCodes.Status500InternalServerError, new Response
            {
                Status = "Error",
                Message = "User not Found :( "
            });

            if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
            {
                var userRoles = await _userManager.GetRolesAsync(user);

                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Email, model.Email),
                    new Claim(Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };

                foreach (var userRole in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                }

                var token = GetToken(authClaims);


                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    expiration = token.ValidTo
                });
            }
            return Unauthorized("Try Again");
        }


        /// <summary>
        /// Permet de s'enregistrer [role= USER]
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        /// <response code="200">User created</response>
        /// <response code="400">Product has missing/invalid values</response>
        /// <response code="500">Oops! Can't create your product right now</response>
        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            var userExist = await _userManager.FindByNameAsync(model.Username);
            var emailExist = await _userManager.FindByNameAsync(model.Email);

            if (userExist != null || emailExist != null)
                return StatusCode(StatusCodes.Status500InternalServerError, new Response
                {
                    Status = "Error",
                    Message = "User already Exist!"
                });

            IdentityUser user = new()
            {
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.Username
            };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
                return StatusCode(StatusCodes.Status500InternalServerError, new Response
                {
                    Status = "Error",
                    Message = "User creation failed! Please check user details and try again."
                });


            if (!await _roleManager.RoleExistsAsync(UserRoles.User))
                await _roleManager.CreateAsync(new IdentityRole(UserRoles.User));


            if (await _roleManager.RoleExistsAsync(UserRoles.User))
            {
                await _userManager.AddToRoleAsync(user, UserRoles.User);
            }

            return Ok(new Response { Status = "Success", Message = "User created successfully!" });
        }


        /// <summary>
        /// Permet de s'enregistrer [role= ADMIN]
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Register-Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RegisterAdmin([FromBody] RegisterModel model)
        {
            var userExist = await _userManager.FindByNameAsync(model.Username);
            var emailExist = await _userManager.FindByNameAsync(model.Email);

            if (userExist != null || emailExist != null)
                return StatusCode(StatusCodes.Status500InternalServerError, new Response
                {
                    Status = "Error",
                    Message = "User already Exist!"
                });

            IdentityUser user = new()
            {
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.Username
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
                return StatusCode(StatusCodes.Status500InternalServerError, new Response
                {
                    Status = "Error",
                    Message = "User creation failed! Please check user details and try again."
                });

            if (!await _roleManager.RoleExistsAsync(UserRoles.Admin))
                await _roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));

            if (await _roleManager.RoleExistsAsync(UserRoles.Admin))
            {
                await _userManager.AddToRoleAsync(user, UserRoles.Admin);
            }


            return Ok(new Response { Status = "Succes", Message = "User created successfully!" });
        }

        /// <summary>
        /// Permet de supprimer utilisateur (quel que soit son role)
        /// </summary>
        /// <param name="model"></param>
        /// <description> </description>
        /// <returns></returns>
        [HttpDelete, Authorize(Roles = "Admin")]
        [Route("Delete-User")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteUser(DeleteDtoAuth model)
        {
            var userId = await _userManager.FindByIdAsync(model.Id); 
            if (userId == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response
                {
                    Status = "Error",
                    Message = "Delete failed"
                });
            }

            await _userManager.DeleteAsync(userId);
            return Ok(new Response { Status = "Success", Message = " User Deleted " });

        }

/*        public Notification Notif()
        {
            //signalR
            Notification notif = new Notification("Vous vous etes bien connecter", DateTime.Now);
            _hubContext.Clients.All.SendAsync("NotificationReceived", notif);
            return ""
        }*/

        private JwtSecurityToken GetToken(List<Claim> authClaims)
        {
            var authSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_configuration["JWT:Key"]));

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"], //emetteur
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.Now.AddHours(1),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );

            return token;
        }


    }
}
