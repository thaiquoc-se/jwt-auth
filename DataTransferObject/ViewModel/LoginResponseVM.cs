using DataTransferObject.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.ViewModel
{
    public class LoginResponseVM
    {
        public string? message { get; set; }

        public string? role { get; set; }

        public CustomerDTO? data { get; set; }

        public string? token { get; set; }
    }
}
