using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace BeltExam.Models
{
    public class Participation
    {
        [Key]
        public int ParticipationId {get;set;}
        public int UserId {get;set;}
        public int ActvtyId {get;set;}

        public User User {get;set;}
        public Actvty Activity {get;set;}
        public DateTime CreadtedAt {get;set;} = DateTime.Now;
        public DateTime UpdatedAt {get;set;} = DateTime.Now;
    }
}