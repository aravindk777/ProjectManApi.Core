using PM.Data.Entities;
using PM.Models.DataModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace PM.Data.Repository
{
    public class TasksRepository : GenericRepository<Tasks>, ITasksRepository
    {
        public TasksRepository(PMDbContext context) : base(context)
        {
        }
    }
}
