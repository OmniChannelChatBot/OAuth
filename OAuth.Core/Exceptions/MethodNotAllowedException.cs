using System;

namespace OAuth.Core.Exceptions
{
    [Serializable]
    public class MethodNotAllowedException :
       Exception
    {
        public MethodNotAllowedException(string message)
            : base(message)
        {
        }

        public MethodNotAllowedException(string message, object apiProblemDetails)
            : base(message) =>
            Data.Add(nameof(apiProblemDetails), apiProblemDetails);
    }
}
