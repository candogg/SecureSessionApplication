namespace SecureSession.ConsoleApp.Dto
{
    /// <summary>
    /// Author: Can DOĞU
    /// </summary>
    public sealed class CryptedDto
    {
        public string ClientSessionCode { get; set; } = null!;
        public string? Data { get; set; }
    }
}
