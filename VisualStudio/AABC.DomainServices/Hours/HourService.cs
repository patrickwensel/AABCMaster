using AABC.Domain2.Hours;
using System;
using System.Collections.Generic;
using System.Linq;
namespace AABC.DomainServices.Hours
{
    public class HourService
    {
        public HourEx Get(int Id)
        {
            var hour = _context.Hours.Where(l => l.ID == Id).FirstOrDefault();
            return Transform(hour);
        }
        public List<HourEx> GetPatientExcessDays(int PatientId, DateTime start, DateTime end, int Limit = 4)
        {
            var hours = _context.Database.SqlQuery<HourEx>("GetPatientExcessDays {0}, {1}, {2}, {3}", PatientId, start, end, Limit);
            return hours.ToList();
        }
        public List<HourEx> GetPatientHours(int PatientId, DateTime start, DateTime end)
        {
            var hours = _context.Hours.Where(h => h.Case.PatientID == PatientId && h.Provider.ProviderTypeID == 17 && h.Date >= start && h.Date < end).OrderBy(h => h.Date);
            return Transform(hours.ToList());
        }
        public void Save(HourEx hour)
        {
            AABC.Domain2.Hours.Hours m = Transform(hour);
            if (m.ID == 0)
            {
                _context.Hours.Add(m);
            }
            _context.SaveChanges();
            hour.Id = m.ID;
        }

        private List<HourEx> Transform(List<AABC.Domain2.Hours.Hours> n)
        {
            List<HourEx> r = new List<HourEx>();
            foreach (var a in n)
            {
                r.Add(Transform(a));
            }
            return r;
        }
        private HourEx Transform(AABC.Domain2.Hours.Hours n)
        {
            HourEx r = new HourEx();
            r.Id = n.ID;
            r.AuthorizationID = n.AuthorizationID;
            r.BillableHours = n.BillableHours;
            r.BillingRef = n.BillingRef;
            r.CaseID = n.CaseID;
            r.PatientName = n.Case.Patient.CommonName;
            r.CorrelationID = n.CorrelationID;
            r.Date = n.Date;
            r.DateCreated = n.DateCreated;
            r.EndTime = n.EndTime;
            r.HasCatalystData = n.HasCatalystData;
            r.InternalMemo = n.InternalMemo;
            r.IsAdjustment = n.IsAdjustment;
            r.Memo = n.Memo;
            r.ParentApprovalID = n.ParentApprovalID;
            r.ParentReported = n.ParentReported;
            r.PayableHours = n.PayableHours;
            r.PayableRef = n.PayableRef;
            r.ProviderID = n.ProviderID;
            r.ProviderName = n.Provider.FirstName + " " + n.Provider.LastName;
            r.ServiceID = n.ServiceID;
            r.ServiceLocationID = n.ServiceLocationID;
            r.SSGParentID = n.SSGParentID;
            r.StartTime = n.StartTime;
            r.Status = n.Status.ToString();
            r.TotalHours = n.TotalHours;
            r.WatchEnabled = n.WatchEnabled;
            r.WatchNote = n.WatchNote;
            return r;
        }
        private AABC.Domain2.Hours.Hours Transform(HourEx n)
        {
            AABC.Domain2.Hours.Hours r;
            if (n.Id > 0)
            {
                r = _context.Hours.Where(p => p.ID == n.Id).FirstOrDefault();
            }
            else
            {
                r = _context.Hours.Create();
            }
            r.ID = n.Id;
            r.AuthorizationID = n.AuthorizationID;
            r.BillableHours = n.BillableHours;
            r.BillingRef = n.BillingRef;
            r.CaseID = n.CaseID;
            r.CorrelationID = n.CorrelationID;
            r.Date = n.Date;
            r.DateCreated = n.DateCreated;
            r.EndTime = n.EndTime;
            r.HasCatalystData = n.HasCatalystData;
            r.InternalMemo = n.InternalMemo;
            r.IsAdjustment = n.IsAdjustment;
            r.Memo = n.Memo;
            r.ParentApprovalID = n.ParentApprovalID;
            r.ParentReported = n.ParentReported;
            r.PayableHours = n.PayableHours;
            r.PayableRef = n.PayableRef;
            r.ProviderID = n.ProviderID;
            r.ServiceID = n.ServiceID;
            r.ServiceLocationID = n.ServiceLocationID;
            r.SSGParentID = n.SSGParentID;
            r.StartTime = n.StartTime;
            r.Status = (HoursStatus) Enum.Parse(typeof(HoursStatus), n.Status, true);
            r.TotalHours = n.TotalHours;
            r.WatchEnabled = n.WatchEnabled;
            r.WatchNote = n.WatchNote;
            return r;
        }
        private Data.V2.CoreContext _context;

        public HourService(Data.V2.CoreContext context)
        {
            _context = context;
        }

    }
}
