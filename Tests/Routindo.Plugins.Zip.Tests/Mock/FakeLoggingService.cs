using System;
using Routindo.Contract.Services;

namespace Routindo.Plugins.Zip.Tests
{
    public class FakeLoggingService: ILoggingService
    {
        public FakeLoggingService(string name, Type type = null)
        {
            
        }
        public void Trace(string message)
        {
            Console.WriteLine(message);
        }

        public void Trace<T>(T value)
        {
            Console.WriteLine(value);
        }

        public void Trace(string message, params object[] args)
        {
            Console.WriteLine(message, args);
        }

        public void Trace(Exception exception, string message, params object[] args)
        {
            Console.WriteLine(exception + Environment.NewLine + message, args);
        }

        public void Debug(string message)
        {
            Console.WriteLine(message);
        }

        public void Debug<T>(T value)
        {
            Console.WriteLine(value);
        }

        public void Debug(string message, params object[] args)
        {
            Console.WriteLine(message, args);
        }

        public void Debug(Exception exception, string message, params object[] args)
        {
            Console.WriteLine(exception + Environment.NewLine + message, args);
        }

        public void Info(string message)
        {
            Console.WriteLine(message);
        }

        public void Info<T>(T value)
        {
            Console.WriteLine(value);
        }

        public void Info(string message, params object[] args)
        {
            Console.WriteLine(message, args);
        }

        public void Info(Exception exception, string message, params object[] args)
        {
            Console.WriteLine(exception + Environment.NewLine + message, args);
        }

        public void Warn(string message)
        {
            Console.WriteLine(message);
        }

        public void Warn<T>(T value)
        {
            Console.WriteLine(value);
        }

        public void Warn(string message, params object[] args)
        {
            Console.WriteLine(message, args);
        }

        public void Warn(Exception exception, string message, params object[] args)
        {
            Console.WriteLine(exception + Environment.NewLine + message, args);
        }

        public void Error(string message)
        {
            Console.WriteLine(message);
        }

        public void Error<T>(T value)
        {
            Console.WriteLine(value);
        }

        public void Error(string message, params object[] args)
        {
            Console.WriteLine(message, args);
        }

        public void Error(Exception exception, string message, params object[] args)
        {
            Console.WriteLine(exception + Environment.NewLine + message, args);
        }

        public void Fatal(string message)
        {
            Console.WriteLine(message);
        }

        public void Fatal<T>(T value)
        {
            Console.WriteLine(value);
        }

        public void Fatal(string message, params object[] args)
        {
            Console.WriteLine(message, args);
        }

        public void Fatal(Exception exception, string message, params object[] args)
        {
            Console.WriteLine(exception + Environment.NewLine + message, args);
        }
    }
}