using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;


namespace DataLibrary
{
    public class UserTheme
    {
        public string userTheme;
        public HttpCookie userThemeCookie;
        private static UserTheme _UserTheme;

        public static UserTheme Instance
        {
            get
            {
                if (_UserTheme == null)
                {
                    _UserTheme = new UserTheme();
                }
                return _UserTheme;
            }
        }
        public void GetUserTheme(HttpRequest Request, HtmlElement html, HttpResponse Response)
        {
            if (Request.Cookies["userTheme"] != null)
            {
                userThemeCookie = Request.Cookies["userTheme"];
                userTheme = userThemeCookie.Value;
                userThemeCookie.Expires = DateTime.Now.AddYears(1);

                switch (userTheme)
                {
                    case "dark":
                        html.Attributes.Add("data-bs-theme", "dark");
                        break;

                    case "light":
                        html.Attributes.Add("data-bs-theme", "light");
                        break;
                }
            }
            else
            {
                userThemeCookie = new HttpCookie("userTheme");
                userThemeCookie.Value = "dark";
                html.Attributes.Add("data-bs-theme", "dark");
                userTheme = "dark";
                Response.Cookies.Add(userThemeCookie);
                userThemeCookie.Expires = DateTime.Now.AddYears(1);
            }
        }
    }
}
