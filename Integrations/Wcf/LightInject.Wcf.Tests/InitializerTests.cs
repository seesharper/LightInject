namespace LightInject.Wcf.Tests
{
    using System;
    using System.IO;
    using System.Reflection;
    using System.Web.Hosting;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// http://www.paraesthesia.com/archive/2010/06/17/unit-testing-an-asp.net-virtualpathprovider.aspx
    /// </summary>
    [TestClass]
    public class InitializerTests
    {
        // Instance property for the HostingEnvironment-enabled AppDomain.
        private AppDomain _hostingEnvironmentDomain = null;
        
        [TestInitialize]
        public void TestInitialize()
        {
            // Create the AppDomain that will support the VPP.
            this._hostingEnvironmentDomain =
              AppDomain.CreateDomain("HostingEnvironmentDomain",
              AppDomain.CurrentDomain.Evidence,
              AppDomain.CurrentDomain.SetupInformation,
              AppDomain.CurrentDomain.PermissionSet);

            // Set some required data that the runtime needs.
            this._hostingEnvironmentDomain.SetData(".appDomain", "HostingEnvironmentDomain");
            this._hostingEnvironmentDomain.SetData(".appId", "HostingEnvironmentTests");
            this._hostingEnvironmentDomain.SetData(".appPath", Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
            this._hostingEnvironmentDomain.SetData(".appVPath", "/");
            this._hostingEnvironmentDomain.SetData(".domainId", "HostingEnvironmentTests");

            // Initialize the hosting environment.
            HostingEnvironment environment = this._hostingEnvironmentDomain.CreateInstanceAndUnwrap(typeof(HostingEnvironment).Assembly.FullName, typeof(HostingEnvironment).FullName) as HostingEnvironment;

            //// Finally, register your VPP instance so you can test.
            //this.Execute(() =>
            //{
            //    HostingEnvironment.RegisterVirtualPathProvider(new VirtualSvcPathProvider());
            //});
        }

        [TestCleanup]
        public void TestCleanup()
        {
            // When the fixture is done, tear down the special AppDomain.
            AppDomain.Unload(this._hostingEnvironmentDomain);        
        }


        // This method allows you to execute code in the
        // special HostingEnvironment-enabled AppDomain.
        private void Execute(CrossAppDomainDelegate testMethod)
        {
            this._hostingEnvironmentDomain.DoCallBack(testMethod);
        }
        
        [TestMethod]
        public void Initialize_InstallsVirtualPathProvider()
        {
            Execute(LightInjectWcfInitializer.Initialize);                        
            Execute(() => Assert.IsNotNull(HostingEnvironment.VirtualPathProvider));                        
        }
    }
}