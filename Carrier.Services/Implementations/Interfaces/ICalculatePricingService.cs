
using Carrier.DataAccessLayer.Models;
using Carrier.Services.RequestModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Carrier.Services.Implementations.Interfaces
{
    public interface ICalculatePricingService
    {
        public Task<List<List<decimal?>>> GetPrice(List<CarrierInputData> carrierInput);
        public bool login(UserInput userInput);
        public List<TblDisclaimer> GetDisclamair();
    }
}
