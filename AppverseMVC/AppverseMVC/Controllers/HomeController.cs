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
using System.Web;
using System.Web.Mvc;
using Appverse.Web.Models;
using Appverse.Web.Core.Controllers;
using Appverse.Core.Repositories;

namespace Appverse.Web.Controllers
{   
    public class HomeController : AppverseBaseController
    {
        private readonly IItemRepository session;

        public HomeController(IItemRepository session)
        {
            this.session = session;
        }

        public ActionResult Index(string id)
        {
            ViewBag.Message = "Welcome to this showcase!";
            if (User.Identity.IsAuthenticated)
                ViewBag.ItemCount = session.Count<Item>(null);
            //session.QueryOver<Item>().RowCount();

            return ShowHelpBullets(id);
        }

        public ActionResult ActiveDirectory()
        {
            return ShowHelpBullets("_ActiveDirectory");
        }

        /// <summary>
        /// Prepares the help bullets to be showed at the bottom of the page.
        /// </summary>
        /// <param name="bullet">The bullet.</param>
        private void PrepareHelpBullets(string bullet)
        {
            string action = GetRouteDataString("action");

            if (string.IsNullOrEmpty(bullet) || !bullet.StartsWith("_"))
            {
                bullet = "_PartialFull";
            }
            PartialHelpPageModel partial = new PartialHelpPageModel();
            partial.ViewName = action;
            partial.Page = bullet;
            partial.ReturnPage = "_PartialFull";
            partial.SummaryMode = partial.Page == partial.ReturnPage;

            ViewBag.mvc4Link = System.Configuration.ConfigurationManager.AppSettings["Mvc4link"] != null ?
                               System.Configuration.ConfigurationManager.AppSettings["Mvc4link"].ToString() : "#";
            ViewBag.mvc5Link = System.Configuration.ConfigurationManager.AppSettings["Mvc5link"] != null ?
                               System.Configuration.ConfigurationManager.AppSettings["Mvc5link"].ToString() : "#";

            ViewBag.HelpPage = partial;
        }

        private ViewResult ShowHelpBullets(string bullet)
        {
            PrepareHelpBullets(bullet);

            return View();
        }

        private ViewResult ShowHelpBullets(string bullet, object model)
        {
            PrepareHelpBullets(bullet);

            return View(model);
        }

        public ActionResult Contact()
        {
            //ViewBag.Message = "Contact page.";
            return View();
        }


        /// <summary>
        /// Culture action view. To specify a new culture.
        /// </summary>
        /// <param name="newCulture">The new culture.</param>
        /// <returns></returns>
        public ActionResult Culture(string newCulture)
        {
            if (newCulture != null)
            {
                if (newCulture == "-")
                {
                    // We remove the culture of the cookie
                    ChangeCulture("");
                }
                else
                {
                    // save the culture into cookie
                    ChangeCulture(newCulture);
                }

                // We reload the view to apply the new culture
                return RedirectToAction("Culture");
            }

            return ShowHelpBullets("_Globalization", Request.UserLanguages);

            //return RedirectToAction("");
            //return View();
        }
    }
}