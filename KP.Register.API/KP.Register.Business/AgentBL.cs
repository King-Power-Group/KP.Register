using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KP.Common.Return;
using KP.Customer.DBModels;
using KP.Customer.ServiceModels;
using KP.Register.IServices.Register;

namespace KP.Register.Business
{
    public class AgentBL : IAgent
    {
        public KP.Customer.DBModels.CustomerDataContext db { get; set; }

        public AgentBL(KP.Customer.DBModels.CustomerDataContext _db)
        {
            db = _db;
        }
        public ReturnObject<CONS_Agent> getAgentByCode(string agentCode, string subBranchCode = "")
        {
            ReturnObject<CONS_Agent> ret = new ReturnObject<CONS_Agent>();

            try
            {
                var agentBL = new KP.Customer.Business.Agent.AgentBL();
                var result = agentBL.getAgentByCode(db, agentCode);
                if (result.Data != null)
                {
                    ret.Data = result.Data;
                }

                if (result.Message.Count() > 0)
                {
                    ret.Message = result.Message;
                }
            }
            catch (Exception ex)
            {
                ret.SetMessage(ex);
            }

            return ret;
        }

        public ReturnObject<List<CONS_Agent>> GetAgentList(string subBranchCode = "")
        {
            ReturnObject<List<CONS_Agent>> ret = new ReturnObject<List<CONS_Agent>>();

            try
            {
                var agentBL = new KP.Customer.Business.Agent.AgentBL();
                var result = agentBL.GetAgentList(db);
                if (result.Data.Count() > 0)
                {
                    ret.Data = result.Data;
                }
            }
            catch (Exception ex)
            {
                ret.SetMessage(ex);
            }

            return ret;
        }

        public ReturnObject<string> getCustomerTypeByAgentCode(string agentCode, string subbranch = "")
        {
            ReturnObject<string> ret = new ReturnObject<string>();

            try
            {
                var agentBL = new KP.Customer.Business.Agent.AgentBL();
                var result = agentBL.getCustomerTypeCodeByAgent(db, agentCode, subbranch);
                if (result.Data != null)
                {
                    ret.Data = result.Data.Trim();
                }
            }
            catch (Exception e)
            {
                ret.SetMessage(e);
            }

            return ret;
        }


        public ReturnObject<OutputAgent> getCustomerTypeByAgentCodeDesc(string agentCode, string subbranch = "")
        {
            ReturnObject<OutputAgent> ret = new ReturnObject<OutputAgent>();

            try
            {
                var agentBL = new KP.Customer.Business.Agent.AgentBL();
                var result = agentBL.getCustomerTypeCodeByAgentDesc(db, agentCode, subbranch);
                if (result.Data != null)
                {
                    ret.Data = result.Data;
                }
            }
            catch (Exception e)
            {
                ret.SetMessage(e);
            }

            return ret;
        }
    }
}
