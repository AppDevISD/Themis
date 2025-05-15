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
    public partial class Sandbox : System.Web.UI.Page
    {
        private ADUser _user = new ADUser();
        public UserInfo userInfo = new UserInfo();
        protected void Page_Load(object sender, EventArgs e)
        {
            Page.Form.Attributes.Add("enctype", "multipart/form-data");
            _user = Session["CurrentUser"] as ADUser;
            userInfo = Session["UserInformation"] as UserInfo;

            string[,] testStrings = { { "kyle.bolinger@cwlp.com" }, { "mike.lesko@springfield.il.us" }, { "misty.buscher@springfield.il.us" }, { "alison.warren@cwlp.com" } };
            rpEmails.DataSource = testStrings;
            rpEmails.DataBind();
        }
    }
}