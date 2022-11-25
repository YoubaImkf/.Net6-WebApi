using AutoMapper;
using WebApiDemo.Core.Models;
using WebApiDemo.Dtos;
using WebDemo.Core.Models;

namespace WebDemo.Api.Helpers
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            //Mapping entre entité et Dto 
            //AutoMapper est une bibliothèque simple qui nous aide à transformer un type d'objet en un autre.
            // source: https://stackoverflow.com/questions/44877379/how-inject-service-in-automapper-profile-class
            #region User
            CreateMap<User, UserDto>();

            CreateMap<UserDto, User>();
            #endregion

            #region Device
            CreateMap<DeviceDto, Device>();

            CreateMap<Device, DeviceDto>();
            #endregion

            #region User-Add
            CreateMap<UserAddOrUpdateDto, User>();
            #endregion



        }
    }
}

