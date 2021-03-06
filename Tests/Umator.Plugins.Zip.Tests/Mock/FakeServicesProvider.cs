using System;
using Umator.Contract.Services;

namespace Umator.Plugins.Zip.Tests
{
    public class FakeServicesProvider: IServicesProvider
    {
        public ILoggingService GetLoggingService(string name, Type type = null)
        {
            return new FakeLoggingService(name, type);
        }
    }
}