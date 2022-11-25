using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApiDemo.Core.Models;

namespace WebApiDemo.Dtos
//namespace WebDemo.Core.Dtos
{
    public class DeviceDto
    {
        public int Id { get; set; }
        public string? ModelName { get; set; }
        public string? Type { get; set; }
       
    }
}
