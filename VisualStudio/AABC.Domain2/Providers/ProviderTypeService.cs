namespace AABC.Domain2.Providers
{
    public class ProviderTypeService
    {


        public int ID { get; set; }
        public int ProviderTypeID { get; set; }
        public int ServiceID { get; set; }

        public virtual ProviderType ProviderType { get; set; }
        public virtual Services.Service Service { get; set; }

    }
}
