namespace AABC.Domain2.Hours
{
    public class SessionReport
    {
        public int ID { get; set; }
        public string Report { get; set; }
        public virtual Hours Session { get; set; }
    }
}
