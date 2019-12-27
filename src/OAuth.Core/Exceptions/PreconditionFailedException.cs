using System;

namespace OAuth.Core.Exceptions
{
    [Serializable]
    public class PreconditionFailedException :
        Exception
    {
        public PreconditionFailedException(string message)
            : base(message)
        {
        }

        public PreconditionFailedException(string message, object apiProblemDetails)
            : base(message) =>
            Data.Add(nameof(apiProblemDetails), apiProblemDetails);
    }
}
