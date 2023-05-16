using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KP.Register.Web.API.Helpers
{
    public static class LanguagesCode
    {
        public const string English = "EN";
        public const string Thai = "TH";
        public const string Chinese = "CH";
        public const string Japan = "JP";
    }
    public static class LanguageHelper
    {
        public static string GetLanguageFromUri(HttpRequest request)
        {
            System.Web.Http.Routing.IHttpRouteData[] routes = request.RequestContext.RouteData.Values["MS_SubRoutes"] as System.Web.Http.Routing.IHttpRouteData[];
            string langCode = LanguagesCode.English;
            try
            {
                langCode = routes[0].Values["lang"].ToString().ToUpper();
            }
            catch (Exception)
            {
                langCode = LanguagesCode.English;
            }

            return langCode;
        }
    }
}