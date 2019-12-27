using System;

namespace OAuth.Core.Exceptions
{
    [Serializable]
    public class PayloadTooLargeException :
        Exception
    {
        public PayloadTooLargeException(string message)
            : base(message)
        {
        }

        public PayloadTooLargeException(string message, object apiProblemDetails)
            : base(message) =>
            Data.Add(nameof(apiProblemDetails), apiProblemDetails);
    }
}
