using PM.Data.Entities;
using System;
using System.Linq;

namespace PM.Api.IntegrationTest.Environment
{
    internal static class SeedData
    {
        public static void CreateTestData(PMDbContext context)
        {
            var testUserDataaModelList = Enumerable.Range(1, 10).Select(
                counter => new Models.DataModels.User
                {
                    UserId = $"Test-User{counter}",
                    FirstName = $"User-{counter}FirstName",
                    LastName = $"User-{counter}LastName",
                    Id = Guid.NewGuid(),
                    Created = DateTime.Today.AddDays(30 - counter),
                    EndDate = counter % 2 == 0 ? DateTime.Today : DateTime.Today.AddDays(2 * counter)
                });
            context.Users.AddRange(testUserDataaModelList);

            var testProjectDataModelList = Enumerable.Range(1, 10).Select(
                counter => new Models.DataModels.Project
                {
                    ProjectId = counter,
                    ProjectName = $"Test-Project-{counter}",
                    Priority = counter,
                    //Manager = testUserDataaModelList.FirstOrDefault(m => m.UserId.Contains($"User{counter}")),
                    ManagerId = testUserDataaModelList.FirstOrDefault(m => m.UserId.Contains($"User{counter}")).Id,
                    ProjectStart = DateTime.Today.AddDays(30 - counter),
                    ProjectEnd = counter % 2 == 0 ? DateTime.Today : DateTime.Today.AddDays(2 * counter)
                });
            context.Projects.AddRange(testProjectDataModelList);

            var testTaskDataModelList = Enumerable.Range(1, 10).Select(
                counter => new Models.DataModels.Task
                {
                    TaskId = counter,
                    TaskName = $"TestTask{counter}",
                    ProjectId = counter,
                    Priority = counter,
                    //TaskOwner = testUserDataaModelList.FirstOrDefault(m => m.UserId.Contains($"User{counter}")),
                    TaskOwnerId = testUserDataaModelList.FirstOrDefault(m => m.UserId.Contains($"User{counter}")).Id,
                    StartDate = DateTime.Today.AddDays(30 - counter),
                    EndDate = counter % 2 == 0 ? DateTime.Today : DateTime.Today.AddDays(2 * counter),
                    Project = testProjectDataModelList.FirstOrDefault(p => p.ProjectId == counter),
                    //ParentTaskId = counter % 3 == 0 ? counter : null,
                });
            context.Tasks.AddRange(testTaskDataModelList);

            context.SaveChanges();
        }
    }
}
