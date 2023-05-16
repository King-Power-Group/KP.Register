using KP.Common.Return;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KP.Register.Business
{
    public class CommonClass
    {
        public enum eConnectioName {
            CutomerDataConnection,
            DDSDataConnection
        }

        public static string getConnectionString(eConnectioName connetioname) {
            string constring="";
            if(connetioname.Equals(eConnectioName.CutomerDataConnection))
            {
                 constring = "Data Source=devdata;Initial Catalog=CustomerData;Persist Security Info=True;User ID=sa;Password=sql2000";
            }else if (connetioname.Equals(eConnectioName.DDSDataConnection))
            {
                constring = "Data Source=dds-server;Initial Catalog=DEVMASTER;Persist Security Info=True;User ID=sa;Password=sql2008";
            }
            return constring;
        }

        public static KP.Customer.DBModels.CustomerDataContext GetDbContext() {
            string constring = getConnectionString(eConnectioName.CutomerDataConnection);
            Customer.DBModels.CustomerDataContext db = new Customer.DBModels.CustomerDataContext(constring);
            return db;
        }

        public static KP.Customer.DBModels.DDSServerDataContext GetDbMsgContext()
        {
            string constring = getConnectionString(eConnectioName.DDSDataConnection);
            Customer.DBModels.DDSServerDataContext db = new Customer.DBModels.DDSServerDataContext(constring);
            return db;
        }

        public static ReturnObject<Customer.ServiceModels.Message> setMessageError(string type,string code,string name,string message){
            ReturnObject<Customer.ServiceModels.Message> ret = new ReturnObject<Customer.ServiceModels.Message>();
            ret.Data = new Customer.ServiceModels.Message();
            ret.Data.messageType = type;
            ret.Data.messageCode = code;
            ret.Data.messageName = name;
            ret.Data.messageDescription = message;
            return ret;
        }
    }
}
