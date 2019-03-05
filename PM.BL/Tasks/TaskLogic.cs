using PM.BL.Common;
using PM.Data.Repos.Tasks;
using System;
using System.Collections.Generic;

namespace PM.BL.Tasks
{
    public class TaskLogic : ITaskLogic, ICommonLogic
    {
        public readonly ITaskRepository taskRepository;

        public TaskLogic(ITaskRepository _repository)
        {
            taskRepository = _repository;
        }

        #region EF REST operations

        public Models.ViewModels.Task CreateTask(Models.ViewModels.Task task)
        {
            return taskRepository.Create(task.AsDataModel()).AsViewModel();
        }

        public bool DeleteTask(int taskId)
        {
            var taskToDelete = taskRepository.GetById(taskId);
            if (taskToDelete != null)
                return taskRepository.Delete(taskToDelete);
            else
                return false;
        }

        public Models.ViewModels.Task GetTask(int taskId)
        {
            var result = taskRepository.GetById(taskId).AsViewModel();
            return result;
        }

        public IEnumerable<Models.ViewModels.Task> GetTasks()
        {
            return taskRepository.GetAll().AsViewModel();
        }

        public bool UpdateTask(int taskId, Models.ViewModels.Task taskModel)
        {
            if (taskRepository.GetById(taskId) != null && taskModel.TaskId == taskId)
                return taskRepository.Update(taskModel.AsDataModel());
            else
                return false;
        }

        #endregion

        #region Search Repo methods
        public IEnumerable<Models.ViewModels.Task> GetAllTasksForProject(int projectId)
        {
            return taskRepository.Search(t => t.ProjectId == projectId).AsViewModel();
        }

        public IEnumerable<Models.ViewModels.Task> GetAllTasksForUser(string userId)
        {
            return taskRepository.Search(t => t.TaskOwner.UserId == userId).AsViewModel();
        }

        public IEnumerable<Models.ViewModels.Task> GetUserProjectTasks(string userId, int projId)
        {
            return taskRepository.Search(t => t.TaskOwner.UserId == userId && t.ProjectId == projId).AsViewModel();
        }

        public bool EndTask(int taskId)
        {
            return taskRepository.EndTask(taskId);
        }
        #endregion

        public int Count()
        {
            return taskRepository.Count();
        }
    }
}
