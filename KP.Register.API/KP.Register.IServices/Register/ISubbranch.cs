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
    public interface ISubbranch
    {
        [OperationContract]
        ReturnObject<List<KP.Customer.ServiceModels.SubBranch>> getListSubbranch(KeySearchParameter model, string lang = "");
        [OperationContract]
        Customer.ServiceModels.SubBranch getSubbranchBySubbranchCode();
        [OperationContract]
        ReturnObject<bool> validSubBranchCode(string subbrachCode);
    }
}
