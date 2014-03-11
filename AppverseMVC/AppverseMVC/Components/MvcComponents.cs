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
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Routing;

namespace Appverse.Web.Components
{
    public static class MvcComponents
    {
        public static MvcHtmlString EditorWithClassFor<TModel, TValue>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TValue>> expression, string className)
        {
            string htmlText = System.Web.Mvc.Html.EditorExtensions.EditorFor<TModel, TValue>(htmlHelper, expression, className).ToHtmlString();            

            string quotation = "\"";
            string classAtrribute = "class=" + quotation ;
            

            int start = htmlText.IndexOf(classAtrribute);
            if (start > 0)
            {
                int end = htmlText.IndexOf(quotation, start + classAtrribute.Length);
                string classToReplace = htmlText.Substring(start, end - start + quotation.Length);
                htmlText = htmlText.Replace(classToReplace, classAtrribute + className + quotation);
            }

            return new MvcHtmlString(htmlText);
        }

    //    public static MvcHtmlString TextBoxFor<TModel, TProp>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProp>> expression, params Action<MvcInputBuilder>[] propertySetters)
    //    {
    //        MvcInputBuilder builder = new MvcInputBuilder();

    //        foreach (var propertySetter in propertySetters)
    //        {
    //            propertySetters.Invoke(builder);
    //        }

    //        var properties = new RouteValueDictionary(builder)
    //            .Select(kvp => kvp)
    //            .Where(kvp => kvp.Value != null)
    //            .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

    //        return htmlHelper.TextBoxFor(expression, properties);
    //    }

        #region Render a Custom WebGrid
        /// <summary>
        /// Webs the grid list.
        /// </summary>
        /// <typeparam name="T">Generic Class</typeparam>
        /// <param name="helper">The helper.</param>
        /// <param name="list">The list. List of Items to render with the WebGrid</param>
        /// <param name="titleColumns">The title columns. array of table titles</param>
        /// <param name="dbColumns">The database columns. array of fields</param>
        /// <param name="pager">The pager. Pager count</param>
        /// <param name="ajaxContainer">The ajax container. Container to render de WebGrid</param>
        /// <param name="controler">The controler. Controler to reference the actions</param>
        /// <returns>The MvcHtmlString to render in the WebPage</returns>
        public static MvcHtmlString WebGridList<T>(this HtmlHelper helper,
                                                   List<T> list = null,
                                                   List<string> titleColumns = null,
                                                   List<string> dbColumns = null,
                                                   int pager = 10,
                                                   string ajaxContainer = null,
                                                   string controler = null) where T : class
        {
            List<WebGridColumn> columns = new List<WebGridColumn>();
            var grid = new WebGrid(list, canSort: true, canPage: true, rowsPerPage: pager, ajaxUpdateContainerId: ajaxContainer);
            for (int i = 0; i < titleColumns.Count; i++)
            {
                columns.Add(new WebGridColumn
                {
                    ColumnName = dbColumns[i],
                    Header = titleColumns[i] + SortDirection(helper, ref grid, dbColumns[i]).ToString(),
                    CanSort = true
                });
            }

            //columns.ForEach(c => c.Header = c.Header + " " + SortDirection(ref grid, c.ColumnName).ToString());
            columns.Add(new WebGridColumn()
            {
                Format = (item) =>
                {
                    return new HtmlString(string.Format("<a href='/{1}/Edit/{0}'>{2}</a>&nbsp;|&nbsp;<a href='/{1}/Details/{0}'>{3}</a>&nbsp;|&nbsp;<a href='/{1}/Delete/{0}'>{4}</a>",
                                                        item.Id, controler, "Edit", "Details", "Delete"));
                }
            });

            return new MvcHtmlString(grid.GetHtml(columns: columns,
                                                  mode: WebGridPagerModes.All,
                                                  tableStyle: "table",
                                                  firstText: "<<",
                                                  lastText: ">>")
                                                  .ToHtmlString());
        }

        /// <summary>
        /// Sorts the direction.
        /// </summary>
        /// <param name="grid">The grid. WebGrid reference</param>
        /// <param name="columnName">Name of the column. Column selected to sort</param>
        /// <returns></returns>
        public static MvcHtmlString SortDirection(this HtmlHelper helper, ref WebGrid grid, String columnName)
        {
            String html = String.Empty;

            if (String.IsNullOrEmpty(columnName))
                return MvcHtmlString.Empty;

            if (grid.SortColumn == columnName &&
                grid.SortDirection == System.Web.Helpers.SortDirection.Ascending)
                html = "▲";
            else if (grid.SortColumn == columnName &&
                     grid.SortDirection == System.Web.Helpers.SortDirection.Descending)
                html = "▼";

            return MvcHtmlString.Create(html);
        }
        #endregion
    }
}
