using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyVerifyModels.Bureauhouse
{
    public class LoginResponse
    {
        public List<string> Results { get; set; }
        public int TotalRecords { get; set; }
        public int TotalReturnedRecords { get; set; }
        public string CreationTime { get; set; }
        public string status_message { get; set; }
        public string bh_response_code { get; set; }
        public string http_code { get; set; }
        public string request_reference { get; set; }
    }
}
