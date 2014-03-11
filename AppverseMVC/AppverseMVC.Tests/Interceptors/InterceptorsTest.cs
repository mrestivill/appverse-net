/*
 Copyright (c) 2014 GFT Appverse, S.L., Sociedad Unipersonal.

 This Source Code Form is subject to the terms of the Appverse Public License
 Version 2.0 (“APL v2.0”). If a copy of the APL was not distributed with this
 file, You can obtain one at http://www.appverse.mobi/licenses/apl_v2.0.pdf. [^]

 Redistribution and use in source and binary forms, with or without modification,
 are permitted provided that the conditions of the AppVerse Public License v2.0
 are met.

 THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
 ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
 WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
 DISCLAIMED. EXCEPT IN CASE OF WILLFUL MISCONDUCT OR GROSS NEGLIGENCE, IN NO EVENT
 SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT,
 INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT
 LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR
 PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY,
 WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT(INCLUDING NEGLIGENCE OR OTHERWISE)
 ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE
 POSSIBILITY OF SUCH DAMAGE.
 */

using Appverse.Core.Interceptors.Cache;
using Appverse.Core.Interceptors.Cache.Attributes;
using Castle.Core;
using Castle.Core.Logging;
using Castle.Facilities.Logging;
using Castle.MicroKernel;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading;

namespace Appverse.Web.Tests.Interceptors
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class InterceptorsTest
    {
        /// <summary>
        /// The container with all the controllers
        /// </summary>
        private IWindsorContainer container;
        private int cacheDurationSeconds = 3;
        private ITestingClass testingClass;

        /// <summary>
        /// Bootstraps the container.
        /// </summary>
        private void BootstrapContainer()
        {
            container = new WindsorContainer();
            container.Kernel.ComponentModelCreated += new ComponentModelDelegate(Kernel_ComponentModelCreated);
            container.AddFacility<LoggingFacility>(f => f.UseLog4Net("log4net.config"));

            container.Register(Component.For<Appverse.Core.Interceptors.LogInterceptor>().ImplementedBy<Appverse.Core.Interceptors.LogInterceptor>().LifestyleTransient()
                .DependsOn(Property.ForKey("LogInputParameters").Eq(true), Property.ForKey("LogResult").Eq(true), Property.ForKey("LogProfiling").Eq(true), Property.ForKey("LogExceptions").Eq(true)));
                
            container.Register(Component.For<ITestingClass>().ImplementedBy<TestingClass>());

            //<!-- Cache Provider-->
            //<component id="CacheProvider"
            //     service="Gft.Appverse.Container.Interceptors.Cache.ICacheProvider, CastleApp"
            //     type="Gft.Appverse.Container.Interceptors.Cache.ObjectCacheProvider, CastleApp"
            //     lifestyle="singleton">
            //</component>
            container.Register(Component.For<ICacheProvider>().ImplementedBy<ObjectCacheProvider>().Named("CacheProvider"));

            //<!-- Cache Interceptor-->
            //<component id="CacheInterceptor"
            //           service="Gft.Appverse.Container.Interceptors.CacheInterceptor, CastleApp"
            //           type="Gft.Appverse.Container.Interceptors.CacheInterceptor, CastleApp"
            //           lifestyle="singleton">
            //  <parameters>
            //    <cachingIsEnabled>true</cachingIsEnabled>
            //    <cacheTimeoutSeconds>60</cacheTimeoutSeconds>
            //    <cacheProvider>${CacheProvider}</cacheProvider>
            //    <logCacheHits>true</logCacheHits>
            //  </parameters>
            //</component>
            container.Register(Component.For<Appverse.Core.Interceptors.CacheInterceptor>().ImplementedBy<Appverse.Core.Interceptors.CacheInterceptor>().LifestyleTransient()
                .DependsOn(Property.ForKey("cachingIsEnabled").Eq(true), Property.ForKey("cacheTimeoutSeconds").Eq(cacheDurationSeconds), Property.ForKey("logCacheHits").Eq(true),
                Dependency.OnComponent(typeof(ICacheProvider), "cacheProvider")));
        }


        /// <summary>
        /// Kernel_s the component model created.
        /// To avoid the problem with LifestyleType.PerWebRequest that cannot be used in test cases, you have define an event in the contructor of the unit test to override the LifestyleType.
        /// </summary>
        /// <param name="model">The model.</param>
        void Kernel_ComponentModelCreated(Castle.Core.ComponentModel model)
        {
            if (model.LifestyleType == LifestyleType.Undefined)
                model.LifestyleType = LifestyleType.Transient;

            if (model.LifestyleType == LifestyleType.PerWebRequest)
                model.LifestyleType = LifestyleType.Transient;
        } 


        public InterceptorsTest()
        {
            BootstrapContainer();

            testingClass = container.Resolve<ITestingClass>();
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void TestResolveLogger()
        {
            ILogger logger = container.Resolve<ILogger>();
            logger.Debug("Test Debug message");
            logger.Info("Test Info message");
            logger.Warn("Test Warn message");
            logger.Error("Test Error message");
        }

        [TestMethod]
        public void TestResolveLogInterceptor()
        {
            var logInterceptor = container.Resolve<Appverse.Core.Interceptors.LogInterceptor>();

            Assert.IsNotNull(logInterceptor);
        }


        [TestMethod]
        public void TestResolveCacheInterceptor()
        {
            var cacheInterceptor = container.Resolve<Appverse.Core.Interceptors.CacheInterceptor>();

            Assert.IsNotNull(cacheInterceptor);
        }

        [TestMethod]
        public void TestLogInterceptor()
        {
            //ITestingClass main = container.Resolve<ITestingClass>();
            Assert.AreEqual(2 + 3, testingClass.TestMethod("1", 2, 3));

            testingClass = container.Resolve<ITestingClass>();
            Assert.AreEqual(8 + 3, testingClass.TestMethod(null, 8, 3));

            ComplexData complex = new ComplexData();
            complex.Prop1 = 1;
            complex.Prop2 = "Text";
            complex.Prop3 = TimeSpan.FromSeconds(12);
            complex.Prop4 = 12.2;


            Assert.AreEqual(2, testingClass.TestComplexData("1", 2, complex));

            Assert.IsNotNull(testingClass.NoParams());
        }

        [TestMethod]
        public void TestCacheInterceptorFull()
        {
            //ITestingClass main = container.Resolve<ITestingClass>();

            long result0 = testingClass.NoParams();

            DateTime result1 = testingClass.GetDateTimeMethod("date", 2, 3, TimeSpan.FromSeconds(12));

            Thread.Sleep(cacheDurationSeconds*1000-1000);

            DateTime result2 = testingClass.GetDateTimeMethod("date", 2, 3, TimeSpan.FromSeconds(12));
            Assert.AreEqual(result2, result1, "Method call has not been cached. Both results should be equal.");

            DateTime resultDif1 = testingClass.GetDateTimeMethod("dateDif", 2, 3, TimeSpan.FromSeconds(12));
            Assert.AreNotEqual(resultDif1, result1, "Method call has been cached. The params were different -1.");


            DateTime resultDif2 = testingClass.GetDateTimeMethod("date", 3, 3, TimeSpan.FromSeconds(12));
            Assert.AreEqual(resultDif2, resultDif1, "Method call has been cached. The params were different -2.");
            Assert.AreNotEqual(resultDif2, result1, "Method call has been cached. The params were different -3.");


            Thread.Sleep(cacheDurationSeconds*1000 + 1000);
            DateTime result3 = testingClass.GetDateTimeMethod("date", 2, 3, TimeSpan.FromSeconds(12));
            Assert.AreNotEqual(result3, result2, "Method call has been cached. It should not been cached. Value shoud be expired.");
        }

        [TestMethod]
        public void TestCacheInterceptorBasic()
        {
            DateTime result1 = testingClass.GetDateTimeMethod("date", 1, 3, TimeSpan.FromSeconds(11));
            DateTime result2 = testingClass.GetDateTimeMethod("date", 1, 3, TimeSpan.FromSeconds(11));
            Assert.AreEqual(result2, result1, "Method call has not been cached. Both results should be equal.");
        }
    }


    [Interceptor(typeof(Appverse.Core.Interceptors.LogInterceptor))]
    [Interceptor(typeof(Appverse.Core.Interceptors.CacheInterceptor))]
    public class TestingClass : ITestingClass
    {        
        public int TestMethod(string param1, int param2, int param3)
        {
            return param2 + param3;
        }

        [CacheMethod]
        public DateTime GetDateTimeMethod(string param1, int param2, int param3, TimeSpan param4)
        {
            return DateTime.Now;
        }


        public int TestComplexData(string param1, int param2, ComplexData paramComplexData)
        {
            return param2;
        }

        public long NoParams()
        {
            return DateTime.Now.Ticks;
        }
    }

    public interface ITestingClass
    {
        int TestMethod(string param1, int param2, int param3);
        DateTime GetDateTimeMethod(string param1, int param2, int param3, TimeSpan param4);
        int TestComplexData(string param1, int param2, ComplexData paramComplexData);
        long NoParams();
    }

    public class ComplexData
    {
        public int Prop1 { get; set; }
        public String Prop2 { get; set; }
        public TimeSpan Prop3 { get; set; }
        public Double Prop4 { get; set; }
    }
}
