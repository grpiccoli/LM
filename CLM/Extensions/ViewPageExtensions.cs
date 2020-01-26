using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Razor;
using System;
using System.Text;
using System.Text.Encodings.Web;
using System.IO;

namespace CLM.Extensions
{
    public static class ViewPageExtensions
    {
        private const string SCRIPTBLOCK_BUILDER = "ScriptBlockBuilder";

        public static HtmlString ScriptBlock(this RazorPage webPage, Func<dynamic, HelperResult> template)
        {
            var sb = new StringBuilder();
            TextWriter tw = new StringWriter(sb);
            var encoder = (HtmlEncoder)webPage.ViewContext.HttpContext.RequestServices.GetService(typeof(HtmlEncoder));

            if (webPage.Context.Request.Headers["x-requested-with"] != "XMLHttpRequest")
            {
                var scriptBuilder = webPage.Context.Items[SCRIPTBLOCK_BUILDER] as StringBuilder ?? new StringBuilder();

                template.Invoke(null).WriteTo(tw, encoder);
                scriptBuilder.Append(sb.ToString());
                webPage.Context.Items[SCRIPTBLOCK_BUILDER] = scriptBuilder;

                return new HtmlString(string.Empty);
            }

            template.Invoke(null).WriteTo(tw, encoder);

            return new HtmlString(sb.ToString());
        }

        public static HtmlString WriteScriptBlocks(this RazorPage webPage)
        {
            var scriptBuilder = webPage.Context.Items[SCRIPTBLOCK_BUILDER] as StringBuilder ?? new StringBuilder();

            return new HtmlString(scriptBuilder.ToString());
        }
    }
}
