using PM.Data.Entities;
using PM.Models.DataModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace PM.Data.Repository
{
    public class ProjectsRepository : GenericRepository<Projects>, IProjectsRepository
    {
        public ProjectsRepository(PMDbContext context) : base(context)
        { }
    }
}
