using NetCoreCrud.Api.Models;
using Swashbuckle.AspNetCore.Examples;

namespace NetCoreCrud.Api.Swagger.Examples
{
    public class InternalServerErrorExample : IExamplesProvider
    {
        public object GetExamples()
        {
            return new InternalServerError();
        }
    }
}