using System.Linq;
using Microsoft.AspNetCore.Http;

namespace NetCoreCrud.Api.Extensions
{
    public static class HttpRequestExtensions
    {
        public static string GetRequestHeaderOrDefault(this HttpRequest request, string key, string defaultValue = null)
        {
            var a = request?.Headers?.FirstOrDefault(_ => _.Key.Equals(key)).Value.FirstOrDefault();

            return a ?? defaultValue;
        }
    }
}
