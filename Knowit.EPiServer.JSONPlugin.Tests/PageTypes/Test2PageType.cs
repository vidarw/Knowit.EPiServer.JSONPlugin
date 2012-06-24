using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EPiServer.Core;
using EPiServer.Editor;
using EPiServer.SpecializedProperties;
using PageTypeBuilder;

namespace Knowit.EPiServer.JSONPlugin.Tests.PageTypes
{
    [Serializable]
    [PageType(Name = "[Test] TestPage 2", Filename = "/Default.aspx")]
    public class Test2PageType : TypedPageData
    {
        [PageTypeProperty(Type = typeof(PropertyString))]
        public virtual string Heading
        {
            get;
            set;
        }

        [PageTypeProperty(Type = typeof(PropertyXhtmlString))]
        public virtual string MainBody
        {
            get;
            set;
        }

        [PageTypeProperty(Type = typeof(PropertyXhtmlString))]
        public virtual string SecondaryBody
        {
            get;
            set;
        }
    }
}
