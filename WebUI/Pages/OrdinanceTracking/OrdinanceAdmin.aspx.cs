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
                        {"Repeater", rpPendingDefaultList },
                        {"NoItems", lblNoItemsPending }
                    }
                },
                {"UnderReview", new Dictionary<string, object>()
                    {
                        {"ID", 2 },
                        {"Repeater", rpUnderReviewDefaultList },
                        {"NoItems", lblNoItemsUnderReview }
                    }
                },
                {"BeingHeld", new Dictionary<string, object>()
                    {
                        {"ID", 3 },
                        {"Repeater", rpBeingHeldDefaultList },
                        {"NoItems", lblNoItemsBeingHeld }
                    }
                },
                {"Drafted", new Dictionary<string, object>()
                    {
                        {"ID", 4 },
                        {"Repeater", rpDraftedDefaultList },
                        {"NoItems", lblNoItemsDrafted }
                    }
                },
                {"Approved", new Dictionary<string, object>()
                    {
                        {"ID", 5 },
                        {"Repeater", rpApprovedDefaultList },
                        {"NoItems", lblNoItemsApproved }
                    }
                },
                {"Rejected", new Dictionary<string, object>()
                    {
                        {"ID", 6 },
                        {"Repeater", rpRejectedDefaultList },
                        {"NoItems", lblNoItemsRejected }
                    }
                },
                {"Deleted", new Dictionary<string, object>()
                    {
                        {"ID", 7 },
                        {"Repeater", rpDeletedDefaultList },
                        {"NoItems", lblNoItemsDeleted }
                    }
                },

                {"FundsCheckBy", new Dictionary<string, object>()
                    {
                        {"ID", 8 },
                        {"Repeater", rpFundsCheckByDefaultList },
                        {"NoItems", lblNoItemsFundsCheckBy }
                    }
                },
                {"DirectorSupervisor", new Dictionary<string, object>()
                    {
                        {"Repeater", rpDirectorSupervisorDefaultList },
                        {"NoItems", lblNoItemsDirectorSupervisor },
                        {"NoItemsTxt", lblNoItemsTxtDirectorSupervisor }
                    }
                },
                {"CityPurchasingAgent", new Dictionary<string, object>()
                    {
                        {"ID", 9 },
                        {"Repeater", rpCPADefaultList },
                        {"NoItems", lblNoItemsCityPurchasingAgent }
                    }
                },
                {"OBMDirector", new Dictionary<string, object>()
                    {
                        {"ID", 10 },
                        {"Repeater", rpOBMDirectorDefaultList },
                        {"NoItems", lblNoItemsOBMDirector }
                    }
                },
                {"Mayor", new Dictionary<string, object>()
                    {
                        {"ID", 11 },
                        {"Repeater", rpMayorDefaultList },
                        {"NoItems", lblNoItemsMayor }
                    }
                },
            };
            


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
                HtmlGenericControl noItems = (HtmlGenericControl)listInfo["NoItems"];

                int defaultEmailID = new int();
                switch (listType.Key)
                {
                    default:
                        defaultEmailID = (int)listInfo["ID"];
                        break;
                    case "DirectorSupervisor":
                        if (!filterDepartment.SelectedValue.IsNullOrWhiteSpace())
                        {
                            string departmentName = filterDepartment.SelectedItem.Text;
                            string divisionName = !filterDivision.SelectedIndex.Equals(0) ? filterDivision.SelectedItem.Text : "None";
                            defaultEmailID = Factory.Instance.GetDefaultEmailsByDepartmentDivision(departmentName, divisionName).DefaultEmailsID;
                            directorSupervisorDefaultEmailAddress.Enabled = true;

                            HtmlGenericControl noItemsTxt = (HtmlGenericControl)listInfo["NoItemsTxt"];
                            noItemsTxt.InnerText = "There are no default emails set for the current department/division";
                            noItemsTxt.Attributes["class"] = "text-danger";
                        }
                        else
                        {
                            defaultEmailID = 0;
                            directorSupervisorDefaultEmailAddress.Enabled = false;

                            HtmlGenericControl noItemsTxt = (HtmlGenericControl)listInfo["NoItemsTxt"];
                            noItemsTxt.InnerText = "Please Select a Department and/or Division";
                            noItemsTxt.Attributes["class"] = "text-gray";
                        }
                            break;
                }

                DefaultEmails defaultList = new DefaultEmails();
                string[] emails = new string[0];
                if (defaultEmailID > 0)
                {
                    defaultList = Factory.Instance.GetByID<DefaultEmails>(defaultEmailID, "sp_GetDefaultEmailByDefaultEmailsID", "DefaultEmailsID");
                   emails = defaultList.EmailAddress.ToString().Split(';').Where(i => !i.IsNullOrWhiteSpace()).ToArray();
                }

                if (emails.Length > 0)
                {
                    noItems.Visible = false;
                    repeater.DataSource = emails;
                    repeater.DataBind();
                }
                else
                {
                    noItems.Visible = true;
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
            Dictionary<string, object> listInfo = defaultListType["DirectorSupervisor"];
            HtmlGenericControl noItems = (HtmlGenericControl)listInfo["NoItems"];

            int defaultEmailID = new int();
            if (!filterDepartment.SelectedValue.IsNullOrWhiteSpace())
            {
                string departmentName = filterDepartment.SelectedItem.Text;
                string divisionName = !filterDivision.SelectedIndex.Equals(0) ? filterDivision.SelectedItem.Text : "None";
                defaultEmailID = Factory.Instance.GetDefaultEmailsByDepartmentDivision(departmentName, divisionName).DefaultEmailsID;
                directorSupervisorDefaultEmailAddress.Enabled = true;

                HtmlGenericControl noItemsTxt = (HtmlGenericControl)listInfo["NoItemsTxt"];
                noItemsTxt.InnerText = "There are no default emails set for the current department/division";
                noItemsTxt.Attributes["class"] = "text-danger";
            }
            else
            {
                defaultEmailID = 0;
                directorSupervisorDefaultEmailAddress.Enabled = false;

                HtmlGenericControl noItemsTxt = (HtmlGenericControl)listInfo["NoItemsTxt"];
                noItemsTxt.InnerText = "Please Select a Department and/or Division";
                noItemsTxt.Attributes["class"] = "text-gray";
            }

            try
            {
                DropDownList dropDown = (DropDownList)sender;

                string commandName = dropDown.Attributes["data-command"]; string commandArgument = dropDown.SelectedItem.ToString();

                List<string> noDivisions = new List<string>()
                {
                    "City Clerk",
                    "Community Relations",
                    "Convention & Visitor's Bureau",
                    "Lincoln Library",
                };

                if (commandName.Equals("department"))
                {
                    filterDivision.Items.Clear();

                    if (!filterDepartment.SelectedValue.IsNullOrWhiteSpace() && !noDivisions.Any(i => filterDepartment.SelectedItem.Text.Equals(i)))
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

            DefaultEmails defaultList = new DefaultEmails();
            string[] emails = new string[0];
            if (defaultEmailID > 0)
            {
                defaultList = Factory.Instance.GetByID<DefaultEmails>(defaultEmailID, "sp_GetDefaultEmailByDefaultEmailsID", "DefaultEmailsID");
                emails = defaultList.EmailAddress.ToString().Split(';').Where(i => !i.IsNullOrWhiteSpace()).ToArray();
            }

            if (emails.Length > 0)
            {
                noItems.Visible = false;
                rpDirectorSupervisorDefaultList.DataSource = emails;
                rpDirectorSupervisorDefaultList.DataBind();
            }
            else
            {
                noItems.Visible = true;
                rpDirectorSupervisorDefaultList.DataSource = null;
                rpDirectorSupervisorDefaultList.DataBind();
            }

            directorSupervisorDefaultEmailAddress.Text = string.Empty;
        }
        protected void AddDefaultEmailAddress_Click(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            TextBox address = (TextBox)pnlAdmin.FindControl(button.Attributes["data-email-text"]);
            string defaultType = button.CommandName.Replace(" ", "").Replace("/", "");

            
            Dictionary<string, object> listInfo = defaultListType[defaultType];
            Repeater repeater = (Repeater)listInfo["Repeater"];
            HtmlGenericControl noItems = (HtmlGenericControl)listInfo["NoItems"];

            int defaultEmailID = new int();

            switch (button.CommandName)
            {
                default:
                    defaultEmailID = Convert.ToInt32(button.CommandArgument);
                    break;
                case "Director/Supervisor":
                    string departmentName = filterDepartment.SelectedItem.Text;
                    string divisionName = !filterDivision.SelectedIndex.Equals(0) ? filterDivision.SelectedItem.Text : "None";
                    defaultEmailID = Factory.Instance.GetDefaultEmailsByDepartmentDivision(departmentName, divisionName).DefaultEmailsID;
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
                emails.Add(item.ToLower());
            }
            string setEmails = string.Join(";", emails.OrderBy(i => i));
            defaultList.EmailAddress = setEmails.ToLower();
            defaultList.LastUpdateBy = _user.Login.ToLower();
            defaultList.LastUpdateDate = DateTime.Now;

            int updateDefaultEmails = Factory.Instance.Update(defaultList, "sp_UpdateDefaultEmail");
            if (updateDefaultEmails > 0)
            {
                address.Text = string.Empty;
                if (emails.Count > 0)
                {
                    noItems.Visible = false;
                    repeater.DataSource = emails.OrderBy(i => i);
                    repeater.DataBind();
                }
                else
                {
                    noItems.Visible = true;
                    repeater.DataSource = null;
                    repeater.DataBind();
                }
            }
            ActiveTabPanes(hdnActiveTabs.Value);
        }
        protected void rpDefaultList_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            Dictionary<string, object> listInfo = defaultListType[e.CommandName];
            int defaultEmailID = new int();
            Repeater repeater = (Repeater)listInfo["Repeater"];
            HtmlGenericControl noItems = (HtmlGenericControl)listInfo["NoItems"];

            switch (e.CommandName)
            {
                default:
                    defaultEmailID = (int)listInfo["ID"];
                    break;
                case "DirectorSupervisor":
                    string departmentName = filterDepartment.SelectedItem.Text;
                    string divisionName = !filterDivision.SelectedIndex.Equals(0) ? filterDivision.SelectedItem.Text : "None";
                    defaultEmailID = Factory.Instance.GetDefaultEmailsByDepartmentDivision(departmentName, divisionName).DefaultEmailsID;
                    break;
            }

            DefaultEmails defaultList = Factory.Instance.GetByID<DefaultEmails>(defaultEmailID, "sp_GetDefaultEmailByDefaultEmailsID", "DefaultEmailsID");
            List<string> emails = defaultList.EmailAddress.ToString().Split(';').Where(i => !i.IsNullOrWhiteSpace()).ToList();

            emails.Remove(e.CommandArgument.ToString());

            string setEmails = string.Join(";", emails.OrderBy(i => i));
            defaultList.EmailAddress = setEmails.ToLower();
            int updateDefaultEmails = Factory.Instance.Update(defaultList, "sp_UpdateDefaultEmail");
            if (updateDefaultEmails > 0)
            {                
                if (emails.Count > 0)
                {
                    noItems.Visible = false;
                    repeater.DataSource = emails.OrderBy(i => i);
                    repeater.DataBind();
                }
                else
                {
                    noItems.Visible = true;
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
    }
}