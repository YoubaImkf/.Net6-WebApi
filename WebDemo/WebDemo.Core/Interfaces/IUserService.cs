﻿using WebApiDemo.Core.Models;
using WebApiDemo.Dtos;


namespace WebDemo.Core.Interfaces
{
    public interface IUserService
    {   //declarer t methodes
        //dans un fichier distinct car il est courant qu'une interface soit implémentée par plusieurs classes.
        Task<IEnumerable<UserDto>> GetAllAsync();
        Task<IEnumerable<UserDto>> FindDevicesByUserIdAsync(int id);
        Task<UserDto> GetUserAsync(int id);
        Task DeleteUserAsync(int id);
        Task AddUserAsync(UserAddOrUpdateDto userAddDto);
        Task UpdateUserAsync(User user, UserAddOrUpdateDto userDto);
        Task<User> FindUserByIdAsync(int id);
    }
}
