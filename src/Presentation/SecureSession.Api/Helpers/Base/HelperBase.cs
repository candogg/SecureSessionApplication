using SecureSession.Api.Generics;

namespace SecureSession.Api.Helpers.Base
{
    /// <summary>
    /// Author: Can DOĞU
    /// </summary>
    public abstract class HelperBase<T> : Singleton<T> where T : class, new()
    {
    }
}
