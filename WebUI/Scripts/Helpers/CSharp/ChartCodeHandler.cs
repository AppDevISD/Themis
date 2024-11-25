using DataLibrary;
using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Xml.Linq;

namespace WebUI
{
    public class ChartCodeHandler
    {
        public static ChartCodeHandler _ChartCodeHandler = null;

        public static ChartCodeHandler Instance
        {
            get
            {
                if (_ChartCodeHandler == null)
                {
                    _ChartCodeHandler = new ChartCodeHandler();
                }
                return _ChartCodeHandler;
            }
        }

        public void SetChartCode(HttpServerUtility Server, HtmlGenericControl codeDiv, string chartKey, string languageKey)
        {
            string jsonFile = Server.MapPath("~/Scripts/ChartCode/ChartHTMLJS.json");
            codeDiv.InnerHtml = JSONKey(jsonFile, chartKey, languageKey);
        }

        public void SetHandlerCode(HttpServerUtility Server, HtmlGenericControl codeDiv)
        {
            string chartHandlerFile = Server.MapPath("~/Scripts/Helpers/CSharp/ChartHandler.asmx.cs");
            codeDiv.InnerHtml = GetCodeFromFile(chartHandlerFile, "csharp");
        }

        protected string JSONKey(string file, string chartKey, string languageKey)
        {
            if (!File.Exists(file))
            {
                return "<span style='color: red;'>File not found.</span>";
            }

            string readCode = File.ReadAllText(file);
            Dictionary<string, Dictionary<string, string>> fileDict = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(readCode);
            string chartDictKey = JsonConvert.SerializeObject(fileDict[chartKey]);
            Dictionary<string, string> codeDict = JsonConvert.DeserializeObject<Dictionary<string, string>>(chartDictKey);
            string codeValue = codeDict[languageKey];
            string formattedCode = System.Web.HttpUtility.HtmlEncode(codeValue);
            return $"<pre class='codeNoClip'><code class='language-{languageKey}' data-prismjs-copy='Copy'>{formattedCode}</code></pre>";
        }

        protected string GetCodeFromFile(string file, string languageKey)
        {
            if (!File.Exists(file))
            {
                return "<span style='color: red;'>File not found.</span>";
            }

            string code = File.ReadAllText(file);
            string formattedCode = System.Web.HttpUtility.HtmlEncode(code);

            return $"<pre class='codeClip'><code class='language-{languageKey}' data-prismjs-copy='Copy'>{formattedCode}</code></pre>";
        }
    }
}