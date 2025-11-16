using SecureSession.ConsoleApp.Extensions;
using SecureSession.ConsoleApp.Helpers.Abstract;

namespace SecureSession.ConsoleApp.Dto
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

        public bool IsValid
        {
            get
            {
                return ClientSessionCode.IsNotNullOrEmpty()
                    && ClientSessionKey.IsNotNullOrEmpty()
                    && ClientSessionIV.IsNotNullOrEmpty();
            }
        }

        public void DecryptAll(string privatePem)
        {
            ClientSessionCode = CryptoHelper.GetInstance.DecryptDataWithPrivate(ClientSessionCode, privatePem);

            ClientSessionKey = CryptoHelper.GetInstance.DecryptDataWithPrivate(ClientSessionKey, privatePem);

            ClientSessionIV = CryptoHelper.GetInstance.DecryptDataWithPrivate(ClientSessionIV, privatePem);
        }

        public override string? ToString()
        {
            return this.Serialize();
        }
    }
}
