using System;
using System.Collections.Generic;
using System.Web.UI.HtmlControls;

namespace WebUI
{
    public partial class Charts : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            SetBaseChartCode();
        }

        protected void SetBaseChartCode()
        {
            List<HtmlGenericControl> codeList = new List<HtmlGenericControl>()
            {
                lineChartHTMLCode,
                lineChartJSCode,
                lineChartHelpersCode,
                barChartHTMLCode,
                barChartJSCode,
                barChartHelpersCode,
            };
            List<HtmlGenericControl> handlerCodeList = new List<HtmlGenericControl>()
            {
                lineChartHandlerCode,
                barChartHandlerCode,
            };

            foreach (HtmlGenericControl control in codeList)
            {
                string chartKey = control.Attributes["data-chart-key"];
                string languageKey = control.Attributes["data-language-key"];
                ChartCodeHandler.Instance.SetChartCode(Server, control, chartKey, languageKey);
            }
            foreach (HtmlGenericControl control in handlerCodeList)
            {
                ChartCodeHandler.Instance.SetHandlerCode(Server, control);
            }

        }
    }
}