using PM.Models.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PM.Data.Repos.Users
{
    public interface IUserRepository : IRepository<Models.DataModels.User>
    {
        bool DeleteUser(string userId);
    }
}
