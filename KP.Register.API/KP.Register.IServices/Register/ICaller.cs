using KP.Common.Return;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KP.Register.IServices.Register
{
    public interface ICaller
    {
        //ReturnObject<KP.Customer.ServiceModels.CallerModel> getCallerbyCallerID(string callerCode);

        ReturnObject<KP.Customer.ServiceModels.CallerTour> getTourbyCallerorTourCode(string search,string type);

        ReturnObject<bool> validTourbyTourCode(string tourCode);
    }
}
