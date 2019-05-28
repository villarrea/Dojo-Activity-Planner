using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BeltExam.Models
{
    public class User
    {
        [Key]

        public int UserId {get;set;}

        [Required]
        [Display(Name = "First Name")]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "First Name must only contain letters!")]
        public string FirstName {get;set;}
        

        [Required]
        [Display(Name = "Last Name")]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Last Name must only contain letters!")]
        public string LastName {get;set;}


        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email {get;set;}


        [Required]
        [MinLength(8, ErrorMessage="Password must be 8 characters or longer!")]
        [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)(?=.*[@$!%*#?&])[A-Za-z\d@$!%*#?&]{8,}$", ErrorMessage = "Password must contain at least 1 letter, 1 number, and 1 special character!")]
        [DataType(DataType.Password)]
        public string Password {get;set;}


        [NotMapped]
        [Required]
        [Compare("Password")]
        [DataType(DataType.Password)]
        [Display(Name = "Password Confirmation")]
        public string ConfirmPassword {get;set;}

        public DateTime CreatedAt {get;set;} = DateTime.Now;
        public DateTime UpdatedAt {get;set;} = DateTime.Now;
        public List<Participation> AttendedActivities {get;set;} //The List of Activities that a User is attending 
    }
}