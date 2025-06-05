using DataLibrary;
using ISD.ActiveDirectory;
using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using static DataLibrary.TablePagination;
using static DataLibrary.Utility;

namespace WebUI
{
    public partial class OrdinanceAdmin : System.Web.UI.Page
    {
        private ADUser _user = new ADUser();
        public UserInfo userInfo = new UserInfo();

        protected void Page_Load(object sender, EventArgs e)
        {
            _user = Session["CurrentUser"] as ADUser;
            userInfo = Session["UserInformation"] as UserInfo;

            if (!Page.IsPostBack && !Response.IsRequestBeingRedirected)
            {
                if (!userInfo.IsAdmin || userInfo.UserView)
                {
                    Response.Redirect("./AccessDenied");
                }
                GetAllDepartments();
                SetStartupActives();
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


        protected void Filter_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Dictionary<string, LinkButton> pageBtns = new Dictionary<string, LinkButton>()
            //{
            //    { "firstBtn", lnkFirstSearchP },
            //    { "previousBtn", lnkPreviousSearchP },
            //    { "nextBtn", lnkNextSearchP },
            //    { "lastBtn", lnkLastSearchP },
            //};

            //SetPagination(rpOrdinanceTable, pageBtns, pnlPagingP, lblCurrentPageBottomSearchP, 10);

            fundsCheckByBtn.Attributes["class"] = "nav-link ordTabs";
            fundsCheckByBtn.Attributes["aria=selected"] = "false";
            directorSupervisorBtn.Attributes["class"] += " active";
            directorSupervisorBtn.Attributes["aria=selected"] = "true";
            fundsCheckByTabPane.Attributes["class"] = "tab-pane fade";
            directorSupervisorTabPane.Attributes["class"] += " active show";

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

            // THIS IS THE TEMPLATE FOR THE INSERT //
            foreach (KeyValuePair<string, List<string>> item in defaultTypes)
            {
                Debug.WriteLine($"Department: {item.Key}");
                foreach (string division in item.Value)
                {
                    Debug.WriteLine($"Department: {item.Key}      |      Division: {division}");
                }
                
            }

            Debug.WriteLine(defaultTypes.Count);
        }
    }
}