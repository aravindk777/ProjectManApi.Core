using PM.Data.Entities;
using PM.Models.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PM.Data.Repos.Tasks
{
    public class TaskRepository : Repository<Task>, ITaskRepository
    {
        public TaskRepository(PMDbContext context) : base(context) { }

        public bool EndTask(int taskId)
        {
            var taskToEnd = GetById(taskId);
            taskToEnd.EndDate = DateTime.Now;
            return Update(taskToEnd);
        }
    }
}
