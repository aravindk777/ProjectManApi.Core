using PM.Models.DataModels;

namespace PM.Data.Repos.Tasks
{
    public interface ITaskRepository : IRepository<Task>
    {
        bool EndTask(int taskId);
    }
}
