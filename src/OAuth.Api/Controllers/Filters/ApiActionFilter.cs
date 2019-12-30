using FluentValidation;
using FluentValidation.AspNetCore;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using OAuth.Api.Models;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OAuth.Api.Controllers.Filters
{
    internal class ApiActionFilter : IAsyncActionFilter
    {
        private readonly IValidatorFactory _validatorFactory;
        public ApiActionFilter(IValidatorFactory validatorFactory) =>
            _validatorFactory = validatorFactory;

        public async Task OnActionExecutionAsync(ActionExecutingContext actionExecutingContext, ActionExecutionDelegate next)
        {
            if (await ValidateModelStateAsync(actionExecutingContext))
            {
                await next();
            }
        }

        private async Task<bool> ValidateModelStateAsync(ActionExecutingContext actionExecutingContext)
        {
            if (!actionExecutingContext.ModelState.IsValid)
            {
                actionExecutingContext.HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                ValidateApiProblemDetails(actionExecutingContext);

                return actionExecutingContext.ModelState.IsValid;
            }

            if (actionExecutingContext.ActionArguments.Count == 0)
            {
                return true;
            }

            var errors = new List<ValidationFailure>();

            foreach (var (_, value) in actionExecutingContext.ActionArguments)
            {
                if (value == default)
                {
                    continue;
                }

                var validator = _validatorFactory.GetValidator(value.GetType());

                if (validator == default)
                {
                    continue;
                }

                var validationResult = await validator.ValidateAsync(value);

                if (validationResult.IsValid)
                {
                    continue;
                }

                errors.AddRange(validationResult.Errors);
            }

            if (errors.Any())
            {
                actionExecutingContext.HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                ValidateApiProblemDetails(actionExecutingContext, errors);

                return false;
            }

            return true;
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

        private void ValidateApiProblemDetails(ActionExecutingContext actionExecutingContext)
        {
            var apiProblemDetails = new ApiProblemDetails(actionExecutingContext.HttpContext, actionExecutingContext.ModelState);
            var objectResult = new ObjectResult(apiProblemDetails)
            {
                StatusCode = actionExecutingContext.HttpContext.Response.StatusCode
            };
            actionExecutingContext.Result = objectResult;
        }
    }
}
