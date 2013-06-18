using System;

namespace WinDock.Services
{
    public enum LogLevel
    {
        Debug,
        Info,
        Notify,
        Warn,
        Error,
        Fatal
    }

    internal class LoggingService
    {
        public static LogLevel DisplayLevel { get; set; }

        static LoggingService()
        {
            Instance = new LoggingService();
        }

        public static LoggingService Instance { get; private set; }

        public void Debug(string message)
        {
            Console.WriteLine("Debug: " + message);
        }

        public void Warn(string message)
        {
            Console.WriteLine("Warning: " + message);
        }

        public void Error(string message)
        {
            Console.WriteLine("Error: " + message);
        }
    }
}