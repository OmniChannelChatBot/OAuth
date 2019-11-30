using System;
using System.Collections.Generic;

namespace OAuth
{
    public static class Const
    {
        public static readonly IDictionary<int, ValueTuple<string, string>> ProblemTypes = new Dictionary<int, ValueTuple<string, string>>
            {
                { 400, ("Bad Request", "https://tools.ietf.org/html/rfc7231#section-6.5.1") },
                { 401, ("Unauthorized", "https://tools.ietf.org/html/rfc7235#section-3.1") },
                { 403, ("Forbidden", "https://tools.ietf.org/html/rfc7231#section-6.5.3") },
                { 404, ("Not Found", "https://tools.ietf.org/html/rfc7231#section-6.5.4") },
                { 405, ("Method Not Allowed", "https://tools.ietf.org/html/rfc7231#section-6.5.5") },
                { 412, ("Precondition Failed", "https://tools.ietf.org/html/rfc7232#section-4.2") },
                { 413, ("Payload Too Large", "https://tools.ietf.org/html/rfc7231#section-6.5.11") },
                { 414, ("URI Too Long", "https://tools.ietf.org/html/rfc7231#section-6.5.12") },
                { 415, ("Unsupported Media Type", "https://tools.ietf.org/html/rfc7231#section-6.5.13") },
                { 500, ("Internal Server Error", "https://tools.ietf.org/html/rfc7231#section-6.6.1") },
                { 501, ("Not Implemented", "https://tools.ietf.org/html/rfc7231#section-6.6.2") },
                { 502, ("Bad Gateway", "https://tools.ietf.org/html/rfc7231#section-6.6.3") },
                { 503, ("Service Unavailable", "https://tools.ietf.org/html/rfc7231#section-6.6.4") },
                { 504, ("Gateway Timeout", "https://tools.ietf.org/html/rfc7231#section-6.6.5") },
            };
    }
}
