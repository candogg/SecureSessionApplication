namespace SecureSession.Api.Dto
{
    /// <summary>
    /// Author: Can DOĞU
    /// </summary>
    public sealed class CryptedRequestDto
    {
        public string ClientSessionCode { get; set; } = null!;
        public string? Data { get; set; }
    }
}
