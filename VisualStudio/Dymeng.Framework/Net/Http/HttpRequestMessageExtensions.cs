using System;

namespace Dymeng.Framework.Net.Http
{
    public static class HttpRequestMessageExtensions
    {

        public static bool IsLocal(this System.Net.Http.HttpRequestMessage request) {
            var local = request.Properties["MS_IsLocal"] as Lazy<bool>;
            return local != null && local.Value;
        }

    }
}
