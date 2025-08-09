using Domain.Constants;
using infrastructure.Helpers;
using infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Http;
using Tasks;

namespace AuthService.Tools
{
    public class Utilitaire
    {
        private static readonly RedisService _redisService;
        static Utilitaire()
        {
            _redisService = IoCContainer.Resolve<RedisService>();
        }
        public static IActionResult constructResponse1( Object _object)
        {
            return new OkObjectResult(_object);
        }
        public async static Task<int> getHistoriqueId1(int idUser)
        {
            int idPlylist;
            var stridPlylist = await _redisService.GetValueAsync("Historique+" + idUser);
            try
            {
                if (!string.IsNullOrEmpty(stridPlylist))
                {
                    var result = int.TryParse(stridPlylist, out idPlylist);
                }
                else
                {
                    PlaylistTask _playlistTask = IoCContainer.Resolve<PlaylistTask>();
                    idPlylist = (int)(await _playlistTask.CheckPlylistAsync(idUser, PlaylistConst.History)).id;
                    await _redisService.SetValueAsync("Historique+" + idUser, idPlylist.ToString(), TimeSpan.FromDays(30));
                }
            }
            catch (Exception ex)
            {
                throw;
            }

            return idPlylist;
        }
    }
}
