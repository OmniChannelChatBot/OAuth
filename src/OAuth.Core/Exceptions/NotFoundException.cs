using System;

namespace OAuth.Core.Exceptions
{
    [Serializable]
    public class NotFoundException : Exception
    {
        public NotFoundException(string message)
            : base(message)
        {
        }

        public NotFoundException(string message, object apiProblemDetails)
            : base(message) =>
            Data.Add(nameof(apiProblemDetails), apiProblemDetails);
    }
}
