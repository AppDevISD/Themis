using DataLibrary;
using ISD.ActiveDirectory;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI.WebControls;

namespace DataLibrary
{
    public class Utility
    {
        private static Utility _Utility;
        public static Utility Instance
        {
            get
            {
                if (_Utility == null)
                {
                    _Utility = new Utility();
                }
                return _Utility;
            }
        }
        public ADUser AuthenticateUser()
        {
            bool spoofUser = false;
            string strLoginID;
            ADUser adu = new ADUser();
            string[] strTemp = { "", "" };
            if (HttpContext.Current.User.Identity.IsAuthenticated)
            {
                switch (spoofUser)
                {
                    case true:
                        strLoginID = "";
                        adu = ISDFactory.Instance.GetUserInformationByLoginName(strLoginID);
                        //adu.Title = EmployeeTitle(Convert.ToInt32(adu.EmployeeID));
                        break;

                    case false:
                        strLoginID = HttpContext.Current.User.Identity.Name;
                        if (strLoginID.Length > 0)
                        {
                            strTemp = strLoginID.Split('\\');
                            strLoginID = strTemp[1];
                        }
                        else { }
                        adu = ISDFactory.Instance.GetUserInformationByLoginName(strLoginID);
                        //adu.Title = EmployeeTitle(Convert.ToInt32(adu.EmployeeID));
                        break;

                }

            }
            return adu;
        }
        //public string EmployeeTitle(int pIntID)
        //{
        //    string employeeTitle = string.Empty;
        //    _cn = new SqlConnection(Properties.Settings.Default["EmployeeDirectoryDB"].ToString());
        //    SqlCommand cmd = new SqlCommand("spGetEmployeeDetail", _cn);
        //    cmd.CommandType = CommandType.StoredProcedure;
        //    cmd.Parameters.AddWithValue("@pIntID", pIntID);
        //    using (_cn)
        //    {
        //        _cn.Open();
        //        SqlDataReader rs;
        //        rs = cmd.ExecuteReader();

        //        while (rs.Read())
        //        {
        //            employeeTitle = rs["Title"].ToString();
        //        }
        //    }
        //    return employeeTitle;
        //}
        public Dictionary<string, string> DepartmentsList()
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>()
            {
                {"Select Department...",                null},
                { "Budget & Management",                "5"},
                {"City Clerk",                          "13"},
                {"City Council",                        "7"},
                {"City Treasurer",                      "12"},
                {"Community Relations",                 "16"},
                {"Convention & Visitor's Bureau",       "14"},
                {"Corporation Counsel",                 "6"},
                {"Fire Department",                     "4"},
                {"Human Resources",                     "8"},
                {"Lincoln Library",                     "15"},
                {"Office of The Mayor",                 "10"},
                {"Planning & Economic Development",     "1"},
                {"Police Department",                   "11"},
                {"Public Utilities",                    "3"},
                {"Public Works",                        "9"},
            };
            
            return dictionary;
        }

        public static decimal CurrencyToDecimal(string currency)
        {
            return decimal.Parse(currency, NumberStyles.Any);
        }
    }
}
