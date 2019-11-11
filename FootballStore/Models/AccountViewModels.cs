using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FootballStore.Models
{
    public class RegisterViewModel
    {
        [Required]
        [Display(Name = "User Name")]
        public string UserName { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 3)]
        [DataType(DataType.Password, ErrorMessage = "The {0} is not a password")]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password, ErrorMessage = "The {0} is not a password")]
        [Compare("Password", ErrorMessage = "The password and confirm password is not match.")]
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; }

        [Display(Name = "Date of birth")]
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }
    }

    public class LoginViewModel
    {
        [Required]
        [Display(Name = "User Name")]
        public string UserName { get; set; }

        [Display(Name = "Password")]
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }

    public class ResetPasswordViewModel
    {
        [Required]
        [Display(Name = "User Name")]
        public string UserName { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 3)]
        [DataType(DataType.Password, ErrorMessage = "The {0} is not a password")]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password, ErrorMessage = "The {0} is not a password")]
        [Compare("Password", ErrorMessage = "The password and confirm password is not match.")]
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }

    public class ForgotPasswordViewModel
    {
        [Required]
        [Display(Name = "User Name")]
        public string UserName { get; set; }
    }
}