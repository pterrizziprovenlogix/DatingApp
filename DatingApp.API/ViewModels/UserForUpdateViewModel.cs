using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatingApp.API.ViewModels
{
    public class UserForUpdateViewModel
    {
        public string Introduction { get; set; }
        public string LookingFor { get; set; }
        public string Interests { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
    }
}
