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

using Castle.MicroKernel;
using Castle.MicroKernel.Lifestyle;

using IDependencyResolver = System.Web.Http.Dependencies.IDependencyResolver;

namespace Appverse.Web.Installers.Dependencies
{
    /// <summary>
    /// During the life-cycle of a request, a dependency scope (implemented by IDependencyScope) is created on the request using
    /// the BeginScope method on the IDependencyResolver. At the end of the request, the dependency scope is disposed. This allow
    /// us to implement the Resolve / Release pattern using Castle Windsor.
    /// 
    /// The Scoped Lifestyle is a new lifestyle in Castle.Windsor 3 that makes it possible to create an arbitrary scope bounded by 
    /// the object returned from the Container.BeginScope call. When this object is disposed, the scope is ended and Castle will 
    /// then release all the objects with Scoped lifestyle that have been resolved in the same call stack after the scope is generated.
    /// http://cangencer.wordpress.com/2012/12/22/integrating-asp-net-web-api-with-castle-windsor/
    /// </summary>
    public class WindsorDependencyScope : IDependencyScope
    {
        private readonly IKernel container;
        private readonly IDisposable scope;

        /// <summary>
        /// Initializes a new instance of the <see cref="WindsorDependencyScope"/> class.
        /// </summary>
        /// <param name="container">The container.</param>
        public WindsorDependencyScope(IKernel container)
        {
            this.container = container;
            this.scope = container.BeginScope();
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
            System.Diagnostics.Debug.WriteLine("IDependencyScope " + serviceType.FullName);
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
            System.Diagnostics.Debug.WriteLine("IDependencyScope " + serviceType.FullName);
            return this.container.ResolveAll(serviceType).Cast<object>();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.scope.Dispose();
        }
    }
}