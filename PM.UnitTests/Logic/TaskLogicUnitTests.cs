using Moq;
using PM.BL.Tasks;
using PM.Data.Repos.Tasks;
using System;
using System.Collections.Generic;
using System.Text;

namespace PM.UnitTests.Logic
{
    public class TaskLogicUnitTests
    {
        private Mock<ITaskRepository> mockTaskRepo;
        private ITaskLogic tasksLogicTest;

        public TaskLogicUnitTests()
        {
            mockTaskRepo = new Mock<ITaskRepository>();
            tasksLogicTest = new TaskLogic(mockTaskRepo.Object);
        }
    }
}
