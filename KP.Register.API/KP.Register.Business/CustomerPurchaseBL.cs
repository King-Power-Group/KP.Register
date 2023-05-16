using KP.Common.Helper;
using KP.Common.Return;
using KP.Customer.Business.Register;
using KP.Customer.ServiceModels;
using KP.Register.IServices.POS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KP.Register.Business
{
    public class CustomerPurchaseBL : ITransaction
    {

        public KP.Customer.DBModels.CustomerDataContext db { get; set; }

        public CustomerPurchaseBL(KP.Customer.DBModels.CustomerDataContext _db)
        {
            db = _db;
        }

        public ReturnObject<OutputTransaction> GetSumPurchase(InputTransaction input)
        {
            ReturnObject<OutputTransaction> ret = new ReturnObject<OutputTransaction>();

            try
            {
                if(input==null)
                {
                    ret.SetMessage("Ex-Data", "Invalid Input");
                    return ret;
                }

                if(input.branch == null || StringBL.StringNullToEmpty(input.branch)=="")
                {
                    ret.SetMessage("Ex-Data", "Branch is Required.");
                    return ret;
                }

                if (input.shoppingcard == null || StringBL.StringNullToEmpty(input.shoppingcard) =="")
                {
                    ret.SetMessage("Ex-Data", "Shopping Card is Required.");
                    return ret;
                }


                ShoppingCardAPIBL shopcard = new ShoppingCardAPIBL();

                var getSumPurchase = shopcard.getsumPurchase(db, input);

                if(getSumPurchase.Message.Count()>0)
                {
                    ret.Message.AddRange(getSumPurchase.Message);
                    return ret;
                }
                else
                {
                    ret = getSumPurchase;
                }
            }
            catch (Exception e)
            {
                ret.SetMessage(e);
            }

            return ret;
        }
    }
}
