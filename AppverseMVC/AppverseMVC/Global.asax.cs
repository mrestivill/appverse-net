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

using Appverse.Web.Core;
using Appverse.Web.Installers.Dependencies;
using Appverse.Web.Models;
using Appverse.Web.Models.DTO;
using AutoMapper;
using Castle.Windsor;
using Castle.Windsor.Installer;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;


namespace Appverse.Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        private static IWindsorContainer container;


        private void PrepareMappers()
        {
            Mapper.CreateMap<Item, ItemDTO>()
                .ForMember(dest => dest.Location, opt => opt.ResolveUsing<CustomResolver>().FromMember(s => s.Location));
        }

        public class CustomResolver : ValueResolver<Location, LocationDTO>
        {
            protected override LocationDTO ResolveCore(Location source)
            {
                if (source != null)
                {
                    LocationDTO loc = new LocationDTO();
                    loc.Name = source.Name;
                    loc.Id = source.Id;
                    return loc;
                }
                else
                    return null;
            }
        }

        private static void BootstrapContainer()
        {
            container = new WindsorContainer()
                .Install(FromAssembly.This()).Install(FromAssembly.Containing(typeof(CultureHelper)))
                .Install(Configuration.FromAppConfig());

            // RegisterDependencyResolver for Web API
            GlobalConfiguration.Configuration.DependencyResolver = new WindsorDependencyResolver(container.Kernel);

            var controllerFactory = new WindsorControllerFactory(container.Kernel);
            ControllerBuilder.Current.SetControllerFactory(controllerFactory);
        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            PrepareMappers();


            //http://www.asp.net/web-api/overview/formats-and-model-binding/json-and-xml-serialization
            //http://stackoverflow.com/questions/14053109/failed-to-serialize-the-response-body-for-content-type

            var json = GlobalConfiguration.Configuration.Formatters.JsonFormatter;
            json.UseDataContractJsonSerializer = true;

            //var json = GlobalConfiguration.Configuration.Formatters.JsonFormatter;
            //json.SerializerSettings.PreserveReferencesHandling = Newtonsoft.Json.PreserveReferencesHandling.All;

            HttpConfiguration config = GlobalConfiguration.Configuration;
            ((Newtonsoft.Json.Serialization.DefaultContractResolver)config.Formatters.JsonFormatter.SerializerSettings.ContractResolver).IgnoreSerializableAttribute = true;

            BootstrapContainer();
        }

        protected void Application_End()
        {
            container.Dispose();
        }


        /// <summary>
        /// To cache multiple versions of page output based on custom strings.
        /// OutputCache by Membership. It works when cache location is OutputCacheLocation.Server or Any.
        /// The outputcache attribute for chached view should look like this: [OutputCache(Duration = 60, VaryByParam = "term", VaryByCustom = "user")]
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="arg">The argument.</param>
        /// <returns></returns>
        public override string GetVaryByCustomString(HttpContext context, string arg)
        {
            if (arg == "user")
            {
                return context.User.Identity.IsAuthenticated ? context.User.Identity.Name : string.Empty;
            }
            return base.GetVaryByCustomString(context, arg);
        }
    }
}
