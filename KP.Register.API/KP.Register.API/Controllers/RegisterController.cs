using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KP.Common.Return;
using KP.Register.API.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace KP.Register.API.Controllers
{
    [Produces("application/json")]
    [Route("api/{lang}")]
    [Authorize]
    public class RegisterController : Controller
    {
        private KP.Register.IServices.Register.IRegister _register;
        public static readonly LanguageHelper Data = new LanguageHelper();

        public RegisterController(KP.Register.IServices.Register.IRegister register)
        {
            _register = register;
        }

        [HttpPost]
        [Route("RegisterAPI")]
        [LanguageHelper]
        public ReturnObject<List<Customer.ServiceModels.Personal>> RegisterAPI(string langCode, [FromBody]List<Customer.ServiceModels.Personal> param)
        {
            ReturnObject<List<Customer.ServiceModels.Personal>> ret = new ReturnObject<List<Customer.ServiceModels.Personal>>();            
            try
            {                
         //       ret = this._register.RegisterAPI(param);
            }
            catch (Exception ex)
            {
                ret.SetMessage(ex);
            }

            return ret;
        }
    }
}