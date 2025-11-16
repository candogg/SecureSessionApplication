using System.Text;

namespace SecureSession.ConsoleApp.Extensions
{
    /// <summary>
    /// Author: Can DOĞU
    /// </summary>
    public static class StringExtensions
    {
        public static bool IsNullOrEmpty(this string? str)
        {
            return str == null || string.IsNullOrWhiteSpace(str);
        }

        public static bool IsNotNullOrEmpty(this string? str)
        {
            return str != null && !string.IsNullOrWhiteSpace(str);
        }

        public static string FromBase64(this string? str)
        {
            if (str.IsNullOrEmpty())
                return string.Empty;

            return Encoding.UTF8.GetString(Convert.FromBase64String(str!));
        }

        public static string ToBase64(this string? str)
        {
            if (str.IsNullOrEmpty())
                return string.Empty;

            return Convert.ToBase64String(Encoding.UTF8.GetBytes(str!));
        }
    }
}
