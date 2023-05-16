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
    public class FlightBL : IFlight
    {

        public KP.Customer.DBModels.CustomerDataContext db { get; set; }

        public FlightBL(KP.Customer.DBModels.CustomerDataContext _db)
        {
            db = _db;
        }

        public ReturnObject<bool> CheckIsLimitFlight(string SubBranchCode, DateTime FlightDateTime, string OverDayMessage = "", string LessThenTime = "")
        {
            ReturnObject<bool> ret = new ReturnObject<bool>();
            try
            {
                var flighBL = new KP.Customer.Business.Flight.FlightBL();

                ret = flighBL.CheckIsLimitFlight(db, SubBranchCode, FlightDateTime, OverDayMessage, LessThenTime);
            }
            catch (Exception ex)
            {
                ret = new ReturnObject<bool>();
                ret.SetMessage("API305", ex.Message);
            }
            return ret;
        }

        public ReturnObject<List<Airline>> GetAirlineList()
        {
            ReturnObject<List<Airline>> ret = new ReturnObject<List<Airline>>();
            try
            {
                var flighBL = new KP.Customer.Business.Flight.FlightBL();

                ret = flighBL.GetAirlineList(db);
            }
            catch (Exception ex)
            {
                ret = new ReturnObject<List<Airline>>();
                ret.SetMessage("API305", ex.Message);
            }
            return ret;
        }

        public ReturnObject<Flight> GetFlight(string subBranchCode, string flightCode)
        {
            ReturnObject<Flight> ret = new ReturnObject<Flight>();
            try
            {
                var flighBL = new KP.Customer.Business.Flight.FlightBL();

                ret = flighBL.GetFlight(db, subBranchCode, flightCode);
            }
            catch (Exception ex)
            {
                ret = new ReturnObject<Flight>();
                ret.SetMessage("API305", ex.Message);
            }
            return ret;
        }

        public ReturnObject<List<Flight>> GetFlightList(string subBranchCode)
        {
            ReturnObject<List<Flight>> ret = new ReturnObject<List<Flight>>();
            try
            {
                var flighBL = new KP.Customer.Business.Flight.FlightBL();

                ret = flighBL.GetFlightList(db, subBranchCode);
            }
            catch (Exception ex)
            {
                ret = new ReturnObject<List<Flight>>();
                ret.SetMessage("API305", ex.Message);
            }
            return ret;
        }

        public ReturnObject<Flight> GetOpenFlight(string subBranchCode)
        {
            ReturnObject<Flight> ret = new ReturnObject<Flight>();
            try
            {
                var flighBL = new KP.Customer.Business.Flight.FlightBL();

                ret = flighBL.GetOpenFlight(db, subBranchCode);
            }
            catch (Exception ex)
            {
                ret = new ReturnObject<Flight>();
                ret.SetMessage("API305", ex.Message);
            }
            return ret;
        }

        public ReturnObject<List<Flight>> SearchFlight(string subBranchCode, string AirlineCode, string FlightCode)
        {
            ReturnObject<List<Flight>> ret = new ReturnObject<List<Flight>>();
            try
            {
                var flighBL = new KP.Customer.Business.Flight.FlightBL();

                ret = flighBL.SerachFlight(db, subBranchCode,AirlineCode,FlightCode);
            }
            catch (Exception ex)
            {
                ret = new ReturnObject<List<Flight>>();
                ret.SetMessage("API305", ex.Message);
            }
            return ret;
        }

        public ReturnObject<List<Flight>> SearchFlightWithFlightCode(string subBranchCode, string AirlineCode, string FlightCode)
        {
            ReturnObject<List<Flight>> ret = new ReturnObject<List<Flight>>();

            return ret;
        }

        public ReturnObject<List<Airline>> SearchAirline(string AirlineCode, int limit = 20)
        {
            ReturnObject<List<Airline>> ret = new ReturnObject<List<Airline>>();
            try
            {
                var flighBL = new KP.Customer.Business.Flight.FlightBL();

                ret = flighBL.SerachAirline(db, AirlineCode, limit);
            }
            catch (Exception ex)
            {
                ret = new ReturnObject<List<Airline>>();
                ret.SetMessage("API305", ex.Message);
            }
            return ret;
        }

        public ReturnObject<List<Flight>> SearchFlightWithoutAirline(string subBranchCode, string FlightCode, int limit = 20)
        {
            ReturnObject<List<Flight>> ret = new ReturnObject<List<Flight>>();
            try
            {
                var flighBL = new KP.Customer.Business.Flight.FlightBL();

                ret = flighBL.SerachFlightWithoutAirline(db, subBranchCode, FlightCode, limit);
            }
            catch (Exception ex)
            {
                ret = new ReturnObject<List<Flight>>();
                ret.SetMessage("API305", ex.Message);
            }
            return ret;
        }

        public ReturnObject<bool> ValidateFlight(string subBranchCode, string FlightCode, DateTime FlightDateTime)
        {
            ReturnObject<bool> ret = new ReturnObject<bool>();
            try
            {
                var flighBL = new KP.Customer.Business.Flight.FlightBL();

                ret = flighBL.ValidateFlight(db, subBranchCode, FlightCode, FlightDateTime);
            }
            catch (Exception ex)
            {
                ret = new ReturnObject<bool>();
                ret.SetMessage("API305", ex.Message);
            }
            return ret;
        }

    }
}
