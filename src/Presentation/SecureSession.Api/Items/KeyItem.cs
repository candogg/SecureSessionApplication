using SecureSession.Api.Extensions;

namespace SecureSession.Api.Items
{
    /// <summary>
    /// Author: Can DOĞU
    /// </summary>
    public sealed class KeyItem
    {
        public string PublicKey { get; set; } = null!;
        public string PrivateKey { get; set; } = null!;

        public bool IsValid
        {
            get
            {
                return PublicKey.IsNotNullOrEmpty() && PrivateKey.IsNotNullOrEmpty();
            }
        }
    }
}
