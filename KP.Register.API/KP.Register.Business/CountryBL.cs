using KP.Common.Return;
using KP.Customer.ServiceModels;
using KP.Register.IServices.Register;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KP.Register.Business
{
    public class CountryBL : ICountry
    {
        public KP.Customer.DBModels.CustomerDataContext db { get; set; }

        public CountryBL(KP.Customer.DBModels.CustomerDataContext _db)
        {
            db = _db;
        }

        public ReturnObject<List<Country>> GetCountryList(CountryParameter model, string lang = "")
        {
            ReturnObject<List<Country>> ret = new ReturnObject<List<Country>>();
            try
            {
                if (model.pageNo <= 0 && (model.pageSize <= 0 || model.pageSize > 100))
                {
                    model.pageNo = 1;
                    model.pageSize = 100;
                }

                var countryBL = new KP.Customer.Business.Country.CountryBL();
                var objData = countryBL.GetCountryList(db);
                if (objData.Data.Count() > 0)
                {
                    if (model.countryCode != "")
                    {
                        objData.Data = objData.Data.Where(m => m.CountryCode.ToUpper().Contains(model.countryCode.ToUpper())
                        || m.CountryName.ToUpper().Contains(model.countryCode.ToUpper())).ToList();
                    }
                    var data = objData.Data.Skip((model.pageNo - 1) * model.pageSize).Take(model.pageSize).ToList();
                    ret.Data = data;
                    ret.totalCount = data.Count();
                    ret.isCompleted = true;
                }
                else
                {
                    ret = objData;
                    ret.isCompleted = false;
                }
            }
            catch (Exception ex)
            {
                ret.isCompleted = false;
                ret = new ReturnObject<List<Country>>();
                ret.SetMessage("API305", ex.Message);
            }
            return ret;
        }

        public ReturnObject<bool> validByCountryCode(string CountryCode, string lang = "")
        {
            ReturnObject<bool> ret = new ReturnObject<bool>();
            ret.Data = false;
            try
            {
                var countryBL = new KP.Customer.Business.Country.CountryBL();

                ret = countryBL.validateCountry(db, CountryCode);
                if (ret.Data) {
                    ret.isCompleted = true;
                    ret.totalCount = 1;
                }
            }
            catch (Exception ex)
            {
                ret.SetMessage("Ex01", ex.Message);
            }

            return ret;
        }

    }
}
