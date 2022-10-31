using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using WebApiDemo.Core.Models;
using WebApiDemo.Dtos;
using WebDemo.Api.Helpers;
using WebDemo.Core.Interfaces;
using WebDemo.Core.Models;
using WebDemo.Infrastructure.Data;
using Xunit;

namespace UniTests
{

    public class UserServiceTests
    {
        private readonly IUserService _userService;
        private readonly IWebApiDbContext _context;
        private int _userId = 17;

        public UserServiceTests()
        {
            var serviceProvider = new ServiceCollection()
                .AddLogging()
                .AddAutoMapper(new Assembly[] { Assembly.GetAssembly(typeof(UserProfile)) })
                .AddDbContext<WebApiDbContext>(options => options.UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()))
                .AddScoped<IWebApiDbContext>(s => s.GetService<WebApiDbContext>())
                .AddMyServices()
                .BuildServiceProvider();

            _userService = serviceProvider.GetService<IUserService>();
            _context = serviceProvider.GetService<IWebApiDbContext>();
        }

        [Fact]
        public async Task Get_AllUserAsyncTest()
        {
            //Arrange
            var user = new User() { Id = 1 };
            _context.User.Add(user);
            await _context.SaveChangesAsync();

            //Act
            var result = await _userService.GetAllAsync();

            //Assert
            Assert.NotNull(result);
            Assert.Single(result); // == Assert.Equal(1, result.Count())
        }

        [Fact]
        public async Task Get_UserByIdAsyncTest()
        {
            //Arrange
            _context.User.Add(new User() { Id = _userId, FirstName = "Nathan", LastName = "Duaval" });
            await _context.SaveChangesAsync();

            //Act
            var OkResult = await _userService.GetUserAsync(_userId);

            //Assert
            Assert.NotNull(OkResult);
            Assert.Equal(_userId, OkResult.Id);
        }

        [Fact]
        public async Task Delete_UserAsyncTest_returnSameCount()
        {
            //Arrange
            var user1 = new User() { Id = 1, FirstName = "coucou", LastName = "cMoi" };
            var user2 = new User() { Id = 2, FirstName = "coucou", LastName = "cMoi" };
            var user3 = new User() { Id = 3, FirstName = "coucou", LastName = "cMoi" };

            _context.User.Add(user1);
            _context.User.Add(user2);
            _context.User.Add(user3);
            await _context.SaveChangesAsync();

            //Act
            await _userService.DeleteUserAsync(3);
            await _context.SaveChangesAsync();
            var result = await _userService.GetAllAsync();

            //Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task Add_UserAsyncTest()
        {
            //Arrange
            var user = new UserAddOrUpdateDto();

            //Act
            await _userService.AddUserAsync(user);
            await _context.SaveChangesAsync();

            //Asert
            Assert.NotNull(user);
            Assert.True(await _context.User.AnyAsync());
        }

        [Fact]
        public async Task Update_UserAsyncTest()
        {
            //Arrange
            var userToUpdate = new User();
            var userDto = new UserAddOrUpdateDto() { FirstName = "coucou", LastName = "cMoi" };
            
            //Act
            await _userService.UpdateUserAsync(userToUpdate, userDto);
            await _context.SaveChangesAsync();

            //Assert
            Assert.NotNull(userToUpdate);
            Assert.True(userToUpdate.FirstName == userDto.FirstName);
            Assert.True(userToUpdate.LastName == userDto.LastName);

        }

        [Fact]
        public async Task Get_DeviceByUserIdAsyncTest()
        {
            //Arrange
            List<Device> devices = new List<Device>()
            { 
                new Device() { Id = 1, ModelName = "SamsungS90", Type = "radio" },
                new Device() { Id = 2, ModelName = "SamsungS90", Type = "radio" }
            };

            var user = new User() { Id = _userId, FirstName = "coucou", LastName = "cMoi", Email = "mermoude@gmail.com", Devices = devices };

            //Act
            await _userService.FindDevicesByUserIdAsync(user.Id);

            //Assert
            Assert.True(user.Devices.Any());
        }


    }

}