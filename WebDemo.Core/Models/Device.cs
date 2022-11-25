using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApiDemo.Core.Models;

namespace WebDemo.Core.Models
{
    public class Device
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string? Type { get; set; }
        public string? Marque { get; set; }
        public string? ModelName { get; set; }


        //Des relations entièrement définies
        //source: https://docs.microsoft.com/en-us/ef/core/modeling/relationships?tabs=fluent-api%2Cfluent-api-simple-key%2Csimple-key
        public virtual User? User { get; set; }


    }
}
