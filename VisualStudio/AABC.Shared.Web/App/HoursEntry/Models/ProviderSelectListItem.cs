namespace AABC.Shared.Web.App.HoursEntry.Models
{
    public class ProviderSelectListItem
    {
        public int ID { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }

        public static ProviderSelectListItem Transform(Domain2.Providers.Provider provider) {

            var item = new ProviderSelectListItem();

            item.ID = provider.ID;
            item.Type = provider.ProviderType.Code;
            item.Name = provider.FirstName + " " + provider.LastName;

            return item;
        }
    }
}