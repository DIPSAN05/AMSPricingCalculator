using Carrier.DataAccessLayer.Models;
using Carrier.Services.Implementations.Interfaces;
using Carrier.Services.RequestModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Carrier.Services.Implementations.Services
{
    public class CalculatePricingService : ICalculatePricingService
    {
        private readonly CarrierDbContext _dbContext;

        public CalculatePricingService(CarrierDbContext dbContext)
        {
             _dbContext = dbContext;
        }
        public async Task<List<List<decimal?>>> GetPrice(List<CarrierInputData> carrierInputs)
        {
            //var Yr1Cost =(decimal) 0;
            //var Yr2Cost = (decimal)0;
            //var Yr3Cost = (decimal)0;
           
            List<List<decimal?>> result = new List<List<decimal?>>();
            foreach (var carrierInput in carrierInputs)
            {
                List<decimal?> Yr4Cost = new List<decimal?>();
                var UAPTicketShareRequest = (decimal)(carrierInput.UAPTicketShareRequest) / 100;
                var iProdUAP = _dbContext.TblProductivities.Where(
                    x => x.ApplicationType == carrierInput.TechnologyCluster && x.Type.ToLower() == "UAP".ToLower()
                    ).Select(x => x.Value).SingleOrDefault();
                var iProdSr = _dbContext.TblProductivities.Where(
                    x => x.ApplicationType == carrierInput.TechnologyCluster && x.Type.ToLower() == "sr"
                    ).Select(x => x.Value).SingleOrDefault();

                var iProdInc15 = _dbContext.TblProductivities.Where(
                    x => x.ApplicationType == carrierInput.TechnologyCluster && x.Type.ToLower() == "l1.5"
                    ).Select(x => x.Value).SingleOrDefault();

                var iProdInc2 = _dbContext.TblProductivities.Where(
                    x => x.ApplicationType == carrierInput.TechnologyCluster && x.Type.ToLower() == "l2-l3"
                ).Select(x => x.Value).SingleOrDefault();

                var iDist1 = _dbContext.TblTicketDistributions.Where(
                    x => x.TickitType.ToLower() == "l1.5" && x.Application == carrierInput.TechnologyCluster
                    ).Select(x => x.Value).SingleOrDefault();

                var iDist2 = _dbContext.TblTicketDistributions.Where(
                    x => x.TickitType.ToLower() == "l2-l3" && x.Application == carrierInput.TechnologyCluster
                    ).Select(x => x.Value).SingleOrDefault();

                var iProdInc15Dist = iDist1 / (iDist1 + iDist2);
                var iProdInc2Dist = iDist2 / (iDist1 + iDist2);


                //var iBaseFTE = (carrierInput.IncidentPerMonth * iProdInc15Dist / iProdInc15) +
                //               (carrierInput.IncidentPerMonth * iProdInc2Dist / iProdInc2) +
                //               (carrierInput.ServiceRequestPerMonth / iProdSr) +
                //               ((decimal)(carrierInput.EnhancementHoursPerMonth / (21 * 8.5)));

                var iBaseFTE = (carrierInput.IncidentPerMonth * iProdInc15Dist / iProdInc15) +
                               (carrierInput.IncidentPerMonth * iProdInc2Dist / iProdInc2) +
                               (carrierInput.ServiceRequestPerMonth * (1 - UAPTicketShareRequest) / iProdSr) +
                               (carrierInput.ServiceRequestPerMonth * (UAPTicketShareRequest) / iProdUAP); //+(carrierInput.EnhancementHoursPerMonth / (21 * (decimal)8.5));
                // var iBaseFTE =(iSRs * (1 - iUAP) / iProdSR) + (iSRs * (iUAP) / iProdUAP) + (iEnhHrs / (21 * 8.5))



                var sBusCriticalityFactor = _dbContext.BusinessCriticalities.
                    Where(x => x.CriticalityName == carrierInput.BusinessCriticality).
                    Select(x => x.AdjFactor).SingleOrDefault();

                //var sUserCountFactor = _dbContext.TblAdjustmentFactors.
                //    Where(x => x.Value.Equals(carrierInput.UsersCount.ToString()) && x.Type == "user_cnt").
                //    Select(x => x.AdjFactor).SingleOrDefault();

                var sMaturityFactor = _dbContext.TblAdjustmentFactors.
                    Where(x => x.Value == carrierInput.ApplicationMaturity && x.Type == "App_maturity").
                    Select(x => x.AdjFactor).SingleOrDefault();

                var sComplexityFactor = _dbContext.TblAdjustmentFactors.
                    Where(x => x.Value == carrierInput.ApplicationComplexity && x.Type == "app_complexity").
                    Select(x => x.AdjFactor).SingleOrDefault();

                var sCoverageFactor = _dbContext.TblAdjustmentFactors.
                    Where(x => x.Value == carrierInput.SupportCoverage && x.Type == "supp_coverage").
                    Select(x => x.AdjFactor).SingleOrDefault();

                //var sLanguageCountFactor = _dbContext.TblAdjustmentFactors.
                //    Where(x => x.Value.Equals(carrierInput.LanguageCount.ToString()) && x.Type == "language").
                //    Select(x => x.AdjFactor).SingleOrDefault();
                //var sGovernanceFactor =  (_dbContext.TblProductivities.Where(
                //    x => x.ApplicationType == carrierInput.TechnologyCluster && x.Type == "Govfactor"
                //    ).Select(x => x.Value).SingleOrDefault())/100;
                var sGovernanceFactor = (decimal)(_dbContext.TblProductivities.Where(
          x => x.ApplicationType == carrierInput.TechnologyCluster && x.Type.ToLower() == "Govfactor"
          ).Select(x => x.Value).SingleOrDefault()) / 100;

                var InnovationFactor = (decimal)(_dbContext.TblProductivities.Where(
         x => x.ApplicationType == carrierInput.TechnologyCluster && x.Type.ToLower() == "Innovation"
         ).Select(x => x.Value).SingleOrDefault()) / 100;

                var Infrastructure = (decimal)(_dbContext.TblProductivities.Where(
        x => x.ApplicationType == carrierInput.TechnologyCluster && x.Type.ToLower() == "Infrastructure"
        ).Select(x => x.Value).SingleOrDefault());

                //var sLanguageUsersFactor = _dbContext.TblAdjustmentFactors.
                //    Where(x => x.Value.Equals(carrierInput.LanguageUsers.ToString()) && x.Type == "lang_user_cnt").
                //    Select(x => x.AdjFactor).SingleOrDefault();


                var iAdjFactor = sBusCriticalityFactor + sMaturityFactor + sComplexityFactor +
                                 sCoverageFactor + sGovernanceFactor+ InnovationFactor;
                //
               // var y = Convert.ToDecimal(carrierInput.EnhancementHoursPerMonth )/ (20 * 8) ;
                var iFinalFTE = iBaseFTE * (1 + iAdjFactor) + ((Convert.ToDecimal(carrierInput.EnhancementHoursPerMonth) / (20 * 8)) * (1 + sGovernanceFactor)); /// (20 * 8)) * (1 + sGovernanceFactor))


                var Yr2PI = _dbContext.TblYoyImprovements.Where(x => x.Years == "Y1").Select(x => x.Value).SingleOrDefault();
                var Yr3PI = _dbContext.TblYoyImprovements.Where(x => x.Years == "Y2").Select(x => x.Value).SingleOrDefault();
               
                var Yr4PI = _dbContext.TblYoyImprovements.Where(x => x.Years == "Y3").Select(x => x.Value).SingleOrDefault();
                var Yr5PI = _dbContext.TblYoyImprovements.Where(x => x.Years == "Y4").Select(x => x.Value).SingleOrDefault();

                var Yr1Onsite = (decimal)(_dbContext.TblOnsiteRatios.Where(x => x.Year == "Y1").Select(x => x.Value).SingleOrDefault()) / 100;
                var Yr2Onsite = (decimal)(_dbContext.TblOnsiteRatios.Where(x => x.Year == "Y2").Select(x => x.Value).SingleOrDefault()) / 100;
                var Yr3Onsite = (decimal)(_dbContext.TblOnsiteRatios.Where(x => x.Year == "Y3").Select(x => x.Value).SingleOrDefault()) / 100;
               
                var Yr4Onsite = (decimal)(_dbContext.TblOnsiteRatios.Where(x => x.Year == "Y4").Select(x => x.Value).SingleOrDefault()) / 100;
                var Yr5Onsite = (decimal)(_dbContext.TblOnsiteRatios.Where(x => x.Year == "Y5").Select(x => x.Value).SingleOrDefault()) / 100;
                // Confused
                var onSite = _dbContext.TblProductivities.Where(
                    x => x.ApplicationType == carrierInput.TechnologyCluster && x.Type.ToLower() == "Onsite Blended Rate"
                ).Select(x => x.Value).SingleOrDefault();

                var offSite = _dbContext.TblProductivities.Where(
                    x => x.ApplicationType == carrierInput.TechnologyCluster && x.Type.ToLower() == "Offshore Blended Rate"
                ).Select(x => x.Value).SingleOrDefault();

                double OffYr1COLA = 1;
                double OffYr2COLA = 1;
                double OffYr3COLA = 1;
                double OffYr4COLA = 1;
                double OffYr5COLA = 1;

                double OnYr1COLA = 1;
                double OnYr2COLA = 1;
                double OnYr3COLA = 1;
                double OnYr4COLA = 1;
                double OnYr5COLA = 1;
                var COLAYr = Convert.ToInt32(_dbContext.TblColas.Where(
                     x => x.ItemKey.ToLower() == "COLAYr"
                 ).Select(x => x.Value).SingleOrDefault());

                var OnsiteCOLA = Convert.ToDouble(_dbContext.TblColas.Where(
                    x => x.ItemKey.ToLower() == "COLAOnsite"
                ).Select(x => x.Value).SingleOrDefault()) / 100;

                var OffshoreCOLA = Convert.ToDouble((_dbContext.TblColas.Where(
                    x => x.ItemKey == "COLAOffshore"
                ).Select(x => x.Value).SingleOrDefault())) / 100;

                var sCurrYr = System.DateTime.Now.Year;

                for (int iYrIdx = COLAYr; iYrIdx <= sCurrYr; iYrIdx++)
                {
                    OffYr1COLA = (OffYr1COLA * (1 + OffshoreCOLA));
                    OnYr1COLA = OnYr1COLA * (1 + OnsiteCOLA);
                }
                OffYr2COLA = OffYr1COLA * (1 + OffshoreCOLA);
                OnYr2COLA = OnYr1COLA * (1 + OnsiteCOLA);

                OffYr3COLA = OffYr2COLA * (1 + OffshoreCOLA);
                OnYr3COLA = OnYr2COLA * (1 + OnsiteCOLA);

                OffYr4COLA = OffYr3COLA * (1 + OffshoreCOLA);
                OnYr4COLA = OnYr3COLA * (1 + OnsiteCOLA);

                OffYr5COLA = OffYr4COLA * (1 + OffshoreCOLA);
                OnYr5COLA = OnYr4COLA * (1 + OnsiteCOLA);

                //var Yr1Cost = (Yr1Onsite * iFinalFTE * 21 * 8 * onSite) +((1 - Yr1Onsite) * iFinalFTE * (21 *(int) 8.8) * offSite);

                //var Yr1Cost = (Yr1Onsite * Convert.ToDouble(iFinalFTE) * 21 * 8 * onSite * OnYr1COLA) + ((1 - Yr1Onsite) * Convert.ToDouble(iFinalFTE) * 21 * 8.8 * offSite * OffYr1COLA);
                //var Yr2Cost = (Yr2Onsite * (Convert.ToDouble( iFinalFTE) * (1 - Convert.ToDouble(Yr2PI))) * 21 * 8 * onSite * OnYr2COLA) + ((1 - Convert.ToDouble(Yr2Onsite)) * (Convert.ToDouble( iFinalFTE) * (1 - Convert.ToDouble(Yr2PI))) * 21 * 8.8 * offSite * OffYr2COLA);
                //var Yr3Cost = (Convert.ToDouble(Yr3Onsite) * (Convert.ToDouble(iFinalFTE) * (1 - Convert.ToDouble(Yr2PI)) * (1 - Convert.ToDouble(Yr3PI))) * 21 * 8 * onSite * OnYr3COLA) + ((1 - Convert.ToDouble(Yr2Onsite)) * (Convert.ToDouble(iFinalFTE * (1 - Yr2PI) * (1 - Yr3PI)) * 21 * 8.8 * offSite * OffYr3COLA));


                //    var    Yr1Cost = (Yr1Onsite * Convert.ToDecimal(iFinalFTE) * 21 * 8 * onSite * (decimal)OnYr1COLA) + ((1 - Yr1Onsite) * (iFinalFTE) * 21 * (decimal)8.8 * offSite * (decimal)OffYr1COLA);
                //  var   Yr2Cost = (Yr2Onsite * ((iFinalFTE) * (1 - (Yr2PI))) * 21 * 8 * onSite * (decimal)OnYr2COLA) + ((1 - (Yr2Onsite)) * ((iFinalFTE) * (1 - (Yr2PI))) * 21 * (decimal)8.8 * offSite * (decimal)OffYr2COLA);
                //   var x = (Convert.ToDouble(Yr3Onsite) * (Convert.ToDouble(iFinalFTE) * (1 - Convert.ToDouble(Yr2PI)) * (1 - Convert.ToDouble(Yr3PI))) * 21 * 8 * onSite * OnYr3COLA) + ((1 - Convert.ToDouble(Yr2Onsite)) * (Convert.ToDouble(iFinalFTE * (1 - Yr2PI) * (1 - Yr3PI)) * 21 * 8.8 * offSite * OffYr3COLA));
                var Yr1Cost = (Yr1Onsite * iFinalFTE * 20 * 8 * Convert.ToDecimal(onSite) * (decimal)OnYr1COLA) + ((1 - Yr1Onsite) * iFinalFTE * 20 * (decimal)8.8 * Convert.ToDecimal(offSite) * (decimal)OffYr1COLA);
                var Yr2Cost = (Yr2Onsite * (iFinalFTE * (1 - Yr2PI)) * 20 * 8 * Convert.ToDecimal(onSite) * (decimal)OnYr2COLA) + ((1 - Yr2Onsite) * (iFinalFTE * (1 - Yr2PI)) * 20 * (decimal)8.8 * Convert.ToDecimal(offSite) * (decimal)OffYr2COLA);
                var x = (Yr3Onsite * (iFinalFTE * (1 - Yr2PI) * (1 - Yr3PI)) * 20 * 8 * Convert.ToDecimal(onSite) * (decimal)OnYr3COLA) + ((1 - Yr2Onsite) * (iFinalFTE * (1 - Yr2PI) * (1 - Yr3PI)) * 20 * (decimal)8.8 * Convert.ToDecimal(offSite) * (decimal)OffYr3COLA);
                var year4 = (Yr4Onsite * (iFinalFTE * (1 - Yr2PI) * (1 - Yr3PI) * (1 - Yr4PI)) * 20 * 8 * Convert.ToDecimal(onSite) * (decimal)OnYr3COLA) + ((1 - Yr2Onsite) * (iFinalFTE * (1 - Yr2PI) * (1 - Yr3PI) * (1 - Yr4PI)) * 20 * (decimal)8.8 * Convert.ToDecimal(offSite) * (decimal)OffYr4COLA);
                var Year5 = (Yr5Onsite * (iFinalFTE * (1 - Yr2PI) * (1 - Yr3PI) * (1 - Yr4PI) * (1 - Yr5PI)) * 20 * 8 * Convert.ToDecimal(onSite) * (decimal)OnYr3COLA) + ((1 - Yr2Onsite) * (iFinalFTE * (1 - Yr2PI) * (1 - Yr3PI) * (1 - Yr4PI) * (1 - Yr5PI)) * 20 * (decimal)8.8 * Convert.ToDecimal(offSite) * (decimal)OffYr5COLA);

                var Yr3Cost = (decimal)(x);

                //added infrastructure 

                Yr1Cost = Math.Round((decimal)Yr1Cost,2)+ (Infrastructure*12);
                Yr2Cost = Math.Round((decimal)Yr2Cost,2)+(Infrastructure * 12);
                Yr3Cost = Math.Round((decimal)Yr3Cost,2) + (Infrastructure * 12);
                year4= Math.Round((decimal)year4, 2) + (Infrastructure * 12);
                Year5= Math.Round((decimal)Year5, 2) + (Infrastructure * 12);
                Yr4Cost.Add(Yr1Cost);
                Yr4Cost.Add(Yr2Cost);
                Yr4Cost.Add(Yr3Cost);
                Yr4Cost.Add(year4);
                Yr4Cost.Add(Year5);
                result.Add(Yr4Cost);

            }
            //  return new List<decimal?>() { Yr1Cost, Yr2Cost, Yr3Cost };
           
            return result;
        }

        public bool login(UserInput userInput)
        {
            var count=_dbContext.TblUsers.Where(x=>x.UserName== userInput.UserName && x.Password== userInput.Password).Count();
            if (count > 0)

            return true;
            else 
                
            return false;
        }

        public List<TblDisclaimer> GetDisclamair()
        {
            var disclaimer = _dbContext.TblDisclaimers.ToList();

            return disclaimer;
        }
    }
}
