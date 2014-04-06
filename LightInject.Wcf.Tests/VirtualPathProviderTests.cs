namespace LightInject.Wcf.Tests
{
    using System;
    using System.Collections;
    using System.IO;
    using System.Reflection;
    using System.Web.Caching;
    using System.Web.Hosting;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    
    [TestClass]
    public class VirtualPathProviderTests
    {                 
        [TestMethod]
        public void FileExists_NonExistingServiceFile_ReturnsTrue()
        {
            Assert.IsTrue(GetProvider().FileExists("~/NoSuchFile.svc"));                        
        }

        [TestMethod]
        public void FileExists_ExistingFile_ReturnsTrue()
        {            
            Assert.IsTrue(GetProvider().FileExists("~/ExistingFile"));                       
        }

        [TestMethod]
        public void FileExists_NonExistingFile_ReturnsFalse()
        {
            Assert.IsFalse(GetProvider().FileExists("~/NoSuchFile"));
        }

        [TestMethod]
        public void DirectoryExists_NonExistingServiceFile_ReturnsTrue()
        {
            Assert.IsTrue(GetProvider().DirectoryExists("~/SampleService.svc"));                        
        }

        [TestMethod]
        public void DirectoryExists_ExistingDirectory_ReturnsTrue()
        {
            Assert.IsTrue(GetProvider().DirectoryExists("~/ExistingDirectory"));
        }

        [TestMethod]
        public void DirectoryExists_NonExistingDirectory_ReturnsTrue()
        {
            Assert.IsFalse(GetProvider().DirectoryExists("~/NoSuchDirectory"));
        }

        [TestMethod]
        public void GetCacheDependency_ExistingServiceFile_ReturnsNull()
        {
            Assert.IsNull(GetProvider().GetCacheDependency("~/SampleService.svc",null, DateTime.MinValue));
        }

        [TestMethod]
        public void GetCacheDependency_NonServiceFile_CallsGetCacheDependencyInPrevious()
        {
            FileProvider.GetCacheDependencyCallCount = 0;
            GetProvider().GetCacheDependency("~/index.html", null, DateTime.MinValue);
            Assert.AreEqual(1, FileProvider.GetCacheDependencyCallCount);            
        }

        [TestMethod]
        public void GetFile_NonExistingServiceFile_ReturnsFile()
        {
            StreamReader streamReader = new StreamReader(GetProvider().GetFile("~/SampleService.svc").Open());

            var content = streamReader.ReadToEnd();

            Assert.AreEqual("<%@ ServiceHost Service=\"SampleService\" Factory = \"LightInject.Wcf.LightInjectServiceHostFactory, LightInject.Wcf\" %>", content);
        }

        [TestMethod]
        public void GetFile_ExistingServiceFile_ReturnsFileContent()
        {
            StreamReader streamReader = new StreamReader(GetProvider().GetFile("~/NoSuchFile.svc").Open());
            var content = streamReader.ReadToEnd();

            Assert.AreEqual("SomeContent", content);
        }


        private VirtualPathProvider GetProvider()
        {
            var previousField = typeof(VirtualPathProvider).GetField(
             "_previous",
             BindingFlags.Instance | BindingFlags.NonPublic);

            var provider = new VirtualSvcPathProvider();

            previousField.SetValue(provider, new FileProvider());

            return provider;
        }


        public class FileProvider : VirtualSvcPathProvider
        {
            [ThreadStatic]
            public static int GetCacheDependencyCallCount;
            
            public override bool FileExists(string virtualPath)
            {                
                return virtualPath.EndsWith("ExistingFile");
            }

            public override bool DirectoryExists(string virtualDir)
            {
                return virtualDir.EndsWith("ExistingDirectory");
            }

            public override CacheDependency GetCacheDependency(string virtualPath, IEnumerable virtualPathDependencies, DateTime utcStart)
            {
                GetCacheDependencyCallCount++;
                return null;
            }

            public override VirtualFile GetFile(string virtualPath)
            {
                return new VirtualSvcFile(virtualPath, "SomeContent");                                  
            }
        }
    }
}