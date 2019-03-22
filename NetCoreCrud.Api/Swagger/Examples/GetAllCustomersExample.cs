using System.Collections.Generic;
using NetCoreCrud.Api.Models;
using Swashbuckle.AspNetCore.Examples;

namespace NetCoreCrud.Api.Swagger.Examples
{
    public class GetAllCustomersExample : IExamplesProvider
    {
        public object GetExamples()
        {
            return new List<Customer>();
        }
    }
}
