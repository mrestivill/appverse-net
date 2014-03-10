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

using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections.Specialized;
using System.Configuration;

namespace Appverse.Web.Core.Installers
{
    /// <summary>
    /// Performs the installation of the CultureHelper class. This installer is a sample. It should be created a new one having the cultures and the cookie name required for the application
    /// </summary>
    public class CultureHelperInstaller : IWindsorInstaller
    {
        /// <summary>
        /// Performs the installation in the <see cref="T:Castle.Windsor.IWindsorContainer" />.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="store">The configuration store.</param>
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            IList<string> cultures; 
            // = new List<string> {
            //    "en-US",  // first culture is the DEFAULT
            //    "es", // Spanish NEUTRAL culture
            //    "es-AR", "es-BO", "es-CL", "es-CO", "es-CR", "es-DO", "es-EC", "es-SV", "es-GT", "es-HN", "es-MX", "es-NI", "es-PA", "es-PY", "es-PE", "es-PR", "es-ES", "es-US", "es-UY", "es-VE", // Specific cultures
            //    "it", // Italian NEUTRAL culture
            //    "it-CH", // Italian specific culture
            //    "ca" // Catalan NEUTRAL culture           
            //};

            string culture = ConfigurationManager.AppSettings["Cultures"];
            if (!string.IsNullOrEmpty(culture))
            {
                cultures = culture.Split(',').Select(p => p.Trim()).ToList();
            }
            else
            {
                cultures = "en".Split(',');
            }

            container.Register(Classes.FromThisAssembly()
                                .Where(Component.IsInSameNamespaceAs<CultureHelper>())
                                .WithService.DefaultInterfaces()
                                .LifestyleSingleton()
                                .Configure(c => c.DependsOn(Dependency.OnValue("cultures", cultures), Dependency.OnValue("cultureCookie", "_culture"))));
        }
    }
}