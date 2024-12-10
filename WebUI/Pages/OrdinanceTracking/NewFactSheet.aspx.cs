using DataLibrary;
using ISD.ActiveDirectory;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Emit;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

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
                new ListItem(" ", " "),
                new ListItem("100", "100"),
                new ListItem("101", "101"),
                new ListItem("102", "102")
            };
        public ListItemCollection agencyCodes = new ListItemCollection()
            {
                new ListItem("", " "),
                new ListItem("100", "100"),
                new ListItem("101", "101"),
                new ListItem("102", "102")
            };
        public ListItemCollection orgCodes = new ListItemCollection()
            {
                new ListItem("", " "),
                new ListItem("CABC", "CABC"),
                new ListItem("BABC", "BABC"),
                new ListItem("ABAC", "ABAC")
            };
        public ListItemCollection activityCodes = new ListItemCollection()
            {
                new ListItem("", " "),
                new ListItem("8018", "8018"),
                new ListItem("8019", "8019"),
                new ListItem("8020", "8020")
            };
        public ListItemCollection objectCodes = new ListItemCollection()
            {
                new ListItem("", " "),
                new ListItem("1418", "1418"),
                new ListItem("1419", "1419"),
                new ListItem("1420", "1420")
            };
        
        List<Accounting> expenditureList = new List<Accounting>();
        public int revRowNum = 0;
        public int expRowNum = 0;

        protected void Page_Load(object sender, EventArgs e)
        {
            _user = Session["CurrentUser"] as ADUser;
            if (!Page.IsPostBack && !Response.IsRequestBeingRedirected)
            {
                Session.Remove("RevList");
                Session.Remove("ExpList");
                GetAllDepartments();
                GetAllPurchaseMethods();
                SetStartupActives();

                //TestOpenVariable();
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
        protected void AddAccountingRow(string type)
        {
            Accounting accountingItem = new Accounting();
            accountingItem.AccountingDesc = type;
            accountingItem.LastUpdateBy = _user.Login;
            accountingItem.LastUpdateDate = DateTime.Now;
            accountingItem.EffectiveDate = DateTime.Now;
            accountingItem.ExpirationDate = DateTime.MaxValue;
            switch (type)
            {
                case "revenue":
                    int retVal = Factory.Instance.Insert(accountingItem, "sp_InsertlkAccounting");
                    if (retVal > 0)
                    {
                        List<Accounting> revenueList = Factory.Instance.GetAll<Accounting>("sp_GetLkAccounting");
                        rpRevenueTable.DataSource = revenueList;
                        rpRevenueTable.DataBind();
                    }
                    break;
                case "expenditure":
                    break;
            }
        }
        protected void RemoveAccountingRow(string type, int row)
        {
            switch (type)
            {
                case "revenue":
                    //if (Session["RevList"] != null)
                    //{
                    //    revenueList = (List<Accounting>)Session["RevList"];
                    //}
                    //revenueList.RemoveAt(row);
                    //rpRevenueTable.DataSource = revenueList;
                    //rpRevenueTable.DataBind();
                    //Session["RevList"] = revenueList;
                    break;
                case "expenditure":
                    //if (Session["ExpList"] != null)
                    //{
                    //    expenditureList = (List<Accounting>)Session["ExpList"];
                    //}
                    //expenditureList.RemoveAt(row);
                    //rpExpenditureTable.DataSource = expenditureList;
                    //rpExpenditureTable.DataBind();
                    //Session["ExpList"] = expenditureList;
                    break;
            }
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
        protected void SubmitForm_Click(object sender, EventArgs e)
        {
            Email.Instance.AddEmailAddress(emailList, _user.Email);
            string formType = "Template Form";

            Email newEmail = new Email();

            newEmail.EmailSubject = "Form Submitted";
            newEmail.EmailTitle = "Form Submitted";
            newEmail.EmailText = $"This is a template email body for the {formType}";

            Template tf = new Template();

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
        protected void newAccountingRow_ServerClick(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            string type = button.CommandName;
            Accounting accountingItem = new Accounting();
            accountingItem.AccountingDesc = type;
            accountingItem.FundCode = " ";
            accountingItem.DepartmentCode = " ";
            accountingItem.UnitCode = " ";
            accountingItem.ActivityCode = " ";
            accountingItem.ObjectCode = " ";
            accountingItem.Amount = Convert.ToDecimal(0);
            accountingItem.LastUpdateBy = _user.Login;
            accountingItem.LastUpdateDate = DateTime.Now;
            accountingItem.EffectiveDate = DateTime.Now;
            accountingItem.ExpirationDate = DateTime.MaxValue;
            //Debug.WriteLine(Convert.ToInt32("$1,000.00"));
            switch (type)
            {
                case "revenue":
                    int retVal = Factory.Instance.Insert(accountingItem, "sp_InsertlkAccounting");
                    Debug.WriteLine(retVal);
                    if (retVal > 0)
                    {
                        List<Accounting> revenueList = Factory.Instance.GetAll<Accounting>("sp_GetLkAccounting");
                        rpRevenueTable.DataSource = revenueList;
                        rpRevenueTable.DataBind();
                    }
                    break;
                case "expenditure":
                    break;
            }
        }

        protected void removeAccountingRow_ServerClick(object sender, EventArgs e)
        {
            //Button button = (Button)sender;
            //string type = button.CommandName;
            //int row = Convert.ToInt32(button.Attributes["data-row-num"]);
            //if (Session["RevList"] != null)
            //{
            //    revenueList = (List<Accounting>)Session["RevList"];
            //}
            //if (revenueList.Count > 0)
            //{
            //    RemoveAccountingRow(type, row);
            //}
        }



        protected void TestOpenVariable()
        {
            Accounting revItem = new Accounting();
            revItem.FundCode = "101";
            revItem.DepartmentCode = "102";
            revItem.UnitCode = "103";
            revItem.ActivityCode = "104";
            revItem.ObjectCode = "105";
            revItem.Amount = Convert.ToDecimal(5);
            Factory.Instance.TestVariable(revItem);
        }
    }
}