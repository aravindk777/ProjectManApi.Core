using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PM.Models.DataModels
{
    [Table("Project")]
    public class Project
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ProjectId { get; set; }

        [Required]
        public string ProjectName { get; set; }

        [Range(1, 30)]
        public int Priority { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public Guid? ManagerId { get; set; }
        public virtual User Manager { get; set; }
    }
}
