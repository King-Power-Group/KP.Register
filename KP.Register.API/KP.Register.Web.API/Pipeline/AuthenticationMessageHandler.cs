using KP.Register.Web.API.Helpers;
using KP.Register.Web.API.Models;
using System;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace KP.Register.Web.API.Pipeline
{
    public class AuthenticationMessageHandler : DelegatingHandler
    {
        public string Token { get; set; }

        public AuthenticationMessageHandler()
        {

        }

        protected async override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
        {
            GlobalVar.LanguageCode = RequestHeaders.GetLanguage(request.Headers);
            this.CheckAuthorizationHeader(request);

            //if (!this.CheckAuthorizationHeader(request))
            //{
            //    //var respError = new HttpResponseMessage(HttpStatusCode.Unauthorized) {  Content = new StringContent("Your token " + this.Token + " is not allowed")};
            //    //return respError;
            //}
            // Call the inner handler.
            var response = await base.SendAsync(request, cancellationToken);
            return response;
        }

        private bool CheckAuthorizationHeader(HttpRequestMessage message)
        {
            //var query = message.RequestUri.ParseQueryString();
            //string key = query["key"];
            //return (key == Key);

            try
            {
                // get token
                AuthenticationHeaderValue authValue = message.Headers.Authorization;
                if (authValue != null && !String.IsNullOrWhiteSpace(authValue.Parameter))
                {
                    string URL_SERVICE = ConfigurationManager.AppSettings["URL_SERVICE"].ToString();
                    string AuthorizationKey = ConfigurationManager.AppSettings["AuthorizationKey"].ToString();
                    if (authValue.Scheme == AuthenticationMethod.Bearer)
                    {
                        this.Token = authValue.Parameter;

                        // select in database or configuration
                        //var chk = (from t in TokenFileReader.GetAll() where t.Token == this.Token select t).FirstOrDefault();
                        //if (chk != null)
                        //{
                        bool isLoginCaller = false;

                        var headerValues = message.Headers.GetValues(AuthenticationMethod.CallerID);
                        var callerID = headerValues.FirstOrDefault();

                        KP.Caller.ServiceModels.CallerInput model = new Caller.ServiceModels.CallerInput()
                        {
                            authorizeKey = this.Token,
                            callerID = callerID
                        };

                        KP.Common.Helper.CallerBL callerBL = new Common.Helper.CallerBL();
                        var ret = callerBL.LoginCaller(model, URL_SERVICE, AuthorizationKey);
                        isLoginCaller = ret.isCompleted;
                        if (isLoginCaller)
                        {
                            UserIdentity user = new UserIdentity();
                            user.AuthenticationType = AuthenticationMethod.Bearer;
                            user.IsAuthenticated = true;
                            user.Name = ret.Data.FirstOrDefault().callerName;
                            user.Token = authValue.Parameter;
                            user.CallerID = callerID;
                            user.CallerAttributeList = ret.Data;

                            UserPrincipal principal = new UserPrincipal();
                            principal.Identity = user;

                            System.Threading.Thread.CurrentPrincipal = principal;
                            if (HttpContext.Current != null)
                            {
                                HttpContext.Current.User = principal;
                            }
                        }

                        // if has in database then create user model


                        return isLoginCaller;
                        //}
                        //else
                        //{
                        //    return false;
                        //}
                    }
                }
            }
            catch (Exception ex)
            {
                return false;
            }

            return false;
        }
    }
}