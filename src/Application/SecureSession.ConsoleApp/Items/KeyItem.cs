using SecureSession.ConsoleApp.Extensions;

namespace SecureSession.ConsoleApp.Items
{
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
