namespace AABC.Web.App.Scheduler.Models
{
    public class ResourceDTO
    {
        public int UniqueID { get; set; }
        public int ResourceID { get; set; }
        public string ResourceName { get; set; }
        public int Color { get; set; }
        public object Image { get; set; }
        public string CustomField1 { get; set; }

        public static ResourceDTO GetDefaultResource()
        {
            return new ResourceDTO()
            {
                UniqueID = 0,
                ResourceID = 0,
                ResourceName = "Default"
            };
        }
    }
}