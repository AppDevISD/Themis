using DataLibrary;
using DataLibrary.OrdinanceTracking;
using ISD.ActiveDirectory;
using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using static DataLibrary.TablePagination;
using static DataLibrary.Utility;

namespace WebUI
{
    public partial class OrdinanceAdmin : System.Web.UI.Page
    {
        private ADUser _user = new ADUser();
        public UserInfo userInfo = new UserInfo();
        public Dictionary<string, Dictionary<string, object>> defaultListType = new Dictionary<string, Dictionary<string, object>>();

        protected void Page_Load(object sender, EventArgs e)
        {
            _user = Session["CurrentUser"] as ADUser;
            userInfo = Session["UserInformation"] as UserInfo;
            defaultListType = new Dictionary<string, Dictionary<string, object>>()
            {
                {"Pending", new Dictionary<string, object>()
                    {
                        {"ID", 1 },
                        {"Repeater", rpPendingDefaultList }
                    }
                },
                {"UnderReview", new Dictionary<string, object>()
                    {
                        {"ID", 2 },
                        {"Repeater", rpUnderReviewDefaultList }
                    }
                },
                {"BeingHeld", new Dictionary<string, object>()
                    {
                        {"ID", 3 },
                        {"Repeater", rpBeingHeldDefaultList }
                    }
                },
                {"Drafted", new Dictionary<string, object>()
                    {
                        {"ID", 4 },
                        {"Repeater", rpDraftedDefaultList }
                    }
                },
                {"Approved", new Dictionary<string, object>()
                    {
                        {"ID", 5 },
                        {"Repeater", rpApprovedDefaultList }
                    }
                },
                {"Rejected", new Dictionary<string, object>()
                    {
                        {"ID", 6 },
                        {"Repeater", rpRejectedDefaultList }
                    }
                },
                {"Deleted", new Dictionary<string, object>()
                    {
                        {"ID", 7 },
                        {"Repeater", rpDeletedDefaultList }
                    }
                },

                {"FundsCheckBy", new Dictionary<string, object>()
                    {
                        {"ID", 8 },
                        {"Repeater", rpFundsCheckByDefaultList }
                    }
                },
                {"DirectorSupervisor", new Dictionary<string, object>()
                    {
                        {"Department", new Dictionary<int, object>() },
                        {"Repeater", rpDirectorSupervisorDefaultList }
                    }
                },
                {"CityPurchasingAgent", new Dictionary<string, object>()
                    {
                        {"ID", 9 },
                        {"Repeater", rpCPADefaultList }
                    }
                },
                {"OBMDirector", new Dictionary<string, object>()
                    {
                        {"ID", 10 },
                        {"Repeater", rpOBMDirectorDefaultList }
                    }
                },
                {"Mayor", new Dictionary<string, object>()
                    {
                        {"ID", 11 },
                        {"Repeater", rpMayorDefaultList }
                    }
                },
            };
            List<DefaultEmails> departmentDefaultIDs = new List<DefaultEmails>();
            Dictionary<string, object> departmentDict = new Dictionary<string, object>();
            Dictionary<string, string> departments = DepartmentsList();
            foreach (var department in departments.Keys)
            {
                if (!department.Equals("Select Department..."))
                {
                    var value = departments[department];
                    departmentDict = (Dictionary<string, object>)defaultListType["DirectorSupervisor"]["Department"];
                    departmentDict.Add(department, new Dictionary<string, object>()
                    {
                        {"ID", new int() },
                        {"Division", string.Empty }
                    });
                }
            }

            foreach (KeyValuePair<string, object> item in departmentDict)
            {
                List<DefaultEmails> deptDefault = Factory.Instance.GetByID<List<DefaultEmails>>(item.Key, "sp_GetDefaultEmailByDepartment", "Department");
            }

            Debug.WriteLine(defaultListType);
            


            if (!Page.IsPostBack && !Response.IsRequestBeingRedirected)
            {
                if (!userInfo.IsAdmin || userInfo.UserView)
                {
                    Response.Redirect("./AccessDenied");
                }
                GetAllDepartments();
                SetStartupActives();
                GetStartupData();
            }

            if (ScriptManager.GetCurrent(Page).IsInAsyncPostBack)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "HideLoading", "hideLoadingModal();", true);
            }
        }

        protected void GetAllDepartments()
        {
            Dictionary<string, string> departments = DepartmentsList();
            foreach (var department in departments.Keys)
            {
                var value = departments[department];
                ListItem newItem = new ListItem(department, value);
                filterDepartment.Items.Add(newItem);
            }
        }
        protected void GetAllDivisions(DropDownList dd, string deptCode)
        {
            dd.Items.Clear();
            dd.Items.Add(new ListItem() { Text = "Select Division...", Value = "" });
            List<Division> divisionList = GetDivisionsByDept(Convert.ToInt32(deptCode));
            foreach (Division item in divisionList)
            {
                ListItem newItem = new ListItem(item.DivisionName, item.DivisionCode.ToString());
                dd.Items.Add(newItem);
            }
        }
        protected void SetStartupActives()
        {
            if (!filterDepartment.SelectedValue.IsNullOrWhiteSpace())
            {
                filterDivision.Enabled = true;
                GetAllDivisions(filterDivision, filterDepartment.SelectedValue);
            }
            else
            {
                filterDivision.Enabled = false;
                filterDivision.Items.Add(new ListItem() { Text = "Select Division...", Value = "" });
            }

        }
        public void GetStartupData()
        {
            foreach (KeyValuePair<string, Dictionary<string, object>> listType in defaultListType)
            {
                Dictionary<string, object> listInfo = listType.Value;
                Repeater repeater = (Repeater)listInfo["Repeater"];
                int id = new int();
                switch (listType.Key)
                {
                    default:
                        id = (int)listInfo["ID"];
                        break;
                    case "DirectorSupervisor":
                        id = 12;
                        break;
                }
                
                string sessionPrefix = repeater.ClientID.Replace("rp", "").Replace("DefaultList", "");

                DefaultEmails defaultList = Factory.Instance.GetByID<DefaultEmails>(id, "sp_GetDefaultEmailByDefaultEmailsID", "DefaultEmailsID");
                string[] emails = defaultList.EmailAddress.ToString().Split(';').Where(i => !i.IsNullOrWhiteSpace()).ToArray();
                if (emails.Length > 0)
                {
                    repeater.DataSource = emails;
                    repeater.DataBind();
                }
                else
                {
                    repeater.DataSource = null;
                    repeater.DataBind();
                }
            }
        }
        public void ActiveTabPanes(string ids)
        {
            List<string> lst = ids.Split(',').Where(i => !i.IsNullOrWhiteSpace()).ToList();
            Dictionary<string, Dictionary<HtmlButton, HtmlGenericControl>> tabs = new Dictionary<string, Dictionary<HtmlButton, HtmlGenericControl>>()
            {
                { "defaultEmailsBtn", new Dictionary<HtmlButton, HtmlGenericControl>() {{defaultEmailsBtn, defaultEmailsTabPane} } },
                { "pendingBtn", new Dictionary<HtmlButton, HtmlGenericControl>() {{pendingBtn, pendingTabPane}}},
                { "underReviewBtn", new Dictionary<HtmlButton, HtmlGenericControl>() {{underReviewBtn, underReviewTabPane}}},
                { "beingHeldBtn", new Dictionary<HtmlButton, HtmlGenericControl>() {{beingHeldBtn, beingHeldTabPane}}},
                { "draftedBtn", new Dictionary<HtmlButton, HtmlGenericControl>() {{draftedBtn, draftedTabPane}}},
                { "approvedBtn", new Dictionary<HtmlButton, HtmlGenericControl>() {{approvedBtn, approvedTabPane}}},
                { "rejectedBtn", new Dictionary<HtmlButton, HtmlGenericControl>() {{rejectedBtn, rejectedTabPane}}},
                { "deletedBtn", new Dictionary<HtmlButton, HtmlGenericControl>() {{deletedBtn, deletedTabPane}}},
                { "fundsCheckByBtn", new Dictionary<HtmlButton, HtmlGenericControl>() {{fundsCheckByBtn, fundsCheckByTabPane}}},
                { "directorSupervisorBtn", new Dictionary<HtmlButton, HtmlGenericControl>() {{directorSupervisorBtn, directorSupervisorTabPane}}},
                { "cPABtn", new Dictionary<HtmlButton, HtmlGenericControl>() {{cPABtn, cPATabPane}}},
                { "obmDirectorBtn", new Dictionary<HtmlButton, HtmlGenericControl>() {{obmDirectorBtn, obmDirectorTabPane}}},
                { "mayorBtn", new Dictionary<HtmlButton, HtmlGenericControl>() {{mayorBtn, mayorTabPane}}},
            };

            foreach (KeyValuePair<string, Dictionary<HtmlButton, HtmlGenericControl>> tab in tabs)
            {
                Dictionary<HtmlButton, HtmlGenericControl> ctrls = tabs[tab.Key];
                foreach (KeyValuePair<HtmlButton, HtmlGenericControl> ctrl in ctrls)
                {
                    ctrl.Key.Attributes["class"] = "nav-link";
                    ctrl.Key.Attributes["aria=selected"] = "false";
                    ctrl.Value.Attributes["class"] = "tab-pane fade";
                }
            }
            foreach (string id in lst)
            {
                Dictionary<HtmlButton, HtmlGenericControl> ctrlDict = tabs[id];
                foreach (KeyValuePair<HtmlButton, HtmlGenericControl> ctrl in ctrlDict)
                {

                    ctrl.Key.Attributes["class"] += " active";
                    ctrl.Key.Attributes["aria=selected"] = "true";
                    ctrl.Value.Attributes["class"] += " active show";
                }
            }
        }


        protected void Filter_SelectedIndexChanged(object sender, EventArgs e)
        {
            ActiveTabPanes(hdnActiveTabs.Value);

            try
            {
                DropDownList dropDown = (DropDownList)sender;

                string commandName = dropDown.Attributes["data-command"]; string commandArgument = dropDown.SelectedItem.ToString();

                if (commandName.Equals("department"))
                {
                    filterDivision.Items.Clear();

                    if (!filterDepartment.SelectedValue.IsNullOrWhiteSpace())
                    {
                        filterDivision.Enabled = true;
                        GetAllDivisions(filterDivision, filterDepartment.SelectedValue);
                    }
                    else
                    {
                        filterDivision.Enabled = false;
                        filterDivision.Items.Add(new ListItem() { Text = "Select Division...", Value = "" });
                    }
                }
            }
            catch (Exception)
            {

            }

            //userInfo = (UserInfo)Session["UserInformation"];

            //int statusID = !filterStatus.SelectedValue.IsNullOrWhiteSpace() ? Convert.ToInt32(filterStatus.SelectedValue) : -1;
            //string department = !filterDepartment.SelectedValue.IsNullOrWhiteSpace() ? filterDepartment.SelectedItem.Text : string.Empty;
            //string division = !filterDivision.SelectedValue.IsNullOrWhiteSpace() ? filterDivision.SelectedItem.Text : string.Empty;
            //string title = !filterSearchTitle.Text.IsNullOrWhiteSpace() ? filterSearchTitle.Text : string.Empty;

            //List<Ordinance> filteredList = new List<Ordinance>();
            //if ((userInfo.UserDepartment.DepartmentName != null && userInfo.UserDivision.DivisionName != null && !userInfo.IsAdmin) || userInfo.UserView)
            //{
            //    department = userInfo.UserDepartment.DepartmentName;
            //}

            //filteredList = Factory.Instance.GetFilteredOrdinances(statusID, department, division, title);

            //if (filteredList.Count > 0)
            //{
            //    foreach (Ordinance ord in filteredList)
            //    {
            //        OrdinanceStatus ordStatus = Factory.Instance.GetByID<OrdinanceStatus>(ord.OrdinanceID, "sp_GetOrdinanceStatusesByOrdinanceID", "OrdinanceID");
            //        ord.StatusDescription = ordStatus.StatusDescription;
            //    }
            //}

            //Dictionary<string, object> sortRet = new Dictionary<string, object>();

            //sortRet = GetCurrentSort(filteredList, Session["curCmd"].ToString(), Session["sortDir"].ToString());


            //Session["ord_list"] = sortRet["list"];
            //if (filteredList.Count > 0)
            //{
            //    formTableDiv.Visible = true;
            //    lblNoItems.Visible = false;
            //}
            //else
            //{
            //    formTableDiv.Visible = false;
            //    lblNoItems.Visible = true;
            //}

            //List<Label> departmentLabels = new List<Label>();
            //List<Label> divisionLabels = new List<Label>();

            //foreach (RepeaterItem item in rpOrdinanceTable.Items)
            //{
            //    Label deptLabel = (Label)item.FindControl("ordTableDepartment");
            //    Label divLabel = (Label)item.FindControl("ordTableDivision");
            //    departmentLabels.Add(deptLabel);
            //    divisionLabels.Add(divLabel);
            //}

            //switch (Session["DeptDivColumn"])
            //{
            //    case "department":
            //        foreach (Label item in departmentLabels)
            //        {
            //            item.Visible = true;
            //        }
            //        foreach (Label item in divisionLabels)
            //        {
            //            item.Visible = false;
            //        }
            //        break;
            //    case "division":
            //        foreach (Label item in departmentLabels)
            //        {
            //            item.Visible = false;
            //        }
            //        foreach (Label item in divisionLabels)
            //        {
            //            item.Visible = true;
            //        }
            //        break;
            //}
            Session["ViewState"] = ViewState;
        }
        protected void AddRequestEmailAddress_Click(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            TextBox address = (TextBox)pnlAdmin.FindControl(button.Attributes["data-email-text"]);
            string defaultType = button.CommandName.Replace(" ", "").Replace("/", "");

            
            Dictionary<string, object> listInfo = defaultListType[defaultType];
            Repeater repeater = (Repeater)listInfo["Repeater"];

            int defaultEmailID = new int();

            switch (button.CommandName)
            {
                default:
                    defaultEmailID = Convert.ToInt32(button.CommandArgument);
                    break;
                case "Director/Supervisor":
                    defaultEmailID = 12;
                    break;
            }

            DefaultEmails defaultList = Factory.Instance.GetByID<DefaultEmails>(defaultEmailID, "sp_GetDefaultEmailByDefaultEmailsID", "DefaultEmailsID");
            List<string> emails = new List<string>();
            if (!defaultList.ToString().IsNullOrWhiteSpace())
            {                
                emails = defaultList.EmailAddress.ToString().Split(';').Where(i => !i.IsNullOrWhiteSpace()).ToList();
            }

            string[] newEmailAddresses = address.Text.Split(';').Where(i => !i.IsNullOrWhiteSpace()).ToArray();
            foreach (string item in newEmailAddresses)
            {
                emails.Add(item);
            }
            string setEmails = string.Join(";", emails.OrderBy(i => i));
            defaultList.EmailAddress = setEmails.ToLower();

            int updateDefaultEmails = Factory.Instance.Update(defaultList, "sp_UpdateDefaultEmail");
            if (updateDefaultEmails > 0)
            {
                address.Text = string.Empty;
                if (emails.Count > 0)
                {
                    repeater.DataSource = emails.OrderBy(i => i);
                    repeater.DataBind();
                }
                else
                {
                    repeater.DataSource = null;
                    repeater.DataBind();
                }
            }
            ActiveTabPanes(hdnActiveTabs.Value);
        }
        protected void rpDefaultList_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            Dictionary<string, object> listInfo = defaultListType[e.CommandName];
            int id = new int();
            Repeater repeater = (Repeater)listInfo["Repeater"];

            switch (e.CommandName)
            {
                default:
                    id = (int)listInfo["ID"];
                    break;
                case "DirectorSupervisor":
                    id = 12;
                    break;
            }

            DefaultEmails defaultList = Factory.Instance.GetByID<DefaultEmails>(id, "sp_GetDefaultEmailByDefaultEmailsID", "DefaultEmailsID");
            List<string> emails = defaultList.EmailAddress.ToString().Split(';').Where(i => !i.IsNullOrWhiteSpace()).ToList();

            emails.Remove(e.CommandArgument.ToString());

            string setEmails = string.Join(";", emails.OrderBy(i => i));
            defaultList.EmailAddress = setEmails.ToLower();
            int updateDefaultEmails = Factory.Instance.Update(defaultList, "sp_UpdateDefaultEmail");
            if (updateDefaultEmails > 0)
            {                
                if (emails.Count > 0)
                {
                    repeater.DataSource = emails.OrderBy(i => i);
                    repeater.DataBind();
                }
                else
                {
                    repeater.DataSource = null;
                    repeater.DataBind();
                }
            }

            ActiveTabPanes(hdnActiveTabs.Value);
        }
        protected void rpDefaultList_ItemCreated(object sender, RepeaterItemEventArgs e)
        {
            LinkButton btn = (LinkButton)e.Item.FindControl("removeBtn");
            ScriptManager.GetCurrent(Page).RegisterAsyncPostBackControl(btn);
        }

        protected void CreateDefaultTypes_Click(object sender, EventArgs e)
        {
            Dictionary<string, string> departments = DepartmentsList();

            Dictionary<string, List<string>> defaultTypes = new Dictionary<string, List<string>>();
            foreach (var department in departments.Keys)
            {
                if (!department.Equals("Select Department..."))
                {
                    var value = departments[department];

                    List<Division> divisionList = GetDivisionsByDept(Convert.ToInt32(value));
                    List<string> divisions = new List<string>();
                    foreach (Division item in divisionList)
                    {
                        divisions.Add(item.DivisionName);
                    }

                    defaultTypes.Add(department, divisions);
                }
            }

            List<string> skips = new List<string>()
            {
                "DefaultEmailsID"
            };
            // THIS IS THE TEMPLATE FOR THE INSERT //
            foreach (KeyValuePair<string, List<string>> item in defaultTypes)
            {
                DefaultEmails defaultDept = new DefaultEmails()
                {
                    EmailAddress = string.Empty,
                    Department = item.Key,
                    Division = string.Empty,
                    DefaultType = "Director/Supervisor",
                    LastUpdateBy = "kbolinger",
                    LastUpdateDate = Convert.ToDateTime("2025-06-05 00:00:00"),
                    EffectiveDate = Convert.ToDateTime("2025-06-05 00:00:00"),
                    ExpirationDate = DateTime.MaxValue
                };
                int ret = Factory.Instance.Insert(defaultDept, "sp_InsertDefaultEmail", skips);
                Debug.WriteLine($"Department: {item.Key}");
                foreach (string division in item.Value)
                {
                    DefaultEmails defaultDiv = new DefaultEmails()
                    {
                        EmailAddress = string.Empty,
                        Department = item.Key,
                        Division = division,
                        DefaultType = "Director/Supervisor",
                        LastUpdateBy = "kbolinger",
                        LastUpdateDate = Convert.ToDateTime("2025-06-05 00:00:00"),
                        EffectiveDate = Convert.ToDateTime("2025-06-05 00:00:00"),
                        ExpirationDate = DateTime.MaxValue
                    };
                    int retVal = Factory.Instance.Insert(defaultDiv, "sp_InsertDefaultEmail", skips);
                    Debug.WriteLine($"Department: {item.Key}      |      Division: {division}");
                }
            }
        }
    }
}