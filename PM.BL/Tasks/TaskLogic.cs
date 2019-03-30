using PM.BL.Common;
using PM.Data.Repos.Tasks;
using PM.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PM.BL.Tasks
{
    public class TaskLogic : ITaskLogic
    {
        public readonly ITaskRepository taskRepository;

        public TaskLogic(ITaskRepository _repository)
        {
            taskRepository = _repository;
        }

        #region Task Repo operations

        public Task CreateTask(Task task)
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

        public Task GetTask(int taskId)
        {
            var result = taskRepository.GetById(taskId).AsViewModel();
            return result;
        }

        public IEnumerable<Task> GetTasks(bool activeOnly = false)
        {
            var tasks = taskRepository.GetAll().AsViewModel();
            if (activeOnly)
                return tasks.Where(t => t.IsActive);
            return tasks;
        }

        public bool UpdateTask(int taskId, Task taskModel)
        {
            var taskEntity = taskRepository.GetById(taskId);
            if (taskEntity != null)
                return taskRepository.Update(taskModel.AsDataModel(taskEntity));
            else
                return false;
        }

        public bool EndTask(int taskId)
        {
            return taskRepository.EndTask(taskId);
        }

        public int Count()
        {
            return taskRepository.Count();
        }
        #endregion

        #region Search
        public IEnumerable<Task> Search(string keyword, bool exactMatch = false, string fieldType = "")
        {
            var resultSet = taskRepository.Search(t => exactMatch ? t.TaskName.ToLower().Equals(keyword.ToLower()) : t.TaskName.ToLower().Contains(keyword.ToLower()))
                            .Union(taskRepository.Search(t => exactMatch ? t.ParentTask.TaskName.ToLower().Equals(keyword.ToLower()) : t.ParentTask.TaskName.ToLower().Contains(keyword.ToLower())))
                            .Union(taskRepository.Search(t => exactMatch ? t.Project.ProjectName.ToLower().Equals(keyword.ToLower()) : t.Project.ProjectName.ToLower().Contains(keyword.ToLower())))
                            .Union(taskRepository.Search(t => exactMatch ? t.TaskOwner.FirstName.ToLower().Equals(keyword.ToLower()) : t.TaskOwner.FirstName.ToLower().Contains(keyword.ToLower())))
                            .Union(taskRepository.Search(t => exactMatch ? t.TaskOwner.LastName.ToLower().Equals(keyword.ToLower()) : t.TaskOwner.LastName.ToLower().Contains(keyword.ToLower())))
                            .Union(taskRepository.Search(t => exactMatch ? t.TaskOwner.UserId.ToLower().Equals(keyword.ToLower()) : t.TaskOwner.UserId.ToLower().Contains(keyword.ToLower())))
                            .AsViewModel();
            return resultSet;
        }
        #endregion

        #region Get User and Project related Tasks
        public IEnumerable<Task> GetAllTasksForProject(int projectId)
        {
            return taskRepository.Search(t => t.ProjectId == projectId).AsViewModel();
        }

        public IEnumerable<Task> GetAllTasksForUser(string userId)
        {
            return taskRepository.Search(t => t.TaskOwner.UserId == userId).AsViewModel();
        }

        public IEnumerable<Task> GetUserProjectTasks(string userId, int projId)
        {
            return taskRepository.Search(t => t.TaskOwner.UserId == userId && t.ProjectId == projId).AsViewModel();
        }
        #endregion
    }
}
