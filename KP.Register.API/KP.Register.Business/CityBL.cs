using KP.Common.Return;
using KP.Customer.DBModels;
using KP.Register.IServices.Register;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KP.Register.Business
{
    public class CityBL : ICity
    {
        public KP.Customer.DBModels.CustomerDataContext db { get; set; }

        public CityBL(KP.Customer.DBModels.CustomerDataContext _db)
        {
            db = _db;
        }

        public ReturnObject<CONS_City> getCityByCityCode(string city, string province, string subBranchCode = "")
        {
            ReturnObject<CONS_City> ret = new ReturnObject<CONS_City>();

            try
            {
                var cityBL = new KP.Customer.Business.City.CityBL();
                var result = cityBL.getCityByCityCode(db, city,province);
                if (result.Data != null)
                {
                    ret.Data = result.Data;
                }
                if(result.Message.Count()>0)
                {
                    ret.Message = result.Message;
                }
            }
            catch (Exception ex)
            {
                ret.SetMessage(ex);
            }

            return ret;
        }

        public ReturnObject<List<CONS_City>> GetCityList( string subBranchCode = "")
        {
            ReturnObject<List<CONS_City>> ret = new ReturnObject<List<CONS_City>>();

            try
            {
                var cityBL = new KP.Customer.Business.City.CityBL();
                var result = cityBL.GetCityList(db);
                if (result.Data.Count()>0)
                {
                    ret.Data = result.Data;
                }
            }
            catch (Exception ex)
            {
                ret.SetMessage(ex);
            }

            return ret;
        }
    }
}
