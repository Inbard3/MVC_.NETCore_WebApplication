﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace RateTheRest.Models
{
    //A Review of a restaurant submittied by a user 
    public class Review
    {
        public int ReviewID { get; set; }

        [Range(0,10,ErrorMessage = "Please insert a number between 0 to 10")]
        [Display(Name = "Restaurant Score")]
        public int Score { get; set; }            //An Individual rating score by this user 1-10

        [Display(Name = "Short Description")]
        public string? Text { get; set; }           //Optional description of the review

        public DateTime DateCreated { get; set; }

        //Linked fields from db

        public Restaurant Restaurant { get; set; }               //One(Restaurant)-to-Many(Reviews)

        public User User { get; set; }                          //One(User)-to-Many(Reviews)
    }
}
