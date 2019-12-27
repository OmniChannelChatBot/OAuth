using System;

namespace OAuth.Core.Exceptions
{
    [Serializable]
    public class ApiException : Exception
    {
        public ApiException(string message, object apiProblemDetails)
            : base(message) =>
            Data.Add(nameof(apiProblemDetails), apiProblemDetails);
    }
}
