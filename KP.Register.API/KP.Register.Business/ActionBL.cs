using KP.Common.Return;
using KP.Register.IServices.Register;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KP.Register.Business
{
    public class ActionBL : IAction
    {
        public KP.Customer.DBModels.CustomerDataContext db { get; set; }

        public ActionBL(KP.Customer.DBModels.CustomerDataContext _db)
        {
            db = _db;
        }

        public ReturnObject<bool> validActionbyActionCode(string actionCode,string lang="")
        {
            ReturnObject<bool> ret = new ReturnObject<bool>();

            try
            {
                var actionBL = new KP.Customer.Business.Action.ActionBL();
                ret = actionBL.validActionbyActionCode(db, actionCode);

            }
            catch (Exception ex) 
            {
                ret.SetMessage(ex);
            }

            return ret;
        }

        public ReturnObject<bool> validByCountryCode(string CountryCode)
        {
            ReturnObject<bool> ret = new ReturnObject<bool>();
            ret.Data = false;
            try
            {
                var db = CommonClass.GetDbContext();
                var countryBL = new KP.Customer.Business.Country.CountryBL();

                ret = countryBL.validateCountry(db, CountryCode);
            }
            catch (Exception ex)
            {
                ret.SetMessage("Ex01", ex.Message);
            }

            return ret;
        }
    }
}
