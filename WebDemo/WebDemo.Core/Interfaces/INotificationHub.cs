using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApiDemo.Dtos;
using WebDemo.Core.RealTimeModels;

namespace WebDemo.Core.Interfaces
{
    public interface INotificationHub
    {
        Task ReceiveNotification(Notification notification, UserAddOrUpdateDto user);
    }
}
