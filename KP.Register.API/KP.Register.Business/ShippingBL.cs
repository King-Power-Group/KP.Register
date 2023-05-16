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
    public class ShippingBL : IShipping
    {
        public KP.Customer.DBModels.CustomerDataContext db { get; set; }

        public ShippingBL(KP.Customer.DBModels.CustomerDataContext _db)
        {
            db = _db;
        }

        public ReturnObject<OutputShipping> getShipping(string sessionID)
        {
            ReturnObject<OutputShipping> ret = new ReturnObject<OutputShipping>();

            try
            {
                var shippingBL = new KP.Customer.Business.Shipping.ShippingBL();
                ret = shippingBL.GetShipping(db, sessionID);
            }
            catch (Exception ex)
            {
                ret.SetMessage(ex);
            }
            return ret;
        }


        public ReturnObject<string> getShippingString(string sessionID)
        {
            ReturnObject<string> ret = new ReturnObject<string>();
            try
            {
                var shippingBL = new KP.Customer.Business.Shipping.ShippingBL();
                ret = shippingBL.GetShippingString(db, sessionID);
            }
            catch (Exception ex)
            {
                ret.SetMessage(ex);
            }
            return ret;
        }
    }
}
