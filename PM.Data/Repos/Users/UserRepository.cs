using PM.Data.Entities;
using PM.Models.DataModels;
using System.Linq;

namespace PM.Data.Repos.Users
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(PMDbContext dbContext) : base(dbContext) { }

        public bool DeleteUser(string userId)
        {
            var userToSoftDelete = GetById(userId);
            if (userToSoftDelete == null) return false;
            userToSoftDelete.EndDate = System.DateTime.Now;
            return Update(userToSoftDelete);
        }

        public override User GetById(object identifier)
        {
            return Search(user => user.UserId.ToLower().Equals(identifier.ToString().ToLower())).FirstOrDefault();
        }
    }
}
