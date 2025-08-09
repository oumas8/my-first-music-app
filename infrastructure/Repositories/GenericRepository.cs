using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace infrastructure.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly DbContextClass _dbContext;

        protected GenericRepository(DbContextClass context)
        {
            _dbContext = context;
        }
        public async Task<T> GetById(int id, bool tracking = false)
        {
            try
            {
                var query = _dbContext.Set<T>().Where(e => EF.Property<long>(e, "id") == id);

                if (!tracking)
                {
                    query = query.AsNoTracking();
                }

                var entity = await query.FirstOrDefaultAsync();

                if (entity == null)
                {
                    Console.WriteLine($"Entity with ID {id} not found.");
                    return null; 
                }

                return entity;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetById: {ex.Message}");
                throw; 
            }
        }

        public async Task<ICollection<T>> GetAll()
        {
            return await _dbContext.Set<T>().AsNoTracking().ToListAsync();
        }

        public async Task Add(T entity)
        {
             _dbContext.Set<T>().Add(entity);
            await _dbContext.SaveChangesAsync();
        }

        public void Delete(T entity)
        {
            _dbContext.Set<T>().Remove(entity);
        }

        public void Update(T entity)
        {
            _dbContext.Set<T>().Update(entity);
        }


    }
}
