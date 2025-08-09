using Domain.Interfaces;
using Domain.Models.Enteties;
using System;
using System.Collections.Generic;
using System.Text;

namespace infrastructure.Repositories
{
     public class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(DbContextClass dbContext) : base(dbContext)
        {

        }
    }
}
