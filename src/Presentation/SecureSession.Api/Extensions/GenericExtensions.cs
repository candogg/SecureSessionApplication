using SecureSession.Api.Helpers.Abstract;

namespace SecureSession.Api.Extensions
{
    /// <summary>
    /// Author: Can DOĞU
    /// </summary>
    public static class GenericExtensions
    {
        public static T? Deserialize<T>(this object? obj)
        {
            return SerializationHelper.GetInstance.DeserializeObject<T>(obj);
        }

        public static string? Serialize(this object? obj)
        {
            return SerializationHelper.GetInstance.SerializeObject(obj);
        }
    }
}
