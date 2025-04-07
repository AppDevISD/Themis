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

namespace WebUI
{
    public partial class SiteMaster : System.Web.UI.MasterPage
    {
        private ADUser _user = new ADUser();
        public string PageTitle;

        protected void Page_Init(object sender, EventArgs e)
        {
            if (Session["CurrentUser"] == null)
            {
                _user = Utility.Instance.AuthenticateUser();
                Session["CurrentUser"] = _user;
                imgUser.Src = Photo.Instance.Base64ImgSrc(_user.PhotoLocation);
            }
            if (!Page.IsPostBack && !Response.IsRequestBeingRedirected)
            {
                RouteConfig.FolderRedirect(Response, Page);
            }
        }
        protected void Page_PreRender(object sender, EventArgs e)
        {
            UserTheme.Instance.GetUserTheme(Request, html, Response);
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            SetPageTitle();
            if (!Page.IsPostBack)
            {
                _user = (ADUser)Session["CurrentUser"];
                Session["UserName"] = _user.Login;
                UserInfo userInfo = new UserInfo()
                {
                    UserFirstName = _user.FirstName,
                    UserLastName = _user.LastName,
                    UserDisplayName = $"{_user.FirstName} {_user.LastName}",
                    UserEmail = _user.Email,
                    UserDepartmentID = Factory.Instance.GetUserDepartmentID(_user.Email)
                };
                Dictionary<string, string> departments = Utility.Instance.DepartmentsList();
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
                string userName = _user.Login.ToUpper();
                string userDisplayName = $"{_user.FirstName} {_user.LastName}";
                //string userPosition = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(_user.Title.ToLower());
                lblUser.Text = userDisplayName;
                lblTitle.Text = _user.Department;
                imgUser.Src = Photo.Instance.Base64ImgSrc(_user.PhotoLocation);
            }
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
    }
}