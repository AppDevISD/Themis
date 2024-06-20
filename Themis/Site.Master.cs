using DataLibrary;
using ISD.ActiveDirectory;
using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Timers;
using System.Web;
using System.Web.Optimization;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Windows.Forms;

namespace Themis
{
    public partial class SiteMaster : System.Web.UI.MasterPage
    {
        private ADUser _user = new ADUser();
        private string userName;
        private string userDisplayName;
        private string userPosition;
        private string userTheme;
        private string userColor;
        private string currentPageString;
        private Page currentPageObj;
        private HtmlAnchor currentPage;
        public HttpCookie userThemeCookie;
        public HttpCookie userColorCookie;
        LiteralControl lightModeStyle = new LiteralControl(ColorSchemes.Instance.BlueLight());
        LiteralControl darkModeStyle = new LiteralControl(ColorSchemes.Instance.BlueDark());


        private bool colorSwitcher = false;

        protected void Page_Init(object sender, EventArgs e)
        {
            if (Session["CurrentUser"] == null)
            {
                _user = Utility.Instance.AuthenticateUser();
                Session["CurrentUser"] = _user;
                imgUser.Src = PhotoBase64ImgSrc(_user.PhotoLocation);
                //HomepageRedirection();
            }
        }
        protected void Page_PreRender(object sender, EventArgs e)
        {
            switch (colorSwitcher)
            {
                case true:
                    GetUserColor();
                    ColorSwitcherNav.Visible = true;
                    break;

                case false:
                    GetUserTheme();
                    ColorSwitcherNav.Visible = false;
                    break;
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                _user = (ADUser)Session["CurrentUser"];
                Session["UserName"] = _user.Login;
                userName = _user.Login.ToUpper();
                userDisplayName = $"{_user.FirstName}&nbsp; {_user.LastName}";
                userPosition = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(_user.Title.ToLower());
                lblUser.Text = userDisplayName;
                lblTitle.Text = userPosition;
                imgUser.Src = PhotoBase64ImgSrc(_user.PhotoLocation);
            }
            currentPageObj = (Page)HttpContext.Current.Handler;
            currentPageString = currentPageObj.Title;
            currentPageString = currentPageString.Replace(" ", string.Empty);
            currentPage = (HtmlAnchor)Page.Master.FindControl(currentPageString);

            switch (currentPage)
            {
                default:
                    string currentPageClassString = currentPage.Attributes["class"];
                    string[] currentPageClassList = currentPageClassString.Split(' ');
                    string currentPageGroup = currentPageClassList[0];
                    currentPage.Attributes.Add("class", $"{currentPageClassString} active");
                    HtmlGenericControl currentPageParent = (HtmlGenericControl)Page.Master.FindControl($"{currentPageGroup}Menu");
                    if (currentPageParent != null)
                    {
                        currentPageParent.Attributes.Add("class", "menu-open");
                    }
            break;
                case null:
                    currentPageString = "TITLE HAS NOT BEEN SET";
                    currentPageObj.Title = currentPageString;
                    Debug.WriteLine($"\n{currentPageString}");
                    break;
            }
        }
        protected string PhotoBase64ImgSrc(string fileNameandPath)
        {
            try
            {
                byte[] byteArray = System.IO.File.ReadAllBytes(fileNameandPath);
                string base64 = Convert.ToBase64String(byteArray);

                return string.Format("data:image/gif;base64,{0}", base64);
            }
            catch
            {
                return "\\assets\\images\\ImageNotAvailable.png";
            }
        }
        protected void GetUserTheme()
        {

            if (Request.Cookies["userTheme"] != null)
            {
                userThemeCookie = Request.Cookies["userTheme"];
                userTheme = userThemeCookie.Value;
                userThemeCookie.Expires = DateTime.Now.AddYears(1);

                switch (userTheme)
                {
                    case "darkmode":
                        colorModeToggle.Attributes.Remove("class");
                        colorModeToggle.Attributes.Add("class", "fas fa-sun");
                        Page.Header.Controls.Remove(lightModeStyle);
                        Page.Header.Controls.Add(darkModeStyle);
                        userThemeCookie.Value = "darkmode";
                        Response.Cookies.Set(userThemeCookie);
                        break;

                    case "lightmode":
                        colorModeToggle.Attributes.Remove("class");
                        colorModeToggle.Attributes.Add("class", "fas fa-moon");
                        Page.Header.Controls.Remove(darkModeStyle);
                        Page.Header.Controls.Add(lightModeStyle);
                        userThemeCookie.Value = "lightmode";
                        Response.Cookies.Set(userThemeCookie);
                        break;
                }
            }
            else
            {
                userThemeCookie = new HttpCookie("userTheme");
                userThemeCookie.Value = "lightmode";
                colorModeToggle.Attributes.Remove("class");
                colorModeToggle.Attributes.Add("class", "fas fa-moon");
                Response.Cookies.Add(userThemeCookie);
                userThemeCookie.Expires = DateTime.Now.AddYears(1);
            }
        }
        protected void colorModeToggle_ServerClick(object sender, EventArgs e)
        {
            userThemeCookie = Request.Cookies["userTheme"];
            userTheme = userThemeCookie.Value;
            userThemeCookie.Expires = DateTime.Now.AddYears(1);

            switch (userTheme)
            {
                case "lightmode":
                    colorModeToggle.Attributes.Remove("class");
                    colorModeToggle.Attributes.Add("class", "fas fa-sun");
                    Page.Header.Controls.Remove(lightModeStyle);
                    Page.Header.Controls.Add(darkModeStyle);
                    userThemeCookie.Value = "darkmode";
                    Response.Cookies.Set(userThemeCookie);
                    break;

                case "darkmode":
                    colorModeToggle.Attributes.Remove("class");
                    colorModeToggle.Attributes.Add("class", "fas fa-moon");
                    Page.Header.Controls.Remove(darkModeStyle);
                    Page.Header.Controls.Add(lightModeStyle);
                    userThemeCookie.Value = "lightmode";
                    Response.Cookies.Set(userThemeCookie);
                    break;
            }

            Response.Redirect(HttpContext.Current.Request.Url.AbsoluteUri);
        }













        //COLOR SCHEME SWITCHER
        protected void GetUserColor()
        {

            if (Request.Cookies["userColor"] != null)
            {
                userColorCookie = Request.Cookies["userColor"];
                userColor = userColorCookie.Value;
                userColorCookie.Expires = DateTime.Now.AddYears(1);
            }
            else
            {
                userColorCookie = new HttpCookie("userColor");
                userColorCookie.Value = "baseTheme";
                userColor = userColorCookie.Value;
                Response.Cookies.Add(userColorCookie);
                userColorCookie.Expires = DateTime.Now.AddYears(1);
            }

            switch (userColor)
            {
                case "baseTheme":
                    lightModeStyle = new LiteralControl(ColorSchemes.Instance.BaseLight());
                    darkModeStyle = new LiteralControl(ColorSchemes.Instance.BaseDark());
                    GetUserTheme();
                    break;

                case "redTheme":
                    lightModeStyle = new LiteralControl(ColorSchemes.Instance.RedLight());
                    darkModeStyle = new LiteralControl(ColorSchemes.Instance.RedDark());
                    GetUserTheme();
                    break;

                case "orangeTheme":
                    lightModeStyle = new LiteralControl(ColorSchemes.Instance.OrangeLight());
                    darkModeStyle = new LiteralControl(ColorSchemes.Instance.OrangeDark());
                    GetUserTheme();
                    break;

                case "yellowTheme":
                    lightModeStyle = new LiteralControl(ColorSchemes.Instance.YellowLight());
                    darkModeStyle = new LiteralControl(ColorSchemes.Instance.YellowDark());
                    GetUserTheme();
                    break;

                case "greenTheme":
                    lightModeStyle = new LiteralControl(ColorSchemes.Instance.GreenLight());
                    darkModeStyle = new LiteralControl(ColorSchemes.Instance.GreenDark());
                    GetUserTheme();
                    break;

                case "blueTheme":
                    lightModeStyle = new LiteralControl(ColorSchemes.Instance.BlueLight());
                    darkModeStyle = new LiteralControl(ColorSchemes.Instance.BlueDark());
                    GetUserTheme();
                    break;

                case "cyanTheme":
                    lightModeStyle = new LiteralControl(ColorSchemes.Instance.CyanLight());
                    darkModeStyle = new LiteralControl(ColorSchemes.Instance.CyanDark());
                    GetUserTheme();
                    break;

                case "purpleTheme":
                    lightModeStyle = new LiteralControl(ColorSchemes.Instance.PurpleLight());
                    darkModeStyle = new LiteralControl(ColorSchemes.Instance.PurpleDark());
                    GetUserTheme();
                    break;
            }

            userColorCookie.Value = userColor;
            Response.Cookies.Set(userColorCookie);
        }
        protected void Theme_ServerClick(object sender, EventArgs e)
        {
            HtmlButton btn = (HtmlButton)sender;
            string themeSwitch = btn.ID.ToString();
            userColorCookie = Request.Cookies["userColor"];
            userColorCookie.Expires = DateTime.Now.AddYears(1);

            switch (themeSwitch)
            {
                case "baseTheme":
                    lightModeStyle = new LiteralControl(ColorSchemes.Instance.BaseLight());
                    darkModeStyle = new LiteralControl(ColorSchemes.Instance.BaseDark());
                    GetUserTheme();
                    break;

                case "redTheme":
                    lightModeStyle = new LiteralControl(ColorSchemes.Instance.RedLight());
                    darkModeStyle = new LiteralControl(ColorSchemes.Instance.RedDark());
                    GetUserTheme();
                    break;

                case "orangeTheme":
                    lightModeStyle = new LiteralControl(ColorSchemes.Instance.OrangeLight());
                    darkModeStyle = new LiteralControl(ColorSchemes.Instance.OrangeDark());
                    GetUserTheme();
                    break;

                case "purpleTheme":
                    lightModeStyle = new LiteralControl(ColorSchemes.Instance.PurpleLight());
                    darkModeStyle = new LiteralControl(ColorSchemes.Instance.PurpleDark());
                    GetUserTheme();
                    break;

            }
            userColorCookie.Value = themeSwitch;
            Response.Cookies.Set(userColorCookie);
            Response.Redirect(HttpContext.Current.Request.Url.AbsoluteUri);
        }
    }
}