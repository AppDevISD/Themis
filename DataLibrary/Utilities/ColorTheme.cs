using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;


namespace DataLibrary
{
    public class ColorTheme
    {
        public string colorTheme;
        public HttpCookie colorThemeCookie;
        private static ColorTheme _ColorTheme;

        public static ColorTheme Instance
        {
            get
            {
                if (_ColorTheme == null)
                {
                    _ColorTheme = new ColorTheme();
                }
                return _ColorTheme;
            }
        }
        public void GetColorTheme(HttpRequest Request, HtmlElement html, HttpResponse Response)
        {
            if (Request.Cookies["colorTheme"] != null)
            {
                colorThemeCookie = Request.Cookies["colorTheme"];
                colorTheme = colorThemeCookie.Value;
                colorThemeCookie.Expires = DateTime.Now.AddYears(1);

                html.Attributes.Add("data-color-theme", colorTheme);
            }
            else
            {
                colorThemeCookie = new HttpCookie("colorTheme");
                colorThemeCookie.Value = html.Attributes["data-color-theme"];
                colorTheme = html.Attributes["data-color-theme"];
                Response.Cookies.Add(colorThemeCookie);
                colorThemeCookie.Expires = DateTime.Now.AddYears(1);
            }
        }

        private const string BaseThemeDirectory = "~/assets/css/";
        private const string ThemeFileName = "Colors.scss";

        public Dictionary<string, string> GetColorThemeColors(HttpServerUtility Server)
        {
            Dictionary<string, string> themeDictionary = new Dictionary<string, string>();
            string filePath = string.Format("{0}{1}", BaseThemeDirectory, ThemeFileName);
            string fileText = File.ReadAllText(Server.MapPath(filePath));
            string[] splitString = fileText.Split(new string[] { "$color-themes:" }, StringSplitOptions.None).ToArray();
            string[] replaceStrings = new string[] { "\"", "(", ")", "\r", "\n", " ", "\u0009" };
            string themeDictString = splitString[1];
            foreach (string item in replaceStrings)
            {
                themeDictString = themeDictString.Replace(item, "");
            }
            string[] colorArray = themeDictString.Split(',');
            foreach (string item in colorArray)
            {
                string[] keyValue = item.Split(':');
                string key = keyValue[0];
                key = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(key.ToLower());
                string value = keyValue[1];
                value = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(value.ToLower());
                themeDictionary.Add(key, value);
            }
            return themeDictionary;
        }
    }
}
