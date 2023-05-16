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
    public interface IPlatform
    {
        /// <summary>
        /// </summary>
        /// <param name="platformCode"></param>
        /// <returns></returns>

        [OperationContract]
        ReturnObject<bool> checkPlatformByCode(string platformCode);

        /// <summary>
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        ReturnObject<List<string>> getListPlatform();
    }
}
