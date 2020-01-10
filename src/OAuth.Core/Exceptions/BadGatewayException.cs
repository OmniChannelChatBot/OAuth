using System;

namespace OAuth.Core.Exceptions
{
    [Serializable]
    public class BadGatewayException :
        Exception
    {
        public BadGatewayException(string message)
            : base(message)
        {
        }

        public BadGatewayException(string message, object apiProblemDetails)
            : base(message) =>
            Data.Add(nameof(apiProblemDetails), apiProblemDetails);
    }
}
