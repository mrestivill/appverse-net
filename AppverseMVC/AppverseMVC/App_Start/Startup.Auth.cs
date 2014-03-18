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

using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Google;
using Owin;
using System;
using System.Configuration;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Appverse.Web
{
    public partial class Startup
    {
        // For more information on configuring authentication, please visit http://go.microsoft.com/fwlink/?LinkId=301864
        public void ConfigureAuth(IAppBuilder app)
        {
            // Enable the application to use a cookie to store information for the signed in user
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/LoginAlert"),

                // When you are using Local Account, OWIN creates this cookie (AspNet.ApplicationCookie). By default the cookie is set to expire when browser session ends or by default to 14 days from log in if the user select the "Remember Me" option.
                // If the cookie is present in the browser and not expired then the user is "logged in" automatically without authenticating the user in the business layer
                // With the default configuration: 14 days and sliding expiration, a user that choose the "remember me" and use the site at least once each 14 days will stay always logged in.
                ExpireTimeSpan = new TimeSpan(2, 0, 0, 0),
                SlidingExpiration = false              
            });
            // Use a cookie to temporarily store information about a user logging in with a third party login provider
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);



            // http://www.theroks.com/social-login-owin-authentication-mvc5/

            // Microsoft : Create application
            // https://account.live.com/developers/applications
            if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings.Get("MicrosoftClientId")))
            {
                app.UseMicrosoftAccountAuthentication(new Microsoft.Owin.Security.MicrosoftAccount.MicrosoftAccountAuthenticationOptions
                {
                    ClientId = ConfigurationManager.AppSettings.Get("MicrosoftClientId"),
                    ClientSecret = ConfigurationManager.AppSettings.Get("MicrosoftClientSecret"),
                    CallbackPath = new PathString("/signin-microsoft"),
                    Provider = new Microsoft.Owin.Security.MicrosoftAccount.MicrosoftAccountAuthenticationProvider()
                    {
                        OnAuthenticated = (context) =>
                        {
                            context.Identity.AddClaim(new System.Security.Claims.Claim("urn:microsoftaccount:access_token", context.AccessToken, XmlSchemaString, "Microsoft"));

                            return Task.FromResult(0);
                        }
                    }
                });
            }


            // Google : Create application
            // https://console.developers.google.com/project
            if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings.Get("GoogleClientId")))
            {
                app.UseGoogleAuthentication(new GoogleOAuth2AuthenticationOptions
                {                    
                    ClientId = ConfigurationManager.AppSettings.Get("GoogleClientId"),
                    ClientSecret = ConfigurationManager.AppSettings.Get("GoogleClientSecret"),
                    CallbackPath = new PathString("/signin-google"),
                    Provider = new GoogleOAuth2AuthenticationProvider
                    {
                        OnAuthenticated = context =>
                        {
                            context.Identity.AddClaim(
                                new Claim(
                                    ClaimTypes.Email,
                                    context.Email));
                            return Task.FromResult<object>(null);
                        }
                    }
                });
            }


            // Uncomment the following lines to enable logging in with third party login providers
            //app.UseTwitterAuthentication(
            //   consumerKey: "",
            //   consumerSecret: "");

            //app.UseFacebookAuthentication(
            //   appId: "",
            //   appSecret: "");
        }

        const string XmlSchemaString = "http://www.w3.org/2001/XMLSchema#string";
    }
}