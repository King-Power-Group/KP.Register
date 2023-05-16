using KP.Common.Return;
using KP.Customer.DBModels;
using KP.Register.IServices.Register;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KP.Register.Business
{
    public class LocationBL : ILocation
    {
        public KP.Customer.DBModels.CustomerDataContext db { get; set; }

        public LocationBL(CustomerDataContext _db)
        {
            db = _db;
        }

        public ReturnObject<List<CE_LOV>> GetLocationList()
        {
            ReturnObject<List<CE_LOV>> ret = null;

            try
            {
                var locationBL = new KP.Customer.Business.Location.LocationBL();

                ret = locationBL.GetLocationList(db);
            }
            catch (Exception ex)
            {
                ret = new ReturnObject<List<CE_LOV>>();
                ret.SetMessage("API305", ex.Message);
            }

            return ret;
        }
    }
}
