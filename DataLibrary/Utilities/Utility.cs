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
using System.Runtime.InteropServices;
using System.Diagnostics;

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
        public static ADUser AuthenticateUser([Optional] string loginID)
        {
            bool spoofUser = loginID != null ? true : false;
            string strLoginID;
            ADUser adu = new ADUser();
            string[] strTemp = { "", "" };
            if (HttpContext.Current.User.Identity.IsAuthenticated)
            {
                switch (spoofUser)
                {
                    case true:
                        strLoginID = loginID;
                        adu = ISDFactory.Instance.GetUserInformationByLoginName(strLoginID);
                        adu.Telephone = EmployeePhone(Convert.ToInt32(adu.EmployeeID)) ?? adu.Telephone;
                        if (adu.Telephone.Length > 0 && !adu.Telephone.StartsWith("217") && !adu.Telephone.StartsWith("(217)"))
                        {
                            adu.Telephone = $"(217) {adu.Telephone}";
                        }
                        else if (adu.Telephone.StartsWith("217") || adu.Telephone.StartsWith("(217)"))
                        {
                            adu.Telephone = $"(217) {adu.Telephone.Replace("(", "").Replace(")", "").Substring(4)}";
                        }
                        if (adu.IPPhone.Length < 1)
                        {
                            adu.IPPhone = EmployeeExt(Convert.ToInt32(adu.EmployeeID)) ?? string.Empty;
                        }
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
                        adu.Telephone = EmployeePhone(Convert.ToInt32(adu.EmployeeID)) ?? adu.Telephone;
                        if (adu.Telephone.Length > 0 && !adu.Telephone.StartsWith("217") && !adu.Telephone.StartsWith("(217)"))
                        {
                            adu.Telephone = $"(217) {adu.Telephone}";
                        }
                        else if (adu.Telephone.StartsWith("217") || adu.Telephone.StartsWith("(217)"))
                        {
                            adu.Telephone = $"(217) {adu.Telephone.Replace("(", "").Replace(")", "").Substring(4)}";
                        }
                        if (adu.IPPhone.Length < 1)
                        {
                            adu.IPPhone = EmployeeExt(Convert.ToInt32(adu.EmployeeID)) ?? string.Empty;
                        }
                        //adu.Title = EmployeeTitle(Convert.ToInt32(adu.EmployeeID));
                        break;

                }

            }
            return adu;
        }
        public static string EmployeeTitle(int pIntID)
        {
            string employeeTitle = string.Empty;
            SqlConnection _cn = new SqlConnection(Properties.Settings.Default["EmployeeDirectoryDB"].ToString());
            SqlCommand cmd = new SqlCommand("spGetEmployeeDetail", _cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@pIntID", pIntID);
            using (_cn)
            {
                _cn.Open();
                SqlDataReader rs;
                rs = cmd.ExecuteReader();

                while (rs.Read())
                {
                    employeeTitle = rs["Title"].ToString();
                }
            }
            return employeeTitle;
        }
        public static string EmployeePhone(int pIntID)
        {
            string employeePhone = string.Empty;
            SqlConnection _cn = new SqlConnection(Properties.Settings.Default["EmployeeDirectoryDB"].ToString());
            SqlCommand cmd = new SqlCommand("spGetEmployeeDetail", _cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@pIntID", pIntID);
            using (_cn)
            {
                _cn.Open();
                SqlDataReader rs;
                rs = cmd.ExecuteReader();

                while (rs.Read())
                {
                    employeePhone = rs["Office"].ToString();
                }
            }
            return employeePhone;
        }
        public static string EmployeeExt(int pIntID)
        {
            string employeeExt = string.Empty;
            SqlConnection _cn = new SqlConnection(Properties.Settings.Default["EmployeeDirectoryDB"].ToString());
            SqlCommand cmd = new SqlCommand("spGetEmployeeDetail", _cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@pIntID", pIntID);
            using (_cn)
            {
                _cn.Open();
                SqlDataReader rs;
                rs = cmd.ExecuteReader();

                while (rs.Read())
                {
                    employeeExt = rs["OfficeExt"].ToString();
                }
            }
            return employeeExt;
        }
        public static Dictionary<string, string> DepartmentsList()
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>()
            {
                {"Select Department...",                ""},
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
        public static Dictionary<string, string> StatusList()
        {
            List<Status> statusList = new List<Status>();
            statusList = Factory.Instance.GetAll<Status>("sp_GetLkStatus");
            Dictionary<string, string> dictionary = new Dictionary<string, string>()
            {
                {"Select Status...", "" }
            };
            foreach (Status status in statusList)
            {
                dictionary.Add(status.StatusDescription, status.StatusID.ToString());
            }
            return dictionary;
        }
        public static string FieldLabels(string key)
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>()
            {
                { "StatusDescription", "Status" },
                { "OrdinanceNumber", "Ordinance Number" },
                { "RequestDepartment", "Requesting Department" },
                { "FirstReadDate", "First Read Date" },
                { "RequestContact", "Requesting Contact" },
                { "RequestPhone", "Phone Number" },
                { "RequestEmail", "Email" },
                { "RequestExt", "Ext" },
                { "EmergencyPassage", "Emergency Passage" },
                { "EmergencyPassageReason", "Emergency Passage Justification" },
                { "OrdinanceFiscalImpact", "Fiscal Impact" },
                { "OrdinanceTitle", "Suggested Title" },
                { "ContractVendorName", "Vendor Name" },
                { "ContractVendorNumber", "Vendor Number" },
                { "ContractStartDate", "Start Date" },
                { "ContractEndDate", "End Date" },
                { "ContractTerm", "Date Term" },
                { "ContractAmount", "Contract Amount" },
                { "ScopeChange", "Change In Scope" },
                { "ChangeOrderNumber", "Change Order Number" },
                { "AdditionalAmount", "Additional Amount" },
                { "ContractMethod", "Purchase Method" },
                { "OtherException", "Other/Exception" },
                { "PreviousOrdinanceNumbers", "Previous Ordinance Numbers" },
                { "CodeProvision", "Code Provision" },
                { "PAApprovalRequired", "Purchasing Agent Approval Required" },
                { "PAApprovalIncluded", "Purchasing Agent Approval Attached" },
                { "OrdinanceAnalysis", "Staff Analysis" },
                { "SupportingDocumentation", "Supporting Documentation" },

                { "fundsCheckBy", "Funds Check By" },
                { "directorSupervisor", "Director/Supervisor" },
                { "cPA", "City Purchasing Agent" },
                { "obmDirector", "OBM Director" },
                { "mayor", "Mayor" },

                { "RejectionReason", "Rejection Reason" },
                { "Revenue", "Revenue" },
                { "Expenditure", "Expenditure" }
            };
            return dictionary[key];
        }
        public static string AuditSymbol(string key)
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>()
            {
                { 
                    "update",
                    "<span class='fas fa-arrow-right-long mx-1 text-warning-light fw-bold align-self-center'></span>"
                },
                { 
                    "add",
                    "<span class='fas fa-plus mx-1 text-success fw-bold align-self-center'></span>"
                },
                {
                    "remove",
                    "<span class='fas fa-minus mx-1 text-danger fw-bold align-self-center'></span>"
                },
                { "rejected", string.Empty },
                { "revenue", string.Empty },
                { "expenditure", string.Empty }
            };
            return dictionary[key];
        }
        public static List<string> Skips(string key)
        {
            Dictionary<string, List<string>> dictionary = new Dictionary<string, List<string>>()
            {
                {"ordInsert", new List<string>() 
                    { 
                        "OrdinanceID",
                        "StatusDescription"
                    } 
                },
                {"accountingInsert", new List<string>()
                    {
                        "AccountingID",
                    }
                },
                {"ordAccountingInsert", new List<string>()
                    {
                        "OrdinanceAccountingID",
                    }
                },
                {"ordDocumentInsert", new List<string>()
                    {
                        "DocumentID",
                    }
                },
                {"ordStatusInsert", new List<string>()
                    {
                        "OrdinanceStatusID",
                        "StatusDescription"
                    }
                },
                {"ordSignatureInsert", new List<string>()
                    {
                        "SignatureID",
                        "SortOrder"
                    }
                },
                {"ordSignatureRequestInsert", new List<string>()
                    {
                        "SignatureRequestID",
                    }
                },
                {"ordAuditInsert", new List<string>()
                    {
                        "OrdinanceAuditID"
                    }
                },
                {"auditInsert", new List<string>()
                    {
                        "AuditID"
                    }
                },
                {"acctAuditInsert", new List<string>()
                    {
                        "AccountingAuditID"
                    }
                },
                {"ordUpdate", new List<string>()
                    {
                        "StatusDescription"
                    }
                },
                {"ordStatusUpdate", new List<string>()
                    {
                        "StatusDescription"
                    }
                }
            };

            return dictionary[key];
        }
        public static decimal CurrencyToDecimal(string currency)
        {
            try
            {
                return decimal.Parse(currency, NumberStyles.Any);
            }
            catch (Exception)
            {
                return decimal.Parse("-1", NumberStyles.Any);
            }
            
        }
        public static string NotApplicable(string data)
        {
            return !data.Equals("-1.00") ? data : string.Empty;
        }
        public static void ConsoleLog(object message)
        {
            string scriptTag = "<script type=\"\" language=\"\">{0}</script>";
            string function = "console.log('{0}');";
            string log = string.Format(string.Format(scriptTag, function), message);
            HttpContext.Current.Response.Write(log);
        }
    }
}