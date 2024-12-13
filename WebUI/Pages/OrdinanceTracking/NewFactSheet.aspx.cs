using DataLibrary;
using ISD.ActiveDirectory;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Emit;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using static DataLibrary.Utility;

namespace WebUI
{
    public partial class NewFactSheet : System.Web.UI.Page
    {
        private ADUser _user = new ADUser();
        private string emailList = "TemplateEmailList";
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
                            Accounting accountingItem = new Accounting();
                            var revItem = rpRevenueTable.Items[i];
                            DropDownList revFundCode = (DropDownList)revItem.FindControl("revenueFundCode");
                            DropDownList revAgencyCode = (DropDownList)revItem.FindControl("revenueAgencyCode");
                            DropDownList revOrgCode = (DropDownList)revItem.FindControl("revenueOrgCode");
                            DropDownList revActivityCode = (DropDownList)revItem.FindControl("revenueActivityCode");
                            DropDownList revObjectCode = (DropDownList)revItem.FindControl("revenueObjectCode");
                            TextBox revAmount = (TextBox)revItem.FindControl("revenueAmount");
                            accountingItem.AccountingDesc = tableDesc;
                            accountingItem.FundCode = revFundCode.SelectedValue;
                            accountingItem.DepartmentCode = revAgencyCode.SelectedValue;
                            accountingItem.UnitCode = revOrgCode.SelectedValue;
                            accountingItem.ActivityCode = revActivityCode.SelectedValue;
                            accountingItem.ObjectCode = revObjectCode.SelectedValue;
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
                            Accounting accountingItem = new Accounting();
                            var expItem = rpExpenditureTable.Items[i];
                            DropDownList expFundCode = (DropDownList)expItem.FindControl("expenditureFundCode");
                            DropDownList expAgencyCode = (DropDownList)expItem.FindControl("expenditureAgencyCode");
                            DropDownList expOrgCode = (DropDownList)expItem.FindControl("expenditureOrgCode");
                            DropDownList expActivityCode = (DropDownList)expItem.FindControl("expenditureActivityCode");
                            DropDownList expObjectCode = (DropDownList)expItem.FindControl("expenditureObjectCode");
                            TextBox expAmount = (TextBox)expItem.FindControl("expenditureAmount");
                            accountingItem.AccountingDesc = tableDesc;
                            accountingItem.FundCode = expFundCode.SelectedValue;
                            accountingItem.DepartmentCode = expAgencyCode.SelectedValue;
                            accountingItem.UnitCode = expOrgCode.SelectedValue;
                            accountingItem.ActivityCode = expActivityCode.SelectedValue;
                            accountingItem.ObjectCode = expObjectCode.SelectedValue;
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
                                DropDownList revFundCode = (DropDownList)revItem.FindControl("revenueFundCode");
                                DropDownList revAgencyCode = (DropDownList)revItem.FindControl("revenueAgencyCode");
                                DropDownList revOrgCode = (DropDownList)revItem.FindControl("revenueOrgCode");
                                DropDownList revActivityCode = (DropDownList)revItem.FindControl("revenueActivityCode");
                                DropDownList revObjectCode = (DropDownList)revItem.FindControl("revenueObjectCode");
                                TextBox revAmount = (TextBox)revItem.FindControl("revenueAmount");
                                accountingItem.AccountingDesc = tableDesc;
                                accountingItem.FundCode = revFundCode.SelectedValue;
                                accountingItem.DepartmentCode = revAgencyCode.SelectedValue;
                                accountingItem.UnitCode = revOrgCode.SelectedValue;
                                accountingItem.ActivityCode = revActivityCode.SelectedValue;
                                accountingItem.ObjectCode = revObjectCode.SelectedValue;
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
                                DropDownList expFundCode = (DropDownList)expItem.FindControl("expenditureFundCode");
                                DropDownList expAgencyCode = (DropDownList)expItem.FindControl("expenditureAgencyCode");
                                DropDownList expOrgCode = (DropDownList)expItem.FindControl("expenditureOrgCode");
                                DropDownList expActivityCode = (DropDownList)expItem.FindControl("expenditureActivityCode");
                                DropDownList expObjectCode = (DropDownList)expItem.FindControl("expenditureObjectCode");
                                TextBox expAmount = (TextBox)expItem.FindControl("expenditureAmount");
                                accountingItem.AccountingDesc = tableDesc;
                                accountingItem.FundCode = expFundCode.SelectedValue;
                                accountingItem.DepartmentCode = expAgencyCode.SelectedValue;
                                accountingItem.UnitCode = expOrgCode.SelectedValue;
                                accountingItem.ActivityCode = expActivityCode.SelectedValue;
                                accountingItem.ObjectCode = expObjectCode.SelectedValue;
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
        protected void SubmitForm_Click(object sender, EventArgs e)
        {
            Email.Instance.AddEmailAddress(emailList, _user.Email);
            string formType = "Ordinance Fact Sheet";

            Email newEmail = new Email();

            newEmail.EmailSubject = "Form Submitted";
            newEmail.EmailTitle = "Form Submitted";
            newEmail.EmailText = $"This is a template email body for the {formType}";

            //Template tf = new Template();

            //int retVal = Factory.Instance.InsertTemplateForm(tf);
            int retVal = 1;
            if (retVal > 0)
            {
                Session["SubmitStatus"] = "success";
                Session["ToastColor"] = "text-bg-success";
                Session["ToastMessage"] = "Form Submitted!";
                Email.Instance.SendEmail(newEmail, emailList);
                Response.Redirect("/NewFactSheet");
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
        












        //protected void newAccountingRow_ServerClick(object sender, EventArgs e)
        //{
        //    Button button = (Button)sender;
        //    string type = button.CommandName;
        //    Accounting accountingItem = new Accounting();
        //    accountingItem.AccountingDesc = type;
        //    accountingItem.FundCode = " ";
        //    accountingItem.DepartmentCode = " ";
        //    accountingItem.UnitCode = " ";
        //    accountingItem.ActivityCode = " ";
        //    accountingItem.ObjectCode = " ";
        //    accountingItem.Amount = CurrencyToDecimal("0");
        //    accountingItem.LastUpdateBy = _user.Login;
        //    accountingItem.LastUpdateDate = DateTime.Now;
        //    accountingItem.EffectiveDate = DateTime.Now;
        //    accountingItem.ExpirationDate = DateTime.MaxValue;
        //    switch (type)
        //    {
        //        case "revenue":
        //            int retVal = Factory.Instance.Insert(accountingItem, "sp_InsertlkAccounting");
        //            if (retVal > 0)
        //            {
        //                //IEnumerable<Accounting> revenueList = Factory.Instance.GetAllAccounting().Where(item => item.AccountingDesc.Equals("revenue"));
        //                IEnumerable<Accounting> revenueList = Factory.Instance.GetAll<Accounting>("sp_GetLkAccounting").Where(item => item.AccountingDesc.Equals("revenue"));
        //                rpRevenueTable.DataSource = revenueList;
        //                rpRevenueTable.DataBind();
        //            }
        //            break;
        //        case "expenditure":
        //            break;
        //    }
        //}

        //protected void rpRevenueTable_ItemCommand(object source, RepeaterCommandEventArgs e)
        //{
        //    switch (e.CommandName)
        //    {
        //        case "delete":
        //            HiddenField hdnID = (HiddenField)e.Item.FindControl("hdnRevID");
        //            int retVal = Factory.Instance.Delete<Accounting>(Convert.ToInt32(hdnID.Value), "lkAccounting");
        //            if (retVal > 0)
        //            {
        //                IEnumerable<Accounting> list = Factory.Instance.GetAll<Accounting>("sp_GetLkAccounting").Where(item => item.AccountingDesc.Equals(e.CommandArgument));
        //                rpRevenueTable.DataSource = list;
        //                rpRevenueTable.DataBind();
        //            }
        //            break;
        //    }
        //}
    }
}