using KP.Common.Return;
using KP.Customer.ServiceModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace KP.Register.IServices.Register
{
    [ServiceContract]
    public interface IRegister
    {
        /// <summary>
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>

        [OperationContract]
        ReturnObject<List<KP.Customer.ServiceModels.Output>> RegisterAPI(List<Customer.ServiceModels.Register> model, List<KP.Common.Helper.CallerModel> CallerAttributeList, string lang = "",string posCon="");

        [OperationContract]
        ReturnObject<ActivatPOS> ActivateAPI(ActivatPOS model, string lang = "");

        [OperationContract]
        ReturnObject<List<PersonalPos>> GetCustomer(ShoppingCardParamAPI model, string lang = "",string pathImageShoppingCard="");
        //ReturnObject<List<KP.Customer.ServiceModels.Personal>> ActivateAPI(List<Customer.ServiceModels.Personal> model);


        [OperationContract]
        ReturnObject<ModelTour> RegisterTour(ModelTour tour, string lang = "");

        [OperationContract]
        ReturnObject<List<OutputKiosk>> RegisterKiosk(List<Customer.ServiceModels.KioskModel> model, List<KP.Common.Helper.CallerModel> CallerAttributeList, string lang = "", string posCon = "",string pathImageShoppingCard="");

        [OperationContract]
        ReturnObject<List<SettingConfigs>> GetSettingConfig();

        [OperationContract]
        ReturnObject<OutputAgent> GetCustomerTypeByAgentCodeDesc(InputAgent data);

        [OperationContract]
        ReturnObject<bool> CheckValidSVCode(InputValidSubAgent scCode);
    }
}
