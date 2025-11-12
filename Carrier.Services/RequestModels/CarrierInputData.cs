using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Carrier.Services.RequestModels
{
    public class CarrierInputData
    {
        //public string AppReferenceId { get; set; }
        public string ApplicationName { get; set; }
        public string TechnologyCluster { get; set; }
        public int IncidentPerMonth { get; set; }
        public int ServiceRequestPerMonth { get; set; }
        public int UAPTicketShareRequest { get; set; }
        public int EnhancementHoursPerMonth { get; set; }
        public string BusinessCriticality { get; set; }
        public int UsersCount { get; set; }
        public string ApplicationMaturity { get; set; }
        public string ApplicationComplexity { get; set; }
        public string SupportCoverage { get; set; }
        public int LanguageCount { get; set; }
        public int LanguageUsers { get; set; }
    }
}
