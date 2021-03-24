using System;
using Routindo.Contract.Services;

namespace Routindo.Plugins.Zip.Tests
{
    public class FakeServicesProvider: IServicesProvider
    {
        public ILoggingService GetLoggingService(string name, Type type = null)
        {
            return new FakeLoggingService(name, type);
        }

        public IEnvironmentService GetEnvironmentService()
        {
            throw new NotImplementedException();
        }
    }
}