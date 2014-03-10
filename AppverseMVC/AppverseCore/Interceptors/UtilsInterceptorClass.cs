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

using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Appverse.Core.Interceptors
{
    /// <summary>
    /// Utils Interceptor Class
    /// </summary>
    public class UtilsInterceptorClass
    {
        /// <summary>
        /// Alternative. Not used: Gets the method signature and the corresponding parameters values.
        /// Does not return the values of complex class types.
        /// </summary>
        /// <param name="invocation">The invocation.</param>
        /// <returns></returns>
        internal static string GetMethodSignature2(IInvocation invocation)
        {
            var methodName = invocation.Method.ToString();
            var arguments = invocation.Arguments.Select(a => (a == null ? "null" : "[" + a.GetType().Name + "]" + a.ToString())).ToArray();
            var argsString = string.Join(",", arguments);
            var cacheKey = methodName + "(" + argsString + ")";

            return cacheKey;
        }

        /// <summary>
        /// Gets the method signature and the corresponding parameters values.
        /// Does not return the values of complex class types.
        /// </summary>
        /// <param name="invocation">The invocation.</param>
        /// <returns>The string signature</returns>
        internal string GetMethodSignature(IInvocation invocation)
        {
            String parameters = invocation.Method.Name;

            if (invocation.Arguments == null)
            {
                return parameters + "()";
            }

            List<String> arguments = new List<string>();
            try
            {
                foreach (var item in invocation.Arguments)
                {
                    if (item != null)
                    {
                        Type objtype = item.GetType();

                        if (objtype == typeof(String) || objtype.IsPrimitive || !objtype.IsClass)
                        {
                            arguments.Add("[" + objtype.Name + "]" + item.ToString());
                        }
                        else
                        {
                            // This could be extended to support "complex" types serializing
                            arguments.Add("[" + objtype.Name + "]" + item.ToString());
                        }
                    }
                    else
                        arguments.Add("null");
                }
                parameters = invocation.Method.Name + "(" + string.Join(",", arguments) + ")";
            }

            catch (Exception)
            {
                //Logger.DebugFormat("{0}.{1} Input parameters: Error parsing input parameters: {2} ", invocation.TargetType.FullName, invocation.Method.Name, ex.Message);
            }
            return parameters;
        }
    }

    //public static class Serialize
    //{
    //    public static string SerializeObject<T>(this T toSerialize, string defaultValue)
    //    {
    //        try
    //        {
    //            XmlSerializer xmlSerializer = new XmlSerializer(toSerialize.GetType());
    //            StringWriter textWriter = new StringWriter();

    //            xmlSerializer.Serialize(textWriter, toSerialize);
    //            return textWriter.ToString();
    //        }
    //        catch (Exception)
    //        {
    //            return defaultValue;
    //        }
    //    }
    //}

}
