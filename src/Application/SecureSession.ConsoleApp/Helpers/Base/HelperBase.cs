using SecureSession.ConsoleApp.Generics;

namespace SecureSession.ConsoleApp.Helpers.Base
{
    /// <summary>
    /// Author: Can DOĞU
    /// </summary>
    public abstract class HelperBase<T> : Singleton<T> where T : class, new()
    {
    }
}
