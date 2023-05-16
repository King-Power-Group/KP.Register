using KP.Common.Return;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KP.Common.Helper;
using System.Globalization;
using KP.Register.IServices.Register;
using System.Collections;
using System.Data.SqlClient;
using KP.Customer.ServiceModels;
using System.Reflection;
using KP.Register.Business.Identity;
using System.Text.RegularExpressions;
using KP.Customer.Business.Register;
using KP.Customer.DBModels;
using KP.Caller.ServiceModels;

namespace KP.Register.Business
{
    public class RegisterBL : IRegister
    {
        public KP.Customer.DBModels.CustomerDataContext db { get; set; }
        public KP.Customer.DBModels.DDSServerDataContext dbmsg { get; set; }


        public RegisterBL(CustomerDataContext _db, DDSServerDataContext _dbmsg)
        {
            db = _db;
            dbmsg = _dbmsg;
        }

        private static readonly HashSet<string> AllTimeZoneIds =
      new HashSet<string>(TimeZoneInfo.GetSystemTimeZones()
                                      .Select(tz => tz.Id));

        public ReturnObject<List<Customer.ServiceModels.Output>> ValidateData(List<Customer.ServiceModels.Register> model,
            List<KP.Common.Helper.CallerModel> callerAttributeList, string lang = "")
        {
            // var data = new SohdrEntity();
            var ret = new ReturnObject<List<Customer.ServiceModels.Output>>();

            var msgData = dbmsg.MAST_Messages.Where(t => t.MsgProject
            == KP.Common.Customer.MessageProject.RegisterAPI
            && t.MsgFunction == KP.Common.Customer.MessageFunction.ValidateAPI).ToList();

            var tempData = model;

            if (model.Count() == 0 || model == null)
            {
                var error = msgData.Where(t => t.MsgNo.Equals("M023")).FirstOrDefault();
                ret.SetMessage(error.MsgCode, error.MsgDesc);
            }
            else
            {
                //validate Only Input 
                foreach (var data in tempData)
                {
                    if (StringBL.StringNullToEmpty(data.action) == "")
                    {
                        var error = msgData.Where(t => t.MsgNo.Equals("M019")).FirstOrDefault();
                        ret.SetMessage(error.MsgCode, error.MsgDesc);
                    }
                    else
                    {
                        var chkAction = validAction(data.action, msgData);
                        if (!chkAction.Data)
                        {
                            ret.Message.AddRange(chkAction.Message);
                        }
                        else
                        {

                            if (data.action == KP.Common.Customer.RegisterAction.PreRegisterAdd
                                || data.action == KP.Common.Customer.RegisterAction.PosRegisterAdd)
                            {
                                //valid Platform
                                if (StringBL.StringNullToEmpty(data.platformCode) == "")
                                {
                                    var error = msgData.Where(t => t.MsgNo.Equals("M024")).FirstOrDefault();
                                    ret.SetMessage(error.MsgCode, error.MsgDesc);
                                }

                                //valid UserCode
                                if (StringBL.StringNullToEmpty(data.userCode) == "")
                                {
                                    var error = msgData.Where(t => t.MsgNo.Equals("M025")).FirstOrDefault();
                                    ret.SetMessage(error.MsgCode, error.MsgDesc);
                                }

                                //valid Prefix ShoppingCard
                                if (StringBL.StringNullToEmpty(data.prefixShoppingCard) == "")
                                {
                                    var error = msgData.Where(t => t.MsgNo.Equals("M021")).FirstOrDefault();
                                    ret.SetMessage(error.MsgCode, error.MsgDesc);
                                }

                                if (data.action == KP.Common.Customer.RegisterAction.PosRegisterAdd)
                                {
                                    if (StringBL.StringNullToEmpty(data.subBranchCode) == "")
                                    {
                                        var error = msgData.Where(t => t.MsgNo.Equals("M020")).FirstOrDefault();
                                        ret.SetMessage(error.MsgCode, error.MsgDesc);
                                    }
                                }
                            }

                            // if input have agent 
                            if (StringBL.StringNullToEmpty(data.agentCode) != "")
                            {
                                data.agentCode = data.agentCode.ToUpper();

                                //if (data.tour == null)
                                //{
                                //    var error = msgData.Where(t => t.MsgNo.Equals("M068")).FirstOrDefault();
                                //    ret.SetMessage(error.MsgCode, error.MsgDesc);
                                //}

                                if (data.agentCode.Length > 0) //Case when found agent
                                {
                                    if (data.agentCode.Length < 10 && data.agentCode.Length > 0)
                                    {
                                        data.agentCode = data.agentCode.Substring(0, 1) + data.agentCode.Substring(1).PadLeft(9, '0');
                                    };
                                }

                            }

                            if (StringBL.StringNullToEmpty(data.subAgentCode) != "")
                            {
                                data.subAgentCode = data.subAgentCode.ToUpper();

                                if (data.subAgentCode.Length > 0) //Case when found agent
                                {
                                    if (data.subAgentCode.Length < 10 && data.subAgentCode.Length > 0)
                                    {
                                        data.subAgentCode = data.subAgentCode.Substring(0, 1) + data.subAgentCode.Substring(1).PadLeft(9, '0');
                                    };
                                }
                            }

                            // valid tour
                            if (data.tour != null)
                            {

                                if (StringBL.StringNullToEmpty(data.tour.tourCode) != ""
                                    && (data.action == KP.Common.Customer.RegisterAction.PreRegisterEdit
                                    || data.action == KP.Common.Customer.RegisterAction.PosRegisterEdit))
                                {
                                    if (StringBL.StringNullToEmpty(data.tour.nationality) == "")
                                    {
                                        var error = msgData.Where(t => t.MsgNo.Equals("M003")).FirstOrDefault();
                                        ret.SetMessage(error.MsgCode, error.MsgDesc);
                                    }
                                    else if (data.tour.nationality.Trim().Length > 3)
                                    {
                                        var error = msgData.Where(t => t.MsgNo.Equals("M026")).FirstOrDefault();
                                        ret.SetMessage(error.MsgCode, error.MsgDesc);
                                    }

                                    if (StringBL.StringNullToEmpty(data.tour.airlineCode) == "")
                                    {
                                        var error = msgData.Where(t => t.MsgNo.Equals("M027")).FirstOrDefault();
                                        ret.SetMessage(error.MsgCode, error.MsgDesc);
                                    }

                                    if (StringBL.StringNullToEmpty(data.tour.flightCode) == "")
                                    {
                                        var error = msgData.Where(t => t.MsgNo.Equals("M028")).FirstOrDefault();
                                        ret.SetMessage(error.MsgCode, error.MsgDesc);
                                    }

                                    if (data.tour.flightDate == null)
                                    {
                                        var error = msgData.Where(t => t.MsgNo.Equals("M029")).FirstOrDefault();
                                        ret.SetMessage(error.MsgCode, error.MsgDesc);
                                    }

                                    if (StringBL.StringNullToEmpty(data.tour.flightTime) == "")
                                    {
                                        var error = msgData.Where(t => t.MsgNo.Equals("M030")).FirstOrDefault();
                                        ret.SetMessage(error.MsgCode, error.MsgDesc);
                                    }
                                }


                                if (StringBL.StringNullToEmpty(data.tour.tourCode) == "")
                                {
                                    if (data.action == KP.Common.Customer.RegisterAction.PreRegisterEdit)
                                    {
                                        var error = msgData.Where(t => t.MsgNo.Equals("M085")).FirstOrDefault();
                                        ret.SetMessage(error.MsgCode, error.MsgDesc);
                                    }

                                    if (StringBL.StringNullToEmpty(data.tour.nationality) == "")
                                    {
                                        var error = msgData.Where(t => t.MsgNo.Equals("M003")).FirstOrDefault();
                                        ret.SetMessage(error.MsgCode, error.MsgDesc);
                                    }
                                    else if (data.tour.nationality.Trim().Length > 3)
                                    {
                                        var error = msgData.Where(t => t.MsgNo.Equals("M026")).FirstOrDefault();
                                        ret.SetMessage(error.MsgCode, error.MsgDesc);
                                    }

                                    if (StringBL.StringNullToEmpty(data.tour.airlineCode) == "")
                                    {
                                        var error = msgData.Where(t => t.MsgNo.Equals("M027")).FirstOrDefault();
                                        ret.SetMessage(error.MsgCode, error.MsgDesc);
                                    }

                                    if (StringBL.StringNullToEmpty(data.tour.flightCode) == "")
                                    {
                                        var error = msgData.Where(t => t.MsgNo.Equals("M028")).FirstOrDefault();
                                        ret.SetMessage(error.MsgCode, error.MsgDesc);
                                    }

                                    if (data.tour.flightDate == null)
                                    {
                                        var error = msgData.Where(t => t.MsgNo.Equals("M029")).FirstOrDefault();
                                        ret.SetMessage(error.MsgCode, error.MsgDesc);
                                    }

                                    if (StringBL.StringNullToEmpty(data.tour.flightTime) == "")
                                    {
                                        var error = msgData.Where(t => t.MsgNo.Equals("M030")).FirstOrDefault();
                                        ret.SetMessage(error.MsgCode, error.MsgDesc);
                                    }
                                }
                            }

                            //valid Personal
                            if (data.listPersonal == null)
                            {
                                var error = msgData.Where(t => t.MsgNo.Equals("M008")).FirstOrDefault();
                                ret.SetMessage(error.MsgCode, error.MsgDesc);
                            }
                            else
                            {
                                if (data.listPersonal.GroupBy(n => n.runningNo).Any(c => c.Count() > 1))
                                {
                                    var error = msgData.Where(t => t.MsgNo.Equals("M031")).FirstOrDefault();
                                    ret.SetMessage(error.MsgCode, error.MsgDesc);
                                }

                                if (data.listPersonal.GroupBy(n => n.passportNo).Any(c => c.Count() > 1))
                                {
                                    var error = msgData.Where(t => t.MsgNo.Equals("M084")).FirstOrDefault();
                                    ret.SetMessage(error.MsgCode, error.MsgDesc);
                                }


                                if (data.action == KP.Common.Customer.RegisterAction.PreRegisterAdd
                                    || data.action == KP.Common.Customer.RegisterAction.PosRegisterAdd)
                                {
                                    //var checktypeTA = data.listPersonal.Where(t => t.customerTypeCode.Equals("TA")).ToList();
                                    //if (checktypeTA.Count() > 0)
                                    //{
                                    //    //check have type ITA or FIT with Tour 
                                    //    var checkhavefitorita = data.listPersonal.Where(t =>
                                    //    t.customerTypeCode.Equals("ITA") ||
                                    //    t.customerTypeCode.Equals("FIT")).ToList();

                                    //    //no have 
                                    //    if (checkhavefitorita.Count() == 0)
                                    //    {
                                    //        Boolean chk_tourcode = false;
                                    //        if (data.tour == null)
                                    //        {
                                    //            chk_tourcode = true;
                                    //        }
                                    //        else if (StringBL.StringNullToEmpty(data.tour.tourCode) == "")
                                    //        {
                                    //            chk_tourcode = true;
                                    //        }

                                    //        if (chk_tourcode)
                                    //        {
                                    //            var error = msgData.Where(t => t.MsgNo.Equals("M068")).FirstOrDefault();
                                    //            ret.SetMessage(error.MsgCode, error.MsgDesc);
                                    //        }
                                    //    }//else have FIT or ITA
                                    //}
                                }

                                foreach (var customer in data.listPersonal)
                                {

                                    //if (data.platformCode.Equals("ONLINEMOB") || data.platformCode.Equals("ONLINEKIOSK"))
                                    //{
                                    //    if (StringBL.StringNullToEmpty(customer.customerTypeCode) == "")
                                    //    {
                                    //        if (StringBL.StringNullToEmpty(data.agentCode) == "")
                                    //        {
                                    //            if (customer.nationality.Equals("CHN"))
                                    //            {
                                    //                customer.customerTypeCode = "FITOT";
                                    //            }
                                    //            else
                                    //            {
                                    //                customer.customerTypeCode = "FIT";
                                    //            }
                                    //        }
                                    //    }
                                    //}
                                    //else
                                    //{
                                    if (StringBL.StringNullToEmpty(customer.customerTypeCode) != "")
                                    {

                                        customer.customerTypeCode = customer.customerTypeCode.ToUpper();
                                        //if (data.allowTakeAway || customer.flightCode.Equals("OP000"))
                                        //{
                                        //    customer.customerTypeCode = "FITT";
                                        //}
                                        //else
                                        //{
                                        //    customer.customerTypeCode = "FIT";
                                        //}
                                        //var error = msgData.Where(t => t.MsgNo.Equals("M032")).FirstOrDefault();
                                        //ret.SetMessage(error.MsgCode, error.MsgDesc);
                                    }
                                    else
                                    {
                                        if (data.isAirport)
                                        {
                                            if (data.allowTakeAway || customer.flightCode.Equals("OP000"))
                                            {
                                                customer.customerTypeCode = "FITT";
                                            }
                                            else
                                            {
                                                customer.customerTypeCode = "FIT";
                                            }
                                        }
                                    }
                                    //}

                                    if (!data.isFastRegister)
                                    {

                                        if (customer.dateOfBirth != null)
                                        {
                                            var getdatetimeServer = DateTimeBL.GetNowDateTime(db);

                                            DateTime zeroTime = new DateTime(1, 1, 1);
                                            TimeSpan diffYear = getdatetimeServer.Date - customer.dateOfBirth.Value;

                                            int years = (zeroTime + diffYear).Year - 1;


                                            if (years < 15)
                                            {
                                                //var error = msgData.Where(t => t.MsgNo.Equals("M187")).FirstOrDefault();
                                                //ret.SetMessage(error.MsgCode, error.MsgDesc);

                                                ret.SetMessage("M487", "Date Server : "+ getdatetimeServer.Date.ToString("dd/MM/yyyy")  +" .BirthDay : " + customer.dateOfBirth.Value.ToString("dd/MM/yyyy") + " have " + years +" years. Under 15 Years can't Register!.");
                                            }
                                        }

                                        if (customer.passportNo != null && customer.passportNo != "")
                                        {
                                            bool fHasSpace = customer.passportNo.Contains(" ");
                                            if (fHasSpace)
                                            {
                                                var error = msgData.Where(t => t.MsgNo.Equals("M086")).FirstOrDefault();
                                                ret.SetMessage(error.MsgCode, error.MsgDesc);
                                            }

                                            customer.passportNo = customer.passportNo.ToUpper();
                                        }


                                        if (data.isAirport)
                                        {
                                            if(StringBL.StringNullToEmpty(customer.englishName) == "")
                                            {
                                                customer.englishName = "TEMPNAME";
                                            }

                                            if (StringBL.StringNullToEmpty(customer.nativeName) == "")
                                            {
                                                customer.nativeName = "TEMPNAME";
                                            }
                                            if(StringBL.StringNullToEmpty(customer.nationality) == "")
                                            {
                                                customer.nationality = "THA";
                                            }
                                        }


                                        //check English Name
                                        if (StringBL.StringNullToEmpty(customer.englishName) == "")
                                        {

                                            var error = msgData.Where(t => t.MsgNo.Equals("M009")).FirstOrDefault();
                                            ret.SetMessage(error.MsgCode, error.MsgDesc);
                                        }
                                        else if (StringBL.StringNullToEmpty(customer.englishName).Length > 100)
                                        {
                                            var error = msgData.Where(t => t.MsgNo.Equals("M033")).FirstOrDefault();
                                            ret.SetMessage(error.MsgCode, error.MsgDesc);
                                        }
                                        else if (StringBL.StringNullToEmpty(customer.englishName) != "")
                                        {
                                            if (customer.englishName.Trim().Length > 0)
                                            {
                                                customer.englishName = customer.englishName.ToUpper();
                                            }
                                        }

                                        //check Native Name
                                        if (StringBL.StringNullToEmpty(customer.nativeName) == "")
                                        {
                                            var error = msgData.Where(t => t.MsgNo.Equals("M010")).FirstOrDefault();
                                            ret.SetMessage(error.MsgCode, error.MsgDesc);
                                        }
                                        else if (StringBL.StringNullToEmpty(customer.nativeName).Length > 100)
                                        {
                                            var error = msgData.Where(t => t.MsgNo.Equals("M034")).FirstOrDefault();
                                            ret.SetMessage(error.MsgCode, error.MsgDesc);
                                        }
                                        else if (StringBL.StringNullToEmpty(customer.nativeName) != "")
                                        {
                                            if (customer.nativeName.Trim().Length > 0)
                                            {
                                                customer.nativeName = customer.nativeName.ToUpper();
                                            }
                                        }

                                        if (StringBL.StringNullToEmpty(customer.nationality) == "")
                                        {
                                            var error = msgData.Where(t => t.MsgNo.Equals("M003")).FirstOrDefault();
                                            ret.SetMessage(error.MsgCode, error.MsgDesc);
                                        }
                                        else if (customer.nationality.Length > 3)
                                        {
                                            var error = msgData.Where(t => t.MsgNo.Equals("M026")).FirstOrDefault();
                                            ret.SetMessage(error.MsgCode, error.MsgDesc);
                                        }
                                    }

                                    if (!data.allowTakeAway)
                                    {
                                        if (StringBL.StringNullToEmpty(customer.airlineCode) == "")
                                        {
                                            var error = msgData.Where(t => t.MsgNo.Equals("M004")).FirstOrDefault();
                                            ret.SetMessage(error.MsgCode, error.MsgDesc);
                                        }

                                        if (StringBL.StringNullToEmpty(customer.flightCode) == "")
                                        {
                                            var error = msgData.Where(t => t.MsgNo.Equals("M005")).FirstOrDefault();
                                            ret.SetMessage(error.MsgCode, error.MsgDesc);
                                        }

                                        if (customer.flightDate == null)
                                        {
                                            var error = msgData.Where(t => t.MsgNo.Equals("M035")).FirstOrDefault();
                                            ret.SetMessage(error.MsgCode, error.MsgDesc);
                                        }

                                        if (StringBL.StringNullToEmpty(customer.flightTime) == "")
                                        {
                                            var error = msgData.Where(t => t.MsgNo.Equals("M006")).FirstOrDefault();
                                            ret.SetMessage(error.MsgCode, error.MsgDesc);
                                        }

                                    }

                                    //if (data.tour == null || StringBL.StringNullToEmpty(data.tour.flightCode) == "")
                                    //{

                                    //}

                                    if (data.action == KP.Common.Customer.RegisterAction.PreRegisterAdd ||
                                        data.action == KP.Common.Customer.RegisterAction.PreRegisterEdit)
                                    {
                                        var validlistContact = validListContact(customer.listContact, msgData, data.action, customer.nationality.ToUpper());
                                        if (validlistContact.Message.Count() > 0)
                                        {
                                            ret.Message.AddRange(validlistContact.Message);
                                        }
                                    }

                                    if (customer.listIdentity == null)
                                    {
                                        var error = msgData.Where(t => t.MsgNo.Equals("M014")).FirstOrDefault();
                                        ret.SetMessage(error.MsgCode, error.MsgDesc);
                                    }
                                    else
                                    {

                                        if (data.action == KP.Common.Customer.RegisterAction.PreRegisterEdit ||
                                            data.action == KP.Common.Customer.RegisterAction.PosRegisterEdit)
                                        {
                                            if (customer.listIdentity.Where(t => t.IdentityType.Equals("SHOPCARD")).Count() != 1)
                                            {
                                                var error = msgData.Where(t => t.MsgNo.Equals("M076")).FirstOrDefault();
                                                ret.SetMessage(error.MsgCode, error.MsgDesc);
                                            }
                                        }

                                        foreach (var dat in customer.listIdentity)
                                        {
                                            if (StringBL.StringNullToEmpty(dat.IdentityType) == "")
                                            {
                                                var error = msgData.Where(t => t.MsgNo.Equals("M015")).FirstOrDefault();
                                                ret.SetMessage(error.MsgCode, error.MsgDesc);
                                            }
                                            if (StringBL.StringNullToEmpty(dat.IdentityValue) == "")
                                            {
                                                var error = msgData.Where(t => t.MsgNo.Equals("M016")).FirstOrDefault();
                                                ret.SetMessage(error.MsgCode, error.MsgDesc);
                                            }

                                            //if(dat.IdentityType.Equals("SHOPCARD"))
                                            //{
                                            //    var chkValidShoppingCard = KP.Common.Helper.ValidateBL.CheckDigit(dat.IdentityValue);

                                            //    if(chkValidShoppingCard==null)
                                            //    {
                                            //        var error = msgData.Where(t => t.MsgNo.Equals("M087")).FirstOrDefault();
                                            //        ret.SetMessage(error.MsgCode, error.MsgDesc);
                                            //    }

                                            //}
                                        }

                                        if (data.action == KP.Common.Customer.RegisterAction.PreRegisterEdit ||
                                            data.action == KP.Common.Customer.RegisterAction.PreRegisterAdd)
                                        {
                                            //valid listIdentity
                                            var validCheckIden = validListIdentity(customer.listIdentity, msgData);
                                            if (!validCheckIden.Data)
                                            {
                                                ret.Message.AddRange(validCheckIden.Message);
                                            }
                                        }
                                    }
                                    //if (AllTimeZoneIds.Contains(customer.flightDate.Value.ToString()))
                                    //{
                                    //    customer.flightDate = DateTime.Parse(customer.flightDate.ToString());
                                    //}
                                }
                            }
                        }//else no have action in Database
                    }//end for
                }//end else big loop


                //after Validate Input to Access validate From Db
                if (ret.Message.Count() == 0)
                {
                    var shoppingCardBL = new ShoppingCardBL(db);
                    var sourceBL = new SourceBL(db);
                    var custTypeBL = new CustomerTypeBL(db);
                    var flightBL = new FlightBL(db);
                    var callerBL = new CallerBL(db);
                    var subBranchBL = new SubbranchBL(db);
                    var agentBL = new AgentBL(db);
                    var cityBL = new CityBL(db);
                    var provinceBL = new ProvinceBL(db);
                    var subAgentBL = new SubAgentBL(db);
                    var platFormBL = new PlatFormBL(db);

                    foreach (var data in tempData)
                    {
                        string CallerID = String.Empty;
                        var validCaller = callerAttributeList;//callerBL.getCallerbyCallerID(data.callerID);
                        if (validCaller.Count == 0)
                        {
                            //Code Caller ผิด
                            var error = msgData.Where(t => t.MsgNo.Equals("M039")).FirstOrDefault();
                            ret.SetMessage(error.MsgCode, error.MsgDesc);
                        }
                        else
                        {
                            CallerID = validCaller.FirstOrDefault().callerID;
                            //data.callerID = validCaller.FirstOrDefault().callerID;
                        }

                        if (StringBL.StringNullToEmpty(data.agentCode) == "")
                        {
                            //Code Caller ถูก ยึด Data ตาม Caller ข้างใน
                            var agentCode = validCaller.Where(n => n.attributeName == Common.Caller.CallerAttribute.AgentCode).FirstOrDefault();
                            if (agentCode != null)
                                data.agentCode = agentCode.valueString;

                            var subAgentCode = validCaller.Where(n => n.attributeName == Common.Caller.CallerAttribute.SubAgentCode).FirstOrDefault();
                            if (subAgentCode != null)
                                data.subAgentCode = subAgentCode.valueString;

                            var platformCode = validCaller.Where(n => n.attributeName == Common.Caller.CallerAttribute.PlatformCode).FirstOrDefault();
                            if (platformCode != null)
                                data.platformCode = platformCode.valueString;

                            var prefixShoppingCard = validCaller.Where(n => n.attributeName == Common.Caller.CallerAttribute.PrefixShoppingCard).FirstOrDefault();
                            if (prefixShoppingCard != null)
                                data.prefixShoppingCard = StringBL.StringNullToEmpty(prefixShoppingCard.valueString);

                            var allowTakeAway = validCaller.Where(n => n.attributeName == Common.Caller.CallerAttribute.IsAllowTakeAway).FirstOrDefault();
                            if (allowTakeAway != null)
                                data.allowTakeAway = allowTakeAway.valueBoolean.Value;

                            if (data.tour != null && data.tour.tourCode != "")
                            {
                                //ถ้ามี Tour Code ใน CUSD_Tour จะเอา Data มาใส่ใน Tour ให้ 

                                if (data.action == KP.Common.Customer.RegisterAction.PreRegisterAdd ||
                                    data.action == KP.Common.Customer.RegisterAction.PosRegisterAdd)
                                {
                                    var dataTour = callerBL.getTourbyCallerorTourCode(data.tour.tourCode, "T");
                                    if (dataTour.Data != null)
                                    {
                                        data.tour.airlineCode = StringBL.StringNullToEmpty(dataTour.Data.airlineCode);
                                        data.tour.bookStatus = dataTour.Data.bookStatus;
                                        data.tour.carType = StringBL.StringNullToEmpty(dataTour.Data.carType);
                                        data.tour.cityTour = StringBL.StringNullToEmpty(dataTour.Data.cityCode);
                                        data.tour.discountSource = StringBL.StringNullToEmpty(dataTour.Data.discountSource);
                                        data.tour.flightCode = StringBL.StringNullToEmpty(dataTour.Data.flightCode);
                                        data.tour.flightDate = dataTour.Data.flightDate;
                                        data.tour.flightTime = StringBL.StringNullToEmpty(dataTour.Data.flightTime);
                                        data.tour.licensePlate = StringBL.StringNullToEmpty(dataTour.Data.licensePlate);
                                        data.tour.nationality = StringBL.StringNullToEmpty(dataTour.Data.nationalityCode);
                                        data.tour.numberPack = dataTour.Data.numberPack;
                                        data.tour.province = StringBL.StringNullToEmpty(dataTour.Data.provinceCode);
                                        data.tour.tourCode = StringBL.StringNullToEmpty(dataTour.Data.tourCode);
                                        data.tour.tourDescription = StringBL.StringNullToEmpty(dataTour.Data.tourDescription);
                                    }
                                }
                            }
                        }
                        else if (StringBL.StringNullToEmpty(data.agentCode) != "")
                        {
                            //if (data.tour == null)
                            //{
                            //    var error = msgData.Where(t => t.MsgNo.Equals("M068")).FirstOrDefault();
                            //    ret.SetMessage(error.MsgCode, error.MsgDesc);
                            //}

                            if (StringBL.StringNullToEmpty(data.subAgentCode) == "")
                            {
                                //กรณีไม่ส่งค่า Sub Agent มา จะ Default เป็น C00000000C
                                data.subAgentCode = "C00000000C";
                            }
                        }

                        if (StringBL.StringNullToEmpty(data.subBranchCode) != "")
                        {
                            var validSubBranch = subBranchBL.validSubBranchCode(data.subBranchCode.Trim().ToUpper());
                            if (!validSubBranch.Data)
                            {
                                if (validSubBranch.Message.Where(a => a.MessageCode.Equals("DBEX")).ToList().Count > 0)
                                {
                                    ret.Message.AddRange(validSubBranch.Message);
                                }
                                else
                                {
                                    var error = msgData.Where(t => t.MsgNo.Equals("M040")).FirstOrDefault();
                                    ret.SetMessage(error.MsgCode, error.MsgDesc);
                                }
                            }

                        }

                        if (StringBL.StringNullToEmpty(data.agentCode) != "")
                        {
                            //valid Agent
                            var chkAgent = validAgent(data.agentCode);
                            if (!chkAgent.Data)
                            {
                                if (chkAgent.Message.Where(a => a.MessageCode.Equals("DBEX")).ToList().Count > 0)
                                {
                                    ret.Message.AddRange(chkAgent.Message);
                                }
                                else
                                {
                                    var error = msgData.Where(t => t.MsgNo.Equals("M080")).FirstOrDefault();
                                    ret.SetMessage(error.MsgCode, error.MsgDesc);
                                }
                            }
                        }

                        if (StringBL.StringNullToEmpty(data.subAgentCode) != "")
                        {
                            //valid SubAgent
                            var chkSubAgent = validSubAgent(data.agentCode, data.subAgentCode);
                            if (!chkSubAgent.Data)
                            {
                                if (chkSubAgent.Message.Where(a => a.MessageCode.Equals("DBEX")).ToList().Count > 0)
                                {
                                    ret.Message.AddRange(chkSubAgent.Message);
                                }

                                if (!data.subAgentCode.Equals("C00000000C"))
                                {
                                    var error = msgData.Where(t => t.MsgNo.Equals("M083")).FirstOrDefault();
                                    ret.SetMessage(error.MsgCode, error.MsgDesc);
                                }
                            }
                        }

                        //if (data.action == KP.Common.Customer.RegisterAction.PosRegisterAdd)
                        //{
                        //    if (StringBL.StringNullToEmpty(data.agentCode) == "" &&
                        //        StringBL.StringNullToEmpty(data.subAgentCode) == "")
                        //    {
                        //        data.tour = null;
                        //    }
                        //}

                        if (data.action == KP.Common.Customer.RegisterAction.PosRegisterAdd ||
                            data.action == KP.Common.Customer.RegisterAction.PosRegisterEdit)
                        {
                            if (StringBL.StringNullToEmpty(data.agentCode) == "" &&
                                StringBL.StringNullToEmpty(data.subAgentCode) == "")
                            {
                                if (data.tour != null && StringBL.StringNullToEmpty(data.tour.tourCode) == "")
                                {
                                    data.tour = null;
                                }
                            }
                        }


                        //valid Platform
                        var chkplatform = platFormBL.validByPlatForm(data.platformCode);
                        if (!chkplatform.Data)
                        {
                            var error = msgData.Where(t => t.MsgNo.Equals("M041")).FirstOrDefault();
                            ret.SetMessage(error.MsgCode, error.MsgDesc);
                        }

                        if (data.action == KP.Common.Customer.RegisterAction.PreRegisterAdd
                                                       || data.action == KP.Common.Customer.RegisterAction.PreRegisterEdit)
                        {
                            //valid shoppingcard
                            var chkPrefix = shoppingCardBL.validPrefixShoppingCard(data.prefixShoppingCard);
                            if (chkPrefix.Message.Count() > 0)
                            {
                                ret.Message.AddRange(chkPrefix.Message);
                            }

                            if (!chkPrefix.Data)
                            {
                                var error = msgData.Where(t => t.MsgNo.Equals("M042")).FirstOrDefault();
                                ret.SetMessage(error.MsgCode, error.MsgDesc);
                            }
                        }

                        //Check tour

                        if (data.tour != null)
                        {
                            if (data.action == KP.Common.Customer.RegisterAction.PreRegisterAdd
                                || data.action == KP.Common.Customer.RegisterAction.PreRegisterEdit)
                            {
                                if (StringBL.StringNullToEmpty(data.tour.tourCode) != "")
                                {
                                    var validTourCode = callerBL.validTourbyTourCode(data.tour.tourCode.ToUpper());
                                    if (!validTourCode.Data)
                                    {

                                        if (validTourCode.Message.Where(a => a.MessageCode.Equals("DBEX")).ToList().Count > 0)
                                        {
                                            ret.Message.AddRange(validTourCode.Message);
                                        }
                                        else
                                        {
                                            var error = msgData.Where(t => t.MsgNo.Equals("M044")).FirstOrDefault();
                                            ret.SetMessage(error.MsgCode, error.MsgDesc);
                                        }
                                    }
                                }
                            }
                            var validCountry = validNationality(data.tour.nationality.ToUpper());
                            if (!validCountry.Data)
                            {
                                if (validCountry.Message.Where(a => a.MessageCode.Equals("DBEX")).ToList().Count > 0)
                                {
                                    ret.Message.AddRange(validCountry.Message);
                                }
                                else
                                {
                                    var error = msgData.Where(t => t.MsgNo.Equals("M043")).FirstOrDefault();
                                    ret.SetMessage(error.MsgCode, error.MsgDesc);

                                }
                            }


                            //check valid Flight
                            var validateFlight = validAirlineandFlight(data.tour.airlineCode, data.tour.flightCode, data.tour.flightDate, data.tour.flightTime, data.subBranchCode, "Tour", msgData, data.platformCode);
                            if (validateFlight.Message.Count > 0)
                            {
                                ret.Message.AddRange(validateFlight.Message);
                            }


                            if (StringBL.StringNullToEmpty(data.tour.province) != "")
                            {
                                //valid Province
                                var chkProvince = validProvince(data.tour.province);
                                if (!chkProvince.Data)
                                {
                                    if (chkProvince.Message.Where(a => a.MessageCode.Equals("DBEX")).ToList().Count > 0)
                                    {
                                        ret.Message.AddRange(chkProvince.Message);
                                    }
                                    else
                                    {
                                        var error = msgData.Where(t => t.MsgNo.Equals("M082")).FirstOrDefault();
                                        ret.SetMessage(error.MsgCode, error.MsgDesc);
                                    }
                                }
                            }

                            if (StringBL.StringNullToEmpty(data.tour.cityTour) != "")
                            {
                                //valid City
                                //valid Province
                                var chkcity = validCity(data.tour.cityTour, StringBL.StringNullToEmpty(data.tour.province));
                                if (!chkcity.Data)
                                {
                                    if (chkcity.Message.Where(a => a.MessageCode.Equals("DBEX")).ToList().Count() > 0)
                                    {
                                        ret.Message.AddRange(chkcity.Message);
                                    }
                                    else
                                    {
                                        var error = msgData.Where(t => t.MsgNo.Equals("M081")).FirstOrDefault();
                                        ret.SetMessage(error.MsgCode, error.MsgDesc);
                                    }
                                }
                            }

                            if (StringBL.StringNullToEmpty(data.tour.carType) == "")
                            {
                                data.tour.carType = "N";
                            }

                            if (data.tour.tourDate != null)
                            {
                                data.tour.tourDate = data.tour.tourDate.Date;
                            }
                        }

                        //valid Customer
                        foreach (var person in data.listPersonal)
                        {
                            if (!data.isFastRegister)
                            {
                                var validCountry = validNationality(person.nationality.ToUpper());
                                if (!validCountry.Data)
                                {
                                    if (validCountry.Message.Where(a => a.MessageCode.Equals("DBEX")).ToList().Count > 0)
                                    {
                                        ret.Message.AddRange(validCountry.Message);
                                    }
                                    else
                                    {
                                        var error = msgData.Where(t => t.MsgNo.Equals("M043")).FirstOrDefault();
                                        ret.SetMessage(error.MsgCode, error.MsgDesc);
                                    }
                                }
                            }
                            if (StringBL.StringNullToEmpty(person.order_status) == "")
                            {
                                //default Order Status 
                                //A = Active
                                //B = Booking

                                person.order_status = "A";
                            }

                            if (StringBL.StringNullToEmpty(person.gender) == "")
                            {
                                person.gender = "M";
                            }

                            var customerType = StringBL.StringNullToEmpty(person.customerTypeCode);
                            var agentCode = StringBL.StringNullToEmpty(data.agentCode);
                            var subbranchCode = StringBL.StringNullToEmpty(data.subBranchCode);

                            var nationality = StringBL.StringNullToEmpty(person.nationality);

                            if (nationality != "" && data.subAgentCode != "" && (data.subAgentCode.Contains("S00") || (data.subAgentCode.Contains("S0V"))))
                            {

                                bool isMember = false;
                                if (person.listIdentity != null && person.listIdentity.Count() > 0 && person.listIdentity.Where(a => a.IdentityType == "MID").Count() > 0)
                                {
                                    isMember = true;
                                }
                                else
                                {
                                    var shopCard = "";
                                    if (person.listIdentity != null && person.listIdentity.Where(t => t.IdentityType.Equals("SHOPCARD")).Count() > 0)
                                    {
                                        shopCard = person.listIdentity.Where(a => a.IdentityType.Equals("SHOPCARD")).Select(a => a.IdentityValue).FirstOrDefault();
                                    }

                                    if (shopCard.Trim() != "")
                                    {
                                        var dbMember = db.DB_Connections.Where(x => x.ConCode.Equals(KP.Common.Customer.DatabaseType.Member)).FirstOrDefault();
                                        var dbMemberConString = string.Format("Data Source={0};Initial Catalog={1};Persist Security Info=True;User ID={2};Password={3}", dbMember.DbServer, dbMember.DbName, dbMember.DbUser, dbMember.DbPass);
                                        using (var dbMemberCon = new KP.eService.DBModels.MemberDataContext(dbMemberConString))
                                        {
                                            var chkMember = eService.Business.Member.MemberServiceBL.CheckExistMemberBy(dbMemberCon, KP.Common.Member.SearchMemberBy.MemberMagneticCard, shopCard);
                                            if (chkMember.Data != null)
                                            {
                                                isMember = true;
                                                if (person.listIdentity.Where(x => x.IdentityType.Equals("MID")).FirstOrDefault() == null)
                                                {
                                                    var identity = new Customer.ServiceModels.Identity();
                                                    identity.IdentityType = "MID";
                                                    identity.IdentityValue = chkMember.Data.MemberId.Trim();
                                                    person.listIdentity.Add(identity);
                                                }
                                                ////check Ename
                                                //if (person.englishName != chkMember.Data.EmbossName)
                                                //{
                                                //    ret.SetMessage("EX-Input", "Name Member not Match!", "WARNING");
                                                //}
                                            }
                                        }
                                    }
                                }

                                if (!(nationality == "THA" || isMember))
                                {
                                    ret.SetMessage("EX-Input", "This Nation don't allow this Guide Code SV", "WARNING");
                                }
                            }

                            if (data.agentCode != "")
                            {
                                if (customerType == "")
                                {
                                    var dataCustType = agentBL.getCustomerTypeByAgentCode(agentCode, subbranchCode);
                                    if (dataCustType.Data != "")
                                    {
                                        person.customerTypeCode = dataCustType.Data;
                                    }
                                }
                            }

                            if (StringBL.StringNullToEmpty(person.customerTypeCode) == "")
                            {
                                if (data.allowTakeAway || person.flightCode.Equals("OP000"))
                                {
                                    person.customerTypeCode = "FITT";
                                }
                                else
                                {
                                    person.customerTypeCode = "FIT";
                                }
                            }

                            //valid CustomerType
                            var validCustType = custTypeBL.validCustomerTypeCode(person.customerTypeCode);
                            if (!validCustType.Data)
                            {
                                if (validCustType.Message.Where(a => a.MessageCode.Equals("DBEX")).ToList().Count() > 0)
                                {
                                    ret.Message.AddRange(validCustType.Message);
                                }
                                else
                                {
                                    var error = msgData.Where(t => t.MsgNo.Equals("M045")).FirstOrDefault();
                                    ret.SetMessage(error.MsgCode, error.MsgDesc);
                                }
                            }

                            if (StringBL.StringNullToEmpty(person.provinceCode) != "")
                            {
                                //valid Province
                                var chkProvince = validProvince(person.provinceCode);
                                if (!chkProvince.Data)
                                {
                                    if (chkProvince.Message.Where(a => a.Equals("DBEX")).ToList().Count() > 0)
                                    {
                                        ret.Message.AddRange(chkProvince.Message);
                                    }
                                    else
                                    {
                                        var error = msgData.Where(t => t.MsgNo.Equals("M082")).FirstOrDefault();
                                        ret.SetMessage(error.MsgCode, error.MsgDesc);
                                    }
                                }
                            }

                            if (StringBL.StringNullToEmpty(person.cityCode) != "")
                            {
                                //valid City
                                //valid Province
                                var chkcity = validCity(person.cityCode, StringBL.StringNullToEmpty(person.provinceCode));
                                if (!chkcity.Data)
                                {
                                    if (chkcity.Message.Where(a => a.MessageCode.Equals("DBEX")).ToList().Count() > 0)
                                    {
                                        ret.Message.AddRange(chkcity.Message);
                                    }
                                    else
                                    {
                                        var error = msgData.Where(t => t.MsgNo.Equals("M081")).FirstOrDefault();
                                        ret.SetMessage(error.MsgCode, error.MsgDesc);
                                    }
                                }
                            }

                            bool istour = false;
                            if (data.tour != null && StringBL.StringNullToEmpty(data.tour.flightCode) != "")
                            {
                                istour = true;
                            }

                            //Special Valid for Add 
                            if (data.action == KP.Common.Customer.RegisterAction.PreRegisterAdd)
                            {
                                var validDupRegister = shoppingCardBL.validDuplicatePreRegisterByPassport(person, CallerID, data.agentCode, istour);
                                if (!validDupRegister.Data)
                                {
                                    if (validDupRegister.Message.Where(a => a.MessageCode.Equals("DBEX")).ToList().Count > 0)
                                    {
                                        ret.Message.AddRange(validDupRegister.Message);
                                    }
                                    else
                                    {
                                        var error = msgData.Where(t => t.MsgNo.Equals("M046")).FirstOrDefault();
                                        if (istour)
                                        {
                                            error = msgData.Where(t => t.MsgNo.Equals("M058")).FirstOrDefault();
                                        }
                                        ret.SetMessage(error.MsgCode, error.MsgDesc);
                                    }
                                }
                            }

                            //Special Valid for Edit
                            if (data.action == KP.Common.Customer.RegisterAction.PreRegisterEdit ||
                                data.action == KP.Common.Customer.RegisterAction.PosRegisterEdit)
                            {
                                var shoppingCard = person.listIdentity.Where(t => t.IdentityType.Equals("SHOPCARD")).Select(t => t.IdentityValue).FirstOrDefault();
                                ShoppingCardParamAPI param = new ShoppingCardParamAPI();
                                param.shoppingCard = shoppingCard;
                                if (StringBL.StringNullToEmpty(data.subBranchCode) != "")
                                {
                                    param.SubBranch = data.subBranchCode;
                                }

                                var chkShoppingCard = shoppingCardBL.CheckShoppingCardisActive(param);
                                if (!chkShoppingCard.isCompleted)
                                {
                                    var error = msgData.Where(t => t.MsgNo.Equals("M077")).FirstOrDefault();
                                    ret.SetMessage(error.MsgCode, error.MsgDesc);
                                }
                            }

                            if (!data.allowTakeAway)
                            {
                                //required passport 

                                if (data.action == KP.Common.Customer.RegisterAction.PreRegisterAdd)
                                {
                                    if (StringBL.StringNullToEmpty(person.passportNo) == "")
                                    {
                                        var error = msgData.Where(t => t.MsgNo.Equals("M047")).FirstOrDefault();
                                        ret.SetMessage(error.MsgCode, error.MsgDesc);
                                    }
                                }

                                //if (StringBL.StringNullToEmpty(data.callerID) != "" && data.tour != null)
                                //{
                                //    person.airlineCode = data.tour.airlineCode;
                                //    person.flightCode = data.tour.flightCode;
                                //    person.flightDate = data.tour.flightDate;
                                //    person.flightTime = data.tour.flightTime;
                                //}

                                //valid Flight
                                var validateFlight = validAirlineandFlight(person.airlineCode, person.flightCode, person.flightDate, person.flightTime, data.subBranchCode, "Person: ", msgData, data.platformCode);
                                if (validateFlight.Message.Count > 0)
                                {
                                    ret.Message.AddRange(validateFlight.Message);
                                }
                            }
                            else// Customer Take away is open flight
                            {
                                //if (String.IsNullOrEmpty(person.airlineCode) && String.IsNullOrEmpty(person.flightCode) && person.flightDate == null && String.IsNullOrEmpty(person.flightTime))
                                //{
                                //set default for openFlight
                                person.flightCode = "OP000";
                                person.airlineCode = "OP";
                                //person.flightRoute = "DDC-DDC";
                                person.flightDate = DateTime.Now.Date.AddDays(1);
                                person.flightTime = "00:00";
                                //}
                                //else
                                //{
                                //    //valid Flight
                                //    var validateFlight = validAirlineandFlight(person.airlineCode, person.flightCode, person.flightDate, person.flightTime, data.subBranchCode, "Person: ", msgData);
                                //    if (validateFlight.Message.Count > 0)
                                //    {
                                //        ret.Message.AddRange(validateFlight.Message);
                                //    }
                                //}
                            }
                        }
                    }
                }
            }
            return ret;
        }


        public ReturnObject<List<Customer.ServiceModels.Output>> NewValidateData(List<Customer.ServiceModels.KioskModel> model,
    List<KP.Common.Helper.CallerModel> callerAttributeList, string lang = "")
        {
            // var data = new SohdrEntity();
            var ret = new ReturnObject<List<Customer.ServiceModels.Output>>();

            var msgData = dbmsg.MAST_Messages.Where(t => t.MsgProject
            == KP.Common.Customer.MessageProject.RegisterAPI
            && t.MsgFunction == KP.Common.Customer.MessageFunction.ValidateAPI).ToList();

            var tempData = model;

            if (model.Count() == 0 || model == null)
            {
                var error = msgData.Where(t => t.MsgNo.Equals("M023")).FirstOrDefault();
                ret.SetMessage(error.MsgCode, error.MsgDesc);
            }
            else
            {
                //validate Only Input 
                foreach (var data in tempData)
                {
                    if (StringBL.StringNullToEmpty(data.Action) == "")
                    {
                        var error = msgData.Where(t => t.MsgNo.Equals("M019")).FirstOrDefault();
                        ret.SetMessage(error.MsgCode, error.MsgDesc);
                    }
                    else
                    {
                        var chkAction = validAction(data.Action, msgData);
                        if (!chkAction.Data)
                        {
                            ret.Message.AddRange(chkAction.Message);
                        }
                        else
                        {

                            if (data.Action == KP.Common.Customer.RegisterAction.PreRegisterAdd
                                || data.Action == KP.Common.Customer.RegisterAction.PosRegisterAdd)
                            {
                                //valid Platform
                                if (StringBL.StringNullToEmpty(data.SourcePlatform) == "")
                                {
                                    var error = msgData.Where(t => t.MsgNo.Equals("M024")).FirstOrDefault();
                                    ret.SetMessage(error.MsgCode, error.MsgDesc);
                                }

                                //valid UserCode
                                if (StringBL.StringNullToEmpty(data.UserCode) == "")
                                {
                                    var error = msgData.Where(t => t.MsgNo.Equals("M025")).FirstOrDefault();
                                    ret.SetMessage(error.MsgCode, error.MsgDesc);
                                }

                                //valid Prefix ShoppingCard
                                if (StringBL.StringNullToEmpty(data.PrefixShoppingCard) == "")
                                {
                                    var error = msgData.Where(t => t.MsgNo.Equals("M021")).FirstOrDefault();
                                    ret.SetMessage(error.MsgCode, error.MsgDesc);
                                }

                                if (data.Action == KP.Common.Customer.RegisterAction.PosRegisterAdd)
                                {
                                    if (StringBL.StringNullToEmpty(data.SubBranchCode) == "")
                                    {
                                        var error = msgData.Where(t => t.MsgNo.Equals("M020")).FirstOrDefault();
                                        ret.SetMessage(error.MsgCode, error.MsgDesc);
                                    }
                                }
                            }

                            // if input have agent 
                            if (StringBL.StringNullToEmpty(data.AgentCode) != "")
                            {
                                data.AgentCode = data.AgentCode.ToUpper();

                                if (data.Tour == null)
                                {
                                    var error = msgData.Where(t => t.MsgNo.Equals("M068")).FirstOrDefault();
                                    ret.SetMessage(error.MsgCode, error.MsgDesc);
                                }

                                if (data.AgentCode.Length > 0) //Case when found agent
                                {
                                    if (data.AgentCode.Length < 10 && data.AgentCode.Length > 0)
                                    {
                                        data.AgentCode = data.AgentCode.Substring(0, 1) + data.AgentCode.Substring(1).PadLeft(9, '0');
                                    };
                                }

                            }

                            // valid tour
                            if (data.Tour != null)
                            {

                                if (StringBL.StringNullToEmpty(data.Tour.TourCode) != ""
                                    && (data.Action == KP.Common.Customer.RegisterAction.PreRegisterEdit
                                    || data.Action == KP.Common.Customer.RegisterAction.PosRegisterEdit))
                                {
                                    if (StringBL.StringNullToEmpty(data.Tour.Nationality) == "")
                                    {
                                        var error = msgData.Where(t => t.MsgNo.Equals("M003")).FirstOrDefault();
                                        ret.SetMessage(error.MsgCode, error.MsgDesc);
                                    }
                                    else if (data.Tour.Nationality.Trim().Length > 3)
                                    {
                                        var error = msgData.Where(t => t.MsgNo.Equals("M026")).FirstOrDefault();
                                        ret.SetMessage(error.MsgCode, error.MsgDesc);
                                    }

                                    if (data.Tour.FlightInfo != null)
                                    {
                                        if (StringBL.StringNullToEmpty(data.Tour.FlightInfo.AirlineCode) == "")
                                        {
                                            var error = msgData.Where(t => t.MsgNo.Equals("M027")).FirstOrDefault();
                                            ret.SetMessage(error.MsgCode, error.MsgDesc);
                                        }

                                        if (StringBL.StringNullToEmpty(data.Tour.FlightInfo.FlightCode) == "")
                                        {
                                            var error = msgData.Where(t => t.MsgNo.Equals("M028")).FirstOrDefault();
                                            ret.SetMessage(error.MsgCode, error.MsgDesc);
                                        }

                                        if (StringBL.StringNullToEmpty(data.Tour.FlightInfo.FlightDate) == null)
                                        {
                                            var error = msgData.Where(t => t.MsgNo.Equals("M029")).FirstOrDefault();
                                            ret.SetMessage(error.MsgCode, error.MsgDesc);
                                        }

                                        if (StringBL.StringNullToEmpty(data.Tour.FlightInfo.FlightTime) == "")
                                        {
                                            var error = msgData.Where(t => t.MsgNo.Equals("M030")).FirstOrDefault();
                                            ret.SetMessage(error.MsgCode, error.MsgDesc);
                                        }
                                    }
                                    else
                                    {
                                        ret.SetMessage("Ex-Input", "Tour Flight Info Not Null");
                                    }

                                }


                                if (StringBL.StringNullToEmpty(data.Tour.TourCode) == "")
                                {
                                    if (data.Action == KP.Common.Customer.RegisterAction.PreRegisterEdit)
                                    {
                                        var error = msgData.Where(t => t.MsgNo.Equals("M085")).FirstOrDefault();
                                        ret.SetMessage(error.MsgCode, error.MsgDesc);
                                    }

                                    if (StringBL.StringNullToEmpty(data.Tour.Nationality) == "")
                                    {
                                        var error = msgData.Where(t => t.MsgNo.Equals("M003")).FirstOrDefault();
                                        ret.SetMessage(error.MsgCode, error.MsgDesc);
                                    }
                                    else if (data.Tour.Nationality.Trim().Length > 3)
                                    {
                                        var error = msgData.Where(t => t.MsgNo.Equals("M026")).FirstOrDefault();
                                        ret.SetMessage(error.MsgCode, error.MsgDesc);
                                    }

                                    if (StringBL.StringNullToEmpty(data.Tour.FlightInfo.AirlineCode) == "")
                                    {
                                        var error = msgData.Where(t => t.MsgNo.Equals("M027")).FirstOrDefault();
                                        ret.SetMessage(error.MsgCode, error.MsgDesc);
                                    }

                                    if (StringBL.StringNullToEmpty(data.Tour.FlightInfo.FlightCode) == "")
                                    {
                                        var error = msgData.Where(t => t.MsgNo.Equals("M028")).FirstOrDefault();
                                        ret.SetMessage(error.MsgCode, error.MsgDesc);
                                    }

                                    if (StringBL.StringNullToEmpty(data.Tour.FlightInfo.FlightDate) == null)
                                    {
                                        var error = msgData.Where(t => t.MsgNo.Equals("M029")).FirstOrDefault();
                                        ret.SetMessage(error.MsgCode, error.MsgDesc);
                                    }

                                    if (StringBL.StringNullToEmpty(data.Tour.FlightInfo.FlightTime) == "")
                                    {
                                        var error = msgData.Where(t => t.MsgNo.Equals("M030")).FirstOrDefault();
                                        ret.SetMessage(error.MsgCode, error.MsgDesc);
                                    }
                                }
                            }

                            //valid Personal
                            if (data.person == null)
                            {
                                var error = msgData.Where(t => t.MsgNo.Equals("M008")).FirstOrDefault();
                                ret.SetMessage(error.MsgCode, error.MsgDesc);
                            }
                            else
                            {
                                // foreach (var customer in data.listPersonal)
                                //{
                                if (StringBL.StringNullToEmpty(data.person.CustomerTypeCode) != "")
                                {
                                    data.person.CustomerTypeCode = data.person.CustomerTypeCode.ToUpper();
                                }

                                if (data.person.PassportNo != null && data.person.PassportNo != "")
                                {
                                    bool fHasSpace = data.person.PassportNo.Contains(" ");
                                    if (fHasSpace)
                                    {
                                        var error = msgData.Where(t => t.MsgNo.Equals("M086")).FirstOrDefault();
                                        ret.SetMessage(error.MsgCode, error.MsgDesc);
                                    }

                                    data.person.PassportNo = data.person.PassportNo.ToUpper();
                                }


                                //check English Name
                                if (StringBL.StringNullToEmpty(data.person.NameEnglish) == "")
                                {
                                    var error = msgData.Where(t => t.MsgNo.Equals("M009")).FirstOrDefault();
                                    ret.SetMessage(error.MsgCode, error.MsgDesc);
                                }
                                else if (StringBL.StringNullToEmpty(data.person.NameEnglish).Length > 100)
                                {
                                    var error = msgData.Where(t => t.MsgNo.Equals("M033")).FirstOrDefault();
                                    ret.SetMessage(error.MsgCode, error.MsgDesc);
                                }
                                else if (StringBL.StringNullToEmpty(data.person.NameEnglish) != "")
                                {
                                    if (data.person.NameEnglish.Trim().Length > 0)
                                    {
                                        data.person.NameEnglish = data.person.NameEnglish.ToUpper();
                                    }
                                }

                                //check Native Name
                                if (StringBL.StringNullToEmpty(data.person.NameNative) == "")
                                {
                                    var error = msgData.Where(t => t.MsgNo.Equals("M010")).FirstOrDefault();
                                    ret.SetMessage(error.MsgCode, error.MsgDesc);
                                }
                                else if (StringBL.StringNullToEmpty(data.person.NameNative).Length > 100)
                                {
                                    var error = msgData.Where(t => t.MsgNo.Equals("M034")).FirstOrDefault();
                                    ret.SetMessage(error.MsgCode, error.MsgDesc);
                                }
                                else if (StringBL.StringNullToEmpty(data.person.NameNative) != "")
                                {
                                    if (data.person.NameNative.Trim().Length > 0)
                                    {
                                        data.person.NameNative = data.person.NameNative.ToUpper();
                                    }
                                }

                                if (StringBL.StringNullToEmpty(data.person.Nationality) == "")
                                {
                                    var error = msgData.Where(t => t.MsgNo.Equals("M003")).FirstOrDefault();
                                    ret.SetMessage(error.MsgCode, error.MsgDesc);
                                }
                                else if (data.person.Nationality.Length > 3)
                                {
                                    var error = msgData.Where(t => t.MsgNo.Equals("M026")).FirstOrDefault();
                                    ret.SetMessage(error.MsgCode, error.MsgDesc);
                                }

                                if (!data.AllowTakeAway)
                                {
                                    if (StringBL.StringNullToEmpty(data.FlightInfo.AirlineCode) == "")
                                    {
                                        var error = msgData.Where(t => t.MsgNo.Equals("M004")).FirstOrDefault();
                                        ret.SetMessage(error.MsgCode, error.MsgDesc);
                                    }

                                    if (StringBL.StringNullToEmpty(data.FlightInfo.FlightCode) == "")
                                    {
                                        var error = msgData.Where(t => t.MsgNo.Equals("M005")).FirstOrDefault();
                                        ret.SetMessage(error.MsgCode, error.MsgDesc);
                                    }

                                    if (StringBL.StringNullToEmpty(data.FlightInfo.FlightDate) == "")
                                    {
                                        var error = msgData.Where(t => t.MsgNo.Equals("M035")).FirstOrDefault();
                                        ret.SetMessage(error.MsgCode, error.MsgDesc);
                                    }

                                    if (StringBL.StringNullToEmpty(data.FlightInfo.FlightTime) == "")
                                    {
                                        var error = msgData.Where(t => t.MsgNo.Equals("M006")).FirstOrDefault();
                                        ret.SetMessage(error.MsgCode, error.MsgDesc);
                                    }

                                }

                                //if (data.Tour == null || StringBL.StringNullToEmpty(data.Tour.FlightInfo.FlightCode) == "")
                                //{
                                if (data.person.DateOfBirth != null)
                                {

                                    var getdatetimeServer = DateTimeBL.GetNowDateTime(db);
                                    DateTime zeroTime = new DateTime(1, 1, 1);
                                    TimeSpan diffYear = getdatetimeServer.Date - data.person.DateOfBirth.Value;

                                    int years = (zeroTime + diffYear).Year - 1;


                                    if (years < 15)
                                    {
                                        //var error = msgData.Where(t => t.MsgNo.Equals("M187")).FirstOrDefault();
                                        //ret.SetMessage(error.MsgCode, error.MsgDesc);
                                        ret.SetMessage("M487", "Date Server : " + getdatetimeServer.Date.ToString("dd/MM/yyyy") + " .BirthDay : " + data.person.DateOfBirth.Value.ToString("dd/MM/yyyy") + " have " + years + " years. Under 15 Years can't Register!.");
                                    }
                                }
                                //}

                                if (data.Action == KP.Common.Customer.RegisterAction.PreRegisterAdd ||
                                    data.Action == KP.Common.Customer.RegisterAction.PreRegisterEdit)
                                {
                                    var validlistContact = validListContact(data.person.listContact, msgData, data.Action, data.person.Nationality.ToUpper());
                                    if (validlistContact.Message.Count() > 0)
                                    {
                                        ret.Message.AddRange(validlistContact.Message);
                                    }
                                }

                                if (data.person.listIdentity == null)
                                {
                                    var error = msgData.Where(t => t.MsgNo.Equals("M014")).FirstOrDefault();
                                    ret.SetMessage(error.MsgCode, error.MsgDesc);
                                }
                                else
                                {

                                    if (data.Action == KP.Common.Customer.RegisterAction.PreRegisterEdit ||
                                        data.Action == KP.Common.Customer.RegisterAction.PosRegisterEdit)
                                    {
                                        if (data.person.listIdentity.Where(t => t.IdentityType.Equals("SHOPCARD")).Count() != 1)
                                        {
                                            var error = msgData.Where(t => t.MsgNo.Equals("M076")).FirstOrDefault();
                                            ret.SetMessage(error.MsgCode, error.MsgDesc);
                                        }
                                    }

                                    foreach (var dat in data.person.listIdentity)
                                    {
                                        if (StringBL.StringNullToEmpty(dat.IdentityType) == "")
                                        {
                                            var error = msgData.Where(t => t.MsgNo.Equals("M015")).FirstOrDefault();
                                            ret.SetMessage(error.MsgCode, error.MsgDesc);
                                        }
                                        if (StringBL.StringNullToEmpty(dat.IdentityValue) == "")
                                        {
                                            var error = msgData.Where(t => t.MsgNo.Equals("M016")).FirstOrDefault();
                                            ret.SetMessage(error.MsgCode, error.MsgDesc);
                                        }
                                    }

                                    if (data.Action == KP.Common.Customer.RegisterAction.PreRegisterEdit ||
                                        data.Action == KP.Common.Customer.RegisterAction.PreRegisterAdd)
                                    {
                                        //valid listIdentity
                                        var validCheckIden = validListIdentity(data.person.listIdentity, msgData);
                                        if (!validCheckIden.Data)
                                        {
                                            ret.Message.AddRange(validCheckIden.Message);
                                        }
                                    }
                                }
                                // }
                            }
                        }//else no have action in Database
                    }//end for
                }//end else big loop


                //after Validate Input to Access validate From Db
                if (ret.Message.Count() == 0)
                {
                    var shoppingCardBL = new ShoppingCardBL(db);
                    var sourceBL = new SourceBL(db);
                    var custTypeBL = new CustomerTypeBL(db);
                    var flightBL = new FlightBL(db);
                    var callerBL = new CallerBL(db);
                    var subBranchBL = new SubbranchBL(db);
                    var agentBL = new AgentBL(db);
                    var cityBL = new CityBL(db);
                    var provinceBL = new ProvinceBL(db);
                    var subAgentBL = new SubAgentBL(db);
                    var platFormBL = new PlatFormBL(db);

                    foreach (var data in tempData)
                    {
                        string CallerID = String.Empty;
                        var validCaller = callerAttributeList;//callerBL.getCallerbyCallerID(data.callerID);
                        if (validCaller.Count == 0)
                        {
                            //Code Caller ผิด
                            var error = msgData.Where(t => t.MsgNo.Equals("M039")).FirstOrDefault();
                            ret.SetMessage(error.MsgCode, error.MsgDesc);
                        }
                        else
                        {
                            CallerID = validCaller.FirstOrDefault().callerID;
                            //data.callerID = validCaller.FirstOrDefault().callerID;
                        }

                        if (StringBL.StringNullToEmpty(data.AgentCode) == "")
                        {
                            //Code Caller ถูก ยึด Data ตาม Caller ข้างใน
                            var agentCodeCaller = validCaller.Where(n => n.attributeName == Common.Caller.CallerAttribute.AgentCode).FirstOrDefault();
                            if (agentCodeCaller != null)
                                data.AgentCode = agentCodeCaller.valueString;

                            var subAgentCode = validCaller.Where(n => n.attributeName == Common.Caller.CallerAttribute.SubAgentCode).FirstOrDefault();
                            if (subAgentCode != null)
                                data.SubAgentCode = subAgentCode.valueString;

                            var platformCode = validCaller.Where(n => n.attributeName == Common.Caller.CallerAttribute.PlatformCode).FirstOrDefault();
                            if (platformCode != null)
                                data.SourcePlatform = platformCode.valueString;

                            var prefixShoppingCard = validCaller.Where(n => n.attributeName == Common.Caller.CallerAttribute.PrefixShoppingCard).FirstOrDefault();
                            if (prefixShoppingCard != null)
                                data.PrefixShoppingCard = StringBL.StringNullToEmpty(prefixShoppingCard.valueString);

                            var allowTakeAway = validCaller.Where(n => n.attributeName == Common.Caller.CallerAttribute.IsAllowTakeAway).FirstOrDefault();
                            if (allowTakeAway != null)
                                data.AllowTakeAway = allowTakeAway.valueBoolean.Value;

                            if (data.Tour != null && data.Tour.TourCode != "")
                            {
                                //ถ้ามี Tour Code ใน CUSD_Tour จะเอา Data มาใส่ใน Tour ให้ 

                                if (data.Action == KP.Common.Customer.RegisterAction.PreRegisterAdd ||
                                    data.Action == KP.Common.Customer.RegisterAction.PosRegisterAdd)
                                {
                                    var dataTour = callerBL.getTourbyCallerorTourCode(data.Tour.TourCode, "T");
                                    if (dataTour.Data != null)
                                    {
                                        data.Tour.FlightInfo = new FlightClass();
                                        data.Tour.FlightInfo.AirlineCode = StringBL.StringNullToEmpty(dataTour.Data.airlineCode);
                                        data.Tour.BookStatus = dataTour.Data.bookStatus;
                                        data.Tour.CarType = StringBL.StringNullToEmpty(dataTour.Data.carType);
                                        data.Tour.CityTour = StringBL.StringNullToEmpty(dataTour.Data.cityCode);
                                        data.Tour.DiscountSource = StringBL.StringNullToEmpty(dataTour.Data.discountSource);
                                        data.Tour.FlightInfo.FlightCode = StringBL.StringNullToEmpty(dataTour.Data.flightCode);
                                        data.Tour.FlightInfo.FlightDate = dataTour.Data.flightDate.ToString("yyyy-MM-dd");
                                        data.Tour.FlightInfo.FlightTime = StringBL.StringNullToEmpty(dataTour.Data.flightTime);
                                        data.Tour.LicensePlate = StringBL.StringNullToEmpty(dataTour.Data.licensePlate);
                                        data.Tour.Nationality = StringBL.StringNullToEmpty(dataTour.Data.nationalityCode);
                                        data.Tour.NumberPack = dataTour.Data.numberPack;
                                        data.Tour.Province = StringBL.StringNullToEmpty(dataTour.Data.provinceCode);
                                        data.Tour.TourCode = StringBL.StringNullToEmpty(dataTour.Data.tourCode);
                                        data.Tour.TourDescription = StringBL.StringNullToEmpty(dataTour.Data.tourDescription);
                                    }
                                }
                            }
                        }
                        else if (StringBL.StringNullToEmpty(data.AgentCode) != "")
                        {
                            if (data.Tour == null)
                            {
                                var error = msgData.Where(t => t.MsgNo.Equals("M068")).FirstOrDefault();
                                ret.SetMessage(error.MsgCode, error.MsgDesc);
                            }

                            if (StringBL.StringNullToEmpty(data.SubAgentCode) == "")
                            {
                                data.SubAgentCode = "C00000000C";
                            }
                        }

                        if (StringBL.StringNullToEmpty(data.SubBranchCode) != "")
                        {
                            var validSubBranch = subBranchBL.validSubBranchCode(data.SubBranchCode.Trim().ToUpper());
                            if (!validSubBranch.Data)
                            {
                                if (validSubBranch.Message.Where(a => a.MessageCode.Equals("DBEX")).ToList().Count > 0)
                                {
                                    ret.Message.AddRange(validSubBranch.Message);
                                }
                                else
                                {
                                    var error = msgData.Where(t => t.MsgNo.Equals("M040")).FirstOrDefault();
                                    ret.SetMessage(error.MsgCode, error.MsgDesc);
                                }
                            }
                        }

                        if (StringBL.StringNullToEmpty(data.AgentCode) != "")
                        {
                            //valid Agent
                            var chkAgent = validAgent(data.AgentCode);
                            if (!chkAgent.Data)
                            {
                                if (chkAgent.Message.Where(a => a.MessageCode.Equals("DBEX")).ToList().Count > 0)
                                {
                                    ret.Message.AddRange(chkAgent.Message);
                                }
                                else
                                {
                                    var error = msgData.Where(t => t.MsgNo.Equals("M080")).FirstOrDefault();
                                    ret.SetMessage(error.MsgCode, error.MsgDesc);
                                }
                            }
                        }

                        if (StringBL.StringNullToEmpty(data.SubAgentCode) != "")
                        {
                            //valid SubAgent
                            var chkSubAgent = validSubAgent(data.AgentCode, data.SubAgentCode);
                            if (!chkSubAgent.Data)
                            {
                                if (chkSubAgent.Message.Where(a => a.MessageCode.Equals("DBEX")).ToList().Count > 0)
                                {
                                    ret.Message.AddRange(chkSubAgent.Message);
                                }

                                if (!data.SubAgentCode.Equals("C00000000C"))
                                {
                                    var error = msgData.Where(t => t.MsgNo.Equals("M083")).FirstOrDefault();
                                    ret.SetMessage(error.MsgCode, error.MsgDesc);
                                }
                            }
                        }

                        //if (StringBL.StringNullToEmpty(data.AgentCode) == "" &&
                        //    StringBL.StringNullToEmpty(data.SubAgentCode) == "")
                        //{
                        //    data.Tour = null;
                        //}

                        if (data.Action == KP.Common.Customer.RegisterAction.PosRegisterAdd ||
                            data.Action == KP.Common.Customer.RegisterAction.PosRegisterEdit)
                        {
                            if (StringBL.StringNullToEmpty(data.AgentCode) == "" &&
                                StringBL.StringNullToEmpty(data.SubAgentCode) == ""
                                )
                            {
                                if (data.Tour != null && StringBL.StringNullToEmpty(data.Tour.TourCode) == "")
                                {
                                    data.Tour = null;
                                }
                            }
                        }


                        //valid Platform
                        var chkplatform = platFormBL.validByPlatForm(data.SourcePlatform);
                        if (!chkplatform.Data)
                        {
                            var error = msgData.Where(t => t.MsgNo.Equals("M041")).FirstOrDefault();
                            ret.SetMessage(error.MsgCode, error.MsgDesc);
                        }

                        if (data.Action == KP.Common.Customer.RegisterAction.PreRegisterAdd
                                                       || data.Action == KP.Common.Customer.RegisterAction.PreRegisterEdit)
                        {
                            //valid shoppingcard
                            var chkPrefix = shoppingCardBL.validPrefixShoppingCard(data.PrefixShoppingCard);
                            if (chkPrefix.Message.Count() > 0)
                            {
                                ret.Message.AddRange(chkPrefix.Message);
                            }

                            if (!chkPrefix.Data)
                            {
                                var error = msgData.Where(t => t.MsgNo.Equals("M042")).FirstOrDefault();
                                ret.SetMessage(error.MsgCode, error.MsgDesc);
                            }
                        }

                        //tour
                        if (data.Tour != null)
                        {
                            if (data.Action == KP.Common.Customer.RegisterAction.PreRegisterAdd
                                || data.Action == KP.Common.Customer.RegisterAction.PreRegisterEdit)
                            {
                                if (StringBL.StringNullToEmpty(data.Tour.TourCode) != "")
                                {
                                    var validTourCode = callerBL.validTourbyTourCode(data.Tour.TourCode.ToUpper());
                                    if (!validTourCode.Data)
                                    {

                                        if (validTourCode.Message.Where(a => a.MessageCode.Equals("DBEX")).ToList().Count > 0)
                                        {
                                            ret.Message.AddRange(validTourCode.Message);
                                        }
                                        else
                                        {
                                            var error = msgData.Where(t => t.MsgNo.Equals("M044")).FirstOrDefault();
                                            ret.SetMessage(error.MsgCode, error.MsgDesc);
                                        }
                                    }
                                }
                            }
                            var validCountryTour = validNationality(data.Tour.Nationality.ToUpper());
                            if (!validCountryTour.Data)
                            {
                                if (validCountryTour.Message.Where(a => a.MessageCode.Equals("DBEX")).ToList().Count > 0)
                                {
                                    ret.Message.AddRange(validCountryTour.Message);
                                }
                                else
                                {
                                    var error = msgData.Where(t => t.MsgNo.Equals("M043")).FirstOrDefault();
                                    ret.SetMessage(error.MsgCode, error.MsgDesc);

                                }
                            }


                            DateTime TourDate = DateTime.Parse(data.Tour.FlightInfo.FlightDate);
                            //check valid Flight
                            var validateFlight = validAirlineandFlight(data.Tour.FlightInfo.AirlineCode, data.Tour.FlightInfo.FlightCode,
                                TourDate, data.Tour.FlightInfo.FlightTime, data.SubBranchCode, "Tour", msgData, data.SourcePlatform);
                            if (validateFlight.Message.Count > 0)
                            {
                                ret.Message.AddRange(validateFlight.Message);
                            }

                            if (StringBL.StringNullToEmpty(data.Tour.Province) != "")
                            {
                                //valid Province
                                var chkProvince = validProvince(data.Tour.Province);
                                if (!chkProvince.Data)
                                {
                                    if (chkProvince.Message.Where(a => a.MessageCode.Equals("DBEX")).ToList().Count > 0)
                                    {
                                        ret.Message.AddRange(chkProvince.Message);
                                    }
                                    else
                                    {
                                        var error = msgData.Where(t => t.MsgNo.Equals("M082")).FirstOrDefault();
                                        ret.SetMessage(error.MsgCode, error.MsgDesc);
                                    }
                                }
                            }

                            if (StringBL.StringNullToEmpty(data.Tour.CityTour) != "")
                            {
                                //valid City
                                //valid Province
                                var chkcity = validCity(data.Tour.CityTour, StringBL.StringNullToEmpty(data.Tour.Province));
                                if (!chkcity.Data)
                                {
                                    if (chkcity.Message.Where(a => a.MessageCode.Equals("DBEX")).ToList().Count() > 0)
                                    {
                                        ret.Message.AddRange(chkcity.Message);
                                    }
                                    else
                                    {
                                        var error = msgData.Where(t => t.MsgNo.Equals("M081")).FirstOrDefault();
                                        ret.SetMessage(error.MsgCode, error.MsgDesc);
                                    }
                                }
                            }

                            if (StringBL.StringNullToEmpty(data.Tour.CarType) == "")
                            {
                                data.Tour.CarType = "N";
                            }

                            if (data.Tour.TourDate != null)
                            {
                                data.Tour.TourDate = data.Tour.TourDate;
                            }
                        }

                        var validCountry = validNationality(data.person.Nationality.ToUpper());
                        if (!validCountry.Data)
                        {
                            if (validCountry.Message.Where(a => a.MessageCode.Equals("DBEX")).ToList().Count > 0)
                            {
                                ret.Message.AddRange(validCountry.Message);
                            }
                            else
                            {
                                var error = msgData.Where(t => t.MsgNo.Equals("M043")).FirstOrDefault();
                                ret.SetMessage(error.MsgCode, error.MsgDesc);
                            }
                        }

                        var nationality = StringBL.StringNullToEmpty(data.person.Nationality.ToUpper());

                        if (nationality != "" && data.SubAgentCode != "" && (data.SubAgentCode.Contains("S00") || (data.SubAgentCode.Contains("S0V"))))
                        {
                            bool isMember = false;
                            if (data.person.listIdentity != null && data.person.listIdentity.Count() > 0 && data.person.listIdentity.Where(a => a.IdentityType == "MID").Count() > 0)
                            {
                                isMember = true;
                            }
                            else
                            {
                                var shopCard = "";
                                if (data.person.listIdentity != null && data.person.listIdentity.Where(t => t.IdentityType.Equals("SHOPCARD")).Count() > 0)
                                {
                                    shopCard = data.person.listIdentity.Where(a => a.IdentityType.Equals("SHOPCARD")).Select(a => a.IdentityValue).FirstOrDefault();
                                }

                                if (shopCard.Trim() != "")
                                {
                                    var dbMember = db.DB_Connections.Where(x => x.ConCode.Equals(KP.Common.Customer.DatabaseType.Member)).FirstOrDefault();
                                    var dbMemberConString = string.Format("Data Source={0};Initial Catalog={1};Persist Security Info=True;User ID={2};Password={3}", dbMember.DbServer, dbMember.DbName, dbMember.DbUser, dbMember.DbPass);
                                    using (var dbMemberCon = new KP.eService.DBModels.MemberDataContext(dbMemberConString))
                                    {
                                        var chkMember = eService.Business.Member.MemberServiceBL.CheckExistMemberBy(dbMemberCon, KP.Common.Member.SearchMemberBy.MemberMagneticCard, shopCard);
                                        if (chkMember.Data != null)
                                        {
                                            isMember = true;
                                            if (data.person.listIdentity.Where(x => x.IdentityType.Equals("MID")).FirstOrDefault() == null)
                                            {
                                                var identity = new Customer.ServiceModels.Identity();
                                                identity.IdentityType = "MID";
                                                identity.IdentityValue = chkMember.Data.MemberId.Trim();
                                                data.person.listIdentity.Add(identity);
                                            }
                                            //if (data.person.NameEnglish != chkMember.Data.EmbossName)
                                            //{
                                            //    ret.SetMessage("EX-Input", "Name Member not Match!", "WARNING");
                                            //}
                                        }
                                    }
                                }
                            }

                            if (!(nationality == "THA" || isMember))
                            {
                                ret.SetMessage("EX-Input", "This Nation don't allow this Guide Code SV", "WARNING");
                            }
                        }


                        if (StringBL.StringNullToEmpty(data.person.Gender) == "")
                        {
                            data.person.Gender = "M";
                        }

                        var customerType = StringBL.StringNullToEmpty(data.person.CustomerTypeCode);
                        var agentCode = StringBL.StringNullToEmpty(data.AgentCode);
                        var subbranchCode = StringBL.StringNullToEmpty(data.SubBranchCode);
                        if (data.AgentCode != "")
                        {
                            if (customerType == "")
                            {
                                var dataCustType = agentBL.getCustomerTypeByAgentCode(agentCode, subbranchCode);
                                if (dataCustType.Data != "")
                                {
                                    data.person.CustomerTypeCode = dataCustType.Data;
                                }
                            }
                        }

                        if (StringBL.StringNullToEmpty(data.person.CustomerTypeCode) == "")
                        {
                            if (data.AllowTakeAway || data.FlightInfo.FlightCode.Equals("OP000"))
                            {
                                data.person.CustomerTypeCode = "FITT";
                            }
                            else
                            {
                                data.person.CustomerTypeCode = "FIT";
                            }
                        }


                        //valid CustomerType
                        var validCustType = custTypeBL.validCustomerTypeCode(data.person.CustomerTypeCode);
                        if (!validCustType.Data)
                        {
                            if (validCustType.Message.Where(a => a.MessageCode.Equals("DBEX")).ToList().Count() > 0)
                            {
                                ret.Message.AddRange(validCustType.Message);
                            }
                            else
                            {
                                var error = msgData.Where(t => t.MsgNo.Equals("M045")).FirstOrDefault();
                                ret.SetMessage(error.MsgCode, error.MsgDesc);
                            }
                        }

                        bool istour = false;
                        if (data.Tour != null && StringBL.StringNullToEmpty(data.Tour.FlightInfo.FlightCode) != "")
                        {
                            istour = true;
                        }

                        //Special Valid for Add 
                        if (data.Action == KP.Common.Customer.RegisterAction.PreRegisterAdd)
                        {

                            Personal person = new Personal()
                            {
                                customerTypeCode = data.person.CustomerTypeCode,
                                passportNo = data.person.PassportNo,
                                flightCode = data.FlightInfo.FlightCode,
                                flightDate = DateTime.Parse(data.FlightInfo.FlightDate),
                                flightTime = data.FlightInfo.FlightTime
                            };

                            var validDupRegister = shoppingCardBL.validDuplicatePreRegisterByPassport(person, CallerID, data.AgentCode, istour);
                            if (!validDupRegister.Data)
                            {
                                if (validDupRegister.Message.Where(a => a.MessageCode.Equals("DBEX")).ToList().Count > 0)
                                {
                                    ret.Message.AddRange(validDupRegister.Message);
                                }
                                else
                                {
                                    var error = msgData.Where(t => t.MsgNo.Equals("M046")).FirstOrDefault();
                                    if (istour)
                                    {
                                        error = msgData.Where(t => t.MsgNo.Equals("M058")).FirstOrDefault();
                                    }
                                    ret.SetMessage(error.MsgCode, error.MsgDesc);
                                }
                            }
                        }

                        //Special Valid for Edit
                        if (data.Action == KP.Common.Customer.RegisterAction.PreRegisterEdit ||
                            data.Action == KP.Common.Customer.RegisterAction.PosRegisterEdit)
                        {
                            var shoppingCard = data.person.listIdentity.Where(t => t.IdentityType.Equals("SHOPCARD")).Select(t => t.IdentityValue).FirstOrDefault();
                            ShoppingCardParamAPI param = new ShoppingCardParamAPI();
                            param.shoppingCard = shoppingCard;
                            if (StringBL.StringNullToEmpty(data.SubBranchCode) != "")
                            {
                                param.SubBranch = data.SubBranchCode;
                            }

                            var chkShoppingCard = shoppingCardBL.getDatafromShoppingCard(param);
                            if (!chkShoppingCard.isCompleted)
                            {
                                var error = msgData.Where(t => t.MsgNo.Equals("M077")).FirstOrDefault();
                                ret.SetMessage(error.MsgCode, error.MsgDesc);
                            }
                        }

                        if (!data.AllowTakeAway)
                        {
                            //required passport 
                            if (data.Action == KP.Common.Customer.RegisterAction.PreRegisterAdd)
                            {
                                if (StringBL.StringNullToEmpty(data.person.PassportNo) == "")
                                {
                                    var error = msgData.Where(t => t.MsgNo.Equals("M047")).FirstOrDefault();
                                    ret.SetMessage(error.MsgCode, error.MsgDesc);
                                }
                            }

                            var PersonFlightDate = DateTime.Parse(data.FlightInfo.FlightDate);

                            //valid Flight
                            var validateFlight = validAirlineandFlight(data.FlightInfo.AirlineCode, data.FlightInfo.FlightCode,
                                PersonFlightDate, data.FlightInfo.FlightTime, data.SubBranchCode, "Person: ", msgData, data.SourcePlatform);
                            if (validateFlight.Message.Count > 0)
                            {
                                ret.Message.AddRange(validateFlight.Message);
                            }
                        }
                        else// Customer Take away is open flight
                        {
                            data.FlightInfo.FlightCode = "OP000";
                            data.FlightInfo.AirlineCode = "OP";
                            //person.flightRoute = "DDC-DDC";
                            data.FlightInfo.FlightDate = DateTime.Now.Date.AddDays(1).ToString("yyyy-MM-dd");
                            data.FlightInfo.FlightTime = "00:00";
                        }
                        //}
                    }
                }
            }
            return ret;
        }


        public ReturnObject<List<OutputKiosk>> RegisterKiosk(List<Customer.ServiceModels.KioskModel> model, List<Common.Helper.CallerModel> callerAttributeList, string lang = "", string posCon = "", string pathImageShoppingCard = "")
        {
            var ret = new ReturnObject<List<OutputKiosk>>();

            try
            {
                BookingOnlineBL bookingBl = new BookingOnlineBL();
                var validate = NewValidateData(model, callerAttributeList);
                if (validate.Message.Count() > 0)
                {
                    ret.Message.AddRange(validate.Message);
                    return ret;
                }
                else
                {
                    ShoppingCardAPIBL shoppingCardAPIBL = new ShoppingCardAPIBL();
                    var customerData = validate.Data;
                    string callerID = String.Empty;
                    if (callerAttributeList.Count > 0)
                        callerID = callerAttributeList.FirstOrDefault().callerID;

                    var result = shoppingCardAPIBL.RegisterKiosk(db, model, callerAttributeList, pathImageShoppingCard);
                    if (result.Message.Count() > 0)
                    {
                        ret.Message.AddRange(result.Message);
                        ret.SetMessage("API401", "Cannot Save Data");

                    }
                    else
                    {
                        ret.Data = result.Data;
                        ret.totalCount = result.Data.Count();
                        ret.isCompleted = true;
                    }
                }
            }
            catch (Exception e)
            {
                ret.SetMessage(e);
            }

            return ret;
        }

        public ReturnObject<List<Customer.ServiceModels.Output>> RegisterAPI(List<Customer.ServiceModels.Register> model, List<KP.Common.Helper.CallerModel> callerAttributeList, string lang = "", string posCon = "")
        {
            var ret = new ReturnObject<List<Customer.ServiceModels.Output>>();
            try
            {
                Customer.Business.Register.BookingOnlineBL bookingBL = new Customer.Business.Register.BookingOnlineBL();
                var validate = ValidateData(model, callerAttributeList);
                if (validate.Message.Count > 0)
                {
                    ret.Message.AddRange(validate.Message);
                }
                else
                {
                    Customer.Business.Register.ShoppingCardAPIBL shoppingCardAPIBL = new Customer.Business.Register.ShoppingCardAPIBL();
                    var customerData = validate.Data;
                    string callerID = String.Empty;
                    if (callerAttributeList.Count > 0)
                        callerID = callerAttributeList.FirstOrDefault().callerID;

                    var result = shoppingCardAPIBL.RegisterAPI(db, model, callerAttributeList);
                    if (result.Message.Count() > 0)
                    {
                        ret.Message.AddRange(result.Message);
                        ret.SetMessage("API401", "Cannot Save Data");

                    }
                    else
                    {
                        ret.Data = result.Data;
                        ret.totalCount = result.Data.Count();
                        ret.isCompleted = true;
                    }
                }
            }
            catch (SqlException sqlex)
            {
                ret.SetMessage("API402", "Cannot connected Database");
            }
            catch (Exception ex)
            {
                ret.SetMessage("API402", ex.Message);
            }
            return ret;
        }

        public ReturnObject<ActivatPOS> ActivateAPI(ActivatPOS model, string lang = "")
        {
            string constring = CommonClass.getConnectionString(CommonClass.eConnectioName.CutomerDataConnection);
            ReturnObject<ActivatPOS> ret = new ReturnObject<ActivatPOS>();
            try
            {
                using (var db = new Customer.DBModels.CustomerDataContext(constring))
                {
                    BookingOnlineAPIBL bookingAPIBL = new BookingOnlineAPIBL();
                    DB_Connection rsDbCon = null;

                    var rsSubBrn = db.CONS_SubBranches.Where(t => t.SubBranchCode == model.register.subBranchCode).FirstOrDefault();
                    if (rsSubBrn != null)
                    {
                        rsDbCon = rsSubBrn.DB_Connections.Where(t => t.IsCancel == false && t.DbType == KP.Common.Customer.DatabaseType.POS && t.SubBranch.Equals(model.register.subBranchCode)).FirstOrDefault();
                        if (rsDbCon != null)
                        {
                            string consPos = KP.Common.Helper.DatabaseBL.GetConnectionString(rsDbCon.DbServer, rsDbCon.DbName, rsDbCon.DbUser, rsDbCon.DbPass);
                            using (POSDTDataContext posdb = new POSDTDataContext(consPos))
                            {
                                ret = bookingAPIBL.POSActivat(posdb, model);
                                ret.totalCount = ret.Data == null ? 0 : 1;
                                ret.isCompleted = true;
                            }
                        }
                    }
                    else
                    {
                        ret.SetMessage("xxx", "Subbranch not found.");
                    }
                }
            }
            catch (Exception ex)
            {
                ret.SetMessage(ex);
            }
            return ret;
        }

        public ReturnObject<ModelTour> RegisterTour(ModelTour tour, string lang = "")
        {
            ReturnObject<ModelTour> ret = new ReturnObject<ModelTour>();

            try
            {
                //string constring = CommonClass.getConnectionString(CommonClass.eConnectioName.CutomerDataConnection);
                //using (var db = new Customer.DBModels.CustomerDataContext(constring))
                //{
                BookingOnlineAPIBL bookingAPIBL = new BookingOnlineAPIBL();
                DB_Connection rsDbCon = null;

                var rsSubBrn = db.CONS_SubBranches.Where(t => t.SubBranchCode == tour.subBranch).FirstOrDefault();
                if (rsSubBrn != null)
                {
                    rsDbCon = rsSubBrn.DB_Connections.Where(t => t.IsCancel == false && t.DbType == KP.Common.Customer.DatabaseType.POS && t.SubBranch.Equals(tour.subBranch)).FirstOrDefault();
                    if (rsDbCon != null)
                    {
                        string consPos = KP.Common.Helper.DatabaseBL.GetConnectionString(rsDbCon.DbServer, rsDbCon.DbName, rsDbCon.DbUser, rsDbCon.DbPass);
                        using (POSDTDataContext posdb = new POSDTDataContext(consPos))
                        {
                            ret = bookingAPIBL.RegisterTour(posdb, tour);
                            ret.totalCount = ret.Data == null ? 0 : 1;
                            ret.isCompleted = true;
                        }
                    }
                }
                else
                {
                    ret.SetMessage("xxx", "Subbranch not found.");
                }
                //    }
            }
            catch (SqlException ex)
            {
                ret.SetMessage("DBEX", ex.Message);
            }
            catch (Exception e)
            {
                ret.SetMessage(e);
            }

            return ret;
        }

        public ReturnObject<List<PersonalPos>> GetCustomer(ShoppingCardParamAPI model, string lang = "", string pathImageShoppingCard = "")
        {
            ReturnObject<List<PersonalPos>> ret = new ReturnObject<List<PersonalPos>>();
            try
            {
                var msgData = dbmsg.MAST_Messages.Where(t => t.MsgProject
          == KP.Common.Customer.MessageProject.RegisterAPI
          && t.MsgFunction == KP.Common.Customer.MessageFunction.ValidateAPI).ToList();
                if (StringBL.StringNullToEmpty(model.shoppingCard) == "")
                {
                    var error = msgData.Where(t => t.MsgNo.Equals("M076")).FirstOrDefault();
                    ret.SetMessage(error.MsgCode, error.MsgDesc);
                }

                if (ret.Message.Count == 0)
                {

                    ShoppingCardBL shoppingBL = new ShoppingCardBL(db);
                    if (StringBL.StringNullToEmpty(model.pickupCode) != "" && StringBL.StringNullToEmpty(model.SubBranch) != "")
                    {
                        ret = shoppingBL.getDataPickup(model, pathImageShoppingCard);
                        ret.totalCount = ret.Data.Count();
                        ret.isCompleted = true;
                    }
                    else
                    {
                        ret = shoppingBL.getDatafromShoppingCard(model, pathImageShoppingCard);
                        ret.totalCount = ret.Data != null ? ret.Data.Count() : 0;
                        ret.isCompleted = ret.Data == null ? false : true;
                    }
                }
            }
            catch (Exception ex)
            {
                ret.SetMessage(ex);
            }
            return ret;
        }

        public ReturnObject<bool> validAirlineandFlight(string airline, string flightCode, DateTime? flightDate, string flightTime, string subbranchCode, string type, List<MAST_Message> msgData, string platform)
        {
            ReturnObject<bool> ret = new ReturnObject<bool>();

            try
            {
                //validate Airline
                var flightBl = new FlightBL(db);

                //check Airline in db
                var validAirline = flightBl.SearchAirline(airline);
                if (validAirline.Message.Count > 0)
                {
                    ret.Message.AddRange(validAirline.Message);
                }
                if (validAirline.Data.Count() == 0)
                {
                    MAST_Message error = msgData.Where(t => t.MsgNo.Equals("M048")).FirstOrDefault();
                    if (type.Equals("Tour"))
                    {
                        error = msgData.Where(t => t.MsgNo.Equals("M049")).FirstOrDefault();
                    }

                    ret.SetMessage(error.MsgCode, error.MsgDesc);
                }

                //valid FlightCode
                var validflight = flightBl.SearchFlight(subbranchCode, airline, flightCode);
                if (validflight.Message.Count > 0)
                {
                    ret.Message.AddRange(validflight.Message);
                }
                if (validflight.Data.Count() == 0)
                {
                    MAST_Message error = msgData.Where(t => t.MsgNo.Equals("M050")).FirstOrDefault();
                    if (type.Equals("Tour"))
                    {
                        error = msgData.Where(t => t.MsgNo.Equals("M051")).FirstOrDefault();
                    }
                    ret.SetMessage(error.MsgCode, error.MsgDesc);
                }

                //format 16:00 
                int findColon = flightTime.IndexOf(":");
                if (findColon > 0)
                {
                    string[] time = flightTime.Split(':');
                    flightDate = flightDate.Value.AddHours(int.Parse(time[0])).AddMinutes(int.Parse(time[1]));
                }
                else
                {
                    //another format  example 1600,1630
                    if (!(flightTime.Length == 4))
                    {
                        MAST_Message error = msgData.Where(t => t.MsgNo.Equals("M052")).FirstOrDefault();
                        if (type.Equals("Tour"))
                        {
                            error = msgData.Where(t => t.MsgNo.Equals("M053")).FirstOrDefault();
                        }
                        ret.SetMessage(error.MsgCode, error.MsgDesc);
                    }
                    else if (!Regex.IsMatch(flightTime, @"\d"))
                    {
                        MAST_Message error = msgData.Where(t => t.MsgNo.Equals("M054")).FirstOrDefault();
                        if (type.Equals("Tour"))
                        {
                            error = msgData.Where(t => t.MsgNo.Equals("M055")).FirstOrDefault();
                        }
                        ret.SetMessage(error.MsgCode, error.MsgDesc);
                    }
                    else
                    {
                        //format time 1600,1500
                        flightDate = flightDate.Value.AddHours(int.Parse(flightTime.Substring(0, 2))).AddMinutes(int.Parse(flightTime.Substring(2, 2)));
                    }
                }//not tour // Customer  Flight Time include in DateTime
                 //validate flight

                if (!StringBL.StringNullToEmpty(airline).Equals("OP") && !StringBL.StringNullToEmpty(flightCode).Equals("OP000"))
                {
                    if (platform.ToUpper() != "FRMTOUR" || platform.ToUpper() != "FRMFIT")
                    {
                        var limitFlight = flightBl.CheckIsLimitFlight(subbranchCode, flightDate.Value);
                        if (limitFlight.Message.Count > 0)
                        {
                            ret.Message.AddRange(limitFlight.Message);
                        }
                    }
                }


                var validFlight = flightBl.ValidateFlight(subbranchCode, flightCode, flightDate.Value);
                if (validflight.Message.Count() > 0)
                {
                    ret.Message.AddRange(validflight.Message);
                }
                if (!validFlight.Data)
                {
                    MAST_Message error = msgData.Where(t => t.MsgNo.Equals("M069")).FirstOrDefault();
                    if (type.Equals("Tour"))
                    {
                        error = msgData.Where(t => t.MsgNo.Equals("M070")).FirstOrDefault();
                    }
                    ret.SetMessage(error.MsgCode, error.MsgDesc);
                }
            }
            catch (Exception ex)
            {
                ret.SetMessage("500", ex.Message);
            }
            return ret;
        }

        public ReturnObject<bool> validPlatForm(string platform)
        {
            ReturnObject<bool> ret = new ReturnObject<bool>();
            ret.Data = true;
            try
            {
                //check in Database valid Country Code is found in Database
                var platformBL = new PlatFormBL(db);
                var chkValidPlatForm = platformBL.validByPlatForm(platform);
                if (!chkValidPlatForm.Data)
                {
                    ret.Data = false;
                }
                if (chkValidPlatForm.Message.Count() > 0)
                {
                    ret.Message.AddRange(chkValidPlatForm.Message);
                }
            }
            catch (Exception ex)
            {
                ret.SetMessage(ex);
            }

            return ret;
        }

        public ReturnObject<bool> validNationality(string nationality)
        {
            ReturnObject<bool> ret = new ReturnObject<bool>();
            ret.Data = true;
            try
            {
                //check in Database valid Country Code is found in Database
                var countryBL = new CountryBL(db);
                var chkValidCountry = countryBL.validByCountryCode(nationality);
                if (!chkValidCountry.Data)
                {
                    ret.Data = false;
                }
                if (chkValidCountry.Message.Count() > 0)
                {
                    ret.Message = chkValidCountry.Message;
                }
            }
            catch (Exception ex)
            {
                ret.SetMessage(ex);
            }

            return ret;
        }

        public ReturnObject<bool> validAgent(string agentCode)
        {
            ReturnObject<bool> ret = new ReturnObject<bool>();
            ret.Data = true;

            try
            {
                //check in Database valid Country Code is found in Database
                var agentBL = new AgentBL(db);
                var chkValidAgent = agentBL.getAgentByCode(agentCode);
                if (chkValidAgent.Data == null)
                {
                    ret.Data = false;
                }
                if (chkValidAgent.Message.Count() > 0)
                {
                    ret.Message = chkValidAgent.Message;
                }
            }
            catch (Exception ex)
            {
                ret.SetMessage(ex);
            }

            return ret;
        }

        public ReturnObject<string> getCustomerTypeByAgent(string agentCode, string subbranch = "")
        {
            ReturnObject<string> ret = new ReturnObject<string>();
            ret.Data = "";
            try
            {
                var agentBL = new AgentBL(db);
                var getCustomerTypeCode = agentBL.getCustomerTypeByAgentCode(agentCode, subbranch);
                if (getCustomerTypeCode.Data != null)
                {
                    ret.Data = StringBL.StringNullToEmpty(getCustomerTypeCode.Data);
                }

            }
            catch (Exception e)
            {
                ret.SetMessage(e);
            }

            return ret;
        }

        public ReturnObject<bool> validCity(string cityCode, string province)
        {
            ReturnObject<bool> ret = new ReturnObject<bool>();
            ret.Data = true;

            try
            {
                //check in Database valid Country Code is found in Database
                var cityBL = new CityBL(db);
                var chkValidCity = cityBL.getCityByCityCode(cityCode, province);
                if (chkValidCity.Data == null)
                {
                    ret.Data = false;
                }
                if (chkValidCity.Message.Count > 0)
                {
                    ret.Message = chkValidCity.Message;
                }
            }
            catch (Exception ex)
            {
                ret.SetMessage(ex);
            }

            return ret;
        }

        public ReturnObject<bool> validProvince(string provinceCode)
        {
            ReturnObject<bool> ret = new ReturnObject<bool>();
            ret.Data = true;

            try
            {
                //check in Database valid Country Code is found in Database
                var provinceBL = new ProvinceBL(db);
                var chkValidProvince = provinceBL.getProvinceByCode(provinceCode);
                if (chkValidProvince.Data == null)
                {
                    ret.Data = false;
                }
                if (chkValidProvince.Message.Count() > 0)
                {
                    ret.Message = chkValidProvince.Message;
                }
            }
            catch (Exception ex)
            {
                ret.SetMessage(ex);
            }

            return ret;
        }

        public ReturnObject<bool> validSubAgent(string agentCode, string subAgentCode)
        {
            ReturnObject<bool> ret = new ReturnObject<bool>();
            ret.Data = true;

            try
            {
                //check in Database valid Country Code is found in Database
                var subAgentBL = new SubAgentBL(db);
                var chkValidSubAgent = subAgentBL.getSubAgentByCode(agentCode, subAgentCode);
                if (chkValidSubAgent.Data == null)
                {
                    ret.Data = false;
                }
                if (chkValidSubAgent.Message.Count() > 0)
                {
                    ret.Message = chkValidSubAgent.Message;
                }
            }
            catch (Exception ex)
            {
                ret.SetMessage(ex);
            }

            return ret;
        }

        public ReturnObject<bool> validListContact(List<Contact> listContact, List<MAST_Message> msgData, string action, string nationality = "")
        {
            ReturnObject<bool> ret = new ReturnObject<bool>();
            ret.Data = true;
            try
            {
                //check chinese

                if (action == KP.Common.Customer.RegisterAction.PreRegisterAdd ||
                    action == KP.Common.Customer.RegisterAction.PreRegisterEdit)
                {
                    if (StringBL.StringNullToEmpty(nationality) == "CHN")
                    {
                        if (listContact.Where(t => t.contactType.ToUpper() == ContactRegister.WECHAT.ToString()).FirstOrDefault() == null)
                        {
                            var error = msgData.Where(t => t.MsgNo.Equals("M036")).FirstOrDefault();
                            ret.SetMessage(error.MsgCode, error.MsgDesc);
                        }

                        var chkdata = listContact.Where(t => t.contactType.ToUpper() == ContactRegister.WECHAT.ToString()).FirstOrDefault();
                        if (chkdata == null || StringBL.StringNullToEmpty(chkdata.contactValue) == "")
                        {
                            var error = msgData.Where(t => t.MsgNo.Equals("M037")).FirstOrDefault();
                            ret.SetMessage(error.MsgCode, error.MsgDesc);
                        }
                    }
                }

                Type contactField = typeof(Identity.ContactRegister);
                FieldInfo[] fields = contactField.GetFields(BindingFlags.Static | BindingFlags.Public);

                if (listContact.GroupBy(n => n.contactType).Any(c => c.Count() > 1))
                {
                    var error = msgData.Where(t => t.MsgNo.Equals("M071")).FirstOrDefault();
                    ret.SetMessage(error.MsgCode, error.MsgDesc);
                }

                foreach (var chkContact in listContact)
                {
                    //check Static string in Database
                    var chk = fields.Where(t => t.Name == chkContact.contactType);
                    if (chk.Count() == 0)
                    {
                        var error = msgData.Where(t => t.MsgNo.Equals("M038")).FirstOrDefault();
                        ret.SetMessage(error.MsgCode, error.MsgDesc);
                    }
                }
                if (ret.Message.Count > 0)
                {
                    ret.Data = false;
                }

            }
            catch (Exception ex)
            {
                ret.SetMessage(ex);
            }
            return ret;
        }

        public ReturnObject<bool> validPlatformCode(string platform)
        {
            ReturnObject<bool> ret = new ReturnObject<bool>();
            ret.Data = true;
            try
            {
                Type platformFields = typeof(PlatformCode);
                FieldInfo[] fields = platformFields.GetFields(BindingFlags.Static | BindingFlags.Public);

                var chk = fields.Where(t => t.Name == platform);
                if (chk.Count() == 0)
                {
                    ret.Data = false;
                }
            }
            catch (Exception ex)
            {
                ret.SetMessage(ex);
            }

            return ret;
        }

        public ReturnObject<bool> validAction(string actionCode, List<MAST_Message> msgData)
        {
            ReturnObject<bool> ret = new ReturnObject<bool>();
            ret.Data = true;
            try
            {
                Type actionList = typeof(Identity.ActionCode);
                FieldInfo[] fields = actionList.GetFields(BindingFlags.Static | BindingFlags.Public);

                //check Static string in Database
                var chk = fields.Where(t => t.Name == actionCode);
                if (chk.Count() == 0)
                {
                    ret.Data = false;
                    var error = msgData.Where(t => t.MsgNo.Equals("M078")).FirstOrDefault();
                    ret.SetMessage(error.MsgCode, error.MsgDesc);
                }
            }
            catch (Exception ex)
            {
                ret.SetMessage(ex);
            }
            return ret;
        }

        public ReturnObject<bool> validListIdentity(List<Customer.ServiceModels.Identity> listIdentity, List<MAST_Message> msgData)
        {
            ReturnObject<bool> ret = new ReturnObject<bool>();
            ret.Data = true;
            try
            {
                Type idenfields = typeof(Identity.Identity);
                FieldInfo[] fields = idenfields.GetFields(BindingFlags.Static | BindingFlags.Public);

                //var chkdup = listIdentity.Where(t => fields.Select(s => s.Name).Contains(t.IdentityType)).ToList();
                //if (chkdup.Count() > 1)
                //{
                //    var error = msgData.Where(t => t.MsgNo.Equals("M072")).FirstOrDefault();
                //    ret.SetMessage(error.MsgCode, error.MsgDesc);
                //}


                if (listIdentity.GroupBy(n => n.IdentityType).Any(c => c.Count() > 1))
                {
                    var error = msgData.Where(t => t.MsgNo.Equals("M072")).FirstOrDefault();
                    ret.SetMessage(error.MsgCode, error.MsgDesc);
                }

                foreach (var chkiden in listIdentity)
                {
                    //check Static string in Database
                    var chk = fields.Where(t => t.Name == chkiden.IdentityType);
                    if (chk.Count() == 0)
                    {
                        ret.Data = false;
                        var error = msgData.Where(t => t.MsgNo.Equals("M073")).FirstOrDefault();
                        ret.SetMessage(error.MsgCode, error.MsgDesc);
                    }

                }
            }
            catch (Exception ex)
            {
                ret.SetMessage(ex);
            }
            return ret;
        }

        public ReturnObject<List<SettingConfigs>> GetSettingConfig()
        {
            ReturnObject<List<SettingConfigs>> ret = new ReturnObject<List<SettingConfigs>>();

            try
            {
                var result = from p in db.CONS_PathUrls
                             where p.Flag == true
                             select new SettingConfigs
                             {
                                 method = p.Module.Trim(),
                                 Url = p.PathUrl.Trim()
                             };

                if (result.Count() > 0)
                {
                    ret.Data = result.ToList();
                }
            }
            catch (Exception e)
            {
                ret.SetMessage(e);
            }

            return ret;
        }

        ReturnObject<OutputAgent> IRegister.GetCustomerTypeByAgentCodeDesc(InputAgent data)
        {
            ReturnObject<OutputAgent> ret = new ReturnObject<OutputAgent>();
            ret.Data = new OutputAgent();
            try
            {
                ret.Data.CustomerTypeCode = "";
                ret.Data.CustomerTypeDesc = "";
                var agentBL = new AgentBL(db);
                var getCustomerTypeCode = agentBL.getCustomerTypeByAgentCodeDesc(data.AgentCode, data.subBranchCode);
                if (getCustomerTypeCode.Data != null)
                {
                    ret.Data = getCustomerTypeCode.Data;
                }

            }
            catch (Exception e)
            {
                ret.SetMessage(e);
            }

            return ret;
        }


        public ReturnObject<OutputShipping> getShipping(string sessionID)
        {
            ReturnObject<OutputShipping> ret = new ReturnObject<OutputShipping>();

            try
            {

            }
            catch (Exception ex)
            {
                ret.SetMessage("Exception", ex.Message);
            }
            return ret;
        }

        public ReturnObject<bool> CheckValidSVCode(InputValidSubAgent scCode)
        {
            ReturnObject<bool> ret = new ReturnObject<bool>();

            try
            {
                var checkSubBranch = StringBL.StringNullToEmpty(scCode.SubBranchCode);
                if(checkSubBranch == "")
                {
                    ret.SetMessage("Ex-Input","Input Subbranch is Required!.");
                    return ret;
                }

                var checkSv = StringBL.StringNullToEmpty(scCode.SvCode);
                if(checkSv=="")
                {
                    ret.SetMessage("Ex-Input", "Input Sv Code is Required!.");
                    return ret;
                }


                if (checkSv.Length < 10 && checkSv.Length > 0)
                {
                    checkSv = checkSv.Substring(0, 1) + checkSv.Substring(1).PadLeft(9, '0');
                };

                var checkSVFromPos = new KP.Customer.Business.Agent.AgentBL().ValidSVCode(db, checkSubBranch, checkSv);
                ret = checkSVFromPos;
            }
            catch (Exception ex)
            {
                ret.SetMessage("Ex-Operation", ex.Message);
            }

            return ret;
        }
    }
}
