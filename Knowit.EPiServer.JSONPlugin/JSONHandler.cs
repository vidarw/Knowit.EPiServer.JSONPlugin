using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EPiServer;
using EPiServer.Core;
using Newtonsoft.Json;
using PageTypeBuilder;

namespace Knowit.EPiServer.JSONPlugin
{
    public class JSONHandler : IHttpHandler
    {
        public PageData CurrentPage { get; set; }

        public void ProcessRequest(HttpContext context)
        {
            var pr = new PageReference(context.Request["id"]);
            if (PageReference.IsNullOrEmpty(pr))
            {
                WriteError(context, "Could not get EPiServer context: Invalid page reference");
                return;
            }

            var pd = DataFactory.Instance.GetPage(pr);
            if (pd == null)
            {
                WriteError(context, "Could not get EPiServer context: Could not fetch page data object");
                return;
            }
            
            CurrentPage = pd;
            var pageType = PageTypeResolver.Instance.GetPageTypeType(CurrentPage.PageTypeID);
            
            if (!Attribute.IsDefined(pageType, typeof(SerializableAttribute)))
            {
                WriteError(context, "Access denied: Serializable attribute not set");
                return;
            }

            var dict = new Dictionary<string, string>();
            foreach (var p in pageType.GetProperties())
            {
                if (CurrentPage.Property[p.Name] != null && !CurrentPage.Property[p.Name].IsNull)
                {
                    dict.Add(CurrentPage.Property[p.Name].Name, HttpUtility.HtmlEncode(CurrentPage.Property[p.Name].Value.ToString()));
                }
            }

                WriteError(context, JsonConvert.SerializeObject(dict));

            //WriteResponse(context);
        }

        protected void WriteResponse(HttpContext context)
        {
            context.Response.ClearContent();
            //context.Response.ContentType = "application/json";

            context.Response.Write("Name: " + CurrentPage.PageName);
        }

        protected void WriteError(HttpContext context, string message)
        {
            context.Response.ClearContent();
            //context.Response.ContentType = "application/json";

            context.Response.Write(message);
        }

        public bool IsReusable
        {
            get { return false; }
        }
    }
}