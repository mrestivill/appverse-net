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

using Castle.Core.Internal;
using Castle.MicroKernel.Facilities;
using Castle.MicroKernel.Registration;

using FluentNHibernate.Automapping;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;

using NHibernate;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;
using Appverse.Core.Facilities.Plumbing;

namespace Appverse.Core.Facilities
{
    /// <summary>
    /// Persistence facility class.
    /// This generic class creates/updates the model in the database
    /// </summary>
    public class PersistenceFacility<T> : AbstractFacility
    {
        /// <summary>
        /// Gets or sets the name of the connection string to the database.
        /// </summary>
        /// <value>
        /// The name of the connection string.
        /// </value>
        private string ConnectionString { get; set; }

        /// <summary>
        /// To force the schema update when initializing the class.
        /// </summary>
        /// <value>
        ///   <c>true</c> if we want to update schema; otherwise, <c>false</c>.
        /// </value>
        private bool UpdateSchema { get; set; }


        /// <summary>
        /// Configures the facility. This method can be called when the facility is added to the container. Example:
        ///    string connectionString = ConfigurationManager.ConnectionStrings["ShowcaseConnection"].ConnectionString;
        ///    container.AddFacility<PersistenceFacility<Appverse.Web.Models.EntityBase>>(f => f.Configure(connectionString, true));
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="updateSchema">if set to <c>true</c> [update schema].</param>
        public void Configure(string connectionString, bool updateSchema)
        {
            ConnectionString = connectionString;
            UpdateSchema = updateSchema;
        }

        
        /// <summary>
        /// Configures the persistence.
        /// </summary>
        /// <param name="config">The configuration.</param>
        protected virtual void ConfigurePersistence(Configuration config)
        {
            SchemaMetadataUpdater.QuoteTableAndColumns(config);
        }

        /// <summary>
        /// Creates the mapping model.
        /// </summary>
        /// <returns></returns>
        protected virtual AutoPersistenceModel CreateMappingModel()
        {
            var m = AutoMap.Assembly(typeof(T).Assembly)
                .Where(IsDomainEntity)
                .OverrideAll(ShouldIgnoreProperty)
                .IgnoreBase<T>();

            return m;
        }

        /// <summary>
        /// The custom initialization for the Facility.This method registers the ISessionFactory and ISession classes
        /// </summary>
        /// <remarks>
        /// It must be overridden.
        /// </remarks>
        protected override void Init()
        {
            var config = BuildDatabaseConfiguration();

            //config.AddFilterDefinition(new NHibernate.Engine.FilterDefinition("IsDeleted", null, new Dictionary<string, NHibernate.Type.IType>(), true));

            //// create filters programatically

            //foreach (var mapping in config.ClassMappings)
            //{
            //    mapping.AddFilter("IsDeleted", "Deleted is null");
            //    foreach (var property in mapping.PropertyIterator)
            //    {
            //        if (property.Value is NHibernate.Mapping.Bag)
            //        {
            //            NHibernate.Mapping.Bag bagProperty = (NHibernate.Mapping.Bag)property.Value;
            //            bagProperty.AddFilter("IsDeleted", "Deleted is null");
            //        }
            //    }
            //}

            Kernel.Register(
                Component.For<ISessionFactory>()
                    .UsingFactoryMethod(_ => config.BuildSessionFactory()),
                Component.For<ISession>()
                    .UsingFactoryMethod(k => k.Resolve<ISessionFactory>().OpenSession())
                    .LifestylePerWebRequest() //.LifestyleSingleton()
                );
        }

        /// <summary>
        /// Determines if the domain is entity
        /// </summary>
        /// <param name="t">The t.</param>
        /// <returns></returns>
        protected virtual bool IsDomainEntity(Type t)
        {
            return typeof(T).IsAssignableFrom(t);
        }

        /// <summary>
        /// Setups the database.
        /// </summary>
        /// <returns></returns>
        protected virtual IPersistenceConfigurer SetupDatabase()
        {
            return MsSqlConfiguration.MsSql2008
                .UseOuterJoin()
                .ConnectionString(ConnectionString)
                .ShowSql();
        }

        /// <summary>
        /// Builds the database configuration.
        /// </summary>
        /// <returns></returns>
        private Configuration BuildDatabaseConfiguration()
        {
            // This configuration will update the schema if it exists and create it if it does not. Also, SchemaUpdate does not do destructive updates (dropping tables, columns, etc.). It will only add them.
            if (UpdateSchema == true)
            {
                return Fluently.Configure()
                    .Database(SetupDatabase)
                    .Mappings(m => m.AutoMappings.Add(CreateMappingModel()))
                    .ExposeConfiguration(cfg => new SchemaUpdate(cfg).Execute(false, true)) //SchemaUpdate will update the schema if it exists and create it if it does not. Also, SchemaUpdate does not do destructive updates (dropping tables, columns, etc.). It will only add them.
                    .ExposeConfiguration(ConfigurePersistence)
                    .BuildConfiguration();
            }
            // This configuration does not creates or updates in any case the scheme of the database
            else
            {
                return Fluently.Configure()
                    .Database(SetupDatabase)
                    .Mappings(m => m.AutoMappings.Add(CreateMappingModel()))
                    .ExposeConfiguration(ConfigurePersistence)
                    .BuildConfiguration();
            }
        }

        /// <summary>
        /// Indicates if the property has to be ignored or not.
        /// </summary>
        /// <param name="property">The property.</param>
        private void ShouldIgnoreProperty(IPropertyIgnorer property)
        {
            property.IgnoreProperties(p => p.MemberInfo.HasAttribute<DoNotMapAttribute>());
        }
    }
}