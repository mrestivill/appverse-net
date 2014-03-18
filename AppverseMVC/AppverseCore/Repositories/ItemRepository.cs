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
using System.Web;

namespace Appverse.Core.Repositories
{
    /// <summary>
    /// ItemRepository class: this is a generic repository class. It contains a server side pager and all CRUD operations
    /// </summary>
    // With this line we indicate that all methods of this class will be intercepted by this interceptor class. We also can do it using a configiration file or in the installer class.
    //[Interceptor(typeof(Appverse.Core.Interceptors.ExceptionInterceptor))]
    public class ItemRepository: IItemRepository
    {
        private readonly ISession _session;

        /// <summary>
        /// Initializes a new instance of the <see cref="ItemRepository"/> class.
        /// </summary>
        /// <param name="pageSize">Size of the page.</param>
        /// <param name="session">The session.</param>
        public ItemRepository(ISession session)
        {
            this._session = session;
        }



        /// <summary>
        /// Gets the session. Only use if you need some functionality not included by this class
        /// </summary>
        /// <value>
        /// The session.
        /// </value>
        public ISession session
        {
            get { return _session; }
        }


        /// <summary>
        /// HQL Select 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="hqlQuery">The query.</param>
        /// <returns></returns>
        public IList<T> Get<T>(string hqlQuery, List<Parameter> parameters) where T : class
        {
            //query = "SELECT TOP 10000 o.* "+ " from ORDERS o where o.Year in (:orderYear));";

            //var queryResult = _session.CreateQuery("FROM Item WHERE Id = :id").List<T>();


            IQuery query = _session.CreateQuery(hqlQuery);
            foreach (Parameter param in parameters)
	        {
                query.SetParameter(param.Name, param.Value);
	        }

            var result = query.List<T>();

            return result;
        }



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
        public Page<T> GetPage<T>(int pageNumber, int pageSize, DetachedCriteria criteria) where T : class
        {
            if (criteria == null)
                criteria = DetachedCriteria.For<T>();

            int totalCount = Count<T>(criteria);

            var firstResult = pageSize * (pageNumber - 1);

            if (pageNumber > 0 && pageSize > 0)
            {
                criteria.SetFirstResult(firstResult);
                criteria.SetMaxResults(pageSize);
            }
            else
            {
                pageNumber = 0;
                pageSize = 0;
            }

            IEnumerable<T> items = criteria.GetExecutableCriteria(_session).List<T>();
            Page<T> page = new Page<T>(items, pageNumber, totalCount, pageSize);
            return page;
        }

        /// <summary>
        /// Server side paginator. If pagenumber or pageSize are 0, it returns all items
        /// </summary>
        /// <typeparam name="T">Generic Class</typeparam>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <returns>
        /// List if Items
        /// </returns>
        public Page<T> GetPage<T>(int pageNumber, int pageSize) where T : class
        {
            return GetPage<T>(pageNumber, pageSize, DetachedCriteria.For<T>());
        }

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
        public Page<T> GetPage<T>(int pageNumber, int pageSize, string sortColumn = null, bool sortDirection = false) where T : class
        {
            DetachedCriteria criteria = DetachedCriteria.For<T>();

            criteria = DetachedCriteria.For<T>();

            if (sortColumn != null)
            {
                if (sortDirection)
                    criteria.AddOrder(Order.Asc(sortColumn));
                else
                    criteria.AddOrder(Order.Desc(sortColumn));
            }

            return GetPage<T>(pageNumber, pageSize, criteria);            
        }


        /// <summary>
        /// Gets a list filtered by a custom criteria.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The filtered list</returns>
        public int Count<T>() where T : class
        {
            return _session.QueryOver<T>().ToRowCountQuery().FutureValue<int>().Value;
        }

        /// <summary>
        /// Gets a list filtered by a custom criteria.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The filtered list</returns>
        public int Count<T>(DetachedCriteria criteria) where T : class
        {
            if (criteria == null)
                criteria = DetachedCriteria.For<T>();

            var countQuery = CriteriaTransformer.TransformToRowCount(criteria);
            return  countQuery.GetExecutableCriteria(_session).FutureValue<int>().Value;
        }



        /// <summary>
        /// Gets a list filtered using the like operator.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="value">The value.</param>
        /// <returns>The filtered list</returns>
        public IList<T> Get<T>(string propertyName, string value) where T : class
        {
            // Alternative
            //return _session.CreateCriteria<T>().Add(Restrictions.Like(propertyName, "%" + value + "%")).List<T>();

            DetachedCriteria criteria = DetachedCriteria.For<T>();
            criteria.Add(Expression.Like(propertyName, value, MatchMode.Anywhere));

            return Get<T>(criteria);

        }

        /// <summary>
        /// Gets a list filtered by a custom criteria.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The filtered list</returns>
        public IList<T> Get<T>(DetachedCriteria criteria) where T : class
        {
            if (criteria == null)
                criteria = DetachedCriteria.For<T>();

            IList<T> items = criteria.GetExecutableCriteria(_session).List<T>();

            return items;
        }

        /// <summary>
        /// Gets an specified item.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public T Get<T>(int id)
        {
            return _session.Get<T>(id);
        }



        /// <summary>
        /// Adds a new item.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="newItem">The new item.</param>
        /// <returns>The new item with the Id field</returns>
        public T Add<T>(T newItem) where T : class
        {
            using (var transaction = _session.BeginTransaction())
            {
                try
                {
                    _session.Save(newItem);
                    transaction.Commit();
                }
                catch (Exception)
                {
                    //http://startbigthinksmall.wordpress.com/2009/05/04/the-transaction-has-aborted-tricky-net-transactionscope-behavior/
                    transaction.Dispose();
                    throw;
                }
            }
            return newItem;
        }


        /// <summary>
        /// Updates the specified item. The item must contain an "id", if not, an exception will be generated
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="itemToUpdate">The item to update.</param>
        public void Update<T>(T itemToUpdate)
        {
            using (var transaction = _session.BeginTransaction())
            {
                try
                {
                    _session.Update(itemToUpdate);

                    //int b = 0;
                    //int A = 0 / b;
                    transaction.Commit();
                }
                catch (Exception)
                {
                    //http://startbigthinksmall.wordpress.com/2009/05/04/the-transaction-has-aborted-tricky-net-transactionscope-behavior/
                    transaction.Dispose();
                    throw;
                }
            }
            
        }

        /// <summary>
        /// Deletes an item.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id">The identifier.</param>
        public void Delete<T>(int id)
        {
            using (var transaction = _session.BeginTransaction())
            {
                try
                {
                    _session.Delete(_session.Load<T>(id));
                    transaction.Commit();
                }
                catch (Exception)
                {
                    //http://startbigthinksmall.wordpress.com/2009/05/04/the-transaction-has-aborted-tricky-net-transactionscope-behavior/
                    transaction.Dispose();
                    throw;
                }
            }
        }
    }
}