using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PM.Models.DataModels
{
    [Table("Projects")]
    public class Project
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ProjectId { get; set; }

        [Required]
        [MaxLength(50, ErrorMessage = "Project Name is required")]
        public string ProjectName { get; set; }

        [Range(1, 30)]
        public int Priority { get; set; }

        public DateTime? ProjectStart { get; set; }
        public DateTime? ProjectEnd { get; set; }

        public Guid? ManagerId { get; set; }
        public User Manager { get; set; }
        public ICollection<Task> Tasks { get; set; }
    }
}
