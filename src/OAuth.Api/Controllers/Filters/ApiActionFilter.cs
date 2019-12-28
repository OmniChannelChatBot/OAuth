using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using OAuth.Api.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OAuth.Api.Controllers.Filters
{
    internal class ApiActionFilter : IAsyncActionFilter
    {
        private readonly IValidator _validator;
        public ApiActionFilter(IValidator validator) =>
            _validator = validator;

        public async Task OnActionExecutionAsync(ActionExecutingContext actionExecutingContext, ActionExecutionDelegate next)
        {
            if (await ValidateModelStateAsync(actionExecutingContext))
            {
                await next();
            }
        }

        private async Task<bool> ValidateModelStateAsync(ActionExecutingContext actionExecutingContext)
        {
            var validationResult = await _validator.ValidateAsync(actionExecutingContext.ActionArguments.First().Value);
            if (!validationResult.IsValid)
            {
                actionExecutingContext.HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                ValidateApiProblemDetails(actionExecutingContext, validationResult.Errors);

                return validationResult.IsValid;
            }

            return validationResult.IsValid;
        }

        private void ValidateApiProblemDetails(ActionExecutingContext actionExecutingContext, IList<ValidationFailure> errors)
        {
            var apiProblemDetails = new ApiProblemDetails(actionExecutingContext.HttpContext, errors);
            var objectResult = new ObjectResult(apiProblemDetails)
            {
                StatusCode = actionExecutingContext.HttpContext.Response.StatusCode
            };
            actionExecutingContext.Result = objectResult;
        }
    }
}
