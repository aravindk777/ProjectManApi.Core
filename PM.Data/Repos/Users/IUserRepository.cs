using PM.Models.DataModels;

namespace PM.Data.Repos.Users
{
    public interface IUserRepository : IRepository<User>
    {
        bool DeleteUser(string userId);
    }
}
