using KP.Register.API.Helpers;
using KP.Register.API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;

namespace KP.Register.API.Middleware
{
    public class VerifyTokenSenderMiddleWare
    {
        private readonly RequestDelegate next;
        private IPathProvider _pathProvider;

        public VerifyTokenSenderMiddleWare(RequestDelegate next, IPathProvider pathProvider)
        {
            this.next = next;
            _pathProvider = pathProvider;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                var header = context.Request.Headers.Where(x => x.Key == AuthenticationKey.Authorization);
                if (header.Count() == 1)
                {
                    var authHeader = header.First().Value.First();
                    string scheme = authHeader.Split(' ')[0];
                    if (scheme == AuthenticationMethod.Bearer)
                    {
                        TokenFileReader _readToken = new TokenFileReader(_pathProvider);
                        string token = authHeader.Split(' ')[1];
                        var chk = (from t in _readToken.GetAll() where t.Token == token select t).FirstOrDefault();
                        if (chk != null)
                        {
                            UserIdentity userIdent = new UserIdentity();
                            userIdent.AuthenticationType = AuthenticationMethod.Bearer;
                            userIdent.IsAuthenticated = true;
                            userIdent.Name = chk.Name;
                            userIdent.Token = token;

                            UserPrincipal principal = new UserPrincipal();
                            principal.Identity = userIdent;

                            string[] Roles = { "superadmin" };
                            var user = new GenericPrincipal(principal.Identity, Roles);
                            if (user != null)
                            {
                                context.User = user;
                            }
                        }
                        else
                        {
                            context.Response.StatusCode = (Int32)HttpStatusCode.Unauthorized; //Unauthorized                
                            await context.Response.WriteAsync("Invalid authentication key");
                            return;
                        }
                    }
                    else
                    {
                        context.Response.StatusCode = (Int32)HttpStatusCode.Unauthorized; //UnAuthorized                
                        await context.Response.WriteAsync("Invalid authentication scheme");
                        return;
                    }
                }
                await next.Invoke(context);

            }
            catch (Exception ex)
            {
                context.Response.StatusCode = (Int32)HttpStatusCode.BadRequest; //Bad Request                
                await context.Response.WriteAsync("Error : " + ex.Message);
                return;
            }
        }
    }
}
