namespace SecureSession.Api.Items
{
    /// <summary>
    /// Author: Can DOĞU
    /// </summary>
    public sealed class ResponseItem<T> where T : class, new()
    {
        public bool IsSuccess { get; set; }
        public T? Data { get; set; }
        public string? Message { get; set; }

        public static ResponseItem<T> GenerateSuccess(T? data = default, string? message = null)
        {
            return new ResponseItem<T> { IsSuccess = true, Data = data, Message = message };
        }

        public static ResponseItem<T> GenerateError(string? message = null)
        {
            return new ResponseItem<T> { IsSuccess = false, Message = message };
        }
    }
}
