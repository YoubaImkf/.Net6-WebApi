using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Moq.EntityFrameworkCore;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebApiDemo.Core.Models;
using WebApiDemo.Dtos;
using WebDemo.Api.Controllers.V2;
using WebDemo.Api.Helpers;
using WebDemo.Api.Options;
using WebDemo.Api.Services;
using WebDemo.Core.Interfaces;
using WebDemo.Core.Models;
using Xunit;


namespace UniTests
{
    public class testController
    {
        /** // C’est quoi un mock ? : https://www.fierdecoder.fr/2015/11/mock-ou-pas-mock/
         * Mock= décrit lors de l’exécution quand adaptée au contexte du test. 
         * Seetup "on te demande ça tu fais ça."
         * !On utilise alors les mocks seulement dans le cas où nous ne disposons pas
         * d’objet adapté au cadre du test!
        **/

        private int _userId = 11;
        private string _userName = "Alex";

        #region [ctor] Moq AutoMapper
        private static IMapper _mapper;
        public testController()
        {
            if (_mapper == null)
            {
                var mappingConfig = new MapperConfiguration(cfg =>
                {
                    cfg.AddProfile(new UserProfile());
                });
                IMapper mapper = mappingConfig.CreateMapper();
                _mapper = mapper;
            }
        }
        #endregion

        [Fact]
        [Description(
            "Nous faisont un Uni Test a l'aide de Moq." +
            "Lors de l'appel la fonction GetAllAsync, celle ci doit nous retourner tout les utilisateurs de la table User.")]
        public async Task GetAll_Async_Function_With_ReturnUsers()
        {
            //Arrange
            #region 🚹UserList
            var data = new List<User>
            {
                new User { Id = _userId, FirstName = _userName },
                new User { Id = 12, FirstName = "Zouhir" },
                new User { Id = 13, FirstName = "Youba" },
            }.AsQueryable();
            #endregion

            #region Moq DbContext(provider?)
            //source:https://learn.microsoft.com/fr-fr/ef/ef6/fundamentals/testing/mocking?redirectedfrom=MSDN
            var mockSet = new Mock<DbSet<User>>();
            mockSet.As<IDbAsyncEnumerable<User>>()
              .Setup(m => m.GetAsyncEnumerator())
              .Returns(new TestDbAsyncEnumerator<User>(data.GetEnumerator())); 
            
            mockSet.As<IQueryable<User>>()
               .Setup(m => m.Provider)
               .Returns(new TestDbAsyncQueryProvider<User>(data.Provider));

            mockSet.As<IQueryable<User>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<User>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<User>>().Setup(m => m.GetEnumerator()).Returns(() => data.GetEnumerator());

            var mockContext = new Mock<IWebApiDbContext>();
            mockContext.Setup(m => m.User).ReturnsDbSet(mockSet.Object);    
            #endregion

            #region Moq IOtion
            PositionOption iOption = new PositionOption() { Title = "Option1" };
            var mockOption = new Mock<IOptions<PositionOption>>();
            mockOption.Setup(ap => ap.Value).Returns(iOption);

            PositionOption iSnap = new PositionOption() { Title = "Option2" };
            var mockSnap = new Mock<IOptionsSnapshot<PositionOption>>();
            mockSnap.Setup(ap => ap.Value).Returns(iSnap);
            #endregion

            #region Service
            var service = new UserService(
                mockContext.Object,
               _mapper,
                mockOption.Object,
                mockSnap.Object,
                new Mock<ILogger<UsersVtwoController>>().Object);
            #endregion

            //Act
            var users =  await service.GetAllAsync();

            //Assert
            Assert.NotNull(users);
            Assert.Equal(3, users.Count());
            Assert.Equal(_userName, users.ElementAt(0).FirstName);
        }

        [Fact]
        public async Task GetDevicesAsync_ByUserId_Function_With_ReturnDevices()
        {
            //Arrange
            #region 🚹UserList
            List<Device> devices = new List<Device>()
            {
                new Device() { Id = 1, ModelName = "SamsungS90", Type = "radio" },
                new Device() { Id = 2, ModelName = "SamsungS90", Type = "radio" }
            };

            var data = new List<User>
            {
                new User { Id = _userId, FirstName = _userName, Devices = devices },
            }.AsQueryable();
            #endregion

            #region Moq DbContext(provider?)
            var mockSet = new Mock<DbSet<User>>();
            mockSet.As<IDbAsyncEnumerable<User>>()
              .Setup(m => m.GetAsyncEnumerator())
              .Returns(new TestDbAsyncEnumerator<User>(data.GetEnumerator()));

            mockSet.As<IQueryable<User>>()
               .Setup(m => m.Provider)
               .Returns(new TestDbAsyncQueryProvider<User>(data.Provider));

            mockSet.As<IQueryable<User>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<User>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<User>>().Setup(m => m.GetEnumerator()).Returns(() => data.GetEnumerator());

            var mockContext = new Mock<IWebApiDbContext>();
            mockContext.Setup(m => m.User).ReturnsDbSet(mockSet.Object);
            #endregion

            #region Moq IOtion
            PositionOption iOption = new PositionOption() { Title = "Option1" };
            var mockOption = new Mock<IOptions<PositionOption>>();
            mockOption.Setup(ap => ap.Value).Returns(iOption);

            PositionOption iSnap = new PositionOption() { Title = "Option2" };
            var mockSnap = new Mock<IOptionsSnapshot<PositionOption>>();
            mockSnap.Setup(ap => ap.Value).Returns(iSnap);
            #endregion

            #region Service
            var service = new UserService(
                mockContext.Object,
               _mapper,
                mockOption.Object,
                mockSnap.Object,
                new Mock<ILogger<UsersVtwoController>>().Object);
            #endregion

            //Act
            var user = await service.FindDevicesByUserIdAsync(_userId);

            //Assert
            Assert.Equal(_userId, user.ElementAt(0).Id);
            Assert.True(user.ElementAt(0).Devices.Any());
        }

        [Fact]
        public async Task GetUserdAsync_ById_Function_With_ReturnUser()
        {
            //Arrange
            #region 🚹UserList
            var data = new List<User>
            {
                new User { Id = _userId, FirstName = _userName },
            }.AsQueryable();
            #endregion

            #region Moq DbContext(provider?)
            var mockSet = new Mock<DbSet<User>>();
            mockSet.As<IDbAsyncEnumerable<User>>();


            mockSet.As<IQueryable<User>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<User>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<User>>().Setup(m => m.GetEnumerator()).Returns(() => data.GetEnumerator());

            var mockContext = new Mock<IWebApiDbContext>();
            mockContext.Setup(m => m.User).ReturnsDbSet(mockSet.Object);

            #endregion

            #region Moq IOtion
            PositionOption iOption = new PositionOption() { Title = "Option1" };
            var mockOption = new Mock<IOptions<PositionOption>>();
            mockOption.Setup(ap => ap.Value).Returns(iOption);

            PositionOption iSnap = new PositionOption() { Title = "Option2" };
            var mockSnap = new Mock<IOptionsSnapshot<PositionOption>>();
            mockSnap.Setup(ap => ap.Value).Returns(iSnap);
            #endregion

            #region Service
            var service = new UserService(
                mockContext.Object,
               _mapper,
                mockOption.Object,
                mockSnap.Object,
                new Mock<ILogger<UsersVtwoController>>().Object);
            #endregion

            //Act
            var user = await service.GetUserAsync(_userId); //null           

            //Assert
            Assert.Equal(_userName, user.FirstName);
        }

        [Fact]
        public async Task DeleteUserByIdAsync_Function_With_ReturnVoid()
        {
            //Arrange
            #region 🚹UserList       
            var data = new List<User>
            {
                new User { Id = _userId, FirstName = _userName},
                new User { Id = 34, FirstName = "marc"},
            }.AsQueryable();

            #endregion

            #region Moq DbContext(provider?)
            var mockSet = new Mock<DbSet<User>>();
            mockSet.As<IDbAsyncEnumerable<User>>()
              .Setup(m => m.GetAsyncEnumerator())
              .Returns(new TestDbAsyncEnumerator<User>(data.GetEnumerator()));

            mockSet.As<IQueryable<User>>()
               .Setup(m => m.Provider)
               .Returns(new TestDbAsyncQueryProvider<User>(data.Provider));

            mockSet.As<IQueryable<User>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<User>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<User>>().Setup(m => m.GetEnumerator()).Returns(() => data.GetEnumerator());


            var mockContext = new Mock<IWebApiDbContext>();
            mockContext.Setup(m => m.User).ReturnsDbSet(mockSet.Object);
            #endregion

            #region Moq IOtion
            PositionOption iOption = new PositionOption() { Title = "Option1" };
            var mockOption = new Mock<IOptions<PositionOption>>();
            mockOption.Setup(ap => ap.Value).Returns(iOption);

            PositionOption iSnap = new PositionOption() { Title = "Option2" };
            var mockSnap = new Mock<IOptionsSnapshot<PositionOption>>();
            mockSnap.Setup(ap => ap.Value).Returns(iSnap);
            #endregion

            #region Service
            var service = new UserService(
                mockContext.Object,
               _mapper,
                mockOption.Object,
                mockSnap.Object,
                new Mock<ILogger<UsersVtwoController>>().Object);
            #endregion

            //Act
/*            mockSet.Setup(m => m.Remove(It.IsAny<User>())).Callback<User>(async (entity) =>await                            service.DeleteUserAsync(entity.Id));*/

            await service.DeleteUserAsync(34);
   
            var user = await service.GetAllAsync();

            //Assert
            mockContext.Verify(x => x.User.Remove(It.IsAny<User>()), Times.Once());
            mockContext.Verify(x => x.SaveChangesAsync(default), Times.Once());
            /*Assert.Single(user);*/
        }

        [Fact]
        public async Task AddAsync_Function_With_ReturnVoid()
        {
            //Arrange
            #region 🚹UserList       
            var data = new List<User>
            {
                new User { Id = _userId, FirstName = _userName},
            }.AsQueryable();

            #endregion

            #region Moq DbContext(provider?)
            var mockSet = new Mock<DbSet<User>>();
            mockSet.As<IDbAsyncEnumerable<User>>()
              .Setup(m => m.GetAsyncEnumerator())
              .Returns(new TestDbAsyncEnumerator<User>(data.GetEnumerator()));

            mockSet.As<IQueryable<User>>()
               .Setup(m => m.Provider)
               .Returns(new TestDbAsyncQueryProvider<User>(data.Provider));

            mockSet.As<IQueryable<User>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<User>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<User>>().Setup(m => m.GetEnumerator()).Returns(() => data.GetEnumerator());
/*            mockSet.Setup(d => d.Add(It.IsAny<User>())).Callback<User>(e => data.ToList().Add(new User { Id = 45, FirstName = "ere" }));*/
            mockSet
           .Setup(x => x.AddAsync(It.IsAny<User>(), It.IsAny<System.Threading.CancellationToken>()))
           .Callback((User model, CancellationToken token) => { data.ToList().Add(model); })
           .ReturnsAsync((User model, CancellationToken token) => null);

            var mockContext = new Mock<IWebApiDbContext>();
            mockContext.Setup(m => m.User).ReturnsDbSet(mockSet.Object);
            #endregion

            #region Moq IOtion
            PositionOption iOption = new PositionOption() { Title = "Option1" };
            var mockOption = new Mock<IOptions<PositionOption>>();
            mockOption.Setup(ap => ap.Value).Returns(iOption);

            PositionOption iSnap = new PositionOption() { Title = "Option2" };
            var mockSnap = new Mock<IOptionsSnapshot<PositionOption>>();
            mockSnap.Setup(ap => ap.Value).Returns(iSnap);
            #endregion

            #region Service
            var service = new UserService(
                mockContext.Object,
               _mapper,
                mockOption.Object,
                mockSnap.Object,
                new Mock<ILogger<UsersVtwoController>>().Object);
            #endregion
            var add = new UserAddOrUpdateDto { FirstName = "xxxx", LastName = "xxxx", Email = "xxx@xxx.xxx" };
            //Act
            await service.AddUserAsync(add);
            var users = await service.GetAllAsync();


            //Assert
            mockContext.Verify(x => x.User.AddAsync(It.IsAny<User>(),default), Times.Once());
            mockContext.Verify(x => x.SaveChangesAsync(default), Times.Once());
            //Assert.Equal(2, users.Count());
        }

        [Fact]
        public async Task UpdateAsync_Function_With_ReturnVoid()
        {
            //Arrange
            #region 🚹UserList       
            var data = new List<User>
            {
                new User { Id = _userId, FirstName = _userName},
            }.AsQueryable();
            var updateUser = new UserAddOrUpdateDto() { FirstName = "coucou", LastName = "cMoi" };
            #endregion

            #region Moq DbContext(provider?)
            var mockSet = new Mock<DbSet<User>>();
            mockSet.As<IDbAsyncEnumerable<User>>()
              .Setup(m => m.GetAsyncEnumerator())
              .Returns(new TestDbAsyncEnumerator<User>(data.GetEnumerator()));

            mockSet.As<IQueryable<User>>()
               .Setup(m => m.Provider)
               .Returns(new TestDbAsyncQueryProvider<User>(data.Provider));

            mockSet.As<IQueryable<User>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<User>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<User>>().Setup(m => m.GetEnumerator()).Returns(() => data.GetEnumerator());

            var mockContext = new Mock<IWebApiDbContext>();
            mockContext.Setup(m => m.User).ReturnsDbSet(mockSet.Object);
            #endregion

            #region Moq IOtion
            PositionOption iOption = new PositionOption() { Title = "Option1" };
            var mockOption = new Mock<IOptions<PositionOption>>();
            mockOption.Setup(ap => ap.Value).Returns(iOption);

            PositionOption iSnap = new PositionOption() { Title = "Option2" };
            var mockSnap = new Mock<IOptionsSnapshot<PositionOption>>();
            mockSnap.Setup(ap => ap.Value).Returns(iSnap);
            #endregion

            #region Service
            var service = new UserService(
                mockContext.Object,
               _mapper,
                mockOption.Object,
                mockSnap.Object,
                new Mock<ILogger<UsersVtwoController>>().Object);
            #endregion

            //Act
            await service.UpdateUserAsync(data.ElementAt(0), updateUser);

            var user = await service.GetAllAsync();


            //Assert
            mockContext.Verify(x => x.User.Update(It.IsAny<User>()), Times.Once());
            mockContext.Verify(x => x.SaveChangesAsync(default), Times.Once());
            //Assert.Equal(2, user.Count());
        }
    }
}
