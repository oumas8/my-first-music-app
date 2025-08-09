using Domain.Models.Enteties;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Tasks.contract
{
    public interface IMorceauTask
    {
        Task<long> CreateMorceau(Morceau _morceau);

        Task<ICollection<Morceau>> GetAllMorceaus();
        Task<ICollection<Morceau>> GetPlylisteWithMorceauxAsync(int id);

        Task<Morceau> GetMorceauById(int _morceauId);

      Task<bool> UpdateMorceau(Morceau _morceau);

        Task<bool> DeleteMorceau(int _morceauId);
    }
}
