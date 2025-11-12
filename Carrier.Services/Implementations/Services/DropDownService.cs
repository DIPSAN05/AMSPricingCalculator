using Carrier.DataAccessLayer.Models;
using Carrier.Services.Implementations.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Carrier.Services.Implementations.Services
{
    public class DropDownService : IDropDownService
    {
        private readonly CarrierDbContext _dbContext;

        public DropDownService(CarrierDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<List<ApplicationComplexity>> GetApplicationComplexities()
        {
            var applicationComplexities = await _dbContext.ApplicationComplexities.ToListAsync();

            if(applicationComplexities!=null)
                return applicationComplexities;
            return new List<ApplicationComplexity>();
        }

        public async Task<List<ApplicationMaturity>> GetApplicationMaturities()
        {
            var applicationMaturities = await _dbContext.ApplicationMaturities.ToListAsync();

            if (applicationMaturities != null)
                return applicationMaturities;
            return new List<ApplicationMaturity>();
        }

        public async Task<List<BusinessCriticality>> GetBusinessCriticalities()
        {
            var businessCriticalities = await _dbContext.BusinessCriticalities.ToListAsync();

            if (businessCriticalities != null)
                return businessCriticalities;

            return new List<BusinessCriticality>();
        }

        public async Task<List<TechnologyCluster>> GetTechnologyClusters()
        {
            var technologyClusters = await _dbContext.TechnologyClusters.ToListAsync();

            if(technologyClusters != null)
                return technologyClusters;  

            return new List<TechnologyCluster>();
        }
    }
}
