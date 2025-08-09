using Domain.Models.Enteties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IkeyvalidationRepository : IGenericRepository<keyvalidation>
    {
        public void DeleteKeyById(long id);
    }
}
