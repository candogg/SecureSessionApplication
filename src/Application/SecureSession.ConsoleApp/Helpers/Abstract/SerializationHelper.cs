using SecureSession.ConsoleApp.Helpers.Base;
using System.Text.Json;

namespace SecureSession.ConsoleApp.Helpers.Abstract
{
    /// <summary>
    /// Author: Can DOĞU
    /// </summary>
    public sealed class SerializationHelper : HelperBase<SerializationHelper>
    {
        private readonly JsonSerializerOptions options;

        public SerializationHelper()
        {
            options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                WriteIndented = true,
                ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles
            };
        }

        public T? DeserializeObject<T>(object? obj)
        {
            if (obj == null) return default;

            var objStr = Convert.ToString(obj);

            if (string.IsNullOrWhiteSpace(objStr))
            {
                return default;
            }

            return JsonSerializer.Deserialize<T>(objStr, options);
        }

        public string SerializeObject(object? obj)
        {
            if (obj == null) return string.Empty;

            return JsonSerializer.Serialize(obj, options);
        }
    }
}
