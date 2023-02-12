using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyVerifyModels.Bureauhouse.Request
{
    public class PersonInfoRequest
    {
        public string Query { get; set; }
        public string IDNumber { get; set; }
        public string? CellNumber { get; set; }
        public string? Pincode { get; set; }
    }
}
