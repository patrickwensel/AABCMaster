using AABC.Web.App.Insurance.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AABC.Web.App.Insurance
{
    public class InsuranceService
    {

        public enum AuthRuleStatsType
        {
            Initial,
            Final
        }


        internal List<ServiceListItem> GetServiceListItems(int insuranceID) {

            var data = _context.InsuranceServices.AsQueryable();
            if (insuranceID > 0) {
                data = data.Where(x => x.InsuranceID == insuranceID);
            }
            var items = new List<ServiceListItem>();

            foreach (var d in data.ToList()) {

                var item = new ServiceListItem();

                var service = _context.Services.Find(d.ServiceID);
                var providerType = _context.ProviderTypes.Find(d.ProviderTypeID);

                item.ID = d.ID;
                item.ServiceID = d.ServiceID;
                item.ServiceCode = service.Code;
                item.ServiceName = service.Name;
                item.ProviderTypeID = d.ProviderTypeID;
                item.ProviderTypeCode = providerType.Code;
                item.EffectiveDate = d.EffectiveDate;
                item.DefectiveDate = d.DefectiveDate;

                items.Add(item);
            }

            return items;
        }


        internal ServiceEditVM GetServiceEditVM(int insuranceServiceID) {

            var insuranceService = _context.InsuranceServices.Find(insuranceServiceID);

            var item = new ServiceEditVM();

            item.ID = insuranceServiceID;
            item.ServiceID = insuranceService.ServiceID;
            item.InsuranceID = insuranceService.InsuranceID;
            item.ProviderTypeID = insuranceService.ProviderTypeID;
            item.EffectiveDate = insuranceService.EffectiveDate;
            item.DefectiveDate = insuranceService.DefectiveDate;

            return item;
        }

        internal void RemoveService(int insuranceServiceID) {
            var x = _context.InsuranceServices.Find(insuranceServiceID);
            _context.InsuranceServices.Remove(x);
            _context.SaveChanges();
        }

        internal void SaveService(ServiceEditVM model) {
            if (model.ID.HasValue) {
                saveServiceExisting(model);
            } else {
                saveServiceNew(model);
            }
        }

        internal List<CarrierListItem> GetCarrierItems(int insuranceID) {

            var items = new List<CarrierListItem>();
            var data = _context.InsuranceLocalCarriers.Where(x => x.InsuranceID == insuranceID).ToList();

            foreach (var d in data) {
                items.Add(new CarrierListItem()
                {
                    ID = d.ID,
                    InsuranceID = d.InsuranceID,
                    Name = d.Name
                });
            }
            return items;
        }

        internal string DeleteCarrier(int carrierID) {
            var manager = new DomainServices.Insurance.InsuranceManager(_context);
            if (!manager.IsCarrierDeleteable(carrierID)) {
                return "cant delete";
            } else {
                manager.DeleteCarrier(carrierID);
                return "ok";
            }
        }

        internal void SaveCarrier(int? id, int insuranceID, string carrierName) {

            Domain2.Insurances.LocalCarrier carrier = null;

            if (id.HasValue) {
                carrier = _context.InsuranceLocalCarriers.Find(id.Value);
            } else {
                carrier = new Domain2.Insurances.LocalCarrier();
                _context.InsuranceLocalCarriers.Add(carrier);
            }

            carrier.InsuranceID = insuranceID;
            carrier.Name = carrierName;

            _context.SaveChanges();
        }

        private void saveServiceExisting(ServiceEditVM model) {

            var inssvc = _context.InsuranceServices.Find(model.ID.Value);

            inssvc.DefectiveDate = model.DefectiveDate;
            inssvc.EffectiveDate = model.EffectiveDate;
            inssvc.InsuranceID = model.InsuranceID;
            inssvc.ProviderTypeID = model.ProviderTypeID;
            inssvc.ServiceID = model.ServiceID;
            
            _context.SaveChanges();
        }

        private void saveServiceNew(ServiceEditVM model) {

            var inssvc = new Domain2.Insurances.InsuranceService();

            inssvc.DefectiveDate = model.DefectiveDate;
            inssvc.EffectiveDate = model.EffectiveDate;
            inssvc.InsuranceID = model.InsuranceID;
            inssvc.ProviderTypeID = model.ProviderTypeID;
            inssvc.ServiceID = model.ServiceID;

            _context.InsuranceServices.Add(inssvc);
            _context.SaveChanges();
        }




        internal AuthRuleEditVM GetAuthRuleEditVM(int ruleID) {

            var rule = _context.AuthorizationMatchRules.Find(ruleID);

            var item = new AuthRuleEditVM();

            item.AllowOverlapping = rule.AllowOverlapping;
            item.FinalAuthorizationID = rule.FinalAuthorizationID;
            item.FinalMinimumMinutes = rule.FinalMinimumMinutes;
            item.FinalUnitSize = rule.FinalUnitSize;
            item.ID = rule.ID;
            item.InitialAuthorizationID = rule.InitialAuthorizationID;
            item.InitialMinimumMinutes = rule.InitialMinimumMinutes;
            item.InitialUnitSize = rule.InitialUnitSize;
            item.InsuranceID = rule.InsuranceID;
            item.IsUntimed = rule.BillingMethod == Domain2.Authorizations.BillingMethod.Service ? true : false;
            item.ProviderTypeID = rule.ProviderTypeID;
            item.RequiresAuthorizedBCBA = rule.RequiresAuthorizedBCBA;
            item.RequiresPreAuthorization = rule.RequiresPreAuthorization;
            item.ServiceID = rule.ServiceID;

            return item;

        }

        internal List<AuthRuleListItem> GetAuthRules(int insuranceID) {

            var rules = _context.AuthorizationMatchRules.AsQueryable();
            if (insuranceID > 0)
            {
                rules = rules.Where(x => x.InsuranceID == insuranceID);
            }
            var items = new List<AuthRuleListItem>();

            foreach (var rule in rules.ToList()) {

                var item = new AuthRuleListItem();

                item.AllowOverlap = rule.AllowOverlapping;
                item.FinalStats = getRuleStatusDisplay(rule, AuthRuleStatsType.Final);
                item.ID = rule.ID;
                item.InitialStats = getRuleStatusDisplay(rule, AuthRuleStatsType.Initial);
                item.ProviderType = rule.ProviderType.Code;
                item.ProviderTypeID = rule.ProviderTypeID;
                item.RequiresBCBA = rule.RequiresAuthorizedBCBA;
                item.RequiresPreAuth = rule.RequiresPreAuthorization;
                item.Service = rule.Service.Code;
                item.ServiceID = rule.ServiceID;
                item.InsuranceID = rule.InsuranceID;
                item.InsuranceName = rule.Insurance.Name;

                items.Add(item);
            }

            return items;
        }

        private string getRuleStatusDisplay(Domain2.Authorizations.AuthorizationMatchRule rule, AuthRuleStatsType type) {

            string display = "";

            if (type == AuthRuleStatsType.Initial) {

                display = rule.InitialAuthorization?.Code;
                
                if (display == null) {
                    return null;
                }

                if (rule.InitialMinimumMinutes.HasValue) {
                    display += ", " + rule.InitialMinimumMinutes.ToString();
                    if (rule.InitialUnitSize.HasValue) {
                        display += "/" + rule.InitialUnitSize.ToString();
                    }
                }
                
            }

            if (type == AuthRuleStatsType.Final) {

                display = rule.FinalAuthorization?.Code;

                if (display == null) {
                    return null;
                }

                if (rule.FinalMinimumMinutes.HasValue) {
                    display += ", " + rule.FinalMinimumMinutes.ToString();
                    if (rule.FinalUnitSize.HasValue) {
                        display += "/" + rule.FinalUnitSize.ToString();
                    }
                }

            }

            return display;
        }

        internal void RemoveAuthRule(int ruleID) {
            var rule = _context.AuthorizationMatchRules.Find(ruleID);
            _context.AuthorizationMatchRules.Remove(rule);
            _context.SaveChanges();
        }

        internal void SaveAuthRule(AuthRuleEditVM model) {
            
            if (model.ID.HasValue) {
                saveAuthRuleExisting(model);
            }  else {
                saveAuthRuleNew(model);
            }
        }

        private void saveAuthRuleNew(AuthRuleEditVM model) {

            var rule = new Domain2.Authorizations.AuthorizationMatchRule();
            
            rule.AllowOverlapping = model.AllowOverlapping;
            rule.BillingMethod = model.IsUntimed ? Domain2.Authorizations.BillingMethod.Service : Domain2.Authorizations.BillingMethod.Timed;
            rule.FinalAuthorizationID = model.FinalAuthorizationID;
            rule.FinalMinimumMinutes = model.FinalMinimumMinutes;
            rule.FinalUnitSize = model.FinalUnitSize;
            rule.InitialAuthorizationID = model.InitialAuthorizationID;
            rule.InitialMinimumMinutes = model.InitialMinimumMinutes;
            rule.InitialUnitSize = model.InitialUnitSize;
            rule.ProviderTypeID = model.ProviderTypeID;
            rule.RequiresAuthorizedBCBA = model.RequiresAuthorizedBCBA;
            rule.RequiresPreAuthorization = model.RequiresPreAuthorization;
            rule.ServiceID = model.ServiceID;
            rule.InsuranceID = model.InsuranceID;

            _context.AuthorizationMatchRules.Add(rule);
            _context.SaveChanges();
        }

        private void saveAuthRuleExisting(AuthRuleEditVM model) {

            var rule = _context.AuthorizationMatchRules.Find(model.ID.Value);

            rule.AllowOverlapping = model.AllowOverlapping;
            rule.BillingMethod = model.IsUntimed ? Domain2.Authorizations.BillingMethod.Service : Domain2.Authorizations.BillingMethod.Timed;
            rule.FinalAuthorizationID = model.FinalAuthorizationID;
            rule.FinalMinimumMinutes = model.FinalMinimumMinutes;
            rule.FinalUnitSize = model.FinalUnitSize;
            rule.InitialAuthorizationID = model.InitialAuthorizationID;
            rule.InitialMinimumMinutes = model.InitialMinimumMinutes;
            rule.InitialUnitSize = model.InitialUnitSize;
            rule.ProviderTypeID = model.ProviderTypeID;
            rule.RequiresAuthorizedBCBA = model.RequiresAuthorizedBCBA;
            rule.RequiresPreAuthorization = model.RequiresPreAuthorization;
            rule.ServiceID = model.ServiceID;

            _context.SaveChanges();
        }

        internal List<InsuranceListItem> GetInsuranceListItems() {

            var insurances = _context.Insurances.Where(x => x.Active).OrderBy(x => x.Name).ToList();

            var items = new List<InsuranceListItem>();

            foreach (var ins in insurances) {
                items.Add(new InsuranceListItem()
                {
                    ID = ins.ID,
                    Name = ins.Name
                });
            }

            return items;
        }





        private Data.V2.CoreContext _context;

        public InsuranceService() {
            _context = AppService.Current.DataContextV2;
        }

        internal InsuranceEditVM GetInsuranceEditItem(int? id) {
            
            if (!id.HasValue) {
                return new InsuranceEditVM();
            }

            var data = _context.Insurances.Find(id);
            return Transform(data);

        }

        internal void SaveInsurance(InsuranceEditVM model)
        {
            var insurance = Transform(model);
            if (model.ID == 0)
            {
                _context.Insurances.Add(insurance);
            }
            _context.SaveChanges();
            model.ID = insurance.ID;
        }

        internal void AddInsurance(string name) {
            
            if (!ValidateInsuranceAddition(name)) {
                throw new InvalidOperationException("Failed validation");
            }

            var insManager = new DomainServices.Insurance.InsuranceManager(_context);
            insManager.Create(name);
        }

        internal void CopyInsurance(string name, int copySourceId)
        {

            if (!ValidateInsuranceAddition(name))
            {
                throw new InvalidOperationException("Failed validation");
            }

            var source = _context.Insurances.Find(copySourceId);
            var insManager = new DomainServices.Insurance.InsuranceManager(_context);

            insManager.Copy(source, name);
        }

        internal bool ValidateInsuranceAddition(string name) {
            string message;
            return ValidateInsuranceAddition(name, out message);
        }

        internal bool ValidateInsuranceAddition(string name, out string message) {

            message = "";
            bool valid = true;

            var ins = _context.Insurances.Where(x => x.Name.ToUpper() == name.ToUpper()).FirstOrDefault();
            if (ins != null) {
                message = "Insurance with this name already exists.";
                valid = false;
            }

            return valid;

        }

        internal void RemoveInsurance(int id) {
            
            if (!ValidateInsuranceDeletion(id)) {
                throw new InvalidOperationException("Validation failed, unable to remove this insurance");
            }

            var insurance = _context.Insurances.Find(id);
            _context.Insurances.Remove(insurance);
            _context.SaveChanges();
        }

        internal bool ValidateInsuranceDeletion(int id) {
            string msg;
            return ValidateInsuranceDeletion(id, out msg);
        }

        internal bool ValidateInsuranceDeletion(int id, out string message) {

            message = "";
            bool valid = true;

            var ins = _context.Insurances.Find(id);

            if (ins.Cases.Count > 0) {
                message += "Insurance has one or more associated patients.  Unable to remove insurance once it has been applied to a case.";
                valid = false;
            }

            return valid;
        }

        internal InsuranceEditVM Transform(Domain2.Insurances.Insurance n)
        {
            InsuranceEditVM r = new InsuranceEditVM();
            r.ID = n.ID;
            r.Name = n.Name;
            r.RequireCredentialsForBCBA = n.RequireCredentialsForBCBA.GetValueOrDefault(false);
            return r;
        }

        internal Domain2.Insurances.Insurance Transform(InsuranceEditVM n)
        {
            Domain2.Insurances.Insurance r;
            if (n.ID > 0)
            {
                r = _context.Insurances.Find(n.ID);
            }else
            {
                r = _context.Insurances.Create();
            }
            r.ID = n.ID;
            r.Name = n.Name;
            r.RequireCredentialsForBCBA = n.RequireCredentialsForBCBA;
            return r;
        }
    }
}