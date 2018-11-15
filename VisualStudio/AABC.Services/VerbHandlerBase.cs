using System.Collections.Generic;

namespace AABC.Services
{
    /// <summary>
    /// Base processing for verb handlers.  The Run() method must be overridden and return an int
    /// to use as an exit code.  Providers common message posting.  Each command handler should
    /// inherit from this class.
    /// </summary>
    public abstract class VerbHandlerBase
    {

        private List<IMessageHandler> messageHandlers = new List<IMessageHandler>();

        public abstract int Run();

        public virtual void AddMessageHandler(IMessageHandler handler) {
            messageHandlers.Add(handler);
        }

        public virtual void PostMessage(string message) {
            foreach (var h in messageHandlers) {
                h.Post(message);
            }
        }

        public virtual void PostMessageLine(string message) {
            foreach (var h in messageHandlers) {
                h.PostLine(message);
            }
        }


    }
}
