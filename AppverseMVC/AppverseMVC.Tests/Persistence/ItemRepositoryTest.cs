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

using Appverse.Core.Facilities;
using Appverse.Core.Interceptors.Cache;
using Appverse.Core.Interceptors.Cache.Attributes;
using Appverse.Core.Repositories;
using Appverse.Web.Models;
using Castle.Core;
using Castle.Core.Logging;
using Castle.Facilities.Logging;
using Castle.MicroKernel;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading;
namespace Appverse.Web.Tests.Persistence
{
    [TestClass]
    public class ItemRepositoryTest
    {
        /// <summary>
        /// The container with all the controllers
        /// </summary>
        private IWindsorContainer container;

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


            //<!-- Cache Provider-->
            //<component id="CacheProvider"
            //     service="Gft.Appverse.Container.Interceptors.Cache.ICacheProvider, CastleApp"
            //     type="Gft.Appverse.Container.Interceptors.Cache.ObjectCacheProvider, CastleApp"
            //     lifestyle="singleton">
            //</component>
            container.Register(Component.For<ICacheProvider>().ImplementedBy<ObjectCacheProvider>().Named("CacheProvider"));

            string connectionString = ConfigurationManager.ConnectionStrings["ShowcaseConnection"].ConnectionString;

            container.AddFacility<PersistenceFacility<Appverse.Web.Models.EntityBase>>(f => f.Configure(connectionString, true));



            //<component id="ItemRepository" service="Appverse.Core.Repositories.IItemRepository" type="Appverse.Core.Repositories.ItemRepository" lifestyle="transient">
            //  <interceptors>
            //    <interceptor>${LogInterceptor}</interceptor>
            //  </interceptors>
            //</component>
            container.Register(Component.For<Appverse.Core.Repositories.IItemRepository>().ImplementedBy<Appverse.Core.Repositories.ItemRepository>().LifestyleTransient());


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


        public ItemRepositoryTest()
        {
            BootstrapContainer();
        }


        /// <summary>
        /// To view the queries in SQL Server Express, we recomend the tool:  http://expressprofiler.codeplex.com/
        /// </summary>
        [TestMethod]
        public void TestGet1()
        {
            IItemRepository repository = container.Resolve<IItemRepository>();

            Item item = repository.Get<Item>(1);


            var itemPage = repository.GetPage<Item>(1, 6, "Id", true);
            System.Diagnostics.Debug.WriteLine("------------");

            System.Diagnostics.Debug.WriteLine(itemPage.Items.GetLength(0));
            System.Diagnostics.Debug.WriteLine(itemPage.Items[0].Id);



            itemPage = repository.GetPage<Item>(1, 6, "Id", false);
            System.Diagnostics.Debug.WriteLine("------------");
            
            System.Diagnostics.Debug.WriteLine(itemPage.Items.GetLength(0));
            System.Diagnostics.Debug.WriteLine(itemPage.Items[0].Id);
        }

        [TestMethod]
        public void TestGet2()
        {
            IItemRepository repository = container.Resolve<IItemRepository>();

            Item item = repository.Get<Item>(1);



            var itemPage = repository.GetPage<Item>(0, 0, "Id", true);
            System.Diagnostics.Debug.WriteLine("------------");

            System.Diagnostics.Debug.WriteLine(itemPage.Items.GetLength(0));
            System.Diagnostics.Debug.WriteLine(itemPage.Items[0].Id);
        }


        /// <summary>
        /// To view the queries in SQL Server Express, we recomend the tool:  http://expressprofiler.codeplex.com/
        /// </summary>
        [TestMethod]
        public void TestGetPage()
        {
            IItemRepository repository = container.Resolve<IItemRepository>();

            Item item = repository.Get<Item>(1);


            // Full pagination

            var itemPage1 = repository.GetPage<Item>(1, 6, null);
            System.Diagnostics.Debug.WriteLine("---------Without filters------------");
            System.Diagnostics.Debug.WriteLine("Total count: " + itemPage1.TotalItemsCount);
            System.Diagnostics.Debug.WriteLine("Title first item:" + itemPage1.Items[0].Title);

            // Adding a filter in the pagination query

            NHibernate.Criterion.DetachedCriteria criteria = NHibernate.Criterion.DetachedCriteria.For<Item>();
            criteria.Add(NHibernate.Criterion.Expression.Like("Title", "u", NHibernate.Criterion.MatchMode.Anywhere));

            var itemPage2 = repository.GetPage<Item>(1, 6, criteria);
            System.Diagnostics.Debug.WriteLine("---------With filters------------");
            System.Diagnostics.Debug.WriteLine("Total count: " + itemPage2.TotalItemsCount);
            System.Diagnostics.Debug.WriteLine("Title first item:" + itemPage2.Items[0].Title);


            Assert.AreNotEqual(itemPage1.TotalItemsCount, itemPage2.TotalItemsCount, "First itemPage should contain more total items");
        }


        /// <summary>
        /// To view the queries in SQL Server Express, we recomend the tool:  http://expressprofiler.codeplex.com/
        /// </summary>
        [TestMethod]
        public void TestCount()
        {
            IItemRepository repository = container.Resolve<IItemRepository>();

            Item item = repository.Get<Item>(1);


            // Full count
            int count1 = repository.Count<Item>();
            System.Diagnostics.Debug.WriteLine("---------Without filters------------");
            System.Diagnostics.Debug.WriteLine("Total count: " + count1);

            // Adding a filter 

            NHibernate.Criterion.DetachedCriteria criteria = NHibernate.Criterion.DetachedCriteria.For<Item>();
            criteria.Add(NHibernate.Criterion.Expression.Like("Title", "u", NHibernate.Criterion.MatchMode.Anywhere));

            int count2 = repository.Count<Item>(criteria);
            System.Diagnostics.Debug.WriteLine("---------With filters------------");
            System.Diagnostics.Debug.WriteLine("Total count: " + count2);


            Assert.AreNotEqual(count1, count2, "First count should be the biggest");
        }



        /// <summary>
        /// To view the queries in SQL Server Express, we recomend the tool:  http://expressprofiler.codeplex.com/
        /// </summary>
        [TestMethod]
        public void TestSave()
        {
            IItemRepository repository = container.Resolve<IItemRepository>();

            //Item item = repository.Get<Item>(1);


            Item item = new Item();
            item.Title = "Test save " + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
            item.Value = DateTime.Now.Second;
            item.Moment = DateTime.Now;

            Location location = new Location();
            location.Name="BCN";
            repository.Add<Location>(location);

            item.Location = location;


            Item item2 = repository.Add<Item>(item);
            Item item3 = repository.Add<Item>(item2);

            //item3.Location += " modification";
            repository.Update<Item>(item3);
        }


        /// <summary>
        /// To view the queries in SQL Server Express, we recomend the tool:  http://expressprofiler.codeplex.com/
        /// </summary>
        [TestMethod]
        public void TestUpdate()
        {
            IItemRepository repository = container.Resolve<IItemRepository>();


            Item item = new Item();
            item.Title = "Test save " + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
            item.Value = DateTime.Now.Second;
            item.Moment = DateTime.Now;
            //item.Location = this.GetType().Name;


            repository.Add<Item>(item);

            item.Title = "Test update " + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
            item.Value = DateTime.Now.Second;
            item.Moment = DateTime.Now;
            //item.Location = this.GetType().Name;

            repository.Update<Item>(item);
        }


        [TestMethod]
        public void TestDelete()
        {
            IItemRepository repository = container.Resolve<IItemRepository>();


            Item item = new Item();
            item.Title = "Test save " + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
            item.Value = DateTime.Now.Second;
            item.Moment = DateTime.Now;
            //item.Location = this.GetType().Name;
            repository.Add<Item>(item);


            repository.Delete<Item>(item.Id);

            Item itemAfterDelete = repository.Get<Item>(item.Id);

            Assert.IsNull(itemAfterDelete, "Item has not been deleted");
        }

        



        /// <summary>
        /// To view the queries in SQL Server Express, we recomend the tool:  http://expressprofiler.codeplex.com/
        /// </summary>
        [TestMethod]
        public void TestQuery()
        {
            IItemRepository repository = container.Resolve<IItemRepository>();

            //  Query without parameters
            IList<Item> aa = repository.Get<Item>("FROM Item", new List<Appverse.Core.Repositories.Parameter>());

            //  Query with parameters
            List<Appverse.Core.Repositories.Parameter> parameters = new List<Appverse.Core.Repositories.Parameter>();
            parameters.Add(new Appverse.Core.Repositories.Parameter("Id", 1));
            IList<Item> bb = repository.Get<Item>("FROM Item WHERE Id = :Id", parameters);
        }
    }
}
