using System;

namespace OAuth.Core.Exceptions
{
    [Serializable]
    public class ServiceUnavailableException :
        Exception
    {
        public ServiceUnavailableException(string message)
            : base(message)
        {
        }

        public ServiceUnavailableException(string message, object apiProblemDetails)
            : base(message) =>
            Data.Add(nameof(apiProblemDetails), apiProblemDetails);
    }
}
