using PM.Data.Entities;
using PM.Models.DataModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PM.Data.Repository
{
    public class UsersRepository : GenericRepository<User>, IUserRepository
    {
        //private readonly PMDbContext _usersDbContext;
        public UsersRepository(PMDbContext context) : base(dbContext: context)
        {
           // _usersDbContext = context;
        }
    }
}
