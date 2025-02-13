using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Web.Services;
using System.Web.UI.WebControls;
using System.Web.UI;
using DataLibrary;
using System.Web.SessionState;
using System.Net;
using System.Web;

namespace WebUI
{
    /// <summary>
    /// Used to get pending upload files and save them on postback
    /// </summary>
    [System.Web.Script.Services.ScriptService]
    public class FileUploadSaving : System.Web.Services.WebService
    {
        
        public Page uploadPage = new Page();
        public FileUpload supportingDocs = new FileUpload();
        public static FileUploadSaving _FileUploadSaving = null;

        public static FileUploadSaving Instance
        {
            get
            {
                if (_FileUploadSaving == null)
                {
                    _FileUploadSaving = new FileUploadSaving();
                }
                return _FileUploadSaving;
            }
        }

        [WebMethod(EnableSession = true)]
        public void SaveFileOnPostback()
        {
            Instance.uploadPage = HttpContext.Current.Session["fileUploadPage"] as Page;
            Instance.supportingDocs = uploadPage.Page.FindControl("supportingDocumentation") as FileUpload;
            HttpContext.Current.Session["fileUploadControl"] = supportingDocs;
            //Debug.WriteLine(supportingDocs);
        }
    }
}
