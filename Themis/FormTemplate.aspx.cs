using DataLibrary;
using ISD.ActiveDirectory;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace Themis
{
    public partial class FormTemplate : System.Web.UI.Page
    {
        private ADUser _user = new ADUser();
        private ADUser contactUser = new ADUser();
        private string userEmail;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["CurrentUser"] == null)
            {
                _user = Utility.Instance.AuthenticateUser();
                Session["UserName"] = _user.Login;
                userEmail = _user.Email;
            }
            else
            {
                _user = (ADUser)Session["CurrentUser"];
                userEmail = _user.Email;
            }
        }

        protected void TemplateFormSubmit_Click(object sender, EventArgs e)
        {
            string emailList = "TemplateEmailList";
            string permanentEmail = "";
            string submitContact = contact_name.Value;
            string submitEmployee = employee_name.Value;
            string submitReason = reason_why.Text;
            Email.Instance.AddEmailAddress(emailList, userEmail);

            Email newEmail = new Email();

            newEmail.EmailSubject = "Template Form";
            newEmail.EmailTitle = "Template Form";
            newEmail.EmailText = $"This is a Template Form email: <br/><br/>Employee: {submitEmployee} <br/>Reason: {submitReason} <br/><br/>If you have any questions regarding this request, please contact {submitContact}.";

            Email.Instance.SendEmail(newEmail, emailList);


            TemplateForm tf = new TemplateForm();
            tf.FormTypeID = Convert.ToInt32(1);
            tf.EffectiveDate = DateTime.Now;
            tf.ContactName = submitContact;
            tf.EmployeeName = submitEmployee;
            tf.Comments = submitReason;
            tf.ExpirationDate = DateTime.MaxValue;
            tf.LastUpdateDate = DateTime.Now;
            tf.LastUpdateBy = _user.Login;

            int flag = tf.Insert();

            if (flag == -32)
            {
                Debug.Write("\nNope\n");
            }
            else
            {
                Debug.Write("\nYep\n");
            }

            divSuccess.Visible = true;
            CleanForm(this);

            Email.Instance.ResetEmailList(emailList, permanentEmail);
        }

        protected void CleanForm(Control control)
        {
            foreach (Control c in control.Controls)
            {
                if (c is TextBox) ((TextBox)c).Text = String.Empty;
                if (c is HtmlInputText) ((HtmlInputText)c).Value = String.Empty;
                if (c is DropDownList) ((DropDownList)c).SelectedValue = "select";
                if (c is CheckBox) ((CheckBox)c).Checked = false;
                if (c is RadioButton) ((RadioButton)c).Checked = false;

                CleanForm(c);
            }
        }
    }
}