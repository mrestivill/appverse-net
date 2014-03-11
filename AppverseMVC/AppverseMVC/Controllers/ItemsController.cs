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

using Appverse.Web.Core.Controllers;
using Appverse.Web.Models;
using Appverse.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NHibernate.Criterion;

namespace Appverse.Web.Controllers
{
    [Authorize]
    public class ItemsController : AppverseBaseController
    {
        private readonly IItemRepository items;
        private static int pageSize =int.Parse(System.Configuration.ConfigurationManager.AppSettings["PageSize"].ToString());

        public ItemsController(IItemRepository items)
        {
            this.items = items;
        }


        [AllowAnonymous]
        public ActionResult WebAPIOperations(string id)
        {
            return ShowHelpBullets(id);
        }

        private void PrepareHelpBullets(string bullet)
        {
            string action = GetRouteDataString("action");


            if (string.IsNullOrEmpty(bullet) || !bullet.StartsWith("_"))
            {
                bullet = "_PartialFull" + action;
            }
            PartialHelpPageModel partial = new PartialHelpPageModel();
            partial.ViewName = action;
            partial.Page = bullet;



            partial.ReturnPage = "_PartialFull" + action;
            partial.SummaryMode = partial.Page == partial.ReturnPage;

            ViewBag.HelpPage = partial;
        }


        private ViewResult ShowHelpBullets(string bullet)
        {
            PrepareHelpBullets(bullet);

            return View();
        }

        /// <summary>
        /// Indexes the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="direction">The direction.</param>
        /// <param name="criteria">The criteria.</param>
        /// <returns>List of Items</returns>
        public virtual ActionResult Index(string id, bool? direction, string criteria)
        {
            int idValue;
            if (!int.TryParse(id, out idValue))
            {
                idValue = 1;
            }
                //var itemPage = items.GetPage<Item>(page.GetValueOrDefault(1));
            var itemPage = criteria == null ? items.GetPage<Item>(idValue, ItemsController.pageSize)
                                                 : direction == null ? items.GetPage<Item>(idValue, ItemsController.pageSize, criteria)
                                                                     : items.GetPage<Item>(idValue, ItemsController.pageSize, criteria, direction.Value);
            PrepareHelpBullets(id);
            return View(itemPage);
        }

        /// <summary>
        /// Views by ajax.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>ViewByAjax View</returns>
        public ActionResult ViewByAjax(string id)
        {
            ViewBag.PageSize = ItemsController.pageSize;
            var itemPage = items.GetPage<Item>(0, ItemsController.pageSize);
            PrepareHelpBullets(id);
            return View(itemPage);
        }

        /// <summary>
        /// Views by jquery. It is not necessary to pass the model to the view, it will be loaded asynchronously using jquery calling the GetItems method.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="page">The pager number.</param>
        /// <returns>ViewByJquery View</returns>
        public ActionResult ViewByJquery(string id, int? page)
        {
            PrepareHelpBullets(id);
            return View();
        }

        /// <summary>
        /// Gets the items.
        /// </summary>
        /// <param name="page">The page number.</param>
        /// <param name="direction">The sort direction ASC=True, DESC=False.</param>
        /// <param name="criteria">The criteria, Field selected to be Sort.</param>
        /// <returns>List of Items in Json format</returns>
        [HttpPost]
        public ActionResult GetItems(int? page, bool? direction, string criteria = null)
        {
            var itemPage = criteria == null ? items.GetPage<Item>(page.GetValueOrDefault(1), ItemsController.pageSize)
                                             : direction == null ? items.GetPage<Item>(page.GetValueOrDefault(1), ItemsController.pageSize, criteria)
                                                                 : items.GetPage<Item>(page.GetValueOrDefault(1), ItemsController.pageSize, criteria, direction.Value);
            JsonResult json = Json(itemPage, JsonRequestBehavior.AllowGet);
            return json;
        }

        //
        // GET: /Movies/Create
        /// <summary>
        /// Creates this instance.
        /// </summary>
        /// <returns>View Create</returns>
        public virtual ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Movies/Create
        /// <summary>
        /// Creates the specified new item.
        /// </summary>
        /// <param name="newItem">The new item.</param>
        /// <returns>The Index Redirect action</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult Create(Item newItem)
        {
            if (ModelState.IsValid)
            {
                items.Add(newItem);
                return RedirectToAction("Index");
            }

            return View(newItem);
        }


        /// <summary>
        /// Automatics the complete item.
        /// </summary>
        /// <param name="term">The term. </param>
        /// <returns>JsonResult, item elements to autocomplete</returns>
        [OutputCache(Duration = 60, VaryByParam = "term", VaryByCustom = "user")]
        public JsonResult AutoCompleteItem(string term)
        {
            var titles = items.Get<Item>(DetachedCriteria.For<Item>().Add(Expression.Like("Title", term, MatchMode.Anywhere)));

            List<object> dropDownInfos = new List<object>();

            Logger.Debug("AutoCompleteItem method call not cached. Param term:" + term + " " + titles.Count + " " + DateTime.Now.ToString());

            foreach (var item in titles)
            {
                if (Request.IsAuthenticated)
                {
                    dropDownInfos.Add(new
                    {
                        label = item.Title,
                        moreDetails = "(" + DateTime.Now.ToLongTimeString() + " - " + User.Identity.Name + ")",
                        value = item.Id
                    });
                }
                else
                {
                    dropDownInfos.Add(new
                    {
                        label = item.Title,
                        moreDetails = "(" + DateTime.Now.ToString() + ")",
                        value = item.Id
                    });
                }

            }

            return Json(dropDownInfos, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public ActionResult SearchEdit(string ItemName, int? id)
        {
            if (id.HasValue)
            {
                var selectedItem = items.Get<Item>(id.Value);
                return RedirectToAction("Edit", new { selectedItem.Id });
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        //
        // GET: /MoreItems/Details/5
        /// <summary>
        /// Details the specified Item.
        /// </summary>
        /// <param name="id">The id of Item.</param>
        /// <returns>View Details</returns>
        public ActionResult Details(int id)
        {
            var selectedItem = items.Get<Item>(id);
            return View(selectedItem);
        }


        //
        // GET: /MoreItems/Edit/5
        /// <summary>
        /// Edits the Item in the specified identifier.
        /// </summary>
        /// <param name="id">The id Item to edit.</param>
        /// <returns>View Edit</returns>
        public ActionResult Edit(int id)
        {

            var locations = items.GetPage<Location>(0, 2);

            ViewBag.Locations = items.GetPage<Location>(0, 2).Items.ToList<Location>();

            var selectedItem = items.Get<Item>(id);
            return View(selectedItem);
        }

        //
        // POST: /MoreItems/Edit/5
        /// <summary>
        /// Edits the specified modified item.
        /// </summary>
        /// <param name="modifiedItem">The modified item.</param>
        /// <returns>The Index Redirect action</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Item modifiedItem)
        {
            try
            {
                //modifiedItem.Id = id;
                items.Update(modifiedItem);
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /MoreItems/Delete/5
        /// <summary>
        /// Deletes the specified identifier.
        /// </summary>
        /// <param name="id">The id Itemo to delete.</param>
        /// <returns>View Delete</returns>
        public ActionResult Delete(int id)
        {
            var selectedItem = items.Get<Item>(id);
            return View(selectedItem);
        }

        //
        // POST: /MoreItems/Delete/5
        /// <summary>
        /// Deletes the specified item to delete.
        /// </summary>
        /// <param name="itemToDelete">The item to delete.</param>
        /// <returns>The Index Redirect action</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(Item itemToDelete)
        {
            try
            {
                items.Delete<Item>(itemToDelete.Id);
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
