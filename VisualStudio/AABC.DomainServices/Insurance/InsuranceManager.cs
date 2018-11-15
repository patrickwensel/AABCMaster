using AABC.Domain2.Insurances;
using System;
using System.Collections.Generic;
using System.Linq;


namespace AABC.DomainServices.Insurance
{
    public class InsuranceManager
    {

        Data.V2.CoreContext _context;


        public InsuranceManager(Data.V2.CoreContext context) {
            _context = context;
        }
        public InsuranceManager() {
            _context = ContextProvider.Context;
        }



        public bool IsCarrierDeleteable(int carrierID) {
            
            var usageCount = _context.CaseInsurances.Where(x => x.CarrierID == carrierID).Count();

            return usageCount == 0 ? true : false;
        }


        public Domain2.Insurances.Insurance Create(string insuranceName) {

            var existingInsuranceByName = _context.Insurances.Where(x => x.Name == insuranceName).FirstOrDefault();

            if (existingInsuranceByName != null) {
                throw new InvalidOperationException("Insurance name already exists");
            }

            var ins = new Domain2.Insurances.Insurance();

            ins.Name = insuranceName;
            ins.Active = true;
            ins.Services = getDefaultServices();

            _context.Insurances.Add(ins);
            _context.SaveChanges();

            return ins;
        }

        public Domain2.Insurances.Insurance Copy(Domain2.Insurances.Insurance source, string newInsuranceName) {

            var existingInsuranceByName = _context.Insurances.Where(x => x.Name == newInsuranceName).FirstOrDefault();

            if (existingInsuranceByName != null) {
                throw new InvalidOperationException("Insurance name already exists");
            }
            
            var ins = new Domain2.Insurances.Insurance();

            ins.Name = newInsuranceName;
            ins.Active = true;
            ins.RequireCredentialsForBCBA = source.RequireCredentialsForBCBA;
            ins.AuthorizationMatchRules = new List<Domain2.Authorizations.AuthorizationMatchRule>();
            ins.Services = new List<InsuranceService>();

            var rules = source.AuthorizationMatchRules;
            var services = source.Services;

            foreach (var rule in rules) {
                ins.AuthorizationMatchRules.Add(new Domain2.Authorizations.AuthorizationMatchRule()
                {
                    AllowOverlapping = rule.AllowOverlapping,
                    BillingMethod = rule.BillingMethod,
                    FinalAuthorizationID = rule.FinalAuthorizationID,
                    FinalMinimumMinutes = rule.FinalMinimumMinutes,
                    FinalUnitSize = rule.FinalUnitSize,
                    InitialAuthorizationID = rule.InitialAuthorizationID,
                    InitialMinimumMinutes = rule.InitialMinimumMinutes,
                    InitialUnitSize = rule.InitialUnitSize,
                    ProviderTypeID = rule.ProviderTypeID,
                    RequiresAuthorizedBCBA = rule.RequiresAuthorizedBCBA,
                    RequiresPreAuthorization = rule.RequiresPreAuthorization,
                    ServiceID = rule.ServiceID
                });
            }

            foreach (var svc in services) {
                ins.Services.Add(new InsuranceService()
                {
                    DefectiveDate = svc.DefectiveDate,
                    EffectiveDate = svc.EffectiveDate,
                    ProviderTypeID = svc.ProviderTypeID,
                    ServiceID = svc.ServiceID
                });
            }

            _context.Insurances.Add(ins);
            _context.SaveChanges();

            return ins;
        }

        public void DeleteCarrier(int carrierID) {
            var carrier = _context.InsuranceLocalCarriers.Find(carrierID);
            _context.InsuranceLocalCarriers.Remove(carrier);
            _context.SaveChanges();
        }

        private List<InsuranceService> getDefaultServices() {

            var defaults = _context.InsuranceServiceDefaults.ToList();

            var items = new List<InsuranceService>();

            foreach (var def in defaults) {
                items.Add(new InsuranceService()
                {
                     ServiceID = def.ServiceID,
                     ProviderTypeID = def.ProviderTypeID
                });
            }

            return items;
        }


    }
}
