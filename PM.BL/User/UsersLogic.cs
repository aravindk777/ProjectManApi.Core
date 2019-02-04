using PM.Data.Repository;
using PM.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace PM.BL.User
{
    public class UsersLogic
    {
        private readonly IUserRepository userRepository;

        public UsersLogic(IUserRepository _userRepository)
        {
            userRepository = _userRepository;
        }

        public void CreateUser(Users userModel)
        {
            var userDataModel = new PM.Models.DataModels.Users();
            userRepository.Create(userDataModel);
        }
    }
}
