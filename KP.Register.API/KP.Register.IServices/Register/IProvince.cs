using KP.Common.Return;
using KP.Customer.DBModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KP.Register.IServices.Register
{
    public interface IProvince
    {
        ReturnObject<List<CONS_Province>> GetProvinceList(string subBranchCode = "");

        ReturnObject<CONS_Province> getProvinceByCode( string provinceCode, string subBranchCode = "");
    }
}
