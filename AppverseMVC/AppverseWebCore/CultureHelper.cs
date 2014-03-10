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
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Appverse.Web.Core
{
    /// <summary>
    /// Culture Helper class. It contains a list
    /// </summary>
    public class CultureHelper
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CultureHelper"/> class.
        /// </summary>
        /// <param name="cultures">The cultures.</param>
        /// <param name="cultureCookie">The culture cookie.</param>
        public CultureHelper(IList<string> cultures, string cultureCookie)
        {
            Cultures = cultures;
            if (!string.IsNullOrEmpty(cultureCookie))
                CultureCookieName = cultureCookie;
        }


        /// <summary>
        /// Gets or sets the accepted cultures.
        /// </summary>
        /// <value>
        /// The cultures. First culture is the DEFAULT
        /// </value>
        public IList<string> Cultures { get; set; }


        /// <summary>
        /// The culture cookie
        /// </summary>
        public string CultureCookieName = "_culture";


        /// <summary>
        /// Checks if culture name is valid
        /// </summary>
        /// <param name="cultureCode">The culture code.</param>
        /// <param name="culture">Returns the culture name proposed</param>
        /// <returns>true if the cultureCode is valid</returns>
        private static bool TryGetCultureInfo(string cultureCode, out string culture)
        {
            try
            {
                culture = CultureInfo.GetCultureInfo(cultureCode).Name;
                return true;
            }
            catch (CultureNotFoundException)
            {
                string neutralCulture = GetNeutralCulture(cultureCode);
                if (neutralCulture != cultureCode)
                    return TryGetCultureInfo(neutralCulture, out culture);
            }

            culture = CultureInfo.CurrentCulture.Name;
            return false;
        }


        /// <summary>
        /// Gets the implemented culture.
        /// </summary>
        /// <param name="requestUserLanguages">List of culture's name (e.g. en-US)</param>
        /// <returns>
        /// Returns a valid culture name based on "name" parameter. If "name" is not valid, it returns the default culture "en-US"
        /// </returns>
        public string GetImplementedCulture(string[] requestUserLanguages)
        {
            return GetImplementedCulture(requestUserLanguages, 0);
        }

        /// <summary>
        /// Gets the implemented culture.
        /// </summary>
        /// <param name="requestUserLanguages">List of culture's name (e.g. en-US)</param>
        /// <param name="index">This variable indicates the item in the list that will be checked.</param>
        /// <returns>
        /// Returns a valid culture name based on "name" parameter. If "name" is not valid, it returns the default culture "en-US"
        /// </returns>
        private string GetImplementedCulture(string[] requestUserLanguages, int index)
        {
            // make sure it's not null
            if (requestUserLanguages== null || index>=requestUserLanguages.Length || string.IsNullOrEmpty(requestUserLanguages[index]))
            {
                if (requestUserLanguages!= null && requestUserLanguages.Length > index)
                    return GetImplementedCulture(requestUserLanguages, index + 1);
                else
                    return GetDefaultCulture(); // return Default culture
            }

            string currentCulture;
            // make sure it is a valid culture first
            if (!TryGetCultureInfo(requestUserLanguages[index], out currentCulture))
            {
                if (requestUserLanguages.Length>index)
                    return GetImplementedCulture(requestUserLanguages, index+1);
                else
                    return GetDefaultCulture(); // return Default culture if it is invalid
            }


            // if it is implemented, accept it
            if (Cultures.Where(c => c.Equals(currentCulture, StringComparison.InvariantCultureIgnoreCase)).Count() > 0)
                return currentCulture; // accept it

            
            // Find a close match. For example, if you have "en-US" defined and the user requests "en-GB", 
            // the function will return closes match that is "en-US" because at least the language is the same (ie English)  
            var n = GetNeutralCulture(currentCulture);
            foreach (var c in Cultures)
                if (c.StartsWith(n))
                    return c;

            // If passed culture is not implemented we will process this condition, if we have more preferred cultures, we will try with next culture
            if (requestUserLanguages.Length>index)
                return GetImplementedCulture(requestUserLanguages, index+1);
            else
                return GetDefaultCulture(); // return Default culture as no match found
        }


        /// <summary>
        /// Gets the default culture.
        /// </summary>
        /// <returns>
        /// Returns default culture name which is the first name decalared (e.g. en-US)
        /// </returns>
        public string GetDefaultCulture()
        {
            return Cultures[0]; // return Default culture
        }

        /// <summary>
        /// Gets the current culture.
        /// </summary>
        /// <returns>returns the current culture name</returns>
        public static string GetCurrentCulture()
        {
            return Thread.CurrentThread.CurrentCulture.Name;
        }

        /// <summary>
        /// Gets the current neutral culture.
        /// </summary>
        /// <returns>if current culture is "en-US", it will return "en"</returns>
        public static string GetCurrentNeutralCulture()
        {
            return GetNeutralCulture(Thread.CurrentThread.CurrentCulture.Name);
        }


        /// <summary>
        /// Gets the neutral culture.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>if the input is "en-US", it will return "en"</returns>
        public static string GetNeutralCulture(string name)
        {
            if (name.Length < 2)
                return name;

            return name.Substring(0, 2); // Read first two chars only. E.g. "en", "es"
        }
    }
}
