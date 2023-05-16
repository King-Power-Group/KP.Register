using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Web;

namespace KP.Register.Web.API.Helpers
{
    public static class RequestHeaders
    {
        public static string GetLanguage(HttpRequestHeaders Headers)
        {
            string ret = null;
            try
            {
                IEnumerable<string> headerValues = Headers.GetValues("X-Custom-Language");
                ret = headerValues.FirstOrDefault();

                if (string.IsNullOrEmpty(ret))
                {
                    ret = "EN";
                }
            }
            catch (Exception ex)
            {
                ret = "EN";
            }

            return ret;
        }
    }

    public static class GlobalVar
    {
        public static string LanguageCode { get; set; }
    }

    public static class AuthenticationMethod
    {
        public const string Bearer = "Bearer";
        public const string Basic = "Basic";
        public const string CallerID = "CallerID";
    }
}