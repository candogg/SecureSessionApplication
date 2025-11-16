using SecureSession.Api.Extensions;

namespace SecureSession.Api.Items
{
    /// <summary>
    /// Author: Can DOĞU
    /// </summary>
    public sealed class SymmetricKeyItem
    {
        public string AesKey { get; set; } = null!;
        public string AesIv { get; set; } = null!;

        public bool IsValid
        {
            get
            {
                return AesKey.IsNotNullOrEmpty() && AesIv.IsNotNullOrEmpty();
            }
        }
    }
}
