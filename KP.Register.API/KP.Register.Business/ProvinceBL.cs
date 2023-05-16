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
    public class ProvinceBL : IProvince
    {

        public KP.Customer.DBModels.CustomerDataContext db { get; set; }

        public ProvinceBL(KP.Customer.DBModels.CustomerDataContext _db)
        {
            db = _db;
        }

        public ReturnObject<CONS_Province> getProvinceByCode(string provinceCode, string subBranchCode = "")
        {
            ReturnObject<CONS_Province> ret = new ReturnObject<CONS_Province>();

            try
            {
                var provinceBL = new KP.Customer.Business.Province.ProvinceBL();
                var result = provinceBL.getProvinceByCode(db, provinceCode);
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

        public ReturnObject<List<CONS_Province>> GetProvinceList(string subBranchCode = "")
        {
            ReturnObject<List<CONS_Province>> ret = new ReturnObject<List<CONS_Province>>();

            try
            {
                var provinceBL = new KP.Customer.Business.Province.ProvinceBL();
                var result = provinceBL.GetProvinceList(db);
                if (result.Data.Count() > 0)
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
