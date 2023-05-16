using KP.Common.Return;
using KP.Customer.ServiceModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace KP.Register.IServices.Register
{
    [ServiceContract]
    public interface IShoppingCard
    {
        [OperationContract]
        ReturnObject<bool> validPrefixShoppingCard(string prefix);

        [OperationContract]
        ReturnObject<bool> validDuplicatePreRegisterByPassport(KP.Customer.ServiceModels.Personal personal,string callerID,string agentCode,bool isTour=false);

        [OperationContract]
        ReturnObject<List<KP.Customer.ServiceModels.PersonalPos>> getDatafromShoppingCard(ShoppingCardParamAPI param,string pathImageShoppingCard="");
    }
}
