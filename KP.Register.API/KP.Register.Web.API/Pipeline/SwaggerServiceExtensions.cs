using Swashbuckle.Swagger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;


namespace KP.Register.Web.API.Pipeline
{
    public class SwaggerServiceExtensions : IOperationFilter
    {
        public void Apply(Operation operation, SchemaRegistry schemaRegistry, ApiDescription apiDescription)
        {
            if (operation.parameters != null)
            {
                operation.parameters.Add(new Parameter
                {
                    name = "Authorization",
                    @in = "header",
                    description = "access token",
                    required = true,
                    type = "string"
                });
                operation.parameters.Add(new Parameter
                {
                    name = "CallerID",
                    @in = "header",
                    description = "caller id",
                    required = true,
                    type = "string"
                });
            }
        }
    }
}