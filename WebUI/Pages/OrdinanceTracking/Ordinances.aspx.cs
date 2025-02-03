using DataLibrary;
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

namespace WebUI
{
    public partial class Ordinances : System.Web.UI.Page
    {
        private ADUser _user = new ADUser();
        private string emailList = "NewFactSheetEmailList";
        public string toastColor;
        public string toastMessage;

        protected void Page_Load(object sender, EventArgs e)
        {
            Page.Form.Attributes.Add("enctype", "multipart/form-data");
            _user = Session["CurrentUser"] as ADUser;
            if (!Page.IsPostBack && !Response.IsRequestBeingRedirected)
            {
                Session.Remove("ordRevTable");
                Session.Remove("ordExpTable");
                GetAllDepartments();
                GetAllPurchaseMethods();
                SetStartupActives();
                SetPagination(rpOrdinanceTable, 13);
                GetStartupData();
            }
            foreach (RepeaterItem item in rpOrdinanceTable.Items)
            {
                LinkButton editButton = item.FindControl("editOrd") as LinkButton;
                LinkButton viewButton = item.FindControl("viewOrd") as LinkButton;
                ScriptManager.GetCurrent(Page).RegisterAsyncPostBackControl(editButton);
                ScriptManager.GetCurrent(Page).RegisterAsyncPostBackControl(viewButton);
            }
            
            ScriptManager.GetCurrent(Page).RegisterPostBackControl(SaveFactSheet);
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
            int ordID = Convert.ToInt32(hdnOrdID.Value);
            Ordinance ord = Factory.Instance.GetByID<Ordinance>(ordID, "sp_GetOrdinanceByOrdinanceID", "OrdinanceID");
            int retVal = Factory.Instance.Expire<Ordinance>(ord, "sp_UpdateOrdinance");
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


            requestDepartment.SelectedValue = Utility.Instance.DepartmentsList()[ord.RequestDepartment];
            firstReadDate.Text = ord.FirstReadDate.ToString("yyyy-MM-dd");
            requestContact.Text = ord.RequestContact;
            requestPhone.Text = ord.RequestPhone.SubstringUpToFirst('x');
            requestExt.Text = ord.RequestPhone.Substring(14);

            switch (ord.EmergencyPassage)
            {
                case true:
                    epYes.Checked = true;
                    epJustificationGroup.Visible = true;
                    break;
                case false:
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
                    scopeChangeOptions.Visible = true;
                    break;
                case false:
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

            switch (e.CommandName)
            {
                case "view":
                    ordView.Attributes["readonly"] = "true";
                    requiredFieldDescriptor.Visible = false;
                    prevOrdinanceNums.Attributes["placeholder"] = string.Empty;
                    newRevenueRowDiv.Visible = false;
                    newExpenditureRowDiv.Visible = false;
                    supportingDocumentation.Visible = false;
                    submitSection.Visible = false;
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
                    foreach (RepeaterItem item in rpSupportingDocumentation.Items)
                    {
                        LinkButton deleteFile = item.FindControl("deleteFile") as LinkButton;
                        deleteFile.Visible = false;
                    }

                    otherException.Enabled = true;
                    changeOrderNumber.Enabled = true;
                    additionalAmount.Enabled = true;
                    break;
                case "edit":
                    ordView.Attributes["readonly"] = "false";
                    requiredFieldDescriptor.Visible = true;
                    newRevenueRowDiv.Visible = true;
                    newExpenditureRowDiv.Visible = true;
                    supportingDocumentationDiv.Visible = true;
                    supportingDocumentation.Visible = true;
                    Session.Remove("RemoveDocs");
                    submitSection.Visible = true;
                    if (rpRevenueTable.Items.Count > 0)
                    {
                        foreach (RepeaterItem item in rpRevenueTable.Items)
                        {
                            HtmlGenericControl removeRevRowDiv = item.FindControl("removeRevRowDiv") as HtmlGenericControl;
                            Button removeRevRow = item.FindControl("removeRevenueRow") as Button;
                            removeRevRowDiv.Visible = true;
                            ScriptManager.GetCurrent(Page).RegisterAsyncPostBackControl(removeRevRow);
                        }
                    }
                    if (rpExpenditureTable.Items.Count > 0)
                    {
                        foreach (RepeaterItem item in rpExpenditureTable.Items)
                        {
                            HtmlGenericControl removeExpRowDiv = item.FindControl("removeExpRowDiv") as HtmlGenericControl;
                            Button removeExpRow = item.FindControl("removeExpenditureRow") as Button;
                            removeExpRowDiv.Visible = true;
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
                                TextBox revFundCode = (TextBox)revItem.FindControl("revenueFundCode");
                                TextBox revAgencyCode = (TextBox)revItem.FindControl("revenueAgencyCode");
                                TextBox revOrgCode = (TextBox)revItem.FindControl("revenueOrgCode");
                                TextBox revActivityCode = (TextBox)revItem.FindControl("revenueActivityCode");
                                TextBox revObjectCode = (TextBox)revItem.FindControl("revenueObjectCode");
                                TextBox revAmount = (TextBox)revItem.FindControl("revenueAmount");
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
                            HiddenField revHdnID = (HiddenField)e.Item.FindControl("hdnRevID");
                            if (Session[tableDesc] != null)
                            {
                                accountingList = (List<Accounting>)Session[tableDesc];
                            }

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
                            if (Convert.ToInt32(revHdnID.Value) > 0)
                            {
                                removeAccs.Add(accountingList[Convert.ToInt32(revHdnIndex.Value)]);
                                OrdinanceAccounting ordAcc = Factory.Instance.GetByID<OrdinanceAccounting>(accountingList[Convert.ToInt32(revHdnIndex.Value)].AccountingID, "sp_GetOrdinanceAccountingByAccountingID", "AccountingID");
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
                                TextBox expFundCode = (TextBox)expItem.FindControl("expenditureFundCode");
                                TextBox expAgencyCode = (TextBox)expItem.FindControl("expenditureAgencyCode");
                                TextBox expOrgCode = (TextBox)expItem.FindControl("expenditureOrgCode");
                                TextBox expActivityCode = (TextBox)expItem.FindControl("expenditureActivityCode");
                                TextBox expObjectCode = (TextBox)expItem.FindControl("expenditureObjectCode");
                                TextBox expAmount = (TextBox)expItem.FindControl("expenditureAmount");
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
                            HiddenField expHdnID = (HiddenField)e.Item.FindControl("hdnExpID");
                            if (Session[tableDesc] != null)
                            {
                                accountingList = (List<Accounting>)Session[tableDesc];
                            }
                            accountingList.RemoveAt(Convert.ToInt32(expHdnID.Value));
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
                    TextBox revFundCode = (TextBox)revItem.FindControl("revenueFundCode");
                    TextBox revAgencyCode = (TextBox)revItem.FindControl("revenueAgencyCode");
                    TextBox revOrgCode = (TextBox)revItem.FindControl("revenueOrgCode");
                    TextBox revActivityCode = (TextBox)revItem.FindControl("revenueActivityCode");
                    TextBox revObjectCode = (TextBox)revItem.FindControl("revenueObjectCode");
                    TextBox revAmount = (TextBox)revItem.FindControl("revenueAmount");
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
                    TextBox expFundCode = (TextBox)expItem.FindControl("expenditureFundCode");
                    TextBox expAgencyCode = (TextBox)expItem.FindControl("expenditureAgencyCode");
                    TextBox expOrgCode = (TextBox)expItem.FindControl("expenditureOrgCode");
                    TextBox expActivityCode = (TextBox)expItem.FindControl("expenditureActivityCode");
                    TextBox expObjectCode = (TextBox)expItem.FindControl("expenditureObjectCode");
                    TextBox expAmount = (TextBox)expItem.FindControl("expenditureAmount");
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
            ordinance.EffectiveDate = DateTime.Now;
            ordinance.ExpirationDate = DateTime.MaxValue;

            int retVal = Factory.Instance.Update(ordinance, "sp_UpdateOrdinance");





            int addDocsVal = new int();
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

                    if (removeAccsVal > 1)
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
                    if (removeOrdAccsVal > 1)
                    {
                        break;
                    }
                }
            }
            else
            {
                removeOrdAccsVal = 1;
            }





















            Email.Instance.AddEmailAddress(emailList, _user.Email);
            string formType = "Ordinance Fact Sheet";

            Email newEmail = new Email();

            newEmail.EmailSubject = "Fact Sheet UPDATED";
            newEmail.EmailTitle = "Fact Sheet UPDATED";
            newEmail.EmailText = $"An {formType} has been updated <br/><br/>Ordinance: {ordinance.OrdinanceNumber}{hdnOrdID.Value}<br/>Date: {DateTime.Now}<br/>Department: {requestDepartment.SelectedItem.Text}<br/>Contact: {requestContact.Text}<br/>Phone: {requestPhone.Text} {requestExt.Text}";

            if (retVal > 0 && removeDocVal > 0 && addDocsVal > 0)
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











        [WebMethod]
        public void OrdVisibility(string fadeOut)
        {
            switch (fadeOut)
            {
                case "table":
                    ordTable.Visible = false;
                    break;
                case "ord":
                    ordView.Visible = false;
                    break;
            }
        }
    }
}