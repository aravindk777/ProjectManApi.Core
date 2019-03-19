﻿using PM.BL.Common;
using PM.Data.Repos.Projects;
using PM.Models.ViewModels;
using System.Collections.Generic;

namespace PM.BL.Projects
{
    public class ProjectLogic : IProjectLogic, ICommonLogic
    {
        private readonly IProjectRepo _projectRepo;

        public ProjectLogic(IProjectRepo projectRepo)
        {
            _projectRepo = projectRepo;
        }

        public int Count()
        {
            return _projectRepo.Count();
        }

        public Project CreateProject(Project project)
        {
            return _projectRepo.Create(project.AsDataModel()).AsViewModel();
        }

        public IEnumerable<Project> GetAllProjects()
        {
            return _projectRepo.GetAll().AsViewModel();
        }

        public Project GetProject(int projId)
        {
            return _projectRepo.GetById(projId).AsViewModel();
        }

        public IEnumerable<Project> GetUserProjects(string userId)
        {
            var result = _projectRepo.Search(p => p.Manager.UserId == userId).AsViewModel();
            return result;
        }

        public bool Modify(int projId, Project projectViewModel)
        {
            if (_projectRepo.GetById(projId) != null)
                return _projectRepo.Update(projectViewModel.AsDataModel());
            else
                return false;
        }

        public bool Remove(int projId)
        {
            var projectToDelete = _projectRepo.GetById(projId);
            if (projectToDelete != null)
                return _projectRepo.Delete(projectToDelete);
            else
                return false;
        }
    }
}
