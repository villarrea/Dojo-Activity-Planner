using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace BeltExam.Models
{
    public class Actvty
    {
        [Key]
        public int ActvtyId {get;set;}

        [Required]
        [MinLength(2, ErrorMessage="Title must be 2 characters or longer!")]
        public string Title {get;set;}


        [Required]
        [MinLength(10, ErrorMessage="Description must be 2 characters or longer!")]
        public string Description {get;set;}

        [Required]
        [FutureDateTime]
        [DataType(DataType.DateTime)]
        public DateTime ActivityDate {get;set;}

        [Required]
        [Display(Name = "Duration")]
        public int Duration {get;set;}

        [Required]
        public string ActUnit {get;set;}

        public DateTime CreatedAt {get;set;} = DateTime.Now;
        public DateTime UpdatedAt {get;set;} = DateTime.Now;


        public int PlannerId {get;set;} //UserId
        public List<Participation> ActivityAttendees {get;set;} //The list of users attending
    }

    public class FutureDateTimeAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if(((DateTime)value) <= DateTime.Now)
            {
                return new ValidationResult("Only dates/times in the future are allowed");
            }
            return ValidationResult.Success;
        }
    }
}