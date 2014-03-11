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

using Appverse.Web.Core.Attributes;
using Appverse.Web.Models;
using Appverse.Core.Repositories;
using Castle.Core.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Appverse.Web.Controllers
{
    /// <summary>
    /// Web api (restful class)
    /// </summary>
    [Authorize]
    public class ItemsCRUDController : ApiController
    {
        // this is Castle.Core.Logging.ILogger, not log4net.Core.ILogger
        public ILogger Logger { get; set; }

        public IItemRepository Repository { get; set; }

        
        /// <summary>
        ///  GET api/itemsCRUD
        ///  Gets all items
        /// </summary>
        /// <returns>returns all itemps</returns>
        [AllowAnonymous]
        [AddCachingHeader(90)]
        public IEnumerable<Item> Get()
        {
            Logger.InfoFormat("Get operation");

            return Repository.GetPage<Item>(0, 0).Items.ToArray<Item>();
        }

        /// <summary>
        /// GET api/itemsCRUD/5
        /// Get an item
        /// </summary>
        /// <param name="id">The identifier of the requested item</param>
        /// <returns></returns>
        /// <exception cref="System.Web.Http.HttpResponseException"></exception>
        public Item GetItem(int id)
        {
            Logger.InfoFormat("Get(" + id.ToString() + ") operation");
            Item item = Repository.Get<Item>(id);
            if (item == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
            return item;
        }

        /// <summary>
        /// GET api/ItemsCRUD?title=text
        /// Gets the items by title.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <returns></returns>
        public IEnumerable<Item> GetItemsByTitle(string title)
        {
            return Repository.GetPage<Item>(0, 50).Items.Where(p => string.Equals(p.Title, title, StringComparison.OrdinalIgnoreCase));
        }

        
        /// <summary>
        /// POST api/itemsCRUD - Create
        /// Posts a new item.
        /// </summary>
        /// <param name="newItem">The new item.</param>
        /// <returns></returns>
        /// <exception cref="System.Web.Http.HttpResponseException"></exception>
        [ValidateHttpAntiForgeryToken]
        public HttpResponseMessage PostItem(Item newItem)
        {
            Logger.InfoFormat("Post(" + newItem + ") operation");
            if (newItem == null)
            {
                Logger.Error("Error when adding a item. It is null");
                throw new HttpResponseException(HttpStatusCode.Conflict);
            }
            else
            {
                Repository.Add<Item>(newItem);
                var response = Request.CreateResponse<Item>(HttpStatusCode.Created, newItem);

                string uri = Url.Link("DefaultApi", new { id = newItem.Id });
                response.Headers.Location = new Uri(uri);
                return response;
            }
        }

        
        /// <summary>
        /// PUT api/itemsCRUD/5 - Update
        /// Puts an item.
        /// </summary>
        /// <param name="id">The identifier of the item to update</param>
        /// <param name="itemToUpdate">The item to update.</param>
        /// <exception cref="System.Web.Http.HttpResponseException"></exception>
        [ValidateHttpAntiForgeryToken]
        public void PutItem(int id, Item itemToUpdate)
        {
            Logger.InfoFormat("Update(" + id.ToString() + ") operation");

            itemToUpdate.Id = id;
            try
            {
                Repository.Update<Item>(itemToUpdate);
            }
            catch (Exception ex)
            {
                Logger.Error("Error when updating the item " + id.ToString(), ex);
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
        }

        
        /// <summary>
        /// DELETE api/itemsCRUD/5
        /// Deletes the item.
        /// </summary>
        /// <param name="id">The identifier of the item to delete</param>
        [ValidateHttpAntiForgeryToken]
        public void DeleteItem(int id)
        {
            Logger.InfoFormat("Delete(" + id.ToString() + ") operation");            
            try
            {
                Repository.Delete<Item>(id);
            }
            catch (Exception ex)
            {
                Logger.Error("Error when deleting the item " + id.ToString(), ex);
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }            
        }
    }
}
