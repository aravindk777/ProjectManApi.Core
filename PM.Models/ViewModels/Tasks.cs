using System;
using System.Collections.Generic;
using System.Text;

namespace PM.Models.ViewModels
{
    public class Tasks
    {
        public int TaskId { get; set; }
        public string TaskName { get; set; }
        public int Priority { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsParent { get; set; }
        public string ProjectName { get; set; }
        public string ParentTask { get; set; }
        public int? ParentTaskId { get; set; }
        public Guid TaskOwner { get; set; }
        public string OwnerFullName { get; set; }
    }
}
