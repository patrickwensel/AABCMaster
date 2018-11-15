using AABC.DomainServices.HoursResolution.Repositories;
using System;
using System.Linq;

namespace AABC.DomainServices.HoursResolution
{
    public class SSGResolution
    {

        private IResolutionService _resolutionService;
        private IResolutionServiceRepository _repository;

        public SSGResolution(IResolutionService resolutionService, IResolutionServiceRepository repository) {
            _resolutionService = resolutionService;
            _repository = repository;
        }

        public bool Resolve() {

            var originallyProposedEntries = _resolutionService.ProposedEntries.ToList();

            foreach (var entry in originallyProposedEntries) {

                _repository.RemoveOrphanedSSGRecords(entry.ID);
                
                if (!isSSG(entry)) {
                    continue;
                }
                
                // add new records for SSG
                foreach (int caseID in entry.SSGCaseIDs) {

                    if (entry.CaseID == caseID) {
                        entry.SSGParentID = entry.ID;
                    } else { 

                        var newEntry = new Domain2.Hours.Hours();

                        newEntry.BillableHours = entry.BillableHours;
                        newEntry.Case = _repository.GetCase(caseID);
                        newEntry.CaseID = caseID;
                        newEntry.Date = entry.Date;
                        newEntry.DateCreated = DateTime.Now;
                        newEntry.EndTime = entry.EndTime;
                        newEntry.ExtendedNotes = entry.ExtendedNotes;
                        newEntry.InternalMemo = entry.InternalMemo;
                        newEntry.IsAdjustment = entry.IsAdjustment;
                        newEntry.Memo = entry.Memo;
                        newEntry.PayableHours = 0;
                        newEntry.Provider = entry.Provider;
                        newEntry.ProviderID = entry.ProviderID;
                        newEntry.Service = entry.Service;
                        newEntry.ServiceID = entry.ServiceID;
                        newEntry.ServiceLocationID = entry.ServiceLocationID;
                        //newEntry.SSGParent = entry;
                        newEntry.SSGParentID = entry.ID;

                        newEntry.StartTime = entry.StartTime;
                        newEntry.Status = entry.Status;
                        newEntry.TotalHours = entry.TotalHours;
                        newEntry.WatchEnabled = entry.WatchEnabled;
                        newEntry.WatchNote = entry.WatchNote;

                        _resolutionService.ProposedEntries.Add(newEntry);   // for later-cross-validation processing
                        _repository.AddSSGEntry(newEntry);                  // for repository level tracking
                    }
                }
                
            }

            return true;
        }




        private bool isSSG(Domain2.Hours.Hours entry) {
            if (entry.ServiceID == (int)Domain2.Services.ServiceIDs.SocialSkillsGroup) {
                return true;
            } else {
                return false;
            }
        }
        
    }
}
