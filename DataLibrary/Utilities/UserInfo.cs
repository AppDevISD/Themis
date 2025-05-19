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
        public bool UserView { get; set; }
        public Department UserDepartment { get; set; }
        public Division UserDivision { get; set; }
    }

    public class Department
    {
        public int DepartmentCode { get; set; }
        public string DepartmentName { get; set; }
    }

    public class Division
    {
        public int DivisionCode { get; set; }
        public string DivisionName { get; set; }
    }
}
