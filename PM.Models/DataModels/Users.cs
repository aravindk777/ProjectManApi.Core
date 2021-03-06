﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PM.Models.DataModels
{
    [Table("Users")]
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string UserId { get; set; }
        [DefaultValue("getdate()")]
        public DateTime Created
        {
            get { return _createdValue == DateTime.MinValue ? DateTime.Now : _createdValue; }
            set { _createdValue = value; }
        }
        private DateTime _createdValue;

        public DateTime? EndDate { get; set; }
        public ICollection<Project> Projects { get; set; }
        public ICollection<Task> Tasks { get; set; }
    }
}
