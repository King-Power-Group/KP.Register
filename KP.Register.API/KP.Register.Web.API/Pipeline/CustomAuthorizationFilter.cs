using KP.Register.Web.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace KP.Register.Web.API.Pipeline
{
    public class CustomAuthorizationFilter : AuthorizationFilterAttribute
    {
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            if (System.Threading.Thread.CurrentPrincipal as UserPrincipal == null)
            {
                this.OnUnauthorizedResponse();
            }
            else
            {
                this.SetPrincipal(System.Threading.Thread.CurrentPrincipal as Models.UserPrincipal);
            }

            base.OnAuthorization(actionContext);
        }

        private void SetPrincipal(Models.UserPrincipal principal)
        {
            if (HttpContext.Current != null)
            {
                HttpContext.Current.User = principal;
            }
        }

        private void OnUnauthorizedResponse()
        {
            throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.Unauthorized) { Content = new StringContent("Access denied because token is not allowed") });
        }
    }
}