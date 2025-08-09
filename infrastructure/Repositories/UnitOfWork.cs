using System;
using System.Collections.Generic;
using System.Text;
using Domain.Interfaces;
using Microsoft.Extensions.Caching.Distributed;

namespace infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DbContextClass _dbContext;
        public IMorceauRepository MorceauRepo { get; }
        public IUserRepository UserRepo { get; }
        public ISettingRepository SettingRepo { get; }
        public IMorceauPlaylistRepository MorceauPlaylistRepo { get; }
        public IPlaylistRepository PlaylistRepo { get; }
        public IApiCloudRepository ApiCloudRepo { get; }
        public IkeyvalidationRepository keyvalidationRepo { get; }
        public IDistributedCache DistributedCache { get; }

        public UnitOfWork(DbContextClass dbContext,
                            IMorceauRepository mrceauRepository,
                            IUserRepository userRepository,
                            ISettingRepository settingRepository,
                            IMorceauPlaylistRepository morceauPlaylistRepository,
                            IPlaylistRepository playlistRepository,
                            IkeyvalidationRepository keyvalidationRepository,
                            IApiCloudRepository apiCloudRepository,
                            IDistributedCache distributedCache)
        {
            _dbContext = dbContext;
            MorceauRepo = mrceauRepository;
            UserRepo = userRepository;
            SettingRepo = settingRepository;
            MorceauPlaylistRepo = morceauPlaylistRepository;
            PlaylistRepo = playlistRepository;
            ApiCloudRepo = apiCloudRepository;
            keyvalidationRepo = keyvalidationRepository;
            DistributedCache = distributedCache;
        }

        public int Save()
        {
            return _dbContext.SaveChanges();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _dbContext.Dispose();
            }
        }

    }
}
