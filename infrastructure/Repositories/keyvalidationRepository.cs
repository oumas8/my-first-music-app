using Domain.Interfaces;
using Domain.Models.Enteties;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace infrastructure.Repositories
{
     public class keyvalidationRepository : GenericRepository<keyvalidation>, IkeyvalidationRepository
    {
        public keyvalidationRepository(DbContextClass dbContext) : base(dbContext)
        {

        }
        public void DeleteKeyById(long id)
        {
            _dbContext.Database.ExecuteSqlInterpolated($@"
        DELETE FROM keyvalidation
        WHERE id = {id}
    ");
        }
    }
}
