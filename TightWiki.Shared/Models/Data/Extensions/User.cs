﻿using System;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace TightWiki.Shared.Models.Data
{
    public partial class User
    {
        public int Id { get; set; }
        [Display(Name = "Email Address")]
        public string EmailAddress { get; set; }
        [Display(Name = "Account Name")]
        public string AccountName { get; set; }
        public string Navigation { get; set; }
        public string PasswordHash { get; set; }
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        [Display(Name = "Timezone")]
        public string TimeZone { get; set; }
        public string Country { get; set; }
        public string Language { get; set; }
        [Display(Name = "Biography")]
        public string AboutMe { get; set; }
        public byte[] Avatar { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public DateTime LastLoginDate { get; set; }
        public string VerificationCode { get; set; }
        public bool EmailVerified { get; set; }
        public int PaginationSize { get; set; }
        public int PaginationCount { get; set; }
        public string Role { get; set; }
    }
}

