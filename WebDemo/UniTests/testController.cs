using AutoMapper;
using Castle.Core.Resource;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;
using Moq;
using Moq.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Runtime.ConstrainedExecution;
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
using WebDemo.Infrastructure.Data;
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
         * 
         * Callback mécanisme dit "Je ne peux pas le décrire pour le moment, mais quand un appel en
         * forme de tel se produit, rappelez-moi et je ferai ce qui doit être fait".
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

            #region Moq DbContext
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
            var users = await service.GetAllAsync();

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
        public async Task DeleteUserByIdAsync_Return_CheckMethodsIsCalled()
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
        }

        [Fact]
        public async Task UpdateAsync_Return_CheckMethodsIsCalled()
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

        [Fact]
        public async Task AddAsync_Return_CheckMethodsIsCalled()
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

            /*            mockSet
                            .Setup(x => x.AddAsync(It.IsAny<User>(), default))
                            .Returns((User t) => t)
                            .Callback((User t) => data.ToList().Add(t)); //<<< for when mocked Add is called.*/
            /*            mockSet.Setup(m => m.AddAsync(It.IsAny<User>(), default))
                                .Callback<User, CancellationToken>((s, token) => { data.ToList().Add(s); });*/

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
            #region comment..
            /*            var usersList = new List<User>
                         {
                             new User { Id = _userId, FirstName = _userName},
                         };

                        mockContext.Setup(m => m.User.AddAsync(It.IsAny<User>(), default))
                        .Callback<User, CancellationToken>((s, token) => { usersList.Add(s); });

                        mockContext.Setup(c => c.SaveChangesAsync(default))
                                    .Returns(Task.FromResult(1))
                                    .Verifiable();

                        await service.AddUserAsync(add);


                        mockContext.Verify(x => x.User.AddAsync(It.IsAny<User>(), default), Times.Once);

                        mockContext.Verify(x => x.SaveChangesAsync(It.IsAny<System.Threading.CancellationToken>()), Times.Once);
            */
            #endregion
            
            var add = new UserAddOrUpdateDto { FirstName = "xxxx", LastName = "xxxx", Email = "xxx@xxx.xxx" };

            var moqS = new Mock<IUserService>();
            moqS.Setup(m => m.AddUserAsync(add)).Returns(Task.FromResult(add));
            await moqS.Object.AddUserAsync(add);
            moqS.Verify(x => x.AddUserAsync(It.IsAny<UserAddOrUpdateDto>()), Times.Once());

            await service.AddUserAsync(add);
            var users = await service.GetAllAsync();

            //Assert
            mockContext.Verify(x => x.User.AddAsync(It.IsAny<User>(), default), Times.Once());
            mockContext.Verify(x => x.SaveChangesAsync(default), Times.Once());
            //Assert.Equal(2, users.Count());
        }

        //=====================================================
        //Async
        [Fact]
        public async Task AsyncAddTest_Return_SpecifiedMethodNotSupported()
        {
            var mockSet = new Mock<DbSet<User>>();

            var mockContext = new Mock<IWebApiDbContext>();
            mockContext.Setup(m => m.User).Returns(mockSet.Object);

            #region Moq IOtion
            PositionOption iOption = new PositionOption() { Title = "Option1" };
            var mockOption = new Mock<IOptions<PositionOption>>();
            mockOption.Setup(ap => ap.Value).Returns(iOption);

            PositionOption iSnap = new PositionOption() { Title = "Option2" };
            var mockSnap = new Mock<IOptionsSnapshot<PositionOption>>();
            mockSnap.Setup(ap => ap.Value).Returns(iSnap);
            #endregion

            #region var service = new UserService(mockContext.Object, ...)
            var service = new UserService(
                mockContext.Object,
               _mapper,
                mockOption.Object,
                mockSnap.Object,
                new Mock<ILogger<UsersVtwoController>>().Object);
            #endregion

            await service.AddUserAsync(new UserAddOrUpdateDto { FirstName = "youba" });

            mockSet.Verify(m => m.AddAsync(It.IsAny<User>(), default), Times.Once());
            mockContext.Verify(x => x.SaveChangesAsync(default), Times.Once());
        }

        [Fact]
        public async Task AsyncAddTest_InMemory_WORK()
        {
            //Arrange
            var options = new DbContextOptionsBuilder<WebApiDbContext>()
                .UseInMemoryDatabase(databaseName: "dbContext")
                .Options;

            var context = new WebApiDbContext(options);

            #region Moq IOtion
            PositionOption iOption = new PositionOption() { Title = "Option1" };
            var mockOption = new Mock<IOptions<PositionOption>>();
            mockOption.Setup(ap => ap.Value).Returns(iOption);

            PositionOption iSnap = new PositionOption() { Title = "Option2" };
            var mockSnap = new Mock<IOptionsSnapshot<PositionOption>>();
            mockSnap.Setup(ap => ap.Value).Returns(iSnap);
            #endregion

            #region var service = new UserService(context, ...)
            var service = new UserService(
                context,
               _mapper,
                mockOption.Object,
                mockSnap.Object,
                new Mock<ILogger<UsersVtwoController>>().Object);
            #endregion

            //Act
            await service.AddUserAsync(new UserAddOrUpdateDto { FirstName = "youba" });
            var users = await service.GetAllAsync();

            //Assert
            Assert.Single(users);
        }

        //Synch
        [Fact]
        public void SynchroneAddTest_Return_MethodUsed()
        {
            var mockSet = new Mock<DbSet<User>>();

            var mockContext = new Mock<IWebApiDbContext>();
            mockContext.Setup(m => m.User).Returns(mockSet.Object);

            #region Moq IOtion
            PositionOption iOption = new PositionOption() { Title = "Option1" };
            var mockOption = new Mock<IOptions<PositionOption>>();
            mockOption.Setup(ap => ap.Value).Returns(iOption);

            PositionOption iSnap = new PositionOption() { Title = "Option2" };
            var mockSnap = new Mock<IOptionsSnapshot<PositionOption>>();
            mockSnap.Setup(ap => ap.Value).Returns(iSnap);
            #endregion

            #region var service = new UserService(mockContext.Object, ...)
            var service = new UserService(
                mockContext.Object,
               _mapper,
                mockOption.Object,
                mockSnap.Object,
                new Mock<ILogger<UsersVtwoController>>().Object);
            #endregion

            service.AddUser(new UserAddOrUpdateDto { FirstName = "youba" });

            mockSet.Verify(m => m.Add(It.IsAny<User>()), Times.Once());
            mockContext.Verify(x => x.SaveChanges(), Times.Once());
        }

        [Fact]
        public void SynchroneAdd_WithDbSetANDMock()
        {
            var data = new List<User>
            {
                new User{FirstName = "Jean"},
            }.AsQueryable();

            var mockSet = new Mock<DbSet<User>>();
            mockSet.As<IQueryable<User>>().Setup(m => m.Provider).Returns(data.Provider);
            mockSet.As<IQueryable<User>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<User>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<User>>().Setup(m => m.GetEnumerator()).Returns(() => data.GetEnumerator());

            var mockContext = new Mock<IWebApiDbContext>();
            mockContext.Setup(m => m.User).Returns(mockSet.Object);

            #region Moq IOtion
            PositionOption iOption = new PositionOption() { Title = "Option1" };
            var mockOption = new Mock<IOptions<PositionOption>>();
            mockOption.Setup(ap => ap.Value).Returns(iOption);

            PositionOption iSnap = new PositionOption() { Title = "Option2" };
            var mockSnap = new Mock<IOptionsSnapshot<PositionOption>>();
            mockSnap.Setup(ap => ap.Value).Returns(iSnap);
            #endregion

            #region var service = new UserService(mockContext.Object, ...);
            var service = new UserService(
                mockContext.Object,
               _mapper,
                mockOption.Object,
                mockSnap.Object,
                new Mock<ILogger<UsersVtwoController>>().Object);
            #endregion

            service.AddUser(new UserAddOrUpdateDto { FirstName = "add" });
            var users = service.GetAll();

            Assert.Equal(2, users.Count());
   /*         Assert.Single(users);*/

        }

        //==============GitHub Test===============

        [Fact]
        public void DbContextMock_CreateDbSetMock_MethodNotSupported()
        {
            var dbContextMock = new Mock<IWebApiDbContext>();

            var dbSetMock = new Mock<DbSet<User>>();
            {
                new User { Id = 1 , FirstName = "first" };
                new User { Id = 2 ,FirstName = "second" };
            };
            dbSetMock.Object.Add(new User { Id = 3 , FirstName = "third" });
            dbContextMock.Object.SaveChanges();

            Assert.Equal("first", dbSetMock.Object.First(x => x.Id == 1).FirstName);
            Assert.Equal("second", dbSetMock.Object.First(x => x.Id == 2).FirstName);
            Assert.Equal("third",dbSetMock.Object.First(x => x.Id == 3).FirstName);
        }

        [Fact]
        public void DeleteProductShouldReturnTrueWhenRemovingProductHasDone()
        {
            // Arrange
            var _dbServiceMock = new Mock<IUserService>();
            var user = new User() { Id = 22,  FirstName = "you"};
            /*  _dbServiceMock.Setup(x => x.DeleteUserAsync(user.FirstName));*/
            _dbServiceMock.Setup(x => x.DeleteUserAsync(user.Id))
            .Returns(Task.FromResult(user));

            IUserService userService = _dbServiceMock.Object;

            // Act
            var result = _dbServiceMock.Object.DeleteUserAsync(user.Id);

            // Assert
            _dbServiceMock.Verify(x => x.DeleteUserAsync(It.IsAny<int>()), Times.AtMostOnce);
        }


    }
}
