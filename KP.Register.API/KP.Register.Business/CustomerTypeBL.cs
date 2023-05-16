using KP.Common.Return;
using KP.Register.IServices.Register;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KP.Register.Business
{
    public class CustomerTypeBL : ICustomerType
    {
        public KP.Customer.DBModels.CustomerDataContext db { get; set; }

        public CustomerTypeBL(KP.Customer.DBModels.CustomerDataContext _db)
        {
            db = _db;
        }

        public ReturnObject<bool> validCustomerTypeCode(string custTypeCode)
        {
            ReturnObject<bool> ret = new ReturnObject<bool>();

            try
            {
                var customerTypeBL = new KP.Customer.Business.CustomerType.CustomerTypeCodeBL();
                ret = customerTypeBL.validCustTypeCode(db, custTypeCode.ToUpper());
            }
            catch (Exception ex)
            {
                ret.SetMessage(ex);
            }

            return ret;
        }
    }
}
