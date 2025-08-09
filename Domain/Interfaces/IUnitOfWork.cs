using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IMorceauRepository MorceauRepo { get; }
        IMorceauPlaylistRepository MorceauPlaylistRepo { get; }
        IPlaylistRepository PlaylistRepo { get; }
        IApiCloudRepository ApiCloudRepo { get; }
        IUserRepository UserRepo { get; }
        ISettingRepository SettingRepo { get; }
        IkeyvalidationRepository keyvalidationRepo { get; }

        int Save();
    }

}
