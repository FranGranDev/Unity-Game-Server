namespace Networking.Services
{
    public static class Logger
    {
        public static bool Enabled { get; set; } = true;

        public static void Log(string value)
        {
            if (!Enabled)
                return;

            UIDebugger.Log($"Networking: {value}");
        }
    }
}
