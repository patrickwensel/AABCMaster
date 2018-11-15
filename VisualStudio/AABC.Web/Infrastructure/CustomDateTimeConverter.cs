using Newtonsoft.Json.Converters;

namespace AABC.Web.Infrastructure
{
    public class CustomDateTimeConverter : IsoDateTimeConverter
    {
        public CustomDateTimeConverter()
        {
            DateTimeFormat = "yyyy-MM-dd";
        }
    }
}