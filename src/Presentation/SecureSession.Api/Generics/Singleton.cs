namespace SecureSession.Api.Generics
{
    /// <summary>
    /// Author: Can DOĞU
    /// </summary>
    public abstract class Singleton<T> where T : class, new()
    {
        private static T? instanceObject;
        private static readonly object lockObject = new();

        public static T GetInstance
        {
            get
            {
                if (instanceObject == null)
                {
                    lock (lockObject)
                    {
                        instanceObject ??= Activator.CreateInstance<T>();
                    }
                }

                return instanceObject;
            }
        }
    }
}
