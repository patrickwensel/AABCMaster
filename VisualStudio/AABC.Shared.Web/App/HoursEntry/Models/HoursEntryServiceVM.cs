namespace AABC.Shared.Web.App.HoursEntry.Models
{
    public class HoursEntryServiceVM
    {

        public int ID { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int TypeID { get; set; }
        public string TypeName { get; set; }

        public static HoursEntryServiceVM MapFromDomain(Domain2.Services.Service service)
        {
            var x = new HoursEntryServiceVM
            {
                ID = service.ID,
                Code = service.Code,
                Name = service.Name,
                Description = service.Description,
                TypeID = (int)service.Type,
                TypeName = service.Type.ToString()
            };
            return x;
        }

    }
}