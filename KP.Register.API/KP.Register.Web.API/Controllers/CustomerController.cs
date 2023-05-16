using KP.Common.Return;
using KP.Customer.ServiceModels;
using KP.Register.Business;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;

namespace KP.Register.Web.API.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [RoutePrefix("api/Customer")]
    [Authorize]
    public class CustomerController : System.Web.Http.ApiController
    {
        // GET: Customer
        public KP.Register.IServices.POS.ITransaction transcation;

        public KP.Customer.DBModels.CustomerDataContext DBCUSD { get; set; }
        public KP.Customer.DBModels.DDSServerDataContext DBDDS { get; set; }

        public CustomerController()
        {
            string customerDataConnStr = ConfigurationManager.ConnectionStrings["CutomerDataConnection"].ConnectionString;
            string ddsDataConnStr = ConfigurationManager.ConnectionStrings["DEVMASTERConnection"].ConnectionString;
            DBCUSD = new Customer.DBModels.CustomerDataContext(customerDataConnStr);
            DBDDS = new KP.Customer.DBModels.DDSServerDataContext(ddsDataConnStr);
            transcation = new CustomerPurchaseBL(DBCUSD);
        }

        [HttpPost]
        [Route("GetSumPurchase")]
        public ReturnObject<OutputTransaction> GetSumPurchase(InputTransaction input)
        {
            ReturnObject<OutputTransaction> ret = new ReturnObject<OutputTransaction>();

            try
            {
                ret = transcation.GetSumPurchase(input);
            }
            catch (Exception e)
            {
                ret.SetMessage(e);
            }

            return ret;
        }
    }
}