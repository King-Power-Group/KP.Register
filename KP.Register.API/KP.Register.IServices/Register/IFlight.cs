using KP.Common.Return;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace KP.Register.IServices.Register
{
    [ServiceContract]
    public interface IFlight
    {
        [OperationContract]
        ReturnObject<List<Customer.ServiceModels.Airline>> GetAirlineList();

        [OperationContract]
        ReturnObject<List<Customer.ServiceModels.Airline>> SearchAirline(string AirlineCode, int limit = 20);

        [OperationContract]
        ReturnObject<List<KP.Customer.ServiceModels.Flight>> SearchFlight(string subBranchCode, string AirlineCode, string FlightCode);

        [OperationContract]
        ReturnObject<List<KP.Customer.ServiceModels.Flight>> SearchFlightWithFlightCode(string subBranchCode, string AirlineCode, string FlightCode);

        [OperationContract]
        ReturnObject<List<Customer.ServiceModels.Flight>> SearchFlightWithoutAirline(string subBranchCode, 
            string FlightCode, int limit = 20);

        [OperationContract]
        ReturnObject<List<Customer.ServiceModels.Flight>> GetFlightList(string subBranchCode);

        [OperationContract]
        ReturnObject<Customer.ServiceModels.Flight> GetOpenFlight(string subBranchCode);

        [OperationContract]
        ReturnObject<bool> ValidateFlight(string subBranchCode, string FlightCode, DateTime FlightDateTime);

        [OperationContract]
        ReturnObject<Customer.ServiceModels.Flight> GetFlight(string subBranchCode, string flightCode);

        [OperationContract]
        ReturnObject<bool> CheckIsLimitFlight(string SubBranchCode,
            DateTime FlightDateTime, string OverDayMessage = "", string LessThenTime = "");

    }
}
