using DataLibrary;
using ISD.ActiveDirectory;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace DataLibrary
{
    public class Factory
    {
        public static Factory _Factory = null;
        private static SqlConnection _cn = null;
        public string errorMsg = "";

        public static Factory Instance
        {
            get
            {
                if (_Factory == null)
                {
                    _Factory = new Factory();
                }
                return _Factory;
            }
        }
        private Factory()
        {
            _cn = new SqlConnection();
            _cn.Close();
        }
        public List<Division> LoadDivisionsByDept(int deptCode)
        {
            List<Division> lDivision = new List<Division>();
            _cn = new SqlConnection(Properties.Settings.Default["EmployeeDirectoryDB"].ToString());
            SqlCommand cmd = new SqlCommand("spLoadDivisionsByDepartment", _cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@pDeptID", deptCode);

            using (_cn)
            {
                _cn.Open();
                SqlDataReader rs;
                rs = cmd.ExecuteReader();

                while (rs.Read())
                {
                    Division e = new Division();
                    e.div_code = rs["divCode"].ToString();
                    e.div_name = rs["divDescription"].ToString();
                    e.div_name = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(e.div_name.ToLower());
                    lDivision.Add(e);
                }

            }
            return lDivision;
        }
        public List<Department> LoadDepartments()
        {
            List<Department> lDepartments = new List<Department>();

            _cn = new SqlConnection(Properties.Settings.Default["ISDAdmin"].ToString());
            SqlCommand cmd = new SqlCommand("GetlkDepartment", _cn);
            cmd.CommandType = CommandType.StoredProcedure;
            using (_cn)
            {
                _cn.Open();
                SqlDataReader rs;
                rs = cmd.ExecuteReader();

                while (rs.Read())
                {
                    Department e = new Department();
                    e.dept_code = rs["DepartmentID"].ToString();
                    e.dept_name = rs["DepartmentDesc"].ToString();
                    lDepartments.Add(e);
                }

            }

            return lDepartments;
        }

    }
}
