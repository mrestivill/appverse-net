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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Castle.Windsor;
using Castle.Windsor.Installer;
using Appverse.Web.Controllers;
using Appverse.Web.Core;
using Castle.Core;
using Castle.MicroKernel;


namespace Appverse.Web.Tests.Controllers
{
    [TestClass]
    public class HomeControllerTest
    {
        /// <summary>
        /// The container with all the controllers
        /// </summary>
        private IWindsorContainer containerWithControllers;

        /// <summary>
        /// Bootstraps the container.
        /// </summary>
        private void BootstrapContainer()
        {
            containerWithControllers = new WindsorContainer();
            containerWithControllers.Kernel.ComponentModelCreated += new ComponentModelDelegate(Kernel_ComponentModelCreated);  

            containerWithControllers.Install(FromAssembly.Containing(typeof(HomeController))).Install(FromAssembly.Containing(typeof(CultureHelper)))
                .Install(Configuration.FromAppConfig());

            // RegisterDependencyResolver
            var controllerFactory = new WindsorControllerFactory(containerWithControllers.Kernel);
            ControllerBuilder.Current.SetControllerFactory(controllerFactory);
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

        public HomeControllerTest()
        {
            BootstrapContainer();
        }
        

        [TestMethod]
        public void Index()
        {
            // Arrange
            HomeController controller = containerWithControllers.Resolve<HomeController>();

            // Act
            ViewResult result = controller.Index(null) as ViewResult;

            // Assert
            Assert.IsNotNull(result); 
            Assert.IsNotNull(result.ViewBag.Message);
            Console.WriteLine(result.ViewBag.ItemCount);
            Assert.IsNotNull(result.ViewBag.ItemCount);
        }

        [TestMethod]
        public void Culture()
        {
            //TODO:  Mock
            // http://stackoverflow.com/questions/1228179/mocking-httpcontextbase-with-moq
            // http://stackoverflow.com/questions/16094126/how-to-handle-httpcontext-in-unittest-mvc
            // http://stackoverflow.com/questions/9063313/testing-response-setcookie-with-mvccontrib
            // https://github.com/Moq/moq4

            // Arrange
            HomeController controller = containerWithControllers.Resolve<HomeController>();

            // Act
            ViewResult result = controller.Culture("es") as ViewResult;

            // Assert
            Assert.AreEqual("Your application description page.", result.ViewBag.Message);
        }

        [TestMethod]
        public void Contact()
        {
            // Arrange
            HomeController controller = containerWithControllers.Resolve<HomeController>();

            // Act
            ViewResult result = controller.Contact() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
        }
    }
}
