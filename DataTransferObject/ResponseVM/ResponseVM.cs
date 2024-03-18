using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.ResponseVM
{
    public class ResponseVM
    {
        public bool IsSuccess { get; set; }
        public string? message { get; set; }
    }
}
