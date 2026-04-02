using System.Text.Json;

namespace YDeveloper.Helpers
{
    public static class JsonHelper
    {
        private static readonly JsonSerializerOptions DefaultOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            WriteIndented = false
        };

        public static string Serialize<T>(T obj)
        {
            return JsonSerializer.Serialize(obj, DefaultOptions);
        }

        public static T? Deserialize<T>(string json)
        {
            try
            {
                return JsonSerializer.Deserialize<T>(json, DefaultOptions);
            }
            catch
            {
                return default;
            }
        }

        public static bool TryDeserialize<T>(string json, out T? result)
        {
            try
            {
                result = JsonSerializer.Deserialize<T>(json, DefaultOptions);
                return result != null;
            }
            catch
            {
                result = default;
                return false;
            }
        }
    }
}
