using KP.Common.Return;
using KP.Customer.DBModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KP.Register.IServices.Register
{
    public interface IAgent
    {
        ReturnObject<List<CONS_Agent>> GetAgentList(string subBranchCode = "");
        //ReturnObject<List<df_agentCustomerData>> GetAgentList(string subBranchCode = "");
        ReturnObject<CONS_Agent> getAgentByCode(string agentCode, string subBranchCode = "");
        //ReturnObject<df_agentCustomerData> getAgentByCode(string agentCode, string subBranchCode = "");
    }
}
