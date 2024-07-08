using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;


namespace DataLibrary
{
    public class Employee
    {
        public Int32 employeID { get; set; }
        public string first { get; set; }
        public string mid { get; set; }
        public string last { get; set; }
        public string full_name { get; set; }
        public string preferred_name { get; set; }
        public string name_suffix { get; set; }
        public string title { get; set; }
        public string department { get; set; }
        public string division { get; set; }
        public string location { get; set; }
        public string location2 { get; set; }
        public string phone { get; set; }
        public string phone_ext { get; set; }
        public string phone_cell { get; set; }
        public string pager { get; set; }
        public string email { get; set; }
        public string photo { get; set; }
        public string under_cover { get; set; }
        public int dept_code { get; set; }
        public int div_code { get; set; }
        public string UnitCode { get; set; }
        public int loc_code { get; set; }
        public int loc2_code { get; set; }
        public string photo_src { get; set; }
        public bool Active { get; set; }
        public int OrgLevel { get; set; }
        public string PrintAdvice { get; set; }

        public string ReportsToEmployeeID { get; set; }
    }

    public class Department
    {
        public string dept_code { get; set; }
        public string dept_name { get; set; }
    }

    public class Division
    {
        public string dept_code { get; set; }
        public string div_code { get; set; }
        public string div_name { get; set; }
    }

    public class Location
    {
        public string loc_code { get; set; }
        public string loc_name { get; set; }
    }

    public class SubLocation
    {
        public string loc_code { get; set; }
        public string subloc_code { get; set; }
        public string subloc_name { get; set; }
    }

    public class AdvancedSearchCriteria
    {
        public string IntID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PreferredName { get; set; }
        public string Department { get; set; }
        public string Division { get; set; }
        public string Location { get; set; }
        public string Location2 { get; set; }
        public string UnderCover { get; set; }
    }
}