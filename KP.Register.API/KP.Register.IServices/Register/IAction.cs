using KP.Common.Return;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KP.Register.IServices.Register
{
    public interface IAction
    {
        ReturnObject<bool> validActionbyActionCode(string actionCode,string lang="");
    }

}
