using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLibrary
{
    public class UserInfo
    {
        public string UserFirstName { get; set; }
        public string UserLastName { get; set; }
        public string UserDisplayName { get; set; }
        public string UserEmail { get; set; }
        public bool IsAdmin { get; set; }
        public string UserDepartmentName { get; set; }
        public int UserDepartmentID { get; set; }
    }
}
