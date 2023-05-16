using KP.Common.Return;
using KP.Customer.DBModels;
using KP.Customer.ServiceModels;
using KP.Register.Web.API.Helpers;
using KP.Register.Web.API.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Configuration;
using System.Web.Http;
using System.Web.Http.Cors;

namespace KP.Register.Web.API.Controllers
{
    /// <summary>
    /// Register api
    /// </summary>
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [RoutePrefix("api/Register")]
    [Authorize]
    public class RegisterController : ApiController
    {
        public KP.Register.IServices.Register.IRegister register;
        public KP.Register.IServices.Register.ICountry nationality;
        public KP.Register.IServices.Register.ISubbranch subbranch;
        public KP.Register.IServices.Register.IShipping shipping;

        public KP.Customer.DBModels.CustomerDataContext DBCUSD { get; set; }
        public KP.Customer.DBModels.DDSServerDataContext DBDDS { get; set; }
        public UserIdentity userIdent { get; set; }
        private string langCode { get; set; }
        public RegisterController()
        {
            string customerDataConnStr = ConfigurationManager.ConnectionStrings["CutomerDataConnection"].ConnectionString;
            string ddsDataConnStr = ConfigurationManager.ConnectionStrings["DEVMASTERConnection"].ConnectionString;
            DBCUSD = new Customer.DBModels.CustomerDataContext(customerDataConnStr);
            DBDDS = new KP.Customer.DBModels.DDSServerDataContext(ddsDataConnStr);
            langCode = LanguageHelper.GetLanguageFromUri(System.Web.HttpContext.Current.Request);
            register = new KP.Register.Business.RegisterBL(DBCUSD, DBDDS);
            nationality = new KP.Register.Business.CountryBL(DBCUSD);
            userIdent = HttpContext.Current.User.Identity as UserIdentity;
            subbranch = new KP.Register.Business.SubbranchBL(DBCUSD);
            shipping = new KP.Register.Business.ShippingBL(DBCUSD);
        }

        [HttpPost]
        [Route("RegisterAPI")]
        [Route("~/api/{lang}/Register/RegisterAPI")]
        public ReturnObject<List<KP.Customer.ServiceModels.Output>> RegisterAPI(List<KP.Customer.ServiceModels.Register> param, string lang = "")
        {
            ReturnObject<List<KP.Customer.ServiceModels.Output>> ret = new ReturnObject<List<KP.Customer.ServiceModels.Output>>();
            try
            {
                if (userIdent != null)
                {
                    //string posConString = "";
                    //if(param.Count()==1 && KP.Common.Helper.StringBL.StringNullToEmpty(param.FirstOrDefault().subBranchCode)!="")
                    //{
                    //    posConString = ConfigurationManager.ConnectionStrings[param.FirstOrDefault().subBranchCode].ConnectionString;
                    //}
                    //else
                    //{
                    //    var subBranch = param.Where(q=>q.subBranchCode!="").Select(a => a.subBranchCode).FirstOrDefault();
                    //    posConString = ConfigurationManager.ConnectionStrings[subBranch].ConnectionString;
                    //}

                    ret = register.RegisterAPI(param, userIdent.CallerAttributeList, lang);
                }
            }
            catch (Exception ex)
            {
                ret.SetMessage(ex);
            }

            return ret;
        }

        [HttpPost]
        [Route("ActivateAPI")]
        [Route("~/api/{lang}/Register/ActivateAPI")]
        public ReturnObject<ActivatPOS> ActivateAPI(ActivatPOS model, string lang = "")
        {
            ReturnObject<ActivatPOS> ret = new ReturnObject<ActivatPOS>();
            try
            {
                if (userIdent != null && userIdent.CallerAttributeList.Count > 0)
                {
                    if (String.IsNullOrEmpty(model.caller.agentCode))
                    {
                        model.caller.agentCode = "";
                        var agentCode = userIdent.CallerAttributeList.Where(n => n.attributeName == Common.Caller.CallerAttribute.AgentCode).FirstOrDefault();
                        if (agentCode != null)
                        {
                            model.caller.agentCode = agentCode.valueString;
                        }

                        model.caller.agentCode = userIdent.CallerAttributeList.Where(n => n.attributeName == Common.Caller.CallerAttribute.AgentCode).FirstOrDefault().valueString;
                    }
                    if (String.IsNullOrEmpty(model.caller.subAgentCode))
                    {
                        model.caller.subAgentCode = "";
                        var subAgentCode = userIdent.CallerAttributeList.Where(n => n.attributeName == Common.Caller.CallerAttribute.SubAgentCode).FirstOrDefault();
                        if (subAgentCode != null)
                        {
                            model.caller.subAgentCode = subAgentCode.valueString;
                        }
                    }
                    if (String.IsNullOrEmpty(model.caller.sourceCode))
                    {
                        model.caller.sourceCode = "";
                        var sourceCode = userIdent.CallerAttributeList.Where(n => n.attributeName == Common.Caller.CallerAttribute.SourceCode).FirstOrDefault();
                        if (sourceCode != null)
                        {
                            model.caller.sourceCode = sourceCode.valueString;
                        }
                    }
                }
                ret = register.ActivateAPI(model, lang);
            }
            catch (Exception ex)
            {
                ret.SetMessage(ex);
            }
            return ret;
        }

        [HttpPost]
        [Route("GetCustomer")]
        [Route("~/api/{lang}/Register/GetCustomer")]
        public ReturnObject<List<PersonalPos>> GetCustomer(ShoppingCardParamAPI model, string lang = "")
        {
            ReturnObject<List<PersonalPos>> ret = new ReturnObject<List<PersonalPos>>();
            try
            {
                string pathImageShoppingCard = WebConfigurationManager.AppSettings["pathImageShoppingCard"];
                ret = register.GetCustomer(model, lang, pathImageShoppingCard);
            }
            catch (Exception ex)
            {
                ret.SetMessage(ex);
            }
            return ret;
        }

        [HttpPost]
        [Route("GetNationality")]
        [Route("~/api/{lang}/Register/GetNationality")]
        public ReturnObject<List<Country>> GetNationality(CountryParameter model)
        {
            ReturnObject<List<Country>> ret = new ReturnObject<List<Country>>();
            try
            {
                ret = nationality.GetCountryList(model);
            }
            catch (Exception ex)
            {
                ret.SetMessage(ex);
            }
            return ret;
        }

        [HttpPost]
        [Route("ValidNationalityByCode")]
        [Route("~/api/{lang}/Register/ValidNationalityByCode")]
        public ReturnObject<bool> ValidNationalityByCode(CountryParameter model, string lang = "")
        {
            ReturnObject<bool> ret = new ReturnObject<bool>();
            try
            {
                ret = nationality.validByCountryCode(model.countryCode, lang);
            }
            catch (Exception ex)
            {
                ret.SetMessage(ex);
            }
            return ret;
        }

        [HttpPost]
        [Route("GetListSubbranch")]
        [Route("~/api/{lang}/Register/GetListSubbranch")]
        public ReturnObject<List<SubBranch>> GetListSubbranch(KeySearchParameter model, string lang = "")
        {
            ReturnObject<List<SubBranch>> ret = new ReturnObject<List<SubBranch>>();
            try
            {
                ret = subbranch.getListSubbranch(model, lang);
            }
            catch (Exception ex)
            {
                ret.SetMessage(ex);
            }
            return ret;
        }


        [HttpPost]
        [Route("RegisterTour")]
        public ReturnObject<ModelTour> RegisterTour (ModelTour tour, string lang="")
        {
            ReturnObject<ModelTour> ret = new ReturnObject<ModelTour>();
            try
            {
                ret = register.RegisterTour(tour); 
            }
            catch (Exception e)
            {
                ret.SetMessage(e);
            }


            return ret;
        }


        [HttpPost]
        [Route("KioskRegister")]
        public ReturnObject<List<OutputKiosk>> KioskRegister(List<KioskModel> input,string lang="")
        {
            ReturnObject<List<OutputKiosk>> ret = new ReturnObject<List<OutputKiosk>>();
            try
            {
                if (userIdent != null)
                {
                    //string posConString = "";
                    //if(param.Count()==1 && KP.Common.Helper.StringBL.StringNullToEmpty(param.FirstOrDefault().subBranchCode)!="")
                    //{
                    //    posConString = ConfigurationManager.ConnectionStrings[param.FirstOrDefault().subBranchCode].ConnectionString;
                    //}
                    //else
                    //{
                    //    var subBranch = param.Where(q=>q.subBranchCode!="").Select(a => a.subBranchCode).FirstOrDefault();
                    //    posConString = ConfigurationManager.ConnectionStrings[subBranch].ConnectionString;
                    //}
                    string pathImageShoppingCard = WebConfigurationManager.AppSettings["pathImageShoppingCard"];
                    ret = register.RegisterKiosk(input, userIdent.CallerAttributeList, lang,"", pathImageShoppingCard);
                }
            }
            catch (Exception e)
            {
                ret.SetMessage(e);
            }

            return ret;
        }

        [HttpPost]
        [Route("~/api/{lang}/Register/GetSettings")]
        public ReturnObject<List<SettingConfigs>> GetSettings()
        {
            ReturnObject<List<SettingConfigs>> ret = new ReturnObject<List<SettingConfigs>>();

            try
            {
                ret = register.GetSettingConfig();
            }
            catch (Exception e)
            {
                ret.SetMessage(e);
            }

            return ret;
        }

        [HttpPost]
        [Route("GetCustomerTypeByAgentCode")]
        public ReturnObject<OutputAgent> GetCustomerTypeByAgentCode(InputAgent data)
        {
            ReturnObject<OutputAgent> ret = new ReturnObject<OutputAgent>();
            try
            {
                ret = register.GetCustomerTypeByAgentCodeDesc(data);
            }
            catch (Exception e)
            {
                ret.SetMessage(e);
            }


            return ret;
        }


        //[HttpPost]
        //[Route("GetShippingBySessionID")]
        //public ReturnObject<OutputShipping> GetShippingValue(string sessionID)
        //{
        //    ReturnObject<OutputShipping> ret = new ReturnObject<OutputShipping>();
        //    try
        //    {
        //        ret = shipping.getShipping(sessionID);
        //    }
        //    catch (Exception e)
        //    {
        //        ret.SetMessage(e);
        //    }

        //    return ret;
        //}

        [HttpPost]
        [Route("GetShippingBySessionID")]
        public ReturnObject<string> GetShippingStringValue(string sessionID)
        {
            ReturnObject<string> ret = new ReturnObject<string>();
            try
            {
                ret = shipping.getShippingString(sessionID);
            }
            catch (Exception e)
            {
                ret.SetMessage(e);
            }

            return ret;
        }


        [HttpPost]
        [Route("ValidSVCode")]
        public ReturnObject<bool> CheckValidSVCode(InputValidSubAgent param)
        {
            ReturnObject<bool> ret = new ReturnObject<bool>();

            try
            {
                ret = register.CheckValidSVCode(param);
            }
            catch (Exception ex)
            {
                ret.SetMessage(ex);
            }

            return ret;
        }
    }

}
