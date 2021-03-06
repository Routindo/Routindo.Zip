using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Umator.Contract.Services;

namespace Umator.Plugins.Zip.Tests
{
    [TestClass]
    public class TestAssemblyInit
    {
        [AssemblyInitialize]
        public static void Initialize(TestContext testContext)
        { 
            ServicesContainer.SetServicesProvider(new FakeServicesProvider());
        }
    }
}
