using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KP.Register.API.Helpers
{
    public class RequestHeaders
    {
    }
    public static class AuthenticationMethod
    {
        public const string Bearer = "Bearer";
        public const string Basic = "Basic";
    }
    public static class AuthenticationKey
    {
        public const string Authorization = "Authorization";
    }
}
