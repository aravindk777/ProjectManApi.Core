using System.Collections.Generic;

namespace PM.BL.Tasks
{
    public interface ITaskLogic
    {
        Models.ViewModels.Task CreateTask(Models.ViewModels.Task task);
        IEnumerable<Models.ViewModels.Task> GetTasks();
        bool UpdateTask(int taskId, Models.ViewModels.Task taskModel);
        bool DeleteTask(int taskId);
        Models.ViewModels.Task GetTask(int taskId);
        IEnumerable<Models.ViewModels.Task> GetAllTasksForProject(int projectId);
        IEnumerable<Models.ViewModels.Task> GetAllTasksForUser(string userId);
        IEnumerable<Models.ViewModels.Task> GetUserProjectTasks(string userId, int projId);
        bool EndTask(int taskId);
    }
}
