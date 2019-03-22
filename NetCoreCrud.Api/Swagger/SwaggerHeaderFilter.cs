using System.Collections.Generic;
using System.Linq;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace NetCoreCrud.Api.Swagger
{
    public class SwaggerHeaderFilter : IOperationFilter
    {
        public void Apply(Operation operation, OperationFilterContext context)
        {
            var headers = context.ControllerActionDescriptor.GetControllerAndActionAttributes(true)
                .OfType<SwaggerHeaderAttribute>().ToList();

            if (operation.Parameters == null)
            {
                operation.Parameters = new List<IParameter>();
            }

            foreach (var header in headers)
            {
                operation.Parameters.Add(new NonBodyParameter
                {
                    Name = header.Name,
                    In = "header",
                    Type = header.Type,
                    Description = header.Description,
                    Required = header.Required
                });
            }
        }
    }
}