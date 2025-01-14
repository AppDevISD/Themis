﻿using DataLibrary;
using ISD.ActiveDirectory;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using static DataLibrary.Utility;

namespace WebUI
{
    public partial class NewFactSheet : System.Web.UI.Page
    {
        private ADUser _user = new ADUser();
        private string emailList = "NewFactSheetEmailList";
        public string toastColor;
        public string toastMessage;

        public ListItemCollection fundCodes = new ListItemCollection()
        {
            new ListItem("", null),
            new ListItem("100", "100"),
            new ListItem("101", "101"),
            new ListItem("102", "102")
        };
        public ListItemCollection agencyCodes = new ListItemCollection()
        {
            new ListItem("", null),
            new ListItem("100", "100"),
            new ListItem("101", "101"),
            new ListItem("102", "102")
        };
        public ListItemCollection orgCodes = new ListItemCollection()
        {
            new ListItem("", null),
            new ListItem("CABC", "CABC"),
            new ListItem("BABC", "BABC"),
            new ListItem("ABAC", "ABAC")
        };
        public ListItemCollection activityCodes = new ListItemCollection()
        {
            new ListItem("", null),
            new ListItem("8018", "8018"),
            new ListItem("8019", "8019"),
            new ListItem("8020", "8020")
        };
        public ListItemCollection objectCodes = new ListItemCollection()
        {
            new ListItem("", null),
            new ListItem("1418", "1418"),
            new ListItem("1419", "1419"),
            new ListItem("1420", "1420")
        };

        protected void Page_Load(object sender, EventArgs e)
        {
            _user = Session["CurrentUser"] as ADUser;
            if (!Page.IsPostBack && !Response.IsRequestBeingRedirected)
            {
                Session.Remove("revenue");
                Session.Remove("expenditure");
                GetAllDepartments();
                GetAllPurchaseMethods();
                SetStartupActives();
                NewAccountingRow("revenue");
                NewAccountingRow("expenditure");
            }
            SubmitStatus();
        }
        protected void SetStartupActives()
        {
            epJustificationGroup.Visible = false;
            changeOrderNumber.Enabled = false;
            additionalAmount.Enabled = false;
            otherException.Enabled = false;
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
        protected void NewAccountingRow(string tableDesc)
        {
            List<Accounting> prvList = new List<Accounting>();
            List<Accounting> accountingList = new List<Accounting>();
            Accounting newAccountingItem = new Accounting();
            newAccountingItem.Amount = CurrencyToDecimal("-1");

            switch (tableDesc)
            {
                case "revenue":
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
                case "expenditure":
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
                        case "revenue":
                            for (int i = 0; i < rpRevenueTable.Items.Count; i++)
                            {
                                Accounting accountingItem = new Accounting();
                                var revItem = rpRevenueTable.Items[i];
                                //DropDownList revFundCode = (DropDownList)revItem.FindControl("revenueFundCode");
                                //DropDownList revAgencyCode = (DropDownList)revItem.FindControl("revenueAgencyCode");
                                //DropDownList revOrgCode = (DropDownList)revItem.FindControl("revenueOrgCode");
                                //DropDownList revActivityCode = (DropDownList)revItem.FindControl("revenueActivityCode");
                                //DropDownList revObjectCode = (DropDownList)revItem.FindControl("revenueObjectCode");
                                //TextBox revAmount = (TextBox)revItem.FindControl("revenueAmount");
                                //accountingItem.FundCode = revFundCode.SelectedValue;
                                //accountingItem.DepartmentCode = revAgencyCode.SelectedValue;
                                //accountingItem.UnitCode = revOrgCode.SelectedValue;
                                //accountingItem.ActivityCode = revActivityCode.SelectedValue;
                                //accountingItem.ObjectCode = revObjectCode.SelectedValue;
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
                                //DropDownList expFundCode = (DropDownList)expItem.FindControl("expenditureFundCode");
                                //DropDownList expAgencyCode = (DropDownList)expItem.FindControl("expenditureAgencyCode");
                                //DropDownList expOrgCode = (DropDownList)expItem.FindControl("expenditureOrgCode");
                                //DropDownList expActivityCode = (DropDownList)expItem.FindControl("expenditureActivityCode");
                                //DropDownList expObjectCode = (DropDownList)expItem.FindControl("expenditureObjectCode");
                                //TextBox expAmount = (TextBox)expItem.FindControl("expenditureAmount");
                                //accountingItem.AccountingDesc = tableDesc;
                                //accountingItem.FundCode = expFundCode.SelectedValue;
                                //accountingItem.DepartmentCode = expAgencyCode.SelectedValue;
                                //accountingItem.UnitCode = expOrgCode.SelectedValue;
                                //accountingItem.ActivityCode = expActivityCode.SelectedValue;
                                //accountingItem.ObjectCode = expObjectCode.SelectedValue;
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
                    //DropDownList revFundCode = (DropDownList)revItem.FindControl("revenueFundCode");
                    //DropDownList revAgencyCode = (DropDownList)revItem.FindControl("revenueAgencyCode");
                    //DropDownList revOrgCode = (DropDownList)revItem.FindControl("revenueOrgCode");
                    //DropDownList revActivityCode = (DropDownList)revItem.FindControl("revenueActivityCode");
                    //DropDownList revObjectCode = (DropDownList)revItem.FindControl("revenueObjectCode");
                    //TextBox revAmount = (TextBox)revItem.FindControl("revenueAmount");
                    //accountingItem.AccountingDesc = tableDesc;
                    //accountingItem.FundCode = revFundCode.SelectedValue;
                    //accountingItem.DepartmentCode = revAgencyCode.SelectedValue;
                    //accountingItem.UnitCode = revOrgCode.SelectedValue;
                    //accountingItem.ActivityCode = revActivityCode.SelectedValue;
                    //accountingItem.ObjectCode = revObjectCode.SelectedValue;
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
                    //DropDownList expFundCode = (DropDownList)expItem.FindControl("expenditureFundCode");
                    //DropDownList expAgencyCode = (DropDownList)expItem.FindControl("expenditureAgencyCode");
                    //DropDownList expOrgCode = (DropDownList)expItem.FindControl("expenditureOrgCode");
                    //DropDownList expActivityCode = (DropDownList)expItem.FindControl("expenditureActivityCode");
                    //DropDownList expObjectCode = (DropDownList)expItem.FindControl("expenditureObjectCode");
                    //TextBox expAmount = (TextBox)expItem.FindControl("expenditureAmount");
                    //accountingItem.AccountingDesc = tableDesc;
                    //accountingItem.FundCode = expFundCode.SelectedValue;
                    //accountingItem.DepartmentCode = expAgencyCode.SelectedValue;
                    //accountingItem.UnitCode = expOrgCode.SelectedValue;
                    //accountingItem.ActivityCode = expActivityCode.SelectedValue;
                    //accountingItem.ObjectCode = expObjectCode.SelectedValue;
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
        protected void SubmitForm_Click(object sender, EventArgs e)
        {
            Email.Instance.AddEmailAddress(emailList, _user.Email);
            string formType = "Ordinance Fact Sheet";

            Email newEmail = new Email();

            newEmail.EmailSubject = "Form Submitted";
            newEmail.EmailTitle = "Form Submitted";
            newEmail.EmailText = $"This is a template email body for the {formType}";

            Ordinance ordinance = new Ordinance();

            //ordinance.OrdinanceNumber = "6515dfgsaf";
            //ordinance.RequestID = 0;
            //ordinance.RequestDepartment = requestDepartment.SelectedItem.Text;
            //ordinance.RequestContact = requestContact.Text;
            //ordinance.RequestPhone = $"{requestPhone.Text}{requestExt.Text}";
            //ordinance.FirstReadDate = Convert.ToDateTime(firstReadDate.Text);
            //ordinance.EmergencyPassage = epYes.Checked;
            //ordinance.EmergencyPassageReason = epJustification.Text ?? string.Empty;
            //ordinance.OrdinanceFiscalImpact = CurrencyToDecimal(fiscalImpact.Text);
            //ordinance.OrdinanceTitle = suggestedTitle.Text;
            //ordinance.ContractVendorID = 0;
            //ordinance.ContractVendorName = vendorName.Text;
            //ordinance.ContractVendorNumber = vendorNumber.Text;
            //ordinance.ContractStartDate = contractStartDate.Text;
            //ordinance.ContractEndDate = contractEndDate.Text;
            //ordinance.ContractTerm = contractTerm.Value;
            //ordinance.ContractAmount = CurrencyToDecimal(contractAmount.Text);
            //ordinance.ScopeChange = scYes.Checked;
            //ordinance.ChangeOrderNumber = changeOrderNumber.Text ?? string.Empty;
            //if (scYes.Checked)
            //{
                
            //    ordinance.AdditionalAmount = CurrencyToDecimal(additionalAmount.Text);
            //}
            //else
            //{
            //    ordinance.AdditionalAmount = CurrencyToDecimal("-1");
            //}
            //ordinance.ContractMethod = purchaseMethod.SelectedValue;
            //ordinance.OtherException = otherException.Text ?? string.Empty;
            //ordinance.PreviousOrdinanceNumbers = prevOrdinanceNums.Text;
            //ordinance.CodeProvision = codeProvision.Text;
            //ordinance.PAApprovalRequired = paApprovalRequiredYes.Checked;
            //ordinance.PAApprovalIncluded = paApprovalAttachedYes.Checked;
            //ordinance.OrdinanceAnalysis = staffAnalysis.Text;
            //ordinance.LastUpdateBy = _user.Login;
            //ordinance.LastUpdateDate = DateTime.Now;
            //ordinance.EffectiveDate = DateTime.Now;
            //ordinance.ExpirationDate = DateTime.MaxValue;

            //int retVal = Factory.Instance.Insert(ordinance, "sp_InsertOrdinance");
            int retVal = 1;
            if (retVal > 0)
            {
                bool revExpTables = false;
                bool documentation = false;
                bool finishSubmit = false;
                if (rpRevenueTable.Items.Count > 0 || rpExpenditureTable.Items.Count > 0)
                {
                    revExpTables = true;
                }
                string testName = supportingDocumentation.FileName;
                Debug.WriteLine(testName);
                //if (supportingDocumentation.PostedFiles.Count > 0)
                //{

                //    documentation = true;
                //}

                switch (revExpTables)
                {
                    case true:
                        bool revSubmit = false;
                        bool expSubmit = false;
                        if (rpRevenueTable.Items.Count > 0)
                        {
                            for (int i = 0; i < rpRevenueTable.Items.Count; i++)
                            {
                                Accounting accountingItem = GetAccountingItem("revenue", i);
                                int ret = Factory.Instance.Insert(accountingItem, "sp_InsertlkAccounting");
                                //int ret = 1;
                                if (ret > 0)
                                {
                                    OrdinanceAccounting oaItem = new OrdinanceAccounting();
                                    oaItem.OrdinanceID = retVal;
                                    oaItem.AccountingID = ret;
                                    oaItem.LastUpdateBy = _user.Login;
                                    oaItem.LastUpdateDate = DateTime.Now;
                                    oaItem.EffectiveDate = DateTime.Now;
                                    oaItem.ExpirationDate = DateTime.MaxValue;
                                    int finalRet = Factory.Instance.Insert(oaItem, "sp_InsertOrdinance_Accounting");
                                    //int finalRet = 1;
                                    if (finalRet > 0)
                                    {
                                        revSubmit = true;
                                    }
                                    else
                                    {
                                        revSubmit = false;
                                    }
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
                                Accounting accountingItem = GetAccountingItem("expenditure", i);
                                //int ret = Factory.Instance.Insert(accountingItem, "sp_InsertlkAccounting");
                                int ret = 1;
                                if (ret > 0)
                                {
                                    OrdinanceAccounting oaItem = new OrdinanceAccounting();
                                    oaItem.OrdinanceID = retVal;
                                    oaItem.AccountingID = ret;
                                    oaItem.LastUpdateBy = _user.Login;
                                    oaItem.LastUpdateDate = DateTime.Now;
                                    oaItem.EffectiveDate = DateTime.Now;
                                    oaItem.ExpirationDate = DateTime.MaxValue;
                                    //int finalRet = Factory.Instance.Insert(oaItem, "sp_InsertOrdinance_Accounting");
                                    int finalRet = 1;
                                    if (finalRet > 0)
                                    {
                                        expSubmit = true;
                                    }
                                    else
                                    {
                                        expSubmit = false;
                                    }
                                }
                            }
                        }
                        else
                        {
                            expSubmit = true;
                        }

                        if (revSubmit && expSubmit)
                        {
                            finishSubmit = true;
                        }
                        else
                        {
                            finishSubmit = false;
                        }
                        break;
                    case false:
                        finishSubmit = true;
                        break;
                }

                try
                {
                    foreach (HttpPostedFile postedFile in supportingDocumentation.PostedFiles)
                    {
                        string fileName = Path.GetFileName(postedFile.FileName);
                        postedFile.SaveAs(Server.MapPath(path: "~/Uploads/") + fileName);
                        Debug.WriteLine(fileName);
                    }
                    finishSubmit = true;
                }
                catch (Exception)
                {

                    finishSubmit = false;
                }

                switch (documentation)
                {
                    case true:
                        //for (int i = 0; i < supportingDocumentation.PostedFiles.Count; i++)
                        //{
                        //    OrdinanceDocument ordDoc = new OrdinanceDocument();
                        //    ordDoc.OrdinanceID = retVal;
                        //    ordDoc.DocumentName = supportingDocumentation.PostedFiles[i].FileName;
                        //    //int ret = Factory.Instance.Insert(ordDoc, "sp_InsertlkAccounting");
                        //    int ret = 1;
                        //    if (ret > 0)
                        //    {
                        //        Debug.WriteLine(ordDoc.DocumentName);
                        //    }
                        //}
                        
                        break;
                    case false:
                        finishSubmit = true;
                        break;
                }

                switch (finishSubmit)
                {
                    case true:
                        Session["SubmitStatus"] = "success";
                        Session["ToastColor"] = "text-bg-success";
                        Session["ToastMessage"] = "Form Submitted!";
                        //Email.Instance.SendEmail(newEmail, emailList);
                        Response.Redirect("./NewFactSheet");
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
    }
}