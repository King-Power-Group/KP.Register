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
    public class ShoppingCardBL : IShoppingCard
    {

        public KP.Customer.DBModels.CustomerDataContext db { get; set; }

        public ShoppingCardBL(KP.Customer.DBModels.CustomerDataContext _db)
        {
            db = _db;
        }

        public ReturnObject<bool> validPrefixShoppingCard(string prefix)
        {
            ReturnObject<bool> ret = new ReturnObject<bool>();
            try
            {
                var ShoppingCardBL = new KP.Customer.Business.Register.ShoppingCardAPIBL();
                ret = ShoppingCardBL.validPrefixShoppingCard(db, prefix);
            }
            catch (Exception ex)
            {
                ret.SetMessage(ex);
            }

            return ret;
            //throw new NotImplementedException();
        }

        public ReturnObject<bool> validDuplicatePreRegisterByPassport(Personal personal, string callerID, string agentCode, bool isTour)
        {
            ReturnObject<bool> ret = new ReturnObject<bool>();

            try
            {
                var shoppingcardBL = new Customer.Business.Register.ShoppingCardAPIBL();
                ret = shoppingcardBL.validDuplicatePreRegisterByPassport(db, personal, callerID, agentCode, isTour);
            }
            catch (Exception ex)
            {
                ret.SetMessage(ex);
            }

            return ret;
        }

        public ReturnObject<List<PersonalPos>> getDataPickup(ShoppingCardParamAPI param, string pathImageShoppingCard = "")
        {
            ReturnObject<List<PersonalPos>> ret = new ReturnObject<List<PersonalPos>>();

            try
            {
                var shoppingcardBl = new Customer.Business.Register.ShoppingCardAPIBL();
                var data = shoppingcardBl.getCustomerByPickupCode(db, param.shoppingCard,param.SubBranch, pathImageShoppingCard);
                if(data.Message.Count()==0)
                {
                    ret.Data = data.Data;
                }
            }
            catch (Exception e)
            {
                ret.SetMessage(e);
            }

            return ret;
        }

        public ReturnObject<bool> CheckShoppingCardisActive(ShoppingCardParamAPI param)
        {
            ReturnObject<bool> ret = new ReturnObject<bool>();
            ret.Data = false;
            try
            {
                var shoppingcardBl = new Customer.Business.Register.ShoppingCardAPIBL();
                ret = shoppingcardBl.CheckShoppingCardisActive(db, param.shoppingCard, param.SubBranch);
            }
            catch (Exception e)
            {
                ret.SetMessage("Ex-input", e.Message);
            }

            return ret;
        }

        public ReturnObject<List<PersonalPos>> getDatafromShoppingCard(ShoppingCardParamAPI param, string pathImageShoppingCard = "")
        {
            ReturnObject<List<PersonalPos>> ret = new ReturnObject<List<PersonalPos>>();

            try
            {
                var shoppingcardBl = new Customer.Business.Register.ShoppingCardAPIBL();
                eService.DBModels.MemberDataContext dbMember;
                if (KP.Common.Helper.StringBL.StringNullToEmpty(param.SubBranch) != "")
                {
                    //ret = shoppingcardBl.getCustomerByShoppingCard(db, param.shoppingCard, param.SubBranch, param.isTour, pathImageShoppingCard,param.platform,param.isGenPdfPromotion);
                    ret = shoppingcardBl.getCustomerShoppingCard(db, param.shoppingCard, param.SubBranch, param.isTour, pathImageShoppingCard, param.platform, param.isGenPdfPromotion);
                }
                else
                {
                    ret = shoppingcardBl.getCustomerByShoppingCard(db, param.shoppingCard, param.isTour);
                }


                if (ret.Message.Where(a=>a.MessageType=="ERROR").Count() == 0)
                {
                    ret.isCompleted = true;
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
