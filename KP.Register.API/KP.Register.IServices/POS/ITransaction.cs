using KP.Common.Return;
using KP.Customer.ServiceModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KP.Register.IServices.POS
{
    public interface ITransaction
    {
        ReturnObject<OutputTransaction> GetSumPurchase(InputTransaction input);
    }
}
