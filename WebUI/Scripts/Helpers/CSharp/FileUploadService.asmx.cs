using DataLibrary;
using ISD.ActiveDirectory;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Services;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebUI
{
    /// <summary>
    /// Used to get pending upload files and save them on postback
    /// </summary>
    /// 
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    //[System.Web.Script.Services.ScriptService]
    public class FileUploadService : System.Web.Services.WebService
    {
        [WebMethod(EnableSession = true)]
        public void UploadFile()
        {
            HttpContext context = HttpContext.Current;
            ADUser _user = Session["CurrentUser"] as ADUser;

            if (_user == null)
            {
                context.Response.StatusCode = 401;
                context.Response.Write("User session missing");
                return;
            }

            if (context.Request.Files.Count == 0)
            {
                context.Response.StatusCode = 400;
                context.Response.Write("No files uploaded");
                return;
            }

            List<OrdinanceDocument> ordDocs = Session["addOrdDocs"] as List<OrdinanceDocument> ?? new List<OrdinanceDocument>();

            for (int i = 0; i < context.Request.Files.Count; i++)
            {
                HttpPostedFile uploadedFile = context.Request.Files[i];

                if (uploadedFile?.ContentLength > 0)
                {
                    using (BinaryReader reader = new BinaryReader(uploadedFile.InputStream))
                    {
                        OrdinanceDocument doc = new OrdinanceDocument()
                        {
                            OrdinanceID = -1,
                            DocumentName = Path.GetFileName(uploadedFile.FileName),
                            DocumentData = reader.ReadBytes(uploadedFile.ContentLength),
                            EffectiveDate = DateTime.Now,
                            ExpirationDate = DateTime.MaxValue,
                            LastUpdateDate = DateTime.Now,
                            LastUpdateBy = _user.Login
                        };

                        ordDocs.Add(doc);
                    }
                }
            }

            Session["addOrdDocs"] = ordDocs;

            context.Response.ContentType = "text/plain";
            context.Response.Write($"{context.Request.Files.Count} files processed and stored");
        }
    }
}
