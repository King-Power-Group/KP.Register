using KP.Common.Return;
using KP.Register.IServices.Register;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KP.Register.Business
{
    public class SourceBL : ISource
    {
        public KP.Customer.DBModels.CustomerDataContext db { get; set; }

        public SourceBL(KP.Customer.DBModels.CustomerDataContext _db)
        {
            db = _db;
        }

        public ReturnObject<bool> validSourceCode(string sourceCode)
        {
            ReturnObject<bool> ret = new ReturnObject<bool>();

            try
            {
                var sourcebl = new KP.Customer.Business.Source.SourceBL();
                ret = sourcebl.validSourceBySourceCode(db, sourceCode);
            }
            catch (Exception ex)
            {
                ret.SetMessage(ex);
            }

            return ret;
        }
    }
}
