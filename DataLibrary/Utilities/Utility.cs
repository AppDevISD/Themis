using DataLibrary;
using ISD.ActiveDirectory;
using System;
using System.Runtime;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
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
                        if (adu.EmployeeID.Equals(null) || adu.EmployeeID.Equals(string.Empty))
                        {
                            adu.EmployeeID = GetEmployeeID(adu.Email);
                        }
                        if (adu.PhotoLocation.Equals(null) || adu.PhotoLocation.Equals(string.Empty))
                        {
                            adu.PhotoLocation = $"\\\\INTRANET\\Shared$\\HRPhotos\\{adu.EmployeeID}.jpg";
                        }
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
                        adu.Email = adu.Email.ToLower();
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
                        if (adu.EmployeeID.Equals(null) || adu.EmployeeID.Equals(string.Empty))
                        {
                            adu.EmployeeID = GetEmployeeID(adu.Email);
                        }
                        if (adu.PhotoLocation.Equals(null) || adu.PhotoLocation.Equals(string.Empty))
                        {
                            adu.PhotoLocation = $"\\\\INTRANET\\Shared$\\HRPhotos\\{adu.EmployeeID}.jpg";
                        }
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
                        adu.Email = adu.Email.ToLower();
                        //adu.Title = EmployeeTitle(Convert.ToInt32(adu.EmployeeID));
                        break;

                }

            }
            return adu;
        }
        public static string EmployeeTitle(int pIntID)
        {
            string employeeTitle = string.Empty;
            SqlConnection cn = new SqlConnection(Properties.Settings.Default["EmployeeDirectoryDB"].ToString());
            SqlCommand cmd = new SqlCommand("spGetEmployeeDetail", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@pIntID", pIntID);
            using (cn)
            {
                cn.Open();
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
            SqlConnection cn = new SqlConnection(Properties.Settings.Default["EmployeeDirectoryDB"].ToString());
            SqlCommand cmd = new SqlCommand("spGetEmployeeDetail", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@pIntID", pIntID);
            using (cn)
            {
                cn.Open();
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
            SqlConnection cn = new SqlConnection(Properties.Settings.Default["EmployeeDirectoryDB"].ToString());
            SqlCommand cmd = new SqlCommand("spGetEmployeeDetail", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@pIntID", pIntID);
            using (cn)
            {
                cn.Open();
                SqlDataReader rs;
                rs = cmd.ExecuteReader();

                while (rs.Read())
                {
                    employeeExt = rs["OfficeExt"].ToString();
                }
            }
            return employeeExt;
        }
        public static string GetEmployeeID(string email)
        {
            Info info = new Info();
            SqlConnection cn = new SqlConnection(Properties.Settings.Default["EmployeeDirectoryDB"].ToString());
            SqlCommand cmd = new SqlCommand("spGetEmployeeInformationByEmail", cn);
            cmd.Parameters.AddWithValue($"@pemail", email);
            cmd.CommandType = CommandType.StoredProcedure;

            using (cn)
            {
                cn.Open();
                SqlDataReader rs;
                rs = cmd.ExecuteReader();
                while (rs.Read())
                {
                    info.intID = Convert.ToInt32(rs["intID"]);
                }
            }
            return info.intID.ToString();
        }
        public static string GetEmployeePhotoLocation(string pIntID)
        {

            SqlConnection cn = new SqlConnection(Properties.Settings.Default["IDCards"].ToString());
            string PhotoLocation = "";
            using (cn)
            {
                cn.Open();
                string strSQLCommand = "Select photo from IdentificationCards Where int_id = " + pIntID;
                SqlCommand cmd2 = new SqlCommand(strSQLCommand, cn);
                SqlDataReader rs2;
                rs2 = cmd2.ExecuteReader();


                while (rs2.Read())
                {
                    PhotoLocation = rs2["Photo"].ToString();
                    Token t = new Token(PhotoLocation, "\\");
                    while (t.hasElements())
                    {
                        PhotoLocation = t.nextElement();
                    }
                    PhotoLocation = @"\\INTRANET\Shared$\HRPhotos\" //Properties.Settings.Default["PictureLocation"].ToString()
                                    + @"\" + PhotoLocation;
                }

                //rs2.Close();

                //_cn.Close();
            }
            return PhotoLocation;
        }
        public static Department GetUserDepartment(string employeeID)
        {
            Department department = new Department();
            SqlConnection cn = new SqlConnection(Properties.Settings.Default["EmployeeDirectoryDB"].ToString());
            SqlCommand cmd = new SqlCommand("spGetEmployeeDetail", cn);
            cmd.Parameters.AddWithValue($"@pIntID", employeeID);
            cmd.CommandType = CommandType.StoredProcedure;

            using (cn)
            {
                cn.Open();
                SqlDataReader rs;
                rs = cmd.ExecuteReader();
                while (rs.Read())
                {
                    department.DepartmentCode = Convert.ToInt32(rs["deptCode"]);
                    department.DepartmentName = GetUserDepartmentName(department.DepartmentCode);
                }
            }
            return department;
        }
        public static string GetUserDepartmentName(int key)
        {
            Dictionary<int, string> dictionary = new Dictionary<int, string>()
            {
                { 5,  "Budget & Management" },
                { 13, "City Clerk" },
                { 7,  "City Council" },
                { 12, "City Treasurer" },
                { 16, "Community Relations" },
                { 14, "Convention & Visitor's Bureau" },
                { 6,  "Corporation Counsel" },
                { 4,  "Fire Department" },
                { 8,  "Human Resources" },
                { 15, "Lincoln Library" },
                { 10, "Office of The Mayor" },
                { 1,  "Planning & Economic Development" },
                { 11, "Police Department" },
                { 3,  "Public Utilities" },
                { 9,  "Public Works" },
            };

            return dictionary[key];
        }
        public static Division GetUserDivision(string employeeID)
        {
            Division division = new Division();
            SqlConnection cn = new SqlConnection(Properties.Settings.Default["EmployeeDirectoryDB"].ToString());
            SqlCommand cmd = new SqlCommand("spGetEmployeeDetail", cn);
            cmd.Parameters.AddWithValue($"@pIntID", employeeID);
            cmd.CommandType = CommandType.StoredProcedure;

            using (cn)
            {
                cn.Open();
                SqlDataReader rs;
                rs = cmd.ExecuteReader();
                while (rs.Read())
                {
                    try
                    {
                        division.DivisionCode = Convert.ToInt32(rs["divCode"]);
                        division.DivisionName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(rs["division"].ToString().ToLower());
                    }
                    catch (Exception)
                    {

                    }
                }
            }
            return division;
        }
        public static List<Division> GetDivisionsByDept(int deptCode)
        {
            List<Division> lDivision = new List<Division>();
            SqlConnection cn = new SqlConnection(Properties.Settings.Default["EmployeeDirectoryDB"].ToString());
            SqlCommand cmd = new SqlCommand("spLoadDivisionsByDepartment", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@pDeptID", deptCode);

            using (cn)
            {
                cn.Open();
                SqlDataReader rs;
                rs = cmd.ExecuteReader();

                while (rs.Read())
                {
                    Division e = new Division();
                    e.DivisionCode = Convert.ToInt32(rs["divCode"]);
                    e.DivisionName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(rs["divDescription"].ToString().ToLower());
                    lDivision.Add(e);
                }

            }
            return lDivision;
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
            foreach (Status status in statusList.Where(i => !i.StatusID.Equals(9)))
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
                { "RequestDivision", "Requesting Division" },
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
                { "budgetOBM", "OBM Budget" },
                { "budgetCWLP", "CWLP Budget" },
                { "mayor", "Mayor" },
                { "ccDirector", "Corporation Counsel Director" },

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
                return decimal.Parse("-1.00", NumberStyles.Any);
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

        public static AccountingAudit BuildAccAudit( OrdinanceAccounting current, OrdinanceAccounting original, PropertyInfo[] properties, List<string> baseData)
        {
            var excludedProps = new HashSet<string>
            {
                "AccountingAuditID",
                "AuditID",
                "OrdinanceAccountingID",
                "AccountingDesc"
            };

            var accAudit = new AccountingAudit
            {
                AccountingDesc = current?.AccountingDesc,
                OrdinanceAccountingID = current?.OrdinanceAccountingID ?? 0
            };

            foreach (var property in properties)
            {
                if (excludedProps.Contains(property.Name))
                    continue;

                if (baseData.Any(b => b.Contains(property.Name)))
                    continue;

                object newValue = current != null ? property.GetValue(current) : null;
                object oldValue = original != null ? property.GetValue(original) : null;

                string dataType = property.PropertyType == typeof(decimal) ? "Decimal" : "String";
                string name = property.Name;

                string change;

                if (original == null)
                {
                    change = $"<span>{AuditSymbol("add")} <span data-type='{dataType}'>{newValue ?? "N/A"}</span></span>";
                }
                else if (!Equals(oldValue, newValue))
                {
                    if (name == "Amount" && (oldValue?.ToString() == "-1" || oldValue?.ToString() == "-1.00"))
                        change = $"<span>{AuditSymbol("add")} <span data-type='Decimal'>{newValue}</span></span>";
                    else if (name == "Amount" && (newValue?.ToString() == "-1" || newValue?.ToString() == "-1.00"))
                        change = $"<span>{AuditSymbol("remove")} <span data-type='Decimal'>{oldValue}</span></span>";
                    else
                        change = $"<span><span data-type='{dataType}'>{oldValue}</span> {AuditSymbol("update")} <span data-type='{dataType}'>{newValue}</span></span>";
                }
                else
                {
                    if (name == "Amount" && (newValue?.ToString() == "-1" || newValue?.ToString() == "-1.00"))
                        change = $"<span data-type='String'>N/A</span>";
                    else
                        change = $"<span data-type='{dataType}'>{newValue}</span>";
                }

                var auditProp = typeof(AccountingAudit).GetProperty(name);
                if (auditProp != null && auditProp.PropertyType == typeof(string))
                {
                    auditProp.SetValue(accAudit, change);
                }
            }

            return accAudit;
        }
    }
}