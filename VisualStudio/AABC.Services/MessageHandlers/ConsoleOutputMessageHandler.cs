using System;

namespace AABC.Services.MessageHandlers
{
    class ConsoleOutputMessageHandler : IMessageHandler
    {

        public void Post(string message) {
            Console.Write(message);
        }

        public void PostLine(string message) {
            Console.WriteLine(message);
        }

    }
}
