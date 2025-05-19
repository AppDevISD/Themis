using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Microsoft.Ajax.Utilities;
using System.IO;
using System.Data.Common;

namespace WebUI
{
    public partial class GenericError : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["Error"] != null)
            {
                HttpException httpException = (HttpException)Session["Error"];
                StackTrace trace = new StackTrace(httpException.InnerException.GetBaseException(), true);
                int httpCode = httpException?.GetHttpCode() ?? 500;
                StackFrame frame = trace.GetFrames().First(i => !i.GetFileName().IsNullOrWhiteSpace());
                string file = Regex.Replace(frame.GetFileName(), @".*?\\Themis\\", "\\Themis\\");
                int line = frame.GetFileLineNumber();
                int column = frame.GetFileColumnNumber();
                string lineText = frame.GetMethod().ToString();
                Dictionary<string, object> CodeDict = GetErrorLabel(httpCode, $"File: {file}%0ALine ({line}:{column}): {lineText}");

                errorLabel.InnerText = CodeDict["label"].ToString();
                errorMessageLine.InnerHtml = $"File: {file}<br />Line ({line}:{column}): {lineText}";
                errorMessage.InnerHtml = CodeDict["message"].ToString();
            }
            else if (Request.QueryString["err"] != null && Session["Error"] == null)
            {
                int httpCode = Convert.ToInt32(Request.QueryString["err"].ToString());
                Dictionary<string, object> CodeDict = GetErrorLabel(httpCode, "");
                errorLabel.InnerText = CodeDict["label"].ToString();
                errorMessageLine.Visible = false;
                errorMessage.InnerHtml = CodeDict["message"].ToString();
            }
            else
            {
                Response.Redirect("./");
            }
        }
        protected Dictionary<string, object> GetErrorLabel(int code, string error)
        {
            Dictionary<string, object> ret = new Dictionary<string, object>();
            switch (code)
            {
                case 404:
                    ret.Add("label", $"Error {code}: Resource Not Found");
                    ret.Add("message", $"We sincerely apologize, but the page or resource you are looking for cannot be found. This might be due to an outdated link, a typing error in the address, or the file having been moved to a new location.<br /><br />We understand that your time is valuable and are committed to ensuring accessibility for all users. If you require any assistance or need accommodations to access information, please do not hesitate to contact our <a href='mailto:ISDAppDev@cwlp.com?subject=Themis%20Application%20Error&body=%0A%0A%0AError%20Information:%0AError {code}: Resource Not Found' class='support-link'>support team</a>.<br /><br />We are continuously working to improve our application to enhance inclusivity and accessibility in accordance with the latest web accessibility standards. Thank you for your patience and understanding.");
                    break;
                case 500:
                    ret.Add("label", $"Error {code}: Internal Server Error");
                    ret.Add("message", $"We sincerely apologize, but an unexpected error has occurred on our server which prevented your request from being completed. This might be due to a temporary malfunction or maintenance activities. Our team is working diligently to resolve the issue. Thank you for your patience and understanding.<br /><br />We understand that your time is valuable and are committed to ensuring accessibility for all users. If you require any assistance or need accommodations to access information, please do not hesitate to contact our <a href='mailto:ISDAppDev@cwlp.com?subject=Themis%20Application%20Error&body=%0A%0A%0AError%20Information:%0A{error}' class='support-link'>support team</a>.<br /><br />We are continuously working to improve our application to enhance inclusivity and accessibility in accordance with the latest web accessibility standards. Thank you for your patience and understanding.");
                    break;
                case 501:
                    ret.Add("label", $"Error {code}: Resource Not Found");
                    ret.Add("message", $"We sincerely apologize, but our server is currently unable to fulfill your request due to an internal error or configuration issue. This might be due to temporary server overloads, maintenance tasks, or necessary updates. We are aware of the situation and are actively working towards a resolution. We appreciate your understanding and patience during this time.<br /><br />We understand that your time is valuable and are committed to ensuring accessibility for all users. If you require any assistance or need accommodations to access information, please do not hesitate to contact our <a href='mailto:ISDAppDev@cwlp.com?subject=Themis%20Application%20Error&body=%0A%0A%0AError%20Information:%0A{error}' class='support-link'>support team</a>.<br /><br />We are continuously working to improve our application to enhance inclusivity and accessibility in accordance with the latest web accessibility standards. Thank you for your patience and understanding.");
                    break;
            }
            return ret;
        }
    }
}