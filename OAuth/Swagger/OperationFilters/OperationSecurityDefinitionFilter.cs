using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;
using System.Linq;

namespace OAuth.Swagger.OperationFilters
{
    internal class OperationSecurityDefinitionFilter : IOperationFilter
    {
        private readonly string _openApiReferenceId;

        public OperationSecurityDefinitionFilter(string openApiReferenceId) =>
            _openApiReferenceId = openApiReferenceId;

        /// <inheritdoc/>
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var isAuthorize = context.ApiDescription.ActionDescriptor.EndpointMetadata
                .Any(s => s is AuthorizeAttribute);

            var isAllowAnonymous = context.ApiDescription.ActionDescriptor.EndpointMetadata
               .Any(s => s is AllowAnonymousAttribute);

            if (isAuthorize && !isAllowAnonymous)
            {
                var schema = new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = _openApiReferenceId
                    }
                };

                operation.Security = new List<OpenApiSecurityRequirement>
                {
                    new OpenApiSecurityRequirement { [schema] = new string[]{ _openApiReferenceId } }
                };

                operation.Responses.Add(
                    StatusCodes.Status401Unauthorized.ToString(),
                    new OpenApiResponse { Description = "Unauthorized" });
            }
        }
    }
}
