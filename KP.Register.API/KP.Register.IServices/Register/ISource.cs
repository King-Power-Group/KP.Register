using KP.Common.Return;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace KP.Register.IServices.Register
{
    public interface ISource
    {
        [OperationContract]
        ReturnObject<bool> validSourceCode(string sourceCode);
    }
}
