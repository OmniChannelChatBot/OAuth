using System;
using System.Runtime.Serialization;

namespace OAuth.Core.Exceptions
{
    [Serializable]
    public class UserException : Exception
    {
        public UserException(string message)
           : base(message)
        {
        }

        public UserException(string message, object apiProblemDetails)
            : base(message) =>
            Data.Add(nameof(apiProblemDetails), apiProblemDetails);
    }
}
