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
using System.Threading.Tasks;
using WebApiDemo.Core.Models;
using WebApiDemo.Dtos;
using WebDemo.Api.Controllers.V1;
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
        public async Task GetAll_Function_With_Return_ValuesAsync()
        {
            //Arrange
            #region 🚹UserList
            var data = new List<User>
            {
                new User { Id = _userId, FirstName = _userName },
                new User { Id = 10, FirstName = "Salim" },
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
            var users =  await service.GetAllAsync();

            //Assert
            Assert.NotNull(users);
            Assert.Equal(2, users.Count());
            Assert.Equal(_userName, users.ElementAt(0).FirstName);
        }

        [Fact]
        public async Task GetDevicesByUserId_Function_With_Return_ValueAsync()
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
        public async Task GetId_Function_With_Return_ValueAsync()
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
            var user = await service.GetUserAsync(_userId); //null           

            //Assert
            Assert.Equal(_userName, user.FirstName);
        }

    }
}
