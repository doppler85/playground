﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Playground.Model
{
    public enum Gender
    {
        Male,
        Female
    }

    public class User
    {
        public int UserID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Gender Gender { get; set; }
        public string EmailAddress { get; set; }
        public string ExternalUserID { get; set; }
        public string PictureUrl { get; set; }

        public virtual List<Player> PlayerProfiles { get; set; }
        public virtual List<Notification> Notification { get; set; }
        public virtual List<PlaygroundUser> Playgrounds { get; set; }
    }
}
