using Domain.Interfaces;
using Domain.Models.Enteties;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace infrastructure.Repositories
{
     public class MorceauRepository : GenericRepository<Morceau>, IMorceauRepository
    {
        protected readonly DbContextClass _dbContext;
        public MorceauRepository(DbContextClass dbContext) : base(dbContext)
        {
            this._dbContext = dbContext;
    }   
        public async Task<ICollection<Morceau>> _GetPlylisteWithMorceauxAsync(int idPaylist)
        {
            try
            {

                var entity = await _dbContext.Set<Morceau>()
             .Where(m => m.MorceauPlaylists.Any(mp => mp.idPlaylist == idPaylist))
            .Include(m => m.MorceauPlaylists)
             .ThenInclude(mp => mp.Playlist)
            .ToListAsync();
                if (entity == null)
                {
                    // Handle the case where the entity is not found, if necessary
                    throw new KeyNotFoundException($"Entity with ID {idPaylist} not found.");
                }
                return entity;
            }
            catch (Exception e)
            {
                return null;
            }

        }
        /*public async Task<(ICollection<Morceau> Items, int TotalPages)> GetMorceauPaged(int pageNumber)
        {
            int pageSize = 20;
            int totalItems = await _dbContext.Set<Morceau>().CountAsync();
            int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            if (pageNumber < 1 || pageNumber > totalPages)
            {
                return (new List<Morceau>(), totalPages); // Retourne une liste vide si la page est invalide
            }

            var items = await _dbContext.Set<T>()
                                        .Skip((pageNumber - 1) * pageSize)
                                        .Take(pageSize)
                                        .ToListAsync();

            return (items, totalPages);
        }*/
    }
}
