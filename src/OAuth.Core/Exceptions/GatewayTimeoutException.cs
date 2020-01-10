using System;

namespace OAuth.Core.Exceptions
{
    [Serializable]
    public class GatewayTimeoutException :
        Exception
    {
        public GatewayTimeoutException(string message)
            : base(message)
        {
        }

        public GatewayTimeoutException(string message, object apiProblemDetails)
            : base(message) =>
            Data.Add(nameof(apiProblemDetails), apiProblemDetails);
    }
}
