using Domain.Constants;
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
     public class PlaylistRepository : GenericRepository<Playlist>, IPlaylistRepository
    {
        protected readonly DbContextClass _dbContext;
        public PlaylistRepository(DbContextClass dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<Playlist> _GetPlylisteWithMorceauxAsync(int id)
        {
            try
            {
                var entity = await _dbContext.Set<Playlist>().Where(p => p.id == id)
            .Include(p => p.MorceauPlaylists)
                .ThenInclude(mp => mp.Morceau)
            .FirstOrDefaultAsync();
                if (entity == null)
                {
                    // Handle the case where the entity is not found, if necessary
                    throw new KeyNotFoundException($"Entity with ID {id} not found.");
                }
                return entity;
            }
            catch (Exception e)
            {
                return null;
            }

        }
        public async Task<Playlist> _checkPlyliste(int idUser ,string name)
        {
            try
            {
                var entity = await _dbContext.Set<Playlist>().Where(p => p.idUser == idUser && p.name== name)
            
            .FirstOrDefaultAsync();
                if (entity == null)
                {
                    // Handle the case where the entity is not found, if necessary
                    throw new KeyNotFoundException($"Entity with ID {idUser} not found.");
                }
                return entity;
            }
            catch (Exception e)
            {
                return null;
            }

        }
        public async Task<ICollection<Playlist>> GetAllwithMorceau()
        {
            return await _dbContext.Set<Playlist>().Include(p => p.MorceauPlaylists).ToListAsync();
        }
        public async Task<ICollection<Playlist>> GetPlaylistType(int idUser,string type)
        {
            return await _dbContext.Set<Playlist>().Where(x=>x.idUser==idUser && !string.IsNullOrEmpty(x.parametre) && x.parametre.Contains("\"type\":\"" + type + "\"")).ToListAsync();
        }
    }
}
