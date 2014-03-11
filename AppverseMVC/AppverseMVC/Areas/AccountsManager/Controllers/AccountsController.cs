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
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using Appverse.Web.Models;
using Appverse.Web.Core.Controllers;

namespace Appverse.Web.Areas.AccountsManager.Controllers
{
    /// <summary>
    /// AccountsController class. Operations and views to add, edit and delete accounts and their respective roles.
    /// </summary>
    [Authorize(Roles = "Admin")]
    public class AccountsController : AppverseBaseController
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AccountsController"/> class.
        /// </summary>
        public AccountsController()
            : this(new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext())))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AccountsController"/> class.
        /// </summary>
        /// <param name="userManager">The user manager.</param>
        public AccountsController(UserManager<ApplicationUser> userManager)
        {
            UserManager = userManager;
        }

        /// <summary>
        /// Returns a view model to show all accounts.
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            var Db = new ApplicationDbContext();
            var users = Db.Users;
            var model = new List<EditUserViewModel>();
            foreach (var user in users)
            {
                var u = new EditUserViewModel(user);
                model.Add(u);
            }
            return View(model);
        }

        /// <summary>
        /// View to edit an account.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="Message">The message.</param>
        /// <returns></returns>
        public ActionResult Edit(string id, ManageMessageId? Message = null)
        {
            if (!string.IsNullOrEmpty(id))
            {
                var Db = new ApplicationDbContext();
                var account = Db.Users.First(u => u.UserName == id);
                var model = new EditUserViewModel(account);
                ViewBag.MessageId = Message;
                return View(model);
            }
            else
            {
                return RedirectToAction("Index");
            }
        }
        /// <summary>
        /// Edits and saves an existing account.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(EditUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var Db = new ApplicationDbContext();
                var account = Db.Users.First(u => u.UserName == model.UserName);
                // Update the user data:
                account.FirstName = model.FirstName;
                account.LastName = model.LastName;
                account.Email = model.Email;
                Db.Entry(account).State = System.Data.Entity.EntityState.Modified;
                await Db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            // If we got this far, something failed, redisplay form
            return View(model);
        }

        public UserManager<ApplicationUser> UserManager { get; private set; }


        [Authorize(Roles = "Admin")]
        public ActionResult Register()
        {
            return View();
        }


        /// <summary>
        /// Registers a new account.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var account = model.GetUser();
                var result = await UserManager.CreateAsync(account, model.Password);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Accounts");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }




        /// <summary>
        /// Releases unmanaged resources and optionally releases managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && UserManager != null)
            {
                UserManager.Dispose();
                UserManager = null;
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// Confirmation view to delete an account.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public ActionResult Delete(string id = null)
        {
            var Db = new ApplicationDbContext();
            var account = Db.Users.First(u => u.UserName == id);
            var model = new EditUserViewModel(account);
            if (account == null)
            {
                return HttpNotFound();
            }
            return View(model);
        }


        /// <summary>
        /// Deletes an account.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            var Db = new ApplicationDbContext();
            var account = Db.Users.First(u => u.UserName == id);
            Db.Users.Remove(account);
            Db.SaveChanges();
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Returns a view showing the roles for a selected user.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public ActionResult UserRoles(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                var Db = new ApplicationDbContext();
                var account = Db.Users.First(u => u.UserName == id);
                var model = new SelectUserRolesViewModel(account);
                return View(model);
            }
            else
            {
                return RedirectToAction("Index");
            }
        }


        /// <summary>
        /// Users the roles.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UserRoles(SelectUserRolesViewModel model)
        {
            if (ModelState.IsValid)
            {
                var idManager = new IdentityManager();
                var Db = new ApplicationDbContext();
                var account = Db.Users.First(u => u.UserName == model.UserName);
                idManager.ClearUserRoles(account.Id);
                foreach (var role in model.Roles)
                {
                    if (role.Selected)
                    {
                        idManager.AddUserToRole(account.Id, role.RoleName);
                    }
                }
                return RedirectToAction("index");
            }
            return View();
        }

        #region Helpers

        /// <summary>
        /// Gets the authentication manager.
        /// </summary>
        /// <value>
        /// The authentication manager.
        /// </value>
        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        /// <summary>
        /// Signs in asynchronous.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="isPersistent">if set to <c>true</c> [is persistent].</param>
        /// <returns></returns>
        private async Task SignInAsync(ApplicationUser user, bool isPersistent)
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
            var identity = await UserManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
            AuthenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = isPersistent }, identity);
        }

        /// <summary>
        /// Adds the errors.
        /// </summary>
        /// <param name="result">The result.</param>
        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        /// <summary>
        /// Determines whether this instance has password.
        /// </summary>
        /// <returns></returns>
        private bool HasPassword()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (user != null)
            {
                return user.PasswordHash != null;
            }
            return false;
        }

        public enum ManageMessageId
        {
            ChangePasswordSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
            Error
        }

        /// <summary>
        /// Redirects to local.
        /// </summary>
        /// <param name="returnUrl">The return URL.</param>
        /// <returns></returns>
        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        #endregion
    }
}


