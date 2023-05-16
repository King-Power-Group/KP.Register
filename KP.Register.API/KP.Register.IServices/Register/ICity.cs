using KP.Common.Return;
using KP.Customer.DBModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KP.Register.IServices.Register
{
    public interface ICity
    {
        ReturnObject<List<CONS_City>> GetCityList(string subBranchCode = "");

        ReturnObject<CONS_City> getCityByCityCode( string city, string province, string subBranchCode = "");
    }
}
