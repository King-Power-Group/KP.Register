using KP.Common.Return;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace KP.Register.IServices.Register
{
    [ServiceContract]
    public interface ICustomerType
    {
        [OperationContract]
        ReturnObject<bool> validCustomerTypeCode(string custTypeCode);
    }
}
