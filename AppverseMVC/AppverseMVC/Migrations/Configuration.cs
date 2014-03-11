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

namespace Appverse.Web.Migrations
{
    using Appverse.Web.Models;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<Appverse.Web.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            ContextKey = "Appverse.Web.Models.ApplicationDbContext";

            // We force the calling of this method to be sure we have correct roles and default users
            //this.AddUserAndRoles();
        }

        protected override void Seed(Appverse.Web.Models.ApplicationDbContext context)
        {
            //  This method will be called after migrating to the latest version.
            this.AddUserAndRoles();
        }

        bool AddUserAndRoles()
        {
            bool success = false;
  
            var idManager = new IdentityManager();
            success = idManager.CreateRole("Admin");
            if (!success == true) return success;
  
            success = idManager.CreateRole("User");
            if (!success) return success;
  
  
            var newUser = new ApplicationUser()
            {
                UserName = "admin",
                FirstName = "administrator",
                LastName = "showcase",
                Email = "admin@appverse.com"
            };
  
            // Be careful here - you  will need to use a password which will 
            // be valid under the password rules for the application, 
            // or the process will abort:

            //Admin user
            success = idManager.CreateUser(newUser, "admin123");
            if (!success) return success;
  
            success = idManager.AddUserToRole(newUser.Id, "Admin");
            if (!success) return success;
  
            success = idManager.AddUserToRole(newUser.Id, "User");
            if (!success) return success;

            newUser = new ApplicationUser()
            {
                UserName = "user",
                FirstName = "user",
                LastName = "test",
                Email = "user@appverse.com"
            };

            //Regular user
            success = idManager.CreateUser(newUser, "user123");
            if (!success) return success;

            success = idManager.AddUserToRole(newUser.Id, "User");
            if (!success) return success;
  
            return success;
        }
    }
}
