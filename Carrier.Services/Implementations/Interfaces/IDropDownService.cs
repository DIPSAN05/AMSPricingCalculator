using Carrier.DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Carrier.Services.Implementations.Interfaces
{
    public interface IDropDownService
    {
        Task<List<TechnologyCluster>> GetTechnologyClusters();
        Task<List<BusinessCriticality>> GetBusinessCriticalities();   
        Task<List<ApplicationComplexity>> GetApplicationComplexities();
        Task<List<ApplicationMaturity>> GetApplicationMaturities();
    }
}
