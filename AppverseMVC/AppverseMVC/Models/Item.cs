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
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

/// <summary>
/// Model
/// With the attributes (annotations in java) we can specify ranges, display text, error messages, formats, ... and they will be used in the presentation layer authomatically
/// </summary>
namespace Appverse.Web.Models
{
	public class Item : EntityBase
	{
        [Display(Name = "Description", ResourceType = typeof(Localization.Model.Item))]
		public virtual string Description { get; set; }

        //public virtual UserInfo GeneratedBy { get; set; }

        [Display(Name = "Title", ResourceType = typeof(Localization.Model.Item))]
        [Required(ErrorMessageResourceName = "FieldRequired", ErrorMessageResourceType = typeof(Localization.Validations.Validator))]
        [StringLength(20, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(Localization.Validations.Validator), MinimumLength = 4)]
        public virtual string Title { get; set; }


        //public virtual bool IsDeleted { get; set; }

        //[DisplayFormat(DataFormatString = "{0:" + System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern + "}", ApplyFormatInEditMode = true)]
        //[Range(typeof(DateTime), "1", "9999", ErrorMessage = "{0} must be a decimal/number between {1} and {2}.")]
        //[CustomValidation
        //[System.ComponentModel.DataAnnotations.EnumDataType(DataType.Date, 
        //[DataType(DataType.DateTime, ErrorMessage = "InvalidData message")]
        //[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date, ErrorMessageResourceName = "FieldMustBeDate", ErrorMessageResourceType = typeof(Localization.Validations.Validator))]
        [Required(ErrorMessageResourceName = "FieldRequired", ErrorMessageResourceType = typeof(Localization.Validations.Validator))]
        //[DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:" + Constants.Formats.HTML5DateFormat + "}", ApplyFormatInEditMode = true)]
        [Display(Name = "When", ResourceType = typeof(Localization.Model.Item))]        
        public virtual DateTime Moment { get; set; }

        //[Required(ErrorMessageResourceName = "FieldRequired", ErrorMessageResourceType = typeof(Localization.Validations.Validator))]
        //[Display(Name = "Where", ResourceType = typeof(Localization.Model.Item))]        
        //public virtual string Location { get; set; }


        [Required(ErrorMessageResourceName = "FieldRequired", ErrorMessageResourceType = typeof(Localization.Validations.Validator))]
        [Display(Name = "Value", ResourceType = typeof(Localization.Model.Item))]
        [Range(-100.00, 100.00, ErrorMessageResourceName = "RangeValidation", ErrorMessageResourceType = typeof(Localization.Validations.Validator))]
        public virtual decimal Value { get; set; }

        /// <summary>
        /// Gets or sets the location.
        /// </summary>
        /// <value>
        /// The location.
        /// </value>
        [Required(ErrorMessageResourceName = "FieldRequired", ErrorMessageResourceType = typeof(Localization.Validations.Validator))]
        [Display(Name = "Where", ResourceType = typeof(Localization.Model.Item))]
        public virtual Location Location { get; set; }
	}
}