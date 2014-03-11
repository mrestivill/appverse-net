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
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Helpers;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace Appverse.Web.Core
{
    namespace Attributes
    {
        /// <summary>
        /// Web API and ValidateAntiForgeryToken
        /// Validates Anti-Forgery CSRF tokens for Web API
        /// To use this feature we have to add the ExecuteAuthorizationFilterAsync method as attribute (annotation in java) in the methods of the Web API controller
        /// we want to validate the antiForgery token
        /// </summary>
        /// <remarks>
        /// We have been unable to register this class using dependency injection to set the ILogger property
        /// </remarks>
        [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
        public sealed class ValidateHttpAntiForgeryToken : FilterAttribute, IAuthorizationFilter
        {
            //// this is Castle.Core.Logging.ILogger, not log4net.Core.ILogger
            //public ILogger Logger { get; set; }

            /// <summary>
            /// Executes the authorization filter.
            /// </summary>
            /// <param name="actionContext">The action context.</param>
            /// <param name="cancellationToken">The cancellation token associated with the filter.</param>
            /// <param name="continuation">The continuation.</param>
            /// <returns>
            /// The authorization filter to synchronize.
            /// </returns>
            public Task<HttpResponseMessage> ExecuteAuthorizationFilterAsync(HttpActionContext actionContext, CancellationToken cancellationToken, Func<Task<HttpResponseMessage>> continuation)
            {
                HttpRequestMessage request = actionContext.Request;

                try
                {
                    if (IsAjaxRequest(request))
                    {
                        ValidateRequestHeader(request);
                    }
                    else
                    {
                        AntiForgery.Validate();
                    }
                }
                catch (Exception)
                {
                    //LogManager.GetCurrentClassLogger().Warn("Anti-XSRF Validation Failed", ex);

                    actionContext.Response = new HttpResponseMessage
                    {
                        StatusCode = HttpStatusCode.Forbidden,
                        RequestMessage = actionContext.ControllerContext.Request
                    };
                    return FromResult(actionContext.Response);
                }
                return continuation();
            }

            /// <summary>
            /// From result.
            /// </summary>
            /// <param name="result">The result.</param>
            /// <returns></returns>
            private Task<HttpResponseMessage> FromResult(HttpResponseMessage result)
            {
                var source = new TaskCompletionSource<HttpResponseMessage>();
                source.SetResult(result);
                return source.Task;
            }

            /// <summary>
            /// Determines if the request has been done using ajax.
            /// </summary>
            /// <param name="request">The request.</param>
            /// <returns></returns>
            private bool IsAjaxRequest(HttpRequestMessage request)
            {
                IEnumerable<string> requestedWithHeaders;
                if (request.Headers.TryGetValues("X-Requested-With", out requestedWithHeaders))
                {
                    string headerValue = requestedWithHeaders.FirstOrDefault();
                    if (!String.IsNullOrEmpty(headerValue))
                    {
                        return String.Equals(headerValue, "XMLHttpRequest", StringComparison.OrdinalIgnoreCase);
                    }
                }
                // Let's check directy for the "X-XSRF-Token" header... To be able to accept the requests of WebApiTestClient (Simple Test Client for ASP.NET Web API)
                else
                {
                    if (request.Headers.TryGetValues("X-XSRF-Token", out requestedWithHeaders))
                    {
                        string headerValue = requestedWithHeaders.FirstOrDefault();
                        if (!String.IsNullOrEmpty(headerValue))
                        {
                            return true;
                        }
                    }
                }
                return false;
            }

            /// <summary>
            /// Validates the request header.
            /// </summary>
            /// <param name="request">The request.</param>
            /// <exception cref="System.InvalidOperationException">
            /// </exception>
            private void ValidateRequestHeader(HttpRequestMessage request)
            {
                var headers = request.Headers;
                var cookie = headers.GetCookies().Select(c => c[AntiForgeryConfig.CookieName]).FirstOrDefault();

                IEnumerable<string> xsrfHeaders;

                if (headers.TryGetValues("X-XSRF-Token", out xsrfHeaders))
                {
                    var rvt = xsrfHeaders.FirstOrDefault();

                    if (cookie == null)
                    {
                        throw new InvalidOperationException(String.Format("Missing {0} cookie", AntiForgeryConfig.CookieName));
                    }

                    AntiForgery.Validate(cookie.Value, rvt);
                }
                else
                {
                    var headerBuilder = new StringBuilder();

                    headerBuilder.AppendLine("Missing X-XSRF-Token HTTP header:");

                    foreach (var header in headers)
                    {
                        headerBuilder.AppendFormat("- [{0}] = {1}", header.Key, header.Value);
                        headerBuilder.AppendLine();
                    }

                    throw new InvalidOperationException(headerBuilder.ToString());
                }
            }
        }
    }
}
