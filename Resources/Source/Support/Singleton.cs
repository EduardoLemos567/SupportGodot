namespace Support
{
    /// <summary>
    /// Implement a simple version of singleton pattern.
    /// </summary>
    /// <typeparam name="T">Expect the derivated class to be accessed by the Instance prop.</typeparam>
    public abstract class Singleton<T> where T : class, new()
    {
        private static T? _instance;
        public static T Instance
        {
            get
            {
                if (!IsInstanced) { _instance = new T(); }
                return _instance!;
            }
        }
        public static bool IsInstanced => _instance != null;
        public static void DestroyInstance() => _instance = null;
    }
}