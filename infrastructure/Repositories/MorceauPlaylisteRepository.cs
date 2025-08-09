using Domain.Interfaces;
using Domain.Models.Enteties;
using Microsoft.Data.SqlClient;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace infrastructure.Repositories
{
     public class MorceauPlaylistRepository : GenericRepository<MorceauPlaylist>, IMorceauPlaylistRepository
    {
        public MorceauPlaylistRepository(DbContextClass dbContext) : base(dbContext)
        {

        }
        public async Task<ICollection<Morceau>> _GetListeMorceaubyIdPlylistAsync2(string input,int idPlaylist,int pageNumber,int pageSize)
        {
            try
            {
                int offset = (pageNumber - 1) * pageSize;
                var conditioninput = (string.IsNullOrEmpty(input)) ?"": " AND LOWER(m.title) LIKE '%' || LOWER({input}) || '%' ";
                var morceauxList = await _dbContext.Morceau.FromSqlInterpolated($@"
        SELECT m.*
        FROM MorceauPlaylist mp
        INNER JOIN Morceau m ON mp.idMorceau = m.id
        WHERE mp.idPlaylist = {idPlaylist} {conditioninput}
        ORDER BY mp.dt_listning DESC
        LIMIT {pageSize} OFFSET {offset};
    ")
     .ToListAsync();

                return morceauxList ?? new List<Morceau>();

            }
            catch (Exception e)
            {
                throw;
            }

        }
        public async Task<ICollection<Morceau>> _GetListeMorceaubyIdPlylistAsync(string input, int idPlaylist, int pageNumber, int pageSize)
        {
            try
            {
                int offset = (pageNumber - 1) * pageSize;
                string baseQuery = @"
            SELECT m.*
            FROM MorceauPlaylist mp
            INNER JOIN Morceau m ON mp.idMorceau = m.id
            WHERE mp.idPlaylist =@idPlaylist {{{filterClause}}}
            ORDER BY mp.dt_listning DESC
            LIMIT @pageSize OFFSET @offset;
        ";

                string filterClause = "";
                if (!string.IsNullOrWhiteSpace(input))
                {
                    filterClause = " AND LOWER(m.title) LIKE @input";
                }

                baseQuery = baseQuery.Replace("{{{filterClause}}}", filterClause);
                var fullQuery = string.Format(baseQuery);

                var morceauxList = await _dbContext.Morceau
                    .FromSqlRaw(baseQuery, new SqliteParameter("@idPlaylist", idPlaylist),
            new SqliteParameter("@title", $"%{input?.ToLower()}%"),
            new SqliteParameter("@offset", offset),
            new SqliteParameter("@pageSize", pageSize))
                    .ToListAsync();

                return morceauxList ?? new List<Morceau>();
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<ICollection<Morceau>> _GetListeMorceaubyIdPlylistAsync1(string input,int idPlaylist,int pageNumber,int pageSize)
        {
            try
            {
                IQueryable<MorceauPlaylist> baseQuery = _dbContext.Set<MorceauPlaylist>()
            .Where(pm => pm.idPlaylist == idPlaylist && pm.Morceau != null)
            .Include(pm => pm.Morceau);

                if (!string.IsNullOrEmpty(input))
                {
                    baseQuery = baseQuery.Where(pm => pm.Morceau.title.ToLower().Contains(input.ToLower()));
                }

                int totalItems = await baseQuery.CountAsync();
                int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
                if (pageNumber > totalPages)
                {
                    return new List<Morceau>();
                }

                if (pageNumber != 0 && pageSize != 0)
                {
                    baseQuery = baseQuery.Skip((pageNumber - 1) * pageSize).Take(pageSize);
                }

                var morceauxList = await baseQuery
                    .OrderByDescending(pm => pm.dt_listning)
                    .Select(pm => pm.Morceau)
                    .ToListAsync();

                return morceauxList ?? new List<Morceau>();

            }
            catch (Exception e)
            {
                throw;
            }

        }
        public async Task<List<MorceauPlaylist>> GetByIdMorceau(int idMorceau)
        {
            return await _dbContext.MorceauPlaylist.Where(m => m.idMorceau == idMorceau).ToListAsync();
        }
    }
}
