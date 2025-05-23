﻿using DataLibrary;
using DataLibrary.OrdinanceTracking;
using ISD.ActiveDirectory;
using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using static DataLibrary.Utility;

namespace WebUI
{
    public partial class NewFactSheet : System.Web.UI.Page
    {
        private ADUser _user = new ADUser();
        public UserInfo userInfo = new UserInfo();
        private readonly string emailList = HttpContext.Current.IsDebuggingEnabled ? "NewFactSheetEmailListTEST" : "NewFactSheetEmailList";

        protected void Page_Load(object sender, EventArgs e)
        {
            _user = Session["CurrentUser"] as ADUser;
            userInfo = Session["UserInformation"] as UserInfo;
            if (!Page.IsPostBack && !Response.IsRequestBeingRedirected)
            {
                Session.Remove("revenue");
                Session.Remove("expenditure");
                Session.Remove("ordDocs");
                GetAllDepartments();
                GetAllPurchaseMethods();
                SetStartupActives();
            }

            if (!Page.IsPostBack && Request.QueryString["id"] != null)
            {
                GetCopyOrdinance(Convert.ToInt32(Request.QueryString["id"].ToString()));
            }

            GetUploadedDocs();
        }
        protected void SetStartupActives()
        {
            epJustificationGroup.Visible = false;
            changeOrderNumber.Enabled = false;
            additionalAmount.Enabled = false;
            otherException.Enabled = false;

            requestDepartment.SelectedValue = userInfo.UserDepartment.DepartmentCode.ToString();
            requestContact.Text = $"{_user.FirstName} {_user.LastName}";
            requestEmail.Text = _user.Email.ToLower();
            requestPhone.Text = _user.Telephone;
            requestExt.Text = _user.IPPhone;

            GetAllDivisions(requestDepartment.SelectedValue);

            if (!requestDepartment.SelectedValue.IsNullOrWhiteSpace())
            {
                requestDivision.Enabled = true;
                requestDivision.SelectedValue = userInfo.UserDivision.DivisionCode.ToString();
            }
            else
            {
                requestDivision.Enabled = false;
                requestDivision.Items.Add(new ListItem() { Text = "Select Division...", Value = "" });
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
            }
        }
        protected void GetAllDivisions(string deptCode)
        {
            requestDivision.Items.Clear();
            requestDivision.Items.Add(new ListItem() { Text = "Select Division...", Value = "" });
            List<Division> divisionList = GetDivisionsByDept(Convert.ToInt32(deptCode));
            foreach (Division item in divisionList)
            {
                ListItem newItem = new ListItem(item.DivisionName, item.DivisionCode.ToString());
                requestDivision.Items.Add(newItem);
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
                    changeOrderNumber.Attributes.Add("placeholder", "0123456789");
                    additionalAmount.Attributes.Add("required", "true");
                    additionalAmount.Attributes.Add("placeholder", "$0.00");
                    break;

                case false:
                    changeOrderNumber.Enabled = false;
                    additionalAmount.Enabled = false;
                    changeOrderNumber.Attributes.Remove("required");
                    changeOrderNumber.Attributes.Remove("placeholder");
                    additionalAmount.Attributes.Remove("required");
                    additionalAmount.Attributes.Remove("placeholder");
                    break;
            }
        }
        protected void NewAccountingRow(string tableDesc)
        {
            List<OrdinanceAccounting> prvList = new List<OrdinanceAccounting>();
            List<OrdinanceAccounting> accountingList = new List<OrdinanceAccounting>();
            OrdinanceAccounting newAccountingItem = new OrdinanceAccounting();
            newAccountingItem.Amount = CurrencyToDecimal("-1");

            switch (tableDesc)
            {
                case "revenue":
                    if (Session["ordRevTable"] != null)
                    {
                        for (int i = 0; i < rpRevenueTable.Items.Count; i++)
                        {
                            OrdinanceAccounting accountingItem = GetAccountingItem("revenue", i);
                            prvList.Add(accountingItem);
                        }
                        Session[tableDesc] = prvList;
                        accountingList = prvList;
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
                        accountingList = prvList;
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
                        case "revenue":
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
                                accountingItem.Amount = CurrencyToDecimal(revAmount.Text);
                                prvList.Add(accountingItem);
                            }
                            Session[tableDesc] = prvList;
                            HiddenField revHdnID = (HiddenField)e.Item.FindControl("hdnRevID");
                            if (Session[tableDesc] != null)
                            {
                                accountingList = (List<Accounting>)Session[tableDesc];
                            }
                            accountingList.RemoveAt(Convert.ToInt32(revHdnID.Value));
                            Session[tableDesc] = accountingList;
                            rpRevenueTable.DataSource = accountingList;
                            rpRevenueTable.DataBind();
                            break;
                        case "expenditure":
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
                                accountingItem.Amount = CurrencyToDecimal(expAmount.Text);
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
        protected OrdinanceAccounting GetAccountingItem(string tableDesc, int itemIndex)
        {
            OrdinanceAccounting accountingItem = new OrdinanceAccounting();
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
                    TextBox expFundCode = (TextBox)expItem.FindControl("expenditureFundCode");
                    TextBox expAgencyCode = (TextBox)expItem.FindControl("expenditureAgencyCode");
                    TextBox expOrgCode = (TextBox)expItem.FindControl("expenditureOrgCode");
                    TextBox expActivityCode = (TextBox)expItem.FindControl("expenditureActivityCode");
                    TextBox expObjectCode = (TextBox)expItem.FindControl("expenditureObjectCode");
                    TextBox expAmount = (TextBox)expItem.FindControl("expenditureAmount");
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
        protected void SubmitForm_Click(object sender, EventArgs e)
        {
            Ordinance ordinance = new Ordinance();

            string xString = !requestExt.Text.Contains("x") ? "x" : string.Empty;

            ordinance.OrdinanceNumber = string.Empty;
            ordinance.RequestDepartment = requestDepartment.SelectedItem.Text;
            ordinance.RequestDivision = requestDivision.SelectedItem.Text;
            ordinance.RequestContact = requestContact.Text;
            ordinance.RequestPhone = $"{requestPhone.Text}{xString}{requestExt.Text}";
            ordinance.RequestEmail = requestEmail.Text;
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
            ordinance.EffectiveDate = DateTime.Now;
            ordinance.ExpirationDate = DateTime.MaxValue;

            int retVal = Factory.Instance.Insert(ordinance, "sp_InsertOrdinance", Skips("ordInsert"));
            //int retVal = 1;
            if (retVal > 0)
            {
                bool revExpTables = false;
                bool documentation = false;
                bool uploadedDocumentation = false;
                List<OrdinanceDocument> ordDocs = new List<OrdinanceDocument>();
                bool finishSubmit = true;
                if (rpRevenueTable.Items.Count > 0 || rpExpenditureTable.Items.Count > 0)
                {
                    revExpTables = true;
                }
                if (supportingDocumentation.HasFiles)
                {
                    documentation = true;
                }
                if (Session["ordDocs"] != null)
                {
                    uploadedDocumentation = true;
                    ordDocs = Session["ordDocs"] as List<OrdinanceDocument>;
                }

                switch (revExpTables)
                {
                    case true:
                        bool revSubmit = false;
                        bool expSubmit = false;
                        if (rpRevenueTable.Items.Count > 0)
                        {
                            for (int i = 0; i < rpRevenueTable.Items.Count; i++)
                            {
                                OrdinanceAccounting accountingItem = GetAccountingItem("revenue", i);
                                accountingItem.OrdinanceID = retVal;
                                int ret = Factory.Instance.Insert(accountingItem, "sp_InsertOrdinance_Accounting", Skips("accountingInsert"));
                                //int ret = 1;
                                if (ret > 0)
                                {
                                    revSubmit = true;
                                }
                                else
                                {
                                    revSubmit = false;
                                }
                            }
                        }
                        else
                        {
                            revSubmit = true;
                        }

                        if (rpExpenditureTable.Items.Count > 0)
                        {
                            for (int i = 0; i < rpExpenditureTable.Items.Count; i++)
                            {
                                OrdinanceAccounting accountingItem = GetAccountingItem("expenditure", i);
                                accountingItem.OrdinanceID = retVal;
                                int ret = Factory.Instance.Insert(accountingItem, "sp_InsertOrdinance_Accounting", Skips("accountingInsert"));
                                //int ret = 1;
                                if (ret > 0)
                                {
                                    expSubmit = true;
                                }
                                else
                                {
                                    expSubmit = false;
                                }
                            }
                        }
                        else
                        {
                            expSubmit = true;
                        }

                        if (!revSubmit || !expSubmit)
                        {
                            finishSubmit = false;
                        }
                        break;
                }

                switch (uploadedDocumentation)
                {
                    case true:
                        foreach (OrdinanceDocument ordDoc in ordDocs)
                        {
                            ordDoc.OrdinanceID = retVal;
                            int ret = Factory.Instance.Insert(ordDoc, "sp_InsertOrdinance_Document", Skips("ordDocumentInsert"));
                            //int ret = 1;
                            if (ret < 1)
                            {
                                finishSubmit = false;
                            }
                        }

                        break;
                }

                switch (documentation)
                {
                    case true:

                        for (int i = 0; i < supportingDocumentation.PostedFiles.Count; i++)
                        {
                            OrdinanceDocument ordDoc = new OrdinanceDocument();
                            ordDoc.OrdinanceID = retVal;
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
                            int ret = Factory.Instance.Insert(ordDoc, "sp_InsertOrdinance_Document", Skips("ordDocumentInsert"));
                            //int ret = 1;
                            if (ret < 1)
                            {
                                finishSubmit = false;
                            }
                        }

                        break;
                }

                OrdinanceStatus ordStatus = new OrdinanceStatus();
                ordStatus.OrdinanceID = retVal;
                ordStatus.StatusID = 1;
                ordStatus.LastUpdateBy = _user.Login;
                ordStatus.LastUpdateDate = DateTime.Now;
                ordStatus.EffectiveDate = DateTime.Now;
                ordStatus.ExpirationDate = DateTime.MaxValue;
                int statusRet = Factory.Instance.Insert(ordStatus, "sp_InsertOrdinance_Status", Skips("ordStatusInsert"));

                if (statusRet < 1)
                {
                    finishSubmit = false;
                }

                SignatureRequest signatureRequest = new SignatureRequest()
                {
                    OrdinanceID = retVal,
                    FundsCheckBy = string.Empty,
                    DirectorSupervisor = string.Empty,
                    CityPurchasingAgent = string.Empty,
                    OBMDirector = string.Empty,
                    Mayor = string.Empty,
                    LastUpdateBy = $"{_user.FirstName} {_user.LastName}",
                    LastUpdateDate = DateTime.Now,
                    EffectiveDate = DateTime.Now,
                    ExpirationDate = DateTime.MaxValue
                };

                int signatureRequestRet = Factory.Instance.Insert(signatureRequest, "sp_InsertOrdinance_SignatureRequest", Skips("ordSignatureRequestInsert"));

                if (signatureRequestRet < 1)
                {
                    finishSubmit = false;
                }

                OrdinanceAudit ordAudit = new OrdinanceAudit()
                {
                    OrdinanceID = Convert.ToInt32(retVal),
                    UpdateType = "CREATED",
                    LastUpdateBy = $"{_user.FirstName} {_user.LastName}",
                    LastUpdateDate = DateTime.Now,
                };
                int ordAuditRet = Factory.Instance.Insert(ordAudit, "sp_InsertOrdinance_Audit", Skips("ordAuditInsert"));
                if (ordAuditRet < 1)
                {
                    finishSubmit = false;
                }


                Email.Instance.AddEmailAddress(emailList, _user.Email);
                string formType = "Ordinance Fact Sheet";
                string href = $"apptest/Themis/Ordinances?id={retVal}&v=view";

                Email newEmail = new Email();

                newEmail.EmailSubject = $"{formType} SUBMITTED";
                newEmail.EmailTitle = $"{formType} SUBMITTED";
                newEmail.EmailText = $"<p style='margin: 0;'><span style='font-size:36.0pt;font-family:\"Times New Roman\",serif;color:#2D71D5;font-weight:bold'>THΣMIS</span></p><div align=center style='text-align:center'><span><hr size='2' width='100%' align='center' style='margin-top: 0;'></span></div><p><span>An <b>{formType}</b> has been SUBMITTED by <b>{_user.FirstName} {_user.LastName}</b>.</span></p><br /><p style='margin: 0; line-height: 1.5;'><span>ID: {retVal}</span></p><p style='margin: 0; line-height: 1.5;'><span>Date: {DateTime.Now}</span></p><p style='margin: 0; line-height: 1.5;'><span>Department: {requestDepartment.SelectedItem.Text}</span></p><p style='margin: 0; line-height: 1.5;'><span>Contact: {requestContact.Text}</span></p><p style='margin: 0; line-height: 1.5;'><span>Phone: {ordinance.RequestPhone}</span></p><br /><p><span>Please click the button below to review the document:</span></p><table border='0' cellpadding='0' cellspacing='0' style='border-collapse: separate; mso-table-lspace: 0pt; mso-table-rspace: 0pt; width: auto;'><tr><td style='font-family: sans-serif; font-size: 14px; vertical-align: top; background-color: #0d6efd; border-radius: 5px; text-align: center;' valign='top' bgcolor='#0d6efd' align='center'><a href='{href}' target='_blank' style='display: inline-block; color: #ffffff; background-color: #0d6efd; border: solid 1px #0d6efd; border-radius: 5px; box-sizing: border-box; cursor: pointer; text-decoration: none; font-size: 18px; font-weight: bold; margin: 0; padding: 15px 25px; text-transform: capitalize; border-color: #0d6efd; '>View Ordinance</a></td></tr></table>";
                switch (finishSubmit)
                {
                    case true:
                        Session["SubmitStatus"] = "success";
                        Session["ToastColor"] = "text-bg-success";
                        Session["ToastMessage"] = "Form Submitted!";
                        Email.Instance.SendEmail(newEmail, emailList);
                        Response.Redirect("./NewFactSheet", false);
                        break;
                    case false:
                        Session["SubmitStatus"] = "error";
                        Session["ToastColor"] = "text-bg-danger";
                        Session["ToastMessage"] = "Something went wrong while submitting!";
                        break;
                }
            }
            else
            {
                Session["SubmitStatus"] = "error";
                Session["ToastColor"] = "text-bg-danger";
                Session["ToastMessage"] = "Something went wrong while submitting!";
            }
        }
        protected void rpSupportingDocumentation_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            //HiddenField hdnDocID = (HiddenField)e.Item.FindControl("hdnDocID");
            HiddenField hdnDocIndex = (HiddenField)e.Item.FindControl("hdnDocIndex");
            List<OrdinanceDocument> ordDocList = Session["ordDocs"] as List<OrdinanceDocument>;
            OrdinanceDocument ordDocItem = ordDocList[Convert.ToInt32(hdnDocIndex.Value)];

            switch (e.CommandName)
            {
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
        protected void GetUploadedDocs()
        {
            if (Session["ordDocs"] != null)
            {
                List<OrdinanceDocument> ordDocList = Session["ordDocs"] as List<OrdinanceDocument>;
                rpSupportingDocumentation.DataSource = ordDocList;
                rpSupportingDocumentation.DataBind();
            }
        }
        protected void UploadDocBtn_Click(object sender, EventArgs e)
        {
            List<OrdinanceDocument> ordDocList = (Session["ordDocs"] != null) ? Session["ordDocs"] as List<OrdinanceDocument> : new List<OrdinanceDocument>();

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
            Session["ordDocs"] = ordDocList;
            rpSupportingDocumentation.DataSource = ordDocList;
            rpSupportingDocumentation.DataBind();
        }      
        protected void GetCopyOrdinance(int ordID)
        {
            Session.Remove("revenue");
            Session.Remove("expenditure");
            Ordinance ord = Factory.Instance.GetByID<Ordinance>(ordID, "sp_GetOrdinanceByOrdinanceID", "OrdinanceID");


            requestDepartment.SelectedValue = DepartmentsList()[ord.RequestDepartment];

            switch (ord.EmergencyPassage)
            {
                case true:
                    epYes.Checked = true;
                    epNo.Checked = false;
                    epJustificationGroup.Visible = true;
                    epJustification.Attributes.Add("required", "true");
                    break;
                case false:
                    epYes.Checked = false;
                    epNo.Checked = true;
                    epJustificationGroup.Visible = false;
                    epJustification.Attributes.Remove("required");
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
                    changeOrderNumber.Enabled = true;
                    additionalAmount.Enabled = true;
                    changeOrderNumber.Attributes.Add("required", "true");
                    additionalAmount.Attributes.Add("required", "true");
                        break;
                case false:
                    scYes.Checked = false;
                    scNo.Checked = true;
                        changeOrderNumber.Enabled = false;
                        additionalAmount.Enabled = false;
                        changeOrderNumber.Attributes.Remove("required");
                        additionalAmount.Attributes.Remove("required");
                        break;
            }
            changeOrderNumber.Text = ord.ChangeOrderNumber;
            additionalAmount.Text = NotApplicable(ord.AdditionalAmount.ToString());


            purchaseMethod.SelectedValue = ord.ContractMethod;
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
            otherException.Text = ord.OtherException;

            List<string> prvOrdNums = ord.PreviousOrdinanceNumbers.Trim().Split(',').Where(i => !i.IsNullOrWhiteSpace()).ToList();
            
            if (!ord.OrdinanceNumber.IsNullOrWhiteSpace())
            {
                prvOrdNums.Add(ord.OrdinanceNumber);
            }
            prevOrdinanceNums.Text = string.Join(", ", prvOrdNums);
            ord.PreviousOrdinanceNumbers = prevOrdinanceNums.Text;
            
            

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
                    rpRevenueTable.DataSource = revItems;
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
                    rpExpenditureTable.DataSource = expItems;
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

            staffAnalysis.Text = ord.OrdinanceAnalysis;

            ScriptManager.RegisterStartupScript(this, this.GetType(), "CurrencyFormatting", "CurrencyFormatting();", true);
            ScriptManager.RegisterStartupScript(this, this.GetType(), "FormatForms", "FormatForms();", true);
        }

        protected void requestDepartment_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!requestDepartment.SelectedValue.IsNullOrWhiteSpace())
            {
                requestDivision.Enabled = true;
                GetAllDivisions(requestDepartment.SelectedValue);
                requestDivision.SelectedValue = userInfo.UserDivision.DivisionCode.ToString();
            }
            else
            {
                requestDivision.Enabled = false;
                requestDivision.Items.Add(new ListItem() { Text = "Select Division...", Value = "" });
            }
        }
    }
}