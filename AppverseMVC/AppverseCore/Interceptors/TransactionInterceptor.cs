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
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Transactions;
using System.Data;
using Castle.Core.Logging;

namespace Appverse.Core.Interceptors
{
    public class TransactionInterceptor : Castle.DynamicProxy.IInterceptor
    {
        public ILogger Logger { get; set; }
        public bool WriteToLog { get; set; }

        public void Intercept(IInvocation invocation)
        {
            using (TransactionScope transaction = new TransactionScope())
            {
                try
                {
                    if (WriteToLog) Logger.DebugFormat("Transaction Begin: {0}.{1}", invocation.TargetType.FullName, invocation.Method.Name);
                    invocation.Proceed();
                    transaction.Complete();
                    if (WriteToLog) Logger.DebugFormat("Transaction Complete: {0}.{1}", invocation.TargetType.FullName, invocation.Method.Name);
                }
                catch (Exception)
                {
                    // Remember to start the service called "Distributed Transaction Coordinator"
                    //http://social.msdn.microsoft.com/Forums/en-US/3749da7a-59df-4db7-ae2e-6d0e414750d4/msdtc-on-server-xxx-is-unavailable

                    // If transaction is not disposed, there will be an error on tot transaction
                    // http://startbigthinksmall.wordpress.com/2009/05/04/the-transaction-has-aborted-tricky-net-transactionscope-behavior/                    
                    transaction.Dispose();

                    // The Exception is logged in the LogInterceptor                    
                    throw;
                }
            }
        }
    }
}
