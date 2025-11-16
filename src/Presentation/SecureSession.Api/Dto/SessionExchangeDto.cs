namespace SecureSession.Api.Dto
{
    /// <summary>
    /// Author: Can DOĞU
    /// </summary>
    public class SessionExchangeDto
    {
        public string? PublicKey { get; set; }
        public string? ClientPublicKey { get; set; }
        public string? ClientSessionKey { get; set; }
        public string? ClientSessionIV { get; set; }
        public string? ClientSessionCode { get; set; }
        public DateTime KeysValidUntil { get; set; }
    }
}
