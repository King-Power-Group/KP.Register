using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;

namespace KP.Register.Web.API.Models
{
    public class UserPrincipal : IPrincipal
    {
        public IIdentity Identity { get; set; }

        public bool IsInRole(string role)
        {
            return true;
        }
    }

    public class UserIdentity : IIdentity
    {
        public string Name { get; set; }
        public string AuthenticationType { get; set; }
        public bool IsAuthenticated { get; set; }
        public string Token { get; set; }
        public string CallerID { get; set; }
        public List<KP.Common.Helper.CallerModel> CallerAttributeList { get; set; }
    }
}