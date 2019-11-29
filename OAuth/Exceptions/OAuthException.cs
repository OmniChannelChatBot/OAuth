using System;
using System.Globalization;

namespace OAuth.Exceptions
{
    // Custom exception class for throwing application specific exceptions (e.g. for validation) 
    // that can be caught and handled within the application
    public class OAuthException : Exception
    {
        public OAuthException()
            : base()
        {
        }

        public OAuthException(string message)
            : base(message)
        {
        }

        public OAuthException(string message, params object[] args)
            : base(String.Format(CultureInfo.CurrentCulture, message, args))
        {
        }
    }
}
