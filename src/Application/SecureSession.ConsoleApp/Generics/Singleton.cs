namespace SecureSession.ConsoleApp.Generics
{
    /// <summary>
    /// Author: Can DOĞU
    /// </summary>
    public class Singleton<T> where T : class, new()
    {
        private static T? instance;
        private static readonly object lockObject = new();

        public static T GetInstance
        {
            get
            {
                if (instance == null)
                {
                    lock (lockObject)
                    {
                        instance ??= Activator.CreateInstance<T>();
                    }
                }

                return instance;
            }
        }
    }
}
