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

using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Appverse.Web.Models
{
    public class ExternalLoginConfirmationViewModel
    {
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }
    }

    public class ManageUserViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        // New Fields added to extend Application User class:
        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required]
        public string Email { get; set; }


        // Return a pre-poulated instance of AppliationUser:
        public ApplicationUser GetUser()
        {
            var user = new ApplicationUser()
            {
                FirstName = this.FirstName,
                LastName = this.LastName,
                Email = this.Email,
            };
            return user;
        }
    }

    public class LoginViewModel
    {
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }

    public class RegisterViewModel
    {
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }


        // New Fields added to extend Application User class:
        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required]
        public string Email { get; set; }


        // Return a pre-poulated instance of AppliationUser:
        public ApplicationUser GetUser()
        {
            var user = new ApplicationUser()
            {
                UserName = this.UserName,
                FirstName = this.FirstName,
                LastName = this.LastName,
                Email = this.Email,
            };
            return user;
        }
    }

    public class EditUserViewModel
    {

        public EditUserViewModel() { }



        // Allow Initialization with an instance of ApplicationUser:

        public EditUserViewModel(ApplicationUser user)
        {

            this.UserName = user.UserName;

            this.FirstName = user.FirstName;

            this.LastName = user.LastName;

            this.Email = user.Email;

        }



        [Required]

        [Display(Name = "User Name")]

        public string UserName { get; set; }



        [Required]

        [Display(Name = "First Name")]

        public string FirstName { get; set; }



        [Required]

        [Display(Name = "Last Name")]

        public string LastName { get; set; }



        [Required]

        public string Email { get; set; }

    }





    public class SelectUserRolesViewModel
    {

        public SelectUserRolesViewModel()
        {

            this.Roles = new List<SelectRoleEditorViewModel>();

        }





        // Enable initialization with an instance of ApplicationUser:

        public SelectUserRolesViewModel(ApplicationUser user)
            : this()
        {

            this.UserName = user.UserName;

            this.FirstName = user.FirstName;

            this.LastName = user.LastName;



            var Db = new ApplicationDbContext();



            // Add all available roles to the list of EditorViewModels:

            var allRoles = Db.Roles;

            foreach (var role in allRoles)
            {

                // An EditorViewModel will be used by Editor Template:

                var rvm = new SelectRoleEditorViewModel(role);

                this.Roles.Add(rvm);

            }



            // Set the Selected property to true for those roles for 

            // which the current user is a member:

            foreach (var userRole in user.Roles)
            {

                var checkUserRole =

                    this.Roles.Find(r => r.RoleName == userRole.Role.Name);

                checkUserRole.Selected = true;

            }

        }



        public string UserName { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public List<SelectRoleEditorViewModel> Roles { get; set; }

    }



    // Used to display a single role with a checkbox, within a list structure:
    public class SelectRoleEditorViewModel
    {

        public SelectRoleEditorViewModel() { }

        public SelectRoleEditorViewModel(IdentityRole role)
        {
            this.RoleName = role.Name;
        }
        

        public bool Selected { get; set; }


        [Required]
        public string RoleName { get; set; }
    }

}
