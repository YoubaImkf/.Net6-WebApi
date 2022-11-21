using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;
using WebApiDemo.Dtos;
using WebDemo.Core.Hubs;
using WebDemo.Core.Interfaces;
using WebDemo.Core.RealTimeModels;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

//source : https://docs.microsoft.com/en-us/aspnet/core/tutorials/min-web-api?view=aspnetcore-6.0&tabs=visual-studio

namespace WebDemo.Api.Controllers.V1
{
    [Route("/api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    public class UsersController : ControllerBase
    {
        //source: https://code-maze.com/automapper-net-core/

        private readonly IUserService _userService; //interface 
        private readonly IHubContext<NotificationHub, INotificationHub> _hubContext;
                                     //Obliger 

        public UsersController(IUserService userService, IHubContext<NotificationHub, INotificationHub> hubContext)
        {
            _userService = userService;
            _hubContext = hubContext;
        }

        /// <summary> 
        /// Retourne tout les clients
        /// </summary>
        [HttpGet, Authorize(Roles ="Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/json")]
        public async Task<IActionResult> GetAsync()
        {
            return Ok(await _userService.GetAllAsync());
        }

        /// <summary> 
        /// Retourne un client en fonction de son id
        /// </summary>       
        /// <remarks>
        /// Sample value of message
        /// 
        ///     Hello World this is a text! :P
        ///     
        /// </remarks>
        // GET api/<UsersController>/5 READ{id}
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/json")]
        public async Task<UserDto> Get(int id)
        {
            return await _userService.GetUserAsync(id);
        }

        /// <summary> 
        ///  Surpprime un client en fonction de son id
        /// </summary>
        /// <param name="id"></param>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Delete(int id)
        {
            await _userService.DeleteUserAsync(id);
            return Ok();
        }

        /// <summary> 
        ///  Ajoute un client
        /// </summary>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Post([FromBody] UserAddOrUpdateDto userAddDto)
        {
            await _userService.AddUserAsync(userAddDto);
            return Ok();
        }

        /// <summary> 
        ///  Modifie un client en fonction de son id
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Put([FromBody] UserAddOrUpdateDto userDto, int id)
        {
            var notification = new Notification("An User has been update", DateTime.Now);
            var user = await _userService.FindUserByIdAsync(id);
            if (user == null)
                return NotFound();

            await _userService.UpdateUserAsync(user, userDto);

            // SignalR Notif 
            await _hubContext.Clients.All.ReceiveNotification(notification, userDto);

            return Ok();
        }

        /// <summary> 
        ///  Recupere les devices d'un client en fonction de son id
        /// </summary>
        //ambiguïté route (action=methode)
        [HttpGet("{id}/[action]")]
        public async Task<IActionResult> Devices(int id)
        {
            return Ok(await _userService.FindDevicesByUserIdAsync(id));
        }

    }
}
