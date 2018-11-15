using Newtonsoft.Json.Converters;

namespace AABC.DomainServices.Utils
{
    public class OnlyDateConverter : IsoDateTimeConverter
    {
        public OnlyDateConverter()
        {
            DateTimeFormat = "yyyy-MM-dd";
        }
    }
}
