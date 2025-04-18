﻿using DataLibrary;
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using static DataLibrary.TablePagination;
using System.Diagnostics;
using Microsoft.Ajax.Utilities;
using System.Web.Services;
using static DataLibrary.Utility;
using ISD.ActiveDirectory;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using Microsoft.Reporting.WebForms;
using System.Data;

namespace WebUI
{
    public partial class Ordinances : System.Web.UI.Page
    {
        private ADUser _user = new ADUser();
        private UserInfo userInfo = new UserInfo();
        private string emailList = "NewFactSheetEmailList";
        public string toastColor;
        public string toastMessage;

        protected void Page_Load(object sender, EventArgs e)
        {
            Page.Form.Attributes.Add("enctype", "multipart/form-data");
            _user = Session["CurrentUser"] as ADUser;
            userInfo = Session["UserInformation"] as UserInfo;

            if (!Page.IsPostBack && !Response.IsRequestBeingRedirected)
            {
                Session.Remove("ordRevTable");
                Session.Remove("ordExpTable");
                Session.Remove("ordDocs");
                Session.Remove("addOrdDocs");
                Session["sortBtn"] = "sortDate";
                Session["sortDir"] = "desc";
                Session["curCmd"] = "EffectiveDate";
                Session["curDir"] = "desc";
                GetAllDepartments();
                GetAllStatuses();
                GetAllPurchaseMethods();
                SetStartupActives();
                SetPagination(rpOrdinanceTable, 10);
                GetStartupData(userInfo.IsAdmin);
            }            
            if (Page.IsPostBack && Page.Request.Params.Get("__EVENTTARGET").Contains("adminSwitch"))
            {
                userInfo = ((SiteMaster)Page.Master).UserView();
                Session.Remove("ordRevTable");
                Session.Remove("ordExpTable");
                Session.Remove("ordDocs");
                Session.Remove("addOrdDocs");
                Session["sortBtn"] = "sortDate";
                Session["sortDir"] = "desc";
                Session["curCmd"] = "EffectiveDate";
                Session["curDir"] = "desc";
                SetStartupActives();
                SetPagination(rpOrdinanceTable, 10);
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

            GetUploadedImages();
            SubmitStatus();
        }
        protected void SetStartupActives()
        {
            ordTable.Visible = true;
            ordView.Visible = false;
            lblNoItems.Visible = false;
            filterDepartmentDiv.Visible = !userInfo.IsAdmin || userInfo.UserView ? false : true;
        }
        public void GetStartupData(bool isAdmin)
        {
            List<Ordinance> ord_list = new List<Ordinance>();
            ord_list = Factory.Instance.GetAll<Ordinance>("sp_GetOrdinanceByEffective");
            if (ord_list.Count > 0)
            {
                foreach (Ordinance ord in ord_list)
                {
                    OrdinanceStatus ordStatus = Factory.Instance.GetByID<OrdinanceStatus>(ord.OrdinanceID, "sp_GetOrdinanceStatusesByOrdinanceID", "OrdinanceID");
                    ord.StatusDescription = ordStatus.StatusDescription;
                }
                if ((userInfo.UserDepartmentName != null && !isAdmin) || userInfo.UserView)
                {
                    ord_list = FilterList(ord_list, "department", userInfo.UserDepartmentName);
                }
                BindDataRepeaterPagination("yes", ord_list);
            }

            Session["ord_list"] = ord_list;
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
        protected void GetAllStatuses()
        {
            Dictionary<string, string> statuses = StatusList();
            foreach (var status in statuses.Keys)
            {
                var value = statuses[status];
                ListItem newItem = new ListItem(status, value);
                if (newItem.Text != "New")
                {
                    ddStatus.Items.Add(newItem);
                }
                filterStatus.Items.Add(newItem);
            }
        }
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
        protected void GetAllPurchaseMethods()
        {
            purchaseMethod.Items.Insert(0, new ListItem("Select Purchase Method...", null));
            purchaseMethod.Items.Insert(1, new ListItem("Low Bid", "Low Bid"));
            purchaseMethod.Items.Insert(2, new ListItem("Low Bid Meeting Specs", "Low Bid Meeting Specs"));
            purchaseMethod.Items.Insert(3, new ListItem("Low Evaluated Bid", "Low Evaluated Bid"));
            purchaseMethod.Items.Insert(4, new ListItem("Other", "Other"));
            purchaseMethod.Items.Insert(5, new ListItem("Exception", "Exception"));
        }
        protected void PurchaseMethodSelectedIndexChanged(object sender, EventArgs e)
        {
            switch (purchaseMethod.SelectedItem.Value)
            {
                default:
                    otherException.Enabled = false;
                    otherException.Text = string.Empty;
                    otherException.Attributes.Remove("required");
                    break;
                case "Other":
                case "Exception":
                    otherException.Enabled = true;
                    otherException.Attributes.Add("required", "true");
                    break;
            }
        }
        protected void EPCheckedChanged(object sender, EventArgs e)
        {
            switch (epYes.Checked)
            {
                case true:
                    epJustificationGroup.Visible = true;
                    epJustification.Attributes.Add("required", "true");
                    break;

                case false:
                    epJustificationGroup.Visible = false;
                    epJustification.Attributes.Remove("required");
                    break;
            }
        }
        protected void SCCheckedChanged(object sender, EventArgs e)
        {
            switch (scYes.Checked)
            {
                case true:
                    changeOrderNumber.Enabled = true;
                    additionalAmount.Enabled = true;
                    changeOrderNumber.Attributes.Add("required", "true");
                    additionalAmount.Attributes.Add("required", "true");
                    break;

                case false:
                    changeOrderNumber.Enabled = false;
                    additionalAmount.Enabled = false;
                    changeOrderNumber.Attributes.Remove("required");
                    additionalAmount.Attributes.Remove("required");
                    break;
            }
        }
        protected void SetPagination(Repeater rpTable, int ItemsPerPage)
        {
            SetViewState(ViewState, ItemsPerPage);
            GetControls(lnkFirstSearchP, lnkPreviousSearchP, lnkNextSearchP, lnkLastSearchP, rpTable, pnlPagingP, lblCurrentPageBottomSearchP);
        }        
        protected void mdlDeleteSubmit_ServerClick(object sender, EventArgs e)
        {
            int ordID = Convert.ToInt32(hdnOrdID.Value);
            Ordinance ord = Factory.Instance.GetByID<Ordinance>(ordID, "sp_GetOrdinanceByOrdinanceID", "OrdinanceID");
            int retVal = Factory.Instance.Expire<Ordinance>(ord, "sp_UpdateOrdinance", 1);
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
                Session["ToastMessage"] = "Something went wrong while deleting!";
            }
        }
        protected void paginationBtn_Click(object sender, EventArgs e)
        {
            SetPagination(rpOrdinanceTable, 10);
            List<Ordinance> ord_list = new List<Ordinance>();
            ord_list = (List<Ordinance>)Session["ord_list"];
            LinkButton button = (LinkButton)sender;
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
            ScriptManager.GetCurrent(Page).RegisterAsyncPostBackControl(backBtn);
            Session.Remove("ordRevTable");
            Session.Remove("ordExpTable");
            HiddenField hdnID = (HiddenField)e.Item.FindControl("hdnID");
            int ordID = Convert.ToInt32(hdnID.Value);
            Ordinance ord = Factory.Instance.GetByID<Ordinance>(ordID, "sp_GetOrdinanceByOrdinanceID", "OrdinanceID");
            hdnOrdID.Value = ordID.ToString();
            hdnEffectiveDate.Value = ord.EffectiveDate.ToString();


            requestDepartment.SelectedValue = DepartmentsList()[ord.RequestDepartment];
            firstReadDate.Text = ord.FirstReadDate.ToString("yyyy-MM-dd");
            requestContact.Text = ord.RequestContact;
            requestPhone.Text = ord.RequestPhone.SubstringUpToFirst('x');
            requestExt.Text = ord.RequestPhone.Substring(14);

            hdnEmail.Value = ord.RequestEmail;

            switch (ord.EmergencyPassage)
            {
                case true:
                    epYes.Checked = true;
                    epNo.Checked = false;
                    epJustificationGroup.Visible = true;
                    break;
                case false:
                    epYes.Checked = false;
                    epNo.Checked = true;
                    epJustificationGroup.Visible = false;
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
                    scNo.Checked = false;
                    scopeChangeOptions.Visible = true;
                    break;
                case false:
                    scYes.Checked = false;
                    scNo.Checked = true;
                    scopeChangeOptions.Visible = false;
                    break;
            }
            changeOrderNumber.Text = ord.ChangeOrderNumber;
            additionalAmount.Text = (ord.AdditionalAmount.ToString() != "-1.00") ? ord.AdditionalAmount.ToString() : string.Empty;


            purchaseMethod.SelectedValue = ord.ContractMethod;
            switch (purchaseMethod.SelectedValue)
            {
                default:
                    otherExceptionDiv.Visible = false;
                    break;
                case "Other":
                case "Exception":
                    otherExceptionDiv.Visible = true;
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
            List<Accounting> revItems = new List<Accounting>();
            List<Accounting> expItems = new List<Accounting>();
            if (ordAcc.Count > 0)
            {
                foreach (OrdinanceAccounting item in ordAcc)
                {
                    Accounting acctItem = Factory.Instance.GetByID<Accounting>(item.AccountingID, "sp_GetLkAccountingByAccountingID", "AccountingID");
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
                else
                {
                    Session.Remove("ordRevTable");
                    rpRevenueTable.DataSource = null;
                    rpRevenueTable.DataBind();
                }
                if (expItems.Count > 0)
                {
                    Session["ordExpTable"] = expItems;
                    rpExpenditureTable.DataSource = expItems;
                    rpExpenditureTable.DataBind();
                }
                else
                {
                    Session.Remove("ordExpTable");
                    rpExpenditureTable.DataSource = null;
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

            List<OrdinanceDocument> ordDocs = Factory.Instance.GetAllLookup<OrdinanceDocument>(ordID, "sp_GetOrdinanceDocumentsByOrdinanceID", "OrdinanceID");

            if (ordDocs.Count > 0)
            {
                supportingDocumentationDiv.Visible = true;
                Session["ordDocs"] = ordDocs;
                rpSupportingDocumentation.DataSource = ordDocs;
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

            OrdinanceStatus ordStatus = new OrdinanceStatus();

            switch (e.CommandName)
            {
                case "view":
                    ordView.Attributes["readonly"] = "true";
                    ddStatusDiv.Visible = false;
                    statusDiv.Visible = true;
                    requiredFieldDescriptor.Visible = false;
                    vendorNumber.Attributes["placeholder"] = "N/A";
                    contractTerm.Attributes["placeholder"] = "N/A";
                    prevOrdinanceNums.Attributes["placeholder"] = "N/A";
                    codeProvision.Attributes["placeholder"] = "N/A";
                    newRevenueRowDiv.Visible = false;
                    newExpenditureRowDiv.Visible = false;
                    supportingDocumentation.Visible = false;
                    UploadImageBtn.Visible = false;
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
                            break;
                        case "Under Review":
                            statusIcon.Attributes["class"] = "fas fa-hourglass-clock text-info";
                            statusLabel.Attributes["class"] = "text-info";
                            break;
                        case "Being Held":
                            statusIcon.Attributes["class"] = "fas fa-triangle-exclamation text-warning-light";
                            statusLabel.Attributes["class"] = "text-warning-light";
                            break;
                        case "Drafted":
                            statusIcon.Attributes["class"] = "fas fa-badge-check text-success";
                            statusLabel.Attributes["class"] = "text-success";
                            break;
                        case "Rejected":
                            statusIcon.Attributes["class"] = "fas fa-ban text-danger";
                            statusLabel.Attributes["class"] = "text-danger";
                            break;
                    }
                    break;
                case "edit":
                    ordView.Attributes["readonly"] = "false";
                    ddStatusDiv.Visible = true;
                    ordStatus = Factory.Instance.GetByID<OrdinanceStatus>(ord.OrdinanceID, "sp_GetOrdinanceStatusesByOrdinanceID", "OrdinanceID");
                    ord.StatusDescription = ordStatus.StatusDescription;
                    if (ordStatus.StatusDescription != "New")
                    {
                        ddStatus.SelectedValue = ordStatus.StatusID.ToString();
                    }
                    else
                    {
                        ddStatus.SelectedIndex = 0;
                    }
                    hdnOrdStatusID.Value = ordStatus.OrdinanceStatusID.ToString();
                    statusDiv.Visible = false;
                    requiredFieldDescriptor.Visible = true;
                    vendorNumber.Attributes["placeholder"] = "0123456789";
                    contractTerm.Attributes["placeholder"] = "Calculating Term...";
                    prevOrdinanceNums.Attributes["placeholder"] = "123-45-6789";
                    codeProvision.Attributes["placeholder"] = "0123456789";
                    contractStartDate.TextMode = TextBoxMode.Date;
                    contractEndDate.TextMode = TextBoxMode.Date;
                    newRevenueRowDiv.Visible = true;
                    newExpenditureRowDiv.Visible = true;
                    supportingDocumentationDiv.Visible = true;
                    supportingDocumentation.Visible = true;
                    UploadImageBtn.Visible = true;
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
                            revAmount.Attributes["placeholder"] = "$10,000.00";
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
                            expAmount.Attributes["placeholder"] = "$10,000.00";
                            ScriptManager.GetCurrent(Page).RegisterAsyncPostBackControl(removeExpRow);
                        }
                    }
                    foreach (RepeaterItem item in rpSupportingDocumentation.Items)
                    {
                        LinkButton deleteFile = item.FindControl("deleteFile") as LinkButton;
                        deleteFile.Visible = true;
                        ScriptManager.GetCurrent(Page).RegisterAsyncPostBackControl(deleteFile);
                    }

                    switch (scYes.Checked)
                    {
                        case true:
                            changeOrderNumber.Enabled = true;
                            additionalAmount.Enabled = true;
                            changeOrderNumber.Attributes.Add("required", "true");
                            additionalAmount.Attributes.Add("required", "true");
                            break;

                        case false:
                            changeOrderNumber.Enabled = false;
                            additionalAmount.Enabled = false;
                            changeOrderNumber.Attributes.Remove("required");
                            additionalAmount.Attributes.Remove("required");
                            break;
                    }
                    switch (purchaseMethod.SelectedItem.Value)
                    {
                        default:
                            otherException.Enabled = false;
                            otherException.Text = string.Empty;
                            otherException.Attributes.Remove("required");
                            break;
                        case "Other":
                        case "Exception":
                            otherException.Enabled = true;
                            otherException.Attributes.Add("required", "true");
                            break;
                    }
                    scopeChangeOptions.Visible = true;
                    otherExceptionDiv.Visible = true;
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
                    RevExpBool HideTables = new RevExpBool() { HideTables = hideTables};

                    IEnumerable<Ordinance> ordData = new[] { ord };
                    IEnumerable<RevExpBool> revExpBoolData = new[] { HideTables };
                    ReportDataSource ordinanceData = new ReportDataSource() { Name = "dsOrdinance", Value = ordData };
                    ReportDataSource revExpTableBoolData = new ReportDataSource() { Name = "dsRevExpBool", Value = revExpBoolData };
                    ReportDataSource ordinanceRevAccountingData = new ReportDataSource() { Name = "dsRevAccounting", Value = revItems,  };
                    ReportDataSource ordinanceExpAccountingData = new ReportDataSource() { Name = "dsExpAccounting", Value = expItems };
                    ReportDataSource ordinanceStatusData = new ReportDataSource() { Name = "dsOrdinanceStatus" };

                    viewer.LocalReport.DataSources.Add(ordinanceData);
                    viewer.LocalReport.DataSources.Add(revExpTableBoolData);
                    viewer.LocalReport.DataSources.Add(ordinanceRevAccountingData);
                    viewer.LocalReport.DataSources.Add(ordinanceExpAccountingData);
                    viewer.LocalReport.DataSources.Add(ordinanceStatusData);

                    viewer.LocalReport.Refresh();

                    Warning[] warnings;
                    string[] streamIds;
                    string mimeType = string.Empty;
                    string encoding = string.Empty;
                    string extension = string.Empty;

                    byte[] bytes = viewer.LocalReport.Render("PDF", null, out mimeType, out encoding, out extension, out streamIds, out warnings);

                    Response.Clear();
                    Response.ClearContent();
                    Response.ClearHeaders();
                    Response.Buffer = true;
                    Response.ContentType = "application/pdf";
                    Response.AddHeader("content-disposition", $"attachment; filename=Ordinance_{ord.OrdinanceID}.pdf");
                    Response.BinaryWrite(bytes); // create the file
                    Context.ApplicationInstance.CompleteRequest();
                    break;
            }

            //ScriptManager.RegisterStartupScript(this, this.GetType(), "FadeOutOrdTable", "OrdTableFadeOut();", true);
            ScriptManager.RegisterStartupScript(this, this.GetType(), "CurrencyFormatting", "CurrencyFormatting();", true);
            ordView.Visible = true;
            ordTable.Visible = false;
        }
        protected void backBtn_Click(object sender, EventArgs e)
        {
            ordTable.Visible = true;
            //ScriptManager.RegisterStartupScript(this, this.GetType(), "FadeInOrdTable", "OrdTableFadeIn();", true);
            ordView.Visible = false;
        }
        protected void NewAccountingRow(string tableDesc)
        {
            List<Accounting> prvList = new List<Accounting>();
            List<Accounting> accountingList = new List<Accounting>();
            Accounting newAccountingItem = new Accounting();
            newAccountingItem.Amount = CurrencyToDecimal("-1");

            switch (tableDesc)
            {
                case "ordRevTable":
                    if (Session[tableDesc] != null)
                    {
                        for (int i = 0; i < rpRevenueTable.Items.Count; i++)
                        {
                            Accounting accountingItem = GetAccountingItem("revenue", i);
                            prvList.Add(accountingItem);
                        }
                        Session[tableDesc] = prvList;
                        accountingList = (List<Accounting>)Session[tableDesc];
                    }
                    accountingList.Add(newAccountingItem);
                    Session[tableDesc] = accountingList;
                    rpRevenueTable.DataSource = accountingList;
                    rpRevenueTable.DataBind();
                    break;
                case "ordExpTable":
                    if (Session[tableDesc] != null)
                    {
                        for (int i = 0; i < rpExpenditureTable.Items.Count; i++)
                        {
                            Accounting accountingItem = GetAccountingItem("expenditure", i);
                            prvList.Add(accountingItem);
                        }
                        Session[tableDesc] = prvList;
                        accountingList = (List<Accounting>)Session[tableDesc];
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
        protected void rpAccountingTable_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            string tableDesc = e.CommandArgument.ToString();
            List<Accounting> prvList = new List<Accounting>();
            List<Accounting> accountingList = new List<Accounting>();

            List<Accounting> removeAccs = new List<Accounting>();
            List<OrdinanceAccounting> removeOrdAccs = new List<OrdinanceAccounting>();

            switch (e.CommandName)
            {
                case "delete":

                    switch (tableDesc)
                    {
                        case "ordRevTable":
                            for (int i = 0; i < rpRevenueTable.Items.Count; i++)
                            {
                                Accounting accountingItem = new Accounting();
                                var revItem = rpRevenueTable.Items[i];
                                HiddenField revHdnIDField = (HiddenField)revItem.FindControl("hdnRevID");
                                TextBox revFundCode = (TextBox)revItem.FindControl("revenueFundCode");
                                TextBox revAgencyCode = (TextBox)revItem.FindControl("revenueAgencyCode");
                                TextBox revOrgCode = (TextBox)revItem.FindControl("revenueOrgCode");
                                TextBox revActivityCode = (TextBox)revItem.FindControl("revenueActivityCode");
                                TextBox revObjectCode = (TextBox)revItem.FindControl("revenueObjectCode");
                                TextBox revAmount = (TextBox)revItem.FindControl("revenueAmount");
                                accountingItem.AccountingID = Convert.ToInt32(revHdnIDField.Value);
                                accountingItem.AccountingDesc = tableDesc;
                                accountingItem.FundCode = revFundCode.Text;
                                accountingItem.DepartmentCode = revAgencyCode.Text;
                                accountingItem.UnitCode = revOrgCode.Text;
                                accountingItem.ActivityCode = revActivityCode.Text;
                                accountingItem.ObjectCode = revObjectCode.Text;
                                if (revAmount.Text.Length == 0)
                                {

                                    accountingItem.Amount = CurrencyToDecimal("-1");
                                }
                                else
                                {
                                    accountingItem.Amount = CurrencyToDecimal(revAmount.Text);
                                }
                                prvList.Add(accountingItem);
                            }
                            Session[tableDesc] = prvList;
                            HiddenField revHdnIndex = (HiddenField)e.Item.FindControl("hdnRevIndex");
                            if (Session[tableDesc] != null)
                            {
                                accountingList = (List<Accounting>)Session[tableDesc];
                            }

                            
                            HiddenField revHdnID = (HiddenField)e.Item.FindControl("hdnRevID");

                            if (Session["RemoveAccs"] != null)
                            {
                                removeAccs = Session["RemoveAccs"] as List<Accounting>;
                            }
                            if (Session["RemoveOrdAccs"] != null)
                            {
                                removeOrdAccs = Session["RemoveOrdAccs"] as List<OrdinanceAccounting>;
                            }
                            if (Convert.ToInt32(revHdnID.Value) > 0)
                            {
                                int accID = Convert.ToInt32(revHdnID.Value);
                                Accounting acc = Factory.Instance.GetByID<Accounting>(accID, "sp_GetLkAccountingByAccountingID", "AccountingID");
                                removeAccs.Add(acc);
                                OrdinanceAccounting ordAcc = Factory.Instance.GetByID<OrdinanceAccounting>(accID, "sp_GetOrdinanceAccountingByAccountingID", "AccountingID");
                                removeOrdAccs.Add(ordAcc);
                            }
                            Session["RemoveAccs"] = removeAccs;
                            Session["RemoveOrdAccs"] = removeOrdAccs;

                            accountingList.RemoveAt(Convert.ToInt32(revHdnIndex.Value));
                            Session[tableDesc] = accountingList;
                            rpRevenueTable.DataSource = accountingList;
                            rpRevenueTable.DataBind();
                            break;
                        case "ordExpTable":
                            for (int i = 0; i < rpExpenditureTable.Items.Count; i++)
                            {
                                Accounting accountingItem = new Accounting();
                                var expItem = rpExpenditureTable.Items[i];
                                HiddenField expHdnIDField = (HiddenField)expItem.FindControl("hdnExpID");
                                TextBox expFundCode = (TextBox)expItem.FindControl("expenditureFundCode");
                                TextBox expAgencyCode = (TextBox)expItem.FindControl("expenditureAgencyCode");
                                TextBox expOrgCode = (TextBox)expItem.FindControl("expenditureOrgCode");
                                TextBox expActivityCode = (TextBox)expItem.FindControl("expenditureActivityCode");
                                TextBox expObjectCode = (TextBox)expItem.FindControl("expenditureObjectCode");
                                TextBox expAmount = (TextBox)expItem.FindControl("expenditureAmount");
                                accountingItem.AccountingID = Convert.ToInt32(expHdnIDField.Value);
                                accountingItem.AccountingDesc = tableDesc;
                                accountingItem.FundCode = expFundCode.Text;
                                accountingItem.DepartmentCode = expAgencyCode.Text;
                                accountingItem.UnitCode = expOrgCode.Text;
                                accountingItem.ActivityCode = expActivityCode.Text;
                                accountingItem.ObjectCode = expObjectCode.Text;
                                if (expAmount.Text.Length == 0)
                                {

                                    accountingItem.Amount = CurrencyToDecimal("-1");
                                }
                                else
                                {
                                    accountingItem.Amount = CurrencyToDecimal(expAmount.Text);
                                }
                                prvList.Add(accountingItem);
                            }
                            Session[tableDesc] = prvList;
                            HiddenField expHdnIndex = (HiddenField)e.Item.FindControl("hdnExpIndex");
                            
                            if (Session[tableDesc] != null)
                            {
                                accountingList = (List<Accounting>)Session[tableDesc];
                            }

                            HiddenField expHdnID = (HiddenField)e.Item.FindControl("hdnExpID");

                            if (Session["RemoveAccs"] != null)
                            {
                                removeAccs = Session["RemoveAccs"] as List<Accounting>;
                            }
                            if (Session["RemoveOrdAccs"] != null)
                            {
                                removeOrdAccs = Session["RemoveOrdAccs"] as List<OrdinanceAccounting>;
                            }
                            if (Convert.ToInt32(expHdnID.Value) > 0)
                            {
                                int accID = Convert.ToInt32(expHdnID.Value);
                                Accounting acc = Factory.Instance.GetByID<Accounting>(accID, "sp_GetLkAccountingByAccountingID", "AccountingID");
                                removeAccs.Add(acc);
                                OrdinanceAccounting ordAcc = Factory.Instance.GetByID<OrdinanceAccounting>(accID, "sp_GetOrdinanceAccountingByAccountingID", "AccountingID");
                                removeOrdAccs.Add(ordAcc);
                            }
                            Session["RemoveAccs"] = removeAccs;
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
        protected Accounting GetAccountingItem(string tableDesc, int itemIndex)
        {
            Accounting accountingItem = new Accounting();
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
                    accountingItem.AccountingID = Convert.ToInt32(revHdnID.Value);
                    accountingItem.AccountingDesc = tableDesc;
                    accountingItem.FundCode = revFundCode.Text;
                    accountingItem.DepartmentCode = revAgencyCode.Text;
                    accountingItem.UnitCode = revOrgCode.Text;
                    accountingItem.ActivityCode = revActivityCode.Text;
                    accountingItem.ObjectCode = revObjectCode.Text;
                    accountingItem.LastUpdateBy = _user.Login;
                    accountingItem.LastUpdateDate = DateTime.Now;
                    accountingItem.EffectiveDate = DateTime.Now;
                    accountingItem.ExpirationDate = DateTime.MaxValue;
                    if (revAmount.Text.Length == 0)
                    {

                        accountingItem.Amount = CurrencyToDecimal("-1");
                    }
                    else
                    {
                        accountingItem.Amount = CurrencyToDecimal(revAmount.Text);
                    }
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
                    accountingItem.AccountingID = Convert.ToInt32(expHdnID.Value);
                    accountingItem.AccountingDesc = tableDesc;
                    accountingItem.FundCode = expFundCode.Text;
                    accountingItem.DepartmentCode = expAgencyCode.Text;
                    accountingItem.UnitCode = expOrgCode.Text;
                    accountingItem.ActivityCode = expActivityCode.Text;
                    accountingItem.ObjectCode = expObjectCode.Text;
                    accountingItem.LastUpdateBy = _user.Login;
                    accountingItem.LastUpdateDate = DateTime.Now;
                    accountingItem.EffectiveDate = DateTime.Now;
                    accountingItem.ExpirationDate = DateTime.MaxValue;
                    if (expAmount.Text.Length == 0)
                    {

                        accountingItem.Amount = CurrencyToDecimal("-1");
                    }
                    else
                    {
                        accountingItem.Amount = CurrencyToDecimal(expAmount.Text);
                    }
                    break;
            }
            return accountingItem;
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
                    Response.Clear();
                    Response.ClearHeaders();
                    Response.AddHeader("Content-Length", ordDocItem.DocumentData.Length.ToString());
                    Response.AddHeader("Content-type", MimeMapping.GetMimeMapping(ordDocItem.DocumentName));
                    Response.AddHeader("Content-Disposition", "attachment; filename=" + ordDocItem.DocumentName);
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
                    rpSupportingDocumentation.DataSource = ordDocList;
                    rpSupportingDocumentation.DataBind();
                    break;
            }
        }
        protected void SaveFactSheet_Click(object sender, EventArgs e)
        {            
            Ordinance ordinance = new Ordinance();

            ordinance.OrdinanceID = Convert.ToInt32(hdnOrdID.Value);
            ordinance.OrdinanceNumber = "TEST";
            ordinance.RequestID = 0;
            ordinance.RequestDepartment = requestDepartment.SelectedItem.Text;
            ordinance.RequestContact = requestContact.Text;
            ordinance.RequestPhone = $"{requestPhone.Text}{requestExt.Text}";
            ordinance.RequestEmail = hdnEmail.Value;
            ordinance.FirstReadDate = Convert.ToDateTime(firstReadDate.Text);
            ordinance.EmergencyPassage = epYes.Checked;
            ordinance.EmergencyPassageReason = epJustification.Text ?? string.Empty;
            ordinance.OrdinanceFiscalImpact = CurrencyToDecimal(fiscalImpact.Text);
            ordinance.OrdinanceTitle = suggestedTitle.Text;
            ordinance.ContractVendorID = 0;
            ordinance.ContractVendorName = vendorName.Text;
            ordinance.ContractVendorNumber = vendorNumber.Text;
            ordinance.ContractStartDate = contractStartDate.Text;
            ordinance.ContractEndDate = contractEndDate.Text;
            ordinance.ContractTerm = contractTerm.Value;
            ordinance.ContractAmount = CurrencyToDecimal(contractAmount.Text);
            ordinance.ScopeChange = scYes.Checked;
            ordinance.ChangeOrderNumber = changeOrderNumber.Text ?? string.Empty;
            if (scYes.Checked)
            {
                ordinance.AdditionalAmount = CurrencyToDecimal(additionalAmount.Text);
            }
            else
            {
                ordinance.AdditionalAmount = CurrencyToDecimal("-1");
            }
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

            int retVal = Factory.Instance.Update(ordinance, "sp_UpdateOrdinance", 1);

            OrdinanceStatus ordStatus = new OrdinanceStatus();
            ordStatus.OrdinanceStatusID = Convert.ToInt32(hdnOrdStatusID.Value);
            ordStatus.OrdinanceID = Convert.ToInt32(hdnOrdID.Value);
            ordStatus.StatusID = Convert.ToInt32(ddStatus.SelectedValue);
            ordStatus.Signature = string.Empty;
            ordStatus.LastUpdateBy = _user.Login;
            ordStatus.LastUpdateDate = DateTime.Now;
            ordStatus.EffectiveDate = Convert.ToDateTime(hdnEffectiveDate.Value);
            ordStatus.ExpirationDate = DateTime.MaxValue;
            ordinance.StatusDescription = ddStatus.SelectedItem.ToString();
            int statusVal = Factory.Instance.Update(ordStatus, "sp_UpdateOrdinance_Status", 1);



            int addDocsVal = new int();
            int addUploadedDocsVal = new int();
            List<OrdinanceDocument> ordDocs = Session["addOrdDocs"] as List<OrdinanceDocument>;
            if (Session["addOrdDocs"] != null)
            {
                foreach (OrdinanceDocument ordDoc in ordDocs)
                {
                    ordDoc.OrdinanceID = Convert.ToInt32(hdnOrdID.Value);
                    addUploadedDocsVal = Factory.Instance.Insert(ordDoc, "sp_InsertOrdinance_Document");
                    //int ret = 1;
                    if (addUploadedDocsVal < 1)
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
                    addDocsVal = Factory.Instance.Insert(ordDoc, "sp_InsertOrdinance_Document");
                    //int ret = 1;
                    if (addDocsVal < 1)
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
            if (Session["RemoveDocs"] != null)
            {
                removeDocs = Session["RemoveDocs"] as List<OrdinanceDocument>;
            }
            if (removeDocs.Count > 0)
            {
                foreach (OrdinanceDocument item in removeDocs)
                {
                    removeDocVal = Factory.Instance.Expire(item, "sp_UpdateOrdinance_Document");
                    if (removeDocVal < 1)
                    {
                        break;
                    }
                }
            }
            else
            {
                removeDocVal = 1;
            }








            int removeAccsVal = new int();
            int removeOrdAccsVal = new int();
            List<Accounting> removeAccs = new List<Accounting>();
            List<OrdinanceAccounting> removeOrdAccs = new List<OrdinanceAccounting>();
            if (Session["RemoveAccs"] != null)
            {
                removeAccs = Session["RemoveAccs"] as List<Accounting>;
            }
            if (Session["RemoveOrdAccs"] != null)
            {
                removeOrdAccs = Session["RemoveOrdAccs"] as List<OrdinanceAccounting>;
            }
            if (removeAccs.Count > 0)
            {
                foreach (Accounting item in removeAccs)
                {
                    item.LastUpdateBy = _user.Login;
                    item.LastUpdateDate = DateTime.Now;
                    item.EffectiveDate = DateTime.Now;
                    removeAccsVal = Factory.Instance.Expire(item, "sp_UpdatelkAccounting");

                    if (removeAccsVal < 1)
                    {
                        break;
                    }
                }
            }
            else
            {
                removeAccsVal = 1;
            }

            if (removeOrdAccs.Count > 0)
            {
                foreach (OrdinanceAccounting item in removeOrdAccs)
                {
                    item.LastUpdateBy = _user.Login;
                    item.LastUpdateDate = DateTime.Now;
                    item.EffectiveDate = DateTime.Now;
                    removeOrdAccsVal = Factory.Instance.Expire(item, "sp_UpdateOrdinance_Accounting");
                    if (removeOrdAccsVal < 1)
                    {
                        break;
                    }
                }
            }
            else
            {
                removeOrdAccsVal = 1;
            }


            int updateRevAccsVal = new int();
            int updateExpAccsVal = new int();
            if (removeOrdAccsVal > 0)
            {
                if (rpRevenueTable.Items.Count > 0)
                {
                    for (int i = 0; i < rpRevenueTable.Items.Count; i++)
                    {
                        Accounting accountingItem = GetAccountingItem("revenue", i);
                        if (accountingItem.AccountingID > 0)
                        {
                            updateRevAccsVal = Factory.Instance.Update(accountingItem, "sp_UpdatelkAccounting");
                        }
                        else
                        {
                            int ret = Factory.Instance.Insert(accountingItem, "sp_InsertlkAccounting");
                            if (ret > 0)
                            {
                                OrdinanceAccounting oaItem = new OrdinanceAccounting();
                                oaItem.OrdinanceID = Convert.ToInt32(hdnOrdID.Value);
                                oaItem.AccountingID = ret;
                                oaItem.LastUpdateBy = _user.Login;
                                oaItem.LastUpdateDate = DateTime.Now;
                                oaItem.EffectiveDate = DateTime.Now;
                                oaItem.ExpirationDate = DateTime.MaxValue;
                                updateRevAccsVal = Factory.Instance.Insert(oaItem, "sp_InsertOrdinance_Accounting");
                            }
                            else
                            {
                                updateRevAccsVal = 0;
                                break;
                            }
                        }
                        if (updateRevAccsVal < 1)
                        {
                            break;
                        }
                    }
                }
                else
                {
                    updateRevAccsVal = 1;
                }

                if (rpExpenditureTable.Items.Count > 0)
                {
                    for (int i = 0; i < rpExpenditureTable.Items.Count; i++)
                    {
                        Accounting accountingItem = GetAccountingItem("expenditure", i);
                        if (accountingItem.AccountingID > 0)
                        {
                            updateExpAccsVal = Factory.Instance.Update(accountingItem, "sp_UpdatelkAccounting");
                        }
                        else
                        {
                            int ret = Factory.Instance.Insert(accountingItem, "sp_InsertlkAccounting");
                            if (ret > 0)
                            {
                                OrdinanceAccounting oaItem = new OrdinanceAccounting();
                                oaItem.OrdinanceID = Convert.ToInt32(hdnOrdID.Value);
                                oaItem.AccountingID = ret;
                                oaItem.LastUpdateBy = _user.Login;
                                oaItem.LastUpdateDate = DateTime.Now;
                                oaItem.EffectiveDate = DateTime.Now;
                                oaItem.ExpirationDate = DateTime.MaxValue;
                                updateRevAccsVal = Factory.Instance.Insert(oaItem, "sp_InsertOrdinance_Accounting");
                            }
                            else
                            {
                                updateExpAccsVal = 0;
                                break;
                            }
                        }
                        if (updateExpAccsVal < 1)
                        {
                            break;
                        }
                    }
                }
                else
                {
                    updateExpAccsVal = 1;
                }
            }



            Email.Instance.AddEmailAddress(emailList, _user.Email);
            Email.Instance.AddEmailAddress(emailList, ordinance.RequestEmail);
            string formType = "Ordinance Fact Sheet";

            Email newEmail = new Email();

            newEmail.EmailSubject = "Fact Sheet UPDATED";
            newEmail.EmailTitle = "Fact Sheet UPDATED";
            newEmail.EmailText = $"An {formType} has been updated <br/><br/>Ordinance: {ordinance.OrdinanceNumber} {hdnOrdID.Value}<br/>Date: {DateTime.Now}<br/>Department: {requestDepartment.SelectedItem.Text}<br/>Contact: {requestContact.Text}<br/>Phone: {requestPhone.Text} {requestExt.Text}<br/><br/>Status: {ordinance.StatusDescription}";

            List<int> submitVals = new List<int>( new int[] {
                retVal,
                statusVal,
                removeDocVal,
                addDocsVal,
                addUploadedDocsVal,
                removeAccsVal,
                removeOrdAccsVal,
                updateRevAccsVal,
                updateExpAccsVal
            });

            if (submitVals.All(i => i > 0))
            {
                Session["SubmitStatus"] = "success";
                Session["ToastColor"] = "text-bg-success";
                Session["ToastMessage"] = "Form Saved!";
                Email.Instance.SendEmail(newEmail, emailList);
                Response.Redirect("./Ordinances");
            }
            else
            {
                Session["SubmitStatus"] = "error";
                Session["ToastColor"] = "text-bg-danger";
                Session["ToastMessage"] = "Something went wrong while saving!";
            }
        }
        protected void GetUploadedImages()
        {
            if (Session["addOrdDocs"] != null && Session["ordDocs"] != null)
            {
                List<OrdinanceDocument> originalOrdDocList = Session["ordDocs"] as List<OrdinanceDocument>;
                List<OrdinanceDocument> ordDocList = Session["addOrdDocs"] as List<OrdinanceDocument>;
                originalOrdDocList.AddRange(ordDocList);
                rpSupportingDocumentation.DataSource = originalOrdDocList;
                rpSupportingDocumentation.DataBind();
            }
        }
        protected void UploadImageBtn_Click(object sender, EventArgs e)
        {
            List<OrdinanceDocument> originalOrdDocList = (Session["ordDocs"] != null) ? Session["ordDocs"] as List<OrdinanceDocument> : new List<OrdinanceDocument>();
            List<OrdinanceDocument> ordDocList = (Session["addOrdDocs"] != null) ? Session["addOrdDocs"] as List<OrdinanceDocument> : new List<OrdinanceDocument>();

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
            rpSupportingDocumentation.DataSource = originalOrdDocList;
            rpSupportingDocumentation.DataBind();
        }
        protected void SortBtn_Click(object sender, EventArgs e)
        {
            SetPagination(rpOrdinanceTable, 10);
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
                sortDate,
                sortTitle,
                sortDepartment,
                sortContact,
                sortStatus
            };
            foreach (LinkButton item in sortButtonsList)
            {
                item.Text = $"<strong>{item.Attributes["data-text"]}<span runat='server' class='float-end lh-1p5'></span></strong>";
            }

            button.Text = $"<strong>{commandText}<span runat='server' class='float-end lh-1p5 fas fa-arrow-{sortRet["arrow"]}'></span></strong>";
        }
        protected void Filter_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetPagination(rpOrdinanceTable, 10);
            List<Ordinance> ord_list = new List<Ordinance>();
            List<Ordinance> noFilterOrdList = new List<Ordinance>();
            
            DropDownList dropDown = (DropDownList)sender;
            string commandName = dropDown.Attributes["data-command"];
            string commandArgument = dropDown.SelectedItem.ToString();
            List<Ordinance> filteredList = new List<Ordinance>();
            userInfo = (UserInfo)Session["UserInformation"];


            switch (commandName)
            {
                case "department":
                    if (filterDepartment.SelectedValue != "")
                    {
                        switch (filterStatus.SelectedValue.Equals(""))
                        {
                            case true:
                                filteredList = Factory.Instance.GetAll<Ordinance>("sp_GetOrdinanceByEffective");
                                if (filteredList.Count > 0)
                                {
                                    foreach (Ordinance ord in filteredList)
                                    {
                                        OrdinanceStatus ordStatus = Factory.Instance.GetByID<OrdinanceStatus>(ord.OrdinanceID, "sp_GetOrdinanceStatusesByOrdinanceID", "OrdinanceID");
                                        ord.StatusDescription = ordStatus.StatusDescription;
                                    }
                                }
                                break;

                            case false:
                                filteredList = Factory.Instance.GetAllLookup<Ordinance>(Convert.ToInt32(filterStatus.SelectedValue), "sp_GetOrdinanceByStatusID", "StatusID");
                                if (filteredList.Count > 0)
                                {
                                    foreach (Ordinance ord in filteredList)
                                    {
                                        OrdinanceStatus ordStatus = Factory.Instance.GetByID<OrdinanceStatus>(ord.OrdinanceID, "sp_GetOrdinanceStatusesByOrdinanceID", "OrdinanceID");
                                        ord.StatusDescription = ordStatus.StatusDescription;
                                    }
                                }
                                break;
                        }
                        filteredList = FilterList(filteredList, commandName, commandArgument);
                    }
                    else
                    {
                        switch (filterStatus.SelectedValue.Equals(""))
                        {
                            case true:
                                filteredList = Factory.Instance.GetAll<Ordinance>("sp_GetOrdinanceByEffective");
                                if (filteredList.Count > 0)
                                {
                                    foreach (Ordinance ord in filteredList)
                                    {
                                        OrdinanceStatus ordStatus = Factory.Instance.GetByID<OrdinanceStatus>(ord.OrdinanceID, "sp_GetOrdinanceStatusesByOrdinanceID", "OrdinanceID");
                                        ord.StatusDescription = ordStatus.StatusDescription;
                                    }
                                }
                                BindDataRepeaterPagination("no", filteredList);
                                break;

                            case false:
                                filteredList = Factory.Instance.GetAllLookup<Ordinance>(Convert.ToInt32(filterStatus.SelectedValue), "sp_GetOrdinanceByStatusID", "StatusID");
                                if (filteredList.Count > 0)
                                {
                                    foreach (Ordinance ord in filteredList)
                                    {
                                        OrdinanceStatus ordStatus = Factory.Instance.GetByID<OrdinanceStatus>(ord.OrdinanceID, "sp_GetOrdinanceStatusesByOrdinanceID", "OrdinanceID");
                                        ord.StatusDescription = ordStatus.StatusDescription;
                                    }
                                }
                                BindDataRepeaterPagination("no", filteredList);
                                break;
                        }
                    }                    
                    break;

                case "status":
                    if (filterStatus.SelectedValue != "")
                    {
                        filteredList = Factory.Instance.GetAllLookup<Ordinance>(Convert.ToInt32(dropDown.SelectedValue), "sp_GetOrdinanceByStatusID", "StatusID");

                        if (filteredList.Count > 0)
                        {
                            foreach (Ordinance ord in filteredList)
                            {
                                OrdinanceStatus ordStatus = Factory.Instance.GetByID<OrdinanceStatus>(ord.OrdinanceID, "sp_GetOrdinanceStatusesByOrdinanceID", "OrdinanceID");
                                ord.StatusDescription = ordStatus.StatusDescription;
                            }
                        }

                        switch (filterDepartment.SelectedValue.Equals(""))
                        {
                            case true:
                                switch (userInfo.IsAdmin && !userInfo.UserView)
                                {
                                    case true:
                                        BindDataRepeaterPagination("no", filteredList);
                                        break;

                                    case false:
                                        filteredList = FilterList(filteredList, "department", userInfo.UserDepartmentName);
                                        break;
                                }
                                break;

                            case false:
                                filteredList = FilterList(filteredList, "department", filterDepartment.SelectedItem.ToString());
                                break;
                        }
                    }
                    else
                    {
                        filteredList = Factory.Instance.GetAll<Ordinance>("sp_GetOrdinanceByEffective");
                        if (filteredList.Count > 0)
                        {
                            foreach (Ordinance ord in filteredList)
                            {
                                OrdinanceStatus ordStatus = Factory.Instance.GetByID<OrdinanceStatus>(ord.OrdinanceID, "sp_GetOrdinanceStatusesByOrdinanceID", "OrdinanceID");
                                ord.StatusDescription = ordStatus.StatusDescription;
                            }
                        }
                        switch (filterDepartment.SelectedValue.Equals(""))
                        {
                            case true:
                                switch (userInfo.IsAdmin && !userInfo.UserView)
                                {
                                    case true:
                                        BindDataRepeaterPagination("no", filteredList);
                                        break;

                                    case false:
                                        filteredList = FilterList(filteredList, "department", userInfo.UserDepartmentName);
                                        break;
                                }
                                break;

                            case false:
                                filteredList = FilterList(filteredList, "department", filterDepartment.SelectedItem.ToString());
                                break;
                        }

                    }
                    break;
            }
            Dictionary<string, object> sortRet = new Dictionary<string, object>();

            sortRet = SortButtonClick(filteredList, Session["curCmd"].ToString(), Session["curDir"].ToString());


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
        }
    }
}