using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Routing;
using Microsoft.AspNet.FriendlyUrls;

namespace WebUI
{
    public static class RouteConfig
    {
        //public static void RegisterRoutes(RouteCollection routes)
        //{
        //    var settings = new FriendlyUrlSettings();
        //    settings.AutoRedirectMode = RedirectMode.Permanent;
        //    RouteFolder(routes, "~/Pages");
        //    routes.EnableFriendlyUrls(settings);
        //}
        //public static void RouteFolder(RouteCollection routes, string folder)
        //{
        //    string rootFolder = HttpContext.Current.Server.MapPath("~/");

        //    if (folder.StartsWith("~/"))
        //    { }
        //    else if (folder.StartsWith("/"))
        //    {
        //        folder = $"~{folder}";
        //    }
        //    else
        //    {
        //        folder = $"~/{folder}";
        //    }

        //    folder = HttpContext.Current.Server.MapPath(folder);
        //    MapFolderRoute(routes, folder, rootFolder);
        //}

        //static void MapFolderRoute(RouteCollection routes, string folder, string rootFolder)
        //{
        //    string[] folders = Directory.GetDirectories(folder);

        //    foreach (var subFolder in folders)
        //    {
        //        MapFolderRoute(routes, subFolder, rootFolder);
        //    }

        //    string[] files = Directory.GetFiles(folder);

        //    foreach (var file in files)
        //    {
        //        if (!file.EndsWith(".aspx"))
        //            continue;

        //        string webPath = file.Replace(rootFolder, "~/").Replace("\\", "/");

        //        var filename = Path.GetFileNameWithoutExtension(file);

        //        if (filename.ToLower() == "default")
        //        {
        //            continue;
        //        }
        //        routes.MapPageRoute(filename, filename, webPath);
        //    }
        //}
    }
}
