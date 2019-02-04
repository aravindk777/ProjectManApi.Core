using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PM.Models.DataModels
{
    [Table("Tasks")]
    public class Tasks
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TaskId { get; set; }

        [Required]
        public string TaskName { get; set; }

        [Required]
        [Range(1, 30, ErrorMessage = "Invalid Priority value")]
        public int Priority { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public int? ProjectId { get; set; }
        public virtual Projects Project { get; set; }

        public bool IsParent { get; set; }
        public virtual Tasks ParentTask { get; set; }
        public int? ParentTaskId { get; set; }

        public Guid? TaskOwnerId { get; set; }
        public virtual Users TaskOwner { get; set; }
    }
}
