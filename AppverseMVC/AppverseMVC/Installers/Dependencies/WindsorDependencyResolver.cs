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
using System.Web.Http.Dependencies;

namespace Appverse.Web.Installers.Dependencies
{
    using Castle.MicroKernel;

    using IDependencyResolver = System.Web.Http.Dependencies.IDependencyResolver;

    /// <summary>
    /// There are a few different ways to integrate a container with Web API, however since Castle uses the Resolve-Release pattern, I was happiest with
    /// using the Dependency Scopes built in to WebAPI as it supports releasing dependencies in a graceful way and it works under both IIS and self-hosting.
    /// The key interfaces are IDependencyResolver and IDependencyScope which serve as the main integration points into the Web API. Keep in mind the
    /// IDependencyResolver in Web API is not the same as the one in MVC, even though they share the same interface name. The namespaces are different, 
    /// which can be a little confusing as a Web API application is also a MVC application at the same time. This seems to be a common theme throughout Web
    /// API as several key interfaces will have a Web API version and MVC version and things might get confusing sometimes.
    /// 
    /// The IDependencyResolver interface is used to resolve everything outside a request scope. This means all the infrastructural interfaces of WebApi
    /// (for example, IHttpControllerActivator) are resolved using the IDependencyResolver. If the resolver returns null, then the default implementation
    /// is used. The IDependencyResolver is never disposed by the framework, and it’s only ever used to resolve singletons so the Resolve / Release pattern
    /// does not apply to the IDependencyResolver itself.  A typical IDependencyResolver implementation using Windsor might look this:
    /// http://cangencer.wordpress.com/2012/12/22/integrating-asp-net-web-api-with-castle-windsor/
    /// </summary>
    internal class WindsorDependencyResolver : IDependencyResolver
    {
        private readonly IKernel container;

        /// <summary>
        /// Initializes a new instance of the <see cref="WindsorDependencyResolver"/> class.
        /// </summary>
        /// <param name="container">The container.</param>
        public WindsorDependencyResolver(IKernel container)
        {
            this.container = container;
        }

        /// <summary>
        /// Starts a resolution scope.
        /// </summary>
        /// <returns>
        /// The dependency scope.
        /// </returns>
        public IDependencyScope BeginScope()
        {
            return new WindsorDependencyScope(this.container);
        }

        /// <summary>
        /// Retrieves a service from the scope.
        /// </summary>
        /// <param name="serviceType">The service to be retrieved.</param>
        /// <returns>
        /// The retrieved service.
        /// </returns>
        public object GetService(Type serviceType)
        {
            System.Diagnostics.Debug.WriteLine("IDependencyResolver " + serviceType.FullName);
            return this.container.HasComponent(serviceType) ? this.container.Resolve(serviceType) : null;
        }

        /// <summary>
        /// Retrieves a collection of services from the scope.
        /// </summary>
        /// <param name="serviceType">The collection of services to be retrieved.</param>
        /// <returns>
        /// The retrieved collection of services.
        /// </returns>
        public IEnumerable<object> GetServices(Type serviceType)
        {
             System.Diagnostics.Debug.WriteLine("IDependencyResolver " + serviceType.FullName);
            return this.container.ResolveAll(serviceType).Cast<object>();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
        }
    }
}