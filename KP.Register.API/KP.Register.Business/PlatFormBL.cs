using KP.Common.Return;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KP.Register.Business
{
    public class PlatFormBL
    {
        public KP.Customer.DBModels.CustomerDataContext db { get; set; }

        public PlatFormBL(KP.Customer.DBModels.CustomerDataContext _db)
        {
            db = _db;
        }

        public ReturnObject<bool> validByPlatForm(string platform, string lang = "")
        {
            ReturnObject<bool> ret = new ReturnObject<bool>();
            ret.Data = false;
            try
            {
                var chkplatform = db.CE_LOVs.Where(a => a.LOVGroupCode == "CUSD_Platform" && a.LOVCode == platform).FirstOrDefault();
                if (chkplatform!=null)
                {
                    ret.Data = true;
                    ret.isCompleted = true;
                    ret.totalCount = 1;
                }
                else
                {
                    ret.SetMessage("Ex-Input", "Invalid PlatForm Code");
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
