using PM.BL.Common;
using PM.Data.Repos.Users;
using PM.Models.ViewModels;
using System.Collections.Generic;
using System.Linq;

namespace PM.BL.Users
{
    public class UserLogic : IUserLogic, ICommonLogic
    {
        private readonly IUserRepository userRepository;

        public UserLogic(IUserRepository _userRepository)
        {
            userRepository = _userRepository;
        }

        public User AddUser(User user)
        {
            return userRepository.Create(user.AsDataModel(true)).AsViewModel();
        }

        public int Count()
        {
            return userRepository.Count();
        }

        public bool DeleteUser(string UserId)
        {
            return userRepository.DeleteUser(UserId);
        }

        public bool EditUser(string UserId, User userViewModel)
        {
            if (userRepository.GetById(UserId) != null)
                return userRepository.Update(userViewModel.AsDataModel());
            else
                return false;
        }

        public User GetUserById(string UserId)
        {
            return userRepository.GetById(UserId).AsViewModel();
        }

        public IEnumerable<User> GetUsers(bool active = false)
        {
            var results = userRepository.GetAll();
            if (active)
                results = results.Where(data => (!data.EndDate.HasValue || data.EndDate.Value.Equals(System.DateTime.MinValue)));
            return results.AsViewModel();
        }

        public IEnumerable<User> Search(string keyword, bool exactMatch = false, string fieldType = "")
        {
            if (!string.IsNullOrEmpty(fieldType))
            { 
                switch (fieldType.ToLower().Trim())
                {
                    case "firstname":
                        return userRepository.Search(u => exactMatch ? u.FirstName.ToLower().Equals(keyword.ToLower()) : u.FirstName.ToLower().Contains(keyword.ToLower())).AsViewModel();

                    case "lastname":
                        return userRepository.Search(u => exactMatch ? u.LastName.ToLower().Equals(keyword.ToLower()) : u.LastName.ToLower().Contains(keyword.ToLower())).AsViewModel();

                    case "userid":
                        return userRepository.Search(u => exactMatch ? u.UserId.ToLower().Equals(keyword.ToLower()) : u.UserId.ToLower().Contains(keyword.ToLower())).AsViewModel();
                }
            }
            var resultSet = userRepository.Search(u => exactMatch ? u.FirstName.ToLower().Equals(keyword.ToLower()) : u.FirstName.ToLower().Contains(keyword.ToLower()))
                            .Union(userRepository.Search(u => exactMatch ? u.LastName.ToLower().Equals(keyword.ToLower()) : u.LastName.ToLower().Contains(keyword.ToLower())))
                            .Union(userRepository.Search(u => exactMatch ? u.UserId.ToLower().Equals(keyword.ToLower()) : u.UserId.ToLower().Contains(keyword.ToLower())))
                            .AsViewModel();
            return resultSet;
                
        }
    }
}
