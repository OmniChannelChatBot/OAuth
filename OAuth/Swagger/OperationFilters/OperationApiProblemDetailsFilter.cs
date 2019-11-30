using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Net.Mime;

namespace OAuth.Swagger.OperationFilters
{
    public class OperationApiProblemDetailsFilter : IOperationFilter
    {
        private readonly int[] _statusCodes;
        private readonly Type _schema;
        private readonly string _mediaType;

        public OperationApiProblemDetailsFilter(int[] statusCodes, Type schema)
        {
            _statusCodes = statusCodes;
            _schema = schema;
        }

        public OperationApiProblemDetailsFilter(int statusCode, Type schema)
            : this(new int[] { statusCode }, schema)
        {
        }

        public OperationApiProblemDetailsFilter(int statusCode, Type schema, string mediaType)
            : this(new int[] { statusCode }, schema) =>
            _mediaType = mediaType;

        public void Apply(OpenApiOperation openApiOperation, OperationFilterContext operationFilterContext)
        {
            foreach (var statusCode in _statusCodes)
            {
                openApiOperation.Responses.Add(
                    statusCode.ToString(),
                    new OpenApiResponse
                    {
                        Description = Const.ProblemTypes[statusCode].Item1,
                        Content = new Dictionary<string, OpenApiMediaType>
                        {
                        {
                            _mediaType ?? MediaTypeNames.Application.Json,
                            new OpenApiMediaType
                            {
                                Schema = operationFilterContext.SchemaGenerator.GenerateSchema(_schema, operationFilterContext.SchemaRepository)
                            }
                        },
                        }
                    });
            }
        }
    }
}
