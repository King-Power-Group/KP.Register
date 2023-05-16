using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace KP.Register.API.Helpers
{
    public static class LanguageCode
    {
        public const string English = "EN";
        public const string Thai = "TH";
        public const string Chinese = "CH";
        public const string Japan = "JP";
    }
    public class LanguageHelper : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            try
            {
                context.ActionArguments["langCode"] = context.RouteData.Values["lang"].ToString().ToUpper();
                base.OnActionExecuting(context);
            }
            catch (Exception)
            {
                context.ActionArguments["langCode"] = LanguageCode.English;
                base.OnActionExecuting(context);
            }            
        }
    }
}
