using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using EPiServer;
using EPiServer.Core;
using EPiServer.PlugIn;
using EPiServer.Web;

namespace Knowit.EPiServer.JSONPlugin
{
    public class JSONPlugin : PlugInAttribute
    {
        private const string RenderExtension = "json";
        public static void Start()
        {
            // Add new rewrite event:
            UrlRewriteModuleBase.HttpRewriteInit += UrlRewriteModuleHttpRewriteInit;
        }

        static void UrlRewriteModuleHttpRewriteInit(object sender, UrlRewriteEventArgs e)
        {
            // Before converting to internal URL, check if JSON mode should be enabled
            Global.UrlRewriteProvider.ConvertingToInternal += UrpConvertingToInternal;

            // After URL is converted to internal, check for JSON mode
            Global.UrlRewriteProvider.ConvertedToInternal += UrpConvertedToInternal;
        }

        static void UrpConvertedToInternal(object sender, UrlRewriteEventArgs e)
        {
            if (e.Internal is PageReference)
            {
                var localpath = HttpContext.Current.Server.MapPath(e.Url.Path);
                var rootFolder = HttpContext.Current.Server.MapPath("\\");

                if (e.Url.Uri.PathAndQuery.Contains("jsonPlugin=true"))
                {
                    UrlBuilder ub = new UrlBuilder(e.Url);
                    ub.Path = "/JSONHandler.ashx";
                    e.Url.Uri = ub.Uri;
                }
            }
        }

        static void UrpConvertingToInternal(object sender, UrlRewriteEventArgs e)
        {
            if (e.Url.Path.ToLower().EndsWith("/json") || e.Url.Path.ToLower().EndsWith("/json/"))
            {
                UrlBuilder ub = new UrlBuilder(e.Url);
                Regex rgx = new Regex("[jJ][sS][oO][nN]/?");
                ub.Path = rgx.Replace(ub.Path, string.Empty);
                ub.QueryCollection.Add("jsonPlugin", "true");

                e.Url.Uri = ub.Uri;
            }

        }
    }
}