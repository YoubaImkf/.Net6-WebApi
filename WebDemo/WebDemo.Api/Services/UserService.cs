using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApiDemo.Core.Models;
using WebApiDemo.Dtos;
using WebDemo.Api.Controllers.V2;
using WebDemo.Api.Options;
using WebDemo.Core.Interfaces;

namespace WebDemo.Api.Services
{
    public class UserService : IUserService
    {
        //Méthodes q'uon utiliseras dans le controller ( _xxxx )
        //source : https://jasonwatmore.com/fr/post/2022/03/15/net-6-exemple-et-didacticiel-d-api-crud#user-service-cs

        private readonly IWebApiDbContext _webApiDbContext;
        private readonly IMapper _mapper;
        private readonly PositionOption _options;
        private readonly PositionOption _snapshotOptions;
        private readonly ILogger<UsersVtwoController> _logger;

        public UserService(IWebApiDbContext webApiDbContext, IMapper mapper, IOptions<PositionOption> options, IOptionsSnapshot<PositionOption> snapshotOptions, ILogger<UsersVtwoController> logger)
        {
            _webApiDbContext = webApiDbContext;
            _mapper = mapper;
            _options = options.Value;
            _snapshotOptions = snapshotOptions.Value;
            _logger = logger;
        }
 
        public async Task<IEnumerable<UserDto>> GetAllAsync()
        {          
            Console.WriteLine("_options. - name :" + _options.Name + "title :" + _options.Title);
            Console.WriteLine("_snapshotOptions - name :" + _snapshotOptions.Name + "title :" + _snapshotOptions.Title);
            var users = _mapper.Map<IEnumerable<UserDto>>(await _webApiDbContext.User.ToListAsync());
            return users;      
        }
/*
        public IList<UserDto> GetAll()
        {
            var users = _mapper.Map<IList<UserDto>>(_webApiDbContext.User.ToList());
            return users;
        }
*/

        public async Task<IEnumerable<UserDto>> FindDevicesByUserIdAsync(int id)
        {
            //source : https://learn.microsoft.com/en-us/ef/ef6/fundamentals/relationships
            /*var device = _mapper.Map<IEnumerable<DeviceDto>>(_webApiDbContext.Device.First(d => d.UserId == idUser));*/
            var device = _webApiDbContext.User
                                         .Include(d => d.Devices) //jointure
                                         .Where(u => u.Id == id);
            // request form:
            /* SELECT [u].[Id], [u].[Email], [u].[FirstName], [u].[LastName], [d].[Id], [d].[Marque], [d].[ModelName], [d].[Type], [d].[UserId]
              FROM[User] AS[u]
              LEFT JOIN[Device] AS[d] ON[u].[Id] = [d].[UserId]
              WHERE[u].[Id] = 7
              ORDER BY[u].[Id]   */

            if (device == null) throw new KeyNotFoundException("No Devices");
            return _mapper.Map<IEnumerable<UserDto>>(await device.ToListAsync());
        }

        public async Task<UserDto> GetUserAsync(int id)
        {
            /*_logger.LogInformation($"item with id '{id}' Found");*/
            var user = _mapper.Map<UserDto>(await FindUserByIdAsync(id));
            return user;
        }

        public async Task DeleteUserAsync(int id)
        {
            var user = await FindUserByIdAsync(id);
            if (user == null)
            {
                _logger.LogWarning($"item with id '{id}' no found");
                throw new KeyNotFoundException("User not found");
            }
            _webApiDbContext.User.Remove(user);
            await _webApiDbContext.SaveChangesAsync();

        }

        public async Task AddUserAsync(UserAddOrUpdateDto userAddDto)
        {      
            var user = _mapper.Map<User>(userAddDto);
            //if user already exist najoute pas ...
            if (_webApiDbContext.User.Any(u => u.Email == user.Email))
                throw new KeyNotFoundException("Email already exists");

             var a = await _webApiDbContext.User.AddAsync(user);

             var s = await _webApiDbContext.SaveChangesAsync();
        }
        public void AddUser(UserAddOrUpdateDto userAddDto)
        {
            var user = _mapper.Map<User>(userAddDto);

            var s =  _webApiDbContext.User.Add(user);
            var l = _webApiDbContext.SaveChanges();
        }

        public async Task UpdateUserAsync(User user, UserAddOrUpdateDto userDto)
        {
            //source: https://www.youtube.com/watch?v=5Gns9ECepR8&ab_channel=KalpeshSatasiya
            /*var user = FindUserById(id);

            user.FirstName = firstName;
            user.LastName = lastName;
            user.Email = email;
            */
            _mapper.Map(userDto, user); //remplace les valeurs
            _webApiDbContext.User.Update(user);
            await _webApiDbContext.SaveChangesAsync();
        }



        //================== PRIVATE tjr à la fin ==================
        public async Task<User> FindUserByIdAsync(int id)
        {
            var user = await _webApiDbContext.User.Where(e => e.Id == id).FirstOrDefaultAsync();
            //var user = await _webApiDbContext.User.FindAsync(id);
            if (user == null) throw new KeyNotFoundException("User not found");
            return user;
        }



    }
}
