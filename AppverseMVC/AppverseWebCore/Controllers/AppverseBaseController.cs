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

using Castle.Core.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace Appverse.Web.Core.Controllers
{
    /// <summary>
    /// Appverse base controller. 
    /// All controllers must inherit from this controller. 
    /// It adds functionality for internationalization and authorization check
    /// </summary>
    public class AppverseBaseController : Controller
    {
        // this is Castle.Core.Logging.ILogger, not log4net.Core.ILogger
        public ILogger Logger { get; set; }

        public CultureHelper CultureHelper { get; set; }

        // http://stackoverflow.com/questions/10786492/which-different-between-executecore-execute-initialize-on-controller
        // http://stackoverflow.com/questions/9555069/executecore-in-base-class-not-fired-in-mvc-4-beta


        /// <summary>
        /// Begins to invoke the action in the current controller context.
        /// </summary>
        /// <param name="callback">The callback.</param>
        /// <param name="state">The state.</param>
        /// <returns>
        /// Returns an IAsyncController instance.
        /// </returns>
        protected override IAsyncResult BeginExecuteCore(AsyncCallback callback, object state)
        {
            string[] culturesName = null;
            // Attempt to read the culture cookie from Request
            HttpCookie cultureCookie = Request.Cookies[CultureHelper.CultureCookieName];
            if (cultureCookie != null && cultureCookie.Value != "")
            {
                culturesName = new string[1];
                culturesName[0] = cultureCookie.Value;
            }
            else
                culturesName = Request.UserLanguages; // obtain it from HTTP header AcceptLanguages

            // Validates culture name, returns first valid culture
            string cultureName = CultureHelper.GetImplementedCulture(culturesName);

            // Modify current thread's cultures            
            Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(cultureName);
            Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture;

            return base.BeginExecuteCore(callback, state);
        }

        /// <summary>
        /// Changes the culture saving it into a cookie
        /// </summary>
        /// <param name="newCulture">The new culture.</param>
        protected void ChangeCulture(string newCulture)
        {
            // saves the culture into cookie
            HttpCookie cookie = new HttpCookie(CultureHelper.CultureCookieName, newCulture);
            if (string.IsNullOrEmpty(newCulture))
                cookie.Expires = DateTime.Now.AddYears(-1);
            else
                cookie.Expires = DateTime.Now.AddYears(1);
            HttpContext.Response.SetCookie(cookie);
        }


        ///// <summary>
        ///// Called when authorization occurs. It checks if current logged used is valid. If not, the page is redirected to the login screen.
        ///// </summary>
        ///// <param name="filterContext">Information about the current request and action.</param>
        //protected override void OnAuthorization(AuthorizationContext filterContext)
        //{
            //if (WebSecurity.Initialized == true && WebSecurity.CurrentUserId == -1 && string.IsNullOrEmpty(WebSecurity.CurrentUserName) == false)
            //{
            //    WebSecurity.Logout();
            //    //return RedirectToAction("LogOff", "Account");
            //    var url = new UrlHelper(filterContext.RequestContext);
            //    var verifyEmailUrl = url.Action("Login", "Account", null);
            //    filterContext.Result = new RedirectResult(verifyEmailUrl);

            //}

            //base.OnAuthorization(filterContext);
        //}


        protected string GetRouteDataString(string valueName)
        {
            string data = "";

            try
            {
                data = ControllerContext.RouteData.GetRequiredString(valueName);
            }
            catch (Exception) { }
            return data;
        }
    }
}
