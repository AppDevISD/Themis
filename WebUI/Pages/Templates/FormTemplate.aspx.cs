using DataLibrary;
using ISD.ActiveDirectory;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebUI
{
    public partial class FormTemplate : System.Web.UI.Page
    {
        private ADUser _user = new ADUser();
        private string emailList = "TemplateEmailList";
        public string toastColor;
        public string toastMessage;

        protected void Page_Load(object sender, EventArgs e)
        {
            _user = Session["CurrentUser"] as ADUser;
            if (!Page.IsPostBack)
            {
                GetAllDropdownOptions();
                SetStartupActives();
            }
            SubmitStatus();
        }
        protected void SetStartupActives()
        {
            dropdownOther.Enabled = false;
        }
        protected void GetAllDropdownOptions()
        {
            dropdown.Items.Insert(0, new ListItem("Select Option...", "N/A"));
            dropdown.Items.Insert(1, new ListItem("Item 1", "Item 1"));
            dropdown.Items.Insert(2, new ListItem("Item 2", "Item 2"));
            dropdown.Items.Insert(3, new ListItem("Item 3", "Item 3"));
            dropdown.Items.Insert(4, new ListItem("Other", "Other"));
        }
        protected void DropdownSelectedIndexChanged(object sender, EventArgs e)
        {
            switch (dropdown.SelectedItem.Value)
            {
                default:
                    dropdownOther.Enabled = false;
                    dropdownOther.Text = string.Empty;
                    dropdownOther.Attributes.Remove("required");
                    break;
                case "Other":
                    dropdownOther.Enabled = true;
                    dropdownOther.Attributes.Add("required", "true");
                    break;
            }
        }

        protected void SubmitForm_Click(object sender, EventArgs e)
        {
            Email.Instance.AddEmailAddress(emailList, _user.Email);
            string formType = "Template Form";

            Email newEmail = new Email();

            newEmail.EmailSubject = "New No-Fun Request";
            newEmail.EmailTitle = "New No-Fun Request";
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
                Response.Redirect("/FormTemplate");
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