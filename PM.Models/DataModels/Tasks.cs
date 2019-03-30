using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PM.Models.DataModels
{
    [Table("Task")]
    public class Task
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TaskId { get; set; }

        [Required]
        [MaxLength(50, ErrorMessage = "Task Name is required")]
        public string TaskName { get; set; }

        [Required]
        [Range(1, 30, ErrorMessage = "Invalid Priority value")]
        public int Priority { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public int ProjectId { get; set; }
        public Project Project { get; set; }

        //public bool IsParent { get; set; }
        public virtual Task ParentTask { get; set; }
        public int? ParentTaskId { get; set; }

        public Guid? TaskOwnerId { get; set; }
        public User TaskOwner { get; set; }
    }
}
