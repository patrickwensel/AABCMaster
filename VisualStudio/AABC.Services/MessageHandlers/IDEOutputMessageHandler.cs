namespace AABC.Services.MessageHandlers
{
    class IDEOutputMessageHandler : IMessageHandler
    {
        public void Post(string message) {
            System.Diagnostics.Debug.Write(message);
        }

        public void PostLine(string message) {
            System.Diagnostics.Debug.WriteLine(message);
        }
    }
}
