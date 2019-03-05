using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PM.BL.Users
{
    public interface IUserLogic
    {
        PM.Models.ViewModels.User AddUser(PM.Models.ViewModels.User user);
        IEnumerable<PM.Models.ViewModels.User> GetUsers(bool active = false);
        bool EditUser(string UserId, PM.Models.ViewModels.User userViewModel);
        bool DeleteUser(string UserId);
        PM.Models.ViewModels.User GetUserById(string UserId);
        IEnumerable<PM.Models.ViewModels.User> Search(string keyword, bool exactMatch = false, string fieldType = "");
    }
}
