using KP.Common.Return;
using KP.Customer.DBModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KP.Register.IServices.Register
{
    public interface ISubAgent
    {
        ReturnObject<List<CONS_SubAgent>> GetSubAgentList(string agentCode, string subBranchCode = "");

        ReturnObject<CONS_SubAgent> getSubAgentByCode( string agentCode, string subAgentCode, string subBranchCode = "");
    }
}
