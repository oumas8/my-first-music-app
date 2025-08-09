using Domain.Interfaces;
using Domain.Models.Enteties;
using System;
using System.Collections.Generic;
using System.Text;

namespace infrastructure.Repositories
{
     public class ApiCloudRepository : GenericRepository<ApiCloud>, IApiCloudRepository
    {
        public ApiCloudRepository(DbContextClass dbContext) : base(dbContext)
        {

        }
    }
}
