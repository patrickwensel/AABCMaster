using System;

namespace AABC.DomainServices
{
    public static class ContextProvider
    {

        static Func<Data.V2.CoreContext> _contextProviderFunc;

        /// <summary>
        /// Sets the action that will retrieve the current context for us
        /// </summary>
        /// <param name="action"></param>
        public static void SetContextProviderFunction(Func<Data.V2.CoreContext> contextFunc)
        {
            _contextProviderFunc = contextFunc;
        }

        /// <summary>
        /// Retrieves the context as per the supplied function
        /// (typically used to retrieve the per-request context from the MVC layer
        /// </summary>
        public static Data.V2.CoreContext Context
        {
            get
            {
                if (_contextProviderFunc == null)
                {
                    throw new InvalidOperationException("Context Provider Function has not been configured");
                }
                return _contextProviderFunc.Invoke();
            }
        }


    }
}
