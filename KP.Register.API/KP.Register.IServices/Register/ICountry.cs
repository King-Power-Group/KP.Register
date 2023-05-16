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
    public interface ICountry
    {
        [OperationContract]
        ReturnObject<List<KP.Customer.ServiceModels.Country>> GetCountryList(CountryParameter model, string lang = "");

        [OperationContract]
        ReturnObject<bool> validByCountryCode(string CountryCode, string lang = "");
    }
}
