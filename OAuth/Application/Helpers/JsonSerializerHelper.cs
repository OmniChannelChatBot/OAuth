using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace OAuth.Application.Helpers
{
    public static class JsonSerializerHelper
    {
        public static StringContent Serialize<TValue>(TValue value)
        {
            // public static string Serialize<TValue>(TValue value, JsonSerializerOptions options = null);
            return new StringContent(JsonSerializer.Serialize(value), Encoding.UTF8, "application/json");
        }

        public static TValue Deserialize<TValue>(HttpResponseMessage response)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            };

            // public static TValue Deserialize<TValue>(string json, JsonSerializerOptions options = null);
            return JsonSerializer.Deserialize<TValue>
                    (response.Content.ReadAsStringAsync().Result, options);
        }
    }
}
