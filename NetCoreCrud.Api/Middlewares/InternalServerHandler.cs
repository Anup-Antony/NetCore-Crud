using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using NetCoreCrud.Api.Extensions;
using NetCoreCrud.Api.Models;
using Newtonsoft.Json;
using Serilog;

namespace NetCoreCrud.Api.Middlewares
{
    public class InternalServerHandler
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;

        public InternalServerHandler(RequestDelegate next, ILogger logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next.Invoke(context);
            }
            catch (Exception exception)
            {
                _logger.Error(exception, "Error occurred on Customers Api");
                using (var writer = new StreamWriter(context.Response.Body))
                {
                    context.Response.StatusCode = 500;
                    context.Response.ContentType = "application/json";
                    await writer.WriteAsync(
                        JsonConvert.SerializeObject(new InternalServerError
                        {
                            CorrelationId = GetOrGenerateCorrelationId(context),
                            Description = "Error occurred while fetching customer data."
                        })
                    );
                }
            }
        }

        private string GetOrGenerateCorrelationId(HttpContext context) => context?.Request?.GetRequestHeaderOrDefault(Constants.CorrelationIdKey, $"GEN-{Guid.NewGuid().ToString()}");
    }
}
