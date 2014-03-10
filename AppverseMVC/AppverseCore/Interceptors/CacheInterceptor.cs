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
using Castle.Core;
using Castle.DynamicProxy;
using Castle.Core.Interceptor;
using Appverse.Core.Interceptors.Cache;
using Castle.Core.Logging;
using Appverse.Core.Interceptors.Cache.Attributes;


namespace Appverse.Core.Interceptors
{
    /// <summary>
    /// Interceptor for caching.
    /// 
    /// Another possibility: http://consultingblogs.emc.com/owainwragg/archive/2008/10/31/caching-with-castle-windsor.aspx
    /// 
    /// If we need to intercept by method, we have this possibility: http://docs.castleproject.org/Windsor.Introduction-to-AOP-With-Castle.ashx
    /// </summary>
    public class CacheInterceptor : UtilsInterceptorClass, IInterceptor
    {
        /// <summary>
        /// The cache provider implementation.
        /// </summary>
        private readonly ICacheProvider cacheProvider;

        /// <summary>
        /// Defines whether caching is enabled.
        /// </summary>
        private readonly bool cachingIsEnabled;

        /// <summary>
        /// The cache timeout.
        /// </summary>
        private readonly TimeSpan cacheTimeout;

        /// <summary>
        /// writes to log caching hits.
        /// </summary>
        private readonly bool logCacheHits;

        /// <summary>
        /// Castle.Core.Logging 
        /// </summary>
        public ILogger Logger { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CacheInterceptor" /> class.
        /// </summary>
        /// <param name="cacheProvider">The cache provider.</param>
        /// <param name="cachingIsEnabled">if set to <c>true</c> caching is enabled.</param>
        /// <param name="cacheTimeoutSeconds">The cache timeout seconds.</param>
        public CacheInterceptor(ICacheProvider cacheProvider, bool cachingIsEnabled, int cacheTimeoutSeconds, bool logCacheHits)
        {
            this.cacheProvider = cacheProvider;
            this.cachingIsEnabled = cachingIsEnabled;
            this.cacheTimeout = TimeSpan.FromSeconds(cacheTimeoutSeconds);
            this.logCacheHits = logCacheHits;
        }

        /// <summary>
        /// Intercepts the specified invocation.
        /// </summary>
        /// <param name="invocation">The invocation.</param>
        public void Intercept(IInvocation invocation)
        {
            // check if the method has a return value
            if (!this.cachingIsEnabled || invocation.Method.ReturnType == typeof(void) || !invocation.MethodInvocationTarget.IsDefined(typeof(CacheMethod), false))
            {
                invocation.Proceed();
                return;
            }

            var cacheKey = invocation.TargetType.FullName + "." + GetMethodSignature(invocation);

            // try get the return value from the cache provider
            var item = this.cacheProvider.Get(cacheKey);

            if (item != null)
            {
                invocation.ReturnValue = item;

                if (logCacheHits) Logger.DebugFormat("Cache hit for invocation: {0}. Result: {1}",  cacheKey, item);

                return;
            }

            // call the intercepted method
            invocation.Proceed();

            if (invocation.ReturnValue != null)
            {
                this.cacheProvider.Insert(cacheKey, invocation.ReturnValue, this.cacheTimeout);
                if (logCacheHits) Logger.DebugFormat("Adding invocation to cache: {0}. Result: {1}", cacheKey, item);
            }
        }
    }
}
