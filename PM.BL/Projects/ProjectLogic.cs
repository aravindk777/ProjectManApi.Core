using PM.BL.Common;
using PM.Data.Repos.Projects;
using PM.Models.ViewModels;
using System.Collections.Generic;
using System.Linq;

namespace PM.BL.Projects
{
    public class ProjectLogic : IProjectLogic
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

        public bool EndProject(int projId)
        {
            var projectToEnd = _projectRepo.GetById(projId);
            if (projectToEnd != null)
            {
                projectToEnd.ProjectEnd = System.DateTime.Today;
                return _projectRepo.Update(projectToEnd);
            }
            else
                return false;
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

        public IEnumerable<Project> Search(string keyword, bool exactMatch = false, string fieldType = "")
        {
            var resultSet = _projectRepo.Search(p => exactMatch ? p.ProjectName.ToLower().Equals(keyword.ToLower()) : p.ProjectName.ToLower().Contains(keyword.ToLower()))
                            .Union(_projectRepo.Search(p => exactMatch ? p.Manager.FirstName.ToLower().Equals(keyword.ToLower()) : p.Manager.FirstName.ToLower().Contains(keyword.ToLower())))
                            .Union(_projectRepo.Search(p => exactMatch ? p.Manager.LastName.ToLower().Equals(keyword.ToLower()) : p.Manager.LastName.ToLower().Contains(keyword.ToLower())))
                            .Union(_projectRepo.Search(p => exactMatch ? p.Manager.UserId.ToLower().Equals(keyword.ToLower()) : p.Manager.UserId.ToLower().Contains(keyword.ToLower())))
                            .AsViewModel();
            return resultSet;
        }
    }
}
