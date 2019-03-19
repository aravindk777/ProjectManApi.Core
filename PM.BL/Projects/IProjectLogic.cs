using System.Collections.Generic;

namespace PM.BL.Projects
{
    public interface IProjectLogic
    {
        Models.ViewModels.Project CreateProject(Models.ViewModels.Project project);
        IEnumerable<Models.ViewModels.Project> GetAllProjects();
        bool Modify(int projId, Models.ViewModels.Project projectViewModel);
        bool Remove(int projId);
        Models.ViewModels.Project GetProject(int projId);
        IEnumerable<Models.ViewModels.Project> GetUserProjects(string userId);
        bool EndProject(int projId);
    }
}
