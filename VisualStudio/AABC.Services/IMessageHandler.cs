namespace AABC.Services
{
    public interface IMessageHandler
    {
        void Post(string message);
        void PostLine(string message);
    }
}
