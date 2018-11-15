using AABC.Data.V2;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Dynamic;
using System.Linq;
using System.Xml;

namespace AABC.DomainServices.Cases
{
    public class CaseTimelineService
    {
        private readonly CoreContext Context;

        public CaseTimelineService(CoreContext context)
        {
            Context = context;
        }

        public IEnumerable<TimelineDTO> GetTimelineEntries(int caseID, bool includeNotes)
        {
            var items = Context.Database.SqlQuery<InnerTimelineDTO>("EXEC dbo.GetTimelineEntries @caseID, @includeNotes"
                , new SqlParameter("@caseID", caseID)
                , new SqlParameter("@includeNotes", includeNotes))
                .ToList()
                .Select(m => new TimelineDTO
                {
                    Source = m.Source,
                    Id = m.Id,
                    AuthorId = m.AuthorId,
                    Author = m.Author,
                    Date = m.Date,
                    Title = m.Title,
                    Notes = m.Notes,
                    Data = ConvertData(m.Data),
                });
            return items;
        }


        private static dynamic ConvertData(string d)
        {
            var doc = new XmlDocument();
            doc.LoadXml(d);
            if (!doc.HasChildNodes)
            {
                return null;
            }
            var jsonText = JsonConvert.SerializeXmlNode(doc, Newtonsoft.Json.Formatting.None, true);
            var obj = JsonConvert.DeserializeObject<ExpandoObject>(jsonText);
            return obj;
        }


        class InnerTimelineDTO : BaseTimelineDTO
        {
            public string Data { get; set; }
        }

    }

    public class BaseTimelineDTO
    {
        public string Source { get; set; }
        public int Id { get; set; }
        public int AuthorId { get; set; }
        public string Author { get; set; }
        public DateTime Date { get; set; }
        public string Title { get; set; }
        public string Notes { get; set; }
    }

    public class TimelineDTO : BaseTimelineDTO
    {
        public dynamic Data { get; set; }
    }
}