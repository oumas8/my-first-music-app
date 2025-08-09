using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StackExchange.Redis;


namespace infrastructure.Services
{
    public class RedisService
    {
        private readonly IConnectionMultiplexer _connectionMultiplexer;

        public RedisService(IConnectionMultiplexer connectionMultiplexer)
        {
            _connectionMultiplexer = connectionMultiplexer;
        }

        /// <summary>
        /// Enregistre une valeur dans Redis avec une expiration.
        /// </summary>
        /// <param name="key">Clé Redis</param>
        /// <param name="value">Valeur à enregistrer</param>
        /// <param name="expiration">Durée de vie de la clé</param>
        public async Task SetValueAsync(string key, string value, TimeSpan? expiration = null)
        {
            var db = _connectionMultiplexer.GetDatabase();
            await db.StringSetAsync(key, value, expiration);
        }

        /// <summary>
        /// Récupère une valeur depuis Redis.
        /// </summary>
        /// <param name="key">Clé Redis</param>
        /// <returns>Valeur associée à la clé</returns>
        public async Task<string?> GetValueAsync(string key)
        {
            var db = _connectionMultiplexer.GetDatabase();
            return await db.StringGetAsync(key);
        }

        /// <summary>
        /// Supprime une clé dans Redis.
        /// </summary>
        /// <param name="key">Clé Redis</param>
        public async Task DeleteKeyAsync(string key)
        {
            var db = _connectionMultiplexer.GetDatabase();
            await db.KeyDeleteAsync(key);
        }
    }

}

