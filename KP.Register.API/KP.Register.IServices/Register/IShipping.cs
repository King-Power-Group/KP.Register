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
    public interface IShipping
    {
        [OperationContract]
        ReturnObject<OutputShipping> getShipping(string sessionID);

        [OperationContract]
        ReturnObject<string> getShippingString(string sessionID);
    }
}
