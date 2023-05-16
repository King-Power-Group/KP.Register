using KP.Common.Return;
using KP.Customer.ServiceModels;
using KP.Register.IServices.Register;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KP.Register.Business
{
    public class CallerBL : ICaller
    {
        public KP.Customer.DBModels.CustomerDataContext db { get; set; }

        public CallerBL(Customer.DBModels.CustomerDataContext _db)
        {
            db = _db;
        }

        //public ReturnObject<CallerModel> getCallerbyCallerID(string callerCode)
        //{
        //    ReturnObject<CallerModel> ret = new ReturnObject<CallerModel>();

        //    try
        //    {
        //        var callerBL = new KP.Customer.Business.Caller.CallerBL();
        //        var result = callerBL.getCallerByCallerID(db, callerCode);
        //        if (result.Data != null)
        //        {
        //            ret.Data = result.Data;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        ret.SetMessage(ex);
        //    }

        //    return ret;
        //}

        public ReturnObject<CallerTour> getTourbyCallerorTourCode(string search, string type)
        {
            ReturnObject<CallerTour> ret = new ReturnObject<CallerTour>();
            try
            {
                var callerBL = new KP.Customer.Business.Caller.CallerBL();
                var result = callerBL.getTourbyCallerID(db, search, type);
                if (result.Data != null)
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

        public ReturnObject<bool> validTourbyTourCode(string tourCode)
        {
            ReturnObject<bool> ret = new ReturnObject<bool>();
            try
            {
                var callerBL = new KP.Customer.Business.Caller.CallerBL();
                var result = callerBL.validTourbyTourCode(db, tourCode);
                ret = result;

                if(result.Message.Count()>0)
                {
                    ret.Message = result.Message;
                }
            }
            catch (Exception ex)
            {
                ret.SetMessage(ex);
                throw;
            }

            return ret;
        }
    }
}
