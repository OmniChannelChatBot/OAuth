using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using OAuth.Api.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace OAuth.Api.Controllers.Filters
{
    internal class ApiActionFilter : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext actionExecutingContext, ActionExecutionDelegate next)
        {
            if (CheckModelState(actionExecutingContext))
            {
                await next();
            }
        }

        private bool CheckModelState(ActionExecutingContext actionExecutingContext)
        {
            if (!actionExecutingContext.ModelState.IsValid)
            {
                actionExecutingContext.HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                ValidateApiProblemDetails(actionExecutingContext);

                return false;
            }

            return true;
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
