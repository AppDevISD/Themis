using DataLibrary;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Timers;
using static DataLibrary.TablePagination;
using AjaxControlToolkit;
using System.Diagnostics;
using Microsoft.Ajax.Utilities;
using System.Linq;

namespace WebUI
{
    public partial class Ordinances : System.Web.UI.Page
    {
        public string toastColor;
        public string toastMessage;        

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack && !Response.IsRequestBeingRedirected)
            {
                GetAllDepartments();
                GetAllPurchaseMethods();
                SetStartupActives();
                SetPagination(rpOrdinanceTable, 10);
                GetStartupData();
            }
            foreach (RepeaterItem item in rpOrdinanceTable.Items)
            {
                LinkButton editButton = item.FindControl("editOrd") as LinkButton;
                LinkButton viewButton = item.FindControl("viewOrd") as LinkButton;
                ScriptManager.GetCurrent(Page).RegisterAsyncPostBackControl(editButton);
                ScriptManager.GetCurrent(Page).RegisterAsyncPostBackControl(viewButton);
            }
            SubmitStatus();
        }
        protected void SetStartupActives()
        {
            ordView.Visible = false;
        }
        protected void GetAllDepartments()
        {
            Dictionary<string, string> departments = Utility.Instance.DepartmentsList();
            foreach (var department in departments.Keys)
            {
                var value = departments[department];
                ListItem newItem = new ListItem(department, value);
                requestDepartment.Items.Add(newItem);
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
        protected void SetPagination(Repeater rpTable, int ItemsPerPage)
        {
            SetViewState(ViewState, ItemsPerPage);
            GetControls(lnkFirstSearchP, lnkPreviousSearchP, lnkNextSearchP, lnkLastSearchP, rpTable, pnlPagingP, lblCurrentPageBottomSearchP);
        }
        protected void GetStartupData()
        {
            List<Ordinance> ord_list = new List<Ordinance>();
            ord_list = Factory.Instance.GetAll<Ordinance>("sp_GetOrdinanceByEffective");
            if (ord_list.Count > 0)
            {
                BindDataRepeaterPagination("yes", ord_list);
            }

            Session["ord_list"] = ord_list;
        }
        protected void mdlDeleteSubmit_ServerClick(object sender, EventArgs e)
        {
            int FormID = Convert.ToInt32(deleteID.Value);
            int retVal = Factory.Instance.Delete<Ordinance>(FormID, "Ordinance");
            if (retVal > 0)
            {
                List<Ordinance> ord_list = new List<Ordinance>();
                ord_list = Session["ord_list"] as List<Ordinance>;
                ord_list = Factory.Instance.GetAll<Ordinance>("sp_GetOrdinanceByEffective");
                rpOrdinanceTable.DataSource = ord_list;
                rpOrdinanceTable.DataBind();
                Session["ord_list"] = ord_list;
                Session["SubmitStatus"] = "success";
                Session["ToastColor"] = "text-bg-success";
                Session["ToastMessage"] = "Entry Deleted!";
                Response.Redirect("./Ordinances");
            }
            else
            {
                Session["SubmitStatus"] = "error";
                Session["ToastColor"] = "text-bg-danger";
                Session["ToastMessage"] = "Something went wrong while submitting!";
            }
        }
        protected void paginationBtn_Click(object sender, EventArgs e)
        {
            SetPagination(rpOrdinanceTable, 10);
            List<Ordinance> ord_list = new List<Ordinance>();
            ord_list = (List<Ordinance>)Session["ord_list"];
            HtmlButton button = (HtmlButton)sender;
            string commandName = button.Attributes["data-command"];
            PageButtonClick(ord_list, commandName);
        }
        protected void SubmitStatus()
        {
            if (Session["SubmitStatus"] != null || (string)Session["SubmitStatus"] == "success")
            {
                toastColor = (string)Session["ToastColor"];
                toastMessage = (string)Session["ToastMessage"];
            }
            else
            {
                Session["SubmitStatus"] = "error";
                Session["ToastColor"] = "text-bg-danger";
                Session["ToastMessage"] = "Something went wrong while submitting!";
                toastColor = (string)Session["ToastColor"];
                toastMessage = (string)Session["ToastMessage"];
            }
        }

        protected void rpOrdinanceTable_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            HiddenField hdnID = (HiddenField)e.Item.FindControl("hdnID");
            int ordID = Convert.ToInt32(hdnID.Value);
            Ordinance ord = Factory.Instance.GetByID<Ordinance>(ordID, "sp_GetOrdinanceByOrdinanceID", "OrdinanceID");

            requestDepartment.SelectedValue = Utility.Instance.DepartmentsList()[ord.RequestDepartment];
            firstReadDate.Text = ord.FirstReadDate.ToString("yyyy-MM-dd");
            requestContact.Text = ord.RequestContact;
            requestPhone.Text = ord.RequestPhone.SubstringUpToFirst('x');
            requestExt.Text = ord.RequestPhone.Substring(14);

            switch (ord.EmergencyPassage)
            {
                case true:
                    epYes.Checked = true;
                    epJustification.Visible = true;
                    break;
                case false:
                    epNo.Checked = true;
                    epJustification.Visible = false;
                    break;
            }
            epJustification.Text = ord.EmergencyPassageReason;

            fiscalImpact.Text = ord.OrdinanceFiscalImpact.ToString();
            suggestedTitle.Text = ord.OrdinanceTitle;

            vendorName.Text = ord.ContractVendorName;
            vendorNumber.Text = ord.ContractVendorNumber;
            contractStartDate.Text = ord.ContractStartDate;
            contractEndDate.Text = ord.ContractEndDate;
            contractTerm.Value = ord.ContractTerm;
            contractAmount.Text = ord.ContractAmount.ToString();

            switch (ord.ScopeChange)
            {
                case true:
                    scYes.Checked = true;
                    scopeChangeOptions.Visible = true;
                    break;
                case false:
                    scNo.Checked = true;
                    scopeChangeOptions.Visible = false;
                    break;
            }
            changeOrderNumber.Text = ord.ChangeOrderNumber;
            additionalAmount.Text = ord.AdditionalAmount.ToString();

            purchaseMethod.SelectedValue = ord.ContractMethod;
            switch (purchaseMethod.SelectedValue)
            {
                default:
                    otherException.Visible = false;
                    break;
                case "Other":
                case "Exception":
                    otherException.Visible = true;
                    break;
            }
            otherException.Text = ord.OtherException;
            prevOrdinanceNums.Text = ord.PreviousOrdinanceNumbers;
            codeProvision.Text = ord.CodeProvision;

            switch (ord.PAApprovalRequired)
            {
                case true:
                    paApprovalRequiredYes.Checked = true;                    
                    break;
                case false:
                    paApprovalRequiredNo.Checked = true;
                    break;
            }
            switch (ord.PAApprovalIncluded)
            {
                case true:
                    paApprovalAttachedYes.Checked = true;                    
                    break;
                case false:
                    paApprovalAttachedNo.Checked = true;
                    break;
            }

            List<OrdinanceAccounting> ordAcc = Factory.Instance.GetAllLookup<OrdinanceAccounting>(ordID, "sp_GetOrdinanceAccountingByOrdinanceID", "OrdinanceID");

            if (ordAcc.Count > 0)
            {
                List<Accounting> revItems = new List<Accounting>();
                List<Accounting> expItems = new List<Accounting>();
                foreach (OrdinanceAccounting item in ordAcc)
                {
                    Accounting acctItem = Factory.Instance.GetByID<Accounting>(item.AccountingID, "sp_GetOrdinanceAccountingByAccountingID", "AccountingID");
                    switch (acctItem.AccountingDesc)
                    {
                        case "revenue":
                            revItems.Add(acctItem);
                            break;
                        case "expenditure":
                            expItems.Add(acctItem);
                            break;
                    }
                }

                if (revItems.Count > 0)
                {
                    Session["ordRevTable"] = revItems;
                    rpRevenueTable.DataSource = revItems;
                    rpRevenueTable.DataBind();
                }
                if (expItems.Count > 0)
                {
                    Session["ordExpTable"] = expItems;
                    rpExpenditureTable.DataSource = expItems;
                    rpExpenditureTable.DataBind();
                }
            }
            else
            {
                Session.Remove("ordRevTable");
                Session.Remove("ordExpTable");
                rpRevenueTable.DataSource = null;
                rpExpenditureTable.DataSource = null;
                rpRevenueTable.DataBind();
                rpExpenditureTable.DataBind();
            }
                      
            

            staffAnalysis.Text = ord.OrdinanceAnalysis;

            switch (e.CommandName)
            {
                case "view":
                    ordView.Attributes["readonly"] = "true";
                    prevOrdinanceNums.Attributes["placeholder"] = string.Empty;
                    if (rpRevenueTable.Items.Count > 0)
                    {
                        foreach (RepeaterItem item in rpRevenueTable.Items)
                        {
                            HtmlGenericControl removeRevRow = item.FindControl("removeRevRowDiv") as HtmlGenericControl;
                            removeRevRow.Visible = false;
                        }
                    }
                    if (rpExpenditureTable.Items.Count > 0)
                    {
                        foreach (RepeaterItem item in rpExpenditureTable.Items)
                        {
                            HtmlGenericControl removeExpRow = item.FindControl("removeExpRowDiv") as HtmlGenericControl;
                            removeExpRow.Visible = false;
                        }
                    }
                    break;
                case "edit":
                    ordView.Attributes["readonly"] = "false";
                    if (rpRevenueTable.Items.Count > 0)
                    {
                        foreach (RepeaterItem item in rpRevenueTable.Items)
                        {
                            HtmlGenericControl removeRevRow = item.FindControl("removeRevRowDiv") as HtmlGenericControl;
                            removeRevRow.Visible = true;
                            //ScriptManager.GetCurrent(Page).RegisterAsyncPostBackControl(removeRevRow);
                        }
                    }
                    if (rpExpenditureTable.Items.Count > 0)
                    {
                        foreach (RepeaterItem item in rpExpenditureTable.Items)
                        {
                            HtmlGenericControl removeExpRow = item.FindControl("removeExpRowDiv") as HtmlGenericControl;
                            removeExpRow.Visible = true;
                            //ScriptManager.GetCurrent(Page).RegisterAsyncPostBackControl(removeExpRow);
                        }
                    }
                    break;
            }

            ScriptManager.RegisterStartupScript(this, this.GetType(), "FadeOutOrdTable", "OrdTableFadeOut();", true);
            ordView.Visible = true;
        }

        protected void backBtn_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "FadeInOrdTable", "OrdTableFadeIn();", true);
        }
    }
}