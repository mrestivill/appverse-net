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

using NHibernate;
using NHibernate.Criterion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Appverse.Core.Repositories
{
    /// <summary>
    /// Generic repository interface class. It contains a server side pager and all CRUD operations
    /// </summary>
    public interface IItemRepository
    {
        /// <summary>
        /// Server side paginator. If pagenumber or pageSize are 0, it returns all items
        /// </summary>
        /// <typeparam name="T">Generic Class</typeparam>
        /// <param name="pageNumber">The page number</param>
        /// <param name="pageSize">The size of the page.</param>
        /// <param name="sortColumn">The criteria or field to sort the list.</param>
        /// <param name="sortDirection">if set to <c>true</c> ASC, otherwise False.</param>
        /// <returns>
        /// List if Items
        /// </returns>
        Page<T> GetPage<T>(int pageNumber, int pageSize, string sortColumn = null, bool sortDirection = false) where T : class;


        /// <summary>
        /// Server side paginator. If pagenumber or pageSize are 0, it returns all items
        /// </summary>
        /// <typeparam name="T">Generic Class</typeparam>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <param name="criteria">The criteria.</param>
        /// <returns>
        /// List if Items
        /// </returns>
        Page<T> GetPage<T>(int pageNumber, int pageSize, DetachedCriteria criteria) where T : class;


        /// <summary>
        /// Server side paginator. If pagenumber or pageSize are 0, it returns all items
        /// </summary>
        /// <typeparam name="T">Generic Class</typeparam>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <returns>
        /// List if Items
        /// </returns>
        Page<T> GetPage<T>(int pageNumber, int pageSize) where T : class;


        /// <summary>
        /// Gets a list filtered using the like operator.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="value">The value.</param>
        /// <returns>The filtered list</returns>        
        IList<T> Get<T>(string propertyName, string value) where T : class;

        /// <summary>
        /// Gets a list filtered by a custom criteria.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The filtered list</returns>
        IList<T> Get<T>(DetachedCriteria criteria) where T : class;


        /// <summary>
        /// HQL Select 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="hqlQuery">The query.</param>
        /// <returns></returns>
        IList<T> Get<T>(string hqlQuery, List<Parameter> parameters) where T : class;


        /// <summary>
        /// Gets an specified item.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        T Get<T>(int id);

        /// <summary>
        /// Adds a new item.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="newItem">The new item.</param>
        /// <returns>The new item with the Id field</returns>
        T Add<T>(T newItem) where T : class;


        /// <summary>
        /// Updates the specified item. The item must contain an "id", if not, an exception will be generated
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="itemToUpdate">The item to update.</param>
        void Update<T>(T itemToUpdate);

        /// <summary>
        /// Deletes an item.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id">The identifier.</param>
        void Delete<T>(int id);


        /// <summary>
        /// Gets a list filtered by a custom criteria.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The filtered list</returns>
        int Count<T>() where T : class;

        /// <summary>
        /// Gets a list filtered by a custom criteria.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The filtered list</returns>
        int Count<T>(DetachedCriteria criteria) where T : class;

        /// <summary>
        /// Gets the session. Only use if you need some functionality not included by this class
        /// </summary>
        /// <value>
        /// The session.
        /// </value>        
        ISession session
        {
            get;
        }
    }

    public class Parameter
    {
        public string Name { get; set; }
        public object Value { get; set; }


        public Parameter()
        {

        }

        public Parameter(string name, object value)
        {
            Name = name;
            Value = value;
        }

    }
}
