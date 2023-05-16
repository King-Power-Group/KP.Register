using KP.Common.Return;
using KP.Customer.DBModels;
using KP.Register.IServices.Register;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KP.Register.Business
{
    public class SubAgentBL : ISubAgent
    {

        public KP.Customer.DBModels.CustomerDataContext db { get; set; }

        public SubAgentBL(KP.Customer.DBModels.CustomerDataContext _db)
        {
            db = _db;
        }

        public ReturnObject<CONS_SubAgent> getSubAgentByCode(string agentCode, string subAgentCode, string subBranchCode = "")
        {
            ReturnObject<CONS_SubAgent> ret = new ReturnObject<CONS_SubAgent>();

            try
            {
                var subAgentBL = new KP.Customer.Business.SubAgent.SubAgentBL();
                var result = subAgentBL.getSubAgentByCode(db, agentCode,subAgentCode);
                if (result.Data != null)
                {
                    ret.Data = result.Data;
                }
                if(result.Message.Count()>0)
                {
                    ret.Message = result.Message;
                }
            }
            catch(SqlException ex)
            {
                ret.SetMessage("DBEX", ex.Message);
            }
            catch (Exception ex)
            {
                ret.SetMessage(ex);
            }

            return ret;
        }

        public ReturnObject<List<CONS_SubAgent>> GetSubAgentList(string agentCode, string subBranchCode = "")
        {
            ReturnObject<List<CONS_SubAgent>> ret = new ReturnObject<List<CONS_SubAgent>>();

            try
            {
                var subAgentBL = new KP.Customer.Business.SubAgent.SubAgentBL();
                var result = subAgentBL.GetSubAgentList(db,agentCode);
                if (result.Data.Count() > 0)
                {
                    ret.Data = result.Data;
                }
            }
            catch (Exception ex)
            {
                ret.SetMessage(ex);
            }

            return ret; ;
        }
    }
}
