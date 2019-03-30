using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PM.Models.ViewModels
{
    public class User
    {
        public Guid Id { get; set; }
        [Required]
        public string FirstName { get; set; }
        //[Required]
        public string LastName { get; set; }
        public string FullName { get { return string.Format($"{LastName}, {FirstName}"); } }
        [Required]
        public string UserId { get; set; }
        public bool Active { get; private set; }
    }
}
