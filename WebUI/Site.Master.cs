using DataLibrary;
using ISD.ActiveDirectory;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Routing;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace WebUI
{
    public partial class SiteMaster : System.Web.UI.MasterPage
    {
        private ADUser _user = new ADUser();
        //public string userName;
        //public string userDisplayName;
        //public string userPosition;
        public string PageTitle;

        protected void Page_Init(object sender, EventArgs e)
        {
            if (Session["CurrentUser"] == null)
            {
                //_user = Utility.Instance.AuthenticateUser();
                //Session["CurrentUser"] = _user;
                //imgUser.Src = Photo.Instance.Base64ImgSrc(_user.PhotoLocation);
            }
            if (!Page.IsPostBack)
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
                //_user = (ADUser)Session["CurrentUser"];
                //Session["UserName"] = _user.Login;
                //string userName = _user.Login.ToUpper();
                //string userDisplayName = $"{_user.FirstName}&nbsp; {_user.LastName}";
                //string userPosition = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(_user.Title.ToLower());
                //lblUser.Text = userDisplayName;
                //lblTitle.Text = userPosition;
                //imgUser.Src = Photo.Instance.Base64ImgSrc(_user.PhotoLocation);
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
                    Page.Title = $"{ProjectName}{BlankSpace}|{BlankSpace}{Page.Title}";
                    break;
                case null:
                case "":
                    string fileName = new FileInfo(Page.Request.Url.LocalPath).Name;
                    string pageTitle = string.Concat(fileName.Select(x => Char.IsUpper(x) ? " " + x : x.ToString())).TrimStart(' ');
                    PageTitle = pageTitle.Replace(" ", string.Empty).ToLower();
                    Page.Title = $"{ProjectName}{BlankSpace}|{BlankSpace}{pageTitle}";
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