using Moq;
using PM.BL.Users;
using PM.Data.Repos.Users;
using System;
using System.Collections.Generic;
using System.Text;

namespace PM.UnitTests.Logic
{
    public class UserLogicUnitTests
    {
        private Mock<IUserRepository> mockUserRepo;
        private IUserLogic userLogicTest;

        public UserLogicUnitTests()
        {
            mockUserRepo = new Mock<IUserRepository>();
            userLogicTest = new UserLogic(mockUserRepo.Object);
        }
    }
}
