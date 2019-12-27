using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Linq;

namespace OAuth.Core.Exceptions
{
    [Serializable]
    public class ValidateException : Exception
    {
        public ValidateException(string message)
            : base(message)
        {
        }

        public ValidateException(string message, object apiProblemDetails)
            : base(message) =>
            Data.Add(nameof(apiProblemDetails), apiProblemDetails);
    }
}
