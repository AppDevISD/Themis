using DataLibrary;
using DataLibrary.OrdinanceTracking;
using ISD.ActiveDirectory;
using Microsoft.Ajax.Utilities;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using static DataLibrary.TablePagination;
using static DataLibrary.Utility;
using static System.Net.Mime.MediaTypeNames;

namespace WebUI
{
    public partial class Ordinances : System.Web.UI.Page
    {
        private ADUser _user = new ADUser();
        public UserInfo userInfo = new UserInfo();
        private readonly string emailList = HttpContext.Current.IsDebuggingEnabled ? "NewFactSheetEmailListTEST" : "NewFactSheetEmailList";
        public string deptDivColumnType = "RequestDepartment";
        
        public readonly List<string> lockedStatus = new List<string>()
        {
            "Pending",
            "Under Review",
            "Being Held",
            "Drafted",
            "Approved",
            "Deleted"
        };

        protected void Page_Load(object sender, EventArgs e)
        {
            Page.Form.Attributes.Add("enctype", "multipart/form-data");
            _user = Session["CurrentUser"] as ADUser;
            userInfo = Session["UserInformation"] as UserInfo;

            if (!Page.IsPostBack && !Response.IsRequestBeingRedirected)
            {
                Session.Remove("revenue");
                Session.Remove("expenditure");
                Session.Remove("ordDocs");
                Session.Remove("addOrdDocs");
                Session["sortBtn"] = "sortDate";
                Session["sortDir"] = "desc";
                Session["curCmd"] = "EffectiveDate";
                Session["curDir"] = "desc";
                Session["DeptDivColumn"] = "department";
                GetAllDepartments();
                GetAllStatuses();
                GetAllPurchaseMethods();
                SetStartupActives();
                Dictionary<string, LinkButton> pageBtns = new Dictionary<string, LinkButton>()
                {
                    { "firstBtn", lnkFirstSearchP },
                    { "previousBtn", lnkPreviousSearchP },
                    { "nextBtn", lnkNextSearchP },
                    { "lastBtn", lnkLastSearchP },
                };
                SetPagination(rpOrdinanceTable, pageBtns, pnlPagingP, lblCurrentPageBottomSearchP, 10);
                Session["ViewState"] = ViewState;
                GetStartupData(userInfo.IsAdmin);
            }            
            if (Page.IsPostBack && Page.Request.Params.Get("__EVENTTARGET").Contains("adminSwitch"))
            {
                userInfo = ((SiteMaster)Page.Master).UserView();
                Session.Remove("revenue");
                Session.Remove("expenditure");
                Session.Remove("ordDocs");
                Session.Remove("addOrdDocs");
                Session["sortBtn"] = "sortDate";
                Session["sortDir"] = "desc";
                Session["curCmd"] = "EffectiveDate";
                Session["curDir"] = "desc";
                SetStartupActives();
                Dictionary<string, LinkButton> pageBtns = new Dictionary<string, LinkButton>()
                {
                    { "firstBtn", lnkFirstSearchP },
                    { "previousBtn", lnkPreviousSearchP },
                    { "nextBtn", lnkNextSearchP },
                    { "lastBtn", lnkLastSearchP },
                };
                SetPagination(rpOrdinanceTable, pageBtns, pnlPagingP, lblCurrentPageBottomSearchP, 10);
                Session["ViewState"] = ViewState;
                filterStatus.SelectedIndex = 0;
                filterDepartment.SelectedIndex = 0;
                GetStartupData(userInfo.IsAdmin);
            }

            foreach (RepeaterItem item in rpOrdinanceTable.Items)
            {
                LinkButton editButton = item.FindControl("editOrd") as LinkButton;
                LinkButton viewButton = item.FindControl("viewOrd") as LinkButton;
                ScriptManager.GetCurrent(Page).RegisterAsyncPostBackControl(editButton);
                ScriptManager.GetCurrent(Page).RegisterAsyncPostBackControl(viewButton);
            }
            if (!Page.IsPostBack && Request.QueryString["id"] != null)
            {
                string id = Request.QueryString["id"];
                string cmd = Request.QueryString["v"] ?? "view";
                
                GetByID(id.ToString(), cmd.ToString());
                if (Request.QueryString["f"] != null)
                {
                    string ctrl = Request.QueryString["f"];
                    List<HtmlGenericControl> signBtnDivs = new List<HtmlGenericControl>()
                    {
                        fundsCheckByBtnDiv,
                        directorSupervisorBtnDiv,
                        cPABtnDiv,
                        obmDirectorBtnDiv,
                        mayorBtnDiv
                    };
                    List<Button> signBtns = new List<Button>()
                    {
                        fundsCheckByBtn,
                        directorSupervisorBtn,
                        cPABtn,
                        obmDirectorBtn,
                        mayorBtn
                    };
                    foreach (HtmlGenericControl item in signBtnDivs.Where(i => !i.ClientID.Equals($"{ctrl.ToString()}Div")))
                    {
                        item.Attributes["readonly"] = "true";
                    }
                    foreach (Button item in signBtns.Where(i => !i.ClientID.Equals(ctrl.ToString())))
                    {
                        item.Text = "Awaiting Signature...";
                    }
                }
            }

            if (ScriptManager.GetCurrent(Page).IsInAsyncPostBack)
            {
                
                ScriptManager.RegisterStartupScript(this, this.GetType(), "HideLoading", "hideLoadingModal();", true);
            }
            GetUploadedDocs();
        }



        // STARTUP DATA //
        protected void GetAllDepartments()
        {
            Dictionary<string, string> departments = DepartmentsList();
            foreach (var department in departments.Keys)
            {
                var value = departments[department];
                ListItem newItem = new ListItem(department, value);
                requestDepartment.Items.Add(newItem);
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
        protected void GetAllStatuses()
        {
            Dictionary<string, string> statuses = StatusList();
            foreach (var status in statuses.Keys)
            {
                var value = statuses[status];
                ListItem newItem = new ListItem(status, value);
                if (newItem.Text != "New" && newItem.Text != "Deleted")
                {
                    ddStatus.Items.Add(newItem);
                }
                filterStatus.Items.Add(newItem);
            }
        }
        protected void GetAllPurchaseMethods()
        {
            purchaseMethod.Items.Insert(0, new ListItem("Select Purchase Method...", null));
            purchaseMethod.Items.Insert(1, new ListItem("Low Bid", "Low Bid"));
            purchaseMethod.Items.Insert(2, new ListItem("Low Bid Meeting Specs", "Low Bid Meeting Specs"));
            purchaseMethod.Items.Insert(3, new ListItem("Low Evaluated Bid", "Low Evaluated Bid"));
            purchaseMethod.Items.Insert(4, new ListItem("Other", "Other"));
            purchaseMethod.Items.Insert(5, new ListItem("Exception", "Exception"));
        }
        protected void SetStartupActives()
        {
            ordTable.Visible = true;
            ordView.Visible = false;
            lblNoItems.Visible = false;
            ddDeptDivision.SelectedValue = "RequestDepartment";
            filterDepartmentDiv.Visible = !userInfo.IsAdmin || userInfo.UserView ? false : true;
            if (!userInfo.IsAdmin || userInfo.UserView)
            {
                filterDivision.Enabled = true;
                GetAllDivisions(filterDivision, userInfo.UserDepartment.DepartmentCode.ToString());
            }
            else if (!filterDepartment.SelectedValue.IsNullOrWhiteSpace())
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
        protected void SetPagination(Repeater rpTable, Dictionary<string, LinkButton> pageBtns, Panel pnlPaging, Label lblPage, int ItemsPerPage, bool GetViewState = false)
        {
            if (GetViewState)
            {
                ViewState["PgNumP"] = Convert.ToInt32(Session["ViewStatePage"]);
            }
            SetViewState(ViewState, ItemsPerPage);
            GetControls(pageBtns["firstBtn"], pageBtns["previousBtn"], pageBtns["nextBtn"], pageBtns["lastBtn"], rpTable, pnlPaging, lblPage);
        }
        protected void SetPaginationAudit(Repeater rpTable, int ItemsPerPage)
        {
            SetViewState(ViewState, ItemsPerPage);
            GetControls(lnkAuditFirstSearchP, lnkAuditPreviousSearchP, lnkAuditNextSearchP, lnkAuditLastSearchP, rpTable, pnlAuditPagingP, lblAuditCurrentPageBottomSearchP);
        }
        public void GetStartupData(bool isAdmin)
        {
            List<Ordinance> ord_list = new List<Ordinance>();
            ord_list = Factory.Instance.GetAllLookup<Ordinance>(0, "sp_GetOrdinanceByFilteredStatusID", "StatusID");
            if (ord_list.Count > 0)
            {
                
                if ((userInfo.UserDepartment.DepartmentName != null && userInfo.UserDivision.DivisionName != null && !isAdmin) || userInfo.UserView)
                {
                    ord_list = Factory.Instance.GetFilteredOrdinances(-1, userInfo.UserDepartment.DepartmentName, string.Empty, string.Empty);
                }
                foreach (Ordinance ord in ord_list)
                {
                    OrdinanceStatus ordStatus = Factory.Instance.GetByID<OrdinanceStatus>(ord.OrdinanceID, "sp_GetOrdinanceStatusesByOrdinanceID", "OrdinanceID");
                    ord.StatusDescription = ordStatus.StatusDescription;
                }
            }
            Dictionary<string, object> sortRet = new Dictionary<string, object>();
            sortRet = GetCurrentSort(ord_list, Session["curCmd"].ToString(), Session["sortDir"].ToString());

            Session["ord_list"] = sortRet["list"];
            Session["noFilterOrdList"] = ord_list;
            if (ord_list.Count > 0)
            {
                formTableDiv.Visible = true;
                lblNoItems.Visible = false;
            }
            else
            {
                formTableDiv.Visible = false;
                lblNoItems.Visible = true;
            }
        }
        protected void GetUploadedDocs()
        {
            if (Session["addOrdDocs"] != null && Session["ordDocs"] != null)
            {
                List<OrdinanceDocument> originalOrdDocList = Session["ordDocs"] as List<OrdinanceDocument>;
                //List<OrdinanceDocument> ordDocList = Session["addOrdDocs"] as List<OrdinanceDocument>;
                //originalOrdDocList.AddRange(ordDocList);
                rpSupportingDocumentation.DataSource = originalOrdDocList;
                rpSupportingDocumentation.DataBind();
            }
        }
        protected void GetByID(string id, string cmd)
        {
            CommandEventArgs eventArgs = new CommandEventArgs(cmd, id);
            RepeaterItem rpItem = new RepeaterItem(0, ListItemType.Item);
            RepeaterCommandEventArgs args = new RepeaterCommandEventArgs(rpItem, rpOrdinanceTable, eventArgs);
            rpOrdinanceTable_ItemCommand(rpOrdinanceTable, args);
        }



        // CONTROL CHANGES //
        protected void Filter_SelectedIndexChanged(object sender, EventArgs e)
        {
            Dictionary<string, LinkButton> pageBtns = new Dictionary<string, LinkButton>()
            {
                { "firstBtn", lnkFirstSearchP },
                { "previousBtn", lnkPreviousSearchP },
                { "nextBtn", lnkNextSearchP },
                { "lastBtn", lnkLastSearchP },
            };

            SetPagination(rpOrdinanceTable, pageBtns, pnlPagingP, lblCurrentPageBottomSearchP, 10);

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

            userInfo = (UserInfo)Session["UserInformation"];

            int statusID = !filterStatus.SelectedValue.IsNullOrWhiteSpace() ? Convert.ToInt32(filterStatus.SelectedValue) : -1;
            string department = !filterDepartment.SelectedValue.IsNullOrWhiteSpace() ? filterDepartment.SelectedItem.Text : string.Empty;
            string division = !filterDivision.SelectedValue.IsNullOrWhiteSpace() ? filterDivision.SelectedItem.Text : string.Empty;
            string title = !filterSearchTitle.Text.IsNullOrWhiteSpace() ? filterSearchTitle.Text : string.Empty;

            List<Ordinance> filteredList = new List<Ordinance>();
            if ((userInfo.UserDepartment.DepartmentName != null && userInfo.UserDivision.DivisionName != null && !userInfo.IsAdmin) || userInfo.UserView)
            {
                department = userInfo.UserDepartment.DepartmentName;
            }

            filteredList = Factory.Instance.GetFilteredOrdinances(statusID, department, division, title);

            if (filteredList.Count > 0)
            {
                foreach (Ordinance ord in filteredList)
                {
                    OrdinanceStatus ordStatus = Factory.Instance.GetByID<OrdinanceStatus>(ord.OrdinanceID, "sp_GetOrdinanceStatusesByOrdinanceID", "OrdinanceID");
                    ord.StatusDescription = ordStatus.StatusDescription;
                }
            }

            Dictionary<string, object> sortRet = new Dictionary<string, object>();

            sortRet = GetCurrentSort(filteredList, Session["curCmd"].ToString(), Session["sortDir"].ToString());


            Session["ord_list"] = sortRet["list"];
            if (filteredList.Count > 0)
            {
                formTableDiv.Visible = true;
                lblNoItems.Visible = false;
            }
            else
            {
                formTableDiv.Visible = false;
                lblNoItems.Visible = true;
            }

            List<Label> departmentLabels = new List<Label>();
            List<Label> divisionLabels = new List<Label>();

            foreach (RepeaterItem item in rpOrdinanceTable.Items)
            {
                Label deptLabel = (Label)item.FindControl("ordTableDepartment");
                Label divLabel = (Label)item.FindControl("ordTableDivision");
                departmentLabels.Add(deptLabel);
                divisionLabels.Add(divLabel);
            }

            switch (Session["DeptDivColumn"])
            {
                case "department":
                    foreach (Label item in departmentLabels)
                    {
                        item.Visible = true;
                    }
                    foreach (Label item in divisionLabels)
                    {
                        item.Visible = false;
                    }
                    break;
                case "division":
                    foreach (Label item in departmentLabels)
                    {
                        item.Visible = false;
                    }
                    foreach (Label item in divisionLabels)
                    {
                        item.Visible = true;
                    }
                    break;
            }
            Session["ViewState"] = ViewState;
        }
        protected void SortBtn_Click(object sender, EventArgs e)
        {
            Dictionary<string, LinkButton> pageBtns = new Dictionary<string, LinkButton>()
                {
                    { "firstBtn", lnkFirstSearchP },
                    { "previousBtn", lnkPreviousSearchP },
                    { "nextBtn", lnkNextSearchP },
                    { "lastBtn", lnkLastSearchP },
                };
            SetPagination(rpOrdinanceTable, pageBtns, pnlPagingP, lblCurrentPageBottomSearchP, 10);
            List<Ordinance> ord_list = new List<Ordinance>();
            ord_list = (List<Ordinance>)Session["ord_list"];
            LinkButton button = (LinkButton)sender;
            string commandName = button.Attributes["data-command"];
            string commandArgument = Session["sortDir"].ToString();
            string commandText = button.Attributes["data-text"];

            Dictionary<string, object> sortRet = new Dictionary<string, object>();

            string prvBtn = Session["sortBtn"].ToString();
            switch (button.ID.Equals(prvBtn))
            {
                case true:
                    sortRet = SortButtonClick(ord_list, commandName, commandArgument);
                    break;

                case false:
                    sortRet = SortButtonClick(ord_list, commandName, "asc");
                    break;
            }



            Session["sortBtn"] = button.ID;
            Session["sortDir"] = sortRet["dir"];
            Session["ord_list"] = sortRet["list"];
            Session["curDir"] = sortRet["curDir"];
            Session["curCmd"] = sortRet["curCmd"];

            List<LinkButton> sortButtonsList = new List<LinkButton>()
            {
                sortID,
                sortDate,
                sortTitle,
                sortDepartmentDivision,
                sortContact,
                sortStatus
            };
            foreach (LinkButton item in sortButtonsList)
            {
                switch (item.ID)
                {
                    case "sortDepartmentDivision":
                        item.Text = $"<strong><span runat='server' class='float-end lh-1p5'></span></strong>";
                        break;
                    default:
                        item.Text = $"<strong>{item.Attributes["data-text"]}<span runat='server' class='float-end lh-1p5'></span></strong>";
                        break;
                }
                
            }

            switch (button.ID)
            {
                case "sortDepartmentDivision":
                    button.Text = $"<strong><span runat='server' class='float-end lh-1p5 fas fa-arrow-{sortRet["arrow"]}'></span></strong>";
                    break;
                default:
                    button.Text = $"<strong>{commandText}<span runat='server' class='float-end lh-1p5 fas fa-arrow-{sortRet["arrow"]}'></span></strong>";
                    break;
            }

            List<Label> departmentLabels = new List<Label>();
            List<Label> divisionLabels = new List<Label>();

            foreach (RepeaterItem item in rpOrdinanceTable.Items)
            {
                Label deptLabel = (Label)item.FindControl("ordTableDepartment");
                Label divLabel = (Label)item.FindControl("ordTableDivision");
                departmentLabels.Add(deptLabel);
                divisionLabels.Add(divLabel);
            }

            switch (Session["DeptDivColumn"])
            {
                case "department":
                    foreach (Label item in departmentLabels)
                    {
                        item.Visible = true;
                    }
                    foreach (Label item in divisionLabels)
                    {
                        item.Visible = false;
                    }
                    break;
                case "division":
                    foreach (Label item in departmentLabels)
                    {
                        item.Visible = false;
                    }
                    foreach (Label item in divisionLabels)
                    {
                        item.Visible = true;
                    }
                    break;
            }
            Session["ViewState"] = ViewState;
        }
        protected void ddDeptDivision_SelectedIndexChanged(object sender, EventArgs e)
        {
            sortDepartmentDivision.Attributes.Remove("data-command");
            sortDepartmentDivision.Attributes.Remove("data-text");

            List<Label> departmentLabels = new List<Label>();
            List<Label> divisionLabels = new List<Label>();

            foreach (RepeaterItem item in rpOrdinanceTable.Items)
            {
                Label deptLabel = (Label)item.FindControl("ordTableDepartment");
                Label divLabel = (Label)item.FindControl("ordTableDivision");
                departmentLabels.Add(deptLabel);
                divisionLabels.Add(divLabel);
            }
            sortDepartmentDivision.Text = "<strong><span runat='server' class='float-end lh-1p5'></span></strong>";
            if (Session["sortBtn"].Equals("sortDepartmentDivision"))
            {
                Session["sortDir"] = "asc";
            }
            switch (ddDeptDivision.SelectedValue)
            {
                case "RequestDepartment":
                    foreach (Label item in departmentLabels)
                    {
                        item.Visible = true;
                    }
                    foreach (Label item in divisionLabels)
                    {
                        item.Visible = false;
                    }
                    break;
                case "RequestDivision":
                    foreach (Label item in departmentLabels)
                    {
                        item.Visible = false;
                    }
                    foreach (Label item in divisionLabels)
                    {
                        item.Visible = true;
                    }
                    break;
            }
            sortDepartmentDivision.Attributes.Add("data-command", ddDeptDivision.SelectedValue);
            sortDepartmentDivision.Attributes.Add("data-text", ddDeptDivision.SelectedItem.Text);
            Session["DeptDivColumn"] = ddDeptDivision.SelectedItem.Text.ToLower();
        }
        protected void btnDeptDivColumn_Click(object sender, EventArgs e)
        {
            sortDepartmentDivision.Attributes.Remove("data-command");
            sortDepartmentDivision.Attributes.Remove("data-text");
            List<Label> departmentLabels = new List<Label>();
            List<Label> divisionLabels = new List<Label>();

            foreach (RepeaterItem item in rpOrdinanceTable.Items)
            {
                Label deptLabel = (Label)item.FindControl("ordTableDepartment");
                Label divLabel = (Label)item.FindControl("ordTableDivision");
                departmentLabels.Add(deptLabel);
                divisionLabels.Add(divLabel);
            }

            Button btn = (Button)sender;
            switch (btn.CommandName)
            {
                case "department":
                    sortDepartmentDivision.Attributes.Add("data-command", "RequestDepartment");
                    sortDepartmentDivision.Attributes.Add("data-text", "Department");
                    foreach (Label item in departmentLabels)
                    {
                        item.Visible = true;
                    }
                    foreach (Label item in divisionLabels)
                    {
                        item.Visible = false;
                    }
                    Session["DeptDivColumn"] = "department";
                    break;
                case "division":
                    sortDepartmentDivision.Attributes.Add("data-command", "RequestDivision");
                    sortDepartmentDivision.Attributes.Add("data-text", "Division");
                    foreach (Label item in departmentLabels)
                    {
                        item.Visible = false;
                    }
                    foreach (Label item in divisionLabels)
                    {
                        item.Visible = true;
                    }
                    Session["DeptDivColumn"] = "division";
                    break;
            }
        }
        protected void paginationBtn_Click(object sender, EventArgs e)
        {
            LinkButton button = (LinkButton)sender;
            string commandName = button.Attributes["data-command"];
            string listType = button.Attributes["data-list"];
            Dictionary<string, LinkButton> pageBtns;
            switch (listType)
            {
                case "ordTable":
                    pageBtns = new Dictionary<string, LinkButton>()
                    {
                        { "firstBtn", lnkFirstSearchP },
                        { "previousBtn", lnkPreviousSearchP },
                        { "nextBtn", lnkNextSearchP },
                        { "lastBtn", lnkLastSearchP },
                    };
                    SetPagination(rpOrdinanceTable, pageBtns, pnlPagingP, lblCurrentPageBottomSearchP, 10, false);
                    List<Ordinance> ord_list = new List<Ordinance>();
                    ord_list = (List<Ordinance>)Session["ord_list"];
                    PageButtonClick(ord_list, commandName);
                    Session["ViewState"] = ViewState;
                    break;
                case "auditTable":
                    pageBtns = new Dictionary<string, LinkButton>()
                    {
                        { "firstBtn", lnkAuditFirstSearchP },
                        { "previousBtn", lnkAuditPreviousSearchP },
                        { "nextBtn", lnkAuditNextSearchP },
                        { "lastBtn", lnkAuditLastSearchP },
                    };
                    SetPagination(rpAudit, pageBtns, pnlAuditPagingP, lblAuditCurrentPageBottomSearchP, 11);
                    List<OrdinanceAudit> audit_list = new List<OrdinanceAudit>();
                    audit_list = (List<OrdinanceAudit>)Session["ordAudit"];
                    PageButtonClick(audit_list, commandName);
                    factSheetTab.Attributes["class"] = "nav-link ordTabs";
                    factSheetTab.Attributes["aria=selected"] = "false";
                    auditTab.Attributes["class"] += " active";
                    auditTab.Attributes["aria=selected"] = "true";
                    factSheetPane.Attributes["class"] = "tab-pane fade";
                    auditPane.Attributes["class"] += " active show";
                    break;
            }
        }
        public bool adminUnlockedOrd(string status)
        {
            bool unlock = false;
            if (userInfo.IsAdmin && !userInfo.UserView)
            {
                if (status.Equals("Deleted"))
                {
                    unlock = false;
                }
                else
                {
                    unlock = true;
                }
            }
            else if (lockedStatus.Any(i => i.Equals(status)))
            {
                unlock = false;
            }
            else
            {
                unlock = true;
            }
            return unlock;
        }
        protected void ddStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            signatureSection.Visible = true;
            if (ddStatus.SelectedItem.Text.Equals("Rejected"))
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "OpenRejectionModal", "OpenRejectionModal();", true);
            }
        }
        protected void requestDepartment_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!requestDepartment.SelectedValue.IsNullOrWhiteSpace())
            {
                requestDivision.Enabled = true;
                GetAllDivisions(requestDivision, requestDepartment.SelectedValue);
            }
            else
            {
                requestDivision.Enabled = false;
                requestDivision.Items.Add(new ListItem() { Text = "Select Division...", Value = "" });
            }
        }
        protected void EPCheckedChanged(object sender, EventArgs e)
        {
            switch (epYes.Checked)
            {
                case true:
                    epJustificationGroup.Visible = true;
                    epJustification.Attributes.Add("data-required", "true");
                    epJustificationValid.Enabled = true;
                    break;

                case false:
                    epJustificationGroup.Visible = false;
                    epJustification.Attributes.Remove("data-required");
                    epJustificationValid.Enabled = false;
                    break;
            }
        }
        protected void SCCheckedChanged(object sender, EventArgs e)
        {
            switch (scYes.Checked)
            {
                case true:
                    changeOrderNumber.Enabled = true;
                    changeOrderNumberValid.Enabled = true;
                    additionalAmount.Enabled = true;
                    additionalAmountValid.Enabled = true;
                    changeOrderNumber.Attributes.Add("data-required", "true");
                    changeOrderNumber.Attributes.Add("placeholder", "0123456789");
                    additionalAmount.Attributes.Add("data-required", "true");
                    additionalAmount.Attributes.Add("placeholder", "$0.00");
                    break;

                case false:
                    changeOrderNumber.Enabled = false;
                    changeOrderNumberValid.Enabled = false;
                    additionalAmount.Enabled = false;
                    additionalAmountValid.Enabled = false;
                    changeOrderNumber.Attributes.Remove("data-required");
                    changeOrderNumber.Attributes.Remove("placeholder");
                    additionalAmount.Attributes.Remove("data-required");
                    additionalAmount.Attributes.Remove("placeholder");
                    break;
            }
        }
        protected void PurchaseMethodSelectedIndexChanged(object sender, EventArgs e)
        {
            switch (purchaseMethod.SelectedItem.Value)
            {
                default:
                    otherException.Enabled = false;
                    otherExceptionValid.Enabled = false;
                    otherException.Text = string.Empty;
                    otherException.Attributes.Remove("data-required");
                    break;
                case "Other":
                case "Exception":
                    otherException.Enabled = true;
                    otherExceptionValid.Enabled = true;
                    otherException.Attributes.Add("data-required", "true");
                    break;
            }
        }
        protected void UploadDocBtn_Click(object sender, EventArgs e)
        {
            List<OrdinanceDocument> originalOrdDocList = new List<OrdinanceDocument>();
            List<OrdinanceDocument> ordDocList = new List<OrdinanceDocument>();
            if (Session["ordDocs"] != null)
            {
                originalOrdDocList = Session["ordDocs"] as List<OrdinanceDocument>;
            }
            if (Session["addOrdDocs"] != null)
            {
                ordDocList = Session["addOrdDocs"] as List<OrdinanceDocument>;
            }
            

            for (int i = 0; i < supportingDocumentation.PostedFiles.Count; i++)
            {
                OrdinanceDocument ordDoc = new OrdinanceDocument();
                ordDoc.DocumentName = supportingDocumentation.PostedFiles[i].FileName;
                Stream stream = supportingDocumentation.PostedFiles[i].InputStream;
                using (var fileBytes = new BinaryReader(stream))
                {
                    ordDoc.DocumentData = fileBytes.ReadBytes((int)stream.Length);
                }
                ordDoc.LastUpdateBy = _user.Login;
                ordDoc.LastUpdateDate = DateTime.Now;
                ordDoc.EffectiveDate = DateTime.Now;
                ordDoc.ExpirationDate = DateTime.MaxValue;
                ordDocList.Add(ordDoc);
            }
            Session["addOrdDocs"] = ordDocList;
            originalOrdDocList.AddRange(ordDocList);
            Session["ordDocs"] = originalOrdDocList;
            rpSupportingDocumentation.DataSource = originalOrdDocList;
            rpSupportingDocumentation.DataBind();
        }
        protected void signatureEmailBtn_Click(object sender, EventArgs e)
        {
            LinkButton btn = (LinkButton)sender;
            string[] args = btn.CommandArgument.Split(';');
            SignatureRequest sigRequests = Session["SigRequestEmails"] as SignatureRequest;
            PropertyInfo sigType = (PropertyInfo)typeof(SignatureRequest).GetProperties().First(i => i.Name.Equals(btn.CommandName)); ;
            string[] emails = sigType.GetValue(sigRequests).ToString().Split(';').Where(i => !i.IsNullOrWhiteSpace()).ToArray();
            if (emails.Length > 0)
            {
                emailListDiv.Visible = true;
                btnSendSigEmail.Enabled = true;
                rpEmailList.DataSource = emails;
                rpEmailList.DataBind();
            }
            else
            {
                emailListDiv.Visible = false;
                btnSendSigEmail.Enabled = false;
                rpEmailList.DataSource = null;
                rpEmailList.DataBind();
            }
            ScriptManager.RegisterStartupScript(this, this.GetType(), "OpenSigEmailModal", $"setEmailModal('{args[0]}', '{args[1]}', '{btn.CommandName}');", true);
        }
        protected void AddRequestEmailAddress_Click(object sender, EventArgs e)
        {
            SignatureRequest sigRequests = Session["SigRequestEmails"] as SignatureRequest;
            PropertyInfo sigType = (PropertyInfo)typeof(SignatureRequest).GetProperties().First(i => i.Name.Equals(sigBtnType.Value));

            List<string> emails = sigType.GetValue(sigRequests).ToString().Split(';').Where(i => !i.IsNullOrWhiteSpace()).ToList();
            string[] newEmailAddresses = signatureEmailAddress.Text.Split(';').Where(i => !i.IsNullOrWhiteSpace()).ToArray();
            foreach (string item in newEmailAddresses)
            {
                emails.Add(item);
            }
            sigType.SetValue(sigRequests, string.Join(";", emails.OrderBy(i => i)));
            if (emails.Count > 0)
            {
                emailListDiv.Visible = true;
                btnSendSigEmail.Enabled = true;
                rpEmailList.DataSource = emails.OrderBy(i => i);
                rpEmailList.DataBind();
            }
            else
            {
                emailListDiv.Visible = false;
                btnSendSigEmail.Enabled = false;
                rpEmailList.DataSource = null;
                rpEmailList.DataBind();
            }

            int updateSigRequest = Factory.Instance.Update(sigRequests, "sp_UpdateOrdinance_SignatureRequest");
            if (updateSigRequest > 0)
            {
                signatureEmailAddress.Text = string.Empty;
            }
        }
      


        // REPEATER COMMANDS //        
        protected void rpOrdinanceTable_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            ScriptManager.GetCurrent(Page).RegisterAsyncPostBackControl(backBtn);

            Session.Remove("OriginalOrdinance");
            Session.Remove("OriginalStatus");
            Session.Remove("OriginalRevTable");
            Session.Remove("OriginalExpTable");
            Session.Remove("SigRequestEmails");


            Session.Remove("revenue");
            Session.Remove("expenditure");
            Session.Remove("insertSigList");            

            List<TextBox> sigTextBoxes = new List<TextBox>()
            {
                fundsCheckBySig,
                directorSupervisorSig,
                cPASig,
                obmDirectorSig,
                mayorSig
            };
            List<LinkButton> emailBtns = new List<LinkButton>() 
            {
                fundsCheckEmailBtn,
                directorSupervisorEmailBtn,
                cPAEmailBtn,
                obmDirectorEmailBtn,
                mayorEmailBtn,
            };
            List<Button> signBtns = new List<Button>()
            {
                fundsCheckByBtn,
                directorSupervisorBtn,
                cPABtn,
                obmDirectorBtn,
                mayorBtn
            };
            List<HtmlGenericControl> sigRows = new List<HtmlGenericControl>()
            {
                fundsCheckRow,
                directorSupervisorRow,
                cPARow,
                obmDirectorRow,
                mayorRow
            };

            int ordID = Convert.ToInt32(e.CommandArgument);
            Ordinance ord = Factory.Instance.GetByID<Ordinance>(ordID, "sp_GetOrdinanceByOrdinanceID", "OrdinanceID");
            hdnOrdID.Value = ordID.ToString();
            hdnEffectiveDate.Value = ord.EffectiveDate.ToString();

            lblOrdID.Text = $"ID: {ordID.ToString()}";
            ddStatus.SelectedValue = ord.StatusDescription;
            ordinanceNumber.Text = ord.OrdinanceNumber;
            requestDepartment.SelectedValue = DepartmentsList()[ord.RequestDepartment];

            requestDivision.Enabled = true;
            GetAllDivisions(requestDivision, requestDepartment.SelectedValue);
            requestDivision.SelectedValue = GetDivisionsByDept(Convert.ToInt32(requestDepartment.SelectedValue)).First(i => i.DivisionName.Equals(ord.RequestDivision)).DivisionCode.ToString();

            firstReadDate.Text = ord.FirstReadDate.ToString("yyyy-MM-dd");
            requestContact.Text = ord.RequestContact;
            requestEmail.Text = ord.RequestEmail;
            requestPhone.Text = ord.RequestPhone.SubstringUpToFirst('x');
            requestExt.Text = ord.RequestPhone.Substring(14);

            hdnEmail.Value = ord.RequestEmail;

            switch (ord.EmergencyPassage)
            {
                case true:
                    epYes.Checked = true;
                    epNo.Checked = false;
                    epJustificationGroup.Visible = true;
                    epJustificationValid.Enabled = true;
                    break;
                case false:
                    epYes.Checked = false;
                    epNo.Checked = true;
                    epJustificationGroup.Visible = false;
                    epJustificationValid.Enabled = false;
                    break;
            }
            epJustification.Text = ord.EmergencyPassageReason;

            fiscalImpact.Text = NotApplicable(ord.OrdinanceFiscalImpact.ToString());
            suggestedTitle.Text = ord.OrdinanceTitle;

            vendorName.Text = ord.ContractVendorName;
            vendorNumber.Text = ord.ContractVendorNumber;
            contractStartDate.Text = ord.ContractStartDate;
            contractEndDate.Text = ord.ContractEndDate;
            contractTerm.Value = ord.ContractTerm;
            contractAmount.Text = NotApplicable(ord.ContractAmount.ToString());

            switch (ord.ScopeChange)
            {
                case true:
                    scYes.Checked = true;
                    scNo.Checked = false;
                    scopeChangeOptions.Visible = true;
                    changeOrderNumberValid.Enabled = true;
                    additionalAmountValid.Enabled = true;
                    break;
                case false:
                    scYes.Checked = false;
                    scNo.Checked = true;
                    scopeChangeOptions.Visible = false;
                    changeOrderNumberValid.Enabled = false;
                    additionalAmountValid.Enabled = false;
                    break;
            }
            changeOrderNumber.Text = ord.ChangeOrderNumber;
            additionalAmount.Text = NotApplicable(ord.AdditionalAmount.ToString());


            purchaseMethod.SelectedValue = ord.ContractMethod;
            switch (purchaseMethod.SelectedValue)
            {
                default:
                    otherExceptionDiv.Visible = false;
                    otherExceptionValid.Enabled = false;
                    break;
                case "Other":
                case "Exception":
                    otherExceptionDiv.Visible = true;
                    otherExceptionValid.Enabled = true;
                    break;
            }
            otherException.Text = ord.OtherException;
            prevOrdinanceNums.Text = ord.PreviousOrdinanceNumbers;
            codeProvision.Text = ord.CodeProvision;

            switch (ord.PAApprovalRequired)
            {
                case true:
                    paApprovalRequiredYes.Checked = true;
                    paApprovalRequiredNo.Checked = false;
                    break;
                case false:
                    paApprovalRequiredYes.Checked = false;
                    paApprovalRequiredNo.Checked = true;
                    break;
            }
            switch (ord.PAApprovalIncluded)
            {
                case true:
                    paApprovalAttachedYes.Checked = true;
                    paApprovalAttachedNo.Checked = false;
                    break;
                case false:
                    paApprovalAttachedYes.Checked = false;
                    paApprovalAttachedNo.Checked = true;
                    break;
            }

            List<OrdinanceAccounting> ordAcc = Factory.Instance.GetAllLookup<OrdinanceAccounting>(ordID, "sp_GetOrdinanceAccountingByOrdinanceID", "OrdinanceID");
            List<OrdinanceAccounting> revItems = new List<OrdinanceAccounting>();
            List<OrdinanceAccounting> expItems = new List<OrdinanceAccounting>();
            if (ordAcc.Count > 0)
            {
                foreach (OrdinanceAccounting item in ordAcc)
                {
                    switch (item.AccountingDesc)
                    {
                        case "revenue":
                            revItems.Add(item);
                            break;
                        case "expenditure":
                            expItems.Add(item);
                            break;
                    }
                }

                if (revItems.Count > 0)
                {
                    Session["revenue"] = revItems;
                    Session["OriginalRevTable"] = revItems;
                    rpRevenueTable.DataSource = revItems.OrderBy(i => i.OrdinanceAccountingID);
                    rpRevenueTable.DataBind();
                }
                else
                {
                    Session.Remove("revenue");
                    rpRevenueTable.DataSource = null;
                    rpRevenueTable.DataBind();
                }
                if (expItems.Count > 0)
                {
                    Session["expenditure"] = expItems;
                    Session["OriginalExpTable"] = expItems;
                    rpExpenditureTable.DataSource = expItems.OrderBy(i => i.OrdinanceAccountingID);
                    rpExpenditureTable.DataBind();
                }
                else
                {
                    Session.Remove("expenditure");
                    rpExpenditureTable.DataSource = null;
                    rpExpenditureTable.DataBind();
                }
            }
            else
            {
                Session.Remove("revenue");
                Session.Remove("expenditure");
                rpRevenueTable.DataSource = null;
                rpExpenditureTable.DataSource = null;
                rpRevenueTable.DataBind();
                rpExpenditureTable.DataBind();
            }

            List<OrdinanceDocument> ordDocs = Factory.Instance.GetAllLookup<OrdinanceDocument>(ordID, "sp_GetOrdinanceDocumentsByOrdinanceID", "OrdinanceID");
            if (ordDocs.Count > 0)
            {
                supportingDocumentationDiv.Visible = true;
                Session["ordDocs"] = ordDocs;
                rpSupportingDocumentation.DataSource = ordDocs.OrderBy(i => i.DocumentID);
                rpSupportingDocumentation.DataBind();

                foreach (RepeaterItem item in rpSupportingDocumentation.Items)
                {
                    LinkButton downloadFile = item.FindControl("supportingDocDownload") as LinkButton;
                    ScriptManager.GetCurrent(Page).RegisterPostBackControl(downloadFile);
                }
            }
            else
            {
                Session.Remove("ordDocs");
                rpSupportingDocumentation.DataSource = null;
                rpSupportingDocumentation.DataBind();
                supportingDocumentationDiv.Visible = false;
            }

            staffAnalysis.Text = ord.OrdinanceAnalysis;

            List<OrdinanceSignature> ordSigs = Factory.Instance.GetAllLookup<OrdinanceSignature>(ordID, "sp_GetOrdinanceSignatureByOrdinanceID", "OrdinanceID");   
            
            SignatureRequest signatureRequest = Factory.Instance.GetByID<SignatureRequest>(Convert.ToInt32(hdnOrdID.Value), "sp_GetOrdinanceSignatureRequestByOrdinanceID", "OrdinanceID");
            Session["SigRequestEmails"] = signatureRequest;

            OrdinanceStatus ordStatus = new OrdinanceStatus();

            Session["ViewStatePage"] = ViewState["PgNumP"];
            List<OrdinanceAudit> ordAudits = Factory.Instance.GetAllLookup<OrdinanceAudit>(ordID, "sp_GetOrdinanceAuditByOrdinanceID", "OrdinanceID");
            Dictionary<string, LinkButton> pageBtns = new Dictionary<string, LinkButton>()
            {
                { "firstBtn", lnkAuditFirstSearchP },
                { "previousBtn", lnkAuditPreviousSearchP },
                { "nextBtn", lnkAuditNextSearchP },
                { "lastBtn", lnkAuditLastSearchP },
            };
            SetPagination(rpAudit, pageBtns, pnlAuditPagingP, lblAuditCurrentPageBottomSearchP, 11);
            if (ordAudits.Count > 0)
            {
                Session["ordAudit"] = ordAudits;
                BindDataRepeaterPagination("yes", ordAudits);
            }
            else
            {
                Session.Remove("ordAudit");
                rpAudit.DataSource = null;
                rpAudit.DataBind();
            }

            switch (e.CommandName)
            {
                case "view":
                    ordView.Attributes.Add("readonly", "true");
                    ordinanceTabs.Visible = true;
                    copyOrd.Visible = Request.QueryString["id"] == null;
                    ddStatusDiv.Visible = false;
                    statusDiv.Visible = true;
                    requiredFieldDescriptor.Visible = false;
                    firstReadDatePicker.Visible = false;
                    contractStartDatePicker.Visible = false;
                    contractEndDatePicker.Visible = false;
                    ordinanceNumber.Attributes["placeholder"] = "N/A";
                    agendaNumber.Attributes["placeholder"] = "N/A";
                    fiscalImpact.Attributes["placeholder"] = "N/A";
                    vendorNumber.Attributes["placeholder"] = "N/A";
                    contractTerm.Attributes["placeholder"] = "N/A";
                    contractAmount.Attributes["placeholder"] = "N/A";
                    prevOrdinanceNums.Attributes["placeholder"] = "N/A";
                    codeProvision.Attributes["placeholder"] = "N/A";
                    newRevenueRowDiv.Visible = false;
                    newExpenditureRowDiv.Visible = false;
                    supportingDocumentation.Visible = false;
                    uploadBtn.Visible = false;
                    submitSection.Visible = false;
                    if (rpRevenueTable.Items.Count > 0)
                    {
                        foreach (RepeaterItem item in rpRevenueTable.Items)
                        {
                            HtmlGenericControl removeRevRow = item.FindControl("removeRevRowDiv") as HtmlGenericControl;
                            TextBox revAmount = item.FindControl("revenueAmount") as TextBox;
                            removeRevRow.Visible = false;
                            revAmount.Attributes["placeholder"] = "N/A";
                        }
                    }
                    if (rpExpenditureTable.Items.Count > 0)
                    {
                        foreach (RepeaterItem item in rpExpenditureTable.Items)
                        {
                            HtmlGenericControl removeExpRow = item.FindControl("removeExpRowDiv") as HtmlGenericControl;
                            TextBox expAmount = item.FindControl("expenditureAmount") as TextBox;
                            removeExpRow.Visible = false;
                            expAmount.Attributes["placeholder"] = "N/A";
                        }
                    }
                    foreach (RepeaterItem item in rpSupportingDocumentation.Items)
                    {
                        LinkButton deleteFile = item.FindControl("deleteFile") as LinkButton;
                        deleteFile.Visible = false;
                    }

                    if (ord.ContractStartDate.Length < 1)
                    {
                        contractStartDate.TextMode = TextBoxMode.SingleLine;
                        contractStartDate.Attributes["placeholder"] = "N/A";
                    }
                    else
                    {
                        contractStartDate.TextMode = TextBoxMode.Date;
                    }

                    if (ord.ContractEndDate.Length < 1)
                    {
                        contractEndDate.TextMode = TextBoxMode.SingleLine;
                        contractEndDate.Attributes["placeholder"] = "N/A";
                    }
                    else
                    {
                        contractEndDate.TextMode = TextBoxMode.Date;
                    }

                    otherException.Enabled = true;
                    changeOrderNumber.Enabled = true;
                    additionalAmount.Enabled = true;

                    ordStatus = Factory.Instance.GetByID<OrdinanceStatus>(ord.OrdinanceID, "sp_GetOrdinanceStatusesByOrdinanceID", "OrdinanceID");
                    ord.StatusDescription = ordStatus.StatusDescription;
                    statusLabel.InnerHtml = ord.StatusDescription;
                    switch (ord.StatusDescription)
                    {
                        case "New":
                            statusIcon.Attributes["class"] = "fas fa-sparkles text-primary";
                            statusLabel.Attributes["class"] = "text-primary";
                            copyOrd.Visible = false;
                            ordinanceNumberDiv.Visible = false;
                            foreach (HtmlGenericControl row in sigRows)
                            {
                                row.Visible = false;
                            }
                            directorSupervisorRow.Visible = true;
                            break;
                        case "Pending":
                            statusIcon.Attributes["class"] = "fas fa-hourglass-clock text-warning-light";
                            statusLabel.Attributes["class"] = "text-warning-light";
                            ordinanceNumberDiv.Visible = true;
                            foreach (HtmlGenericControl row in sigRows)
                            {
                                row.Visible = true;
                            }
                            break;
                        case "Under Review":
                            statusIcon.Attributes["class"] = "fa-kit fa-solid-memo-magnifying-glass text-info";
                            statusLabel.Attributes["class"] = "text-info";
                            ordinanceNumberDiv.Visible = true;
                            foreach (HtmlGenericControl row in sigRows)
                            {
                                row.Visible = true;
                            }
                            break;
                        case "Being Held":
                            statusIcon.Attributes["class"] = "fas fa-triangle-exclamation text-hazard";
                            statusLabel.Attributes["class"] = "text-hazard";
                            ordinanceNumberDiv.Visible = true;
                            foreach (HtmlGenericControl row in sigRows)
                            {
                                row.Visible = true;
                            }
                            break;
                        case "Drafted":
                            statusIcon.Attributes["class"] = "fa-kit fa-solid-file-contract-circle-check text-drafted";
                            statusLabel.Attributes["class"] = "text-drafted";
                            ordinanceNumberDiv.Visible = true;
                            foreach (HtmlGenericControl row in sigRows)
                            {
                                row.Visible = true;
                            }
                            break;
                        case "Approved":
                            statusIcon.Attributes["class"] = "fas fa-badge-check text-success";
                            statusLabel.Attributes["class"] = "text-success";
                            ordinanceNumberDiv.Visible = true;
                            foreach (HtmlGenericControl row in sigRows)
                            {
                                row.Visible = true;
                            }
                            break;
                        case "Rejected":
                            statusIcon.Attributes["class"] = "fas fa-ban text-danger";
                            statusLabel.Attributes["class"] = "text-danger";
                            ordinanceNumberDiv.Visible = true;
                            foreach (HtmlGenericControl row in sigRows)
                            {
                                row.Visible = true;
                            }
                            break;
                        case "Deleted":
                            statusIcon.Attributes["class"] = "fas fa-trash-xmark text-danger";
                            statusLabel.Attributes["class"] = "text-danger";
                            ordinanceNumberDiv.Visible = true;
                            foreach (HtmlGenericControl row in sigRows)
                            {
                                row.Visible = true;
                            }
                            break;
                    }

                    foreach (LinkButton item in emailBtns)
                    {
                        item.Visible = false;
                    }

                    foreach (TextBox item in sigTextBoxes) if (!item.Text.IsNullOrWhiteSpace())
                        {
                            switch (item.ClientID)
                            {
                                case "fundsCheckBySig":
                                    fundsCheckByBtnDiv.Visible = false;
                                    fundsCheckByInputGroup.Visible = true;
                                    break;
                                case "directorSupervisorSig":
                                    directorSupervisorBtnDiv.Visible = false;
                                    directorSupervisorInputGroup.Visible = true;
                                    break;
                                case "cPASig":
                                    cPABtnDiv.Visible = false;
                                    cPAInputGroup.Visible = true;
                                    break;
                                case "obmDirectorSig":
                                    obmDirectorBtnDiv.Visible = false;
                                    obmDirectorInputGroup.Visible = true;
                                    break;
                                case "mayorSig":
                                    mayorBtnDiv.Visible = false;
                                    mayorInputGroup.Visible = true;
                                    break;
                            }
                        }
                    else
                        {
                            switch (item.ClientID)
                            {
                                case "fundsCheckBySig":
                                    fundsCheckByBtnDiv.Visible = true;
                                    fundsCheckByBtn.Text = "Awaiting Signature...";
                                    fundsCheckByInputGroup.Visible = false;
                                    break;
                                case "directorSupervisorSig":
                                    directorSupervisorBtnDiv.Visible = true;
                                    directorSupervisorBtn.Text = "Awaiting Signature...";
                                    directorSupervisorInputGroup.Visible = false;
                                    break;
                                case "cPASig":
                                    cPABtnDiv.Visible = true;
                                    cPABtn.Text = "Awaiting Signature...";
                                    cPAInputGroup.Visible = false;
                                    break;
                                case "obmDirectorSig":
                                    obmDirectorBtnDiv.Visible = true;
                                    obmDirectorBtn.Text = "Awaiting Signature...";
                                    obmDirectorInputGroup.Visible = false;
                                    break;
                                case "mayorSig":
                                    mayorBtnDiv.Visible = true;
                                    mayorBtn.Text = "Awaiting Signature...";
                                    mayorInputGroup.Visible = false;
                                    break;
                            }
                        }
                    break;
                case "edit":
                    ordView.Attributes.Remove("readonly");
                    ordinanceTabs.Visible = false;
                    copyOrd.Visible = false;
                    firstReadDatePicker.Visible = true;
                    contractStartDatePicker.Visible = true;
                    contractEndDatePicker.Visible = true;
                    ordStatus = Factory.Instance.GetByID<OrdinanceStatus>(ord.OrdinanceID, "sp_GetOrdinanceStatusesByOrdinanceID", "OrdinanceID");
                    ord.StatusDescription = ordStatus.StatusDescription;
                    bool adminUser = (userInfo.IsAdmin || !userInfo.UserView) ? true : false;
                    hdnStatusID.Value = ordStatus.StatusID.ToString();
                    if (!adminUser || ord.StatusDescription.Equals("New"))
                    {
                        ddStatusDiv.Visible = false;
                        statusDiv.Visible = true;
                    }
                    else
                    {
                        ddStatus.SelectedValue = ordStatus.StatusID.ToString();
                        ddStatusDiv.Visible = true;
                        statusDiv.Visible = false;
                    }
                    statusLabel.InnerHtml = ord.StatusDescription;
                    switch (ord.StatusDescription)
                    {
                        case "New":
                            statusIcon.Attributes["class"] = "fas fa-sparkles text-primary";
                            statusLabel.Attributes["class"] = "text-primary";
                            signatureSection.Visible = true;
                            foreach (HtmlGenericControl row in sigRows)
                            {
                                row.Visible = false;
                            }
                            directorSupervisorRow.Visible = true;
                            break;
                        case "Pending":
                            statusIcon.Attributes["class"] = "fas fa-hourglass-clock text-warning-light";
                            statusLabel.Attributes["class"] = "text-warning-light";
                            signatureSection.Visible = true;
                            foreach (HtmlGenericControl row in sigRows)
                            {
                                row.Visible = true;
                            }
                            break;
                        case "Under Review":
                            statusIcon.Attributes["class"] = "fas fa-memo-circle-info text-info";
                            statusLabel.Attributes["class"] = "text-info";
                            signatureSection.Visible = true;
                            foreach (HtmlGenericControl row in sigRows)
                            {
                                row.Visible = true;
                            }
                            break;
                        case "Being Held":
                            statusIcon.Attributes["class"] = "fas fa-triangle-exclamation text-hazard";
                            statusLabel.Attributes["class"] = "text-hazard";
                            signatureSection.Visible = true;
                            foreach (HtmlGenericControl row in sigRows)
                            {
                                row.Visible = true;
                            }
                            break;
                        case "Drafted":
                            statusIcon.Attributes["class"] = "fas fa-badge-check text-success";
                            statusLabel.Attributes["class"] = "text-success";
                            signatureSection.Visible = true;
                            foreach (HtmlGenericControl row in sigRows)
                            {
                                row.Visible = true;
                            }
                            break;
                        case "Rejected":
                            statusIcon.Attributes["class"] = "fas fa-ban text-danger";
                            statusLabel.Attributes["class"] = "text-danger";
                            signatureSection.Visible = true;
                            foreach (HtmlGenericControl row in sigRows)
                            {
                                row.Visible = true;
                            }
                            break;
                        case "Deleted":
                            statusIcon.Attributes["class"] = "fas fa-trash-xmark text-danger";
                            statusLabel.Attributes["class"] = "text-danger";
                            signatureSection.Visible = true;
                            foreach (HtmlGenericControl row in sigRows)
                            {
                                row.Visible = true;
                            }
                            break;
                    }
                    hdnOrdStatusID.Value = ordStatus.OrdinanceStatusID.ToString();
                    requiredFieldDescriptor.Visible = true;
                    ordinanceNumber.Attributes["placeholder"] = "123-45-6789";
                    agendaNumber.Attributes["placeholder"] = "123456789";
                    fiscalImpact.Attributes["placeholder"] = "$0.00";
                    vendorNumber.Attributes["placeholder"] = "0123456789";
                    contractTerm.Attributes["placeholder"] = "Calculating Term...";
                    contractAmount.Attributes["placeholder"] = "$0.00";
                    prevOrdinanceNums.Attributes["placeholder"] = "123-45-6789";
                    codeProvision.Attributes["placeholder"] = "0123456789";
                    contractStartDate.TextMode = TextBoxMode.Date;
                    contractEndDate.TextMode = TextBoxMode.Date;
                    newRevenueRowDiv.Visible = true;
                    newExpenditureRowDiv.Visible = true;
                    supportingDocumentationDiv.Visible = true;
                    supportingDocumentation.Visible = true;
                    uploadBtn.Visible = true;
                    Session.Remove("RemoveAccs");
                    Session.Remove("RemoveOrdAccs");
                    Session.Remove("RemoveDocs");
                    submitSection.Visible = true;
                    if (rpRevenueTable.Items.Count > 0)
                    {
                        foreach (RepeaterItem item in rpRevenueTable.Items)
                        {
                            HtmlGenericControl removeRevRowDiv = item.FindControl("removeRevRowDiv") as HtmlGenericControl;
                            Button removeRevRow = item.FindControl("removeRevenueRow") as Button;
                            TextBox revAmount = item.FindControl("revenueAmount") as TextBox;
                            removeRevRow.Visible = true;
                            revAmount.Attributes["placeholder"] = "$0.00";
                            ScriptManager.GetCurrent(Page).RegisterAsyncPostBackControl(removeRevRow);
                        }
                    }
                    if (rpExpenditureTable.Items.Count > 0)
                    {
                        foreach (RepeaterItem item in rpExpenditureTable.Items)
                        {
                            HtmlGenericControl removeExpRowDiv = item.FindControl("removeExpRowDiv") as HtmlGenericControl;
                            Button removeExpRow = item.FindControl("removeExpenditureRow") as Button;
                            TextBox expAmount = item.FindControl("expenditureAmount") as TextBox;
                            removeExpRow.Visible = true;
                            expAmount.Attributes["placeholder"] = "$0.00";
                            ScriptManager.GetCurrent(Page).RegisterAsyncPostBackControl(removeExpRow);
                        }
                    }
                    foreach (RepeaterItem item in rpSupportingDocumentation.Items)
                    {
                        LinkButton deleteFile = item.FindControl("deleteFile") as LinkButton;
                        deleteFile.Visible = true;
                        ScriptManager.GetCurrent(Page).RegisterAsyncPostBackControl(deleteFile);
                    }

                    switch (epYes.Checked)
                    {
                        case true:
                            epJustification.Attributes.Add("data-required", "true");
                            break;
                        case false:
                            epJustification.Attributes.Remove("data-required");
                            break;
                    }
                    switch (scYes.Checked)
                    {
                        case true:
                            changeOrderNumber.Enabled = true;
                            additionalAmount.Enabled = true;
                            changeOrderNumber.Attributes.Add("data-required", "true");
                            changeOrderNumber.Attributes.Add("placeholder", "0123456789");
                            additionalAmount.Attributes.Add("data-required", "true");
                            additionalAmount.Attributes.Add("placeholder", "$0.00");
                            break;

                        case false:
                            changeOrderNumber.Enabled = false;
                            additionalAmount.Enabled = false;
                            changeOrderNumber.Attributes.Remove("data-required");
                            changeOrderNumber.Attributes.Remove("placeholder");
                            additionalAmount.Attributes.Remove("data-required");
                            additionalAmount.Attributes.Remove("placeholder");
                            break;
                    }
                    switch (purchaseMethod.SelectedItem.Value)
                    {
                        default:
                            otherException.Enabled = false;
                            otherException.Text = string.Empty;
                            otherException.Attributes.Remove("data-required");
                            break;
                        case "Other":
                        case "Exception":
                            otherException.Enabled = true;
                            otherException.Attributes.Add("data-required", "true");
                            break;
                    }
                    scopeChangeOptions.Visible = true;
                    otherExceptionDiv.Visible = true;

                    foreach (LinkButton item in emailBtns) if (!userInfo.IsAdmin || userInfo.UserView || Request.QueryString["id"] != null)
                        {
                            item.Visible = false;
                        }
                        else
                        {
                            item.Visible = true;
                        }
                    foreach (Button item in signBtns)
                    {
                        item.Text = "Sign";
                    }
                    foreach (TextBox item in sigTextBoxes) if (!item.Text.IsNullOrWhiteSpace())
                        {
                            switch (item.ClientID)
                            {
                                case "fundsCheckBySig":
                                    fundsCheckByBtnDiv.Visible = false;
                                    fundsCheckByInputGroup.Visible = true;
                                    break;
                                case "directorSupervisorSig":
                                    directorSupervisorBtnDiv.Visible = false;
                                    directorSupervisorInputGroup.Visible = true;
                                    break;
                                case "cPASig":
                                    cPABtnDiv.Visible = false;
                                    cPAInputGroup.Visible = true;
                                    break;
                                case "obmDirectorSig":
                                    obmDirectorBtnDiv.Visible = false;
                                    obmDirectorInputGroup.Visible = true;
                                    break;
                                case "mayorSig":
                                    mayorBtnDiv.Visible = false;
                                    mayorInputGroup.Visible = true;
                                    break;
                            }
                        }
                        else
                        {
                            string[] validEmails;
                            switch (item.ClientID)
                            {
                                case "fundsCheckBySig":
                                    validEmails = signatureRequest.FundsCheckBy.Split(';').Where(i => !i.IsNullOrWhiteSpace()).ToArray();
                                    fundsCheckByBtnDiv.Visible = true;
                                    fundsCheckByBtn.Text = (userInfo.IsAdmin && !userInfo.UserView || Request.QueryString["f"] != null || validEmails.Any(i => _user.Email.ToLower().Equals(i))) ? "Sign" : "Awaiting Signature...";
                                    fundsCheckByBtnDiv.Attributes["readonly"] = (userInfo.IsAdmin && !userInfo.UserView || Request.QueryString["f"] != null || validEmails.Any(i => _user.Email.ToLower().Equals(i))) ? "false" : "true";
                                    fundsCheckByInputGroup.Visible = false;
                                    break;
                                case "directorSupervisorSig":
                                    validEmails = signatureRequest.DirectorSupervisor.Split(';').Where(i => !i.IsNullOrWhiteSpace()).ToArray();
                                    directorSupervisorBtnDiv.Visible = true;
                                    directorSupervisorBtn.Text = (userInfo.IsAdmin && !userInfo.UserView || Request.QueryString["f"] != null || validEmails.Any(i => _user.Email.ToLower().Equals(i))) ? "Sign" : "Awaiting Signature...";
                                    directorSupervisorBtnDiv.Attributes["readonly"] = (userInfo.IsAdmin && !userInfo.UserView || Request.QueryString["f"] != null || validEmails.Any(i => _user.Email.ToLower().Equals(i))) ? "false" : "true";
                                    directorSupervisorInputGroup.Visible = false;
                                    break;
                                case "cPASig":
                                    validEmails = signatureRequest.CityPurchasingAgent.Split(';').Where(i => !i.IsNullOrWhiteSpace()).ToArray();
                                    cPABtnDiv.Visible = true;
                                    cPABtn.Text = (userInfo.IsAdmin && !userInfo.UserView || Request.QueryString["f"] != null || validEmails.Any(i => _user.Email.ToLower().Equals(i))) ? "Sign" : "Awaiting Signature...";
                                    cPABtnDiv.Attributes["readonly"] = (userInfo.IsAdmin && !userInfo.UserView || Request.QueryString["f"] != null || validEmails.Any(i => _user.Email.ToLower().Equals(i))) ? "false" : "true";
                                    cPAInputGroup.Visible = false;
                                    break;
                                case "obmDirectorSig":
                                    validEmails = signatureRequest.OBMDirector.Split(';').Where(i => !i.IsNullOrWhiteSpace()).ToArray();
                                    obmDirectorBtnDiv.Visible = true;
                                    obmDirectorBtn.Text = (userInfo.IsAdmin && !userInfo.UserView || Request.QueryString["f"] != null || validEmails.Any(i => _user.Email.ToLower().Equals(i))) ? "Sign" : "Awaiting Signature...";
                                    obmDirectorBtnDiv.Attributes["readonly"] = (userInfo.IsAdmin && !userInfo.UserView || Request.QueryString["f"] != null || validEmails.Any(i => _user.Email.ToLower().Equals(i))) ? "false" : "true";
                                    obmDirectorInputGroup.Visible = false;
                                    break;
                                case "mayorSig":
                                    validEmails = signatureRequest.Mayor.Split(';').Where(i => !i.IsNullOrWhiteSpace()).ToArray();
                                    mayorBtnDiv.Visible = true;
                                    mayorBtn.Text = (userInfo.IsAdmin && !userInfo.UserView || Request.QueryString["f"] != null || validEmails.Any(i => _user.Email.ToLower().Equals(i))) ? "Sign" : "Awaiting Signature...";
                                    mayorBtnDiv.Attributes["readonly"] = (userInfo.IsAdmin && !userInfo.UserView || Request.QueryString["f"] != null || validEmails.Any(i => _user.Email.ToLower().Equals(i))) ? "false" : "true";
                                    mayorInputGroup.Visible = false;
                                    break;
                            }
                        }

                    switch (ord.StatusDescription)
                    {
                        case "New":
                            ordinanceNumberDiv.Visible = false;
                            break;
                        case "Pending":
                            ordinanceNumberDiv.Visible = true;
                            break;
                        case "Under Review":
                            ordinanceNumberDiv.Visible = true;
                            break;
                        case "Being Held":
                            ordinanceNumberDiv.Visible = true;
                            break;
                        case "Drafted":
                            ordinanceNumberDiv.Visible = true;
                            break;
                        case "Rejected":
                            ordinanceNumberDiv.Visible = true;
                            break;
                        case "Deleted":
                            ordinanceNumberDiv.Visible = true;
                            break;
                    }

                    Session["OriginalOrdinance"] = ord;
                    Session["OriginalStatus"] = ordStatus;
                    break;
                case "download":
                    ReportViewer viewer = new ReportViewer();
                    viewer.LocalReport.ReportPath = "./Reports/OrdinanceTracking/Ordinance.rdlc";
                    if (ord.ContractStartDate.Length > 0)
                    {
                        ord.ContractStartDate = Convert.ToDateTime(ord.ContractStartDate).ToString("MM/dd/yyyy");
                    }
                    if (ord.ContractEndDate.Length > 0)
                    {
                        ord.ContractEndDate = Convert.ToDateTime(ord.ContractEndDate).ToString("MM/dd/yyyy");
                    }
                    bool hideTables = true;
                    if (revItems.Count > 0 || expItems.Count > 0)
                    {
                        hideTables = false;
                    }
                    RevExpBool HideTables = new RevExpBool() { HideTables = hideTables };
                    List<string> sigTypeList = new List<string>()
        {
            "fundsCheckBy",
            "directorSupervisor",
            "cPA",
            "obmDirector",
            "mayor",
        };
                    foreach (string item in sigTypeList)
                    {
                        if (!ordSigs.Any(i => i.SignatureType.Equals(item)))
                        {
                            OrdinanceSignature blankSig = new OrdinanceSignature()
                            {
                                SignatureID = -1,
                                SortOrder = 0,
                                OrdinanceID = Convert.ToInt32(ordID),
                                SignatureType = item,
                                Signature = string.Empty,
                                DateSigned = DateTime.MinValue,
                                SignatureCertified = false,
                                LastUpdateBy = string.Empty,
                                LastUpdateDate = DateTime.Now
                            };
                            ordSigs.Add(blankSig);
                        }
                    }
                    foreach (OrdinanceSignature item in ordSigs)
                    {
                        switch (item.SignatureType)
                        {
                            case "fundsCheckBy":
                                item.SortOrder = 0;
                                break;
                            case "directorSupervisor":
                                item.SortOrder = 1;
                                break;
                            case "cPA":
                                item.SortOrder = 2;
                                break;
                            case "obmDirector":
                                item.SortOrder = 3;
                                break;
                            case "mayor":
                                item.SortOrder = 4;
                                break;
                        }
                    }

                    IEnumerable<Ordinance> ordData = new[] { ord };
                    IEnumerable<RevExpBool> revExpBoolData = new[] { HideTables };
                    ReportDataSource ordinanceData = new ReportDataSource() { Name = "dsOrdinance", Value = ordData };
                    ReportDataSource revExpTableBoolData = new ReportDataSource() { Name = "dsRevExpBool", Value = revExpBoolData };
                    ReportDataSource ordinanceRevAccountingData = new ReportDataSource() { Name = "dsRevAccounting", Value = revItems, };
                    ReportDataSource ordinanceExpAccountingData = new ReportDataSource() { Name = "dsExpAccounting", Value = expItems };
                    ReportDataSource ordinanceStatusData = new ReportDataSource() { Name = "dsOrdinanceStatus" };
                    ReportDataSource ordinanceSignaturesData = new ReportDataSource() { Name = "dsSignatures", Value = ordSigs.OrderBy(i => i.SortOrder) };

                    viewer.LocalReport.DataSources.Add(ordinanceData);
                    viewer.LocalReport.DataSources.Add(revExpTableBoolData);
                    viewer.LocalReport.DataSources.Add(ordinanceRevAccountingData);
                    viewer.LocalReport.DataSources.Add(ordinanceExpAccountingData);
                    viewer.LocalReport.DataSources.Add(ordinanceStatusData);
                    viewer.LocalReport.DataSources.Add(ordinanceSignaturesData);

                    viewer.LocalReport.Refresh();

                    Warning[] warnings;
                    string[] streamIds;
                    string mimeType = string.Empty;
                    string encoding = string.Empty;
                    string extension = string.Empty;

                    byte[] bytes = viewer.LocalReport.Render("PDF", null, out mimeType, out encoding, out extension, out streamIds, out warnings);

                    string delivery = HttpContext.Current.IsDebuggingEnabled ? "inline" : "attachment";
                    string fileName = ord.OrdinanceNumber.IsNullOrWhiteSpace() ? $"Ordinance_{ord.OrdinanceID}" : ord.OrdinanceNumber;

                    Response.Clear();
                    Response.ClearContent();
                    Response.ClearHeaders();
                    Response.Buffer = true;
                    Response.ContentType = "application/pdf";
                    Response.AddHeader("content-disposition", $"{delivery}; filename={fileName}.pdf");
                    Response.BinaryWrite(bytes); // create the file                    
                    Context.ApplicationInstance.CompleteRequest();
                    break;
            }
            
            foreach (OrdinanceSignature item in ordSigs)
            {
                switch (item.SignatureType)
                {
                    case "fundsCheckBy":
                        fundsCheckByBtnDiv.Visible = false;
                        fundsCheckByInputGroup.Visible = true;
                        fundsCheckEmailBtn.Visible = false;
                        fundsCheckBySig.Text = item.Signature;
                        fundsCheckByDate.Text = item.DateSigned.ToString("yyyy-MM-dd");
                        break;
                    case "directorSupervisor":
                        directorSupervisorBtnDiv.Visible = false;
                        directorSupervisorInputGroup.Visible = true;
                        directorSupervisorEmailBtn.Visible = false;
                        directorSupervisorSig.Text = item.Signature;
                        directorSupervisorDate.Text = item.DateSigned.ToString("yyyy-MM-dd");
                        break;
                    case "cPA":
                        cPABtnDiv.Visible = false;
                        cPAInputGroup.Visible = true;
                        cPAEmailBtn.Visible = false;
                        cPASig.Text = item.Signature;
                        cPADate.Text = item.DateSigned.ToString("yyyy-MM-dd");
                        break;
                    case "obmDirector":
                        obmDirectorBtnDiv.Visible = false;
                        obmDirectorInputGroup.Visible = true;
                        obmDirectorEmailBtn.Visible = false;
                        obmDirectorSig.Text = item.Signature;
                        obmDirectorDate.Text = item.DateSigned.ToString("yyyy-MM-dd");
                        break;
                    case "mayor":
                        mayorBtnDiv.Visible = false;
                        mayorInputGroup.Visible = true;
                        mayorEmailBtn.Visible = false;
                        mayorSig.Text = item.Signature;
                        mayorDate.Text = item.DateSigned.ToString("yyyy-MM-dd");
                        break;
                }
            }

            ScriptManager.RegisterStartupScript(this, this.GetType(), "CurrencyFormatting", "CurrencyFormatting();", true);

            ordView.Visible = true;
            ordTable.Visible = false;
        }
        protected void rpAccountingTable_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            string tableDesc = e.CommandArgument.ToString();
            List<OrdinanceAccounting> prvList = new List<OrdinanceAccounting>();
            List<OrdinanceAccounting> accountingList = new List<OrdinanceAccounting>();

            List<OrdinanceAccounting> removeOrdAccs = new List<OrdinanceAccounting>();

            switch (e.CommandName)
            {
                case "delete":
                    switch (tableDesc)
                    {
                        case "revenue":
                            for (int i = 0; i < rpRevenueTable.Items.Count; i++)
                            {
                                OrdinanceAccounting accountingItem = new OrdinanceAccounting();
                                var revItem = rpRevenueTable.Items[i];
                                HiddenField revHdnIDField = (HiddenField)revItem.FindControl("hdnRevID");
                                TextBox revFundCode = (TextBox)revItem.FindControl("revenueFundCode");
                                TextBox revAgencyCode = (TextBox)revItem.FindControl("revenueAgencyCode");
                                TextBox revOrgCode = (TextBox)revItem.FindControl("revenueOrgCode");
                                TextBox revActivityCode = (TextBox)revItem.FindControl("revenueActivityCode");
                                TextBox revObjectCode = (TextBox)revItem.FindControl("revenueObjectCode");
                                TextBox revAmount = (TextBox)revItem.FindControl("revenueAmount");
                                accountingItem.OrdinanceAccountingID = Convert.ToInt32(revHdnIDField.Value);
                                accountingItem.OrdinanceID = Convert.ToInt32(hdnOrdID.Value);
                                accountingItem.AccountingDesc = tableDesc;
                                accountingItem.FundCode = revFundCode.Text;
                                accountingItem.DepartmentCode = revAgencyCode.Text;
                                accountingItem.UnitCode = revOrgCode.Text;
                                accountingItem.ActivityCode = revActivityCode.Text;
                                accountingItem.ObjectCode = revObjectCode.Text;
                                accountingItem.Amount = CurrencyToDecimal(revAmount.Text);
                                prvList.Add(accountingItem);
                            }
                            Session[tableDesc] = prvList;
                            HiddenField revHdnIndex = (HiddenField)e.Item.FindControl("hdnRevIndex");
                            if (Session[tableDesc] != null)
                            {
                                accountingList = (List<OrdinanceAccounting>)Session[tableDesc];
                            }


                            HiddenField revHdnID = (HiddenField)e.Item.FindControl("hdnRevID");

                            if (Session["RemoveOrdAccs"] != null)
                            {
                                removeOrdAccs = Session["RemoveOrdAccs"] as List<OrdinanceAccounting>;
                            }
                            if (Convert.ToInt32(revHdnID.Value) > 0)
                            {
                                int accID = Convert.ToInt32(revHdnID.Value);
                                OrdinanceAccounting ordAcc = Factory.Instance.GetByID<OrdinanceAccounting>(accID, "sp_GetOrdinanceAccountingByAccountingID", "AccountingID");
                                removeOrdAccs.Add(ordAcc);
                            }
                            Session["RemoveOrdAccs"] = removeOrdAccs;

                            accountingList.RemoveAt(Convert.ToInt32(revHdnIndex.Value));
                            Session[tableDesc] = accountingList;
                            rpRevenueTable.DataSource = accountingList;
                            rpRevenueTable.DataBind();
                            break;
                        case "expenditure":
                            for (int i = 0; i < rpExpenditureTable.Items.Count; i++)
                            {
                                OrdinanceAccounting accountingItem = new OrdinanceAccounting();
                                var expItem = rpExpenditureTable.Items[i];
                                HiddenField expHdnIDField = (HiddenField)expItem.FindControl("hdnExpID");
                                TextBox expFundCode = (TextBox)expItem.FindControl("expenditureFundCode");
                                TextBox expAgencyCode = (TextBox)expItem.FindControl("expenditureAgencyCode");
                                TextBox expOrgCode = (TextBox)expItem.FindControl("expenditureOrgCode");
                                TextBox expActivityCode = (TextBox)expItem.FindControl("expenditureActivityCode");
                                TextBox expObjectCode = (TextBox)expItem.FindControl("expenditureObjectCode");
                                TextBox expAmount = (TextBox)expItem.FindControl("expenditureAmount");
                                accountingItem.OrdinanceAccountingID = Convert.ToInt32(expHdnIDField.Value);
                                accountingItem.OrdinanceID = Convert.ToInt32(hdnOrdID.Value);
                                accountingItem.AccountingDesc = tableDesc;
                                accountingItem.FundCode = expFundCode.Text;
                                accountingItem.DepartmentCode = expAgencyCode.Text;
                                accountingItem.UnitCode = expOrgCode.Text;
                                accountingItem.ActivityCode = expActivityCode.Text;
                                accountingItem.ObjectCode = expObjectCode.Text;
                                accountingItem.Amount = CurrencyToDecimal(expAmount.Text);
                                prvList.Add(accountingItem);
                            }
                            Session[tableDesc] = prvList;
                            HiddenField expHdnIndex = (HiddenField)e.Item.FindControl("hdnExpIndex");

                            if (Session[tableDesc] != null)
                            {
                                accountingList = (List<OrdinanceAccounting>)Session[tableDesc];
                            }

                            HiddenField expHdnID = (HiddenField)e.Item.FindControl("hdnExpID");

                            if (Session["RemoveOrdAccs"] != null)
                            {
                                removeOrdAccs = Session["RemoveOrdAccs"] as List<OrdinanceAccounting>;
                            }
                            if (Convert.ToInt32(expHdnID.Value) > 0)
                            {
                                int accID = Convert.ToInt32(expHdnID.Value);
                                OrdinanceAccounting ordAcc = Factory.Instance.GetByID<OrdinanceAccounting>(accID, "sp_GetOrdinanceAccountingByAccountingID", "AccountingID");
                                removeOrdAccs.Add(ordAcc);
                            }
                            Session["RemoveOrdAccs"] = removeOrdAccs;

                            accountingList.RemoveAt(Convert.ToInt32(expHdnIndex.Value));
                            Session[tableDesc] = accountingList;
                            rpExpenditureTable.DataSource = accountingList;
                            rpExpenditureTable.DataBind();
                            break;
                    }
                    break;
            }
        }
        protected void rpRevExpTable_ItemCreated(object sender, RepeaterItemEventArgs e)
        {
            Repeater rpTable = (Repeater)sender;
            string rpType = string.Empty;
            switch (rpTable.ClientID)
            {
                case "rpRevenueTable":
                    rpType = "revenue";
                    break;
                case "rpExpenditureTable":
                    rpType = "expenditure";
                    break;
            }
            RepeaterItem rpItem = (RepeaterItem)e.Item;
            List<TextBox> textBoxes = new List<TextBox>()
            {
                (TextBox)e.Item.FindControl($"{rpType}FundCode"),
                (TextBox)e.Item.FindControl($"{rpType}AgencyCode"),
                (TextBox)e.Item.FindControl($"{rpType}OrgCode"),
                (TextBox)e.Item.FindControl($"{rpType}ActivityCode"),
                (TextBox)e.Item.FindControl($"{rpType}ObjectCode")
            };

            foreach (TextBox box in textBoxes)
            {
                box.Attributes.Add("data-validate", $"{box.ID}r{rpItem.ItemIndex}");
                RequiredFieldValidator rfv = new RequiredFieldValidator()
                {
                    ID = $"{box.ID}Validr{rpItem.ItemIndex}",
                    ControlToValidate = box.ID,
                    ValidationGroup = "factSheetMain",
                    SetFocusOnError = false,
                    Display = ValidatorDisplay.None
                };
                rfv.Attributes.Add("data-table-validator", "true");
                box.Parent.Controls.Add(rfv);
            }
        }
        protected void rpSupportingDocumentation_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            HiddenField hdnDocID = (HiddenField)e.Item.FindControl("hdnDocID");
            HiddenField hdnDocIndex = (HiddenField)e.Item.FindControl("hdnDocIndex");
            List<OrdinanceDocument> ordDocList = Session["ordDocs"] as List<OrdinanceDocument>;
            OrdinanceDocument ordDocItem = ordDocList[Convert.ToInt32(hdnDocIndex.Value)];

            switch (e.CommandName)
            {
                case "download":
                    string delivery = HttpContext.Current.IsDebuggingEnabled ? "inline" : "attachment";
                    Response.Clear();
                    Response.ClearHeaders();
                    Response.AddHeader("Content-Length", ordDocItem.DocumentData.Length.ToString());
                    Response.AddHeader("Content-type", MimeMapping.GetMimeMapping(ordDocItem.DocumentName));
                    Response.AddHeader("Content-Disposition", $"{delivery}; filename=" + ordDocItem.DocumentName);
                    Response.BinaryWrite(ordDocItem.DocumentData);
                    Response.Flush();
                    Response.End();
                    break;
                case "delete":
                    List<OrdinanceDocument> removeDocs = new List<OrdinanceDocument>();
                    if (Session["RemoveDocs"] != null)
                    {
                        removeDocs = Session["RemoveDocs"] as List<OrdinanceDocument>;
                    }
                    removeDocs.Add(ordDocItem);
                    Session["RemoveDocs"] = removeDocs;
                    ordDocList.Remove(ordDocItem);
                    Session["ordDocs"] = ordDocList;
                    rpSupportingDocumentation.DataSource = ordDocList.OrderBy(i => i.DocumentID);
                    rpSupportingDocumentation.DataBind();
                    break;
            }
        }
        protected void rpEmailList_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            switch (e.CommandName)
            {
                case "remove":
                    SignatureRequest sigRequests = Session["SigRequestEmails"] as SignatureRequest;
                    PropertyInfo sigType = (PropertyInfo)typeof(SignatureRequest).GetProperties().First(i => i.Name.Equals(sigBtnType.Value));

                    List<string> emails = sigType.GetValue(sigRequests).ToString().Split(';').Where(i => !i.IsNullOrWhiteSpace()).ToList();
                    emails.Remove(e.CommandArgument.ToString());
                    sigType.SetValue(sigRequests, string.Join(";", emails.OrderBy(i => i)));
                    if (emails.Count > 0)
                    {
                        emailListDiv.Visible = true;
                        btnSendSigEmail.Enabled = true;
                        rpEmailList.DataSource = emails.OrderBy(i => i);
                        rpEmailList.DataBind();
                    }
                    else
                    {
                        emailListDiv.Visible = false;
                        btnSendSigEmail.Enabled = false;
                        rpEmailList.DataSource = null;
                        rpEmailList.DataBind();
                    }

                    int updateSigRequest = Factory.Instance.Update(sigRequests, "sp_UpdateOrdinance_SignatureRequest");
                    if (updateSigRequest > 0)
                    {
                        signatureEmailAddress.Text = string.Empty;
                    }
                    break;
            }
        }
        protected void rpEmailList_ItemCreated(object sender, RepeaterItemEventArgs e)
        {
            LinkButton btn = (LinkButton)e.Item.FindControl("removeBtn");
            ScriptManager.GetCurrent(Page).RegisterAsyncPostBackControl(btn);
        }
        protected void rpAudit_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            HiddenField hdnAuditItem = (HiddenField)e.Item.FindControl("hdnAuditItem");
            int auditID = Convert.ToInt32(hdnAuditItem.Value);
            Repeater rpAuditDesc = (Repeater)e.Item.FindControl("rpAuditDesc");

            

            List<OrdinanceAudit> ordAudits = Session["ordAudit"] as List<OrdinanceAudit>;
            List<Audit> audits = Factory.Instance.GetAllLookup<Audit>(auditID, "sp_GetAuditDescriptionByID", "OrdinanceAuditID");
            if (audits.Count > 0)
            {
                List<string> descList = new List<string>();
                foreach (Audit item in audits)
                {
                    List<string> longTexts = new List<string>()
                    {
                        "EmergencyPassageReason",
                        "OrdinanceTitle",
                        "OrdinanceAnalysis"
                    };
                    string label = FieldLabels(item.Label);
                    string oldValue = item.OldValue;
                    string symbol = AuditSymbol(item.Type);
                    string newValue = item.NewValue;
                    string itemString = string.Empty;

                    StringBuilder sb = new StringBuilder();

                    string headerBegin = "<thead><tr class='h-50'>";
                    string fundHeader = "<th style='width: 13%; text-align: center;'>Fund</th>";
                    string agencyHeader = "<th style='width: 15%; text-align: center;'>Agency</th>";
                    string orgHeader = "<th style='width: 15%; text-align: center;'>Org</th>";
                    string activityHeader = "<th style='width: 16%; text-align: center;'>Activity</th>";
                    string objectHeader = "<th style='width: 15%; text-align: center;'>Object</th>";
                    string amountHeader = "<th style='width: 18%; text-align: center;'>Amount</th>";
                    string headerEnd = "</tr></thead>";
                    List<string> HeaderCells = new List<string>()
                    {
                        headerBegin,
                        fundHeader,
                        agencyHeader,
                        orgHeader,
                        activityHeader,
                        objectHeader,
                        amountHeader,
                        headerEnd
                    };


                    if (oldValue == "-1.00" || oldValue == "-1")
                    {
                        oldValue = null;
                    }
                    if (newValue == "-1.00" || newValue == "-1")
                    {
                        newValue = null;
                    }
                    switch (item.Type)
                    {
                        case "add":
                            switch (!longTexts.Any(i => i.Equals(item.Label)))
                            {
                                case true:
                                    itemString = $"{label}: <span class='change-bg'>{symbol} <span data-type='{item.DataType}'>{newValue}</span> </span>";
                                    break;
                                case false:
                                    itemString = $"<p class='m-0'>{label}:</p> <div class='d-flex change-bg w-100 lh-1p5'> {symbol} <div class='w-100 ps-2'>{newValue}</div> </div>";
                                    break;
                            }
                            break;
                        case "update":
                            switch (!longTexts.Any(i => i.Equals(item.Label)))
                            {
                                case true:
                                    itemString = $"{label}: <span class='change-bg'><span data-type='{item.DataType}'>{oldValue}</span> {symbol} <span data-type='{item.DataType}'>{newValue}</span></span>";
                                    break;
                                case false:
                                    itemString = $"<p class='m-0'>{label}:</p> <div class='d-flex change-bg mw-100 lh-1p5'> <div class='w-50 pe-2 text-center'>{oldValue}</div> {symbol} <div class='w-50 ps-2 text-center'>{newValue}</div> </div>";
                                    break;
                            }
                            break;
                        case "remove":
                            switch (!longTexts.Any(i => i.Equals(item.Label)))
                            {
                                case true:
                                    itemString = $"{label}: <span class='change-bg'>{symbol} <span data-type='{item.DataType}'>{oldValue}</span></span>";
                                    break;
                                case false:
                                    itemString = $"<p class='m-0'>{label}:</p> <div class='d-flex change-bg w-100 lh-1p5'> {symbol} <div class='w-100 ps-2'>{oldValue}</div> </div>";
                                    break;
                            }
                            break;
                        case "rejected":
                            itemString = $"<div class='change-bg lh-1p5'> {newValue} </div>";
                            break;
                        case "revenue": case "expenditure":
                            sb.Append("<table class='table table-bordered table-hover table-standard text-center w-100' style='padding: 0px; margin: 0px'>");
                            foreach (string header in HeaderCells)
                            {
                                sb.Append(header);
                            }
                            sb.Append("<tbody>");

                            List<AccountingAudit> acctAudits = Factory.Instance.GetAllLookup<AccountingAudit>(item.AuditID, "sp_GetAccountingAuditDescriptionByID", "AuditID");
                            foreach (AccountingAudit acctAudit in acctAudits)
                            {

                                sb.Append("<tr>");
                                sb.Append($"<td style='vertical-align: middle; padding: 0 !important;'>{acctAudit.FundCode}</td>");
                                sb.Append($"<td style='vertical-align: middle; padding: 0 !important;'>{acctAudit.DepartmentCode}</td>");
                                sb.Append($"<td style='vertical-align: middle; padding: 0 !important;'>{acctAudit.UnitCode}</td>");
                                sb.Append($"<td style='vertical-align: middle; padding: 0 !important;'>{acctAudit.ActivityCode}</td>");
                                sb.Append($"<td style='vertical-align: middle; padding: 0 !important;'>{acctAudit.ObjectCode}</td>");
                                sb.Append($"<td style='vertical-align: middle; padding: 0 !important;'>{acctAudit.Amount}</td>");
                                sb.Append("</tr>");
                            }
                            sb.Append("</tbody></table>");
                            
                            
                            itemString = $"<p class='m-0'>{label}:</p> {sb}";
                            break;
                    }
                    descList.Add(itemString);
                }
                rpAuditDesc.DataSource = descList;
                rpAuditDesc.DataBind();
            }
            else
            {
                rpAuditDesc.DataSource = null;
                rpAuditDesc.DataBind();
            }
        }



        // ACCOUNTING TABLES //
        protected void NewAccountingRow(string tableDesc)
        {
            List<OrdinanceAccounting> prvList = new List<OrdinanceAccounting>();
            List<OrdinanceAccounting> accountingList = new List<OrdinanceAccounting>();
            OrdinanceAccounting newAccountingItem = new OrdinanceAccounting();
            newAccountingItem.Amount = CurrencyToDecimal("-1");

            switch (tableDesc)
            {
                case "revenue":
                    if (Session[tableDesc] != null)
                    {
                        for (int i = 0; i < rpRevenueTable.Items.Count; i++)
                        {
                            OrdinanceAccounting accountingItem = GetAccountingItem("revenue", i);
                            prvList.Add(accountingItem);
                        }
                        Session[tableDesc] = prvList;
                        accountingList = (List<OrdinanceAccounting>)Session[tableDesc];
                    }
                    accountingList.Add(newAccountingItem);
                    Session[tableDesc] = accountingList;
                    rpRevenueTable.DataSource = accountingList;
                    rpRevenueTable.DataBind();
                    break;
                case "expenditure":
                    if (Session[tableDesc] != null)
                    {
                        for (int i = 0; i < rpExpenditureTable.Items.Count; i++)
                        {
                            OrdinanceAccounting accountingItem = GetAccountingItem("expenditure", i);
                            prvList.Add(accountingItem);
                        }
                        Session[tableDesc] = prvList;
                        accountingList = (List<OrdinanceAccounting>)Session[tableDesc];
                    }
                    accountingList.Add(newAccountingItem);
                    Session[tableDesc] = accountingList;
                    rpExpenditureTable.DataSource = accountingList;
                    rpExpenditureTable.DataBind();
                    break;
            }
        }
        protected void newAccountingRow_ServerClick(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            NewAccountingRow(button.CommandName);
        }
        protected OrdinanceAccounting GetAccountingItem(string tableDesc, int itemIndex)
        {
            OrdinanceAccounting accountingItem = new OrdinanceAccounting();
            switch (tableDesc)
            {
                case "revenue":
                    var revItem = rpRevenueTable.Items[itemIndex];
                    HiddenField revHdnID = (HiddenField)revItem.FindControl("hdnRevID");
                    TextBox revFundCode = (TextBox)revItem.FindControl("revenueFundCode");
                    TextBox revAgencyCode = (TextBox)revItem.FindControl("revenueAgencyCode");
                    TextBox revOrgCode = (TextBox)revItem.FindControl("revenueOrgCode");
                    TextBox revActivityCode = (TextBox)revItem.FindControl("revenueActivityCode");
                    TextBox revObjectCode = (TextBox)revItem.FindControl("revenueObjectCode");
                    TextBox revAmount = (TextBox)revItem.FindControl("revenueAmount");
                    accountingItem.OrdinanceAccountingID = Convert.ToInt32(revHdnID.Value);
                    accountingItem.OrdinanceID = Convert.ToInt32(hdnOrdID.Value);
                    accountingItem.AccountingDesc = tableDesc;
                    accountingItem.FundCode = revFundCode.Text.ToUpper();
                    accountingItem.DepartmentCode = revAgencyCode.Text.ToUpper();
                    accountingItem.UnitCode = revOrgCode.Text.ToUpper();
                    accountingItem.ActivityCode = revActivityCode.Text.ToUpper();
                    accountingItem.ObjectCode = revObjectCode.Text.ToUpper();
                    accountingItem.LastUpdateBy = _user.Login;
                    accountingItem.LastUpdateDate = DateTime.Now;
                    accountingItem.EffectiveDate = DateTime.Now;
                    accountingItem.ExpirationDate = DateTime.MaxValue;
                    accountingItem.Amount = CurrencyToDecimal(revAmount.Text);
                    break;
                case "expenditure":
                    var expItem = rpExpenditureTable.Items[itemIndex];
                    HiddenField expHdnID = (HiddenField)expItem.FindControl("hdnExpID");
                    TextBox expFundCode = (TextBox)expItem.FindControl("expenditureFundCode");
                    TextBox expAgencyCode = (TextBox)expItem.FindControl("expenditureAgencyCode");
                    TextBox expOrgCode = (TextBox)expItem.FindControl("expenditureOrgCode");
                    TextBox expActivityCode = (TextBox)expItem.FindControl("expenditureActivityCode");
                    TextBox expObjectCode = (TextBox)expItem.FindControl("expenditureObjectCode");
                    TextBox expAmount = (TextBox)expItem.FindControl("expenditureAmount");
                    accountingItem.OrdinanceAccountingID = Convert.ToInt32(expHdnID.Value);
                    accountingItem.OrdinanceID = Convert.ToInt32(hdnOrdID.Value);
                    accountingItem.AccountingDesc = tableDesc;
                    accountingItem.FundCode = expFundCode.Text.ToUpper();
                    accountingItem.DepartmentCode = expAgencyCode.Text.ToUpper();
                    accountingItem.UnitCode = expOrgCode.Text.ToUpper();
                    accountingItem.ActivityCode = expActivityCode.Text.ToUpper();
                    accountingItem.ObjectCode = expObjectCode.Text.ToUpper();
                    accountingItem.LastUpdateBy = _user.Login;
                    accountingItem.LastUpdateDate = DateTime.Now;
                    accountingItem.EffectiveDate = DateTime.Now;
                    accountingItem.ExpirationDate = DateTime.MaxValue;
                    accountingItem.Amount = CurrencyToDecimal(expAmount.Text);
                    break;
            }
            return accountingItem;
        }
        


        // SUBMITS //
        protected void SaveFactSheet_Click(object sender, EventArgs e)
        {
            bool rejected;
            try
            {
                rejected = Convert.ToBoolean(sender);
            }
            catch (Exception)
            {
                rejected = false;
            }
            List<string> baseData = new List<string>()
            {
                "StatusID",
                "LastUpdateBy",
                "LastUpdateDate",
                "EffectiveDate",
                "ExpirationDate",
            };
            Ordinance ordinance = new Ordinance();

            ordinance.OrdinanceID = Convert.ToInt32(hdnOrdID.Value);
            ordinance.OrdinanceNumber = ordinanceNumber.Text ?? string.Empty;
            ordinance.AgendaNumber = agendaNumber.Text ?? string.Empty;
            ordinance.RequestDepartment = requestDepartment.SelectedItem.Text;
            ordinance.RequestDivision = requestDivision.SelectedItem.Text;
            ordinance.RequestContact = requestContact.Text;
            ordinance.RequestPhone = $"{requestPhone.Text}{requestExt.Text}";
            ordinance.RequestEmail = requestEmail.Text.ToLower();
            ordinance.FirstReadDate = Convert.ToDateTime(firstReadDate.Text);
            ordinance.EmergencyPassage = epYes.Checked;
            ordinance.EmergencyPassageReason = epJustification.Text ?? string.Empty;
            ordinance.OrdinanceFiscalImpact = CurrencyToDecimal(fiscalImpact.Text);
            ordinance.OrdinanceTitle = suggestedTitle.Text;
            ordinance.ContractVendorName = vendorName.Text;
            ordinance.ContractVendorNumber = vendorNumber.Text;
            ordinance.ContractStartDate = contractStartDate.Text;
            ordinance.ContractEndDate = contractEndDate.Text;
            ordinance.ContractTerm = contractTerm.Value;
            ordinance.ContractAmount = CurrencyToDecimal(contractAmount.Text);
            ordinance.ScopeChange = scYes.Checked;
            ordinance.ChangeOrderNumber = changeOrderNumber.Text ?? string.Empty;
            ordinance.AdditionalAmount = CurrencyToDecimal(additionalAmount.Text);
            ordinance.ContractMethod = purchaseMethod.SelectedValue;
            ordinance.OtherException = otherException.Text ?? string.Empty;
            ordinance.PreviousOrdinanceNumbers = prevOrdinanceNums.Text;
            ordinance.CodeProvision = codeProvision.Text;
            ordinance.PAApprovalRequired = paApprovalRequiredYes.Checked;
            ordinance.PAApprovalIncluded = paApprovalAttachedYes.Checked;
            ordinance.OrdinanceAnalysis = staffAnalysis.Text;
            ordinance.LastUpdateBy = _user.Login;
            ordinance.LastUpdateDate = DateTime.Now;
            ordinance.EffectiveDate = Convert.ToDateTime(hdnEffectiveDate.Value);
            ordinance.ExpirationDate = DateTime.MaxValue;

            int retVal = Factory.Instance.Update(ordinance, "sp_UpdateOrdinance", Skips(key: "ordUpdate"));

            OrdinanceStatus ordStatus = new OrdinanceStatus();
            ordStatus.OrdinanceStatusID = Convert.ToInt32(hdnOrdStatusID.Value);
            ordStatus.OrdinanceID = Convert.ToInt32(hdnOrdID.Value);
            try
            {
                ordStatus.StatusID = Convert.ToInt32(ddStatus.SelectedValue);
            }
            catch (Exception)
            {
                ordStatus.StatusID = Convert.ToInt32(hdnStatusID.Value);
            }
            ordStatus.LastUpdateBy = _user.Login;
            ordStatus.LastUpdateDate = DateTime.Now;
            ordStatus.EffectiveDate = Convert.ToDateTime(hdnEffectiveDate.Value);
            ordStatus.ExpirationDate = DateTime.MaxValue;
            if (ddStatus.SelectedItem.ToString().ToLower().Contains("select"))
            {
                ordinance.StatusDescription = "New";
                ordStatus.StatusDescription = "New";
            }
            else
            {
                ordinance.StatusDescription = ddStatus.SelectedItem.ToString();
                ordStatus.StatusDescription = ddStatus.SelectedItem.ToString();
            }
            

            int addDocsVal = new int();
            int addUploadedDocsVal = new int();
            List<OrdinanceDocument> ordDocs = Session["addOrdDocs"] as List<OrdinanceDocument>;
            List<string> addDocNames = new List<string>();
            if (Session["addOrdDocs"] != null)
            {
                foreach (OrdinanceDocument ordDoc in ordDocs)
                {
                    ordDoc.OrdinanceID = Convert.ToInt32(hdnOrdID.Value);
                    addUploadedDocsVal = Factory.Instance.Insert(ordDoc, "sp_InsertOrdinance_Document", Skips("ordDocumentInsert"));
                    if (addUploadedDocsVal > 0)
                    {
                        addDocNames.Add(ordDoc.DocumentName);
                    }
                    else if (addUploadedDocsVal < 1)
                    {
                        break;
                    }
                }
            }
            else
            {
                addUploadedDocsVal = 1;
            }
            if (supportingDocumentation.HasFiles)
            {
                for (int i = 0; i < supportingDocumentation.PostedFiles.Count; i++)
                {
                    OrdinanceDocument ordDoc = new OrdinanceDocument();
                    ordDoc.OrdinanceID = Convert.ToInt32(hdnOrdID.Value);
                    ordDoc.DocumentName = supportingDocumentation.PostedFiles[i].FileName;
                    Stream stream = supportingDocumentation.PostedFiles[i].InputStream;
                    using (var fileBytes = new BinaryReader(stream))
                    {
                        ordDoc.DocumentData = fileBytes.ReadBytes((int)stream.Length);
                    }
                    ordDoc.LastUpdateBy = _user.Login;
                    ordDoc.LastUpdateDate = DateTime.Now;
                    ordDoc.EffectiveDate = DateTime.Now;
                    ordDoc.ExpirationDate = DateTime.MaxValue;
                    addDocsVal = Factory.Instance.Insert(ordDoc, "sp_InsertOrdinance_Document", Skips("ordDocumentInsert"));
                    if (addDocsVal > 0)
                    {
                        addDocNames.Add(ordDoc.DocumentName);
                    }
                    else if (addDocsVal < 1)
                    {
                        break;
                    }
                }
            }
            else
            {
                addDocsVal = 1;
            }

            int removeDocVal = new int();
            List<OrdinanceDocument> removeDocs = new List<OrdinanceDocument>();
            List<string> removeDocNames = new List<string>();
            if (Session["RemoveDocs"] != null)
            {
                removeDocs = Session["RemoveDocs"] as List<OrdinanceDocument>;
            }
            if (removeDocs.Count > 0)
            {
                foreach (OrdinanceDocument item in removeDocs)
                {
                    removeDocVal = Factory.Instance.Expire(item, "sp_UpdateOrdinance_Document");
                    if (removeDocVal > 0)
                    {
                        removeDocNames.Add(item.DocumentName);
                    }
                    else if (removeDocVal < 1)
                    {
                        break;
                    }
                }
            }
            else
            {
                removeDocVal = 1;
            }

            List<AccountingAudit> accAuditList = new List<AccountingAudit>();
            List<AccountingAudit> revAccAuditList = new List<AccountingAudit>();
            List<AccountingAudit> expAccAuditList = new List<AccountingAudit>();
            int updateRevAccsVal = new int();
            int updateExpAccsVal = new int();
            int removeOrdAccsVal = new int();

            PropertyInfo[] accProperties = typeof(OrdinanceAccounting).GetProperties();
            List<OrdinanceAccounting> originalExpList = Session["OriginalExpTable"] as List<OrdinanceAccounting>;
            List<OrdinanceAccounting> originalRevList = Session["OriginalRevTable"] as List<OrdinanceAccounting>;

            List<OrdinanceAccounting> removeOrdAccs = new List<OrdinanceAccounting>();
            if (Session["RemoveOrdAccs"] != null)
            {
                removeOrdAccs = Session["RemoveOrdAccs"] as List<OrdinanceAccounting>;
            }

            if (removeOrdAccs.Count > 0)
            {
                foreach (OrdinanceAccounting item in removeOrdAccs)
                {
                    item.LastUpdateBy = _user.Login;
                    item.LastUpdateDate = DateTime.Now;
                    item.EffectiveDate = DateTime.Now;

                    removeOrdAccsVal = Factory.Instance.Expire(item, "sp_UpdateOrdinance_Accounting");
                    if (removeOrdAccsVal > 0)
                    {
                        string getAmount = string.Empty;
                        if (item.Amount.ToString().Equals("-1.00") || item.Amount.ToString().Equals("-1"))
                        {
                            getAmount = $"<span>{AuditSymbol("remove")} <span data-type='String'>N/A</span></span>";
                        }
                        else
                        {
                            getAmount = $"<span>{AuditSymbol("remove")} <span data-type='Decimal'>{item.Amount}</span></span>";
                        }

                        AccountingAudit accAudit = new AccountingAudit()
                        {
                            AccountingDesc = item.AccountingDesc,
                            OrdinanceAccountingID = item.OrdinanceAccountingID,
                            FundCode = $"<span>{AuditSymbol("remove")} <span data-type='String'>{item.FundCode}</span></span>",
                            DepartmentCode = $"<span>{AuditSymbol("remove")} <span data-type='String'>{item.DepartmentCode}</span></span>",
                            UnitCode = $"<span>{AuditSymbol("remove")} <span data-type='String'>{item.UnitCode}</span></span>",
                            ActivityCode = $"<span>{AuditSymbol("remove")} <span data-type='String'>{item.ActivityCode}</span></span>",
                            ObjectCode = $"<span>{AuditSymbol("remove")} <span data-type='String'>{item.ObjectCode}</span></span>",
                            Amount = getAmount,
                        };
                        accAuditList.Add(accAudit);
                    }
                    else
                    {
                        break;
                    }
                }
            }
            else
            {
                removeOrdAccsVal = 1;
            }

            bool revTableHasChanges = rpRevenueTable.Items.Cast<RepeaterItem>().Any(item =>
            {
                OrdinanceAccounting accountingItem = GetAccountingItem("revenue", item.ItemIndex);
                OrdinanceAccounting originalItem = originalRevList.FirstOrDefault(r => r.OrdinanceAccountingID == accountingItem.OrdinanceAccountingID);
                bool isExisting = accountingItem.OrdinanceAccountingID > 0;

                return (isExisting && originalItem != null &&
                        accProperties.Any(p =>
                            !baseData.Contains(p.Name) &&
                            !Equals(p.GetValue(originalItem), p.GetValue(accountingItem))))
                       || !isExisting;
            });

            bool expTableHasChanges = rpExpenditureTable.Items.Cast<RepeaterItem>().Any(item =>
            {
                OrdinanceAccounting accountingItem = GetAccountingItem("expenditure", item.ItemIndex);
                OrdinanceAccounting originalItem = originalExpList.FirstOrDefault(r => r.OrdinanceAccountingID == accountingItem.OrdinanceAccountingID);
                bool isExisting = accountingItem.OrdinanceAccountingID > 0;

                return (isExisting && originalItem != null &&
                        accProperties.Any(p =>
                            !baseData.Contains(p.Name) &&
                            !Equals(p.GetValue(originalItem), p.GetValue(accountingItem))))
                       || !isExisting;
            });

            if (rpRevenueTable.Items.Count > 0 && (revTableHasChanges || removeOrdAccs.Any(i => i.AccountingDesc.Equals("revenue"))))
            {
                for (int i = 0; i < rpRevenueTable.Items.Count; i++)
                {
                    OrdinanceAccounting accountingItem = GetAccountingItem("revenue", i);
                    OrdinanceAccounting originalItem = originalRevList.FirstOrDefault(r => r.OrdinanceAccountingID == accountingItem.OrdinanceAccountingID);
                    bool isExisting = accountingItem.OrdinanceAccountingID > 0;
                    bool hasChanges = isExisting && originalItem != null &&
                                      accProperties.Any(p => !Equals(p.GetValue(originalItem), p.GetValue(accountingItem)));

                    if (hasChanges)
                    {
                        updateRevAccsVal = Factory.Instance.Update(accountingItem, "sp_UpdateOrdinance_Accounting");

                        if (updateRevAccsVal > 0)
                        {
                            var audit = BuildAccAudit(accountingItem, originalItem, accProperties, baseData);
                            audit.AccountingDesc = "revenue";
                            revAccAuditList.Add(audit);
                        }
                    }
                    else if (!isExisting)
                    {
                        updateRevAccsVal = Factory.Instance.Insert(accountingItem, "sp_InsertOrdinance_Accounting", Skips("accountingInsert"));

                        if (updateRevAccsVal > 0)
                        {
                            var audit = BuildAccAudit(accountingItem, null, accProperties, baseData);
                            audit.AccountingDesc = "revenue";
                            audit.OrdinanceAccountingID = updateRevAccsVal;
                            revAccAuditList.Add(audit);
                        }
                        else
                        {
                            updateRevAccsVal = 0;
                            break;
                        }
                    }

                    if (updateRevAccsVal < 1)
                        break;
                }
            }
            else
            {
                updateRevAccsVal = 1;
            }

            if (rpExpenditureTable.Items.Count > 0 && (expTableHasChanges || removeOrdAccs.Any(i => i.AccountingDesc.Equals("expenditure"))))
            {
                for (int i = 0; i < rpExpenditureTable.Items.Count; i++)
                {
                    OrdinanceAccounting accountingItem = GetAccountingItem("expenditure", i);
                    OrdinanceAccounting originalItem = originalExpList.FirstOrDefault(r => r.OrdinanceAccountingID == accountingItem.OrdinanceAccountingID);
                    bool isExisting = accountingItem.OrdinanceAccountingID > 0;
                    bool hasChanges = isExisting && originalItem != null &&
                                      accProperties.Any(p => !Equals(p.GetValue(originalItem), p.GetValue(accountingItem)));

                    if (hasChanges)
                    {
                        updateExpAccsVal = Factory.Instance.Update(accountingItem, "sp_UpdateOrdinance_Accounting");

                        if (updateExpAccsVal > 0)
                        {
                            var audit = BuildAccAudit(accountingItem, originalItem, accProperties, baseData);
                            audit.AccountingDesc = "expenditure";
                            expAccAuditList.Add(audit);
                        }
                    }
                    else if (!isExisting)
                    {
                        updateExpAccsVal = Factory.Instance.Insert(accountingItem, "sp_InsertOrdinance_Accounting", Skips("accountingInsert"));

                        if (updateExpAccsVal > 0)
                        {
                            var audit = BuildAccAudit(accountingItem, null, accProperties, baseData);
                            audit.AccountingDesc = "expenditure";
                            audit.OrdinanceAccountingID = updateExpAccsVal;
                            expAccAuditList.Add(audit);
                        }
                        else
                        {
                            updateExpAccsVal = 0;
                            break;
                        }
                    }

                    if (updateExpAccsVal < 1)
                        break;
                }
            }
            else
            {
                updateExpAccsVal = 1;
            }

            if (revAccAuditList.Any())
                accAuditList.AddRange(revAccAuditList);

            if (expAccAuditList.Any())
                accAuditList.AddRange(expAccAuditList);

            int insertSignatureVal = new int();
            int sigAuditVal = new int();
            bool isPending = false;
            if (Session["insertSigList"] != null)
            {
                List<OrdinanceSignature> insertSigList = (List<OrdinanceSignature>)Session["insertSigList"];
                foreach (OrdinanceSignature item in insertSigList)
                {
                    insertSignatureVal = Factory.Instance.Insert(item, "sp_InsertOrdinance_Signature", Skips("ordSignatureInsert"));

                    if (insertSignatureVal > 0)
                    {
                        if (ordinance.StatusDescription.Equals("New") && item.SignatureType.Equals("directorSupervisor"))
                        {
                            ordStatus.StatusID = 2;
                            ordStatus.StatusDescription = "Pending";
                            ordinance.StatusDescription = "Pending";
                            isPending = true;
                        }
                        OrdinanceAudit sigAudit = new OrdinanceAudit()
                        {
                            OrdinanceID = Convert.ToInt32(hdnOrdID.Value),
                            UpdateType = $"SIGNED '{item.Signature}' for {FieldLabels(item.SignatureType)}",
                            LastUpdateBy = $"{_user.FirstName} {_user.LastName}",
                            LastUpdateDate = DateTime.Now,
                        };
                        sigAuditVal = Factory.Instance.Insert(sigAudit, "sp_InsertOrdinance_Audit", Skips("ordAuditInsert"));
                    }
                }
            }
            else
            {
                insertSignatureVal = 1;
                sigAuditVal = 1;
            }

            int statusVal = Factory.Instance.Update(ordStatus, "sp_UpdateOrdinance_Status", Skips("ordStatusUpdate"));

            List<string> addEmailList = new List<string>()
            {
                _user.Email.ToLower(),
                ordinance.RequestEmail.ToLower()
            };
            foreach (string item in addEmailList)
            {
                Email.Instance.AddEmailAddress(emailList, item);
            }
            string formType = "Ordinance Fact Sheet";
            string href = $"apptest/Themis/Ordinances?id={hdnOrdID.Value}&v=view";
            string ordinanceNumInfo = !ordinanceNumber.Text.IsNullOrWhiteSpace() ? $"<p style='margin: 0; line-height: 1.5;'><span>Ordinance: {ordinance.OrdinanceNumber}</span></p>" : string.Empty;

            Email newEmail = new Email();

            newEmail.EmailSubject = $"{formType} UPDATED";
            newEmail.EmailTitle = $"{formType} UPDATED";
            newEmail.EmailText = $"<p style='margin: 0;'><span style='font-size:36.0pt;font-family:\"Times New Roman\",serif;color:#2D71D5;font-weight:bold;'>THΣMIS</span></p><div align=center style='text-align:center'><span><hr size='2' width='100%' align='center' style='margin-top: 0;'></span></div><p><span>An <b>{formType}</b> has been UPDATED by <b>{_user.FirstName} {_user.LastName}</b>.</span></p><br /><p style='margin: 0; line-height: 1.5;'><span>ID: {ordinance.OrdinanceID}</span></p>{ordinanceNumInfo}<p style='margin: 0; line-height: 1.5;'><span>Date: {DateTime.Now}</span></p><p style='margin: 0; line-height: 1.5;'><span>Department: {requestDepartment.SelectedItem.Text}</span></p><p style='margin: 0; line-height: 1.5;'><span>Contact: {requestContact.Text}</span></p><p style='margin: 0; line-height: 1.5;'><span>Phone: {ordinance.RequestPhone}</span></p><p><span>Status: {ordinance.StatusDescription}</span></p><br /><p><span>Please click the button below to review the document:</span></p><table border='0' cellpadding='0' cellspacing='0' style='border-collapse: separate; mso-table-lspace: 0pt; mso-table-rspace: 0pt; width: auto;'><tr><td style='font-family: sans-serif; font-size: 14px; vertical-align: top; background-color: #0d6efd; border-radius: 5px; text-align: center;' valign='top' bgcolor='#0d6efd' align='center'><a href='{href}' target='_blank' style='display: inline-block; color: #ffffff; background-color: #0d6efd; border: solid 1px #0d6efd; border-radius: 5px; box-sizing: border-box; cursor: pointer; text-decoration: none; font-size: 18px; font-weight: bold; margin: 0; padding: 15px 25px; text-transform: capitalize; border-color: #0d6efd; '>View Ordinance</a></td></tr></table>";



            int ordAuditVal = -1;
            int auditVal = new int();
            int acctAuditVal = new int();
            OrdinanceAudit ordAudit = new OrdinanceAudit()
            {
                OrdinanceID = Convert.ToInt32(hdnOrdID.Value),
                UpdateType = "UPDATED",
                LastUpdateBy = $"{_user.FirstName} {_user.LastName}",
                LastUpdateDate = rejected ? DateTime.Now.AddSeconds(-2) : DateTime.Now,
            };
            
            List<Audit> auditList = new List<Audit>();

            if (Session["OriginalStatus"] != null)
            {
                OrdinanceStatus oldOrdStatus = Session["OriginalStatus"] as OrdinanceStatus;
                PropertyInfo[] properties = typeof(OrdinanceStatus).GetProperties();
                if (properties.Any(i => !i.GetValue(ordStatus).Equals(i.GetValue(oldOrdStatus)) && !baseData.Any(b => b.Contains(i.Name))) && !rejected)
                {
                    if (ordAuditVal < 0)
                    {
                        ordAuditVal = Factory.Instance.Insert(ordAudit, "sp_InsertOrdinance_Audit", Skips("ordAuditInsert"));
                    }
                    if (ordAuditVal > 0)
                    {
                        foreach (PropertyInfo property in properties.Where(i => !i.GetValue(ordStatus).Equals(i.GetValue(oldOrdStatus)) && !baseData.Any(b => b.Contains(i.Name))))
                        {
                            Audit audit = new Audit()
                            {
                                OrdinanceAuditID = ordAuditVal,
                                Label = property.Name,
                                DataType = property.GetValue(ordStatus).GetType().Name
                            };
                            audit.Type = "update";
                            audit.OldValue = property.GetValue(oldOrdStatus).ToString();
                            audit.NewValue = property.GetValue(ordStatus).ToString();

                            auditList.Add(audit);
                        }
                    }
                }
                else
                {
                    auditVal = 1;
                }
            }
            if (Session["OriginalOrdinance"] != null)
            {
                Ordinance oldOrd = Session["OriginalOrdinance"] as Ordinance;
                PropertyInfo[] properties = typeof(Ordinance).GetProperties();
                baseData.Add("StatusDescription");
                if (properties.Any(i => !i.GetValue(ordinance).Equals(i.GetValue(oldOrd)) && !baseData.Any(b => b.Contains(i.Name))))
                {
                    if (ordAuditVal < 0)
                    {
                        ordAuditVal = Factory.Instance.Insert(ordAudit, "sp_InsertOrdinance_Audit", Skips("ordAuditInsert"));
                    }
                    if (ordAuditVal > 0)
                    {
                        foreach (PropertyInfo property in properties.Where(i => !i.GetValue(ordinance).Equals(i.GetValue(oldOrd)) && !baseData.Any(b => b.Contains(i.Name))))
                        {
                            Audit audit = new Audit()
                            {
                                OrdinanceAuditID = ordAuditVal,
                                Label = property.Name,
                                DataType = property.GetValue(ordinance).GetType().Name
                            };
                            // ADD //
                            if (property.GetValue(oldOrd) == null || property.GetValue(oldOrd).ToString() == string.Empty || property.GetValue(oldOrd).ToString() == "-1" || property.GetValue(oldOrd).ToString() == "-1.00")
                            {
                                audit.Type = "add";
                                audit.OldValue = string.Empty;
                                audit.NewValue = property.GetValue(ordinance).ToString();
                                if (audit.DataType.Equals("DateTime") || audit.Label.Equals("ContractStartDate") || audit.Label.Equals("ContractEndDate"))
                                {
                                    audit.NewValue = Convert.ToDateTime(property.GetValue(ordinance).ToString()).ToString("MM/dd/yyyy");
                                }
                            }
                            // REMOVE //
                            else if (property.GetValue(ordinance) == null || property.GetValue(ordinance).ToString() == string.Empty || property.GetValue(ordinance).ToString() == "-1" || property.GetValue(ordinance).ToString() == "-1.00")
                            {
                                audit.Type = "remove";
                                audit.OldValue = property.GetValue(oldOrd).ToString();
                                audit.NewValue = string.Empty;
                                if (audit.DataType.Equals("DateTime") || audit.Label.Equals("ContractStartDate") || audit.Label.Equals("ContractEndDate"))
                                {
                                    audit.OldValue = Convert.ToDateTime(property.GetValue(oldOrd).ToString()).ToString("MM/dd/yyyy");
                                }
                            }
                            // UPDATE //
                            else
                            {
                                audit.Type = "update";
                                audit.OldValue = property.GetValue(oldOrd).ToString();
                                audit.NewValue = property.GetValue(ordinance).ToString();
                                if (audit.DataType.Equals("DateTime") || audit.Label.Equals("ContractStartDate") || audit.Label.Equals("ContractEndDate"))
                                {
                                    audit.OldValue = Convert.ToDateTime(property.GetValue(oldOrd).ToString()).ToString("MM/dd/yyyy");
                                    audit.NewValue = Convert.ToDateTime(property.GetValue(ordinance).ToString()).ToString("MM/dd/yyyy");
                                }
                            }
                            if (audit.OldValue == "False")
                            {
                                audit.OldValue = "No";
                            }
                            if (audit.OldValue == "True")
                            {
                                audit.OldValue = "Yes";
                            }
                            if (audit.NewValue == "True")
                            {
                                audit.NewValue = "Yes";
                            }
                            if (audit.NewValue == "False")
                            {
                                audit.NewValue = "No";
                            }
                            auditList.Add(audit);
                        }
                    }
                }
                baseData.Remove("StatusDescription");
            }
            if (addDocNames.Count > 0)
            {
                if (ordAuditVal < 0)
                {
                    ordAuditVal = Factory.Instance.Insert(ordAudit, "sp_InsertOrdinance_Audit", Skips("ordAuditInsert"));
                }
                if (ordAuditVal > 0)
                {
                    foreach (string item in addDocNames)
                    {
                        Audit audit = new Audit()
                        {
                            OrdinanceAuditID = ordAuditVal,
                            Label = "SupportingDocumentation",
                            DataType = "String",
                            Type = "add",
                            OldValue = string.Empty,
                            NewValue = item
                        };
                        auditList.Add(audit);
                    }
                }
            }
            if (removeDocNames.Count > 0)
            {
                if (ordAuditVal < 0)
                {
                    ordAuditVal = Factory.Instance.Insert(ordAudit, "sp_InsertOrdinance_Audit", Skips("ordAuditInsert"));
                }
                if (ordAuditVal > 0)
                {
                    foreach (string item in removeDocNames)
                    {
                        Audit audit = new Audit()
                        {
                            OrdinanceAuditID = ordAuditVal,
                            Label = "SupportingDocumentation",
                            DataType = "String",
                            Type = "remove",
                            OldValue = item,
                            NewValue = string.Empty
                        };
                        auditList.Add(audit);
                    }
                }
            }
            if (accAuditList.Count > 0)
            {
                if (accAuditList.Any(i => i.AccountingDesc.Equals("revenue")))
                {
                    if (ordAuditVal < 0)
                    {
                        ordAuditVal = Factory.Instance.Insert(ordAudit, "sp_InsertOrdinance_Audit", Skips("ordAuditInsert"));
                    }
                    if (ordAuditVal > 0)
                    {
                        Audit audit = new Audit()
                        {
                            OrdinanceAuditID = ordAuditVal,
                            Label = "Revenue",
                            DataType = "Accounting",
                            Type = "revenue",
                            NewValue = string.Empty,
                            OldValue = string.Empty
                        };
                        auditList.Add(audit);
                    }
                }
                if (accAuditList.Any(i => i.AccountingDesc.Equals("expenditure")))
                {
                    if (ordAuditVal < 0)
                    {
                        ordAuditVal = Factory.Instance.Insert(ordAudit, "sp_InsertOrdinance_Audit", Skips("ordAuditInsert"));
                    }
                    if (ordAuditVal > 0)
                    {
                        Audit audit = new Audit()
                        {
                            OrdinanceAuditID = ordAuditVal,
                            Label = "Expenditure",
                            DataType = "Accounting",
                            Type = "expenditure",
                            NewValue = string.Empty,
                            OldValue = string.Empty
                        };
                        auditList.Add(audit);
                    }
                }
            }

            if (auditList.Count > 0)
            {
                foreach (Audit item in auditList)
                {
                    auditVal = Factory.Instance.Insert(item, "sp_InsertAuditDescription", Skips("auditInsert"));

                    if (auditVal > 0 && item.Type.Equals("revenue") && accAuditList.Count > 0)
                    {
                        foreach (AccountingAudit accAudit in accAuditList.Where(i => i.AccountingDesc.Equals("revenue")))
                        {
                            accAudit.AuditID = auditVal;
                            acctAuditVal = Factory.Instance.Insert(accAudit, "sp_InsertAccountingAuditDescription", Skips("acctAuditInsert"));
                        }
                    }
                    else
                    {
                        acctAuditVal = 1;
                    }
                    if (auditVal > 0 && item.Type.Equals("expenditure") && accAuditList.Count > 0)
                    {
                        foreach (AccountingAudit accAudit in accAuditList.Where(i => i.AccountingDesc.Equals("expenditure")))
                        {
                            accAudit.AuditID = auditVal;
                            acctAuditVal = Factory.Instance.Insert(accAudit, "sp_InsertAccountingAuditDescription", Skips("acctAuditInsert"));
                        }
                    }
                    else
                    {
                        acctAuditVal = 1;
                    }
                }
            }
            else
            {
                auditVal = 1;
            }


            List<int> submitVals = new List<int>(new int[] 
            {
                retVal,
                statusVal,
                removeDocVal,
                addDocsVal,
                addUploadedDocsVal,
                //removeAccsVal,
                //removeOrdAccsVal,
                //updateRevAccsVal,
                //updateExpAccsVal,
                insertSignatureVal,
                sigAuditVal,
                auditVal
            });

            if (submitVals.All(i => i > 0))
            {
                Session["SubmitStatus"] = "success";
                Session["ToastColor"] = "text-bg-success";
                switch (rejected)
                {
                    case true:
                        Session["ToastMessage"] = "Rejection Sent!";
                        break;
                    case false:
                        Session["ToastMessage"] = "Form Saved!";
                        //Email.Instance.SendEmail(newEmail, emailList);
                        break;
                }
                Response.Redirect("./Ordinances");
            }
            else
            {
                Session["SubmitStatus"] = "error";
                Session["ToastColor"] = "text-bg-danger";
                Session["ToastMessage"] = "Something went wrong while saving!";
            }
        }
        protected void mdlDeleteSubmit_ServerClick(object sender, EventArgs e)
        {
            int ordID = Convert.ToInt32(hdnOrdID.Value);
            Ordinance ord = Factory.Instance.GetByID<Ordinance>(ordID, "sp_GetOrdinanceByOrdinanceID", "OrdinanceID");
            OrdinanceStatus ordStatus = new OrdinanceStatus();
            ordStatus.OrdinanceStatusID = Convert.ToInt32(hdnOrdStatusID.Value);
            ordStatus.OrdinanceID = Convert.ToInt32(hdnOrdID.Value);
            ordStatus.StatusID = 8;
            ordStatus.LastUpdateBy = _user.Login;
            ordStatus.LastUpdateDate = DateTime.Now;
            ordStatus.EffectiveDate = Convert.ToDateTime(hdnEffectiveDate.Value);
            ordStatus.ExpirationDate = DateTime.MaxValue;
            int retVal = Factory.Instance.Expire<Ordinance>(ord, "sp_UpdateOrdinance", Skips("ordUpdate"));

            List<string> addEmailList = new List<string>()
            {
                _user.Email.ToLower(),
                ord.RequestEmail.ToLower()
            };
            foreach (string item in addEmailList)
            {
                Email.Instance.AddEmailAddress(emailList, item);
            }
            string formType = "Ordinance Fact Sheet";
            string href = $"apptest/Themis/Ordinances?id={hdnOrdID.Value}&v=view";
            string ordinanceNumInfo = !ordinanceNumber.Text.IsNullOrWhiteSpace() ? $"<p style='margin: 0; line-height: 1.5;'><span>Ordinance: {ord.OrdinanceNumber}</span></p>" : string.Empty;

            Email newEmail = new Email();

            newEmail.EmailSubject = $"{formType} DELETED";
            newEmail.EmailTitle = $"{formType} DELETED";
            newEmail.EmailText = $"<p style='margin: 0;'><span style='font-size:36.0pt;font-family:\"Times New Roman\",serif;color:#2D71D5;font-weight:bold;'>THΣMIS</span></p><div align=center style='text-align:center'><span><hr size='2' width='100%' align='center' style='margin-top: 0;'></span></div><p><span>An <b>{formType}</b> has been DELETED by <b>{_user.FirstName} {_user.LastName}</b>.</span></p><br /><p style='margin: 0; line-height: 1.5;'><span>ID: {ord.OrdinanceID}</span></p>{ordinanceNumInfo}<p style='margin: 0; line-height: 1.5;'><span>Date: {DateTime.Now}</span></p><p style='margin: 0; line-height: 1.5;'><span>Department: {requestDepartment.SelectedItem.Text}</span></p><p style='margin: 0; line-height: 1.5;'><span>Contact: {requestContact.Text}</span></p><p style='margin: 0; line-height: 1.5;'><span>Phone: {ord.RequestPhone}</span></p><p><span>Status: Deleted</span></p><br /><p><span>Please click the button below to view the deleted document:</span></p><table border='0' cellpadding='0' cellspacing='0' style='border-collapse: separate; mso-table-lspace: 0pt; mso-table-rspace: 0pt; width: auto;'><tr><td style='font-family: sans-serif; font-size: 14px; vertical-align: top; background-color: #0d6efd; border-radius: 5px; text-align: center;' valign='top' bgcolor='#0d6efd' align='center'><a href='{href}' target='_blank' style='display: inline-block; color: #ffffff; background-color: #0d6efd; border: solid 1px #0d6efd; border-radius: 5px; box-sizing: border-box; cursor: pointer; text-decoration: none; font-size: 18px; font-weight: bold; margin: 0; padding: 15px 25px; text-transform: capitalize; border-color: #0d6efd; '>View Ordinance</a></td></tr></table>";

            if (retVal > 0)
            {
                int statusVal = Factory.Instance.Update(ordStatus, "sp_UpdateOrdinance_Status", Skips("ordStatusUpdate"));
                
                if (statusVal > 0)
                {
                    OrdinanceAudit deleteAudit = new OrdinanceAudit()
                    {
                        OrdinanceID = Convert.ToInt32(hdnOrdID.Value),
                        UpdateType = $"DELETED",
                        LastUpdateBy = $"{_user.FirstName} {_user.LastName}",
                        LastUpdateDate = DateTime.Now,
                    };
                    int deleteAuditVal = Factory.Instance.Insert(deleteAudit, "sp_InsertOrdinance_Audit", Skips("ordAuditInsert"));
                    if (deleteAuditVal > 0)
                    {

                        Session["SubmitStatus"] = "success";
                        Session["ToastColor"] = "text-bg-success";
                        Session["ToastMessage"] = "Entry Deleted!";
                        Email.Instance.SendEmail(newEmail, emailList, true);
                        Response.Redirect("./Ordinances");
                    }
                    else
                    {
                        Session["SubmitStatus"] = "error";
                        Session["ToastColor"] = "text-bg-danger";
                        Session["ToastMessage"] = "Something went wrong while deleting!";
                    }
                }
                else
                {
                    Session["SubmitStatus"] = "error";
                    Session["ToastColor"] = "text-bg-danger";
                    Session["ToastMessage"] = "Something went wrong while deleting!";
                }
            }
            else
            {
                Session["SubmitStatus"] = "error";
                Session["ToastColor"] = "text-bg-danger";
                Session["ToastMessage"] = "Something went wrong while deleting!";
            }
        }
        protected void backBtn_Click(object sender, EventArgs e)
        {
            if (Request.QueryString["id"] != null)
            {
                Response.Redirect("./Ordinances");
            }
            else
            {
                ordTable.Visible = true;
                List<TextBox> sigTextBoxes = new List<TextBox>()
                {
                    fundsCheckBySig,
                    directorSupervisorSig,
                    cPASig,
                    obmDirectorSig,
                    mayorSig
                };
                foreach (TextBox item in sigTextBoxes)
                {
                    item.Text = string.Empty;
                }
                SaveFactSheet.CssClass = SaveFactSheet.CssClass.Replace(" emphasize", string.Empty);
                ordView.Visible = false;

                Dictionary<string, LinkButton> pageBtns = new Dictionary<string, LinkButton>()
                {
                    { "firstBtn", lnkFirstSearchP },
                    { "previousBtn", lnkPreviousSearchP },
                    { "nextBtn", lnkNextSearchP },
                    { "lastBtn", lnkLastSearchP },
                };
                SetPagination(rpOrdinanceTable, pageBtns, pnlPagingP, lblCurrentPageBottomSearchP, 10, true);
                List<Ordinance> ord_list = new List<Ordinance>();
                ord_list = (List<Ordinance>)Session["ord_list"];
                BindDataRepeaterPagination("no", ord_list);
                auditTab.Attributes["class"] = "nav-link ordTabs";
                auditTab.Attributes["aria=selected"] = "false";
                factSheetTab.Attributes["class"] += " active";
                factSheetTab.Attributes["aria=selected"] = "true";
                auditPane.Attributes["class"] = "tab-pane fade";
                factSheetPane.Attributes["class"] += " active show";
            }
        }
        protected void copyOrd_Click(object sender, EventArgs e)
        {
            Response.Redirect($"./NewFactSheet?id={hdnOrdID.Value}");
        }
        protected void btnSendSigEmail_Click(object sender, EventArgs e)
        {
            SignatureRequest sigRequests = Session["SigRequestEmails"] as SignatureRequest;
            PropertyInfo sigType = (PropertyInfo)typeof(SignatureRequest).GetProperties().First(i => i.Name.Equals(sigBtnType.Value));

            List<string> emails = sigType.GetValue(sigRequests).ToString().Split(';').Where(i => !i.IsNullOrWhiteSpace()).ToList();

            foreach (string item in emails)
            {
                Email.Instance.AddEmailAddress("SingleEmail", item);
            }

            string href = $"apptest/Themis/Ordinances?id={hdnOrdID.Value.ToString()}&v=edit&f={sigBtnTarget.Value.ToString()}";
            string formType = "THΣMIS";

            Email newEmail = new Email();

            newEmail.EmailSubject = $"{formType} Signature Requested";
            newEmail.EmailTitle = $"{formType} Signature Requested";
            newEmail.EmailText = $"<p style='margin: 0;'><span style='font-size:36.0pt;font-family:\"Times New Roman\",serif;color:#2D71D5;font-weight:bold'>THΣMIS</span></p><div align=center style='text-align:center'><span><hr size='2' width='100%' align='center' style='margin-top: 0;'></span></div><p><span>You are receiving this message because your signature is required in the role of <b>{sigBtnLabel.Value.ToString()}</b> for Ordinance ID #{hdnOrdID.Value.ToString()} on THΣMIS.</span></p><p><span>Please click the button below to review and sign the document</span></p><table border='0' cellpadding='0' cellspacing='0' style='border-collapse: separate; mso-table-lspace: 0pt; mso-table-rspace: 0pt; width: auto;'><tr><td style='font-family: sans-serif; font-size: 14px; vertical-align: top; background-color: #198754; border-radius: 5px; text-align: center;' valign='top' bgcolor='#198754' align='center'><a href='{href}' target='_blank' style='display: inline-block; color: #ffffff; background-color: #198754; border: solid 1px #198754; border-radius: 5px; box-sizing: border-box; cursor: pointer; text-decoration: none; font-size: 18px; font-weight: bold; margin: 0; padding: 15px 25px; text-transform: capitalize; border-color: #198754; '>Sign Ordinance</a></td></tr></table><br /><p><span>Thank you for your prompt attention to this matter.</span></p>";

            Email.Instance.SendEmail(newEmail, "SingleEmail");
        }
        protected void btnSignDoc_Click(object sender, EventArgs e)
        {
            switch (sigType.Value)
            {
                case "fundsCheckBy":
                    fundsCheckByBtnDiv.Visible = false;
                    fundsCheckByInputGroup.Visible = true;
                    fundsCheckEmailBtn.Visible = false;
                    fundsCheckBySig.Text = sigName.Text;
                    fundsCheckByDate.Text = sigDate.Text;
                    break;
                case "directorSupervisor":
                    directorSupervisorBtnDiv.Visible = false;
                    directorSupervisorInputGroup.Visible = true;
                    directorSupervisorEmailBtn.Visible = false;
                    directorSupervisorSig.Text = sigName.Text;
                    directorSupervisorDate.Text = sigDate.Text;
                    break;
                case "cPA":
                    cPABtnDiv.Visible = false;
                    cPAInputGroup.Visible = true;
                    cPAEmailBtn.Visible = false;
                    cPASig.Text = sigName.Text;
                    cPADate.Text = sigDate.Text;
                    break;
                case "obmDirector":
                    obmDirectorBtnDiv.Visible = false;
                    obmDirectorInputGroup.Visible = true;
                    obmDirectorEmailBtn.Visible = false;
                    obmDirectorSig.Text = sigName.Text;
                    obmDirectorDate.Text = sigDate.Text;
                    break;
                case "mayor":
                    mayorBtnDiv.Visible = false;
                    mayorInputGroup.Visible = true;
                    mayorEmailBtn.Visible = false;
                    mayorSig.Text = sigName.Text;
                    mayorDate.Text = sigDate.Text;
                    break;
            }

            OrdinanceSignature signature = new OrdinanceSignature();
            signature.OrdinanceID = Convert.ToInt32(hdnOrdID.Value);
            signature.SignatureType = sigType.Value;
            signature.Signature = sigName.Text;
            signature.DateSigned = Convert.ToDateTime(sigDate.Text);
            signature.SignatureCertified = certifySig.Checked;
            signature.LastUpdateBy = _user.Login;
            signature.LastUpdateDate = DateTime.Now;

            List<OrdinanceSignature> insertSigList = (List<OrdinanceSignature>)Session["insertSigList"] ?? new List<OrdinanceSignature>();
            insertSigList.Add(signature);
            Session["insertSigList"] = insertSigList;
            SaveFactSheet.CssClass += " emphasize";
        }
        protected void sendRejection_Click(object sender, EventArgs e)
        {
            List<string> addEmailList = new List<string>()
            {
                _user.Email.ToLower(),
                requestEmail.Text.ToLower()
            };
            foreach (string item in addEmailList)
            {
                Email.Instance.AddEmailAddress(emailList, item);
            }
            string formType = "Ordinance Fact Sheet";
            string href = $"apptest/Themis/Ordinances?id={hdnOrdID.Value}&v=edit";
            string ordinanceNumInfo = !ordinanceNumber.Text.IsNullOrWhiteSpace() ? $"<p style='margin: 0; line-height: 1.5;'><span>Ordinance: {ordinanceNumber.Text}</span></p>" : string.Empty;
            string reason = rejectionReason.Text;

            Email newEmail = new Email();

            newEmail.EmailSubject = $"{formType} REJECTED";
            newEmail.EmailTitle = $"{formType} REJECTED";
            newEmail.EmailText = $"<p style='margin: 0;'><span style='font-size:36.0pt;font-family:\"Times New Roman\",serif;color:#2D71D5;font-weight:bold;'>THΣMIS</span></p><div align=center style='text-align:center'><span><hr size='2' width='100%' align='center' style='margin-top: 0;'></span></div><p><span>An <b>{formType}</b> has been REJECTED by <b>{_user.FirstName} {_user.LastName}</b>.</span></p><br /><p style='margin: 0; line-height: 1.5;'><span>ID: {hdnOrdID.Value}</span></p>{ordinanceNumInfo}<p style='margin: 0; line-height: 1.5;'><span>Date: {DateTime.Now}</span></p><p><span>Status: Rejected</span></p><br /><p style='margin: 0; line-height: 1.5;'><span>Rejection Reason:</span></p><p style='margin: 0; line-height: 1.5;'><span>{rejectionReason.Text}</span></p><p><span>Please click the button below to review the document and make changes if necessary:</span></p><table border='0' cellpadding='0' cellspacing='0' style='border-collapse: separate; mso-table-lspace: 0pt; mso-table-rspace: 0pt; width: auto;'><tr><td style='font-family: sans-serif; font-size: 14px; vertical-align: top; background-color: #0d6efd; border-radius: 5px; text-align: center;' valign='top' bgcolor='#0d6efd' align='center'><a href='{href}' target='_blank' style='display: inline-block; color: #ffffff; background-color: #0d6efd; border: solid 1px #0d6efd; border-radius: 5px; box-sizing: border-box; cursor: pointer; text-decoration: none; font-size: 18px; font-weight: bold; margin: 0; padding: 15px 25px; text-transform: capitalize; border-color: #0d6efd; '>View Ordinance</a></td></tr></table><br /><p><span>If you believe this is a mistake or have any questions please contact the rejector at <a href='mailto:{_user.Email.ToLower()}'>{_user.Email.ToLower()}</a></span></p>";

            Email.Instance.SendEmail(newEmail, emailList);

            int rejectedOrdAuditVal = new int();
            int rejectedAuditVal = new int();
            OrdinanceAudit rejectedAudit = new OrdinanceAudit()
            {
                OrdinanceID = Convert.ToInt32(hdnOrdID.Value),
                UpdateType = "REJECTED",
                LastUpdateBy = $"{_user.FirstName} {_user.LastName}",
                LastUpdateDate = DateTime.Now,
            };
            rejectedOrdAuditVal = Factory.Instance.Insert(rejectedAudit, "sp_InsertOrdinance_Audit", Skips("ordAuditInsert"));
            if (rejectedOrdAuditVal > 0)
            {
                Audit audit = new Audit()
                {
                    OrdinanceAuditID = rejectedOrdAuditVal,
                    Label = "RejectionReason",
                    DataType = "String",
                    Type = "rejected",
                    OldValue = string.Empty,
                    NewValue = reason
                };
               rejectedAuditVal = Factory.Instance.Insert(audit, "sp_InsertAuditDescription", Skips("auditInsert"));
            }
            if (rejectedAuditVal > 0)
            {
                SaveFactSheet_Click(true, e);
            }
        }
        protected void cancelRejection_Click(object sender, EventArgs e)
        {
            ddStatus.SelectedValue = hdnStatusID.Value;
        }

    }
}