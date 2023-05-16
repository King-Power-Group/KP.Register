using KP.Common.Return;
using KP.Customer.DBModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace KP.Register.IServices.Register
{
    [ServiceContract]
    public interface ILocation
    {
        [OperationContract]
        ReturnObject<List<CE_LOV>> GetLocationList();
    }
}
