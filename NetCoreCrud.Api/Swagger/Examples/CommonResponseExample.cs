using NetCoreCrud.Api.Models;
using Swashbuckle.AspNetCore.Examples;

namespace NetCoreCrud.Api.Swagger.Examples
{
    public class CommonResponseExample : IExamplesProvider
    {
        public object GetExamples()
        {
            return new CommonResponse(0, "string");
        }
    }
}