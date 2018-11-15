namespace AABC.Web.App.Shared
{
    public class DropDownSource
    {
        public string Name { get; set; }
        public int ID { get; set; }

        public DropDownSource(string name, int id)
        {
            Name = name;
            ID = id;
        }

    }
}