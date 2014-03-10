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
using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Appverse.Core.Interceptors
{
    /// <summary>
    /// Log Interceptor class
    /// </summary>
    public class LogInterceptor : UtilsInterceptorClass,IInterceptor
    {
        /// <summary>
        /// Gets or sets the logger. It is required
        /// </summary>
        /// <value>
        /// The logger.
        /// </value>
        public ILogger Logger { get; set; }
        public bool LogInputParameters { get; set; }
        public bool LogResult { get; set; }
        public bool LogProfiling { get; set; }
        public bool LogExceptions { get; set; }



        //private static string DumpObject(object argument)
        //{
        //    Type objtype = argument.GetType();
        //    if (objtype == typeof(String) || objtype.IsPrimitive || !objtype.IsClass)
        //        return objtype.ToString();
        //    else
        //        return objtype.ToString();

        //    //return DataContractSerialize(argument, objtype);
        //}


        //public static String CreateInvocationLogString(IInvocation invocation)
        //{
        //    StringBuilder sb = new StringBuilder(100);            
        //    foreach (object argument in invocation.Arguments)
        //    {
        //        String argumentDescription = argument == null ? "null" : DumpObject(argument);
        //        sb.Append(argumentDescription).Append(",");
        //    }
        //    if (invocation.Arguments.Count() > 0) sb.Length--;
        //    sb.Append(")");
        //    return sb.ToString();
        //}
        

        /// <summary>
        /// Interceptor only works for virtual methods or methos include in an inteface 
        /// http://docs.castleproject.org/Tools.Kinds-of-proxy-objects.ashx
        /// </summary>
        /// <param name="invocation"></param>
        public void Intercept(IInvocation invocation)
        {
            DateTime startTime = DateTime.Now;

            if (LogInputParameters)
            {
                Logger.DebugFormat("{0}.{1}", invocation.TargetType.FullName, GetMethodSignature(invocation));
            }
            try
            {
                invocation.Proceed();
            }
            catch (Exception ex)
            {
                if (LogExceptions)
                {
                    string message = ex.Message;
                    if (ex.InnerException != null)
                        message = message + " " + ex.InnerException.Message;

                    Logger.ErrorFormat(ex, "{0}.{1} {2} ", invocation.TargetType.FullName, invocation.Method.Name, message);
                }
                throw;
            }
            if (LogProfiling) Logger.DebugFormat("{0}.{1} Execution time: {2}", invocation.TargetType.FullName, invocation.Method.Name, DateTime.Now.Subtract(startTime));
            if (LogResult)
            {
                if (invocation.ReturnValue != null)
                    Logger.DebugFormat("{0}.{1} Return result: {2}", invocation.TargetType.FullName, invocation.Method.Name, invocation.ReturnValue.ToString());
                else
                    Logger.DebugFormat("{0}.{1} No return result for this method", invocation.TargetType.FullName, invocation.Method.Name);
            }
        }
    }
}
