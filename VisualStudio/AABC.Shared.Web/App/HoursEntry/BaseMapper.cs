using AABC.Data.V2;
using AABC.Domain2.Hours;
using AABC.Domain2.Providers;
using AABC.Shared.Web.App.HoursEntry.Models.Request;
using System;

namespace AABC.Shared.Web.App.HoursEntry
{
    public abstract class BaseMapper<TEntry>
        where TEntry : BaseHoursEntryRequestVM
    {
        private static readonly string SignatureType = "image/png";
        protected CoreContext Context { get; private set; }

        protected BaseMapper(CoreContext context)
        {
            Context = context;
        }


        public Hours Map(TEntry request, Hours entry)
        {
            if (entry == null)
            {
                entry = new Hours();
            }
            var provider = Context.Providers.Find(request.ProviderID);
            var providerType = (ProviderTypeIDs)provider.ProviderTypeID;
            var patient = Context.Patients.Find(request.PatientID);
            entry.CaseID = patient.ActiveCase.ID;
            entry.ProviderID = request.ProviderID;
            entry.ServiceID = request.ServiceID;
            entry.Date = request.Date;
            entry.StartTime = request.TimeIn.TimeOfDay;
            entry.EndTime = request.TimeOut.TimeOfDay;
            entry.ServiceLocationID = request.ServiceLocationID;
            entry.Provider = provider;
            entry.Service = Context.Services.Find(request.ServiceID);
            entry.Status = (HoursStatus)request.Status;
            entry.IsTrainingEntry = request.IsTrainingEntry;
            if (request.Signatures != null && request.Signatures.Length == 2)
            {
                entry.SessionSignature = new SessionSignature
                {
                    ProviderSignature = request.Signatures[0].Base64Signature,
                    ProviderName = request.Signatures[0].Name,
                    ProviderSignatureType = SignatureType,
                    ParentSignature = request.Signatures[1].Base64Signature,
                    ParentName = request.Signatures[1].Name,
                    ParentSignatureType = SignatureType
                };
            }
            if (providerType == ProviderTypeIDs.Aide)
            {
                entry.SSGCaseIDs = request.SsgIDs;
                MapAide(request, entry);
            }
            else if (providerType == ProviderTypeIDs.BoardCertifiedBehavioralAnalyst)
            {
                MapBCBA(request, entry);
            }
            else
            {
                throw new InvalidOperationException("Provider type not registered for this action");
            }
            return entry;
        }

        protected abstract void MapAide(TEntry request, Hours entry);
        protected abstract void MapBCBA(TEntry request, Hours entry);
    }
}
