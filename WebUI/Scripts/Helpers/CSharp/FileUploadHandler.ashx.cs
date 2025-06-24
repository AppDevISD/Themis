using DataLibrary;
using ISD.ActiveDirectory;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace WebUI
{
    /// <summary>
    /// Summary description for FileUploadHandler
    /// </summary>
    public class FileUploadHandler : IHttpHandler, System.Web.SessionState.IRequiresSessionState
    {
        public void ProcessRequest(HttpContext context)
        {
            ADUser _user = context.Session["CurrentUser"] as ADUser;
            if (_user == null)
            {
                context.Response.StatusCode = 401;
                context.Response.Write("Not logged in.");
                return;
            }

            if (context.Request.Files.Count == 0)
            {
                context.Response.StatusCode = 400;
                context.Response.Write("No files uploaded");
                return;
            }

            List<OrdinanceDocument> ordDocs = context.Session["ordDocs"] as List<OrdinanceDocument> ?? new List<OrdinanceDocument>();
            List<OrdinanceDocument> addOrdDocs = context.Session["addOrdDocs"] as List<OrdinanceDocument> ?? new List<OrdinanceDocument>();

            for (int i = 0; i < context.Request.Files.Count; i++)
            {
                HttpPostedFile uploadedFile = context.Request.Files[i];

                if (uploadedFile?.ContentLength > 0)
                {
                    using (BinaryReader reader = new BinaryReader(uploadedFile.InputStream))
                    {
                        OrdinanceDocument doc = new OrdinanceDocument()
                        {
                            DocumentName = Path.GetFileName(uploadedFile.FileName),
                            DocumentData = reader.ReadBytes(uploadedFile.ContentLength),
                            EffectiveDate = DateTime.Now,
                            ExpirationDate = DateTime.MaxValue,
                            LastUpdateDate = DateTime.Now,
                            LastUpdateBy = _user.Login
                        };

                        ordDocs.Add(doc);
                        addOrdDocs.Add(doc);
                    }
                }
            }

            context.Session["ordDocs"] = ordDocs;
            context.Session["addOrdDocs"] = addOrdDocs;

            context.Response.ContentType = "text/plain";
            context.Response.Write($"{context.Request.Files.Count} files processed and stored");
        }

        public bool IsReusable => false;
    }
}