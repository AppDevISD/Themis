using DataLibrary;
using ISD.ActiveDirectory;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web.UI;
using System.Diagnostics;
using static DataLibrary.Utility;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Web.Services.Description;

namespace WebUI
{
    public partial class SiteMaster : System.Web.UI.MasterPage
    {
        private ADUser _user = new ADUser();
        private UserInfo userInfo = new UserInfo();
        public string PageTitle;

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
                        UserDepartmentID = Factory.Instance.GetUserDepartmentID(employeeID.ToString())
                    };
                    Dictionary<string, string> departments = DepartmentsList();
                    foreach (var department in departments.Keys)
                    {
                        var value = departments[department];
                        ListItem newItem = new ListItem(department, value);
                        if (newItem.Value == userInfo.UserDepartmentID.ToString())
                        {
                            userInfo.UserDepartmentName = newItem.Text;
                        }
                    }
                    Session["UserInformation"] = userInfo;
                }
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
            lblTitle.Text = userInfo.UserDepartmentName;
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
                    Page.Title = $"{ProjectName} | {Page.Title}";
                    break;
            }
        }
        public bool ActivePage(string pageTitle)
        {
            bool activePage = false;
            switch (PageTitle.Contains(pageTitle.Replace(" ", string.Empty).ToLower()))
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
            adminSwitchDiv.Visible = aDGroups.Any(i => i.GroupName.Equals("PG-THEMIS-ADMIN"));
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
            ret = userInfo;
            return ret;
        }
        protected void lnkInactivityRefresh_Click(object sender, EventArgs e)
        {
            //FOR INACTIVE REFRESHING - DO NOT REMOVE
        }
    }
}