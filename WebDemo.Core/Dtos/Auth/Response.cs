﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebDemo.Core.Dtos.Auth
{
    public class Response
    {
        public string? Status { get; set; }
        public string? Message { get; set; }

        public string? UserId { get; set; }
    }
}
