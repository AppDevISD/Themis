using Microsoft.AspNet.FriendlyUrls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Web.Routing;
using System.Web.UI.WebControls.WebParts;

namespace WebUI
{
    public static class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            var settings = new FriendlyUrlSettings();
            settings.AutoRedirectMode = RedirectMode.Off;
            RouteFolder(routes, "~/Pages");
            routes.MapPageRoute("Default", "./", "~/Default.aspx");
            routes.EnableFriendlyUrls(settings);
        }
        public static void RouteFolder(RouteCollection routes, string folder)
        {
            string rootFolder = HttpContext.Current.Server.MapPath("~/");

            if (folder.StartsWith("~/"))
            { }
            else if (folder.StartsWith("/"))
            {
                folder = $"~{folder}";
            }
            else
            {
                folder = $"~/{folder}";
            }

            folder = HttpContext.Current.Server.MapPath(folder);
            MapFolderRoute(routes, folder, rootFolder);
        }
        static void MapFolderRoute(RouteCollection routes, string folder, string rootFolder)
        {
            string[] folders = Directory.GetDirectories(folder);

            foreach (var subFolder in folders)
            {
                MapFolderRoute(routes, subFolder, rootFolder);
            }

            string[] files = Directory.GetFiles(folder);

            foreach (var file in files)
            {
                if (!file.EndsWith(".aspx"))
                    continue;

                string webPath = file.Replace(rootFolder, "~/").Replace("\\", "/");

                var filename = Path.GetFileNameWithoutExtension(file);

                if (filename.ToLower() == "default")
                {
                    continue;
                }
                routes.MapPageRoute(filename, filename, webPath);
            }
        }
        public static void FolderRedirect(HttpResponse Response, Page Page)
        {
            string fileName = new FileInfo(Page.Request.Url.LocalPath).Name;
            if (Page.Request.FilePath.ToLower().Contains("pages") || fileName.ToLower() == "default")
            {
                Response.RedirectToRoute(fileName);
            }
        }

    }
}
