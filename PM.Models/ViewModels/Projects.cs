using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PM.Models.ViewModels
{
    public class Project
    {
        public int ProjectId { get; set; }
        [Required]
        public string ProjectName { get; set; }
        public int Priority { get; set; }
        public DateTime? ProjectStart { get; set; }
        public DateTime? ProjectEnd { get; set; }
        public bool IsActive { get; set; }
        [Required]
        public Guid ManagerId { get; set; }
        public string ManagerName { get; set; }
    }
}
