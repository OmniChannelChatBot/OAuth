using System;

namespace OAuth.Core.Exceptions
{
    [Serializable]
    public class UnsupportedMediaTypeException :
        Exception
    {
        public UnsupportedMediaTypeException(string message)
            : base(message)
        {
        }

        public UnsupportedMediaTypeException(string message, object apiProblemDetails)
            : base(message) =>
            Data.Add(nameof(apiProblemDetails), apiProblemDetails);
    }
}
