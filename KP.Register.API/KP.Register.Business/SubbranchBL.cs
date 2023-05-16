using KP.Common.Return;
using KP.Customer.ServiceModels;
using KP.Register.IServices.Register;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KP.Register.Business
{
    public class SubbranchBL : ISubbranch
    {
        public KP.Customer.DBModels.CustomerDataContext db { get; set; }

        public SubbranchBL(KP.Customer.DBModels.CustomerDataContext _db)
        {
            db = _db;
        }

        public ReturnObject<List<SubBranch>> getListSubbranch(KeySearchParameter model, string lang = "")
        {
            ReturnObject<List<SubBranch>> ret = new ReturnObject<List<SubBranch>>();
            try
            {
                if (model.pageNo <= 0 && (model.pageSize <= 0 || model.pageSize > 100))
                {
                    model.pageNo = 1;
                    model.pageSize = 100;
                }

                var sunBranchBL = new KP.Customer.Business.SubBranch.SubBranchBL();
                var objData = sunBranchBL.GetSubBranchList(db);
                var data = new List<SubBranch>();

                if (objData.Data.Count() > 0)
                {

                    if (model.key == "registershop")
                    {
                        objData.Data = objData.Data.Skip((model.pageNo - 1) * model.pageSize).Take(model.pageSize).ToList();

                        objData.Data = objData.Data.Where(m => m.IsRegisterShop==true).ToList();

                        foreach (var item in objData.Data)
                        {
                            var tmp = new SubBranch();
                            tmp.subbranchCode = item.SubBranchCode;
                            tmp.subbranchName = item.SubBranchNameShop;
                            tmp.BranchNo = item.BranchNo;
                            tmp.ConfigSC = item.ConfigSC;
                            tmp.CutOffTime = item.CutOffTime;
                            data.Add(tmp);
                        }
                    }
                    else
                    {
                        if (model.key != "")
                        {
                            objData.Data = objData.Data.Where(m => m.SubBranchCode.ToUpper().Contains(model.key.ToUpper())
                           || m.SubBranchName.ToUpper().Contains(model.key.ToUpper())).ToList();
                        }

                        objData.Data = objData.Data.Skip((model.pageNo - 1) * model.pageSize).Take(model.pageSize).ToList();

                       
                        foreach (var item in objData.Data)
                        {
                            var tmp = new SubBranch();
                            tmp.subbranchCode = item.SubBranchCode;
                            tmp.subbranchName = item.SubBranchName;
                            tmp.BranchNo = item.BranchNo;
                            tmp.ConfigSC = item.ConfigSC;
                            tmp.CutOffTime = item.CutOffTime;
                            data.Add(tmp);
                        }

                    }


                    ret.Data = data;
                    ret.totalCount = data.Count();
                    ret.isCompleted = true;
                }
                else
                {
                    ret.SetMessage("SB001", "Subbranch not found.");
                    ret.isCompleted = false;
                }
            }
            catch (Exception ex)
            {
                ret.isCompleted = false;
                ret = new ReturnObject<List<SubBranch>>();
                ret.SetMessage("API305", ex.Message);
            }
            return ret;
        }

        public SubBranch getSubbranchBySubbranchCode()
        {
            throw new NotImplementedException();
        }

        public ReturnObject<bool> validSubBranchCode(string subbrachCode)
        {
            ReturnObject<bool> ret = new ReturnObject<bool>();
            try
            {
                var subbrachBl = new KP.Customer.Business.SubBranch.SubBranchBL();
                ret = subbrachBl.validSubbrachCode(db, subbrachCode);
            }
            catch (Exception ex)
            {
                ret.SetMessage(ex);
            }

            return ret;
        }
    }
}
