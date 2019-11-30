using System;
using System.Globalization;

namespace OAuth.Application.Exceptions
{
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
