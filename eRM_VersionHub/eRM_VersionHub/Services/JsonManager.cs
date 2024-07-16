using System.Text.Json;

namespace eRM_VersionHub.Services
{
    public static class JsonManager
    {
        public static JsonSerializerOptions options = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true
        };
        public static T? Deserialize<T>(this string json)
        {
            try
            {
                return JsonSerializer.Deserialize<T>(json, options);

            }
            catch (Exception ex)
            {
                return default;
            }
        }

        public static string Serialize<T>(this T json)
        {
            return JsonSerializer.Serialize(json);
        }
    }
}
