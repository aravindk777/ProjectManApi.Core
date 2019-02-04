using System;
using System.Collections.Generic;
using System.Text;

namespace PM.Models.ViewModels
{
    public class Projects
    {
        public string ProjectName { get; set; }
        public int Priority { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public Guid Manager { get; set; }
        public string ManagerName { get; set; }
    }
}
