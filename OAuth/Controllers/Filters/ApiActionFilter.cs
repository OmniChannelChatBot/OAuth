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
                await next()
                  .ContinueWith(
                      async taskNext =>
                      {
                          if (taskNext.Status != TaskStatus.Canceled)
                          {
                              await CheckResultAsync(actionExecutingContext, await taskNext);
                          }
                      });
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

        private Task CheckResultAsync(ActionExecutingContext actionExecutingContext, ActionExecutedContext actionExecutedContext)
        {
            switch (actionExecutedContext.Exception)
            {
                case TimeoutException tex:
                    actionExecutingContext.HttpContext.Response.StatusCode = StatusCodes.Status504GatewayTimeout;
                    ExceptionApiProblemDetails(actionExecutingContext, actionExecutedContext, tex);

                    return Task.FromCanceled(new CancellationToken(true));

                case Exception ex:
                    actionExecutingContext.HttpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    ExceptionApiProblemDetails(actionExecutingContext, actionExecutedContext, ex);

                    return Task.FromCanceled(new CancellationToken(true));
            }

            switch (actionExecutedContext.Result)
            {
                case NotFoundObjectResult nfor:
                    actionExecutedContext.HttpContext.Response.StatusCode = StatusCodes.Status404NotFound;
                    AfterResultApiProblemDetails(actionExecutedContext, nfor.Value.ToString());

                    return Task.FromCanceled(new CancellationToken(true));

                case BadRequestObjectResult bror:
                    actionExecutedContext.HttpContext.Response.StatusCode = StatusCodes.Status404NotFound;
                    AfterResultApiProblemDetails(actionExecutedContext, bror.Value.ToString());

                    return Task.FromCanceled(new CancellationToken(true));

                case NotFoundResult _:
                    actionExecutedContext.HttpContext.Response.StatusCode = StatusCodes.Status404NotFound;
                    AfterResultApiProblemDetails(actionExecutedContext);

                    return Task.FromCanceled(new CancellationToken(true));
            }

            return Task.CompletedTask;
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

        private void ExceptionApiProblemDetails(ActionExecutingContext actionExecutingContext, ActionExecutedContext actionExecutedContext, Exception ex)
        {
            var apiProblemDetails = new ApiProblemDetails(actionExecutingContext.HttpContext, ex);
            var objectResult = new ObjectResult(apiProblemDetails)
            {
                StatusCode = actionExecutingContext.HttpContext.Response.StatusCode
            };
            actionExecutedContext.Result = objectResult;
            actionExecutedContext.ExceptionHandled = true;
        }

        private void AfterResultApiProblemDetails(ActionExecutedContext actionExecutedContext, string title = default(string))
        {
            var apiProblemDetails = new ApiProblemDetails(actionExecutedContext.HttpContext, title);
            var objectResult = new ObjectResult(apiProblemDetails)
            {
                StatusCode = actionExecutedContext.HttpContext.Response.StatusCode
            };
            actionExecutedContext.Result = objectResult;
        }
    }
}
