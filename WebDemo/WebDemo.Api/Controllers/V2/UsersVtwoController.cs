using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApiDemo.Dtos;
using WebDemo.Core.Interfaces;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

//source : https://docs.microsoft.com/en-us/aspnet/core/tutorials/min-web-api?view=aspnetcore-6.0&tabs=visual-studio

namespace WebDemo.Api.Controllers.V2
{
    [Route("/api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("2.0")]
    public class UsersVtwoController : ControllerBase
    {
        //source: https://code-maze.com/automapper-net-core/

        private readonly IUserService _userService; //interface 
        private readonly ILogger<UsersVtwoController> _logger;
        
        public UsersVtwoController(IUserService userService, ILogger<UsersVtwoController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        //We ned to enable the generation of XML comments
        /// <summary> 
        /// Retourne tout les clients
        /// </summary>
        /// <description> salut </description>
        [HttpGet, Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesDefaultResponseType]
        /*      [ApiConventionMethod(typeof(DefaultApiConventions),nameof(DefaultApiConventions.Get))]*/
        public async Task<IActionResult> GetAsync()
        {

            _logger.LogInformation("Web Api is runninggggg...");
            return Ok(await _userService.GetAllAsync());
        }


        /// <summary> 
        ///  Ajoute un client
        /// </summary>
        [HttpPost /*,Authorize(Roles = "admin")*/] 
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Post))]
        public async Task<IActionResult> Post([FromBody] UserAddOrUpdateDto userAddDto)
        {
            await _userService.AddUserAsync(userAddDto);
            return Ok();
        }
    }
}
