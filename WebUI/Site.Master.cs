using DataLibrary;
using DataLibrary.OrdinanceTracking;
using ISD.ActiveDirectory;
using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Services.Description;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using static DataLibrary.Utility;

namespace WebUI
{
    public partial class SiteMaster : System.Web.UI.MasterPage
    {
        private ADUser _user = new ADUser();
        private UserInfo userInfo = new UserInfo();
        public string PageTitle;
        public string toastColor;
        public string toastMessage;
        public string hideAdmin;

        protected void Page_Init(object sender, EventArgs e)
        {
            if (Session["CurrentUser"] == null)
            {
                _user = AuthenticateUser();
                imgUser.Src = Photo.Instance.Base64ImgSrc(_user.PhotoLocation);
                Session["CurrentUser"] = _user;

                List<ADGroups> aDGroups = ISDFactory.Instance.GetAllGroupsByLoginName(_user.Login);
                int employeeID = Convert.ToInt32(_user.EmployeeID.TrimStart());
                if (Session["UserInformation"] == null)
                {
                    userInfo = new UserInfo()
                    {
                        UserFirstName = _user.FirstName,
                        UserLastName = _user.LastName,
                        UserDisplayName = $"{_user.FirstName} {_user.LastName}",
                        UserEmail = _user.Email,
                        IsAdmin = aDGroups.Any(i => i.GroupName.Equals("PG-THEMIS-ADMIN")),
                        UserView = false,
                        UserDepartment = GetUserDepartment(employeeID.ToString()),
                        UserDivision = GetUserDivision(employeeID.ToString())
                    };
                    Session["UserInformation"] = userInfo;
                }
                Session["ImpersonateUser"] = false;
            }
            
            if (!Page.IsPostBack && !Response.IsRequestBeingRedirected)
            {
                RouteConfig.FolderRedirect(Response, Page);
            }

            GetUser();
        }
        protected void Page_PreRender(object sender, EventArgs e)
        {
            UserTheme.Instance.GetUserTheme(Request, html, Response);
            if (Page.IsPostBack && Page.Request.Params.Get("__EVENTTARGET").Contains("adminSwitch"))
            {
                if (!SettingsMenu.Attributes["class"].Contains("show"))
                {
                    SettingsMenu.Attributes["class"] += " show";
                }
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            SetPageTitle();
            SetStartupActives();
            if (Request.QueryString["err"] == null && Session["Error"] != null && !Response.IsRequestBeingRedirected)
            {
                Session.Remove("Error");
            }

            SubmitStatus();
        }
        public void GetUser()
        {
            _user = (ADUser)Session["CurrentUser"];
            List<ADGroups> aDGroups = ISDFactory.Instance.GetAllGroupsByLoginName(_user.Login);
            Session["UserName"] = _user.Login;
            

            userInfo = (UserInfo)Session["UserInformation"];
            if (userInfo.IsAdmin)
            {
                adminSwitch.Checked = userInfo.UserView;
            }
            
            Session["UserInformation"] = userInfo;
            string userName = _user.Login.ToUpper();
            string userDisplayName = $"{_user.FirstName} {_user.LastName}";
            lblUser.Text = userDisplayName;
            lblDepartment.Text = userInfo.UserDepartment.DepartmentName;
            lblDivision.Text = userInfo.UserDivision.DivisionName;
            imgUser.Src = Photo.Instance.Base64ImgSrc(_user.PhotoLocation);
        }
        protected void SetPageTitle()
        {
            string ProjectName = "THΣMIS";
            string BlankSpace = string.Join("", Enumerable.Repeat("&nbsp;", 3));
            switch (Page.Title)
            {
                default:
                    PageTitle = Page.Title.Replace(" ", string.Empty).ToLower();
                    Page.Title = $"{ProjectName} | {Page.Title}";
                    break;
                case null:
                case "":
                    string fileName = new FileInfo(Page.Request.Url.LocalPath).Name;
                    string pageTitle = string.Concat(fileName.Select(x => Char.IsUpper(x) ? " " + x : x.ToString())).TrimStart(' ');
                    PageTitle = pageTitle.Replace(" ", string.Empty).ToLower();
                    Page.Title = $"{ProjectName} | {PageTitle}";
                    break;
            }
        }
        public bool ActivePage(string pageTitle)
        {
            bool activePage = false;
            List<string> pageTitles = pageTitle.Split(';').Where(i => !i.IsNullOrWhiteSpace()).ToList();
                switch (pageTitles.Any(i => PageTitle.ToLower().Contains(i.Replace(" ", string.Empty).ToLower())))
                {
                    case true:
                        activePage = true;
                        break;
                    case false:
                        activePage = false;
                        break;
                }

            return activePage;
        }
        protected void SetStartupActives()
        {
            _user = (ADUser)Session["CurrentUser"];
            List<ADGroups> aDGroups = ISDFactory.Instance.GetAllGroupsByLoginName(_user.Login);
            ordAdmin.Visible = aDGroups.Any(i => i.GroupName.Equals("PG-THEMIS-ADMIN"));
            ordAdmin.Attributes.Add("data-active-page", ActivePage("OrdinanceAdmin") ? "activePage" : "");
            adminSwitchDiv.Visible = aDGroups.Any(i => i.GroupName.Equals("PG-THEMIS-ADMIN"));
            appDevToolsParent.Visible = aDGroups.Any(i => i.GroupName.Equals("DG-PublicUtilities-InformationSystems-AppDev")) || (bool)Session["ImpersonateUser"];
            ImpersonateUser.Visible = aDGroups.Any(i => i.GroupName.Equals("DG-PublicUtilities-InformationSystems-AppDev"));
            StopImpersonate.Visible = (bool)Session["ImpersonateUser"];
            TriggerError.Visible = aDGroups.Any(i => i.GroupName.Equals("DG-PublicUtilities-InformationSystems-AppDev"));

            DefaultEmails defaultList = Factory.Instance.GetByID<DefaultEmails>(107, "sp_GetDefaultEmailByDefaultEmailsID", "DefaultEmailsID");
            string[] emails = defaultList.EmailAddress.ToString().Split(';').Where(i => !i.IsNullOrWhiteSpace()).ToArray();

            if (emails.Length > 0)
            {
                lblNoItemsTesting.Visible = false;
                rpTestingDefaultList.DataSource = emails.OrderBy(i => i);
                rpTestingDefaultList.DataBind();
            }
            else
            {
                lblNoItemsTesting.Visible = true;
                rpTestingDefaultList.DataSource = null;
                rpTestingDefaultList.DataBind();
            }
        }
        protected void adminSwitch_CheckedChanged(object sender, EventArgs e)
        {
            if (Request.Path.ToLower().Contains("/default.aspx"))
            {
                Session["UserInformation"] = UserView();
            }            
        }
        public UserInfo UserView()
        {
            UserInfo ret = new UserInfo();
            userInfo.UserView = adminSwitch.Checked;
            ordAdmin.Visible = !userInfo.UserView;
            ret = userInfo;
            return ret;
        }
        protected void lnkInactivityRefresh_Click(object sender, EventArgs e)
        {
            //FOR INACTIVE REFRESHING - DO NOT REMOVE
        }
        protected void TriggerError_Click(object sender, EventArgs e)
        {
            int errorCode = Convert.ToInt32(txtErrorCode.Text);
            throw new HttpException(errorCode, $"Error");
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

        protected void btnImpersonateUser_Click(object sender, EventArgs e)
        {
            Session["ImpersonateUser"] = true;
            Session.Remove("CurrentUser");
            Session.Remove("UserInformation");
            ADUser impersonateUser = new ADUser();

            impersonateUser = AuthenticateUser(txtImpersonateUser.Text);
            imgUser.Src = Photo.Instance.Base64ImgSrc(impersonateUser.PhotoLocation);
            Session["CurrentUser"] = impersonateUser;

            List<ADGroups> aDGroups = ISDFactory.Instance.GetAllGroupsByLoginName(impersonateUser.Login);
            int employeeID = Convert.ToInt32(impersonateUser.EmployeeID.TrimStart());
            if (Session["UserInformation"] == null)
            {
                userInfo = new UserInfo()
                {
                    UserFirstName = impersonateUser.FirstName,
                    UserLastName = impersonateUser.LastName,
                    UserDisplayName = $"{impersonateUser.FirstName} {impersonateUser.LastName}",
                    UserEmail = impersonateUser.Email,
                    IsAdmin = aDGroups.Any(i => i.GroupName.Equals("PG-THEMIS-ADMIN")),
                    UserView = false,
                    UserDepartment = GetUserDepartment(employeeID.ToString()),
                    UserDivision = GetUserDivision(employeeID.ToString())
                };
                Session["UserInformation"] = userInfo;
            }
            Response.Redirect(Request.RawUrl);
        }

        protected void StopImpersonate_Click(object sender, EventArgs e)
        {
            Session["ImpersonateUser"] = false;
            Session.Remove("CurrentUser");
            Session.Remove("UserInformation");
            Response.Redirect(Request.RawUrl);
        }

        protected void DeleteOrd_Click(object sender, EventArgs e)
        {
            int ret = Factory.Instance.Delete<Ordinance>(Convert.ToInt32(txtOrdID.Text), "Ordinance");
            if (ret > 0)
            {
                Session["SubmitStatus"] = "success";
                Session["ToastColor"] = "text-bg-success";
                Session["ToastMessage"] = "Entry Deleted!";
                Response.Redirect(Request.RawUrl);
            }
        }

        protected void AddTestingEmailAddress_Click(object sender, EventArgs e)
        {
            DefaultEmails defaultList = Factory.Instance.GetByID<DefaultEmails>(107, "sp_GetDefaultEmailByDefaultEmailsID", "DefaultEmailsID");
            List<string> emails = new List<string>();
            if (!defaultList.ToString().IsNullOrWhiteSpace())
            {
                emails = defaultList.EmailAddress.ToString().Split(';').Where(i => !i.IsNullOrWhiteSpace()).ToList();
            }

            string[] newEmailAddresses = testingEmailAddress.Text.Split(';').Where(i => !i.IsNullOrWhiteSpace()).ToArray();
            foreach (string item in newEmailAddresses)
            {
                emails.Add(item.ToLower());
            }
            string setEmails = string.Join(";", emails.OrderBy(i => i));
            defaultList.EmailAddress = setEmails.ToLower();

            int updateDefaultEmails = Factory.Instance.Update(defaultList, "sp_UpdateDefaultEmail");
            if (updateDefaultEmails > 0)
            {
                testingEmailAddress.Text = string.Empty;
                if (emails.Count > 0)
                {
                    lblNoItemsTesting.Visible = false;
                    rpTestingDefaultList.DataSource = emails.OrderBy(i => i);
                    rpTestingDefaultList.DataBind();
                }
                else
                {
                    lblNoItemsTesting.Visible = true;
                    rpTestingDefaultList.DataSource = null;
                    rpTestingDefaultList.DataBind();
                }
            }
        }
        protected void rpTestingList_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            DefaultEmails defaultList = Factory.Instance.GetByID<DefaultEmails>(107, "sp_GetDefaultEmailByDefaultEmailsID", "DefaultEmailsID");
            List<string> emails = defaultList.EmailAddress.ToString().Split(';').Where(i => !i.IsNullOrWhiteSpace()).ToList();

            emails.Remove(e.CommandArgument.ToString());

            string setEmails = string.Join(";", emails.OrderBy(i => i));
            defaultList.EmailAddress = setEmails.ToLower();
            int updateDefaultEmails = Factory.Instance.Update(defaultList, "sp_UpdateDefaultEmail");
            if (updateDefaultEmails > 0)
            {
                if (emails.Count > 0)
                {
                    lblNoItemsTesting.Visible = false;
                    rpTestingDefaultList.DataSource = emails.OrderBy(i => i);
                    rpTestingDefaultList.DataBind();
                }
                else
                {
                    lblNoItemsTesting.Visible = true;
                    rpTestingDefaultList.DataSource = null;
                    rpTestingDefaultList.DataBind();
                }
            }
        }
        protected void rpTestingList_ItemCreated(object sender, RepeaterItemEventArgs e)
        {
            LinkButton btn = (LinkButton)e.Item.FindControl("removeBtn");
            ScriptManager.GetCurrent(Page).RegisterAsyncPostBackControl(btn);
        }
    }
}